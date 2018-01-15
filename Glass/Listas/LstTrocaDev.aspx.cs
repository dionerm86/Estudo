using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstTrocaDev : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (Request["popup"] == "1")
                Page.ClientScript.RegisterStartupScript(GetType(), "troca", "hidePopup();", true);

            // A opção de agrupar o relatório pelo funcionário associado ao cliente poderá ser selecionada somente se 
            // a opção de agrupar o relatório pelo funcionário estiver desmarcada.
            chkAgruparFuncionarioAssociadoCliente.Checked = chkAgruparFuncionarioAssociadoCliente.Checked && !chkAgruparFunc.Checked;

            lkbInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdTroca.PageIndex = 0;
        }
    
        protected void lkbInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadTrocaDev.aspx" + (!String.IsNullOrEmpty(Request["popup"]) ? "?popup=1" : ""));
        }
    }
}
