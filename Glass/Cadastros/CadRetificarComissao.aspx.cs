using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarComissao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);
    
                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
            }
            
            grdComissao.Columns[4].Visible = drpTipo.SelectedIndex == 2;
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetificarComissao));
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
            else if (drpTipo.SelectedIndex == 2) 
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

            hdfNome.Value = "";
            drpNome.DataBind();
            imgPesq_Click(null, new ImageClickEventArgs(0, 0));
        }
    
        protected void grdComissao_DataBound(object sender, EventArgs e)
        {
            gerarComissao.Visible = grdComissao.Rows.Count > 0;
    
            if (grdComissao.Rows.Count == 0)
                grdComissao.EmptyDataText = "Não há comissões para o filtro especificado.";
        }
    
        protected void btnRetificarComissao_Click(object sender, EventArgs e)
        {
            string idPedido = string.Empty;
    
            // Pega o id dos Pedidos que serão removidos da comissão já paga
            foreach (GridViewRow r in grdComissao.Rows)
                if (!((CheckBox)r.FindControl("chkSel")).Checked)
                    idPedido += ((HiddenField)r.FindControl("hdfIdPedido")).Value + ",";
    
            try
            {
                decimal valorComissao = decimal.Parse(hdfValorComissao.Value);
                Glass.Data.Model.Pedido.TipoComissao tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaUint(drpTipo.SelectedValue);
    
                ComissaoDAO.Instance.RetificarComissao(Glass.Conversoes.StrParaUint(drpIdComissao.SelectedValue), tipo, Glass.Conversoes.StrParaUint(drpNome.SelectedValue),
                    idPedido.TrimEnd(','), valorComissao, ((TextBox)ctrlDataComissao.FindControl("txtData")).Text);
    
                Glass.MensagemAlerta.ShowMsg("Comissão retificada!", Page);
    
                drpIdComissao.DataBind();
                grdComissao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar comissão.", ex, Page);
            }
        }
    
        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            Glass.Data.Model.Pedido p = linha != null ? linha.DataItem as Glass.Data.Model.Pedido : null;
    
            if (p == null)
                return;
    
            ((CheckBox)sender).Attributes.Add("ValorPagoComissao", p.ValorPagoComissao.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("ValorPedido", p.Total.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("IdPedido", p.IdPedido.ToString().Replace(",", "."));
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
    
            grdComissao.DataBind();
        }
    
        protected void odsComissoesFunc_Selecting(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount)
                return;
    
            while (drpIdComissao.Items.Count > 1)
                drpIdComissao.Items.RemoveAt(1);
        }
    
        protected void drpIdComissao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpIdComissao.SelectedValue == "")
                return;
    
            uint idComissao = Glass.Conversoes.StrParaUint(drpIdComissao.SelectedValue);
            ContasPagar conta = ContasPagarDAO.Instance.GetByComissao(idComissao);
    
            if (conta != null)
                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = conta.DataVenc.ToString("dd/MM/yyyy");
        }
    }
}
