using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelDAL;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Collections;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoOrcamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ((TextBox)ctrlDataIni.FindControl("txtData")).Text == String.Empty)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-2).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
    
            GeraGrafico();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        private void GeraGrafico()
        {
            #region Filtros
    
            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
            uint idVendedor = String.IsNullOrEmpty(drpVendedor.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpVendedor.SelectedValue);
            int agrupar = Glass.Conversoes.StrParaInt(drpAgrupar.SelectedValue);
            int situacao = Glass.Conversoes.StrParaInt(drpSituacao.SelectedValue);
            DateTime dataIni = Convert.ToDateTime(((TextBox)ctrlDataIni.FindControl("txtData")).Text);
            DateTime dataFim = Convert.ToDateTime(((TextBox)ctrlDataFim.FindControl("txtData")).Text);
    
            #endregion
    
            #region Gráfico
    
            //Cria a área do gráfico
            Chart1.Width = 1000;
            Chart1.Height = 300;
            Chart1.ChartAreas.Add("Totais");
            Chart1.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
            Chart1.ChartAreas[0].Position.Height = 90;
            Chart1.ChartAreas[0].Position.Width = 77;
            Chart1.ChartAreas[0].Position.Y = 5;
            Chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.IsMarginVisible = true;
            Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{C}";
            //Titulo
            Chart1.Titles.Add("Totais");
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
            Chart1.Legends.Add("legenda");
            Chart1.ChartAreas[0].AxisY.Interval = 120000;
            Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
    
            if (agrupar == 2)
            {
                Chart1.ChartAreas[0].AxisY.Interval = 60000;
                Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
            }
    
            if (agrupar == 3)
            {
                Chart1.ChartAreas[0].AxisY.Interval = 10000;
                Chart1.ChartAreas[0].AxisY.IntervalOffset = 0;
            }
    
            #endregion
    
            var series = new Dictionary<string, List<Serie>>();
            var valores = new Dictionary<string, List<Valor>>();
            
            var dados = GraficoOrcamentosDAO.Instance.GetOrcamentos(idLoja, idVendedor, new int[] { situacao }, dataIni.ToString("dd/MM/yyyy"),
                 dataFim.ToString("dd/MM/yyyy"), agrupar, true);
    
            foreach (var g in dados)
            {
                // Variável de controle dos meses
                DateTime dataCateg = dataIni;
    
                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    // Cria o nome da série
                    string nome = FuncoesData.ObtemMes(dataCateg.Month, true) + "/" + dataCateg.ToString("yy");
                    if (!series.ContainsKey(nome))
                        series.Add(nome, new List<Serie>());
    
                    // Recupera os dados da série
                    Serie nova = new Serie();
                    nova.Loja = g.IdLoja;
                    nova.Vendedor = g.IdFunc;
                    nova.Situacao = g.Situacao;
    
                    // Adiciona a categoria passando a data
                    if (!series[nome].Contains(nova))
                        series[nome].Add(nova);
    
                    dataCateg = dataCateg.AddMonths(1);
                }
            }
    
            var dadosSeries = new List<Serie>();
    
            // Seleciona apenas os itens distintos das séries
            foreach (string k in series.Keys)
                foreach (Serie serie in series[k])
                    if (!dadosSeries.Contains(serie))
                        dadosSeries.Add(serie);
    
            foreach (Serie s in dadosSeries)
            {
                // Busca os dados que servirão para preencher as séries do gráfico
                var v = GraficoOrcamentosDAO.Instance.GetOrcamentos(agrupar == 1 ? s.Loja : idLoja, agrupar == 2 ? s.Vendedor : idVendedor,
                    agrupar == 3 ? new int[] { s.Situacao } : new int[] { situacao }, dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), agrupar, false);
    
                var dataCateg = dataIni;
    
                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    Valor novo = new Valor();
                    novo.Data = dataCateg.ToString("MM/yyyy");
                    string nome = "";
    
                    // Para cada categoria (mes/ano) do gráfico, atribui o valor correspondente
                    foreach (var g in v)
                    {
                        nome = agrupar == 1 ? g.NomeLoja : agrupar == 2 ? g.NomeVendedor : agrupar == 3 ? g.DescrSituacao : "Empresa";
                        if (!valores.ContainsKey(nome))
                            valores.Add(nome, new List<Valor>());
    
                        if (g.DataVenda != novo.Data)
                            continue;
    
                        novo.Total = g.TotalVenda;
                        if (!valores[nome].Contains(novo))
                        {
                            valores[nome].Add(novo);
                            break;
                        }
                    }
    
                    bool encontrado = false;
                    foreach (Valor val in valores[nome])
                        if (val.Data == novo.Data)
                        {
                            encontrado = true;
                            break;
                        }
    
                    if (!encontrado)
                    {
                        novo.Total = 0;
                        valores[nome].Add(novo);
                    }
    
                    dataCateg = dataCateg.AddMonths(1);
                }
            }
    
            foreach(KeyValuePair<string, List<Valor>> val in valores)
            {
                ArrayList lstValuesX = new ArrayList();
                ArrayList lstValuesY = new ArrayList();
    
                Series serie = new Series(val.Key);
                serie.ChartType = SeriesChartType.Line;
                //series.XValueType = ChartValueType.String;
                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 8;
                serie.MarkerColor = serie.BorderColor;
                serie.BorderWidth = 3;
                //Chart1.Series[i].IsValueShownAsLabel = true;
                serie.ToolTip = val.Key + ", #VALX, " + "#VALY{C}";
                serie.Legend = "legenda";
                serie.LegendText = val.Key;
                //series.IsVisibleInLegend = true;
                serie.LegendToolTip = val.Key;
                Chart1.Series.Add(serie);
    
                foreach (Valor s in val.Value)
                {
                    lstValuesX.Add(s.Data);
                    lstValuesY.Add(s.Total);
                }
    
                serie.Points.DataBindXY(lstValuesX, lstValuesY);
            }
    
            var buffer = Util.Helper.ChartToByteArray(Chart1);
    
            hdfTempFile.Value = Glass.Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "Orcamentos" + DateTime.Now.ToString("ddMMyyyyHHmmss")));      
        }
    
        private class Serie
        {
            public uint Loja = 0;
            public uint Vendedor = 0;
            public int Situacao = 0;
            public uint Cliente = 0;
    
            public override bool Equals(object obj)
            {
                if (!(obj is Serie))
                    return false;
    
                Serie comp = (Serie)obj;
                return comp.Loja == Loja && comp.Vendedor == Vendedor && comp.Situacao == Situacao &&
                    comp.Cliente == Cliente;
            }
    
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    
        private class Valor
        {
            public string Nome = string.Empty;
            public string Data = string.Empty;
            public decimal Total = 0;
    
            public override bool Equals(object obj)
            {
                if (!(obj is Valor))
                    return false;
    
                Valor comp = (Valor)obj;
                return comp.Nome == Nome && comp.Data == Data && comp.Total == Total;
            }
    
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}



