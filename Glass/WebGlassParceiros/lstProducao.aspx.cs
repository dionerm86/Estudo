using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.Model;
using Glass.Data.ItemTemplates;
using Glass.Configuracoes;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class lstProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPecas.PageIndex = 0;
        }
    
        protected void grdPecas_DataBound(object sender, EventArgs e)
        {
            GridView grid = (GridView)sender;
    
            // Muda a cor das linhas da grid
            foreach (GridViewRow row in grid.Rows)
            {
                if (row.Cells[0].FindControl("hdfCorLinha") == null)
                    continue;
    
                Color color = Color.FromName(((HiddenField)row.Cells[0].FindControl("hdfCorLinha")).Value);
                foreach (TableCell c in row.Cells)
                    c.ForeColor = color;
            }
        }
    
        protected void grdPecas_Load(object sender, EventArgs e)
        {
            Setor[] lstSetor = Data.Helper.Utils.GetSetores;
            int QTD_COLUNAS_GRID = 10;
            int QTD_COLUNAS_FINAL_GRID = Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca ? 5 : 4;
    
            if (grdPecas.Columns.Count <= QTD_COLUNAS_GRID)
            {
                if (UserInfo.GetUserInfo.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Vendedor ||
                    !ProducaoConfig.TelaConsulta.EsconderPecasParaVendedores)
                    for (int i = 0; i < lstSetor.Length; i++)
                    {
                        if (!Data.Helper.Utils.GetSetores[i].ExibirRelatorio)
                            continue;
    
                        TemplateField tf = new TemplateField();
                        tf.HeaderText = lstSetor[i].Descricao;
                        grdPecas.Columns.Add(tf);
                    }
    
                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                {
                    BoundField bfDataPerda = new BoundField();
                    bfDataPerda.DataField = "DataPerda";
                    bfDataPerda.HeaderText = "Perda";
                    bfDataPerda.SortExpression = "DataPerda";
                    grdPecas.Columns.Add(bfDataPerda);
                }
    
                if (Glass.Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao)
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaFabrica";
                    bfPrevEntrega.HeaderText = "Pronto Fábrica";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntregaFabrica";
                    grdPecas.Columns.Add(bfPrevEntrega);
    
                    //lblPeriodoFabrica.Text = "Período (Pronto Fábrica)";
                }
                else
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaExibicao";
                    bfPrevEntrega.HeaderText = "Prev. Entrega";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntrega";
                    grdPecas.Columns.Add(bfPrevEntrega);
                }
    
                BoundField bfDataLib = new BoundField();
                bfDataLib.DataField = "DataLiberacaoPedido";
                bfDataLib.HeaderText = "Data Lib.";
                bfDataLib.DataFormatString = "{0:d}";
                bfDataLib.SortExpression = "DataLiberacaoPedido";
                grdPecas.Columns.Add(bfDataLib);
    
                BoundField bfPlanoCorte = new BoundField();
                bfPlanoCorte.DataField = "PlanoCorte";
                bfPlanoCorte.HeaderText = "Plano Corte";
                bfPlanoCorte.SortExpression = "PlanoCorte";
                grdPecas.Columns.Add(bfPlanoCorte);
    
                if (ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela)
                {
                    TemplateField tfNumEtiqueta = new TemplateField();
                    tfNumEtiqueta.ItemStyle.Wrap = false;
                    tfNumEtiqueta.HeaderText = "Etiqueta";
                    tfNumEtiqueta.SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";
    
                    grdPecas.Columns.Add(tfNumEtiqueta);
                }
                else
                {
                    grdPecas.Columns[3].ItemStyle.Wrap = false;
                    grdPecas.Columns[3].HeaderText = "Etiqueta";
                    grdPecas.Columns[3].SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";
    
                    grdPecas.Columns[3].Visible = true;
                }
            }
    
            if (ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela)
                QTD_COLUNAS_FINAL_GRID--;
    
            int numeroColuna = 0;
            for (int i = 0; i < Data.Helper.Utils.GetSetores.Length; i++)
            {
                if (!Data.Helper.Utils.GetSetores[i].ExibirRelatorio)
                    continue;
    
                ((TemplateField)grdPecas.Columns[QTD_COLUNAS_GRID + numeroColuna]).ItemTemplate = new MultipleItemTemplate(
                    new MultipleItemTemplate.Item("VetDataLeitura[" + numeroColuna + "]", null),
                    Data.Helper.Utils.GetSetores[i].Corte && PCPConfig.Etiqueta.UsarControleChapaCorte ? new MultipleItemTemplate.Item("NumEtiquetaChapa", null, typeof(string), "<br /><b>Chapa: {0}</b>", "", "", "TemLeitura[" + numeroColuna + "]") : null,
                    Data.Helper.Utils.GetSetores[i].Corte && PCPConfig.Etiqueta.UsarControleChapaCorte ? new MultipleItemTemplate.Item("NumeroNFeChapa", null, typeof(string), "<i> NFe: {0}</i>", "", "", "TemLeitura[" + numeroColuna + "]") : null);
    
                numeroColuna++;
            }
        }
    }
}
