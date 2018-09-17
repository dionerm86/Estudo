using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class DepositoChequeDAO : BaseDAO<DepositoCheque, DepositoChequeDAO>
    {
        //private DepositoChequeDAO() { }

        #region Busca Listagem

        private string Sql(uint idDeposito, uint idContaBanco, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "d.*, Concat(c.Nome, ' Agência: ', c.Agencia, ' Conta: ', c.Conta) as DescrContaBanco," +
                "f.Nome as NomeFuncDeposito, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + " From deposito_cheque d " +
                "Left Join conta_banco c On (d.idContaBanco=c.idContaBanco) " +
                "Left Join funcionario f On (d.usuDeposito=f.idFunc) Where 1";

            if (idDeposito > 0)
                sql += " And d.idDeposito=" + idDeposito;

            if (idContaBanco > 0)
            {
                sql += " And d.idContaBanco=" + idContaBanco;
                criterio += "Conta Bancária: " + ContaBancoDAO.Instance.GetElement(idContaBanco).Descricao + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And d.dataDeposito>=?dataIni";
                criterio += "Data Depósito: A partir de " + dataIni;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And d.dataDeposito<=?dataFim";

                if (!String.IsNullOrEmpty(dataIni))
                    criterio += " até " + dataFim;
                else
                    criterio += "Data Depósito: Até " + dataFim;
            }

            return sql.Replace("$$$", criterio);
        }

        public DepositoCheque GetElement(uint idDeposito)
        {
            return objPersistence.LoadOneData(Sql(idDeposito, 0, null, null, true));
        }

        public IList<DepositoCheque> GetForRpt(uint idDeposito, uint idContaBanco, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(idDeposito, idContaBanco, dataIni, dataFim, true) + " Order By d.DataDeposito",
                GetParam(dataIni, dataFim)).ToList();
        }

        public IList<DepositoCheque> GetList(uint idDeposito, uint idContaBanco, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idDeposito, idContaBanco, dataIni, dataFim, true);
            string sort = String.IsNullOrEmpty(sortExpression) ? "d.DataDeposito Desc" : sortExpression;

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, GetParam(dataIni, dataFim));
        }

        public int GetCount(uint idDeposito, uint idContaBanco, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idDeposito, idContaBanco, dataIni, dataFim, false), GetParam(dataIni, dataFim));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Verifica se depósito existe

        public bool DepositoExists(uint idDeposito)
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From deposito_cheque Where idDeposito=" + idDeposito, null) > 0;
        }

        #endregion

        #region Efetua depósito de cheques

        private static readonly object _efetuarDepositoLock = new object();

        /// <summary>
        /// Efetua depósito de cheques
        /// </summary>
        public uint EfetuarDeposito(string idsCheque, uint idContaBanco, DateTime dataDeposito, decimal valorDeposito,
            decimal taxaAntecip, string obs)
        {
            lock (_efetuarDepositoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        LoginUsuario login = UserInfo.GetUserInfo;

                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para efetuar depósito de cheques.");

                        #region Cadastra um novo depósito

                        DepositoCheque deposito = new DepositoCheque();
                        deposito.IdContaBanco = idContaBanco;
                        deposito.DataDeposito = dataDeposito;
                        deposito.Valor = valorDeposito - taxaAntecip;
                        deposito.TaxaAntecip = taxaAntecip;
                        deposito.UsuDeposito = login.CodUser;
                        deposito.Obs = obs;
                        deposito.Situacao = (int)DepositoCheque.SituacaoEnum.Aberto;
                        uint idDeposito = Insert(transaction, deposito);

                        if (idDeposito == 0)
                            throw new Exception("Falha ao Efetuar Depósito. Erro: inserção retornou 0.");

                        #endregion

                        #region Atualiza Cheques com o IdDeposito gerado
                        
                        foreach (var id in idsCheque.TrimEnd(' ').TrimEnd(',').Split(','))
                        {
                            var chequeNovo = ChequesDAO.Instance.GetElementByPrimaryKey(transaction,
                                Conversoes.StrParaUint(id));

                            if (chequeNovo.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                throw new Exception("Um dos cheques deste depósito já foi compensado.");

                            if (chequeNovo.Situacao == (int)Cheques.SituacaoCheque.Cancelado)
                                throw new Exception(
                                    string.Format("O cheque {0} está cancelado, não é possível efetuar o depósito do mesmo.",
                                        chequeNovo.Num));

                            chequeNovo.Situacao = (int)Cheques.SituacaoCheque.Compensado;
                            chequeNovo.IdDeposito = idDeposito;
                            chequeNovo.IdContaBanco = idContaBanco;
                            ChequesDAO.Instance.UpdateBase(transaction, chequeNovo, false);
                        }

                        #endregion

                        // Compensa os cheques.
                        CompensaCheques(transaction, null, idDeposito, ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito),
                            valorDeposito, taxaAntecip, idContaBanco, dataDeposito, 2);

                        transaction.Commit();
                        transaction.Close();

                        return idDeposito;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("Falha ao efetuar depósito.", ex);
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Realiza as operações para compensar um cheque.
        /// </summary>
        internal void CompensaCheques(GDASession session, string idsCheque, uint idDeposito, decimal valorCheques,
            decimal valorTotal, decimal taxaAntecip, uint idContaBanco, DateTime data, int tipoCheque)
        {
            #region Retira valor do Caixa Geral

            uint idCaixaGeral = 0;

            try
            {
                // Faz uma retirada no caixa geral com o valor dos cheques do depósito.
                if (valorCheques > 0 && tipoCheque == 2)
                {
                    if (idDeposito == 0 && !string.IsNullOrEmpty(idsCheque) && idsCheque.IndexOf(",") == -1)
                    {
                        var origemCheque = ChequesDAO.Instance.ObterOrigem(session, idsCheque.StrParaUint());
                        var movimentarCaixaGeral = ChequesDAO.Instance.ObterMovimentarCaixaGeral(session, idsCheque.StrParaUint());

                        /* Chamado 51808.
                         * "origemCheque != (int)Cheques.OrigemCheque.FinanceiroPagto": a origem FinanceiroPagto é salva
                         * quando o cheque é cadastrado avulso, ou seja, verifica se não foi cadastrado avulso.
                         * "movimentarCaixaGeral": verifica se a opção "Gerar movimentação no caixa geral" foi marcada
                         * quando o cheque foi cadastrado de forma avulsa. */
                        if (origemCheque != (int)Cheques.OrigemCheque.FinanceiroPagto || movimentarCaixaGeral)
                            idCaixaGeral = CaixaGeralDAO.Instance.MovCxCheque(session, idsCheque.StrParaUint(), null, null, null, null,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques), tipoCheque, valorCheques, 0, null, true, null, null);
                    }
                    else if (idDeposito > 0)
                    {
                        idCaixaGeral = CaixaGeralDAO.Instance.MovCxDeposito(session, idDeposito, null, null, null,
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques), tipoCheque, 0, valorCheques, 0, null, true, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
            }

            #endregion

            #region Efetua a movimentação da taxa de antecipação do depósito

            uint idTxAntecip = 0;

            try
            {
                if (taxaAntecip > 0)
                    idTxAntecip = ContaBancoDAO.Instance.MovContaDeposito(session, idContaBanco,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTaxaAntecipDepositoCheque), (int)UserInfo.GetUserInfo.IdLoja, idDeposito, tipoCheque,
                        taxaAntecip, data);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
            }

            #endregion

            #region Movimenta Conta Bancária

            uint idMovBanco = 0;

            try
            {
                bool movDeposito = idDeposito > 0 || idsCheque.TrimEnd(',').Split(',').Length > 1;
                idMovBanco = movDeposito ?
                    ContaBancoDAO.Instance.MovContaDeposito(session, idContaBanco, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque), (int)UserInfo.GetUserInfo.IdLoja, idDeposito,
                        (tipoCheque == 1 ? 2 : 1), valorTotal, data) :
                    ContaBancoDAO.Instance.MovContaCheque(session, idContaBanco, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque), (int)UserInfo.GetUserInfo.IdLoja, null,
                        Glass.Conversoes.StrParaUint(idsCheque), null, null, (tipoCheque == 1 ? 2 : 1), valorTotal, data);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
            }

            #endregion
        }

        /// <summary>
        /// Realiza as operações para cancelar a compensação um cheque.
        /// </summary>
        internal void CancelarCompensarChequesReapresentados(GDASession session, Cheques cheque, DateTime dataReapresentacao)
        {
            #region Retorna valor ao Caixa Geral
            try
            {
                // Gera a movimentação de entrada no caixa geral somente se o cheque possuir movimentação no caixa geral e se for cheque de terceiros.
                if (cheque.Tipo == 2 && !cheque.NaoMovCxGeral)
                    // Gera uma movimentação de entrada no caixa geral
                    CaixaGeralDAO.Instance.MovCxCheque(session, cheque.IdCheque, null, null, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques),
                        1, cheque.Valor, 0, null, true, "Cancelamento de reapresentação", null);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar reapresentação.", ex));
            }
            #endregion

            #region Apaga a movimentação bancária

            try
            {
                //Pega a ultima movimentação bancaria do cheque em questão
                var m = MovBancoDAO.Instance.GetByCheque(session, cheque.IdCheque).OrderByDescending(f => f.IdMovBanco).FirstOrDefault();

                //verifica se a movimentação não possui depósito tem determinado plano de contas e se foi feita no mesmo dia da reapresentação
                if (m.IdDeposito.GetValueOrDefault() == 0 &&
                    m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque) && m.DataCad.Date == dataReapresentacao.Date)
                {
                    ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, m.IdMovBanco);

                    objPersistence.ExecuteCommand(session, "update mov_banco set valorMov=0 where idMovBanco in (" + m.IdMovBanco + ")");

                    MovBancoDAO.Instance.CorrigeSaldo(session, m.IdMovBanco, m.IdMovBanco);

                    objPersistence.ExecuteCommand(session, "delete from mov_banco where idMovBanco in (" + m.IdMovBanco + ")");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar reapresentação.", ex));
            }

            #endregion
        }

        #endregion

        #region Retifica Depósito

        private static readonly object _retificarDepositoLock = new object();

        public void RetificarDeposito(uint idDeposito, string idsChequesNovos, uint idContaBanco, DateTime dataDeposito,
            decimal valorDeposito, decimal taxaAntecip)
        {
            lock(_retificarDepositoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        LoginUsuario login = UserInfo.GetUserInfo;

                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para efetuar depósito de cheques.");

                        var deposito = GetElementByPrimaryKey(transaction, idDeposito);

                        /* Chamado 57119. */
                        valorDeposito += ChequesDAO.Instance.ObterValorChequesDevolvidosDeposito(transaction, idDeposito);

                        // Busca os ids dos cheques que estão relacionados à este depósito
                        var lstChequesAntigos = ChequesDAO.Instance.GetByDeposito(transaction, idDeposito);
                        var idsChequesAntigos = string.Empty;

                        foreach (var c in lstChequesAntigos)
                            idsChequesAntigos += c.IdCheque + ",";

                        idsChequesAntigos = idsChequesAntigos.TrimEnd(',');

                        #region Estorna valor do depósito no Caixa Geral
                        
                        try
                        {
                            // Faz uma retirada no caixa geral com o valor dos cheques do depósito
                            CaixaGeralDAO.Instance.MovCxDeposito(transaction, idDeposito, null, null, null,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDepositoCheque), 1, 0,
                                ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito), 0, null, true, null, null);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao estornar valor no Caixa Geral.", ex));
                        }

                        #endregion

                        #region Estorna a movimentação da taxa de antecipação do depósito
                        
                        try
                        {
                            if (deposito.TaxaAntecip > 0)
                                ContaBancoDAO.Instance.MovContaDeposito(transaction, idContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoTaxaAntecipDepositoCheque), (int)UserInfo.GetUserInfo.IdLoja,
                                    idDeposito, 1, deposito.TaxaAntecip, deposito.DataDeposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
                        }

                        #endregion

                        #region Volta situação dos cheques deste depósito para Em Aberto

                        try
                        {
                            /* Retira a referência deste depósito dos cheques associados ao mesmo
                             * Volta a situação dos cheques para em aberto, a menos que esteja cancelado. */
                            foreach (var id in idsChequesAntigos.TrimEnd(' ').TrimEnd(',').Split(','))
                            {
                                var chequeAntigo = ChequesDAO.Instance.GetElementByPrimaryKey(transaction, id.StrParaInt());

                                if (chequeAntigo.Situacao != (int)Cheques.SituacaoCheque.Cancelado)
                                    chequeAntigo.Situacao = (int)Cheques.SituacaoCheque.EmAberto;

                                chequeAntigo.IdDeposito = null;

                                /* Chamado 62308. */
                                chequeAntigo.IdContaBanco = null;

                                ChequesDAO.Instance.UpdateBase(transaction, chequeAntigo, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao voltar situação dos Cheques para Em Aberto.", ex));
                        }

                        #endregion

                        #region Estorna valor do depósito na Conta Bancária que foi feito

                        try
                        {
                            // Pega a movimentação anterior da conta bancária foi feita esta movimentação apenas para alterar o saldo
                            object obj = objPersistence.ExecuteScalar(transaction,
                                string.Format("Select idMovBanco from mov_banco Where idDeposito={0} AND IdConta<>{1} order by dataMov asc limit 1", idDeposito,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido)));
                            uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                            // Verifica a conciliação bancária
                            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idMovBanco);

                            MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idMovBanco);

                            // Corrige saldo
                            objPersistence.ExecuteCommand(transaction,
                                string.Format("Update mov_banco Set valorMov=0 Where idDeposito={0} AND IdConta<>{1}", idDeposito,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido)));
                            if (movAnterior != null)
                                MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idMovBanco);

                            // Exclui movimentações geradas por esta função
                            objPersistence.ExecuteCommand(transaction,
                                string.Format("DELETE FROM mov_banco WHERE IdDeposito={0} AND IdConta<>{1}", idDeposito,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido)));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao estornar depósito na Conta Bancária.", ex));
                        }

                        #endregion

                        #region Marca Cheques passados como compensado e com o IdDeposito passado

                        try
                        {
                            foreach (var id in idsChequesNovos.TrimEnd(' ').TrimEnd(',').Split(','))
                            {
                                var chequeNovo = ChequesDAO.Instance.GetElementByPrimaryKey(transaction, Conversoes.StrParaUint(id));

                                if (chequeNovo.Situacao == (int)Cheques.SituacaoCheque.Cancelado)
                                    throw new Exception(
                                        string.Format("O cheque {0} está cancelado, não é possível efetuar o depósito do mesmo.",
                                            chequeNovo.Num));

                                chequeNovo.Situacao = (int)Cheques.SituacaoCheque.Compensado;
                                chequeNovo.IdDeposito = idDeposito;
                                chequeNovo.IdContaBanco = idContaBanco;
                                ChequesDAO.Instance.UpdateBase(transaction, chequeNovo, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar cheques novos.", ex));
                        }

                        #endregion

                        #region Gera a movimentação da taxa de antecipação do depósito retificado
                        
                        try
                        {
                            if (taxaAntecip > 0)
                                ContaBancoDAO.Instance.MovContaDeposito(transaction, idContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTaxaAntecipDepositoCheque), (int)UserInfo.GetUserInfo.IdLoja,
                                    idDeposito, 2, taxaAntecip, dataDeposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
                        }

                        #endregion

                        #region Movimenta Conta Bancária

                        try
                        {
                            ContaBancoDAO.Instance.MovContaDeposito(transaction, idContaBanco,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque), (int)UserInfo.GetUserInfo.IdLoja,
                                idDeposito, 1, valorDeposito + taxaAntecip, dataDeposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao movimentar conta bancária.", ex));
                        }

                        #endregion

                        #region Retira valor do Caixa Geral
                        
                        try
                        {
                            // Faz uma retirada no caixa geral com o valor dos cheques do depósito
                            CaixaGeralDAO.Instance.MovCxDeposito(transaction, idDeposito, null, null, null,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques), 2,
                                0, ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito), 0, null, true, null, null);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Atualizar retirar valor dos cheques no Caixa Geral.", ex));
                        }

                        #endregion

                        #region Atualiza dados do depósito

                        try
                        {
                            deposito.Valor = valorDeposito;
                            deposito.DataDeposito = dataDeposito;
                            deposito.IdContaBanco = idContaBanco;
                            deposito.UsuDeposito = UserInfo.GetUserInfo.CodUser;
                            deposito.TaxaAntecip = taxaAntecip;

                            Update(transaction, deposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Atualizar dados do Depósito.", ex));
                        }

                        #endregion
                        
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

        #region Cancela Depósito
        
        private static readonly object _cancelarDepositoLock = new object();

        public void CancelarDeposito(uint idDeposito, string motivo, DateTime? dataEstorno, bool estornarMovimentacaoBancaria)
        {
            lock(_cancelarDepositoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        LoginUsuario login = UserInfo.GetUserInfo;

                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para efetuar depósito de cheques.");

                        // Não permite cancelar o depósito se o mesmo tiver cheques devolvidos
                        if (ExecuteScalar<bool>(transaction, "Select Count(*)>0 From cheques Where idDepositoCanc=" + idDeposito))
                            throw new Exception("Antes de cancelar este depósito será necessário desfazer as devoluções feitas em alguns dos cheques do mesmo.");

                        DepositoCheque deposito = GetElementByPrimaryKey(transaction, idDeposito);

                        // Busca os ids dos cheques que estão relacionados à este depósito.
                        var idsChequesAntigos = string.Empty;

                        foreach (Cheques c in ChequesDAO.Instance.GetByDeposito(transaction, idDeposito))
                        {
                            // O depósito não pode ter nenhum cheque devolvido
                            if (c.Situacao == (int)Cheques.SituacaoCheque.Devolvido)
                                throw new Exception("Este depósito possui cheques devolvidos ou reapresentados, cancele a devolução antes de cancelar o depósito.");

                            idsChequesAntigos += c.IdCheque + ",";

                            /* Chamado 47204. */
                            deposito.DadosChequesDesassociadosAoCancelar +=
                                string.Format("Número: {0} | Titular: {1} | Cliente: {2} | Conta: {3} | Agência: {4} | Banco: {5} | Valor: {6} | Venc.: {7}\n\n",
                                c.Num, c.Titular, c.IdNomeCliente, c.Conta, c.Agencia, c.Banco, c.Valor, c.DataVenc.GetValueOrDefault().ToString("dd-MM-yyyy"));
                        }

                        idsChequesAntigos = idsChequesAntigos.TrimEnd(',');

                        #region Estorna valor do depósito no Caixa Geral

                        try
                        {
                            // Faz uma retirada no caixa geral com o valor dos cheques do depósito
                            CaixaGeralDAO.Instance.MovCxDeposito(transaction, idDeposito, null, null, null,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDepositoCheque), 1, 0,
                                ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito), 0, null, true, null, dataEstorno);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao estornar valor no Caixa Geral.", ex));
                        }

                        #endregion

                        #region Estorna a movimentação da taxa de antecipação do depósito

                        try
                        {
                            if (deposito.TaxaAntecip > 0)
                                ContaBancoDAO.Instance.MovContaDeposito(transaction, deposito.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoTaxaAntecipDepositoCheque), (int)UserInfo.GetUserInfo.IdLoja,
                                    idDeposito, 1, deposito.TaxaAntecip, dataEstorno.HasValue ? dataEstorno.Value : deposito.DataDeposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Efetuar Depósito.", ex));
                        }

                        #endregion

                        #region Volta situação dos cheques deste depósito para Em Aberto

                        try
                        {
                            if (!string.IsNullOrEmpty(idsChequesAntigos))
                            {
                                // Retira a referência deste depósito dos cheques associados ao mesmo
                                objPersistence.ExecuteCommand(transaction, "Update cheques Set idDeposito=null Where idCheque In (" + idsChequesAntigos + ")");

                                /* Chamado 62308. */
                                // Retira a referência da conta bancária dos cheques.
                                objPersistence.ExecuteCommand(transaction, string.Format("UPDATE cheques SET IdContaBanco=NULL WHERE IdCheque IN ({0})", idsChequesAntigos));

                                // Volta a situação dos cheques para em aberto, a menos que esteja cancelado
                                objPersistence.ExecuteCommand(transaction, "Update cheques ch Set ch.Situacao=" + (int)Cheques.SituacaoCheque.EmAberto +
                                    " Where ch.idCheque In (" + idsChequesAntigos + ") and ch.Situacao Not In (" + (int)Cheques.SituacaoCheque.Cancelado +
                                    "," + (int)Cheques.SituacaoCheque.Protestado + ")");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao voltar situação dos Cheques para Em Aberto.", ex));
                        }

                        #endregion

                        #region Estorna valor do depósito na Conta Bancária que foi feito

                        try
                        {
                            // Exclui movimentações que este depósito gerou
                            if (!estornarMovimentacaoBancaria)
                            {
                                // Pega a primeira movimentação da conta bancária foi feita esta movimentação apenas para alterar o saldo
                                uint idMovBanco = ExecuteScalar<uint>(transaction, "Select idMovBanco from mov_banco Where idDeposito=" + deposito.IdDeposito +
                                    " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Asc, IdMovBanco Asc limit 1");

                                // Verifica a conciliação bancária
                                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idMovBanco);

                                MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idMovBanco);

                                // Corrige saldo
                                objPersistence.ExecuteCommand(transaction, "Update mov_banco Set valorMov=0 Where idDeposito=" + idDeposito);
                                if (movAnterior != null)
                                    MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idMovBanco);

                                // Exclui movimentações geradas por esta função
                                objPersistence.ExecuteCommand(transaction, "Delete From mov_banco Where idDeposito=" + idDeposito);
                            }
                            else // Estorna valores
                            {
                                ContaBancoDAO.Instance.MovContaDeposito(transaction, deposito.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDepositoCheque), (int)UserInfo.GetUserInfo.IdLoja, idDeposito, 2, deposito.Valor + deposito.TaxaAntecip,
                                    dataEstorno.HasValue ? dataEstorno.Value : deposito.DataDeposito);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao estornar depósito na Conta Bancária.", ex));
                        }

                        #endregion

                        #region Retira valor do Caixa Geral

                        if (ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito) > 0)
                        {
                            try
                            {
                                // Faz uma retirada no caixa geral com o valor dos cheques do depósito
                                CaixaGeralDAO.Instance.MovCxDeposito(transaction, idDeposito, null, null, null,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques), 2,
                                    0, ChequesDAO.Instance.GetTotalInDeposito(transaction, idDeposito), 0, null, true, null, dataEstorno);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Atualizar retirar valor dos cheques no Caixa Geral.", ex));
                            }
                        }

                        #endregion

                        #region Atualiza dados do depósito

                        try
                        {
                            deposito.Situacao = (int)DepositoCheque.SituacaoEnum.Cancelado;
                            Update(transaction, deposito);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao Atualizar dados do Depósito.", ex));
                        }

                        #endregion

                        //Salva o log do cancelamento
                        LogCancelamentoDAO.Instance.LogDepostioCheque(transaction, deposito, motivo);

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

        #region Cheque Devolvido/Protestado

        private static readonly object _devolverChequeLock = new object();

        /// <summary>
        /// Marca o cheque passado como devolvido e estorna valor na conta bancária que foi depositado
        /// </summary>
        private void DevolverCheque(uint idCheque, Cheques.SituacaoCheque novaSituacao, string obs, DateTime dataEstorno)
        {
            lock (_devolverChequeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                            throw new Exception("Você não tem permissão para marcar cheque como " + novaSituacao + ".");

                        Cheques cheque = ChequesDAO.Instance.GetElement(transaction, idCheque);
                        uint idMovBanco = 0;
                        uint idCaixaGeral = 0;
                        int situacao = cheque.Situacao;
                        uint? idDeposito = cheque.IdDeposito;

                        if (cheque.Situacao == (int)novaSituacao && !cheque.Reapresentado)
                            throw new Exception("Este cheque já está marcado como " + novaSituacao + ".");

                        /* Chamado 37210. */
                        if (novaSituacao == Cheques.SituacaoCheque.Devolvido &&
                            cheque.Situacao != (int)Cheques.SituacaoCheque.Compensado && !cheque.Reapresentado)
                            throw new Exception(string.Format("Somente cheques Compensados ou Reapresentados podem ser marcados como {0}.", novaSituacao));

                        string motivo = !String.IsNullOrEmpty(cheque.Obs) ? cheque.Obs + ". " + obs : obs;
                        if (motivo.Length > 1000)
                            throw new Exception("O motivo da devolução juntamente com a observação já inserida no cheque ultrapasa o tamanho máximo do campo." +
                                " Tamanho inserido: " + motivo.Length + " caracteres, tamanho máximo do campo: 1000 caracteres.");

                        //if (novaSituacao == Cheques.SituacaoCheque.Devolvido && cheque.IdCreditoFornecedor.GetValueOrDefault(0) > 0)
                        //    throw new Exception("Não é possível marcar esse cheque como devolvido, pois o mesmo está vinculado ao crédito do fornecedor: " +
                        //        cheque.IdCreditoFornecedor.Value + ". Cancele o crédito antes de fazer a devolução.");

                        // Se estiver marcando um cheque devolvido como protestado, apenas troca a situação
                        if (cheque.Situacao == (int)Cheques.SituacaoCheque.Devolvido &&
                            novaSituacao == Cheques.SituacaoCheque.Protestado)
                        {
                            cheque.Situacao = (int)novaSituacao;
                            // Chama o método UpdateBase para que não verifique se o cheque foi cadastrado
                            ChequesDAO.Instance.UpdateBase(transaction, cheque, false);
                            
                            transaction.Commit();
                            transaction.Close();

                            return;
                        }

                        // Verifica se cheque possui idContaBanco para estornar valor na conta bancária
                        if (cheque.IdContaBanco > 0)
                        {
                            // Se o depósito tiver sido feito com taxa de antecipação, calcula o valor do cheque menos a taxa de antecipacao
                            decimal valorAEstornar = cheque.Valor;

                            // TODO: verificar se o calculo da taxa de antecipacao esta correto

                            // Estorna valor depositado na conta bancária
                            idMovBanco = ContaBancoDAO.Instance.MovContaCheque(transaction, cheque.IdContaBanco.Value,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido), (int)UserInfo.GetUserInfo.IdLoja,
                                cheque.IdDeposito, idCheque, null, null, cheque.Tipo, valorAEstornar, dataEstorno);
                        }
                        else if (cheque.MovBanco)
                        {
                            if (cheque.IdContaBanco == null || cheque.IdContaBanco == 0)
                                throw new Exception("O cheque não possui referência à sua conta bancária.");

                            // Estorna valor depositado na conta bancária
                            idMovBanco = ContaBancoDAO.Instance.MovContaCheque(transaction, cheque.IdContaBanco.Value,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido), (int)UserInfo.GetUserInfo.IdLoja,
                                null, idCheque, null, null, cheque.Tipo, cheque.Valor, dataEstorno);
                        }

                        // Altera situação do Cheque para Devolvido, e exclui seu idDeposito, caso tenha sido reapresentado e devolvido novamente,
                        // para não perder o idDepositoCanc, só preenche o mesmo se o cheque possuir idDeposito, caso contrário permanece o idDepositoCanc
                        // que já estava associado ao mesmo.
                        cheque.Situacao = (int)novaSituacao;
                        cheque.CancelouDevolucao = false;
                        cheque.IdDepositoCanc = cheque.IdDeposito > 0 ? cheque.IdDeposito : cheque.IdDepositoCanc;
                        cheque.IdDeposito = null;
                        cheque.Obs = !String.IsNullOrEmpty(cheque.Obs) ? cheque.Obs + ". " + obs : obs;

                        // Chama o método UpdateBase para que não verifique se o cheque foi cadastrado
                        ChequesDAO.Instance.UpdateBase(transaction, cheque, false);

                        // Desmarca o cheque como reapresentado 
                        objPersistence.ExecuteCommand(transaction, "Update cheques Set reapresentado=false Where idCheque=" + cheque.IdCheque);

                        // Gera a movimentação de entrada no caixa geral somente se o cheque possuir movimentação no caixa geral e se for cheque de terceiros.
                        if (cheque.Tipo == 2 && !cheque.NaoMovCxGeral)
                        {
                            // Gera uma movimentação de entrada no caixa geral
                            idCaixaGeral = CaixaGeralDAO.Instance.MovCxCheque(transaction, cheque.IdCheque, null, null, 0,
                                null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido),
                                1, cheque.Valor, 0, null, true, null, dataEstorno);
                            /* Chamado 23202. */
                        }

                        // Observação que será usada no cheque e para validar se já foi gerada uma conta a pagar para a devolução desse cheque
                        var obsContaPagar = "Num. Cheque Dev.: " + cheque.Num;

                        #region Cria aonta a pagar a partir do pagamento associado ao cheque

                        // Gera uma conta a pagar caso o cheque tenha sido usado em um pagto, desde que já não tenha sido gerada 
                        // (por ter devolvido o cheque anteriormente)
                        if (cheque.Tipo == 2 && cheque.IdPagto > 0 &&
                            //Chamado #48681, é necessario apagar o segunda verificação do contas a pagar posteriormente pois, em contas a pagar nao existia o idcheque,
                            //E o idpagto pode ser diferente invalidando o segundo sql
                            !ExecuteScalar<bool>(transaction, "Select Count(*)>0 From contas_pagar Where IdCheque=?idCheque", new GDAParameter("?idCheque", cheque.IdCheque)) &&
                            !ExecuteScalar<bool>(transaction,
                                "Select Count(*)>0 From contas_pagar Where idPagtoRestante=?idPagto and idPagtoRestante=idPagto and idConta=?idConta and valorVenc=?valor and obs like ?obs",
                                new GDAParameter("?idPagto", cheque.IdPagto),
                                new GDAParameter("?idConta",
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto)),
                                new GDAParameter("?valor", cheque.Valor),
                                new GDAParameter("?obs", string.Format("{0}%", obsContaPagar))))
                        {
                            // Gera uma nova conta a pagar para o cheque
                            ContasPagar cp = new ContasPagar();
                            cp.IdPagto = cheque.IdPagto;
                            cp.IdPagtoRestante = cheque.IdPagto;
                            cp.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                            cp.Paga = false;
                            cp.DataVenc = DateTime.Now;
                            cp.ValorVenc = cheque.Valor;
                            cp.IdFornec = PagtoDAO.Instance.ObtemValorCampo<uint?>(transaction, "idFornec",
                                "idPagto=" + cheque.IdPagto);
                            cp.Obs = obsContaPagar;
                            cp.IdCheque = (int)cheque.IdCheque;

                            ContasPagarDAO.Instance.Insert(transaction, cp);
                            ContasPagarDAO.Instance.AtualizaNumParcPagto(transaction, cheque.IdPagto.Value);
                        }

                        #endregion

                        #region Cria conta a pagar a partir do crédito de fornecedor associado ao cheque

                        // Gera uma conta a pagar caso o cheque tenha sido usado em um crédito de fornecedor, desde que já não tenha sido gerada 
                        // (por ter devolvido o cheque anteriormente)
                        else if (cheque.Tipo == 2 && cheque.IdCreditoFornecedor > 0 &&
                            !ExecuteScalar<bool>(transaction, "Select Count(*)>0 From contas_pagar Where idConta=?idConta and valorVenc=?valor and idCreditoFornecedor=?idCreditoFornecedor and obs like ?obs",
                            new GDAParameter("?idCreditoFornecedor", cheque.IdCreditoFornecedor),
                            new GDAParameter("?idConta", UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto)),
                            new GDAParameter("?valor", cheque.Valor),
                            new GDAParameter("?obs", string.Format("{0}%", obsContaPagar))))
                        {
                            // Gera uma nova conta a pagar para o cheque
                            ContasPagar cp = new ContasPagar();
                            cp.IdCreditoFornecedor = cheque.IdCreditoFornecedor;
                            cp.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                            cp.Paga = false;
                            cp.DataVenc = DateTime.Now;
                            cp.ValorVenc = cheque.Valor;
                            cp.IdFornec = CreditoFornecedorDAO.Instance.ObtemValorCampo<uint?>(transaction, "idFornec", "idCreditoFornecedor=" + cheque.IdCreditoFornecedor);
                            cp.Obs = obsContaPagar;
                            cp.IdCheque = (int)cheque.IdCheque;

                            ContasPagarDAO.Instance.Insert(transaction, cp);
                            ContasPagarDAO.Instance.AtualizaNumParcCreditoFornecedor(transaction, cheque.IdCreditoFornecedor.Value);
                        }

                        #endregion
                        
                        #region Cria conta a pagar a partir do sinal de compra associado ao cheque

                        // Gera uma conta a pagar caso o cheque tenha sido usado em um crédito de fornecedor, desde que já não tenha sido gerada 
                        // (por ter devolvido o cheque anteriormente)
                        else if (cheque.Tipo == 2 && cheque.IdSinalCompra > 0 &&
                            !ExecuteScalar<bool>(transaction, "SELECT COUNT(*)>0 FROM contas_pagar WHERE IdConta=?idConta AND ValorVenc=?valor AND IdSinalCompra=?idSinalCompra AND Obs LIKE ?obs",
                            new GDAParameter("?idSinalCompra", cheque.IdSinalCompra),
                            new GDAParameter("?idConta", UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto)),
                            new GDAParameter("?valor", cheque.Valor),
                            new GDAParameter("?obs", string.Format("{0}%", obsContaPagar))))
                        {
                            // Gera uma nova conta a pagar para o cheque.
                            var cp = new ContasPagar();
                            cp.IdSinalCompra = cheque.IdSinalCompra;
                            cp.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                            cp.Paga = false;
                            cp.DataVenc = DateTime.Now;
                            cp.ValorVenc = cheque.Valor;
                            cp.IdFornec = SinalCompraDAO.Instance.ObtemIdFornec(transaction, cheque.IdSinalCompra.Value);
                            cp.Obs = obsContaPagar;
                            cp.IdCheque = (int)cheque.IdCheque;

                            ContasPagarDAO.Instance.Insert(transaction, cp);
                            ContasPagarDAO.Instance.AtualizaNumParcSinalCompra(transaction, cheque.IdSinalCompra.Value);
                        }

                        #endregion

                        // Verifica se a empresa inativa o cliente ao devolver cheque
                        if (FinanceiroConfig.FinanceiroRec.BloquearClienteAoDevolverProtestarCheque && cheque.IdCliente > 0)
                        {
                            // Precisa ser GetElement para que seja recuperada a rota do cliente
                            Cliente cliente = ClienteDAO.Instance.GetElement(transaction, cheque.IdCliente.Value);
                            cliente.Situacao = (int)SituacaoCliente.Bloqueado;
                            cliente.IdRota = (int)ClienteDAO.Instance.ObtemIdRota(transaction, (uint)cliente.IdCli);

                            if (!cliente.Obs.Contains(" Cliente bloqueado por ter cheque devolvido/protestado."))
                                cliente.Obs += " Cliente bloqueado por ter cheque devolvido/protestado.";
                            
                            LogAlteracaoDAO.Instance.LogCliente(transaction, cliente);

                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE cliente SET Situacao={0}, Obs=?obs WHERE Id_Cli={1};", cliente.Situacao, cliente.IdCli),
                                new GDAParameter("?obs", cliente.Obs));
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("Devolução de cheque", ex);
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Marca o cheque passado como devolvido e estorna valor na conta bancária que foi depositado
        /// </summary>
        /// <param name="idCheque"></param>
        public void ChequeDevolvido(uint idCheque, string obs, DateTime dataEstorno)
        {
            DevolverCheque(idCheque, Cheques.SituacaoCheque.Devolvido, obs, dataEstorno);
        }

        /// <summary>
        /// Marca o cheque passado como protestado e estorna valor na conta bancária que foi depositado
        /// </summary>
        /// <param name="idCheque"></param>
        public void ChequeProtestado(uint idCheque, string obs)
        {
            DevolverCheque(idCheque, Cheques.SituacaoCheque.Protestado, obs, DateTime.Now);
        }

        #endregion

        #region Cancela devolução de cheque

        public void CancelarDevolucao(GDASession sessao, uint idCheque)
        {
            uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                throw new Exception("Você não tem permissão para cancelar devolução do cheque.");

            Cheques cheque = ChequesDAO.Instance.GetElement(sessao, idCheque);

            if (cheque.Situacao != (int)Cheques.SituacaoCheque.Devolvido || cheque.Reapresentado)
                throw new Exception("Este cheque não está marcado como devolvido.");

            List<MovBanco> movs = new List<MovBanco>(MovBancoDAO.Instance.GetByCheque(sessao, idCheque));
            Dictionary<uint, decimal> movBanco = new Dictionary<uint, decimal>();
            string idsMovBanco = "";
            uint idCaixaGeral = 0;

            // Gera a movimentação de entrada no caixa geral somente se o cheque possuir movimentação no caixa geral e se for cheque de terceiros.
            if (cheque.Tipo == 2 && !cheque.NaoMovCxGeral)
                // Gera uma movimentação de entrada no caixa geral
                idCaixaGeral = CaixaGeralDAO.Instance.MovCxCheque(sessao, cheque.IdCheque, null, null, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque),
                    2, cheque.Valor, 0, null, true, "Cancelamento de devolução", null);

            // Exclui a conta a pagar gerada pelo cheque.
            if (cheque.Tipo == 2 && (cheque.IdPagto > 0 || cheque.IdCreditoFornecedor > 0 || cheque.IdSinalCompra > 0))
            {
                // Cancela a conta a pagar do cheque, se houver
                var cp = ContasPagarDAO.Instance.GetByChequeDev(sessao, (int)idCheque, (int?)cheque.IdPagto, (int?)cheque.IdCreditoFornecedor, (int?)cheque.IdSinalCompra);
                if (cp != null)
                {
                    if (cp.Paga)
                        throw new Exception("Conta associada ao cheque já foi paga. Cancele o pagamento antes de cancelar a devolução do cheque.");

                    ContasPagarDAO.Instance.DeleteByPrimaryKey(sessao, cp.IdContaPg);
                }
            }

            foreach (var m in movs)
                // Chamado 29320: Ao cancelar devolução, todas as entradas e saídas devem ser excluídas (exceto a do depósito), principalmente entrada feita ao reapresentar cheque
                //Chamado: 55138 - Ao cancelar a devolução a movimentação da entrada estava sendo apagada.
                if (!(m.TipoMov == 1 && cheque.Origem == (int)Cheques.OrigemCheque.FinanceiroPagto) && (m.IdDeposito.GetValueOrDefault() == 0 || m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido)))
                {
                    movBanco.Add(m.IdMovBanco, m.ValorMov);
                    idsMovBanco += m.IdMovBanco + ",";
                }

            if (movBanco.Count > 0)
            {
                // Verifica a conciliação bancária
                foreach (string id in idsMovBanco.TrimEnd(',').Split(','))
                    ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(sessao, Glass.Conversoes.StrParaUint(id));

                objPersistence.ExecuteCommand(sessao, "update mov_banco set valorMov=0 where idMovBanco in (" + idsMovBanco.TrimEnd(',') + ")");

                foreach (var idMovBanco in idsMovBanco.Split(',').Select(f => f.StrParaInt()))
                    MovBancoDAO.Instance.CorrigeSaldo(sessao, (uint)idMovBanco, (uint)idMovBanco);

                objPersistence.ExecuteCommand(sessao, "delete from mov_banco where idMovBanco in (" + idsMovBanco.TrimEnd(',') + ")");
            }

            // Volta situação do Cheque para Compensado
            cheque.Situacao = cheque.IdPagto > 0 || cheque.IdDeposito > 0 || cheque.IdDepositoCanc > 0 ? (int)Cheques.SituacaoCheque.Compensado : (int)Cheques.SituacaoCheque.EmAberto;
            cheque.IdDeposito = cheque.IdDepositoCanc;
            cheque.IdDepositoCanc = null;
            cheque.CancelouDevolucao = true;
            // Como a devolução do cheque foi cancelada o mesmo voltou para compensado, sendo assim a variável Advogado deve ser atualizada
            // para falsa pois o cheque não está com o advogado.
            cheque.Advogado = false;
            cheque.Obs = String.IsNullOrEmpty(cheque.Obs) || !cheque.Obs.Contains("Motivo da Devolução: ") ? cheque.Obs :
                cheque.Obs.Remove(cheque.Obs.IndexOf("Motivo da Devolução: "));

            // Chama o método UpdateBase para que não verifique se o cheque foi cadastrado
            ChequesDAO.Instance.UpdateBase(sessao, cheque, false);
        }

        #endregion
    }
}