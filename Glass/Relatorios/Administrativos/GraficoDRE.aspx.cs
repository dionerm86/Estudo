using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Collections;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Data;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoDRE : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (DateTime.Now.Month != 1)
                    ((TextBox)ctrlDataIni.FindControl("txtData")).Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 01).ToString("dd/MM/yyyy");
                else
                    ((TextBox)ctrlDataIni.FindControl("txtData")).Text = new DateTime(DateTime.Now.Year - 1, 12, 1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("dd/MM/yyyy");
            }
    
            GeraGrafico();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void drpCategoriaConta_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpGrupoConta.Items.Clear();
            drpGrupoConta.DataBind();
    
            drpGrupoConta.Items.Insert(0, new ListItem("Todos", "0"));
        }
    
        private void GeraGrafico()
        {
            #region Declarações
    
            uint idCategoriaConta = Convert.ToUInt32(drpCategoriaConta.SelectedValue);
            uint idGrupoConta = Convert.ToUInt32(drpGrupoConta.SelectedValue);
            uint idPlanoConta = drpPlanoConta.SelectedValue == "" ? 0 : Convert.ToUInt32(drpPlanoConta.SelectedValue);
            uint idLoja = Convert.ToUInt32(drpLoja.SelectedValue);
            string dataIni = ((TextBox)ctrlDataIni.FindControl("txtData")).Text;
            string dataFim = ((TextBox)ctrlDataFim.FindControl("txtData")).Text;
            string grupos = hdfGrupos.Value;
            int tipoMov = Convert.ToInt32(drpTipoMov.SelectedValue);
            int tipoConta = Convert.ToInt32(drpContabil.SelectedValue);
            bool ajustado = chkAjustado.Checked;
    
            ArrayList lstValuesX = new ArrayList();
            ArrayList lstValuesY = new ArrayList();
    
            List<string> periodo = new List<string>();
            DateTime periodoIni = DateTime.Parse(((TextBox)ctrlDataIni.FindControl("txtData")).Text);
            DateTime periodoFim = DateTime.Parse(((TextBox)ctrlDataFim.FindControl("txtData")).Text).AddDays(1);
            Dictionary<string, string> seriesLista = new Dictionary<string, string>();
    
            // Recupera os IDs dos filtros e preenche o hdfSeries
            List<uint> ids = new List<uint>();
    
            Series series = null;
    
            #endregion
    
            #region Período
    
            while (periodoIni < periodoFim)
            {
                seriesLista.Add(periodoIni.ToString("MMM-yy"), periodoIni.ToString("MMM-yy") + "|");
                periodo.Add(periodoIni.ToString("MMM-yy"));
                periodoIni = periodoIni.AddMonths(+1);
            }
    
            #endregion
    
            #region Gráfico
    
            Chart1.ChartAreas.Clear();
            //Cria a área do gráfico
            Chart1.Width = 1000;
            Chart1.Height = 300;
            Chart1.ChartAreas.Add("Area");
            Chart1.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
            Chart1.ChartAreas[0].Position.Height = 90;
            Chart1.ChartAreas[0].Position.Width = 77;
            Chart1.ChartAreas[0].Position.Y = 5;
            Chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{C}";
            //Titulo
            Chart1.Titles.Add("");
            Chart1.Titles[0].Alignment = ContentAlignment.TopCenter;
            Chart1.Titles[0].Docking = Docking.Top;
            Chart1.Titles[0].IsDockedInsideChartArea = false;
            Chart1.Titles[0].Position.X = 13;
            Chart1.Titles[0].Position.Y = 30;
            Chart1.Titles[0].Position.Width = 63;
            Chart1.Titles[0].Position.Height = 100;
            Chart1.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
            Chart1.ChartAreas[0].AxisX.Title = "Período";
            Chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            Chart1.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 11, FontStyle.Italic);
    
            Chart1.Legends.Clear();
            Chart1.Legends.Add("legenda");
            Chart1.Legends[0].BorderColor = Color.Black;
            Chart1.Legends[0].BorderWidth = 1;
            Chart1.Legends[0].BorderDashStyle = ChartDashStyle.Solid;
            Chart1.Legends[0].ShadowOffset = 1;
            Chart1.Legends[0].LegendStyle = LegendStyle.Table;
            Chart1.Legends[0].TableStyle = LegendTableStyle.Auto;
            Chart1.Legends[0].Docking = Docking.Right;
            Chart1.Legends[0].Alignment = StringAlignment.Far;
            Chart1.Legends[0].Enabled = true;
            Chart1.Legends[0].Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
            Chart1.Legends[0].AutoFitMinFontSize = 5;
    
            #endregion
    
            #region Series
    
            Loja[] lojas = drpLoja.SelectedValue == "0" ? LojaDAO.Instance.GetAll() : new Loja[] { LojaDAO.Instance.GetElementByPrimaryKey(idLoja) };
    
            Chart1.ChartAreas[0].AxisY.Interval = 300000;
            Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
    
            foreach (Loja l in lojas)
            {
                ids.Add((uint)l.IdLoja);
    
                if (Chart1.Series.FindByName(l.IdLoja.ToString()) == null)
                {
                    series = new Series(l.IdLoja.ToString());
                    series.ChartType = SeriesChartType.Line;
                    //series.XValueType = ChartValueType.String;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.MarkerSize = 8;
                    series.MarkerColor = series.BorderColor;
                    series.BorderWidth = 3;
                    //Chart1.Series[i].IsValueShownAsLabel = true;
                    series.ToolTip = "#VALX" + Environment.NewLine + l.NomeFantasia + " : #VALY{C}";
                    series.Legend = "legenda";
                    series.LegendText = l.NomeFantasia;
                    //series.IsVisibleInLegend = true;
                    series.LegendToolTip = l.NomeFantasia;
                    Chart1.Series.Add(series);
                }
            }
    
            #endregion
    
            #region Dados
    
            Dictionary<uint, List<ChartDRE>> dados = ChartDREDAO.Instance.ObterDados(idCategoriaConta, idGrupoConta, idPlanoConta, dataIni, 
                dataFim, tipoMov, tipoConta, ajustado, ids);
    
            foreach (KeyValuePair<uint, List<ChartDRE>> entry in dados)
                foreach (ChartDRE ch in entry.Value)
                    seriesLista[ch.Periodo] += ch.Total + "|";
    
            List<ChartDRE> listaDRE = new List<ChartDRE>();
    
            foreach (KeyValuePair<uint, List<ChartDRE>> itens in dados)
                foreach (ChartDRE c in itens.Value)
                    listaDRE.Add(c);
    
            foreach (Series s in Chart1.Series)
            {
                lstValuesX = new ArrayList();
                lstValuesY = new ArrayList();
    
                for (int i = 0; i < listaDRE.Count; i++)
                {
                    if (listaDRE[i].IdLoja.ToString() == s.Name)
                    {
                        lstValuesX.Add(Convert.ToDateTime(listaDRE[i].Periodo).ToString("MMM-yy"));
                        lstValuesY.Add(listaDRE[i].Total);
                    }
                }
    
                s.Points.DataBindXY(lstValuesX, lstValuesY);
            }
            #endregion
    
            byte[] buffer = Util.Helper.ChartToByteArray(Chart1);
    
            hdfTempFile.Value = Glass.Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "Vendas" + ((TextBox)ctrlDataIni.FindControl("txtData")).Text.Replace("/", "") + ((TextBox)ctrlDataFim.FindControl("txtData")).Text.Replace("/", "")));
    
            #region Apos gerar imagem
    
            foreach (Series s in Chart1.Series)
            {
                lstValuesX = new ArrayList();
                lstValuesY = new ArrayList();
    
                for (int i = 0; i < listaDRE.Count; i++)
                {
                    if (listaDRE[i].IdLoja.ToString() == s.Name)
                    {
                        lstValuesX.Add(Convert.ToDateTime(listaDRE[i].Periodo).ToString("MMM-yy"));
                        lstValuesY.Add(listaDRE[i].Total);
                    }
                }
    
                s.Points.DataBindXY(lstValuesX, lstValuesY);
            }
    
            #endregion
    
            #region Grid
    
            Grid(dados, seriesLista);
    
            #endregion
        }
    
        private void Grid(Dictionary<uint, List<ChartDRE>> dre, Dictionary<string, string> series)
        {
            #region DataTable -> Gridview
    
            grdDRE.Columns.Clear();
    
            if (dre.Count > 0)
            {
                DataTable dt = new DataTable();
    
                #region Colunas
    
                DataColumn dcol = new DataColumn("Loja");
                dt.Columns.Add(dcol);
    
                Dictionary<int, List<decimal>> dictTotais = new Dictionary<int, List<decimal>>();
                int totalIndex = 0;
                foreach (KeyValuePair<string, string> entry in series)
                {
                    DataColumn dcolserie = new DataColumn(entry.Key, typeof(System.String));
                    dt.Columns.Add(dcolserie);
                    dictTotais.Add(totalIndex, new List<decimal>());
                    totalIndex++;
                }
    
                DataColumn dcoltotal = new DataColumn("Total", typeof(System.String));
                dt.Columns.Add(dcoltotal);
                dictTotais.Add(totalIndex, new List<decimal>());
    
                #endregion
    
                foreach (KeyValuePair<uint, List<ChartDRE>> entry in dre)
                {
                    DataRow dr = dt.NewRow();
                    dr["Loja"] = entry.Value[0].NomeLoja;
    
                    decimal total = 0;
                    for (int i = 0; i < series.Count; i++)
                    {
                        total += entry.Value[i].Total;
                        dictTotais[i].Add(entry.Value[i].Total);
                        dr[entry.Value[i].Periodo] = entry.Value[i].Total.ToString("C");
                    }
    
                    dr["Total"] = total.ToString("C");
                    dictTotais[series.Count].Add(total);
    
                    dt.Rows.Add(dr);
                }
    
                DataRow drTotais = dt.NewRow();
                drTotais["Loja"] = "Total";
    
                for (int i = 0; i <= series.Count; i++)
                {
                    decimal total = 0;
                    foreach (decimal d in dictTotais[i])
                        total += d;
    
                    drTotais[(i + 1)] = total.ToString("C");
                }
    
                dt.Rows.Add(drTotais);
    
                foreach (DataColumn dc in dt.Columns)
                {
                    BoundField bfield = new BoundField();
                    bfield.DataField = dc.ColumnName;
                    bfield.HeaderText = dc.ColumnName;
                    grdDRE.Columns.Add(bfield);
                }
    
                grdDRE.DataSource = dt;
                grdDRE.DataBind();
    
            }
    
            #endregion
        }
    }
}

