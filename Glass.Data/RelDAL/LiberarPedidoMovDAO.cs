using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;

namespace Glass.Data.RelDAL
{
    public sealed class LiberarPedidoMovDAO : Glass.Pool.Singleton<LiberarPedidoMovDAO>
    {
        private LiberarPedidoMovDAO() { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="situacao"></param>
        /// <param name="buscarCredito">0-Não buscar, 1-Incluir crédito gerado na listagem, 2-Buscar apenas crédito gerado</param>
        private LiberarPedidoMov[] BaseGet(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao,
            int buscarCredito, bool buscarPagtoAntecip, bool buscarSinal, string sortExpression, int startRow, int pageSize)
        {
            LiberarPedido[] liberacoes = null;
            TrocaDevolucao[] lstTroca = null;
            Obra[] lstObra = null;
            List<Sinal> lstPagtoAntecip = null;
            List<LiberarPedidoMov> retorno = new List<LiberarPedidoMov>();

            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "IdLiberarPedido desc";

            if (!buscarPagtoAntecip && !buscarSinal)
            {
                if (buscarCredito != 2)
                {
                    liberacoes = LiberarPedidoDAO.Instance.GetForRpt(0, 0, null, idFunc, idCli, nomeCli, 0, dataIni, dataFim, situacao, 0, null, null);
                    lstTroca = ((List<TrocaDevolucao>)TrocaDevolucaoDAO.Instance.GetForRpt(0, 0, 0, situacao == 1 ? 2 : situacao == 2 ? 3 : 0,
                        idCli, nomeCli, idFunc.ToString(), null, dataIni, dataFim, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0)).ToArray();
                }

                if (buscarCredito == 1 || buscarCredito == 2)
                    lstObra = ObraDAO.Instance.GetListRpt(idCli, nomeCli, 0, idFunc, 0, (int)Obra.SituacaoObra.Finalizada, dataIni, dataFim, null, null, true, null, 0, 0, null);
            }

            else if (buscarPagtoAntecip)
            {
                // Busca os pagamentos antecipados
                lstPagtoAntecip = SinalDAO.Instance.GetForMovLib(idCli, idFunc, dataIni, dataFim, (int)Sinal.SituacaoEnum.Aberto, true);
            }

            else if (buscarSinal)
            {
                // Busca os sinais
                lstPagtoAntecip = (SinalDAO.Instance.GetForMovLib(idCli, idFunc, dataIni, dataFim, (int)Sinal.SituacaoEnum.Aberto, false));
            }

            if (liberacoes != null)
                foreach (LiberarPedido lp in liberacoes)
                {
                    LiberarPedidoMov novo = new LiberarPedidoMov();
                    novo.IdLiberarPedido = lp.IdLiberarPedido;
                    novo.NomeCliente = lp.IdCliente + " - " + lp.NomeCliente;
                    novo.Total = lp.Total;
                    novo.Debito = lp.CreditoUtilizado;
                    novo.Credito = lp.CreditoGerado;
                    novo.Criterio = lp.Criterio;
                    novo.Situacao = lp.DescrSituacao;
                    novo.Desconto = lp.TipoDesconto == 1 ? lp.Total * (lp.Desconto / 100) : lp.Desconto;
                    novo.Acrescimo = lp.TipoAcrescimo == 1 ? lp.Total * (lp.Acrescimo / 100) : lp.Acrescimo;
                    novo.IdsPedidos = LiberarPedidoDAO.Instance.IdsPedidos(null, novo.IdLiberarPedido.ToString());
                    novo.CanceladoSistema = lp.CanceladoSistema;

                    #region Busca as formas de pagamento das liberações

                    foreach (PagtoLiberarPedido plp in PagtoLiberarPedidoDAO.Instance.GetByLiberacao(lp.IdLiberarPedido))
                    {
                        switch ((Glass.Data.Model.Pagto.FormaPagto)plp.IdFormaPagto)
                        {
                            case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                                novo.Boleto += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                                novo.Cartao += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                            case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                                novo.Outros += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                            case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                                novo.Cheque += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                                novo.Deposito += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                                novo.Dinheiro += plp.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Prazo:
                                novo.Prazo += plp.ValorPagto;
                                break;
                        }
                    }

                    #endregion

                    retorno.Add(novo);
                }

            if (lstTroca != null)
                foreach (TrocaDevolucao troca in lstTroca)
                {
                    if (troca.ValorExcedente <= 0)
                        continue;

                    LiberarPedidoMov novo = new LiberarPedidoMov();
                    novo.NomeCliente = troca.IdCliente + " - " + troca.NomeCliente;
                    novo.Total = troca.ValorExcedente;
                    novo.Debito = troca.CreditoUtilizadoFinalizar > 0 ? troca.CreditoUtilizadoFinalizar.Value : 0;
                    novo.Credito = troca.CreditoGerado;
                    novo.Criterio = troca.Criterio;
                    novo.Situacao = troca.DescrSituacao;
                    novo.IdsPedidos = "Troca " + troca.IdTrocaDevolucao;

                    #region Busca as formas de pagamento da troca

                    var lstPagtoTroca = PagtoTrocaDevolucaoDAO.Instance.GetByTrocaDevolucao(troca.IdTrocaDevolucao);

                    if (lstPagtoTroca.Count == 0)
                        novo.Prazo += novo.Total;

                    foreach (PagtoTrocaDevolucao ptd in lstPagtoTroca)
                    {
                        switch ((Glass.Data.Model.Pagto.FormaPagto)ptd.IdFormaPagto)
                        {
                            case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                                novo.Boleto += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                                novo.Cartao += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                            case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                                novo.Outros += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                            case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                                novo.Cheque += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                                novo.Deposito += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                                novo.Dinheiro += ptd.ValorPago;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Prazo:
                                novo.Prazo += ptd.ValorPago;
                                break;
                        }
                    }

                    #endregion

                    retorno.Add(novo);
                }

            if (lstObra != null)
                foreach (Obra obra in lstObra)
                {
                    LiberarPedidoMov novo = new LiberarPedidoMov();
                    novo.NomeCliente = obra.IdCliente + " - " + obra.NomeCliente;
                    novo.Total = obra.ValorObra;
                    novo.Criterio = liberacoes != null && liberacoes.Length > 0 ? liberacoes[0].Criterio : String.Empty;
                    novo.Situacao = "Finalizado";
                    novo.IdsPedidos = "Crédito Gerado";

                    #region Busca as formas de pagamento do crédito gerado

                    foreach (PagtoObra po in PagtoObraDAO.Instance.GetByObra(obra.IdObra))
                    {
                        switch ((Glass.Data.Model.Pagto.FormaPagto)po.IdFormaPagto)
                        {
                            case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                                novo.Boleto += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                                novo.Cartao += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                            case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                                novo.Outros += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                            case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                                novo.Cheque += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                                novo.Deposito += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                                novo.Dinheiro += po.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Prazo:
                                novo.Prazo += po.ValorPagto;
                                break;
                        }
                    }

                    #endregion

                    retorno.Add(novo);
                }

            if (lstPagtoAntecip != null)
                foreach (Sinal pagtoAntecip in lstPagtoAntecip)
                {
                    LiberarPedidoMov novo = new LiberarPedidoMov();
                    novo.NomeCliente = pagtoAntecip.NomeCliente;
                    novo.Total = (decimal)pagtoAntecip.TotalSinal;
                    novo.Criterio = liberacoes != null && liberacoes.Length > 0 ? liberacoes[0].Criterio : String.Empty;
                    novo.Situacao = "Finalizado";
                    novo.IdsPedidos = SinalDAO.Instance.ObtemIdsPedidos(pagtoAntecip.IdSinal);
                    novo.Credito = pagtoAntecip.CreditoGeradoCriar > 0 ? pagtoAntecip.CreditoGeradoCriar.Value : 0;
                    novo.Debito = pagtoAntecip.CreditoUtilizadoCriar > 0 ? pagtoAntecip.CreditoUtilizadoCriar.Value : 0;

                    #region Busca as formas de pagamento do pagto antecipado

                    foreach (PagtoSinal pagto in PagtoSinalDAO.Instance.GetBySinal(pagtoAntecip.IdSinal))
                        switch ((Glass.Data.Model.Pagto.FormaPagto)pagto.IdFormaPagto)
                        {
                            case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                                novo.Boleto += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                                novo.Cartao += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                            case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                                novo.Outros += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                            case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                                novo.Cheque += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                                novo.Deposito += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                                novo.Dinheiro += pagto.ValorPagto;
                                break;

                            case Glass.Data.Model.Pagto.FormaPagto.Prazo:
                                novo.Prazo += pagto.ValorPagto;
                                break;
                        }

                    #endregion

                    retorno.Add(novo);
                }

            string[] dadosSort = sortExpression.Split(' ');
            string sort = dadosSort[0];
            string direcaoSort = dadosSort.Length > 1 ? dadosSort[1] : "asc";

            retorno.Sort(new Comparison<LiberarPedidoMov>(delegate (LiberarPedidoMov x, LiberarPedidoMov y)
            {
                switch (sort)
                {
                    case "NomeCliente": return Comparer<string>.Default.Compare(x.NomeCliente, y.NomeCliente);
                    case "Situacao": return Comparer<string>.Default.Compare(x.Situacao, y.Situacao);
                    case "Total": return Comparer<decimal>.Default.Compare(x.Total, y.Total);
                    case "Desconto": return Comparer<decimal>.Default.Compare(x.Desconto, y.Desconto);
                    case "Dinheiro": return Comparer<decimal>.Default.Compare(x.Dinheiro, y.Dinheiro);
                    case "Cheque": return Comparer<decimal>.Default.Compare(x.Cheque, y.Cheque);
                    case "Prazo": return Comparer<decimal>.Default.Compare(x.Prazo, y.Prazo);
                    case "Boleto": return Comparer<decimal>.Default.Compare(x.Boleto, y.Boleto);
                    case "Deposito": return Comparer<decimal>.Default.Compare(x.Deposito, y.Deposito);
                    case "Cartao": return Comparer<decimal>.Default.Compare(x.Cartao, y.Cartao);
                    case "Outros": return Comparer<decimal>.Default.Compare(x.Outros, y.Outros);
                    case "Debito": return Comparer<decimal>.Default.Compare(x.Debito, y.Debito);
                    case "Credito": return Comparer<decimal>.Default.Compare(x.Credito, y.Credito);
                    default: return Comparer<uint>.Default.Compare(x.IdLiberarPedido, y.IdLiberarPedido);
                }

            }));

            if (direcaoSort.ToLower() == "desc")
                retorno.Reverse();

            if (startRow > 0)
                retorno.RemoveRange(0, startRow);

            return retorno.ToArray();
        }

        public LiberarPedidoMov[] GetCreditoGeradoRpt(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao)
        {
            return BaseGet(idCli, nomeCli, idFunc, dataIni, dataFim, situacao, 2, false, false, null, 0, 0);
        }

        public LiberarPedidoMov[] GetPagtoAntecipadoRpt(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao)
        {
            return BaseGet(idCli, nomeCli, idFunc, dataIni, dataFim, situacao, 2, true, false, null, 0, 0);
        }

        public LiberarPedidoMov[] GetSinalRpt(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao)
        {
            return BaseGet(idCli, nomeCli, idFunc, dataIni, dataFim, situacao, 2, false, true, null, 0, 0);
        }

        public LiberarPedidoMov[] GetForRpt(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao)
        {
            return BaseGet(idCli, nomeCli, idFunc, dataIni, dataFim, situacao, 0, false, false, null, 0, 0);
        }

        public LiberarPedidoMov[] GetList(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao, string sortExpression, int startRow, int pageSize)
        {
            return BaseGet(idCli, nomeCli, idFunc, dataIni, dataFim, situacao, 0, false, false, sortExpression, startRow, pageSize);
        }

        public int GetListCount(uint idCli, string nomeCli, uint idFunc, string dataIni, string dataFim, int situacao)
        {
            return LiberarPedidoDAO.Instance.GetCount(0, 0, null, idFunc, idCli, nomeCli, 0, dataIni, dataFim, situacao, 0, null, null);
        }
    }
}
