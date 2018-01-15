using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasComissionado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime dataInicio = DateTime.Now.AddMonths(-6);
                DateTime dataFim = DateTime.Now;
    
                drpInicio.SelectedIndex = dataInicio.Month - 1;
                txtInicio.Text = dataInicio.Year.ToString();
    
                drpFim.SelectedIndex = dataFim.Month - 1;
                txtFim.Text = dataFim.Year.ToString();
            }
        }
    
        protected void odsVendas_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            List<Vendas> retorno = e.ReturnValue as List<Vendas>;
    
            if (retorno == null || retorno.Count == 0)
                return;
    
            int mesInicio = Glass.Conversoes.StrParaInt(drpInicio.SelectedValue);
            int anoInicio = Glass.Conversoes.StrParaInt(txtInicio.Text);
            int mesFim = Glass.Conversoes.StrParaInt(drpFim.SelectedValue);
            int anoFim = Glass.Conversoes.StrParaInt(txtFim.Text);
            string[] mesVenda = VendasDAO.Instance.GetMesesVenda(0, null, null, false, 0, txtNome.Text, mesInicio, anoInicio, mesFim, anoFim, null, 1, null, null, 0, false, null, 0);
    
            bool mudar = mesVenda.Length + 2 != grdVendas.Columns.Count;
            if (!mudar)
                for (int i = 0; i < mesVenda.Length; i++)
                {
                    mudar = grdVendas.Columns[i + 1].HeaderText != mesVenda[i];
                    if (mudar)
                        break;
                }
    
            if (mudar)
            {
                if (grdVendas.Columns.Count > 2)
                {
                    DataControlField cliente = grdVendas.Columns[0];
                    DataControlField total = grdVendas.Columns[grdVendas.Columns.Count - 1];
                    grdVendas.Columns.Clear();
                    grdVendas.Columns.Add(cliente);
                    grdVendas.Columns.Add(total);
                }
    
                foreach (string mesAno in mesVenda)
                {
                    int index = grdVendas.Columns.Count - 1;
                    grdVendas.Columns.Insert(index, new TemplateField());
                    grdVendas.Columns[index].HeaderText = mesAno;
                    grdVendas.Columns[index].ItemStyle.Wrap = false;
                }
            }
        }
    
        protected void grdVendas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            Vendas item = e.Row.DataItem as Vendas;
            if (item == null)
                return;
    
            for (int i = 1; i < grdVendas.Columns.Count - 1; i++)
            {
                e.Row.Cells[i].Text = "";
    
                for (int j = 0; j < item.MesVenda.Length; j++)
                    if (item.MesVenda[j] == grdVendas.Columns[i].HeaderText)
                    {
                        e.Row.Cells[i].Text = item.ValorVenda[j].ToString("C");
                        break;
                    }
    
                if (String.IsNullOrEmpty(e.Row.Cells[i].Text))
                    e.Row.Cells[i].Text = 0.ToString("C");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendas.DataBind();
            grdVendas.PageIndex = 0;
        }
    }
}
