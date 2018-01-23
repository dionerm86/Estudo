using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Linq;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.Helper
{
    public static class UtilsFinanceiro
    {
        #region Enumeradores

        public enum TipoReceb
        {
            PedidoAVista,
            LiberacaoAVista,
            SinalPedido,
            SinalLiberacao,
            ContaReceber,
            Acerto,
            ChequeDevolvido,
            Obra,
            TrocaDevolucao,
            CreditoValeFuncionario,
            DebitoValeFuncionario,
            DevolucaoPagto,
            ChequeReapresentado,
            ChequeProprioDevolvido,
            ChequeProprioReapresentado,
            LiberacaoAPrazoCheque,
            CartaoNaoIdentificado
        }

        public enum TipoPagto
        {
            ChequeProprio,
            TransfBanco
        }

        #endregion

        #region Métodos de Suporte

        /// <summary>
        /// Verifica se a forma de pagamento desejada está no vetor.
        /// </summary>
        /// <param name="formaPagtoDesejada"></param>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static bool ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagtoDesejada, params uint[] formasPagto)
        {
            return IndexFormaPagto(formaPagtoDesejada, formasPagto) > -1;
        }

        /// <summary>
        /// Retorna o índice da primeira ocorrência da forma de pagamento desejada no vetor.
        /// </summary>
        /// <param name="formaPagtoDesejada"></param>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static int IndexFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagtoDesejada, params uint[] formasPagto)
        {
            for (int i = 0; i < formasPagto.Length; i++)
                if (formasPagto[i] == (uint)formaPagtoDesejada)
                    return i;

            return -1;
        }

        /// <summary>
        /// Retorna a taxa cobrada pelo cartão, dependendo da forma de pagamento.
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static float TaxaCartao(GDASession sessao, uint tipoCartao, uint idLoja, uint parcelas)
        {
            return JurosParcelaCartaoDAO.Instance.GetByTipoCartaoNumParc(sessao, tipoCartao, idLoja, (int)parcelas).Juros;
        }

        /// <summary>
        /// Indica se a forma de pagamento é usando cartão de débito.
        /// </summary>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static bool IsFormaPagtoCartaoDebito(uint[] formasPagto, uint[] tiposCartao)
        {
            for (int i = 0; i < formasPagto.Length; i++)
            {
                if (formasPagto[i] == (uint)Pagto.FormaPagto.Cartao && TipoCartaoCreditoDAO.Instance.ObterTipoCartao(null, (int)tiposCartao[i]) == TipoCartaoEnum.Debito)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indica se a forma de pagamento é usando cartão de débito.
        /// </summary>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static bool IsFormaPagtoCartaoDebito(uint formaPagto, uint tipoCartao)
        {
            return IsFormaPagtoCartaoDebito(new uint[] { formaPagto }, new uint[] { tipoCartao });
        }

        /// <summary>
        /// Indica se a forma de pagamento é usando cartão de crédito.
        /// </summary>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static bool IsFormaPagtoCartaoCredito(uint[] formasPagto, uint[] tiposCartao)
        {
            for (int i = 0; i < formasPagto.Length; i++)
            {
                if (formasPagto[i] == (uint)Pagto.FormaPagto.Cartao && TipoCartaoCreditoDAO.Instance.ObterTipoCartao(null, (int)tiposCartao[i]) == TipoCartaoEnum.Credito)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indica se a forma de pagamento é usando cartão de crédito.
        /// </summary>
        /// <param name="formasPagto"></param>
        /// <returns></returns>
        public static bool IsFormaPagtoCartaoCredito(uint formaPagto, uint tipoCartao)
        {
            return IsFormaPagtoCartaoCredito(new uint[] { formaPagto }, new uint[] { tipoCartao });
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor total de juros dos cartões de crédito/débito se esses forem cobrados dos clientes.
        /// </summary>
        /// <param name="valoresPagos"></param>
        /// <param name="formasPagto"></param>
        /// <param name="tiposCartao"></param>
        /// <param name="numParcelas"></param>
        /// <returns></returns>
        public static decimal GetJurosCartoes(uint idLoja, decimal[] valoresPagos, uint[] formasPagto, uint[] tiposCartao, uint[] numParcelas)
        {
            return GetJurosCartoes(null, idLoja, valoresPagos, formasPagto, tiposCartao, numParcelas);
        }

        /// <summary>
        /// Retorna o valor total de juros dos cartões de crédito/débito se esses forem cobrados dos clientes.
        /// </summary>
        /// <param name="valoresPagos"></param>
        /// <param name="formasPagto"></param>
        /// <param name="tiposCartao"></param>
        /// <param name="numParcelas"></param>
        /// <returns></returns>
        public static decimal GetJurosCartoes(GDASession sessao, uint idLoja, decimal[] valoresPagos, uint[] formasPagto, uint[] tiposCartao, uint[] numParcelas)
        {
            if (!FinanceiroConfig.Cartao.CobrarJurosCartaoCliente)
                return 0;

            decimal juros = 0;
            for (int i = 0; i < valoresPagos.Length; i++)
            {
                if (valoresPagos[i] == 0 || formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao)
                    continue;

                juros += GetValorJuros(sessao, idLoja, valoresPagos[i], formasPagto[i], tiposCartao[i], numParcelas[i]);
            }

            return Math.Round(juros, 2);
        }

        /// <summary>
        /// Retorna o valor de uma parcela, já debitada a taxa do cartão de crédito.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="formaPagto"></param>
        /// <param name="parcelas"></param>
        /// <returns></returns>
        public static decimal GetValorParcela(GDASession sessao, uint idLoja, decimal valor, uint formaPagto, uint tipoCartao, uint parcelas)
        {
            decimal numParcelas = IsFormaPagtoCartaoCredito(formaPagto, tipoCartao) ? (decimal)parcelas : 1;

            if (FinanceiroConfig.Cartao.CobrarJurosCartaoCliente)
                return valor / numParcelas;

            return (valor * ((100 - (decimal)TaxaCartao(sessao, tipoCartao, idLoja, parcelas)) / 100)) / numParcelas;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor total que será pago de juros a um tipo de cartão.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static decimal GetValorJurosParc(uint idLoja, decimal valor, uint formaPagto, uint tipoCartao, uint parcelas, int numeroParc)
        {
            return GetValorJurosParc(null, idLoja, valor, formaPagto, tipoCartao, parcelas, numeroParc);
        }

        /// <summary>
        /// Retorna o valor total que será pago de juros a um tipo de cartão.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static decimal GetValorJurosParc(GDASession sessao, uint idLoja, decimal valor, uint formaPagto, uint tipoCartao, uint parcelas, int numeroParc)
        {
            decimal jurosTotais = GetValorJuros(sessao, idLoja, valor, formaPagto, tipoCartao, parcelas);

            if (!FinanceiroConfig.Cartao.CobrarJurosCartaoCliente)
            {
                decimal juros = Math.Round(jurosTotais / parcelas, 2);
                return parcelas > 1 && numeroParc == 0 ? Math.Round(jurosTotais - (juros * (parcelas - 1)), 2) : juros;
            }
            else
                return jurosTotais;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor total que será pago de juros a um tipo de cartão.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static decimal GetValorJuros(uint idLoja, decimal valor, uint formaPagto, uint tipoCartao, uint parcelas)
        {
            return GetValorJuros(null, idLoja, valor, formaPagto, tipoCartao, parcelas);
        }

        /// <summary>
        /// Retorna o valor total que será pago de juros a um tipo de cartão.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static decimal GetValorJuros(GDASession sessao, uint idLoja, decimal valor, uint formaPagto, uint tipoCartao, uint parcelas)
        {
            decimal retorno = 0;
            decimal taxaJuros = (decimal)TaxaCartao(sessao, tipoCartao, idLoja, parcelas);

            if (!FinanceiroConfig.Cartao.CobrarJurosCartaoCliente)
                retorno = valor * (taxaJuros / 100);
            else
            {
                decimal valorParcela = Math.Round(valor * (1 / (1 + (taxaJuros / 100))), 2);
                retorno = valor - valorParcela;
            }

            return Math.Round(retorno, 2);
        }

        /// <summary>
        /// Retorna o valor das comissões dos pedidos selecionados no controle.
        /// </summary>
        /// <param name="ids">Os IDs das contas a receber ou dos pedidos.</param>
        /// <param name="tipo">O nome da model que contém os IDs.</param>
        /// <returns>O valor da comissão somado.</returns>
        public static decimal GetValorComissao(string ids, string tipo)
        {
            return GetValorComissao(null, ids, tipo);
        }

        /// <summary>
        /// Retorna o valor das comissões dos pedidos selecionados no controle.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ids">Os IDs das contas a receber ou dos pedidos.</param>
        /// <param name="tipo">O nome da model que contém os IDs.</param>
        /// <returns>O valor da comissão somado.</returns>
        public static decimal GetValorComissao(GDASession session, string ids, string tipo)
        {
            decimal valor = 0;
            Pedido[] pedidos = GetPedidosForComissao(session, ids, tipo);

            foreach (Pedido ped in pedidos)
            {
                // Soma o valor da comissão do comissionado
                ped.ComissaoFuncionario = Pedido.TipoComissao.Comissionado;
                valor += ped.ValorComissaoPagar;
            }

            // Retorna o valor
            return valor;
        }

        /// <summary>
        /// Recupera a lista de pedidos válidos para calcular a comissão.
        /// </summary>
        /// <param name="ids">Os IDs das contas a receber ou dos pedidos.</param>
        /// <param name="tipo">O nome da model que contém os IDs.</param>
        /// <returns>Uma lista de pedidos.</returns>
        public static Pedido[] GetPedidosForComissao(GDASession sessao, string ids, string tipo)
        {
            uint idComissionado = 0;
            List<uint> idsPedidos = new List<uint>();

            // Verifica o tipo da model
            switch (tipo)
            {
                // Verifica se é uma conta a receber
                case "ContasReceber":
                    // Recupera a lista de contas
                    var contas = ContasReceberDAO.Instance.GetByPks(sessao, ids);

                    // Percorre todas as contas
                    for (int i = 0; i < contas.Length; i++)
                    {
                        // Verifica se a conta é de um pedido
                        if (contas[i].IdPedido != null)
                        {
                            // Recupera o pedido
                            uint idComissionadoPed = PedidoDAO.Instance.ObtemIdComissionado(sessao, contas[i].IdPedido.Value);

                            // Se o pedido houver comissionado
                            if (idComissionadoPed > 0)
                            {
                                // Verifica se o pedido já foi calculado
                                if (idsPedidos.Contains(contas[i].IdPedido.Value))
                                    continue;

                                // Verifica se o comissionado é válido
                                if (idComissionadoPed != idComissionado && idComissionado > 0)
                                {
                                    idsPedidos.Clear();
                                    break;
                                }

                                idComissionado = idComissionadoPed;
                                idsPedidos.Add(contas[i].IdPedido.Value);
                            }
                        }
                    }
                    break;

                // Verifica se é um pedido
                case "Pedido":
                    // Recupera a lista de pedidos
                    Pedido[] pedidos = PedidoDAO.Instance.GetByString(sessao, ids);

                    // Percorre todos os pedidos
                    for (int i = 0; i < pedidos.Length; i++)
                    {
                        // Se o pedido houver comissionado
                        if (pedidos[i].IdComissionado != null)
                        {
                            // Verifica se o pedido já foi calculado
                            if (idsPedidos.Contains(pedidos[i].IdPedido))
                                continue;

                            // Verifica se o comissionado é válido
                            if (pedidos[i].IdComissionado.Value != idComissionado && idComissionado > 0)
                            {
                                idsPedidos.Clear();
                                break;
                            }

                            idComissionado = pedidos[i].IdComissionado.Value;
                            idsPedidos.Add(pedidos[i].IdPedido);
                        }
                    }
                    break;
            }

            if (idsPedidos.Count == 0)
                return new Pedido[0];

            // Cria a string de busca
            string idsPedString = "";
            foreach (uint id in idsPedidos)
                idsPedString += "," + id;

            // Retorna a lista de pedidos válidos
            return PedidoDAO.Instance.GetByString(sessao, idsPedString.Substring(1));
        }

        #endregion

        #region Recebimento

        #region Classe de retorno do recebimento

        public class DadosRecebimento
        {
            public List<Cheques> lstChequesInseridos = new List<Cheques>();
            public List<uint> idMovBanco;
            public List<uint> idCxDiario;
            public List<uint> idCxGeral;
            public List<uint> idMovBancoJuros;
            public List<uint> idParcCartao;
            public uint idCxDiarioPagarCredito;
            public uint idCxDiarioGerarCredito;
            public uint idCxGeralPagarCredito;
            public uint idCxGeralGerarCredito;
            public uint idContaParcial;
            public bool creditoDebitado; // Controla se o crédito que o cliente possui foi debitado
            public bool creditoCreditado; // Controla se o crédito que o cliente possui foi creditado
            public decimal creditoGerado;

            public Exception ex = null;
        }

        #endregion

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Realiza o recebimento de todas as formas de recebimento do sistema
        /// </summary>
        public static DadosRecebimento Receber(uint idLoja, Pedido pedido, Sinal sinal, LiberarPedido liberacao, ContasReceber conta, Acerto acerto,
            ContasReceber[] lstContasReceber, TrocaDevolucao trocaDev, string contasReceber, uint? idAcertoCheque, Obra obra, string idsPedidoLib,
            uint idCliente, uint idDevolucaoPagto, uint? idCartaoNaoIdentificado, string dataRecebido, decimal totalASerPago, decimal totalPago, decimal[] valoresReceb,
            uint[] formasPagto, uint[] contasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, uint[] tiposCartao, uint[] tiposBoleto, decimal[] txAntecip, decimal juros, bool recebParcial,
            bool gerarCredito, decimal creditoUtilizado, string numAutConstrucard, bool cxDiario, uint[] numParcCartoes, string chequesPagto,
            bool descontarComissao, TipoReceb tipoReceb)
        {
            return Receber(null, idLoja, pedido, sinal, liberacao, conta, acerto, lstContasReceber, trocaDev, contasReceber, idAcertoCheque,
                obra, idsPedidoLib, idCliente, idDevolucaoPagto, idCartaoNaoIdentificado, dataRecebido, totalASerPago, totalPago, valoresReceb, formasPagto,
                contasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao, tiposBoleto, txAntecip, juros, recebParcial, gerarCredito, creditoUtilizado,
                numAutConstrucard, cxDiario, numParcCartoes, chequesPagto, descontarComissao, tipoReceb);
        }

        /// <summary>
        /// Realiza o recebimento de todas as formas de recebimento do sistema
        /// </summary>
        public static DadosRecebimento Receber(GDASession sessao, uint idLoja, Pedido pedido, Sinal sinal, LiberarPedido liberacao, ContasReceber conta, Acerto acerto,
            ContasReceber[] lstContasReceber, TrocaDevolucao trocaDev, string contasReceber, uint? idAcertoCheque, Obra obra, string idsPedidoLib,
            uint idCliente, uint idDevolucaoPagto, uint? idCartaoNaoIdentificado, string dataRecebido, decimal totalASerPago, decimal totalPago, decimal[] valoresReceb,
            uint[] formasPagto, uint[] contasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, uint[] tiposCartao, uint[] tiposBoleto,
            decimal[] txAntecip, decimal juros, bool recebParcial, bool gerarCredito, decimal creditoUtilizado, string numAutConstrucard, bool cxDiario,
            uint[] numParcCartoes, string chequesPagto, bool descontarComissao, TipoReceb tipoReceb)
        {
            DadosRecebimento retorno = new DadosRecebimento();
            retorno.idMovBanco = new List<uint>();
            retorno.idCxDiario = new List<uint>();
            retorno.idCxGeral = new List<uint>();
            retorno.idMovBancoJuros = new List<uint>();
            retorno.idParcCartao = new List<uint>();

            uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

            if (String.IsNullOrEmpty(dataRecebido))
                dataRecebido = DateTime.Now.ToString("dd/MM/yyyy 23:00:00");

            // Conta a qtd de formas de pagamento utilizadas
            int numPagtos = 0;
            for (int i = 0; i < valoresReceb.Length; i++)
                if (valoresReceb[i] > 0)
                    numPagtos++;

            try
            {
                // Verifica se será gerada movimentação de juros e se a mesma está associada
                if (juros > 0 && FinanceiroConfig.PlanoContaJurosReceb == 0)
                    throw new Exception("Associe o planos de conta referente aos juros de recebimento.");

                // Verifica se o valor recebido é suficiente para pagar os juros
                if (totalPago < juros && !recebParcial)
                    throw new Exception("O valor recebido é menor que o valor dos juros. Valor recebido: " + totalPago.ToString("C") + " Juros: " + juros.ToString("C"));

                if (!gerarCredito && Math.Round(totalASerPago, 2) + Math.Round(juros, 2) < Math.Round(totalPago, 2))
                    throw new Exception("O total pago é maior que o total a ser pago.");

                if (formasPagto.Where(f => f == (uint)Pagto.FormaPagto.CartaoNaoIdentificado).Count() > 0 && cartaoNaoIdentificado.Where(f => f > 0).Count() == 0)
                    throw new Exception("Foi selecionado a forma de pagto Cartão Não Identificado, porém nenhum foi informado.");

                //Verifica se a loja da conta bancaria tem de ser a mesma loja do cliente
                var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(sessao, idCliente);
                if (FinanceiroConfig.ReceberApenasComLojaContaBancoIgualLojaCliente && idLojaCliente > 0)
                {
                    for (int i = 0; i < contasBanco.Length; i++)
                    {
                        if (formasPagto[i] != (int)Pagto.FormaPagto.Boleto && formasPagto[i] != (int)Pagto.FormaPagto.Deposito
                            && formasPagto[i] != (int)Pagto.FormaPagto.Cartao && formasPagto[i] != (int)Pagto.FormaPagto.Construcard)
                            continue;

                        var idLojaContaBanco = ContaBancoDAO.Instance.ObtemIdLoja(sessao, contasBanco[i]);

                        if (idLojaCliente != idLojaContaBanco)
                            throw new Exception("A loja da conta bancária não é a mesma loja do cliente");
                    }
                }

                // O juros não deve ser nem dinheiro e nem em cheque, uma vez que caso o mesmo seja gerado por boleto o valor ficará incorreto
                int formaSaidaJuros = 0; /*formasPagto[0] == (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro ? 1 :
                     formasPagto[0] == (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || formasPagto[0] == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro ? 2 : 0;*/

                // Se o funcionário for Caixa Diário, ou tiver permissão de caixa diário e tiver pagando conta através deste menu
                var isCaixaDiario = Geral.ControleCaixaDiario && Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && cxDiario;

                // Se o funcionário for Financeiro
                var isCaixaGeral = !cxDiario && (Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) ||
                    ((tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado) &&
                    Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento)));

                // Valida o tipo de funcionário
                if (!isCaixaDiario && !isCaixaGeral)
                    throw new Exception("Você não tem permissão para receber contas.");

                #region Cheques

                // Se a forma de pagamento for cheques de terceiros, associa os mesmos ao pagamento 
                // e muda seus status para compensado, pode haver também cheques próprios
                if ((ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, formasPagto) ||
                    ContemFormaPagto(Pagto.FormaPagto.ChequeTerceiro, formasPagto)) &&
                    tipoReceb != TipoReceb.ChequeReapresentado && tipoReceb != TipoReceb.LiberacaoAPrazoCheque &&
                    tipoReceb != TipoReceb.ChequeProprioDevolvido)
                {
                    // Separa os cheques guardando-os em um vetor
                    string[] vetCheque = chequesPagto.TrimEnd(' ').TrimEnd('|').Split('|');

                    if (tipoReceb != TipoReceb.DevolucaoPagto)
                    {
                        if (string.IsNullOrEmpty(chequesPagto))
                        {
                            retorno.ex = new Glass.Data.Exceptions.LogoutException("Informe os dados do(s) cheque(s) utilizado(s) no pagamento.");
                            return retorno;
                        }

                        try
                        {
                            foreach (string c in vetCheque)
                            {
                                // Carrega os cheques na model
                                Cheques cheque = ChequesDAO.Instance.GetFromString(sessao, c);

                                if (cxDiario)
                                    cheque.MovCaixaDiario = true;

                                if (sinal != null)
                                {
                                    cheque.IdSinal = sinal.IdSinal;
                                    cheque.IdCliente = sinal.IdCliente;
                                }

                                if (liberacao != null)
                                {
                                    cheque.IdLiberarPedido = liberacao.IdLiberarPedido;
                                    cheque.IdCliente = liberacao.IdCliente;
                                }

                                if (acerto != null)
                                    cheque.IdAcerto = acerto.IdAcerto;

                                if (idAcertoCheque > 0)
                                    cheque.IdAcertoCheque = idAcertoCheque;

                                cheque.Situacao = (int)Cheques.SituacaoCheque.EmAberto;
                                cheque.Tipo = 2;

                                // Verifica se foram inseridos cheques com os mesmos dados
                                foreach (Cheques cIns in retorno.lstChequesInseridos)
                                {
                                    if (cheque.Banco == cIns.Banco && cheque.Agencia == cIns.Agencia && cheque.Conta == cIns.Conta && cheque.Num == cIns.Num)
                                        throw new Exception("Existe um ou mais cheques cadastrados com os mesmos dados.");

                                    if (FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador && idCliente > 0 && !string.IsNullOrEmpty(cIns.DigitoNum) &&
                                        cheque.Num == cIns.Num && cheque.DigitoNum == cIns.DigitoNum)
                                        throw new Exception("Existe um ou mais cheques cadastrados com os mesmos dados.");
                                }

                                // Adiciona este cheque à lista de cheques inseridos
                                retorno.lstChequesInseridos.Add(cheque);
                            }

                            #region Valida o valor de cheques com o valor dos cheques inseridos

                            decimal valorTotalCheques = 0;

                            for (var i = 0; i < formasPagto.Count(); i++)
                                if (formasPagto[i] == (uint)Pagto.FormaPagto.ChequeProprio || formasPagto[i] == (uint)Pagto.FormaPagto.ChequeTerceiro)
                                    valorTotalCheques += valoresReceb[i];

                            /* Chamado 63372. */
                            if (valorTotalCheques != retorno.lstChequesInseridos.Sum(f => f.Valor))
                                throw new Exception("O valor total de cheques é diferente da somatória do valor dos cheques inseridos. Informe os dados do recebimento novamente.");

                            #endregion

                            // Define os parâmetros que serão desconsiderados
                            string idsPedidos =
                                tipoReceb == TipoReceb.LiberacaoAPrazoCheque ||
                                tipoReceb == TipoReceb.LiberacaoAVista ?
                                    ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacaoString(sessao, liberacao.IdLiberarPedido) :
                                tipoReceb == TipoReceb.SinalLiberacao ?
                                    (!string.IsNullOrEmpty(idsPedidoLib) ? idsPedidoLib : ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacaoString(sessao, liberacao.IdLiberarPedido)) :
                                tipoReceb == TipoReceb.PedidoAVista ||
                                tipoReceb == TipoReceb.SinalPedido ? pedido != null ? pedido.IdPedido.ToString() : null : null;

                            string idsContasR = !string.IsNullOrEmpty(contasReceber) ? contasReceber :
                                tipoReceb == TipoReceb.Acerto ? AcertoDAO.Instance.GetIdsContasR(sessao, acerto.IdAcerto) :
                                tipoReceb == TipoReceb.ContaReceber ? conta.IdContaR.ToString() : null;

                            string idsChequesR = tipoReceb == TipoReceb.ChequeDevolvido ||
                                tipoReceb == TipoReceb.ChequeProprioDevolvido ||
                                tipoReceb == TipoReceb.ChequeProprioReapresentado ||
                                tipoReceb == TipoReceb.ChequeReapresentado ? AcertoChequeDAO.Instance.GetIdsChequesByAcertoCheque(sessao, idAcertoCheque.Value) : null;

                            if ((tipoReceb == TipoReceb.LiberacaoAPrazoCheque || tipoReceb == TipoReceb.LiberacaoAVista ||
                                tipoReceb == TipoReceb.SinalLiberacao || tipoReceb == TipoReceb.SinalPedido) && string.IsNullOrEmpty(idsPedidos))
                                idsPedidos = idsPedidoLib;

                            // Verifica se o cliente pode inserir os cheques
                            // Se não puder o método dispara uma exceção com a mensagem de erro
                            // Se for recebimento de cheque devolvido com outro cheque, não valida o limite, a menos que esteja gerando crédito
                            // Não deve validar ao efetuar acerto também uma vez que as contas que estão sendo debitadas estão sendo "trocadas" pelos cheques
                            if (tipoReceb != TipoReceb.ChequeProprioDevolvido && !gerarCredito)
                                ClienteDAO.Instance.ValidaInserirCheque(sessao, idCliente, retorno.lstChequesInseridos, idsPedidos, idsContasR, idsChequesR,
                                    (tipoReceb != TipoReceb.ChequeDevolvido && tipoReceb != TipoReceb.Acerto) || gerarCredito);

                            // Valida o limite do cheque por CPF/CNPJ, independente de estar gerando crédito ou não
                            // Só não valida se for devolução de cheque próprio
                            if (tipoReceb != TipoReceb.ChequeProprioDevolvido)
                                ChequesDAO.Instance.ValidaValorLimiteCheque(sessao, retorno.lstChequesInseridos);

                            for (int i = 0; i < retorno.lstChequesInseridos.Count; i++)
                            {
                                if (retorno.lstChequesInseridos[i].IdCliente.GetValueOrDefault() == 0 && idCliente > 0)
                                    retorno.lstChequesInseridos[i].IdCliente = idCliente;

                                retorno.lstChequesInseridos[i].IdCheque = ChequesDAO.Instance.InsertBase(sessao, retorno.lstChequesInseridos[i], false);

                                // Se for acerto e se as contas a receber do mesmo forem do mesmo pedido, atualiza o campo idPedido no cheque
                                if (tipoReceb == TipoReceb.Acerto && !PedidoConfig.LiberarPedido && ContasReceberDAO.Instance.ContasRecMesmoPedido(sessao, contasReceber) && lstContasReceber[0].IdPedido > 0)
                                    ContasReceberDAO.Instance.AtualizaChequeIdPedido(sessao, lstContasReceber[0].IdPedido.Value, retorno.lstChequesInseridos[i].IdCheque);
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.ex = new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar cheques ao pagamento.", ex));
                            return retorno;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(chequesPagto))
                            {
                                ChequesDAO.Instance.CancelaForDevolucaoPagto(sessao, idDevolucaoPagto, chequesPagto);
                                retorno.lstChequesInseridos.AddRange(ChequesDAO.Instance.GetByPks(sessao, chequesPagto));
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.ex = new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar cheques para devolução.", ex));
                            return retorno;
                        }
                    }
                }
                else if (tipoReceb == TipoReceb.ChequeProprioDevolvido && !string.IsNullOrEmpty(chequesPagto))
                {
                    try
                    {
                        // Para cada cheque próprio informado, cadastra o mesmo, guardando os ids que cada um retorna
                        foreach (var c in chequesPagto.TrimEnd(' ').TrimEnd('|').Split('|'))
                        {
                            // Divide o cheque para pegar suas propriedades
                            string[] dadosCheque = c.Split('\t');

                            // Cheque próprio empresa.
                            if (dadosCheque[0] == "proprio") // Se for cheque próprio
                            {
                                // Insere cheque no BD
                                var cheque = ChequesDAO.Instance.GetFromString(sessao, c);

                                if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado && string.IsNullOrEmpty(dataRecebido))
                                    cheque.DataReceb = DateTime.Parse(dataRecebido);

                                cheque.IdAcertoCheque = idAcertoCheque;
                                cheque.Obs += string.Format("Utilizado no acerto de cheques {0}", idAcertoCheque);
                                cheque.IdCheque = ChequesDAO.Instance.InsertBase(sessao, cheque, false);

                                if (cheque.IdCheque < 1)
                                    throw new Exception("Falha ao inserir cheque próprio.");

                                // Gera movimentação no caixa geral de cada cheque, mas sem alterar o saldo, 
                                // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral
                                CaixaGeralDAO.Instance.MovCxPagto(sessao, 0, (int?)idAcertoCheque, null, null,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoChequeProprio), 2,
                                    cheque.Valor, 0, null, cheque.Obs, 0, false, null);

                                if (cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                                    ContaBancoDAO.Instance.MovContaChequePagto(sessao, (int?)idAcertoCheque, cheque.IdContaBanco.Value, UtilsPlanoConta.GetPlanoContaPagto(2), (int)UserInfo.GetUserInfo.IdLoja,
                                        cheque.IdCheque, null, 0, 2, cheque.Valor, 0, dataRecebido != null ? DateTime.Parse(dataRecebido) : DateTime.Now);
                            }
                            // Cheque de terceiros.
                            else
                            {
                                var cheque = ChequesDAO.Instance.GetFromString(sessao, c, true);
                                cheque = ChequesDAO.Instance.GetElement(sessao, cheque.IdCheque);
                                cheque.IdAcertoCheque = idAcertoCheque;
                                cheque.Situacao =
                                    cheque.Situacao == (int)Cheques.SituacaoCheque.Devolvido ? (int)Cheques.SituacaoCheque.Quitado : (int)Cheques.SituacaoCheque.Compensado;

                                ChequesDAO.Instance.Update(sessao, cheque);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cadastrar/atualizar o(s) cheque(s).", ex));
                    }
                }

                #endregion

                #region Gera contas a receber se a forma de pagamento for cartão de crédito

                bool[] parcelaCartaoGerada = new bool[valoresReceb.Length];
                Array.ForEach(parcelaCartaoGerada, x => x = false);

                if (FinanceiroConfig.Cartao.PedidoJurosCartao && ((FinanceiroConfig.Cartao.QuitarParcCartaoDebito
                    && IsFormaPagtoCartaoDebito(formasPagto, tiposCartao)) ||
                    IsFormaPagtoCartaoCredito(formasPagto, tiposCartao)))
                {
                    // Exclui todas as contas a receber do pedido antes de gerar as que serão geradas abaixo
                    if (tipoReceb == TipoReceb.PedidoAVista)
                        ContasReceberDAO.Instance.DeleteByPedido(sessao, pedido.IdPedido);

                    ContasReceber novaConta = new ContasReceber();
                    novaConta.IdLoja = UserInfo.GetUserInfo.IdLoja;

                    if (tipoReceb == TipoReceb.PedidoAVista)
                    {
                        novaConta.IdLoja = pedido.IdLoja;
                        novaConta.IdPedido = pedido.IdPedido;
                        novaConta.IdCliente = pedido.IdCli;
                        novaConta.IdFormaPagto = pedido.IdFormaPagto;
                    }
                    else if (tipoReceb == TipoReceb.SinalPedido)
                    {
                        novaConta.IdSinal = sinal.IdSinal;
                        novaConta.IdCliente = idCliente;
                    }
                    else if (tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao)
                    {
                        novaConta.IdLiberarPedido = liberacao.IdLiberarPedido;
                        novaConta.IdCliente = liberacao.IdCliente;
                    }
                    else if (tipoReceb == TipoReceb.ContaReceber)
                    {
                        novaConta.IdLoja = conta.IdLoja;
                        novaConta.IdFormaPagto = conta.IdFormaPagto;
                        novaConta.IdCliente = conta.IdCliente;
                        novaConta.IdPedido = conta.IdPedido;
                        novaConta.IdLiberarPedido = conta.IdLiberarPedido;
                        novaConta.IdAcerto = conta.IdAcerto;
                        novaConta.IdAcertoParcial = conta.IdAcertoParcial;
                        novaConta.IdTrocaDevolucao = conta.IdTrocaDevolucao;
                        novaConta.IdDevolucaoPagto = conta.IdDevolucaoPagto;
                        novaConta.IdObra = conta.IdObra;
                        novaConta.Obs = conta.Obs;
                    }
                    else if (tipoReceb == TipoReceb.Acerto)
                    {
                        novaConta.IdAcerto = acerto.IdAcerto;
                        novaConta.IdCliente = acerto.IdCli;
                    }
                    else if (tipoReceb == TipoReceb.ChequeDevolvido)
                    {
                        //novaConta.IdCheque = idCheque;
                        novaConta.IdCliente = idCliente;
                        novaConta.IdAcertoCheque = idAcertoCheque;
                    }
                    else if (tipoReceb == TipoReceb.Obra)
                    {
                        novaConta.IdObra = obra.IdObra;
                        /* Chamado 63211. */
                        novaConta.IdLoja = obra.IdLoja > 0 ? obra.IdLoja : novaConta.IdLoja;
                        novaConta.IdCliente = obra.IdCliente;
                    }
                    else if (tipoReceb == TipoReceb.TrocaDevolucao)
                    {
                        novaConta.IdLoja = Convert.ToUInt32(trocaDev.IdLoja);
                        novaConta.IdTrocaDevolucao = trocaDev.IdTrocaDevolucao;
                        novaConta.IdPedido = trocaDev.IdPedido;
                        novaConta.IdCliente = trocaDev.IdCliente;
                    }
                    else if (tipoReceb == TipoReceb.DevolucaoPagto)
                    {
                        novaConta.IdDevolucaoPagto = idDevolucaoPagto;
                        novaConta.IdCliente = idCliente;
                    }
                    else if (tipoReceb == TipoReceb.CartaoNaoIdentificado)
                    {
                        novaConta.IdLoja = idLoja;
                        novaConta.IdCartaoNaoIdentificado = (int)idCartaoNaoIdentificado;
                    }
                   
                    novaConta.IsParcelaCartao = true;
                    novaConta.IdContaRCartao = conta != null && conta.IdContaR > 0 ? conta.IdContaR : (uint?)null;

                    // Gera as contas a receber do cartão de crédito das formas de pagamento
                    for (int i = 0; i < valoresReceb.Length; i++)
                    {
                        if (valoresReceb[i] > 0 && ((FinanceiroConfig.Cartao.QuitarParcCartaoDebito && IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i])) ||
                            IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i])))
                        {
                            decimal valorMovReal = valoresReceb[i];
                            decimal valorAplicado = 0;

                            var dataConsiderar = dataRecebido.StrParaDate().Value;

                            for (int j = 0; j < numParcCartoes[i]; j++)
                            {
                                novaConta.DataVec =
                                    IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i]) ?
                                        dataConsiderar.AddDays((j + 1) * FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito) :
                                        dataConsiderar.AddDays(FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito);

                                while (!novaConta.DataVec.DiaUtil())
                                    novaConta.DataVec = novaConta.DataVec.AddDays(1);

                                /* Chamado 49396 */
                                /* Chamado 63211. */
                                novaConta.ValorVec = Math.Round(GetValorParcela(sessao, novaConta.IdLoja, valorMovReal, formasPagto[i], tiposCartao[i], numParcCartoes[i]), 2);
                                novaConta.ValorJurosCartao = GetValorJurosParc(sessao, novaConta.IdLoja, valorMovReal, formasPagto[i], tiposCartao[i], numParcCartoes[i], j);

                                /* Chamado 53946.
                                 * Soma o valor de juros somente se ele for cobrado do cliente, caso contrário, ele não influencia no valor da conta. */
                                valorAplicado += novaConta.ValorVec + (FinanceiroConfig.Cartao.CobrarJurosCartaoCliente ? 0 : novaConta.ValorJurosCartao);

                                // Caso exista diferença entre o valor aplicado e o valor real das parcelas, ajusta a diferença na última parcela de cartão gerada.
                                if (Math.Abs(valorAplicado - valorMovReal) > 0 && Math.Abs(valorAplicado - valorMovReal) <= (decimal)0.5)
                                    novaConta.ValorVec += (valorAplicado > valorMovReal ? -Math.Abs(valorAplicado - valorMovReal) : Math.Abs(valorAplicado - valorMovReal));

                                novaConta.IdConta =
                                    tipoReceb == TipoReceb.SinalLiberacao || tipoReceb == TipoReceb.SinalPedido ?
                                        UtilsPlanoConta.GetPlanoRecebTipoCartaoEntrada(tiposCartao[i]) :
                                    tipoReceb == TipoReceb.ChequeDevolvido ?
                                        UtilsPlanoConta.GetPlanoChequeDevTipoCartao(tiposCartao[i]) :
                                    UtilsPlanoConta.GetPlanoRecebTipoCartao(tiposCartao[i]);
                                novaConta.NumParc = j + 1;
                                novaConta.NumParcMax = (int)numParcCartoes[i];
                                novaConta.IdContaBanco = contasBanco[i] > 0 ? (uint?)contasBanco[i] : null;
                                novaConta.TipoRecebimentoParcCartao = (int)tiposCartao[i];

                                uint idParcelaCartao = ContasReceberDAO.Instance.Insert(sessao, novaConta);

                                if (idParcelaCartao == 0)
                                    throw new Exception("Conta a Receber não foi inserida.");
                                else
                                    retorno.idParcCartao.Add(idParcelaCartao);

                                parcelaCartaoGerada[i] = true;
                            }
                        }
                    }
                }

                #endregion

                // Se for quitamento de conta a receber já antecipada, não gera valores no caixa
                if (tipoReceb == TipoReceb.ContaReceber && conta.IdAntecipContaRec > 0)
                    return retorno;

                // Define se irá lançar movimentação na conta bancária
                bool lancaMovContaBanco = false;

                decimal jurosRateado = numPagtos == 0 ? 0 : juros / numPagtos;
                var recebApenasCxGeral =
                    tipoReceb == TipoReceb.ChequeProprioDevolvido ||
                    tipoReceb == TipoReceb.CreditoValeFuncionario || tipoReceb == TipoReceb.DebitoValeFuncionario;

                // Se o funcionário for Caixa Diário, ou tiver permissão de caixa diário e tiver pagando conta através deste menu
                if (isCaixaDiario && !recebApenasCxGeral)
                {
                    if (!CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                        throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                    #region Recebimentos

                    for (int i = 0; i < valoresReceb.Length; i++)
                    {
                        if (valoresReceb[i] <= 0)
                            continue;

                        decimal valorMovReal = valoresReceb[i];

                        // Verifica se a forma de pagamento atual é construcard e se a taxa de juros foi configurada
                        bool isConstrucardComJuros = FinanceiroConfig.Cartao.TaxaJurosConstrucard > 0 && formasPagto[i] == (uint)Pagto.FormaPagto.Construcard;
                        decimal valorJurosConstrucard = !isConstrucardComJuros ? 0 :
                            Math.Round(valorMovReal * (decimal)(FinanceiroConfig.Cartao.TaxaJurosConstrucard / 100), 2);

                        // Define se irá lançar movimentação na conta bancária
                        lancaMovContaBanco = formasPagto[i] == (uint)Pagto.FormaPagto.Deposito ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Cartao ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Construcard;

                        //Se for depósito não identificado, vincula o mesmo ao qu esta sendo recebeido e altera a situação do mesmo
                        //para em uso. 
                        string obs = string.Empty;
                        if (formasPagto[i] == (uint)Pagto.FormaPagto.DepositoNaoIdentificado)
                        {
                            DepositoNaoIdentificadoDAO.Instance.VinculaDepositoNaoIdentificado(sessao, depositoNaoIdentificado[i], pedido, liberacao,
                                acerto, conta, obra, sinal, trocaDev, idDevolucaoPagto > 0 ? idDevolucaoPagto : (uint?)null, idAcertoCheque > 0 ? idAcertoCheque : (uint?)null);

                            //se for devolução de pagamento cancela
                            if (tipoReceb == TipoReceb.DevolucaoPagto)
                                DepositoNaoIdentificadoDAO.Instance.Cancelar(sessao, depositoNaoIdentificado[i], "Devolução de Pagto.:" + idDevolucaoPagto);

                            formasPagto[i] = (uint)Pagto.FormaPagto.Deposito;
                            obs = "Depósito não identificado: " + depositoNaoIdentificado[i];
                        }

                        if (formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                        {
                            foreach (var c in cartaoNaoIdentificado.Where(f => f > 0))
                            {
                                CartaoNaoIdentificadoDAO.Instance.VincularCartaoNaoIdentificado(sessao, c, pedido, liberacao,
                                acerto, conta, obra, sinal, trocaDev, idDevolucaoPagto > 0 ? idDevolucaoPagto : (uint?)null, idAcertoCheque > 0 ? idAcertoCheque : null);

                                //se for devolução de pagamento cancela
                                if (tipoReceb == TipoReceb.DevolucaoPagto)
                                    CartaoNaoIdentificadoDAO.Instance.Cancelar(sessao, cartaoNaoIdentificado, "Devolução de Pagto.:" + idDevolucaoPagto);
                            }
                        }

                        var idsContaValor = new Dictionary<int, Tuple<uint, decimal>>();
                        uint idContaAssociar = 0;

                        // Pega o idConta para esta movimentação
                        if (formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                        {
                            var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                            foreach (var item in CNIs)
                            {
                                idContaAssociar = tipoReceb == TipoReceb.PedidoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAPrazoCheque ||
                                    tipoReceb == TipoReceb.Obra ||
                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        UtilsPlanoConta.GetPlanoVistaTipoCartao((uint)item.TipoCartao) :
                                    tipoReceb == TipoReceb.SinalPedido ||
                                    tipoReceb == TipoReceb.SinalLiberacao ?
                                        UtilsPlanoConta.GetPlanoSinalTipoCartao((uint)item.TipoCartao) :
                                    tipoReceb == TipoReceb.ContaReceber ||
                                    tipoReceb == TipoReceb.Acerto ?
                                        UtilsPlanoConta.GetPlanoRecebTipoCartao((uint)item.TipoCartao) :
                                    tipoReceb == TipoReceb.ChequeDevolvido ||
                                    tipoReceb == TipoReceb.ChequeProprioDevolvido ?
                                        UtilsPlanoConta.GetPlanoChequeDevTipoCartao((uint)item.TipoCartao) :
                                    tipoReceb == TipoReceb.DevolucaoPagto ?
                                        UtilsPlanoConta.GetPlanoContaDevolucaoPagto((uint)Pagto.FormaPagto.CartaoNaoIdentificado, (uint)item.TipoCartao, 0) :
                                    /* Chamado 48332. */
                                    tipoReceb == TipoReceb.CartaoNaoIdentificado ?
                                        UtilsPlanoConta.GetPlanoRecebTipoCartao((uint)item.TipoCartao) : 0;

                                if (idContaAssociar == 0)
                                    throw new Exception("Recebimento não permitido no caixa diário.");

                                idsContaValor.Add(item.IdCartaoNaoIdentificado, Tuple.Create(idContaAssociar, item.Valor));
                            }
                        }
                        else
                        {
                            idContaAssociar =
                                    tipoReceb == TipoReceb.PedidoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAPrazoCheque ||
                                    tipoReceb == TipoReceb.Obra ||
                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                        UtilsPlanoConta.GetPlanoVista(formasPagto[i]) :
                                        UtilsPlanoConta.GetPlanoVistaTipoCartao(tiposCartao[i])) :
                                    tipoReceb == TipoReceb.SinalPedido ||
                                    tipoReceb == TipoReceb.SinalLiberacao ?
                                        (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                        UtilsPlanoConta.GetPlanoSinal(formasPagto[i]) :
                                        UtilsPlanoConta.GetPlanoSinalTipoCartao(tiposCartao[i])) :
                                    tipoReceb == TipoReceb.ContaReceber || tipoReceb == TipoReceb.Acerto ?
                                        (formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ?
                                        UtilsPlanoConta.GetPlanoRecebTipoBoleto(tiposBoleto[i]) :
                                        formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                        UtilsPlanoConta.GetPlanoReceb(formasPagto[i]) :
                                        UtilsPlanoConta.GetPlanoRecebTipoCartao(tiposCartao[i])) :
                                    tipoReceb == TipoReceb.ChequeDevolvido ||
                                    tipoReceb == TipoReceb.ChequeProprioDevolvido ?
                                        (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                        UtilsPlanoConta.GetPlanoContaRecebChequeDev(formasPagto[i]) :
                                        UtilsPlanoConta.GetPlanoChequeDevTipoCartao(tiposCartao[i])) :
                                    tipoReceb == TipoReceb.DevolucaoPagto ?
                                        UtilsPlanoConta.GetPlanoContaDevolucaoPagto(formasPagto[i], tiposCartao[i], tiposBoleto[i]) :
                                    /* Chamado 48332. */
                                    tipoReceb == TipoReceb.CartaoNaoIdentificado ?
                                        UtilsPlanoConta.GetPlanoRecebTipoCartao(tiposCartao[i]) : 0;

                            if (idContaAssociar == 0)
                                throw new Exception("Recebimento não permitido no caixa diário.");

                            idsContaValor.Add(0, Tuple.Create(idContaAssociar, valorMovReal));
                        }

                        var naoGerarMovCaixaCNI = formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado && !FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario;

                        // Registra recebimento no caixa diário apenas se não for cartão e o flag estiver marcado
                        if ((!parcelaCartaoGerada[i] || FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario) && !naoGerarMovCaixaCNI)
                        {
                            foreach (var item in idsContaValor)
                            {
                                decimal valorMov = item.Value.Item2 - (!isConstrucardComJuros ? 0 :
                                    Math.Round(item.Value.Item2 * (decimal)(FinanceiroConfig.Cartao.TaxaJurosConstrucard / 100), 2));

                                // Não altera saldo se for Permuta (Na Transferência para o Caixa Geral, não será acrescido o valor de permuta/boleto/depósito/construcard/cartão)
                                bool mudarSaldoCxDiario = MudarSaldo(item.Value.Item1, false);

                                var idCxDiario = tipoReceb == TipoReceb.PedidoAVista ?
                                        CaixaDiarioDAO.Instance.MovCxPedido(sessao, UserInfo.GetUserInfo.IdLoja, pedido.IdCli, pedido.IdPedido, 1, valorMov, 0, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.SinalPedido ?
                                        CaixaDiarioDAO.Instance.MovCxSinal(sessao, UserInfo.GetUserInfo.IdLoja, idCliente, sinal.IdSinal, 1, valorMov, 0, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                        CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, UserInfo.GetUserInfo.IdLoja, liberacao.IdCliente, liberacao.IdLiberarPedido, 1, valorMov, 0, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.ContaReceber ?
                                        CaixaDiarioDAO.Instance.MovCxContaRec(sessao, UserInfo.GetUserInfo.IdLoja, conta.IdCliente, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, 1, valorMov, juros, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, 0, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.Acerto ?
                                        CaixaDiarioDAO.Instance.MovCxAcerto(sessao, UserInfo.GetUserInfo.IdLoja, acerto.IdCli, acerto.IdAcerto, 1, valorMov, juros, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, 0, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.Obra ?
                                        CaixaDiarioDAO.Instance.MovCxObra(sessao, UserInfo.GetUserInfo.IdLoja, obra.IdCliente, obra.IdObra, 1, valorMov, juros, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        CaixaDiarioDAO.Instance.MovCxTrocaDev(sessao, UserInfo.GetUserInfo.IdLoja, trocaDev.IdCliente, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, 1, valorMov, juros, item.Value.Item1,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, obs, mudarSaldoCxDiario) :

                                    tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                        CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, UserInfo.GetUserInfo.IdLoja, null, null, idAcertoCheque.Value, idCliente, null,
                                        item.Value.Item1, 1, valorMov, juros, null, mudarSaldoCxDiario, obs) :

                                    tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                        CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, UserInfo.GetUserInfo.IdLoja, null, null, idAcertoCheque.Value, idCliente, null,
                                        item.Value.Item1, 2, valorMov, juros, null, mudarSaldoCxDiario, obs) :

                                    tipoReceb == TipoReceb.DevolucaoPagto ?
                                        CaixaDiarioDAO.Instance.MovCxDevolucaoPagto(sessao, UserInfo.GetUserInfo.IdLoja, idDevolucaoPagto, idCliente, item.Value.Item1, 2, valorMov, jurosRateado, null, mudarSaldoCxDiario, obs) :
                                        0;

                                if (formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                                    CaixaDiarioDAO.Instance.AssociarCaixaDiarioIdCartaoNaoIdentificado(sessao, idCxDiario, cartaoNaoIdentificado[(idsContaValor.ToList()).IndexOf(item)]);

                                retorno.idCxDiario.Add(idCxDiario);
                            }
                        }

                        try
                        {
                            // Verifica se a forma de pagto foi depósito, boleto ou cartão, se tiver sido, lança movimentação
                            if (lancaMovContaBanco || isConstrucardComJuros)
                            {
                                if (!parcelaCartaoGerada[i])
                                {
                                    foreach (var item in idsContaValor)
                                    {
                                        decimal valorMov = formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ?
                                            item.Value.Item2 - (txAntecip != null ? txAntecip[i] : 0) : valoresReceb[i];
                                        decimal valorJuros = 0;

                                        if (IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i]) ||
                                            IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]))
                                            valorJuros = GetValorJuros(sessao, idLoja, valoresReceb[i], formasPagto[i], tiposCartao[i], numParcCartoes[i]);

                                        var dataMovimentacaoBancaria = DateTime.Parse(dataRecebido);

                                        if (!isConstrucardComJuros)
                                        {
                                            if (IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i]))
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito);
                                            else if (IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]))
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito);

                                            while (!dataMovimentacaoBancaria.DiaUtil())
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(1);
                                        }

                                        var idMovBanco =
                                            tipoReceb == TipoReceb.PedidoAVista ?
                                                ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, pedido.IdPedido, pedido.IdCli, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.SinalPedido ?
                                                ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, sinal.IdSinal, idCliente, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, liberacao.IdLiberarPedido, liberacao.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.ContaReceber ?
                                                ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, conta.IdPedido, conta.IdLiberarPedido,
                                                    conta.IdContaR, conta.IdSinal, conta.IdCliente, 1, valorMov, juros, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.Acerto ?
                                                ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, acerto.IdAcerto, acerto.IdCli, 1, valorMov,
                                                    juros, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.Obra ?
                                                ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra, obra.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.TrocaDevolucao ?
                                                ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, null, idAcertoCheque.Value, idCliente, 1, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, null, idAcertoCheque.Value, idCliente, 2, valorMov, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.DevolucaoPagto ?
                                            ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, idDevolucaoPagto, idCliente, 2, valorMov, dataMovimentacaoBancaria) : 0;

                                        /* Chamado 48332.
                                         * Caso o cartão não identificado seja de débito e o sistema não esteja configurado para 
                                         * gerar parcela de cartão de débito, a movimentação na conta bancária deve ser feita. */
                                        if (idCartaoNaoIdentificado > 0 && idMovBanco == 0 && tipoReceb == TipoReceb.CartaoNaoIdentificado &&
                                            IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]) && !FinanceiroConfig.Cartao.QuitarParcCartaoDebito)
                                        {
                                            idMovBanco = ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, contasBanco[i], item.Value.Item1,
                                                (int)UserInfo.GetUserInfo.IdLoja, idCartaoNaoIdentificado.Value, 2, valorMov, dataMovimentacaoBancaria);
                                        }

                                        retorno.idMovBanco.Add(idMovBanco);

                                        if (valorJuros > 0)
                                        {
                                            idContaAssociar = FinanceiroConfig.PlanoContaJurosCartao;

                                            idMovBanco =
                                                tipoReceb == TipoReceb.PedidoAVista ?
                                                    ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, pedido.IdPedido, pedido.IdCli, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.SinalPedido ?
                                                    ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, sinal.IdSinal, idCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                    ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, liberacao.IdLiberarPedido, liberacao.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.ContaReceber ?
                                                    ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, conta.IdPedido, conta.IdLiberarPedido,
                                                        conta.IdContaR, conta.IdSinal, conta.IdCliente, 2, valorJuros, juros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.Acerto ?
                                                    ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, acerto.IdAcerto, acerto.IdCli, 2, valorJuros,
                                                        juros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.Obra ?
                                                    ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra, obra.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                                    ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, null, idAcertoCheque.Value, idCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, null, idAcertoCheque.Value, idCliente, 1, valorJuros, dataMovimentacaoBancaria) :

                                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                                    ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, idDevolucaoPagto, idCliente, 1, valorJuros, dataMovimentacaoBancaria) : 0;

                                            /* Chamado 48332.
                                             * Caso o cartão não identificado seja de débito e o sistema não esteja configurado para 
                                             * gerar parcela de cartão de débito, a movimentação na conta bancária deve ser feita. */
                                            if (idCartaoNaoIdentificado > 0 && idMovBanco == 0 && tipoReceb == TipoReceb.CartaoNaoIdentificado &&
                                                IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]) && !FinanceiroConfig.Cartao.QuitarParcCartaoDebito)
                                            {
                                                idMovBanco = ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, contasBanco[i], idContaAssociar,
                                                    (int)UserInfo.GetUserInfo.IdLoja, idCartaoNaoIdentificado.Value, 2, valorJuros, dataMovimentacaoBancaria);
                                            }

                                            retorno.idMovBanco.Add(idMovBanco);
                                        }

                                        if (isConstrucardComJuros)
                                        {
                                            idContaAssociar = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard);
                                            retorno.idMovBanco.Add(
                                                tipoReceb == TipoReceb.PedidoAVista ?
                                                    ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, pedido.IdPedido, pedido.IdCli, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.SinalPedido ?
                                                    ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, sinal.IdSinal, idCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                    ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, liberacao.IdLiberarPedido, liberacao.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.ContaReceber ?
                                                    ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, conta.IdPedido, conta.IdLiberarPedido,
                                                        conta.IdContaR, conta.IdSinal, conta.IdCliente, 2, valorJurosConstrucard, juros, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.Acerto ?
                                                    ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, acerto.IdAcerto, acerto.IdCli, 2, valorJurosConstrucard,
                                                        juros, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.Obra ?
                                                    ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra, obra.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                                    ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :
                                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                                    ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, idDevolucaoPagto, idCliente, 1, valorJurosConstrucard, DateTime.Parse(dataRecebido)) : 0);
                                        }
                                    }
                                }

                                // Atualiza acerto com a taxa de antecipação utilizada
                                if (tipoReceb == TipoReceb.Acerto && txAntecip != null && txAntecip[i] > 0)
                                    AcertoDAO.Instance.AtualizaTaxaAntecip(sessao, acerto.IdAcerto, txAntecip[i]);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErroDAO.Instance.InserirFromException("Falha ao movimentar conta bancária.", ex);

                            retorno.ex = new Exception(MensagemAlerta.FormatErrorMsg("Falha ao movimentar conta bancária.", ex));
                            return retorno;
                        }
                    }

                    #endregion

                    #region Gera juros

                    // Não lança a movimentação de juros na conta bancária
                    // Correção do problema da Funcional de valores duplicados no banco
                    lancaMovContaBanco = false;

                    // Gera movimentação de juros (CONTA A RECEBER SIMPLES E COMPOSTO)
                    if (juros > 0)
                    {
                        // Verifica se movimentação deve ser lançada na conta bancária
                        if (lancaMovContaBanco)
                        {
                            // Busca conta banco
                            uint idContaBanco = 0;
                            for (int i = 0; i < contasBanco.Length; i++)
                                if (contasBanco[i] > 0)
                                    idContaBanco = contasBanco[i];

                            // Lança movimentação na conta bancária
                            retorno.idMovBanco.Add(
                                tipoReceb == TipoReceb.ContaReceber ?
                                    ContaBancoDAO.Instance.MovContaContaR(sessao, idContaBanco, FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja,
                                    conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdSinal, conta.IdCliente, 1, juros, 0, DateTime.Parse(dataRecebido)) :
                                tipoReceb == TipoReceb.Acerto ?
                                    ContaBancoDAO.Instance.MovContaAcerto(sessao, idContaBanco,
                                    FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja, acerto.IdAcerto, acerto.IdCli, 1, juros, 0,
                                    DateTime.Parse(dataRecebido)) : 0);
                        }
                        // Se não lançar movimentação na conta bancária, lança no caixa diário
                        else
                        {
                            retorno.idCxDiario.Add(
                                tipoReceb == TipoReceb.ContaReceber ?
                                    CaixaDiarioDAO.Instance.MovCxContaRec(sessao, UserInfo.GetUserInfo.IdLoja, conta.IdCliente, conta.IdPedido,
                                    conta.IdLiberarPedido, conta.IdContaR, 1, juros, 0,
                                    FinanceiroConfig.PlanoContaJurosReceb, null, formaSaidaJuros, null, false) :
                                tipoReceb == TipoReceb.Acerto ?
                                    CaixaDiarioDAO.Instance.MovCxAcerto(sessao, UserInfo.GetUserInfo.IdLoja, acerto.IdCli, acerto.IdAcerto, 1, juros, 0,
                                    FinanceiroConfig.PlanoContaJurosReceb, null, formaSaidaJuros, null, false) : 0);
                        }
                    }

                    #endregion

                    #region Crédito

                    try
                    {
                        if (creditoUtilizado > 0 && creditoUtilizado > ClienteDAO.Instance.GetCredito(sessao, idCliente))
                            throw new Exception("O cliente não possui o crédito informado para uso neste recebimento.");

                        // Se houver algum crédito do cliente sendo utilizado, 
                        // gera movimentação de entrada no caixa diário de pagamento com crédito
                        if (creditoUtilizado > 0)
                        {
                            // Chamado 12870.
                            // O valor de juros não é informado na movimentação do caixa geral quando o recebimento é efetuado somente com o crédito do
                            // cliente. A variável "numPagtos" é igual a "0" quando o recebimento é feito somente com crédito e, neste caso, os juros
                            // devem ser salvos na movimentação gerada pelo recebimento com crédito.
                            var jurosCredito = numPagtos == 0 ? juros : 0;

                            retorno.idCxDiarioPagarCredito =
                                tipoReceb == TipoReceb.PedidoAVista ?
                                    CaixaDiarioDAO.Instance.MovCxPedido(sessao, UserInfo.GetUserInfo.IdLoja, pedido.IdCli, pedido.IdPedido, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), null, false) :
                                tipoReceb == TipoReceb.LiberacaoAVista ?
                                    CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, UserInfo.GetUserInfo.IdLoja, liberacao.IdCliente, liberacao.IdLiberarPedido, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), null, null, false) :
                                tipoReceb == TipoReceb.SinalLiberacao ?
                                    CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, UserInfo.GetUserInfo.IdLoja, liberacao.IdCliente, liberacao.IdLiberarPedido, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito), null, null, false) :
                                tipoReceb == TipoReceb.SinalPedido ?
                                    CaixaDiarioDAO.Instance.MovCxSinal(sessao, UserInfo.GetUserInfo.IdLoja, idCliente, sinal.IdSinal, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito), null, null, false) :
                                tipoReceb == TipoReceb.ContaReceber ?
                                    CaixaDiarioDAO.Instance.MovCxContaRec(sessao, UserInfo.GetUserInfo.IdLoja, conta.IdCliente, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito), null, 0, null, false) :
                                tipoReceb == TipoReceb.Acerto ?
                                    CaixaDiarioDAO.Instance.MovCxAcerto(sessao, UserInfo.GetUserInfo.IdLoja, acerto.IdCli, acerto.IdAcerto, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito), null, 0, null, false) :
                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                    CaixaDiarioDAO.Instance.MovCxTrocaDev(sessao, UserInfo.GetUserInfo.IdLoja, trocaDev.IdCliente, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), null, null, false) :
                                tipoReceb == TipoReceb.ChequeDevolvido ?
                                        CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, UserInfo.GetUserInfo.IdLoja, null, null, idAcertoCheque.Value, idCliente, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito), 1, creditoUtilizado, 0, null, false, null) :
                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                    CaixaDiarioDAO.Instance.MovCxDevolucaoPagto(sessao, UserInfo.GetUserInfo.IdLoja, idDevolucaoPagto, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DevolucaoPagtoCredito), 2, creditoUtilizado, jurosCredito, null, false, null) :
                                /* Chamado 49146. */
                                tipoReceb == TipoReceb.Obra ?
                                    CaixaDiarioDAO.Instance.MovCxObra(sessao, UserInfo.GetUserInfo.IdLoja, idCliente, obra.IdObra, 1, creditoUtilizado, jurosCredito, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecObraCredito), null, null, false) : 0;

                            // Debita Crédito do cliente
                            ClienteDAO.Instance.DebitaCredito(sessao, idCliente, creditoUtilizado);
                            retorno.creditoDebitado = true;
                        }

                        // Se o valor pago for superior ao valor da conta, 
                        // gera movimentação de saída no caixa diário de geração de crédito
                        if (gerarCredito && (totalPago - totalASerPago) > 0)
                        {
                            retorno.creditoGerado = totalPago - totalASerPago;

                            if (tipoReceb != TipoReceb.ChequeDevolvido && tipoReceb != TipoReceb.ChequeProprioDevolvido)
                                retorno.creditoGerado -= juros;

                            // 07/11/2012 A única situação que o total pago será igual ao crédito gerado é na liberação, caso o pedido tenha sido 
                            // pago antecipadamente fazendo com que o valor da liberação fique zerado ou negativo, esta condição deve ficar
                            // exatamente assim para evitar um erro que voltou a ocorrer de gerar crédito do total pago e não do 
                            // totalpago-totalaserpago, o qual não foi possível reproduzir
                            if (totalPago == retorno.creditoGerado &&
                                (totalASerPago > 0 || (tipoReceb != TipoReceb.LiberacaoAVista && tipoReceb != TipoReceb.LiberacaoAPrazoCheque)))
                            {
                                Erro erro = new Erro();
                                erro.DataErro = DateTime.Now;
                                erro.IdFuncErro = UserInfo.GetUserInfo.CodUser;
                                erro.Mensagem = totalPago + " " + juros + " " + totalASerPago;
                                erro.UrlErro = "Erro sinistro crédito";
                                ErroDAO.Instance.Insert(sessao, erro);
                                throw new Exception("Não é possível gerar crédito do total do pagamento.");
                            }

                            if (retorno.creditoGerado > 0)
                            {
                                retorno.idCxDiarioGerarCredito =
                                    tipoReceb == TipoReceb.PedidoAVista ?
                                        CaixaDiarioDAO.Instance.MovCxPedido(sessao, UserInfo.GetUserInfo.IdLoja, pedido.IdCli, pedido.IdPedido, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(tipoReceb == TipoReceb.PedidoAVista ? UtilsPlanoConta.PlanoContas.CreditoVendaGerado : UtilsPlanoConta.PlanoContas.CreditoEntradaGerado), null, false) :
                                    tipoReceb == TipoReceb.SinalPedido ?
                                        CaixaDiarioDAO.Instance.MovCxSinal(sessao, UserInfo.GetUserInfo.IdLoja, idCliente, sinal.IdSinal, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(tipoReceb == TipoReceb.PedidoAVista ? UtilsPlanoConta.PlanoContas.CreditoVendaGerado : UtilsPlanoConta.PlanoContas.CreditoEntradaGerado), null, null, false) :
                                    tipoReceb == TipoReceb.LiberacaoAVista ?
                                        CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, UserInfo.GetUserInfo.IdLoja, liberacao.IdCliente, liberacao.IdLiberarPedido, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), null, null, false) :
                                    tipoReceb == TipoReceb.ContaReceber ?
                                        CaixaDiarioDAO.Instance.MovCxContaRec(sessao, UserInfo.GetUserInfo.IdLoja, conta.IdCliente, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado), null, 0, null, false) :
                                    tipoReceb == TipoReceb.Acerto ?
                                        CaixaDiarioDAO.Instance.MovCxAcerto(sessao, UserInfo.GetUserInfo.IdLoja, acerto.IdCli, acerto.IdAcerto, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado), null, 0, null, false) :
                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        CaixaDiarioDAO.Instance.MovCxTrocaDev(sessao, UserInfo.GetUserInfo.IdLoja, trocaDev.IdCliente, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, 1, retorno.creditoGerado, 0, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), null, null, false) :
                                    tipoReceb == TipoReceb.ChequeDevolvido ?
                                        CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, UserInfo.GetUserInfo.IdLoja, null, null, idAcertoCheque.Value, idCliente, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, retorno.creditoGerado, 0, null, false, null) : 0;


                                // Credita crédito do cliente
                                ClienteDAO.Instance.CreditaCredito(sessao, idCliente, retorno.creditoGerado);
                                retorno.creditoCreditado = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(MensagemAlerta.FormatErrorMsg(ex.Message, ex));
                    }

                    #endregion
                }
                // Se o funcionário for Financeiro
                else if (isCaixaGeral)
                {
                    #region Recebimentos

                    var contadorDataUnica = 0;

                    for (int i = 0; i < valoresReceb.Length; i++)
                    {
                        if (valoresReceb[i] <= 0)
                            continue;

                        decimal valorMovReal = valoresReceb[i];

                        // Não altera saldo se for Permuta/Boleto/Depósito/Cartão/CartaoNaoIdentificado
                        bool mudarSaldoCxGeral = formasPagto[i] != (uint)Pagto.FormaPagto.Permuta &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.Boleto &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.Deposito &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.DepositoNaoIdentificado &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.Cartao &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.CartaoNaoIdentificado &&
                            formasPagto[i] != (uint)Pagto.FormaPagto.Construcard;

                        // Verifica se a forma de pagamento atual é construcard e se a taxa de juros foi configurada
                        bool isConstrucardComJuros = FinanceiroConfig.Cartao.TaxaJurosConstrucard > 0 && formasPagto[i] == (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard;
                        decimal valorJurosConstrucard = !isConstrucardComJuros ? 0 :
                            Math.Round(valorMovReal * (decimal)(FinanceiroConfig.Cartao.TaxaJurosConstrucard / 100), 2);

                        // Verifica se deve lançar movimentação na conta bancária
                        lancaMovContaBanco = tipoReceb == TipoReceb.ChequeReapresentado ||
                            (formasPagto[i] == (uint)Pagto.FormaPagto.Deposito ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Cartao ||
                            formasPagto[i] == (uint)Pagto.FormaPagto.Construcard);

                        //Se for depósito não identificado, vincula o mesmo ao que esta sendo recebeido e altera a situação do mesmo
                        //para em uso. 
                        string obs = string.Empty;
                        if (formasPagto[i] == (uint)Pagto.FormaPagto.DepositoNaoIdentificado)
                        {
                            DepositoNaoIdentificadoDAO.Instance.VinculaDepositoNaoIdentificado(sessao, depositoNaoIdentificado[i], pedido, liberacao,
                                acerto, conta, obra, sinal, trocaDev, idDevolucaoPagto > 0 ? idDevolucaoPagto : (uint?)null, idAcertoCheque > 0 ? idAcertoCheque : (uint?)null);

                            //se for devolução de pagamento cancela
                            if (tipoReceb == TipoReceb.DevolucaoPagto)
                                DepositoNaoIdentificadoDAO.Instance.Cancelar(sessao, depositoNaoIdentificado[i], "Devolução de Pagto.:" + idDevolucaoPagto);

                            formasPagto[i] = (uint)Pagto.FormaPagto.Deposito;
                            obs = "Depósito não identificado: " + depositoNaoIdentificado[i];
                        }

                        if (formasPagto.Length > i && formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                        {
                            foreach (var item in cartaoNaoIdentificado.Where(f => f > 0))
                            {
                                CartaoNaoIdentificadoDAO.Instance.VincularCartaoNaoIdentificado(sessao, item, pedido, liberacao,
                                    acerto, conta, obra, sinal, trocaDev, idDevolucaoPagto > 0 ? idDevolucaoPagto : (uint?)null, idAcertoCheque > 0 ? idAcertoCheque : null);

                                //se for devolução de pagamento cancela
                                if (tipoReceb == TipoReceb.DevolucaoPagto)
                                    CartaoNaoIdentificadoDAO.Instance.Cancelar(sessao, cartaoNaoIdentificado, "Devolução de Pagto.:" + idDevolucaoPagto);
                            }
                        }

                        //Id da conta, ValorReceb
                        var idsContaValor = new Dictionary<int, Tuple<uint, decimal>>();
                        uint idContaAssociar = 0;

                        if (formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                        {
                            var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                            foreach (var item in CNIs)
                            {
                                // Pega o idConta para esta movimentação
                                idContaAssociar =
                                    tipoReceb == TipoReceb.PedidoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAVista ||
                                    tipoReceb == TipoReceb.LiberacaoAPrazoCheque ||
                                    tipoReceb == TipoReceb.Obra ||
                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        UtilsPlanoConta.GetPlanoVistaTipoCartao((uint)item.TipoCartao) :

                                    tipoReceb == TipoReceb.SinalPedido ||
                                    tipoReceb == TipoReceb.SinalLiberacao ?
                                        UtilsPlanoConta.GetPlanoSinalTipoCartao((uint)item.TipoCartao) :

                                    tipoReceb == TipoReceb.ContaReceber ||
                                    tipoReceb == TipoReceb.Acerto ||
                                    /* Chamado 48332. */
                                    tipoReceb == TipoReceb.CartaoNaoIdentificado ?
                                        UtilsPlanoConta.GetPlanoRecebTipoCartao((uint)item.TipoCartao) :

                                    tipoReceb == TipoReceb.ChequeDevolvido ||
                                    tipoReceb == TipoReceb.ChequeReapresentado ||
                                    tipoReceb == TipoReceb.ChequeProprioDevolvido ||
                                    tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                        UtilsPlanoConta.GetPlanoChequeDevTipoCartao((uint)item.TipoCartao) :

                                    tipoReceb == TipoReceb.CreditoValeFuncionario ||
                                    tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                        UtilsPlanoConta.GetPlanoContaVendaFunc(formasPagto[i], (uint)item.TipoCartao) :

                                    tipoReceb == TipoReceb.DevolucaoPagto ?
                                        UtilsPlanoConta.GetPlanoContaDevolucaoPagto(formasPagto[i], (uint)item.TipoCartao, tiposBoleto[i]) : 0;

                                idsContaValor.Add(item.IdCartaoNaoIdentificado, Tuple.Create(idContaAssociar, item.Valor));
                            }
                        }
                        else
                        {
                            // Pega o idConta para esta movimentação
                            idContaAssociar =
                                tipoReceb == TipoReceb.PedidoAVista ||
                                tipoReceb == TipoReceb.LiberacaoAVista ||
                                tipoReceb == TipoReceb.LiberacaoAPrazoCheque ||
                                tipoReceb == TipoReceb.Obra ||
                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                    (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                    UtilsPlanoConta.GetPlanoVista(formasPagto[i]) :
                                    UtilsPlanoConta.GetPlanoVistaTipoCartao(tiposCartao[i])) :

                                tipoReceb == TipoReceb.SinalPedido ||
                                tipoReceb == TipoReceb.SinalLiberacao ?
                                    (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                    UtilsPlanoConta.GetPlanoSinal(formasPagto[i]) :
                                    UtilsPlanoConta.GetPlanoSinalTipoCartao(tiposCartao[i])) :

                                tipoReceb == TipoReceb.ContaReceber ||
                                tipoReceb == TipoReceb.Acerto ?
                                    (formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ?
                                    UtilsPlanoConta.GetPlanoRecebTipoBoleto(tiposBoleto[i]) :
                                    formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                    UtilsPlanoConta.GetPlanoReceb(formasPagto[i]) :
                                    UtilsPlanoConta.GetPlanoRecebTipoCartao(tiposCartao[i])) :

                                /* Chamado 48332. */
                                tipoReceb == TipoReceb.CartaoNaoIdentificado ?
                                    UtilsPlanoConta.GetPlanoRecebTipoCartao(tiposCartao[i]) :

                                tipoReceb == TipoReceb.ChequeDevolvido ||
                                tipoReceb == TipoReceb.ChequeReapresentado ||
                                tipoReceb == TipoReceb.ChequeProprioDevolvido ||
                                tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                    (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                    UtilsPlanoConta.GetPlanoContaRecebChequeDev(formasPagto[i]) :
                                    UtilsPlanoConta.GetPlanoChequeDevTipoCartao(tiposCartao[i])) :

                                tipoReceb == TipoReceb.CreditoValeFuncionario ||
                                tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                    (formasPagto[i] != (uint)Pagto.FormaPagto.Cartao ?
                                    UtilsPlanoConta.GetPlanoContaVendaFunc(formasPagto[i], null) :
                                    UtilsPlanoConta.GetPlanoContaVendaFunc(formasPagto[i], tiposCartao[i])) :

                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                    UtilsPlanoConta.GetPlanoContaDevolucaoPagto(formasPagto[i], tiposCartao[i], tiposBoleto[i]) : 0;

                            idsContaValor.Add(0, Tuple.Create(idContaAssociar, valorMovReal));
                        }

                        if (idsContaValor.Count == 0)
                            throw new Exception("Plano de contas não mapeado.");

                        DateTime? dataMovBanco = lancaMovContaBanco ? (DateTime?)DateTime.Parse(dataRecebido) : null;

                        var naoGerarMovCaixaCNI = formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado && !FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario;

                        // Registra recebimento no caixa geral
                        // Se a empresa trabalha com quitamento de cartao de credito posterior e esta movimentação 
                        // for cartão de crédito, não gera movimentação no caixa geral, a menos que seja WebGlass Lite ou 
                        // que esteja no config para movimentar o caixa
                        if (((tipoReceb != TipoReceb.ChequeReapresentado && !parcelaCartaoGerada[i]) ||
                            FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario) && !naoGerarMovCaixaCNI)
                        {
                            foreach (var item in idsContaValor)
                            {
                                decimal valorMov = item.Value.Item2 - (!isConstrucardComJuros ? 0 :
                                    Math.Round(valorMovReal * (decimal)(FinanceiroConfig.Cartao.TaxaJurosConstrucard / 100), 2));

                                // Registra recebimento no caixa geral
                                var idCxGeral = tipoReceb == TipoReceb.PedidoAVista ?
                                        CaixaGeralDAO.Instance.MovCxPedido(sessao, pedido.IdPedido, pedido.IdCli, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.SinalPedido ?
                                        CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, idCliente, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                        CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.ContaReceber ?
                                        CaixaGeralDAO.Instance.MovCxContaRec(sessao, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdCliente, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, 0, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.Acerto ?
                                        CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, 0, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, item.Value.Item1, 1, valorMov, jurosRateado, null, 0, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, item.Value.Item1, 2, valorMov, jurosRateado, null, 0, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.Obra ?
                                        CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        CaixaGeralDAO.Instance.MovCxTrocaDev(sessao, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, item.Value.Item1, 1, valorMov, jurosRateado, null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.CreditoValeFuncionario || tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                        CaixaGeralDAO.Instance.MovCxMovFunc(sessao, idCliente, pedido != null ? (uint?)pedido.IdPedido : null, liberacao != null ? (uint?)liberacao.IdLiberarPedido : null, item.Value.Item1,
                                        tipoReceb == TipoReceb.CreditoValeFuncionario ? 1 : 2, valorMov, jurosRateado, null, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.DevolucaoPagto ?
                                        CaixaGeralDAO.Instance.MovCxDevolucaoPagto(sessao, idDevolucaoPagto, idCliente, item.Value.Item1, 2, valorMov, jurosRateado, null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) :

                                    tipoReceb == TipoReceb.LiberacaoAPrazoCheque ?
                                        CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, item.Value.Item1, 1, valorMov, jurosRateado,
                                        formasPagto[i] == (uint)Pagto.FormaPagto.Construcard ? numAutConstrucard : null, mudarSaldoCxGeral, dataMovBanco, obs, contadorDataUnica++) : 0;

                                if (formasPagto.Length > i && formasPagto[i] == (uint)Pagto.FormaPagto.CartaoNaoIdentificado)
                                    CaixaGeralDAO.Instance.AssociarCaixaGeralIdCartaoNaoIdentificado(sessao, idCxGeral, (uint)item.Key);

                                retorno.idCxGeral.Add(idCxGeral);
                            }

                            if (retorno.idCxGeral.Count >= i + 1 && retorno.idCxGeral[i] == 0 && tipoReceb != TipoReceb.CartaoNaoIdentificado)
                                throw new Exception("Falha ao creditar valor no caixa geral.");
                        }

                        try
                        {
                            // Verifica se deve lançar movimentação na conta bancária
                            if (lancaMovContaBanco || isConstrucardComJuros)
                            {
                                if (!parcelaCartaoGerada[i])
                                {
                                    foreach (var item in idsContaValor)
                                    {
                                        decimal valorMov = formasPagto[i] == (uint)Pagto.FormaPagto.Boleto ? valoresReceb[i] - (txAntecip != null ? txAntecip[i] : 0) : item.Value.Item2;
                                        decimal valorJuros = 0;

                                        if (IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i]) ||
                                            IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]))
                                            valorJuros = GetValorJuros(idLoja, valoresReceb[i], formasPagto[i], tiposCartao[i], numParcCartoes[i]);

                                        var dataMovimentacaoBancaria = DateTime.Parse(dataRecebido);

                                        if (!isConstrucardComJuros)
                                        {
                                            if (IsFormaPagtoCartaoCredito(formasPagto[i], tiposCartao[i]))
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito);
                                            else if (IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]))
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(FinanceiroConfig.Cartao.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito);

                                            while (!dataMovimentacaoBancaria.DiaUtil())
                                                dataMovimentacaoBancaria = dataMovimentacaoBancaria.AddDays(1);
                                        }

                                        var idMovBanco = tipoReceb == TipoReceb.PedidoAVista ?
                                                ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                pedido.IdPedido, pedido.IdCli, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.SinalPedido ?
                                                ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                sinal.IdSinal, idCliente, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                liberacao.IdLiberarPedido, liberacao.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ContaReceber ?
                                                ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdSinal, conta.IdCliente, 1, valorMov, juros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.Acerto ?
                                                ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                acerto.IdAcerto, acerto.IdCli, 1, valorMov, juros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                null, idAcertoCheque.Value, idCliente, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                null, idAcertoCheque.Value, idCliente, 2, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.Obra ?
                                                ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra,
                                                obra.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.TrocaDevolucao ?
                                                ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, trocaDev.IdTrocaDevolucao,
                                                trocaDev.IdPedido, trocaDev.IdCliente, 1, valorMov, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.CreditoValeFuncionario || tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                                ContaBancoDAO.Instance.MovContaMovFunc(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja, idCliente,
                                                pedido != null ? (uint?)pedido.IdPedido : null, liberacao != null ? (uint?)liberacao.IdLiberarPedido : null,
                                                tipoReceb == TipoReceb.CreditoValeFuncionario ? 1 : 2, valorMovReal, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.DevolucaoPagto ?
                                                ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], item.Value.Item1, (int)UserInfo.GetUserInfo.IdLoja,
                                                idDevolucaoPagto, idCliente, 2, valorMov, dataMovimentacaoBancaria) : 0;

                                        /* Chamado 48332.
                                         * Caso o cartão não identificado seja de débito e o sistema não esteja configurado para 
                                         * gerar parcela de cartão de débito, a movimentação na conta bancária deve ser feita. */
                                        if (idCartaoNaoIdentificado > 0 && idMovBanco == 0 && tipoReceb == TipoReceb.CartaoNaoIdentificado &&
                                            IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]) && !FinanceiroConfig.Cartao.QuitarParcCartaoDebito)
                                        {
                                            idMovBanco = ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, contasBanco[i], item.Value.Item1,
                                                (int)UserInfo.GetUserInfo.IdLoja, idCartaoNaoIdentificado.Value, 1, valorMov, dataMovimentacaoBancaria);
                                        }

                                        retorno.idMovBanco.Add(idMovBanco);

                                        if (valorJuros > 0)
                                        {
                                            idContaAssociar = FinanceiroConfig.PlanoContaJurosCartao;

                                            idMovBanco = tipoReceb == TipoReceb.PedidoAVista ?
                                                ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                pedido.IdPedido, pedido.IdCli, 2, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.SinalPedido ?
                                                ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                sinal.IdSinal, idCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                liberacao.IdLiberarPedido, liberacao.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ContaReceber ?
                                                ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdSinal, conta.IdCliente, 2, valorJuros, juros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.Acerto ?
                                                ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, acerto.IdAcerto,
                                                acerto.IdCli, 2, valorJuros, juros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                null, idAcertoCheque.Value, idCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                null, idAcertoCheque.Value, idCliente, 1, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.Obra ?
                                                ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra,
                                                obra.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :
                                            tipoReceb == TipoReceb.TrocaDevolucao ?
                                                ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, 2, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.CreditoValeFuncionario || tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                                ContaBancoDAO.Instance.MovContaMovFunc(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                idCliente, pedido != null ? (uint?)pedido.IdPedido : null, liberacao != null ? (uint?)liberacao.IdLiberarPedido :
                                                null, tipoReceb == TipoReceb.CreditoValeFuncionario ? 2 : 1, valorJuros, dataMovimentacaoBancaria) :

                                            tipoReceb == TipoReceb.DevolucaoPagto ?
                                                ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                idDevolucaoPagto, idCliente, 1, valorJuros, dataMovimentacaoBancaria) : 0;

                                            /* Chamado 48332.
                                             * Caso o cartão não identificado seja de débito e o sistema não esteja configurado para 
                                             * gerar parcela de cartão de débito, a movimentação na conta bancária deve ser feita. */
                                            if (idCartaoNaoIdentificado > 0 && idMovBanco == 0 && tipoReceb == TipoReceb.CartaoNaoIdentificado &&
                                                IsFormaPagtoCartaoDebito(formasPagto[i], tiposCartao[i]) && !FinanceiroConfig.Cartao.QuitarParcCartaoDebito)
                                            {
                                                idMovBanco = ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, contasBanco[i], idContaAssociar,
                                                    (int)UserInfo.GetUserInfo.IdLoja, idCartaoNaoIdentificado.Value, 2, valorJuros, dataMovimentacaoBancaria);
                                            }

                                            retorno.idMovBanco.Add(idMovBanco);
                                        }

                                        if (isConstrucardComJuros)
                                        {
                                            idContaAssociar = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard);

                                            idMovBanco = tipoReceb == TipoReceb.PedidoAVista ?
                                                    ContaBancoDAO.Instance.MovContaPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    pedido.IdPedido, pedido.IdCli, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.SinalPedido ?
                                                    ContaBancoDAO.Instance.MovContaSinal(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    sinal.IdSinal, idCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.LiberacaoAVista || tipoReceb == TipoReceb.SinalLiberacao ?
                                                    ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    liberacao.IdLiberarPedido, liberacao.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.ContaReceber ?
                                                    ContaBancoDAO.Instance.MovContaContaR(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdSinal, conta.IdCliente, 2, valorJurosConstrucard, juros, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.Acerto ?
                                                    ContaBancoDAO.Instance.MovContaAcerto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    acerto.IdAcerto, acerto.IdCli, 2, valorJurosConstrucard, juros, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    null, idAcertoCheque.Value, idCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    null, idAcertoCheque.Value, idCliente, 1, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.Obra ?
                                                    ContaBancoDAO.Instance.MovContaObra(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra,
                                                    obra.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                                    ContaBancoDAO.Instance.MovContaTrocaDev(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, 2, valorJurosConstrucard, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.CreditoValeFuncionario || tipoReceb == TipoReceb.DebitoValeFuncionario ?
                                                    ContaBancoDAO.Instance.MovContaMovFunc(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja, idCliente,
                                                    pedido != null ? (uint?)pedido.IdPedido : null, liberacao != null ? (uint?)liberacao.IdLiberarPedido : null,
                                                    tipoReceb == TipoReceb.CreditoValeFuncionario ? 2 : 1, valorMovReal, DateTime.Parse(dataRecebido)) :

                                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                                    ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, contasBanco[i], idContaAssociar, (int)UserInfo.GetUserInfo.IdLoja,
                                                    idDevolucaoPagto, idCliente, 1, valorJurosConstrucard, DateTime.Parse(dataRecebido)) : 0;

                                            retorno.idMovBanco.Add(idMovBanco);
                                        }
                                    }
                                }
                                // Atualiza acerto com a taxa de antecipação utilizada
                                if (tipoReceb == TipoReceb.Acerto && txAntecip != null && txAntecip[i] > 0)
                                    AcertoDAO.Instance.AtualizaTaxaAntecip(sessao, acerto.IdAcerto, txAntecip[i]);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao movimentar conta bancária.", ex));
                        }
                    }

                    #endregion

                    #region Gera juros

                    // Não lança a movimentação de juros na conta bancária
                    // Correção do problema da Funcional de valores duplicados no banco
                    lancaMovContaBanco = false;

                    // Gera movimentação de juros (CONTA A RECEBER SIMPLES E COMPOSTO)
                    if (juros > 0)
                    {
                        // Verifica se movimentação deve ser lançada na conta bancária
                        if (lancaMovContaBanco)
                        {
                            // Busca conta banco
                            uint idContaBanco = 0;
                            for (int i = 0; i < contasBanco.Length; i++)
                                if (contasBanco[i] > 0)
                                    idContaBanco = contasBanco[i];

                            // Lança movimentação na conta bancária
                            retorno.idMovBanco.Add(
                                tipoReceb == TipoReceb.ContaReceber ?
                                    ContaBancoDAO.Instance.MovContaContaR(sessao, idContaBanco,
                                    FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja, conta.IdPedido, conta.IdLiberarPedido,
                                    conta.IdContaR, conta.IdSinal, conta.IdCliente, 1, juros, 0, DateTime.Parse(dataRecebido)) :
                                tipoReceb == TipoReceb.Acerto ?
                                    ContaBancoDAO.Instance.MovContaAcerto(sessao, idContaBanco, FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja,
                                    acerto.IdAcerto, acerto.IdCli, 1, juros, 0, DateTime.Parse(dataRecebido)) :
                                tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, idContaBanco, FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja,
                                    null, idAcertoCheque.Value, idCliente, 1, juros, DateTime.Parse(dataRecebido)) :
                                tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                    ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, idContaBanco, FinanceiroConfig.PlanoContaJurosReceb, (int)UserInfo.GetUserInfo.IdLoja,
                                    null, idAcertoCheque.Value, idCliente, 2, juros, DateTime.Parse(dataRecebido)) : 0);
                        }
                        // Se não lançar movimentação na conta bancária, lança no caixa geral
                        else
                        {
                            /*
                            bool alterarSaldoJuros = true;

                            if (ControleSistema.GetSite() == "vidrovalle" && formasPagto[0] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                                alterarSaldoJuros = false;
                            */

                            retorno.idCxGeral.Add(
                                tipoReceb == TipoReceb.ContaReceber ?
                                    CaixaGeralDAO.Instance.MovCxContaRec(sessao, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdCliente,
                                    FinanceiroConfig.PlanoContaJurosReceb, 1, juros, 0, null, formaSaidaJuros, false, DateTime.Parse(dataRecebido),
                                    null) :
                                tipoReceb == TipoReceb.Acerto ?
                                    CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli,
                                    FinanceiroConfig.PlanoContaJurosReceb, 1, juros, 0, null, formaSaidaJuros, false, DateTime.Parse(dataRecebido),
                                    null) :
                                tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                    CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente,
                                    FinanceiroConfig.PlanoContaJurosReceb, 1, juros, 0, null, formaSaidaJuros, false, DateTime.Parse(dataRecebido),
                                    null) :
                                tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                    CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente,
                                    FinanceiroConfig.PlanoContaJurosReceb, 2, juros, 0, null, formaSaidaJuros, false, DateTime.Parse(dataRecebido),
                                    null) : 0);
                        }
                    }

                    #endregion

                    #region Crédito

                    try
                    {
                        if (creditoUtilizado > 0 && creditoUtilizado > ClienteDAO.Instance.GetCredito(sessao, idCliente))
                            throw new Exception("O cliente não possui o crédito informado para uso neste recebimento.");

                        // Gera movimentação de entrada no caixa diário de pagamento com crédito, 
                        // se houver algum crédito do cliente sendo utilizado
                        if (creditoUtilizado > 0)
                        {
                            // Chamado 12870.
                            // O valor de juros não é informado na movimentação do caixa geral quando o recebimento é efetuado somente com o crédito do
                            // cliente. A variável "numPagtos" é igual a "0" quando o recebimento é feito somente com crédito e, neste caso, os juros
                            // devem ser salvos na movimentação gerada pelo recebimento com crédito.
                            var jurosCredito = numPagtos == 0 ? juros : 0;

                            retorno.idCxGeralPagarCredito =
                                tipoReceb == TipoReceb.PedidoAVista ?
                                    CaixaGeralDAO.Instance.MovCxPedido(sessao, pedido.IdPedido, pedido.IdCli, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), 1, creditoUtilizado, jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.LiberacaoAVista ?
                                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), 1, creditoUtilizado, jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.SinalLiberacao ?
                                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito), 1, creditoUtilizado, jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.SinalPedido ?
                                    CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, sinal.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito), 1, creditoUtilizado, jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.ContaReceber ?
                                    CaixaGeralDAO.Instance.MovCxContaRec(sessao, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito), 1, creditoUtilizado, jurosCredito, null, 0, false, null, null) :
                                tipoReceb == TipoReceb.Acerto ?
                                    CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito), 1, creditoUtilizado, jurosCredito, null, 0, false, null, null) :
                                tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                    CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito), 1, creditoUtilizado, jurosCredito, null, 0, false, null, null) :
                                tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                    CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito), 2, creditoUtilizado, jurosCredito, null, 0, false, null, null) :
                                tipoReceb == TipoReceb.Obra ?
                                    CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecObraCredito), 1, creditoUtilizado,
                                        jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.TrocaDevolucao ?
                                    CaixaGeralDAO.Instance.MovCxTrocaDev(sessao, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito), 1, creditoUtilizado, jurosCredito, null, false, null, null) :
                                tipoReceb == TipoReceb.DevolucaoPagto ?
                                    CaixaGeralDAO.Instance.MovCxDevolucaoPagto(sessao, idDevolucaoPagto, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DevolucaoPagtoCredito), 2, creditoUtilizado, jurosCredito, null, false, null, null) : 0;

                            // Debita Crédito do cliente
                            ClienteDAO.Instance.DebitaCredito(sessao, idCliente, creditoUtilizado);
                            retorno.creditoDebitado = true;
                        }

                        // Gera movimentação de entrada no caixa diário de geração de crédito, 
                        // se o valor pago for superior ao valor do pedido
                        if (gerarCredito && (totalPago - totalASerPago) > 0)
                        {
                            retorno.creditoGerado = Math.Round(totalPago, 2) - Math.Round(totalASerPago, 2);

                            if (tipoReceb != TipoReceb.ChequeDevolvido && tipoReceb != TipoReceb.ChequeProprioDevolvido)
                                retorno.creditoGerado -= Math.Round(juros, 2);

                            // 07/11/2012 A única situação que o total pago será igual ao crédito gerado é na liberação, caso o pedido tenha sido 
                            // pago antecipadamente fazendo com que o valor da liberação fique zerado ou negativo, esta condição deve ficar
                            // exatamente assim para evitar um erro que voltou a ocorrer de gerar crédito do total pago e não do 
                            // totalpago-totalaserpago, o qual não foi possível reproduzir
                            if (totalPago == retorno.creditoGerado &&
                                (totalASerPago > 0 || (tipoReceb != TipoReceb.LiberacaoAVista && tipoReceb != TipoReceb.LiberacaoAPrazoCheque)))
                            {
                                Erro erro = new Erro();
                                erro.DataErro = DateTime.Now;
                                erro.IdFuncErro = UserInfo.GetUserInfo.CodUser;
                                erro.Mensagem = totalPago + " " + juros + " " + totalASerPago;
                                erro.UrlErro = "Erro ao gerar crédito (gerar crédito total).";
                                ErroDAO.Instance.Insert(sessao, erro);
                                throw new Exception("Não é possível gerar crédito do total do pagamento.");
                            }

                            if (retorno.creditoGerado > 0)
                            {
                                retorno.idCxGeralGerarCredito =
                                    tipoReceb == TipoReceb.PedidoAVista ?
                                        CaixaGeralDAO.Instance.MovCxPedido(sessao, pedido.IdPedido, pedido.IdCli, UtilsPlanoConta.GetPlanoConta(tipoReceb == TipoReceb.PedidoAVista ? UtilsPlanoConta.PlanoContas.CreditoVendaGerado : UtilsPlanoConta.PlanoContas.CreditoEntradaGerado), 1, retorno.creditoGerado, 0, null, false, null, null) :
                                    tipoReceb == TipoReceb.SinalPedido ?
                                        CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, idCliente, UtilsPlanoConta.GetPlanoConta(tipoReceb == TipoReceb.PedidoAVista ? UtilsPlanoConta.PlanoContas.CreditoVendaGerado : UtilsPlanoConta.PlanoContas.CreditoEntradaGerado), 1, retorno.creditoGerado, 0, null, false, null, null) :
                                    tipoReceb == TipoReceb.LiberacaoAVista ?
                                        CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, retorno.creditoGerado, 0, null, false, null, null) :
                                    tipoReceb == TipoReceb.ContaReceber ?
                                        CaixaGeralDAO.Instance.MovCxContaRec(sessao, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR, conta.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado), 1, retorno.creditoGerado, 0, null, 0, false, null, null) :
                                    tipoReceb == TipoReceb.Acerto ?
                                        CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado), 1, retorno.creditoGerado, 0, null, 0, false, null, null) :
                                    tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado ?
                                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, retorno.creditoGerado, 0, null, 0, false, null, null) :
                                    tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado ?
                                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque.Value, idCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 2, retorno.creditoGerado, 0, null, 0, false, null, null) :
                                    tipoReceb == TipoReceb.TrocaDevolucao ?
                                        CaixaGeralDAO.Instance.MovCxTrocaDev(sessao, trocaDev.IdTrocaDevolucao, trocaDev.IdPedido, trocaDev.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, retorno.creditoGerado, 0, null, false, null, null) : 0;

                                // Credita crédito do cliente
                                ClienteDAO.Instance.CreditaCredito(sessao, idCliente, retorno.creditoGerado);
                                retorno.creditoCreditado = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg(ex.Message, ex));
                    }

                    #endregion
                }
                else
                    throw new Exception("Você não tem permissão para receber contas.");
            }
            catch (Exception ex)
            {
                retorno.ex = ex;
            }

            return retorno;
        }

        #endregion

        #region Pagamento

        public static void ValidaValorCreditoFornecedor(GDASession sessao, uint idFornec, List<CaixaGeral> cxGeral)
        {
            List<uint> gerado = UtilsPlanoConta.GetLstCredito(2);
            List<uint> utilizado = UtilsPlanoConta.GetLstCredito(3);

            decimal valorGerado = 0, valorUtilizado = 0;

            #region Soma os valores utilizados/gerados

            foreach (CaixaGeral c in cxGeral)
                if (gerado.Contains(c.IdConta))
                    valorGerado += c.ValorMov;
                else if (utilizado.Contains(c.IdConta))
                    valorUtilizado += c.ValorMov;

            #endregion

            decimal creditoFornecedor = FornecedorDAO.Instance.GetCredito(sessao, idFornec);
            if (valorUtilizado < valorGerado && Math.Abs(valorUtilizado - valorGerado) > creditoFornecedor)
                throw new Exception("O crédito que será estornado é maior que o valor de crédito do fornecedor. " +
                    "Crédito fornecedor: " + creditoFornecedor.ToString("C") + " Valor a estornar: " +
                    Math.Abs(valorUtilizado - valorGerado).ToString("C"));
        }

        #endregion

        #region Cancelamento de Recebimento

        /// <summary>
        /// Realiza o cancelamento de recebimento de várias partes do sistema
        /// </summary>
        public static void CancelaRecebimento(GDASession sessao, TipoReceb tipoReceb, Pedido pedido, Sinal sinal,
            LiberarPedido liberacao, ContasReceber contaRec, Acerto acerto, uint idAcertoCheque, Obra obra, TrocaDevolucao trocaDevolucao,
            DevolucaoPagto devolucaoPagto, CartaoNaoIdentificado cartaoNaoIdentificado, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCredito)
        {
            if (!cancelamentoErroTef && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.CancelarRecebimentos))
                throw new Exception("Você não tem permissão para cancelar recebimentos, contacte o administrador");

            // Variável criada para impedir que o estorno seja duplicado no caixa geral.
            var estornouCaixaGeral = false;

            // Define a hora de estorno para as 23:00
            dataEstornoBanco = dataEstornoBanco.Date.AddHours(23);

            IList<MovBanco> lstMovBanco = new List<MovBanco>();

            #region Cancelamento TEF

            if (gerarCredito)
            {
                //Se utilizar o controle de tef e tiver usado cartão como pagamento, não cancela o recebimento e sim gera credito.
                switch (tipoReceb)
                {
                    case TipoReceb.PedidoAVista:
                        break;
                    case TipoReceb.LiberacaoAVista:
                        if (TransacaoCapptaTefDAO.Instance.TemRecebimentoComTef(sessao, tipoReceb, (int)liberacao.IdLiberarPedido))
                        {
                            ClienteDAO.Instance.CreditaCredito(sessao, liberacao.IdCliente, liberacao.Total);
                            CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, liberacao.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1,
                                liberacao.Total, 0, null, false, dataEstornoBanco, "Geração de crédito de cancelamento da liberação recebida com TEF");
                            return;
                        }
                        break;
                    case TipoReceb.SinalPedido:
                        if (TransacaoCapptaTefDAO.Instance.TemRecebimentoComTef(sessao, tipoReceb, (int)sinal.IdSinal))
                        {
                            ClienteDAO.Instance.CreditaCredito(sessao, sinal.IdCliente, sinal.TotalSinal);
                            CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, sinal.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1,
                                sinal.TotalSinal, 0, null, false, dataEstornoBanco, "Geração de crédito de cancelamento do pagto. antecipado/sinal recebido com TEF");
                            return;
                        }
                        break;
                    case TipoReceb.SinalLiberacao:
                        break;
                    case TipoReceb.ContaReceber:
                        break;
                    case TipoReceb.Acerto:
                        if (TransacaoCapptaTefDAO.Instance.TemRecebimentoComTef(sessao, tipoReceb, (int)acerto.IdAcerto))
                        {
                            ClienteDAO.Instance.CreditaCredito(sessao, acerto.IdCli, acerto.TotalAcerto);
                            CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1,
                                acerto.TotalAcerto, 0, null, 0, false, dataEstornoBanco, "Geração de crédito de cancelamento do acerto recebido com TEF");
                            return;
                        }
                        break;
                    case TipoReceb.ChequeDevolvido:
                        if (TransacaoCapptaTefDAO.Instance.TemRecebimentoComTef(sessao, tipoReceb, (int)idAcertoCheque))
                        {
                            var acertoCheque = AcertoChequeDAO.Instance.GetElement(sessao, idAcertoCheque);
                            ClienteDAO.Instance.CreditaCredito(sessao, acertoCheque.IdCliente.Value, acertoCheque.ValorAcerto);
                            CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque, acertoCheque.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito), 1,
                                acertoCheque.ValorAcerto, 0, null, 0, false, dataEstornoBanco, "Geração de crédito de cancelamento do cheque devolvido recebido com TEF");
                            return;
                        }
                        break;
                    case TipoReceb.Obra:
                        if (TransacaoCapptaTefDAO.Instance.TemRecebimentoComTef(sessao, tipoReceb, (int)obra.IdObra))
                        {
                            #region Estorno da Obra antes de gerar Credito TEF

                            lstMovBanco = MovBancoDAO.Instance.GetByObra(sessao, obra.IdObra);
                            var cxGeral = CaixaGeralDAO.Instance.GetByObra(sessao, obra.IdObra);
                            var cxDiario = CaixaDiarioDAO.Instance.GetByObra(sessao, obra.IdObra);

                            if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                                throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                            ValidaValorCredito(sessao, obra.IdCliente, cxDiario, cxGeral, lstMovBanco);
                            VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                            // Exclui/Cancela cheques desta obra
                            ChequesDAO.Instance.DeleteByObra(sessao, obra.IdObra);

                            // Estorna movimentações no caixa diário
                            EstornaObra(sessao, obra, false, cxDiario);

                            /* Chamado 65319. */
                            var contadorDataUnica = 0;

                            foreach (CaixaGeral cx in cxGeral)
                            {
                                if (cx.TipoMov == 2)
                                {
                                    // Se for juros de venda cartão continua
                                    if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                                        continue;
                                    else
                                        break;
                                }

                                bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                                // Soma ou subtrai crédito do cliente
                                CreditoCliente(sessao, obra.IdCliente, cx.IdConta, cx.ValorMov);

                                // Estorna valor no caixa geral                    
                                CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente, UtilsPlanoConta.EstornoAVista(cx.IdConta), 2, cx.ValorMov, cx.Juros, null, mudarSaldo, null, null,
                                    contadorDataUnica++);
                            }                          
                            #endregion

                            //Gera o Credito para o cliente
                            ClienteDAO.Instance.CreditaCredito(sessao, obra.IdCliente, obra.ValorObra);
                            CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado), 1,
                                obra.ValorObra, 0, null, false, dataEstornoBanco, "Geração de crédito de cancelamento da obra recebida com TEF", null);
                            return;
                        }
                        break;
                    case TipoReceb.TrocaDevolucao:
                        break;
                    case TipoReceb.CreditoValeFuncionario:
                        break;
                    case TipoReceb.DebitoValeFuncionario:
                        break;
                    case TipoReceb.DevolucaoPagto:
                        break;
                    case TipoReceb.ChequeReapresentado:
                        break;
                    case TipoReceb.ChequeProprioDevolvido:
                        break;
                    case TipoReceb.ChequeProprioReapresentado:
                        break;
                    case TipoReceb.LiberacaoAPrazoCheque:
                        break;
                    case TipoReceb.CartaoNaoIdentificado:
                        break;
                    default:
                        break;
                }
            }

            #endregion

            if (tipoReceb == TipoReceb.PedidoAVista)
            {
                #region Pedido

                lstMovBanco = MovBancoDAO.Instance.GetByPedidoAVista(sessao, pedido.IdPedido);
                var cxGeral = CaixaGeralDAO.Instance.GetByPedidoAVista(sessao, pedido.IdPedido);
                var cxDiario = CaixaDiarioDAO.Instance.GetListForEstorno(sessao, pedido.IdPedido, null, null, 1);

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, pedido.IdCli, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Caso algum cheque recebido na liberação tenha sido usado em algum pagamento, gera um valor restante de pagto no mesmo
                if (!PedidoConfig.LiberarPedido)
                    foreach (Cheques c in ChequesDAO.Instance.GetByPedido(pedido.IdPedido, 0, 0, 0))
                    {
                        if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                        {
                            uint idPagto = PagtoChequeDAO.Instance.GetPagtoByCheque(sessao, c.IdCheque);
                            if (idPagto > 0)
                            {
                                // Insere outra parcela contendo o valor restante a ser pago
                                ContasPagar contaPagar = new ContasPagar();
                                contaPagar.ValorVenc = c.Valor;
                                contaPagar.DataVenc = DateTime.Now;
                                contaPagar.Paga = false;
                                contaPagar.IdFornec = PagtoDAO.Instance.ObtemValorCampo<uint>(sessao, "idFornec", "idPagto=" + idPagto);
                                contaPagar.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                                contaPagar.IdPagtoRestante = idPagto;
                                contaPagar.IdFormaPagto = null;
                                contaPagar.IdLoja = UserInfo.GetUserInfo.IdLoja;
                                contaPagar.Obs = "Pagto. rest. gerado por cheque usado no mesmo pertencer à uma liberação cancelada.";
                                contaPagar.NumParc = 1;
                                contaPagar.NumParcMax = 1;

                                contaPagar.IdContaPg = ContasPagarDAO.Instance.Insert(sessao, contaPagar);
                            }
                        }
                    }

                // Exclui cheques relacionados à este pedido
                ChequesDAO.Instance.DeleteByPedido(sessao, pedido.IdPedido);

                // Estorna movimentações deste pedido no caixa diário
                EstornaPedidoCxDiario(sessao, pedido, false, cxDiario, ref estornouCaixaGeral);

                #region Estorna movimentações deste pedido no caixa geral

                foreach (CaixaGeral cx in cxGeral)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    if (cx.TipoMov == 2)
                        break;

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, pedido.IdCli, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxPedido(sessao, pedido.IdPedido, pedido.IdCli,
                        UtilsPlanoConta.EstornoAVista(cx.IdConta), 2, cx.ValorMov, 0, null, mudarSaldo, null, null);
                }

                if (!estornouCaixaGeral)
                    // Estorna movimentações feitas outro dia no caixa diário no caixa geral
                    EstornaPedidoCxDiario(sessao, pedido, true, cxDiario, ref estornouCaixaGeral);

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna valores à vista
                foreach (MovBanco m in lstMovBanco)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    // Se for juros de venda cartão, estorna
                    if (m.TipoMov == 2)
                    {
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                            m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                        {
                            ExcluiMovBanco(sessao, m.IdMovBanco);
                            continue;
                        }
                        else
                            break;
                    }

                    ExcluiMovBanco(sessao, m.IdMovBanco);
                }

                #endregion

                // Cancela o sinal do pedido
                if (pedido.RecebeuSinal)
                {
                    Pedido[] lstPed = PedidoDAO.Instance.GetBySinal(sessao, pedido.IdSinal.Value, false);

                    // Se for liberação de pedido ou se o sinal tiver sido recebido de apenas um pedido, cancela o sinal,
                    // senão cancela o sinal gerando crédito para o cliente.
                    if (PedidoConfig.LiberarPedido || lstPed.Length == 1)
                        SinalDAO.Instance.Cancelar(sessao, pedido.IdSinal.Value, null, true, false, "Cancelamento do pedido " + pedido.IdPedido, dataEstornoBanco, false, false);
                    else
                        SinalDAO.Instance.Cancelar(sessao, pedido.IdSinal.Value, pedido.IdPedido, true, true, "Cancelamento do pedido " + pedido.IdPedido, dataEstornoBanco, false, false);
                }

                // Atualiza o saldo da obra, se pedido tiver obra associada
                if (pedido.IdObra != null && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                    ObraDAO.Instance.AtualizaSaldo(sessao, pedido.IdObra.Value, false);

                // Exclui contas a receber que podem ter sido geradas para este pedido e que ainda não foi paga
                ContasReceberDAO.Instance.DeleteByPedido(sessao, pedido.IdPedido);

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByPedido(sessao, pedido.IdPedido);

                #endregion
            }
            else if (tipoReceb == TipoReceb.LiberacaoAVista)
            {
                #region Liberação

                lstMovBanco = MovBancoDAO.Instance.GetByLiberacao(sessao, liberacao.IdLiberarPedido, 1);
                var cxGeralVista = CaixaGeralDAO.Instance.GetByLiberacao(sessao, liberacao.IdLiberarPedido, 1);
                var cxGeralEntrada = CaixaGeralDAO.Instance.GetByLiberacao(sessao, liberacao.IdLiberarPedido, 2);
                var cxDiarioVista = CaixaDiarioDAO.Instance.GetListForEstorno(sessao, null, liberacao.IdLiberarPedido, null, 1);
                var cxDiarioEntrada = CaixaDiarioDAO.Instance.GetListForEstorno(sessao, null, liberacao.IdLiberarPedido, null, 2);

                List<CaixaGeral> cxGeral = new List<CaixaGeral>();
                cxGeral.AddRange(cxGeralVista);
                cxGeral.AddRange(cxGeralEntrada);

                List<CaixaDiario> cxDiario = new List<CaixaDiario>();
                cxDiario.AddRange(cxDiarioVista);
                cxDiario.AddRange(cxDiarioEntrada);

                if (cxDiario.Count > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, liberacao.IdCliente, cxDiario.ToArray(), cxGeral.ToArray(), lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Caso algum cheque recebido na liberação tenha sido usado em algum pagamento, gera um valor restante de pagto no mesmo
                foreach (Cheques c in ChequesDAO.Instance.GetByLiberacaoPedido(sessao, liberacao.IdLiberarPedido))
                {
                    if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                    {
                        uint idPagto = PagtoChequeDAO.Instance.GetPagtoByCheque(sessao, c.IdCheque);
                        if (idPagto > 0)
                        {
                            // Insere outra parcela contendo o valor restante a ser pago
                            ContasPagar contaPagar = new ContasPagar();
                            contaPagar.ValorVenc = c.Valor;
                            contaPagar.DataVenc = DateTime.Now;
                            contaPagar.Paga = false;
                            contaPagar.IdFornec = PagtoDAO.Instance.ObtemValorCampo<uint>(sessao, "idFornec", "idPagto=" + idPagto);
                            contaPagar.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorRestantePagto);
                            contaPagar.IdPagtoRestante = idPagto;
                            contaPagar.IdFormaPagto = null;
                            contaPagar.IdLoja = UserInfo.GetUserInfo.IdLoja;
                            contaPagar.Obs = "Pagto. rest. gerado por cheque usado no mesmo pertencer à uma liberação cancelada.";
                            contaPagar.NumParc = 1;
                            contaPagar.NumParcMax = 1;

                            contaPagar.IdContaPg = ContasPagarDAO.Instance.Insert(sessao, contaPagar);
                        }
                    }
                }

                // Exclui cheques relacionados à esta liberação de pedido
                ChequesDAO.Instance.DeleteByLiberarPedido(sessao, liberacao.IdLiberarPedido);

                // Estorna valores desta liberação de pedido gerados no caixa diário
                EstornaLiberarPedido(sessao, liberacao, false, cxDiarioVista, cxDiarioEntrada, ref estornouCaixaGeral);

                #region Estorna valores desta liberação de pedido gerados no caixa geral

                var contadorDataUnica = 0;
                // Estorna valores à vista
                foreach (CaixaGeral cx in cxGeralVista)
                {
                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, liberacao.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, cx.IdCliente,
                        UtilsPlanoConta.EstornoAVista(cx.IdConta), 2, cx.ValorMov, 0, null, mudarSaldo, null, null, contadorDataUnica++);
                }

                // Estorna valores de sinal da liberação
                foreach (CaixaGeral cx in cxGeralEntrada)
                {
                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, liberacao.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, cx.IdCliente,
                        UtilsPlanoConta.EstornoSinalPedido(cx.IdConta), 2, cx.ValorMov, 0, null, mudarSaldo, null, null, contadorDataUnica++);
                }

                if (!estornouCaixaGeral)
                    // Estorna movimentações feitas outro dia no caixa diário no caixa geral
                    EstornaLiberarPedido(sessao, liberacao, true, cxDiarioVista, cxDiarioEntrada, ref estornouCaixaGeral);

                #endregion

                #region Estorna valores na conta bancária

                // Estorna valores gerados pelo pedido nas contas bancárias à vista
                foreach (MovBanco m in lstMovBanco)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    // Se for juros de venda cartão, estorna
                    if (m.TipoMov == 2)
                    {
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                            m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                        {
                            ExcluiMovBanco(sessao, m.IdMovBanco);

                            continue;
                        }
                        else
                            break;
                    }

                    ExcluiMovBanco(sessao, m.IdMovBanco);
                }

                // Estorna valores gerados pelo pedido nas contas bancárias referente à sinal
                foreach (MovBanco m in MovBancoDAO.Instance.GetByLiberacao(sessao, liberacao.IdLiberarPedido, 2))
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    // Se for juros de venda cartão, estorna
                    if (m.TipoMov == 2)
                    {
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        {
                            ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                liberacao.IdLiberarPedido, m.IdCliente, 1, m.ValorMov,
                                DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                            continue;
                        }
                        else
                            break;
                    }

                    ContaBancoDAO.Instance.MovContaLiberarPedido(sessao, m.IdContaBanco,
                        UtilsPlanoConta.EstornoSinalPedido(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja, liberacao.IdLiberarPedido, m.IdCliente, 2, m.ValorMov,
                        DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                }

                #endregion

                // Remove as contas a receber
                ContasReceberDAO.Instance.DeleteByLiberarPedido(sessao, liberacao.IdLiberarPedido);

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByLiberacao(sessao, liberacao.IdLiberarPedido);

                // Remove as contas recebidas geradas automaticamente
                if (liberacao.TipoPagto == (int)LiberarPedido.TipoPagtoEnum.AVista)
                {
                    foreach (ContasReceber conta in ContasReceberDAO.Instance.GetByLiberacaoPedido(sessao, liberacao.IdLiberarPedido, false))
                        /* Chamado 23476. */
                        //if (conta.DataCad.Date == liberacao.DataLiberacao.Date && conta.Recebida)
                        ContasReceberDAO.Instance.DeleteByPrimaryKey(sessao, conta.IdContaR);
                }

                #endregion
            }
            else if (tipoReceb == TipoReceb.SinalPedido)
            {
                #region Sinal Pedido

                lstMovBanco = MovBancoDAO.Instance.GetBySinal(sessao, sinal.IdSinal);
                var cxGeral = CaixaGeralDAO.Instance.GetBySinal(sessao, sinal.IdSinal);
                var cxDiario = CaixaDiarioDAO.Instance.GetListForEstorno(sessao, null, null, sinal.IdSinal, 2);

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, sinal.IdCliente, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui/Cancela cheques usados no sinal do pedido
                ChequesDAO.Instance.DeleteBySinalPedido(sessao, sinal.IdSinal);

                #region Estorna movimentações deste sinal no caixa geral/diário

                foreach (CaixaGeral c in cxGeral)
                {
                    if (c.TipoMov == 2)
                    {
                        // Se for juros de venda cartão, continua
                        if (c.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(c.IdConta, true);

                    // Soma ou subtrai crédito do cliente ()
                    CreditoCliente(sessao, sinal.IdCliente, c.IdConta, c.ValorMov);

                    CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, c.IdCliente,
                        UtilsPlanoConta.EstornoSinalPedido(c.IdConta), 2, c.ValorMov, 0, null, mudarSaldo, null, null);
                }

                // Estorna movimentações feitas no caixa diário, deve sempre entrar aqui, pois pode ser que o sinal tenha sido recebido no caixa diário
                // porém retificado no caixa geral, nesta situação, ao cancelar o sinal, da forma como estava antes iria estornar o valor da retificação
                // que foi gerada no caixa geral e não entraria no caixa diário para estornar o valor restante (Chamado 9484).
                EstornaSinal(sessao, sinal, false, cxDiario);

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna valores à vista
                foreach (MovBanco m in lstMovBanco)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    if (m.TipoMov == 2)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);

                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaSinal(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    sinal.IdSinal, m.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaSinal(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    sinal.IdSinal, m.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        ContaBancoDAO.Instance.MovContaSinal(sessao, m.IdContaBanco,
                            UtilsPlanoConta.EstornoSinalPedido(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja,
                            sinal.IdSinal, m.IdCliente, 2, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoBySinal(sessao, sinal.IdSinal);

                #endregion
            }
            else if (tipoReceb == TipoReceb.ContaReceber)
            {
                #region Conta Recebida

                lstMovBanco = MovBancoDAO.Instance.GetByContaRecForCanc(sessao, contaRec.IdContaR).ToArray();
                var cxGeral = CaixaGeralDAO.Instance.GetByContaRec(sessao, contaRec.IdContaR);
                var cxDiario = CaixaDiarioDAO.Instance.GetByContaRec(sessao, contaRec.IdContaR);

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, contaRec.IdCliente, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui cheques utilizados para pagar esta conta, se possível
                ChequesDAO.Instance.DeleteByContaRec(sessao, contaRec.IdContaR);

                // Estorna movimentações desta conta no caixa diário
                EstornaContaReceber(sessao, contaRec, false, cxDiario, ref estornouCaixaGeral);

                #region Estorna movimentações desta conta no caixa geral

                var contadorDataUnica = 0;

                foreach (CaixaGeral cx in cxGeral)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    var mudarSaldo = false;

                    /* Chamado 16223 e 45152.
                     * O recebimento de juros está definido para não movimentar o saldo do caixa,
                     * sendo assim, o estorno também não pode movimentar. */
                    if (cx.IdConta == FinanceiroConfig.PlanoContaJurosReceb)
                        mudarSaldo = false;
                    else
                        mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, contaRec.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxContaRec(sessao, contaRec.IdPedido, contaRec.IdLiberarPedido, contaRec.IdContaR,
                        contaRec.IdCliente, UtilsPlanoConta.EstornoAPrazo(cx.IdConta), 2, cx.ValorMov, cx.Juros, null, cx.FormaSaida,
                        mudarSaldo, null, null, contadorDataUnica++);

                    estornouCaixaGeral = true;
                }

                if (!estornouCaixaGeral)
                    // Estorna movimentações desta conta no caixa diário
                    EstornaContaReceber(sessao, contaRec, true, cxDiario, ref estornouCaixaGeral);

                #endregion

                #region Estorna movimentações desta conta nas movimentações de contas bancárias

                foreach (MovBanco m in lstMovBanco)
                {
                    // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                    // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                    // por isso, para o loop neste momento
                    if (m.TipoMov == 2)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);

                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaContaR(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    contaRec.IdPedido, contaRec.IdLiberarPedido, m.IdContaR, m.IdSinal, contaRec.IdCliente, 1, m.ValorMov,
                                    m.Juros, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaContaR(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    contaRec.IdPedido, contaRec.IdLiberarPedido, m.IdContaR, m.IdSinal, contaRec.IdCliente, 1, m.ValorMov,
                                    m.Juros, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        ContaBancoDAO.Instance.MovContaContaR(sessao, m.IdContaBanco,
                            UtilsPlanoConta.EstornoAPrazo(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja,
                            m.IdPedido, m.IdLiberarPedido, m.IdContaR, m.IdSinal, contaRec.IdCliente, 2, m.ValorMov, m.Juros,
                            DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByContaR(sessao, contaRec.IdContaR);

                #endregion
            }
            else if (tipoReceb == TipoReceb.Acerto)
            {
                #region Acerto

                lstMovBanco = MovBancoDAO.Instance.GetByAcerto(sessao, acerto.IdAcerto);
                var cxGeral = CaixaGeralDAO.Instance.GetByAcerto(sessao, acerto.IdAcerto);
                var cxDiario = CaixaDiarioDAO.Instance.GetByAcerto(sessao, acerto.IdAcerto);

                if (cxDiario.Count > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, acerto.IdCli, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui cheques deste acerto
                ChequesDAO.Instance.DeleteByAcerto(sessao, acerto.IdAcerto);

                // Estorna movimentações deste acerto no caixa diário
                EstornaAcerto(sessao, acerto, false, cxDiario, ref estornouCaixaGeral);

                #region Estorna movimentações deste acerto no caixa geral

                var contadorDataUnica = 0;

                foreach (CaixaGeral cx in cxGeral)
                {
                    var mudarSaldo = false;

                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    /* Chamado 16223.
                     * O recebimento de juros está definido para não movimentar o saldo do caixa,
                     * sendo assim, o estorno também não pode movimentar. */
                    if (cx.IdConta == FinanceiroConfig.PlanoContaJurosReceb)
                        mudarSaldo = false;
                    else
                        mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, acerto.IdCli, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli,
                        UtilsPlanoConta.EstornoAPrazo(cx.IdConta),
                        2, cx.ValorMov, cx.Juros, null, cx.FormaSaida, mudarSaldo, null, null, contadorDataUnica++);
                }

                if (!estornouCaixaGeral)
                    // Estorna movimentações deste acerto no caixa diário
                    EstornaAcerto(sessao, acerto, true, cxDiario, ref estornouCaixaGeral);

                #endregion

                #region Estorna movimentações deste acerto na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 2)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);

                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaAcerto(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    acerto.IdAcerto, acerto.IdCli, 1, m.ValorMov, m.Juros, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaAcerto(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    acerto.IdAcerto, acerto.IdCli, 1, m.ValorMov, m.Juros, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        ContaBancoDAO.Instance.MovContaAcerto(sessao, m.IdContaBanco,
                            UtilsPlanoConta.EstornoAPrazo(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja,
                            acerto.IdAcerto, acerto.IdCli, 2, m.ValorMov, m.Juros, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByAcerto(sessao, acerto.IdAcerto);

                #endregion
            }
            else if (tipoReceb == TipoReceb.ChequeDevolvido || tipoReceb == TipoReceb.ChequeReapresentado)
            {
                #region Cheque Devolvido

                AcertoCheque acertoCheque = AcertoChequeDAO.Instance.GetElement(sessao, idAcertoCheque);
                var contadorDataUnica = 0;
                lstMovBanco = MovBancoDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque);
                var cxGeral = CaixaGeralDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque);
                var cxDiario = CaixaDiarioDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque);

                if (cxDiario.Count > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                if (acertoCheque.IdCliente > 0)
                    ValidaValorCredito(sessao, acertoCheque.IdCliente.Value, cxDiario, cxGeral, lstMovBanco);

                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui/Cancela cheques utilizados no acerto
                ChequesDAO.Instance.DeleteByAcertoCheque(sessao, idAcertoCheque);

                #region Estorno dos valores pagos no Cx. Diario

                foreach (var cx in cxDiario)
                {
                    if (cx.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                    {
                        if (cx.TipoMov == 2)
                        {
                            // Se for juros de venda cartão continua
                            if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                                continue;
                            else if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado))
                            {
                                // Realiza estorno de cheque trocado
                                CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, cx.IdLoja, cx.IdCheque, null, idAcertoCheque, acertoCheque.IdCliente,
                                    null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoChequeTrocado), 1, cx.Valor,
                                    cx.Juros, null, true, null, contadorDataUnica++);
                                continue;
                            }
                            else
                                break;
                        }

                        bool mudarSaldo = MudarSaldo(cx.IdConta, false);

                        // Soma ou subtrai crédito do cliente
                        if (acertoCheque.IdCliente > 0)
                            CreditoCliente(sessao, acertoCheque.IdCliente.Value, cx.IdConta, cx.Valor);

                        // Estorna valor no caixa diario                    
                        uint idConta = 0;

                        try
                        {
                            idConta = UtilsPlanoConta.GetPlanoContaEstornoChequeDev(cx.IdConta);
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("Plano de conta de estorno de cheque devolvido não encontrado."))
                                throw ex;
                        }

                        if (idConta > 0)
                            CaixaDiarioDAO.Instance.MovCxAcertoCheque(sessao, cx.IdLoja, null, null, idAcertoCheque, acertoCheque.IdCliente,
                                null, idConta, 2, cx.Valor, cx.Juros, null, mudarSaldo, null, contadorDataUnica++);
                    }
                }

                #endregion

                #region Estorno dos valores pagos no Cx. Geral

                foreach (CaixaGeral cx in cxGeral)
                {
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado))
                        {
                            // Realiza estorno de cheque trocado
                            CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque, cx.IdCheque, acertoCheque.IdCliente,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoChequeTrocado), 1, cx.ValorMov,
                                cx.Juros, null, cx.FormaSaida, true, null, null, contadorDataUnica++);
                            continue;
                        }
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    if (acertoCheque.IdCliente > 0)
                        CreditoCliente(sessao, acertoCheque.IdCliente.Value, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    uint idConta = 0;

                    try
                    {
                        idConta = UtilsPlanoConta.GetPlanoContaEstornoChequeDev(cx.IdConta);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Plano de conta de estorno de cheque devolvido não encontrado."))
                            throw ex;
                    }

                    if (idConta > 0)
                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque, acertoCheque.IdCliente,
                            idConta, 2, cx.ValorMov, cx.Juros, null, cx.FormaSaida, mudarSaldo, null, null, contadorDataUnica++);
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 2)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);
                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    null, idAcertoCheque, acertoCheque.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    null, idAcertoCheque, acertoCheque.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        uint idConta = 0;

                        try
                        {
                            idConta = UtilsPlanoConta.GetPlanoContaEstornoChequeDev(m.IdConta);
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("Plano de conta de estorno de cheque devolvido não encontrado."))
                                throw ex;
                        }

                        if (idConta > 0)
                            ContaBancoDAO.Instance.MovContaAcertoCheque(sessao, m.IdContaBanco,
                                idConta, (int)UserInfo.GetUserInfo.IdLoja, null, idAcertoCheque, acertoCheque.IdCliente, 2, m.ValorMov,
                                DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                #region Remove os valores pagos nos cheques

                foreach (ItemAcertoCheque i in ItemAcertoChequeDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque))
                {
                    Cheques c = ChequesDAO.Instance.GetElementByPrimaryKey(sessao, i.IdCheque);
                    decimal valor = Math.Min(c.ValorReceb, i.ValorReceb);
                    var jurosAcerto = AcertoChequeDAO.Instance.ObtemValorCampo<decimal>(sessao, "Juros", "idAcertoCheque=" + idAcertoCheque);
                    var numCheques = ItemAcertoChequeDAO.Instance.ObtemValorCampo<int>(sessao, "Count(*)", "idAcertoCheque=" + i.IdAcertoCheque);
                    var descontoAcerto = AcertoChequeDAO.Instance.ObtemValorCampo<decimal>(sessao, "Desconto", "idAcertoCheque=" + idAcertoCheque);
                    var valorTotal = AcertoChequeDAO.Instance.ObtemValorCampo<decimal>(sessao, "ValorAcerto", "idAcertoCheque=" + idAcertoCheque);
                    valorTotal = Math.Round(valorTotal - jurosAcerto + descontoAcerto, 2);

                    c.ValorReceb -= valor;
                    c.JurosReceb -= Math.Round(jurosAcerto / numCheques, 2);
                    c.DescontoReceb -= Math.Round((c.ValorRestante + c.DescontoReceb) / valorTotal * descontoAcerto, 2);
                    c.Situacao = c.Situacao == (int)Cheques.SituacaoCheque.Quitado ? (int)Cheques.SituacaoCheque.Devolvido :
                        c.Situacao == (int)Cheques.SituacaoCheque.Trocado && c.ValorReceb == 0 ? (int)Cheques.SituacaoCheque.EmAberto : c.Situacao;
                    c.DataReceb = c.Situacao == (int)Cheques.SituacaoCheque.Devolvido || c.Situacao == (int)Cheques.SituacaoCheque.EmAberto ? null : c.DataReceb;
                    ChequesDAO.Instance.UpdateBase(sessao, c);

                    i.ValorReceb -= valor;
                    ItemAcertoChequeDAO.Instance.Update(sessao, i);
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByAcertoCheque(sessao, idAcertoCheque);

                #endregion
            }
            else if (tipoReceb == TipoReceb.ChequeProprioDevolvido || tipoReceb == TipoReceb.ChequeProprioReapresentado)
            {
                #region Cheque Próprio Devolvido

                AcertoCheque acertoCheque = AcertoChequeDAO.Instance.GetElement(sessao, idAcertoCheque);

                lstMovBanco = MovBancoDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque);
                var cxGeral = CaixaGeralDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque);
                var cxDiario = new CaixaDiario[0];

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                if (acertoCheque.IdCliente > 0)
                    ValidaValorCredito(sessao, acertoCheque.IdCliente.Value, cxDiario, cxGeral, lstMovBanco);

                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 2);

                if (tipoReceb == TipoReceb.ChequeProprioReapresentado)
                    // Exclui/Cancela cheques utilizados no acerto
                    ChequesDAO.Instance.DeleteByAcertoCheque(sessao, idAcertoCheque);
                else
                {
                    var c = ChequesDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque, false);
                    foreach (var cheque in c.Where(f => f.Tipo == 2).ToList())
                    {
                        cheque.Situacao = cheque.Situacao == (int)Cheques.SituacaoCheque.Compensado ? (int)Cheques.SituacaoCheque.EmAberto :
                            cheque.Situacao == (int)Cheques.SituacaoCheque.Quitado ? (int)Cheques.SituacaoCheque.Devolvido : cheque.Situacao;
                        cheque.IdAcertoCheque = null;
                        ChequesDAO.Instance.Update(sessao, cheque);
                    }

                    if (c.Count(f => f.Tipo == 1) > 0)
                    {
                        if (c.Count(f => f.IdDeposito > 0) > 0)
                            throw new Exception(
                                string.Format("Não é possível cancelar o acerto de cheques porque o(s) cheque(s) de número {0} " +
                                    "foi(ram) compensado(s) no(s) depósito(s) {1}. Cancele o(s) depósito(s) antes de cancelar o acerto de cheques.",
                                    string.Join(",", c.Where(f => f.IdDeposito > 0).Select(f => f.Num).ToList()),
                                    string.Join(",", c.Where(f => f.IdDeposito > 0).Select(f => f.IdDeposito).ToList())));

                        ChequesDAO.Instance.CancelaChequesProprioAcertoChequeProprioDevolvido(sessao, (int)idAcertoCheque, Cheques.SituacaoCheque.Cancelado);
                    }
                }

                #region Estorno dos valores pagos no Cx. Geral

                foreach (CaixaGeral cx in cxGeral)
                {
                    // Se for juros de venda cartão continua
                    if (cx.TipoMov == 1 && cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else if (cx.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado))
                    {
                        // Realiza estorno de cheque trocado
                        CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque, cx.IdCheque, acertoCheque.IdCliente,
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoChequeTrocado), 1,
                            cx.ValorMov, cx.Juros, null, cx.FormaSaida, true, null, null);

                        continue;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    if (acertoCheque.IdCliente > 0)
                        CreditoCliente(sessao, acertoCheque.IdCliente.Value, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral            
                    if (cx.IdConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado))
                    {
                        uint idConta = 0;

                        try
                        {
                            idConta = UtilsPlanoConta.GetPlanoContaEstornoChequeDev(cx.IdConta);
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("Plano de conta de estorno de cheque devolvido não encontrado."))
                                throw ex;
                        }

                        if (idConta > 0)
                            CaixaGeralDAO.Instance.MovCxAcertoCheque(sessao, idAcertoCheque, cx.IdCheque, acertoCheque.IdCliente,
                                idConta, 1, cx.ValorMov, cx.Juros, null, cx.FormaSaida, mudarSaldo, null, null);
                    }
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 1)
                    {
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                            m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                        {
                            ExcluiMovBanco(sessao, m.IdMovBanco);

                            continue;
                        }
                        else
                            break;
                    }

                    ExcluiMovBanco(sessao, m.IdMovBanco);
                }

                #endregion

                #region Remove os valores pagos nos cheques

                foreach (ItemAcertoCheque i in ItemAcertoChequeDAO.Instance.GetByAcertoCheque(sessao, idAcertoCheque))
                {
                    Cheques c = ChequesDAO.Instance.GetElementByPrimaryKey(sessao, i.IdCheque);
                    decimal valor = Math.Min(c.ValorReceb, i.ValorReceb);

                    c.ValorReceb -= valor;
                    c.Situacao = c.Situacao == (int)Cheques.SituacaoCheque.Quitado ? (int)Cheques.SituacaoCheque.Devolvido :
                        c.Situacao == (int)Cheques.SituacaoCheque.Trocado && c.ValorReceb == 0 ? (int)Cheques.SituacaoCheque.EmAberto : c.Situacao;
                    ChequesDAO.Instance.Update(sessao, c);

                    i.ValorReceb -= valor;
                    ItemAcertoChequeDAO.Instance.Update(sessao, i);
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByAcertoCheque(sessao, idAcertoCheque);

                #endregion
            }
            else if (tipoReceb == TipoReceb.Obra)
            {
                #region Obra

                lstMovBanco = MovBancoDAO.Instance.GetByObra(sessao, obra.IdObra);
                var cxGeral = CaixaGeralDAO.Instance.GetByObra(sessao, obra.IdObra);
                var cxDiario = CaixaDiarioDAO.Instance.GetByObra(sessao, obra.IdObra);

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, obra.IdCliente, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui/Cancela cheques desta obra
                ChequesDAO.Instance.DeleteByObra(sessao, obra.IdObra);

                // Estorna movimentações no caixa diário
                EstornaObra(sessao, obra, false, cxDiario);

                #region Estorno dos valores pagos no Cx. Geral

                /* Chamado 65319. */
                var contadorDataUnica = 0;

                foreach (CaixaGeral cx in cxGeral)
                {
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, obra.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente, UtilsPlanoConta.EstornoAVista(cx.IdConta), 2, cx.ValorMov, cx.Juros, null, mudarSaldo, null, null,
                        contadorDataUnica++);
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 2)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);

                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaObra(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    obra.IdObra, obra.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaObra(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    obra.IdObra, obra.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        ContaBancoDAO.Instance.MovContaObra(sessao, m.IdContaBanco,
                            UtilsPlanoConta.EstornoAVista(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja, obra.IdObra, obra.IdCliente, 2,
                            m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByObra(sessao, obra.IdObra);

                #endregion
            }
            else if (tipoReceb == TipoReceb.TrocaDevolucao)
            {
                #region Troca/Devolução

                lstMovBanco = MovBancoDAO.Instance.GetByTrocaDevolucao(sessao, trocaDevolucao.IdTrocaDevolucao);
                var cxGeral = CaixaGeralDAO.Instance.GetByTrocaDevolucao(sessao, trocaDevolucao.IdTrocaDevolucao);
                var cxDiario = new CaixaDiario[0];

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, trocaDevolucao.IdCliente, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                // Exclui/Cancela cheques desta troca/devoluão
                ChequesDAO.Instance.DeleteByTrocaDevolucao(sessao, trocaDevolucao.IdTrocaDevolucao);

                #region Estorno dos valores pagos no Cx. Geral

                foreach (CaixaGeral cx in cxGeral)
                {
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, trocaDevolucao.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxTrocaDev(sessao, trocaDevolucao.IdTrocaDevolucao, trocaDevolucao.IdPedido, trocaDevolucao.IdCliente,
                        UtilsPlanoConta.EstornoAVista(cx.IdConta), 2, cx.ValorMov, cx.Juros, null, mudarSaldo, null, null);
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 2)
                    {
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                            m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                        {
                            ExcluiMovBanco(sessao, m.IdMovBanco);

                            continue;
                        }
                        else
                            break;
                    }

                    ExcluiMovBanco(sessao, m.IdMovBanco);
                }

                #endregion

                // Exclui contas a receber que podem ter sido geradas para esta troca/devolução e que ainda não foi paga
                ContasReceberDAO.Instance.DeleteByTrocaDevolucao(sessao, trocaDevolucao.IdTrocaDevolucao);

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByTrocaDevolucao(sessao, trocaDevolucao.IdTrocaDevolucao);

                #endregion
            }
            else if (tipoReceb == TipoReceb.DevolucaoPagto)
            {
                #region Devolução de pagamento

                lstMovBanco = MovBancoDAO.Instance.GetByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);
                var cxGeral = CaixaGeralDAO.Instance.GetByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);
                var cxDiario = CaixaDiarioDAO.Instance.GetByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);

                if (cxDiario.Length > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                ValidaValorCredito(sessao, devolucaoPagto.IdCliente, cxDiario, cxGeral, lstMovBanco);
                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 2);

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias

                // Reabre cheques desta devoluão de pagamento
                ChequesDAO.Instance.ReabreByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);

                #region Estorno dos valores pagos no Cx. Geral

                foreach (CaixaGeral cx in cxGeral)
                {
                    if (cx.TipoMov == 1)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, devolucaoPagto.IdCliente, cx.IdConta, cx.ValorMov);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto, devolucaoPagto.IdCliente,
                        UtilsPlanoConta.GetEstornoDevolucaoPagto(cx.IdConta), 1, cx.ValorMov, cx.Juros, null, mudarSaldo, null, null);
                }

                #endregion

                #region Estorno dos valores pagos no Cx. Diario

                foreach (CaixaDiario cx in cxDiario)
                {
                    if (cx.TipoMov == 1)
                    {
                        // Se for juros de venda cartão continua
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    bool mudarSaldo = MudarSaldo(cx.IdConta, false);

                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, devolucaoPagto.IdCliente, cx.IdConta, cx.Valor);

                    // Estorna valor no caixa geral                    
                    CaixaDiarioDAO.Instance.MovCxDevolucaoPagto(sessao, cx.IdLoja, devolucaoPagto.IdDevolucaoPagto, devolucaoPagto.IdCliente,
                         UtilsPlanoConta.GetEstornoDevolucaoPagto(cx.IdConta), 1, cx.Valor, cx.Juros, null, mudarSaldo, null);
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias
                foreach (MovBanco m in lstMovBanco)
                {
                    if (m.TipoMov == 1)
                    {
                        if (dataEstornoBanco == default(DateTime))
                        {
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao ||
                                m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ExcluiMovBanco(sessao, m.IdMovBanco);

                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            // Se for juros de venda cartão continua
                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            {
                                ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, m.IdContaBanco, FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                    devolucaoPagto.IdDevolucaoPagto, devolucaoPagto.IdCliente, 2, m.ValorMov,
                                    DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                            {
                                ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, m.IdContaBanco,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), (int)UserInfo.GetUserInfo.IdLoja,
                                    devolucaoPagto.IdDevolucaoPagto, devolucaoPagto.IdCliente, 2, m.ValorMov,
                                    DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                                continue;
                            }
                            else
                                break;
                        }
                    }

                    if (dataEstornoBanco == default(DateTime))
                    {
                        ExcluiMovBanco(sessao, m.IdMovBanco);
                    }
                    else
                    {
                        ContaBancoDAO.Instance.MovContaDevolucaoPagto(sessao, m.IdContaBanco,
                            UtilsPlanoConta.GetEstornoDevolucaoPagto(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja, devolucaoPagto.IdDevolucaoPagto,
                            devolucaoPagto.IdCliente, 1, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                    }
                }

                #endregion

                // Exclui contas a receber que podem ter sido geradas para esta devolução de pagamento e que ainda não foi paga
                ContasReceberDAO.Instance.DeleteByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);

                #endregion

                // Exclui as parcelas de cartão
                ContasReceberDAO.Instance.DeleteParcCartaoByDevolucaoPagto(sessao, devolucaoPagto.IdDevolucaoPagto);

                #endregion
            }
            else if (tipoReceb == TipoReceb.CartaoNaoIdentificado)
            {
                #region Cartão Não Identificado

                lstMovBanco = MovBancoDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cartaoNaoIdentificado.IdCartaoNaoIdentificado);
                var cxGeral = CaixaGeralDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cartaoNaoIdentificado.IdCartaoNaoIdentificado);
                var cxDiario = CaixaDiarioDAO.Instance.ObterMovimentacoesPorCartaoNaoIdentificado(sessao, cartaoNaoIdentificado.IdCartaoNaoIdentificado);

                if (cxDiario.Count > 0 && !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, UserInfo.GetUserInfo.IdLoja))
                    throw new Exception("O caixa não foi fechado no último dia de trabalho.");

                // A movimentação será sempre estornada.
                dataEstornoBanco = dataEstornoBanco == default(DateTime) ? DateTime.Now : dataEstornoBanco;

                VerificaDataEstorno(lstMovBanco, ref dataEstornoBanco, 1);

                #region Estorna movimentações na conta bancária

                #region Estorno dos valores pagos no Cx. Geral

                foreach (var cx in cxGeral)
                {
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua.
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    var mudarSaldo = MudarSaldo(cx.IdConta, true);

                    // Estorna valor no caixa geral.
                    CaixaGeralDAO.Instance.MovCxCartaoNaoIdentificado(sessao,
                        (uint)cartaoNaoIdentificado.IdCartaoNaoIdentificado, UtilsPlanoConta.EstornoAPrazo(cx.IdConta), 2, cx.ValorMov,
                        cx.Juros, mudarSaldo, dataEstornoBanco, null);
                }

                #endregion

                #region Estorno dos valores pagos no Cx. Diario

                foreach (var cx in cxDiario)
                {
                    if (cx.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua.
                        if (cx.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                            continue;
                        else
                            break;
                    }

                    var mudarSaldo = MudarSaldo(cx.IdConta, false);

                    // Estorna valor no caixa diário.
                    CaixaDiarioDAO.Instance.MovCxCartaoNaoIdentificado(sessao, cx.IdLoja,
                        (uint)cartaoNaoIdentificado.IdCartaoNaoIdentificado, UtilsPlanoConta.EstornoAPrazo(cx.IdConta), 2, cx.Valor,
                        cx.Juros, mudarSaldo, null);
                }

                #endregion

                #region Estorna movimentações na conta bancária

                // Estorna movimentações deste acerto nas movimentações de contas bancárias.
                foreach (var m in lstMovBanco)
                {
                    if (m.TipoMov == 2)
                    {
                        // Se for juros de venda cartão continua.
                        if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        {
                            ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, m.IdContaBanco,
                                FinanceiroConfig.PlanoContaEstornoJurosCartao, (int)UserInfo.GetUserInfo.IdLoja,
                                (uint)cartaoNaoIdentificado.IdCartaoNaoIdentificado, 1, m.ValorMov,
                                DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));

                            continue;
                        }
                        else
                            break;
                    }

                    ContaBancoDAO.Instance.MovContaCartaoNaoIdentificado(sessao, m.IdContaBanco,
                        UtilsPlanoConta.EstornoAPrazo(m.IdConta), (int)UserInfo.GetUserInfo.IdLoja, (uint)cartaoNaoIdentificado.IdCartaoNaoIdentificado,
                        2, m.ValorMov, DateTime.Parse(dataEstornoBanco.ToString("dd/MM/yyyy 23:00")));
                }

                #endregion

                // Exclui contas a receber que podem ter sido geradas para este cartão não identificado e que ainda não foram pagas.
                ContasReceberDAO.Instance.DeleteByCartaoNaoIdentificado(sessao, cartaoNaoIdentificado.IdCartaoNaoIdentificado);

                #endregion

                // Exclui as parcelas de cartão.
                ContasReceberDAO.Instance.DeleteParcCartaoByCartaoNaoIdentificado(sessao, cartaoNaoIdentificado.IdCartaoNaoIdentificado);

                #endregion
            }

            if (pedido != null || liberacao != null || acerto != null || contaRec != null || obra != null || sinal != null ||
                trocaDevolucao != null || devolucaoPagto != null || idAcertoCheque > 0)
            {
                //Se o recebimento foi feito com deposito nao identificado, desvilcula o mesmo
                // e volta sua situação para ativo
                DepositoNaoIdentificadoDAO.Instance.DesvinculaDepositoNaoIdentificado(sessao, pedido, liberacao, acerto,
                    contaRec, obra, sinal, trocaDevolucao, devolucaoPagto, idAcertoCheque);

                CartaoNaoIdentificadoDAO.Instance.DesvincularCartaoNaoIdentificado(sessao, pedido, liberacao, acerto,
                    contaRec, obra, sinal, trocaDevolucao, devolucaoPagto, idAcertoCheque);
            }
        }

        #region Identifica se o idConta permite mudar o saldo do Cx. diário ou Cx. geral

        /// <summary>
        /// Identifica se o idConta permite mudar o saldo do Cx. diário ou Cx. geral
        /// </summary>
        /// <param name="idConta"></param>
        /// <param name="cxGeral">A mudança de saldo ocorrerá no caixa geral?</param>
        private static bool MudarSaldo(uint idConta, bool cxGeral)
        {
            if (!cxGeral)
                return !UtilsPlanoConta.ListContasTipo(Glass.Data.Model.Pagto.FormaPagto.Permuta).Contains(idConta) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DevolucaoPagtoCredito) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDevolucaoPagtoCredito) &&
                    idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito) &&
                    idConta != FinanceiroConfig.PlanoContaJurosReceb;

            return
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DevolucaoPagtoCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDevolucaoPagtoCredito) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaPermuta) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoPermuta) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaPermuta) &&
                idConta != UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevPermuta) &&
                (cxGeral ? !UtilsPlanoConta.ListContasTipo(Glass.Data.Model.Pagto.FormaPagto.Deposito).Contains(idConta) : true) &&
                (cxGeral ? !UtilsPlanoConta.ListContasTipo(Glass.Data.Model.Pagto.FormaPagto.Boleto).Contains(idConta) : true) &&
                (cxGeral ? !UtilsPlanoConta.ListContasTipo(Glass.Data.Model.Pagto.FormaPagto.Cartao).Contains(idConta) : true) &&
                (cxGeral ? !UtilsPlanoConta.ListContasTipo(Glass.Data.Model.Pagto.FormaPagto.Construcard).Contains(idConta) : true);
        }

        #endregion

        #region Aplica estorno no crédito do cliente

        /// <summary>
        /// Valida os dados do crédito do cliente.
        /// Verifica se o cliente possui crédito suficiente para ser estornado.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cxDiario"></param>
        /// <param name="cxGeral"></param>
        /// <param name="movBanco"></param>
        private static void ValidaValorCredito(uint idCliente, IList<CaixaDiario> cxDiario, IList<CaixaGeral> cxGeral,
            IList<MovBanco> movBanco)
        {
            ValidaValorCredito(null, idCliente, cxDiario, cxGeral, movBanco);
        }

        /// <summary>
        /// Valida os dados do crédito do cliente.
        /// Verifica se o cliente possui crédito suficiente para ser estornado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <param name="cxDiario"></param>
        /// <param name="cxGeral"></param>
        /// <param name="movBanco"></param>
        private static void ValidaValorCredito(GDASession sessao, uint idCliente, IList<CaixaDiario> cxDiario, IList<CaixaGeral> cxGeral,
            IList<MovBanco> movBanco)
        {
            List<uint> gerado = UtilsPlanoConta.GetLstCredito(2);
            List<uint> utilizado = UtilsPlanoConta.GetLstCredito(3);
            List<uint> estornado = UtilsPlanoConta.GetLstCredito(5);

            decimal valorGerado = 0, valorUtilizado = 0;

            #region Soma os valores utilizados/gerados

            foreach (CaixaDiario c in cxDiario)
                if (gerado.Contains(c.IdConta))
                    valorGerado += c.Valor;
                else if (utilizado.Contains(c.IdConta))
                    valorUtilizado += c.Valor;
                else if (estornado.Contains(c.IdConta))
                    valorGerado -= c.Valor;

            foreach (CaixaGeral c in cxGeral)
                if (gerado.Contains(c.IdConta))
                    valorGerado += c.ValorMov;
                else if (utilizado.Contains(c.IdConta))
                    valorUtilizado += c.ValorMov;
                else if (estornado.Contains(c.IdConta))
                    valorGerado -= c.ValorMov;

            foreach (MovBanco m in movBanco)
                if (gerado.Contains(m.IdConta))
                    valorGerado += m.ValorMov;
                else if (utilizado.Contains(m.IdConta))
                    valorUtilizado += m.ValorMov;
                else if (estornado.Contains(m.IdConta))
                    valorGerado -= m.ValorMov;

            #endregion

            decimal creditoCliente = ClienteDAO.Instance.GetCredito(sessao, idCliente);
            if (valorUtilizado < valorGerado && (valorGerado - valorUtilizado) > creditoCliente)
                throw new Exception("O crédito que será estornado é maior que o valor de crédito do cliente. " +
                    "Crédito cliente: " + creditoCliente.ToString("C") + " Valor a estornar: " +
                    (valorGerado - valorUtilizado).ToString("C"));
        }

        /// <summary>
        /// Aplica estorno no crédito do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idConta"></param>
        /// <param name="retorno"></param>
        private static void CreditoCliente(GDASession sessao, uint idCliente, uint idConta, decimal valor)
        {
            // Se algum crédito tiver sido gasto, estorna no crédito do cliente
            if (UtilsPlanoConta.GetLstCredito(3).Contains(idConta))
            {
                ClienteDAO.Instance.CreditaCredito(sessao, idCliente, valor);
            }
            // Se algum crédito tiver sido gerado, estorna no crédito do cliente
            else if (UtilsPlanoConta.GetLstCredito(2).Contains(idConta))
            {
                ClienteDAO.Instance.DebitaCredito(sessao, idCliente, valor);
            }
        }

        #endregion

        #region Estorna todos os valores gerados pelo pedido (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna todos os valores gerados pelo pedido
        /// </summary>
        private static void EstornaPedidoCxDiario(GDASession session, Pedido pedido, bool cxGeral,
            IList<CaixaDiario> cxDiario, ref bool estornouCaixaGeral)
        {
            // Estorna valores à vista
            foreach (CaixaDiario c in cxDiario)
            {
                // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                // por isso, para o loop neste momento
                if (c.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (c.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (c.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(session, pedido.IdCli, c.IdConta, c.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(c.IdConta, cxGeral);

                        // Estorna valor no caixa diário
                        CaixaDiarioDAO.Instance.MovCxPedido(session, c.IdLoja, c.IdCliente, pedido.IdPedido, 2,
                            c.Valor, 0, UtilsPlanoConta.EstornoAVista(c.IdConta), null, mudarSaldo);
                    }
                }
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(session, pedido.IdCli, c.IdConta, c.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(c.IdConta, true);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxPedido(session, pedido.IdPedido, pedido.IdCli,
                        UtilsPlanoConta.EstornoAVista(c.IdConta), 2, c.Valor, 0, null, mudarSaldo, null, null);

                    // Impede que o estorno seja duplicado no caixa geral.
                    estornouCaixaGeral = true;
                }
            }
        }

        #endregion

        #region Estorna todos os valores gerados pela liberação (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna todos os valores gerados pela liberação do pedido
        /// </summary>
        public static void EstornaLiberarPedido(GDASession sessao, LiberarPedido liberacao, bool cxGeral,
            IList<CaixaDiario> lstAVista, IList<CaixaDiario> lstEntrada, ref bool estornouCaixaGeral)
        {
            // Estorna valores à vista
            foreach (CaixaDiario c in lstAVista)
            {
                if (c.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (c.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (c.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    //Estorna no caixa diario.
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, liberacao.IdCliente, c.IdConta, c.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(c.IdConta, cxGeral);

                        // Estorna valor no caixa geral
                        CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, c.IdLoja, c.IdCliente, liberacao.IdLiberarPedido, 2,
                             c.Valor, 0, UtilsPlanoConta.EstornoAVista(c.IdConta), null, null, mudarSaldo);
                    }
                }
                // Se a movimentação não tiver sido feita no caixa diário hoje, ela DEVE ser feita no caixa geral
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, liberacao.IdCliente, c.IdConta, c.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(c.IdConta, true);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, c.IdCliente,
                        UtilsPlanoConta.EstornoAVista(c.IdConta), 2, c.Valor, 0, null, mudarSaldo, null, null);

                    // Impede que o estorno seja duplicado no caixa geral.
                    estornouCaixaGeral = true;
                }
            }

            // Estorna valores de sinal de pedido
            foreach (CaixaDiario c in lstEntrada)
            {
                if (c.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (c.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (c.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, liberacao.IdCliente, c.IdConta, c.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(c.IdConta, cxGeral);

                        // Estorna valor no caixa geral
                        CaixaDiarioDAO.Instance.MovCxLiberarPedido(sessao, c.IdLoja, c.IdCliente, liberacao.IdLiberarPedido, 2,
                            c.Valor, 0, UtilsPlanoConta.EstornoSinalPedido(c.IdConta), null, null, mudarSaldo);
                    }
                }
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, liberacao.IdCliente, c.IdConta, c.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(c.IdConta, true);

                    // Estorna valor no caixa geral                    
                    CaixaGeralDAO.Instance.MovCxLiberarPedido(sessao, liberacao.IdLiberarPedido, c.IdCliente,
                        UtilsPlanoConta.EstornoSinalPedido(c.IdConta), 2, c.Valor, 0, null, mudarSaldo, null, null);

                    // Impede que o estorno seja duplicado no caixa geral.
                    estornouCaixaGeral = true;
                }
            }
        }

        #endregion

        #region Estorna o recebimento de sinal do pedido (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna o recebimento de sinal do pedido.
        /// </summary>
        public static void EstornaSinal(GDASession sessao, Sinal sinal, bool cxGeral, CaixaDiario[] cxDiario)
        {
            foreach (CaixaDiario c in cxDiario)
            {
                // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                // por isso, para o loop neste momento
                if (c.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (c.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (c.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, sinal.IdCliente, c.IdConta, c.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(c.IdConta, cxGeral);

                        // Estorna valor no caixa diário
                        CaixaDiarioDAO.Instance.MovCxSinal(sessao, c.IdLoja, sinal.IdCliente, sinal.IdSinal, 2, c.Valor, 0,
                            UtilsPlanoConta.EstornoSinalPedido(c.IdConta), null, null, mudarSaldo);
                    }
                }
                // Deve gerar estorno no caixa geral sempre que não puder ser gerado no caixa diário
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, sinal.IdCliente, c.IdConta, c.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(c.IdConta, true);

                    CaixaGeralDAO.Instance.MovCxSinal(sessao, sinal.IdSinal, c.IdCliente,
                        UtilsPlanoConta.EstornoSinalPedido(c.IdConta), 2, c.Valor, 0, null, mudarSaldo, null, null);
                }
            }
        }

        #endregion

        #region Estorna o recebimento de conta a receber (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna o recebimento de conta a receber
        /// </summary>
        public static void EstornaContaReceber(GDASession sessao, ContasReceber contaRec, bool cxGeral,
            CaixaDiario[] cxDiario, ref bool estornouCaixaGeral)
        {
            var contadorDataUnica = 0;

            foreach (CaixaDiario cd in cxDiario)
            {
                // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente, 
                // por isso, para o loop neste momento
                if (cd.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (cd.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (cd.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, contaRec.IdCliente, cd.IdConta, cd.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(cd.IdConta, cxGeral);

                        // Estorna valor no caixa diário                    
                        CaixaDiarioDAO.Instance.MovCxContaRec(sessao, cd.IdLoja, contaRec.IdCliente, contaRec.IdPedido,
                            contaRec.IdLiberarPedido, contaRec.IdContaR, 2, cd.Valor, cd.Juros,
                            UtilsPlanoConta.EstornoAPrazo(cd.IdConta), null, cd.FormaSaida != null ? cd.FormaSaida.Value : 0, null, mudarSaldo);
                    }
                }
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, contaRec.IdCliente, cd.IdConta, cd.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(cd.IdConta, true);

                    CaixaGeralDAO.Instance.MovCxContaRec(sessao, contaRec.IdPedido, contaRec.IdLiberarPedido, contaRec.IdContaR,
                        cd.IdCliente, UtilsPlanoConta.EstornoAPrazo(cd.IdConta), 2, cd.Valor, cd.Juros, null,
                        cd.FormaSaida != null ? cd.FormaSaida.Value : 0, mudarSaldo, null, null, contadorDataUnica++);

                    // Impede que o estorno seja duplicado no caixa geral.
                    estornouCaixaGeral = true;
                }
            }
        }

        #endregion

        #region Estorna o recebimento de acerto (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna o recebimento de conta a receber
        /// </summary>
        public static void EstornaAcerto(GDASession sessao, Acerto acerto, bool cxGeral,
            IList<CaixaDiario> cxDiario, ref bool estornouCaixaGeral)
        {
            foreach (CaixaDiario cd in cxDiario)
            {
                if (cd.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (cd.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (cd.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, acerto.IdCli, cd.IdConta, cd.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(cd.IdConta, cxGeral);

                        // Estorna valor no caixa diário                    
                        CaixaDiarioDAO.Instance.MovCxAcerto(sessao, cd.IdLoja, cd.IdCliente, acerto.IdAcerto, 2,
                            cd.Valor, cd.Juros, UtilsPlanoConta.EstornoAPrazo(cd.IdConta), null, cd.FormaSaida != null ? cd.FormaSaida.Value : 0, null, mudarSaldo);
                    }
                }
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, acerto.IdCli, cd.IdConta, cd.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(cd.IdConta, true);

                    CaixaGeralDAO.Instance.MovCxAcerto(sessao, acerto.IdAcerto, acerto.IdCli,
                        UtilsPlanoConta.EstornoAPrazo(cd.IdConta), 2, cd.Valor, cd.Juros, null, cd.FormaSaida != null ? cd.FormaSaida.Value : 0, mudarSaldo, null, null);

                    // Impede que o estorno seja duplicado no caixa geral.
                    estornouCaixaGeral = true;
                }
            }
        }

        #endregion

        #region Estorna o crédito gerado (CAIXA DIÁRIO)

        /// <summary>
        /// Estorna o crédito gerado para o cliente
        /// </summary>
        public static void EstornaObra(GDASession sessao, Obra obra, bool cxGeral, CaixaDiario[] cxDiario)
        {
            foreach (CaixaDiario cd in cxDiario)
            {
                if (cd.TipoMov == 2)
                {
                    // Se for juros de venda cartão continua
                    if (cd.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                        continue;
                    else
                        break;
                }

                // Se a data de cadastro desta movimentação tiver sido feita hoje, realiza estorno no caixa diário,
                // senão, se a variável cxGeral for true, realiza estorno no caixa geral
                if (cd.DataCad >= DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00")))
                {
                    if (!cxGeral)
                    {
                        // Soma ou subtrai crédito do cliente
                        CreditoCliente(sessao, obra.IdCliente, cd.IdConta, cd.Valor);

                        //Verifica se o plano de conta altera saldo no caixa diário
                        var mudarSaldo = MudarSaldo(cd.IdConta, cxGeral);

                        // Estorna valor no caixa diário                    
                        CaixaDiarioDAO.Instance.MovCxObra(sessao, cd.IdLoja, obra.IdCliente, obra.IdObra, 2,
                            cd.Valor, cd.Juros, UtilsPlanoConta.EstornoAVista(cd.IdConta), null, null, mudarSaldo);
                    }
                }
                else
                {
                    // Soma ou subtrai crédito do cliente
                    CreditoCliente(sessao, obra.IdCliente, cd.IdConta, cd.Valor);

                    //Verifica se o plano de conta altera saldo no caixa geral
                    var mudarSaldo = MudarSaldo(cd.IdConta, true);

                    CaixaGeralDAO.Instance.MovCxObra(sessao, obra.IdObra, obra.IdCliente,
                        UtilsPlanoConta.EstornoAVista(cd.IdConta), 2, cd.Valor, cd.Juros, null, mudarSaldo, null, null);
                }
            }
        }

        #endregion

        #region Verifica se a data de estorno bancário é válida

        private static void VerificaDataEstorno(IEnumerable<MovBanco> movBanco, ref DateTime dataEstornoBanco, int tipoMovValido)
        {
            var validos = movBanco.Where(x => x.TipoMov == tipoMovValido).ToArray();

            if (validos.Length == 0)
                return;

            DateTime menorData = DateTime.MaxValue;
            foreach (MovBanco m in validos)
                if (m.DataMov < menorData)
                    menorData = m.DataMov;

            // Chamado 13290.
            // Como a data do estorno não é informada, a mesma deve ser igual a data de hoje, porém se a data da movimentação
            // for superior a data atual então a data da movimentação deve ser considerada.
            if (dataEstornoBanco < menorData)
                dataEstornoBanco = menorData;
            //throw new Exception("Não é possível realizar as movimentações de estorno em uma data anterior à data das movimentações originais.");
        }

        #endregion

        #region Exclui uma movimentação bancária

        private static void ExcluiMovBanco(GDASession sessao, uint idMovBanco)
        {
            MovBancoDAO.Instance.ApagaMovimentacaoCorrigindoSaldo(sessao, idMovBanco);
        }

        #endregion

        #endregion
    }
}