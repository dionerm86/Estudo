using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public static class TipoPerdaReposDAO
    {
        public static TipoPerdaRepos GetByPedidoRepos(uint idPedidoRepos)
        {
            TipoPerdaRepos retorno = new TipoPerdaRepos();

            var produtos = ProdutosPedidoDAO.Instance.GetByPedido(idPedidoRepos);
            foreach (ProdutosPedido p in produtos)
            {
                try
                {
                    ProdutoPedidoProducao ppp = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(p.NumEtiquetaRepos);
                    if (ppp.TipoPerda == null)
                        continue;

                    string nomeCampo = ((Utils.TipoPerda)ppp.TipoPerda.Value).ToString();
                    typeof(TipoPerdaRepos).GetProperty(nomeCampo).SetValue(retorno, true, null);
                }
                catch { }
            }

            return retorno;
        }
    }
}
