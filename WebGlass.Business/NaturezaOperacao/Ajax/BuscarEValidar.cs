using System;
using Glass.Data.DAL;

namespace WebGlass.Business.NaturezaOperacao.Ajax
{
    public interface IBuscarEValidar
    {
        string CalcSt(string codigoNaturezaOperacao);
        string ObtemDadosComplementares(string codigoNaturezaOperacao);
        string ObterCalcularIcmsIpi(string codigoNaturezaOperacao);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string CalcSt(string codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, Glass.Conversoes.StrParaUint(codigoNaturezaOperacao)).ToString().ToLower();
        }

        public string ObtemDadosComplementares(string codigoNaturezaOperacao)
        {
            uint id = Glass.Conversoes.StrParaUint(codigoNaturezaOperacao);

            var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
            var cfopFluxo = serviceLocator.GetInstance<Glass.Fiscal.Negocios.ICfopFluxo>();

            var natOp = NaturezaOperacaoDAO.Instance.ObtemElemento((int)id);

            var devolucao = cfopFluxo.VerificarCfopDevolucao(natOp.IdCfop);

            string retorno = String.Format(@"
                'CstIcms': '{0}',
                'PercReducaoBcIcms': '{1}',
                'CstIpi': '{2}',
                'CstPisCofins': '{3}',
                'CfopDevolucao': {4},
                'Csosn': '{5}'
            ", 
                
            natOp.CstIcms, 
            natOp.PercReducaoBcIcms,
            Colosoft.Translator.Translate(natOp.CstIpi.GetValueOrDefault()).Format().Replace("'", "\'"),
            Glass.Data.EFD.DataSourcesEFD.Instance.GetDescrCstPisCofins(natOp.CstPisCofins.GetValueOrDefault()).Replace("'", "\'"),
            devolucao.ToString().ToLower(),
            natOp.Csosn);

            return "{" + retorno + "}";
        }

        public string ObterCalcularIcmsIpi(string idNaturezaOperacaoStr)
        {
            try
            {
                uint idNatureza = Glass.Conversoes.StrParaUint(idNaturezaOperacaoStr);

                var calcIcms = NaturezaOperacaoDAO.Instance.CalculaIcms(null, idNatureza);
                var calcIcmsSt = NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, idNatureza);
                var calcIpi = NaturezaOperacaoDAO.Instance.CalculaIpi(null, idNatureza);

                return "OK;" + calcIcms.ToString().ToLower() + ";" + calcIcmsSt.ToString().ToLower() + ";" + calcIpi.ToString().ToLower();
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    }
}
