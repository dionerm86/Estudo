using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class EncontroContasDAO : BaseDAO<EncontroContas, EncontroContasDAO>
    {
        //private EncontroContasDAO() { }

        private object EncontroContasLock = new object();

        #region Métodos de retorno de itens

        private string Sql(uint idEncontroContas, uint idCliente, string nomeCliente, uint idFornecedor, string nomeFornecedor,
            string obs, int situacao, string dataCadIni, string dataCadFim, uint usuCad, string dataFinIni, string dataFinFim,
            uint usuFin, bool selecionar)
        {
            return Sql(null, idEncontroContas, idCliente, nomeCliente, idFornecedor, nomeFornecedor, obs, situacao, dataCadIni,
                dataCadFim, usuCad, dataFinIni, dataFinFim, usuFin, selecionar);
        }

        private string Sql(GDASession session, uint idEncontroContas, uint idCliente, string nomeCliente, uint idFornecedor,
            string nomeFornecedor, string obs, int situacao, string dataCadIni, string dataCadFim, uint usuCad, string dataFinIni,
            string dataFinFim, uint usuFin, bool selecionar)
        {
            string campos = selecionar ? "ec.*, c.Nome as NomeCliente, fornec.NomeFantasia as NomeFornecedor, '$$$' as Criterio" : "COUNT(*)";

            string sql = @"SELECT " + campos + @"
                           FROM encontro_contas ec
                           LEFT JOIN cliente c ON (ec.idCliente = c.id_cli)
                           LEFT JOIN fornecedor fornec ON (ec.idFornecedor = fornec.idFornec)
                           LEFT JOIN funcionario f ON (ec.idFuncCad = f.idFunc)
                           WHERE 1";

            string criterio = string.Empty;

            if (idEncontroContas > 0)
            {
                sql += " AND ec.IdEncontroContas=" + idEncontroContas;
                criterio = "Encontro de Contas a Pagar/Receber: " + idEncontroContas + "  ";
            }

            if (idCliente > 0)
            {
                sql += " AND ec.IdCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, (uint)idCliente) + "  ";
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " AND ec.IdCliente IN(" + ids + ")";
                criterio = "Cliente: " + nomeCliente;
            }

            if (idFornecedor > 0)
            {
                sql += " AND ec.IdFornecedor=" + idFornecedor;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(session, idFornecedor) + "   ";
            }
            else if (!string.IsNullOrEmpty(nomeFornecedor))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(session, nomeFornecedor);
                sql += " AND ec.IdFOrnecedor IN(" + ids + ")";
                criterio += "Fornecedor: " + nomeFornecedor + "    ";
            }

            if (!string.IsNullOrEmpty(obs))
            {
                sql += " AND ec.Observacao=?obs";
                criterio += "Obs.: " + obs + "   ";
            }

            if (situacao > 0)
            {
                sql += " AND ec.Situacao=" + situacao;
                criterio += "Situação: " + ObtemSituacaoStr(situacao) + "   ";
            }

            if (!string.IsNullOrEmpty(dataCadIni))
            {
                sql += " AND ec.DataCad >=?dataCadIni";
                criterio += "A partir de: " + dataCadIni + "";
            }

            if (!string.IsNullOrEmpty(dataCadFim))
            {
                sql += " AND ec.DataCad <=?dataCadFim";
                criterio += "Até: " + dataCadFim + "";
            }

            if (usuCad > 0)
            {
                sql += " AND ec.IdFuncCad=" + usuCad;
                criterio += "Funcionário cadastro: " + FuncionarioDAO.Instance.GetNome(session, usuCad);
            }

            if (!string.IsNullOrEmpty(dataFinIni))
            {
                sql += " AND ec.DataFin >=?dataFinIni";
                criterio += "A partir de: " + dataCadIni + "";
            }

            if (!string.IsNullOrEmpty(dataFinFim))
            {
                sql += " AND ec.DataFin <=?dataFinFim";
                criterio += "Até: " + dataCadFim + "";
            }

            if (usuFin > 0)
            {
                sql += " AND ec.IdFuncFin=" + usuFin;
                criterio += "Funcionário finalização: " + FuncionarioDAO.Instance.GetNome(session, usuFin);
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Recupera um Encontro de Contas
        /// </summary>
        public EncontroContas GetElement(uint idEncontroContas)
        {
            return GetElement(null, idEncontroContas);
        }

        /// <summary>
        /// Recupera um Encontro de Contas
        /// </summary>
        public EncontroContas GetElement(GDASession session, uint idEncontroContas)
        {
            return objPersistence.LoadOneData(session, Sql(session, idEncontroContas, 0, null, 0, null, null, 0, null, null, 0, null, null, 0, true));
        }

        private GDAParameter[] GetParams(string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string obs)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataCadIni))
                lst.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataCadFim))
                lst.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59")));

            if (!string.IsNullOrEmpty(dataFinIni))
                lst.Add(new GDAParameter("?dataFinIni", DateTime.Parse(dataFinIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataFinFim))
                lst.Add(new GDAParameter("?dataFinFim", DateTime.Parse(dataFinFim + " 23:59")));

            if (!String.IsNullOrEmpty(obs))
                lst.Add(new GDAParameter("?obs", obs));

            return lst.ToArray();
        }

        /// <summary>
        /// Metodo para recuperar a lista de encontros para a grid
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idFornecedor"></param>
        /// <param name="nomeFornecedor"></param>
        /// <param name="obs"></param>
        /// <param name="situacao"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="usuCad"></param>
        /// <param name="dataFinIni"></param>
        /// <param name="dataFinFim"></param>
        /// <param name="usuFin"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public EncontroContas[] GetList(uint idEncontroContas, uint idCliente, string nomeCliente, uint idFornecedor, string nomeFornecedor,
            string obs, int situacao, string dataCadIni, string dataCadFim, uint usuCad, string dataFinIni, string dataFinFim,
            uint usuFin, string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idEncontroContas, idCliente, nomeCliente, idFornecedor, nomeFornecedor, obs, situacao, dataCadIni, dataCadFim, usuCad,
                dataFinIni, dataFinFim, usuFin, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs)).ToArray();
        }

        /// <summary>
        /// Recupera o numero de registro da pesquisa
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idFornecedor"></param>
        /// <param name="nomeFornecedor"></param>
        /// <param name="obs"></param>
        /// <param name="situacao"></param>
        /// <param name="dataCadIni"></param>
        /// <param name="dataCadFim"></param>
        /// <param name="usuCad"></param>
        /// <param name="dataFinIni"></param>
        /// <param name="dataFinFim"></param>
        /// <param name="usuFin"></param>
        /// <returns></returns>
        public int GetListCount(uint idEncontroContas, uint idCliente, string nomeCliente, uint idFornecedor, string nomeFornecedor,
            string obs, int situacao, string dataCadIni, string dataCadFim, uint usuCad, string dataFinIni, string dataFinFim,
            uint usuFin)
        {
            string sql = Sql(idEncontroContas, idCliente, nomeCliente, idFornecedor, nomeFornecedor, obs, situacao, dataCadIni, dataCadFim, usuCad,
                dataFinIni, dataFinFim, usuFin, false);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs));
        }

        /// <summary>
        /// Recupera um encontro para exibir no relatório
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public EncontroContas[] GetForRpt(uint idEncontroContas)
        {
            string sql = Sql(idEncontroContas, 0, null, 0, null, null, 0, null, null, 0, null, null, 0, true);

            return objPersistence.LoadData(sql).ToArray();
        }


        public EncontroContas[] GetForListRpt(uint idEncontroContas, uint idCliente, string nomeCliente, uint idFornecedor, string nomeFornecedor,
            string obs, int situacao, string dataCadIni, string dataCadFim, uint usuCad, string dataFinIni, string dataFinFim,
            uint usuFin)
        {
            string sql = Sql(idEncontroContas, idCliente, nomeCliente, idFornecedor, nomeFornecedor, obs, situacao, dataCadIni, dataCadFim, usuCad,
                dataFinIni, dataFinFim, usuFin, true);

            return objPersistence.LoadData(sql, GetParams(dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs)).ToArray();
        }

        /// <summary>
        /// Recupera lista de encontro para seleção na tele de retificar
        /// </summary>
        /// <returns></returns>
        public EncontroContas[] GetForRetificar(int situacao)
        {
            string sql = Sql(0, 0, null, 0, null, null, situacao, null, null, 0, null, null, 0, true);

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Busca de Campos

        /// <summary>
        /// Retorna a situação de um encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public EncontroContas.SituacaoEncontroContas ObtemSituacao(uint idEncontroContas)
        {
            return (EncontroContas.SituacaoEncontroContas)ObtemValorCampo<int>("situacao", "idEncontroContas=" + idEncontroContas);
        }

        /// <summary>
        /// Retorna a desc. da situação do encontro de contas
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string ObtemSituacaoStr(int situacao)
        {
            EncontroContas ec = new EncontroContas();
            ec.Situacao = situacao;
            return ec.SituacaoStr;
        }

        /// <summary>
        /// Retorna o valor total de vinculos de conta a pagar
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public decimal ObtemTotalPagar(GDASession sessao, uint idEncontroContas)
        {
            string sql = @"SELECT COALESCE(SUM(cp.ValorVenc),0)
                           FROM contas_pagar_encontro_contas cpec
                           LEFT JOIN contas_pagar cp ON (cpec.idContaPg = cp.idContaPg)
                           WHERE cpec.idEncontroContas=" + idEncontroContas;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        /// <summary>
        /// Retorna o valor total de vinculos de conta a recever
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public decimal ObtemTotalReceber(GDASession sessao, uint idEncontroContas)
        {
            string sql = @"SELECT COALESCE(SUM(cr.ValorVec),0)
                           FROM contas_receber_encontro_contas crec
                           LEFT JOIN contas_receber cr ON (crec.idContaR = cr.idContaR)
                           WHERE crec.idEncontroContas=" + idEncontroContas;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        /// <summary>
        /// Recupera o fornecedor do encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public uint ObtemIdFornec(uint idEncontroContas)
        {
            return ObtemValorCampo<uint>("idFornecedor", "idEncontroContas=" + idEncontroContas);
        }

        /// <summary>
        /// Recupera o cliente do encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idEncontroContas)
        {
            return ObtemValorCampo<uint>("idCliente", "idEncontroContas=" + idEncontroContas);
        }

        /// <summary>
        /// Verifica se a conta a ser gerada deve ser a pagar ou a receber
        /// </summary>
        /// <returns>0 - Nenhuma 1 - A pagar 2 - A receber</returns>
        public int ObtemTipoContaGerar(uint idEncontroContas)
        {
            return ObtemTipoContaGerar(null, idEncontroContas);
        }

        /// <summary>
        /// Verifica se a conta a ser gerada deve ser a pagar ou a receber
        /// </summary>
        /// <returns>0 - Nenhuma 1 - A pagar 2 - A receber</returns>
        public int ObtemTipoContaGerar(GDASession session, uint idEncontroContas)
        {
            decimal valorPagar = ObtemTotalPagar(session, idEncontroContas);
            decimal valorReceber = ObtemTotalReceber(session, idEncontroContas);

            if (valorPagar == valorReceber)
                return 0;
            else if (valorPagar > valorReceber)
                return 1;
            else
                return 2;
        }

        /// <summary>
        /// Recupera a obs. do encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public string ObtemObs(uint idEncontroContas)
        {
            return ObtemValorCampo<string>("obs", "idEncontroContas=" + idEncontroContas);
        }

        #endregion

        #region Finaliza Encontro

        /// <summary>
        /// Finaliza o encontro de contas
        /// </summary>
        public void FinalizaEncontro(uint idEncontroContas, DateTime dataVenc, ref int contaGerada, ref decimal valorGerado)
        {
            lock (EncontroContasLock)
            {
                using (var sessao = new GDA.GDATransaction())
                {
                    try
                    {
                        sessao.BeginTransaction();

                        //Vincula contas a pagar/receber (Marca a conta como paga ou recebida)
                        ContasPagarEncontroContasDAO.Instance.GeraVinculoContaPagar(sessao, idEncontroContas);
                        ContasReceberEncontroContasDAO.Instance.GeraVinculoContaReceber(sessao, idEncontroContas);

                        Instance.GeraContasPagarReceber(sessao, idEncontroContas, dataVenc,
                            ref contaGerada, ref valorGerado);

                        string sql = @"UPDATE encontro_contas
                           SET situacao=" + (int)EncontroContas.SituacaoEncontroContas.Finalizado + @",
                               dataFin=?dtFin, idFuncFin=" + UserInfo.GetUserInfo.CodUser + @"
                           WHERE idEncontroContas=" + idEncontroContas;

                        objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?dtFin", DateTime.Now));


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
        }

        #endregion

        #region Cancela Encontro

        /// <summary>
        /// Cancela um encontro de contas
        /// </summary>
        public void CancelaEncontro(uint idEncontroContas, string movito)
        {
            lock (EncontroContasLock)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        //Apaga os vinculos das contas a pagar/receber (desmarca a conta como paga ou recebida)
                        ContasPagarEncontroContasDAO.Instance.RemoveVinculoContaPagar(transaction, idEncontroContas);
                        ContasReceberEncontroContasDAO.Instance.RemoveVinculoContaReceber(transaction, idEncontroContas);

                        //Apaga os contas a pagar/receber gerados
                        var tipoConta = Instance.ObtemTipoContaGerar(transaction, idEncontroContas);

                        if (tipoConta == 1)
                            ContasPagarDAO.Instance.DeleteByEncontroContas(transaction, idEncontroContas);
                        else if (tipoConta == 2)
                            ContasReceberDAO.Instance.DeleteByEncontroContas(transaction, idEncontroContas);

                        var ec = GetElement(transaction, idEncontroContas);

                        LogCancelamentoDAO.Instance.LogEncontroContas(transaction, ec, movito, true);

                        if (ec.Situacao == (int)EncontroContas.SituacaoEncontroContas.Aberto)
                        {
                            Delete(transaction, ec);
                            return;
                        }

                        ec.Situacao = (int)EncontroContas.SituacaoEncontroContas.Cancelado;
                        ec.IdFuncCanc = UserInfo.GetUserInfo.IdCliente;
                        ec.DataCanc = DateTime.Now;

                        Update(transaction, ec);

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
        }

        #endregion

        #region Cancela Encontro

        /// <summary>
        /// Retifica um encontro de contas
        /// </summary>
        public void RetificaEncontro(int idEncontroContas, List<string> idsContasPagar, List<string> idsContasReceber,
            DateTime dataVenc, ref int contaGerada, ref decimal valorGerado)
        {
            lock (EncontroContasLock)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        //Apaga as contas a pagar/receber geradas
                        var tipoConta = Instance.ObtemTipoContaGerar(transaction, (uint)idEncontroContas);

                        if (tipoConta == 1)
                            ContasPagarDAO.Instance.DeleteByEncontroContas(transaction, (uint)idEncontroContas);
                        else if (tipoConta == 2)
                            ContasReceberDAO.Instance.DeleteByEncontroContas(transaction, (uint)idEncontroContas);

                        //Apaga os vinculos das contas a pagar/receber (desmarca a conta como paga ou recebida)
                        ContasPagarEncontroContasDAO.Instance.RemoveVinculoContaPagar(transaction, (uint)idEncontroContas);
                        ContasReceberEncontroContasDAO.Instance.RemoveVinculoContaReceber(transaction, (uint)idEncontroContas);

                        foreach (var idContaPg in idsContasPagar)
                            ContasPagarEncontroContasDAO.Instance.DeleteByIdContaPg(transaction, idContaPg.StrParaUint());
                        foreach (var idContaR in idsContasReceber)
                            ContasReceberEncontroContasDAO.Instance.DeleteByIdContaR(transaction, idContaR.StrParaUint());

                        //Vincula contas a pagar/receber (Marca a conta como paga ou recebida)
                        ContasPagarEncontroContasDAO.Instance.GeraVinculoContaPagar(transaction, (uint)idEncontroContas);
                        ContasReceberEncontroContasDAO.Instance.GeraVinculoContaReceber(transaction, (uint)idEncontroContas);

                        //Gera contas a pagar/receber
                        Instance.GeraContasPagarReceber(transaction, (uint)idEncontroContas, dataVenc, ref contaGerada, ref valorGerado);

                        //Salva log de retificação
                        var dados =
                            string.Format(
                                "Contas a pagar removidas: {0} Contas a receber removidas: {1}",
                                string.Join(",", idsContasPagar), string.Join(",", idsContasReceber));

                        Instance.AtualizaObs(transaction, (uint)idEncontroContas, dados);

                        var ec = Instance.GetElement(transaction, (uint)idEncontroContas);
                        ec.DadosRetificar = dados;

                        LogAlteracaoDAO.Instance.LogEncontroContas(transaction, ec);

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
        }

        #endregion

        #region Gera contas a pagar/receber

        /// <summary>
        /// Gera as contas a pagar ou a receber de um encontro
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="dtVenc"></param>
        /// <param name="ContaGerar">tipo de conta gerada: 1 - Conta a pagar 2 - Conta a receber</param>
        /// <param name="valorGerar">valor gerado</param>
        public void GeraContasPagarReceber(GDASession sessao, uint idEncontroContas, DateTime dtVenc, ref int contaGerar, ref decimal valorGerar)
        {
            try
            {
                decimal valorPagar = ObtemTotalPagar(sessao, idEncontroContas);
                decimal valorReceber = ObtemTotalReceber(sessao, idEncontroContas);

                if (valorPagar == valorReceber)
                {
                    contaGerar = 0;
                    valorGerar = 0;
                    return;
                }

                string tipoContas = ObtemTipoConta(sessao, idEncontroContas);

                if (valorPagar > valorReceber)
                {
                    valorGerar = valorPagar - valorReceber;

                    ContasPagarDAO.Instance.Insert(sessao, new ContasPagar()
                    {
                        IdEncontroContas = idEncontroContas,
                        IdFornec = ObtemIdFornec(idEncontroContas),
                        IdLoja = UserInfo.GetUserInfo.IdLoja,
                        IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorExcedenteEncontroContas),
                        DataVenc = dtVenc,
                        ValorVenc = valorGerar,
                        NumParc = 1,
                        NumParcMax = 1,
                        Contabil = tipoContas == FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil
                    });

                    contaGerar = 1;
                }
                else
                {
                    valorGerar = valorReceber - valorPagar;

                    ContasReceberDAO.Instance.Insert(sessao, new ContasReceber()
                    {
                        IdLoja = UserInfo.GetUserInfo.IdLoja,
                        IdEncontroContas = idEncontroContas,
                        IdCliente = ObtemIdCliente(idEncontroContas),
                        IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorExcedenteEncontroContas),
                        DataVec = dtVenc,
                        ValorVec = valorGerar,
                        NumParc = 1,
                        NumParcMax = 1,
                        TipoConta = tipoContas == FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil ? (byte)ContasReceber.TipoContaEnum.Contabil :
                            tipoContas == FinanceiroConfig.ContasPagarReceber.DescricaoContaCupomFiscal ? (byte)ContasReceber.TipoContaEnum.CupomFiscal :
                            (byte)ContasReceber.TipoContaEnum.NaoContabil
                    });

                    contaGerar = 2;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Verifica conta paga ou recebida

        /// <summary>
        /// Verifica se o encontro de contas tem uma conta paga ou recebida
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public bool TemContaPagaRecebida(uint idEncontroContas)
        {
            try
            {
                string sqlPaga = @"SELECT COUNT(*) FROM contas_pagar WHERE paga=true AND idEncontroContas=" + idEncontroContas;
                string sqlRecebida = @"SELECT COUNT(*) FROM contas_receber WHERE recebida=true AND idEncontroContas=" + idEncontroContas;

                if (objPersistence.ExecuteSqlQueryCount(sqlPaga) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(sqlRecebida) > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Atualiza Observação

        /// <summary>
        /// Atualiza observação do encontro de contas
        /// </summary>
        public void AtualizaObs(uint idEncontroContas, string obs)
        {
            AtualizaObs(null, idEncontroContas, obs);
        }

        /// <summary>
        /// Atualiza observação do encontro de contas
        /// </summary>
        public void AtualizaObs(GDASession session, uint idEncontroContas, string obs)
        {
            objPersistence.ExecuteCommand(session, "Update encontro_contas Set observacao=Trim(Concat(Coalesce(observacao, ''), ?obs)) Where idEncontroContas=" + idEncontroContas,
                new GDAParameter("?obs", " - " + obs + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
        }

        #endregion

        #region Retorna o tipo de contas a receber/pagar do encontro de contas

        /// <summary>
        /// Retorna o tipo de contas a receber/pagar do encontro de contas.
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <returns></returns>
        public string ObtemTipoConta(GDASession sessao, uint idEncontroContas)
        {
            string ids, sql;

            ids = GetValoresCampo(sessao, "select idContaR from contas_receber_encontro_contas " +
                "where idEncontroContas=" + idEncontroContas, "idContaR");

            if (!String.IsNullOrEmpty(ids))
            {
                sql = "select " + ContasReceberDAO.Instance.SqlCampoDescricaoContaContabil("c") + @"
                    from contas_receber c where c.idContaR in (" + ids + ")";

                return ExecuteScalar<string>(sessao, sql).Split(',')[0].Trim();
            }
            else
            {
                ids = GetValoresCampo(sessao, "select idContaPg from contas_pagar_encontro_contas " +
                    "where idEncontroContas=" + idEncontroContas, "idContaPg");

                if (!String.IsNullOrEmpty(ids))
                {
                    sql = "select " + ContasPagarDAO.Instance.SqlCampoDescricaoContaContabil("c") + @"
                        from contas_pagar c where c.idContaPg in (" + ids + ")";

                    return ExecuteScalar<string>(sessao, sql);
                }
            }

            return null;
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Insere Encontro de Contas
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public override uint Insert(Glass.Data.Model.EncontroContas objInsert)
        {
            objInsert.IdFuncCad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;
            objInsert.Situacao = (int)EncontroContas.SituacaoEncontroContas.Aberto;
            objInsert.IdEncontroContas = base.Insert(objInsert);

            return objInsert.IdEncontroContas;
        }

        /// <summary>
        /// Atualiza o Encontro de Contas
        /// </summary>
        public override int Update(EncontroContas objUpdate)
        {
            return Update(null, objUpdate);
        }

        /// <summary>
        /// Atualiza o Encontro de Contas
        /// </summary>
        public override int Update(GDASession session, EncontroContas objUpdate)
        {
            EncontroContas ec = GetElement(session, objUpdate.IdEncontroContas);

            objUpdate.IdFuncCad = ec.IdFuncCad;
            objUpdate.DataCad = ec.DataCad;

            //Se for alterado o cliente ou o fornecedor, apaga os vinculos ja adicionados
            if (ec.IdCliente != objUpdate.IdCliente || ec.IdFornecedor != objUpdate.IdFornecedor)
            {
                ContasReceberEncontroContasDAO.Instance.DeleteByIdEncontroContas(session, objUpdate.IdEncontroContas);
                ContasPagarEncontroContasDAO.Instance.DeleteByIdEncontroContas(session, objUpdate.IdEncontroContas);
            }

            return base.Update(session, objUpdate);
        }

        #endregion
    }
}
