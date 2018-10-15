using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;

namespace WebGlass.Business.Pedido.Fluxo
{
    /// <summary>
    /// Classe de confirmação de pedido.
    /// </summary>
    public sealed class ConfirmarPedido : BaseFluxo<ConfirmarPedido>
    {
        /// <summary>
        /// Ajax.
        /// </summary>
        private static Ajax.IConfirmar ajax;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConfirmarPedido"/>.
        /// </summary>
        private ConfirmarPedido() { }

        /// <summary>
        /// Obtém Ajax.
        /// </summary>
        public static Ajax.IConfirmar Ajax
        {
            get { return ajax ?? (ajax = new Ajax.Confirmar()); }
        }

        /// <summary>
        /// Confirmação de pedido em sistema de liberação.
        /// </summary>
        /// <param name="idsPedido">IDs dos pedidos que serão confirmados.</param>
        /// <param name="alterarDataEntrega">Informa se a data de entrega deverá ser alterada.</param>
        /// <param name="dataEntrega">Nova data de entrega, usada somente se o parâmetro alterarDataEntrega for true.</param>
        /// <param name="gerarEspelho">Define se o PCP do pedido deverá ser gerado, após a confirmação do comercial.</param>
        /// <param name="scriptExecutar">Script que será executado após a confirmação do pedido (montado dentro do método, de acordo com o resultado da confirmação do pedido).</param>
        public void ConfirmarPedidoLiberacao(List<int> idsPedido, bool alterarDataEntrega, DateTime? dataEntrega, bool gerarEspelho, out string scriptExecutar)
        {
            var idsPedidoErro = new List<int>();
            var idsOrcamentoGerados = new List<int>();
            var erroConferencia = string.Empty;

            try
            {
                if (idsPedido.Count == 0)
                {
                    throw new Exception("Informe os pedidos que serão confirmados.");
                }

                Exception erroConf = null;

                try
                {
                    PedidoDAO.Instance.ConfirmarLiberacaoPedidoComTransacao(idsPedido, out idsPedido, out idsPedidoErro, false, false);
                }
                catch (ValidacaoPedidoFinanceiroException ex)
                {
                    if (!ex.Message.Contains("demais pedidos"))
                    {
                        throw;
                    }

                    PedidoDAO.Instance.DisponibilizaConfirmacaoFinanceiro(null, ex.IdsPedido, Glass.MensagemAlerta.FormatErrorMsg("", ex));
                }
                catch (Exception ex1)
                {
                    if (!ex1.Message.Contains("demais pedidos"))
                    {
                        throw;
                    }

                    erroConf = ex1;
                }

                // Altera a data de entrega dos pedidos antes de enviá-los para conferência,
                // para que a data de entrega da fábrica fique correta
                if (alterarDataEntrega && dataEntrega != null)
                {
                    PedidoDAO.Instance.AlteraDataEntrega(string.Join(",", idsPedido), dataEntrega.Value);
                }

                // Se estiver marcado para gerar espelho, gera conferência já finalizada deste pedido
                if (gerarEspelho)
                {
                    foreach (var idPedido in idsPedido)
                    {
                        try
                        {
                            if (idPedido > 0)
                            {
                                PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao((uint)idPedido);

                                if (idsPedido.Count > 1 || Glass.Configuracoes.PedidoConfig.TelaConfirmaPedidoLiberacao.FinalizarPedidoAoGerarEspelho ||
                                    (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas)))
                                {
                                    var idOrcamento = PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao((uint)idPedido);

                                    if (idOrcamento > 0)
                                    {
                                        idsOrcamentoGerados.Add((int)idOrcamento);
                                    }
                                }
                                else
                                {
                                    scriptExecutar = "redirectUrl('../Cadastros/CadPedidoEspelho.aspx?idPedido=" + idPedido +
                                                     "');";
                                    return;
                                }
                            }
                        }
                        catch (ValidacaoPedidoFinanceiroException f)
                        {
                            throw f;
                        }
                        catch (Exception ex)
                        {
                            erroConferencia += string.Format("{0}, {1} {2}", idPedido, ex.Message, ex.InnerException);
                        }
                    }
                }

                var mensagemFinal = string.Empty;

                if (erroConf == null)
                {
                    mensagemFinal = $"Pedidos confirmados. { (string.IsNullOrWhiteSpace(erroConferencia) ? string.Empty : $" Erro com os seguintes pedidos: { erroConferencia }") }";
                }
                else
                {
                    mensagemFinal = Glass.MensagemAlerta.FormatErrorMsg(string.Empty, erroConf, false);
                }

                if (idsOrcamentoGerados.Count > 0)
                {
                    mensagemFinal += $" Orçamento(s) gerado(s): { string.Join(", ", idsOrcamentoGerados) }";
                }

                scriptExecutar = $"alert('{ mensagemFinal }');";

                if (idsPedido.Count > 0)
                {
                    scriptExecutar += $"openRptConf('{ string.Join(",", idsPedido) }');";
                }
            }
            catch (ValidacaoPedidoFinanceiroException f)
            {
                ErroDAO.Instance.InserirFromException("Gerar conferência exception financeiro", f);

                throw;
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Gerar conferência", ex);

                var mensagemFinal = Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar pedido.", ex);
                var idPedidoErro = idsPedidoErro.Count == 1 ? idsPedidoErro[0] : 0;
                var pedidoTemSinalReceber = PedidoDAO.Instance.TemSinalReceber(null, (uint)idPedidoErro);

                scriptExecutar = $"alert('{ mensagemFinal }');";

                if (UserInfo.GetUserInfo.IsFinanceiroReceb)
                {
                    scriptExecutar += $@"if ({ idPedidoErro } > 0 && confirm('Deseja receber o { (pedidoTemSinalReceber ? "sinal" : "pagamento antecipado") } do pedido { idPedidoErro }?'))
                    redirectUrl('../Cadastros/CadReceberSinal.aspx?idPedido={ idPedidoErro }{ (!pedidoTemSinalReceber ? "&antecipado=1" : string.Empty) }');\n";
                }
            }
        }
    }
}
