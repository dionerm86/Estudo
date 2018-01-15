using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstComissaoCalcular : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);
    
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = inicial.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = final.ToString("dd/MM/yyyy");
            }
    
            grdComissao.Columns[1].Visible = drpTipo.SelectedIndex == 0;
            grdComissao.Columns[2].Visible = drpTipo.SelectedIndex == 1;
            grdComissao.Columns[3].Visible = drpTipo.SelectedIndex == 2;
            grdComissao.Columns[6].Visible = drpTipo.SelectedIndex == 2;
    
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

            while (drpNome.Items.Count > 1)
                drpNome.Items.RemoveAt(1);
    
            hdfNome.Value = "";
            drpNome.DataBind();
            imgPesq_Click(null, new ImageClickEventArgs(0, 0));
        }
    
        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            Glass.Data.Model.Pedido p = linha != null ? linha.DataItem as Glass.Data.Model.Pedido : null;
    
            if (p == null)
                return;
    
            ((CheckBox)sender).Enabled = p.ComissaoAPagar;
            ((CheckBox)sender).Attributes.Add("ValorComissao", p.ValorComissaoPagar.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("ValorPedido", p.Total.ToString().Replace(",", "."));
        }
    
        protected void drpNome_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfNome.Value = drpNome.SelectedValue;
        }
    
        protected void drpNome_DataBound(object sender, EventArgs e)
        {
            if (hdfNome.Value != "")
            {
                drpNome.SelectedIndex = drpNome.Items.IndexOf(drpNome.Items.FindByValue(hdfNome.Value));
                if (drpNome.SelectedValue != hdfNome.Value)
                    hdfNome.Value = "";
            }
    
            uint idFunc = 0;
            grdComissao.Columns[9].Visible = drpTipo.SelectedIndex == 1 || (uint.TryParse(drpNome.SelectedValue, out idFunc) && ComissaoConfigDAO.Instance.IsFaixaUnica(idFunc));
    
            grdComissao.DataBind();
        }
    
        protected void drpNome_DataBinding(object sender, EventArgs e)
        {
            drpNome.Items.Clear();
            drpNome.Items.Add(new ListItem("Todos", ""));
        }
    }
}
