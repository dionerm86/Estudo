using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using WebGlass.Business.ProducaoDiariaRealizada.Entidade;
using WebGlass.Business.ProducaoDiariaRealizada.Fluxo;
using System.Web.UI.HtmlControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelComercialSetores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((PainelGraficos)this.Master).TempoSegundosAtualizar = 1200;
            MontaGraficoPainelComercial();

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel =
                "<b>M2:</b> Pedidos que n�o est�o nas situa��es Ativo e Cancelado; " +
                    "Pedidos dos tipos Produ��o e Venda; Caso haja confer�ncia gerada, " +
                    "considera pedidos em confer�ncia nas situa��es Finalizado, Impresso Comum ou Impresso.<br />" +
                "<b>Capaciade:</b> � a capacidade de produ��o definida para o setor na tela PCP > Capacidade de Produ��o Di�ria.<br />" +
                "<b>Data:</b> Data de f�brica do pedido, se houver confer�ncia gerada, sen�o, data de entrega.";
        }
    
        private void MontaGraficoPainelComercial()
        {
            var lstCharts = new List<Chart>();
            var idsSetores = SetorDAO.Instance.ObtemIdsSetoresPainelComercial();
    
            for (int i = 0; i < idsSetores.Count; i++)
            {
                var dataFabrica = DateTime.Today;
                int countData = 0;
                var lstResultado = new List<ProducaoDiariaRealizada>();
    
                while (countData < 10)
                {
                    if (!dataFabrica.DiaUtil())
                        dataFabrica = dataFabrica.AddDays(1);
                    else
                    {
                        var dataEntrega = dataFabrica;
    
                        for (int j = 0; j < PCPConfig.Etiqueta.DiasDataFabrica; j++)
                        {
                            dataEntrega = dataEntrega.AddDays(1);
    
                            while (!dataEntrega.DiaUtil())
                                dataEntrega = dataEntrega.AddDays(1);
                        }
    
                        lstResultado.AddRange(BuscarEValidar.Instance.ObtemDadosProducaoForPainelComercialSetores(idsSetores[i], dataFabrica, dataEntrega));
    
                        dataFabrica = dataFabrica.AddDays(1);
                        countData++;
                    }
                }
    
    
                #region Declara��es
    
                //Grafico
                var chart = new Chart();
                chart.Width = 450;
    
                // Series do Gr�fico.
                var serieCapacidade = new Series("Capacidade");
                var seriePrevisto = new Series("Previsto");
    
                #endregion
    
                #region �reas e Eixos
    
                // Configura��es do ChartArea.
                chart.ChartAreas.Add("Area");
    
                // Configura��es do Eixo Y.
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.IsMarginVisible = true;
    
                // Configura��es do Eixo X.
                chart.ChartAreas[0].AxisY.IsMarginVisible = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
    
                #endregion
    
                #region T�tulo
    
                // Configura��es do t�tulo do gr�fico.
                chart.Titles.Add("ProducaoDiaria");
                chart.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);
                chart.Titles[0].Text = Data.Helper.Utils.GetSetores.Where(f => f.IdSetor == idsSetores[i]).FirstOrDefault().Descricao;
                chart.Titles[0].Docking = Docking.Top;
                chart.Titles[0].Alignment = ContentAlignment.MiddleCenter;
    
                #endregion
    
                #region Legenda
    
                // Legenda do gr�fico.
                //chart.Legends.Add("ProducaoDiaria");
    
                #endregion
    
                #region S�ries
    
                // Define o tipo de gr�fico.
                serieCapacidade.ChartType = SeriesChartType.Bar;
                seriePrevisto.ChartType = SeriesChartType.Bar;
    
                // Define o tipo de valor do eixo X.
                serieCapacidade.XValueType = ChartValueType.String;
                seriePrevisto.XValueType = ChartValueType.String;
    
                // Define se o valor ser� mostrado em uma label.
                serieCapacidade.IsValueShownAsLabel = false;
                seriePrevisto.IsValueShownAsLabel = true;
    
                // Define a cor das s�ries.
                ColorConverter cc = new ColorConverter();
                serieCapacidade.Color = (Color)cc.ConvertFromString("#F08C41");
                seriePrevisto.Color = (Color)cc.ConvertFromString("#418CF0");
    
                // Insere as s�ries no gr�fico.
                chart.Series.Add(serieCapacidade);
                chart.Series.Add(seriePrevisto);
    
                #endregion
    
                #region Dados
    
                lstResultado = lstResultado.OrderBy(f => f.DataProducao).ToList();
    
                // Desenha os dados no gr�fico.
                chart.Series[0].Points.DataBindXY(lstResultado, "DataProducaoStr", lstResultado, "Capacidade");
                chart.Series[1].Points.DataBindXY(lstResultado, "DataProducaoStr", lstResultado, "M2Previsto");
    
                #endregion
    
                lstCharts.Add(chart);
            }
    
            var lstDivs = new List<HtmlGenericControl>() { new HtmlGenericControl("DIV") };
            var countDivAtual = 0;
    
            for (int i = 0; i < lstCharts.Count; i++)
            {
                if (i % 3 == 0)
                {
                    lstDivs.Add(new HtmlGenericControl("DIV"));
                    countDivAtual++;
                }
    
                lstDivs[countDivAtual].Controls.Add(lstCharts[i]);
            }
    
            foreach (var div in lstDivs)
                divGraficos.Controls.Add(div);
        }
    }
}
