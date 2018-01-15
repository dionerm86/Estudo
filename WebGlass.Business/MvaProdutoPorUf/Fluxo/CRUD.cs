using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.MvaProdutoPorUf.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public uint NovoMvaProdutoPorUf(Entidade.MvaProdutoPorUf mvaProdutoUf)
        {
            return MvaProdutoUfDAO.Instance.Insert(mvaProdutoUf._mvaProduto);
        }

        #endregion

        #region Read

        public IList<Entidade.MvaProdutoPorUf> ObtemLista(uint codigoProduto, string ufOrigem, string ufDestino,
            string sortExpression, int startRow, int pageSize)
        {
            var itens = MvaProdutoUfDAO.Instance.ObtemLista(codigoProduto, ufOrigem, ufDestino,
                sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.MvaProdutoPorUf(x)).ToList();
        }

        public int ObtemNumeroRegistros(uint codigoProduto, string ufOrigem, string ufDestino)
        {
            return MvaProdutoUfDAO.Instance.ObtemNumeroRegistros(codigoProduto, ufOrigem, ufDestino);
        }

        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
