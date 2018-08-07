using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.ItemTemplates;
using System.Linq;
using Glass.Configuracoes;
using System.Web.UI.HtmlControls;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class LstProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Producao.LstProducao));

            if (!IsPostBack)
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                if (UserInfo.GetUserInfo.TipoUsuario == (int)Data.Helper.Utils.TipoFuncionario.Vendedor &&
                    ProducaoConfig.TelaConsulta.EsconderLinksImpressaoParaVendedores)
                {
                    lnkImprimir.Visible = false;
                    lnkExportarExcel.Visible = false;
                }

                txtNumPedido.Text = Request["idPedido"];

                if (PCPConfig.ExibirPecasCancMaoObraPadrao)
                    cbdExibirPecas.SelectedValue = "0,1";

                PreencherControles();

                hdfRefresh.Value = "1";

                /* Chamado 44192. */
                if (Request.QueryString.Count <= 1)
                    imgPesq_Click(null, null);
            }
            /* Chamado 45021 e 45249. */
            else
            {
                grdPecas.PageIndex = 0;
                if (hdfRefresh.Value == "0")
                {
                    imgPesq_Click(null, null);
                    hdfRefresh.Value = "1";
                }
            }

            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "submit", "FindControl('hdfBenef', 'input').value = " + ctrlBenefSetor1.ClientID + ".Selecionados();");

            if (Request["cliente"] != null)
            {
                txtNumCli.Text = Request["cliente"];
                ctrlDataIniEnt.DataString = Request["inicio"];
                ctrlDataFimEnt.DataString = Request["fim"];
                drpSituacao.CheckAll = false;
                drpSituacao.SelectedValue = Request["situacao"];
            }

            trImpressaoRoteiro.Visible = Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {                 
            Response.Redirect("~/Cadastros/Producao/LstProducao.aspx" + ObterQueryStrPesquisa(true));
        }

        protected void direcionaPagina()
        {
            Response.Redirect("~/Cadastros/Producao/LstProducao.aspx" + ObterQueryStrPesquisa(false));
        }

        public bool EmpresaTrabalhaComOrdemCarga()
        {
            return OrdemCargaConfig.UsarControleOrdemCarga;
        }

        public bool IsLiberacao()
        {
            return PedidoConfig.LiberarPedido;
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

            uint idPedido = !String.IsNullOrEmpty(txtNumPedido.Text) ? Glass.Conversoes.StrParaUint(txtNumPedido.Text) : 0;

            if (idPedido > 0)
            {
                int qtdPecaEstoque = ProdutosPedidoEspelhoDAO.Instance.ObtemQtdPecasVidroEstoquePedido(idPedido);
                if (qtdPecaEstoque > 0 && !PedidoDAO.Instance.IsProducao(null, idPedido))
                    lblPecasEstoque.Text = "Este pedido possui " + qtdPecaEstoque + " peça(s) de estoque.";

                if (ProdutosPedidoEspelhoDAO.Instance.PossuiPecaASerImpressa(idPedido) && !lblPecasEstoque.Text.Contains("Este pedido possui peças não impressas."))
                    lblPecasEstoque.Text += " Este pedido possui peças não impressas.";
            }
        }

        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Setor setorSelecionado = SetorDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(drpSetor.SelectedValue));

            if (drpSituacao.SelectedValue == ((int)ProdutoPedidoProducao.SituacaoEnum.Perda).ToString())
                lblPeriodoSetor.Text = "Período (Perda)";
            else if (drpSetor.SelectedValue != "0")
                lblPeriodoSetor.Text = "Período (" + drpSetor.SelectedItem.Text + ")";
            else
                lblPeriodoSetor.Text = "Período (Setor)";

            if (setorSelecionado != null && setorSelecionado.ExibirRelatorio)
                trImpresaoSetorFiltrado.Visible = true;
            else
                trImpresaoSetorFiltrado.Visible = false;
        }

        protected void grdPecas_Load(object sender, EventArgs e)
        {
            Setor[] lstSetor = SetorDAO.Instance.GetOrdered();

            int QTD_COLUNAS_GRID = 12;

            var exibirSetores = UserInfo.GetUserInfo.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Vendedor || !ProducaoConfig.TelaConsulta.EsconderPecasParaVendedores;

            if (grdPecas.Columns.Count <= QTD_COLUNAS_GRID)
            {
                if (exibirSetores)
                    for (int i = 0; i < lstSetor.Length; i++)
                    {
                        if (!lstSetor[i].ExibirRelatorio)
                            continue;

                        TemplateField tf = new TemplateField();
                        tf.HeaderText = lstSetor[i].Descricao;
                        grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, tf);
                    }

                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                {
                    BoundField bfDataPerda = new BoundField();
                    bfDataPerda.DataField = "DataPerda";
                    bfDataPerda.HeaderText = "Perda";
                    bfDataPerda.SortExpression = "DataPerda";
                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfDataPerda);
                }

                if (Glass.Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao)
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaFabrica";
                    bfPrevEntrega.HeaderText = "Pronto Fábrica";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntregaFabrica";
                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfPrevEntrega);

                    lblPeriodoFabrica.Text = "Período (Pronto Fábrica)";
                }
                else
                {
                    BoundField bfPrevEntrega = new BoundField();
                    bfPrevEntrega.DataField = "DataEntregaExibicao";
                    bfPrevEntrega.HeaderText = "Prev. Entrega";
                    bfPrevEntrega.DataFormatString = "{0:d}";
                    bfPrevEntrega.SortExpression = "DataEntrega";
                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfPrevEntrega);
                }

                if (PedidoConfig.LiberarPedido)
                {
                    BoundField bfDataLib = new BoundField();
                    bfDataLib.DataField = "DataLiberacaoPedido";
                    bfDataLib.HeaderText = "Data Lib.";
                    bfDataLib.DataFormatString = "{0:d}";
                    bfDataLib.SortExpression = "DataLiberacaoPedido";
                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfDataLib);
                }

                BoundField bfPlanoCorte = new BoundField();
                bfPlanoCorte.DataField = "PlanoCorte";
                bfPlanoCorte.HeaderText = "Plano Corte";
                bfPlanoCorte.SortExpression = "PlanoCorte";
                grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfPlanoCorte);

                if (!ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela)
                {
                    TemplateField tfNumEtiqueta = new TemplateField();
                    tfNumEtiqueta.ItemStyle.Wrap = false;
                    tfNumEtiqueta.HeaderText = "Etiqueta";
                    tfNumEtiqueta.SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, tfNumEtiqueta);
                }
                else
                {
                    grdPecas.Columns[3].ItemStyle.Wrap = false;
                    grdPecas.Columns[3].HeaderText = "Etiqueta";
                    grdPecas.Columns[3].SortExpression = "coalesce(NumEtiqueta, NumEtiquetaCanc)";

                    grdPecas.Columns[3].Visible = true;
                }

                if (PCPConfig.ControleCavalete)
                {
                    BoundField bfCavalete = new BoundField();
                    bfCavalete.DataField = "NumCavalete";
                    bfCavalete.HeaderText = "Cavalete";
                    bfCavalete.SortExpression = "NumCavalete";

                    grdPecas.Columns.Insert(grdPecas.Columns.Count - 1, bfCavalete);
                }

            }

            int numeroColuna = 0;

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
                }

                itens.Insert(2, new MultipleItemTemplate.Item("VetNomeFuncLeitura[" + numeroColuna + "]", String.Empty, typeof(string), "<br />Func.: {0}"));

                if (exibirSetores)
                    ((TemplateField)grdPecas.Columns[QTD_COLUNAS_GRID - 1 + numeroColuna]).ItemTemplate = new MultipleItemTemplate(itens.ToArray());

                numeroColuna++;
            }

            int posEtiq = ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela ? 3 : grdPecas.Columns.Count - (PCPConfig.ControleCavalete ? 3 : 2);
            ((TemplateField)grdPecas.Columns[posEtiq]).ItemTemplate = new MultipleItemTemplate(
                new MultipleItemTemplate.Item("NumEtiquetaExibir", String.Empty),
                new MultipleItemTemplate.Item("IdImpressao", String.Empty, typeof(string), "", "<br />(Impressão: ", ")", "PecaCancelada"));
        }

        #region Métodos AJAX

        [Ajax.AjaxMethod()]
        public string GetDescProd(string idProdPedProducao)
        {
            return ProdutoPedidoProducaoDAO.Instance.ObterDescrProdEtiqueta(null, idProdPedProducao.StrParaUint());
        }

        [Ajax.AjaxMethod()]
        public string VoltarPeca(string idProdPedProducao)
        {
            try
            {
                ProdutoPedidoProducaoDAO.Instance.VoltarPeca(idProdPedProducao.StrParaUint(), null, true);
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        #endregion

        protected string ExibirPecasRepostas()
        {
            return Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca ? "" : "display: none";
        }

        private string ObterQueryStrPesquisa(bool pageIndex)
        {
            var dic = new Dictionary<string, string>();

            foreach (Control c in ObterControles(trFiltros))
            {
                if (c is TextBox && ((TextBox)c).Attributes["QueryString"] != null)
                {
                    var control = (TextBox)c;
                    dic.Add(control.Attributes["QueryString"], control.Text);
                }
                else if (c is Controls.ctrlData && ((Controls.ctrlData)c).Attributes["QueryString"] != null)
                {
                    var control = (Controls.ctrlData)c;
                    dic.Add(control.Attributes["QueryString"], control.DataString);
                }
                else if (c is DropDownList && ((DropDownList)c).Attributes["QueryString"] != null)
                {
                    var control = (DropDownList)c;
                    dic.Add(control.Attributes["QueryString"], control.SelectedValue);
                }
                else if (c is Sync.Controls.CheckBoxListDropDown && ((Sync.Controls.CheckBoxListDropDown)c).Attributes["QueryString"] != null)
                {
                    var control = (Sync.Controls.CheckBoxListDropDown)c;
                    dic.Add(control.Attributes["QueryString"], control.SelectedValue);
                }
                else if (c is HtmlInputHidden && ((HtmlInputHidden)c).Attributes["QueryString"] != null)
                {
                    var control = (HtmlInputHidden)c;
                    dic.Add(control.Attributes["QueryString"], control.Value);
                }
                else if (c is CheckBox && ((CheckBox)c).Attributes["QueryString"] != null)
                {
                    var control = (CheckBox)c;
                    dic.Add(control.Attributes["QueryString"], control.Checked.ToString().ToLower());
                }
            }

            var queryStr = "?q=1";

            if (pageIndex == true)
                dic["pageIndex"] = grdPecas.PageIndex.ToString();

            foreach (var item in dic)
            {
                if (item.Key == "idsSubgrupos" && item.Value == "0")
                    queryStr += "&" + item.Key + "=";
                else
                    queryStr += "&" + item.Key + "=" + item.Value;                
            }

            return queryStr;
        }

        private List<Control> ObterControles(Control c)
        {
            var retorno = new List<Control>();

            if (c.Controls.Count > 0)
                foreach (Control cChild in c.Controls)
                    retorno.AddRange(ObterControles(cChild));

            retorno.Add(c);

            return retorno;
        }

        private void SetarValorControle(List<Control> controls, string id)
        {
            foreach (var c in controls)
            {
                if (c is TextBox && ((TextBox)c).Attributes["QueryString"] != null && ((TextBox)c).Attributes["QueryString"] == id)
                    ((TextBox)c).Text = Request[id];
                else if (c is Controls.ctrlData && ((Controls.ctrlData)c).Attributes["QueryString"] != null && ((Controls.ctrlData)c).Attributes["QueryString"] == id)
                    ((Controls.ctrlData)c).DataString = Request[id];
                else if (c is DropDownList && ((DropDownList)c).Attributes["QueryString"] != null && ((DropDownList)c).Attributes["QueryString"] == id)
                    ((DropDownList)c).SelectedValue = Request[id];
                else if (c is Sync.Controls.CheckBoxListDropDown && ((Sync.Controls.CheckBoxListDropDown)c).Attributes["QueryString"] != null && ((Sync.Controls.CheckBoxListDropDown)c).Attributes["QueryString"] == id)
                    ((Sync.Controls.CheckBoxListDropDown)c).SelectedValue = Request[id];
                else if (c is HtmlInputHidden && ((HtmlInputHidden)c).Attributes["QueryString"] != null && ((HtmlInputHidden)c).Attributes["QueryString"] == id)
                {
                    /* Chamado 55594. */
                    if (c.ID == "hdfBenef" && id == "idsBenef")
                        ctrlBenefSetor1.QueryStringRecuperarIds = !string.IsNullOrWhiteSpace(Request[id]) ? Request[id] : string.Empty;

                    ((HtmlInputHidden)c).Value = Request[id];
                }
                else if (c is CheckBox && ((CheckBox)c).Attributes["QueryString"] != null && ((CheckBox)c).Attributes["QueryString"] == id)
                    ((CheckBox)c).Checked = Request[id] == "true";
            }
        }

        private void PreencherControles()
        {
            var queryStr = Request.QueryString.AllKeys;
            var controls = ObterControles(trFiltros);

            foreach (var q in queryStr)
                SetarValorControle(controls, q);

            grdPecas.PageIndex = Request["pageIndex"].StrParaInt() / 10;
        }

        protected void grdPecas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            hdfPageIndex.Value = (e.NewPageIndex * 10).ToString();
            direcionaPagina();
        }

        protected void drpTipoPedido_Load(object sender, EventArgs e)
        {
            if (EstoqueConfig.ControlarEstoqueVidrosClientes)
                ((Sync.Controls.CheckBoxListDropDown)sender).Items.Add(new ListItem("Mão-de-obra Especial", "4"));
        }

        protected void odsPecas_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            // Trata e exibe o erro ocorrido ao selecionar os dados.
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            /* Chamado .
             * Verifica se o select dos campos retornou algum registro. Se não tiver retornado, esconde a Details View do rodapé da tela para cancelar o Bind dele.
             * Ao filtrar os dados novamente, caso a consulta retorne algum registro, o visible da Details View será alterado automaticamente para true.
             * A primeira comparação verifica o select dos campos e a segunda a quantidade de registros retornados,
             * é preciso utlizar as duas porque quando a tela é aberta, por exemplo, a primeira condição não é atendida. */
            else if (e.ReturnValue != null && ((e.ReturnValue is ProdutoPedidoProducao[] && ((ProdutoPedidoProducao[])e.ReturnValue).Count() == 0) ||
                (!(e.ReturnValue is ProdutoPedidoProducao[]) && e.ReturnValue.ToString().StrParaInt() == 0)))
                dtvContagemPecas.Visible = false;
        }
    }
}