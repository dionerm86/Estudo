using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Drawing;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoOrcaVendas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.Administrativos.GraficoOrcaVendas));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Today.AddMonths(-6).ToString("01/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = (DateTime.Parse(DateTime.Today.ToString("01/MM/yyyy")).AddDays(-1)).ToString("dd/MM/yyyy");
            }
            
            //força as datas a serem sempre no 1o dia do mês
            if (IsPostBack)
            {
                string dataIni = "01" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Remove(0, 2);
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = dataIni;
    
                DateTime dataFim = DateTime.Parse("01" + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Remove(0, 2)).AddMonths(1).AddDays(-1);
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = dataFim.ToString("dd/MM/yyyy");
            }
    
            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
            uint idVendedor = String.IsNullOrEmpty(drpVendedor.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpVendedor.SelectedValue);
            var situacao = !string.IsNullOrEmpty(drpSituacao.SelectedValue) ? drpSituacao.SelectedValue.Split(',').Select(f => f.StrParaInt()) : null;
            int tipoFunc = Glass.Conversoes.StrParaInt(drpTipoFunc.SelectedValue);
    
            ChartOrcaVendas[] series = ChartOrcaVendasDAO.Instance.GetOrcaVendas(idLoja, idVendedor,  situacao, tipoFunc, ((TextBox)ctrlDataIni.FindControl("txtData")).Text, ((TextBox)ctrlDataFim.FindControl("txtData")).Text);
    
            Chart1.Series[0].Name = "Orçamentos";
            Chart1.Series[1].Name = "Vendas";
    
            List<string> lstValuesX = new List<string>();
            List<double> lstValuesYO = new List<double>();
            List<double> lstValuesYV = new List<double>();
    
            foreach (ChartOrcaVendas s in series)
            {
                lstValuesX.Add(s.Periodo);
                lstValuesYO.Add((double)s.OrcamentoDouble);
                lstValuesYV.Add((double)s.VendaDouble);
            }
            
            Chart1.Titles.Add("Orçamentos x Vendas");
            Chart1.Titles[0].Alignment = System.Drawing.ContentAlignment.TopLeft;
            Chart1.Titles[0].Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
    
            Chart1.Series["Orçamentos"].Points.DataBindXY(lstValuesX, lstValuesYO);
            Chart1.Series["Orçamentos"].ToolTip = "#VALX" + Environment.NewLine + "Orçamentos: (#VALY{C})";
            Chart1.Series["Orçamentos"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column ;
            Chart1.Series["Orçamentos"].Color = Color.Blue;
    
            Chart1.Series["Vendas"].Points.DataBindXY(lstValuesX, lstValuesYV);
            Chart1.Series["Vendas"].ToolTip = "#VALX" + Environment.NewLine + "Vendas: (#VALY{C})";
            Chart1.Series["Vendas"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
            Chart1.Series["Vendas"].Color = Color.Red;
    
            Chart1.ChartAreas[0].AxisX.Title = "Período";
            Chart1.ChartAreas[0].AxisX.TitleAlignment = System.Drawing.StringAlignment.Center;
            Chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
    
            Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{C}";
    
            Chart1.Width = new Unit(700);
    
            byte[] buffer = Util.Helper.ChartToByteArray(Chart1);
    
            hdfTempFile.Value = Glass.Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "OrcamentoVenda" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Replace("/", "") + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Replace("/", "") + idLoja + idVendedor + tipoFunc + situacao));
    
            Chart1.Series["Orçamentos"].Points.DataBindXY(lstValuesX, lstValuesYO);
    
            Chart1.Series["Vendas"].Points.DataBindXY(lstValuesX, lstValuesYV);
    
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected string generateChartData()
        {
            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
            uint idVendedor = String.IsNullOrEmpty(drpVendedor.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpVendedor.SelectedValue);
            var situacao = !string.IsNullOrEmpty(drpSituacao.SelectedValue) ? drpSituacao.SelectedValue.Split(',').Select(f => f.StrParaInt()) : null;
            int tipoFunc = Glass.Conversoes.StrParaInt(drpTipoFunc.SelectedValue);
    
            ChartOrcaVendas[] series = ChartOrcaVendasDAO.Instance.GetOrcaVendas(idLoja, idVendedor, situacao, tipoFunc, ((TextBox)ctrlDataIni.FindControl("txtData")).Text, ((TextBox)ctrlDataFim.FindControl("txtData")).Text);
    
            string chartData = "";
            foreach (ChartOrcaVendas g in series)
            {
                chartData += g.Periodo + "|" + g.Orcamento + "|" + g.Venda + ";";
            }
    
            return chartData.TrimEnd(';');
        }
    }
}
