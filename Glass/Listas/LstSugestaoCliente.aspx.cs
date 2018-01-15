using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstSugestaoCliente : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdSugestao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstSugestaoCliente));

            if (!string.IsNullOrEmpty(Request["idCliente"]))
            {
                lblCliente.Text = ClienteDAO.Instance.GetNome(Conversoes.StrParaUint(Request["idCliente"]));
                hdfIdCli.Value = Request["idCliente"];
                txtNome.Visible = txtNumCli.Visible = lblNome.Visible = lblNumCli.Visible = false;
                txtNumCli.Text = Request["idCliente"];
            }
            else if (!string.IsNullOrEmpty(Request["idPedido"]))
            {
                hdfIdPedido.Value = Request["idPedido"];
                lblPedido.Visible = true;
                txtPedido.Visible = true;
                btnPedido.Visible = true;
                txtPedido.Text = Request["idPedido"];
                // Desabilita filtro Orçamento
                hdfIdOrcamento.Value = "0";
                lblOrcamento.Visible = false;
                txtOrcamento.Visible = false;
                imbOrcamento.Visible = false;
            }
            else if (!string.IsNullOrEmpty(Request["idOrcamento"]))
            {
                hdfIdOrcamento.Value = Request["idOrcamento"];
                lblOrcamento.Visible = true;
                txtOrcamento.Visible = true;
                imbOrcamento.Visible = true;
                txtOrcamento.Text = Request["idOrcamento"];
                // Desabilita filtro Pedido
                hdfIdPedido.Value = "0";
                lblPedido.Visible = false;
                txtPedido.Visible = false;
                btnPedido.Visible = false;
            }
            else
            {
                hdfIdPedido.Value = "0";
                lblPedido.Visible = false;
                txtPedido.Visible = false;
                btnPedido.Visible = false;
                hdfIdOrcamento.Value = "0";
                lblOrcamento.Visible = false;
                txtOrcamento.Visible = false;
                imbOrcamento.Visible = false;

                hdfIdCli.Value = "0";
                
                lblInfo.Visible = lblCliente.Visible = btnVoltar.Visible = false;
            }

            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);

            grdSugestao.DataBind();
        }

        /// <summary>
        /// Verifica se pode apagar a sugestão
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["idCliente"]))
                Response.Redirect("~/Cadastros/CadSugestaoCliente.aspx?idCliente=" + Request["idCliente"]);
            else if (!string.IsNullOrEmpty(Request["idPedido"]))
                Response.Redirect("~/Cadastros/CadSugestaoCliente.aspx?idPedido=" + Request["idPedido"]);
            else if (!string.IsNullOrEmpty(Request["idOrcamento"]))
                Response.Redirect("~/Cadastros/CadSugestaoCliente.aspx?idOrcamento=" + Request["idOrcamento"]);
            else
                Response.Redirect("~/Cadastros/CadSugestaoCliente.aspx");
        }
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCliente.aspx");
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetFunc(string idFunc)
        {
            if (String.IsNullOrEmpty(idFunc) || !FuncionarioDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idFunc)))
                return "Erro;Funcionário não encontrado.";
            else
                return "Ok;" + FuncionarioDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idFunc));
        }
    
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
        #endregion
        
        protected void imgPesq_click(object sender, ImageClickEventArgs e)
        {
            grdSugestao.PageIndex = 0;
        }
        protected void grdSugestao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ISugestaoFluxo>();
                var sugestao = fluxo.ObtemSugestao(Convert.ToInt32(e.CommandArgument));
                sugestao.Cancelada = true;

                var resultado = fluxo.SalvarSugestao(sugestao);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Não foi possível cancelar a sugestão.", resultado);
                else
                    grdSugestao.DataBind();
            }
        }
        protected void grdSugestao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton excluir = (LinkButton)e.Row.FindControl("lnkExcluir");
                HiddenField hdfIdFunc = (HiddenField)e.Row.FindControl("hdfIdFunc");

                if (UserInfo.GetUserInfo.IsAdministrador ||
                    (hdfIdFunc != null && hdfIdFunc.Value.StrParaUint() == UserInfo.GetUserInfo.CodUser))
                    excluir.Visible = true;
                else
                    excluir.Visible = false;
            }
        }
    }
}
