using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadParcelas : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvParcela.Register("~/Listas/LstParcelas.aspx");
            odsParcelas.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idParcela"] != null)
                dtvParcela.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstParcelas.aspx");
        }

        protected void dtvParcela_Load(object sender, EventArgs e)
        {
            dtvParcela.Rows[3].Visible = Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela;
        }
    }
}
