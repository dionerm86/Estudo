using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdEtiqueta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SelProdEtiqueta));

            if (!IsPostBack)
            {
                txtDataIni.Text = DateTime.Now.AddMonths(-3).ToString("dd/MM/yyyy");
                txtDataFim.Text = DateTime.Now.AddMonths(3).ToString("dd/MM/yyyy");
            }

            if (Configuracoes.EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja && !Data.Helper.UserInfo.GetUserInfo.IsAdministrador)
            {
                drpLoja.Enabled = false;
                drpLoja.SelectedValue = Data.Helper.UserInfo.GetUserInfo.IdLoja.ToString();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }

        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            var idPedido = !string.IsNullOrWhiteSpace(this.txtNumPedido.Text) ? this.txtNumPedido.Text.StrParaUint() : 0;
            var produtosPedidoEspelho = (this.odsProduto.Select() as IEnumerable<ProdutosPedidoEspelho>).ToList();
            var script = string.Empty;

            // Chamado 45770.
            // Caso o checkbox chkComposicaoLaminado esteja marcado, deve ignorar a seleção de produto pai da composição.
            if (this.chkComposicaoLaminado.Checked)
            {
                produtosPedidoEspelho = produtosPedidoEspelho.Where(f => !f.IsProdutoLaminadoComposicao).ToList();
            }

            var idsProdPedQuantidadeEtiquetasExportadas = ProdutosPedidoEspelhoDAO.Instance.ObterQuantidadeEtiquetasExportadas(
                null,
                produtosPedidoEspelho.Select(f => (int)f.IdProdPed).ToList());

            // Chama a função de buscar etiquetas desta página (popup), limpa a tela das peças antes,
            // pois este método desativa a validação de etiquetas já adicionadas.
            foreach (var ppe in produtosPedidoEspelho)
            {
                var ultimoRegistro = produtosPedidoEspelho.IndexOf(ppe) == produtosPedidoEspelho.Count - 1;

                var qtde = ppe.Qtde;

                if (ppe.PecaReposta)
                {
                    qtde = 1;
                }
                else if (ppe.IsProdutoLaminadoComposicao)
                {
                    qtde = (int)ppe.QtdeImpressaoProdLamComposicao;
                }
                else if (ppe.IsProdFilhoLamComposicao)
                {
                    qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(ppe.IdProdPedParent.Value) * ppe.Qtde;

                    var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, ppe.IdProdPedParent.Value);

                    if (idProdPedParentPai > 0)
                    {
                        qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPedParentPai.Value);
                    }
                }

                var observacao = !string.IsNullOrWhiteSpace(ppe.Obs)
                        ? $"'{ppe.Obs.Replace("\n", " ").Replace("\t", " ").Replace("\r", " ")}'"
                        : "''";
                var qtdeCalcular = qtde > 0 ? qtde : ppe.Qtde;
                var totM2 = ppe.PecaReposta ? ppe.TotM / ppe.Qtde : ppe.TotM / ppe.Qtde * (qtdeCalcular - ppe.QtdImpresso);
                var totM2Calc = ppe.PecaReposta ? ppe.TotM2Calc / ppe.Qtde : ppe.TotM2Calc / ppe.Qtde * (qtdeCalcular - ppe.QtdImpresso);
                var quantidade = ppe.PecaReposta ? 1 : qtde;
                var quantidadeImpressa = ppe.PecaReposta ? 1 : ppe.QtdImpresso;
                var quantidadeImprimir = quantidade - quantidadeImpressa;
                var quantidadeEtiquetasExportadas = idsProdPedQuantidadeEtiquetasExportadas?.Where(f => f.IdProdPed == ppe.IdProdPed)?.Sum(f => f.QuantidadeExportada) ?? 0;

                script += string.Format(
                    "setProdEtiqueta({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16});",
                    ppe.IdProdPed,
                    ppe.IdAmbientePedido > 0 ? ppe.IdAmbientePedido.ToString() : "null",
                    ppe.IdPedido,
                    $"'{ppe.DescrProduto}'",
                    $"'{ppe.CodProcesso}'",
                    $"'{ppe.CodAplicacao}'",
                    quantidade,
                    quantidadeImpressa,
                    ppe.Altura,
                    ppe.Largura,
                    $"'{totM2.ToString("0.##")}'",
                    observacao,
                    $"'{ppe.NumEtiqueta}'",
                    false,
                    ultimoRegistro.ToString().ToLower(),
                    $"'{totM2Calc.ToString("0.##")}'",
                    quantidadeEtiquetasExportadas);
            }

            script += "closeWindow();";

            this.ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }

        protected string GetIdProdPed(object idPedido, object idProdPed)
        {
            return !PedidoDAO.Instance.IsMaoDeObra(null, Glass.Conversoes.StrParaUint(idPedido.ToString())) ? idProdPed.ToString() : "";
        }

        protected string GetIdAmbientePedido(object idPedido, object idAmbientePedido)
        {
            return PedidoDAO.Instance.IsMaoDeObra(null, Glass.Conversoes.StrParaUint(idPedido.ToString())) ? idAmbientePedido.ToString() : "";
        }

        protected string ObterQtde(object objIdProdPed, object objPecaReposta, object objQtde)
        {
            var idProdPed = objIdProdPed != null ? objIdProdPed.ToString().StrParaInt() : 0;
            var pecaReposta = objPecaReposta != null ? objPecaReposta.ToString().ToLower() == "true" : false;
            var qtde = objQtde != null ? objQtde.ToString().StrParaFloat() : 0;

            if (pecaReposta)
                qtde = 1;
            else if (idProdPed > 0)
            {
                var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd((uint)idProdPed);
                var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);
                var idProdPedParent = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, (uint)idProdPed);

                if (tipoSubgrupoProd == TipoSubgrupoProd.VidroDuplo || tipoSubgrupoProd == TipoSubgrupoProd.VidroLaminado)
                    qtde = ProdutosPedidoEspelhoDAO.Instance.ObterQtdePecasParaImpressaoComposicao(idProdPed);
                else if (idProdPedParent > 0)
                {
                    qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde((uint)idProdPedParent) * qtde;
                    
                    var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, (uint)idProdPedParent);

                    if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                        qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPedParentPai.Value);
                }
            }

            return qtde.ToString().Replace(",", ".");
        }

        protected string ObterQuantidadeEtiquetasExportadas(object objIdProdPed)
        {
            var idProdPed = objIdProdPed?.ToString()?.StrParaInt() ?? 0;

            if (idProdPed == 0)
            {
                return "0";
            }

            var idProdPedQuantidadeEtiquetasExportadas = ProdutosPedidoEspelhoDAO.Instance.ObterQuantidadeEtiquetasExportadas(
                null,
                new List<int> { idProdPed });
            var quantidadeEtiquetasExportadas = idProdPedQuantidadeEtiquetasExportadas?.Sum(f => f.QuantidadeExportada) ?? 0;

            return quantidadeEtiquetasExportadas.ToString();
        }

        protected string ObterM2Impressao(object objTotM, object objIdProdPed, object objQtde)
        {
            var idProdPed = objIdProdPed != null ? objIdProdPed.ToString().StrParaInt() : 0;
            var totM = objTotM != null ? objTotM.ToString().StrParaFloat() : 0;
            var qtde = objQtde != null ? objQtde.ToString().StrParaFloat() : 0;

            var qtdeOriginal = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde((uint)idProdPed);

            var totM2 = totM / qtdeOriginal;

            return (qtde * totM2).ToString().Replace(",", ".");
        }

        [Ajax.AjaxMethod]
        public string IsProdutoLaminadoComposicao(string idProdPed)
        {
            return ProdutosPedidoEspelhoDAO.Instance.IsProdutoLaminadoComposicao(idProdPed.StrParaUint()).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string PodeImprimirPedidoImportado(string idPedido)
        {
            if (Glass.Configuracoes.PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos && PedidoDAO.Instance.IsPedidoImportado(null, idPedido.StrParaUint()))
                return PedidoEspelhoDAO.Instance.IsPedidoConferido(idPedido.StrParaUint()).ToString();
            return "true";
        }
    }
}
