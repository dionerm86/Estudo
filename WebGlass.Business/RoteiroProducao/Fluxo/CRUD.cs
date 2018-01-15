using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.RoteiroProducao.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public int NovoRoteiroProducao(Entidade.RoteiroProducao roteiro)
        {
            int idRoteiro = (int)RoteiroProducaoDAO.Instance.Insert(roteiro._roteiroProducao);

            RoteiroProducaoSetorDAO.Instance.InserirPorRoteiroProducao(idRoteiro, roteiro.CodigosSetores);

            return idRoteiro;
        }

        #endregion

        #region Read

        public IList<Entidade.RoteiroProducao> ObtemLista(int codigoRoteiroProducao, uint codigoGrupoProduto, 
            uint codigoSubgrupoProduto, uint codigoProcesso, string sortExpression, int startRow, int pageSize)
        {
            var itens = RoteiroProducaoDAO.Instance.ObtemLista(codigoRoteiroProducao, codigoGrupoProduto,
                codigoSubgrupoProduto, codigoProcesso, null, 0, false, sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.RoteiroProducao(x)).ToList();
        }

        public int ObtemNumeroRegistros(int codigoRoteiroProducao, uint codigoGrupoProduto,
            uint codigoSubgrupoProduto, uint codigoProcesso)
        {
            return RoteiroProducaoDAO.Instance.ObtemNumeroRegistros(codigoRoteiroProducao, codigoGrupoProduto,
                codigoSubgrupoProduto, codigoProcesso, null, 0, false);
        }

        public Entidade.RoteiroProducao ObtemItem(int codigoRoteiroProducao)
        {
            var item = RoteiroProducaoDAO.Instance.ObtemElemento(codigoRoteiroProducao);
            return new Entidade.RoteiroProducao(item);
        }

        #endregion

        #region Update

        public int Atualizar(Entidade.RoteiroProducao roteiro)
        {
            roteiro._roteiroProducao.Setores = roteiro.DescricaoSetores;
            int retorno = RoteiroProducaoDAO.Instance.Update(roteiro._roteiroProducao);
            
            RoteiroProducaoSetorDAO.Instance.InserirPorRoteiroProducao(roteiro.Codigo, roteiro.CodigosSetores);

            return retorno;
        }

        #endregion

        #region Delete

        public int Excluir(Entidade.RoteiroProducao roteiro)
        {
            int retorno = RoteiroProducaoDAO.Instance.Delete(roteiro._roteiroProducao);
            if (retorno > 0)
                RoteiroProducaoSetorDAO.Instance.ApagarPorRoteiroProducao(roteiro._roteiroProducao.IdRoteiroProducao);

            return retorno;
        }

        #endregion
    }
}
