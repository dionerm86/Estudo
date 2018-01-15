using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.NaturezaOperacao.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public uint NovaNaturezaOperacao(Entidade.NaturezaOperacao naturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.Insert(naturezaOperacao._naturezaOperacao);
        }

        #endregion

        #region Read

        public IList<Entidade.NaturezaOperacao> ObtemLista(uint codigoCfop, string sortExpression, int startRow, int pageSize)
        {
            var itens = NaturezaOperacaoDAO.Instance.ObtemLista(codigoCfop, sortExpression, startRow, pageSize);
            return itens.Select(x => new Entidade.NaturezaOperacao(x)).ToList();
        }

        public int ObtemNumeroItens(uint codigoCfop)
        {
            return NaturezaOperacaoDAO.Instance.ObtemNumeroItens(codigoCfop);
        }

        #endregion

        #region Update

        public int Atualizar(Entidade.NaturezaOperacao naturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.Update(naturezaOperacao._naturezaOperacao);
        }

        #endregion

        #region Delete

        public int Excluir(Entidade.NaturezaOperacao naturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.Delete(naturezaOperacao._naturezaOperacao);
        }

        #endregion
    }
}
