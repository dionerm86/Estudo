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
            uint idPedido = !String.IsNullOrEmpty(txtNumPedido.Text) ? Glass.Conversoes.StrParaUint(txtNumPedido.Text) : 0;
            var lstProdPedEsp = odsProduto.Select() as IEnumerable<ProdutosPedidoEspelho>;
    
            string script = String.Empty;

            //Chamado 45770
            //Caso o checkbox chkComposicaoLaminado esteja marcado, deve ignorar a seleção de produto pai da composição
            if (chkComposicaoLaminado.Checked)
                lstProdPedEsp = lstProdPedEsp.Where(f => !f.IsProdutoLaminadoComposicao).ToList();

            // Chama a função de buscar etiquetas desta página (popup), limpa a tela das peças antes, 
            // pois este método desativa a validação de etiquetas já adicionadas
            foreach (var ppe in lstProdPedEsp)
            {
                var ultimoRegistro = ((List<ProdutosPedidoEspelho>)lstProdPedEsp).IndexOf(ppe) == ((List<ProdutosPedidoEspelho>)lstProdPedEsp).Count - 1;

                double totM2 = ppe.PecaReposta ? (ppe.TotM / ppe.Qtde) : (ppe.TotM / ppe.Qtde) * (ppe.Qtde - ppe.QtdImpresso);
                totM2 = Math.Round(totM2, 2);

                double totM2Calc = ppe.PecaReposta ? (ppe.TotM2Calc / ppe.Qtde) : (ppe.TotM2Calc / ppe.Qtde) * (ppe.Qtde - ppe.QtdImpresso);
                totM2Calc = Math.Round(totM2Calc, 2);

                var qtde = ppe.Qtde;

                if (ppe.PecaReposta)
                    qtde = 1;
                else if (ppe.IsProdutoLaminadoComposicao)
                    qtde = (int)ppe.QtdeImpressaoProdLamComposicao;
                else if (ppe.IsProdFilhoLamComposicao)
                {
                    qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(ppe.IdProdPedParent.Value) * ppe.Qtde;

                    var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, ppe.IdProdPedParent.Value);

                    if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                        qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPedParentPai.Value);
                }

                script += "setProdEtiqueta(" + ppe.IdProdPed + "," + (ppe.IdAmbientePedido > 0 ? ppe.IdAmbientePedido.ToString() : "null") +
                    "," + ppe.IdPedido + ", '" + ppe.DescrProduto + "','" + ppe.CodProcesso + "','" + ppe.CodAplicacao + "'," + (ppe.PecaReposta ? 1 : qtde) +
                    "," + (ppe.PecaReposta ? 1 : ppe.QtdImpresso) + "," + ppe.Altura + "," + ppe.Largura + ",'" + totM2 + "', '" +
                    (!string.IsNullOrEmpty(ppe.Obs) ? ppe.Obs.Replace("\n", " ").Replace("\t", " ").Replace("\r", " ") : string.Empty) + "', '" + ppe.NumEtiqueta +
                    "', false, " + ultimoRegistro.ToString().ToLower() + ",'" + totM2Calc + "', null);";
            }

            script += "closeWindow();";
    
            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }
    
        protected string GetIdProdPed(object idPedido, object idProdPed)
        {
            return !PedidoDAO.Instance.IsMaoDeObra(Glass.Conversoes.StrParaUint(idPedido.ToString())) ? idProdPed.ToString() : "";
        }

        protected string GetIdAmbientePedido(object idPedido, object idAmbientePedido)
        {
            return PedidoDAO.Instance.IsMaoDeObra(Glass.Conversoes.StrParaUint(idPedido.ToString())) ? idAmbientePedido.ToString() : "";
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

        [Ajax.AjaxMethod]
        public string IsProdutoLaminadoComposicao(string idProdPed)
        {
            return ProdutosPedidoEspelhoDAO.Instance.IsProdutoLaminadoComposicao(idProdPed.StrParaUint()).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string PodeImprimirPedidoImportado(string idPedido)
        {
            if (Glass.Configuracoes.PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos && PedidoDAO.Instance.IsPedidoImportado(idPedido.StrParaUint()))
                return PedidoEspelhoDAO.Instance.IsPedidoConferido(idPedido.StrParaUint()).ToString();
            return "true";
        }
    }
}
