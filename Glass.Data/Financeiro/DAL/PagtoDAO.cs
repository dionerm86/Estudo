using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class PagtoDAO : BaseDAO<Pagto, PagtoDAO>
    {
        //private PagtoDAO() { }

        #region Novo pagamento

        /// <summary>
        /// Cria um novo pagamento (para renegociação).
        /// </summary>
        public uint NovoPagto(GDASession session, uint idFornec, decimal[] valoresPago, decimal juros, decimal multa, string obs)
        {
            decimal valorTotal = 0;
            foreach (decimal v in valoresPago)
                valorTotal += v;

            Pagto pagto = new Pagto();
            pagto.IdFuncPagto = UserInfo.GetUserInfo.CodUser;
            pagto.Situacao = (int)Pagto.SituacaoPagto.Finalizado;
            pagto.DataPagto = DateTime.Now;
            pagto.ValorPago = valorTotal;
            pagto.Juros = juros;
            pagto.Multa = multa;
            pagto.Desconto = 0;
            pagto.Obs = obs;

            if (idFornec > 0)
                pagto.IdFornec = idFornec;

            if (pagto.Obs.Length > 300)
                pagto.Obs = obs.Substring(0, 300);

            return Insert(session, pagto);
        }

        /// <summary>
        /// Cria um novo pagamento
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="dataPagto"></param>
        /// <param name="valorPago"></param>
        /// <param name="formaPagto"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="adicional"></param>
        /// <param name="desconto"></param>
        /// <param name="motivoDesc"></param>
        /// <returns></returns>
        public uint NovoPagto(GDASession sessao, uint idFornec, DateTime dataPagto, decimal[] valoresPago, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao,
            string[] boletos, uint[] antecipFornec, DateTime[] datasPagto, decimal juros, decimal multa, decimal desconto, decimal creditoUtilizado,
            decimal creditoGerado, string obs)
        {
            decimal valorTotal = 0;

            // Os valoresPago já possuem juros/multa/desconto embutido
            foreach (decimal v in valoresPago)
                valorTotal += v;

            Pagto pagto = new Pagto();
            pagto.IdFuncPagto = UserInfo.GetUserInfo.CodUser;
            pagto.Situacao = (int)Pagto.SituacaoPagto.Finalizado;
            pagto.DataPagto = dataPagto;
            pagto.ValorPago = valorTotal;
            pagto.Juros = juros;
            pagto.Multa = multa;
            pagto.Desconto = desconto;
            pagto.Obs = obs;

            if (idFornec > 0)
            {
                pagto.IdFornec = idFornec;
                pagto.ValorCreditoAoPagar = FornecedorDAO.Instance.ObtemValorCampo<decimal>(sessao, "credito", "idFornec=" + idFornec);
                pagto.CreditoUtilizado = creditoUtilizado;
                pagto.CreditoGerado = creditoGerado;
            }

            if (pagto.Obs.Length > 300)
                pagto.Obs = obs.Substring(0, 300);

            uint idPagto = Insert(sessao, pagto);

            int numPagto = 1;

            // Insere as formas de pagamento
            for (int i = 0; i < valoresPago.Length; i++)
            {
                if (valoresPago[i] == 0)
                    continue;

                PagtoPagto dadosPagto = new PagtoPagto();
                dadosPagto.IdPagto = idPagto;
                dadosPagto.NumFormaPagto = numPagto++;
                dadosPagto.IdFormaPagto = formasPagto[i];

                // Se for cheque próprio, deve salvar referência da conta bancária para estornar os juros caso o pagto seja cancelado
                dadosPagto.IdContaBanco = formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro && idContasBanco[i] > 0 ? (uint?)idContasBanco[i] : null;

                dadosPagto.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;
                dadosPagto.IdAntecipFornec = antecipFornec[i] > 0 ? (uint?)antecipFornec[i] : null;

                dadosPagto.NumBoleto = boletos[i];
                dadosPagto.ValorPagto = valoresPago[i];

                PagtoPagtoDAO.Instance.Insert(sessao, dadosPagto);
            }

            if (creditoUtilizado > 0)
                PagtoPagtoDAO.Instance.Insert(sessao, new PagtoPagto()
                {
                    IdPagto = idPagto,
                    NumFormaPagto = numPagto++,
                    IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito,
                    ValorPagto = creditoUtilizado
                });

            return idPagto;
        }

        #endregion

        #region Cancelar pagamento

        static volatile object _cancelarPagamentoLock = new object();

        /// <summary>
        /// Cancela pagamento
        /// </summary>
        public void CancelarPagtoComTransacao(uint idPagto, string motivoCanc, bool estornarMovimentacaoBancaria,
            DateTime? dataEstornoBanco)
        {
            lock(_cancelarPagamentoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CancelarPagto(transaction, idPagto, motivoCanc, estornarMovimentacaoBancaria, dataEstornoBanco);

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

        /// <summary>
        /// Cancela pagamento
        /// </summary>
        public void CancelarPagto(GDASession session, uint idPagto, string motivoCanc, bool estornarMovimentacaoBancaria,
            DateTime? dataEstornoBanco)
        {
            List<uint> lstCaixaGeral = new List<uint>(), lstMovBanco = new List<uint>();
            decimal credito = 0;
            ContasPagar[] pagas = null, geradas = null;
            Pagto pagto = new Pagto();
            MovBanco[] movs = MovBancoDAO.Instance.GetByPagto(session, idPagto, 0).ToArray();
            List<AntecipacaoFornecedor> lstAntecipFornec = new List<AntecipacaoFornecedor>();
            var lstCheques = ChequesDAO.Instance.GetByPagto(session, idPagto).ToArray();
            var contadorDataUnica = 0;

            pagto = GetElementByPrimaryKey(session, idPagto);

            if (pagto.Situacao == (int) Pagto.SituacaoPagto.Cancelado)
                throw new Exception("Este pagamento já foi cancelado.");

            // Verifica se este pagto possui boletos gerados e pagos
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From contas_pagar Where idConta=" +
                UtilsPlanoConta.GetPlanoConta(
                    UtilsPlanoConta.PlanoContas.PagtoRenegociacao) +
                " And idPagtoRestante=" + idPagto + " And paga=true") > 0)
                throw new Exception(
                    "Este pagamento gerou outras contas a pagar e as mesmas estão pagas, cancele o pagamento destas contas geradas antes de cancelar este pagamento");

            // Verifica se este pagto possui valores restante já pagos
            if (
                objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From contas_pagar Where idPagtoRestante=" +
                    idPagto + " And paga=true") > 0)
                throw new Exception(
                    "Este pagamento gerou um valor restante e o mesmo foi pago, cancele o pagamento deste valor restante antes de cancelar este pagamento");

            // Verifica se este pagto gerou juros e ou multa e se os planos de conta dos mesmos estão associados
            if (ContasPagarDAO.Instance.PossuiJurosMulta(session, idPagto) &&
                (FinanceiroConfig.PlanoContaEstornoJurosPagto == 0 ||
                    FinanceiroConfig.PlanoContaEstornoMultaPagto == 0))
                throw new Exception("Associe os planos de conta de estorno de juros e multa de pagamentos.");

            // Ao efetuar um pagamento com cheque próprio Compensado o campo ValorReceb fica zerado, é preenchido na quitação do cheque próprio que foi cadastrado em aberto no pagamento.
            if (lstCheques.Count(f => f.Tipo == 1 && (f.ValorReceb > 0 || f.IdDeposito > 0)) > 0)
            {
                var idsAcertoCheques = new List<int>();

                if (lstCheques.Count(f => f.Tipo == 1 && f.ValorReceb > 0) > 0)
                {
                    foreach (var cheque in lstCheques.Where(f => f.Tipo == 1 && f.ValorReceb > 0).ToList())
                    {
                        var idsAcertoCheque = ItemAcertoChequeDAO.Instance.GetIdsAcertoByCheque(session, cheque.IdCheque, true);

                        if (!string.IsNullOrEmpty(idsAcertoCheque))
                            idsAcertoCheques.AddRange(idsAcertoCheque.Split(',').Select(f => f.StrParaInt()).ToList());
                    }
                }

                if (lstCheques.Count(f => f.Tipo == 1 && f.IdDeposito > 0) > 0 || idsAcertoCheques.Count > 0)
                {
                    throw new Exception(
                        string.Format("Não é possível cancelar este pagamento, o(s) cheque(s) próprio de número {0} foi(ram) quitado(s):\n\n{1}{2}\n\nCancele a quitação dos cheques e tente cancelar o pagamento novamente.",
                            string.Join(", ", lstCheques.Where(f => f.Tipo == 1 && (f.ValorReceb > 0 || f.IdDeposito > 0)).Select(f => f.Num).ToList()),
                            idsAcertoCheques.Count > 0 ? string.Format("Acerto(s) de cheque próprio {0}", string.Join(", ", idsAcertoCheques)) : string.Empty,
                            lstCheques.Count(f => f.Tipo == 1 && f.IdDeposito > 0) > 0 ?
                                string.Format("{0}Depósito(s) {1}", idsAcertoCheques.Count > 0 ? "\n" : string.Empty,
                                    string.Join(", ", lstCheques.Where(f => f.Tipo == 1 && f.IdDeposito > 0).Select(f => f.IdDeposito).ToList())) : string.Empty));
                }
            }

            /* Chamado 52690. */
            if (lstCheques.Any(f => f.Situacao == (int)Cheques.SituacaoCheque.Devolvido))
                throw new Exception(string.Format("Não é possível cancelar este pagamento, o(s) cheque(s) de número {0} foi(ram) devolvido(s). Cancele a devolução deles para, depois, cancelar o pagamento.",
                    string.Join(", ", lstCheques.Where(f => f.Situacao == (int)Cheques.SituacaoCheque.Devolvido).Select(f => f.Num).ToList())));

            bool jurosMultaEstornado = false;

            var lstPagto = PagtoPagtoDAO.Instance.GetByPagto(session, idPagto).ToArray();

            #region Estorna Crédito

            if (FinanceiroConfig.FormaPagamento.CreditoFornecedor)
            {
                List<CaixaGeral> lstCxGeral =
                    new List<CaixaGeral>(CaixaGeralDAO.Instance.GetByPagto(session, idPagto));
                lstCxGeral.Sort(
                    delegate(CaixaGeral x, CaixaGeral y)
                    {
                        int comp = y.DataMov.CompareTo(x.DataMov);
                        if (comp == 0)
                            comp = y.IdCaixaGeral.CompareTo(x.IdCaixaGeral);

                        return comp;
                    }
                    );

                if (pagto.IdFornec > 0)
                    UtilsFinanceiro.ValidaValorCreditoFornecedor(session, (uint)pagto.IdFornec, lstCxGeral);

                foreach (CaixaGeral cx in lstCxGeral)
                {
                    if ((cx.TipoMov == 1 && cx.IdConta !=
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor)) ||
                        (cx.TipoMov == 2 &&
                            cx.IdConta ==
                            UtilsPlanoConta.GetPlanoConta(
                                UtilsPlanoConta.PlanoContas.EstornoPagtoCreditoFornecedor)))
                    {
                        // Se for juros de venda cartão, continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    // Se algum credito tiver sido utilizado, estorna no crédito do fornecedor
                    if (cx.IdConta ==
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor) &&
                        pagto.IdFornec != null)
                    {
                        FornecedorDAO.Instance.CreditaCredito(session, pagto.IdFornec.Value, cx.ValorMov);
                        credito += cx.ValorMov;

                        // Estorna crédito utilizado pelo fornecedor
                        lstCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                            pagto.IdFornec,
                            UtilsPlanoConta.GetPlanoConta(
                                UtilsPlanoConta.PlanoContas.EstornoPagtoCreditoFornecedor), 2,
                            cx.ValorMov, 0, null, null, 0, false, null));
                    }

                    //Se algum credito tiver sido gerado, estorna do crédito do fornecedor.
                    if (cx.IdConta ==
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado) &&
                        pagto.IdFornec != null)
                    {
                        FornecedorDAO.Instance.DebitaCredito(session, pagto.IdFornec.Value, cx.ValorMov);
                        credito -= cx.ValorMov;

                        // Estorna crédito venda gerado
                        lstCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                            pagto.IdFornec,
                            UtilsPlanoConta.GetPlanoConta(
                                UtilsPlanoConta.PlanoContas.EstornoCreditoCompraGerado), 1,
                            cx.ValorMov, 0, null, null, 0, false, null));
                    }
                }
            }

            #endregion

            // Estorna os pagamentos
            foreach (PagtoPagto p in lstPagto)
            {
                // Não estorna os cheques ou crédito no loop (crédito: idFormaPagto = 0);
                if (p.IdFormaPagto == 0 ||
                    /* Chamado 17551. */
                    p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Credito ||
                    p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ||
                    p.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                    continue;

                #region Estorna juros

                // Calcula o total de juros e multa para esta forma de pagto
                decimal jurosMultaPagto = (pagto.Juros + pagto.Multa) / lstPagto.Length;

                // Estorna os juros
                if (p.IdContaBanco > 0)
                {
                    // Gera movimentação de estorno na conta bancária referente aos juros
                    if (pagto.Juros > 0)
                        lstMovBanco.AddRange(EstornoBancario(session, idPagto, p.IdContaBanco.Value,
                            FinanceiroConfig.PlanoContaJurosPagto,
                            FinanceiroConfig.PlanoContaEstornoJurosPagto, estornarMovimentacaoBancaria,
                            dataEstornoBanco, contadorDataUnica));

                    // Gera movimentação de estorno na conta bancária referente à multa
                    if (pagto.Multa > 0)
                        lstMovBanco.AddRange(EstornoBancario(session, idPagto, p.IdContaBanco.Value,
                            FinanceiroConfig.PlanoContaMultaPagto,
                            FinanceiroConfig.PlanoContaEstornoMultaPagto, estornarMovimentacaoBancaria,
                            dataEstornoBanco, contadorDataUnica));
                }
                else if (!jurosMultaEstornado)
                {
                    // Gera uma movimentação de estorno para o juros
                    if (pagto.Juros > 0)
                        lstCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                            pagto.IdFornec, FinanceiroConfig.PlanoContaEstornoJurosPagto, 1,
                            pagto.Juros, 0, null, null, 1, true, null));

                    // Gera uma movimentação de estorno para o juros
                    if ((pagto.Multa / lstPagto.Length) > 0)
                        lstCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                            pagto.IdFornec, FinanceiroConfig.PlanoContaEstornoMultaPagto, 1,
                            pagto.Multa, 0, null, null, 1, true, null));

                    jurosMultaEstornado = true;
                }

                #endregion

                // Recupera o plano de contas
                uint idConta = UtilsPlanoConta.GetPlanoContaEstornoPagto(p.IdFormaPagto);

                // Gera uma movimentação de estorno para cada forma de pagto
                var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null, pagto.IdFornec, idConta, 1,
                    p.ValorPagto - jurosMultaPagto,
                            jurosMultaPagto, null, null, 0,
                            p.IdFormaPagto != (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec &&
                            p.IdFormaPagto != (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta &&
                            (p.IdContaBanco == null || p.IdContaBanco == 0), null);

                objPersistence.ExecuteCommand(session,
                    string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                        contadorDataUnica++, idCaixaGeral));
            }

            if (lstPagto.Any(f => f.IdContaBanco > 0))
            {
                var contasBanco = new List<int>();

                /* Chamado 39565. */
                foreach (var idContaBanco in lstPagto.Where(f => f.IdContaBanco > 0).Select(f => (int)f.IdContaBanco.Value))
                {
                    if (contasBanco.Contains(idContaBanco))
                        continue;
                    else
                        contasBanco.Add(idContaBanco);

                    // Gera movimentação de estorno na conta bancária    
                    lstMovBanco.AddRange(EstornoBancario(session, idPagto, (uint)idContaBanco,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTransfBanco),
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoTransfBancaria),
                        estornarMovimentacaoBancaria, dataEstornoBanco, contadorDataUnica));

                    lstMovBanco.AddRange(EstornoBancario(session, idPagto, (uint)idContaBanco,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoBoleto),
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoBoleto),
                        estornarMovimentacaoBancaria, dataEstornoBanco, contadorDataUnica));
                }
            }

            #region Estorna Cheques

            // Gera movimentação no caixa geral de estorno de cada cheque de terceiro
            foreach (var c in lstCheques.Where(f => f.Tipo == 2))
            {
                // Estorna valor gerado pelo cheque
                var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                    pagto.IdFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoChequeTerceiros),
                    1, c.Valor - c.JurosPagto - c.MultaPagto, c.JurosPagto + c.MultaPagto, null, null, 2, true, null);

                lstCaixaGeral.Add(idCaixaGeral);

                objPersistence.ExecuteCommand(session,
                    string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                        contadorDataUnica++, idCaixaGeral));

                // Estorna valor dos juros pagos pelo cheque
                if (c.JurosPagto > 0)
                {
                    idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                        pagto.IdFornec, FinanceiroConfig.PlanoContaEstornoJurosPagto, 1,
                        c.JurosPagto, 0, null, null, 2, true, null);

                    lstCaixaGeral.Add(idCaixaGeral);

                    objPersistence.ExecuteCommand(session,
                        string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                            contadorDataUnica++, idCaixaGeral));
                }

                // Estorna valor dos juros pagos pelo cheque
                if (c.MultaPagto > 0)
                {
                    idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null,
                        pagto.IdFornec, FinanceiroConfig.PlanoContaEstornoMultaPagto, 1,
                        c.MultaPagto, 0, null, null, 2, true, null);

                    lstCaixaGeral.Add(idCaixaGeral);

                    objPersistence.ExecuteCommand(session,
                        string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                            contadorDataUnica++, idCaixaGeral));
                }
            }

            // Altera situação dos cheques de terceiros utilizados no pagto desta conta para em aberto
            var dados = ChequesDAO.Instance.CancelaChequesPagto(session, idPagto, 2,
                Cheques.SituacaoCheque.EmAberto);
            lstCaixaGeral.AddRange(dados.Key);
            lstMovBanco.AddRange(dados.Value);

            // Cancela cheques próprios utilizados no pagamento desta conta
            dados = ChequesDAO.Instance.CancelaChequesPagto(session, idPagto, 1,
                Cheques.SituacaoCheque.Cancelado);
            lstCaixaGeral.AddRange(dados.Key);
            lstMovBanco.AddRange(dados.Value);

            #endregion

            // Exclui contas a pagar geradas neste pagto
            geradas = ContasPagarDAO.Instance.GetRenegociadasPagto(session, idPagto);
            objPersistence.ExecuteCommand(session, "Delete from contas_pagar Where idConta=" +
                                                        UtilsPlanoConta.GetPlanoConta(
                                                            UtilsPlanoConta.PlanoContas.PagtoRenegociacao) +
                                                        " And idPagtoRestante=" + idPagto);

            // Exclui conta a pagar restante gerada por este pagamento
            objPersistence.ExecuteCommand(session, "Delete from contas_pagar Where idPagtoRestante=" + idPagto);

            // Volta situação das contas a pagar para estado inicial
            pagas = ContasPagarDAO.Instance.GetByPagto(session, idPagto);
            objPersistence.ExecuteCommand(session, @"UPDATE contas_pagar SET Desconto=0, DataPagto=null, Paga=0, ValorPago=0, Juros=0, Multa=0,
                IdPagto=IF(IdCheque>0, IdPagtoRestante, NULL), IdChequePagto=null, DestinoPagto=null WHERE IdPagto=" + idPagto);

            string idsContasPg = "", valoresPg = "", idsChequesPg = "";
            foreach (ContasPagar c in pagas)
            {
                idsContasPg += c.IdContaPg + ",";
                valoresPg += c.ValorPago.ToString().Replace(".", "").Replace(",", ".") + ",";
            }

            foreach (Cheques c in lstCheques)
                idsChequesPg += c.IdCheque + ",";

            // Atualiza situação e motivo do cancelamento do pagto
            objPersistence.ExecuteCommand(session,
                "Update pagto set situacao=" + (int) Pagto.SituacaoPagto.Cancelado +
                ", motivoCanc=?motivo, idsContasPg=?idsContasPg, valoresPg=?valoresPg, idsChequesPg=?idsChequesPg Where idPagto=" +
                idPagto,
                new GDAParameter("?motivo",
                    motivoCanc.Length > 500 ? motivoCanc.Substring(0, 500) : motivoCanc),
                new GDAParameter("?idsContasPg", idsContasPg.TrimEnd(',')),
                new GDAParameter("?valoresPg", valoresPg.TrimEnd(',')),
                new GDAParameter("?idsChequesPg", idsChequesPg.TrimEnd(',')));

            // Corrige o saldo das antecipações de fornecedor utilizadas
            foreach (var id in lstPagto.Where(x => x.IdAntecipFornec > 0).Select(x => x.IdAntecipFornec.Value))
            {
                lstAntecipFornec.Add(AntecipacaoFornecedorDAO.Instance.GetElementByPrimaryKey(session, id));

                decimal cred;
                var idMov = AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(session, id, out cred).Key;
                credito += cred;

                if (idMov > 0)
                    lstCaixaGeral.Add(idMov.Value);
            }
        }

        #endregion

        #region Estorno bancário

        /// <summary>
        /// Efetua estorno bancário de pagto
        /// </summary>
        private uint[] EstornoBancario(uint idPagto, uint idContaBanco, uint idConta, uint idContaEstorno,
            DateTime? dataEstornoBanco)
        {
            return EstornoBancario(null, idPagto, idContaBanco, idConta, idContaEstorno, dataEstornoBanco, null);
        }

        /// <summary>
        /// Efetua estorno bancário de pagto
        /// </summary>
        private uint[] EstornoBancario(GDASession session, uint idPagto, uint idContaBanco, uint idConta, uint idContaEstorno,
            DateTime? dataEstornoBanco, int? contadorDataUnica)
        {
            return EstornoBancario(session, idPagto, idContaBanco, idConta, idContaEstorno, false, dataEstornoBanco, contadorDataUnica);
        }

        /// <summary>
        /// Efetua estorno bancário de pagto
        /// </summary>
        private uint[] EstornoBancario(GDASession session, uint idPagto, uint idContaBanco, uint idConta, uint idContaEstorno, bool estornarMovimentacaoBancaria,
            DateTime? dataEstornoBanco, int? contadorDataUnica)
        {
            if (!estornarMovimentacaoBancaria)
            {
                // Pega a primeira movimentação da conta bancária referente ao pagto
                object obj = objPersistence.ExecuteScalar(session, "Select idMovBanco from mov_banco Where idContaBanco=" + idContaBanco +
                    " And idPagto=" + idPagto + " order by idMovBanco asc limit 1");

                uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                if (idMovBanco == 0)
                    return new uint[0];

                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idMovBanco);

                MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(session, idMovBanco);

                // Corrige saldo
                objPersistence.ExecuteCommand(session, string.Format(
                    "Update mov_banco Set valorMov=0, DataUnica=Concat(DataUnica, {0}) Where idConta=" + idConta + " And idPagto=" + idPagto, 
                    contadorDataUnica != null ? "''": contadorDataUnica.Value.ToString()));
                
                if (movAnterior != null) MovBancoDAO.Instance.CorrigeSaldo(session, movAnterior.IdMovBanco, idMovBanco);

                // Exclui movimentações geradas
                objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idConta=" + idConta + " And idPagto=" + idPagto);

                return new uint[0];
            }
            else
            {
                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idContaBanco, dataEstornoBanco.Value);

                List<uint> lst = new List<uint>();
                Pagto pagto = GetPagto(session, idPagto);

                foreach (MovBanco m in MovBancoDAO.Instance.GetByPagto(session, idPagto, idConta))
                {
					// Condição necessária para não estornar a conta mais de uma vez, no caso de efetuar um pagamento com duas ou mais 
                    // formas de pagamento que envolvem conta bancária (pagto. bancário)
                    if (idContaBanco == m.IdContaBanco)
                        lst.Add(ContaBancoDAO.Instance.MovContaPagto(session, idContaBanco, idContaEstorno, (int)UserInfo.GetUserInfo.IdLoja,
                            idPagto, null, pagto.IdFornec, m.TipoMov == 1 ? 2 : 1, m.ValorMov, m.Juros, dataEstornoBanco.Value, m.Obs));
                }

                return lst.ToArray();
            }
        }

        #endregion

        #region Busca pagamentos

        private string SqlList(uint idPagto, uint idCompra, uint idFornec, string nomeFornec, string dataIni, string dataFim, 
            float valorInicial, float valorFinal, int situacao, uint numeroNfe, uint idCustoFixo, uint idImpostoServ, string obs, bool selecionar)
        {
            string campos = selecionar ? $@"p.*, func.Nome as NomeFuncPagto, Coalesce(forn.NomeFantasia, forn.RazaoSocial) as NomeFornec, 
                Concat(cb.Nome, ' Agência: ', cb.Agencia, ' Conta: ', cb.Conta) as DescrContaBanco, 
                group_concat(concat(if(fp.idFormaPagto <> {(int)Pagto.FormaPagto.Deposito}, fp.descricao, 'Pagto. Bancário'), if(pp.idAntecipFornec is not null, concat(': ', pp.idAntecipFornec), ''), 
                if(cb.idContaBanco is not null, Concat(' (Banco: ', cb.Nome, ' Agência: ', cb.Agencia, ' Conta: ', 
                cb.Conta, ')'), ''), if(length(coalesce(pp.numBoleto,''))>0, concat(' Num. ', pp.numBoleto), ''), if(pp.dataPagto is not null, 
                concat(' Data: ', date_format(pp.dataPagto, '%d/%m/%Y')), '')) separator ', ') as descrFormaPagto, 
                cast(group_concat(pp.valorPagto separator ';') as char) as valoresPagos" : "Count(*)";

            string sql = $@"
                Select {campos}
                From pagto p 
                    Left Join pagto_pagto pp On (p.idPagto=pp.idPagto)
                    Left Join formapagto fp On (pp.idFormaPagto=fp.idFormaPagto)
                    Left Join funcionario func On (p.idFuncPagto=func.idFunc)
                    Left Join fornecedor forn On (p.idFornec=forn.idFornec)  
                    Left Join conta_banco cb On (pp.idContaBanco=cb.idContaBanco) 
                Where 1";

            if (idPagto > 0)
            {
                sql += $" And p.idPagto= {idPagto} ";
            }

            if (idFornec > 0)
            {
                sql += $" And p.IdPagto In (Select idPagto From contas_pagar Where idFornec = {idFornec})";
            }
            else if (!string.IsNullOrWhiteSpace(nomeFornec))
            {
                sql += @" And p.IdPagto In (Select cp.idPagto From contas_pagar cp 
                    Inner Join fornecedor fn On (cp.idFornec=fn.idFornec) 
                    Where fn.NomeFantasia Like ?nomeFornec Or fn.RazaoSocial Like ?nomeFornec)";
            }

            if (idCompra > 0)
            {
                sql += $" And p.IdPagto In (Select idPagto From contas_pagar Where idCompra = {idCompra})";
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += " And p.DataPagto >= ?dataIni";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " And p.DataPagto <= ?dataFim";
            }

            if (valorInicial > 0)
            {
                sql += $" And p.valorPago >= {valorInicial.ToString().Replace(',', '.')}";
            }

            if (valorFinal > 0)
            {
                sql += $" And p.valorPago <= {valorFinal.ToString().Replace(',', '.')}";
            }

            if (situacao > 0)
            {
                sql += $" And p.situacao = {situacao}";
            }

            if (numeroNfe > 0)
            {
                sql += $" And p.idPagto In (Select idPagto From contas_pagar Where idNf In (Select idNf From nota_fiscal Where numeroNfe = {numeroNfe}))";
            }

            if (idCustoFixo > 0)
            {
                sql += $" And p.idPagto in (Select idPagto from contas_pagar Where IdCustoFixo = {idCustoFixo})";
            }

            if (idImpostoServ > 0)
            {
                sql += $" And p.idPagto in (Select idPagto from contas_pagar Where idImpostoServ = {idImpostoServ})";
            }

            if (!string.IsNullOrWhiteSpace(obs))
            {
                sql += " And p.obs Like ?obs";
            }

            if (selecionar)
            {
                sql += " group by p.idPagto";
            }

            return sql;
        }

        public Pagto GetPagto(uint idPagto)
        {
            return GetPagto(null, idPagto);
        }

        public Pagto GetPagto(GDASession session, uint idPagto)
        {
            List<Pagto> pagto = objPersistence.LoadData(session, SqlList(idPagto, 0, 0, null, null, null, 0, 0, 0, 0, 0, 0, null, true));
            return pagto.Count > 0 ? pagto[0] : null;
        }

        public IList<Pagto> GetList(uint idPagto, uint idCompra, uint idFornec, string nomeFornec, string dataIni, string dataFim, float valorInicial, float valorFinal,
            int situacao, uint numeroNfe, uint idCustoFixo, uint idImpostoServ, string obs, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "p.IdPagto Desc" : sortExpression;
            return LoadDataWithSortExpression(SqlList(idPagto, idCompra, idFornec, nomeFornec, dataIni, dataFim, valorInicial, valorFinal,
                situacao, numeroNfe, idCustoFixo, idImpostoServ, obs, true), filtro, startRow, pageSize, GetParam(nomeFornec, obs, dataIni, dataFim));
        }

        public int GetCount(uint idPagto, uint idCompra, uint idFornec, string nomeFornec, string dataIni, string dataFim, float valorInicial, float valorFinal,
            int situacao, uint numeroNfe, uint idCustoFixo, uint idImpostoServ, string obs)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idPagto, idCompra, idFornec, nomeFornec, dataIni, dataFim, valorInicial, valorFinal,
                situacao, numeroNfe, idCustoFixo, idImpostoServ, obs, false), GetParam(nomeFornec, obs, dataIni, dataFim));
        }

        public IList<Pagto> GetListSel(uint idFornec, string nomeFornec, string dataIni, string dataFim,
            float valorInicial, float valorFinal, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "p.IdPagto Desc" : sortExpression;
            return LoadDataWithSortExpression(SqlList(0, 0, idFornec, nomeFornec, dataIni, dataFim, valorInicial, valorFinal, 
                (int)Pagto.SituacaoPagto.Finalizado, 0, 0, 0, null, true), filtro, startRow, pageSize, GetParam(nomeFornec, null, dataIni, dataFim));
        }

        public int GetCountSel(uint idFornec, string nomeFornec, string dataIni, string dataFim, float valorInicial, float valorFinal)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(0, 0, idFornec, nomeFornec, dataIni, dataFim, valorInicial, valorFinal,
                (int)Pagto.SituacaoPagto.Finalizado, 0, 0, 0, null, false), GetParam(nomeFornec, null, dataIni, dataFim));
        }

        private GDAParameter[] GetParam(string nomeFornec, string obs, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca para Log

        private string FormatCheque(Cheques c)
        {
            return String.Format("* Núm.: {0} Banco: {1} Agência: {2} Conta: {3} Valor: {4:C} Venc.: {5:dd/MM/yyyy}\n",
                c.Num, c.Banco, c.Agencia, c.Conta, c.Valor, c.DataVenc);
        }

        private string FormatContaPagar(GDASession session, ContasPagar c)
        {
            return String.Format("* Cód.: {0} Ref.: ({1}) Valor: {2:C} Venc.: {3:dd/MM/yyyy} Juros: {4:C} Multa: {5:C}{6}\n",
                c.IdContaPg, c.Referencia.Trim(), c.ValorVenc, c.DataVenc, c.Juros, c.Multa, c.IdChequePagto > 0 && ChequesDAO.Instance.ExisteCheque(session, c.IdChequePagto.Value) ?
                " Cheque: (" + FormatCheque(ChequesDAO.Instance.GetForLogPagto(session, c.IdChequePagto.Value)).TrimEnd('\n') + ")" : "");
        }

        /// <summary>
        /// Busca um pagamento com dados para Log.
        /// </summary>
        public Pagto GetForLog(uint idPagto)
        {
            return GetForLog(null, idPagto);
        }

        /// <summary>
        /// Busca um pagamento com dados para Log.
        /// </summary>
        public Pagto GetForLog(GDASession session, uint idPagto)
        {
            Pagto retorno = GetElementByPrimaryKey(session, idPagto);

            string cheques = "";
            foreach (Cheques c in ChequesDAO.Instance.GetByPagto(session, idPagto))
                cheques += FormatCheque(c);

            string contas = "";
            foreach (ContasPagar c in ContasPagarDAO.Instance.GetByPagto(session, idPagto))
                contas += FormatContaPagar(session, c);

            string formasPagto = "";
            foreach (PagtoPagto p in PagtoPagtoDAO.Instance.GetByPagto(session, idPagto))
            {
                formasPagto += String.Format("* {0} {1:C}\n",
                    p.DescrFormaPagto, p.ValorPagto);
            }

            retorno.ChequesLog = cheques.TrimEnd('\n');
            retorno.ContasLog = contas.TrimEnd('\n');
            retorno.FormasPagtoLog = formasPagto.TrimEnd('\n');

            return retorno;
        }

        #endregion

        #region Obtém dados do pagamento

        public int ObtemSituacao(uint idPagto)
        {
            return ObtemSituacao(null, idPagto);
        }

        public int ObtemSituacao(GDASession session, uint idPagto)
        {
            return ObtemValorCampo<int>(session, "Situacao", string.Format("IdPagto={0}", idPagto));
        }

        #endregion

        #region Verifica se um pagamento existe

        /// <summary>
        /// Verifica se um pagamento existe.
        /// </summary>
        /// <param name="idPagto"></param>
        /// <returns></returns>
        public new bool Exists(uint idPagto)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from pagto where idPagto=" + idPagto) > 0;
        }

        #endregion

        /// <summary>
        /// Retorna a situação da compra
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string GetSituacaoCompra(int situacao)
        {
            switch (situacao)
            {
                case (int)Compra.SituacaoEnum.Ativa:
                    return "Ativa";
                case (int)Compra.SituacaoEnum.Cancelada:
                    return "Cancelada";
                case (int)Compra.SituacaoEnum.EmAndamento:
                    return "Em Andamento";
                case (int)Compra.SituacaoEnum.Finalizada:
                    return "Finalizada";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Retorna o id da forma de pagamento informada
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public uint GetIdFormaPagto(string descrFormaPagto)
        {
            switch (descrFormaPagto)
            {
                case "dinheiro":
                    return (uint)Model.Pagto.FormaPagto.Dinheiro;
                case "cheque proprio":
                    return (uint)Model.Pagto.FormaPagto.ChequeProprio;
                case "construcard":
                    return (uint)Model.Pagto.FormaPagto.Construcard;
                case "boleto":
                    return (uint)Model.Pagto.FormaPagto.Boleto;
                case "cartao":
                    return (uint)Model.Pagto.FormaPagto.Cartao;
                case "permuta":
                    return (uint)Model.Pagto.FormaPagto.Permuta;
                case "deposito":
                    return (uint)Model.Pagto.FormaPagto.Deposito;
                case "prazo":
                    return (uint)Model.Pagto.FormaPagto.Prazo;
                default:
                    throw new Exception("Forma de pagamento não definida.");
            }
        }

        /// <summary>
        /// Retorna a descricao da forma de pagamento informada
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public string GetDescrFormaPagto(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Dinheiro:
                    return "dinheiro";
                case (uint)Pagto.FormaPagto.ChequeProprio:
                    return "cheque proprio";
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    return "cheque terceiros";
                case (uint)Pagto.FormaPagto.Construcard:
                    return "construcard";
                case (uint)Pagto.FormaPagto.Boleto:
                    return "boleto";
                case (uint)Pagto.FormaPagto.Cartao:                
                    return "cartao";
                case (uint)Pagto.FormaPagto.Permuta:
                    return "permuta";
                case (uint)Pagto.FormaPagto.Deposito:
                    return "deposito";
                case (uint)Pagto.FormaPagto.Prazo:
                    return "prazo";
                case (uint)Pagto.FormaPagto.DepositoNaoIdentificado:
                    return "deposito nao identificado";
                case (uint)Pagto.FormaPagto.AntecipFornec:
                    return "antecipacao de fornecedor";     
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    return "cartao nao identificado";
                default:
                    throw new Exception("Forma de pagamento não definida.");
            }
        }

        /// <summary>
        /// Obtem os pagamentos para API do sistema mobile
        /// </summary>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        public object ObterPagamentosParaApi(DateTime dataIni, DateTime dataFim)
        {
            var sql = @"
                SELECT CONCAT(Date(p.DataPagto), ';', pp.IdFormaPagto, ';', CAST(SUM(pp.ValorPagto) as char))
                FROM pagto p
	                INNER JOIN pagto_pagto pp ON (p.IdPagto = pp.IdPagto)
                WHERE Date(p.DataPagto) >= ?dtIni AND Date(p.DataPagto) <= ?dtFim
                GROUP BY Date(p.DataPagto), pp.IdFormaPagto";

            var dados = ExecuteMultipleScalar<string>(sql, new GDAParameter("?dtIni", dataIni), new GDAParameter("?dtFim", dataFim));

            var retorno = dados.Select(f => new
            {
                Data = DateTime.Parse(f.Split(';')[0]).ToShortDateString(),
                FormaPagto = Colosoft.Translator.Translate(((Pagto.FormaPagto)f.Split(';')[1].StrParaInt())).Format(),
                Valor = f.Split(';')[2].StrParaDecimal()
            })
            .ToList();

            return retorno;
        }
    }
}