using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelContaBanco : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request["tipo"])
            {
                case "1": // Cheque reapresentado
                    lblSubtitulo.Text = "Indique a conta onde o cheque será depositado";
                    break;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdContaBanco.PageIndex = 0;
        }
    
        protected void grdContaBanco_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Selecionar")
            {
                uint idContaBanco = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                switch (Request["tipo"])
                {
                    case "1": // Cheque reapresentado
                        uint idCheque = Glass.Conversoes.StrParaUint(Request["idCheque"]);
                        ChequesDAO.Instance.AlteraContaBancoCheque(idCheque, idContaBanco);
    
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", @"
                            window.opener.reapresentar();
                            closeWindow(); ", true);
    
                        break;
                }
            }
        }
    }
}
