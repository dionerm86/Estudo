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

        string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery);

        string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery);
    }

    static class CalculoIcmsStFactory
    {
        public static ICalculoIcmsSt ObtemInstancia(GDA.GDASession sessao, int idLoja, int? idCliente, int? idFornec, int? idCfop,
            string produtoNfCst, int? idNf)
        {
            var cnaeCliente = idCliente > 0 ? ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "cnae", "id_Cli=" + idCliente) : null;
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
                    var codInterno = CfopDAO.Instance.ObtemCodInterno(sessao, (uint)idCfop.Value);

                    if (codInterno == "5401" || codInterno == "5403")
                        percentualCargaTributaria = Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10;

                    #region Recupera o percentual de carga tributária com base nas notas referenciadas

                    /* Chamado 50444. */
                    else if (idNf > 0 && CfopDAO.Instance.IsCfopDevolucao(codInterno))
                    {
                        // Recupera as notas fiscais referenciadas na nota fiscal que está sendo alterada.
                        var idsNotaFiscalReferenciada = NotaFiscalDAO.Instance.ObtemIdsNfRef(sessao, (uint)idNf);

                        // O percentual de carga tributária configurado deve ser recuperado somente se a nota que está
                        // sendo alterada esteja associada a alguma nota fiscal.
                        if (!string.IsNullOrEmpty(idsNotaFiscalReferenciada))
                        {
                            // Variável criada para verificar se o percentual de carga tributária configurado deverá ser recuperado.
                            var considerarPercentualCargaTributariaNotaOriginal = false;

                            // Todas as notas referenciadas devem se enquadrar nos mesmos critérios da nota fiscal de devolução para
                            // que o percentual de carga tributária configurado seja recuperado.
                            foreach(var idNotaFiscalReferenciada in idsNotaFiscalReferenciada.Split(','))
                            {
                                if (idNotaFiscalReferenciada.StrParaUintNullable().GetValueOrDefault() == 0)
                                    continue;

                                #region Valida dados do cliente

                                // Recupera o ID do cliente da nota fiscal referenciada.
                                var idClienteNotaFiscalReferenciada = NotaFiscalDAO.Instance.ObtemIdCliente(sessao, idNotaFiscalReferenciada.StrParaUint());

                                // Caso a nota não possua cliente, considera que o percentual de carga tributária deverá
                                // ser buscado na tabela, ao invés de ser buscado pela configuração.
                                if (idClienteNotaFiscalReferenciada.GetValueOrDefault() == 0)
                                {
                                    considerarPercentualCargaTributariaNotaOriginal = false;
                                    break;
                                }

                                // Recupera o CRT do cliente da nota fiscal referenciada.
                                var crtClienteNotaFiscalReferenciada = ClienteDAO.Instance.ObterCrt(sessao, (int)idClienteNotaFiscalReferenciada);

                                // Caso o cliente não esteja cadastrado com o CRT Simples Nacional, considera que o percentual de carga
                                // tributária deverá ser buscado na tabela, ao invés de ser buscado pela configuração.
                                if (crtClienteNotaFiscalReferenciada != Model.CrtCliente.SimplesNacional)
                                {
                                    considerarPercentualCargaTributariaNotaOriginal = false;
                                    break;
                                }

                                #endregion

                                #region Valida CFOP

                                // Recupera o ID do CFOP da nota fiscal referenciada.
                                var idCfopNotaFiscalReferenciada = NotaFiscalDAO.Instance.GetIdCfop(sessao, idNotaFiscalReferenciada.StrParaUint());
                                // Recupera o código interno do CFOP da nota fiscal referenciada.
                                var codInternoCfopNotaFiscalReferenciada = CfopDAO.Instance.ObtemCodInterno(sessao, idCfopNotaFiscalReferenciada);

                                // Caso o CFOP não seja de de, considera que o percentual de carga
                                // tributária deverá ser buscado na tabela, ao invés de ser buscado pela configuração.
                                if (codInternoCfopNotaFiscalReferenciada != "5401" && codInternoCfopNotaFiscalReferenciada != "5403")
                                {
                                    considerarPercentualCargaTributariaNotaOriginal = false;
                                    break;
                                }

                                #endregion

                                considerarPercentualCargaTributariaNotaOriginal = true;
                            }

                            // Recupera o percentual de carga tributária configurado caso as condições das
                            // notas fiscais referenciadas tenham sido atendidas.
                            if (considerarPercentualCargaTributariaNotaOriginal)
                                percentualCargaTributaria = Configuracoes.FiscalConfig.NotaFiscalConfig.PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10;
                        }
                    }

                    #endregion
                }
            }

            return Configuracoes.FiscalConfig.NotaFiscalConfig.CalcularIcmsStUtilizandoWebServiceMT && !string.IsNullOrEmpty(cnaeCliente) ?
                new CalculoIcmsStMT(sessao, idCliente.Value, cnaeCliente, percentualCargaTributaria) as ICalculoIcmsSt :
                new CalculoIcmsStGeral(sessao, idLoja, idCliente ?? 0, idFornec) as ICalculoIcmsSt;
        }
    }
}