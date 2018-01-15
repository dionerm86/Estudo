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
        /// Busca as peças de aluminio do orcamento informado para serem otimizadas.
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string BuscarProdutosOrcamento(string idOrcamento)
        {
            var prodsPed = Data.DAL.ProdutosOrcamentoDAO.Instance.ObterAluminiosParaOtimizacao(idOrcamento.StrParaInt());

            var retorno = new List<string>();

            foreach (var po in prodsPed)
            {
                for (int i = 0; i < po.Qtde; i++)
                {
                    var dados = new List<string>();
                    dados.Add(po.IdOrcamento.ToString());
                    dados.Add(po.IdProd.ToString());
                    dados.Add(po.CodInternoDescProd);
                    dados.Add(po.Peso.ToString());
                    dados.Add(po.Altura.ToString());
                    dados.Add(po.PecaOtimizada.ToString().ToLower());
                    dados.Add(po.IdProduto.ToString());
                    dados.Add(po.GrauCorte != null && po.GrauCorte.ToString() != "" ? ((int)po.GrauCorte).ToString() : string.Empty);
                    dados.Add(po.ProjetoEsquadria.ToString().ToLower());

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
        public string GerarOtimizacao(int[] lstProdPed, int[] lstProdOrca, int[] lstIdProd, string[] lstComprimento, int[] lstGrau, bool projEsquadria)
        {
            var result = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IOtimizacaoFluxo>()
                .GerarOtimizacaoLinear(lstProdPed, lstProdOrca, lstIdProd, lstComprimento.Select(f => f.StrParaDecimal()).ToArray(), lstGrau, projEsquadria);

            if (!result)
                throw new Exception(result.Message.Format());

            return result.Message.Format();
        }
    }
}