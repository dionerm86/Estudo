using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCustoFixoCancelar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCustoFixoCancelar));
    
            if (txtData.Text != "")
            {
                if (!dados.Visible)
                {
                    dados.Visible = true;
                    dados.DataBind();
                    btnCancelar.Visible = true;
                }
            }
            else
            {
                dados.Visible = false;
                btnCancelar.Visible = false;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Cancelar(string mesAno)
        {
            try
            {
                ContasPagarDAO.Instance.CancelaCustoFixo(mesAno);
    
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar contas a pagar.", ex);
            }
        }
    
        protected void grdContasPagar_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                try
                {
                    uint idContaPg = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    ContasPagarDAO.Instance.CancelaCustoFixo(idContaPg);
    
                    Glass.MensagemAlerta.ShowMsg("Custo fixo cancelado.", Page);
                    grdContasPagar.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar custo fixo.", ex, Page);
                }
            }
        }
    }
}
