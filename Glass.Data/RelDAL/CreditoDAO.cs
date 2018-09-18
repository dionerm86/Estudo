using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class CreditoDAO : Glass.Pool.Singleton<CreditoDAO>
    {
        private CreditoDAO() { }

        #region Struct Totais

        public struct TotaisCredito
        {
            private decimal _gerado;

            public decimal Gerado
            {
                get { return _gerado; }
                set { _gerado = value; }
            }

            private decimal _utilizado;

            public decimal Utilizado
            {
                get { return _utilizado; }
                set { _utilizado = value; }
            }
        }

        #endregion

        #region Propriedades

        private static List<uint> pcEstorno = UtilsPlanoConta.GetLstCredito(1);
        private static List<uint> pcGerado = UtilsPlanoConta.GetLstCredito(2);
        private static List<uint> pcUtilizado = UtilsPlanoConta.GetLstCredito(3);

        private bool IsEstornoGerado(uint planoConta)
        {
            var estornoCreditoFornec = new List<uint>(Array.ConvertAll<string, uint>(UtilsPlanoConta.ContasEstornoCreditoFornec().Split(','),
                x => Glass.Conversoes.StrParaUint(x)));

            return planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoCompraGerado) ||
                 planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoVendaGerado) ||
                 planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoEntradaGerado) ||
                 planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoRecPrazoGerado) ||
                 planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraEstorno) ||
                 planoConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntencipFornecEstorno) ||
                 estornoCreditoFornec.Contains(planoConta);
        }

        #endregion

        #region Crédito cliente

        private Credito[] GetCreditoList(uint idCliente, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            var input = new List<Credito>();

            var planosConta = UtilsPlanoConta.GetLstCredito(0).ToArray();
            var caixaDiario = Data.DAL.CaixaDiarioDAO.Instance.GetByCliente(idCliente, inicio, fim, planosConta);
            var caixaGeral = CaixaGeralDAO.Instance.GetByCliente(idCliente, inicio, fim, planosConta);

            foreach (Data.Model.CaixaDiario cd in caixaDiario)
            {
                Credito novo = new Credito();
                novo.IdCaixaDiario = cd.IdCaixaDiario;
                novo.IdConta = cd.IdConta;
                novo.Valor = cd.Valor;
                novo.Data = cd.DataCad;

                novo.IdAcerto = cd.IdAcerto;
                novo.IdPedido = cd.IdPedido;
                novo.IdContaR = cd.IdContaR;
                novo.IdDevolucaoPagto = cd.IdDevolucaoPagto;
                novo.IdLiberarPedido = cd.IdLiberarPedido;
                novo.IdObra = cd.IdObra;
                novo.IdTrocaDevolucao = cd.IdTrocaDevolucao;
                novo.IdSinal = cd.IdSinal;
                novo.IdCheque = cd.IdCheque;
                novo.IdAcertoCheque = cd.IdAcertoCheque;
                novo.TipoMov = GetTipoMov(cd.IdConta);

                input.Add(novo);
            }

            foreach (CaixaGeral cg in caixaGeral)
            {
                Credito novo = new Credito();
                novo.IdCaixaGeral = cg.IdCaixaGeral;
                novo.IdConta = cg.IdConta;
                novo.Valor = cg.ValorMov;
                novo.Data = cg.DataMovExibir;

                novo.IdAcerto = cg.IdAcerto;
                novo.IdPedido = cg.IdPedido;
                novo.IdContaR = cg.IdContaR;
                novo.IdLiberarPedido = cg.IdLiberarPedido;
                novo.IdObra = cg.IdObra;
                novo.IdTrocaDevolucao = cg.IdTrocaDevolucao;
                novo.TipoMov = GetTipoMov(cg.IdConta);
                novo.IdSinal = cg.IdSinal;
                novo.IdCheque = cg.IdCheque;
                novo.IdAcertoCheque = cg.IdAcertoCheque;
                novo.IdCompra = cg.IdCompra;
                novo.IdContaPg = cg.IdContaPg;
                novo.IdDeposito = cg.IdDeposito;
                novo.IdDevolucaoPagto = cg.IdDevolucaoPagto;
                novo.IdPagto = cg.IdPagto;

                novo.TipoMov = GetTipoMov(cg.IdConta);

                input.Add(novo);
            }

            var output = new List<Credito>();

            // Caso o tipo de movimentação seja nulo então todos os tipos de movimentação devem ser considerados.
            tipoMovimentacao = String.IsNullOrEmpty(tipoMovimentacao) ? "1,2,3,4" : tipoMovimentacao;

            var tipos = tipoMovimentacao.Split(',');

            for (int i = 0; i < tipos.Length; i++)
            {
                foreach (Credito item in input)
                {
                    if (item.TipoMov == Convert.ToInt32(tipos[i]))
                        output.Add(item);
                }
            }

            return output.ToArray();
        }

        public Credito[] GetCredito(uint idCliente, DateTime inicio, DateTime fim, string tipoMovimentacao, string sortExpression, int startRow, int pageSize)
        {
            var dataFimFiltro = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 23:59"));
            dataFimFiltro = fim < dataFimFiltro ? dataFimFiltro : fim;

            var retorno = new List<Credito>(GetCreditoList(idCliente, inicio, dataFimFiltro, tipoMovimentacao));

            if (retorno.Count > 0)
            {
                AtualizaSaldo(ClienteDAO.Instance.GetCredito(idCliente), ref retorno);

                // Retira as movimentações que não condizem com o filtro e foram utilizadas somente para calcular o saldo corretamente
                fim = DateTime.Parse(fim.ToString("dd/MM/yyyy 23:59"));
                if (dataFimFiltro > fim)
                    for (int i = 0; i < retorno.Count; i++)
                        if (retorno[i].Data > fim)
                        {
                            retorno.Remove(retorno[i]);
                            i--;
                        }

                sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "Data DESC";

                retorno.Sort(new Comparison<Credito>(delegate (Credito x, Credito y)
                {
                    int retornoSort = 0;
                    string sort = sortExpression.IndexOf(" ") == -1 ? sortExpression : sortExpression.Substring(0, sortExpression.IndexOf(" "));

                    switch (sort)
                    {
                        case "IdPedido":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdPedido, y.IdPedido);
                            break;

                        case "IdLiberarPedido":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdLiberarPedido, y.IdLiberarPedido);
                            break;

                        case "IdAcerto":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdAcerto, y.IdAcerto);
                            break;

                        case "IdContaR":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdContaR, y.IdContaR);
                            break;

                        case "IdObra":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdObra, y.IdObra);
                            break;

                        case "DescrPlanoConta":
                            retornoSort = x.DescrPlanoConta.CompareTo(y.DescrPlanoConta);
                            break;

                        case "Valor":
                            retornoSort = Comparer<decimal>.Default.Compare(x.Valor, y.Valor);
                            break;

                        case "Data":
                            retornoSort = Comparer<DateTime>.Default.Compare(x.Data, y.Data);
                            break;
                    }

                    if (retornoSort == 0)
                        retornoSort = x.IdCaixaDiario > 0 ? y.IdCaixaDiario > 0 ? x.IdCaixaDiario.CompareTo(y.IdCaixaDiario) : -1 :
                            y.IdCaixaGeral > 0 ? x.IdCaixaGeral.CompareTo(y.IdCaixaGeral) : 1;

                    if (sortExpression.IndexOf(" ") > -1)
                        retornoSort = -retornoSort;

                    return retornoSort;

                }));

                // Faz paginação
                if (startRow > 0)
                    retorno.RemoveRange(0, startRow);

                if (pageSize > 0 && (retorno.Count - pageSize > 0))
                    retorno.RemoveRange(pageSize, retorno.Count - pageSize);
            }

            return retorno.ToArray();
        }

        public int GetCreditoCount(uint idCliente, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            var input = new List<Credito>();

            var planosConta = UtilsPlanoConta.GetLstCredito(0).ToArray();
            var caixaDiario = Data.DAL.CaixaDiarioDAO.Instance.GetByCliente(idCliente, inicio, fim, planosConta);
            var caixaGeral = CaixaGeralDAO.Instance.GetByCliente(idCliente, inicio, fim, planosConta);

            foreach (Data.Model.CaixaDiario cd in caixaDiario)
            {
                var novo = new Credito();
                novo.IdCaixaDiario = cd.IdCaixaDiario;
                novo.IdConta = cd.IdConta;
                novo.Valor = cd.Valor;
                novo.Data = cd.DataCad;

                novo.IdAcerto = cd.IdAcerto;
                novo.IdPedido = cd.IdPedido;
                novo.IdContaR = cd.IdContaR;
                novo.IdLiberarPedido = cd.IdLiberarPedido;
                novo.IdObra = cd.IdObra;
                novo.IdTrocaDevolucao = cd.IdTrocaDevolucao;
                novo.IdSinal = cd.IdSinal;
                novo.IdCheque = cd.IdCheque;
                novo.TipoMov = GetTipoMov(cd.IdConta);

                input.Add(novo);
            }

            foreach (CaixaGeral cg in caixaGeral)
            {
                var novo = new Credito();
                novo.IdCaixaGeral = cg.IdCaixaGeral;
                novo.IdConta = cg.IdConta;
                novo.Valor = cg.ValorMov;
                novo.Data = cg.DataMovExibir;

                novo.IdAcerto = cg.IdAcerto;
                novo.IdPedido = cg.IdPedido;
                novo.IdContaR = cg.IdContaR;
                novo.IdLiberarPedido = cg.IdLiberarPedido;
                novo.IdObra = cg.IdObra;
                novo.IdTrocaDevolucao = cg.IdTrocaDevolucao;
                novo.IdSinal = cg.IdSinal;
                novo.IdCheque = cg.IdCheque;
                novo.IdAcertoCheque = cg.IdAcertoCheque;
                novo.IdCompra = cg.IdCompra;
                novo.IdContaPg = cg.IdContaPg;
                novo.IdDeposito = cg.IdDeposito;
                novo.IdDevolucaoPagto = cg.IdDevolucaoPagto;
                novo.IdPagto = cg.IdPagto;

                novo.TipoMov = GetTipoMov(cg.IdConta);

                input.Add(novo);
            }

            var output = new List<Credito>();

            // Caso o tipo de movimentação seja nulo então todos os tipos de movimentação devem ser considerados.
            tipoMovimentacao = String.IsNullOrEmpty(tipoMovimentacao) ? "1,2,3,4" : tipoMovimentacao;

            var tipos = tipoMovimentacao.Split(',');

            for (int i = 0; i < tipos.Length; i++)
            {
                foreach (Credito item in input)
                {
                    if (item.TipoMov == Convert.ToInt32(tipos[i]))
                        output.Add(item);
                }
            }

            return output.ToArray().Length;
        }

        public TotaisCredito GetTotaisCredito(uint idCliente, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            return GetTotaisCredito(GetCreditoList(idCliente, inicio, fim, tipoMovimentacao));
        }

        #endregion

        #region Crédito fornecedor

        private Credito[] GetCreditoListFornecedor(uint idFornec, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            List<Credito> input = new List<Credito>();

            uint[] planosConta = UtilsPlanoConta.GetLstCredito(0).ToArray();
            CaixaGeral[] caixaGeral = CaixaGeralDAO.Instance.GetByFornecedor(idFornec, inicio, fim, planosConta);

            foreach (CaixaGeral cg in caixaGeral)
            {
                Credito novo = new Credito();
                novo.IdCaixaGeral = cg.IdCaixaGeral;
                novo.IdConta = cg.IdConta;
                novo.Valor = cg.ValorMov;
                novo.Data = cg.DataMovExibir;

                novo.IdAcerto = cg.IdAcerto;
                novo.IdPedido = cg.IdPedido;
                novo.IdContaR = cg.IdContaR;
                novo.IdLiberarPedido = cg.IdLiberarPedido;
                novo.IdObra = cg.IdObra;
                novo.IdAntecipFornec = cg.IdAntecipFornec;
                novo.IdTrocaDevolucao = cg.IdTrocaDevolucao;
                novo.TipoMov = GetTipoMov(cg.IdConta);
                novo.IdSinal = cg.IdSinal;
                novo.IdCheque = cg.IdCheque;
                novo.IdAcertoCheque = cg.IdAcertoCheque;
                novo.IdCompra = cg.IdCompra;
                novo.IdContaPg = cg.IdContaPg;
                novo.IdDeposito = cg.IdDeposito;
                novo.IdDevolucaoPagto = cg.IdDevolucaoPagto;
                novo.IdPagto = cg.IdPagto;
                novo.IdCreditoFornecedor = cg.IdCreditoFornecedor;

                input.Add(novo);
            }

            var output = new List<Credito>();

            // Caso o tipo de movimentação seja nulo então todos os tipos de movimentação devem ser considerados.
            tipoMovimentacao = String.IsNullOrEmpty(tipoMovimentacao) ? "1,2,3,4" : tipoMovimentacao;

            var tipos = tipoMovimentacao.Split(',');

            for (int i = 0; i < tipos.Length; i++)
            {
                foreach (Credito item in input)
                {
                    if (item.TipoMov == Convert.ToInt32(tipos[i]))
                        output.Add(item);
                }
            }

            return output.ToArray();
        }

        public Credito[] GetCreditoFornecedor(uint idFornec, DateTime inicio, DateTime fim, string tipoMovimentacao, string sortExpression,
            int startRow, int pageSize)
        {
            var dataFimFiltro = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 23:59"));
            dataFimFiltro = fim < dataFimFiltro ? dataFimFiltro : fim;

            var retorno = new List<Credito>(GetCreditoListFornecedor(idFornec, inicio, dataFimFiltro, tipoMovimentacao));

            if (retorno.Count > 0)
            {
                AtualizaSaldo(FornecedorDAO.Instance.GetCredito(idFornec), ref retorno);

                // Retira as movimentações que não condizem com o filtro e foram utilizadas somente para calcular o saldo corretamente
                fim = DateTime.Parse(fim.ToString("dd/MM/yyyy 23:59"));
                if (dataFimFiltro > fim)
                    for (int i = 0; i < retorno.Count; i++)
                        if (retorno[i].Data > fim)
                        {
                            retorno.Remove(retorno[i]);
                            i--;
                        }

                sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "Data DESC";

                retorno.Sort(new Comparison<Credito>(delegate (Credito x, Credito y)
                {
                    int retornoSort = 0;
                    string sort = sortExpression.IndexOf(" ") == -1 ? sortExpression : sortExpression.Substring(0,
                        sortExpression.IndexOf(" "));

                    switch (sort)
                    {
                        case "IdPedido":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdPedido, y.IdPedido);
                            break;

                        case "IdLiberarPedido":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdLiberarPedido, y.IdLiberarPedido);
                            break;

                        case "IdAcerto":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdAcerto, y.IdAcerto);
                            break;

                        case "IdContaR":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdContaR, y.IdContaR);
                            break;

                        case "IdContaPg":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdContaPg, y.IdContaPg);
                            break;

                        case "IdObra":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdObra, y.IdObra);
                            break;

                        case "DescrPlanoConta":
                            retornoSort = x.DescrPlanoConta.CompareTo(y.DescrPlanoConta);
                            break;

                        case "Valor":
                            retornoSort = Comparer<decimal>.Default.Compare(x.Valor, y.Valor);
                            break;

                        case "Data":
                            retornoSort = Comparer<DateTime>.Default.Compare(x.Data, y.Data);
                            break;

                        case "IdCreditoFornecedor":
                            retornoSort = Comparer<uint?>.Default.Compare(x.IdCreditoFornecedor, y.IdCreditoFornecedor);
                            break;
                    }

                    if (retornoSort == 0)
                        retornoSort = y.IdCaixaGeral > 0 ? x.IdCaixaGeral.CompareTo(y.IdCaixaGeral) : 1;

                    if (sortExpression.IndexOf(" ") > -1)
                        retornoSort = -retornoSort;

                    return retornoSort;

                }));

                retorno = retorno.Where(f => f.Data >= inicio && f.Data <= fim).ToList();

                // Faz paginação
                if (startRow > 0)
                    retorno.RemoveRange(0, startRow);

                if (pageSize > 0 && (retorno.Count - pageSize > 0))
                    retorno.RemoveRange(pageSize, retorno.Count - pageSize);
            }

            return retorno.ToArray();
        }

        public int GetCreditoFornecedorCount(uint idFornec, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            var input = new List<Credito>();

            var planosConta = UtilsPlanoConta.GetLstCredito(0).ToArray();
            var caixaGeral = CaixaGeralDAO.Instance.GetByFornecedor(idFornec, inicio, fim, planosConta);

            foreach (CaixaGeral cg in caixaGeral)
            {
                Credito novo = new Credito();
                novo.IdCaixaGeral = cg.IdCaixaGeral;
                novo.IdConta = cg.IdConta;
                novo.Valor = cg.ValorMov;
                novo.Data = cg.DataMovExibir;

                novo.IdAcerto = cg.IdAcerto;
                novo.IdPedido = cg.IdPedido;
                novo.IdContaR = cg.IdContaR;
                novo.IdLiberarPedido = cg.IdLiberarPedido;
                novo.IdObra = cg.IdObra;
                novo.IdTrocaDevolucao = cg.IdTrocaDevolucao;
                novo.TipoMov = cg.TipoMov;
                novo.IdSinal = cg.IdSinal;
                novo.IdCheque = cg.IdCheque;
                novo.IdAcertoCheque = cg.IdAcertoCheque;
                novo.IdCompra = cg.IdCompra;
                novo.IdContaPg = cg.IdContaPg;
                novo.IdDeposito = cg.IdDeposito;
                novo.IdDevolucaoPagto = cg.IdDevolucaoPagto;
                novo.IdPagto = cg.IdPagto;
                novo.IdCreditoFornecedor = cg.IdCreditoFornecedor;

                input.Add(novo);
            }

            var output = new List<Credito>();

            // Caso o tipo de movimentação seja nulo então todos os tipos de movimentação devem ser considerados.
            tipoMovimentacao = String.IsNullOrEmpty(tipoMovimentacao) ? "1,2,3,4" : tipoMovimentacao;

            var tipos = tipoMovimentacao.Split(',');

            for (int i = 0; i < tipos.Length; i++)
            {
                foreach (Credito item in input)
                {
                    if (item.TipoMov == Convert.ToInt32(tipos[i]))
                        output.Add(item);
                }
            }

            return output.ToArray().Length;
        }

        public TotaisCredito GetTotaisCreditoFornecedor(uint idFornecedor, DateTime inicio, DateTime fim, string tipoMovimentacao)
        {
            return GetTotaisCredito(GetCreditoListFornecedor(idFornecedor, inicio, fim, tipoMovimentacao));
        }

        #endregion

        #region Atualiza saldo

        /// <summary>
        /// Atualiza saldo, método corrigido para funcionar na atualização de saldo do cliente, se necessário criar outro método para
        /// atualizar o saldo do fornecedor
        /// </summary>
        /// <param name="saldoInicial"></param>
        /// <param name="inverterMov"></param>
        /// <param name="itens"></param>
        private void AtualizaSaldo(decimal saldoInicial, ref List<Credito> itens)
        {
            // Ordena a lista pela data de movimentação (crescente)
            itens.Sort((x, y) => x.Data.CompareTo(y.Data) != 0 ? x.Data.CompareTo(y.Data) :
                x.IdCaixaDiario > 0 ? y.IdCaixaDiario > 0 ? x.IdCaixaDiario.CompareTo(y.IdCaixaDiario) : -1 :
                y.IdCaixaGeral > 0 ? x.IdCaixaGeral.CompareTo(y.IdCaixaGeral) : 1);

            // Calcula o saldo de cada movimentação
            itens[itens.Count - 1].Saldo = saldoInicial;

            for (int i = itens.Count - 1; i >= 0; i--)
            {
                decimal saldoAnterior = i < itens.Count - 1 ? itens[i + 1].Saldo : 0;

                if (i < itens.Count - 1)
                {
                    // Se for somar ao crédito
                    if (itens[i + 1].TipoMov == 3 || itens[i + 1].TipoMov == 4)
                        itens[i].Saldo += saldoAnterior + (itens[i + 1].Valor);

                    // Se for subtrair do crédito
                    else if (itens[i + 1].TipoMov == 1 || itens[i + 1].TipoMov == 2)
                        itens[i].Saldo += saldoAnterior - (itens[i + 1].Valor);

                    /* else
                        itens[i].Saldo = itens[i - 1].Saldo - (itens[i - 1].Valor * -1); */
                }
            }
        }

        #endregion

        #region Obtém totais

        public TotaisCredito GetTotaisCredito(IEnumerable<Credito> itens)
        {
            var retorno = new TotaisCredito();

            if (itens == null)
                return retorno;

            foreach (Credito c in itens)
            {
                int tipoMov = GetTipoMov(c.IdConta);

                if (tipoMov == 1)
                    retorno.Gerado += c.Valor;
                else if (tipoMov == 2)
                    retorno.Utilizado -= c.Valor;
                else if (tipoMov == 3)
                    retorno.Utilizado += c.Valor;
                else if (tipoMov == 4)
                    retorno.Gerado -= c.Valor;
            }

            return retorno;
        }

        #endregion

        #region Obtém tipo movimentação

        /// <summary>
        /// Verifica o tipo de movimentação de acordo com o plano de conta
        /// 1-Gerado
        /// 2-Estorno Utilizado
        /// 3-Utilizado
        /// 4-Estorno Gerado
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public int GetTipoMov(uint idConta)
        {
            if (pcEstorno.Contains(idConta))
            {
                if (IsEstornoGerado(idConta))
                    return 4;
                else
                    return 2;
            }
            else if (pcGerado.Contains(idConta))
                return 1;
            else if (pcUtilizado.Contains(idConta))
                return 3;

            return 0;
        }

        #endregion
    }
}
