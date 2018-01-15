using System;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Exceptions;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class ConfirmarPedido : BaseFluxo<ConfirmarPedido>
    {
        private ConfirmarPedido() { }

        #region Ajax

        private static Ajax.IConfirmar _ajax;

        public static Ajax.IConfirmar Ajax
        {
            get { return _ajax ?? (_ajax = new Ajax.Confirmar()); }
        }

        #endregion

        public void ConfirmarPedidoLiberacao(string idsPedidos, bool alterarDataEntrega, DateTime? dataEntrega,
            bool gerarEspelho, out string scriptExecutar)
        {
            var idsPedidosErro = string.Empty;
            var idsOrcamentoGerados = string.Empty;
            var erroConferencia = string.Empty;

            try
            {
                if (idsPedidos.Length == 0)
                    throw new Exception("Informe os pedidos que serão confirmados.");
                
                // Motivo da retirada: É mais seguro utilizar o método TrimStart ao invés do Substring
                // com o substring o primeiro número do pedido estava sendo desconsiderado
                // isto porque a variável já havia sido tratada onde este método foi chamado.
                // idsPedidos = idsPedidos.Substring(1);
                idsPedidos = idsPedidos.TrimStart(',');

                Exception erroConf = null;

                try
                {
                    PedidoDAO.Instance.ConfirmarLiberacaoPedidoComTransacao(idsPedidos, out idsPedidos, out idsPedidosErro, false, false);
                }
                catch (ValidacaoPedidoFinanceiroException ex)
                {
                    if (!ex.Message.Contains("demais pedidos"))
                        throw;
                    
                    PedidoDAO.Instance.DisponibilizaConfirmacaoFinanceiro(ex.IdsPedidos, Glass.MensagemAlerta.FormatErrorMsg("", ex));
                }
                catch (Exception ex1)
                {
                    if (!ex1.Message.Contains("demais pedidos"))
                        throw;
                    
                    erroConf = ex1;
                }

                // Altera a data de entrega dos pedidos antes de enviá-los para conferência,
                // para que a data de entrega da fábrica fique correta
                if (alterarDataEntrega && dataEntrega != null)
                    PedidoDAO.Instance.AlteraDataEntrega(idsPedidos, dataEntrega.Value);

                // Se estiver marcado para gerar espelho, gera conferência já finalizada deste pedido
                if (gerarEspelho)
                {
                    var ids = idsPedidos.Split(',');
                    foreach (var s in ids)
                    {
                        try
                        {
                            var idPedido = Glass.Conversoes.StrParaUint(s);
                            PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao(idPedido);

                            if (ids.Length > 1 || Glass.Configuracoes.PedidoConfig.TelaConfirmaPedidoLiberacao.FinalizarPedidoAoGerarEspelho ||
                                (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas)))
                            {
                                var idOrcamento = PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao(idPedido);

                                if (idOrcamento > 0)
                                    idsOrcamentoGerados += idOrcamento + ",";
                            }
                            else
                            {
                                scriptExecutar = "redirectUrl('../Cadastros/CadPedidoEspelho.aspx?idPedido=" + idPedido +
                                                 "');";
                                return;
                            }
                        }
                        catch (ValidacaoPedidoFinanceiroException f)
                        {
                            throw f;
                        }
                        catch (Exception ex)
                        {
                            erroConferencia += string.Format("{0}, {1} {2}", s, ex.Message, ex.InnerException);                            
                        }
                    }
                }

                var mensagemFinal = erroConf == null ? string.Format("Pedidos confirmados. {0}",
                    string.IsNullOrEmpty(erroConferencia) ? "" : string.Format(" Erro com os seguintes pedidos: {0}", erroConferencia)) :
                    Glass.MensagemAlerta.FormatErrorMsg("", erroConf, false);

                if (!string.IsNullOrEmpty(idsOrcamentoGerados))
                    mensagemFinal += " Orçamento(s) gerado(s): " + idsOrcamentoGerados.TrimEnd(',');

                scriptExecutar = @"
                    alert('" + mensagemFinal + @"');
                    openRptConf('" + idsPedidos + "');";
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
                uint idPedido = uint.TryParse(idsPedidosErro.Split(',')[0], out idPedido) ? idPedido : 0;
                var isSinal = PedidoDAO.Instance.TemSinalReceber(idPedido);

                scriptExecutar = "alert('" + mensagemFinal + "');";

                if (UserInfo.GetUserInfo.IsFinanceiroReceb)
                    scriptExecutar += "if (" + idPedido + " > 0 && confirm('Deseja receber o " + (isSinal ? "sinal" : "pagamento antecipado") + @" do pedido " + idPedido + @"?'))
                    redirectUrl('../Cadastros/CadReceberSinal.aspx?idPedido=" + idPedido + (!isSinal ? "&antecipado=1" : "") + "');\n";
            }
        }
    }
}
