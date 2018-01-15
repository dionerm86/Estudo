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
                "<b>M2:</b> Pedidos nas situa��es Aguardando Confirma��o Financeiro, Em Confer�ncia, Conferido, Confirmado PCP," +
                    " Liberado Parcialmente e Liberado; N�o considera produtos de Revenda;" +
                    (Configuracoes.ProducaoConfig.ConsiderarApenasPedidosEntregaDeRotaPainelComercial ?
                        " Somente pedidos de Entrega que possuem rota;" : "") +
                    (Configuracoes.PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ?
                        " M2 calculado do produto." : "M2 real do produto.") + "<br />" +
                "<b>Data:</b> Data de entrega do pedido.";
        }
    
        private void GraficoDataEntrega()
        {
            var dados = GraficoDataEntregaDAO.Instance.ObtemDadosEntrega(DateTime.Today, 10);
    
            // Se nenhum dado for buscado o gr�fico n�o � gerado.
            if (dados != null && dados.Count > 0)
            {
                #region Declara��es
    
                // Series do Gr�fico.
                var serieDados = new Series("Total m�");
                var serieMeta = new Series("Limite m�");
    
                #endregion
    
                #region �reas e Eixos
    
                // Configura��es do ChartArea.
                this.chtDataEntrega.ChartAreas.Add("Area");
    
                // Configura��es do Eixo X.
                this.chtDataEntrega.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtDataEntrega.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtDataEntrega.ChartAreas[0].AxisX.IsStartedFromZero = true;
                
                // Configura��es do Eixo Y.
                this.chtDataEntrega.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtDataEntrega.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtDataEntrega.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtDataEntrega.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
    
                #endregion
    
                #region T�tulo
    
                // Configura��es do t�tulo do gr�fico.
                this.chtDataEntrega.Titles.Add("EntregaDiaria");
                this.chtDataEntrega.Titles[0].Font = new Font(this.chtDataEntrega.Titles[0].Font, FontStyle.Bold);
                this.chtDataEntrega.Titles[0].Text = "M� ENTREGA POR DATA";
    
                #endregion
    
                #region Legenda
    
                // Legenda do gr�fico.
                this.chtDataEntrega.Legends.Add("EntregaDiaria");
    
                #endregion
    
                #region S�ries
    
                // Define o tipo de gr�fico.
                serieDados.ChartType = SeriesChartType.Column;
                serieMeta.ChartType = SeriesChartType.Line;
                serieMeta.BorderDashStyle = ChartDashStyle.Dash;
                
                // Define o tipo de valor do eixo X.
                serieDados.XValueType = ChartValueType.String;
                serieMeta.XValueType = ChartValueType.String;
                
                // Define se o valor ser� mostrado em uma label.
                serieDados.IsValueShownAsLabel = true;
                serieMeta.IsValueShownAsLabel = true;
    
                // Define a cor das s�ries.
                ColorConverter cc = new ColorConverter();
                serieDados.Color = (Color)cc.ConvertFromString("#418CF0");
                serieMeta.Color = (Color)cc.ConvertFromString("#F08C41");
    
                // Insere as s�ries no gr�fico.
                this.chtDataEntrega.Series.Add(serieDados);
                this.chtDataEntrega.Series.Add(serieMeta);
    
                #endregion
    
                #region Dados
    
                // Desenha os dados no gr�fico.
                this.chtDataEntrega.Series[0].Points.DataBindXY(dados, "DataEntrega", dados, "TotalM2");
                this.chtDataEntrega.Series[1].Points.DataBindXY(dados, "DataEntrega", dados, "Meta");
    
                #endregion
            }
        }
    }
}
