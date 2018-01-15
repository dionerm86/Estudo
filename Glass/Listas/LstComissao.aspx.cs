using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstComissao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            drpTipo.Items[2].Enabled = Geral.ControleInstalacao &&
                PedidoConfig.Instalacao.ComissaoInstalacao;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdComissao.PageIndex = 0;
            grdComissao.DataBind();
        }
    
        protected void drpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpNome.AppendDataBoundItems = false;
    
            if (drpTipo.SelectedIndex == 0)
            {
                drpNome.DataSourceID = "odsFuncionario";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
            else if (drpTipo.SelectedIndex == 1)
            {
                drpNome.DataSourceID = "odsComissionado";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdComissionado";
            }
            else if(drpTipo.SelectedIndex == 2)
            {
                drpNome.DataSourceID = "odsInstalador";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
            else
            {
                drpNome.DataSourceID = "odsGerente";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
    
            drpNome.DataBind();
            drpNome.Items.Insert(0, new ListItem("Todos", "0"));
    
            imgPesq_Click(null, new ImageClickEventArgs(0, 0));
        }
    
        protected void odsComissao_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir comissão.", e.Exception, Page);
            }
        }
    }
}
