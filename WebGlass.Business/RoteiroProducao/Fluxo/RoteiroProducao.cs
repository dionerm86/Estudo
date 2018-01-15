using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.RoteiroProducao.Fluxo
{
    public sealed class RoteiroProducao: BaseFluxo<RoteiroProducao>
    {
        private RoteiroProducao() { }

        #region Classificação do Roteiro

        public IList<Entidade.RoteiroProducao> ObtemLista(int idClassificacaoRoteiroProducao, string sortExpression, int startRow, int pageSize)
        {
            var itens = RoteiroProducaoDAO.Instance.ObtemLista(0, 0, 0, 0, null, idClassificacaoRoteiroProducao, false, sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.RoteiroProducao(x)).ToList();
        }

        public int ObtemListaCount(int idClassificacaoRoteiroProducao)
        {
            return RoteiroProducaoDAO.Instance.ObtemNumeroRegistros(0, 0, 0, 0, null, idClassificacaoRoteiroProducao, false);
        }

        public IList<Entidade.RoteiroProducao> ObtemListaParaSelecao(string codProcesso, string sortExpression, int startRow, int pageSize)
        {
            var itens = RoteiroProducaoDAO.Instance.ObtemLista(0, 0, 0, 0, codProcesso, 0, true, sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.RoteiroProducao(x)).ToList();
        }

        public int ObtemListaParaSelecaoCount(string codProcesso)
        {
            return RoteiroProducaoDAO.Instance.ObtemNumeroRegistros(0, 0, 0, 0, codProcesso, 0, true);
        }

        /// <summary>
        /// Associa um roteiro a uma classificação
        /// </summary>
        public void AssociaRoteiroClassificacao(int idRoteiro, int idClassificacao)
        {
            var roteiro = RoteiroProducaoDAO.Instance.GetElementByPrimaryKey((uint)idRoteiro);
            var classificacao = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>().ObtemClassificacao(idClassificacao);

            if (roteiro == null)
                throw new Exception("Roteiro não encontrado.");

            if (roteiro.IdClassificacaoRoteiroProducao.GetValueOrDefault(0) > 0)
                throw new Exception("O roteiro escolhido já esta associado a uma classificação.");

            if (classificacao == null)
                throw new Exception("Classificação não encontada.");

            RoteiroProducaoDAO.Instance.AssociaRoteiroClassificacao(idRoteiro, idClassificacao);
        }

        /// <summary>
        /// Associa um subgrupo a uma classificação
        /// </summary>
        public void AssociaSubgrupoClassificacao(int idSubgrupo, int idClassificacao)
        {
            ClassificacaoSubgrupoDAO.Instance.AssociarSubgrupo(idSubgrupo, idClassificacao);
        }

        /// <summary>
        /// Desassocia a classificação de um roteiro
        /// </summary>
        public void DesassociarRoteiroClassificacao(Entidade.RoteiroProducao roteiro)
        {
            if (roteiro == null)
                throw new Exception("Roteiro não encontrado.");

            RoteiroProducaoDAO.Instance.DesassociarRoteiroClassificacao(roteiro.Codigo);
        }

        #endregion
    }
}
