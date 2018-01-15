using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.NaturezaOperacao.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        #region Ajax

        private static Ajax.IBuscarEValidar _ajax = null;

        public static Ajax.IBuscarEValidar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.BuscarEValidar();

                return _ajax;
            }
        }

        #endregion

        public IList<Entidade.NaturezaOperacao> ObtemParaRelatorio()
        {
            var itens = NaturezaOperacaoDAO.Instance.ObtemTodosOrdenados();
            return itens.Select(x => new Entidade.NaturezaOperacao(x)).ToList();
        }

        public bool Existe(uint codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.Exists(codigoNaturezaOperacao);
        }

        public string ObtemCodigoCompleto(uint codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto(codigoNaturezaOperacao);
        }

        public string ObtemCodigoControle(uint codigoNaturezaOperacao)
        {
            var codigoInterno = NaturezaOperacaoDAO.Instance.ObtemCodigoInterno(codigoNaturezaOperacao);

            if (string.IsNullOrEmpty(codigoInterno))
            {
                var idCfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(codigoNaturezaOperacao);
                var codigoCfop = CfopDAO.Instance.ObtemCodInterno(idCfop);

                return codigoCfop;
            }
            else
                return codigoInterno;
        }

        public string ObtemCstIcms(uint codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.ObtemCstIcms(null, codigoNaturezaOperacao);
        }

        public Glass.Data.Model.ProdutoCstIpi? ObtemCstIpi(uint codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.ObtemCstIpi(null, codigoNaturezaOperacao);
        }
    }
}
