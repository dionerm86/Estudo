using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class AntecipacaoFornecedorDAO : BaseDAO<AntecipacaoFornecedor, AntecipacaoFornecedorDAO>
    {
        //private AntecipacaoFornecedorDAO() { }

        private static readonly object _antecipacaoFornecedorLock = new object();

        #region Métodos de retorno de itens

        private string Sql(uint idAntecipFornec, uint? idFornec, string nomeFornec, uint idFunc, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            string idsNotasIgnorar, bool selecionar)
        {
            string campos = selecionar ? @"af.*, fr.NomeFantasia as NomeFornec, fr.credito as creditoFornec, f.Nome as NomeFunc, 
                (" + SqlTotalNotasAbertas("af.IdAntecipFornec", idsNotasIgnorar) + ") as totalNotasAbertas, '$$$' as Criterio" : "Count(*)";

            string sql = "select " + campos + @" from antecipacao_fornecedor af 
                left join fornecedor fr on (af.IdFornec=fr.IdFornec) 
                left join funcionario f on (af.IdFunc=f.IdFunc) where 1";

            string criterio = string.Empty;

            if (idAntecipFornec > 0)
            {
                sql += " And af.idAntecipFornec=" + idAntecipFornec;
                criterio += "Num: " + idAntecipFornec + "     ";
            }

            if (idFornec > 0)
            {
                sql += " and af.idFornec=" + idFornec.Value;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome((uint)idFornec) + "     ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                sql += " And af.idFornec in (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "     ";
            }

            if (idFunc > 0)
            {
                sql += " and af.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "     ";
            }

            if (idFormaPagto > 0)
            {
                sql += " and af.idAntecipFornec In (Select idAntecipFornec From pagto_antecipacao_fornecedor Where idFormaPagto=" + idFormaPagto + ")";
                criterio += "Forma Pagto.: " + FormaPagtoDAO.Instance.GetDescricao(idFormaPagto) + "     ";
            }

            if (situacao > 0)
            {
                AntecipacaoFornecedor af = new AntecipacaoFornecedor();
                af.Situacao = situacao;

                sql += " and af.Situacao=" + situacao;
                criterio += "Situação: " + af.DescrSituacao + "     ";
            }

            if (dtIni != null)
            {
                sql += " and af.DataCad>='" + dtIni.Value.ToString("yyyy-MM-dd 00:00") + "'";
                criterio += "A partir de " + dtIni.Value.ToString("yyyy-MM-dd") + "     ";
            }

            if (dtFim != null)
            {
                sql += " and af.DataCad<='" + dtFim.Value.ToString("yyyy-MM-dd 23:59") + "'";
                criterio += "Até " + dtFim.Value.ToString("yyyy-MM-dd") + "     ";
            }

            return sql.Replace("$$$", criterio);
        }

        internal string SqlTotalNotasAbertas(string idAntecipFornec, string idsNotasIgnorar)
        {
            string sql = @"SELECT SUM(COALESCE(totalnota,0))
                          FROM nota_fiscal
                          WHERE tipodocumento=" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + @"
                                AND situacao NOT IN (" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + @")
                                AND idAntecipFornec=" + idAntecipFornec;

            if (!String.IsNullOrEmpty(idsNotasIgnorar))
                sql += " AND idNF NOT IN (" + idsNotasIgnorar + ")";

            return sql;
        }

        public AntecipacaoFornecedor GetElement(uint idAntecipFornec)
        {
            return objPersistence.LoadOneData(Sql(idAntecipFornec, null, null, 0, 0, 0, null, null, null, true));
        }

        public AntecipacaoFornecedor[] GetListRpt(uint? idFornec, string nomeFornec, uint idFunc, uint idFormaPagto, int situacao, string dtIni,
            string dtFim, string idsNotasIgnorar)
        {
            return objPersistence.LoadData(Sql(0, idFornec, nomeFornec, idFunc, idFormaPagto, situacao, Glass.Conversoes.StrParaDate(dtIni), Glass.Conversoes.StrParaDate(dtFim),
                idsNotasIgnorar, true)).ToArray();
        }

        public IList<AntecipacaoFornecedor> GetList(uint? idFornec, string nomeFornec, uint idFunc, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            string idsNotasIgnorar, string sortExpression, int startRow, int pageSize)
        {
            string order = String.IsNullOrEmpty(sortExpression) ? "af.idAntecipFornec desc" : sortExpression;
            return LoadDataWithSortExpression(Sql(0, idFornec, nomeFornec, idFunc, idFormaPagto, situacao, dtIni, dtFim,
                idsNotasIgnorar, true), order, startRow, pageSize, null);
        }

        public int GetListCount(uint? idFornec, string nomeFornec, uint idFunc, uint idFormaPagto, int situacao, DateTime? dtIni, DateTime? dtFim,
            string idsNotasIgnorar)
        {
            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(Sql(0, idFornec, nomeFornec, idFunc, idFormaPagto, situacao, dtIni, dtFim,
                idsNotasIgnorar, false)).ToString());
        }

        #endregion

        #region Atualização do saldo

        /// <summary>
        /// Atualiza o saldo de uma antecipação.
        /// </summary>
        /// <param name="idObra"></param>
        public decimal AtualizaSaldo(GDASession sessao, uint idAntecipFornec)
        {
            decimal temp;
            return AtualizaSaldo(sessao, idAntecipFornec, out temp).Value;
        }

        /// <summary>
        /// Atualiza o saldo de uma antecipação.
        /// </summary>
        /// <param name="idObra"></param>
        internal KeyValuePair<uint?, decimal> AtualizaSaldo(GDASession sessao, uint idAntecipFornec, out decimal creditoGerado)
        {
            decimal valorAntecip = ObtemValorCampo<decimal>(sessao, "valorantecip", "idAntecipFornec=" + idAntecipFornec); 
            
            string sqlValorGastoNf = @"
                SELECT COALESCE(SUM(totalnota),0)
                FROM nota_fiscal 
                WHERE idAntecipFornec=" + idAntecipFornec + @"
                    AND situacao =" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;

            string sqlValorGastoCompra = @"
                SELECT COALESCE(SUM(total),0)
                FROM compra 
                WHERE idAntecipFornec=" + idAntecipFornec + @"
                    AND situacao IN(" + (int)Compra.SituacaoEnum.AguardandoEntrega + "," + (int)Compra.SituacaoEnum.Finalizada + ")";

            string sqlValorGastoContasPagar = @"
                select coalesce(sum(pp.valorPagto),0)
                from pagto_pagto pp
                    inner join pagto p on (pp.idPagto=p.idPagto)
                    inner join contas_pagar cp on (pp.idPagto=cp.idPagto)
                where pp.idAntecipFornec=" + idAntecipFornec + @"
                    and pp.idFormaPagto=" + (int)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec + @"
                    and p.situacao=" + (int)Pagto.SituacaoPagto.Finalizado;

            decimal valorGastoNf = ExecuteScalar<decimal>(sessao, sqlValorGastoNf);
            decimal valorGastoCompra = ExecuteScalar<decimal>(sessao, sqlValorGastoCompra);
            decimal valorGastoContasPagar = ExecuteScalar<decimal>(sessao, sqlValorGastoContasPagar);

            decimal valorGasto = valorGastoNf + valorGastoCompra + valorGastoContasPagar;

            objPersistence.ExecuteCommand(sessao, @"
                UPDATE antecipacao_fornecedor
                SET Saldo=?saldo, situacao=" + (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada + @"
                WHERE idAntecipFornec=" + idAntecipFornec, 
                new GDAParameter("?saldo", Math.Max(valorAntecip - valorGasto, 0)));

            string sqlSaldo = @"
                SELECT COALESCE(saldo, 0) 
                FROM antecipacao_fornecedor 
                WHERE idAntecipFornec=" + idAntecipFornec;

            decimal saldo = ExecuteScalar<decimal>(sessao, sqlSaldo);
            uint? id = null;
            creditoGerado = 0;

            var situacao = ObtemValorCampo<AntecipacaoFornecedor.SituacaoAntecipFornec>(sessao, "situacao", "idAntecipFornec=" + idAntecipFornec);
            if (saldo == 0 && situacao != AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada)
                id = Finalizar(sessao, idAntecipFornec, out creditoGerado);
            else if (saldo > 0 && situacao == AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada)
            {
                // Reabre a antecipação de fornecedor se o saldo ficar positivo
                objPersistence.ExecuteCommand(sessao, "update antecipacao_fornecedor set situacao=" +
                    (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada + " where idAntecipFornec=" + idAntecipFornec);
            }

            return new KeyValuePair<uint?,decimal>(id, saldo);
        }

        #endregion

        #region Finalizar antecipação

        /// <summary>
        /// Finaliza uma antecipação.
        /// </summary>
        public uint? Finalizar(GDASession sessao, uint idAntecipFornec, out decimal creditoGerado)
        {
            creditoGerado = 0;

            int situacao = ObtemValorCampo<int>(sessao, "situacao", "idAntecipFornec=" + idAntecipFornec);

            if (situacao != (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Aberta 
                && situacao != (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada)
                throw new Exception("Somente antecipações que estão abertas/confirmadas podem ser finalizadas.");

            decimal saldo = ObtemValorCampo<decimal>(sessao, "saldo", "idAntecipFornec=" + idAntecipFornec);

            if (saldo > 0 && !FinanceiroConfig.FormaPagamento.CreditoFornecedor)
                throw new Exception("Ainda há saldo para essa antecipação. A mesma não pode ser finalizada.");

            uint? id = null;
            
            if (saldo > 0)
            {
                uint idFornec = ObtemValorCampo<uint>(sessao, "idFornec", "idAntecipFornec=" + idAntecipFornec);
                creditoGerado = saldo;

                FornecedorDAO.Instance.CreditaCredito(sessao, idFornec, creditoGerado);
                id = CaixaGeralDAO.Instance.MovCxAntecipFornec(sessao, idAntecipFornec, idFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntecipFornecGerado), 2,
                    creditoGerado, null, 0, false, null);
            }

            objPersistence.ExecuteCommand(sessao, "update antecipacao_fornecedor set idFuncFin=?func, saldo=0, dataFin=?data, situacao=?sit where idAntecipFornec=" + idAntecipFornec,
                new GDAParameter("?func", UserInfo.GetUserInfo.CodUser), new GDAParameter("?data", DateTime.Now),
                new GDAParameter("?sit", (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada));

            return id;
        }

        #endregion

        #region Pagamento à vista

        public string PagamentoVista(AntecipacaoFornecedor antecip)
        {
            lock(_antecipacaoFornecedorLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        uint[] formasPagto = antecip.FormasPagto;
                        uint[] tiposCartao = antecip.TiposCartaoPagto;
                        uint[] contasBanco = antecip.ContasBancoPagto;

                        if (UtilsFinanceiro.ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, formasPagto) && String.IsNullOrEmpty(antecip.ChequesPagto))
                            throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento da conta.");

                        if (ObtemValorCampo<int>(transaction, "Situacao", "IdAntecipFornec=" + antecip.IdAntecipFornec) ==
                            (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada)
                            throw new Exception("Esta antecipação já foi finalizada.");


                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        // Se não for caixa diário ou financeiro, não pode cadastrar obra
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para cadastrar antecipação de pagto. fornecedor.");

                        decimal total = antecip.CreditoUtilizado;
                        foreach (decimal v in antecip.ValoresPagto)
                            total += v;

                        if (Math.Round(total, 2) < Math.Round(antecip.ValorAntecip, 2) ||
                            (!FinanceiroConfig.FormaPagamento.CreditoFornecedor && Math.Round(total, 2) != Math.Round(antecip.ValorAntecip, 2)))
                            throw new Exception("O valor pago não confere com o valor a pagar. Valor pago: " + total.ToString("C") +
                                " Valor a pagar: " + antecip.ValorAntecip.ToString("C"));

                        string data = (antecip.DataRecebimento != null ? antecip.DataRecebimento.Value : DateTime.Now).ToString("dd/MM/yyyy");

                        antecip.ValorCreditoAoCriar = FornecedorDAO.Instance.GetCredito(transaction, antecip.IdFornec);

                        #region Insere as informações sobre pagamentos

                        PagtoAntecipacaoFornecedorDAO.Instance.DeleteByAntecipFornec(transaction, antecip.IdAntecipFornec);

                        int numPagto = 1;
                        for (int i = 0; i < antecip.ValoresPagto.Length; i++)
                        {
                            if (antecip.ValoresPagto[i] == 0)
                                continue;

                            PagtoAntecipacaoFornecedor paf = new PagtoAntecipacaoFornecedor();
                            paf.IdAntecipFornec = antecip.IdAntecipFornec;
                            paf.NumFormaPagto = numPagto++;
                            paf.ValorPagto = antecip.ValoresPagto[i];
                            paf.IdFormaPagto = formasPagto[i];
                            paf.IdContaBanco = contasBanco[i] > 0 ? (uint?)contasBanco[i] : null;
                            paf.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;
                            paf.NumAutCartao = antecip.NumAutCartao[i];

                            PagtoAntecipacaoFornecedorDAO.Instance.Insert(transaction, paf);
                        }

                        // Se for pago com crédito, gera o pagto do credito
                        if (antecip.CreditoUtilizado > 0)
                            PagtoAntecipacaoFornecedorDAO.Instance.Insert(transaction, new PagtoAntecipacaoFornecedor()
                            {
                                IdAntecipFornec = antecip.IdAntecipFornec,
                                IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito,
                                NumFormaPagto = numPagto++,
                                ValorPagto = antecip.CreditoUtilizado
                            });

                        #endregion

                        #region Gera as movimentações

                        PagarContas(transaction, antecip.IdAntecipFornec, antecip.IdFornec, antecip.ValoresPagto, antecip.FormasPagto, antecip.ContasBancoPagto, antecip.TiposCartaoPagto,
                            antecip.ParcelasCartaoPagto, antecip.ChequesPagto, DateTime.Parse(data), null, null, true, antecip.CreditoUtilizado, total, antecip.ValorAntecip);

                        #endregion

                        if (total > antecip.ValorAntecip && FinanceiroConfig.FormaPagamento.CreditoFornecedor)
                            antecip.CreditoGeradoCriar = total - antecip.ValorAntecip;
                        antecip.CreditoUtilizadoCriar = antecip.CreditoUtilizado;

                        // Atualiza a situação da Obra
                        antecip.Situacao = (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada;
                        Update(transaction, antecip);

                        // O saldo precisa ser atualizado antes de finalizar a obra para que gere crédito
                        AtualizaSaldo(transaction, antecip.IdAntecipFornec);

                        transaction.Commit();
                        transaction.Close();

                        return "Pagamento antecipado efetuado com sucesso.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cadastrar antecipação.", ex));
                    }
                }
            }
        }

        private void PagarContas(GDASession session, uint idAntecipFornec, uint idFornec, decimal[] valores, uint[] formasPagto,
            uint[] idContasBanco, uint[] tiposCartao, uint[] numParcCartao, string chequesPagto, DateTime dataPagto,
            DateTime[] datasFormasPagto, string obs, bool gerarCredito, decimal creditoUtilizado, decimal totalPago, decimal totalASerPago)
        {
            #region Variáveis

            // Dinheiro
            List<uint> lstIdCaixaGeral = new List<uint>();

            // Cheques próprios
            PagtoCheque pagtoCheque = new PagtoCheque();
            List<Cheques> lstChequesInseridos = new List<Cheques>();
            List<uint> lstIdMovBanco = new List<uint>();
            List<uint> lstIdCxGeral = new List<uint>();

            // Guarda os ids dos cheques de terceiros
            string idsChequeTerc = String.Empty;

            // Crédito Gerado/Utilizado
            uint idCreditoGerado = 0;
            uint idCreditoUtilizado = 0;

            #endregion
            
            // Recupera o número de formas de pagamento válidas
            int numFormasPagto = 0;
            for (int i = 0; i < valores.Length; i++)
                if (valores[i] > 0 && formasPagto[i] > 0)
                    numFormasPagto++;

            for (int i = 0; i < formasPagto.Length; i++)
            {
                if (valores[i] == 0)
                    continue;

                DateTime dataUsar = dataPagto;

                #region Dinheiro

                // Se a forma de pagto for Dinheiro, gera movimentação no caixa geral
                if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                {
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro), 2, valores[i], obs, 1, true, null));
                }

                #endregion

                #region Cheques

                // Se a forma de pagamento for cheques próprios, cadastra cheques, associa os mesmos com as contas
                // que estão sendo pagas, debita valor da conta que foi escolhida em cada cheque
                else if ((formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro) &&
                        !String.IsNullOrEmpty(chequesPagto))
                {
                    Cheques cheque;

                    // Separa os cheques guardando-os em um vetor
                    string[] vetCheque = chequesPagto.TrimEnd(' ').TrimEnd('|').Split('|');

                    pagtoCheque = new PagtoCheque();

                    // Cria um idPagto, que será utilizado para identificar este pagto.
                    pagtoCheque.IdAntecipFornec = idAntecipFornec;

                    if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                    {
                        #region Associa cheques ao pagamento e Gera movimentações no caixa geral

                        try
                        {
                            // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                            foreach (string c in vetCheque)
                            {
                                // Divide o cheque para pegar suas propriedades
                                string[] dadosCheque = c.Split('\t');

                                if (dadosCheque[0] == "proprio") // Se for cheque próprio
                                {
                                    // Insere cheque no BD
                                    cheque = ChequesDAO.Instance.GetFromString(session, c);
                                    cheque.IdAntecipFornec = idAntecipFornec;

                                    if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                        cheque.DataReceb = dataPagto;

                                    cheque.IdCheque = ChequesDAO.Instance.InsertBase(session, cheque);

                                    if (cheque.IdCheque < 1)
                                        throw new Exception("retorno do insert do cheque=0");

                                    // Adiciona este cheque à lista de cheques inseridos
                                    lstChequesInseridos.Add(cheque);
                                    
                                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio), 2,
                                        cheque.Valor, obs, 0, false, null));
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
                            // Associa cada cheque utilizado no pagamento ao pagto
                            foreach (Cheques c in lstChequesInseridos)
                            {
                                pagtoCheque.IdCheque = c.IdCheque;
                                pagtoCheque.IdContaBanco = c.IdContaBanco;
                                PagtoChequeDAO.Instance.Insert(session, pagtoCheque);
                            }

                            // Para cada cheque "Compensado" utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                            foreach (Cheques c in lstChequesInseridos)
                                if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                {
                                    lstIdMovBanco.Add(ContaBancoDAO.Instance.MovContaChequeAntecipFornec(session, c.IdContaBanco.Value,
                                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio),
                                        (int)UserInfo.GetUserInfo.IdLoja, c.IdCheque, idAntecipFornec, idFornec, 2, c.Valor, dataUsar));
                                }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar cheques às contas a pagar", ex));
                        }

                        #endregion
                    }
                    // Se a forma de pagamento for cheques de terceiros
                    else if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
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
                                    pagtoCheque.IdCheque = Glass.Conversoes.StrParaUint(dadosCheque[18]);
                                    PagtoChequeDAO.Instance.Insert(session, pagtoCheque);
                                    idsChequeTerc += dadosCheque[18] + ",";
                                    
                                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro), 2,
                                        ChequesDAO.Instance.ObtemValor(session, pagtoCheque.IdCheque), null, 2, true, null));
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
                            ChequesDAO.Instance.UpdateSituacao(session, idsChequeTerc.TrimEnd(','), Cheques.SituacaoCheque.Compensado);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao marcar cheques como compensados.", ex)); ;
                        }

                        #endregion
                    }
                }

                #endregion

                #region Depósito (Pagto. Bancário) ou Boleto

                else if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito || formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                {
                    uint idConta = formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito ? UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Deposito) :
                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Boleto);

                    // Salva o pagto. bancário no Cx. Geral
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                        idConta, 2, valores[i], obs, 0, false, dataUsar));

                    // Gera movimentação de saída na conta bancária
                    lstIdMovBanco.Add(ContaBancoDAO.Instance.MovContaAntecipFornec(session, idContasBanco[i],
                        idConta, (int)UserInfo.GetUserInfo.IdLoja, idAntecipFornec, null, idFornec, 2, valores[i], dataUsar, obs));
                }

                #endregion

                #region Permuta

                else if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                {
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                        UtilsPlanoConta.GetPlanoContaAntecipFornec((uint)Glass.Data.Model.Pagto.FormaPagto.Permuta), 2, valores[i], obs, 0, false, dataUsar));
                }

                #endregion

            }

            #region Crédito

            // Se a empresa trabalhar com crédito de fornecedor
            if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && idFornec > 0)
            {
                // Se algum crédito do fornecedor tive sido utilizado
                if (creditoUtilizado > 0)
                {
                    idCreditoUtilizado = CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor),
                        1, creditoUtilizado, null, 0, false, null);

                    // Debita Crédito com o fornecedor
                    FornecedorDAO.Instance.DebitaCredito(session, idFornec, creditoUtilizado);
                }

                // Se tiver sido gerado algum crédito para com o fornecedor
                if (gerarCredito && totalPago > totalASerPago)
                {
                    decimal valorCreditoGerado = totalPago - totalASerPago;
                    idCreditoGerado = CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, idFornec,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntecipFornecGerado),
                        2, valorCreditoGerado, null, 0, false, null);

                    // Credita crédito do fornecedor
                    FornecedorDAO.Instance.CreditaCredito(session, idFornec, valorCreditoGerado);
                }
            }

            #endregion
        }

        #endregion

        #region Pagamento à prazo

        public string PagamentoPrazo(AntecipacaoFornecedor antecip)
        {
            lock(_antecipacaoFornecedorLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        List<uint> lstContaPagar = new List<uint>();

                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        // Se não for caixa diário ou financeiro, não pode cadastrar obra
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para cadastrar antecipação de pagto. fornecedor.");

                        if (ObtemValorCampo<int>(transaction, "Situacao", "IdAntecipFornec=" + antecip.IdAntecipFornec) ==
                            (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada)
                            throw new Exception("Esta antecipação já foi finalizada.");

                        decimal total = 0;
                        for (int i = 0; i < antecip.ValoresParcelas.Length; i++)
                            total += antecip.ValoresParcelas[i];

                        if (Math.Round(antecip.ValorAntecip, 2) != Math.Round(total, 2))
                            throw new Exception("O valor pago não confere com o valor das parcelas. Valor das parcelas: " + total.ToString("C") +
                                " Valor a pagar: " + antecip.ValorAntecip.ToString("C"));

                        for (int i = 0; i < antecip.NumParcelas; i++)
                        {
                            ContasPagar c = new ContasPagar();
                            c.IdAntecipFornec = antecip.IdAntecipFornec;
                            c.ValorVenc = antecip.ValoresParcelas[i];
                            c.DataVenc = antecip.DatasParcelas[i];
                            c.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ParcelamentoPagtoAntecipFornec);
                            c.IdFornec = antecip.IdFornec;
                            c.NumParc = (i + 1);
                            c.NumParcMax = antecip.NumParcelas;

                            lstContaPagar.Add(ContasPagarDAO.Instance.Insert(transaction, c));
                        }

                        // Atualiza a situação da antecipação
                        antecip.Situacao = (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada;
                        Update(transaction, antecip);

                        // O saldo precisa ser atualizado antes de finalizar a antecipação para que gere crédito
                        AtualizaSaldo(transaction, antecip.IdAntecipFornec);

                        transaction.Commit();
                        transaction.Close();

                        return "Parcelas geradas com sucesso.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        return Glass.MensagemAlerta.FormatErrorMsg("Erro ao gerar parcelas da obra.", ex);
                    }
                }
            }
        }

        #endregion

        #region Cancela antecipação

        /// <summary>
        /// Efetua o cancelamento da antecipação
        /// </summary>
        public void CancelaAntecipFornec(uint idAntecipFornec, string motivo, DateTime dataEstornoBanco)
        {
            lock(_antecipacaoFornecedorLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        AntecipacaoFornecedor antecip = GetElementByPrimaryKey(transaction, idAntecipFornec);

                        if (antecip == null)
                            throw new Exception("Antecipação de Fornecedor informada não foi encontrada.");

                        // Verifica se existe alguma nota associada à esta antecipação
                        if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(transaction, "Select count(*) From nota_fiscal Where idAntecipFornec=" + idAntecipFornec +
                            " And situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros).ToString()) > 0)
                            throw new Exception("Cancele todas as notas associadas à esta antecipação antes de cancelar a mesma.");

                        // Verifica se existe alguma compra associada à esta antecipação
                        if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(transaction, "Select count(*) From compra Where idAntecipFornec=" + idAntecipFornec +
                            " And situacao IN(" + (int)Compra.SituacaoEnum.AguardandoEntrega + "," + (int)Compra.SituacaoEnum.Finalizada + ")").ToString()) > 0)
                            throw new Exception("Cancele todas as notas associadas à esta antecipação antes de cancelar a mesma.");

                        // Verifica se alguma parcela desta antecipação já foi paga
                        if (ContasPagarDAO.Instance.ExistePagaAntecipFornec(transaction, idAntecipFornec))
                            throw new Exception("Existe uma conta paga associada à esta antecipação.");

                        if (antecip.Situacao == (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Cancelada)
                            throw new Exception("A antecipação de fornecedor informada já está cancelada.");

                        ContasPagar[] lstContasPg = ContasPagarDAO.Instance.GetByAntecipFornec(transaction, antecip.IdAntecipFornec);

                        #region Estorna Crédito

                        if (FinanceiroConfig.FormaPagamento.CreditoFornecedor)
                        {
                            List<CaixaGeral> lstCxGeral = new List<CaixaGeral>(CaixaGeralDAO.Instance.GetByAntecipFornec(transaction, antecip.IdAntecipFornec));
                            lstCxGeral.Sort(
                                delegate (CaixaGeral x, CaixaGeral y)
                                {
                                    int comp = y.DataMov.CompareTo(x.DataMov);
                                    if (comp == 0)
                                        comp = y.IdCaixaGeral.CompareTo(x.IdCaixaGeral);

                                    return comp;
                                }
                            );

                            foreach (CaixaGeral cx in lstCxGeral)
                            {
                                // Se algum credito tiver sido utilizado, estorna no crédito do fornecedor
                                if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor))
                                {
                                    FornecedorDAO.Instance.CreditaCredito(transaction, antecip.IdFornec, cx.ValorMov);

                                    // Estorna crédito utilizado pelo fornecedor
                                    CaixaGeralDAO.Instance.MovCxAntecipFornec(transaction, antecip.IdAntecipFornec, antecip.IdFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntencipFornecEstorno), 2,
                                        cx.ValorMov, null, 0, false, null);
                                }

                                if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntecipFornecGerado))
                                {
                                    FornecedorDAO.Instance.DebitaCredito(transaction, antecip.IdFornec, cx.ValorMov);

                                    // Estorna crédito venda gerado
                                    CaixaGeralDAO.Instance.MovCxAntecipFornec(transaction, antecip.IdAntecipFornec, antecip.IdFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoAntencipFornecEstorno), 1,
                                        cx.ValorMov, null, 0, false, null);
                                }
                            }
                        }

                        #endregion

                        PagtoAntecipacaoFornecedor[] pagtoAntecipFornec = PagtoAntecipacaoFornecedorDAO.Instance.GetByAntecipFornec(transaction, antecip.IdAntecipFornec);

                        #region Estorna pagamentos

                        // Estorna os pagamentos
                        foreach (PagtoAntecipacaoFornecedor paf in pagtoAntecipFornec)
                        {
                            // Não estorna os cheques ou crédito no loop (crédito: idFormaPagto = 0);
                            if (paf.IdFormaPagto == 0 || paf.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || paf.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                                continue;

                            // Recupera o plano de contas
                            uint idConta = UtilsPlanoConta.GetPlanoContaEstornoAntecipFornec(paf.IdFormaPagto);

                            // Gera uma movimentação de estorno para cada forma de pagto
                            CaixaGeralDAO.Instance.MovCxAntecipFornec(transaction, antecip.IdAntecipFornec, antecip.IdFornec, idConta, 1, paf.ValorPagto,
                                null, 0, paf.IdContaBanco == null || paf.IdContaBanco == 0, null);

                            if (paf.IdContaBanco > 0)
                            {
                                // Gera movimentação de estorno na conta bancária  
                                if (paf.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito)
                                {
                                    EstornoBancario(transaction, antecip.IdAntecipFornec, paf.IdContaBanco.Value, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoAntecipFornecDeposito),
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoAntecipFornecDeposito), dataEstornoBanco);
                                }
                                else if (paf.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                                {
                                    EstornoBancario(transaction, antecip.IdAntecipFornec, paf.IdContaBanco.Value, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoAntecipFornecBoleto),
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoAntecipFornecBoleto), dataEstornoBanco);
                                }
                            }
                        }

                        #region Estorna Cheques

                        var lstChequesTerc = ChequesDAO.Instance.GetByAntecipFornec(transaction, antecip.IdAntecipFornec).ToArray();

                        // Gera movimentação no caixa geral de estorno de cada cheque de terceiro
                        foreach (Cheques c in lstChequesTerc)
                        {
                            if (c.Tipo == 1)
                                continue;

                            // Estorna valor gerado pelo cheque
                            CaixaGeralDAO.Instance.MovCxAntecipFornec(transaction, antecip.IdAntecipFornec, antecip.IdFornec,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoAntecipFornecChequeTerceiros), 1,
                                c.Valor, null, 2, true, null);
                        }

                        // Altera situação dos cheques de terceiros utilizados em aberto
                        ChequesDAO.Instance.CancelaChequesAntecipFornec(transaction, antecip.IdAntecipFornec, 2, Cheques.SituacaoCheque.EmAberto);

                        // Cancela cheques próprios utilizados
                        ChequesDAO.Instance.CancelaChequesAntecipFornec(transaction, antecip.IdAntecipFornec, 1, Cheques.SituacaoCheque.Cancelado);

                        #endregion

                        #endregion

                        #region Gera Log Cancelamento

                        LogCancelamentoDAO.Instance.LogAntecipFornec(transaction, antecip, motivo, true);

                        #endregion

                        //Marca as notas
                        objPersistence.ExecuteCommand(transaction, "UPDATE nota_fiscal SET IdAntecipFornec= null WHERE IdAntecipFornec IN(" + antecip.IdAntecipFornec + ")");

                        //Apaga o pagamento
                        PagtoAntecipacaoFornecedorDAO.Instance.DeleteByAntecipFornec(transaction, antecip.IdAntecipFornec);

                        //Apaga a conta paga
                        ContasPagarDAO.Instance.DeleteByAntecipFornec(transaction, antecip.IdAntecipFornec);

                        //Cancela a antecipação
                        antecip.Situacao = (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Cancelada;
                        Update(transaction, antecip);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar antecipação de pagto. fornecedor.", ex));
                    }
                }
            }
        }

        #endregion

        #region Estorno bancário
        
        /// <summary>
        /// Efetua estorno bancário
        /// </summary>
        private void EstornoBancario(GDASession session, uint idAntecipFornec, uint idContaBanco, uint idConta, uint idContaEstorno, DateTime? dataEstornoBanco)
        {
            if (dataEstornoBanco == null || dataEstornoBanco.Value.Year == 1)
            {
                // Pega a primeira movimentação da conta bancária referente ao pagto
                object obj = objPersistence.ExecuteScalar(session, "Select idMovBanco from mov_banco Where idContaBanco=" + idContaBanco +
                    " And idAntecipFornec=" + idAntecipFornec + " order by idMovBanco asc limit 1");

                uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                if (idMovBanco == 0)
                    return;

                // Verifica se a data está conciliada
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idMovBanco);

                MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(session, idMovBanco);

                // Corrige saldo
                objPersistence.ExecuteCommand(session, "Update mov_banco Set valorMov=0 Where idConta=" + idConta + " And idAntecipFornec=" + idAntecipFornec);
                if (movAnterior != null) MovBancoDAO.Instance.CorrigeSaldo(session, movAnterior.IdMovBanco, idMovBanco);

                // Exclui movimentações geradas
                objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idConta=" + idConta + " And idAntecipFornec=" + idAntecipFornec);
            }
            else
            {
                // Verifica se a data está conciliada
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idContaBanco, dataEstornoBanco.Value);

                uint idFornec = ObtemValorCampo<uint>(session, "idFornec", "idAntecipFornec=" + idAntecipFornec);

                foreach (MovBanco m in MovBancoDAO.Instance.GetByAntecipFornec(session, idAntecipFornec, idConta))
                {
                    // Condição necessária para não estornar a conta mais de uma vez, no caso de efetuar um pagamento com duas ou mais 
                    // formas de pagamento que envolvem conta bancária (pagto. bancário)
                    if (idContaBanco == m.IdContaBanco)
                        ContaBancoDAO.Instance.MovContaAntecipFornec(session, idContaBanco, idContaEstorno, (int)UserInfo.GetUserInfo.IdLoja,
                            idAntecipFornec, null, idFornec, m.TipoMov == 1 ? 2 : 1, m.ValorMov, dataEstornoBanco.Value, m.Obs);
                }
            }
        }

        #endregion

        #region Recupera o nome do fornecedor

        /// <summary>
        /// Retorna o nome do fornecedor.
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public string GetNomeFornec(uint idAntecipFornec, bool incluirCodigoFornec)
        {
            object fornec = objPersistence.ExecuteScalar("select coalesce(idFornec, 0) from antecipacao_fornecedor where idAntecipFornec=" + idAntecipFornec);
            uint idFornec = fornec != null && fornec.ToString() != "" ? Glass.Conversoes.StrParaUint(fornec.ToString()) : 0;
            return (incluirCodigoFornec ? idFornec + " - " : "") + FornecedorDAO.Instance.GetNome(idFornec);
        }

        #endregion

        #region Recupera informações da antecipação

        public decimal GetValorAntecip(uint idAntecipFornec)
        {
            return ObtemValorCampo<decimal>("valorantecip", "idAntecipFornec=" + idAntecipFornec);
        }

        public uint GetIdFornec(uint idAntecipFornec)
        {
            return GetIdFornec(null, (int)idAntecipFornec);
        }

        public uint GetIdFornec(GDASession session, int idAntecipFornec)
        {
            return ObtemValorCampo<uint>(session, "idFornec", "idAntecipFornec=" + idAntecipFornec);
        }

        public decimal GetSaldo(uint idAntecipFornec)
        {
            return GetSaldo(null, idAntecipFornec);
        }

        public decimal GetSaldo(GDASession session, uint idAntecipFornec)
        {
            return ObtemValorCampo<decimal>(session, "saldo", "idAntecipFornec=" + idAntecipFornec);
        }

        public string GetDescricao(uint idAntecipFornec)
        {
            return ObtemValorCampo<string>("descricao", "idAntecipFornec=" + idAntecipFornec);
        }

        #endregion

        #region Busca antecipações em aberto

        /// <summary>
        /// Busca antecipações em aberto.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public AntecipacaoFornecedor[] ObtemAntecipacoesEmAberto(uint idFornec)
        {
            string sql = Sql(0, idFornec, null, 0, 0, (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada, null, null, null, true);
            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Verifica se há antecipações em aberto.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public bool PossuiAntecipacoesEmAberto(uint idFornec)
        {
            string sql = Sql(0, idFornec, null, 0, 0, (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada, null, null, null, false);
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(AntecipacaoFornecedor objInsert)
        {
            if (!FinanceiroConfig.UsarPgtoAntecipFornec)
                throw new Exception("Empresa não trabalha com controle de Pagto. Antecipado de Fornecedor.");

            objInsert.DataCad = DateTime.Now;
            objInsert.IdAntecipFornec = base.Insert(objInsert);

            return objInsert.IdAntecipFornec;
        }

        public override int Delete(AntecipacaoFornecedor objDelete)
        {
            CancelaAntecipFornec(objDelete.IdAntecipFornec, null, DateTime.Now);
            return 1;
        }

        #endregion
    }
}
