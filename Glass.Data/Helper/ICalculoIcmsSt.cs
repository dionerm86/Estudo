using System;
using Glass.Data.DAL;

namespace Glass.Data.Helper
{
    public interface ICalculoIcmsSt
    {
        float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool saida);

        float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool incluirIpiNoCalculo, bool saida);

        decimal ObtemBaseCalculoIcmsSt(Model.IProdutoIcmsSt produto, bool saida);

        float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida);

        decimal ObtemValorIcmsSt(Model.IProdutoIcmsSt produto, bool saida);

        string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery);

        string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery);
    }

    public static class CalculoIcmsStFactory
    {
        public static ICalculoIcmsSt ObtemInstancia(GDA.GDASession sessao, int idLoja, int? idCliente, int? idFornec, int? idCfop, string produtoNfCst, int? idNf)
        {
            var cnaeCliente = idCliente > 0 ? ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "Cnae", string.Format("Id_Cli={0}", idCliente)) : null;
            float? percentualCargaTributaria = null;
            
            if (Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacional > 0 && idCfop > 0 && idCliente > 0 && !string.IsNullOrEmpty(produtoNfCst))
            {
                var crtCliente = ClienteDAO.Instance.ObterCrt(sessao, idCliente.Value);

                if (crtCliente == Model.CrtCliente.SimplesNacional)
                {
                    percentualCargaTributaria = Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacional;
                }
            }

            return Configuracoes.FiscalConfig.NotaFiscalConfig.CalcularIcmsStUtilizandoWebServiceMT && !string.IsNullOrEmpty(cnaeCliente) ?
                new CalculoIcmsStMT(sessao, idCliente.Value, cnaeCliente, percentualCargaTributaria) as ICalculoIcmsSt :
                new CalculoIcmsStGeral(sessao, idLoja, idCliente ?? 0, idFornec) as ICalculoIcmsSt;
        }
    }
}