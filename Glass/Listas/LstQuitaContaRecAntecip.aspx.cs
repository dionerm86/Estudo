using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstQuitaContaRecAntecip : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void grdConta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Quitar")
            {
                try
                {
                    GridViewRow linha = (GridViewRow)((Button)e.CommandSource).Parent.Parent;
                    var data = ((TextBox)linha.FindControl("txtDataQuitar")).Text;
    
                    ContasReceberDAO.Instance.ReceberContaAntecipadaComTransacao(e.CommandArgument.ToString().StrParaUint(), data);
    
                    grdConta.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Boleto quitado com sucesso!", this);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao quitar boleto antecipado.", ex, Page);
                }
            }
            else if (e.CommandName == "Cancelar")
            {
                try
                {
                    ContasReceberDAO.Instance.CancelarContaAntecipada(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
                    grdConta.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar quitação de boleto antecipado.", ex, Page);
                }
            }
        }
    
        protected void txtDataQuitar_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || ((TextBox)sender).Text == "")
                ((TextBox)sender).Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    
        protected void imgDataQuitar_Load(object sender, EventArgs e)
        {
            ((ImageButton)sender).OnClientClick = "return SelecionaData('" + ((ImageButton)sender).ClientID.Replace("img", "txt") + "', this)";
        }
    
        protected void btnQuitar_DataBinding(object sender, EventArgs e)
        {
            Button btnQuitar = (Button)sender;
            GridViewRow linha = btnQuitar.Parent.Parent as GridViewRow;
            ContasReceber item = linha.DataItem as ContasReceber;
    
            btnQuitar.CommandArgument = DataBinder.Eval(item, "IdContaR").ToString();
        }
    }
}
