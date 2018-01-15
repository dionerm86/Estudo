using System;
using Glass.Data.DAL;

namespace Glass.Data.Helper
{
    interface ICalculoIcmsSt
    {
        float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool saida);
        float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool incluirIpiNoCalculo, bool saida);

        decimal ObtemBaseCalculoIcmsSt(Model.IProdutoIcmsSt produto, bool saida);
        float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida);
        decimal ObtemValorIcmsSt(Model.IProdutoIcmsSt produto, bool saida);

        string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt);
        string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal,
            string campoValorDesconto, string campoAliquotaIcmsSt);
    }

    static class CalculoIcmsStFactory
    {
        public static ICalculoIcmsSt ObtemInstancia(GDA.GDASession sessao, int idLoja, int? idCliente, int? idFornec, int? idCfop, string produtoNfCst)
        {
            string cnaeCliente = idCliente > 0 ?
                ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "cnae", "id_Cli=" + idCliente) :
                null;

            float? percentualCargaTributaria = null;

            /* Chamado 44504.
             * Alteração temporária para a Guaporé. Quando for desnecessária, os parâmetros int? idNf e string produtoNfCst deverão ser removidos.
             * Será necessário remover também a variável percentualCargaTributaria e o parâmetro percentualCargaTributaria do contrutor da classe CalculoIcmsStMT. */
            if (Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10.HasValue &&
                idCfop > 0 && idCliente > 0 && !string.IsNullOrEmpty(produtoNfCst) && produtoNfCst == "10")
            {
                var crtCliente = ClienteDAO.Instance.ObterCrt(sessao, idCliente.Value);

                if (crtCliente == Model.CrtCliente.SimplesNacional)
                {
                    var codInterno = CfopDAO.Instance.ObtemCodInterno((uint)idCfop.Value);

                    if (codInterno == "5401" || codInterno == "5403")
                        percentualCargaTributaria = Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10;
                }
            }

            return Configuracoes.FiscalConfig.NotaFiscalConfig.CalcularIcmsStUtilizandoWebServiceMT && !string.IsNullOrEmpty(cnaeCliente) ?
                new CalculoIcmsStMT(sessao, idCliente.Value, cnaeCliente, percentualCargaTributaria) as ICalculoIcmsSt :
                new CalculoIcmsStGeral(sessao, idLoja, idCliente ?? 0, idFornec) as ICalculoIcmsSt;
        }
    }
}