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

        public Entidade.RoteiroProducao ObtemItem(int codigoRoteiroProducao)
        {
            var item = RoteiroProducaoDAO.Instance.ObtemElemento(null, codigoRoteiroProducao);
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
    }
}
