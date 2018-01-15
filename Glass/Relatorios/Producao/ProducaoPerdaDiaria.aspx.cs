using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Glass.Data.RelModel;
using System.Collections;
using Glass.Data.Helper;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoPerdaDiaria : System.Web.UI.Page
    {
        private GraficoProdPerdaDiaria[] _dadosProducaoPerda = null;
    
        private GraficoProdPerdaDiaria[] _dadosPerdaSetores = null;
    
        private Dictionary<string, double> _dadosPerdaMensal = null;
    
        private GraficoProdPerdaDiaria[] _dados10mm = null;
    
        private GraficoProdPerdaDiaria[] _dados8mm = null;
    
        private GraficoProdPerdaDiaria[] _dados6mm = null;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            
            if (!IsPostBack)
            {
                this.MontarComboAno();
                this.drpMes.SelectedValue = DateTime.Today.AddMonths(-1).Month.ToString();

                if (DateTime.Today.Month == 1)
                    this.drpAno.SelectedValue = DateTime.Today.AddYears(-1).Year.ToString();
            }
        }
    
        private void MontarComboAno()
        {
            ListItem objListItem;
            int menorAno = Glass.Data.DAL.FuncionarioDAO.Instance.GetMenorAnoCadastro();
    
            for (int anoAtual = DateTime.Today.Year ; anoAtual >= menorAno; anoAtual--)
            {
                objListItem = new ListItem(anoAtual.ToString(), anoAtual.ToString());
                this.drpAno.Items.Add(objListItem);
            }
        }
    
        protected void imbPesquisar_Click(Object sender, EventArgs e)
        {
            this.MontarGraficos();
            this.PreencherHiddens();
        }
    
        private GraficoProdPerdaDiaria[] GetDados()
        {
            this._dadosProducaoPerda = Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetForRpt(this.drpSetor.SelectedValue,
                this.drpMes.SelectedValue, this.drpAno.SelectedValue);
    
            return this._dadosProducaoPerda;
        }
    
        private GraficoProdPerdaDiaria[] GetDadosSetores()
        {
            this._dadosPerdaSetores = Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetPerdaSetores(Convert.ToInt32(this.drpSetor.SelectedValue),
                this.drpMes.SelectedValue, this.drpAno.SelectedValue);
    
            return this._dadosPerdaSetores;
        }
    
        private Dictionary<string, double> GetDadosPerdaMensal()
        {

            _dadosPerdaMensal = Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetPerda(this.drpMes.SelectedValue,
                this.drpAno.SelectedValue, true, false);
    
            return _dadosPerdaMensal;
        }
    
        private void GetDadosEspessuraProduto()
        {
            Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetPerdaProduto(Convert.ToInt32(drpSetor.SelectedValue), chkIncluirTrocaDevolucao.Checked,
                drpMes.SelectedValue, drpAno.SelectedValue, out _dados6mm, out _dados8mm, out _dados10mm);
        }
    
        private void MontarGraficoProducaoAcumulada(GraficoProdPerdaDiaria[] dados)
        {
            if (dados != null && dados.Length > 0)
            {
                this.chtProdAcumulada.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series seriesProducao = null;
                Series seriesMedia = null;
    
                // Arrays de Dados
                ArrayList lstDias = new ArrayList();
                ArrayList lstValuesMedia = new ArrayList();
                ArrayList lstValuesProducao = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtProdAcumulada.Width = 1200;
                this.chtProdAcumulada.Height = 500;
                this.chtProdAcumulada.BorderlineColor = System.Drawing.Color.Black;
                this.chtProdAcumulada.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtProdAcumulada.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtProdAcumulada.ChartAreas.Add("Area");
                this.chtProdAcumulada.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
                this.chtProdAcumulada.ChartAreas[0].Position.Height = 90;
                this.chtProdAcumulada.ChartAreas[0].Position.Width = 80;
                this.chtProdAcumulada.ChartAreas[0].Position.Y = 8;
    
                // Configurações do Eixo Y
                this.chtProdAcumulada.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProdAcumulada.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProdAcumulada.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtProdAcumulada.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProdAcumulada.ChartAreas[0].AxisX.Interval = 1;
                //this.chtProdAcumulada.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo X
                this.chtProdAcumulada.ChartAreas[0].AxisY.Interval = 2000;
                this.chtProdAcumulada.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProdAcumulada.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProdAcumulada.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtProdAcumulada.ChartAreas[0].AxisY.IsMarginVisible = true;
    
                #endregion
    
                #region Titulo
    
                this.chtProdAcumulada.Titles.Add("ProducaoAcumulada");
                this.chtProdAcumulada.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtProdAcumulada.Titles[0].Docking = Docking.Top;
                this.chtProdAcumulada.Titles[0].IsDockedInsideChartArea = false;
                this.chtProdAcumulada.Titles[0].Position.X = 13;
                this.chtProdAcumulada.Titles[0].Position.Y = 30;
                this.chtProdAcumulada.Titles[0].Position.Width = 63;
                this.chtProdAcumulada.Titles[0].Position.Height = 98;
                this.chtProdAcumulada.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtProdAcumulada.Titles[0].Text = "TEMPERA ACUMULADA DIA/MEDIA - " + this.drpMes.SelectedItem.Text.ToUpper() + "/" + this.drpAno.SelectedValue;
    
                #endregion
    
                #region Legenda
    
                this.chtProdAcumulada.Legends.Add("Producao");
                this.chtProdAcumulada.Legends.Add("Media");
    
                #endregion
    
                #region Series
    
                // Cria e configura as séries do gráfico
                seriesProducao = new Series("Produção Acumulada");
                seriesMedia = new Series("Média Diária de Produção");
    
                seriesProducao.ChartType = SeriesChartType.Bar;
                seriesMedia.ChartType = SeriesChartType.Bar;
    
                seriesProducao.XValueType = ChartValueType.Int32;
                seriesMedia.XValueType = ChartValueType.Int32;
    
                seriesProducao.YValueType = ChartValueType.String;
                seriesMedia.YValueType = ChartValueType.String;
    
                seriesProducao.IsValueShownAsLabel = true;
                seriesProducao.Font = new Font("Arial", 7, FontStyle.Bold);
                seriesMedia.IsValueShownAsLabel = true;
                seriesMedia.Font = new Font("Arial", 7, FontStyle.Bold);
    
                // Insere as séries no gráfico
                this.chtProdAcumulada.Series.Add(seriesProducao);
                this.chtProdAcumulada.Series.Add(seriesMedia);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < dados.Length; i++)
                {
                    lstDias.Add(new DateTime(Convert.ToInt32(this.drpAno.SelectedValue), Convert.ToInt32(this.drpMes.SelectedValue),
                        Convert.ToInt32(dados[i].Dia)).ToString("dd/MMM"));
                    lstValuesProducao.Add(dados[i].ProducaoAcumulada);
                    lstValuesMedia.Add(dados[i].MediaDiariaProducao);
                }
    
                // Desenha os dados no gráfico
                this.chtProdAcumulada.Series[0].Points.DataBindXY(lstDias, lstValuesProducao);
                this.chtProdAcumulada.Series[1].Points.DataBindXY(lstDias, lstValuesMedia);
    
                #endregion
            }
        }
    
        private void MontarGraficoIncidePerda(GraficoProdPerdaDiaria[] dados)
        {    
            if (dados != null && dados.Length > 0 && dados[dados.Length-1].ProducaoAcumulada > 0)
            {
                this.chtIndicePerda.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series seriesPerdaTotal = null;
                Series seriesIndicePerda = null;
    
                // Arrays de Dados
                ArrayList lstDias = new ArrayList();
                ArrayList lstValuesPerdaTotal = new ArrayList();
                ArrayList lstValuesIndicePerda = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtIndicePerda.Width = 1200;
                this.chtIndicePerda.Height = 500;
                this.chtIndicePerda.BorderlineColor = System.Drawing.Color.Black;
                this.chtIndicePerda.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtIndicePerda.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtIndicePerda.ChartAreas.Add("Area");
                this.chtIndicePerda.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
                this.chtIndicePerda.ChartAreas[0].Position.Height = 90;
                this.chtIndicePerda.ChartAreas[0].Position.Width = 80;
                this.chtIndicePerda.ChartAreas[0].Position.Y = 8;
    
                // Configurações do Eixo Y
                this.chtIndicePerda.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtIndicePerda.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtIndicePerda.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtIndicePerda.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtIndicePerda.ChartAreas[0].AxisX.Interval = 1;
    
                // Configurações do Eixo X
                //this.chtIndicePerda.ChartAreas[0].AxisY.Interval = 25;
                this.chtIndicePerda.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtIndicePerda.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtIndicePerda.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtIndicePerda.ChartAreas[0].AxisY.IsMarginVisible = true;
    
                #endregion
    
                #region Titulo
    
                this.chtIndicePerda.Titles.Add("IndicePerda");
                this.chtIndicePerda.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtIndicePerda.Titles[0].Docking = Docking.Top;
                this.chtIndicePerda.Titles[0].IsDockedInsideChartArea = false;
                this.chtIndicePerda.Titles[0].Position.X = 13;
                this.chtIndicePerda.Titles[0].Position.Y = 30;
                this.chtIndicePerda.Titles[0].Position.Width = 63;
                this.chtIndicePerda.Titles[0].Position.Height = 98;
                this.chtIndicePerda.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtIndicePerda.Titles[0].Text = "PERDA ACUMULADA DIA / INDICE DE PERDA";
    
                #endregion
    
                #region Legenda
    
                this.chtIndicePerda.Legends.Add("PerdaTotal");
                this.chtIndicePerda.Legends.Add("IndicePerda");
    
                #endregion
    
                #region Series
    
                // Cria e configura as séries do gráfico
                seriesPerdaTotal = new Series("Perda Acumulada");
                seriesIndicePerda = new Series("Índice de Perda Diária");
    
                seriesPerdaTotal.ChartType = SeriesChartType.Bar;
                seriesIndicePerda.ChartType = SeriesChartType.Bar;
    
                seriesPerdaTotal.XValueType = ChartValueType.Int32;
                seriesIndicePerda.XValueType = ChartValueType.Int32;
    
                seriesPerdaTotal.YValueType = ChartValueType.String;
                seriesIndicePerda.YValueType = ChartValueType.String;
    
                seriesPerdaTotal.IsValueShownAsLabel = true;
                seriesPerdaTotal.Font = new Font("Arial", 7, FontStyle.Bold);
                seriesIndicePerda.IsValueShownAsLabel = true;
                seriesIndicePerda.Font = new Font("Arial", 7, FontStyle.Bold);
    
                // Insere as séries no gráfico
                this.chtIndicePerda.Series.Add(seriesPerdaTotal);
                this.chtIndicePerda.Series.Add(seriesIndicePerda);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < dados.Length; i++)
                {
                    lstDias.Add(new DateTime(Convert.ToInt32(this.drpAno.SelectedValue), Convert.ToInt32(this.drpMes.SelectedValue),
                        Convert.ToInt32(dados[i].Dia)).ToString("dd/MMM"));
                    lstValuesPerdaTotal.Add(dados[i].PerdaAcumulada);
                    lstValuesIndicePerda.Add(dados[i].IndicePerdaDiaria);
                }
    
                // Desenha os dados no gráfico
                this.chtIndicePerda.Series[0].Points.DataBindXY(lstDias, lstValuesPerdaTotal);
                this.chtIndicePerda.Series[1].Points.DataBindXY(lstDias, lstValuesIndicePerda);
    
                #endregion
            }
        }
    
        private void MontarGraficoProducaoDiaria(GraficoProdPerdaDiaria[] dados)
        {
            if (dados != null && dados.Length > 0)
            {
                this.chtProducaoDiaria.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series serieProducao = null;
    
                // Arrays de Dados
                ArrayList lstDias = new ArrayList();
                ArrayList lstValuesProducao = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtProducaoDiaria.Width = 600;
                this.chtProducaoDiaria.Height = 250;
                this.chtProducaoDiaria.BorderlineColor = System.Drawing.Color.Black;
                this.chtProducaoDiaria.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtProducaoDiaria.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtProducaoDiaria.ChartAreas.Add("Area");
                this.chtProducaoDiaria.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.AxesView;
                this.chtProducaoDiaria.ChartAreas[0].Position.Height = 90;
                this.chtProducaoDiaria.ChartAreas[0].Position.Width = 90;
                this.chtProducaoDiaria.ChartAreas[0].Position.Y = 8;
                this.chtProducaoDiaria.ChartAreas[0].Position.X = 8;
    
                // Configurações do Eixo X
                this.chtProducaoDiaria.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProducaoDiaria.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProducaoDiaria.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtProducaoDiaria.ChartAreas[0].AxisX.Title = "Dia (" + drpMes.SelectedItem.Text + " - " + drpAno.SelectedValue + ")";
                this.chtProducaoDiaria.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProducaoDiaria.ChartAreas[0].AxisX.Interval = 1;
                this.chtProducaoDiaria.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                this.chtProducaoDiaria.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProducaoDiaria.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProducaoDiaria.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtProducaoDiaria.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtProducaoDiaria.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
    
                #endregion
    
                #region Titulo
    
                this.chtProducaoDiaria.Titles.Add("ProducaoDiaria");
                this.chtProducaoDiaria.Titles[0].Alignment = ContentAlignment.MiddleLeft;
                this.chtProducaoDiaria.Titles[0].Docking = Docking.Left;
                this.chtProducaoDiaria.Titles[0].IsDockedInsideChartArea = false;
                this.chtProducaoDiaria.Titles[0].Position.X = 2;
                this.chtProducaoDiaria.Titles[0].Position.Y = 50;
                this.chtProducaoDiaria.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtProducaoDiaria.Titles[0].Text = "TEMPERA DO DIAA";
                this.chtProducaoDiaria.Titles[0].TextOrientation = TextOrientation.Rotated270;
    
                #endregion
    
                #region Legenda
                // Sem legenda
                #endregion
    
                #region Series
    
                // Cria e configura a série do gráfico
                serieProducao = new Series("Perda Acumulada");
                serieProducao.ChartType = SeriesChartType.Column;
                serieProducao.XValueType = ChartValueType.String;
                serieProducao.YValueType = ChartValueType.Int32;
                serieProducao.IsValueShownAsLabel = true;
                serieProducao.Font = new Font("Arial", 7, FontStyle.Bold);
                serieProducao.IsVisibleInLegend = false;
    
                // Insere as séries no gráfico
                this.chtProducaoDiaria.Series.Add(serieProducao);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < dados.Length; i++)
                {
                    string dia = (dados[i].Dia < 10 ? "0" + dados[i].Dia.ToString() : dados[i].Dia.ToString()).ToString();
    
                    lstDias.Add(dia);
                    lstValuesProducao.Add(dados[i].TotProdM2);
                }
    
                // Desenha os dados no gráfico
                this.chtProducaoDiaria.Series[0].Points.DataBindXY(lstDias, lstValuesProducao);
    
                #endregion
            }
        }
    
        private void MontarGraficoPerdaPorSetor()
        {
            // Obtem os dados pra montar o gráfico
            GraficoProdPerdaDiaria[] lstResultado = this.GetDadosSetores();
    
            if (lstResultado != null && lstResultado.Length > 0)
            {
                this.chtPerdaSetores.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series seriePerda = null;
                Series serieDesafio = null;
                Series serieMeta = null;
    
                // Arrays de Dados
                ArrayList lstSetores = new ArrayList();
                ArrayList lstValuesPerda = new ArrayList();
                ArrayList lstValuesMeta = new ArrayList();
                ArrayList lstValuesDesafio = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtPerdaSetores.Width = 1200;
                this.chtPerdaSetores.Height = 500;
                this.chtPerdaSetores.BorderlineColor = System.Drawing.Color.Black;
                this.chtPerdaSetores.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtPerdaSetores.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtPerdaSetores.ChartAreas.Add("Area");
                this.chtPerdaSetores.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
                this.chtPerdaSetores.ChartAreas[0].Position.Height = 90;
                this.chtPerdaSetores.ChartAreas[0].Position.Width = 85;
                this.chtPerdaSetores.ChartAreas[0].Position.Y = 8;
    
                // Configurações do Eixo X
                this.chtPerdaSetores.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtPerdaSetores.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtPerdaSetores.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtPerdaSetores.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtPerdaSetores.ChartAreas[0].AxisX.Interval = 1;
                this.chtPerdaSetores.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                //this.chtIndicePerda.ChartAreas[0].AxisY.Interval = 25;
                this.chtPerdaSetores.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtPerdaSetores.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtPerdaSetores.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtPerdaSetores.ChartAreas[0].AxisY.IsMarginVisible = true;
    
                #endregion
    
                #region Titulo
    
                this.chtPerdaSetores.Titles.Add("PerdaSetores");
                this.chtPerdaSetores.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtPerdaSetores.Titles[0].Docking = Docking.Top;
                this.chtPerdaSetores.Titles[0].IsDockedInsideChartArea = false;
                this.chtPerdaSetores.Titles[0].Position.X = 13;
                this.chtPerdaSetores.Titles[0].Position.Y = 30;
                this.chtPerdaSetores.Titles[0].Position.Width = 63;
                this.chtPerdaSetores.Titles[0].Position.Height = 98;
                this.chtPerdaSetores.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtPerdaSetores.Titles[0].Text = "PERDA POR SETOR - " + this.drpMes.SelectedItem.Text.ToUpper() + "/" + drpAno.SelectedValue;
    
                #endregion
    
                #region Legenda
    
                this.chtPerdaSetores.Legends.Add("PerdaReal");
                this.chtPerdaSetores.Legends.Add("Desafio");
                this.chtPerdaSetores.Legends.Add("Meta");
                //this.chtPerdaSetores.Legends[0].Font = new Font("Arial", 10, FontStyle.Bold);
                //this.chtPerdaSetores.Legends[1].Font = new Font("Arial", 10, FontStyle.Bold);
                //this.chtPerdaSetores.Legends[2].Font = new Font("Arial", 10, FontStyle.Bold);
    
                #endregion
    
                #region Series
    
                // Cria e configura as séries do gráfico
                seriePerda = new Series("Real");
                serieDesafio = new Series("Desafio");
                serieMeta = new Series("Meta");
    
                seriePerda.ChartType = SeriesChartType.Bar;
                serieDesafio.ChartType = SeriesChartType.Bar;
                serieMeta.ChartType = SeriesChartType.Bar;
    
                seriePerda.XValueType = ChartValueType.Int32;
                serieDesafio.XValueType = ChartValueType.Int32;
                serieMeta.XValueType = ChartValueType.Int32;
    
                seriePerda.YValueType = ChartValueType.String;
                serieDesafio.YValueType = ChartValueType.String;
                serieMeta.YValueType = ChartValueType.String;
    
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.Font = new Font("Arial", 7, FontStyle.Bold);
                serieDesafio.IsValueShownAsLabel = true;
                serieDesafio.Font = new Font("Arial", 7, FontStyle.Bold);
                serieMeta.IsValueShownAsLabel = true;
                serieMeta.Font = new Font("Arial", 7, FontStyle.Bold);
    
                // Insere as séries no gráfico
                this.chtPerdaSetores.Series.Add(seriePerda);
                this.chtPerdaSetores.Series.Add(serieDesafio);
                this.chtPerdaSetores.Series.Add(serieMeta);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < lstResultado.Length; i++)
                {
                    lstSetores.Add(lstResultado[i].DescricaoSetor);
                    lstValuesPerda.Add(lstResultado[i].TotPerdaM2);
                    lstValuesDesafio.Add(lstResultado[i].DesafioPerda);
                    lstValuesMeta.Add(lstResultado[i].MetaPerda);
                }
    
                // Desenha os dados no gráfico
                this.chtPerdaSetores.Series[0].Points.DataBindXY(lstSetores, lstValuesPerda);
                this.chtPerdaSetores.Series[1].Points.DataBindXY(lstSetores, lstValuesDesafio);
                this.chtPerdaSetores.Series[2].Points.DataBindXY(lstSetores, lstValuesMeta);
    
                #endregion
            }
        }
    
        private void MontarGraficoPerdaMensal()
        {
            Dictionary<string, double> dadosRelatorio = this.GetDadosPerdaMensal();
    
            if (dadosRelatorio != null && dadosRelatorio.Count > 0)
            {
                this.chtPerdaMensal.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series seriePerda = null;
    
                // Arrays de Dados
                ArrayList lstMeses = new ArrayList();
                ArrayList lstValores = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtPerdaMensal.Width = 596;
                this.chtPerdaMensal.Height = 250;
                this.chtPerdaMensal.BorderlineColor = System.Drawing.Color.Black;
                this.chtPerdaMensal.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtPerdaMensal.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtPerdaMensal.ChartAreas.Add("Area");
                this.chtPerdaMensal.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.Position;
                this.chtPerdaMensal.ChartAreas[0].Position.Height = 90;
                this.chtPerdaMensal.ChartAreas[0].Position.Width = 85;
                this.chtPerdaMensal.ChartAreas[0].Position.Y = 8;
                this.chtPerdaMensal.ChartAreas[0].Position.X = 3;
    
                // Configurações do Eixo X
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtPerdaMensal.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtPerdaMensal.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtPerdaMensal.ChartAreas[0].AxisX.Interval = 1;
                this.chtPerdaMensal.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                //this.chtIndicePerda.ChartAreas[0].AxisY.Interval = 25;
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtPerdaMensal.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtPerdaMensal.ChartAreas[0].AxisY.IsMarginVisible = true;
    
                #endregion
    
                #region Titulo
    
                this.chtPerdaMensal.Titles.Add("PerdaMensal");
                this.chtPerdaMensal.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtPerdaMensal.Titles[0].Docking = Docking.Top;
                this.chtPerdaMensal.Titles[0].IsDockedInsideChartArea = false;
                this.chtPerdaMensal.Titles[0].Position.X = 13;
                this.chtPerdaMensal.Titles[0].Position.Y = 30;
                this.chtPerdaMensal.Titles[0].Position.Width = 63;
                this.chtPerdaMensal.Titles[0].Position.Height = 98;
                this.chtPerdaMensal.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtPerdaMensal.Titles[0].Text = "PERDA MENSAL - ANO DE " + drpAno.SelectedValue;
    
                #endregion
    
                #region Legenda
    
                this.chtPerdaMensal.Legends.Add("PerdaMensal");
                //this.chtPerdaSetores.Legends[0].Font = new Font("Arial", 10, FontStyle.Bold);
    
                #endregion
    
                #region Series
    
                // Cria e configura as séries do gráfico
                seriePerda = new Series("PerdaMensal");
                seriePerda.ChartType = SeriesChartType.Pie;
                seriePerda.XValueType = ChartValueType.Int32;
                seriePerda.YValueType = ChartValueType.String;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.Font = new Font("Arial", 7, FontStyle.Bold);
    
                // Insere as séries no gráfico
                this.chtPerdaMensal.Series.Add(seriePerda);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                foreach (KeyValuePair<string, double> chave in dadosRelatorio)
                {
                    lstMeses.Add(chave.Key);
                    lstValores.Add(Math.Round(chave.Value, 2));
                }
    
                // Desenha os dados no gráfico
                this.chtPerdaMensal.Series[0].Points.DataBindXY(lstMeses, lstValores);
    
                #endregion
            }
        }
    
        private void MontarGrafico10mm()
        {
            if (this._dados10mm != null && this._dados10mm.Length > 0)
            {
                this.chtProd10mm.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series serieProducao = null;
                Series seriePerda = null;
                Series serieIndice = null;
    
                // Arrays de Dados
                ArrayList lstProducao = new ArrayList();
                ArrayList lstPerda = new ArrayList();
                ArrayList lstIndice = new ArrayList();
                ArrayList lstCor = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtProd10mm.Width = 600;
                this.chtProd10mm.Height = 900;
                this.chtProd10mm.BorderlineColor = System.Drawing.Color.Black;
                this.chtProd10mm.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtProd10mm.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtProd10mm.ChartAreas.Add("Area");
                this.chtProd10mm.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.AxesView;
                this.chtProd10mm.ChartAreas[0].Position.Height = 80;
                this.chtProd10mm.ChartAreas[0].Position.Width = 90;
                this.chtProd10mm.ChartAreas[0].Position.Y = 10;
                this.chtProd10mm.ChartAreas[0].Position.X = 3;
    
                // Configurações do Eixo X
                this.chtProd10mm.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProd10mm.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProd10mm.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtProd10mm.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProd10mm.ChartAreas[0].AxisX.Interval = 1;
                this.chtProd10mm.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                this.chtProd10mm.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProd10mm.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProd10mm.ChartAreas[0].AxisY.LabelStyle.Format = "{N3}";
                this.chtProd10mm.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
    
                #endregion
    
                #region Titulo
    
                this.chtProd10mm.Titles.Add("Producao10mm");
                this.chtProd10mm.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtProd10mm.Titles[0].Docking = Docking.Top;
                this.chtProd10mm.Titles[0].IsDockedInsideChartArea = false;
                this.chtProd10mm.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtProd10mm.Titles[0].Text = "10MM";
    
                #endregion
    
                #region Legenda
    
                this.chtProd10mm.Legends.Add("Producao10mm");
                this.chtProd10mm.Legends[0].Docking = Docking.Bottom;
                this.chtProd10mm.Legends[0].LegendStyle = LegendStyle.Row;
                this.chtProd10mm.Legends[0].Position.X = 20;
                this.chtProd10mm.Legends[0].Position.Y = 95;
                this.chtProd10mm.Legends[0].Position.Width = 95;
                this.chtProd10mm.Legends[0].Position.Height = 10;
                
                #endregion
    
                #region Series
    
                // Cria e configura a série do gráfico
                serieProducao = new Series("PRODUZIDO/PRODUZINDO");
                seriePerda = new Series("PERDA");
                serieIndice = new Series("ÍNDICE (PERDA x PRODUÇÃO)");
    
                // Configura a Série "Produção"
                serieProducao.ChartType = SeriesChartType.Bar;
                serieProducao.XValueType = ChartValueType.String;
                serieProducao.YValueType = ChartValueType.Int32;
                serieProducao.IsValueShownAsLabel = true;
                serieProducao.Font = new Font("Arial", 6, FontStyle.Bold);
                serieProducao.IsVisibleInLegend = true;
    
                // Configura a Série "Perda"
                seriePerda.ChartType = SeriesChartType.Bar;
                seriePerda.XValueType = ChartValueType.String;
                seriePerda.YValueType = ChartValueType.Int32;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.Font = new Font("Arial", 6, FontStyle.Bold);
                seriePerda.IsVisibleInLegend = true;
    
                // Configura a Série "Produção"
                serieIndice.ChartType = SeriesChartType.Bar;
                serieIndice.XValueType = ChartValueType.String;
                serieIndice.YValueType = ChartValueType.Int32;
                serieIndice.IsValueShownAsLabel = false;
                serieIndice.Label = "#VALY" + "%";
                serieIndice.Font = new Font("Arial", 6, FontStyle.Bold);
                serieIndice.IsVisibleInLegend = true;
    
                // Insere as séries no gráfico
                this.chtProd10mm.Series.Add(serieProducao);
                this.chtProd10mm.Series.Add(seriePerda);
                this.chtProd10mm.Series.Add(serieIndice);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < _dados10mm.Length; i++)
                {
                    lstCor.Add(_dados10mm[i].CorVidro);
                    lstProducao.Add(_dados10mm[i].TotProdM2);
                    lstPerda.Add(_dados10mm[i].TotPerdaM2);
                    lstIndice.Add(_dados10mm[i].IndicePerdaProducao);
                }
    
                // Desenha os dados no gráfico
                this.chtProd10mm.Series[0].Points.DataBindXY(lstCor, lstProducao);
                this.chtProd10mm.Series[1].Points.DataBindXY(lstCor, lstPerda);
                this.chtProd10mm.Series[2].Points.DataBindXY(lstCor, lstIndice);
    
                #endregion
            }
        }
    
        private void MontarGrafico8mm()
        {
            if (this._dados8mm != null && this._dados8mm.Length > 0)
            {
                this.chtProd8mm.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series serieProducao = null;
                Series seriePerda = null;
                Series serieIndice = null;
    
                // Arrays de Dados
                ArrayList lstProducao = new ArrayList();
                ArrayList lstPerda = new ArrayList();
                ArrayList lstIndice = new ArrayList();
                ArrayList lstCor = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtProd8mm.Width = 600;
                this.chtProd8mm.Height = 900;
                this.chtProd8mm.BorderlineColor = System.Drawing.Color.Black;
                this.chtProd8mm.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtProd8mm.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtProd8mm.ChartAreas.Add("Area");
                this.chtProd8mm.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.AxesView;
                this.chtProd8mm.ChartAreas[0].Position.Height = 80;
                this.chtProd8mm.ChartAreas[0].Position.Width = 90;
                this.chtProd8mm.ChartAreas[0].Position.Y = 10;
                this.chtProd8mm.ChartAreas[0].Position.X = 3;
    
                // Configurações do Eixo X
                this.chtProd8mm.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProd8mm.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProd8mm.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtProd8mm.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProd8mm.ChartAreas[0].AxisX.Interval = 1;
                this.chtProd8mm.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                //this.chtProd8mm.ChartAreas[0].AxisY.IsMarginVisible = true;
                //this.chtProd8mm.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProd8mm.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProd8mm.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProd8mm.ChartAreas[0].AxisY.LabelStyle.Format = "{N3}";
                this.chtProd8mm.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
    
                #endregion
    
                #region Titulo
    
                this.chtProd8mm.Titles.Add("Producao8mm");
                this.chtProd8mm.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtProd8mm.Titles[0].Docking = Docking.Top;
                this.chtProd8mm.Titles[0].IsDockedInsideChartArea = false;
                this.chtProd8mm.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtProd8mm.Titles[0].Text = "8MM";
    
                #endregion
    
                #region Legenda
    
                this.chtProd8mm.Legends.Add("Producao8mm");
                this.chtProd8mm.Legends[0].Docking = Docking.Bottom;
                this.chtProd8mm.Legends[0].LegendStyle = LegendStyle.Row;
                this.chtProd8mm.Legends[0].Position.X = 20;
                this.chtProd8mm.Legends[0].Position.Y = 95;
                this.chtProd8mm.Legends[0].Position.Width = 95;
                this.chtProd8mm.Legends[0].Position.Height = 10;
    
                #endregion
    
                #region Series
    
                // Cria e configura a série do gráfico
                serieProducao = new Series("PRODUZIDO/PRODUZINDO");
                seriePerda = new Series("PERDA");
                serieIndice = new Series("ÍNDICE (PERDA x PRODUÇÃO)");
    
                // Configura a Série "Produção"
                serieProducao.ChartType = SeriesChartType.Bar;
                serieProducao.XValueType = ChartValueType.String;
                serieProducao.YValueType = ChartValueType.Int32;
                serieProducao.IsValueShownAsLabel = true;
                serieProducao.Font = new Font("Arial", 6, FontStyle.Bold);
                serieProducao.IsVisibleInLegend = true;
    
                // Configura a Série "Perda"
                seriePerda.ChartType = SeriesChartType.Bar;
                seriePerda.XValueType = ChartValueType.String;
                seriePerda.YValueType = ChartValueType.Int32;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.Font = new Font("Arial", 6, FontStyle.Bold);
                seriePerda.IsVisibleInLegend = true;
    
                // Configura a Série "Produção"
                serieIndice.ChartType = SeriesChartType.Bar;
                serieIndice.XValueType = ChartValueType.String;
                serieIndice.YValueType = ChartValueType.Int32;
                serieIndice.IsValueShownAsLabel = false;
                serieIndice.Label = "#VALY" + "%";
                serieIndice.Font = new Font("Arial", 6, FontStyle.Bold);
                serieIndice.IsVisibleInLegend = true;
    
                // Insere as séries no gráfico
                this.chtProd8mm.Series.Add(serieProducao);
                this.chtProd8mm.Series.Add(seriePerda);
                this.chtProd8mm.Series.Add(serieIndice);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < _dados8mm.Length; i++)
                {
                    lstCor.Add(_dados8mm[i].CorVidro);
                    lstProducao.Add(_dados8mm[i].TotProdM2);
                    lstPerda.Add(_dados8mm[i].TotPerdaM2);
                    lstIndice.Add(_dados8mm[i].IndicePerdaProducao);
                }
    
                // Desenha os dados no gráfico
                this.chtProd8mm.Series[0].Points.DataBindXY(lstCor, lstProducao);
                this.chtProd8mm.Series[1].Points.DataBindXY(lstCor, lstPerda);
                this.chtProd8mm.Series[2].Points.DataBindXY(lstCor, lstIndice);
    
                #endregion
            }
        }
    
        private void MontarGrafico6mm()
        {
            if (this._dados6mm != null && this._dados6mm.Length > 0)
            {
                this.chtProd6mm.Visible = true;
    
                #region Declaracoes
    
                // Series do Grafico
                Series serieProducao = null;
                Series seriePerda = null;
                Series serieIndice = null;
    
                // Arrays de Dados
                ArrayList lstProducao = new ArrayList();
                ArrayList lstPerda = new ArrayList();
                ArrayList lstIndice = new ArrayList();
                ArrayList lstCor = new ArrayList();
    
                #endregion
    
                #region Area e Eixos
    
                // Cria a área do Gráfico
                this.chtProd6mm.Width = 600;
                this.chtProd6mm.Height = 900;
                this.chtProd6mm.BorderlineColor = System.Drawing.Color.Black;
                this.chtProd6mm.BorderlineDashStyle = ChartDashStyle.Solid;
                this.chtProd6mm.BorderlineWidth = 1;
    
                // Configurações do ChartArea
                this.chtProd6mm.ChartAreas.Add("Area");
                this.chtProd6mm.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.AxesView;
                this.chtProd6mm.ChartAreas[0].Position.Height = 80;
                this.chtProd6mm.ChartAreas[0].Position.Width = 90;
                this.chtProd6mm.ChartAreas[0].Position.Y = 10;
                this.chtProd6mm.ChartAreas[0].Position.X = 3;
    
                // Configurações do Eixo X
                this.chtProd6mm.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProd6mm.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtProd6mm.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 7, FontStyle.Bold);
                this.chtProd6mm.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProd6mm.ChartAreas[0].AxisX.Interval = 1;
                this.chtProd6mm.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y
                this.chtProd6mm.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProd6mm.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProd6mm.ChartAreas[0].AxisY.LabelStyle.Format = "{N3}";
                this.chtProd6mm.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
    
                #endregion
    
                #region Titulo
    
                this.chtProd6mm.Titles.Add("Producao6mm");
                this.chtProd6mm.Titles[0].Alignment = ContentAlignment.TopCenter;
                this.chtProd6mm.Titles[0].Docking = Docking.Top;
                this.chtProd6mm.Titles[0].IsDockedInsideChartArea = false;
                this.chtProd6mm.Titles[0].Font = new Font("Arial", 11, FontStyle.Bold);
                this.chtProd6mm.Titles[0].Text = "6MM";
    
                #endregion
    
                #region Legenda
    
                this.chtProd6mm.Legends.Add("Producao6mm");
                this.chtProd6mm.Legends[0].Docking = Docking.Bottom;
                this.chtProd6mm.Legends[0].LegendStyle = LegendStyle.Row;
                this.chtProd6mm.Legends[0].Position.X = 20;
                this.chtProd6mm.Legends[0].Position.Y = 95;
                this.chtProd6mm.Legends[0].Position.Width = 95;
                this.chtProd6mm.Legends[0].Position.Height = 10;
    
                #endregion
    
                #region Series
    
                // Cria e configura a série do gráfico
                serieProducao = new Series("PRODUZIDO/PRODUZINDO");
                seriePerda = new Series("PERDA");
                serieIndice = new Series("ÍNDICE (PERDA x PRODUÇÃO)");
    
                // Configura a Série "Produção"
                serieProducao.ChartType = SeriesChartType.Bar;
                serieProducao.XValueType = ChartValueType.String;
                serieProducao.YValueType = ChartValueType.Int32;
                serieProducao.IsValueShownAsLabel = true;
                serieProducao.Font = new Font("Arial", 6, FontStyle.Bold);
                serieProducao.IsVisibleInLegend = true;
    
                // Configura a Série "Perda"
                seriePerda.ChartType = SeriesChartType.Bar;
                seriePerda.XValueType = ChartValueType.String;
                seriePerda.YValueType = ChartValueType.Int32;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.Font = new Font("Arial", 6, FontStyle.Bold);
                seriePerda.IsVisibleInLegend = true;
    
                // Configura a Série "Produção"
                serieIndice.ChartType = SeriesChartType.Bar;
                serieIndice.XValueType = ChartValueType.String;
                serieIndice.YValueType = ChartValueType.Int32;
                serieIndice.IsValueShownAsLabel = false;
                serieIndice.Label = "#VALY" + "%";
                serieIndice.Font = new Font("Arial", 6, FontStyle.Bold);
                serieIndice.IsVisibleInLegend = true;
    
                // Insere as séries no gráfico
                this.chtProd6mm.Series.Add(serieProducao);
                this.chtProd6mm.Series.Add(seriePerda);
                this.chtProd6mm.Series.Add(serieIndice);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados
                for (int i = 0; i < _dados6mm.Length; i++)
                {
                    lstCor.Add(_dados6mm[i].CorVidro);
                    lstProducao.Add(_dados6mm[i].TotProdM2);
                    lstPerda.Add(_dados6mm[i].TotPerdaM2);
                    lstIndice.Add(_dados6mm[i].IndicePerdaProducao);
                }
    
                // Desenha os dados no gráfico
                this.chtProd6mm.Series[0].Points.DataBindXY(lstCor, lstProducao);
                this.chtProd6mm.Series[1].Points.DataBindXY(lstCor, lstPerda);
                this.chtProd6mm.Series[2].Points.DataBindXY(lstCor, lstIndice);
    
                #endregion
            }
        }
    
        private void PreencherHiddens()
        {
            byte[] bufferGraf;
    
            if (this.drpTipoGrafico.SelectedValue == "1")
            {
                bufferGraf = Util.Helper.ChartToByteArray(this.chtIndicePerda);
                this.hdfGrafIndicePerda.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtPerdaMensal);
                this.hdfGrafPerdaMensal.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtPerdaSetores);
                this.hdfGrafPerdaSetores.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtProdAcumulada);
                this.hdfGrafProducaoAcumulada.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtProducaoDiaria);
                this.hdfGrafProducaoDiaria.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
            }
            else if (this.drpTipoGrafico.SelectedValue == "2")
            {
                bufferGraf = Util.Helper.ChartToByteArray(this.chtProd10mm);
                this.hdfGraf10mm.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtProd8mm);
                this.hdfGraf8mm.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
    
                bufferGraf = Util.Helper.ChartToByteArray(this.chtProd6mm);
                this.hdfGraf6mm.Value = Glass.Conversoes.CodificaPara64(bufferGraf);
            }
        }
    
        private void MontarGraficos()
        {
            if (this.drpTipoGrafico.SelectedValue == "1")
            {
                /* Chamado 63401. */
                // Obtem os dados pra montar o gráfico
                var dados = this.GetDados();

                this.tbGraficosPerdaAcumulada.Visible = true;
                this.MontarGraficoProducaoAcumulada(dados);
                this.MontarGraficoIncidePerda(dados);
                this.MontarGraficoProducaoDiaria(dados);
                this.MontarGraficoPerdaPorSetor();
                this.MontarGraficoPerdaMensal();
            }
            else if (this.drpTipoGrafico.SelectedValue == "2")
            {
                this.GetDadosEspessuraProduto();
                this.tbGraficosProduto.Visible = true;
                this.MontarGrafico10mm();
                this.MontarGrafico8mm();
                this.MontarGrafico6mm();
            }
        }
    }
}
