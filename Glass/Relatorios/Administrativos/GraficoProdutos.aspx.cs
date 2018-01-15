using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelDAL;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class GraficoProdutos : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(GraficoProdutos));

            if (!IsPostBack && ((TextBox)ctrlDataIni.FindControl("txtData")).Text == string.Empty)
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
            uint idLoja = !string.IsNullOrEmpty(drpLoja.SelectedValue) ? Convert.ToUInt32(drpLoja.SelectedValue) : 0;
            uint idVendedor = !string.IsNullOrEmpty(drpVendedor.SelectedValue) ? Convert.ToUInt32(drpVendedor.SelectedValue) : 0;
            var idCliente = !string.IsNullOrEmpty(txtNumCli.Text) ? txtNumCli.Text.StrParaInt() : 0;
            var nomeCliente = txtNome.Text;
            uint grupo = !string.IsNullOrEmpty(drpGrupoProd.SelectedValue) ? Convert.ToUInt32(drpGrupoProd.SelectedValue) : 0;
            uint subGrupo = !string.IsNullOrEmpty(drpSubgrupoProd.SelectedValue) ? Convert.ToUInt32(drpSubgrupoProd.SelectedValue) : 0;
            int quantidade =  !string.IsNullOrEmpty(txtQuantidade.Text) ? Convert.ToInt32(txtQuantidade.Text) : 0;
            int tipo = !string.IsNullOrEmpty(drpTipo.SelectedValue) ? Convert.ToInt32(drpTipo.SelectedValue) : 0;
            string dataIni = Convert.ToDateTime(((TextBox)ctrlDataIni.FindControl("txtData")).Text).ToString("dd/MM/yyyy");
            string dataFim = Convert.ToDateTime(((TextBox)ctrlDataFim.FindControl("txtData")).Text).ToString("dd/MM/yyyy");
            string codProduto = txtCodProd.Text;
            string descricaoProduto = txtDescr.Text;
            bool apenasMS = chkApenasMateriaPrima.Checked;
    
            //Busca os dados que servirão para preencher as séries do gráfico
            var v = GraficoProdutosDAO.Instance.GetMaisVendidos(idLoja, idVendedor, idCliente, nomeCliente, grupo, subGrupo, quantidade,
                tipo, dataIni, dataFim, codProduto, descricaoProduto, apenasMS);
    
            if (v.Length > 0)
            {
                var lstValuesX = new List<string>();
                var lstValuesY = new List<double>();
    
                foreach (var prod in v)
                {
                    lstValuesX.Add(prod.DescrProduto + (tipo == 1 ? " (#VALY{N} " + (ProdutoDAO.Instance.IsVidro(null, (int)prod.IdProd) ? "m²" :
                        ProdutoDAO.Instance.ObtemUnidadeMedida((int)prod.IdProd).ToLower()) + ")" :
                    " (#VALY{C})"));
                    lstValuesY.Add((double)prod.ValorExibir);
                }
    
                Chart1.ChartAreas.Add("Area");
                //Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
                Chart1.Series.Add("Produto");
    
                Chart1.Legends.Add("legenda");
                Chart1.Series["Produto"].Legend = "legenda";
                Chart1.Series["Produto"].IsValueShownAsLabel = true;
                Chart1.Series["Produto"].Points.DataBindXY(lstValuesX, lstValuesY);
                Chart1.Series["Produto"].ToolTip = "#VALX";
                Chart1.Series["Produto"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
                Chart1.Series["Produto"].Label = "#VALX";
                Chart1.Series["Produto"]["PieDrawingStyle"] = "SoftEdge";
    
                if (chkLegenda.Checked)
                {
                    Chart1.Series["Produto"].IsVisibleInLegend = true;
                    Chart1.Series["Produto"]["PieLabelStyle"] = "Disabled";
                    Chart1.Width = new Unit(1000);
                    Chart1.Height = new Unit(500);
                }    
                else
                {
                    Chart1.Series["Produto"].IsVisibleInLegend = false;
                    Chart1.Series["Produto"]["PieLabelStyle"] = "Outside";
                    Chart1.Series["Produto"]["PieLineColor"] = "Black";
                    Chart1.Width = new Unit(1000);
                    Chart1.Height = new Unit(400);
                }
    
                byte[] buffer = Util.Helper.ChartToByteArray(Chart1);
    
                hdfTempFile.Value = Glass.Conversoes.CodificaPara64(buffer); //String.Concat("file:///", Glass.Util.Helper.SalvaGraficoTemp(Chart1, "Produtos" + DateTime.Now.ToString("ddMMyyyyHHmmss")));
                //Atribuo o databind de novo para exibir corretamente o label
                Chart1.Series["Produto"].Points.DataBindXY(lstValuesX, lstValuesY);
            }
        }

        #region Métodos AJAX

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (string.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(idCli.StrParaUint()))
                return "Erro;Cliente não encontrado.";
            
            return "Ok;" + ClienteDAO.Instance.GetNome(idCli.StrParaUint());
        }

        #endregion
    }
}
