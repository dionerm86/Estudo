using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class AntecipContaRecDAO : BaseDAO<AntecipContaRec, AntecipContaRecDAO>
    {
        //private AntecipContaRecDAO() { }

        #region Busca padrão

        public string Sql(uint idAntecipContaRec, uint idContaBanco, uint numeroNfe, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, int situacao, bool selecionar)
        {
            return Sql(null, idAntecipContaRec, idContaBanco, numeroNfe, idCliente, nomeCliente, dataIni,
                dataFim, situacao, selecionar);
        }

        public string Sql(GDASession session, uint idAntecipContaRec, uint idContaBanco, uint numeroNfe, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, int situacao, bool selecionar)
        {
            string campos = selecionar ? "a.*, f.Nome as NomeFunc, Concat(c.Nome, ' Agência: ', c.Agencia, ' Conta: ', c.Conta) " + 
                "as DescrContaBanco, '$$$' as criterio" : "Count(*)";

            string sql = "Select " + campos + @" From antecip_conta_rec a
                Left Join funcionario f On (a.idFuncAntecip=f.idFunc)
                Left Join conta_banco c On (a.idContaBanco=c.idContaBanco) 
                Where 1";

            string criterio = String.Empty;

            if (idAntecipContaRec > 0)
            {
                sql += " And a.IdAntecipContaRec=" + idAntecipContaRec;
                criterio += "Num. Antecip.: " + idAntecipContaRec + "    ";
            }

            if (idContaBanco > 0)
            {
                sql += " And c.IdContaBanco=" + idContaBanco;
                criterio += "Banco: " + ContaBancoDAO.Instance.GetDescricao(session, idContaBanco) + "    ";
            }

            if (numeroNfe > 0)
            {
                sql += " And a.idAntecipContaRec In (Select idAntecipContaRec From (select idAntecipContaRec" +
                    ContasReceberDAO.Instance.SqlBuscarNF(session, "c", selecionar, numeroNfe, false, true) + 
                    " from contas_receber c having concat(',', numeroNFe, ',') like '%," + numeroNfe + ",%') as temp)";

                criterio += "Número NF: " + numeroNfe + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And a.Data>=?dataIni ";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And a.Data<=?dataFim ";
                criterio += "Data término: " + dataFim + "    ";
            }

            if (situacao > 0)
            {
                sql += " And a.situacao=" + situacao;

                AntecipContaRec temp = new AntecipContaRec();
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And a.idAntecipContaRec In (Select idAntecipContaRec From contas_receber Where idCliente=" + idCliente + ")";
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += @" And a.idAntecipContaRec In (Select idAntecipContaRec From contas_receber c Inner Join cliente cli On 
                    (c.idCliente=cli.id_cli) Where cli.id_cli In (" + ids + "))";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<AntecipContaRec> GetList(uint idAntecipContaRec, uint idContaBanco, uint numeroNfe, uint idCliente, string nomeCliente,
            string dataIni, string dataFim, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? " Order By a.IdAntecipContaRec desc" : String.Empty;

            return LoadDataWithSortExpression(Sql(idAntecipContaRec, idContaBanco, numeroNfe, idCliente, nomeCliente, dataIni, dataFim, situacao, true) + sort, sortExpression, startRow, pageSize, GetParam(dataIni, dataFim, nomeCliente));
        }

        public int GetCount(uint idAntecipContaRec, uint idContaBanco, uint idCliente, uint numeroNfe, string nomeCliente, string dataIni, 
            string dataFim, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idAntecipContaRec, idContaBanco, numeroNfe, idCliente, nomeCliente, dataIni, dataFim, situacao, false), GetParam(dataIni, dataFim, nomeCliente));
        }

        public IList<AntecipContaRec> GetForRpt(uint idAntecipContaRec, uint idContaBanco, uint numeroNfe, uint idCliente, string nomeCliente, 
            string dataIni, string dataFim, int situacao)
        {
            return objPersistence.LoadData(Sql(idAntecipContaRec, idContaBanco, numeroNfe, idCliente, nomeCliente, dataIni, dataFim, situacao, true) + " Order By a.IdAntecipContaRec desc", 
                GetParam(dataIni, dataFim, nomeCliente)).ToList();
        }

        public AntecipContaRec GetElement(uint idAntecipContaRec)
        {
            return GetElement(null, idAntecipContaRec);
        }

        public AntecipContaRec GetElement(GDASession session, uint idAntecipContaRec)
        {
            return objPersistence.LoadOneData(session, Sql(session, idAntecipContaRec, 0, 0, 0, null, null, null, 0, true));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim, string nomeCliente)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            
            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Antecipar

        public uint Antecipar(string idsContaRec, uint idContaBanco, decimal total, decimal taxa, decimal juros, decimal iof, 
            DateTime dataRec, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    FilaOperacoes.AntecipacaoContasRec.AguardarVez();

                    uint idContaBancoAntecip = 0, idMovBancoTaxa = 0;

                    // Verifica se foi associado um plano de conta com os campos Taxa, IOF e Juros
                    uint idContaTxAntecip = FinanceiroConfig.PlanoContaTaxaAntecip;
                    uint idContaIOFAntecip = FinanceiroConfig.PlanoContaIOFAntecip;
                    uint idContaJurosAntecip = FinanceiroConfig.PlanoContaJurosAntecip;

                    if (idContaTxAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada à taxa de antecipação.");

                    if (idContaIOFAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada ao IOF de antecipação.");

                    if (idContaJurosAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada aos juros de antecipação.");

                    // Verifica se as contas passadas já foram antecipadas
                    if (ExecuteScalar<bool>(transaction, "Select Count(*)>0 From contas_receber Where idAntecipContaRec>0 And idContaR In (" + idsContaRec + ")"))
                        throw new Exception("Uma ou mais contas selecionadas já foram antecipadas.");

                    AntecipContaRec antecip = new AntecipContaRec();
                    antecip.IdFuncAntecip = UserInfo.GetUserInfo.CodUser;
                    antecip.IdContaBanco = idContaBanco;
                    antecip.Situacao = 1;
                    antecip.Data = dataRec;
                    antecip.Taxa = taxa;
                    antecip.Juros = juros;
                    antecip.Iof = iof;
                    antecip.Valor = total;
                    antecip.Obs = obs;

                    antecip.IdAntecipContaRec = AntecipContaRecDAO.Instance.Insert(transaction, antecip);

                    // Atualiza referência das contas a receber para esta antecipação
                    objPersistence.ExecuteCommand(transaction, "Update contas_receber Set idAntecipContaRec=" + antecip.IdAntecipContaRec +
                        " Where idContaR In (" + idsContaRec + ")");

                    // Gera a movimentação com o valor da antecipação
                    idContaBancoAntecip = ContaBancoDAO.Instance.MovContaAntecip(transaction, idContaBanco, UtilsPlanoConta.GetPlanoConta(
                        UtilsPlanoConta.PlanoContas.AntecipBoleto), (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 1, total, dataRec, obs);

                    // Gera a movimentação com a taxa cobrada pela antecipação
                    if (taxa > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(transaction, idContaBanco, idContaTxAntecip,
                            (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 2, taxa, dataRec, obs);

                    // Gera a movimentação com a taxa cobrada pela antecipação
                    if (juros > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(transaction, idContaBanco, idContaJurosAntecip,
                            (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 2, juros, dataRec, obs);

                    if (iof > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(transaction, idContaBanco, idContaIOFAntecip,
                            (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 2, iof, dataRec, obs);

                    transaction.Commit();
                    transaction.Close();

                    return antecip.IdAntecipContaRec;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.AntecipacaoContasRec.ProximoFila();
                }
            }
        }

        #endregion

        #region Retificar

        public void Retificar(uint idAntecip, string idsContasRecRemover, uint idContaBanco, decimal total, decimal taxa, decimal juros, decimal iof,
            DateTime dataRec, string obs, bool estornar, DateTime? dataEstorno)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    uint idContaBancoAntecip = 0, idMovBancoTaxa = 0;

                    // Verifica se foi associado um plano de conta com os campos Taxa, IOF e Juros
                    uint idContaTxAntecip = FinanceiroConfig.PlanoContaTaxaAntecip;
                    uint idContaIOFAntecip = FinanceiroConfig.PlanoContaIOFAntecip;
                    uint idContaJurosAntecip = FinanceiroConfig.PlanoContaJurosAntecip;

                    if (idContaTxAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada à taxa de antecipação.");

                    if (idContaIOFAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada ao IOF de antecipação.");

                    if (idContaJurosAntecip == 0)
                        throw new Exception("Associe o plano de conta relacionada aos juros de antecipação.");

                    if (!ContasReceberDAO.Instance.PodeRetificarAntecipacao(sessao, idAntecip))
                        throw new Exception("Não é possível retificar essa antecipação, pois ela possui contas que já foram recebidas.");

                    var antecipAtual = GetElement(sessao, idAntecip);
                    var antecip = GetElement(sessao, idAntecip);

                    antecip.IdFuncAntecip = UserInfo.GetUserInfo.CodUser;
                    antecip.IdContaBanco = idContaBanco;
                    antecip.Situacao = 1;
                    antecip.Data = dataRec;
                    antecip.Taxa = taxa;
                    antecip.Juros = juros;
                    antecip.Iof = iof;
                    antecip.Valor = total;
                    antecip.Obs = obs;

                    Update(sessao, antecip);

                    // Remove referência da antecipação das contas
                    if (!String.IsNullOrEmpty(idsContasRecRemover))
                        objPersistence.ExecuteCommand(sessao, "UPDATE contas_receber SET idAntecipContaRec= null WHERE idContaR IN (" + idsContasRecRemover.Trim(',') + ")");

                    if (estornar)
                    {
                        ContaBancoDAO.Instance.MovContaAntecip(sessao, antecipAtual.IdContaBanco, UtilsPlanoConta.GetPlanoConta(
                            UtilsPlanoConta.PlanoContas.AntecipBoleto), (int)UserInfo.GetUserInfo.IdLoja, antecipAtual.IdAntecipContaRec, 2, antecipAtual.Valor, dataEstorno.Value, "Estorno");

                        if (taxa > 0)
                            ContaBancoDAO.Instance.MovContaAntecip(sessao, antecipAtual.IdContaBanco, idContaTxAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                                antecipAtual.IdAntecipContaRec, 1, antecipAtual.Taxa, dataEstorno.Value, "Estorno");

                        if (juros > 0)
                            ContaBancoDAO.Instance.MovContaAntecip(sessao, antecipAtual.IdContaBanco, idContaJurosAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                                antecipAtual.IdAntecipContaRec, 1, antecipAtual.Juros, dataEstorno.Value, "Estorno");

                        if (iof > 0)
                            ContaBancoDAO.Instance.MovContaAntecip(sessao, antecipAtual.IdContaBanco, idContaIOFAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                                antecipAtual.IdAntecipContaRec, 1, antecipAtual.Iof, dataEstorno.Value, "Estorno");
                    }
                    else
                    {
                        var movsBanco = MovBancoDAO.Instance.GetByAntecipacaoContaRec(sessao, idAntecip);

                        foreach (var mb in movsBanco)
                            MovBancoDAO.Instance.Cancelar(sessao, mb.IdMovBanco, "Retificação de boletos antecipados", false);
                    }

                    // Gera a movimentação com o valor da antecipação
                    idContaBancoAntecip = ContaBancoDAO.Instance.MovContaAntecip(sessao, idContaBanco, UtilsPlanoConta.GetPlanoConta(
                        UtilsPlanoConta.PlanoContas.AntecipBoleto), (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 1, total, dataRec, obs);

                    // Gera a movimentação com a taxa cobrada pela antecipação
                    if (taxa > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(sessao, idContaBanco, idContaTxAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                            antecip.IdAntecipContaRec, 2, taxa, dataRec, obs);

                    // Gera a movimentação com a taxa cobrada pela antecipação
                    if (juros > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(sessao, idContaBanco, idContaJurosAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                            antecip.IdAntecipContaRec, 2, juros, dataRec, obs);

                    if (iof > 0)
                        idMovBancoTaxa = ContaBancoDAO.Instance.MovContaAntecip(sessao, idContaBanco, idContaIOFAntecip, (int)UserInfo.GetUserInfo.IdLoja,
                            antecip.IdAntecipContaRec, 2, iof, dataRec, obs);
                    
                    sessao.Commit();
                    sessao.Close();
                }
                catch
                {
                    sessao.Rollback();
                    sessao.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Cancelar

        /// <summary>
        /// Cancela antecipação
        /// </summary>
        public void Cancelar(uint idAntecipContaRec, bool gerarEstorno, string motivo, DateTime? dataEstorno)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    AntecipContaRec antecip = GetElement(transaction, idAntecipContaRec);

                    if (antecip.Situacao == (uint)AntecipContaRec.SituacaoEnum.Cancelada)
                        throw new Exception("Esta antecipação já foi cancelada.");

                    // Se as contas a receber tiverem ter sido recebidas não permite cancelar a antecipação
                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From contas_receber Where recebida=1 And idAntecipContaRec=" +
                        idAntecipContaRec, null) > 0)
                        throw new Exception("Algumas contas antecipadas já foram recebidas, cancele o recebimento das mesmas antes de cancelar esta antecipação.");

                    // Desassocia contas a receber com essa antecipação
                    objPersistence.ExecuteCommand(transaction, "Update contas_receber Set idAntecipContaRec=null Where idAntecipContaRec=" + idAntecipContaRec);

                    // Estorna a antecipação
                    if (!gerarEstorno) // Exclui movimentações que esta antecipação gerou
                    {
                        // Pega a primeira movimentação da conta bancária que esta antecipação foi feita apenas para alterar o saldo
                        string sql = "Select idMovBanco from mov_banco Where idContaBanco=" + antecip.IdContaBanco +
                            " And idAntecipContaRec=" + idAntecipContaRec + " order by idMovBanco asc limit 1";

                        uint idMovBanco = ExecuteScalar<uint>(transaction, sql);

                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idMovBanco);

                        MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idMovBanco);

                        // Corrige saldo
                        objPersistence.ExecuteCommand(transaction, "Update mov_banco Set valorMov=0 Where idAntecipContaRec=" + idAntecipContaRec);
                        if (movAnterior != null)
                            MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idMovBanco);

                        // Exclui movimentações que esta antecipação gerou
                        objPersistence.ExecuteCommand(transaction, "Delete From mov_banco Where idAntecipContaRec=" + idAntecipContaRec);
                    }
                    else // Estorna valores
                    {
                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, antecip.IdContaBanco, dataEstorno.Value);

                        ContaBancoDAO.Instance.MovContaAntecip(transaction, antecip.IdContaBanco, UtilsPlanoConta.GetPlanoConta(
                            UtilsPlanoConta.PlanoContas.EstornoAntecipBoleto), (int)UserInfo.GetUserInfo.IdLoja, antecip.IdAntecipContaRec, 2, antecip.Valor - antecip.Taxa -
                            antecip.Juros - antecip.Iof, dataEstorno.Value, motivo);
                    }

                    // Cancela a antecipação
                    objPersistence.ExecuteCommand(transaction, "Update antecip_conta_rec Set situacao=" + (int)AntecipContaRec.SituacaoEnum.Cancelada +
                        ", obs=?obs Where idAntecipContaRec=" + idAntecipContaRec, new GDAParameter("?obs", "Motivo do Cancelamento: " + motivo));

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion
    }
}
