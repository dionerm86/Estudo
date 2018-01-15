using Glass.Configuracoes;
using Glass.Data.Model;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaClientesDescAcresc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDescontoAcrescimo.PageIndex = 0;
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdDescontoAcrescimo.PageIndex = 0;
        }

        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && ClienteConfig.ListarAtivosPadrao)
                drpSituacao.SelectedValue = SituacaoCliente.Ativo.ToString();
        }
    }
}
