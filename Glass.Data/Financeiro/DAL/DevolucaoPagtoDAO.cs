using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class DevolucaoPagtoDAO : BaseCadastroDAO<DevolucaoPagto, DevolucaoPagtoDAO>
    {
        //private DevolucaoPagtoDAO() { }

        #region Busca padrão

        private string Sql(uint idDevolucaoPagto, uint idCliente, string nomeCliente, string dataIni, string dataFim, bool selecionar)
        {
            string criterio = "";
            string campos = selecionar ? @"dp.*, c.nome as nomeCliente, group_concat(concat(cast(cg.idConta as char), ';', 
                cast(cg.valorMov as char)) separator '|') as formasPagto, f.Nome as NomeFunc, '$$$' as criterio" : "count(*)";

            string sql = @"
                select " + campos + @"
                from devolucao_pagto dp
                    inner join cliente c on (dp.idCliente=c.id_Cli)
                    left join caixa_geral cg on (dp.idDevolucaoPagto=cg.idDevolucaoPagto)
                    LEFT JOIN funcionario f ON (dp.UsuCad = f.IdFunc)
                where (cg.idConta is null or cg.idConta in (" + UtilsPlanoConta.ContasDevolucaoPagto() + "))";

            if (idDevolucaoPagto > 0)
            {
                sql += " and dp.idDevolucaoPagto=" + idDevolucaoPagto;
                criterio += "Devolução de pagto.: " + idDevolucaoPagto + "    ";
            }

            if (idCliente > 0)
            {
                sql += " and dp.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and dp.dataCad>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and dp.dataCad<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            if (selecionar)
                sql += " group by dp.idDevolucaoPagto";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.ToArray();
        }

        public IList<DevolucaoPagto> GetList(uint idCliente, string nomeCliente, string dataIni, string dataFim, 
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "dp.idDevolucaoPagto Desc" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, idCliente, nomeCliente, dataIni, dataFim, true), sortExpression, startRow, pageSize,
                GetParams(nomeCliente, dataIni, dataFim));
        }

        public int GetCount(uint idCliente, string nomeCliente, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idCliente, nomeCliente, dataIni, dataFim, false),
                GetParams(nomeCliente, dataIni, dataFim));
        }

        public IList<DevolucaoPagto> GetForRpt(uint idCliente, string nomeCliente, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(0, idCliente, nomeCliente, dataIni, dataFim, true),
                GetParams(nomeCliente, dataIni, dataFim)).ToList();
        }

        public DevolucaoPagto GetElement(uint idDevolucaoPagto)
        {
            return objPersistence.LoadOneData(Sql(idDevolucaoPagto, 0, null, null, null, true));
        }

        #endregion

        #region Devolve o pagamento

        /// <summary>
        /// Faz a devolução de um pagamento.
        /// </summary>
        public uint Devolver(uint idCliente, DateTime data, decimal[] valores, uint[] idFormaPagto, uint[] idContaBanco, uint[] depositoNaoIdentificado, uint[] idTipoCartao,
            uint[] numeroParcelas, uint[] idTipoBoleto, decimal[] taxaAntecip, string chequesPagto, string numAutConstrucard, decimal creditoUtilizado,
            string obs, bool caixaDiario)
        {
            Glass.FilaOperacoes.ReceberDevolucaoPagto.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    UtilsFinanceiro.DadosRecebimento retorno = null;

                    decimal totalDevolucao = 0;
                    foreach (decimal v in valores)
                        totalDevolucao += v;

                    // Insere o registro da devolução
                    DevolucaoPagto dev = new DevolucaoPagto();
                    dev.IdCliente = idCliente;
                    dev.Valor = totalDevolucao;
                    dev.Situacao = (int)DevolucaoPagto.SituacaoDevolucao.Aberta;
                    dev.Obs = obs;
                    uint idDevolucaoPagto = Insert(transaction, dev);
                    dev.IdDevolucaoPagto = idDevolucaoPagto;

                    if (idDevolucaoPagto == 0)
                        throw new Exception("Falha ao inserir devolução de pagamento. Registro 0.");

                    #region Cheques

                    // Cheques próprios
                    List<Cheques> lstChequesInseridos = new List<Cheques>();

                    // Guarda os ids dos cheques de terceiros
                    string idsChequeTerc = String.Empty;
                    string idsChequeProp = String.Empty;

                    for (int i = 0; i < idFormaPagto.Length; i++)
                    {
                        // Se a forma de pagamento for cheques próprios cadastra os cheques
                        if ((idFormaPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || idFormaPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro) &&
                            !String.IsNullOrEmpty(chequesPagto))
                        {
                            Cheques cheque;

                            // Separa os cheques guardando-os em um vetor
                            string[] vetCheque = chequesPagto.TrimEnd(' ').TrimEnd('|').Split('|');

                            if (idFormaPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                            {
                                #region Cadastra os cheques próprios

                                // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                                foreach (string c in vetCheque)
                                {
                                    // Divide o cheque para pegar suas propriedades
                                    string[] dadosCheque = c.Split('\t');

                                    if (dadosCheque[0] == "proprio") // Se for cheque próprio
                                    {
                                        // Insere cheque no BD
                                        cheque = ChequesDAO.Instance.GetFromString(c);
                                        if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado) cheque.DataReceb = data;
                                        cheque.IdDevolucaoPagto = idDevolucaoPagto;
                                        cheque.IdCheque = ChequesDAO.Instance.InsertBase(transaction, cheque);

                                        if (cheque.IdCheque < 1)
                                            throw new Exception("retorno do insert do cheque=0");

                                        // Adiciona este cheque à lista de cheques inseridos
                                        lstChequesInseridos.Add(cheque);
                                    }
                                }

                                #endregion

                                #region Gera Movimentação Bancária

                                // Salva o id de cada cheque próprio utilizado na devolução de pagamento
                                foreach (Cheques c in lstChequesInseridos)
                                    idsChequeProp += c.IdCheque.ToString() + ",";

                                // Para cada cheque "Compensado" utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                                foreach (Cheques c in lstChequesInseridos)
                                    if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                    {
                                        ContaBancoDAO.Instance.MovContaDevolucaoPagto(transaction, c.IdContaBanco.Value,
                                            UtilsPlanoConta.GetPlanoContaDevolucaoPagto(idFormaPagto[i], 0, 0), (int)UserInfo.GetUserInfo.IdLoja,
                                            idDevolucaoPagto, idCliente, 2, c.Valor, data);
                                    }

                                #endregion
                            }
                            // Se a forma de pagamento for cheques de terceiros
                            else if (idFormaPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            {
                                // Verifica cheque por cheque
                                foreach (string c in vetCheque)
                                {
                                    // Divide o cheque para pegar suas propriedades
                                    string[] dadosCheque = c.Split('\t');

                                    if (dadosCheque[0] == "terceiro") // Se for cheque de terceiro
                                    {
                                        // Salva o id do cheque
                                        idsChequeTerc += dadosCheque[18] + ",";
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    dev.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, idCliente);
                    dev.Cheques = idsChequeProp + idsChequeTerc.TrimEnd(',');

                    retorno = UtilsFinanceiro.Receber(transaction, UserInfo.GetUserInfo.IdLoja, null, null, null, null, null, null, null, null, null, null, null,
                        idCliente, idDevolucaoPagto, null, data.ToString(), totalDevolucao, totalDevolucao, valores, idFormaPagto, idContaBanco, depositoNaoIdentificado, new uint[] { }, idTipoCartao,
                        idTipoCartao, taxaAntecip, 0, false, false, creditoUtilizado, numAutConstrucard, caixaDiario, numeroParcelas, String.Concat(idsChequeTerc.TrimEnd(',')),
                        false, UtilsFinanceiro.TipoReceb.DevolucaoPagto);

                    if (retorno.ex != null)
                        throw retorno.ex;

                    dev.CreditoGeradoCriar = retorno.creditoGerado;
                    dev.CreditoUtilizadoCriar = creditoUtilizado;
                    Update(transaction, dev);
                        
                    transaction.Commit();
                    transaction.Close();

                    return idDevolucaoPagto;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao efetuar a devolução de pagamento.", ex));
                }
                finally
                {
                    Glass.FilaOperacoes.ReceberDevolucaoPagto.ProximoFila();
                }
            }
        }

        #endregion

        #region Cancela a devolução

        /// <summary>
        /// Cancela uma devolução de pagamento.
        /// </summary>
        public void Cancelar(uint idDevolucaoPagto, string motivo, DateTime dataEstornoBanco)
        {
            Glass.FilaOperacoes.CancelarDevolucaoPagto.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    DevolucaoPagto dev = null;
                    UtilsFinanceiro.DadosCancReceb retorno = null;
                    var idsCheques = string.Empty;

                    dev = GetElementByPrimaryKey(transaction, idDevolucaoPagto);

                    if (dev.Situacao == (int)DevolucaoPagto.SituacaoDevolucao.Cancelada)
                        throw new Exception("Esta devolução de pagamento já foi cancelada.");

                    idsCheques = ChequesDAO.Instance.GetIdsByDevolucaoPagto(transaction, idDevolucaoPagto);

                    retorno = UtilsFinanceiro.CancelaRecebimento(transaction, UtilsFinanceiro.TipoReceb.DevolucaoPagto, null, null, null, null, null, 0,
                        null, null, dev, dataEstornoBanco);

                    if (retorno.ex != null)
                        throw retorno.ex;

                    // Marca a devolução como cancelada
                    dev.Situacao = (int)DevolucaoPagto.SituacaoDevolucao.Cancelada;
                    Update(transaction, dev);

                    LogCancelamentoDAO.Instance.LogDevolucaoPagamento(transaction, dev, motivo, true);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar devolução de pagamento.", ex));
                }
                finally
                {
                    Glass.FilaOperacoes.CancelarDevolucaoPagto.ProximoFila();
                }
            }
        }

        #endregion
    }
}
