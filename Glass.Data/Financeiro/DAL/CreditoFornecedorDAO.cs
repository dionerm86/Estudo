using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CreditoFornecedorDAO : BaseDAO<CreditoFornecedor, CreditoFornecedorDAO>
    {
        //private CreditoFornecedorDAO() { }

        private string Sql(uint? idCreditoFornecedor, uint? idFornecedor, DateTime? dtIni, DateTime? dtFim, bool selecionar)
        {
            string campos = selecionar ? "cf.*, fn.NomeFantasia as NomeFornecedor, f.Nome as NomeUsuarioCad, '$$$' as Criterio" : "Count(*)";
            string sql = "select " + campos + " from credito_fornecedor cf " +
                "left join fornecedor fn on (cf.IdFornec=fn.IdFornec) " +
                "left join funcionario f on (cf.UsuCad=f.IdFunc) where 1";
            string criterio = string.Empty;

            if (idCreditoFornecedor > 0)
            {
                sql += " And cf.idCreditoFornecedor=" + idCreditoFornecedor;
            }

            if (idFornecedor > 0)
            {
                sql += " and cf.IdFornec=" + idFornecedor.Value.ToString();
            }

            if (dtIni != null)
            {
                sql += " and cf.DataCad>='" + dtIni.Value.ToString("yyyy-MM-dd 00:00") + "'";
                criterio += "A partir de " + dtIni.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (dtFim != null)
            {
                sql += " and cf.DataCad<='" + dtFim.Value.ToString("yyyy-MM-dd 23:59") + "'";
                criterio += "Até " + dtFim.Value.ToString("yyyy-MM-dd") + "     ";
            }
 
            return sql.Replace("$$$", criterio);
        }

        public IList<CreditoFornecedor> GetList(uint? idCreditoFornec, uint? idFornecedor, DateTime? dtIni, DateTime? dtFim, string sortExpression, int startRow, int pageSize)
        {
            string order = String.IsNullOrEmpty(sortExpression) ? "cf.idCreditoFornecedor desc" : sortExpression;
            var lista =  LoadDataWithSortExpression(Sql(idCreditoFornec, idFornecedor, dtIni, dtFim, true), order, startRow, pageSize, null);

            return lista;
        }

        public int GetListCount(uint? idCreditoFornec, uint? idFornecedor, DateTime? dtIni, DateTime? dtFim)
        {
            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(Sql(idCreditoFornec, idFornecedor, dtIni, dtFim, false)).ToString());
        }

        public CreditoFornecedor ObterCreditoFornecedor(uint idCreditoFornecedor)
        {
            return ObterCreditoFornecedor(null, idCreditoFornecedor);
        }

        public CreditoFornecedor ObterCreditoFornecedor(GDASession session, uint idCreditoFornecedor)
        {
            string sql = Sql(idCreditoFornecedor, 0, null, null, true);
            CreditoFornecedor cred = objPersistence.LoadOneData(session, sql);

            cred.Pagamentos = PagtoCreditoFornecedorDAO.Instance.GetPagamentos(session, idCreditoFornecedor);

            for (int i = 0; i < cred.Pagamentos.Length; i++)
            {
                cred.ContasBancoPagto[i] = cred.Pagamentos[i].IdContaBanco == null ? 0 : (uint)cred.Pagamentos[i].IdContaBanco;
                cred.TiposCartaoPagto[i] = cred.Pagamentos[i].IdTipoCartao == null ? 0 : (uint)cred.Pagamentos[i].IdTipoCartao;
                cred.ValoresPagto[i] = cred.Pagamentos[i].ValorPagto;
                cred.FormasPagto[i] = cred.Pagamentos[i].IdFormaPagto == null ? 0 : (uint)cred.Pagamentos[i].IdFormaPagto;
            }

            return cred;
        }

        public IList<CreditoFornecedor> ObterListaCreditoFornecedor(uint? idCreditoFornec, uint? idFornec, DateTime? dtIni, DateTime? dtFim)
        {
            string sql = Sql(idCreditoFornec, idFornec, dtIni, dtFim, true);

            var creds = objPersistence.LoadData(sql).ToList();

            foreach (CreditoFornecedor cred in creds)
            {
                cred.Pagamentos = PagtoCreditoFornecedorDAO.Instance.GetPagamentos(cred.IdCreditoFornecedor);
            }

            return creds;
        }

        private static readonly object _insertLock = new object();

        public override uint Insert(CreditoFornecedor objInsert)
        {
            lock(_insertLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();
                        
                        var lstChequesInseridos = new List<Cheques>();
                        // Guarda os ids dos cheques de terceiros
                        var idsChequeTerc = string.Empty;

                        objInsert.DataCad = DateTime.Now;
                        objInsert.UsuCad = UserInfo.GetUserInfo.CodUser;
                        objInsert.Situacao = (uint)CreditoFornecedor.SituacaoCredito.Ativo;
                        objInsert.IdCreditoFornecedor = base.Insert(transaction, objInsert);                        

                        uint numPagto = 0;
                        var contadorDataUnica = 0;
                        decimal valor = 0;

                        for (int i = 0; i < objInsert.FormasPagto.Length; i++)
                        {
                            if (objInsert.ValoresPagto[i] > 0)
                            {
                                PagtoCreditoFornecedor pagto = new PagtoCreditoFornecedor();
                                pagto.IdContaBanco = objInsert.ContasBancoPagto[i] == 0 ? null : (uint?)objInsert.ContasBancoPagto[i];
                                pagto.IdCreditoFornecedor = objInsert.IdCreditoFornecedor;
                                pagto.IdFormaPagto = objInsert.FormasPagto[i] == 0 ? null : (uint?)objInsert.FormasPagto[i];
                                pagto.IdTipoCartao = objInsert.TiposCartaoPagto[i] == 0 ? null : (uint?)objInsert.TiposCartaoPagto[i];
                                pagto.NumFormaPagto = ++numPagto;
                                pagto.ValorPagto = objInsert.ValoresPagto[i];

                                PagtoCreditoFornecedorDAO.Instance.Insert(transaction, pagto);

                                if (objInsert.IdFornecedor == 0)
                                    throw new Exception("Fornecedor não informado para inserção de crédito do mesmo");

                                FornecedorDAO.Instance.CreditaCredito(transaction, (uint)objInsert.IdFornecedor, pagto.ValorPagto);

                                #region Dinheiro

                                // Se a forma de pagto for Dinheiro, gera movimentação no caixa geral
                                if (objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                                    CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                        UtilsPlanoConta.GetPlanoContaCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro), 2, objInsert.ValoresPagto[i], 1, true,
                                        objInsert.DatasPagto[i], null);

                                #endregion

                                #region Cheques

                                // Se a forma de pagamento for cheques próprios, cadastra cheques, associa os mesmos com as contas
                                // que estão sendo pagas, debita valor da conta que foi escolhida em cada cheque
                                else if ((objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro))
                                {
                                    Cheques cheque;

                                    // Separa os cheques guardando-os em um vetor
                                    string[] vetCheque = objInsert.ChequesPagto.TrimEnd(' ').TrimEnd('|').Split('|');

                                    if (objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                                    {
                                        #region Associa cheques ao pagamento e Gera movimentações no caixa geral

                                        try
                                        {                                           
                                            // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                                            foreach (var c in vetCheque)
                                            {
                                                // Divide o cheque para pegar suas propriedades
                                                string[] dadosCheque = c.Split('\t');

                                                if (dadosCheque[0] == "proprio") // Se for cheque próprio
                                                {
                                                    // Insere cheque no BD                                                    
                                                    cheque = ChequesDAO.Instance.GetFromString(transaction, c);
                                                    cheque.IdCreditoFornecedor = objInsert.IdCreditoFornecedor;
                                                    cheque.JurosPagto = 0;
                                                    cheque.MultaPagto = 0;
                                                    if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                                        cheque.DataReceb = objInsert.DataCad;
                                                    cheque.DataCad = DateTime.Now;
                                                    cheque.IdFornecedor = objInsert.IdFornecedor;
                                                    cheque.IdCheque = ChequesDAO.Instance.InsertBase(transaction, cheque);                                                    

                                                    if (cheque.IdCheque < 1)
                                                        throw new Exception("retorno do insert do cheque=0");

                                                    // Adiciona este cheque à lista de cheques inseridos
                                                    lstChequesInseridos.Add(cheque);

                                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                                         UtilsPlanoConta.GetPlanoContaCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio), 2,
                                                        cheque.Valor, 2, true, objInsert.DatasPagto[i], null);

                                                    objPersistence.ExecuteCommand(transaction,
                                                        string.Format(
                                                            "UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                                                            contadorDataUnica++, idCaixaGeral));
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar cheques ao pagamento.", ex));
                                        }

                                        #endregion

                                        #region Associa cheques inseridos ao pagamento e Gera Movimentação Bancária

                                        try
                                        {
                                            // Para cada cheque "Compensado" utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                                            foreach (Cheques c in lstChequesInseridos)
                                                if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                                {
                                                    ContaBancoDAO.Instance.MovContaChequeCreditoFornecedor(transaction, c.IdContaBanco.Value,
                                                        UtilsPlanoConta.GetPlanoContaCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio), (int)UserInfo.GetUserInfo.IdLoja,
                                                        c.IdCheque, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor, 2, c.Valor, 0, objInsert.DataCad);

                                                    // Gera movimentação de juros
                                                    if (c.JurosPagto > 0)
                                                        ContaBancoDAO.Instance.MovContaChequeCreditoFornecedor(transaction, c.IdContaBanco.Value, FinanceiroConfig.PlanoContaJurosPagto, (int)UserInfo.GetUserInfo.IdLoja,
                                                            c.IdCheque, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor, 2, c.JurosPagto, 0, objInsert.DataCad);

                                                    // Gera movimentação de multa
                                                    if (c.MultaPagto > 0)
                                                        ContaBancoDAO.Instance.MovContaChequeCreditoFornecedor(transaction, c.IdContaBanco.Value,
                                                            FinanceiroConfig.PlanoContaMultaPagto, (int)UserInfo.GetUserInfo.IdLoja,
                                                            c.IdCheque, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor, 2, c.MultaPagto, 0, objInsert.DataCad);
                                                }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar cheques às contas a pagar", ex));
                                        }

                                        #endregion
                                    }
                                    // Se a forma de pagamento for cheques de terceiros
                                    else if (objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                                    {
                                        #region Associa cheques ao pagamento e Gera movimentações no caixa geral

                                        try
                                        {
                                            // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                                            foreach (string c in vetCheque)
                                            {
                                                // Divide o cheque para pegar suas propriedades
                                                string[] dadosCheque = c.Split('\t');

                                                if (dadosCheque[0] == "terceiro") // Se for cheque de terceiro
                                                {
                                                    // Associa cada cheque utilizado no pagto à cada conta paga
                                                    uint idCheque = Glass.Conversoes.StrParaUint(dadosCheque[18]);
                                                    idsChequeTerc += dadosCheque[18] + ",";

                                                    // Coloca juros e multa gerados no cheque
                                                    objPersistence.ExecuteCommand(transaction, @"update cheques set jurosPagto=?juros, multaPagto=?multa,
                                                        idCreditoFornecedor=?id where idCheque=" + idCheque, new GDAParameter("?juros", 0),
                                                        new GDAParameter("?multa", 0), new GDAParameter("?id", objInsert.IdCreditoFornecedor));

                                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                                        UtilsPlanoConta.GetPlanoContaCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro), 2,
                                                        ChequesDAO.Instance.ObtemValor(transaction, idCheque), 2, true, objInsert.DatasPagto[i], null);

                                                    objPersistence.ExecuteCommand(transaction,
                                                        string.Format(
                                                            "UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                                                            contadorDataUnica++, idCaixaGeral));
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar cheques ao pagamento.", ex));
                                        }

                                        #endregion

                                        #region Altera situação dos cheques para compensado e gera movimentação se houver adicional

                                        try
                                        {
                                            // Marca cheques de terceiros utilizados no pagamento como compensados
                                            ChequesDAO.Instance.UpdateSituacao(transaction, idsChequeTerc.TrimEnd(','), Cheques.SituacaoCheque.Compensado);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao marcar cheques como compensados.", ex));
                                            ;
                                        }

                                        #endregion
                                    }
                                }

                                #endregion

                                #region Depósito (Pagto. Bancário) ou Boleto

                                else if (objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito || objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                                {
                                    // Salva o pagto. bancário no Cx. Geral
                                    CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                       UtilsPlanoConta.GetPlanoContaCreditoFornec(objInsert.FormasPagto[i]), 2, objInsert.ValoresPagto[i], 0, false,
                                       objInsert.DatasPagto[i], null);

                                    // Gera movimentação de saída na conta bancária
                                    ContaBancoDAO.Instance.MovContaCreditoFornecedor(transaction, objInsert.ContasBancoPagto[i],
                                        UtilsPlanoConta.GetPlanoContaCreditoFornec(objInsert.FormasPagto[i]), (int)UserInfo.GetUserInfo.IdLoja, objInsert.IdCreditoFornecedor, null,
                                        objInsert.IdFornecedor, 2, objInsert.ValoresPagto[i], 0,
                                        objInsert.DatasPagto[i].GetValueOrDefault(objInsert.DataRecebimento.GetValueOrDefault()), null);
                                }

                                #endregion

                                #region Permuta

                                else if (objInsert.FormasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                                    CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                       UtilsPlanoConta.GetPlanoContaCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Permuta), 2, objInsert.ValoresPagto[i], 0, false,
                                       objInsert.DatasPagto[i], null);

                                #endregion

                                valor += objInsert.ValoresPagto[i];
                            }
                        }
                        
                        CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, objInsert.IdCreditoFornecedor, objInsert.IdFornecedor,
                                objInsert.IdConta, 2, valor , 0, false, objInsert.DataCad, null);

                        transaction.Commit();
                        transaction.Close();

                        return objInsert.IdCreditoFornecedor;
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

        public override int Delete(CreditoFornecedor objDelete)
        {
            try
            {
                Cancela(objDelete.IdCreditoFornecedor, null);
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static readonly object _cancelaLock = new object();

        public void Cancela(uint idCreditoFornecedor, string motivo)
        {
            lock (_cancelaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CreditoFornecedor cred = ObterCreditoFornecedor(transaction, idCreditoFornecedor);
                        decimal valor = 0;

                        var lstMov = MovBancoDAO.Instance.GetByCreditoFornec(transaction, idCreditoFornecedor, null);

                        foreach (PagtoCreditoFornecedor p in cred.Pagamentos)
                            valor += p.ValorPagto;

                        // Verifica se o fornecedor possui crédito suficiente para que este seja cancelado
                        if (FornecedorDAO.Instance.GetCredito(transaction, cred.IdFornecedor) < valor)
                            throw new Exception("O crédito gerado já foi utilizado, não é possível cancelá-lo."); 

                        if (ExecuteScalar<bool>(transaction, "Select Count(*)>0 From cheques c Where c.IdCreditoFornecedor=" + idCreditoFornecedor + " And Situacao > 1"))
                            throw new Exception(@"Um ou mais cheques que compõe esse procedimento já foram utilizados em outras transações, cancele ou retifique as transações dos cheques antes de cancelar este crédito.");

                        objPersistence.ExecuteCommand(transaction, "update credito_fornecedor set situacao=?s where idCreditoFornecedor=" +
                        idCreditoFornecedor, new GDAParameter("?s", (int)CreditoFornecedor.SituacaoCredito.Cancelado));

                        foreach (PagtoCreditoFornecedor p in cred.Pagamentos)
                        {
                            #region Dinheiro

                            // Se a forma de pagto for Dinheiro, gera movimentação no caixa geral
                            if (p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                                CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, idCreditoFornecedor, cred.IdFornecedor,
                                   UtilsPlanoConta.GetPlanoContaEstornoCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro), 1, p.ValorPagto, 1, true, null, null);

                            #endregion

                            #region Cheques

                            // Se a forma de pagamento for cheques próprios, cadastra cheques, associa os mesmos com as contas
                            // que estão sendo pagas, debita valor da conta que foi escolhida em cada cheque
                            else if ((p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro))
                            {
                                var cheques = ChequesDAO.Instance.GetByCreditoFornecedor(transaction, idCreditoFornecedor);
                                var contadorDataUnica = 0;
                                foreach (Cheques c in cheques)
                                {
                                    // Só executa para o tipo de cheque da forma de pagamento adequada
                                    if ((c.Tipo == 1 && p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro) ||
                                        (c.Tipo == 2 && p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio))
                                        continue;

                                    CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, idCreditoFornecedor, cred.IdFornecedor,
                                        UtilsPlanoConta.GetPlanoContaEstornoCreditoFornec(p.IdFormaPagto), 1, c.Valor, 2, true, null, contadorDataUnica++);

                                    // Cheque próprio
                                    if (c.Tipo == 1)
                                    {
                                        ChequesDAO.Instance.Delete(transaction, c);

                                        // Para cada cheque "Compensado" utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                                        if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado && c.IdContaBanco > 0)
                                        {
                                            // Pega a primeira movimentação da conta bancária referente ao pagto
                                            object obj = objPersistence.ExecuteScalar(transaction, "Select idMovBanco from mov_banco Where idContaBanco=" + c.IdContaBanco +
                                                " And idCreditoFornecedor=" + idCreditoFornecedor + " and idCheque=" + c.IdCheque + " order by idMovBanco asc limit 1");

                                            uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                                            if (idMovBanco == 0)
                                                return;

                                            // Verifica a conciliação bancária
                                            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idMovBanco);

                                            MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idMovBanco);

                                            // Corrige saldo
                                            objPersistence.ExecuteCommand(transaction, "Update mov_banco Set valorMov=0 Where idCheque=" + c.IdCheque + " And idCreditoFornecedor=" + idCreditoFornecedor);
                                            if (movAnterior != null)
                                                MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idMovBanco);

                                            // Exclui movimentações geradas
                                            objPersistence.ExecuteCommand(transaction, "Delete From mov_banco Where idCheque=" + c.IdCheque + " And idCreditoFornecedor=" + idCreditoFornecedor);
                                        }
                                    }

                                    // Cheque de terceiros
                                    else if (c.Tipo == 2)
                                    {
                                        objPersistence.ExecuteCommand(transaction, @"update cheques set jurosPagto=null, multaPagto=null,
                                        idCreditoFornecedor=null, situacao=" + (int)Cheques.SituacaoCheque.EmAberto + " where idCheque=" + c.IdCheque);
                                    }
                                }
                            }

                            #endregion

                            #region Depósito (Pagto. Bancário) ou Boleto

                            else if (p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito || p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                            {
                                uint idConta = UtilsPlanoConta.GetPlanoContaCreditoFornec(p.IdFormaPagto);

                                // Pega a primeira movimentação da conta bancária referente ao pagto
                                object obj = objPersistence.ExecuteScalar(transaction, "Select idMovBanco from mov_banco Where " +
                                    "idCreditoFornecedor=" + idCreditoFornecedor + " and idConta=" + idConta + " order by idMovBanco asc limit 1");

                                uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                                if (idMovBanco == 0)
                                    continue;

                                // Verifica a conciliação bancária
                                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idMovBanco);

                                MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idMovBanco);

                                // Corrige saldo
                                objPersistence.ExecuteCommand(transaction, "Update mov_banco Set valorMov=0 Where idConta=" + idConta + " And idCreditoFornecedor=" + idCreditoFornecedor);
                                if (movAnterior != null)
                                    MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idMovBanco);

                                // Exclui movimentações geradas
                                objPersistence.ExecuteCommand(transaction, "Delete From mov_banco Where idConta=" + idConta + " And idCreditoFornecedor=" + idCreditoFornecedor);

                                // Salva o pagto. bancário no Cx. Geral
                                CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, idCreditoFornecedor, cred.IdFornecedor,
                                    UtilsPlanoConta.GetPlanoContaEstornoCreditoFornec(p.IdFormaPagto), 1, p.ValorPagto, 0, false, null, null);
                            }

                            #endregion

                            #region Permuta

                            else if (p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                                CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, idCreditoFornecedor, cred.IdFornecedor,
                                    UtilsPlanoConta.GetPlanoContaEstornoCreditoFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Permuta), 1, p.ValorPagto, 0, false, null, null);
                            
                            #endregion                                          
                        }
                                 
                        CaixaGeralDAO.Instance.MovCxCreditoFornecedor(transaction, idCreditoFornecedor, cred.IdFornecedor,
                           cred.IdConta, 1, valor, 0, false, null, null);

                        FornecedorDAO.Instance.DebitaCredito(transaction, cred.IdFornecedor, valor);
                        LogCancelamentoDAO.Instance.LogCreditoFornecedor(cred, motivo, true);
                        
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
    }
}
