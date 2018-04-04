using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL
{
    public sealed class ContasPagarDAO : BaseCadastroDAO<ContasPagar, ContasPagarDAO>
    {
        //private ContasPagarDAO() { }

        #region Pagar Contas

        #region Métodos de suporte

        private string GetIdsContas(ContasPagar[] contas)
        {
            return String.Join(",", Array.ConvertAll<ContasPagar, string>(contas, new Converter<ContasPagar, string>(
                delegate(ContasPagar cp)
                {
                    return cp.IdContaPg.ToString();
                }
            )));
        }
        
        private uint PagarContas(GDASession sessao, uint idPagto, uint idFornec, ContasPagar[] contas, string chequesAssoc, string vetJurosMulta, DateTime dataPagto, DateTime[] datasFormasPagto,
            decimal[] valores, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao, uint[] numParcCartao, string[] boletos, uint[] antecipFornec, string chequesPagto,
            decimal desconto, string obs, bool gerarCredito, decimal creditoUtilizado, bool pagtoParcial, string numAutConstrucard, decimal totalMulta,
            decimal totalJuros, decimal totalPago, decimal totalASerPago)
        {
            #region Variáveis

            // Data utilizada na geração das movimentações bancárias.
            DateTime dataUsar;
            // Loja utilizada na geração das movimentações do caixa e conta bancária.
            var idLoja = (int)UserInfo.GetUserInfo.IdLoja;
            // Recupera o número de formas de pagamento válidas
            var numFormasPagto = 0;
            // Usado para evitar bloqueio de índice no caixa geral/na conta bancária.
            var contadorDataUnica = 0;
            // Cheques próprios.
            var chequesInseridos = new List<Cheques>();
            // Salva os ids dos cheques de terceiros.
            var idsChequeTerc = string.Empty;
            // Total de juros aplicado em cada forma de pagamento.
            decimal jurosAplicado = 0;
            // Juros rateado por forma de pagamento.
            decimal jurosRateado = 0;
            // Total de multa aplicada em cada forma de pagamento.
            decimal multaAplicada = 0;
            // Multa rateada por forma de pagamento.
            decimal multaRateada = 0;
            // Soma do juros e da multa rateados.
            decimal jurosMultaRateados = 0;

            #endregion
            
            // Recupera a quantidade de formas de pagamento que possuem valor.
            for (int i = 0; i < valores.Length; i++)
                if (valores[i] > 0 && formasPagto[i] > 0)
                    numFormasPagto++;

            try
            {
                for (var i = 0; i < formasPagto.Length; i++)
                {
                    // Somente formas de pagamento com valor podem gerar movimentação no caixa ou no banco.
                    if (valores[i] == 0)
                        continue;

                    dataUsar = dataPagto;

                    // Descrescenta a quantidade de formas de pagamento. Na última forma de pagamento, os valores de juros e multa, rateados,
                    // devem ser preenchidos com o valor restante de juros e multa que não foi aplicado.
                    numFormasPagto--;
                    
                    #region Recupera o valor de juros e de multa, para cada forma de pagamento válida.

                    // Caso seja a última forma de pagamento, os valores de juros e multa, rateados, devem considerar o valor de juros e multa que não foram aplicados.
                    if (numFormasPagto == 0)
                    {
                        // Recupera o valor de juros/multa que, ainda, não foi aplicado.
                        jurosRateado = totalJuros - jurosAplicado;
                        multaRateada = totalMulta - multaAplicada;
                    }
                    else
                    {
                        /* Chamado 66985. */
                        // Recupera o valor de juros/multa que deve ser aplicado para a forma de pagamento, com base em seu valor.
                        jurosRateado = Math.Round((totalJuros / valores.Sum(f => f)) * valores[i], 2, MidpointRounding.AwayFromZero);
                        multaRateada = Math.Round((totalMulta / valores.Sum(f => f)) * valores[i], 2, MidpointRounding.AwayFromZero);

                        // Salva o valor de juros e multa aplicados, para que o rateio da última forma de pagamento fique correto.
                        jurosAplicado += jurosRateado;
                        multaAplicada += multaAplicada;
                    }
                    
                    // Salva, em uma variável, a soma do valor de juros e do valor da multa, rateados. Isso foi feito para reduzir a quantidades de soma que estavam sendo feitas nesse bloco.
                    jurosMultaRateados = jurosRateado + multaRateada;

                    #endregion
                    
                    #region Dinheiro

                    // Se a forma de pagto for Dinheiro, gera movimentação no caixa geral
                    if (formasPagto[i] == (uint)Pagto.FormaPagto.Dinheiro)
                    {
                        CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoContaPagto((uint)Pagto.FormaPagto.Dinheiro), 2, valores[i] - jurosMultaRateados,
                            jurosMultaRateados, null, obs, 1, true, null);
                    }

                    #endregion

                    #region Cheques

                    // Se a forma de pagamento for cheques próprios, cadastra cheques, associa os mesmos com as contas
                    // que estão sendo pagas, debita valor da conta que foi escolhida em cada cheque
                    else if ((formasPagto[i] == (uint)Pagto.FormaPagto.ChequeProprio || formasPagto[i] == (uint)Pagto.FormaPagto.ChequeTerceiro) && !string.IsNullOrEmpty(chequesPagto))
                    {
                        Cheques cheque;
                        var pagtoCheque = new PagtoCheque();
                        // Separa os cheques guardando-os em um vetor.
                        var vetCheque = chequesPagto.TrimEnd(' ').TrimEnd('|').Split('|');
                        var jurosRateadoCheque = jurosRateado / vetCheque.Length;
                        var multaRateadaCheque = multaRateada / vetCheque.Length;
                        var jurosMultaRateadosCheque = jurosRateadoCheque + multaRateadaCheque;

                        // Cria um idPagto, que será utilizado para identificar este pagto.
                        pagtoCheque.IdPagto = idPagto;

                        if (formasPagto[i] == (uint)Pagto.FormaPagto.ChequeProprio)
                        {
                            #region Associa cheques ao pagamento e Gera movimentações no caixa geral

                            try
                            {
                                // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                                foreach (var c in vetCheque)
                                {
                                    // Divide o cheque para pegar suas propriedades
                                    var dadosCheque = c.Split('\t');

                                    if (dadosCheque[0] == "proprio") // Se for cheque próprio
                                    {
                                        // Insere cheque no BD
                                        cheque = ChequesDAO.Instance.GetFromString(c);
                                        cheque.JurosPagto = jurosRateadoCheque;
                                        cheque.MultaPagto = multaRateadaCheque;
                                        cheque.Obs += string.Format("Utilizado no pagamento {0}", idPagto);
                                        cheque.IdCheque = ChequesDAO.Instance.InsertBase(sessao, cheque, false);

                                        if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                            cheque.DataReceb = dataPagto;

                                        if (cheque.IdCheque < 1)
                                            throw new Exception("retorno do insert do cheque=0");

                                        // Adiciona este cheque à lista de cheques inseridos
                                        chequesInseridos.Add(cheque);

                                        // Gera movimentação no caixa geral de cada cheque, mas sem alterar o saldo, 
                                        // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral
                                        var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoChequeProprio),
                                            2, cheque.Valor - jurosMultaRateadosCheque, jurosMultaRateadosCheque, null, obs, 0, false, null);

                                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                            idCaixaGeral));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao associar cheques ao pagamento.", ex));
                            }

                            #endregion

                            #region Associa contas a pagar ao cheque

                            var vetChequeAssoc = chequesAssoc.Split('|');

                            foreach (var c in vetChequeAssoc)
                            {
                                if (string.IsNullOrEmpty(c))
                                    continue;

                                var vetC = c.Split(';');

                                if (string.IsNullOrEmpty(vetC[1]))
                                    continue;

                                var idContaPg = vetC[0].StrParaUint(); // Pega o idContaPg
                                var dadosCheque = vetC[1].Split('/'); // Pega os dados do cheque 0-Num Cheque, 1-Agencia, 2-Conta

                                foreach (var cIns in chequesInseridos)
                                {
                                    if (cIns.Num == dadosCheque[0].StrParaInt() && cIns.Agencia == dadosCheque[1] && cIns.Conta == dadosCheque[2])
                                    {
                                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE contas_pagar SET IdChequePagto={0} WHERE IdContaPg={1}", cIns.IdCheque, idContaPg));
                                        break;
                                    }
                                }
                            }

                            #endregion

                            #region Associa cheques inseridos ao pagamento e Gera Movimentação Bancária

                            try
                            {
                                // Associa cada cheque utilizado no pagamento ao pagto
                                foreach (var c in chequesInseridos)
                                {
                                    pagtoCheque.IdCheque = c.IdCheque;
                                    pagtoCheque.IdContaBanco = c.IdContaBanco;
                                    PagtoChequeDAO.Instance.Insert(sessao, pagtoCheque);
                                }

                                // Para cada cheque "Compensado" utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                                foreach (var c in chequesInseridos)
                                    if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                    {
                                        ContaBancoDAO.Instance.MovContaChequePagto(sessao, c.IdContaBanco.Value, UtilsPlanoConta.GetPlanoContaPagto(2), idLoja, c.IdCheque, idPagto, idFornec, 2,
                                            c.Valor - jurosMultaRateadosCheque, jurosMultaRateadosCheque, dataUsar);

                                        // Gera movimentação de juros.
                                        if (c.JurosPagto > 0)
                                            ContaBancoDAO.Instance.MovContaChequePagto(sessao, c.IdContaBanco.Value, FinanceiroConfig.PlanoContaJurosPagto, idLoja, c.IdCheque, idPagto, idFornec, 2,
                                                jurosRateadoCheque, 0, dataUsar);

                                        // Gera movimentação de multa
                                        if (c.MultaPagto > 0)
                                            ContaBancoDAO.Instance.MovContaChequePagto(sessao, c.IdContaBanco.Value, FinanceiroConfig.PlanoContaMultaPagto, idLoja, c.IdCheque, idPagto, idFornec, 2,
                                                multaRateadaCheque, 0, dataUsar);
                                    }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao associar cheques às contas a pagar", ex));
                            }

                            #endregion
                        }
                        // Se a forma de pagamento for cheques de terceiros
                        else if (formasPagto[i] == (uint)Pagto.FormaPagto.ChequeTerceiro)
                        {
                            #region Associa cheques ao pagamento e Gera movimentações no caixa geral

                            try
                            {
                                contadorDataUnica = 0;

                                // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                                foreach (var c in vetCheque)
                                {
                                    // Divide o cheque para pegar suas propriedades
                                    var dadosCheque = c.Split('\t');

                                    if (dadosCheque[0] == "terceiro") // Se for cheque de terceiro
                                    {
                                        // Associa cada cheque utilizado no pagto à cada conta paga
                                        pagtoCheque.IdCheque = dadosCheque[18].StrParaUint();
                                        var valorCheque = ChequesDAO.Instance.ObtemValorCampo<decimal>(sessao, "Valor", string.Format("IdCheque={0}", pagtoCheque.IdCheque));

                                        /* Chamado 55066. */
                                        if (pagtoCheque.IdCheque == 0)
                                            throw new Exception("Não foi possível recuperar um dos cheques utilizados no pagamento.");

                                        PagtoChequeDAO.Instance.Insert(sessao, pagtoCheque);
                                        idsChequeTerc += string.Format("{0},", dadosCheque[18]);

                                        // Coloca juros e multa gerados no cheque
                                        objPersistence.ExecuteCommand(sessao, string.Format(@"UPDATE cheques c SET c.JurosPagto=?juros, c.MultaPagto=?multa, c.Obs=CONCAT(c.Obs, ' ', ?obs)
                                            WHERE c.IdCheque={0}", pagtoCheque.IdCheque),
                                            new GDAParameter("?juros", jurosRateadoCheque),
                                            new GDAParameter("?multa", multaRateadaCheque),
                                            new GDAParameter("?obs", string.Format("Utilizado no pagamento {0}", idPagto)));
                                        
                                        // Gera movimentação no caixa geral de cada cheque, mas sem alterar o saldo
                                        var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoChequeTerceiros),
                                            2, valorCheque - jurosMultaRateadosCheque, jurosMultaRateadosCheque, null, null, 2, true, null);
                                            
                                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                            idCaixaGeral));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao associar cheques ao pagamento.", ex));
                            }

                            #endregion

                            #region Altera situação dos cheques para compensado e gera movimentação se houver adicional

                            try
                            {
                                // Marca cheques de terceiros utilizados no pagamento como compensados
                                ChequesDAO.Instance.UpdateSituacao(sessao, idsChequeTerc.TrimEnd(','), Cheques.SituacaoCheque.Compensado);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao marcar cheques como compensados.", ex)); ;
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Depósito (Pagto. Bancário) ou Boleto

                    else if (formasPagto[i] == (uint)Pagto.FormaPagto.Deposito || formasPagto[i] == (uint)Pagto.FormaPagto.Boleto)
                    {
                        var idContaDepositoBoleto = formasPagto[i] == (uint)Pagto.FormaPagto.Deposito ?
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoTransfBanco) :
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoBoleto);
                        // Salva o pagto. bancário no Cx. Geral.
                        var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, idContaDepositoBoleto, 2, valores[i] - jurosMultaRateados, jurosMultaRateados, null, obs,
                            0, false, dataUsar);
                        // Gera movimentação de saída na conta bancária.
                        var idMovBanco = ContaBancoDAO.Instance.MovContaPagto(sessao, idContasBanco[i], idContaDepositoBoleto, idLoja, idPagto, null, idFornec, 2, valores[i] - jurosMultaRateados,
                            jurosMultaRateados, dataUsar, obs);

                        // Necessário para evitar bloqueio do índice no caixa geral.
                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++, idCaixaGeral));
                        // Necessário para evitar bloqueio do índice na conta bancária.
                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdMovBanco={1}", contadorDataUnica, idMovBanco));
                    }

                    #endregion

                    #region Permuta

                    else if (formasPagto[i] == (uint)Pagto.FormaPagto.Permuta)
                    {
                        CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoContaPagto((uint)Pagto.FormaPagto.Permuta), 2, valores[i] - jurosMultaRateados,
                            jurosMultaRateados, null, obs, 0, false, null);
                    }

                    #endregion

                    #region Antecipação de Fornecedor

                    if (formasPagto[i] == (uint)Pagto.FormaPagto.AntecipFornec)
                    {
                        CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoContaPagto((uint)Pagto.FormaPagto.AntecipFornec), 2, valores[i] - jurosMultaRateados,
                            jurosMultaRateados, null, obs, 0, false, null);
                    }

                    #endregion
                }

                #region Gera movimentações de juros e multa

                // Recupera o número de formas de pagamento válidas para juros (todas menos permuta)
                numFormasPagto = 0;

                for (var i = 0; i < valores.Length; i++)
                    if (valores[i] > 0 && (formasPagto[i] != (uint)Pagto.FormaPagto.Permuta))
                        numFormasPagto++;

                if (numFormasPagto > 0)
                {
                    for (var i = 0; i < formasPagto.Length; i++)
                    {
                        if (valores[i] <= 0 || formasPagto[i] == (uint)Pagto.FormaPagto.Permuta)
                            continue;

                        dataUsar = dataPagto;

                        var multaC = GetJurosMulta(vetJurosMulta, 1);
                        var jurosC = GetJurosMulta(vetJurosMulta, 2);

                        for (var j = 0; j < contas.Length; j++)
                        {
                            // Cheque próprio não deve gerar movimentação de juros/multa, pois caso esteja em aberto,
                            // a movimentação de juros/multa será gerada ao quitar o cheque e caso esteja compensado
                            // a movimentação já foi gerada acima
                            if (formasPagto[i] == (uint)Pagto.FormaPagto.ChequeProprio)
                                continue;

                            // Gera movimentação na conta bancária
                            if (idContasBanco[i] > 0 && formasPagto[i] != (uint)Pagto.FormaPagto.Dinheiro)
                            {
                                // Gera movimentação de juros
                                if (jurosC[j] > 0)
                                    ContaBancoDAO.Instance.MovContaPagto(sessao, idContasBanco[i], FinanceiroConfig.PlanoContaJurosPagto, idLoja, idPagto, contas[j].IdContaPg, idFornec, 2,
                                        jurosC[j] / numFormasPagto, 0, dataUsar, null);

                                // Gera movimentação de multa
                                if (multaC[j] > 0)
                                    ContaBancoDAO.Instance.MovContaPagto(sessao, idContasBanco[i], FinanceiroConfig.PlanoContaMultaPagto, idLoja, idPagto, contas[j].IdContaPg, idFornec, 2,
                                        multaC[j] / numFormasPagto, 0, dataUsar, null);
                            }
                            // Gera movimentação no caixa geral
                            else
                            {
                                var formaSaida = formasPagto[i] == (int)Pagto.FormaPagto.ChequeTerceiro ? 2 : formasPagto[i] == (int)Pagto.FormaPagto.Dinheiro ? 1 : 0;
                                var mudarSaldo = formaSaida == 0 ? false : true;

                                // Gera movimentação de juros
                                if (jurosC[j] > 0)
                                {
                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, contas[j].IdContaPg, idFornec, FinanceiroConfig.PlanoContaJurosPagto, 2,
                                        jurosC[j] / numFormasPagto, 0, null, null, formaSaida, mudarSaldo, dataUsar);

                                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                        idCaixaGeral));
                                }

                                // Gera movimentação de multa
                                if (multaC[j] > 0)
                                {
                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, contas[j].IdContaPg, idFornec, FinanceiroConfig.PlanoContaMultaPagto, 2,
                                        multaC[j] / numFormasPagto, 0, null, null, formaSaida, mudarSaldo, dataUsar);

                                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                        idCaixaGeral));
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Crédito

                // Se a empresa trabalhar com crédito de fornecedor
                if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && idFornec > 0)
                {
                    // Se algum crédito do fornecedor tive sido utilizado
                    if (creditoUtilizado > 0)
                    {
                        CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor), 1,
                            creditoUtilizado, 0, null, null, 0, false, null);
                        
                        // Debita Crédito com o fornecedor
                        FornecedorDAO.Instance.DebitaCredito(sessao, idFornec, creditoUtilizado);

                        #region Gera movimentações de juros e multa caso o pagamento tenha sido efetuado somente com crédito

                        /* Chamado 34577.
                         * O pagamento efetuado totalmente com crédito deve gerar as movimentações de juros e multa. */
                        if (numFormasPagto == 0 && !formasPagto.Any(f => f > 0))
                        {
                            dataUsar = dataPagto.Ticks > 0 ? dataPagto : DateTime.Now;

                            var multaC = GetJurosMulta(vetJurosMulta, 1);
                            var jurosC = GetJurosMulta(vetJurosMulta, 2);

                            for (var i = 0; i < contas.Length; i++)
                            {
                                // Gera movimentação de juros.
                                if (jurosC[i] > 0)
                                {
                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, contas[i].IdContaPg, idFornec, FinanceiroConfig.PlanoContaJurosPagto, 2, jurosC[i], 0, null,
                                        null, 0, false, dataUsar);

                                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                        idCaixaGeral));
                                }

                                // Gera movimentação de multa.
                                if (multaC[i] > 0)
                                {
                                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, contas[i].IdContaPg, idFornec, FinanceiroConfig.PlanoContaMultaPagto, 2, multaC[i], 0, null,
                                        null, 0, false, dataUsar);

                                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}", contadorDataUnica++,
                                        idCaixaGeral));
                                }
                            }
                        }

                        #endregion
                    }

                    // Se tiver sido gerado algum crédito para com o fornecedor
                    // Chamado 13169. O total a ser pago já considera o valor do desconto, não é necessário subtraí-lo novamente
                    //if (gerarCredito && totalPago > (totalASerPago - desconto))
                    if (gerarCredito && totalPago > totalASerPago)
                    {
                        CaixaGeralDAO.Instance.MovCxPagto(sessao, idPagto, null, idFornec, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado), 2,
                            totalPago - totalASerPago, 0, null, null, 0, false, null);

                        // Credita crédito do fornecedor
                        FornecedorDAO.Instance.CreditaCredito(sessao, idFornec, totalPago - totalASerPago);
                    }
                }

                #endregion

                #region Pagamento parcial

                /* Chamado 22203.
                 * O total a ser pago já considera o desconto. */
                //if (pagtoParcial && ((totalASerPago - desconto) - totalPago) > 0)
                if (pagtoParcial && (totalASerPago - totalPago) > 0)
                {
                    // IdConta que será salvo na conta a pagar restante.
                    var idContaPagtoRestante = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                    var idsContaPg = GetIdsContas(contas);
                    // SQL base utilizado para verificar quais propriedades das contas a pagar, do pagamento, são as mesmas. Para que ela seja preenchida na conta do pagamento restante.
                    var sqlBasePropriedadesEmComum = string.Format(@"
                        SELECT 
                            IF(
                                (SELECT SUM(cont) 
                                FROM (
                                    SELECT COUNT(*) as cont
                                    FROM contas_pagar 
                                    WHERE IdContaPg IN ({0}) 
                                    GROUP BY {1}
                                    ) as tbl
                                ) = 1, {1}, NULL
                            )
                        FROM contas_pagar WHERE IdContaPg IN ({0}) GROUP BY {1};", idsContaPg, "{0}");

                    // Recupera o ID da compra, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idCompra = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdCompra"));
                    // Recupera o ID do custo fixo, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idCustoFixo = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdCustoFixo"));
                    // Recupera o ID do imposto/serviço avulso, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idImpostoServico = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdImpostoServ"));
                    // Recupera o ID da nota fiscal, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idNf = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdNf"));
                    // Recupera o ID da comissão, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idComissao = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdComissao"));
                    // Recupera a propriedade CONTABIL, caso ela esteja preenchida, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var contabil = ExecuteScalar<bool?>(sessao, string.Format(sqlBasePropriedadesEmComum, "Contabil"));
                    // Recupera o ID da loja, caso ele esteja preenchido, com o mesmo valor, em todas as contas a pagar do pagamento.
                    var idLojaPagtoParcial = ExecuteScalar<uint?>(sessao, string.Format(sqlBasePropriedadesEmComum, "IdLoja"));

                    // Insere outra parcela contendo o valor restante a ser pago.
                    var contaPagar = new ContasPagar();
                    /* Chamado 22203.
                     * O total a ser pago já considera o desconto. */
                    //contaPagar.ValorVenc = (totalASerPago - desconto) - totalPago;
                    contaPagar.ValorVenc = totalASerPago - totalPago;
                    contaPagar.DataVenc = DateTime.Now;
                    contaPagar.Paga = false;
                    contaPagar.IdFornec = idFornec;
                    contaPagar.IdConta = idContaPagtoRestante;
                    contaPagar.IdPagtoRestante = idPagto;
                    contaPagar.IdFormaPagto = formasPagto[0];
                    contaPagar.IdLoja = idLojaPagtoParcial > 0 ? idLojaPagtoParcial.Value : (uint)idLoja;
                    contaPagar.Obs = obs;
                    contaPagar.NumParc = 1;
                    contaPagar.NumParcMax = 1;
                    contaPagar.IdCompra = idCompra;
                    contaPagar.IdCustoFixo = idCustoFixo;
                    contaPagar.IdImpostoServ = idImpostoServico;
                    contaPagar.IdNf = idNf;
                    contaPagar.IdComissao = idComissao;
                    contaPagar.Contabil = contabil.GetValueOrDefault();

                    // Se houver apenas uma conta sendo paga parcialmente, recupera o idComissao da mesma, se houver
                    if (contas.Length == 1)
                        contaPagar.IdComissao = contas[0].IdComissao;

                    contaPagar.IdContaPg = Insert(sessao, contaPagar);
                }

                #endregion

                #region Atualiza o desconto das contas a pagar

                if (desconto > 0)
                {
                    decimal descontoAplicado = 0;

                    for (var i = 0; i < contas.Length; i++)
                    {
                        var multaC = GetJurosMulta(vetJurosMulta, 1);
                        var jurosC = GetJurosMulta(vetJurosMulta, 2);

                        if ((i + 1) < contas.Length)
                        {
                            contas[i].Desconto = Math.Round((desconto / (totalASerPago + desconto)) * (contas[i].ValorVenc + jurosC[i] + multaC[i]), 2);
                            descontoAplicado += contas[i].Desconto;
                        }
                        else
                            contas[i].Desconto = desconto - descontoAplicado;

                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE contas_pagar SET Desconto=?desconto WHERE IdContaPg={0}", contas[i].IdContaPg),
                            new GDAParameter("?desconto", contas[i].Desconto));
                    }
                }

                #endregion

                // Marca Contas a Pagar como Pagas
                MarcarComoPaga(sessao, contas, idPagto, vetJurosMulta, dataPagto, false, obs, totalASerPago, totalPago, antecipFornec, formasPagto[0]);

                PreencheLocalizacao(sessao, ref contas);
            }
            catch (Exception ex)
            {
                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao alterar situação das contas para paga.", ex));
            }

            return idPagto;
        }

        /// <summary>
        /// Recupera o valor do crédito gerado.
        /// </summary>
        /// <param name="contas"></param>
        /// <param name="desconto"></param>
        /// <param name="valoresPagos"></param>
        /// <returns></returns>
        private decimal GetCreditoGerado(ContasPagar[] contas, decimal desconto, decimal[] valores, decimal totalJuros, decimal totalMulta, decimal creditoUtilizado)
        {
            decimal totalPago = 0;
            foreach (decimal v in valores)
                totalPago += v;

            if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && creditoUtilizado > 0)
                totalPago += creditoUtilizado;

            decimal totalASerPago = totalJuros + totalMulta;
            foreach (ContasPagar c in contas)
                totalASerPago += c.ValorVenc;

            return totalPago - (totalASerPago - desconto);
        }

        /// <summary>
        /// Retorna o vetor de juros ou multa.
        /// </summary>
        /// <param name="jurosMulta"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private decimal[] GetJurosMulta(string jurosMulta, int tipo)
        {
            string[] vetMultaJuros = jurosMulta.Trim(' ').Trim('|').Trim(',').Split('|');
            decimal[] retorno = new decimal[vetMultaJuros.Length];

            for (int i = 0; i < vetMultaJuros.Length; i++)
            {
                string[] multaJuros = vetMultaJuros[i].Split(';');

                // Multa
                if (tipo == 1 && !String.IsNullOrEmpty(multaJuros[0]))
                    retorno[i] = Glass.Conversoes.StrParaDecimal(multaJuros[0]);

                // Juros
                else if (tipo == 2 && !String.IsNullOrEmpty(multaJuros[1]))
                    retorno[i] = Glass.Conversoes.StrParaDecimal(multaJuros[1]);
            }

            return retorno;
        }

        /// <summary>
        /// Soma o total de multa e juros pago em um pagto
        /// </summary>
        /// <param name="multasJuros"></param>
        /// <param name="tipo">1-Multa, 2-Juros</param>
        /// <returns></returns>
        private decimal TotalJurosMulta(string jurosMulta, int tipo)
        {
            decimal totalJurosMulta = 0;
            foreach (decimal v in GetJurosMulta(jurosMulta, tipo))
                totalJurosMulta += v;

            return totalJurosMulta;
        }

        /// <summary>
        /// Marca todas as contas a pagar passadas por parâmetro como paga, atualizando 
        /// o valor pago para o valor venc e a data de pagto para o valor do parâmetro passado
        /// </summary>
        /// <param name="idsContaPagar"></param>
        /// <param name="idPagto"></param>
        /// <param name="multasJuros"></param>
        /// <param name="dataPagto"></param>
        private void MarcarComoPaga(GDASession sessao, ContasPagar[] contas, uint idPagto, string multasJuros, DateTime dataPagto, bool renegociar,
            string obs, decimal valorTotal, decimal valorPago, uint[] antecipFornecedor, uint idFormaPagto)
        {
            var sql = string.Empty;
            var vetMultaJuros = multasJuros.Trim(' ').Trim('|').Trim(',').Split('|');
            // Se for renegociação, o valor do vencimento fica sendo "0".
            var sqlValorPago = string.Empty;

            if (renegociar || valorPago <= 0)
            {
                sqlValorPago = "ValorPago=0,";
            }
            else
            {
                sqlValorPago = "ValorPago=ROUND(ValorVenc{0}, 2)-COALESCE(Desconto, 0),";


                sqlValorPago = string.Format(sqlValorPago, string.Format("*{0}", ((valorPago < valorTotal) ? (valorPago / valorTotal) : 1).ToString().Replace(",", ".")));
               
            }

            for (var i = 0; i < contas.Length; i++)
            {
                var multaJuros = vetMultaJuros[i].Split(';');

                sql += string.Format("UPDATE contas_pagar SET DataPagto=?dataPagto, Paga=1, {0} Juros={1}, Multa={2}, IdPagto={3}", sqlValorPago, multaJuros[1].Replace(',', '.'),
                    multaJuros[0].Replace(',', '.'), idPagto);

                if (idFormaPagto > 0)
                {
                    sql += string.Format(", IdFormaPagto={0}", idFormaPagto);
                }

                if (renegociar)
                {
                    sql += ", Renegociada=1";
                }

                contas[i].IdPagto = idPagto;
                sql += string.Format(" WHERE IdContaPg={0};", contas[i].IdContaPg);
            }

            if (valorTotal > valorPago)
            {
                ContasPagar[] semUltima = new ContasPagar[contas.Length - 1];
                Array.Copy(contas, semUltima, semUltima.Length);
                string ids = contas.Length > 1 ? GetIdsContas(semUltima) : "";

                string soma = ids.Length > 0 ? "(select sum(valorPago) from (select valorPago from contas_pagar where idContaPg in (" + ids + ")) as temp)" : "0";
                sql += "update contas_pagar set valorPago=" + valorPago.ToString().Replace(",", ".") + "-" + soma + " where idContaPg=" + contas[contas.Length - 1].IdContaPg;
            }

            try
            {
                objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?dataPagto", dataPagto));
            }
            catch (Exception ex)
            {
                string observacao = "";
                for (int i = 0; i < contas.Length; i++)
                {
                    observacao = ContasPagarDAO.Instance.ObtemValorCampo<string>(sessao, "obs", "idContaPg=" + contas[i].IdContaPg);
                    if (obs.Length + (contas[i].Obs != null ? contas[i].Obs.Length : 0) > 500)
                        throw new Exception("O campo OBS ultrapassou o limite de caracteres permitido. " +
                            "O limite máximo é de até 500 caracteres, quantidade de caracteres inseridos: " + (obs.Length + (contas[i].Obs != null ? contas[i].Obs.Length : 0)) +
                            ". Referência da conta: " + contas[i].Referencia);
                }

                throw new Exception(ex.Message);
            }

            // Atualiza o saldo das antecipações de fornecedor
            foreach (var af in antecipFornecedor.Where(x => x > 0))
                AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(sessao, af);
        }

        #endregion

        /// <summary>
        /// Faz o pagamento das contas passadas por parâmetro
        /// </summary>
        public uint PagarContasComTransacao(uint idFornec, string contas, string chequesAssoc, string vetJurosMulta, DateTime dataPagto, DateTime[] datasFormasPagto,
            decimal[] valores, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao, uint[] numParcCartao, string[] boletos, uint[] antecipFornec,
            string chequesPagto, decimal desconto, string obs, bool gerarCredito, decimal creditoUtilizado, bool pagtoParcial, string numAutConstrucard)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    
                    var retorno = PagarContas(transaction, idFornec, contas, chequesAssoc, vetJurosMulta, dataPagto, datasFormasPagto, valores, formasPagto, idContasBanco, tiposCartao,
                        numParcCartao, boletos, antecipFornec, chequesPagto, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial, numAutConstrucard);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao pagar contas.", ex));
                }
            }
        }

        /// <summary>
        /// Faz o pagamento das contas passadas por parâmetro
        /// </summary>
        public uint PagarContas(GDASession session, uint idFornec, string contas, string chequesAssoc, string vetJurosMulta, DateTime dataPagto, DateTime[] datasFormasPagto,
            decimal[] valores, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao, uint[] numParcCartao, string[] boletos, uint[] antecipFornec,
            string chequesPagto, decimal desconto, string obs, bool gerarCredito, decimal creditoUtilizado, bool pagtoParcial, string numAutConstrucard)
        {
            uint idPagto = 0;

            // Se o fornecedor não tiver sido passado, verifica se os fornecedores das contas a pagar é o mesmo
            if (idFornec == 0)
                idFornec = GetFornecFromVariasContas(session, contas);

            // Se o id do fornecedor for 0, não deve-se gerar/utilizar crédito 
            if (idFornec == 0 || !FinanceiroConfig.FormaPagamento.CreditoFornecedor)
            {
                // Se a opção de gerar crédito estiver marcada mas não tiver fornecedor, não permite gerar crédito
                if (gerarCredito && FinanceiroConfig.FormaPagamento.CreditoFornecedor)
                    throw new Exception("Não é possível gerar crédito neste pagamento, as contas a pagar não possuem fornecedor associado ou possuem fornecedores diferentes.");

                gerarCredito = false;
                creditoUtilizado = 0;
            }

            decimal totalMulta = TotalJurosMulta(vetJurosMulta, 1);
            decimal totalJuros = TotalJurosMulta(vetJurosMulta, 2);

            ContasPagar[] cp = GetByString(session, contas);

            // CHamado 13908.
            // Um pagamento foi criado em duplicidade, criamos este bloqueio para evitar que isso ocorra novamente.
            foreach (var contaPagar in cp)
                if (contaPagar.Paga)
                    throw new Exception("A conta " + contaPagar.Referencia + " já foi paga.");

            // Valida as informaçoes do pagto e calcula o total pago e o total a ser pago
            decimal totalPago = 0, totalASerPago = 0;
            ValidaPagto(session, idFornec, cp, ref valores, formasPagto, ref totalPago, ref totalASerPago, creditoUtilizado, totalJuros, totalMulta, desconto,
                pagtoParcial, gerarCredito, vetJurosMulta, false, false);

            decimal creditoGerado = !gerarCredito ? 0 :
                GetCreditoGerado(cp, desconto, valores, totalJuros, totalMulta, creditoUtilizado);
                    
            // Cria um novo pagamento
            idPagto = PagtoDAO.Instance.NovoPagto(session, idFornec, dataPagto, valores, formasPagto, idContasBanco, tiposCartao, boletos,
                antecipFornec, datasFormasPagto, totalJuros, totalMulta, desconto, creditoUtilizado, creditoGerado, obs);

            var retorno = PagarContas(session, idPagto, idFornec, cp, chequesAssoc, vetJurosMulta, dataPagto, datasFormasPagto, valores, formasPagto, idContasBanco,
                tiposCartao, numParcCartao, boletos, antecipFornec, chequesPagto, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial,
                numAutConstrucard, totalMulta, totalJuros, totalPago, totalASerPago);
            
            return retorno;
        }

        #endregion

        #region Retificar Pagamento

        /// <summary>
        /// Retifica o pagamento das contas passadas por parâmetro
        /// </summary>
        public uint RetificarPagto(uint idPagto, uint idFornec, string contas, string chequesAssoc, string vetJurosMulta, DateTime dataPagto, DateTime[] datasFormasPagto,
            decimal[] valores, uint[] formasPagto, uint[] idContasBanco, uint[] tiposCartao, uint[] numParcCartao, string[] boletos, uint[] antecipFornec, string chequesPagto,
            decimal desconto, string obs, bool gerarCredito, decimal creditoUtilizado, bool pagtoParcial, string numAutConstrucard, bool gerarCreditoContasRemovidas,
            string idsContasRemovidas)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Se o id do fornecedor for 0, não deve-se gerar/utilizar crédito 
                    if (idFornec == 0)
                    {
                        gerarCredito = false;
                        creditoUtilizado = 0;
                    }

                    // Recupera o pagamento atual
                    var pagto = PagtoDAO.Instance.GetForLog(transaction, idPagto);
                    var cheques = ChequesDAO.Instance.GetByPagto(transaction, idPagto).ToArray();
                    var contasPagar = new List<ContasPagar>(GetByPagto(transaction, idPagto));
                    var dadosPagto = PagtoPagtoDAO.Instance.GetByPagto(transaction, idPagto);

                    var cp = GetByString(transaction, contas);

                    string gerarCreditoFornec = "";
                    Dictionary<uint, decimal> creditoGeradoFornec = new Dictionary<uint, decimal>();

                    if (gerarCreditoContasRemovidas && String.IsNullOrEmpty(idsContasRemovidas))
                        throw new Exception("Não é possível gerar crédito se não houver contas removidas.");

                    decimal totalMulta = TotalJurosMulta(vetJurosMulta, 1);
                    decimal totalJuros = TotalJurosMulta(vetJurosMulta, 2);

                    decimal creditoGerado = !gerarCredito ? 0 :
                        GetCreditoGerado(cp, desconto, valores, totalJuros, totalMulta, creditoUtilizado);

                    // Valida as informaçoes do pagto.
                    decimal totalPago = 0, totalASerPago = 0;
                    ValidaPagto(transaction, idFornec, cp, ref valores, formasPagto, ref totalPago, ref totalASerPago, creditoUtilizado, totalJuros, totalMulta, desconto,
                        pagtoParcial, gerarCredito, vetJurosMulta, true, gerarCreditoContasRemovidas);
                    
                    PagtoDAO.Instance.CancelarPagto(transaction, idPagto, "", false, null);

                    #region Gera o crédito para as contas removidas

                    if (gerarCreditoContasRemovidas)
                    {
                        foreach (string idConta in idsContasRemovidas.Split(','))
                        {
                            uint idContaPg;
                            if (!uint.TryParse(idConta, out idContaPg))
                                continue;

                            ContasPagar contaPag = contasPagar.Find(new Predicate<ContasPagar>(
                                delegate (ContasPagar conta)
                                {
                                    return conta.IdContaPg == idContaPg;
                                }
                            ));

                            if (cp == null)
                                continue;

                            // Adiciona o crédito à variável de controle
                            if (!creditoGeradoFornec.ContainsKey(contaPag.IdFornec.Value))
                                creditoGeradoFornec.Add(contaPag.IdFornec.Value, contaPag.ValorPago);
                            else
                                creditoGeradoFornec[contaPag.IdFornec.Value] += contaPag.ValorPago;

                            // Gera a movimentação no caixa geral
                            gerarCreditoFornec += CaixaGeralDAO.Instance.MovCxPagto(transaction, idPagto, null, contaPag.IdFornec.Value,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado),
                                2, contaPag.ValorPago, 0, null, "Retificação de pagamento", 0, false, null) + ",";
                        }

                        foreach (uint f in creditoGeradoFornec.Keys)
                            FornecedorDAO.Instance.CreditaCredito(transaction, f, creditoGeradoFornec[f]);
                    }

                    #endregion

                    //RICARDO ALTERAÇÃO
                    //uint retorno = PagarContas(idPagto, idFornec, cp, chequesAssoc, vetJurosMulta, dataPagto, datasFormasPagto, valores, formasPagto, idContasBanco,
                    //    tiposCartao, numParcCartao, boletos, antecipFornec, chequesPagto, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial,
                    //    numAutConstrucard, totalMulta, totalJuros, totalPago, totalASerPago);

                    //RICARDO ALTERAÇÃO
                    uint retorno = PagarContas(transaction, idFornec, contas, chequesAssoc, vetJurosMulta,
                            dataPagto, datasFormasPagto, valores, formasPagto, idContasBanco, tiposCartao, numParcCartao, boletos,
                            antecipFornec, chequesPagto, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial, numAutConstrucard);

                    // Salva as alterações de retificação
                    LogAlteracaoDAO.Instance.LogPagto(pagto);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        #endregion

        #region Valida Pagamento
        
        /// <summary>
        /// Valida as informações do pagto e calcula o total pago e o total a ser pago
        /// Método criado para validar os dados do pagto ao pagar e ao retificar
        /// </summary>
        private void ValidaPagto(GDASession session, uint idFornec, ContasPagar[] contas, ref decimal[] valores, uint[] formasPagto,
            ref decimal totalPago, ref decimal totalASerPago, decimal creditoUtilizado, decimal totalJuros, decimal totalMulta,
            decimal desconto, bool pagtoParcial, bool gerarCredito, string vetJurosMulta, bool retificar, bool gerarCreditoContasRemovidas)
        {
            uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

            // Apenas Financeiro Pagto. pode pagar contas
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                throw new Exception("Apenas funcionários Financeiro Pagamento podem efetuar pagamento de contas.");

            totalPago = 0;

            if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && creditoUtilizado > 0)
                totalPago += creditoUtilizado;

            totalASerPago = totalJuros + totalMulta;
            foreach (ContasPagar c in contas)
                totalASerPago += c.ValorVenc;

            totalASerPago -= desconto;

            // Não permite pagar contas de fornecedor caso o crédito com o mesmo esteja negativo
            if (FornecedorDAO.Instance.GetCredito(session, idFornec) < 0)
                throw new Exception("Não é possível pagar contas de fornecedor que possua crédito negativo. Regularize esta situação antes de efetuar o pagamento.");

            for (int i = 0; i < valores.Length; i++)
                if (valores[i] > 0)
                {
                    if (formasPagto[i] == (int)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec)
                    {
                        // Faz a validação de antecipação de fornecedor
                        if (!FinanceiroConfig.UsarPgtoAntecipFornec)
                            throw new Exception("A antecipação de fornecedor está desabilitada.");

                        else if (FornecedorConfig.TipoUsoAntecipacaoFornecedor != DataSources.TipoUsoAntecipacaoFornecedor.ContasPagar)
                            throw new Exception("Não é possível utilizar a antecipação de fornecedor para o pagamento. Vincule a antecipação diretamente à compra/NF-e.");
                    }
                    else
                        totalPago += valores[i];
                }

            for (int i = 0; i < valores.Length; i++)
                if (valores[i] > 0 && formasPagto[i] == (int)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec)
                {
                    // Calcula o valor a ser utilizado de cada antecipação
                    valores[i] = Math.Max(Math.Min(valores[i], totalASerPago - totalPago), 0);
                    totalPago += valores[i];
                }

            // Se for pagto. parcial, o valor pago não pode ser superior ao valor do pagamento
            if (pagtoParcial)
            {
                // Se o fornecedor não tiver sido informado e mais de uma conta estiver sendo paga, não permite, a menos que sejam contas
                // de comissionados
                if (idFornec == 0 && contas.Length > 1)
                {
                    string idsContaPg = String.Empty;
                    foreach (ContasPagar cp in contas)
                        idsContaPg += cp.IdContaPg + ",";

                    // Verifica se as contas a pagar deste pagamento possuem o mesmo idComissao
                    object objIdComissao = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idComissao) as char) From contas_pagar 
                        Where idContaPg In (" + idsContaPg.TrimEnd(',') + ") Group By idComissao");

                    if (objIdComissao == null || objIdComissao.ToString() == String.Empty || objIdComissao.ToString().Contains(","))
                        throw new Exception("A opção pagamento parcial só pode ser utilizada quando todas as contas a pagar forem do mesmo fornecedor.");
                }

                if (totalPago > totalASerPago)
                    throw new Exception("Valor pago excede o valor a ser pagamento.");
            }

            // Se for para gerar crédito, o total a ser pago deve ser menor ou igual que o valor pago
            else if (FinanceiroConfig.FormaPagamento.CreditoFornecedor && (gerarCredito || gerarCreditoContasRemovidas) && Math.Round(totalASerPago, 2) > Math.Round(totalPago, 2))
                throw new Exception("Total a ser pago é maior que o valor pago. Total a ser pago: " + Math.Round(totalASerPago, 2).ToString("C") + " Valor pago: " + Math.Round(totalPago, 2).ToString("C"));
            
            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito
            else if (!gerarCreditoContasRemovidas && !gerarCredito && Math.Round(totalASerPago, 2) != Math.Round(totalPago, 2))
                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + Math.Round(totalASerPago, 2).ToString("C") + " Valor pago: " + Math.Round(totalPago, 2).ToString("C"));

            // Se tiver juros e multa, obriga a selecionar um plano de conta de juros e multa
            if (!String.IsNullOrEmpty(vetJurosMulta) && (FinanceiroConfig.PlanoContaJurosPagto == 0 ||
                FinanceiroConfig.PlanoContaMultaPagto == 0))
                throw new Exception("Associe os planos de contas referente ao juros e multa de pagamentos.");

            bool impedirPagtoLoja = FinanceiroConfig.FinanceiroPagto.ImpedirPagamentoPorLoja && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador;
            uint idLojaUsuario = UserInfo.GetUserInfo.IdLoja;

            // Verifica se as contas a pagar já foram pagas e existem
            foreach (ContasPagar c in contas)
            {
                /*
                if (!ExisteContaPg(c.IdContaPg))
                    throw new Exception("Uma das contas a pagar não existe mais, provavelmente a compra que gerou a mesma foi cancelada.");
                */
                if (impedirPagtoLoja && c.IdLoja > 0 && c.IdLoja != idLojaUsuario)
                    throw new Exception("Uma das contas a pagar selecionadas pertence à " + LojaDAO.Instance.GetNome(session, c.IdLoja.GetValueOrDefault()) + 
                        ". Não é permitido pagar contas de outra loja.");

                if (!retificar && c.Paga)
                    throw new Exception("Uma das contas a pagar selecionadas já foi paga.");
            }
        }

        #endregion

        #region Renegociar Contas

        #region Métodos privados

        /// <summary>
        /// Renegocia contas a pagar, gerando um pagamento e novas contas com as datas e valores estipulados.
        /// </summary>
        private uint RenegociarContas(GDASession session, uint idPagto, uint idFornec, string contas, int numParcelas, DateTime[] datas, decimal[] valores,
            string vetJurosMulta, decimal totalJuros, decimal totalMulta, string obs)
        {
            List<uint> lstIdContaPgBoleto = new List<uint>();

            decimal totalASerPago = totalJuros + totalMulta;
            foreach (ContasPagar c in GetByString(session, contas.Trim().Trim(',')))
                totalASerPago += c.ValorVenc;

            decimal totalPago = 0;
            foreach (decimal v in valores)
                totalPago += v;

            if (Math.Round(totalASerPago, 2) != Math.Round(totalPago, 2))
                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + Math.Round(totalASerPago, 2).ToString("C") + " Valor pago: " + Math.Round(totalPago, 2).ToString("C"));

            try
            {
                // Dados que serão usados nas contas que serão geradas
                uint idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoRenegociacao);
                uint? idCompra = null;
                uint? idCustoFixo = null;
                uint? idImpostoServico = null;
                uint? idNf = null;
                uint? idComissao = null;
                uint? idLoja = null;
                bool contabil = false;

                // Verifica se as contas a pagar deste pagamento possuem o mesmo plano de contas
                object value = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idConta) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (value != null && value.ToString() != String.Empty && !value.ToString().Contains(","))
                    idConta = Glass.Conversoes.StrParaUint(value.ToString());

                // Verifica se as contas a pagar deste pagamento possuem a mesma compra
                object objIdCompra = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idCompra) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdCompra != null && objIdCompra.ToString() != String.Empty && !objIdCompra.ToString().Contains(","))
                    idCompra = Glass.Conversoes.StrParaUint(objIdCompra.ToString());

                // Verifica se as contas a pagar deste pagamento possuem o mesmo custo fixo
                object objIdCustoFixo = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idCustoFixo) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdCustoFixo != null && objIdCustoFixo.ToString() != String.Empty && !objIdCustoFixo.ToString().Contains(","))
                    idCustoFixo = Glass.Conversoes.StrParaUint(objIdCustoFixo.ToString());

                // Verifica se as contas a pagar deste pagamento possuem o mesmo imposto serviço
                object objIdImpostoServico = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct IdImpostoServ) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdImpostoServico != null && objIdImpostoServico.ToString() != String.Empty && !objIdImpostoServico.ToString().Contains(","))
                    idImpostoServico = Glass.Conversoes.StrParaUint(objIdImpostoServico.ToString());

                // Verifica se as contas a pagar deste pagamento possuem a mesma nf
                object objIdNf = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idNf) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdNf != null && objIdNf.ToString() != String.Empty && !objIdNf.ToString().Contains(","))
                    idNf = Glass.Conversoes.StrParaUint(objIdNf.ToString());

                // Verifica se as contas a pagar deste pagamento possuem o mesmo idComissao
                object objIdComissao = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idComissao) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdComissao != null && objIdComissao.ToString() != String.Empty && !objIdComissao.ToString().Contains(","))
                    idComissao = Glass.Conversoes.StrParaUint(objIdComissao.ToString());

                // Verifica se as contas a pagar deste pagamento são contábeis ou não
                object objContabil = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct contabil) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objContabil != null && objContabil.ToString() != String.Empty && !objContabil.ToString().Contains(","))
                    contabil = objContabil.ToString() == "1";

                // Verifica se as contas a pagar deste pagamento possuem a mesma loja
                object objIdLoja = objPersistence.ExecuteScalar(session, @"Select Cast(group_concat(distinct idLoja) as char) From contas_pagar 
                    Where idContaPg In (" + contas.Trim().Trim(',') + ")");
                if (objIdLoja != null && objIdLoja.ToString() != String.Empty && !objIdLoja.ToString().Contains(","))
                    idLoja = Glass.Conversoes.StrParaUint(objIdLoja.ToString());
                
                if (idFornec == 0)
                    idFornec = GetFornecFromConta(session, Glass.Conversoes.StrParaUint(contas.TrimEnd(' ').TrimEnd(',').Split(',')[0]));

                ContasPagar contaPg;
                for (int i = 0; i < numParcelas; i++)
                {
                    contaPg = new ContasPagar();
                    contaPg.ValorVenc = valores[i];
                    contaPg.DataVenc = datas[i];
                    contaPg.IdFornec = idFornec;
                    contaPg.IdConta = idConta;
                    contaPg.IdLoja = idLoja > 0 ? idLoja.Value : UserInfo.GetUserInfo.IdLoja;
                    contaPg.Paga = false;
                    contaPg.IdPagtoRestante = idPagto;
                    contaPg.IdCompra = idCompra;
                    contaPg.IdCustoFixo = idCustoFixo;
                    contaPg.IdImpostoServ = idImpostoServico;
                    contaPg.IdNf = idNf;
                    contaPg.IdComissao = idComissao;
                    contaPg.Contabil = contabil;
                    lstIdContaPgBoleto.Add(Insert(session, contaPg));
                }

                AtualizaNumParcPagtoRestante(session,idPagto);

                // Marca Contas a Pagar como Pagas
                MarcarComoPaga(session, GetByString(session,  contas.TrimStart(',').TrimEnd(',')), idPagto, vetJurosMulta.TrimEnd('|'), DateTime.Now, true, obs, totalASerPago,
                    totalASerPago, new uint[0], 0); 

                return idPagto;
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao renegociar contas a pagar.", ex));
            }
        }

        #endregion


        /// <summary>
        /// Renegocia contas a pagar, gerando um pagamento e novas contas com as datas e valores estipulados.
        /// </summary>
        public uint RenegociarContasComTransacao(uint idFornec, string contas, int numParcelas, DateTime[] datas, decimal[] valores,
            string vetJurosMulta, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = RenegociarContas(transaction, idFornec, contas, numParcelas, datas, valores, vetJurosMulta, obs);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao pagar contas.", ex));
                }
            }
        }

        /// <summary>
        /// Renegocia contas a pagar, gerando um pagamento e novas contas com as datas e valores estipulados.
        /// </summary>
        public uint RenegociarContas(uint idFornec, string contas, int numParcelas, DateTime[] datas, decimal[] valores,
            string vetJurosMulta, string obs)
        {
            return RenegociarContas(null, idFornec, contas, numParcelas, datas, valores, vetJurosMulta, obs);
        }

        /// <summary>
        /// Renegocia contas a pagar, gerando um pagamento e novas contas com as datas e valores estipulados.
        /// </summary>
        public uint RenegociarContas(GDASession session, uint idFornec, string contas, int numParcelas, DateTime[] datas, decimal[] valores,
            string vetJurosMulta, string obs)
        {
            uint idPagto = 0;
            decimal totalMulta = TotalJurosMulta(vetJurosMulta, 1);
            decimal totalJuros = TotalJurosMulta(vetJurosMulta, 2);
            
            // Cria um novo pagamento
            idPagto = PagtoDAO.Instance.NovoPagto(session, idFornec, valores, totalJuros, totalMulta, obs);
            return RenegociarContas(session, idPagto, idFornec, contas, numParcelas, datas, valores, vetJurosMulta,
                totalJuros, totalMulta, obs);
        }

        /// <summary>
        /// Retifica um pagamento, renegociando contas a pagar, gerando um pagamento e novas contas com as datas e valores estipulados.
        /// </summary>
        public uint RetificarRenegociando(uint idPagto, uint idFornec, string contas, int numParcelas, DateTime[] datas, decimal[] valores,
            string vetJurosMulta, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Recupera o pagamento atual
                    var pagto = PagtoDAO.Instance.GetElementByPrimaryKey(transaction, idPagto);
                    var cheques = ChequesDAO.Instance.GetByPagto(transaction, idPagto);
                    var contasPagar = GetByPagto(transaction, idPagto);
                    var dadosPagto = PagtoPagtoDAO.Instance.GetByPagto(transaction, idPagto);

                    var totalMulta = TotalJurosMulta(vetJurosMulta, 1);
                    var totalJuros = TotalJurosMulta(vetJurosMulta, 2);

                    PagtoDAO.Instance.CancelarPagto(transaction, idPagto, "", false, null);

                    var retorno = RenegociarContas(transaction, idFornec, contas, numParcelas, datas, valores, vetJurosMulta, obs);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        #endregion

        #region Busca por compra/pagto/sinal compra/antecip. fornec.

        /// <summary>
        /// Busca as contas a pagar geradas pelo sinal da compra
        /// </summary>
        public ContasPagar[] GetBySinalCompra(GDASession session, uint idSinalCompra)
        {
            string sql = "Select * From contas_pagar Where idSinalCompra=" + idSinalCompra;

            return objPersistence.LoadData(session, sql).ToArray();
        }

        /// <summary>
        /// Busca as contas a pagar geradas pela compra passada
        /// </summary>
        /// <param name="idCompra"></param>
        public ContasPagar[] GetByCompra(uint idCompra)
        {
            return GetByCompra(null, idCompra);
        }

        /// <summary>
        /// Busca as contas a pagar geradas pela compra passada
        /// </summary>
        /// <param name="sessao, "></param>
        /// <param name="idCompra"></param>
        public ContasPagar[] GetByCompra(GDASession sessao, uint idCompra)
        {
            string sql = "Select * From contas_pagar Where idCompra=" + idCompra;

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        /// <summary>
        /// Verifica se a compra tem contas a pagar à vista.
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public bool TemContaAVista(uint idCompra)
        {
            string sql = "Select count(*) From contas_pagar Where aVista=true and idCompra=" + idCompra;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Busca as contas pagas em um Pagamento
        /// </summary>
        public ContasPagar[] GetByPagto(uint idPagto)
        {
            return GetByPagto(null, idPagto);
        }

        /// <summary>
        /// Busca as contas pagas em um Pagamento
        /// </summary>
        public ContasPagar[] GetByPagto(GDASession session, uint idPagto)
        {
            var buscarReais = !PagtoDAO.Instance.Exists(session, idPagto) ||
                PagtoDAO.Instance.ObtemValorCampo<Pagto.SituacaoPagto>(session, "situacao", "idPagto=" + idPagto) != Pagto.SituacaoPagto.Cancelado;

            var formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            var sql = @"
                Select c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, " + formatoFunc + @") as NomeFornec, 
                    Coalesce(t.nomeFantasia, t.nome) as nomeTransportador,
                    concat(g.descricao, ' - ', pl.Descricao) as DescrPlanoConta, nf.NumeroNfe as NumeroNf 
                From contas_pagar c  
                    Left Join nota_fiscal nf On (c.idNf=nf.idNf) 
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                    Left Join comissao com On (c.idComissao=com.idComissao) 
                    Left Join funcionario fCom On (com.idFunc=fCom.idFunc)
                    Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                    Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                    Left Join transportador t On (c.idTransportador=t.idTransportador)
                Where 1";

            uint[] idContaPg = null;
            decimal[] valorPagoConta = null;

            if (buscarReais)
                sql += " and c.idPagto=" + idPagto;
            else
            {
                Pagto p = new Pagto();
                p.IdsContasPg = PagtoDAO.Instance.ObtemValorCampo<string>(session, "idsContasPg", "idPagto=" + idPagto);
                p.ValoresPg = PagtoDAO.Instance.ObtemValorCampo<string>(session, "valoresPg", "idPagto=" + idPagto);

                sql += " and c.idContaPg in (" + (!String.IsNullOrEmpty(p.IdsContasPg) ? p.IdsContasPg : "0") + ")";

                idContaPg = p.IdContaPg;
                valorPagoConta = p.ValorPagoConta;
            }

            ContasPagar[] contas = objPersistence.LoadData(session, sql).ToArray();
            
            if (!buscarReais)
            {
                for (int i = 0; i < idContaPg.Length; i++)
                {
                    int j = Array.FindIndex<ContasPagar>(contas, new Predicate<ContasPagar>(
                        delegate(ContasPagar c)
                        {
                            return c.IdContaPg == idContaPg[i];
                        }
                    ));

                    if (j == -1)
                        continue;

                    contas[j].ValorPago = valorPagoConta[i];
                }
            }

            return contas;
        }

        /// <summary>
        /// Retorna as contas geradas pela renegociação de pagamento.
        /// </summary>
        public ContasPagar[] GetRenegociadasPagto(uint idPagto)
        {
            return GetRenegociadasPagto(null, idPagto);
        }

        /// <summary>
        /// Retorna as contas geradas pela renegociação de pagamento.
        /// </summary>
        public ContasPagar[] GetRenegociadasPagto(GDASession session, uint idPagto)
        {
            var formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            var sql = @"Select c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, " + formatoFunc + @") as NomeFornec, 
                concat(g.descricao, ' - ', pl.Descricao) as DescrPlanoConta, Coalesce(t.nomeFantasia, t.nome) as nomeTransportador
                From contas_pagar c  
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                    Left Join comissao com On (c.idComissao=com.idComissao) 
                    Left Join funcionario fCom On (com.idFunc=fCom.idFunc)
                    Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                    Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                    Left Join transportador t On (c.idTransportador=t.idTransportador)
                Where idPagtoRestante=" + idPagto + " Order by c.dataVenc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        /// <summary>
        /// Busca as contas a pagar geradas pelo sinal da compra
        /// </summary>
        public ContasPagar[] GetByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            string sql = "Select * From contas_pagar Where idAntecipFornec=" + idAntecipFornec;

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca as contas pagas

        private string SqlPagas(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ, uint idFornec, string nomeFornec, uint formaPagto, string dataIniCad,
            string dataFimCad, string dtIniPago, string dtFimPago, string dtIniVenc, string dtFimVenc, Single valorInicial, Single valorFinal, int tipo, bool comissao, bool renegociadas, bool jurosMulta,
            string planoConta, bool custoFixo, bool selecionar, string ordenar, bool exibirAPagar, int idComissao, int numCte, string observacao, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = exibirAPagar ? "" : " AND Paga = 1";

            var nomeFornecBD = Glass.Configuracoes.FinanceiroConfig.FinanceiroPagto.ExibirRazaoSocialContasPagarPagas ? "f.RazaoSocial, f.NomeFantasia" :
                "f.NomeFantasia, f.RazaoSocial";

            string nomeFornecComis = @"Coalesce(" + nomeFornecBD + ", concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y')))";
            string nomeFornecComisSemData = @"Coalesce(" + nomeFornecBD + ", fCom.Nome, cCom.Nome, iCom.Nome)";

            string campos = selecionar ? @"c.idContaPg, c.IdFornec, c.idConta, c.idAntecipFornec, c.idSinalCompra, c.idCompra, 
                c.idCustoFixo, c.idLoja, c.idChequePagto, c.idEncontroContas, c.idCte, " + SqlCampoDescricaoContaContabil("c") + @" as descricaoContaContabil,
                c.DataVenc, c.ValorVenc, c.DataPagto, c.ValorPago, c.Paga, c.BoletoChegou, c.NumBoleto, c.Multa, c.Juros, c.idPagto, 
                c.idPagtoRestante, " + nomeFornecComis + @" as NomeFornec, c.idFormaPagto, cmp.Nf, " + nomeFornecComisSemData + @" 
                as NomeFornecSemData, Concat(g.descricao, ' - ', pl.Descricao) as DescrPlanoConta, c.idNf, nf.NumeroNfe as NumeroNf, 
                c.contabil, c.obs,
                cmp.obs as obsCompra, c.DataCad, c.UsuCad, c.idComissao, c.renegociada, c.desconto, '^^^' as Criterio, c.aVista,
                c.descontoParc, c.acrescimoParc, c.motivoDescontoAcresc, c.idFuncDescAcresc, c.dataDescAcresc, c.idImpostoServ, DestinoPagto,
                i.obs as obsImpostoServ, c.numParc, c.numParcMax, c.idTransportador, Coalesce(t.nomeFantasia, t.nome) as nomeTransportador, fPag.nome as NomeFuncPagto,
                (CONCAT(COALESCE(GROUP_CONCAT(DISTINCT fp.descricao SEPARATOR ', '),''), IF(c.renegociada, ' (Renegociada)', ''))) as DescrFormaPagto" : "Count(*)";

            string sql = @"
                Select " + campos + @" From contas_pagar c 
                    Left Join imposto_serv i On (c.idImpostoServ=i.idImpostoServ)
                    Left Join nota_fiscal nf On (c.idNf=nf.idNf) 
                    Left Join compra cmp On (c.IdCompra=cmp.idCompra) 
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                    Left Join categoria_conta cc On (g.idCategoriaConta=cc.idCategoriaConta)
                    Left Join pagto pag On (c.idPagto=pag.idPagto) 
                    LEFT JOIN pagto_pagto pagpag ON (pag.idPagto = pagpag.IdPagto) 
                    LEFT JOIN formapagto fp ON (pagpag.IdFormaPagto = fp.IdFormaPagto)
                    Left Join funcionario fPag On (pag.idFuncPagto=fPag.idfunc)
                    Left Join comissao com On (c.idComissao=com.idComissao) 
                    Left Join funcionario fCom On (com.idFunc=fCom.idFunc) 
                    Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                    Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                    Left Join transportador t On (c.idTransportador=t.idTransportador)
                Where 1 ?filtroAdicional?";

            string criterio = String.Empty;

            if (idContaPg > 0)
            {
                filtroAdicional += " AND c.IdContaPg=" + idContaPg;
                criterio = "Cód. conta paga: " + idContaPg + "    ";
                temFiltro = true;
            }

            if (idCompra > 0)
            {
                filtroAdicional += " And c.idCompra=" + idCompra;
                criterio = "Num. Compra: " + idCompra + "    ";
                temFiltro = true;
            }
            else if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " And (" + nomeFornecComisSemData + " Like ?nomeFantasia Or f.razaoSocial Like ?nomeFantasia)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(nf))
            {
                sql += " And (cmp.Nf=?Nf or nf.NumeroNfe=?Nf or i.Nf=?Nf)";
                criterio += "NF/Pedido: " + nf + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniCad))
            {
                filtroAdicional += " And c.DataCad>=?dataIniCad";
                criterio += "Data Início Cad.: " + dataIniCad + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimCad))
            {
                filtroAdicional += " And c.DataCad<=?dataFimCad";
                criterio += "Data Fim Cad.: " + dataFimCad + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtIniPago))
            {
                filtroAdicional += " And c.DataPagto>=?dtIniPago";
                criterio += "Data Início Pagto.: " + dtIniPago + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtFimPago))
            {
                filtroAdicional += " And c.DataPagto<=?dtFimPago";
                criterio += "Data Fim Pagto.: " + dtFimPago + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtIniVenc))
            {
                filtroAdicional += " And c.DataVenc>=?dtIniVenc";
                criterio += "Data Início Venc.: " + dtIniVenc + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtFimVenc))
            {
                filtroAdicional += " And c.DataVenc<=?dtFimVenc";
                criterio += "Data Fim Venc.: " + dtFimVenc + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                filtroAdicional += " AND c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
                temFiltro = true;
            }

            if (idCustoFixo > 0)
            {
                filtroAdicional += " And c.idCustoFixo=" + idCustoFixo;
                criterio += "Custo Fixo: " + idCustoFixo + "    ";
                temFiltro = true;
            }

            if (idImpostoServ > 0)
            {
                filtroAdicional += " AND c.IdImpostoServ=" + idImpostoServ;
                criterio += "Imposto/Serv: " + idImpostoServ + "    ";
                temFiltro = true;
            }

            if (formaPagto > 0)
            {
                sql += " And c.idFormaPagto=" + formaPagto;
                criterio += "Forma Pagto: " + FormaPagtoDAO.Instance.ObtemValorCampo<string>("Descricao", "idFormaPagto=" + formaPagto);
                temFiltro = true;
            }

            if (valorInicial > 0)
            {
                filtroAdicional += " And c.valorPago>=" + valorInicial.ToString().Replace(',', '.');
                criterio += "A partir de: " + valorInicial.ToString("C") + "    ";
                temFiltro = true;
            }

            if (valorFinal > 0)
            {
                filtroAdicional += " And c.valorPago<=" + valorFinal.ToString().Replace(',', '.');
                criterio += "Até: " + valorFinal.ToString("C") + "    ";
                temFiltro = true;
            }

            if (tipo > 0)
            {
                filtroAdicional += tipo == 1 ? " And c.contabil=true" : " And (c.contabil is null or c.contabil=false)";
                criterio += "Tipo: " + (tipo == 1 ? "Contábil" : tipo == 2 ? "Não Contábil" : "N/D");
                temFiltro = true;
            }

            if (comissao)
            {
                filtroAdicional += " and c.idComissao is not null";
                criterio += "Apenas contas de comissão    ";
                temFiltro = true;
            }

            if (!renegociadas)
            {
                filtroAdicional += " and (c.renegociada is null or c.renegociada=false)";
                temFiltro = true;
            }
            else
                criterio += "Exibir contas renegociadas    ";

            if(jurosMulta)
            {
                sql += @" AND (c.Juros > 0 OR c.Multa > 0) ";
            }

            if (!String.IsNullOrEmpty(planoConta))
            {
                sql += " and (pl.descricao like ?planoConta or g.descricao like ?planoConta or cc.descricao like ?planoConta)";
                criterio += "Plano de conta: " + planoConta + "    ";
                temFiltro = true;
            }

            if (custoFixo)
            {
                filtroAdicional += " and idCustoFixo is not null";
                criterio += "Contas de custo fixo    ";
                temFiltro = true;
            }

            if (idComissao > 0)
            {
                filtroAdicional += " AND c.IdComissao = " + idComissao;
                criterio += "Comissão: " + idComissao + "   ";
                temFiltro = true;
            }

            if (numCte > 0)
            {
                var idsCte = string.Join(",", Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemIdCteByNumero((uint)numCte).Select(f => f.ToString()).ToArray());

                if (String.IsNullOrEmpty(idsCte) || String.IsNullOrEmpty(idsCte.Trim(',')))
                    idsCte = "0";

                filtroAdicional += " AND c.idCte IN (" + idsCte + ")";

                criterio += " Num. CT-e:" + numCte + "    ";

                temFiltro = true;
            }
 
            if (!string.IsNullOrEmpty(observacao))
            {
                filtroAdicional += " AND c.Obs LIKE ?observacao";
                criterio += string.Format("Obs.: {0}    ", observacao);
                temFiltro = true;
            }

            sql += " GROUP BY c.IdContaPg";

            if (!String.IsNullOrEmpty(ordenar))
                sql += " order by " + ordenar;

            return sql.Replace("^^^", criterio);
        }

        /// <summary>
        /// Busca todas as contas a pagar pagas para o relatório
        /// </summary>
        public ContasPagar[] GetPagasForRpt(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ, uint idFornec, string nomeFornec, uint formaPagto,
            string dataIniCad, string dataFimCad, string dtIniPago, string dtFimPago, string dtIniVenc, string dtFimVenc, Single valorInicial, Single valorFinal, int tipo, bool comissao,
            bool renegociadas, bool jurosMulta, string planoConta, bool custoFixo, bool exibirAPagar, int idComissao, int numCte, string observacao, string ordenar)
        {
            ordenar = !String.IsNullOrEmpty(ordenar) ? ordenar : "c.DataVenc Asc";

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ, idFornec, nomeFornec, formaPagto, dataIniCad, dataFimCad, dtIniPago, dtFimPago, dtIniVenc, dtFimVenc,
                valorInicial, valorFinal, tipo, comissao, renegociadas, jurosMulta, planoConta, custoFixo, true, ordenar, exibirAPagar, idComissao, numCte, observacao, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            ContasPagar[] lstContasPagar = objPersistence.LoadData(sql, GetParamPagas(nf, nomeFornec, dataIniCad, dataFimCad, dtIniPago, dtFimPago, dtIniVenc, dtFimVenc, planoConta, observacao)).ToArray();

            return lstContasPagar;
        }

        /// <summary>
        /// Busca contas pagas
        /// </summary>
        public IList<ContasPagar> GetPagas(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ, uint idFornec, string nomeFornec, uint formaPagto,
            string dataIniCad, string dataFimCad, string dtIniPago, string dtFimPago, string dtIniVenc, string dtFimVenc, Single valorInicial, Single valorFinal, int tipo, bool comissao,
            bool renegociadas, bool jurosMulta, string planoConta, bool custoFixo, bool exibirAPagar, int idComissao, int numCte, string observacao, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "c.DataVenc Asc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ, idFornec, nomeFornec, formaPagto, dataIniCad, dataFimCad, dtIniPago, dtFimPago, dtIniVenc, dtFimVenc,
                valorInicial, valorFinal, tipo, comissao, renegociadas, jurosMulta, planoConta, custoFixo, true, null, exibirAPagar, idComissao, numCte, observacao, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var lstContasPagar = LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional, GetParamPagas(nf, nomeFornec, dataIniCad, dataFimCad, dtIniPago, dtFimPago,
                dtIniVenc, dtFimVenc, planoConta, observacao)).ToArray();

            return lstContasPagar;
        }

        public int GetPagasCount(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ, uint idFornec, string nomeFornec, uint formaPagto, string dataIniCad,
            string dataFimCad, string dtIniPago, string dtFimPago, string dtIniVenc, string dtFimVenc, Single valorInicial, Single valorFinal, int tipo, bool comissao, bool renegociadas,
            bool jurosMulta, string planoConta, bool custoFixo, bool exibirAPagar, int idComissao, int numCte, string observacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ, idFornec, nomeFornec, formaPagto, dataIniCad, dataFimCad, dtIniPago, dtFimPago, dtIniVenc, dtFimVenc,
                valorInicial, valorFinal, tipo, comissao, renegociadas, jurosMulta, planoConta, custoFixo, true, null, exibirAPagar, idComissao, numCte, observacao, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamPagas(nf, nomeFornec, dataIniCad, dataFimCad, dtIniPago, dtFimPago, dtIniVenc, dtFimVenc, planoConta, observacao));
        }

        /// <summary>
        /// Retorna a qtd de contas pagas da compra
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public int GetPagasCount(uint idCompra)
        {
            return GetPagasCount(null, idCompra);
        }

        /// <summary>
        /// Retorna a qtd de contas pagas da compra
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public int GetPagasCount(GDASession sessao, uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(0, idCompra, null, 0, 0, 0, 0, null, 0, null, null, null, null, null, null, 0, 0, 0, false, true, false, null, false, false, null, false, 0, 0, null, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null);
        }

        /// <summary>
        /// Retorna o valor pago para uma compra.
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public decimal GetPagasTotal(GDASession sessao, uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(0, idCompra, null, 0, 0, 0, 0, null, 0, null, null, null, null, null, null, 0, 0, 0, false, true, false, null, false, true, null, false, 0, 0, null, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return ExecuteScalar<decimal>(sessao, "select sum(valorPago) from (" + sql + ") as temp");
        }

        /// <summary>
        /// Verifica se a entrada da compra está paga.
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public bool IsEntradaPaga(uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagas(0, idCompra, null, 0, 0, 0, 0, null, 0, null, null, null, null, null, null, 0, 0, 0, false, true, false, null, false, true, null, false, 0, 0, null, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount("select count(*) from (" + sql + ") as temp where aVista=true") > 0;
        }

        private GDAParameter[] GetParamPagas(string nf, string nomeFornec, string dataIniCad, string dataFimCad, string dtIniPago, string dtFimPago, string dtIniVenc, string dtFimVenc,
            string planoConta, string observacao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nf))
                lstParam.Add(new GDAParameter("?Nf", nf));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFantasia", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dataIniCad))
                lstParam.Add(new GDAParameter("?dataIniCad", DateTime.Parse(dataIniCad + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimCad))
                lstParam.Add(new GDAParameter("?dataFimCad", DateTime.Parse(dataFimCad + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniPago))
                lstParam.Add(new GDAParameter("?dtIniPago", DateTime.Parse(dtIniPago + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimPago))
                lstParam.Add(new GDAParameter("?dtFimPago", DateTime.Parse(dtFimPago + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniVenc))
                lstParam.Add(new GDAParameter("?dtIniVenc", DateTime.Parse(dtIniVenc + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimVenc))
                lstParam.Add(new GDAParameter("?dtFimVenc", DateTime.Parse(dtFimVenc + " 23:59")));

            if (!String.IsNullOrEmpty(planoConta))
                lstParam.Add(new GDAParameter("?planoConta", "%" + planoConta + "%"));

            if (!string.IsNullOrEmpty(observacao))
                lstParam.Add(new GDAParameter("?observacao", "%" + observacao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca por string
        
        /// <summary>
        /// Retorna as contas a pagar/pagas através de seus IDs.
        /// </summary>
        public ContasPagar[] GetByString(GDASession sessao, string idsContasPg)
        {
            return objPersistence.LoadData(sessao, "select * from contas_pagar where idContaPg in (" + idsContasPg + ")").ToArray();
        }

        #endregion

        #region Busca as contas a pagar para desconto/acréscimo

        /// <summary>
        /// Busca as contas a pagar para desconto/acréscimo.
        /// </summary>
        /// <param name="idsContasPg"></param>
        /// <returns></returns>
        public ContasPagar[] GetForDescontoParcela(int tipoBusca, uint idCompra, uint numeroNf, uint idCustoFixo, uint idImpostoServ,
            Pedido.TipoComissao tipoComissao, uint idFuncComissao, string dataIniComissao, string dataFimComissao, uint numCte)
        {
            var lstParam = new List<GDAParameter>();

            string sql = @"select c.*, f.nomeFantasia as nomeFornec, pc.descricao as descrPlanoConta,
                    Coalesce(t.nomeFantasia, t.nome) as nomeTransportador
                from contas_pagar c
                    left join fornecedor f on (c.idFornec=f.idFornec)
                    left join transportador t on (c.idTransportador=t.idTransportador)
                    left join plano_contas pc on (c.idConta=pc.idConta) " +
                    (tipoBusca != 5 ? @"left join nota_fiscal nf on (c.idNf=nf.idNf) 
                    left join comissao cm on (c.idComissao=cm.idComissao) " :
                    "left join conhecimento_transporte ct on (c.idCte=ct.idCte) ") +
                    "where coalesce(paga, false)=false";

            bool buscar = false;

            if (tipoBusca == 0 && idCompra > 0)
            {
                buscar = true;
                sql += " and c.idCompra=" + idCompra;
            }

            if (tipoBusca == 1 && numeroNf > 0)
            {
                buscar = true;
                sql += " and nf.numeroNFe=" + numeroNf + " and nf.tipoDocumento=" + (int)NotaFiscal.TipoDoc.EntradaTerceiros +
                    " and nf.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros;
            }

            if (tipoBusca == 2 && idFuncComissao > 0)
            {
                buscar = true;

                string nomeCampo = tipoComissao == Pedido.TipoComissao.Funcionario ? "idFunc" :
                    tipoComissao == Pedido.TipoComissao.Comissionado ? "idComissionado" :
                    tipoComissao == Pedido.TipoComissao.Instalador ? "idInstalador" : "";

                sql += " and cm." + nomeCampo + "=" + idFuncComissao;
            }
 
            if (!String.IsNullOrEmpty(dataIniComissao))
            {
                sql += " AND c.dataCad>=?dataIniComissao";
                lstParam.Add(new GDAParameter("?dataIniComissao", DateTime.Parse(dataIniComissao + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(dataFimComissao))
            {
                sql += " AND c.dataCad<=?dataFimComissao";
                lstParam.Add(new GDAParameter("?dataFimComissao", DateTime.Parse(dataFimComissao + " 23:59:59")));
            }

            if (tipoBusca == 3 && idCustoFixo > 0)
            {
                buscar = true;
                sql += " and c.idCustoFixo=" + idCustoFixo;
            }

            if (tipoBusca == 4 && idImpostoServ > 0)
            {
                buscar = true;
                sql += " and c.idImpostoServ=" + idImpostoServ;
            }

            if (tipoBusca == 5 && numCte > 0)
            {
                buscar = true;
                sql += " and ct.numeroCte=" + numCte + " and ct.tipoDocumentoCte=" + (int)ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros +
                    " and ct.situacao=" + (int)ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros;
            }
 
            if (!buscar)
                sql += " and false";

            return objPersistence.LoadData(sql, lstParam.ToArray()).ToArray();
        }

        #endregion

        #region Desconta conta a pagar

        /// <summary>
        /// Marca desconto em parcela
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <param name="desconto"></param>
        /// <param name="motivo"></param>
        public void DescontaAcrescentaContaPagar(uint idContaPg, decimal desconto, decimal acrescimo, string motivo)
        {
            var sql = @"
                UPDATE contas_pagar cp
                    SET cp.valorVenc = (cp.valorVenc + COALESCE(cp.descontoParc, 0) - COALESCE(cp.acrescimoParc,0)) - ?desconto + ?acrescimo
                WHERE idContaPg=" + idContaPg + @";
 
                UPDATE contas_pagar cp
                    SET cp.descontoParc=?desconto, cp.acrescimoParc=?acrescimo, cp.motivoDescontoAcresc=?motivo,
                        cp.idFuncDescAcresc=" + UserInfo.GetUserInfo.CodUser + @", cp.dataDescAcresc=now() 
                WHERE cp.idContaPg=" + idContaPg;

            List<GDAParameter> lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?desconto", desconto));
            lstParam.Add(new GDAParameter("?acrescimo", acrescimo));
            lstParam.Add(new GDAParameter("?motivo", motivo));

            objPersistence.ExecuteCommand(sql, lstParam.ToArray());
        }

        #endregion

        #region Busca contas a pagar que foram dados descontos

        private string SqlContaComDesconto(uint idCompra, uint numeroNf, string dataIni, string dataFim, string dataDescIni, 
            string dataDescFim, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and (cp.descontoParc>0 or cp.acrescimoParc>0)";

            string campos = selecionar ? @"cp.*, fo.NomeFantasia as NomeFornec, f.Nome as nomeFuncDesc, 
                pl.Descricao as DescrPlanoConta, Coalesce(t.nomeFantasia, t.nome) as nomeTransportador, 
                '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From contas_pagar cp
                Left Join transportador t On (cp.idTransportador=t.idTransportador)
                Left Join fornecedor fo On (cp.idFornec=fo.idFornec)
                Left Join funcionario f On (f.idFunc=cp.idFuncDescAcresc) 
                Left Join nota_fiscal nf On (cp.idNf=nf.idNf)
                Left Join plano_contas pl On (cp.IdConta=pl.IdConta)
                Where 1 ?filtroAdicional?";

            if (idCompra > 0)
            {
                filtroAdicional += " and cp.idCompra=" + idCompra;
                criterio += "Compra: " + idCompra + "    ";
            }

            if (numeroNf > 0)
            {
                sql += " and nf.numeroNFe=" + numeroNf;
                criterio += "NF: " + numeroNf + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " And cp.dataVenc>=?dataIni";
                criterio += "Data Inicial: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " And cp.dataVenc<=?dataFim";
                criterio += "Data Final: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataDescIni))
            {
                filtroAdicional += " And cp.dataDescAcresc>=?dataDescIni";
                criterio += "Data Inicial: " + dataDescIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataDescFim))
            {
                filtroAdicional += " And cp.dataDescAcresc<=?dataDescFim";
                criterio += "Data Final: " + dataDescFim + "    ";
            }

            return sql;
        }

        public IList<ContasPagar> GetListContaComDesconto(uint idCompra, uint numeroNf, string dataIni, string dataFim, string dataDescIni, string dataDescFim, string sortExpression, int startRow, int pageSize)
        {
            string sort = sortExpression == null ? "cp.DataVec" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idCompra, numeroNf, dataIni, dataFim, dataDescIni, dataDescFim, true, 
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional, 
                GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim));
        }

        public int GetCountContaComDesconto(uint idCompra, uint numeroNf, string dataIni, string dataFim, string dataDescIni, string dataDescFim)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idCompra, numeroNf, dataIni, dataFim, dataDescIni, dataDescFim, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim));
        }

        public ContasPagar[] GetListContaComDescontoRpt(uint idCompra, uint numeroNf, string dataIni, string dataFim, string dataDescIni, string dataDescFim)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idCompra, numeroNf, dataIni, dataFim, dataDescIni, dataDescFim, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim)).ToArray();
        }

        private GDAParameter[] GetParamContaComDesconto(string dataIni, string dataFim, string dataDescIni, string dataDescFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataDescIni))
                lstParam.Add(new GDAParameter("?dataDescIni", DateTime.Parse(dataDescIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataDescFim))
                lstParam.Add(new GDAParameter("?dataDescFim", DateTime.Parse(dataDescFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Preenche localização de uma conta paga

        /// <summary>
        /// Preenche a localização de cada conta recebida da lista
        /// </summary>
        /// <param name="lst"></param>
        public void PreencheLocalizacao(GDASession sessao, ref ContasPagar[] lst)
        {
            foreach (ContasPagar cp in lst)
            {
                if ((cp.IdPagto == null || cp.IdPagto == 0) && !string.IsNullOrEmpty(cp.DestinoPagto))
                    continue;

                string obj = "";
                foreach (PagtoPagto pagto in PagtoPagtoDAO.Instance.GetByPagto(cp.IdPagto.Value))
                {
                    if (pagto.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                        obj += "Cx. Geral (Dinheiro), ";
                    else if (pagto.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                        obj += "Cheque Próprio" + (pagto.IdContaBanco > 0 ? " " + ContaBancoDAO.Instance.GetElement(pagto.IdContaBanco.Value).Descricao : String.Empty) + ", ";
                    else if (pagto.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                        obj += "Cx. Geral (Cheque Terc.), ";
                    else if (pagto.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito)
                        obj += "Banco" + (pagto.IdContaBanco > 0 ? " " + ContaBancoDAO.Instance.GetElement(pagto.IdContaBanco.Value).Descricao : "") + ", ";
                    else if (pagto.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                        obj += "Boleto, ";
                }

                cp.DestinoPagto = obj.TrimEnd(' ', ',');

                objPersistence.ExecuteCommand(sessao, "update contas_pagar set DestinoPagto=?obj where idContaPg=" + cp.IdContaPg,
                        new GDAParameter("?obj", cp.DestinoPagto));
            }
        }

        #endregion

        #region Busca fornecedor de uma conta a pagar

        /// <summary>
        /// Busca o fornecedor de uma conta a pagar
        /// </summary>
        private uint GetFornecFromConta(GDASession session, uint idContaPg)
        {
            string sql = "Select idFornec From contas_pagar Where idContaPg=" + idContaPg;

            object obj = objPersistence.ExecuteScalar(session, sql);

            if (obj == null || obj.ToString().Trim() == String.Empty)
                return 0;
            else
                return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        /// <summary>
        /// Verifica se os fornecedores das compras são iguais e retorna-o
        /// </summary>
        private uint GetFornecFromVariasContas(GDASession session, string idsContaPg)
        {
            if (String.IsNullOrEmpty(idsContaPg))
                return 0;

            string sql = "Select Cast(Coalesce(group_concat(idFornec), 0) as char) From contas_pagar Where idContaPg In (" + idsContaPg + ")";

            object obj = objPersistence.ExecuteScalar(session, sql);

            if (obj == null || obj.ToString().Trim() == String.Empty || obj.ToString().Contains(","))
                return 0;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }

        #endregion

        #region Busca histórico de um fornecedor

        private string SqlHist(uint idFornec, string dataIniVenc, string dataFimVenc, string dataIniPag, string dataFimPag,
            float vIniVenc, float vFinVenc, float vIniPag, float vFinPag, bool emAberto, bool pagEmDia, bool pagComAtraso,
            string sort, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";

            string totalEmAberto = GetHistValor(1, idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag,
                vFinPag, emAberto, pagEmDia, pagComAtraso).ToString().Replace(',', '.') + " + 0.001";
            string totalEmDia = GetHistValor(2, idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag,
                vFinPag, emAberto, pagEmDia, pagComAtraso).ToString().Replace(',', '.') + " + 0.001";
            string totalComAtraso = GetHistValor(3, idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag,
                vFinPag, emAberto, pagEmDia, pagComAtraso).ToString().Replace(',', '.') + " + 0.001";

            string formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            string campos = selecionar ? @"c.*, coalesce(f.RazaoSocial, f.NomeFantasia, " + formatoFunc + @") as NomeFornec, 
                Coalesce(t.nomeFantasia, t.nome) as nomeTransportador,
                pl.Descricao as DescrPlanoConta, " + totalEmAberto + " as totalEmAberto, " + totalEmDia + " as totalRecEmDia, " + 
                totalComAtraso + @" as totalRecComAtraso, '$$$' as Criterio" : "Count(*) as contagem";

            string sql = "Select " + campos + @" From contas_pagar c
                Left Join transportador t On (c.idTransportador=t.idTransportador)
                Left Join fornecedor f On (c.IdFornec=f.idFornec) 
                Left Join comissao com On (c.idComissao=com.idComissao) 
                Left Join funcionario fCom On (com.idFunc=fCom.idFunc) 
                Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                Left Join plano_contas pl On (c.IdConta=pl.IdConta) Where 1 ?filtroAdicional?";

            string criterio = String.Empty;

            if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniVenc))
            {
                filtroAdicional += " And DATAVENC>=?dtIniVenc";
                criterio += "Data venc.: " + dataIniVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimVenc))
            {
                filtroAdicional += " And DATAVENC<=?dtFimVenc";

                if (!String.IsNullOrEmpty(dataIniVenc))
                    criterio += " até " + dataFimVenc + "    ";
                else
                    criterio += "Data venc.: até " + dataFimVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniPag))
            {
                filtroAdicional += " And DATAPAGTO>=?dtIniPag";
                criterio += "Data pagto.: " + dataIniPag + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimPag))
            {
                filtroAdicional += " And DATAPAGTO<=?dtFimPag";

                if (!String.IsNullOrEmpty(dataIniPag))
                    criterio += " até " + dataFimPag + "    ";
                else
                    criterio += "Data pagto.: até " + dataFimPag + "    ";
            }

            if (vIniVenc > 0)
            {
                filtroAdicional += " And c.valorVenc>=" + vIniVenc.ToString().Replace(',', '.');
                criterio += "Valor venc.: " + vIniVenc.ToString("C") + "    ";
            }

            if (vFinVenc > 0)
            {
                filtroAdicional += " And c.valorVenc<=" + vFinVenc.ToString().Replace(',', '.');

                if (vIniVenc > 0)
                    criterio += "Até: " + vFinVenc.ToString("C") + "    ";
                else
                    criterio += "Valor venc.: até " + vFinVenc.ToString("C") + "    ";
            }

            if (vIniPag > 0)
            {
                filtroAdicional += " And c.valorPago>=" + vIniPag.ToString().Replace(',', '.');
                criterio += "Valor pago: " + vIniPag.ToString("C") + "    ";
            }

            if (vFinPag > 0)
            {
                filtroAdicional += " And c.valorPago<=" + vFinPag.ToString().Replace(',', '.');

                if (vIniPag > 0)
                    criterio += "Até: " + vFinPag.ToString("C") + "    ";
                else
                    criterio += "Valor pago: até " + vFinPag.ToString("C") + "    ";
            }

            if (!emAberto)
                filtroAdicional += " And paga=1";

            if (pagEmDia && !pagComAtraso)
                filtroAdicional += " And (c.dataPagto<=c.dataVenc Or c.dataPagto is null)";
            else if (!pagEmDia && pagComAtraso)
                filtroAdicional += " And (c.dataPagto>c.dataVenc Or c.dataPagto is null)";
            else if (emAberto && !pagEmDia && !pagComAtraso)
                filtroAdicional += " And (paga=0 Or paga is null)";

            if (idFornec == 0)
                sql += " And 1=0";

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Retorna o valor das contas do fornecedor por situação
        /// </summary>
        /// <param name="tipoValor">1-Em aberto, 2-Rec em dia, 3-Rec com atraso</param>
        public decimal GetHistValor(int tipoValor, uint idFornec, string dataIniVenc, string dataFimVenc, string dataIniPag,
            string dataFimPag, float vIniVenc, float vFinVenc, float vIniPag, float vFinPag, bool emAberto, bool pagEmDia, bool pagComAtraso)
        {
            string sql = "Select Coalesce(Sum(c.valorVenc) + 0.001, 0.001) From contas_pagar c ";

            sql += tipoValor == 1 ? "Where (paga=0 Or paga is null) " :
                tipoValor == 2 ? "Where c.dataPagto<=c.dataVenc " :
                tipoValor == 3 ? "Where c.dataPagto>c.dataVenc " : "Where 1";

            if (idFornec > 0)
                sql += " And c.IdFornec=" + idFornec;

            if (!String.IsNullOrEmpty(dataIniVenc))
                sql += " And DATAVENC>=?dtIniVenc";

            if (!String.IsNullOrEmpty(dataFimVenc))
                sql += " And DATAVENC<=?dtFimVenc";

            if (!String.IsNullOrEmpty(dataIniPag))
                sql += " And DATAPAGTO>=?dtIniPag";

            if (!String.IsNullOrEmpty(dataFimPag))
                sql += " And DATAPAGTO<=?dtFimPag";

            if (vIniVenc > 0)
                sql += " And c.valorVenc>=" + vIniVenc.ToString().Replace(',', '.');

            if (vFinVenc > 0)
                sql += " And c.valorVenc<=" + vFinVenc.ToString().Replace(',', '.');

            if (vIniPag > 0)
                sql += " And c.valorPago>=" + vIniPag.ToString().Replace(',', '.');

            if (vFinPag > 0)
                sql += " And c.valorPago<=" + vFinPag.ToString().Replace(',', '.');

            if (!emAberto)
                sql += " And paga=1";

            if (pagEmDia && !pagComAtraso)
                sql += " And (c.dataPagto<=c.dataVenc Or c.dataPagto is null)";
            else if (!pagEmDia && pagComAtraso)
                sql += " And (c.dataPagto>c.dataVenc Or c.dataPagto is null)";
            else if (emAberto && !pagEmDia && !pagComAtraso)
                sql += " And (paga=0 Or paga is null)";

            if (idFornec == 0)
                sql += " And 1=0";

            return ExecuteScalar<decimal>(sql, GetParamHist(dataIniVenc, dataFimVenc, dataIniPag, dataFimPag));
        }

        public ContasPagar[] GetForRptHist(uint idFornec, string dataIniVenc, string dataFimVenc, string dataIniPag, string dataFimPag, float vIniVenc, float vFinVenc, float vIniPag, float vFinPag, bool emAberto, bool pagEmDia, bool pagComAtraso, string sort)
        {
            string filtroAdicional;
            string sql = SqlHist(idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag,
                vFinPag, emAberto, pagEmDia, pagComAtraso, sort, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, GetParamHist(dataIniVenc, dataFimVenc, dataIniPag, dataFimPag)).ToArray();
        }

        public IList<ContasPagar> GetListHist(uint idFornec, string dataIniVenc, string dataFimVenc, string dataIniPag, string dataFimPag, float vIniVenc, float vFinVenc, float vIniPag, float vFinPag, bool emAberto, bool pagEmDia, bool pagComAtraso, string sort, string sortExpression, int startRow, int pageSize)
        {
            if (!String.IsNullOrEmpty(sortExpression))
                sort = "0";
            else
                switch (sort)
                {
                    case "1": // Vencimento
                        sortExpression = "DataVenc desc"; break;
                    case "2": // Pagamento
                        sortExpression = "DataPagto desc"; break;
                    case "3": // Situação (Em aberto, Recebida em dia, Recebida com atraso)
                        sortExpression = "Paga, (DataPagto>DataVenc Or DataPagto is null), DataVenc desc"; break;
                }

            string filtroAdicional;
            string sql = SqlHist(idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag, vFinPag, emAberto, 
                pagEmDia, pagComAtraso, sort, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, 
                GetParamHist(dataIniVenc, dataFimVenc, dataIniPag, dataFimPag));
        }

        public int GetCountHist(uint idFornec, string dataIniVenc, string dataFimVenc, string dataIniPag, string dataFimPag, float vIniVenc, float vFinVenc, float vIniPag, float vFinPag, bool emAberto, bool pagEmDia, bool pagComAtraso, string sort)
        {
            string filtroAdicional;
            string sql = SqlHist(idFornec, dataIniVenc, dataFimVenc, dataIniPag, dataFimPag, vIniVenc, vFinVenc, vIniPag, vFinPag, emAberto,
                pagEmDia, pagComAtraso, sort, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamHist(dataIniVenc, dataFimVenc, dataIniPag, dataFimPag));
        }

        private GDAParameter[] GetParamHist(string dataIniVenc, string dataFimVenc, string dataIniPag, string dataFimPag)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIniVenc))
                lstParam.Add(new GDAParameter("?dtIniVenc", DateTime.Parse(dataIniVenc + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimVenc))
                lstParam.Add(new GDAParameter("?dtFimVenc", DateTime.Parse(dataFimVenc + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniPag))
                lstParam.Add(new GDAParameter("?dtIniPag", DateTime.Parse(dataIniPag + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimPag))
                lstParam.Add(new GDAParameter("?dtFimPag", DateTime.Parse(dataFimPag + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca as contas a pagar que ainda não foram pagas junto com cheques que ainda não foram quitados
        
        private string SqlPagtos(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ, uint idFornec,
            string nomeFornec, string dtIni, string dtFim, string dataCadIni, string dataCadFim, uint[] idsFormaPagto, Single valorInicial,
            Single valorFinal, int tipo, bool incluirCheques, bool incluirChequesPropDev, bool previsaoCustoFixo, bool comissao,
            string planoConta, uint idPagtoRestante, bool custoFixo, bool buscarSomenteComValor, string dtBaixadoIni, string dtBaixadoFim,
            string dtNfCompraIni, string dtNfCompraFim, uint numCte, uint idTransportadora, string nomeTransportadora, int idFuncComissao,
            int idComissao, bool selecionar, string ordenar)
        {
            var nomeFornecBD = Glass.Configuracoes.FinanceiroConfig.FinanceiroPagto.ExibirRazaoSocialContasPagarPagas ? "f.RazaoSocial, f.NomeFantasia" :
                "f.NomeFantasia, f.RazaoSocial";

            string formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            string nomeFornecComisSemData = @"Coalesce(" + nomeFornecBD + ", fCom.Nome, cCom.Nome, iCom.Nome)";
            string campos = selecionar ? @"c.idContaPg, c.IdFornec, c.idConta, c.idCompra, c.idSinalCompra, c.idAntecipFornec, c.idCustoFixo,
                c.idLoja, c.idChequePagto, c.idEncontroContas, c.idCte, " + SqlCampoDescricaoContaContabil("c") + @" as descricaoContaContabil,
                c.DataVenc, c.ValorVenc, c.DataPagto, c.ValorPago, c.Paga, c.BoletoChegou, c.NumBoleto, c.Multa, c.Juros, c.idPagto, 
                c.idPagtoRestante, Coalesce(" + nomeFornecBD + ", " + formatoFunc + @") as NomeFornec,
                " + nomeFornecComisSemData + @" as nomeFornecSemData, Coalesce(t.nomeFantasia, t.nome) as nomeTransportador, c.idFormaPagto, cmp.Nf, 
                Concat(g.descricao, ' - ', pl.Descricao) as DescrPlanoConta, fp.Descricao as FormaPagtoCompra, '$$$' as Criterio, 
                c.DataCad, c.UsuCad, c.idNf, nf.NumeroNfe as NumeroNf, c.contabil, c.obs, cmp.obs as obsCompra, c.idComissao, 
                c.renegociada, c.desconto, c.aVista, false as previsaoCustoFixo, c.descontoParc, c.acrescimoParc, c.motivoDescontoAcresc, 
                c.idFuncDescAcresc, c.dataDescAcresc, c.idImpostoServ, i.obs as obsImpostoServ, c.numParc, c.numParcMax, 
                f.credito as CreditoFornec, c.idTransportador, Coalesce(fCom.nome, cCom.nome, iCom.nome) As NomeComissionado" : "Count(*)";

            string where = String.Empty, criterio = String.Empty, wherePlanoConta = String.Empty;
            string wherePrevisaoCustoFixo = String.Empty;

            if (idContaPg > 0)
            {
                where += string.Format(" AND c.IdContaPg={0}", idContaPg);
                criterio = string.Format("Cód. conta pagar: {0}    ", idContaPg);
            }

            if (idCompra > 0)
            {
                where += " And c.idCompra=" + idCompra;
                criterio = "Num. Compra: " + idCompra + "    ";
            }
            else if (idFornec > 0)
            {
                where += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                where += " And " + nomeFornecComisSemData + " Like ?nomeFantasia";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!String.IsNullOrEmpty(nf))
            {
                where += " And (cmp.Nf=?Nf or nf.NumeroNfe=?Nf or i.Nf=?Nf)";
                criterio += "NF/Pedido: " + nf + "    ";
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                where += " And DataVenc>=?dtIni";
                criterio += "Data Início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                where += " And DataVenc<=?dtFim";
                criterio += "Data Fim: " + dtFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                where += " and c.DataCad>=?dataCadIni";
                criterio += "Período Cad.: " + (!String.IsNullOrEmpty(dataCadFim) ? "de " + dataCadIni : " a partir de " + dataCadIni + "    ");
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                where += " and c.DataCad<=?dataCadFim";
                criterio += (!String.IsNullOrEmpty(dataCadIni) ? " até " + dataCadFim : "Período Cad.: até " + dataCadFim) + "    ";
            }

            if (idsFormaPagto != null && idsFormaPagto.Count() > 0 &&
                !(idsFormaPagto.Count() == 1 && idsFormaPagto[0] == 0))
            {
                var idsFormaPagtoString = string.Join(",", idsFormaPagto);
                where += " and c.idFormaPagto IN (" + idsFormaPagtoString + ")";
                foreach (var id in idsFormaPagto)
                    criterio += "Forma Pagto.: " + FormaPagtoDAO.Instance.GetDescricao(id) + "    ";
            }

            if (idLoja > 0)
            {
                where += " AND c.IdLoja=" + idLoja;
                wherePlanoConta += " AND c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idCustoFixo > 0)
            {
                where += " And c.idCustoFixo=" + idCustoFixo;
                criterio += "Custo Fixo: " + idCustoFixo + "    ";
            }

            if (idImpostoServ > 0)
            {
                where += " And c.idImpostoServ=" + idImpostoServ;
                criterio += "Imposto/Serviço: " + idImpostoServ + "    ";
            }

            if (valorInicial > 0)
            {
                where += " And c.valorVenc>=" + valorInicial.ToString().Replace(',', '.');
                criterio += "A partir de: " + valorInicial.ToString("C") + "    ";
            }

            if (valorFinal > 0)
            {
                where += " And c.valorVenc<=" + valorFinal.ToString().Replace(',', '.');
                criterio += "Até: " + valorFinal.ToString("C") + "    ";
            }

            if (tipo > 0)
            {
                where += tipo == 1 ? " And c.contabil=true" : " And (c.contabil is null or c.contabil=false)";
                criterio += "Tipo: " + (tipo == 1 ? FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil : tipo == 2 ?
                    FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil : "N/D");
            }

            if (comissao)
            {
                where += " and c.idComissao is not null";
                criterio += "Apenas contas de comissão    ";
            }

            if (!String.IsNullOrEmpty(planoConta))
            {
                wherePlanoConta += " and (pl.descricao like ?planoConta or g.descricao like ?planoConta or cc.descricao like ?planoConta)";
                criterio += "Plano de conta: " + planoConta + "    ";
            }

            if (idPagtoRestante > 0)
            {
                where += " and c.idPagtoRestante=" + idPagtoRestante;
                criterio += "Rest. Pagto: " + idPagtoRestante + "    ";
            }

            if (custoFixo)
            {
                where += " And idCustoFixo>0";
                criterio += "Contas de custo fixo    ";
            }

            if (buscarSomenteComValor)
            {
                where += " AND c.valorVenc>0";
                criterio += " Contas com valor    ";
            }

            bool filtroBaixado = false;

            if (!String.IsNullOrEmpty(dtBaixadoIni))
            {
                where += " And (DataPagto is null Or DataPagto>=?dtBaixadoIni)";
                criterio += "Data Baixado Início: " + dtBaixadoIni + "    ";
                filtroBaixado = true;
            }

            if (!String.IsNullOrEmpty(dtBaixadoFim))
            {
                where += " And (DataPagto is null Or DataPagto<=?dtbaixadoFim)";
                criterio += "Data Baixado Fim: " + dtBaixadoFim + "    ";
                filtroBaixado = true;
            }

            bool filtroNfCompra = false;

            if (!String.IsNullOrEmpty(dtNfCompraIni) && !String.IsNullOrEmpty(dtNfCompraFim))
            {
                if (!string.IsNullOrEmpty(dtNfCompraIni))
                {
                    where += " AND IF(COALESCE(c.idNf, 0) > 0, Coalesce(nf.dataEmissao,nf.dataSaidaEnt)>=?dtNfCompraIni, IF(COALESCE(c.idCompra, 0) > 0, cmp.dataCad>=?dtNfCompraIni, 1))";
                    criterio += "Data NF-e/Compra Início: " + dtNfCompraIni + "     ";
                    filtroNfCompra = true;
                }

                if (!string.IsNullOrEmpty(dtNfCompraFim))
                {
                    where += " AND IF(COALESCE(c.idNf, 0) > 0, Coalesce(nf.dataEmissao,nf.dataSaidaEnt)<=?dtNfCompraFim, IF(COALESCE(c.idCompra, 0) > 0, cmp.dataCad<=?dtNfCompraFim, 1))";
                    criterio += "Data NF-e/Compra Fim: " + dtNfCompraFim + "     ";
                    filtroNfCompra = true;
                }
            }

            if (filtroNfCompra)
                where += " AND (COALESCE(c.idNf, 0) > 0 OR COALESCE(c.idCompra, 0) > 0)";

            if (numCte > 0)
            {
                var idsCte = string.Join(",", Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemIdCteByNumero((uint)numCte).Select(f => f.ToString()).ToArray());

                if (String.IsNullOrEmpty(idsCte) || String.IsNullOrEmpty(idsCte.Trim(',')))
                    idsCte = "0";

                where += " AND c.idCte IN (" + idsCte + ")";
                criterio += " Num. CT-e:" + numCte + "    ";
            }

            if (idTransportadora > 0)
            {
                where += " AND c.idTransportador=" + idTransportadora;
                criterio += "Transportadora: " + TransportadorDAO.Instance.GetNome(idTransportadora) + "  ";
            }
            else if (!string.IsNullOrEmpty(nomeTransportadora))
            {
                var idsTransportadoras = TransportadorDAO.Instance.GetList(0, nomeTransportadora).Select(f => f.IdTransportador).ToList();
                where += " AND c.idTransportador IN(" + string.Join(",", idsTransportadoras.Select(f => f.ToString()).ToArray()) + ")";
                criterio += "Transportadora: " + nomeTransportadora + "    ";
            }

            if (idFuncComissao > 0)
            {
                where += " AND com.IdFunc = " + idFuncComissao;
                criterio += "Func. Comissão: " + FuncionarioDAO.Instance.GetNome((uint)idFuncComissao) + "  ";
            }

            if (idComissao > 0)
            {
                where += " AND c.IdComissao = " + idComissao;
                criterio += "Comissão: " + idComissao + "   ";
            }

            string sql = "(Select " + campos + @" From contas_pagar c 
                Left Join transportador t On (c.idTransportador=t.idTransportador)
                Left Join imposto_serv i On (c.idImpostoServ=i.idImpostoServ)
                Left Join compra cmp On (c.IdCompra=cmp.idCompra) 
                Left Join formapagto fp On (c.idFormaPagto=fp.idFormaPagto) 
                Left Join fornecedor f On (c.idFornec=f.idFornec) 
                Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                Left Join categoria_conta cc On (g.idCategoriaConta=cc.idCategoriaConta)
                Left Join nota_fiscal nf On (c.idNf=nf.idNf) 
                Left Join conhecimento_transporte cte On (c.IdCte=cte.IdCte) 
                Left Join comissao com On (c.idComissao=com.idComissao)
                Left Join funcionario fCom On (com.idFunc=fCom.idFunc) 
                Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                Where " + (filtroBaixado ? "1 " : "(paga=false or paga is null) ") + where + wherePlanoConta;

            // Busca os cheques próprios juntamente com as contas a pagar
            if ((incluirCheques || incluirChequesPropDev) && idCompra == 0 && idImpostoServ == 0 && String.IsNullOrEmpty(nf) && !comissao)
            {
                sql += selecionar ? @") union (Select null as idContaPg, null as IdFornec, null as idConta, null as idCompra, null as idSinalCompra, 
                    null as idAntecipFornec, null as idCustoFixo, null as idLoja, null as idChequePagto, null as idEncontroContas, null as idCte, 
                    null as descricaoContaContabil, c.DataVenc, c.Valor as ValorVenc, null as DataPagto, 0 as ValorPago, null as Paga, null as BoletoChegou, 
                    null as NumBoleto, 0 as Multa, 0 as Juros, null as idPagto, p.idPagto as idPagtoRestante, Coalesce(f.NomeFantasia, f.RazaoSocial, 
                    'Fornecedores Diversos') as NomeFornec, Coalesce(f.NomeFantasia, f.RazaoSocial, 'Fornecedores Diversos') as NomeFornecSemData, 
                    null as nomeTransportador, null as idFormaPagto, null as Nf, Concat('Pagamento de Cheque (Num. Cheque: ', Cast(c.Num as CHAR), ')') as DescrPlanoConta, 
                    'Cheque Próprio' as FormaPagtoCompra, '$$$' as Criterio, c.DataCad, c.usuCad as UsuCad, null as IdNf, null as NumeroNf, 
                    false as contabil, null as Obs, null as ObsCompra, null as idComissao, false as renegociada, null as desconto, 
                    false as aVista, false as previsaoCustoFixo, null as descontoParc, null as acrescimoParc, null as motivoDescontoAcresc, 
                    null as idFuncDescAcresc, null as dataDescAcresc, null as idImpostoServ, null as obsImpostoServ, null as numParc, 
                    null as numParcMax, null as creditoFornec, null as idTransportador, Null As NomeComissionado" : ") + (Select Count(*)";
                
                sql += @" From cheques c Left Join pagto_cheque pc On (c.idCheque=pc.idCheque) 
                    Left Join pagto p On (pc.idPagto=p.idPagto) 
                    Left Join fornecedor f On (p.idFornec=f.idFornec) 
                    Where c.Tipo=1 And c.Situacao in (" + (incluirCheques ? ((int)Cheques.SituacaoCheque.EmAberto).ToString() : "") + 
                        (incluirCheques && incluirChequesPropDev ? "," : "") + (incluirChequesPropDev ? ((int)Cheques.SituacaoCheque.Devolvido).ToString() : "") + ")";

                if (!String.IsNullOrEmpty(dtIni))
                    sql += " And c.DataVenc>=?dtIni";

                if (!String.IsNullOrEmpty(dtFim))
                    sql += " And c.DataVenc<=?dtFim";

                if (valorInicial > 0)
                    sql += " And c.valor>=" + valorInicial.ToString().Replace(',', '.');

                if (valorFinal > 0)
                    sql += " And c.valor<=" + valorFinal.ToString().Replace(',', '.');

                if (idFornec > 0)
                    sql += " And f.idFornec=" + idFornec;
                else if (!String.IsNullOrEmpty(nomeFornec))
                    sql += " And f.idFornec In (Select IdFornec From fornecedor Where NomeFantasia Like ?nomeFantasia)";

                if (idsFormaPagto != null && idsFormaPagto.Count() > 0)
                    foreach (var id in idsFormaPagto)
                        if (id != (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio)
                            sql += " And false";

                if (!String.IsNullOrEmpty(planoConta))
                    sql += " And 'Pagamento de Cheque' like ?planoConta";
            }

            if (previsaoCustoFixo)
            {
                {
                    sql += selecionar ? @") union (select null as idContaPg, idFornec, idConta, null as idCompra, null as idSinalCompra, null as idAntecipFornec, 
                    idCustoFixo, idLoja, null as idChequePagto, null as idEncontroContas, null as idCte, null as descricaoContaContabil, 
                    ((data.data - INTERVAL DAY(data.data) DAY) + INTERVAL diaVenc DAY) as dataVenc, valorVenc, null as dataPagto, 
                    0 as valorpago, null as paga, null as boletoChegou, null as numBoleto, 0 as multa, 0 as juros, null as idPagto, 
                    null as idPagtoRestante, nomeFornec, nomeFornec as nomeFornecSemData, null as nomeTransportador, null as idFormaPagto, null as nf, 
                    descrPlanoConta, null as formaPagtoCompra, '$$$' as criterio, null as dataCad, null as usuCad, null as idNf, null as numeroNf, contabil, null as obs, 
                    null as obsCompra, null as idComissao, false as renegociada, null as desconto, false as aVista, 
                    true as previsaoCustoFixo, null as descontoParc, null as acrescimoParc, null as motivoDescontoAcresc, 
                    null as idFuncDescAcresc, null as dataDescAcresc, null as idImpostoServ, null as obsImpostoServ, null as numParc,
                    null as numParcMax, null as CreditoFornec, null as idTransportador, Null As NomeComissionado" : ") + (select count(*)";

                    wherePrevisaoCustoFixo = where;

                    if (!string.IsNullOrEmpty(dtIni))
                        wherePrevisaoCustoFixo = wherePrevisaoCustoFixo.Replace("?dtIni", "?dtPrevCusFixoIni");

                    if (!string.IsNullOrEmpty(dtFim))
                        wherePrevisaoCustoFixo = wherePrevisaoCustoFixo.Replace("?dtFim", "?dtPrevCusFixoFim");

                    var dataInicialCustoFixo =
                        string.Format("{0}",
                            !string.IsNullOrEmpty(dtIni) ?
                                @"STR_TO_DATE(?dtIni, '%Y-%m-%d') AS Data,
                            STR_TO_DATE(?dtIni, '%Y-%m-%d') AS DataIni" :
                                "NULL AS Data, NULL AS DataIni");

                    var dataFinalCustoFixo =
                    string.Format("{0}",
                        !string.IsNullOrEmpty(dtFim) ?
                            @"STR_TO_DATE(?dtFim, '%Y-%m-%d') AS DataFim" :
                            "NULL AS DataFim");

                    var dataCustoFixo =
                        string.Format(@"SELECT {0}, {1} UNION",
                            dataInicialCustoFixo, dataFinalCustoFixo);

                    sql += string.Format(@"
                    From (
	                    select c.*, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornec, pl.descricao as descrPlanoConta
                        from custo_fixo c 
                            left join plano_contas pl on (c.idConta=pl.idConta)
                            left join grupo_conta g on (pl.idGrupo=g.idGrupo)
                            left join categoria_conta cc on (g.idCategoriaConta=cc.idCategoriaConta)
		                    left join fornecedor f on (c.idFornec=f.idFornec)
                        where 1 {0}
                    ) as dados,	(
                            {1}
		                    SELECT DISTINCT DataVenc AS Data, NULL AS DataIni, NULL AS DataFim
                            from contas_pagar c
                            where (paga=false or paga is null)
                                {2}
	                    ) as data
                    where situacao={3}
                        AND ((data.Data - INTERVAL DAY(data.Data) DAY) + INTERVAL DiaVenc DAY) BETWEEN data.DataIni AND Data.DataFim
                        and cast(concat(idCustoFixo, ',', month(data.data), ',', year(data.data)) as char) not in (
                            select * from (
                                select distinct cast(concat(idCustoFixo, ',', month(dataVenc), ',', year(dataVenc)) as char)
                                from contas_pagar c
                                where idCustoFixo is not null
                                    {4}
                            ) as tbl
                        )", wherePlanoConta, dataCustoFixo, where, (int)CustoFixo.SituacaoEnum.Ativo, wherePrevisaoCustoFixo);

                    // Utiliza a data gerada dinamicamente para o custo fixo para fazer o filtro corretamente.
                    // Ex.: 30/10 até 02/11, não buscava custos fixos do dia 01/11 antes desta alteração
                    if (!string.IsNullOrEmpty(dtIni))
                        sql += " and ((data.data - interval day(data.data) day)+interval diaVenc day)>=?dtIni";

                    if (!string.IsNullOrEmpty(dtFim))
                        sql += " and ((data.data - interval day(data.data) day)+interval diaVenc day)<=?dtFim";

                    if (tipo > 0)
                        sql += " and coalesce(contabil,false)=" + (tipo == 1 ? "true" : "false");
                }

                /* Chamado 32459. */
                if (!string.IsNullOrEmpty(dtIni) && !string.IsNullOrEmpty(dtFim) &&
                    DateTime.Parse(dtIni + " 00:00").Month != DateTime.Parse(dtFim + " 00:00").Month)
                {
                    sql += selecionar ? @") union (select null as idContaPg, idFornec, idConta, null as idCompra, null as idSinalCompra, null as idAntecipFornec, 
                    idCustoFixo, idLoja, null as idChequePagto, null as idEncontroContas, null as idCte, null as descricaoContaContabil, 
                    ((data.data - INTERVAL DAY(data.data) DAY) + INTERVAL diaVenc DAY) as dataVenc, valorVenc, null as dataPagto, 
                    0 as valorpago, null as paga, null as boletoChegou, null as numBoleto, 0 as multa, 0 as juros, null as idPagto, 
                    null as idPagtoRestante, nomeFornec, nomeFornec as nomeFornecSemData, null as nomeTransportador, null as idFormaPagto, null as nf, 
                    descrPlanoConta, null as formaPagtoCompra, '$$$' as criterio, null as dataCad, null as usuCad, null as idNf, null as numeroNf, contabil, null as obs, 
                    null as obsCompra, null as idComissao, false as renegociada, null as desconto, false as aVista, 
                    true as previsaoCustoFixo, null as descontoParc, null as acrescimoParc, null as motivoDescontoAcresc, 
                    null as idFuncDescAcresc, null as dataDescAcresc, null as idImpostoServ, null as obsImpostoServ, null as numParc,
                    null as numParcMax, null as CreditoFornec, null as idTransportador, Null As NomeComissionado" : ") + (select count(*)";

                    wherePrevisaoCustoFixo = where;

                    if (!string.IsNullOrEmpty(dtIni))
                        wherePrevisaoCustoFixo = wherePrevisaoCustoFixo.Replace("?dtIni", "?dtPrevCusFixoIni");

                    if (!string.IsNullOrEmpty(dtFim))
                        wherePrevisaoCustoFixo = wherePrevisaoCustoFixo.Replace("?dtFim", "?dtPrevCusFixoFim");

                    var dataInicialCustoFixo =
                        string.Format("{0}",
                            !string.IsNullOrEmpty(dtFim) ?
                                @"STR_TO_DATE(?dtFim, '%Y-%m-%d') AS Data,
                                STR_TO_DATE(?dtIni, '%Y-%m-%d') AS DataIni" :
                                "NULL AS Data, NULL AS DataIni");

                    var dataFinalCustoFixo =
                    string.Format("{0}",
                        !string.IsNullOrEmpty(dtFim) ?
                            @"STR_TO_DATE(?dtFim, '%Y-%m-%d') AS DataFim" :
                            "NULL AS DataFim");

                    var dataCustoFixo =
                        string.Format(@"SELECT {0}, {1} UNION",
                            dataInicialCustoFixo, dataFinalCustoFixo);

                    sql += string.Format(@"
                    From (
	                    select c.*, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornec, pl.descricao as descrPlanoConta
                        from custo_fixo c 
                            left join plano_contas pl on (c.idConta=pl.idConta)
                            left join grupo_conta g on (pl.idGrupo=g.idGrupo)
                            left join categoria_conta cc on (g.idCategoriaConta=cc.idCategoriaConta)
		                    left join fornecedor f on (c.idFornec=f.idFornec)
                        where 1 {0}
                    ) as dados,	(
                            {1}
		                    SELECT DISTINCT DataVenc AS Data, NULL AS DataIni, NULL AS DataFim
                            from contas_pagar c
                            where (paga=false or paga is null)
                                {2}
	                    ) as data
                    where situacao={3}
                        AND ((data.Data - INTERVAL DAY(data.Data) DAY) + INTERVAL DiaVenc DAY) BETWEEN data.DataIni AND Data.DataFim
                        and cast(concat(idCustoFixo, ',', month(data.data), ',', year(data.data)) as char) not in (
                            select * from (
                                select distinct cast(concat(idCustoFixo, ',', month(dataVenc), ',', year(dataVenc)) as char)
                                from contas_pagar c
                                where idCustoFixo is not null
                                    {4}
                            ) as tbl
                        )", wherePlanoConta, dataCustoFixo, where, (int)CustoFixo.SituacaoEnum.Ativo, wherePrevisaoCustoFixo);

                    // Utiliza a data gerada dinamicamente para o custo fixo para fazer o filtro corretamente.
                    // Ex.: 30/10 até 02/11, não buscava custos fixos do dia 01/11 antes desta alteração
                    if (!string.IsNullOrEmpty(dtIni))
                        sql += " and ((data.data - interval day(data.data) day)+interval diaVenc day)>=?dtIni";

                    if (!string.IsNullOrEmpty(dtFim))
                        sql += " and ((data.data - interval day(data.data) day)+interval diaVenc day)<=?dtFim";

                    if (tipo > 0)
                        sql += " and coalesce(contabil,false)=" + (tipo == 1 ? "true" : "false");
                }

                criterio += "Exibir previsão de custo fixo    ";
            }

            sql += ")";

            if (selecionar)
            {
                sql = "select * from (" + sql + ") as temp";
                if (!String.IsNullOrEmpty(ordenar))
                    sql += " order by " + ordenar;
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Busca todas as contas a pagar que ainda não foram pagas para o relatório
        /// </summary>
        public ContasPagar[] GetPagtosForRpt(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ,
            uint idFornec, string nomeFornec, string dtIni, string dtFim, string dataCadIni, string dataCadFim, string idsFormaPagtoString,
            Single valorInicial, Single valorFinal, int tipo, bool incluirCheques, bool incluirChequesPropDev, bool previsaoCustoFixo,
            bool comissao, string planoConta, uint idPagtoRestante, bool custoFixo, string ordenar, ref decimal[] lstPrevisao,
            bool contasSemValor, string dtBaixadoIni, string dtBaixadoFim, string dtNfCompraIni, string dtNfCompraFim, uint numCte,
            uint idTransportadora, string nomeTransportadora, int idFuncComissao, int idComissao)
        {
            // Sql para buscar gastos com Salários
            string sqlSalarios = "Select Sum(coalesce(salario, 0) + coalesce(gratificacao, 0) + coalesce(auxalimentacao, 0)) " +
                "From funcionario Where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gastos com férias
            string sqlFerias = "Select Sum((coalesce(salario, 0) / 3) / 12) From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gastos com décimo terceiro
            string sqlDecTerc = "Select Sum(coalesce(salario, 0) / 12) From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gastos com IPVA
            string sqlIpva = "Select Coalesce(Sum(Coalesce(ValorIpva, 0) / 12), 0) From veiculo";

            lstPrevisao[0] = ExecuteScalar<decimal>(sqlSalarios);
            lstPrevisao[1] = ExecuteScalar<decimal>(sqlFerias);
            lstPrevisao[2] = ExecuteScalar<decimal>(sqlDecTerc);
            lstPrevisao[3] = ExecuteScalar<decimal>(sqlIpva);

            ordenar = !String.IsNullOrEmpty(ordenar) ? ordenar : "DataVenc Asc";

            var idsFormaPagto = !string.IsNullOrEmpty(idsFormaPagtoString) ? idsFormaPagtoString.Split(',').Select(f=>f.StrParaUint()).ToArray() : null;

            /* Chamado 53571. */
            ordenar = ordenar.Replace("NomeExibir DESC", "NomeFornec DESC, NomeTransportador DESC").Replace("NomeExibir", "NomeFornec, NomeTransportador");

            string sql = SqlPagtos(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ, idFornec, nomeFornec, dtIni, dtFim,
                dataCadIni, dataCadFim, idsFormaPagto, valorInicial, valorFinal, tipo, incluirCheques, incluirChequesPropDev,
                previsaoCustoFixo, comissao, planoConta, idPagtoRestante, custoFixo, !contasSemValor, dtBaixadoIni, dtBaixadoFim,
                dtNfCompraIni, dtNfCompraFim, numCte, idTransportadora, nomeTransportadora, idFuncComissao, idComissao, true, ordenar);

            return objPersistence.LoadData(sql, GetPagtosParam(nf, nomeFornec, dtIni, dtFim, dataCadIni, dataCadFim, dtIni, dtFim,
                planoConta, dtBaixadoIni, dtBaixadoFim, dtNfCompraIni, dtNfCompraFim)).ToArray();
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas
        /// </summary>
        public IList<ContasPagar> GetPagtos(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ,
            uint idFornec, string nomeFornec, string dtIni, string dtFim, string dataCadIni, string dataCadFim, uint[] idsFormaPagto,
            Single valorInicial, Single valorFinal, bool incluirCheques, int tipo, bool previsaoCustoFixo, bool comissao,
            string planoConta, uint idPagtoRestante, bool custoFixo, bool contasSemValor, string dtBaixadoIni, string dtBaixadoFim,
            string dtNfCompraIni, string dtNfCompraFim, uint numCte, uint idTransportadora, string nomeTransportadora, int idFuncComissao,
            int idComissao, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? (comissao && !previsaoCustoFixo && !custoFixo ? "NomeComissionado, DataVenc asc" :
                "DataVenc Asc") : sortExpression;
				
			sort += (!string.IsNullOrEmpty(sort) ? "," : string.Empty) + "IdContaPg ASC";

            /* Chamado 32630. */
            sort += string.Format("{0}IdContaPg ASC", string.IsNullOrEmpty(sort) ? string.Empty : ",");

            /* Chamado 39357. */
            sort = sort.Replace("NomeExibir DESC", "NomeFornec DESC, NomeTransportador DESC").Replace("NomeExibir", "NomeFornec, NomeTransportador");

            return LoadDataWithSortExpression(SqlPagtos(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ, idFornec, nomeFornec,
                dtIni, dtFim, dataCadIni, dataCadFim, idsFormaPagto, valorInicial, valorFinal, tipo, incluirCheques, true,
                previsaoCustoFixo, comissao, planoConta, idPagtoRestante, custoFixo, !contasSemValor, dtBaixadoIni, dtBaixadoFim,
                dtNfCompraIni, dtNfCompraFim, numCte, idTransportadora, nomeTransportadora, idFuncComissao, idComissao, true, null), sort,
                startRow, pageSize,
                GetPagtosParam(nf, nomeFornec, dtIni, dtFim, dataCadIni, dataCadFim, dtIni, dtFim, planoConta, dtBaixadoIni, dtBaixadoFim,
                    dtNfCompraIni, dtNfCompraFim));
        }

        public int GetPagtosCount(int? idContaPg, uint idCompra, string nf, uint idLoja, uint idCustoFixo, uint idImpostoServ,
            uint idFornec, string nomeFornec, string dtIni, string dtFim, string dataCadIni, string dataCadFim, uint[] idsFormaPagto,
            Single valorInicial, Single valorFinal, bool incluirCheques, int tipo, bool previsaoCustoFixo, bool comissao,
            string planoConta, uint idPagtoRestante, bool custoFixo, bool contasSemValor, string dtBaixadoIni, string dtBaixadoFim,
            string dtNfCompraIni, string dtNfCompraFim, uint numCte, uint idTransportadora, string nomeTransportadora,
            int idFuncComissao, int idComissao)
        {
            return objPersistence.ExecuteSqlQueryCount("Select (" + SqlPagtos(idContaPg, idCompra, nf, idLoja, idCustoFixo, idImpostoServ,
                idFornec, nomeFornec, dtIni, dtFim, dataCadIni, dataCadFim, idsFormaPagto, valorInicial, valorFinal, tipo, incluirCheques,
                true, previsaoCustoFixo, comissao, planoConta, idPagtoRestante, custoFixo, !contasSemValor, dtBaixadoIni, dtBaixadoFim,
                dtNfCompraIni, dtNfCompraFim, numCte, idTransportadora, nomeTransportadora, idFuncComissao, idComissao, false, null) + ")",
                GetPagtosParam(nf, nomeFornec, dtIni, dtFim, dataCadIni, dataCadFim, dtIni, dtFim, planoConta, dtBaixadoIni, dtBaixadoFim,
                    dtNfCompraIni, dtNfCompraFim));
        }

        private GDAParameter[] GetPagtosParam(string nf, string nomeFornec, string dtIni, string dtFim, string dtCadIni,
            string dtCadFim, string dtPrevCusFixoIni, string dtPrevCusFixoFim, string planoConta, string dtBaixadoIni, string dtBaixadoFim,
            string dtNfCompraIni, string dtNfCompraFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nf))
                lstParam.Add(new GDAParameter("?Nf", nf));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFantasia", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtBaixadoIni))
                lstParam.Add(new GDAParameter("?dtBaixadoIni", DateTime.Parse(dtBaixadoIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtBaixadoFim))
                lstParam.Add(new GDAParameter("?dtBaixadoFim", DateTime.Parse(dtBaixadoFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dtCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dtCadFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtNfCompraIni))
                lstParam.Add(new GDAParameter("?dtNfCompraIni", DateTime.Parse(dtNfCompraIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtNfCompraFim))
                lstParam.Add(new GDAParameter("?dtNfCompraFim", DateTime.Parse(dtNfCompraFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtPrevCusFixoIni))
            {
                DateTime dataIni = DateTime.Parse(dtPrevCusFixoIni);
                lstParam.Add(new GDAParameter("?dtPrevCusFixoIni", DateTime.Parse("01/" + dataIni.Month + "/" + dataIni.Year + " 00:00")));
            }

            if (!String.IsNullOrEmpty(dtPrevCusFixoFim))
            {
                DateTime dataFim = DateTime.Parse(dtPrevCusFixoFim);
                dataFim = DateTime.Parse(DateTime.DaysInMonth(dataFim.Year, dataFim.Month) + "/" +
                    dataFim.Month + "/" + dataFim.Year + " 23:59");
                lstParam.Add(new GDAParameter("?dtPrevCusFixoFim", dataFim));
            }

            if (!String.IsNullOrEmpty(planoConta))
                lstParam.Add(new GDAParameter("?planoConta", "%" + planoConta + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca as contas a pagar

        internal string SqlCampoDescricaoContaContabil(string aliasContasPagar)
        {
            return String.Format("if({0}.contabil, '{1}', '{2}')", aliasContasPagar,
                FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil);
        }

        private string SqlNaoPagas(uint idCompra, uint idCustoFixo, uint idImpostoServ, string nf, uint idLoja, uint idFornec, string nomeFornec,
            string dtIni, string dtFim, Single valorInicial, Single valorFinal, string tipoContaContabil, int tipoConta, bool custoFixo, bool soAVista, int numCte, bool contasVinculadas,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and Paga <> 1";

            var nomeFornecBD = Glass.Configuracoes.FinanceiroConfig.FinanceiroPagto.ExibirRazaoSocialContasPagarPagas ? "f.RazaoSocial, f.NomeFantasia" : 
                "f.NomeFantasia, f.RazaoSocial";

            string formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            string nomeFornecComisSemData = @"Coalesce(" + nomeFornecBD + ", fCom.Nome, cCom.Nome, iCom.Nome)";
            string campoTipoContaContabil = SqlCampoDescricaoContaContabil("c");
            
            string campos = selecionar ? "c.*, Coalesce(" + nomeFornecBD + ", " + formatoFunc + @") as NomeFornec,
                " + nomeFornecComisSemData + @" as nomeFornecSemData, Coalesce(t.nomeFantasia, t.nome) as nomeTransportador, cmp.Nf, nf.NumeroNfe as NumeroNf, 
                Concat(g.descricao, ' - ', pl.Descricao) as DescrPlanoConta, cmp.Obs as ObsCompra, i.obs as obsImpostoServ,
                fp.Descricao as FormaPagtoCompra, " + campoTipoContaContabil + " as descricaoContaContabil, '$$$' as Criterio" : "Count(*)";

            string sql = @"
                Select " + campos + @" From contas_pagar c 
                    Left Join imposto_serv i On (c.idImpostoServ=i.idImpostoServ)
                    Left Join compra cmp On (c.IdCompra=cmp.idCompra)
                    Left Join formapagto fp On (c.idFormaPagto=fp.idFormaPagto)
                    Left Join nota_fiscal nf On (c.idNf=nf.idNf) 
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                    Left Join comissao com On (c.idComissao=com.idComissao) 
                    Left Join funcionario fCom On (com.idFunc=fCom.idFunc) 
                    Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                    Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                    Left Join transportador t On (c.idTransportador=t.idTransportador)
                Where 1 ?filtroAdicional?";

            string criterio = String.Empty;

            if (idCompra > 0)
            {
                filtroAdicional += " And c.idCompra=" + idCompra;
                criterio = "Num. Compra: " + idCompra + "    ";
            }
            else if (idCustoFixo > 0)
            {
                filtroAdicional += " And c.idCustoFixo=" + idCustoFixo;
                criterio += "Num. Custo Fixo: " + idCustoFixo + "    ";
            }
            else if (idImpostoServ > 0)
            {
                filtroAdicional += " And c.idImpostoServ=" + idImpostoServ;
                criterio = "Num. Lanç. Imposto/Serviço: " + idImpostoServ + "    ";
            }
            else if (contasVinculadas && idFornec > 0)
            {
                filtroAdicional += " And (c.idFornec IN (SELECT IdFornecVinculo FROM fornecedor_vinculo Where idFornec=" + idFornec + ") OR f.IdFornec=" + idFornec + ")";
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + " e fornecedores vinculados    ";
            }
            else if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                if (tipoConta != 1)
                {
                    string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                    filtroAdicional += " And c.idFornec In (" + ids +")";
                    criterio += "Fornecedor: " + nomeFornec + "    ";
                }
                else
                {
                    sql += " And " + nomeFornecComisSemData + " Like ?nomeFantasia";
                    criterio += "Comissionado: " + nomeFornec + "    ";
                    temFiltro = true;
                }
            }

            if (!string.IsNullOrEmpty(nf))
            {
                sql += " And (cmp.Nf=?Nf or nf.NumeroNfe=?Nf or i.Nf=?Nf or cmp.IdPedidoEspelho = ?Nf)";
                criterio += "Nota Fiscal: " + nf + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                filtroAdicional += " And DataVenc>=?dtIni";
                criterio += "Data Início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                filtroAdicional += " And DataVenc<=?dtFim";
                criterio += "Data Fim: " + dtFim + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " AND c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (valorInicial > 0)
            {
                filtroAdicional += " And c.valorVenc>=" + valorInicial.ToString().Replace(',', '.');
                criterio += "Valor: a partir de " + valorInicial.ToString("C") + "    ";
            }

            if (valorFinal > 0)
            {
                filtroAdicional += " And c.valorVenc<=" + valorFinal.ToString().Replace(',', '.');
                criterio += "Valor: até " + valorFinal.ToString("C") + "    ";
            }

            if (!String.IsNullOrEmpty(tipoContaContabil))
            {
                filtroAdicional += " and " + campoTipoContaContabil + "=?tipoContaContabil";
                criterio += "Tipo: " + tipoContaContabil + "    ";
            }

            switch (tipoConta)
            {
                case 1:
                    filtroAdicional += " and c.idComissao is not null";
                    criterio += "Apenas contas de comissão    ";
                    break;
                case 2:
                    filtroAdicional += " and c.idImpostoServ is not null";
                    criterio += "Apenas contas de lanç. imposto/serviço    ";
                    break;
            }

            if (custoFixo)
            {
                filtroAdicional += " and c.idCustoFixo>0";
                criterio += "Apenas contas de custo fixo    ";
            }

            if (soAVista)
            {
                filtroAdicional += " and c.aVista=true";
                criterio += "Apenas contas à vista/entrada    ";
            }

            if (numCte > 0)
            {
                var idsCte = string.Join(",", Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemIdCteByNumero((uint)numCte).Select(f => f.ToString()).ToArray());

                if (String.IsNullOrEmpty(idsCte) || String.IsNullOrEmpty(idsCte.Trim(',')))
                    idsCte = "0";

                filtroAdicional += " AND c.idCte IN (" + idsCte + ")";
                criterio += " Num. CT-e:" + numCte + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas
        /// </summary>
        public IList<ContasPagar> GetNaoPagas(uint idCompra, uint idImpostoServ, string nf, uint idLoja, uint idFornec, string nomeFornec, string dtIni,
            string dtFim, Single valorInicial, Single valorFinal, string tipoContaContabil, int tipoConta, bool custoFixo, int numCte, bool contasVinculadas,
            string sortExpression, int startRow, int pageSize)
        {
            // A ordenação de c.idContaPg deve ser feita pois estava ocorrendo um problema no "LoadDataWithSortExpression",
            // ao recuperar os ids da tabela para otimizar o sql, o mesmo id estava sendo repetido pelas paginas.
            string sort = String.IsNullOrEmpty(sortExpression) ? "c.DataVenc Asc, c.idContaPg" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlNaoPagas(idCompra, 0, idImpostoServ, nf, idLoja, idFornec, nomeFornec, dtIni, dtFim, valorInicial,
                valorFinal, tipoContaContabil, tipoConta, custoFixo, false, numCte, contasVinculadas, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");
            
            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional,
                GetParam(nf, nomeFornec, dtIni, dtFim, tipoContaContabil));
        }

        public int GetNaoPagasCount(uint idCompra, uint idImpostoServ, string nf, uint idLoja, uint idFornec, string nomeFornec, string dtIni,
            string dtFim, string tipoContaContabil, Single valorInicial, Single valorFinal, int tipoConta, bool custoFixo, int numCte, bool contasVinculadas)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlNaoPagas(idCompra, 0, idImpostoServ, nf, idLoja, idFornec, nomeFornec, dtIni, dtFim, valorInicial,
                valorFinal, tipoContaContabil, tipoConta, custoFixo, false, numCte, contasVinculadas, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam(nf, nomeFornec, dtIni, dtFim, tipoContaContabil));
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas de uma compra
        /// </summary>
        public ContasPagar[] GetNaoPagasByCompra(uint idCompra, bool soAVista)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlNaoPagas(idCompra, 0, 0, null, 0, 0, null, null, null, 0, 0, null, 0, false, soAVista, 0, false, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas de um custo fixo
        /// </summary>
        public ContasPagar[] GetNaoPagasByCustoFixo(uint idCustoFixo, bool soAVista)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlNaoPagas(0, idCustoFixo, 0, null, 0, 0, null, null, null, 0, 0, null, 0, false, soAVista, 0, false, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas de um lançamento de imposto/serviço avulso.
        /// </summary>
        public ContasPagar[] GetNaoPagasByImpostoServ(uint idImpostoServ, bool soAVista)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlNaoPagas(0, 0, idImpostoServ, null, 0, 0, null, null, null, 0, 0, null, 0, false, soAVista, 0, false, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToArray();
        }

        private GDAParameter[] GetParam(string nf, string nomeFornec, string dtIni, string dtFim, string tipoContaContabil)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nf))
                lstParam.Add(new GDAParameter("?Nf", nf));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFantasia", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            if (!String.IsNullOrEmpty(tipoContaContabil))
                lstParam.Add(new GDAParameter("?tipoContaContabil", tipoContaContabil));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca as contas a pagar para marcar se o boleto chegou

        private string SqlBoletoChegou(uint idCompra, uint idLoja, string nf, uint idFornec, string nomeFornec, bool selecionar,
            out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and Paga <> 1";

            string formatoFunc = "concat(coalesce(fCom.Nome, cCom.Nome, iCom.Nome), ' - ', date_format(c.DataCad, '%d/%m/%Y'))";
            string campos = selecionar ? "c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, " + formatoFunc + @") as NomeFornec,
                cmp.Nf, Concat(g.Descricao, ' - ', pl.Descricao) as DescrPlanoConta, cmp.Obs as ObsCompra, i.obs as obsImpostoServ,
                Coalesce(t.nomeFantasia, t.nome) as nomeTransportador, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From contas_pagar c 
                    Left Join transportador t On (c.idTransportador=t.idTransportador)
                    Left Join imposto_serv i On (c.idImpostoServ=i.idImpostoServ)
                    Left Join compra cmp On (c.IdCompra=cmp.idCompra) 
                    Left Join nota_fiscal nf On (c.idNf=nf.idNf) 
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo)                 
                    Left Join comissao com On (c.idComissao=com.idComissao) 
                    Left Join funcionario fCom On (com.idFunc=fCom.idFunc) 
                    Left Join comissionado cCom On (com.idComissionado=cCom.idComissionado) 
                    Left Join funcionario iCom On (com.idInstalador=iCom.idFunc) 
                Where 1 ?filtroAdicional?";

            if (idCompra > 0)
            {
                filtroAdicional += " And c.idCompra=" + idCompra;
                criterio = "Num. Compra: " + idCompra + "    ";
            }
            else if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And c.idFornec In (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(nf))
            {
                sql += " And (cmp.Nf Like ?Nf Or nf.numeroNfe=" + nf + ")";
                criterio += "Nota Fiscal: " + nf + "    ";
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Busca as contas a pagar que ainda não foram pagas
        /// </summary>
        public IList<ContasPagar> GetBoletoChegou(uint idCompra, uint idLoja, string nf, uint idFornec, string nomeFornec, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "c.DataVenc Asc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlBoletoChegou(idCompra, idLoja, nf, idFornec, nomeFornec, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro,
                filtroAdicional, GetBoletoChegouParam(nf, nomeFornec));
        }

        public int GetBoletoChegouCount(uint idCompra, uint idLoja, string nf, uint idFornec, string nomeFornec)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlBoletoChegou(idCompra, idLoja, nf, idFornec, nomeFornec, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetBoletoChegouParam(nf, nomeFornec));
        }
        
        private GDAParameter[] GetBoletoChegouParam(string nf, string nomeFornec)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nf))
                lstParam.Add(new GDAParameter("?Nf", nf));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFantasia", "%" + nomeFornec + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }


        #endregion

        #region Exclui contas da compra/comissão passada

        /// <summary>
        /// Exclui todas as contas a pagar da compra passada
        /// </summary>
        /// <param name="idCompra"></param>
        public void DeleteByCompra(GDASession sessao, uint idCompra)
        {
            foreach (ContasPagar c in objPersistence.LoadData(sessao, "select * from contas_pagar where idCompra=" + idCompra).ToList())
                LogCancelamentoDAO.Instance.LogContaPagar(sessao, c, "Cancelamento da Compra " + idCompra, false);

            string sql = "Delete From contas_pagar Where idCompra=" + idCompra;
            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Exclui todas as contas a pagar da comissão passada
        /// </summary>
        /// <param name="idComissao"></param>
        /// <param name="inicioMotivo"></param>
        public void DeleteByComissao(GDATransaction session, uint idComissao, string inicioMotivo)
        {
            foreach (ContasPagar c in objPersistence.LoadData("select * from contas_pagar where idComissao=" + idComissao).ToList())
                LogCancelamentoDAO.Instance.LogContaPagar(session, c, inicioMotivo + " da Comissão " + idComissao, false);

            string sql = "Delete From contas_pagar Where idComissao=" + idComissao;
            objPersistence.ExecuteCommand(session, sql);
        }

        public void DeleteByComissao(uint idComissao, string inicioMotivo)
        {
            DeleteByComissao(null, idComissao, inicioMotivo);
        }

        #endregion

        #region Exclui contas a pagar de um imposto/serviço

        /// <summary>
        /// Exclui contas a pagar de um imposto/serviço
        /// </summary>
        public void DeleteByImpostoServ(GDASession session, uint idImpostoServ)
        {
            string sql = "Delete From contas_pagar Where coalesce(paga,false)=false and idImpostoServ=" + idImpostoServ;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Exclui contas a pagar de um sinal da compra

        /// <summary>
        /// Exclui contas a pagar de um sinal da compra
        /// </summary>
        public void DeleteBySinalCompra(GDASession session, uint idSinalCompra)
        {
            string sql = "Delete From contas_pagar Where coalesce(paga,false)=false and idSinalCompra=" + idSinalCompra;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Exclui contas a pagar de um encontro de contas

        /// <summary>
        /// Exclui contas a pagar de um encontro de contas
        /// </summary>
        public void DeleteByEncontroContas(uint idEncontroContas)
        {
            DeleteByEncontroContas(null, idEncontroContas);
        }

        /// <summary>
        /// Exclui contas a pagar de um encontro de contas
        /// </summary>
        public void DeleteByEncontroContas(GDASession session, uint idEncontroContas)
        {
            string sql = "DELETE FROM contas_pagar WHERE coalesce(paga,false)=false AND idEncontroContas=" + idEncontroContas;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Exclui contas a pagar de uma antecipação de fornecedor

        /// <summary>
        /// Exclui contas a pagar de uma antecipação de fornecedor
        /// </summary>
        public void DeleteByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            string sql = "Delete From contas_pagar Where coalesce(paga,false)=false and idAntecipFornec=" + idAntecipFornec;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Exclui contas a pagar não pagas do custo fixo passado

        /// <summary>
        /// Exclui contas a pagar não pagas do custo fixo passado
        /// </summary>
        /// <param name="idCustoFixo"></param>
        public void DeleteFromCustoFixo(uint idCustoFixo)
        {
            foreach (ContasPagar c in objPersistence.LoadData("SELECT * FROM contas_pagar WHERE paga<>1 AND idPagtoRestante IS NULL AND idCustoFixo=" + idCustoFixo).ToList())
                LogCancelamentoDAO.Instance.LogContaPagar(null, c, "Cancelamento do Custo Fixo " + idCustoFixo, false);

            string sql = "DELETE FROM contas_pagar WHERE paga<>1 AND idPagtoRestante IS NULL AND idCustoFixo=" + idCustoFixo;
            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Excluir contas a pagar que estiverem em idsContasPagar

        /// <summary>
        /// Exclui as contas a pagar passadas por parâmetro
        /// </summary>
        /// <param name="idsContasPagar">Ids das contas a serem excluídos separados por ",". Ex.: 1,2,3</param>
        public void ExcluiContas(string idsContasPagar)
        {
            string sql = "Delete From contas_pagar Where idContaPg In (" + idsContasPagar.TrimEnd(' ').Trim(',') + ")";

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Verifica se Custo Fixo já foi gerado

        public bool ExistsCustoFixo(uint idCustoFixo, string mesAno)
        {
            return ExistsCustoFixo(null, idCustoFixo, mesAno);
        }

        /// <summary>
        /// Verifica se já foi gerada uma Conta a Pagar a partir do custo fixo, do mês e do ano passados
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        public bool ExistsCustoFixo(GDASession sessao, uint idCustoFixo, string mesAno)
        {
            int mes = Glass.Conversoes.StrParaInt(mesAno.Substring(0, 2));
            int ano = Glass.Conversoes.StrParaInt(mesAno.Substring(3, 4));

            string sql = "Select Count(*) From contas_pagar Where idCustoFixo=" + idCustoFixo +
                " And Month(DataVenc)=" + mes + " And Year(DataVenc)=" + ano + " and idPagtoRestante is null";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se Custo Fixo gerado já foi pago

        /// <summary>
        /// Verifica se já foi pago uma Conta a Pagar a partir do custo fixo, do mês e do ano passados
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        public bool IsCustoFixoPago(uint idCustoFixo, string mesAno)
        {
            int mes = Glass.Conversoes.StrParaInt(mesAno.Substring(0, 2));
            int ano = Glass.Conversoes.StrParaInt(mesAno.Substring(3, 4));

            string sql = "Select Count(*) From contas_pagar Where Paga=1 And idCustoFixo=" + idCustoFixo +
                " And Month(DataVenc)=" + mes + " And Year(DataVenc)=" + ano + " AND IdPagtoRestante IS NULL";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se já foi pago uma Conta a Pagar a partir do custo fixo
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        public bool IsCustoFixoPago(uint idCustoFixo)
        {
            string sql = "Select Count(*) From contas_pagar Where Paga=1 And idCustoFixo=" + idCustoFixo;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Busca Conta a Pagar referente ao custo fixo e ao mesAno passados

        /// <summary>
        /// Retorna a conta a pagar referente ao custo fixo e ao mes passados
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mesAno"></param>
        public ContasPagar GetByCustoFixo(uint idCustoFixo, string mesAno)
        {
            int mes = Glass.Conversoes.StrParaInt(mesAno.Substring(0, 2));
            int ano = Glass.Conversoes.StrParaInt(mesAno.Substring(3, 4));

            string sql = "Select * From contas_pagar Where idCustoFixo=" + idCustoFixo +
                " And Month(DataVenc)=" + mes + " And Year(DataVenc)=" + ano + " AND IdPagtoRestante IS NULL";

            List<ContasPagar> lst = objPersistence.LoadData(sql).ToList();

            if (lst.Count == 0)
                throw new Exception("Nenhuma Conta a Pagar encontrada para o Custo Fixo e o Mês informados.");
            else if (lst.Count > 1)
                throw new Exception("Mais de uma Conta a Pagar encontrada para o Custo Fixo e o Mês informados.");
            else
                return lst[0];
        }

        #endregion
        
        #region Cancela contas a pagar provenientes de Custo Fixo

        /// <summary>
        /// Busca as contas a pagar de um mês e ano relacionadas a custos fixos.
        /// </summary>
        /// <param name="mesAno"></param>
        /// <returns></returns>
        public ContasPagar[] GetCustoFixoByMesAno(string mesAno)
        {
            if (String.IsNullOrEmpty(mesAno))
                return new ContasPagar[0];

            string campos = @"cp.*, coalesce(f.NomeFantasia, f.RazaoSocial) as NomeFornec, cf.Descricao as DescrCustoFixo, 
                coalesce(l.NomeFantasia, l.RazaoSocial) as NomeLoja";

            string sql = "Select " + campos + " From contas_pagar cp " +
                "left join fornecedor f on (cp.idFornec=f.idFornec) " +
                "left join custo_fixo cf on (cp.idCustoFixo=cf.idCustoFixo) " +
                "left join loja l on (cp.idLoja=l.idLoja) " +
                "Where cp.idCustoFixo is not null And idPagtoRestante is null" +
                " And Month(DataVenc)=" + Glass.Conversoes.StrParaInt(mesAno.Substring(0, 2)) + " And Year(DataVenc)=" +
                Glass.Conversoes.StrParaInt(mesAno.Substring(3, 4));

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Cancela todos os custos fixo gerados no mesAno passados
        /// </summary>
        /// <param name="mesAno"></param>
        public void CancelaCustoFixo(string mesAno)
        {
            // Busca as contas a pagar relacionadas ao custo fixo do mesAno passado
            ContasPagar[] lstContas = GetCustoFixoByMesAno(mesAno);

            string idsContasPagarAExcluir = String.Empty;

            // Para cada conta a pagar desta compra
            foreach (ContasPagar c in lstContas)
            {
                // Se a conta não tiver paga, guarda seu id para ser excluída
                if (!c.Paga)
                    idsContasPagarAExcluir += c.IdContaPg + ",";
                else
                {
                    // Se esta conta estiver no caixa Geral, e ainda não tiver sido estornada, estorna o valor
                    if (CaixaGeralDAO.Instance.ExisteContaPaga(c.IdContaPg) && !CaixaGeralDAO.Instance.ExisteEstornoConta(c.IdContaPg))
                        CaixaGeralDAO.Instance.EstornaContaPaga(c.IdContaPg, c.ValorPago);
                    
                    // TODO: Implementar estorno de cheques proprios

                    // TODO: Implementar estorno de cheques de terceiros

                }
            }

            try
            {
                // Exclui contas a pagar que não foram pagas
                if (idsContasPagarAExcluir != String.Empty)
                    ExcluiContas(idsContasPagarAExcluir);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao excluir contas a pagar provenientes de custos fixos.", ex));
            }
        }

        /// <summary>
        /// Cancela todos um custo fixo gerado.
        /// </summary>
        /// <param name="mesAno"></param>
        public void CancelaCustoFixo(uint idContaPg)
        {
            if (ObtemValorCampo<bool>("paga", "idContaPg=" + idContaPg))
                throw new Exception("Este custo fixo está pago. Cancele o seu pagamento antes de cancelá-lo.");

            ExcluiContas(idContaPg.ToString());
        }

        #endregion

        #region Marcar se boletou chegou ou não

        /// <summary>
        /// Marcar se boletou chegou ou não
        /// </summary>
        /// <param name="boletoChegou"></param>
        /// <param name="idCompra"></param>
        /// <param name="idContaPg"></param>
        public void BoletoChegou(bool boletoChegou, uint idCompra, uint idContaPg, string dataVenc)
        {
            string sql = "Update contas_pagar set boletoChegou=" + (boletoChegou ? 1 : 0);

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataVenc))
            {
                sql += ", dataVenc=?dataVenc";
                lstParam.Add(new GDAParameter("?dataVenc", DateTime.Parse(dataVenc)));
            }
                
            sql += " Where 1";

            if (idCompra > 0)
                sql += " And idCompra=" + idCompra;
            else if (idContaPg > 0)
                sql += " And idContaPg=" + idContaPg;
            else
                return;
            
            objPersistence.ExecuteCommand(sql, lstParam.Count > 0 ? lstParam.ToArray() : null);
        }

        #endregion

        #region Verifica se a conta a pagar existe e se está paga

        /// <summary>
        /// Verifica se a conta a pagar existe e se está paga
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public bool ExisteContaPg(uint idContaPg)
        {
            string sql = "Select Count(*) From contas_pagar Where idContaPg=" + idContaPg;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se a conta a pagar existe e se está paga
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public bool EstaPaga(uint idContaPg)
        {
            string sql = "Select Count(*) From contas_pagar Where paga=true And idContaPg=" + idContaPg;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se existem contas a pagar/pagas

        /// <summary>
        /// Verifica se existem contas pagas para uma NF
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool ExistePagasNf(GDASession sessao, uint idNf)
        {
            string sql = "select count(*) from contas_pagar where paga=1 and idNf=" + idNf;
            object numero = objPersistence.ExecuteScalar(sessao, sql);

            if (numero == null || numero.ToString() == "")
                return false;

            return Glass.Conversoes.StrParaInt(numero.ToString()) > 0;
        }

        /// <summary>
        /// Verifica se existem contas pagas para um CTe
        /// </summary>
        public bool ExistePagasCte(GDASession session, uint idCte)
        {
            string sql = "select count(*) from contas_pagar where paga=1 and idCte=" + idCte;
            object numero = objPersistence.ExecuteScalar(session, sql);

            if (numero == null || numero.ToString() == "")
                return false;

            return Glass.Conversoes.StrParaInt(numero.ToString()) > 0;
        }

        /// <summary>
        /// Verifica se existe alguma conta a pagar/paga de imposto/serviço
        /// </summary>
        public bool ExisteImpostoServ(GDASession session, uint idImpostoServ)
        {
            return ExecuteScalar<bool>(session, "select count(*) > 0 from contas_pagar where idImpostoServ=" + idImpostoServ);
        }

        #endregion

        #region Verifica se há alguma conta paga na antecipação de fornecedor passada

        /// <summary>
        /// Verifica se há alguma conta recebida na obra passada
        /// </summary>
        public bool ExistePagaAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            string sql = "Select Count(*) From contas_pagar Where paga=true And idAntecipFornec=" + idAntecipFornec;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se as contas pagas possuem juros e ou multa

        /// <summary>
        /// Verifica se as contas pagas possuem juros e ou multa
        /// </summary>
        /// <returns></returns>
        public bool PossuiJurosMulta(uint idPagto)
        {
            return PossuiJurosMulta(null, idPagto);
        }

        /// <summary>
        /// Verifica se as contas pagas possuem juros e ou multa
        /// </summary>
        /// <returns></returns>
        public bool PossuiJurosMulta(GDASession session, uint idPagto)
        {
            string sql = "Select Count(*) From contas_pagar Where idPagto=" + idPagto + " And juros>0 Or multa>0";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Exclui as contas a pagar de uma NF

        /// <summary>
        /// Exclui as contas a pagar de uma NF.
        /// </summary>
        /// <param name="idNf"></param>
        public void DeleteByNf(GDASession sessao, uint idNf)
        {
            foreach (ContasPagar c in objPersistence.LoadData(sessao, "select * from contas_pagar where idNf=" + idNf).ToList())
                LogCancelamentoDAO.Instance.LogContaPagar(sessao, c, "Cancelamento da NFe " + idNf, false);

            objPersistence.ExecuteCommand(sessao, "delete from contas_pagar where idNf=" + idNf);
        }

        #endregion

        #region Exclui as contas a pagar de um CTe

        /// <summary>
        /// Exclui as contas a pagar de um CTe.
        /// </summary>
        public void DeleteByCte(GDASession session, uint idCte)
        {
            foreach (ContasPagar c in objPersistence.LoadData(session, "select * from contas_pagar where idCte=" + idCte).ToList())
                LogCancelamentoDAO.Instance.LogContaPagar(null, c, "Cancelamento do CT-e " + idCte, false);

            objPersistence.ExecuteCommand(session, "delete from contas_pagar where idCte=" + idCte);
        }

        #endregion

        #region Recupera a conta a pagar de uma comissão

        /// <summary>
        /// Recupera a conta a pagar de uma comissão.
        /// </summary>
        /// <param name="idComissao"></param>
        /// <returns></returns>
        public ContasPagar GetByComissao(GDASession sessao, uint idComissao)
        {
            string sql = "select * from contas_pagar where idComissao=" + idComissao;
            ContasPagar[] contas = objPersistence.LoadData(sessao, sql).ToArray();
            return contas.Length > 0 ? contas[0] : null;
        }

        public ContasPagar GetByComissao(uint idComissao)
        {
            return GetByComissao(null, idComissao);
        }

        #endregion

        #region Recupera as contas a pagar de um lançamento de imposto/serviço

        /// <summary>
        /// Recupera as contas a pagar de um lançamento de imposto/serviço.
        /// </summary>
        public ContasPagar[] GetByImpostoServ(uint idImpostoServ)
        {
            return GetByImpostoServ(null, idImpostoServ);
        }

        /// <summary>
        /// Recupera as contas a pagar de um lançamento de imposto/serviço.
        /// </summary>
        public ContasPagar[] GetByImpostoServ(GDASession session, uint idImpostoServ)
        {
            string sql = "select * from contas_pagar where idImpostoServ=" + idImpostoServ;
            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca a conta a pagar gerada por um cheque próprio devolvido

        /// <summary>
        /// Busca a conta a pagar gerada por um cheque próprio devolvido.
        /// </summary>
        public ContasPagar GetByChequeDev(GDASession session, int idCheque, int? idPagto, int? idCreditoFornecedor, int? idSinalCompra)
        {
            if (idPagto.GetValueOrDefault() == 0 && idCreditoFornecedor.GetValueOrDefault() == 0 && idSinalCompra.GetValueOrDefault() == 0)
                return null;

            var valorVenc = ChequesDAO.Instance.ObtemValorCampo<float>(session, "valor", string.Format("idCheque={0}", idCheque));

            var sql = string.Format("SELECT * FROM contas_pagar WHERE ValorVenc=?valor AND IdConta={0} AND IF(IdCheque>0, IdCheque={1}, 1)",
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto), idCheque);

            /* Chamado 56581.
             * Removi a condição que busca a conta a pagar pela propriedade IdPagto, pois, caso a conta seja paga essa propriedade será atualizada com o novo IdPagto,
             * sendo assim, esse método nunca iria retornar contas pagas referentes ao cheque. */
            if (idPagto > 0)
                sql += string.Format(" AND IdPagtoRestante={0}", idPagto);

            if (idCreditoFornecedor > 0)
                sql += string.Format(" AND IdCreditoFornecedor={0}", idCreditoFornecedor);

            if (idSinalCompra > 0)
                sql += string.Format(" AND IdSinalCompra={0}", idSinalCompra);

            var conta = objPersistence.LoadData(session, sql, new GDAParameter("?valor", valorVenc)).ToList();

            return conta.Count > 0 ? conta[0] : null;
        }

        #endregion

        #region Atualiza os números de parcela

        private void AtualizaNumParcBase(string nomeCampo, uint id)
        {
            AtualizaNumParcBase(null, nomeCampo, id);
        }

        private void AtualizaNumParcBase(GDASession sessao, string nomeCampo, uint id)
        {
            ContasPagar[] contas = objPersistence.LoadData(sessao, "select * from contas_pagar where " + nomeCampo + "=" +
                id + " order by !aVista, dataVenc").ToArray();

            StringBuilder sql = new StringBuilder();

            for (int i = 0; i < contas.Length; i++)
                sql.AppendFormat("update contas_pagar set numParc={0}, numParcMax={1} where idContaPg={2}; ",
                    i + 1, contas.Length, contas[i].IdContaPg);

            if (!String.IsNullOrEmpty(sql.ToString()))
                objPersistence.ExecuteCommand(sessao, sql.ToString());
        }

        /// <summary>
        /// Atualiza os números de parcela por compra.
        /// </summary>
        /// <param name="idCompra"></param>
        public void AtualizaNumParcCompra(GDASession sessao, uint idCompra)
        {
            AtualizaNumParcBase(sessao, "idCompra", idCompra);
        }

        /// <summary>
        /// Atualiza os números de parcela por custo fixo.
        /// </summary>
        /// <param name="idCustoFixo"></param>
        public void AtualizaNumParcCustoFixo(uint idCustoFixo)
        {
            AtualizaNumParcBase("idCustoFixo", idCustoFixo);
        }

        public void AtualizaNumParcCustoFixo(GDASession sessao, uint idCustoFixo)
        {
            AtualizaNumParcBase(sessao, "idCustoFixo", idCustoFixo);
        }

        /// <summary>
        /// Atualiza os números de parcela por pagto.
        /// </summary>
        /// <param name="idPagto"></param>
        public void AtualizaNumParcPagto(GDASession sessao, uint idPagto)
        {
            AtualizaNumParcBase(sessao, "idPagto", idPagto);
        }

        public void AtualizaNumParcCreditoFornecedor(GDASession sessao, uint idCreditoFornecedor)
        {
            AtualizaNumParcBase(sessao, "idCreditoFornecedor", idCreditoFornecedor);
        }

        /// <summary>
        /// Atualiza os números de parcela por sinal de compra.
        /// </summary>
        public void AtualizaNumParcSinalCompra(GDASession sessao, uint idSinalCompra)
        {
            AtualizaNumParcBase(sessao, "IdSinalCompra", idSinalCompra);
        }

        /// <summary>
        /// Atualiza os números de parcela por pagto restante.
        /// </summary>
        /// <param name="idPagtoRestante"></param>
        public void AtualizaNumParcPagtoRestante(uint idPagtoRestante)
        {
            AtualizaNumParcPagtoRestante(null, idPagtoRestante);
        }

        /// <summary>
        /// Atualiza os números de parcela por pagto restante.
        /// </summary>
        /// <param name="idPagtoRestante"></param>
        public void AtualizaNumParcPagtoRestante(GDASession sessao ,uint idPagtoRestante)
        {
            AtualizaNumParcBase(sessao,"idPagtoRestante", idPagtoRestante);
        }

        /// <summary>
        /// Atualiza os números de parcela por imposto/serviço.
        /// </summary>
        /// <param name="idPagto"></param>
        public void AtualizaNumParcImpostoServ(GDASession sessao, uint idImpostoServ)
        {
            AtualizaNumParcBase(sessao, "idImpostoServ", idImpostoServ);
        }

        /// <summary>
        /// Atualiza os números de parcela por NF.
        /// </summary>
        /// <param name="idPagto"></param>
        public void AtualizaNumParcNf(GDASession sessao, uint idNf)
        {
            AtualizaNumParcBase(sessao, "idNf", idNf);
        }

        /// <summary>
        /// Atualiza os números de parcelas por conta a pagar.
        /// </summary>
        /// <param name="idContaPg"></param>
        public void AtualizaNumParcContaPg(uint idContaPg)
        {
            string[] campos = new string[] { "idCompra", "idComissao", "idCustoFixo", "idNf", "idImpostoServ", "idPagto", "idPagtoRestante" };
            for (int i = 0; i < campos.Length; i++)
            {
                uint? id = ObtemValorCampo<uint?>(campos[i], "idContaPg=" + idContaPg);
                if (id > 0)
                {
                    AtualizaNumParcBase(campos[i], id.Value);
                    break;
                }
            }
        }

        #endregion

        #region Recupera a referência de uma conta a pagar

        public string GetReferencia(uint idContaPg)
        {
            return GetReferencia(null, idContaPg);
        }

        public string GetReferencia(GDASession session, uint idContaPg)
        {
            string where = "idContaPg=" + idContaPg;

            ContasPagar cp = new ContasPagar();
            cp.IdCompra = ObtemValorCampo<uint?>(session, "idCompra", where);
            cp.IdPagto = ObtemValorCampo<uint?>(session, "idPagto", where);
            cp.IdNf = ObtemValorCampo<uint?>(session, "idNf", where);
            cp.NumeroNf = NotaFiscalDAO.Instance.ObtemValorCampo<uint?>(session, "numeroNfe", "idNf=" + cp.IdNf.GetValueOrDefault());
            cp.Nf = CompraDAO.Instance.ObtemValorCampo<string>(session, "nf", "idCompra=" + cp.IdCompra.GetValueOrDefault());
            cp.NumBoleto = ObtemValorCampo<string>(session, "numBoleto", where);
            cp.IdCustoFixo = ObtemValorCampo<uint?>(session, "idCustoFixo", where);
            cp.IdImpostoServ = ObtemValorCampo<uint?>(session, "idImpostoServ", where);

            return cp.Referencia;
        }

        #endregion

        #region Obtém informações da conta a pagar
        
        /// <summary>
        /// Obtém o tipo de conta a pagar.
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public string ObtemTipoConta(uint idContaPg)
        {
            return ExecuteScalar<string>("select " + SqlCampoDescricaoContaContabil("c") + @"
                from contas_pagar c where c.idContaPg=" + idContaPg);
        }

        public int ObtemIdConta(GDASession session, int idContaPg)
        {
            return ObtemValorCampo<int>(session, "IdConta", "idContaPg = " + idContaPg);
        }

        /// <summary>
        /// Obtem o total pago com permuta das contas informadas
        /// </summary>
        /// <param name="idsContasPg"></param>
        /// <returns></returns>
        public decimal ObtemValorPagtoPermuta(string idsContasPg)
        {
            if (string.IsNullOrEmpty(idsContasPg))
                return 0;

            var sql = @"
                SELECT SUM(ValorPagto) 
                FROM (
                    SELECT pagpag.idPagto, pagpag.ValorPagto
                    FROM contas_pagar pg
                        INNER JOIN pagto_pagto pagpag ON (pg.IdPagto = pagpag.IdPagto)
                    WHERE pagpag.IdFormaPagto = " + (int)Glass.Data.Model.Pagto.FormaPagto.Permuta + @"
                        AND pg.IdContaPg IN(" + idsContasPg + @")
				    GROUP BY pagpag.idPagto
                    ) as tmp";

            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Obtem o total pago com encontro de contas das contas informadas
        /// </summary>
        /// <param name="idsContasPg"></param>
        /// <returns></returns>
        public decimal ObtemValorPagtoEncontroContas(string idsContasPg)
        {
            if (string.IsNullOrEmpty(idsContasPg) || string.IsNullOrWhiteSpace(idsContasPg))
                return 0;

            var sql = @"
                SELECT SUM(pg.ValorVenc)
                FROM contas_pagar pg
	                INNER JOIN contas_pagar_encontro_contas cpec ON (pg.IdContaPg = cpec.IdContaPg)
                WHERE pg.IdContaPg IN(" + idsContasPg + @")
                ";

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Recupera a conta pagar

        public ContasPagar GetElement(uint idContaPg)
        {
            return GetElementByPrimaryKey(idContaPg);
        }

        #endregion

        #region Recupera campos específicos da conta a pagar

        /// <summary>
        /// Insere todas as contas a pagar na tabela contas_pagar_data_base.
        /// </summary>
        public void InserirContasPagarNoHistoricoDeContasAPagar()
        {
            objPersistence.ExecuteCommand(@"INSERT INTO contas_pagar_data_base (IDCONTAPG, IDFORNEC, DATACAD, DATAVENC, VALORVENC, DATACOPIA)
                (SELECT c.IdContaPg, c.IdFornec, c.DataCad, c.DataVenc, c.ValorVenc, NOW()
                FROM contas_pagar c
                WHERE c.Paga = 0
                ORDER BY c.DataVenc)");
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(GDASession sessao, ContasPagar objInsert)
        {
            if (objInsert.IdFormaPagto.GetValueOrDefault() == 0)
                objInsert.IdFormaPagto = (uint)Pagto.FormaPagto.Prazo;

            return base.Insert(sessao, objInsert);
        }

        public override uint Insert(ContasPagar objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Atualiza os dados da conta a pagar/paga.
        /// </summary>
        public override int Update(ContasPagar contaPagar)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var contaPagarOld = GetElementByPrimaryKey(transaction, contaPagar.IdContaPg);

                    // Recupera a descrição da forma de pagamento para alterar o log.
                    if (contaPagarOld.IdFormaPagto > 0)
                        contaPagarOld.FormaPagtoCompra = FormaPagtoDAO.Instance.GetDescricao((Pagto.FormaPagto)contaPagarOld.IdFormaPagto);

                    if (contaPagar.IdFormaPagto > 0)
                        contaPagar.FormaPagtoCompra = FormaPagtoDAO.Instance.GetDescricao((Pagto.FormaPagto)contaPagar.IdFormaPagto);

                    if (!contaPagarOld.Paga && FuncoesData.DateDiff(DateInterval.Year, contaPagarOld.DataVenc, contaPagar.DataVenc) > 2)
                        throw new Exception("A data de vencimento não pode ser alterada com mais de 2 anos de diferença da data original nesta opção.");

                    // Caso a conta esteja paga, não é permitido alterar a Data de Vencimento, Número do Boleto ou a Forma de Pagamento.
                    if (contaPagarOld.Paga)
                    {
                        var mensagemAlerta = "A conta está paga, portanto, não é possível alterar {0}";

                        if (contaPagarOld.DataVenc != contaPagar.DataVenc)
                            throw new Exception(string.Format(mensagemAlerta, "a data de vencimento."));

                        if (contaPagarOld.NumBoleto != contaPagar.NumBoleto)
                            throw new Exception(string.Format(mensagemAlerta, "o número do boleto."));

                        if (contaPagarOld.IdFormaPagto != contaPagar.IdFormaPagto)
                            throw new Exception(string.Format(mensagemAlerta, "a forma de pagamento."));
                    }

                    var retorno = base.Update(transaction, contaPagar);

                    /* Chamado 49106. */
                    LogAlteracaoDAO.Instance.LogContaPagar(transaction, contaPagarOld, contaPagar);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Delete(ContasPagar objDelete)
        {
            return Delete(null, objDelete);
        }

        public override int Delete(GDASession session, ContasPagar objDelete)
        {
            return DeleteByPrimaryKey(session, objDelete.IdContaPg, true);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            return GDAOperations.Delete(sessao, new ContasPagar { IdContaPg = Key });
        }

        private int DeleteByPrimaryKey(uint Key, bool manual)
        {
            return DeleteByPrimaryKey(null, Key, manual);
        }

        private int DeleteByPrimaryKey(GDASession session, uint Key, bool manual)
        {
            ContasPagar objDelete = GetElementByPrimaryKey(session, Key);
            LogCancelamentoDAO.Instance.LogContaPagar(session, objDelete, null, manual);
            return base.DeleteByPrimaryKey(session, Key);
        }

        #endregion

        public string GetTotalContasPagarDespesaVariavel(string dtIni, string dtFim)
        {
            string sql = @"select sum(case cp.paga when 1 then cp.valorPago else cp.valorVenc end) As Valor,
                cp.* from contas_pagar cp
                inner join plano_contas pc on(cp.idConta=pc.idConta)
                inner join grupo_conta gc on(gc.idGrupo=pc.idGrupo)
                where gc.PontoEquilibrio=true and cp.dataVenc >= ?dataIni
                and cp.dataVenc <= ?dataFim and cp.idCustoFixo is null";

            return objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59"))).ToString();
        }

        public string GetTotalContasPagarDespesaFixa(string dtIni, string dtFim)
        {
            string sql = @"select sum(case cp.paga when 1 then cp.valorPago else cp.valorVenc end) As Valor,
                cp.* from contas_pagar cp
                inner join custo_fixo cf on(cp.idCustoFixo=cf.idCustoFixo)
                where cf.PontoEquilibrio=true and cp.dataVenc >= ?dataIni
                and cp.dataVenc <= ?dataFim and cp.idCustoFixo is not null";

            return objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59"))).ToString();
        }

        public uint InsertExecScript(ContasPagar objInsert)
        {
            throw new NotImplementedException("Alterar método para buscar conta já existente.");

            //string sql = @"select idContaPg from contas_pagar where idFornec{0} and idLoja=?idLoja and date(dataVenc)=date(?dataVenc) 
            //    and valorVenc=?valorVenc and numParc=?numParc and idConta=?idConta";

            //var idContaPg = ExecuteScalar<uint?>(String.Format(sql, objInsert.IdFornec != null ? "=" + objInsert.IdFornec : " is null"),
            //    new GDAParameter("?idLoja", objInsert.IdLoja), new GDAParameter("?dataVenc", objInsert.DataVenc), new GDAParameter("?valorVenc", objInsert.ValorVenc),
            //    new GDAParameter("?numParc", objInsert.NumParc), new GDAParameter("?idConta", objInsert.IdConta));

            //if (idContaPg > 0)
            //{
            //    objInsert.IdContaPg = idContaPg.Value;
            //    base.Update(objInsert);
            //    return idContaPg.Value;
            //}
            //else
            //    return base.Insert(objInsert);
        }
    }
}