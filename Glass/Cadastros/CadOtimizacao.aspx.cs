using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadOtimizacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadOtimizacao));
        }

        /// <summary>
        /// Busca as peças de aluminio do pedido informado para serem otimizadas.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string BuscarProdutos(string idPedido)
        {
            var prodsPed = Glass.Data.DAL.ProdutosPedidoDAO.Instance.ObterAluminiosParaOtimizacao(idPedido.StrParaInt());

            var retorno = new List<string>();

            foreach (var pp in prodsPed)
            {
                for (int i = 0; i < pp.Qtde; i++)
                {
                    var dados = new List<string>();
                    dados.Add(pp.IdPedido.ToString());
                    dados.Add(pp.IdProdPed.ToString());
                    dados.Add(pp.CodInternoDescProd);
                    dados.Add(pp.Peso.ToString());
                    dados.Add(pp.AlturaReal.ToString());
                    dados.Add(pp.PecaOtimizada.ToString().ToLower());
                    dados.Add(pp.IdProd.ToString());
                    dados.Add(pp.GrauCorte != null && pp.GrauCorte.ToString() != "" ? ((int)pp.GrauCorte).ToString() : string.Empty);
                    dados.Add(pp.ProjetoEsquadria.ToString().ToLower());

                    retorno.Add(string.Join(";", dados));
                }
            }

            return string.Join("|", retorno);
        }

        /// <summary>
        /// Gera a otimização das peças informadas
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GerarOtimizacao(int[] lstProdPed, int[] lstIdProd, string[] lstComprimento, int[] lstGrau, bool projEsquadria)
        {
            var result = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IOtimizacaoFluxo>()
                .GerarOtimizacaoLinear(lstProdPed, lstIdProd, lstComprimento.Select(f => f.StrParaDecimal()).ToArray(), lstGrau, projEsquadria);

            if (!result)
                throw new Exception(result.Message.Format());

            return result.Message.Format();
        }
    }
}