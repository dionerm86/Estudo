using System;
using Glass.Data.Helper;
using System.Text;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaBoxImprimir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstEtiquetaBoxImprimir));
    
            txtNumero.Focus();
        }
    
        /// <summary>
        /// Retorna os produtos do grupo vidro ou de pedido mão de obra do pedido passado para serem impressos
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="noCache"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetProdByPedido(string idPedidoStr, string idProcessoStr, string idAplicacaoStr,
            string idCorVidroStr, string espessuraStr, string idSubgrupoProdStr, string noCache)
        {
            try
            {
                var idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
    
                // Verifica se o pedido é do tipo Revenda.
                if (!PedidoDAO.Instance.IsRevenda(idPedido))
                    return "Erro\tÉ possível imprimir somente etiquetas de box de pedidos de Revenda.";
    
                // Verifica se o pedido está confirmado.
                if (!PedidoDAO.Instance.IsPedidoConfirmadoLiberado(idPedido))
                    return "Erro\tO pedido " + idPedido + " ainda não foi confirmado.";
    
                var str = new StringBuilder();
    
                var idProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
                var idAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);
                var idCorVidro = Glass.Conversoes.StrParaUint(idCorVidroStr);
                var espessura = Glass.Conversoes.StrParaFloat(espessuraStr);
                var idSubgrupoProd = Glass.Conversoes.StrParaUint(idSubgrupoProdStr);
    
                var lstProd = ProdutosPedidoDAO.Instance.GetProdEtiqBox(idPedido, idProcesso, idAplicacao,
                    idCorVidro, espessura, idSubgrupoProd);
    
                // Verifica se há produtos a serem adicionados.
                if (lstProd.Count == 0)
                {
                    var msg = "Erro\tNão há nenhum produto que atende aos filtros selecionados neste Pedido que não tenha sido impresso.";
                    return msg;
                }
    
                foreach (var p in lstProd)
                {
                    var totM2 = p.TotM;
    
                    str.Append(p.IdProdPed + ";");
                    str.Append(p.IdPedido + ";");
                    str.Append(p.DescricaoProdutoComBenef.Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.CodProcesso != null ? p.CodProcesso.Replace("|", "").Replace(";", "") + ";" : ";");
                    str.Append(p.CodAplicacao != null ? p.CodAplicacao.Replace("|", "").Replace(";", "") + ";" : ";");
                    str.Append(p.Qtde + ";");
                    str.Append(p.QtdeBoxImpresso + ";");
                    str.Append((p.AlturaReal > 0 ? p.AlturaReal : p.Altura) + ";");
                    str.Append((p.LarguraReal > 0 ? p.LarguraReal : p.Largura) + ";");
                    str.Append((totM2).ToString("0.##") + ";");
                    str.Append("|");
                }
    
                return "ok\t" + str.ToString().Replace('\t', ' ').TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string PermissaoParaImprimir()
        {
             if (!Configuracoes.Geral.ControlePCP || !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas))
                return "false";
            else
                return "true";
        }
    }
}
