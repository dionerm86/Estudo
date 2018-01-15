using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Collections;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelProducaoSetores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Monta os gr�ficos
            this.MontarGraficoMeta();
            this.MontarGraficoProducaoDiaSetores();
            this.MontarGraficoPerdaMensal();
            this.MontarGraficoProducaoPendente();

            var setores = new List<Setor>();

            // Informa as mensagens que ser�o passadas no rodap� da p�gina.
            if (!ProducaoConfig.ExibirTotalEtiquetaNaoImpressaPainel)
                setores = Data.Helper.Utils.GetSetores.ToList();
            else
            {
                var setorEtiquetaNaoImpressa = new Setor();
                setorEtiquetaNaoImpressa.IdSetor = 0;
                setorEtiquetaNaoImpressa.Descricao = "Etiqueta N�o Impressa";
                setorEtiquetaNaoImpressa.ExibirRelatorio = true;
                setorEtiquetaNaoImpressa.Tipo = TipoSetor.Pendente;

                setores.Add(setorEtiquetaNaoImpressa);
                setores.AddRange(SetorDAO.Instance.GetOrdered());
            }

            foreach (var s in setores)
            {
                if (s.ExibirRelatorio && s.Tipo != TipoSetor.Entregue && s.Tipo != TipoSetor.ExpCarregamento)
                {
                    var mensagem = new System.Text.StringBuilder()
                        .AppendFormat("{0}: ", s.Descricao);

                    var pedidoProducaoDAO = ProdutoPedidoProducaoDAO.Instance;

                    // Verifica quais dados ser�o exibidos no rodap� do painel da produ��o, de acordo com a empresa.
                    if (PCPConfig.PainelProducao.ExibirTotalM2LidoNoDia)
                        mensagem.AppendFormat("{0:N}m�", pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, DateTime.Today, DateTime.Today.AddDays(1)));

                    else if (PCPConfig.PainelProducao.ExibirTotalQtdeLidoNoDia)
                        mensagem.AppendFormat("{0} p� (s)", pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, null, DateTime.Today, DateTime.Today.AddDays(1), false));

                    else if (OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC != null)
                        mensagem.AppendFormat("{0} p� (s)", (int)pedidoProducaoDAO
                            .ObtemTotM2Setor(
                                s.IdSetor, null, OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC,
                                DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 23:59:59"), false));

                    else
                        mensagem.AppendFormat("{0:N}m�", ProdutoPedidoProducaoDAO.Instance.ObtemTotM2Setor(s.IdSetor));

                    (this.Master as PainelGraficos).MensagensRodape.Add(mensagem.ToString());
                }
            }

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel = "";
        }

        /// <summary>
        /// Obt�m a produ��o pendente dos �ltimos 7 dias.
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
        /// Obt�m a quantidade de pe�as perdidas dos �ltimos 4 meses.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, double> GetDadosPerdaMensal()
        {
            return Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO.Instance.GetPerda(DateTime.Today.Month.ToString(), DateTime.Today.Year.ToString(), true, false);
        }

        /// <summary>
        /// Cria o gr�fico da meta di�ria.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoMeta()
        {
            if (chtMeta != null)
            {
                #region Declara��es

                // Series do Gr�fico.
                var serieProducao = new Series("Producao");
                var serieMeta = new Series("Meta");

                // Arrays de Dados.
                var meta = new ArrayList();
                var producao = new ArrayList();
                var lstDias = new ArrayList();

                #endregion

                #region �reas e Eixos

                // Configura��es do ChartArea.
                this.chtMeta.ChartAreas.Add("Area");

                // Configura��es do Eixo Y.
                this.chtMeta.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtMeta.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;

                // Configura��es do Eixo X.
                this.chtMeta.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtMeta.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtMeta.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtMeta.ChartAreas[0].AxisY.LabelStyle.Enabled = false;

                #endregion

                #region T�tulo

                // Configura��es do t�tulo do gr�fico.
                this.chtMeta.Titles.Add("MetaDiaria");
                this.chtMeta.Titles[0].Font = new Font(this.chtMeta.Titles[0].Font, FontStyle.Bold);
                this.chtMeta.Titles[0].Text = "META DO DIA";

                #endregion

                #region S�ries

                // Define o tipo de gr�fico.
                serieMeta.ChartType = SeriesChartType.Column;
                serieProducao.ChartType = SeriesChartType.Line;

                // Define o tipo de valor do eixo X.
                serieMeta.XValueType = ChartValueType.String;
                serieProducao.XValueType = ChartValueType.String;

                // Define se o valor ser� mostrado em uma label.
                serieMeta.IsValueShownAsLabel = true;
                serieProducao.IsValueShownAsLabel = false;

                // Define a cor das s�ries.
                ColorConverter cc = new ColorConverter();
                serieMeta.Color = (Color)cc.ConvertFromString("#056492");
                serieProducao.Color = Color.Transparent;

                // Insere as s�ries no gr�fico.
                this.chtMeta.Series.Add(serieMeta);
                this.chtMeta.Series.Add(serieProducao);

                #endregion

                #region Dados

                // Popula os arrays de dados.
                var dia = DateTime.Today.Day.ToString();
                lstDias.Add(dia);

                producao.Add(0.0);

                foreach (var idSetor in SetorDAO.Instance.ObtemIdsSetoresPainelProducao())
                {
                    var producaoSetor = Math.Round(ProdutoPedidoProducaoDAO.Instance.ObtemTotM2LidoSetor((int)idSetor,
                        DateTime.Today, DateTime.Today.AddDays(1)), 2);

                    producao[0] = (double)producao[0] < producaoSetor ? producaoSetor : (double)producao[0];
                }

                // Meta de produ��o di�ria.
                meta.Add(Math.Round(!PCPConfig.ConsiderarMetaProducaoM2PecasPorDataFabrica ?
                    PCPConfig.MetaProducaoDiaria : ProdutoPedidoProducaoDAO.Instance.ObtemM2MetaProdDia(), 2));

                // Desenha os dados no gr�fico.
                this.chtMeta.Series[0].Points.DataBindXY(lstDias, meta);
                this.chtMeta.Series[1].Points.DataBindXY(lstDias, producao);

                #endregion
            }
        }

        /// <summary>
        /// Cria o gr�fico da produ��o de v�rios setores.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoProducaoDiaSetores()
        {
            // Se nenhum dado for buscado o gr�fico n�o � gerado.
            if (chtProducaoDiaSetores != null)
            {
                #region Declara��es

                // Series do Gr�fico.
                var serieProducao = new Series("Perda Acumulada");
                var serieMeta = new Series("Meta");

                // Arrays de Dados.
                var lstSetores = new ArrayList();
                var lstValoresProducao = new ArrayList();
                var lstMeta = new ArrayList();

                #endregion

                #region �reas e Eixos

                // Configura��es do ChartArea.
                this.chtProducaoDiaSetores.ChartAreas.Add("Area");

                // Configura��es do Eixo X.
                this.chtProducaoDiaSetores.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtProducaoDiaSetores.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                //Quando o grafico tiver mais que dez setores, continua exibindo o nome de todos.
                this.chtProducaoDiaSetores.ChartAreas[0].AxisX.Interval = 1;

                // Configura��es do Eixo Y.
                this.chtProducaoDiaSetores.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtProducaoDiaSetores.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtProducaoDiaSetores.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtProducaoDiaSetores.ChartAreas[0].AxisY.LabelStyle.Enabled = false;

                #endregion

                #region T�tulo

                // Configura��es do t�tulo do gr�fico.
                this.chtProducaoDiaSetores.Titles.Add("ProducaoDiariaSetores");
                this.chtProducaoDiaSetores.Titles[0].Font = new Font(this.chtProducaoDiaSetores.Titles[0].Font, FontStyle.Bold);
                this.chtProducaoDiaSetores.Titles[0].Text = "PRODU��O DO DIA POR SETOR";

                #endregion

                #region S�ries

                // Define o tipo de gr�fico.
                serieProducao.ChartType = SeriesChartType.Column;
                serieMeta.ChartType = SeriesChartType.Line;

                // Define o tipo de valor do eixo X.
                serieProducao.XValueType = ChartValueType.String;
                serieMeta.XValueType = ChartValueType.String;

                // Define se o valor ser� mostrado em uma label.
                serieProducao.IsValueShownAsLabel = true;
                serieMeta.IsValueShownAsLabel = false;

                // Define a cor das s�ries.
                ColorConverter cc = new ColorConverter();
                serieProducao.Color = (Color)cc.ConvertFromString("#FCB441");
                serieMeta.Color = Color.Transparent;

                // Insere as s�ries no gr�fico.
                this.chtProducaoDiaSetores.Series.Add(serieProducao);
                this.chtProducaoDiaSetores.Series.Add(serieMeta);

                #endregion

                #region Dados

                foreach (var idSetor in SetorDAO.Instance.ObtemIdsSetoresPainelProducao())
                {
                    lstSetores.Add(SetorDAO.Instance.ObtemNomeSetor(idSetor));
                    lstValoresProducao.Add(Math.Round(ProdutoPedidoProducaoDAO.Instance.ObtemTotM2LidoSetor((int)idSetor,
                        DateTime.Today, DateTime.Today.AddDays(1)), 2));
                    lstMeta.Add(PCPConfig.MetaProducaoDiaria);
                }

                // Desenha os dados no gr�fico.
                this.chtProducaoDiaSetores.Series[0].Points.DataBindXY(lstSetores, lstValoresProducao);
                this.chtProducaoDiaSetores.Series[1].Points.DataBindXY(lstSetores, lstMeta);

                #endregion
            }
        }

        /// <summary>
        /// Cria o gr�fico de perda mensal.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoPerdaMensal()
        {
            // Obt�m os dados de perda mensal.
            var dadosRelatorio = this.GetDadosPerdaMensal();

            // Se nenhum dado for buscado o gr�fico n�o � gerado.
            if (dadosRelatorio != null && dadosRelatorio.Count > 0 && chtPerdaMensal != null)
            {
                #region Declara��es

                // S�ries do Gr�fico.
                var seriePerda = new Series("PerdaMensal");

                // Arrays de Dados.
                var lstMeses = new ArrayList();
                var lstValores = new ArrayList();

                #endregion

                #region �reas e Eixos

                // Configura��es do ChartArea.
                this.chtPerdaMensal.ChartAreas.Add("Area");

                // Configura��es do Eixo X.
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtPerdaMensal.ChartAreas[0].AxisX.IsInterlaced = false;
                this.chtPerdaMensal.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtPerdaMensal.ChartAreas[0].AxisX.Interval = 1;
                this.chtPerdaMensal.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;

                // Configura��es do Eixo Y.
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtPerdaMensal.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtPerdaMensal.ChartAreas[0].AxisY.LabelStyle.Format = "{N2}";
                this.chtPerdaMensal.ChartAreas[0].AxisY.IsMarginVisible = false;

                #endregion

                #region T�tulo

                // Configura��es do t�tulo do gr�fico.
                this.chtPerdaMensal.Titles.Add("PerdaMensal");
                this.chtPerdaMensal.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                this.chtPerdaMensal.Titles[0].Docking = Docking.Top;
                this.chtPerdaMensal.Titles[0].IsDockedInsideChartArea = false;
                this.chtPerdaMensal.Titles[0].Font = new Font(this.chtPerdaMensal.Titles[0].Font, FontStyle.Bold);
                this.chtPerdaMensal.Titles[0].Text = "PERDA MENSAL";

                #endregion

                #region Legenda

                // Legenda do gr�fico.
                this.chtPerdaMensal.Legends.Add("PerdaMensal");
                this.chtPerdaMensal.Legends[0].Docking = Docking.Left;

                #endregion

                #region S�ries

                // Cria e configura as s�ries do gr�fico.
                seriePerda.ChartType = SeriesChartType.Pie;
                seriePerda.XValueType = ChartValueType.Int32;
                seriePerda.YValueType = ChartValueType.String;
                seriePerda.IsValueShownAsLabel = true;
                seriePerda.LabelFormat = "{0:N2}";

                // Insere as s�ries no gr�fico.
                this.chtPerdaMensal.Series.Add(seriePerda);

                #endregion

                #region Dados

                // Popula os arrays de dados.
                foreach (var chave in dadosRelatorio)
                {
                    lstMeses.Add(chave.Key);
                    lstValores.Add(chave.Value > 0 ? chave.Value : 0.001);
                }

                // Desenha os dados no gr�fico.
                this.chtPerdaMensal.Series[0].Points.DataBindXY(lstMeses, lstValores);

                #endregion
            }
        }

        /// <summary>
        /// Cria o gr�fico de toda a produ��o pendente.
        /// </summary>
        /// <returns></returns>
        private void MontarGraficoProducaoPendente()
        {
            // Obt�m os dados da produ��o pendente.
            var lstResultado = this.GetProducaoPendente();

            // Se nenhum dado for buscado o gr�fico n�o � gerado.
            if (lstResultado != null && lstResultado.Count > 0 && chtProducaoPendente != null)
            {
                #region Declara��es

                // S�ries do Gr�fico.
                var seriePendente = new Series("Pendente Setor");

                // Arrays de Dados.
                var lstDias = new List<string>();
                var lstValuesProducao = new ArrayList();

                #endregion

                #region �reas e Eixos

                // Configura��es do ChartArea.
                this.chtProducaoPendente.ChartAreas.Add("Area");

                // Configura��es do Eixo X.
                this.chtProducaoPendente.ChartAreas[0].AxisX.IsMarginVisible = true;
                this.chtProducaoPendente.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                this.chtProducaoPendente.ChartAreas[0].AxisX.IntervalOffset = 1;
                this.chtProducaoPendente.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;

                // Configura��es do Eixo Y.
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalOffset = 0;
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chtProducaoPendente.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtProducaoPendente.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
                this.chtProducaoPendente.ChartAreas[0].AxisY.IntervalOffset = 99999;

                #endregion

                #region T�tulo

                // Configura��es do t�tulo do gr�fico.
                this.chtProducaoPendente.Titles.Add("ProducaoPendente");
                this.chtProducaoPendente.Titles[0].IsDockedInsideChartArea = false;
                this.chtProducaoPendente.Titles[0].Font = new Font(this.chtProducaoPendente.Titles[0].Font, FontStyle.Bold);
                this.chtProducaoPendente.Titles[0].Text = "PRODU��O PENDENTE";

                #endregion

                #region Legenda

                // Legenda do gr�fico.
                //this.chtProducaoPendente.Legends.Add("ProducaoPendente");

                #endregion

                #region S�ries

                // Cria e configura a s�rie do gr�fico.
                seriePendente.ChartType = SeriesChartType.Column;
                seriePendente.Color = Color.OrangeRed;
                seriePendente.XValueType = ChartValueType.String;
                seriePendente.YValueType = ChartValueType.Int32;
                seriePendente.IsValueShownAsLabel = true;

                // Insere as s�ries no gr�fico.
                this.chtProducaoPendente.Series.Add(seriePendente);

                #endregion

                #region Dados

                // Popula os arrays de dados.
                foreach (var i in lstResultado.Keys)
                {
                    lstDias.Add(i.ToString());
                    lstValuesProducao.Add(lstResultado[i]);
                }

                // Desenha os dados no gr�fico.
                this.chtProducaoPendente.Series[0].Points.DataBindXY(lstDias, lstValuesProducao);

                #endregion
            }
        }
    }
}