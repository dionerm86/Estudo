using System;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Glass.Data.RelModel;
using System.Collections;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Data.RelDAL;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Monta os gráficos
            this.MontarGraficoProducaoDia();
            this.MontarGraficoPerdaMensal();
            this.MontarGraficoProducaoPendente();

            var producoesDiariasRealizadas = ProducaoDiariaRealizadaDAO.Instance.ObtemProducaoRealizadaTodosSetores();

            foreach(var p in producoesDiariasRealizadas)
            {
                var mensagem = new System.Text.StringBuilder().AppendFormat("{0}: ", p.DescricaoSetor);

                var pedidoProducaoDAO = ProdutoPedidoProducaoDAO.Instance;

                // Verifica quais dados serão exibidos no rodapé do painel da produção, de acordo com a empresa.
                if (PCPConfig.PainelProducao.ExibirTotalM2LidoNoDia)
                    mensagem.AppendFormat("{0:N}m²", p.TotRealizado);

                else if (PCPConfig.PainelProducao.ExibirTotalQtdeLidoNoDia)
                    mensagem.AppendFormat("{0} pç (s)", p.TotRealizado);

                else if (OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC != null)
                    mensagem.AppendFormat("{0} pç (s)", (int)p.TotRealizado);

                else
                    mensagem.AppendFormat("{0:N}m²",p.TotRealizado);

                (this.Master as PainelGraficos).MensagensRodape.Add(mensagem.ToString());
            }
        }
    
        /// <summary>
        /// Obtém a produção do dia.
        /// </summary>
        /// <returns></returns>
        private GraficoProdPerdaDiaria GetDadosDia()
        {
            return Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetProdPerdaDia();
        }
    
        /// <summary>
        /// Obtém a produção pendente dos últimos 7 dias.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, double> GetProducaoPendente()
        {
            var pp = Glass.Data.RelDAL.PecasPendentesDAO.Instance.ProducaoPendente(DateTime.Today.AddDays(-9).ToString("yyyy-MM-dd"), DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
    
            if (pp.Count == 0)
                pp.Add(0, 0.0);
    
            return pp;
        }
    
        /// <summary>
        /// Obtém a quantidade de peças perdidas dos últimos 4 meses.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, double> GetDadosPerdaMensal()
        {
            var tipo = Request["tipo"];
            return Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetPerda(DateTime.Today.Month.ToString(), DateTime.Today.Year.ToString(), tipo == "Geral", tipo == "Departamento");
        }
    
        /// <summary>
        /// Cria o gráfico da produção do dia.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoProducaoDia()
        {
            // Se nenhum dado for buscado o gráfico não é gerado.
            if (chtProducaoDia != null)
            {
                #region Declarações
    
                // Series do Gráfico.
                var serieMeta = new Series("Meta");
                var serieProducao = new Series("Perda Acumulada");
    
                // Arrays de Dados.
                var lstDias = new ArrayList();
                var dia = DateTime.Today.Day.ToString();
                var meta = new ArrayList();
                var lstValoresProducao = new ArrayList();

                lstValoresProducao.Add(0.0);

                lstValoresProducao[0] = this.GetDadosDia().TotProdM2;
    
                #endregion
    
                #region Áreas e Eixos
    
                // Configurações do ChartArea.
                this.chtProducaoDia.ChartAreas.Add("Area");
    
                // Configurações do Eixo Y.
                this.chtProducaoDia.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtProducaoDia.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo X.
                this.chtProducaoDia.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtProducaoDia.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtProducaoDia.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtProducaoDia.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
    
                #endregion
    
                #region Título
    
                // Configurações do título do gráfico.
                this.chtProducaoDia.Titles.Add("ProducaoDiaria");
                this.chtProducaoDia.Titles[0].Font = new Font(this.chtProducaoDia.Titles[0].Font, FontStyle.Bold);
                this.chtProducaoDia.Titles[0].Text = "PRODUÇÃO DO DIA";
                
                #endregion
    
                #region Legenda
    
                // Legenda do gráfico.
                //this.chtProducaoDia.Legends.Add("ProducaoDia");
    
                #endregion
    
                #region Séries
    
                // Define o tipo de gráfico.
                serieMeta.ChartType = SeriesChartType.Column;
                serieProducao.ChartType = SeriesChartType.Column;
    
                // Define o tipo de valor do eixo X.
                serieMeta.XValueType = ChartValueType.String;
                serieProducao.XValueType = ChartValueType.String;
    
                // Define se o valor será mostrado em uma label.
                serieMeta.IsValueShownAsLabel = true;
                serieProducao.IsValueShownAsLabel = true;
    
                // Define a cor das séries.
                ColorConverter cc = new ColorConverter();
                serieMeta.Color = (Color)cc.ConvertFromString("#056492");
                serieProducao.Color = (double)lstValoresProducao[0] >= PCPConfig.MetaProducaoDiaria ? Color.PaleGreen : (Color)cc.ConvertFromString("#FCB441");
    
                // Insere as séries no gráfico.
                this.chtProducaoDia.Series.Add(serieMeta);
                this.chtProducaoDia.Series.Add(serieProducao);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados.
                lstDias.Add(dia);
                
                // Meta de produção diária.
                if (!PCPConfig.ConsiderarMetaProducaoM2PecasPorDataFabrica)
                    meta.Add(Math.Round(PCPConfig.MetaProducaoDiaria, 2));
                else
                    meta.Add(Math.Round(ProdutoPedidoProducaoDAO.Instance.ObtemM2MetaProdDia(), 2));
    
                // Desenha os dados no gráfico.
                this.chtProducaoDia.Series[0].Points.DataBindXY(lstDias, meta);
                this.chtProducaoDia.Series[1].Points.DataBindXY(lstDias, lstValoresProducao);
    
                #endregion
            }
        }
    
        /// <summary>
        /// Cria o gráfico de perda mensal.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoPerdaMensal()
        {
            // Obtém os dados de perda mensal.
            var dadosRelatorio = this.GetDadosPerdaMensal();
            
            this.tbPercentualPerda.Visible = true;
                
            var mesCorrente = DateTime.Today.ToString("MMMM").ToUpper();
            var totM2Perda = dadosRelatorio.ContainsKey(mesCorrente) ? Math.Round((decimal)dadosRelatorio[mesCorrente], 2) : 0;

            var totM2Produzido = Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetTotM2ProducaoMensal(DateTime.Today.Month, DateTime.Today.Year);

            var porcentagemPerda = totM2Perda > 0 && totM2Produzido > 0 ? (totM2Perda / totM2Produzido) : 0;

            lblRetrabalho.Text = totM2Perda.ToString();
            lblProduzido.Text = totM2Produzido.ToString();
            lblPorcentagemPerda.Text = porcentagemPerda.ToString("P");
            
            this.chtPerdaMensal.Visible = true;

            // Se nenhum dado for buscado o gráfico não é gerado.
            if (dadosRelatorio != null && dadosRelatorio.Count > 0 && chtPerdaMensal != null)
            {
                #region Declarações

                // Séries do Gráfico.
                var seriePerda = new Series("PerdaMensal");

                // Arrays de Dados.
                var lstMeses = new ArrayList();
                var lstValores = new ArrayList();

                #endregion

                #region Áreas e Eixos

                // Configurações do ChartArea.
                this.chtPerdaMensal.ChartAreas.Add("Area");

                // Configurações do Eixo X.
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtPerdaMensal.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtPerdaMensal.ChartAreas[0].AxisX.Interval = 1;
                this.chtPerdaMensal.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;

                // Configurações do Eixo Y.
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtPerdaMensal.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtPerdaMensal.ChartAreas[0].AxisY.IsMarginVisible = false;

                #endregion

                #region Título

                // Configurações do título do gráfico.
                this.chtPerdaMensal.Titles.Add("PerdaMensal");
                this.chtPerdaMensal.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                this.chtPerdaMensal.Titles[0].Docking = Docking.Top;
                this.chtPerdaMensal.Titles[0].IsDockedInsideChartArea = false;
                this.chtPerdaMensal.Titles[0].Font = new Font(this.chtPerdaMensal.Titles[0].Font, FontStyle.Bold);
                this.chtPerdaMensal.Titles[0].Text = "PERDA MENSAL";

                #endregion

                #region Legenda

                // Legenda do gráfico.
                this.chtPerdaMensal.Legends.Add("PerdaMensal");
                this.chtPerdaMensal.Legends[0].Docking = Docking.Left;

                #endregion

                #region Séries

                // Cria e configura as séries do gráfico.
                seriePerda.ChartType = SeriesChartType.Pie;
                seriePerda.XValueType = ChartValueType.Int32;
                seriePerda.YValueType = ChartValueType.String;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.LabelFormat = "{0:N2}";

                // Insere as séries no gráfico.
                this.chtPerdaMensal.Series.Add(seriePerda);

                #endregion

                #region Dados

                // Popula os arrays de dados.
                foreach (var chave in dadosRelatorio)
                {
                    lstMeses.Add(chave.Key);
                    lstValores.Add(chave.Value > 0 ? chave.Value : 0.001);
                }

                // Desenha os dados no gráfico.
                this.chtPerdaMensal.Series[0].Points.DataBindXY(lstMeses, lstValores);

                #endregion
            }
        }
    
        /// <summary>
        /// Cria o gráfico de toda a produção pendente.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoProducaoPendente()
        {
            // Obtém os dados da produção pendente.
            var lstResultado = this.GetProducaoPendente();
    
            // Se nenhum dado for buscado o gráfico não é gerado.
            if (lstResultado != null && lstResultado.Count > 0 && chtProducaoPendente != null)
            {
                #region Declarações
    
                // Séries do Gráfico.
                var seriePendente = new Series("Pendente Setor");
    
                // Arrays de Dados.
                var lstDias = new List<string>();
                var lstValuesProducao = new ArrayList();
    
                #endregion
    
                #region Áreas e Eixos
    
                // Configurações do ChartArea.
                this.chtProducaoPendente.ChartAreas.Add("Area");
    
                // Configurações do Eixo X.
                this.chtProducaoPendente.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProducaoPendente.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProducaoPendente.ChartAreas[0].AxisX.IntervalOffset = 1;
                this.chtProducaoPendente.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configurações do Eixo Y.
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProducaoPendente.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtProducaoPendente.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalOffset = 99999;
    
                #endregion
    
                #region Título
    
                // Configurações do título do gráfico.
                this.chtProducaoPendente.Titles.Add("ProducaoPendente");
                this.chtProducaoPendente.Titles[0].IsDockedInsideChartArea = false;
                this.chtProducaoPendente.Titles[0].Font = new Font(this.chtProducaoPendente.Titles[0].Font, FontStyle.Bold);
                this.chtProducaoPendente.Titles[0].Text = "PRODUÇÂO PENDENTE";
                
                #endregion
    
                #region Legenda
    
                // Legenda do gráfico.
                //this.chtProducaoPendente.Legends.Add("ProducaoPendente");
    
                #endregion
    
                #region Séries
    
                // Cria e configura a série do gráfico.
                seriePendente.ChartType = SeriesChartType.Column;
                seriePendente.Color = Color.OrangeRed;
                seriePendente.XValueType = ChartValueType.String;
                seriePendente.YValueType = ChartValueType.Int32;
                seriePendente.IsValueShownAsLabel = true;
    
                // Insere as séries no gráfico.
                this.chtProducaoPendente.Series.Add(seriePendente);
    
                #endregion
    
                #region Dados
    
                // Popula os arrays de dados.
                foreach (var i in lstResultado.Keys)
                {
                    lstDias.Add(i.ToString());
                    lstValuesProducao.Add(lstResultado[i]);
                }
    
                // Desenha os dados no gráfico.
                this.chtProducaoPendente.Series[0].Points.DataBindXY(lstDias, lstValuesProducao);
    
                #endregion
            }
        }
    }
}