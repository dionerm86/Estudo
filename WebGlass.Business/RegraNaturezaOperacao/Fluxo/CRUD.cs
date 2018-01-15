using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.RegraNaturezaOperacao.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public uint NovaRegra(Entidade.RegraNaturezaOperacao nova)
        {
            return RegraNaturezaOperacaoDAO.Instance.Insert(nova._regraNaturezaOperacao);
        }

        #endregion

        #region Read

        public IList<Entidade.RegraNaturezaOperacao> ObtemLista(uint codigoLoja, uint codigoTipoCliente, uint codigoGrupoProd, 
            uint codigoSubgrupoProd, uint codigoCorVidro, uint codigoCorFerragem, uint codigoCorAluminio, float espessura, 
            uint codigoNaturezaOperacao, string sortExpression, int startRow, int pageSize)
        {
            var itens = RegraNaturezaOperacaoDAO.Instance.ObtemLista(codigoLoja, codigoTipoCliente, codigoGrupoProd, codigoSubgrupoProd,
                codigoCorVidro, codigoCorFerragem, codigoCorAluminio, espessura, codigoNaturezaOperacao, sortExpression, startRow, pageSize);

            return itens.Select(x => new Entidade.RegraNaturezaOperacao(x)).ToList();
        }

        public int ObtemNumeroRegistros(uint codigoLoja, uint codigoTipoCliente, uint codigoGrupoProd, uint codigoSubgrupoProd, 
            uint codigoCorVidro, uint codigoCorFerragem, uint codigoCorAluminio, float espessura, uint codigoNaturezaOperacao)
        {
            return RegraNaturezaOperacaoDAO.Instance.ObtemNumeroRegistros(codigoLoja, codigoTipoCliente, codigoGrupoProd, 
                codigoSubgrupoProd, codigoCorVidro, codigoCorFerragem, codigoCorAluminio, espessura, codigoNaturezaOperacao);
        }

        public Entidade.RegraNaturezaOperacao ObtemItem(uint codigoRegraNaturezaOperacao)
        {
            var item = RegraNaturezaOperacaoDAO.Instance.ObtemItem(codigoRegraNaturezaOperacao);
            return new Entidade.RegraNaturezaOperacao(item);
        }

        #endregion

        #region Update

        public int Atualizar(Entidade.RegraNaturezaOperacao regraNaturezaOperacao)
        {
            return RegraNaturezaOperacaoDAO.Instance.Update(regraNaturezaOperacao._regraNaturezaOperacao);
        }

        #endregion

        #region Delete

        public int Excluir(uint codigoRegraNaturezaOperacao, string motivo, bool manual)
        {
            return RegraNaturezaOperacaoDAO.Instance.Excluir(codigoRegraNaturezaOperacao, motivo, manual);
        }

        #endregion
    }
}
