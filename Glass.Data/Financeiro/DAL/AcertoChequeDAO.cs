using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class AcertoChequeDAO : BaseDAO<AcertoCheque, AcertoChequeDAO>
    {
        //private AcertoChequeDAO() { }

        #region Busca padrão

        private string Sql(uint idAcertoCheque, uint idFunc, uint idCliente, string nomeCliente, string dataIni, string dataFim,
            bool? chequesProprios, bool chequesCaixaDiario, bool selecionar)
        {
            return Sql(null, idAcertoCheque, idFunc, idCliente, nomeCliente, dataIni, dataFim, chequesProprios, chequesCaixaDiario,
                selecionar);
        }

        private string Sql(GDA.GDASession session, uint idAcertoCheque, uint idFunc, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, bool? chequesProprios, bool chequesCaixaDiario, bool selecionar)
        {
            StringBuilder criterio = new StringBuilder();

            StringBuilder sql = new StringBuilder("select ");
            sql.Append(selecionar ? @"a.*, f.Nome as NomeFunc, cli.nome as nomeCliente, temp.FormasPagto, '$$$' as Criterio, 
                (select count(*) from item_acerto_cheque iac inner join cheques c on (iac.idCheque=c.idCheque) where c.tipo=1 and iac.idAcertoCheque=a.idAcertoCheque" +
                (chequesCaixaDiario ? " and c.movcaixadiario " : "") + ")>0 " +
                "as chequesProprios" : "Count(*)");

            sql.Append(@"
                from acerto_cheque a 
                    left join funcionario f on (a.idFunc=f.idFunc)
                    left join cliente cli On (a.idCliente=cli.id_cli)
                    left join (
                        select pac.idAcertoCheque, cast(group_concat(
                            concat(fp.Descricao, ' - R$ ', replace(cast(pac.valorPagto as char), '.', ','), 
                                if(pac.idContaBanco>0, Concat(' Banco: ', cb.nome, ' Agência: ', cb.agencia, ' Conta: ', cb.conta), '')
                            ) SEPARATOR ', ') as char) as FormasPagto
                        from pagto_acerto_cheque pac
                            left join formapagto fp on (pac.idFormaPagto=fp.idformaPagto)
                            left join conta_banco cb on (pac.idContaBanco=cb.idContaBanco)
                        group by idAcertoCheque
                    ) as temp on (a.idAcertoCheque=temp.idAcertoCheque)
                where 1");

            if (idAcertoCheque > 0)
            {
                sql.Append(" and a.idAcertoCheque=");
                sql.Append(idAcertoCheque);

                criterio.Append("Núm. Acerto: ");
                criterio.Append(idAcertoCheque);
                criterio.Append("    ");
            }

            if (idFunc > 0)
            {
                sql.Append(" and a.idFunc=");
                sql.Append(idFunc);

                criterio.Append("Funcionário: ");
                criterio.Append(idFunc);
                criterio.Append(" - ");
                criterio.Append(FuncionarioDAO.Instance.GetNome(session, idFunc));
                criterio.Append("    ");
            }

            if (idCliente > 0)
            {
                sql.Append(" and a.idCliente=");
                sql.Append(idCliente);

                criterio.Append("Cliente: ");
                criterio.Append(ClienteDAO.Instance.GetNome(session, idCliente));
                criterio.Append("    ");
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql.Append(" And cli.id_Cli in (");
                sql.Append(ids);
                sql.Append(")");

                criterio.Append("Cliente: ");
                criterio.Append(nomeCliente);
                criterio.Append("    ");
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql.Append(" and a.DataAcerto>=?dataIni");
                
                criterio.Append("Data inicial: ");
                criterio.Append(dataIni);
                criterio.Append("    ");
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql.Append(" and a.DataAcerto<=?dataFim");
                
                criterio.Append("Data término: ");
                criterio.Append(dataFim);
                criterio.Append("    ");
            }

            if (chequesCaixaDiario)
            {
                sql.Append(" and a.idAcertoCheque in (select iac.idAcertoCheque from item_acerto_cheque iac inner join cheques c on " +
                    "(iac.idCheque=c.idCheque) where c.movcaixadiario)");
                criterio.Append("Movimenta caixa diario: ");
                criterio.Append(chequesCaixaDiario);
                criterio.Append("    ");
            }

            if (chequesProprios != null)
            {
                sql.Append(" and a.idAcertoCheque in (select iac.idAcertoCheque from item_acerto_cheque iac inner join cheques c on " +
                    "(iac.idCheque=c.idCheque) where c.tipo=");
                sql.Append(chequesProprios.Value ? "1" : "2");
                sql.Append(")");

                criterio.Append("Cheques ");
                criterio.Append(chequesProprios.Value ? "Próprios" : "de Terceiros");
                criterio.Append("    ");
            }            

            return sql.ToString().Replace("$$$", criterio.ToString());
        }

        public IList<AcertoCheque> GetList(uint idAcertoCheque, uint idFunc, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, string chequesProprios, bool chequesCaixaDiario, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idAcertoCheque, idFunc, idCliente, nomeCliente, dataIni, dataFim, chequesProprios == "1", chequesCaixaDiario, 
                true) + " order by idAcertoCheque desc", sortExpression, startRow, pageSize, GetParams(nomeCliente, dataIni, dataFim));
        }

        public IList<AcertoCheque> GetListForRpt(uint idAcertoCheque, uint idFunc, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, string chequesProprios, bool chequesCaixaDiario)
        {
            return objPersistence.LoadData(Sql(idAcertoCheque, idFunc, idCliente, nomeCliente, dataIni, dataFim, chequesProprios == "1", chequesCaixaDiario,
                true) + " order by idAcertoCheque desc", GetParams(nomeCliente, dataIni, dataFim)).ToList();
        }

        public int GetListCount(uint idAcertoCheque, uint idFunc, uint idCliente, string nomeCliente, string dataIni, 
            string dataFim, string chequesProprios, bool chequesCaixaDiario)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idAcertoCheque, idFunc, idCliente, nomeCliente, dataIni, dataFim, 
                chequesProprios == "1", chequesCaixaDiario, false), GetParams(nomeCliente, dataIni, dataFim));
        }

        public AcertoCheque GetElement(uint idAcertoCheque)
        {
            return GetElement(null, idAcertoCheque);
        }

        public AcertoCheque GetElement(GDASession session, uint idAcertoCheque)
        {
            return objPersistence.LoadOneData(session, Sql(session, idAcertoCheque, 0, 0, null, null, null, null, false, true));
        }

        private GDAParameter[] GetParams(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lst.ToArray();
        }

        #endregion

        #region Atualiza dados do acerto do cheque

        /// <summary>
        /// Atualiza dados do acerto do cheque, após efetivá-lo
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        /// <param name="idCliente"></param>
        /// <param name="valor"></param>
        /// <param name="juros"></param>
        public void AtualizaAcertoCheque(GDASession sessao, uint idAcertoCheque, uint idCliente, decimal valor, decimal juros)
        {
            string sql = "Update acerto_cheque Set valorAcerto=" + (valor + juros).ToString().Replace(",", ".");

            if (idCliente > 0)
                sql += ", idCliente=" + idCliente;

            if (juros > 0)
                sql += ", juros=" + juros.ToString().Replace(",", ".");

            sql += " Where idAcertoCheque=" + idAcertoCheque;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Verifica se o acerto pode ser cancelado

        /// <summary>
        /// Verifica se o acerto pode ser cancelado.
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        /// <returns></returns>
        public bool PodeCancelar(uint idAcertoCheque)
        {
            string sql = "select count(*) from item_acerto_cheque where valorReceb>0 and idAcertoCheque=" + idAcertoCheque;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Cancelar o acerto de cheques

        private static readonly object _cancelarAcertoChequeLock = new object();

        /// <summary>
        /// Cancela o acerto de cheques.
        /// </summary>
        public void CancelarAcertoCheque(uint idAcertoCheque, string motivo, DateTime dataEstornoBanco)
        {
            lock(_cancelarAcertoChequeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        UtilsFinanceiro.DadosCancReceb retorno = null;
                        AcertoCheque acertoCheque = null;

                        acertoCheque = GetElement(transaction, idAcertoCheque);

                        retorno = UtilsFinanceiro.CancelaRecebimento(transaction, !acertoCheque.ChequesProprios ?
                            UtilsFinanceiro.TipoReceb.ChequeDevolvido : UtilsFinanceiro.TipoReceb.ChequeProprioDevolvido, null, null, null,
                            null, null, idAcertoCheque, null, null, null, dataEstornoBanco);

                        if (retorno.ex != null)
                            throw retorno.ex;

                        // Altera a situação do acerto
                        objPersistence.ExecuteCommand(transaction, "update acerto_cheque set situacao=" +
                            (int)AcertoCheque.SituacaoEnum.Cancelado + " where idAcertoCheque=" + idAcertoCheque);

                        LogCancelamentoDAO.Instance.LogAcertoCheque(acertoCheque, motivo, true);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar acerto de cheques.", ex));
                    }
                }
            }
        }

        #endregion

        #region Busca os IDs dos cheques em um acerto de cheque
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os IDs dos cheques em um acerto de cheque.
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        /// <returns></returns>
        public string GetIdsChequesByAcertoCheque(uint idAcertoCheque)
        {
            return GetIdsChequesByAcertoCheque(null, idAcertoCheque);
        }

        /// <summary>
        /// Busca os IDs dos cheques em um acerto de cheque.
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        /// <returns></returns>
        public string GetIdsChequesByAcertoCheque(GDASession sessao, uint idAcertoCheque)
        {
            string sql = "select cast(group_concat(idCheque) as char) from item_acerto_cheque where idAcertoCheque=" + idAcertoCheque;
            return ExecuteScalar<string>(sessao, sql);
        }

        #endregion
    }
}
