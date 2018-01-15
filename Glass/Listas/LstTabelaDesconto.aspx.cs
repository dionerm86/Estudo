using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstTabelaDesconto : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdTabelaDesconto.Register(false, true);
            odsTabelaDesconto.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            var novo = new Glass.Global.Negocios.Entidades.TabelaDescontoAcrescimoCliente();
            novo.Descricao = ((TextBox)grdTabelaDesconto.FooterRow.FindControl("txtDescricao")).Text;

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IClienteFluxo>();

            var res = fluxo.SalvarTabelaDescontoAcrescimo(novo);
            if (!res)
                Glass.MensagemAlerta.ShowMsg("Falha ao inserir. " + res.Message, Page);
    
            grdTabelaDesconto.DataBind();
        }
    
        protected void grdTabelaDesconto_DataBound(object sender, EventArgs e)
        {
            if (grdTabelaDesconto.Rows.Count == 1)
                grdTabelaDesconto.Rows[0].Visible = TabelaDescontoAcrescimoClienteDAO.Instance.GetCountReal() > 0;
            else if (grdTabelaDesconto.Rows.Count > 0)
                grdTabelaDesconto.Rows[0].Visible = true;
        }
    }
}
