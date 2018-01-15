using System;
using Glass.Data.RelDAL;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Glass.Data.Helper;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelComercial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GraficoDataEntrega();

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel =
                "<b>M2:</b> Pedidos nas situações Aguardando Confirmação Financeiro, Em Conferência, Conferido, Confirmado PCP," +
                    " Liberado Parcialmente e Liberado; Não considera produtos de Revenda;" +
                    (Configuracoes.ProducaoConfig.ConsiderarApenasPedidosEntregaDeRotaPainelComercial ?
                        " Somente pedidos de Entrega que possuem rota;" : "") +
                    (Configuracoes.PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ?
                        " M2 calculado do produto." : "M2 real do produto.") + "<br />" +
                "<b>Data:</b> Data de entrega do pedido.";
        }
    
        private void GraficoDataEntrega()
        {
            var dados = GraficoDataEntregaDAO.Instance.ObtemDadosEntrega(DateTime.Today, 10);
    
            // Se nenhum dado for buscado o gráfico não é gerado.
            if (dados != null && dados.Count > 0)
            {
                #region Declarações
    
                // Series do Gráfico.
                var serieDados = new Series("Total m²");
                var serieMeta = new Series("Limite m²");
    
                #endregion
    
                #region Áreas e Eixos
    
                // Configurações do ChartArea.
                this.chtDataEntrega.ChartAreas.Add("Area");
    
                // Configurações do Eixo X.
                this.chtDataEntrega.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtDataEntrega.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtDataEntrega.ChartAreas[0].AxisX.IsStartedFromZero = true;
                
                // Configurações do Eixo Y.
                this.chtDataEntrega.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtDataEntrega.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtDataEntrega.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtDataEntrega.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
    
                #endregion
    
                #region Título
    
                // Configurações do título do gráfico.
                this.chtDataEntrega.Titles.Add("EntregaDiaria");
                this.chtDataEntrega.Titles[0].Font = new Font(this.chtDataEntrega.Titles[0].Font, FontStyle.Bold);
                this.chtDataEntrega.Titles[0].Text = "M² ENTREGA POR DATA";
    
                #endregion
    
                #region Legenda
    
                // Legenda do gráfico.
                this.chtDataEntrega.Legends.Add("EntregaDiaria");
    
                #endregion
    
                #region Séries
    
                // Define o tipo de gráfico.
                serieDados.ChartType = SeriesChartType.Column;
                serieMeta.ChartType = SeriesChartType.Line;
                serieMeta.BorderDashStyle = ChartDashStyle.Dash;
                
                // Define o tipo de valor do eixo X.
                serieDados.XValueType = ChartValueType.String;
                serieMeta.XValueType = ChartValueType.String;
                
                // Define se o valor será mostrado em uma label.
                serieDados.IsValueShownAsLabel = true;
                serieMeta.IsValueShownAsLabel = true;
    
                // Define a cor das séries.
                ColorConverter cc = new ColorConverter();
                serieDados.Color = (Color)cc.ConvertFromString("#418CF0");
                serieMeta.Color = (Color)cc.ConvertFromString("#F08C41");
    
                // Insere as séries no gráfico.
                this.chtDataEntrega.Series.Add(serieDados);
                this.chtDataEntrega.Series.Add(serieMeta);
    
                #endregion
    
                #region Dados
    
                // Desenha os dados no gráfico.
                this.chtDataEntrega.Series[0].Points.DataBindXY(dados, "DataEntrega", dados, "TotalM2");
                this.chtDataEntrega.Series[1].Points.DataBindXY(dados, "DataEntrega", dados, "Meta");
    
                #endregion
            }
        }
    }
}
