using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Web;
using Glass.Data.Exceptions;
using System.Linq;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using Glass.Global;
using Colosoft;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed partial class PedidoDAO : BaseCadastroDAO<Pedido, PedidoDAO>
    {
        #region Confirmar Pedido

        private static readonly object _confirmarPedidoLock = new object();

        /// <summary>
        /// Confirma o pedido, gera contas a receber, dá baixa no estoque, 
        /// </summary>
        public string ConfirmarPedido(uint idPedido, uint[] formasPagto, uint[] tiposCartao, decimal[] valores, uint[] contasBanco, uint[] depositoNaoIdentificado,
            bool gerarCredito, decimal creditoUtilizado, string numAutConstrucard, uint[] numParcCartoes, string chequesPagto,
            bool descontarComissao, int tipoVendaObra, bool verificarParcelas, string[] numAutCartao)
        {
            lock (_confirmarPedidoLock)
            {
                FilaOperacoes.ConfirmarPedido.AguardarVez();

                using (var trans = new GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        Pedido ped = null;
                        string msg = string.Empty;
                        uint idCliente = PedidoDAO.Instance.ObtemValorCampo<uint>(trans, "idCli", "idPedido=" + idPedido);
                        int? tipoVenda = GetTipoVenda(trans, idPedido);
                        Pedido.TipoPedidoEnum tipoPedido = GetTipoPedido(trans, idPedido);
                        uint idFuncVenda = ObtemValorCampo<uint>(trans, "idFuncVenda", "idPedido=" + idPedido);
                        uint? idFormaPagto = ObtemValorCampo<uint?>(trans, "idFormaPagto", "idPedido=" + idPedido);
                        DateTime? dataEntrega = ObtemDataEntrega(trans, idPedido);
                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        ProdutosPedido[] lstProdPed = null;

                        // Guarda id do caixa diario inserido pelo valor da entrada do pedido
                        List<uint> lstIdContaRecVista = new List<uint>();
                        List<uint> lstIdContaRecPrazo = new List<uint>();

                        decimal totalPedido = GetTotal(trans, idPedido);

                        decimal totalPago = 0;

                        // Se a empresa tiver permissão para trabalhar com caixa diário
                        if (Geral.ControleCaixaDiario)
                        {
                            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                                throw new Exception("Apenas o Caixa pode confirmar pedidos.");
                        }
                        else
                        {
                            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                                !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                                throw new Exception("Você não tem permissão para confirmar pedidos.");
                        }

                        Pedido.SituacaoPedido situacao = ObtemSituacao(trans, idPedido);

                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido já confirmado.");

                        if (situacao != Pedido.SituacaoPedido.Conferido)
                            throw new Exception("O pedido ainda não foi conferido, portanto não pode ser confirmado.");

                        if (TemSinalReceber(trans, idPedido))
                            LancarExceptionValidacaoPedidoFinanceiro("O pedido tem sinal a receber. Receba-o para confirmar o pedido.",
                                idPedido, false, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                        else if (ProdutosPedidoDAO.Instance.GetCount(trans, idPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");

                        // Verifica se o pedido já possui contas a receber/recebidas para impedir que o mesmo seja confirmado duas vezes
                        if (ObtemIdSinal(trans, idPedido) == null && ContasReceberDAO.Instance.ExistsByPedido(trans, idPedido))
                            throw new Exception("Este pedido não pode ser confirmado pois já possui contas a receber/recebidas");

                        // Verifica se a data de entrega é inferior a hoje
                        if (dataEntrega != null && dataEntrega.Value < DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00")))
                            throw new Exception("A data de entrega do pedido não pode ser inferior à hoje, data de entrega: " + dataEntrega.Value.ToString("dd/MM/yyyy"));

                        var valorEntrada = ObtemValorEntrada(trans, idPedido);
                        var idPagamentoAntecipado = ObtemValorCampo<int>(trans, "IdPagamentoAntecipado", string.Format("IdPedido={0}", idPedido));
                        var valorPagamentoAntecipado = ObtemValorCampo<decimal>(trans, "ValorPagamentoAntecipado", string.Format("IdPedido={0}", idPedido));
                        var idSinal = ObtemValorCampo<int>(trans, "IdSinal", string.Format("IdPedido={0}", idPedido));

                        /* Chamado 52558.
                         * Se o pedido tiver sido pago antecipadamente e o valor de entrada mais o valor pago não seja igual ao valor do pedido, bloqueia a confirmação dele. */
                        if (valorPagamentoAntecipado > 0 && (idSinal > 0 ? valorEntrada : 0) + valorPagamentoAntecipado != totalPedido)
                            throw new Exception("O valor de entrada somado ao valor pago antecipadamente ultrapassa o valor total do pedido.");

                        // Quando aplicável, verifica se os produtos do pedido existem em estoque
                        if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
                        {
                            var pedidoReposicaoGarantia = tipoVenda == (int)Pedido.TipoVendaPedido.Reposição || tipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
                            var pedidoMaoObraEspecial = tipoPedido == Pedido.TipoPedidoEnum.MaoDeObraEspecial;
                            var lstProd = ProdutosPedidoDAO.Instance.GetByPedidoLite(trans, idPedido).ToArray();

                            foreach (var prod in lstProd)
                            {
                                var qtdProd = 0F;
                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(trans, (int)prod.IdProd);

                                // É necessário refazer o loop nos produtos do pedido para que caso tenha sido inserido o mesmo produto 2 ou mais vezes,
                                // seja somada a quantidade total inserida no pedido
                                foreach (var prod2 in lstProd)
                                {
                                    // Soma somente produtos iguais ao produto do loop principal de produtos
                                    if (prod.IdProd != prod2.IdProd)
                                        continue;

                                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                                        qtdProd += prod2.TotM;

                                    else if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                                        qtdProd += prod2.Qtde * prod2.Altura;

                                    else
                                        qtdProd += prod2.Qtde;
                                }

                                if (GrupoProdDAO.Instance.BloquearEstoque(trans, (int)prod.IdGrupoProd, (int)prod.IdSubgrupoProd))
                                {
                                    var estoque = ProdutoLojaDAO.Instance.GetEstoque(trans, ObtemIdLoja(trans, idPedido), prod.IdProd, null, IsProducao(trans, idPedido), false, true);

                                    if (estoque < qtdProd)
                                        throw new Exception("O produto " + prod.DescrProduto + " possui apenas " + estoque + " em estoque.");
                                }

                                // Verifica se o valor unitário do produto foi informado, pois pode acontecer do usuário inserir produtos zerados em 
                                // um pedido reposição/garantia e depois alterar o pedido para à vista/à prazo
                                if (!pedidoReposicaoGarantia && prod.ValorVendido == 0)
                                    throw new Exception("O produto " + prod.DescrProduto + " não pode ter valor zerado.");
                            }
                        }

                        ParcelasPedido[] lstParcPed;
                        lstProdPed = ProdutosPedidoDAO.Instance.GetByPedidoLite(trans, idPedido).ToArray();

                        // Se for venda para funcionário
                        if (idFuncVenda > 0 && tipoVenda == (int)Pedido.TipoVendaPedido.Funcionario)
                        {
                            #region Venda para funcionário

                            // Gera a movimentação no controle
                            MovFuncDAO.Instance.MovimentarPedido(trans, idFuncVenda, idPedido,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.FuncRecebimento), 2, totalPedido, null);

                            #endregion
                        }
                        // Se for venda à vista
                        else if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista ||
                            (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista))
                        {
                            #region Venda à vista

                            if (tipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                                totalPago += ObraDAO.Instance.GetSaldo(trans, ObtemValorCampo<uint>(trans, "idObra", "idPedido=" + idPedido));

                            foreach (decimal valor in valores)
                                totalPago += valor;

                            if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, formasPagto) && String.IsNullOrEmpty(chequesPagto))
                                throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento do pedido.");

                            // Se for pago com crédito, soma o mesmo ao totalPago
                            if (creditoUtilizado > 0)
                                totalPago += creditoUtilizado;

                            // Ignora os juros dos cartões ao calcular o valor pago/a pagar
                            totalPago -= UtilsFinanceiro.GetJurosCartoes(trans, UserInfo.GetUserInfo.IdLoja, valores, formasPagto, tiposCartao, numParcCartoes);

                            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção
                            if (gerarCredito && totalPago < totalPedido)
                                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + totalPedido.ToString("C") + " Valor pago: " + totalPago.ToString("C"));
                            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito
                            else if (!gerarCredito && Math.Round(totalPedido, 2) != Math.Round(totalPago, 2))
                                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + totalPedido.ToString("C") + " Valor pago: " + totalPago.ToString("C"));

                            #region Atualiza formas de pagamento

                            bool possuiFormaPagto = false;

                            for (int i = 0; i < formasPagto.Length; i++)
                                if (formasPagto[i] > 0)
                                {
                                    possuiFormaPagto = true;
                                    break;
                                }

                            if (possuiFormaPagto)
                            {
                                // Atualiza forma de pagamento de acordo com aquela que foi escolhida pelo caixa.
                                if (formasPagto.Length > 0 && formasPagto[0] > 0)
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdFormaPagto={0} Where IdPedido={1}", formasPagto[0], idPedido));

                                if (formasPagto.Length > 1 && formasPagto[1] > 0)
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdFormaPagto2={0} Where IdPedido={1}", formasPagto[1], idPedido));

                                // Atualiza tipo de cartão de acordo com aquele que foi escolhido pelo caixa
                                if ((uint)Pagto.FormaPagto.Cartao == formasPagto[0])
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdTipoCartao={0} WHERE IdPedido={1}", tiposCartao[0], idPedido));

                                if (formasPagto.Length > 1 && (uint)Pagto.FormaPagto.Cartao == formasPagto[1])
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdTipoCartao2={0} WHERE IdPedido={1}", tiposCartao[1], idPedido));
                            }
                            else if (creditoUtilizado == totalPedido) // Foi pago com crédito
                            {
                                // Altera a forma de pagamento do pedido para crédito
                                objPersistence.ExecuteCommand(trans, "Update pedido set idFormaPagto=" + (uint)Pagto.FormaPagto.Credito + " Where IdPedido=" + idPedido);
                            }

                            #endregion

                            #endregion
                        }
                        // Se for venda à prazo
                        else if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo || (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.APrazo))
                        {
                            #region Venda à prazo

                            // Se o pedido possuir sinal e o mesmo nao tiver sido recebido
                            // o pedido não poderá ser finalizado
                            if (valorEntrada > 0 && idSinal == 0)
                            {
                                var pedidoRecebido = false;

                                /* Chamado 37593. */
                                if (idPagamentoAntecipado > 0)
                                {
                                    if (valorPagamentoAntecipado == totalPedido)
                                    {
                                        pedidoRecebido = true;
                                        objPersistence.ExecuteCommand(trans, string.Format("UPDATE pedido SET ValorEntrada=NULL WHERE IdPedido={0}", idPedido));
                                    }
                                }

                                if (!pedidoRecebido)
                                    throw new Exception("Receba o sinal do pedido antes de confirmá-lo.");
                            }

                            if (idPagamentoAntecipado == 0 || valorPagamentoAntecipado != totalPedido)
                            {
                                // Calcula o valor de entrada + o valor à prazo
                                decimal valorTotalPago = valorEntrada;
                                if (tipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                                    valorTotalPago += ObraDAO.Instance.GetSaldo(trans, ObtemValorCampo<uint>(trans, "idObra", "idPedido=" + idPedido));

                                // Busca o valor das parcelas e verifica se as parcelas possuem data igual ou maior que hoje (dia da confirmação)
                                var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(trans, idPedido).ToArray();
                                foreach (ParcelasPedido p in lstParc)
                                {
                                    if (verificarParcelas && (p.Data != null && p.Data < DateTime.Now.AddDays(-1)))
                                        throw new Exception("A data de vencimento das parcelas do pedido deve ser igual ou maior que a data de hoje.");

                                    valorTotalPago += p.Valor;
                                }

                                // Verifica se o valor total do pedido bate com o valorTotalPago
                                if (Math.Round(totalPedido, 2) != Math.Round(valorTotalPago, 2))
                                    throw new Exception("O valor total do pedido não bate com o valor do pagamento do mesmo. Valor Pedido: " + Math.Round(totalPedido, 2).ToString("C") + " Valor Pago: " + Math.Round(valorTotalPago, 2).ToString("C"));

                                // Verifica se cliente possui limite disponível para realizar a compra
                                decimal limite = ClienteDAO.Instance.ObtemValorCampo<decimal>(trans, "limite", "id_Cli=" + idCliente);

                                // Determina o valor que será somado aos débitos do cliente para verificar se ficará tudo dentro do limite
                                decimal valorAConsiderar = FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite ? 0 : totalPedido - ObtemValorEntrada(trans, idPedido);

                                if (limite > 0 && ContasReceberDAO.Instance.GetDebitos(trans, idCliente, null) + valorAConsiderar > limite)
                                    LancarExceptionValidacaoPedidoFinanceiro("O cliente não possui limite disponível para realizar esta compra. Contate o setor Financeiro.",
                                        idPedido, false, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
                            }

                            #endregion
                        }

                        decimal creditoAtual = ClienteDAO.Instance.GetCredito(trans, idCliente);

                        GerarInstalacao(trans, idPedido, dataEntrega);

                        #region Gera contas a receber se for venda à prazo

                        try
                        {
                            var pagamentoAntecipado = ObtemIdPagamentoAntecipado(trans, idPedido);
                            if (tipoVenda == (int)Pedido.TipoVendaPedido.APrazo && pagamentoAntecipado.GetValueOrDefault() == 0)
                            {
                                ContasReceber conta;
                                lstParcPed = ParcelasPedidoDAO.Instance.GetByPedido(trans, idPedido).ToArray();

                                if (idFormaPagto == null)
                                    throw new Exception("A forma de pagamento não foi definida.");

                                // Exclui todas as contas a receber do pedido antes de gerar as que serão geradas abaixo
                                ContasReceberDAO.Instance.DeleteByPedido(trans, idPedido);

                                var numParc = ContasReceberDAO.Instance.ObtemNumParcPedido(trans, idPedido);

                                foreach (var p in lstParcPed)
                                {
                                    conta = new ContasReceber
                                    {
                                        IdLoja = UserInfo.GetUserInfo.IdLoja,
                                        IdCliente = idCliente,
                                        IdPedido = idPedido,
                                        DataVec = p.Data.Value,
                                        ValorVec = p.Valor,
                                        IdConta = UtilsPlanoConta.GetPlanoPrazo(idFormaPagto.Value),
                                        NumParc = numParc,
                                        IdFormaPagto = idFormaPagto.Value,
                                        IdFuncComissaoRec = idCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(idCliente) : null
                                    };
                                    numParc++;
                                    conta.IdContaR = ContasReceberDAO.Instance.Insert(trans, conta);

                                    if (conta.IdContaR == 0)
                                        throw new Exception("Conta a Receber não foi inserida.");

                                    lstIdContaRecPrazo.Add(conta.IdContaR);
                                }

                                // Atualiza o número de parcelas do pedido
                                ContasReceberDAO.Instance.AtualizaNumParcPedido(trans, idPedido, numParc - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar contas a receber.", ex));
                        }

                        #endregion

                        ped = GetElementByPrimaryKey(trans, idPedido);

                        #region Gera movimentação no caixa diário ou no caixa geral e ou na conta bancária quando for venda à vista

                        try
                        {
                            if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista || (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista))
                            {
                                retorno = UtilsFinanceiro.Receber(trans, UserInfo.GetUserInfo.IdLoja, ped, null, null, null, null, null, null, null,
                                    null, null, null, idCliente, 0, null, DateTime.Now.ToString("dd/MM/yyyy"), totalPedido, totalPago, valores, formasPagto, contasBanco, depositoNaoIdentificado, new uint[] { }, tiposCartao,
                                    null, null, 0, false, gerarCredito, creditoUtilizado, numAutConstrucard, Geral.ControleCaixaDiario,
                                    numParcCartoes, chequesPagto, descontarComissao, UtilsFinanceiro.TipoReceb.PedidoAVista);

                                if (retorno.ex != null)
                                    throw retorno.ex;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir valor no caixa diário.", ex));
                        }

                        #endregion

                        ped.ValorCreditoAoConfirmar = creditoAtual;
                        ped.CreditoGeradoConfirmar = retorno != null ? retorno.creditoGerado : 0;
                        ped.CreditoUtilizadoConfirmar = creditoUtilizado;
                        UpdateBase(trans, ped);

                        #region Gera conta recebida se o pedido for à vista

                        if (!ped.VendidoFuncionario && (ped.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista)))
                        {
                            for (int i = 0; i < valores.Length; i++)
                            {
                                if (valores[i] <= 0)
                                    continue;

                                ContasReceber contaRecSinal = new ContasReceber
                                {
                                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                                    IdPedido = ped.IdPedido,
                                    IdCliente = ped.IdCli,
                                    IdConta = UtilsPlanoConta.GetPlanoVista(formasPagto[i]),
                                    DataVec = DateTime.Now,
                                    ValorVec = valores[i],
                                    DataRec = DateTime.Now,
                                    ValorRec = valores[i],
                                    Recebida = true,
                                    UsuRec = UserInfo.GetUserInfo.CodUser,
                                    NumParc = 1,
                                    NumParcMax = 1,
                                    IdFuncComissaoRec = ped.IdCli > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(ped.IdCli) : null
                                };

                                var idContaR = ContasReceberDAO.Instance.Insert(trans, contaRecSinal);

                                lstIdContaRecVista.Add(idContaR);

                                var pagto = new PagtoContasReceber
                                {
                                    IdContaR = idContaR,
                                    IdFormaPagto = formasPagto[i],
                                    IdContaBanco =
                                        formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro &&
                                        contasBanco[i] > 0
                                            ? (uint?)contasBanco[i]
                                            : null,
                                    IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                                    IdDepositoNaoIdentificado =
                                        depositoNaoIdentificado[i] > 0 ? (uint?)depositoNaoIdentificado[i] : null,
                                    ValorPagto = valores[i],
                                    NumAutCartao = numAutCartao[i]
                                };

                                PagtoContasReceberDAO.Instance.Insert(trans, pagto);
                            }

                            if (creditoUtilizado > 0)
                            {
                                ContasReceber contaRecSinal = new ContasReceber();
                                contaRecSinal.IdLoja = UserInfo.GetUserInfo.IdLoja;
                                contaRecSinal.IdPedido = ped.IdPedido;
                                contaRecSinal.IdCliente = ped.IdCli;
                                contaRecSinal.IdConta = UtilsPlanoConta.GetPlanoVista((uint)Glass.Data.Model.Pagto.FormaPagto.Credito);
                                contaRecSinal.DataVec = DateTime.Now;
                                contaRecSinal.ValorVec = creditoUtilizado;
                                contaRecSinal.DataRec = DateTime.Now;
                                contaRecSinal.ValorRec = creditoUtilizado;
                                contaRecSinal.Recebida = true;
                                contaRecSinal.UsuRec = UserInfo.GetUserInfo.CodUser;
                                contaRecSinal.NumParc = 1;
                                contaRecSinal.NumParcMax = 1;

                                var idContaR = ContasReceberDAO.Instance.Insert(trans, contaRecSinal);

                                lstIdContaRecVista.Add(idContaR);

                                var pagto = new PagtoContasReceber();
                                pagto.IdContaR = idContaR;
                                pagto.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito;
                                pagto.ValorPagto = creditoUtilizado;

                                PagtoContasReceberDAO.Instance.Insert(trans, pagto);
                            }
                        }

                        #endregion

                        #region Atualiza custo do pedido

                        try
                        {
                            UpdateCustoPedido(trans, idPedido);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar custo do pedido.", ex));
                        }

                        #endregion

                        #region Altera situação do pedido para Confirmado

                        try
                        {
                            ped.UsuConf = UserInfo.GetUserInfo.CodUser;
                            ped.DataConf = DateTime.Now;
                            ped.Situacao = Pedido.SituacaoPedido.Confirmado;

                            if (UtilsFinanceiro.ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard, formasPagto) &&
                                ped.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && !String.IsNullOrEmpty(numAutConstrucard))
                                ped.NumAutConstrucard = numAutConstrucard;

                            if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.APrazo)
                            {
                                AlteraLiberarFinanc(trans, idPedido, true);
                                ped.LiberadoFinanc = true;
                            }

                            if (UpdateBase(trans, ped) == 0)
                                UpdateBase(trans, ped);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                        }

                        #endregion

                        #region Coloca produtos na reserva no estoque da loja

                        try
                        {
                            uint idSaidaEstoque = 0;
                            if (FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
                                idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(trans, ped.IdLoja, idPedido, null, null, false);

                            var idsProdQtde = new Dictionary<int, float>();

                            foreach (var p in lstProdPed)
                            {
                                var m2 = new List<int> { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto }.Contains(GrupoProdDAO.Instance.TipoCalculo(trans, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd));

                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(trans, (int)p.IdProd);
                                var qtdSaida = p.Qtde - p.QtdSaida;

                                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                                    qtdSaida *= p.Altura;

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                    idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM : qtdSaida);
                                else
                                    idsProdQtde[(int)p.IdProd] += m2 ? p.TotM : qtdSaida;

                                if (FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar && qtdSaida > 0)
                                {
                                    ProdutosPedidoDAO.Instance.MarcarSaida(trans, p.IdProdPed, p.Qtde - p.QtdSaida, idSaidaEstoque);

                                    // Dá baixa no estoque da loja
                                    MovEstoqueDAO.Instance.BaixaEstoquePedido(trans, p.IdProd, ped.IdLoja, idPedido, p.IdProdPed,
                                        (decimal)(m2 ? p.TotM : qtdSaida), (decimal)(m2 ? p.TotM2Calc : 0), true, null);
                                }
                            }

                            if (!ped.Producao)
                            {
                                if (!FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
                                    ProdutoLojaDAO.Instance.ColocarReserva(trans, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - ConfirmarPedido");
                                else
                                    MarcaPedidoEntregue(trans, idPedido);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao dar baixa no estoque.", ex));
                        }

                        #endregion

                        #region Gera a comissão do pedido

                        if (descontarComissao)
                            ComissaoDAO.Instance.GerarComissao(trans, Pedido.TipoComissao.Comissionado, ped.IdComissionado.Value, ped.IdPedido.ToString(), ped.DataConf.Value.ToString(), ped.DataConf.Value.ToString(), 0, null);

                        #endregion

                        // Atualiza data da última compra para hoje
                        ClienteDAO.Instance.AtualizaUltimaCompra(trans, ped.IdCli);

                        // Atualiza o total comprado
                        ClienteDAO.Instance.AtualizaTotalComprado(trans, ped.IdCli);

                        msg = "Pedido confirmado. ";

                        if (retorno != null && retorno.creditoGerado > 0)
                            msg += "Foi gerado " + retorno.creditoGerado.ToString("C") + " de crédito para o cliente. ";

                        if (retorno != null && retorno.creditoDebitado)
                            msg += "Foi utilizado " + creditoUtilizado.ToString("C") + " de crédito do cliente, restando " +
                                ClienteDAO.Instance.GetCredito(trans, idCliente).ToString("C") + " de crédito. ";

                        trans.Commit();
                        trans.Close();

                        return msg;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao confirmar pedido.", ex));
                    }
                    finally
                    {
                        FilaOperacoes.ConfirmarPedido.ProximoFila();
                    }
                }
            }
        }

        /// <summary>
        /// Marca o pedido como entregue após baixar o estoque do mesmo, se todos os produtos tiverem dado baixa e 
        /// se a empresa não trabalhar com produção de vidro ou não possuir vidros de produção no pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        public void MarcaPedidoEntregue(GDASession sessao, uint idPedido)
        {
            if (!PossuiProdutosPendentesSaida(sessao, idPedido) &&
                (!PCPConfig.ControlarProducao || ((!PossuiVidrosEstoque(sessao, idPedido) || Geral.EmpresaSomenteRevendeBox)
                && !PossuiVidrosProducao(sessao, idPedido))))
                AlteraSituacaoProducao(sessao, idPedido, Pedido.SituacaoProducaoEnum.Entregue);
        }

        #endregion

        #region Gera Instalações para o pedido

        /// <summary>
        /// Gera Instalações para o pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="dataEntrega"></param>
        public void GerarInstalacao(GDASession sessao, uint idPedido, DateTime? dataEntrega)
        {
            try
            {
                if (!Geral.ControleInstalacao)
                    return;

                int tipoEntrega = ObtemTipoEntrega(sessao, idPedido);

                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum || tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado || tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                {
                    bool comum = ContemTipo(sessao, idPedido, 1);
                    bool temperado = ContemTipo(sessao, idPedido, 2);
                    bool entrega = false;

                    // Se o tipo de entrega for esquadria, gera instalação temperado
                    if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                    {
                        comum = false;
                        temperado = true;
                    }

                    // Se o tipo de entrega for entrega, gera instalação Entrega
                    else if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                    {
                        comum = false;
                        temperado = false;
                        entrega = true;
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 4, false);
                    }

                    // Se tiver produtos temperado, gera instalação temperado
                    if (temperado)
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 2, false);

                    // Se tiver produtos comum, gera instalação comum
                    if (comum)
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 1, false);

                    // Se não tiver nenhum dos três, gera instalação pelo tipo de entrega escolhido
                    if (!comum && !temperado && !entrega)
                    {
                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                            InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 1, false);
                        else
                            InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 2, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar instalações para o pedido", ex));
            }
        }

        #endregion

        #region Confirmar Pedido Liberação

        static volatile object _confirmarLiberacaoPedidoLock = new object();

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedidoComTransacao(string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool finalizando, bool financeiro)
        {
            lock (_confirmarLiberacaoPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        ConfirmarLiberacaoPedido(transaction, idsPedidos, out idsPedidosOk, out idsPedidosErro, finalizando, financeiro);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (ValidacaoPedidoFinanceiroException f)
                    {
                        //Caso a Exceção contenha "Os demais pedidos foram confirmados com sucesso"
                        //Executa o commit dos pedidos que foram confirmados
                        if (f.Message.Contains("demais pedidos"))
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        transaction.Close();
                        throw f;
                    }
                    catch (Exception ex)
                    {
                        //Caso a Exceção contenha "Os demais pedidos foram confirmados com sucesso"
                        //Executa o commit dos pedidos que foram confirmados
                        if (ex.Message.Contains("demais pedidos"))
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        transaction.Close();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        private void ConfirmarLiberacaoPedido(GDASession session, string idsPedidos, bool finalizando)
        {
            string temp, temp1;
            ConfirmarLiberacaoPedido(session, idsPedidos, out temp, out temp1, finalizando, false);
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedido(GDASession sessao, string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool financeiro)
        {
            ConfirmarLiberacaoPedido(sessao, idsPedidos, out idsPedidosOk, out idsPedidosErro, false, financeiro);
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedido(GDASession sessao, string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool finalizando, bool financeiro)
        {
            try
            {
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;
                bool confApenasMaoDeObra = false;

                if (!finalizando)
                {
                    if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0)
                        if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ConfirmarPedidoLiberacao))
                        {
                            // Verifica se o usuário pode imprimir pedidos de mão de obra
                            if (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra))
                                confApenasMaoDeObra = true;
                            else
                                throw new Exception("Apenas o Gerente pode confirmar pedidos.");
                        }
                }

                Pedido[] pedidos = GetByString(sessao, idsPedidos);

                string idsClientes = ",";
                foreach (Pedido p in pedidos)
                {
                    if (!finalizando)
                    {
                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (p.Situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                            throw new Exception("Pedido '" + p.IdPedido + "' já confirmado.");
                        else if (p.Situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido '" + p.IdPedido + "' já liberado.");
                        else if (p.Situacao != Pedido.SituacaoPedido.Conferido && p.Situacao != Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro)
                            throw new Exception("O pedido '" + p.IdPedido + "' ainda não foi conferido, portanto não pode ser confirmado.");

                        if (ProdutosPedidoDAO.Instance.GetCount(sessao, p.IdPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido '" + p.IdPedido + "' não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");

                        if (!p.MaoDeObra && confApenasMaoDeObra)
                            throw new Exception("Você pode confirmar apenas pedidos de mão de obra.");
                    }

                    // Salva o id dos clientes para a consulta do limite
                    idsClientes += !idsClientes.Contains("," + p.IdCli + ",") ? p.IdCli + "," : "";
                }

                var consideraPedidoConferido = FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite;
                var naoVerificaPedidoAVista = PedidoConfig.EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite;

                // Verifica se a empresa considera pedidos conferidos (todos ou apenas à vista) no limite do cliente
                if (!consideraPedidoConferido || naoVerificaPedidoAVista)
                {
                    foreach (var id in idsClientes.TrimStart(',').TrimEnd(',').Split(','))
                    {
                        // Se a empresa não considera pedidos conferidos no limite, soma o total de todos os pedidos sendo confirmados,
                        // mas caso a empresa apenas não verifique o limite ao finalizar pedido à vista, puxa o total de todos os pedidos
                        // sendo confirmados que forem à vista e que não foi recebido antecipado
                        var whereTotal = !consideraPedidoConferido ?
                            "idCli=" + id + " And idPedido In (" + idsPedidos + ")" :
                            "idCli=" + id + " And idPedido In (" + idsPedidos + ") And (Coalesce(idPagamentoAntecipado, 0)=0 Or Coalesce(valorPagamentoAntecipado, 0)=0)  And tipoVenda=" + (int)Pedido.TipoVendaPedido.AVista;

                        // Quando a empresa considera pedido conferido no limite, os débitos do pedido já são buscados no método GetDebitos, por isso foi removido dos totais e do pagoAntecipado.
                        // Recupera o valor de todos os pedidos do cliente que estão sendo confirmados
                        var totaisPedidos = PedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Sum(total)", !consideraPedidoConferido ? whereTotal : whereTotal + " And Situacao != " + (int)Pedido.SituacaoPedido.Conferido);

                        // Total pago antecipado
                        var totalPagoAntecipado = PedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Sum(Coalesce(valorPagamentoAntecipado,0))",
                            !consideraPedidoConferido ? whereTotal : whereTotal + " And Situacao != " + (int)Pedido.SituacaoPedido.Conferido);

                        // Recupera os débitos do cliente
                        var debitos = ContasReceberDAO.Instance.GetDebitos(sessao, id.StrParaUint(), null);

                        // Recupera o limite do cliente
                        var limite = ClienteDAO.Instance.ObtemValorCampo<decimal>(sessao, "limite", "id_cli=" + id, null);

                        // Verifica se o total dos pedidos mais o total de débitos ultrapassa o limite do cliente, se sim é lançada uma exceção
                        if (limite > 0 && (totaisPedidos + debitos - totalPagoAntecipado) > limite &&
                            /* Chamado 45457. */
                            totaisPedidos - totalPagoAntecipado > 0)
                        {
                            // Passa somente os pedidos do cliente desta iteração
                            var idsPedidoCliente = string.Join(",", ExecuteMultipleScalar<string>(sessao, "Select Cast(idPedido as char) From pedido Where " + whereTotal).ToArray());

                            if (!string.IsNullOrWhiteSpace(idsPedidoCliente) && !idsPedidoCliente.Contains(","))
                            {
                                //Cria um log no pedido caso ocorra problema com o limite do cliente
                                // Feito pois ao finalizar pedido de revenda não é lancada uma exceção, sendo assim será salvo um log no pedido.
                                var logConfirmacao = new LogAlteracao();
                                logConfirmacao.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                                logConfirmacao.IdRegistroAlt = idsPedidoCliente.StrParaInt();
                                logConfirmacao.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(sessao, LogAlteracao.TabelaAlteracao.Pedido, idsPedidoCliente.StrParaInt());
                                logConfirmacao.Campo = "Falha ao finalizar/Confirmar Pedido";
                                logConfirmacao.DataAlt = DateTime.Now;
                                logConfirmacao.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                                logConfirmacao.ValorAnterior = null;
                                logConfirmacao.ValorAtual = @"O cliente '" + id + " - " + ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "nome", "id_cli=" + id) +
                                    " não possui limite suficiente. " + "\nLimite disponível: R$ " + limite + "\nLimite necessário: R$ " + (totaisPedidos + debitos);
                                logConfirmacao.Referencia = LogAlteracao.GetReferencia(sessao, (int)LogAlteracao.TabelaAlteracao.Pedido, idsPedidoCliente.StrParaUint());

                                //Salva o log no pedido
                                LogAlteracaoDAO.Instance.Insert(sessao, logConfirmacao);
                            }
                            LancarExceptionValidacaoPedidoFinanceiro("O cliente '" + id + " - " + ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "nome", "id_cli=" + id) +
                                " não possui limite suficiente. " + "\nLimite disponível: R$ " + limite + "\nLimite necessário: R$ " + (totaisPedidos + debitos),
                                !string.IsNullOrWhiteSpace(idsPedidoCliente) && !idsPedidoCliente.Contains(",") ? idsPedidoCliente.StrParaUint() : 0, false, idsPedidoCliente,
                                ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
                        }
                    }
                }

                List<uint> idPedidoOk = new List<uint>(), idPedidoErro = new List<uint>();
                var mensagem = "";
                var situacaoCliente = ClienteDAO.Instance.GetSituacao(pedidos[0].IdCli);

                // Se, bloquear confirmação de pedido com sinal à receber.
                if (PedidoConfig.ImpedirConfirmacaoPedidoPagamento && idPedidoOk.Count == 0 &&
                    !VerificaSinalPagamentoReceber(sessao, pedidos, out mensagem, out idPedidoOk, out idPedidoErro))
                {
                    idsPedidosOk = "";
                    idsPedidosErro = idsPedidos;

                    LancarExceptionValidacaoPedidoFinanceiro(mensagem, !string.IsNullOrWhiteSpace(idsPedidos) && !idsPedidos.Contains(",") ? idsPedidos.StrParaUint() : 0, false, idsPedidos,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                    // Permite que os pedidos sejam liberados pelo funcionário do Financeiro
                    idPedidoOk.Clear();
                    idPedidoOk.AddRange(idPedidoErro);
                    idPedidoErro.Clear();
                }
                // Se, pedido gerado pelo Parceiro e cliente Inativo ou Bloqueado.
                else if (FinanceiroConfig.ClienteInativoBloqueadoEmitirPedidoComConfirmacaoPeloFinanceiro &&
                    pedidos[0].GeradoParceiro && (situacaoCliente == (int)SituacaoCliente.Inativo || situacaoCliente == (int)SituacaoCliente.Bloqueado))
                {
                    idsPedidosOk = "";
                    idsPedidosErro = idsPedidos;
                    mensagem = "Pedido emitido no e-commerce por cliente inativo ou bloqueado";
                    LancarExceptionValidacaoPedidoFinanceiro(mensagem, !string.IsNullOrWhiteSpace(idsPedidos) && !idsPedidos.Contains(",") ? idsPedidos.StrParaUint() : 0, false, idsPedidos,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                    // Permite que os pedidos sejam liberados pelo funcionário do Financeiro
                    idPedidoOk.Clear();
                    idPedidoOk.AddRange(idPedidoErro);
                    idPedidoErro.Clear();
                }

                if (idPedidoErro.Count > 0)
                {
                    idsPedidosOk = string.Join(",", Array.ConvertAll<uint, string>(idPedidoOk.ToArray(), x => x.ToString()));
                    idsPedidosErro = string.Join(",", Array.ConvertAll<uint, string>(idPedidoErro.ToArray(), x => x.ToString()));

                    pedidos = GetByString(sessao, idsPedidosOk);
                }
                else
                {
                    idsPedidosOk = idsPedidos;
                    idsPedidosErro = "";
                }

                var idsProdQtde = new Dictionary<int, float>();

                // Se houver alteração neste método alterar também na confirmação de garantia/reposição
                #region Coloca produtos na reserva no estoque da loja

                var produtosPedidosEstoque = new Dictionary<uint, Dictionary<uint, float>>();

                try
                {
                    foreach (var idProdPed in ProdutosPedidoDAO.Instance.ObtemIdsPedidoExcetoProducao(sessao, idsPedidosOk))
                    {
                        var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(sessao, idProdPed);
                        var totM = ProdutosPedidoDAO.Instance.ObtemTotM(sessao, idProdPed);
                        var qtde = ProdutosPedidoDAO.Instance.ObtemQtde(sessao, idProdPed);
                        var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(sessao, idProdPed);
                        var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
                        var idGrupo = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd);
                        var idSubGrupo = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd);
                        var alturaProd = ProdutosPedidoDAO.Instance.ObterAltura(sessao, (uint)idProdPed);

                        float qtdProd = 0;

                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)idProd);

                        if (tipoCalc == (int)TipoCalculoGrupoProd.M2 || tipoCalc == (int)TipoCalculoGrupoProd.M2Direto)
                            qtdProd += totM;
                        else if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                            qtdProd += qtde * alturaProd;
                        else
                            qtdProd += qtde;

                        if (!produtosPedidosEstoque.ContainsKey(idPedido))
                        {
                            produtosPedidosEstoque.Add(idPedido, new Dictionary<uint, float> { { idProd, qtdProd } });
                        }
                        else if (!produtosPedidosEstoque[idPedido].ContainsKey(idProd))
                        {
                            produtosPedidosEstoque[idPedido].Add(idProd, qtdProd);
                        }
                        else
                        {
                            produtosPedidosEstoque[idPedido][idProd] += qtdProd;
                        }

                        if (idGrupo == 0 || idSubGrupo.GetValueOrDefault() == 0)
                        {
                            var descricaoProd = ProdutoDAO.Instance.ObtemDescricao(sessao, (int)idProd);
                            throw new Exception(string.Format("Verifique o cadastro do produto {0} sem {1}", descricaoProd, idGrupo == 0 ? "Grupo" : "Subgrupo"));
                        }

                        //Verifica se o produto possui estoque para inserir na reserva 
                        if (GrupoProdDAO.Instance.BloquearEstoque(sessao, (int)idGrupo, (int)idSubGrupo))
                        {
                            var estoque = ProdutoLojaDAO.Instance.GetEstoque(sessao, idLoja, idProd, null, false, false, true);

                            if (estoque < produtosPedidosEstoque[idPedido][idProd])
                            {
                                var descricaoProd = ProdutoDAO.Instance.ObtemDescricao((int)idProd);
                                throw new Exception("O produto " + descricaoProd + " possui apenas " + estoque + " em estoque.");
                            }
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!idsProdQtde.ContainsKey((int)idProd))
                            idsProdQtde.Add((int)idProd, produtosPedidosEstoque[idPedido][idProd]);
                        else
                            idsProdQtde[(int)idProd] += produtosPedidosEstoque[idPedido][idProd];
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Falha ao colocar produtos na reserva. ", e);
                }

                #endregion

                string sql;

                #region Altera situação do pedido para Confirmado Liberacao e atualiza o saldo da obra

                try
                {
                    /* Chamado 37030. */
                    foreach (var id in idsPedidosOk.Split(','))
                    {
                        // Recupera o pedido
                        var ped = pedidos.Where(f => f.IdPedido == id.StrParaUint()).FirstOrDefault();

                        // Atualiza para confirmado PCP 
                        AlteraSituacao(sessao, id.StrParaUint(), Pedido.SituacaoPedido.ConfirmadoLiberacao);

                        // Chamado 59179: Atualiza o saldo da obra (Deve ser feito neste momento)
                        AtualizaSaldoObra(sessao, ped.IdPedido, null, ped.IdObra, ped.Total, ped.Total, true);

                        /* Chamado 53136. */
                        LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElementByPrimaryKey(sessao, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
                    }

                    sql = string.Format("UPDATE pedido SET UsuConf={0}, DataConf=NOW(), TaxaFastDelivery=IF(FastDelivery, {1}, NULL) WHERE IdPedido IN ({2})",
                        "{0}", PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery.ToString().Replace(",", "."), "{1}");

                    if (financeiro)
                    {
                        foreach (var idPedido in idsPedidos.Split(','))
                        {
                            // Salva o usuário atual que estiver confirmando o pedido (Chamado 12100)
                            var usuConf = UserInfo.GetUserInfo.CodUser;//ObtemValorCampo<uint>("idFuncConfirmarFinanc", "idPedido=" + idPedido);
                            objPersistence.ExecuteCommand(sessao, string.Format(sql, usuConf, idPedido));
                        }
                    }
                    else
                    {
                        // Confirma somente os pedidos que estiverem ok
                        objPersistence.ExecuteCommand(sessao, string.Format(sql, UserInfo.GetUserInfo.CodUser, idsPedidosOk));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação dos pedidos.", ex));
                }

                #endregion

                // Coloca produtos na reserva do estoque da loja (Deve ser feito depois de alterar a situação do pedido)
                if (idsProdQtde.Count > 0)
                {
                    var idsLojaReserva = new List<int>();

                    foreach (var pedido in pedidos)
                        if (!idsLojaReserva.Contains((int)pedido.IdLoja))
                            idsLojaReserva.Add((int)pedido.IdLoja);

                    foreach (var idLojaReserva in idsLojaReserva)
                        ProdutoLojaDAO.Instance.ColocarReserva(sessao, idLojaReserva, idsProdQtde, null, null, null, null, null,
                            string.Join(",", pedidos.Select(f => f.IdPedido).ToList()), null, "PedidoDAO - ConfirmarLiberacaoPedido");
                }

                if (!string.IsNullOrEmpty(mensagem))
                    LancarExceptionValidacaoPedidoFinanceiro(mensagem + "\n\nOs demais pedidos foram confirmados com sucesso.",
                        !string.IsNullOrWhiteSpace(idsPedidosErro) && !idsPedidosErro.Contains(",") ? idsPedidosErro.StrParaUint() : 0, false, idsPedidosErro,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
            }
            catch (ValidacaoPedidoFinanceiroException f)
            {
                throw f;
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException(string.Format("PedidoDAO - ConfirmarLiberacaoPedido. Pedidos: {0}", idsPedidos), ex);
                throw ex;
            }
        }

        #endregion

        #region Reabrir Pedido

        public bool PodeReabrir(uint idPedido)
        {
            return PodeReabrir(null, idPedido);
        }

        public bool PodeReabrir(GDASession session, uint idPedido)
        {
            var valorPagtoAntecipado = ObtemValorCampo<decimal>(session, "valorPagamentoAntecipado", "idPedido=" + idPedido);
            var situacao = ObtemSituacao(session, idPedido);
            var geradoParceiro = IsGeradoParceiro(session, idPedido);
            var idCli = ObtemIdCliente(session, idPedido);
            var temEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);
            var possuiObraAssociada = GetIdObra(session, idPedido) > 0;
            var tipoPedido = GetTipoPedido(session, idPedido);
            var importado = IsPedidoImportado(session, idPedido);
            var recebeuSinal = ObtemValorCampo<bool>(session, "IdSinal > 0", "idPedido=" + idPedido);

            return PodeReabrir(session, idPedido, valorPagtoAntecipado, situacao, geradoParceiro, idCli, temEspelho, possuiObraAssociada, tipoPedido, importado, recebeuSinal);
        }

        public bool PodeReabrir(uint idPedido, decimal valorPagtoAntecipado, Pedido.SituacaoPedido situacao,
            bool geradoParceiro, uint idCli, bool temEspelho, bool possuiObraAssociada, Pedido.TipoPedidoEnum tipoPedido,
            bool importado, bool recebeuSinal)
        {
            return PodeReabrir(null, idPedido, valorPagtoAntecipado, situacao, geradoParceiro, idCli, temEspelho, possuiObraAssociada, tipoPedido, importado, recebeuSinal);
        }

        public bool PodeReabrir(GDASession session, uint idPedido, decimal valorPagtoAntecipado, Pedido.SituacaoPedido situacao, bool geradoParceiro,
            uint idCli, bool temEspelho, bool possuiObraAssociada, Pedido.TipoPedidoEnum tipoPedido, bool importado, bool recebeuSinal)
        {
            // Define que apenas administrador pode reabrir pedido
            var apenasAdminReabrePedido = PCPConfig.ReabrirPCPSomenteAdmin;
            // Define que todos usuários podem reabrir pedido confirmado PCP, exceto o vendedor (a menos que seja pedido de revenda)
            var apenasVendedorNaoReabrePedidoConfirmadoPCP = PedidoConfig.ReabrirPedidoConfirmadoPCPTodosMenosVendedor;

            /* Chamado 52903. */
            if (geradoParceiro && !PedidoConfig.PodeReabrirPedidoGeradoParceiro && idCli != UserInfo.GetUserInfo.IdCliente)
                return false;

            // Não deixa reabrir se recebeu sinal
            if (PedidoConfig.ReabrirPedidoNaoPermitidoComSinalRecebido && recebeuSinal)
                return false;

            return (((valorPagtoAntecipado == 0 || PedidoConfig.ReabrirPedidoComPagamentoAntecipado ||
                /* Chamado 16956 e 17824. */
                (possuiObraAssociada && ObraDAO.Instance.ObtemSituacao(session, GetIdObra(session, idPedido).Value) != Obra.SituacaoObra.Finalizada)) &&
                ((situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao && !temEspelho) || situacao == Pedido.SituacaoPedido.Conferido))
                && (!apenasAdminReabrePedido || UserInfo.GetUserInfo.IsAdministrador)
                && (!apenasVendedorNaoReabrePedidoConfirmadoPCP || UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor ||
                    situacao == Pedido.SituacaoPedido.Conferido || tipoPedido == Pedido.TipoPedidoEnum.Revenda)
                && (!OrdemCargaConfig.UsarControleOrdemCarga || !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido)))
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && importado)
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && PedidoExportacaoDAO.Instance.GetSituacaoExportacao(session, idPedido) == PedidoExportacao.SituacaoExportacaoEnum.Exportado)
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && Instance.PossuiImpressaoBox(session, idPedido));
        }

        private static object _reabrirPedido = new object();

        /// <summary>
        /// Reabre um pedido.
        /// </summary>
        public void Reabrir(uint idPedido)
        {
            lock (_reabrirPedido)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // #69907 - Altera a OBS do pedido para bloquear qualquer outra alteração na tabela fora dessa transação
                        var obsPedido = ObtemObs(transaction, idPedido);
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET obs='Reabrindo pedido' WHERE IdPedido={0}", idPedido));

                        if (!PodeReabrir(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido.");

                        /* Chamado 33940. */
                        if (objPersistence.ExecuteSqlQueryCount(transaction,
                            string.Format("SELECT COUNT(*) FROM produto_pedido_producao WHERE IdPedidoExpedicao={0}", idPedido)) > 0)
                            throw new Exception("Este pedido não pode ser reaberto porque uma ou mais peças foram entregues, volte a situação delas na produção e tente novamente.");

                        if (TemVolume(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido, pois o mesmo possui volumes gerados.");

                        var nova = Pedido.SituacaoPedido.Ativo;
                        if (PedidoConferenciaDAO.Instance.IsInConferencia(transaction, idPedido) || Instance.ObtemIdSinal(transaction, idPedido) > 0)
                            nova = Pedido.SituacaoPedido.AtivoConferencia;

                        var pedido = GetElementByPrimaryKey(transaction, idPedido);

                        if (pedido.GerarPedidoProducaoCorte && Instance.PedidoProducaoCorteAtivo(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido, pois o mesmo possui pedidos de produção em andamento. Cancele-os para reabrir este pedido.");

                        var situacao = ObtemSituacao(transaction, idPedido);
                        Instance.AlteraSituacao(transaction, idPedido, nova);

                        objPersistence.ExecuteCommand(transaction, "update pedido set dataFin=null, usuFin=null where idPedido=" + idPedido);

                        //Verifica se o  ValorPagamentoAntecipado é proveniente de uma Obra, então zera.
                        if (pedido.IdObra.GetValueOrDefault() > 0 && pedido.IdPagamentoAntecipado.GetValueOrDefault() == 0)
                            objPersistence.ExecuteCommand(transaction, "UPDATE pedido SET ValorPagamentoAntecipado=null WHERE IdPedido=" + idPedido);

                        var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, idPedido);

                        //Movimenta o estoque da materia-prima
                        foreach (var p in produtos)
                        {
                            if (ProdutoDAO.Instance.IsVidro(transaction, (int)p.IdProd))
                                MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(transaction, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                        }

                        // Tira os produtos da reserva, se o pedido estivesse confirmado
                        if (situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                        {
                            try
                            {
                                var login = UserInfo.GetUserInfo;

                                var idsProdQtde = new Dictionary<int, float>();

                                // Pedido de produção não deve tirar nem colocar na reserva
                                if (pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.Producao)
                                {
                                    foreach (var pp in ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, idPedido))
                                    {
                                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)pp.IdGrupoProd,
                                            (int)pp.IdSubgrupoProd);
                                        var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;

                                        var qtdEstornoEstoque = pp.Qtde;

                                        if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                                        {
                                            var altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(transaction, "altura",
                                                "idProdPed=" + pp.IdProdPed);
                                            qtdEstornoEstoque = pp.Qtde * altura;
                                        }

                                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                        if (!idsProdQtde.ContainsKey((int)pp.IdProd))
                                            idsProdQtde.Add((int)pp.IdProd, m2 ? pp.TotM : qtdEstornoEstoque);
                                        else
                                            idsProdQtde[(int)pp.IdProd] += m2 ? pp.TotM : qtdEstornoEstoque;
                                    }
                                }

                                /* Chamado 17824. */
                                // Zera o campo pagamento antecipado
                                //objPersistence.ExecuteCommand("update pedido set valorPagamentoAntecipado=0, dataConf=null, usuConf=null where idPedido=" + idPedido);
                                objPersistence.ExecuteCommand(transaction,
                                    "update pedido set dataConf=null, usuConf=null where idPedido=" + idPedido);

                                var idObra = ObtemValorCampo<uint>(transaction, "idObra", "idPedido=" + idPedido);
                                if (idObra > 0)
                                    ObraDAO.Instance.AtualizaSaldo(transaction, idObra, false);

                                ProdutoLojaDAO.Instance.TirarReserva(transaction, (int)ObtemIdLoja(transaction, idPedido), idsProdQtde,
                                    null, null, null, null, (int)idPedido, null, null, "PedidoDAO - Reabrir");
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Falha ao tirar produtos na reserva.", ex);
                            }
                        }

                        LogAlteracaoDAO.Instance.LogPedido(transaction, pedido, GetElementByPrimaryKey(transaction, pedido.IdPedido),
                            LogAlteracaoDAO.SequenciaObjeto.Atual);

                        // #69907 - Ao final da transação volta a situação original do pedido
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET obs=?obs WHERE IdPedido={0}", idPedido), new GDAParameter("?obs", obsPedido));

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

        #region Confirmar Pedido Obra

        /// <summary>
        /// Confirma pedido de obra
        /// </summary>
        /// <param name="idPedido"></param>
        public void ConfirmarPedidoObra(uint idPedido, bool obraPagouPedidoInteiro)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;

                    bool cxDiario = false;

                    // Se a empresa tiver permissão para trabalhar com caixa diário
                    if (Geral.ControleCaixaDiario)
                    {
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                            throw new Exception("Apenas o Caixa pode confirmar pedidos.");

                        cxDiario = true;
                    }
                    else
                    {
                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                            throw new Exception("Você não tem permissão para confirmar pedidos.");
                    }

                    Pedido pedido = GetElementByPrimaryKey(transaction, idPedido);

                    if (pedido.IdObra == null || pedido.IdObra == 0)
                        throw new Exception("Associe uma obra à este pedido antes de confirmá-lo.");

                    //Obra obra = ObraDAO.Instance.GetElementByPrimaryKey(pedido.IdObra.Value);

                    if (obraPagouPedidoInteiro)
                    {
                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (pedido.Situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido já liberado.");

                        else if (pedido.Situacao != Pedido.SituacaoPedido.Conferido)
                            throw new Exception("O pedido ainda não foi conferido, portanto não pode ser confirmado.");

                        else if (ProdutosPedidoDAO.Instance.GetCount(idPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");
                    }

                    /*
                    // Verifica se o saldo da obra é suficiente para confirmar este pedido
                    if (pedido.Total > obra.Saldo)
                        throw new Exception("O saldo da obra não é suficiente para cobrir o valor do pedido.");
                    */

                    #region Altera situação do pedido para Confirmado

                    try
                    {
                        pedido.UsuConf = UserInfo.GetUserInfo.CodUser;
                        pedido.DataConf = DateTime.Now;
                        pedido.Situacao = Pedido.SituacaoPedido.Confirmado;

                        if (UpdateBase(transaction, pedido) == 0)
                            UpdateBase(transaction, pedido);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                    }

                    #endregion

                    #region Gera Instalações para o pedido

                    try
                    {
                        int tipoEntrega = ObtemTipoEntrega(transaction, idPedido);

                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum || tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                            tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado || tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria ||
                            tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                        {
                            bool comum = ContemTipo(transaction, idPedido, 1);
                            bool temperado = ContemTipo(transaction, idPedido, 2);
                            bool entrega = false;

                            // Se o tipo de entrega for esquadria, gera instalação temperado
                            if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                            {
                                comum = false;
                                temperado = true;
                            }

                            // Se o tipo de entrega for entrega, gera instalação Entrega
                            else if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                            {
                                comum = false;
                                temperado = false;
                                entrega = true;

                                InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 4, false);
                            }

                            uint idInstTemperado = 0;
                            uint idInstComum = 0;

                            // Se tiver produtos temperado, gera instalação temperado
                            if (temperado)
                                idInstTemperado = InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 2, false);

                            // Se tiver produtos comum, gera instalação comum
                            if (comum)
                                idInstComum = InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 1, false);

                            // Se não tiver nenhum dos três, gera instalação pelo tipo de entrega escolhido
                            if (!comum && !temperado && !entrega)
                            {
                                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                                    InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 1, false);
                                else
                                    InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 2, false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar instalações para o pedido", ex));
                    }

                    #endregion

                    // Atualiza o saldo da obra
                    ObraDAO.Instance.AtualizaSaldo(transaction, pedido.IdObra.Value, cxDiario);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                }
            }
        }

        #endregion

        #region Verifica se o pedido possui vidro temperado/comum

        /// <summary>
        /// Verifica se o pedido possui vidro comum ou vidro temperado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipo">1-Comum, 2-Temperado</param>
        private bool ContemTipo(GDASession sessao, uint idPedido, int tipo)
        {
            var sql = @"Select Count(*) From produtos_pedido pp 
                Left Join produto p On (pp.IdProd=p.IdProd) 
                Where idPedido=" + idPedido;

            var subgruposMarcadosFiltro = Glass.Data.DAL.GrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(sessao, 0);

            if (!String.IsNullOrEmpty(subgruposMarcadosFiltro))
            {
                if (tipo == 1)
                    sql += @" And (p.IdGrupoProd=1 And p.IdSubgrupoProd Not In (" + subgruposMarcadosFiltro + @"))";
                else if (tipo == 2)
                    sql += @" And (p.IdGrupoProd=1 And p.IdSubgrupoProd In (" + subgruposMarcadosFiltro + "))";
            }
            else
                return false;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null) > 0;
        }

        #endregion

        #region Verifica se o pedido está em conferência

        /// <summary>
        /// Verifica se o pedido está em conferência
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool EstaEmConferencia(uint idPedido)
        {
            string sql = "Select Count(*) From pedido_conferencia where idPedido=" + idPedido +
                " And Situacao<>" + (int)PedidoConferencia.SituacaoConferencia.Finalizada;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion

        #region Confirma pedido de garantia e de reposição

        static volatile object _confirmaGarantiaReposicaoLock = new object();

        /// <summary>
        /// Confirma pedidos de garantia e de reposição
        /// </summary>
        public void ConfirmaGarantiaReposicaoComTransacao(uint idPedido, bool financeiro)
        {
            lock (_confirmaGarantiaReposicaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        ConfirmaGarantiaReposicao(transaction, idPedido, financeiro);

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
        /// Confirma pedidos de garantia e de reposição
        /// </summary>
        public void ConfirmaGarantiaReposicao(GDASession session, uint idPedido, bool financeiro)
        {
            var tipoEntrega = ObtemTipoEntrega(session, idPedido);
            DateTime? dataEntrega = ObtemDataEntrega(session, idPedido);
            var pedidoAtual = GetElementByPrimaryKey(session, idPedido);

            if (PedidoConfig.LiberarPedido)
            {
                // Se for liberação e o pedido não possuir produtos do grupo vidro calculado por m², muda para Confirmado
                if (PedidoConfig.LiberarPedido && !ProdutosPedidoDAO.Instance.PossuiVidroCalcM2(session, idPedido))
                {
                    AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);

                    // TODO: Colocar função para recalcular a reserva
                }
                else
                {
                    AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.Conferido);

                    // Salva a data e usuário de finalização
                    var usuConf = UserInfo.GetUserInfo.CodUser;

                    if (financeiro)
                        usuConf = ObtemValorCampo<uint>(session, "idFuncFinalizarFinanc", "idPedido=" + idPedido);

                    objPersistence.ExecuteCommand(session, "update pedido set dataFin=?data, usuFin=?usu where idPedido=" + idPedido,
                        new GDAParameter("?data", DateTime.Now), new GDAParameter("?usu", usuConf));
                }
            }
            else
            {
                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                {
                    var comum = ContemTipo(session, idPedido, 1);
                    var temperado = ContemTipo(session, idPedido, 2);

                    // Se for esquadria de alumínio, gera instalação temperado
                    if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                    {
                        comum = false;
                        temperado = true;
                    }

                    // Se tiver produtos comum, gera instalação comum
                    if (comum)
                        InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 1, false);

                    // Se tiver produtos temperado, gera instalação temperado
                    if (temperado)
                        InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 2, false);

                    // Se não tiver nenhum dos dois, gera instalação pelo tipo de entrega escolhido
                    if (!comum && !temperado)
                    {
                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                            InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 1, false);
                        else
                            InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 2, false);
                    }
                }

                AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.Confirmado);

                // Confirma/Libera o pedido
                objPersistence.ExecuteCommand(session, string.Format("Update pedido Set UsuConf=UsuCad, DataConf=?data Where idPedido={0}",
                    idPedido), new GDAParameter("?data", DateTime.Now));
            }

            #region Coloca produtos na reserva no estoque da loja

            /* Chamado 39942. */
            var idsProdQtde = new Dictionary<int, float>();

            try
            {
                if (GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Producao)
                {
                    foreach (var prodPed in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                    {
                        var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(session, prodPed.IdProdPed);
                        var totM = ProdutosPedidoDAO.Instance.ObtemTotM(session, prodPed.IdProdPed);
                        var qtde = ProdutosPedidoDAO.Instance.ObtemQtde(session, prodPed.IdProdPed);

                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(session, (int)idProd);
                        var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;

                        var qtdEstornoEstoque = qtde;

                        if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                        {
                            var altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdPed=" + prodPed.IdProdPed);
                            qtdEstornoEstoque = qtde * altura;
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!idsProdQtde.ContainsKey((int)idProd))
                            idsProdQtde.Add((int)idProd, m2 ? totM : qtdEstornoEstoque);
                        else
                            idsProdQtde[(int)idProd] += m2 ? totM : qtdEstornoEstoque;
                    }

                    // Coloca produtos na reserva do estoque da loja (Deve ser feito depois de alterar a situação do pedido)
                    if (idsProdQtde.Count > 0)
                        ProdutoLojaDAO.Instance.ColocarReserva(session, (int)ObtemIdLoja(session, idPedido), idsProdQtde, null, null, null,
                            null, (int)idPedido, null, null, "PedidoDAO - ConfirmarGarantiaReposicao");
                }
            }
            catch
            {
                throw new Exception("Falha ao colocar produtos na reserva.");
            }

            #endregion

            LogAlteracaoDAO.Instance.LogPedido(session, pedidoAtual, GetElementByPrimaryKey(session, idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Altera situação do pedido

        public int AlteraSituacao(uint idPedido, Pedido.SituacaoPedido situacao)
        {
            return AlteraSituacao(null, idPedido, situacao);
        }

        public int AlteraSituacao(GDASession sessao, uint idPedido, Pedido.SituacaoPedido situacao)
        {
            return objPersistence.ExecuteCommand(sessao, "Update pedido Set Situacao=" + (int)situacao + " Where idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="situacaoProducao"></param>
        /// <returns></returns>
        public int AlteraSituacaoProducao(uint idPedido, Pedido.SituacaoProducaoEnum situacaoProducao)
        {
            return AlteraSituacaoProducao(null, idPedido, situacaoProducao);
        }

        public int AlteraSituacaoProducao(GDASession sessao, uint idPedido, Pedido.SituacaoProducaoEnum situacaoProducao)
        {
            return objPersistence.ExecuteCommand(sessao, "Update pedido Set SituacaoProducao=" + (int)situacaoProducao + " Where idPedido=" + idPedido);
        }

        #endregion

        #region Altera o Status de Liberação para Entrega - Financeiro

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="liberar"></param>
        /// <returns></returns>
        public int AlteraLiberarFinanc(uint idPedido, bool liberar)
        {
            return AlteraLiberarFinanc(null, idPedido, liberar);
        }

        public int AlteraLiberarFinanc(GDASession sessao, uint idPedido, bool liberar)
        {
            Pedido ped = GetElementByPrimaryKey(idPedido);
            objPersistence.ExecuteCommand(sessao, "Update pedido set liberadoFinanc=" + liberar + " Where idPedido=" + idPedido);

            LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElementByPrimaryKey(idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

            return 1;
        }

        #endregion

        #region Cancelar Pedido

        static volatile object _cancelarPedidoLock = new object();

        /// <summary>
        /// Cancela o pedido, apagando contas a pagar, contas a receber,
        /// estornando produtos que foi dado baixa.
        /// </summary>
        public void CancelarPedidoComTransacao(uint idPedido, string motivoCancelamento, bool gerarCredito, bool gerarDebitoComissao,
            DateTime dataEstornoBanco)
        {
            lock (_cancelarPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CancelarPedido(transaction, idPedido, motivoCancelamento, gerarCredito, gerarDebitoComissao, dataEstornoBanco);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar pedido.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Cancela o pedido, apagando contas a pagar, contas a receber,
        /// estornando produtos que foi dado baixa.
        /// </summary>
        public void CancelarPedido(GDASession session, uint idPedido, string motivoCancelamento, bool gerarCredito,
            bool gerarDebitoComissao, DateTime dataEstornoBanco)
        {
            Pedido ped = GetElementByPrimaryKey(session, idPedido);
            var gerarCreditoObra = false;

            // Verifica se o caixa diário não foi fechado no dia anterior
            if (gerarCredito && Geral.ControleCaixaDiario &&
                !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(session, ped.IdLoja))
                throw new Exception("O caixa não foi fechado no último dia de trabalho.");

            // Verifica se o pedido já foi cancelado
            if (ped.Situacao == Pedido.SituacaoPedido.Cancelado)
                throw new Exception("Este pedido já foi cancelado.");

            /* Chamado 63742. */
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.CancelarPedido))
                throw new Exception("O usuário logado não possui permissão para cancelar pedidos.");

            // Verifica se há trocas/devoluções em aberto para esse pedido
            if (TrocaDevolucaoDAO.Instance.ExistsByPedido(session, idPedido))
                throw new Exception("Cancele as trocas/devoluções relacionadas a esse pedido antes de cancelá-lo.");

            // Verifica se existe um acerto associado à este pedido
            if (!gerarCredito && AcertoDAO.Instance.ExisteAcerto(session, idPedido))
                throw new Exception(
                    "Existe um acerto associado à este pedido, cancele-o antes de cancelar o pedido.");

            // Verifica se alguma parcela deste pedido já foi recebida
            if (!gerarCredito && ContasReceberDAO.Instance.ExisteRecebida(session, idPedido, false) &&
                (ped.TipoVenda != (int)Pedido.TipoVendaPedido.AVista || PedidoConfig.LiberarPedido))
                throw new Exception(
                    "Existe uma conta recebida associada à este pedido, cancele-a antes de cancelar o pedido.");

            // Verifica se há liberações ativas para este pedido
            if (!gerarCredito && !LiberarPedidoDAO.Instance.PodeCancelarPedido(session, idPedido))
                throw new Exception(
                    "Ainda há liberações ativas para esse pedido. Cancele-as antes de cancelar o pedido.");

            // Verifica se o caixa já foi fechado
            if (Geral.ControleCaixaDiario &&
                CaixaDiarioDAO.Instance.CaixaFechadoPedido(session, idPedido))
                throw new Exception("O caixa já foi fechado.");

            // Verifica se o pedido possui sinal pago
            if (ped.RecebeuSinal && ped.Situacao != Pedido.SituacaoPedido.Confirmado)
                throw new Exception("Cancele o sinal deste pedido antes de cancelar o mesmo.");

            // Verifica se o pedido possui pagamento antecipado
            if (ped.IdPagamentoAntecipado > 0)
                throw new Exception("Cancele o pagamento antecipado deste pedido antes de cancelar o mesmo.");

            // Se o pedido estiver em uma obra já finalizada, não permite o cancelamento deste
            if (!gerarCredito && ped.IdObra > 0 &&
                ObraDAO.Instance.ObtemValorCampo<int>(session, "situacao", "idObra=" + ped.IdObra) == (int)Obra.SituacaoObra.Finalizada)
                /* Chamado 52310. */
                gerarCreditoObra = true;
            //throw new Exception("Este pedido está associado à uma obra já finalizada, cancele-a antes de cancelar este pedido.");

            // Chamado 14793: Não permite cancelar pedidos que possuam conferência gerada
            if (PedidoConfig.LiberarPedido && PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido))
                throw new Exception("Não é possível cancelar pedidos que possuam conferência gerada.");

            // Verifica se já existe alguma impressão para este pedido, se tiver, não permite cancelamento
            if (ped.Situacao != Pedido.SituacaoPedido.Ativo &&
                ped.TipoVenda != (int)Pedido.TipoVendaPedido.Reposição &&
                ped.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia && !ped.VendidoFuncionario)
            {
                if (objPersistence.ExecuteSqlQueryCount(session, @"Select Count(*) From produto_impressao pi
                        inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                        Where pi.idPedido=" + idPedido + " and !coalesce(pi.cancelado,false) and ie.situacao=" +
                                                                 (int)
                                                                     ImpressaoEtiqueta.SituacaoImpressaoEtiqueta
                                                                         .Ativa) > 0)
                {
                    throw new Exception("Este pedido não pode ser cancelado pois já está em produção.\\n" +
                                        "Contacte o responsável pelo PCP para cancelar a produção desse pedido para que você possa cancelá-lo.");
                }
            }

            if (
                objPersistence.ExecuteSqlQueryCount(session,
                    "Select Count(*) From produto_pedido_producao Where idPedidoExpedicao=" + idPedido) > 0)
                throw new Exception(
                    "Este pedido possui peças entregues na produção, volte a situação dessas peças antes de cancelá-lo.");

            if (
                objPersistence.ExecuteSqlQueryCount(session,
                    "Select Count(*) From produto_impressao Where idPedidoExpedicao=" + idPedido) > 0)
                throw new Exception("Não é possível cancelar esse pedido porque há peças entregues na produção.");

            //Valida se o pedido ja tem OC se tiver não pode cancelar
            if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido))
                throw new Exception("Não é possível cancelar esse pedido porque há uma OC vinculada.");

            if (TemVolume(session, idPedido))
                throw new Exception("Não é possível cancelar esse pedido porque há volumes gerados.");

            // Verifica se há comissão paga para esse pedido
            string idsComissoes = ComissaoPedidoDAO.Instance.IdsComissoesPagasPedido(session, idPedido);
            if (!gerarDebitoComissao && !String.IsNullOrEmpty(idsComissoes))
                throw new ComissaoGeradaException(idsComissoes, true);

            // Verifica se há comissão gerada para esse pedido
            else if (!gerarDebitoComissao && String.IsNullOrEmpty(idsComissoes) &&
                     ComissaoPedidoDAO.Instance.TemComissao(session, idPedido))
                throw new Exception(
                    "Não é possível cancelar esse pedido porque há comissões não pagas geradas para ele.");

            // Gera os débitos de comissão, se necessário
            else if (gerarDebitoComissao && !String.IsNullOrEmpty(idsComissoes))
                DebitoComissaoDAO.Instance.GeraDebito(session, idPedido, idsComissoes);

            if (!PedidoConfig.LiberarPedido &&
                ExecuteScalar<bool>(session,
                    "Select Count(*)>0 From cheques Where idDeposito>0 And idPedido=" + idPedido))
                throw new Exception("Este pedido possui cheques que já foram depositados, cancele ou retifique o depósito antes de cancelá-lo.");

            //verifica se há pedido de corte ativo associado
            if (ped.GerarPedidoProducaoCorte &&
                objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM pedido WHERE  IdPedidoRevenda={0} AND Situacao<>{1}",
                    ped.IdPedido, (int)Pedido.SituacaoPedido.Cancelado)) > 0)
                throw new Exception("Este pedido possui pedidos de produção associados ativos, para cancelar este pedido antes você deve cancelar os pedidos de produção ativos associados a ele.");
            var situacaoPed = ped.Situacao;

            // Salva o motivo do cancelamento
            //Concatena o Obs do pedido com o motivo do cancelamento
            ped.Obs = !String.IsNullOrEmpty(ped.Obs) ? ped.Obs + " " + motivoCancelamento : motivoCancelamento;
            // Se o tamanho do campo Obs exceder 1000 caracteres, salva apenas os 1000 primeiros, descartando o restante
            ped.Obs = ped.Obs.Length > 1000 ? ped.Obs.Substring(0, 1000) : ped.Obs;

            if (ped.Situacao == Pedido.SituacaoPedido.Ativo ||
                ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição ||
                ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia || ped.VendidoFuncionario)
            {
                if (!gerarCredito)
                {
                    // Exclui cheques relacionados em aberto à este pedido e cancela os demais
                    ChequesDAO.Instance.DeleteByPedido(session, idPedido);
                }

                if (!PedidoConfig.LiberarPedido && ped.VendidoFuncionario)
                    EstornaMovFunc(session, idPedido, ped.IdFuncVenda.Value);

                // Salva a situação do pedido
                Pedido.SituacaoPedido situacaoAtual = ped.Situacao;

                #region Altera situação para cancelado

                try
                {
                    ped.DataCanc = DateTime.Now;
                    ped.UsuCanc = UserInfo.GetUserInfo.CodUser;
                    ped.Situacao = Pedido.SituacaoPedido.Cancelado;

                    UpdateBase(session, ped);
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                    throw new Exception("Falha ao atualizar situação do pedido. Erro: " + msg);
                }

                #endregion

                #region Estorna/Tira da reserva ou liberação produtos deste pedido

                if (situacaoAtual == Pedido.SituacaoPedido.Confirmado ||
                    situacaoAtual == Pedido.SituacaoPedido.ConfirmadoLiberacao ||
                    situacaoAtual == Pedido.SituacaoPedido.LiberadoParcialmente)
                {
                    var idsProdQtde = new Dictionary<int, float>();

                    // Tira produtos da reserva ou estorna se já tiver dado baixa
                    foreach (var p in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                    {
                        var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session,
                            (int)p.IdProd);

                        var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                  tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                        var m2Saida = CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                            (int)p.Altura, p.QtdSaida, (int)p.IdProd, p.Redondo, 0,
                            tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                        var qtdSaida = p.Qtde - p.QtdSaida;
                        var qtdCreditoEstoque = p.QtdSaida;

                        if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                        {
                            qtdSaida *= p.Altura;
                            qtdCreditoEstoque *= p.Altura;
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!PedidoDAO.Instance.IsProducao(session, idPedido))
                        {
                            if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM - m2Saida : qtdSaida);
                            else
                                idsProdQtde[(int)p.IdProd] += m2 ? p.TotM - m2Saida : qtdSaida;
                        }

                        if (p.QtdSaida > 0)
                            MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja, idPedido,
                                p.IdProdPed,
                                (decimal)(m2 ? m2Saida : qtdCreditoEstoque), true);

                        if (situacaoAtual == Pedido.SituacaoPedido.Confirmado && ProdutoDAO.Instance.IsVidro(session, (int)p.IdProd))
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(null, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                    }

                    if (situacaoAtual == Pedido.SituacaoPedido.Confirmado)
                    {
                        if (PedidoConfig.LiberarPedido)
                            ProdutoLojaDAO.Instance.TirarLiberacao(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                        else
                            ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                    }
                    else
                        ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                }

                #endregion

                #region Volta a situação das etiquetas da reposição (tira da perda)

                if (PedidoReposicaoDAO.Instance.IsPedidoReposicao(session, idPedido))
                {
                    foreach (ProdutosPedido pp in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                        if (!string.IsNullOrEmpty(pp.NumEtiquetaRepos))
                            ProdutoPedidoProducaoDAO.Instance.VoltarPerdaProducao(session, pp.NumEtiquetaRepos, false);
                }

                #endregion
            }
            else // Se o pedido estiver confirmado e for a vista ou a prazo ou obra
            {
                List<ProdutosPedido> lstProdPed = new List<ProdutosPedido>();

                if (!gerarCredito)
                {
                    // Realiza os estornos/cancelamentos financeiros
                    UtilsFinanceiro.CancelaRecebimento(session, UtilsFinanceiro.TipoReceb.PedidoAVista,
                        ped, null, null, null, null, 0,
                        null, null, null, null, dataEstornoBanco, false, false);
                }
                else
                {
                    // Deve ser chamado ao gerar crédito.
                    // Exclui contas a receber que podem ter sido geradas para este pedido e que ainda não foi paga
                    ContasReceberDAO.Instance.DeleteByPedido(session, idPedido);
                }

                // Salva a situação do pedido
                Pedido.SituacaoPedido situacaoAtual = ped.Situacao;

                #region Altera situação para cancelado e atualização observação

                try
                {
                    ped.DataCanc = DateTime.Now;
                    ped.UsuCanc = UserInfo.GetUserInfo.CodUser;
                    ped.Situacao = Pedido.SituacaoPedido.Cancelado;

                    UpdateBase(session, ped);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                }

                #endregion

                #region Cancela o pedido espelho

                try
                {
                    if (PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido))
                        objPersistence.ExecuteCommand(session,
                            "update pedido_espelho set situacao=" + (int)PedidoEspelho.SituacaoPedido.Cancelado +
                            " where idPedido=" + idPedido);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido espelho.", ex));
                }

                #endregion

                #region Estorna produtos/Tira da reserva ou liberação

                try
                {
                    if (situacaoAtual == Pedido.SituacaoPedido.Confirmado ||
                        situacaoAtual == Pedido.SituacaoPedido.ConfirmadoLiberacao ||
                        situacaoAtual == Pedido.SituacaoPedido.LiberadoParcialmente)
                    {
                        // Estorna produtos ao estoque da loja
                        lstProdPed =
                            new List<ProdutosPedido>(ProdutosPedidoDAO.Instance.GetByPedidoLite(session,
                                idPedido, true));

                        if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                        {
                            // Tira produtos do estoque
                            foreach (ProdutosPedido p in lstProdPed)
                            {
                                // Busca a quantidade que foi dado baixa deste produto no estoque
                                int qtdBaixa = objPersistence.ExecuteSqlQueryCount(session,
                                    @"Select Count(*) From produto_pedido_producao 
                                        Where idProdPed In (Select idProdPed from produtos_pedido_espelho Where idPedido=" +
                                    idPedido + @" 
                                        And idProd=" + p.IdProd +
                                    ") And idSetor in (select idSetor from setor where forno)");

                                bool m2 =
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) ==
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) ==
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                Single m2Saida = Glass.Global.CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                                    (int)p.Altura, qtdBaixa, (int)p.IdProd, p.Redondo, 0,
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) !=
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto);

                                float areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(session,
                                    (int)p.IdProd);

                                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);

                                float m2CalcAreaMinima = Glass.Global.CalculosFluxo.CalcM2Calculo(session,
                                    idCliente, (int)p.Altura, p.Largura,
                                    qtdBaixa, (int)p.IdProd, p.Redondo,
                                    p.Beneficiamentos.CountAreaMinimaSession(session), areaMinimaProd, false,
                                    p.Espessura, true);

                                MovEstoqueDAO.Instance.BaixaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                    idPedido, p.IdProdPed,
                                    (decimal)(m2 ? m2Saida : qtdBaixa), (decimal)(m2 ? m2CalcAreaMinima : 0),
                                    false, null);

                                MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                    idPedido, p.IdProdPed,
                                    (decimal)(m2 ? m2Saida : qtdBaixa), true);
                            }
                        }
                        else
                        {
                            var idsProdQtde = new Dictionary<int, float>();

                            // Tira produtos da reserva ou estorna se já tiver dado baixa
                            foreach (var p in lstProdPed)
                            {
                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session,
                                    (int)p.IdProd);

                                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                          tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                                var m2Saida = CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                                    (int)p.Altura, p.QtdSaida, (int)p.IdProd, p.Redondo, 0,
                                    tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                                var qtdSaida = p.Qtde - p.QtdSaida;
                                var qtdCreditoEstoque = p.QtdSaida;

                                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                                {
                                    qtdSaida *= p.Altura;
                                    qtdCreditoEstoque *= p.Altura;
                                }

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                    idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM - m2Saida : qtdSaida);
                                else
                                    idsProdQtde[(int)p.IdProd] += m2 ? p.TotM - m2Saida : qtdSaida;

                                if (p.QtdSaida > 0)
                                    MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                        idPedido, p.IdProdPed,
                                        (decimal)(m2 ? m2Saida : qtdCreditoEstoque), true);
                            }

                            if (situacaoAtual == Pedido.SituacaoPedido.Confirmado)
                            {
                                if (PedidoConfig.LiberarPedido)
                                    ProdutoLojaDAO.Instance.TirarLiberacao(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                                else
                                    ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                            }
                            else
                                ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao estornar produtos ao estoque da loja.", ex));
                }

                #endregion

                // Exclui contas recebida gerada por este pedido se for à vista
                ContasReceberDAO.Instance.DeleteByPedidoAVista(session, idPedido);
            }

            #region Movimenta a materia-prima

            if (situacaoPed == Pedido.SituacaoPedido.Conferido || situacaoPed == Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro
                || situacaoPed == Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro)
            {
                foreach (var p in ProdutosPedidoDAO.Instance.GetByPedidoLite(idPedido, true))
                {
                    if (ProdutoDAO.Instance.IsVidro(null, (int)p.IdProd))
                        MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(null, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                }
            }

            #endregion

            if (gerarCredito || gerarCreditoObra)
            {
                #region Gera o crédito para o cliente

                /* Chamado 52310. */
                var totalUsar = gerarCreditoObra ? ped.ValorPagamentoAntecipado : ped.Total;

                if (Geral.ControleCaixaDiario)
                {
                    CaixaDiarioDAO.Instance.MovCxPedido(session, ped.IdLoja, ped.IdCli, ped.IdPedido, 1, totalUsar, 0,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), null, "Cancelamento do pedido", false);
                }
                else
                {
                    CaixaGeralDAO.Instance.MovCxPedido(session, ped.IdPedido, ped.IdCli,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado),
                        1, totalUsar, 0, null, "Cancelamento do pedido", false, null);
                }

                #endregion

                ClienteDAO.Instance.CreditaCredito(session, ped.IdCli, totalUsar);
            }

            #region Atualiza o saldo da obra do pedido

            if (ped.IdObra > 0)
                ObraDAO.Instance.AtualizaSaldo(session, ped.IdObra.Value, false);

            #endregion

            #region Reabre o orçamento e o projeto

            try
            {
                if (ped.IdOrcamento != null)
                {
                    objPersistence.ExecuteCommand(session,
                        "Update orcamento Set idPedidoGerado=null where idOrcamento=" + ped.IdOrcamento);

                    // Remove os IDs dos produtos do pedido dos produtos do orçamento
                    objPersistence.ExecuteCommand(session, @"
                            update produtos_orcamento po
                                left join (
                                    select idProdParent, count(*) as num
                                    from produtos_orcamento
                                    where idOrcamento=" + ped.IdOrcamento + @" and idProdParent is not null
                                    group by idProdParent
                                ) as pc on (po.idProd=pc.idProdParent)
                            set po.idProdPed=null
                            where po.idOrcamento=" + ped.IdOrcamento + @" and (po.idProdPed=0
                                or (po.idItemProjeto is null and (
                                    po.idProdPed in (select idProdPed from produtos_pedido where idPedido=" + idPedido +
                                                           @")
                                    or po.idProdPed not in (select idProdPed from produtos_pedido)
                                    or po.idProdPed in (select idProdPed from produtos_pedido where idPedido in (
                                        select idPedido from pedido where situacao=" +
                                                           (int)Pedido.SituacaoPedido.Cancelado + @"))
                                )) or ((po.idItemProjeto is not null or coalesce(pc.num, 0)=0) and (
                                    po.idProdPed in (select idAmbientePedido from ambiente_pedido where idPedido=" +
                                                           idPedido + @")
                                    or po.idProdPed not in (select idAmbientePedido from ambiente_pedido)
                                    or po.idProdPed in (select idAmbientePedido from ambiente_pedido where idPedido in (
                                        select idPedido from pedido where situacao=" +
                                                           (int)Pedido.SituacaoPedido.Cancelado + @"))
                                )))");

                    if (OrcamentoConfig.NegociarParcialmente)
                    {
                        int situacao =
                            !OrcamentoDAO.Instance.IsNegociadoParcialmente(session, ped.IdOrcamento.Value)
                                ? (int)Orcamento.SituacaoOrcamento.Negociado
                                : (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente;

                        objPersistence.ExecuteCommand(session,
                            "update orcamento set situacao=" + situacao + " where idOrcamento=" +
                            ped.IdOrcamento.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao reabrir o orçamento.", ex));
            }

            try
            {
                if (ped.IdProjeto != null)
                    ProjetoDAO.Instance.ReabrirProjeto(session, ped.IdProjeto.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao reabrir o projeto.", ex));
            }

            #endregion

            #region Cancela Instalações

            // Cancela as instalações do pedido, se houver
            var lstInst = InstalacaoDAO.Instance.GetByPedido(session, ped.IdPedido).ToArray();

            // Se a instalação estiver aberta, cancela
            foreach (Instalacao inst in lstInst)
                if (inst.Situacao == 1)
                    InstalacaoDAO.Instance.Cancelar(session, inst.IdInstalacao);

            #endregion

            #region Marca etiquetas como canceladas

            objPersistence.ExecuteCommand(session, @"
                    update produto_pedido_producao ppp
                        inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                    set ppp.situacao=" + (!ped.MaoDeObra
                ? (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda
                : (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra) + " where ppe.idPedido=" + idPedido);

            // Altera a situação de produção do pedido para Etiqueta Não Impressa.
            objPersistence.ExecuteCommand(session,
                string.Format("UPDATE pedido p SET p.SituacaoProducao={0} WHERE p.IdPedido={1}",
                    (int)Pedido.SituacaoProducaoEnum.NaoEntregue, idPedido));

            #endregion

            LogCancelamentoDAO.Instance.LogPedido(session, ped,
                motivoCancelamento.Substring(motivoCancelamento.ToLower().IndexOf("motivo do cancelamento: ") +
                              "motivo do cancelamento: ".Length), true);
        }

        /// <summary>
        /// Estorna movimentações de funcionários
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idFuncVenda"></param>
        internal void EstornaMovFunc(GDASession sessao, uint idPedido, uint idFuncVenda)
        {
            foreach (MovFunc m in MovFuncDAO.Instance.GetByPedido(sessao, idPedido))
            {
                if (m.TipoMov == (int)MovFunc.TipoMovEnum.Entrada)
                    break;

                // Recupera o plano de conta
                uint idConta = m.IdConta;

                // Recupera a lista de planos de conta de sinal, à vista e à prazo
                List<string> sinal = new List<string>(UtilsPlanoConta.ListaEstornosSinalPedido().Split(','));
                List<string> vista = new List<string>(UtilsPlanoConta.ListaEstornosAVista().Split(','));
                List<string> prazo = new List<string>(UtilsPlanoConta.ListaEstornosAPrazo().Split(','));

                // Recupera o plano de conta de estorno, se possível
                if (sinal.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoSinalPedido(m.IdConta);
                else if (vista.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoAVista(m.IdConta);
                else if (prazo.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoAPrazo(m.IdConta);

                // Efetua o estorno
                MovFuncDAO.Instance.MovimentarPedido(sessao, idFuncVenda, idPedido, idConta, 1, m.ValorMov, null);
            }
        }

        #endregion

        #region Busca o percentual de desconto do pedido

        /// <summary>
        /// Busca o percentual de desconto do pedido
        /// </summary>
        public float GetPercDesc(uint idPedido)
        {
            return GetPercDesc(null, idPedido);
        }

        /// <summary>
        /// Busca o percentual de desconto do pedido
        /// </summary>
        public float GetPercDesc(GDASession session, uint idPedido)
        {
            string sql = "Select Coalesce(if(tipoDesconto=1, desconto/100, desconto/(total+desconto)), 0) From pedido Where idPedido=" + idPedido;
            return ExecuteScalar<float>(session, sql);
        }

        #endregion

        #region Verifica se o desconto do pedido está dentro do permitido

        /// <summary>
        /// Verifica se o desconto do pedido está dentro do permitido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        private bool DescontoPermitido(GDASession sessao, Pedido pedido)
        {
            string somaDesconto = "(select sum(coalesce(valorDescontoQtde,0)" + (PedidoConfig.RatearDescontoProdutos ? "+coalesce(valorDesconto,0)+coalesce(valorDescontoProd,0)" :
                "") + ") from produtos_pedido where idPedido=p.idPedido)";

            uint idFunc = UserInfo.GetUserInfo.CodUser;
            if (Geral.ManterDescontoAdministrador)
                idFunc = pedido.IdFuncDesc.GetValueOrDefault(idFunc);

            if (idFunc == 0)
                idFunc = pedido.IdFunc;

            float descontoMaximoPermitido = PedidoConfig.Desconto.GetDescontoMaximoPedido(sessao, idFunc, pedido.TipoVenda ?? 0, (int?)pedido.IdParcela);

            if (descontoMaximoPermitido == 100)
                return true;

            if (FinanceiroConfig.UsarDescontoEmParcela)
            {
                var idParcela = pedido.IdParcela;
                if (idParcela.GetValueOrDefault(0) > 0)
                {
                    var desconto = ParcelasDAO.Instance.ObtemDesconto(sessao, idParcela.Value);
                    if (desconto == ObtemDescontoCalculado(sessao, pedido.IdPedido))
                        return true;
                }
            }
            else if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
            {
                uint tipoVenda = (uint?)pedido.TipoVenda ?? 0;
                uint? idFormaPagto = pedido.IdFormaPagto;
                uint? idTipoCartao = pedido.IdTipoCartao;
                uint? idParcela = pedido.IdParcela;
                uint? idGrupoProd = null;
                uint? idSubgrupoProd = null;

                var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(sessao, pedido.IdPedido);
                if (produtosPedido != null && produtosPedido.Count > 0)
                {
                    idGrupoProd = produtosPedido[0].IdGrupoProd;
                    idSubgrupoProd = produtosPedido[0].IdSubgrupoProd;
                }

                var desconto = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDesconto(tipoVenda, idFormaPagto, idTipoCartao, idParcela, idGrupoProd, idSubgrupoProd);
                if (desconto == ObtemDescontoCalculado(sessao, pedido.IdPedido))
                    return true;
            }
            var valorDescontoConsiderar = (Data.DAL.FuncionarioDAO.Instance.ObtemIdTipoFunc(sessao, UserInfo.GetUserInfo.CodUser) == (int)Glass.Seguranca.TipoFuncionario.Administrador ? "100"
                : descontoMaximoPermitido.ToString().Replace(",", "."));

            string sql = $@"Select Count(*) from pedido p Where idPedido={pedido.IdPedido} And (
                (tipoDesconto=1 And desconto<={valorDescontoConsiderar}) Or
                (tipoDesconto=2 And Coalesce(round(desconto/(total+{(somaDesconto + (!PedidoConfig.RatearDescontoProdutos ? "+desconto" : ""))}),2),0)<=({valorDescontoConsiderar}/100)))";

            return ExecuteScalar<int>(sessao, sql) > 0;
        }

        private void RemoveDescontoNaoPermitido(GDASession sessao, Pedido pedido)
        {
            if (pedido == null)
                return;

            // Remove o desconto dos produtos
            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, pedido.IdPedido, false, true);
            var removidos = new List<uint>();

            if (RemoverDesconto(sessao, pedido, produtosPedido))
                removidos.AddRange(produtosPedido.Select(p => p.IdProdPed));

            foreach (AmbientePedido ambiente in (pedido as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>())
            {
                var produtosAmbiente = produtosPedido.Where(p => p.IdAmbientePedido == ambiente.IdAmbientePedido);

                if (AmbientePedidoDAO.Instance.RemoverDesconto(sessao, pedido, ambiente.IdAmbientePedido, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            var produtosAtualizar = produtosPedido
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, pedido, produtosAtualizar, true);

            objPersistence.ExecuteCommand(sessao, @"
                Update pedido set desconto=0 
                Where idPedido=" + pedido.IdPedido + @";
                Update pedido p set Total=Round((   
                    Select Sum(Total + coalesce(valorBenef, 0)) 
                    From produtos_pedido 
                    Where IdPedido=p.IdPedido 
                        And (InvisivelPedido = false or InvisivelPedido is null)), 2) 
                Where p.IdPedido=" + pedido.IdPedido);

            // Chamado 21923: Não deve salvar log se pedido já estiver liberado, pois a alteração de desconto não será salva.
            if (pedido.Situacao != Pedido.SituacaoPedido.Confirmado)
            {
                Erro novo = new Erro();
                novo.UrlErro = "Desconto Pedido " + pedido.IdPedido;
                novo.DataErro = DateTime.Now;
                novo.IdFuncErro = UserInfo.GetUserInfo.CodUser;
                novo.Mensagem = "Removido desconto do pedido " + pedido.IdPedido;

                ErroDAO.Instance.Insert(novo);
            }
        }

        #endregion

        #region Fast Delivery

        public bool IsFastDelivery(uint idPedido)
        {
            return IsFastDelivery(null, idPedido);
        }

        public bool IsFastDelivery(GDASession sessao, uint idPedido)
        {
            string sql = "select Coalesce(FastDelivery, 0) from pedido where idPedido=" + idPedido;
            int retorno = Convert.ToInt32(objPersistence.ExecuteScalar(sessao, sql));
            return retorno == 1;
        }

        #endregion

        #region Verifica o tipo do pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido.TipoPedidoEnum GetTipoPedido(uint idPedido)
        {
            return GetTipoPedido(null, idPedido);
        }

        public Pedido.TipoPedidoEnum GetTipoPedido(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<Pedido.TipoPedidoEnum>(sessao, "tipoPedido", "idPedido=" + idPedido);
        }

        #region Verifica se pedido é Mão de Obra

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se pedido é mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObra(uint idPedido)
        {
            return IsMaoDeObra(null, idPedido);
        }

        /// <summary>
        /// Verifica se pedido é mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObra(GDASession sessao, uint idPedido)
        {
            return GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObra;
        }

        #endregion

        #region Verifica se pedido é Produção

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se pedido é Produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsProducao(uint idPedido)
        {
            return IsProducao(null, idPedido);
        }

        /// <summary>
        /// Verifica se pedido é Produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsProducao(GDASession sessao, uint idPedido)
        {
            return GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.Producao;
        }

        #endregion

        #region Verifica se pedido é Venda

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verific se pedido é Venda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsVenda(uint idPedido)
        {
            return IsVenda(null, idPedido);
        }

        /// <summary>
        /// Verific se pedido é Venda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsVenda(GDASession sessao, uint idPedido)
        {
            return !IsMaoDeObra(sessao, idPedido) && !IsProducao(sessao, idPedido);
        }

        #endregion

        #region Verifica se o pedido é do tipo Revenda

        /// <summary>
        /// Verifica se o pedido é do tipo Revenda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsRevenda(uint idPedido)
        {
            return GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.Revenda;
        }

        #endregion

        #region Verifica se pedido é Mão de Obra Especial

        /// <summary>
        /// Verifica se pedido é mão de obra especial
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObraEspecial(uint idPedido)
        {
            return GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial;
        }

        #endregion

        #region Verifica se os pedidos são de loja diferente

        /// <summary>
        /// Verifica se os pedidos são de loja diferente
        /// </summary>
        public bool PedidosLojasDiferentes(string idsPedidos)
        {
            var sql = string.Format("Select IdLoja from pedido where idPedido IN({0}) group by idloja", idsPedidos);
            return this.CurrentPersistenceObject.LoadResult(sql, null).Count() > 1;
        }

        #endregion

        #endregion

        #region Descrição do tipo de Entrega

        public string DescrTipoEntrega(int tipoEntrega)
        {
            switch (tipoEntrega)
            {
                case 1:
                    return "Balcão";
                case 2:
                    return "Colocação Comum";
                case 3:
                    return "Colocação Temperado";
                case 4:
                    return "Entrega";
                case 5:
                    return "Manutenção Temperado";
                case 6:
                    return "Colocação Esquadria";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Verifica se o pedido possui vidros para produção/estoque

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido possui vidros para produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosProducao(uint idPedido)
        {
            return PossuiVidrosProducao(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui vidros para produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosProducao(GDASession sessao, uint idPedido)
        {
            if (IsMaoDeObra(sessao, idPedido))
                return true;

            var sql = @"
                SELECT COUNT(*) FROM produtos_pedido pp 
                    INNER JOIN produto p On (pp.IdProd=p.IdProd)
                WHERE pp.IdPedido=" + idPedido + @"
                    AND p.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"  
                    AND (p.IdSubgrupoProd IN (
                        SELECT sgp.IdSubgrupoProd From subgrupo_prod sgp Where sgp.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            AND (sgp.ProdutosEstoque=FALSE OR sgp.ProdutosEstoque IS NULL))" +
                /* Chamado 16470.
                 * Produtos sem associação de subgrupo não estavam sendo considerados como vidros de produção,
                 * por isso, colocamos uma condição que irá verificar se o produto não tem subgrupo. */
                " OR COALESCE(p.IdSubgrupoProd, 0)=0)";
            /*string sql = @"
                Select Count(*) From produtos_pedido pp 
                    Inner Join produto p On (pp.idProd=p.idProd)
                Where pp.idPedido=" + idPedido + @"
                    And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"  
                    And p.idSubgrupoProd In (
                        Select idSubgrupoProd From subgrupo_prod Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            And (produtosEstoque=false or produtosEstoque is null)
                    )";*/

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possui volumes
        /// </summary>
        public bool PossuiVolume(GDASession session, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM produtos_pedido pp 
                    INNER JOIN produto p On (pp.idProd=p.idProd)
                    LEFT JOIN grupo_prod gp ON (p.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
                WHERE pp.idPedido=" + idPedido + @"
                    AND COALESCE(sgp.geraVolume, gp.geraVolume, false) = true
                    AND COALESCE(pp.invisivelFluxo, false) = false";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Retorna a quantidade de vidros para retirada no estoque
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemQtdVidrosProducao(GDASession session, uint idPedido)
        {
            string sql = @"
                Select Sum(Qtde) From produtos_pedido pp 
                    Inner Join produto p On (pp.idProd=p.idProd)
                Where pp.idPedido=" + idPedido + @" and Coalesce(invisivelFluxo,false)=false
                    And p.idGrupoProd=1 
                    And p.idSubgrupoProd In (
                        Select idSubgrupoProd From subgrupo_prod Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            And produtosEstoque=true
                    )";

            return ExecuteScalar<int>(session, sql);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido possui vidros para retirada no estoque
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosEstoque(uint idPedido)
        {
            return PossuiVidrosEstoque(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui vidros para retirada no estoque
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosEstoque(GDASession sessao, uint idPedido)
        {
            if (IsMaoDeObra(sessao, idPedido))
                return true;

            return ObtemQtdVidrosProducao(sessao, idPedido) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se existe algum produto no pedido passado que ainda não foi marcada saída
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiProdutosPendentesSaida(uint idPedido)
        {
            return PossuiProdutosPendentesSaida(null, idPedido);
        }

        /// <summary>
        /// Verifica se existe algum produto no pedido passado que ainda não foi marcada saída
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiProdutosPendentesSaida(GDASession sessao, uint idPedido)
        {
            string sql = @"
                Select Count(*) From produtos_pedido pp 
                Where pp.idPedido=" + idPedido + @"
                    And pp.qtde<>Coalesce(pp.qtdSaida, 0)";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Produção

        #region Pedido único para Corte

        /// <summary>
        /// Retorna o pedido para corte, se puder ser retornado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido GetForCorte(uint idPedido, int situacao)
        {
            if (idPedido == 0 || situacao == 0)
                return null;
            else if (!PedidoExists(idPedido))
                throw new Exception("Não foi encontrado nenhum pedido com o número informado.");
            else
            {
                bool existsPedidoCorte = PedidoCorteDAO.Instance.ExistsByPedido(idPedido);

                if (situacao == (int)PedidoCorte.SituacaoEnum.Producao)
                {
                    // Se já existir um pedido corte para este pedido, verifica sua situação
                    if (existsPedidoCorte)
                    {
                        PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                        switch (pedidoCorte.Situacao)
                        {
                            case (int)PedidoCorte.SituacaoEnum.Producao:
                                throw new Exception("Este Pedido já está em Produção.");
                            case (int)PedidoCorte.SituacaoEnum.Pronto:
                                throw new Exception("Este Pedido já está Pronto.");
                            case (int)PedidoCorte.SituacaoEnum.Entregue:
                                throw new Exception("Este Pedido já está Entregue.");
                        }
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Pronto)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        throw new Exception("Este pedido ainda não entrou em Produção.");

                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            throw new Exception("Este Pedido ainda não entrou em Produção.");
                        case (int)PedidoCorte.SituacaoEnum.Pronto:
                            throw new Exception("Este Pedido já está Pronto.");
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            throw new Exception("Este Pedido já está Entregue.");
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Entregue)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        throw new Exception("Este pedido ainda não entrou em Produção.");

                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            throw new Exception("Este Pedido ainda não entrou em Produção.");
                        case (int)PedidoCorte.SituacaoEnum.Producao:
                            throw new Exception("Este Pedido ainda está em Produção, precisa estar Pronto para ser Entregue.");
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            throw new Exception("Este Pedido já está Entregue.");
                    }
                }
            }

            bool temFiltro;
            string filtroAdicional;

            Pedido pedido = objPersistence.LoadOneData(Sql(idPedido, 0, null, null, 0, 0, null, 0, null, 0, null, null, null, null,
                String.Empty, String.Empty, String.Empty, String.Empty, null, null, null, null, null, null, 0, false,
                false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional));

            if (pedido.Situacao == Pedido.SituacaoPedido.Cancelado)
                throw new Exception("Este pedido foi cancelado.");

            #region Busca as parcelas do pedido

            var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(idPedido).ToArray();

            string parcelas = lstParc.Length + " vez(es): ";

            pedido.ValoresParcelas = new decimal[lstParc.Length];
            pedido.DatasParcelas = new DateTime[lstParc.Length];

            for (int i = 0; i < lstParc.Length; i++)
            {
                pedido.ValoresParcelas[i] = lstParc[i].Valor;
                pedido.DatasParcelas[i] = lstParc[i].Data != null ? lstParc[i].Data.Value : new DateTime();
                parcelas += lstParc[i].Valor.ToString("c") + "-" + (lstParc[i].Data != null ? lstParc[i].Data.Value.ToString("d") : "") + ",  ";
            }

            if (lstParc.Length > 0 && pedido.TipoVenda != (int)Pedido.TipoVendaPedido.AVista)
                pedido.DescrParcelas = parcelas.TrimEnd(' ').TrimEnd(',');

            #endregion

            return pedido;
        }

        #endregion

        #region Busca Pedidos pela situação do corte

        private string SqlCorte(uint idPedido, string dataIni, string dataFim, int situacao, bool selecionar,
            out bool temFiltro, out string dataPesq)
        {
            temFiltro = false;

            // Data que será utilizada para pesquisar e ordenar
            dataPesq = String.Empty;

            string campos = selecionar ? "p.*, pc.Situacao as SitProducao, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From pedido p 
                Left Join cliente c On (p.IdCli=c.Id_Cli) 
                Inner Join pedido_corte pc On (p.IdPedido=pc.IdPedido) 
                Where 1 ";

            // Se nenhuma situação tiver sido especificada, não retorna nada
            if (situacao == 0)
            {
                temFiltro = true;
                return sql += " And 0>1";
            }
            else
            {
                sql += " And pc.Situacao=" + situacao;
                temFiltro = true;
            }

            LoginUsuario login = UserInfo.GetUserInfo;

            // Se a situação for 2 (Producao) ou 3 (Pronto) e não for gerente, busca so os pedidos do funcionário logado
            if (situacao == 2 || situacao == 3)
            {
                sql += " And pc.IdFuncProducao=" + login.CodUser;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(login.CodUser) + "    ";
                temFiltro = true;
            }

            // Descrição da situação que será filtrada por data
            string filtroData = String.Empty;

            // Verifica por qual Data será pesquisado (Producao, Pronto, Entregue)
            switch (situacao)
            {
                case 2:
                    dataPesq = "pc.DataProducao";
                    filtroData = "Produção"; break;
                case 3:
                    dataPesq = "pc.DataPronto";
                    filtroData = "Pronto"; break;
                case 4:
                    dataPesq = "pc.DataEntregue";
                    filtroData = "Entregue"; break;
            }

            if (idPedido > 0)
            {
                sql += " And p.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }
            else
            {
                if (!String.IsNullOrEmpty(dataIni))
                {
                    sql += " And " + dataPesq + ">=?dataIni";
                    criterio += "Data Início (" + filtroData + "): " + dataIni + "    ";
                    temFiltro = true;
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    sql += " And " + dataPesq + "<=?dataFim";
                    criterio += "Data Fim (" + filtroData + "): " + dataFim + "    ";
                    temFiltro = true;
                }
            }

            sql = sql.Replace("$$$", criterio);

            return sql; // dataPesq != String.Empty ? (sql + " Order By " + dataPesq) : sql;
        }

        public Pedido[] GetForCorteRpt(uint idPedido, string dataIni, string dataFim, int situacao)
        {
            bool temFiltro;
            string dataPesq;
            return objPersistence.LoadData(SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq),
                GetParamCorte(dataIni, dataFim)).ToArray();
        }

        public IList<Pedido> GetForCorte(uint idPedido, string dataIni, string dataFim, int situacao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string dataPesq;

            string sql = SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq);
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : dataPesq;

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, GetParamCorte(dataIni, dataFim));
        }

        public int GetCountCorte(uint idPedido, string dataIni, string dataFim, int situacao)
        {
            bool temFiltro;
            string dataPesq;

            string sql = SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq);

            return GetCountWithInfoPaging(sql, temFiltro, GetParamCorte(dataIni, dataFim));
        }

        private GDAParameter[] GetParamCorte(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Listagem de Pedidos de Corte Padrão

        private string SqlListCorte(uint idPedido, uint idCli, string nomeCli, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, 
                pc.DataProducao, pc.DataEntregue, pc.DataPronto as DataProntoCorte, fprod.Nome as FuncProd, fe.Nome as FuncEntregue,  
                pc.Situacao as SitProducao, l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto" : "Count(*)";

            string sql = @"
                Select " + campos + @" From pedido p 
                Inner Join cliente c On (p.idCli=c.id_Cli) 
                Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                Inner Join loja l On (p.IdLoja = l.IdLoja) 
                Inner Join pedido_corte pc On (p.idPedido=pc.idPedido) 
                Left Join funcionario fprod On (pc.idFuncProducao=fprod.idFunc) 
                Left Join funcionario fe On (pc.idFuncEntregue=fe.idFunc) 
                Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) Where 1 ";

            if (idPedido > 0)
            {
                sql += " And p.IdPedido=" + idPedido;
                temFiltro = true;
            }
            else if (idCli > 0)
            {
                sql += " And IdCli=" + idCli;
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And idCli in (" + ids + ")";
                temFiltro = true;
            }

            return sql;
        }

        public IList<Pedido> GetListCorte(uint idPedido, uint idCli, string nomeCli, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "pc.DataProducao desc" : sortExpression;

            bool temFiltro;
            string sql = SqlListCorte(idPedido, idCli, nomeCli, true, out temFiltro);

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro,
                GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null));
        }

        public int GetCountListCorte(uint idPedido, uint idCli, string nomeCli)
        {
            bool temFiltro;
            string sql = SqlListCorte(idPedido, idCli, nomeCli, true, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro,
                GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null));
        }

        #endregion

        #region Retira pedido de alguma situação, voltando para a anterior

        public void VoltaSituacao(Pedido pedido)
        {
            PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(pedido.IdPedido);

            switch (pedidoCorte.Situacao)
            {
                case 2: // Produção, exclui pedido da tabela pedido_corte
                    PedidoCorteDAO.Instance.Delete(pedidoCorte); break;
                case 3: // Pronto, volta para produção
                    pedidoCorte.DataPronto = null;
                    pedidoCorte.Situacao = (int)PedidoCorte.SituacaoEnum.Producao;
                    PedidoCorteDAO.Instance.Update(pedidoCorte);
                    break;
                case 4: // Entregue, volta para pronto
                    pedidoCorte.IdFuncEntregue = null;
                    pedidoCorte.DataEntregue = null;
                    pedidoCorte.Situacao = (int)PedidoCorte.SituacaoEnum.Pronto;
                    PedidoCorteDAO.Instance.Update(pedidoCorte);
                    break;
            }
        }

        #endregion

        #endregion
    }
}
