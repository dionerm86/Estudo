using System.Collections.Generic;

namespace WebGlass.Business.FCI.Fluxo
{
    public class ProdutosArquivoFCIFluxo : BaseFluxo<ProdutosArquivoFCIFluxo>
    {
        /// <summary>
        /// Recupera os produtos de um arquivo FCI
        /// </summary>
        /// <param name="idArquivoFci"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Data.Model.ProdutosArquivoFCI> ObterLista(uint idArquivoFci, string sortExpression, int startRow, int pageSize)
        {
            return Glass.Data.DAL.ProdutosArquivoFCIDAO.Instance.ObterLista(idArquivoFci, sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Recupera a quantidade de produtos de um arquivo FCI
        /// </summary>
        /// <param name="idArquivoFCI"></param>
        /// <returns></returns>
        public int ObterListaCount(uint idArquivoFCI)
        {
            return Glass.Data.DAL.ProdutosArquivoFCIDAO.Instance.ObterListaCount(idArquivoFCI);
        }
    }
}
