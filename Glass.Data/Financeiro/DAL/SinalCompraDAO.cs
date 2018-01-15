using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class SinalCompraDAO : BaseDAO<SinalCompra, SinalCompraDAO>
    {
        //private SinalCompraDAO() { }

        #region Busca Sinais

        private string SqlList(uint idSinal, uint idCompra, uint idFornec, uint idFunc, string dataIni, string dataFim, uint idFormaPagto,
             bool cancelado, bool selecionar)
        {
            string campos = selecionar ? @"s.*, f.NomeFantasia as NomeFornec, fun.Nome as Funcionario" : "count(distinct s.idSinalCompra)";

            string sql = @"
                Select " + campos + @" 
                From sinal_compra s
                    Left Join fornecedor f on (s.IdFornec = f.idFornec) 
                    Left Join funcionario fun On (s.UsuCad=fun.IdFunc) 
                Where 1";

            if (idSinal > 0)
                sql += " And s.IdSinalCompra=" + idSinal;

            if (idCompra > 0)
                sql += " And s.idSinalCompra in (select idSinalCompra from compra where idCompra=" + idCompra + ")";

            if (idFornec > 0)
                sql += " And s.IdFornec=" + idFornec;

            if (idFunc > 0)
                sql += " And s.usuCad=" + idFunc;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " And s.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " And s.DataCad<=?dataFim";

            if (cancelado)
                sql += " And s.cancelado=true";

            return sql;
        }

        public void PreencheTotal(ref List<SinalCompra> lstSinal)
        {
            PreencheTotal(null, ref lstSinal);
        }

        public void PreencheTotal(GDASession session, ref List<SinalCompra> lstSinal)
        {
            string sql = @"
                            Select cast(Sum(psc.valorPagto) as decimal(12,2)) 
                            From pagto_sinal_compra psc
                            Where psc.idSinalCompra=?idSinalCompra 
                            Group By psc.idSinalCompra";

            foreach (SinalCompra s in lstSinal)
            {
                object totalSinal = objPersistence.ExecuteScalar(session, sql.Replace("?idSinalCompra", s.IdSinalCompra.ToString()));

                if (totalSinal != null && totalSinal.ToString() != String.Empty)
                    s.TotalSinal = Glass.Conversoes.StrParaDecimal(totalSinal.ToString());

                s.TotalSinal += s.CreditoUtilizadoCriar.GetValueOrDefault(0);
            }
        }

        /// <summary>
        /// Retorna todos os acertos do fornecedor passado
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public IList<SinalCompra> GetByFornecRpt(uint idFornec)
        {
            var lstSinal = objPersistence.LoadData(SqlList(0, 0, idFornec, 0, null, null, 0, false, true) + " Order By s.idSinalCompra desc").ToList();

            PreencheTotal(ref lstSinal);

            return lstSinal;
        }

        public IList<SinalCompra> GetList(uint idSinal, uint idCompra, uint idFornec, string dataIni, string dataFim, uint idFormaPagto,
             string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "s.idSinalCompra desc" : sortExpression;
            var lstSinal = LoadDataWithSortExpression(SqlList(idSinal, idCompra, idFornec, 0, dataIni, dataFim, idFormaPagto, false, true),
                filtro, startRow, pageSize, GetParam(dataIni, dataFim)).ToList();

            PreencheTotal(ref lstSinal);

            return lstSinal;
        }

        public int GetCount(uint idSinal, uint idCompra, uint idFornec, string dataIni, string dataFim, uint idFormaPagto)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idSinal, idCompra, idFornec, 0, dataIni, dataFim, idFormaPagto, false, false), GetParam(dataIni, dataFim));
        }
        
        /// <summary>
        /// Retorna dados do Sinal
        /// </summary>
        public SinalCompra GetSinalCompraDetails(GDASession session, uint idSinalCompra)
        {
            string formaPagto = String.Empty;

            // Busca movimentações relacionadas a este sinal e agrupadas pela forma de pagto
            foreach (PagtoSinalCompra ps in PagtoSinalCompraDAO.Instance.GetBySinalCompra(session, idSinalCompra))
                formaPagto += ps.DescrFormaPagto + ": " + ps.ValorPagto.ToString("C") +
                    (ps.IdContaBanco > 0 ? " (" + ContaBancoDAO.Instance.GetDescricao(session, ps.IdContaBanco.Value) + ")" : "") + "\n";

            // Retorna o acerto, apenas um registro deverá ser retornado
            var lst = objPersistence.LoadData(session, SqlList(idSinalCompra, 0, 0, 0, null, null, 0, false, true)).ToList();

            PreencheTotal(session, ref lst);

            if (lst.Count() > 0)
            {
                lst[0].FormaPagto = formaPagto.TrimEnd('\n');
                return lst[0];
            }
            else
                return null;
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni = dataIni + " 00:00") : DateTime.Parse(dataIni))));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim = dataFim + " 23:59:59") : DateTime.Parse(dataFim))));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Busca apenas sinais abertos que tem mais de uma compra vinculada.
        /// </summary>
        /// <param name="isPagtoAntecipado"></param>
        /// <returns></returns>
        public IList<SinalCompra> GetForRetificar()
        {
            string sql = SqlList(0, 0, 0, 0, null, null, 0, false, true);
            var sinais = objPersistence.LoadData(sql + " and (select count(*) from Compra where idSinalCompra=s.idSinalCompra)>1 order by idSinalCompra desc").ToList();

            PreencheTotal(ref sinais);

            return sinais;
        }

        #endregion

        #region Cria Sinal Compra

        private static object _pagarLock = new object();

        /// <summary>
        /// Cria o sinal da compra
        /// </summary>
        public uint Pagar(string idsComprasStr, decimal[] valores, uint[] formasPagto, uint[] contasBanco,
            uint[] tiposCartao, uint[] numParcCartao, DateTime[] datasFormasPagto, string obs, string[] boletos, bool gerarCredito,
            decimal creditoUtilizado, string cheques, uint idFornec)
        {
            lock(_pagarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        //Valida o pagamento do sinal
                        ValidaSinalCompra(transaction, idsComprasStr, valores, creditoUtilizado, gerarCredito);

                        var compras = CompraDAO.Instance.GetByString(transaction, idsComprasStr).ToArray();
                        decimal totalASerPago = 0;
                        uint idContaPagtoSinal = 0;

                        foreach (Compra com in compras)
                            totalASerPago += com.ValorEntrada;

                        decimal totalSinal = 0;

                        for (int i = 0; i < valores.Length; i++)
                        {
                            if (valores[i] <= 0)
                                continue;

                            totalSinal += valores[i];
                        }

                        #region Gera o sinal da compra

                        SinalCompra sc = new SinalCompra();
                        sc.IdFornec = idFornec;
                        sc.ValorCreditoAoCriar = FornecedorDAO.Instance.GetCredito(transaction, idFornec);
                        sc.CreditoUtilizadoCriar = creditoUtilizado;
                        sc.CreditoGeradoCriar = gerarCredito ? totalSinal - totalASerPago : 0;
                        sc.Obs = obs;
                        sc.IdsCompras = idsComprasStr;
                        sc.Valores = String.Join(";", new List<decimal>(valores).ConvertAll(i => i.ToString()).ToArray());
                        sc.UsuCad = UserInfo.GetUserInfo.CodUser;
                        sc.DataCad = DateTime.Now;

                        #endregion

                        //insere o sinal da compra
                        sc.IdSinalCompra = Insert(sc);

                        #region Salva o pagamento do sinal

                        uint idConta = 0;

                        int numPagto = 1;

                        for (int i = 0; i < valores.Length; i++)
                        {
                            if (valores[i] == 0)
                                continue;

                            if (idConta == 0)
                                idConta = UtilsPlanoConta.GetPlanoContaPagto(formasPagto[i]);

                            PagtoSinalCompra pagtoSinal = new PagtoSinalCompra();
                            pagtoSinal.IdSinalCompra = sc.IdSinalCompra;
                            pagtoSinal.NumFormaPagto = numPagto++;
                            pagtoSinal.ValorPagto = valores[i];
                            pagtoSinal.IdFormaPagto = formasPagto[i];
                            pagtoSinal.IdContaBanco = contasBanco[i] > 0 ? (uint?)contasBanco[i] : null;
                            pagtoSinal.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;

                            PagtoSinalCompraDAO.Instance.Insert(transaction, pagtoSinal);
                        }

                        if (creditoUtilizado > 0)
                            PagtoSinalCompraDAO.Instance.Insert(transaction, new PagtoSinalCompra()
                            {
                                IdSinalCompra = sc.IdSinalCompra,
                                NumFormaPagto = numPagto++,
                                IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito,
                                ValorPagto = creditoUtilizado
                            });

                        #endregion

                        //Atualiza as compras para colocar o idsinal
                        objPersistence.ExecuteCommand(transaction, "UPDATE compra SET idSinalCompra=" + sc.IdSinalCompra + " WHERE idCompra IN(" + idsComprasStr + ")");

                        DateTime dataPagamento = DateTime.Now;

                        #region Gera o conta paga referente ao sinal da compra

                        if (creditoUtilizado > 0)
                            totalSinal += creditoUtilizado;

                        ContasPagar contaPagSinal = new ContasPagar();
                        contaPagSinal.IdSinalCompra = sc.IdSinalCompra;
                        contaPagSinal.IdFornec = idFornec;
                        contaPagSinal.IdConta = idConta == 0 && creditoUtilizado > 0 ? UtilsPlanoConta.GetPlanoContaPagto((int)Glass.Data.Model.Pagto.FormaPagto.Credito) : idConta;
                        contaPagSinal.DataVenc = dataPagamento;
                        contaPagSinal.ValorVenc = totalSinal;
                        contaPagSinal.NumParc = 1;
                        contaPagSinal.NumParcMax = 1;
                        contaPagSinal.Paga = true;
                        contaPagSinal.DataPagto = dataPagamento;
                        contaPagSinal.ValorPago = totalSinal;
                        contaPagSinal.IdLoja = contaPagSinal.IdLoja == 0 ? null : contaPagSinal.IdLoja;

                        idContaPagtoSinal = ContasPagarDAO.Instance.Insert(transaction, contaPagSinal);

                        PagarContas(transaction, sc.IdSinalCompra, idContaPagtoSinal, idFornec, valores, formasPagto, contasBanco, tiposCartao,
                            numParcCartao, boletos, cheques, dataPagamento, datasFormasPagto, obs, gerarCredito,
                            creditoUtilizado, totalSinal, totalASerPago);

                        #endregion

                        transaction.Commit();
                        transaction.Close();

                        return sc.IdSinalCompra;
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

        private void PagarContas(GDASession session, uint idSinalCompra, uint idPagto, uint idFornec, decimal[] valores, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao,
            uint[] numParcCartao, string[] boletos, string chequesPagto, DateTime dataPagto, DateTime[] datasFormasPagto,
            string obs, bool gerarCredito, decimal creditoUtilizado, decimal totalPago, decimal totalASerPago)
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

                if (FinanceiroConfig.FormaPagamento.DatasDiferentesFormaPagto)
                    dataUsar = datasFormasPagto != null && datasFormasPagto.Length >= i && datasFormasPagto[i] != null &&
                        datasFormasPagto[i].Ticks > 0 ? datasFormasPagto[i] : dataPagto;

                #region Dinheiro

                // Se a forma de pagto for Dinheiro, gera movimentação no caixa geral
                if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                {
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                        UtilsPlanoConta.GetPlanoContaPagto((uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro), 2, valores[i], obs, 1, true, null));
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
                    pagtoCheque.IdSinalCompra = idSinalCompra;

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
                                    cheque.IdSinalCompra = idSinalCompra;

                                    if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                        cheque.DataReceb = dataPagto;

                                    cheque.IdCheque = ChequesDAO.Instance.InsertBase(session, cheque);

                                    if (cheque.IdCheque < 1)
                                       throw new Exception("retorno do insert do cheque=0");

                                    // Adiciona este cheque à lista de cheques inseridos
                                    lstChequesInseridos.Add(cheque);

                                    // Gera movimentação no caixa geral de cada cheque, mas sem alterar o saldo, 
                                    // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral
                                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoChequeProprio), 2,
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
                                    lstIdMovBanco.Add(ContaBancoDAO.Instance.MovContaChequeSinalCompra(session, c.IdContaBanco.Value,
                                        UtilsPlanoConta.GetPlanoContaPagto(2), (int)UserInfo.GetUserInfo.IdLoja, c.IdCheque, idSinalCompra, idFornec, 2, c.Valor, dataUsar));
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

                                    // Gera movimentação no caixa geral de cada cheque, mas sem alterar o saldo
                                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoChequeTerceiros), 2,
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
                            /* Chamado 23596.
                             * Associa o sinal da compra aos cheques. */
                            if (!string.IsNullOrEmpty(idsChequeTerc))
                                objPersistence.ExecuteCommand(session,
                                    string.Format("UPDATE cheques SET IdSinalCompra={0} WHERE IdCheque IN ({1});",
                                        idSinalCompra, idsChequeTerc.TrimEnd(',')));
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
                    uint idConta = formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito ? UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTransfBanco) :
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoBoleto);

                    // Salva o pagto. bancário no Cx. Geral
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                        idConta, 2, valores[i], obs, 0, false, dataUsar));

                    // Gera movimentação de saída na conta bancária
                    lstIdMovBanco.Add(ContaBancoDAO.Instance.MovContaSinalCompra(session, idContasBanco[i],
                        idConta, (int)UserInfo.GetUserInfo.IdLoja, idSinalCompra, null, idFornec, 2, valores[i], dataUsar, obs));
                }

                #endregion

                #region Permuta

                else if (formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                {
                    lstIdCaixaGeral.Add(CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                       UtilsPlanoConta.GetPlanoContaPagto((uint)Glass.Data.Model.Pagto.FormaPagto.Permuta), 2, valores[i], obs, 0, false, dataUsar));
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
                    idCreditoUtilizado = CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor),
                        1, creditoUtilizado, null, 0, false, null);

                    // Debita Crédito com o fornecedor
                    FornecedorDAO.Instance.DebitaCredito(session, idFornec, creditoUtilizado);
                }

                // Se tiver sido gerado algum crédito para com o fornecedor
                if (gerarCredito && totalPago > totalASerPago)
                {
                    decimal valorCreditoGerado = totalPago - totalASerPago;
                    idCreditoGerado = CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, idFornec,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado),
                        2, valorCreditoGerado, null, 0, false, null);

                    // Credita crédito do fornecedor
                    FornecedorDAO.Instance.CreditaCredito(session, idFornec, valorCreditoGerado);
                }
            }

            #endregion
        }

        #region Validações

        /// <summary>
        /// Valida as compras para o pagamento.
        /// </summary>
        public void ValidaSinalCompra(GDASession session, string idsCompras, decimal[] valores, decimal creditoUtilizado, bool gerarCredito)
        {
            try
            {
                var compras = CompraDAO.Instance.GetByString(session, idsCompras).ToArray();

                decimal totalSinal = 0;
                foreach (decimal valor in valores)
                    totalSinal += valor;

                // Se for pago com crédito, soma o mesmo ao totalPago
                if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && creditoUtilizado > 0)
                    totalSinal += creditoUtilizado;

                decimal totalASerPago = 0;

                // Valida os pedidos
                foreach (Compra com in compras)
                {
                    totalASerPago += com.ValorEntrada;

                    if (com.Situacao == Compra.SituacaoEnum.Finalizada)
                        throw new Exception("A compra " + com.IdCompra + " já está finalizada.");

                    if (com.Situacao == Compra.SituacaoEnum.Cancelada)
                        throw new Exception("A compra " + com.IdCompra + " está cancelada.");

                    if (com.RecebeuSinal)
                        throw new Exception("O sinal da compra " + com.IdCompra + " já foi pago.");

                    if (!CompraDAO.Instance.TemSinalPagar(session, com.IdCompra))
                        throw new Exception("A compra " + com.IdCompra + " não tem sinal a receber.");

                    if (FornecedorDAO.Instance.ObtemSituacao(session, com.IdFornec.GetValueOrDefault()) != (int)SituacaoFornecedor.Ativo)
                        throw new Exception("O fornecedor da compra " + com.IdCompra + " está inativo.");
                }

                // Verifica se o cheque para pagar o sinal foi cadastrado
                //if (UtilsFinanceiro.ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, formasPagto) && String.IsNullOrEmpty(chequesPagto))
                //  throw new Exception("Cadastre o(s) cheque(s) referente(s) ao sinal da compra.");

                // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lanÃ§a exceÃ§Ã£o
                if (gerarCredito && Math.Round(totalSinal, 2) < Math.Round(totalASerPago, 2))
                    throw new Exception("Valor do sinal não confere com valor pago. Valor pago: " + Math.Round(totalSinal, 2).ToString("C") + " Valor do sinal: " + Math.Round(totalASerPago, 2).ToString("C"));

                // Se o total a ser pago for diferente do valor pago, considerando que nÃ£o Ã© para gerar crÃ©dito, apenas para empresas que nÃ£o liberam pedido
                else if (!gerarCredito && Math.Round(totalSinal, 2) != Math.Round(totalASerPago, 2))
                    throw new Exception("Valor do sinal não confere com valor pago. Valor pago: " + Math.Round(totalSinal, 2).ToString("C") + " Valor do sinal: " + Math.Round(totalASerPago, 2).ToString("C"));

                // Se o total a ser pago for menor que o valor pago, apenas para empresas que liberam pedido
                else if (!gerarCredito && Math.Round(totalSinal, 2) < Math.Round(totalASerPago, 2))
                    throw new Exception("Valor do sinal não pode ser menor que o valor pago. Valor pago: " + Math.Round(totalSinal, 2).ToString("C") + " Valor do sinal: " + Math.Round(totalASerPago, 2).ToString("C"));

                foreach (string idCompra in idsCompras.Split(','))
                    if (!String.IsNullOrEmpty(idCompra) && CompraDAO.Instance.ObtemTipoCompra(session, Glass.Conversoes.StrParaUint(idCompra)) != (int)Compra.TipoCompraEnum.APrazo)
                        throw new Exception("O tipo de pagamento da compra" + idCompra + " não é á prazo. Altere o tipo de compra para à prazo para receber um sinal da mesma.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Obtem Campos

        /// <summary>
        /// Retorna todos os ids das compras do sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public string ObtemIdsCompras(uint idSinalCompra)
        {
            string sql = @"Select Cast(group_concat(idCompra) as char) From compra Where idSinalCompra=" + idSinalCompra + " Group by idCompra";

            object ids = objPersistence.ExecuteScalar(sql);

            return ids != null ? ids.ToString() : String.Empty;
        }

        /// <summary>
        /// Obtém o id do cliente
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public uint ObtemIdFornec(uint idSinal)
        {
            string sql = "Select idFornec From sinal_compra Where idSinalCompra=" + idSinal;

            object idFornec = objPersistence.ExecuteScalar(sql);

            return idFornec != null && idFornec.ToString() != String.Empty ? Glass.Conversoes.StrParaUint(idFornec.ToString()) : 0;
        }


        #endregion

        #region Cancelar Sinal

        private static object _cancelarSinalLock = new object();

        /// <summary>
        /// Cancela o sinal da compra
        /// </summary>
        public void CancelarSinal(uint idSinalCompra, string Motivo, DateTime? dataEstorno)
        {
            lock(_cancelarSinalLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (!SinalCompraDAO.Instance.Exists(transaction, idSinalCompra))
                            throw new Exception("O sinal informado não foi encontrado.");

                        SinalCompra sc = SinalCompraDAO.Instance.GetSinalCompraDetails(transaction, idSinalCompra);

                        var compras = CompraDAO.Instance.GetByString(transaction, sc.IdsCompras).ToArray();

                        for (int i = 0; i < compras.Length; i++)
                        {
                            if (compras[i].Situacao == Compra.SituacaoEnum.Finalizada)
                                throw new Exception("A compra: " + compras[i].IdCompra + " está finalizada.");
                        }

                        if (sc.Cancelado)
                            throw new Exception("O sinal informado já está cancelado");

                        ContasPagar[] lstContasPg = ContasPagarDAO.Instance.GetBySinalCompra(transaction, idSinalCompra);

                        #region Estorna Crédito

                        if (FinanceiroConfig.FormaPagamento.CreditoFornecedor)
                        {
                            List<CaixaGeral> lstCxGeral = new List<CaixaGeral>(CaixaGeralDAO.Instance.GetBySinalCompra(transaction, idSinalCompra));
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
                                if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor) &&
                                    lstContasPg[0].IdFornec != null)
                                {
                                    FornecedorDAO.Instance.CreditaCredito(transaction, sc.IdFornec.Value, cx.ValorMov);

                                    // Estorna crédito utilizado pelo fornecedor
                                    CaixaGeralDAO.Instance.MovCxSinalCompra(transaction, idSinalCompra, null, sc.IdFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoCreditoFornecedor), 2,
                                        cx.ValorMov, null, 0, false, null);
                                }

                                if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado) &&
                                    lstContasPg[0].IdFornec != null)
                                {
                                    FornecedorDAO.Instance.DebitaCredito(transaction, lstContasPg[0].IdFornec.Value, cx.ValorMov);

                                    // Estorna crédito venda gerado
                                    CaixaGeralDAO.Instance.MovCxSinalCompra(transaction, idSinalCompra, null, sc.IdFornec,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCreditoCompraGerado), 1,
                                        cx.ValorMov, null, 0, false, null);
                                }
                            }
                        }

                        #endregion

                        PagtoSinalCompra[] pagtoSinalCompra = PagtoSinalCompraDAO.Instance.GetBySinalCompra(transaction, idSinalCompra);

                        #region Estorna pagamentos 

                        // Estorna os pagamentos
                        foreach (PagtoSinalCompra psc in pagtoSinalCompra)
                        {
                            // Não estorna os cheques ou crédito no loop (crédito: idFormaPagto = 0);
                            if (psc.IdFormaPagto == 0 || psc.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || psc.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                                continue;

                            // Recupera o plano de contas
                            uint idConta = UtilsPlanoConta.GetPlanoContaEstornoPagto(psc.IdFormaPagto);

                            // Gera uma movimentação de estorno para cada forma de pagto
                            CaixaGeralDAO.Instance.MovCxSinalCompra(transaction, idSinalCompra, null, sc.IdFornec.Value, idConta, 1, psc.ValorPagto,
                                null, 0, psc.IdContaBanco == null || psc.IdContaBanco == 0, null);

                            if (psc.IdContaBanco > 0)
                            {
                                // Gera movimentação de estorno na conta bancária    
                                EstornoBancario(transaction, idSinalCompra, psc.IdContaBanco.Value, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTransfBanco),
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoTransfBancaria), dataEstorno);

                                EstornoBancario(transaction, idSinalCompra, psc.IdContaBanco.Value, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoBoleto),
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoBoleto), dataEstorno);
                            }
                        }

                        #region Estorna Cheques

                        var lstChequesTerc = ChequesDAO.Instance.GetBySinalCompra(transaction, idSinalCompra).ToArray();

                        // Gera movimentação no caixa geral de estorno de cada cheque de terceiro
                        foreach (Cheques c in lstChequesTerc)
                        {
                            if (c.Tipo == 1)
                                continue;

                            // Estorna valor gerado pelo cheque
                            CaixaGeralDAO.Instance.MovCxSinalCompra(transaction, idSinalCompra, null, sc.IdFornec,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoChequeTerceiros), 1,
                                c.Valor, null, 2, true, null);
                        }

                        // Altera situação dos cheques de terceiros utilizados em aberto
                        ChequesDAO.Instance.CancelaChequesSinalCompra(transaction, idSinalCompra, 2, Cheques.SituacaoCheque.EmAberto);

                        // Cancela cheques próprios utilizados
                        ChequesDAO.Instance.CancelaChequesSinalCompra(transaction, idSinalCompra, 1, Cheques.SituacaoCheque.Cancelado);

                        #endregion

                        #endregion

                        #region Gera Log Cancelamento

                        LogCancelamentoDAO.Instance.LogSinalCompra(transaction, sc, Motivo, false);

                        #endregion

                        //Marca as compras como ainda não recebeu o sinal
                        objPersistence.ExecuteCommand(transaction, "UPDATE compra SET idSinalCompra= null WHERE idCompra IN(" + sc.IdsCompras + ")");

                        //Apaga o pagamento do sinal
                        PagtoSinalCompraDAO.Instance.DeleteBySinal(transaction, sc.IdSinalCompra);

                        //Apaga a conta paga
                        ContasPagarDAO.Instance.DeleteBySinalCompra(transaction, idSinalCompra);

                        //Cancela o sinal
                        objPersistence.ExecuteCommand(transaction, "UPDATE sinal_compra set cancelado = true, obs=?obs where idSinalCompra=" + idSinalCompra,
                            new GDAParameter("?obs", Motivo));

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

        #region Estorno bancário

        /// <summary>
        /// Efetua estorno bancário do sinal da compra
        /// </summary>
        private void EstornoBancario(GDASession session, uint idSinalCompra, uint idContaBanco, uint idConta, uint idContaEstorno, DateTime? dataEstornoBanco)
        {
            if (dataEstornoBanco == null)
            {
                // Pega a primeira movimentação da conta bancária referente ao pagto
                object obj = objPersistence.ExecuteScalar(session, "Select idMovBanco from mov_banco Where idContaBanco=" + idContaBanco +
                    " And idSinalCompra=" + idSinalCompra + " order by idMovBanco asc limit 1");

                uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

                if (idMovBanco == 0)
                    return;

                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idMovBanco);

                MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(session, idMovBanco);

                // Corrige saldo
                objPersistence.ExecuteCommand(session, "Update mov_banco Set valorMov=0 Where idConta=" + idConta + " And idSinalCompra=" + idSinalCompra);
                if (movAnterior != null) MovBancoDAO.Instance.CorrigeSaldo(session, movAnterior.IdMovBanco, idMovBanco);

                // Exclui movimentações geradas
                objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idConta=" + idConta + " And idSinalCompra=" + idSinalCompra);
            }
            else
            {
                SinalCompra sc = SinalCompraDAO.Instance.GetSinalCompraDetails(session, idSinalCompra);
                foreach (MovBanco m in MovBancoDAO.Instance.GetBySinalCompra(session, idSinalCompra, idConta))
                {
                    // Condição necessária para não estornar a conta mais de uma vez, no caso de efetuar um pagamento com duas ou mais 
                    // formas de pagamento que envolvem conta bancária (pagto. bancário)
                    if (idContaBanco == m.IdContaBanco)
                        ContaBancoDAO.Instance.MovContaSinalCompra(session, idContaBanco, idContaEstorno, (int)UserInfo.GetUserInfo.IdLoja,
                            idSinalCompra, null, sc.IdFornec, m.TipoMov == 1 ? 2 : 1, m.ValorMov, dataEstornoBanco.Value, m.Obs);
                }
            }
        }

        #endregion

        #region ratificar Sinal

        /// <summary>
        /// Ratifica o sinal da compra
        /// </summary>
        /// <param name="idSinalCompra"></param>
        /// <param name="idsCompras"></param>
        public void RetificaSinal(uint idSinalCompra, string idsComprasRemover)
        {
            if (idsComprasRemover == null || String.IsNullOrEmpty((idsComprasRemover = idsComprasRemover.Trim(',', ' '))))
                return;

            // Variáveis de controle
            bool atualizouCompra = false;

            // Recupera as compras do sinal (para restauração em caso de erro)
            List<Compra> compras = new List<Compra>(CompraDAO.Instance.GetBySinalCompra(idSinalCompra));

            try
            {
                // Recupera o valor do crédito que será feito ao fornecedor
                decimal valorCredito = ExecuteScalar<decimal>("select sum(valorEntrada) from compra where idCompra in (" + idsComprasRemover + @")
                    and idSinalCompra=" + idSinalCompra);

                // Remove a referência do sinal das compras removidas
                objPersistence.ExecuteCommand("update compra set idSinalCompra=null where idCompra in (" + idsComprasRemover + @")
                    and idSinalCompra=" + idSinalCompra);

                atualizouCompra = true;

                // Recupera o cliente e variáveis de controle adicionais
                uint idFornec = ObtemIdFornec(idSinalCompra), idMovCxGeral = 0;
                bool creditou = false;

                try
                {
                    // Credita o valor dos pedidos já pagos ao crédito do cliente
                    FornecedorDAO.Instance.CreditaCredito(null, idFornec, valorCredito);
                    creditou = true;

                    // Faz uma movimentação no caixa geral para que o crédito entre na movimentação de crédito do cliente
                    idMovCxGeral = CaixaGeralDAO.Instance.MovCxSinalCompra(null, idSinalCompra, null, idFornec,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado),
                        2, valorCredito, "Retificação do Sinal", 0, false, null);
                }
                catch
                {
                    // Restaura os dados em caso de erro
                    if (creditou)
                        FornecedorDAO.Instance.CreditaCredito(null, idFornec, valorCredito);

                    if (idMovCxGeral > 0)
                        CaixaGeralDAO.Instance.DeleteByPrimaryKey(idMovCxGeral);

                    throw;
                }

                // Recupera a observação original do sinal (para restauração)
                string obs = ObtemValorCampo<string>("obs", "idSinalCompra=" + idSinalCompra);
                string dados = "Compras Removidas: " + idsComprasRemover + " / Valor de Crédito Gerado: " + valorCredito.ToString("C");

                // Recupera o valor pago do sinal (para restauração)
                decimal valorPago = PagtoSinalCompraDAO.Instance.ObtemValorCampo<decimal>("valorPagto", "idSinalCompra=" + idSinalCompra);

                try
                {
                    // Salva os dados da retificação na observação do sinal

                    string sql = "update sinal_compra set obs=trim(concat(coalesce(obs, ''), ?obs)) where idSinalCompra=" + idSinalCompra;
                    string obsStr = " - " + dados + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    objPersistence.ExecuteCommand(sql, new GDAParameter("?obs", obsStr));

                    // Atualiza o valor pago do sinal
                    objPersistence.ExecuteCommand("Update pagto_sinal_compra set valorPagto=valorPagto-" + valorCredito.ToString().Replace(',', '.') + " Where idSinalCompra=" + idSinalCompra);

                    // Cria um log com a alteração
                    SinalCompra s = GetElementByPrimaryKey(idSinalCompra);
                    s.DadosRetificar = dados;
                    LogAlteracaoDAO.Instance.LogSinalCompra(s);
                }
                catch
                {
                    // Restaura a observação
                    objPersistence.ExecuteCommand("update sinal_compra set obs=?obs where idSinalCompra=" + idSinalCompra,
                        new GDAParameter("?obs", obs));

                    // Restaura o valor pago
                    objPersistence.ExecuteCommand("Update pagto_sinal_compra set valorPagto=" + valorPago + " Where idSinalCompra=" + idSinalCompra);

                    throw;
                }
            }
            catch
            {
                // Restaura os dados dos pedidos em caso de erro
                if (atualizouCompra)
                    foreach (string id in idsComprasRemover.Split(','))
                    {
                        objPersistence.ExecuteCommand("update compra set idSinalCompra=" + idSinalCompra + " where idCompra=" + id);
                    }

                throw;
            }
        }

        #endregion
    }
}