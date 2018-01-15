using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Data;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoRecebimentosTipo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.Administrativos.GraficoRecebimentosTipo));
    
            if (!IsPostBack && ((TextBox)ctrlDataIni.FindControl("txtData")).Text == String.Empty)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
    
            GeraGrafico();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        private void GeraGrafico()
        {
            //Busca os dados que servirão para preencher as séries do gráfico
            var rt = RecebimentoDAO.Instance.GetRecebimentosTipo(((TextBox)ctrlDataIni.FindControl("txtData")).Text, 
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text, Convert.ToUInt32(drpLoja.SelectedValue), Convert.ToUInt32(drpFunc.SelectedValue));
    
            if (rt.Count() > 0)
            {
                Chart1.Series[0].Name = "RecebimentoTipo";
    
                List<string> lstValuesX = new List<string>();
                List<double> lstValuesY = new List<double>();

                lblTotal.Text = "TOTAL: R$ " + rt.Where(f=> f.Descricao ==  "TOTAL").First().Valor;                               

                foreach (Recebimento tipo in rt.Where(f=> f.Descricao != "TOTAL").ToList())
                {
                    lstValuesX.Add(tipo.Descricao);
                    lstValuesY.Add((double)tipo.Valor);
                }
    
                Chart1.Legends.Add("legenda");
                Chart1.Series["RecebimentoTipo"].Legend = "legenda";
                Chart1.Series["RecebimentoTipo"].IsValueShownAsLabel = true;
                Chart1.Series["RecebimentoTipo"].IsVisibleInLegend = false;
                Chart1.Series["RecebimentoTipo"].Points.DataBindXY(lstValuesX, lstValuesY);
                Chart1.Series["RecebimentoTipo"].ToolTip = "#VALX (#VALY{C})";
                Chart1.Series["RecebimentoTipo"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
                Chart1.Series["RecebimentoTipo"].Label = "#VALX (#VALY{C})";
                Chart1.Series["RecebimentoTipo"]["PieDrawingStyle"] = "SoftEdge";
    
                if (chkLegenda.Checked)
                {
                    Chart1.Series["RecebimentoTipo"].IsVisibleInLegend = true;
                    Chart1.Series["RecebimentoTipo"]["PieLabelStyle"] = "Disabled";
                    Chart1.Width = new Unit(700);
                }
                else
                {
                    Chart1.Series["RecebimentoTipo"].IsVisibleInLegend = false;
                    Chart1.Series["RecebimentoTipo"]["PieLabelStyle"] = "Outside";
                    Chart1.Series["RecebimentoTipo"]["PieLineColor"] = "Black";
                    Chart1.Width = new Unit(700);
                }
    
                byte[] buffer = Util.Helper.ChartToByteArray(Chart1);
    
                hdfTempFile.Value = Glass.Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "RecebimentosTipo" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Replace("/", "") + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Replace("/", "") + drpLoja.SelectedValue + drpFunc.SelectedValue));
                //Atribuo o databind de novo para exibir corretamente o label
                Chart1.Series["RecebimentoTipo"].Points.DataBindXY(lstValuesX, lstValuesY);
            }
        }
    
        protected void lnkImprimir_Click(object sender, EventArgs e)
        {
        }
        protected void chkLegenda_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}
