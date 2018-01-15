using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCanc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetMotivoCanc));
    
            uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(idPedido);
    
            // A opção de gerar crédito para o cliente só pode ser usada se o pedido tiver sido totalmente recebido e se for à vista,
            // caso contrário, o crédito gerado poderá ser diferente do valor já pago pelo cliente ou o acerto poderá ser cancelado posteriormente
            gerarCredito.Visible = !PedidoConfig.LiberarPedido && 
                PedidoDAO.Instance.ObtemTipoVenda(idPedido) == (int)Data.Model.Pedido.TipoVendaPedido.AVista &&
                // Se a FormaPagto for diferente de Cartão
                PedidoDAO.Instance.ObtemFormaPagto(idPedido) != 5 &&
                (CaixaDiarioDAO.Instance.ExistsByPedido(idPedido) ||
                CaixaGeralDAO.Instance.ExistsByPedido(idPedido) || MovBancoDAO.Instance.ExistsByPedido(idPedido) ||
                ContasReceberDAO.Instance.ExistsByPedido(idPedido)) && !ClienteDAO.Instance.IsConsumidorFinal(idCliente);
    
            if (!IsPostBack)
            {
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = false;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string ValidaPedido(string idPedido)
        {
            if (PedidoExportacaoDAO.Instance.IsPedidoExportado(Glass.Conversoes.StrParaUint(idPedido)) &&
                PedidoExportacaoDAO.Instance.GetSituacaoExportacao(Glass.Conversoes.StrParaUint(idPedido)) != PedidoExportacao.SituacaoExportacaoEnum.Cancelado)
                return "ESTE PEDIDO FOI EXPORTADO, AO CANCELÁ-LO O MESMO NÃO SERÁ CANCELADO NA FÁBRICA, TEM CERTEZA QUE DESEJA PROSSEGUIR?";
    
            return String.Empty;
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Glass.Data.Model.Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdPedido"]));
    
            // Concatena a observação do pedido já existente com o motivo do cancelamento
            string motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
            ped.Obs = !String.IsNullOrEmpty(ped.Obs) ? ped.Obs + " " + motivo : motivo;
    
            // Se o tamanho do campo Obs exceder 1000 caracteres, salva apenas os 1000 primeiros, descartando o restante
            ped.Obs = ped.Obs.Length > 1000 ? ped.Obs.Substring(0, 1000) : ped.Obs;
    
            try
            {
                if (ped.Importado)
                {
                    string urlSistema = ClienteDAO.Instance.ObtemUrlSistema(ped.IdCli);
    
                    if (!string.IsNullOrEmpty(urlSistema))
                    {
                        string urlService = urlSistema.ToLower().Replace("webglass", "service/wsexportacaopedido.asmx").TrimEnd('/');
    
                        object[] parametros = new object[] { LojaDAO.Instance.ObtemCnpj(UserInfo.GetUserInfo.IdLoja), 2, Glass.Conversoes.StrParaInt(ped.CodCliente) };
    
                        object retorno = WebService.ChamarWebService(urlService, "SyncService", "CancelarPedido", parametros);
    
                        string[] dados = retorno as string[];
    
                        if (dados[0] == "1")
                            throw new Exception("Ocorreu um erro: " + dados[1] + ".");
                    }
                    else
                    {
                        throw new Exception("Atenção: Para pedidos importados é necessário o preenchimento da URL do sistema do cliente no cadastro do mesmo.");
                    }
                }
    
                PedidoDAO.Instance.CancelarPedidoComTransacao(ped.IdPedido, ped.Obs, chkGerarCredito.Checked, false, ctrlDataEstorno.Data);
            }
            catch (Glass.Data.Exceptions.ComissaoGeradaException ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "confirmar", "if (!confirm('" + Glass.MensagemAlerta.FormatErrorMsg(null, ex) + "')) closeWindow(); " +
                    "else confirmarCancelamento(" + ped.IdPedido + ", " + chkGerarCredito.Checked.ToString().ToLower() + ", '" + motivo + "', '" + ctrlDataEstorno.Data + "');", true);
    
                return;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.location.href=window.opener.location.href;closeWindow();", true);
        }
    
        [Ajax.AjaxMethod]
        public string Confirmar(string idPedido, string gerarCredito, string motivo, string data)
        {
            try
            {
                PedidoDAO.Instance.CancelarPedidoComTransacao(Glass.Conversoes.StrParaUint(idPedido), motivo, gerarCredito == "true", true, DateTime.Parse(data));
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar pedido.", ex);
            }
        }
    }
}
