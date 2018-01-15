using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.ItemTemplates;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLstProdProducao : System.Web.UI.UserControl
    {
        #region Propiedades

        public object IdProdPedProducao
        {
            get
            {
                return hdfIdProdPedProducao.Value.StrParaInt();
            }
            set
            {
                hdfIdProdPedProducao.Value = value.ToString();
            }
        }

        #endregion

        #region Métodos Protegidos 

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdPecasParent_DataBound(object sender, EventArgs e)
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

        protected void grdPecasParent_Load(object sender, EventArgs e)
        {
            Setor[] lstSetor = SetorDAO.Instance.GetOrdered();

            int QTD_COLUNAS_GRID = 7;

            var exibirSetores = UserInfo.GetUserInfo.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Vendedor || !ProducaoConfig.TelaConsulta.EsconderPecasParaVendedores;

            if (grdPecasParent.Columns.Count <= QTD_COLUNAS_GRID)
            {
                if (exibirSetores)
                    for (int i = 0; i < lstSetor.Length; i++)
                    {
                        if (!lstSetor[i].ExibirRelatorio)
                            continue;

                        TemplateField tf = new TemplateField();
                        tf.HeaderText = lstSetor[i].Descricao;
                        grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, tf);
                    }

                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                {
                    BoundField bfDataPerda = new BoundField();
                    bfDataPerda.DataField = "DataPerda";
                    bfDataPerda.HeaderText = "Perda";
                    bfDataPerda.SortExpression = "DataPerda";
                    grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, bfDataPerda);
                }

                if (Glass.Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao)
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaFabrica";
                    bfPrevEntrega.HeaderText = "Pronto Fábrica";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntregaFabrica";
                    grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, bfPrevEntrega);
                }
                else
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaExibicao";
                    bfPrevEntrega.HeaderText = "Prev. Entrega";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntrega";
                    grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, bfPrevEntrega);
                }

                if (PedidoConfig.LiberarPedido)
                {
                    BoundField bfDataLib = new BoundField();
                    bfDataLib.DataField = "DataLiberacaoPedido";
                    bfDataLib.HeaderText = "Data Lib.";
                    bfDataLib.DataFormatString = "{0:d}";
                    bfDataLib.SortExpression = "DataLiberacaoPedido";
                    grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, bfDataLib);
                }

                BoundField bfPlanoCorte = new BoundField();
                bfPlanoCorte.DataField = "PlanoCorte";
                bfPlanoCorte.HeaderText = "Plano Corte";
                bfPlanoCorte.SortExpression = "PlanoCorte";
                grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, bfPlanoCorte);

                if (!ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela)
                {
                    TemplateField tfNumEtiqueta = new TemplateField();
                    tfNumEtiqueta.ItemStyle.Wrap = false;
                    tfNumEtiqueta.HeaderText = "Etiqueta";
                    tfNumEtiqueta.SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecasParent.Columns.Insert(grdPecasParent.Columns.Count - 1, tfNumEtiqueta);
                }
                else
                {
                    grdPecasParent.Columns[3].ItemStyle.Wrap = false;
                    grdPecasParent.Columns[3].HeaderText = "Etiqueta";
                    grdPecasParent.Columns[3].SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecasParent.Columns[3].Visible = true;
                }
            }

            int numeroColuna = 0;
            //bool colunaPlanoCorteAdicionada = false;
            for (int i = 0; i < lstSetor.Length; i++)
            {
                if (!lstSetor[i].ExibirRelatorio)
                    continue;

                var itens = new List<MultipleItemTemplate.Item>() {
                    new MultipleItemTemplate.Item("VetDataLeitura[" + numeroColuna + "]", String.Empty),
                    new MultipleItemTemplate.Item("SetorNaoObrigatorio[" + numeroColuna + "]", String.Empty)
                };

                if ((lstSetor[i].Corte || lstSetor[i].Laminado) && PCPConfig.Etiqueta.UsarControleChapaCorte)
                {
                    itens.AddRange(new[] {
                        new MultipleItemTemplate.Item("NumEtiquetaChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<br /><b>Matéria-prima: {0}</b>", "", "", "TemLeitura[" + numeroColuna + "]"),
                        new MultipleItemTemplate.Item("NumeroNFeChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<i> NFe: {0}</i>", "", "", "TemLeitura[" + numeroColuna + "]"),
                        new MultipleItemTemplate.Item("LoteChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<i> Lote: {0}</i>", "", "", "TemLeitura[" + numeroColuna + "]")});

                    //colunaPlanoCorteAdicionada = true;
                }

                itens.Insert(2, new MultipleItemTemplate.Item("VetNomeFuncLeitura[" + numeroColuna + "]", String.Empty, typeof(string), "<br />Func.: {0}"));

                if (exibirSetores)
                    ((TemplateField)grdPecasParent.Columns[QTD_COLUNAS_GRID - 1 + numeroColuna]).ItemTemplate = new MultipleItemTemplate(itens.ToArray());

                numeroColuna++;
            }

            int posEtiq = ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela ? 3 : grdPecasParent.Columns.Count - 2;
            ((TemplateField)grdPecasParent.Columns[posEtiq]).ItemTemplate = new MultipleItemTemplate(
                new MultipleItemTemplate.Item("NumEtiquetaExibir", String.Empty),
                new MultipleItemTemplate.Item("IdImpressao", String.Empty, typeof(string), "", "<br />(Impressão: ", ")", "PecaCancelada"));
        }

        protected void grdPecasParentChild_DataBound(object sender, EventArgs e)
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

        protected void grdPecasParentChild_Load(object sender, EventArgs e)
        {
            Setor[] lstSetor = SetorDAO.Instance.GetOrdered();

            var grdPecasParentChild = (GridView)sender;

            int QTD_COLUNAS_GRID = 5;

            var exibirSetores = UserInfo.GetUserInfo.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Vendedor || !ProducaoConfig.TelaConsulta.EsconderPecasParaVendedores;

            if (grdPecasParentChild.Columns.Count <= QTD_COLUNAS_GRID)
            {
                if (exibirSetores)
                    for (int i = 0; i < lstSetor.Length; i++)
                    {
                        if (!lstSetor[i].ExibirRelatorio)
                            continue;

                        TemplateField tf = new TemplateField();
                        tf.HeaderText = lstSetor[i].Descricao;
                        grdPecasParentChild.Columns.Add(tf);
                    }

                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                {
                    BoundField bfDataPerda = new BoundField();
                    bfDataPerda.DataField = "DataPerda";
                    bfDataPerda.HeaderText = "Perda";
                    bfDataPerda.SortExpression = "DataPerda";
                    grdPecasParentChild.Columns.Add(bfDataPerda);
                }

                if (Glass.Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao)
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaFabrica";
                    bfPrevEntrega.HeaderText = "Pronto Fábrica";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntregaFabrica";
                    grdPecasParentChild.Columns.Add(bfPrevEntrega);
                }
                else
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaExibicao";
                    bfPrevEntrega.HeaderText = "Prev. Entrega";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntrega";
                    grdPecasParentChild.Columns.Add(bfPrevEntrega);
                }

                if (PedidoConfig.LiberarPedido)
                {
                    BoundField bfDataLib = new BoundField();
                    bfDataLib.DataField = "DataLiberacaoPedido";
                    bfDataLib.HeaderText = "Data Lib.";
                    bfDataLib.DataFormatString = "{0:d}";
                    bfDataLib.SortExpression = "DataLiberacaoPedido";
                    grdPecasParentChild.Columns.Add(bfDataLib);
                }

                BoundField bfPlanoCorte = new BoundField();
                bfPlanoCorte.DataField = "PlanoCorte";
                bfPlanoCorte.HeaderText = "Plano Corte";
                bfPlanoCorte.SortExpression = "PlanoCorte";
                grdPecasParentChild.Columns.Add(bfPlanoCorte);

                if (!ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela)
                {
                    TemplateField tfNumEtiqueta = new TemplateField();
                    tfNumEtiqueta.ItemStyle.Wrap = false;
                    tfNumEtiqueta.HeaderText = "Etiqueta";
                    tfNumEtiqueta.SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecasParentChild.Columns.Add(tfNumEtiqueta);
                }
                else
                {
                    grdPecasParentChild.Columns[3].ItemStyle.Wrap = false;
                    grdPecasParentChild.Columns[3].HeaderText = "Etiqueta";
                    grdPecasParentChild.Columns[3].SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecasParentChild.Columns[3].Visible = true;
                }
            }

            int numeroColuna = 0;
            //bool colunaPlanoCorteAdicionada = false;
            for (int i = 0; i < lstSetor.Length; i++)
            {
                if (!lstSetor[i].ExibirRelatorio)
                    continue;

                var itens = new List<MultipleItemTemplate.Item>() {
                    new MultipleItemTemplate.Item("VetDataLeitura[" + numeroColuna + "]", String.Empty),
                    new MultipleItemTemplate.Item("SetorNaoObrigatorio[" + numeroColuna + "]", String.Empty)
                };

                if ((lstSetor[i].Corte || lstSetor[i].Laminado) && PCPConfig.Etiqueta.UsarControleChapaCorte)
                {
                    itens.AddRange(new[] {
                        new MultipleItemTemplate.Item("NumEtiquetaChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<br /><b>Matéria-prima: {0}</b>", "", "", "TemLeitura[" + numeroColuna + "]"),
                        new MultipleItemTemplate.Item("NumeroNFeChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<i> NFe: {0}</i>", "", "", "TemLeitura[" + numeroColuna + "]"),
                        new MultipleItemTemplate.Item("LoteChapa", lstSetor[i].Corte ? "Corte[" + numeroColuna + "]" : "Laminado[" + numeroColuna + "]", typeof(string), "<i> Lote: {0}</i>", "", "", "TemLeitura[" + numeroColuna + "]")});

                    //colunaPlanoCorteAdicionada = true;
                }

                itens.Insert(2, new MultipleItemTemplate.Item("VetNomeFuncLeitura[" + numeroColuna + "]", String.Empty, typeof(string), "<br />Func.: {0}"));

                if (exibirSetores)
                    ((TemplateField)grdPecasParentChild.Columns[QTD_COLUNAS_GRID + numeroColuna]).ItemTemplate = new MultipleItemTemplate(itens.ToArray());

                numeroColuna++;
            }

            int posEtiq = ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela ? 3 : grdPecasParentChild.Columns.Count - 1;
            ((TemplateField)grdPecasParentChild.Columns[posEtiq]).ItemTemplate = new MultipleItemTemplate(
                new MultipleItemTemplate.Item("NumEtiquetaExibir", String.Empty),
                new MultipleItemTemplate.Item("IdImpressao", String.Empty, typeof(string), "", "<br />(Impressão: ", ")", "PecaCancelada"));
        }

        #endregion

        protected void grdPecasParentChild_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VoltarPeca")
            {
                ProdutoPedidoProducaoDAO.Instance.VoltarPeca(0, 0, true);
            }
        }
    }
}