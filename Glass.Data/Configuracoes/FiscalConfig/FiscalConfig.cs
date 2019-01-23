using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.EFD;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    public static partial class FiscalConfig
    {
        /// <summary>
        /// Indicador de incidência tributária.
        /// EFD PIS/Cofins.
        /// </summary>
        public static DataSourcesEFD.CodIncTribEnum? IndicadorIncidenciaTributaria
        {
            get { return Config.GetConfigItem<DataSourcesEFD.CodIncTribEnum?>(Config.ConfigEnum.IndicadorIncidenciaTributaria); }
        }
        
        /// <summary>
        /// Método de apropriação de créditos.
        /// EFD PIS/Cofins.
        /// </summary>
        public static DataSourcesEFD.IndAproCredEnum? MetodoApropriacaoCreditos
        {
            get { return Config.GetConfigItem<DataSourcesEFD.IndAproCredEnum?>(Config.ConfigEnum.MetodoApropriacaoCreditos); }
        }

        /// <summary>
        /// Tipo de contribuição apurada.
        /// EFD PIS/Cofins.
        /// </summary>
        public static DataSourcesEFD.CodTipoContEnum? TipoContribuicaoApurada
        {
            get { return Config.GetConfigItem<DataSourcesEFD.CodTipoContEnum?>(Config.ConfigEnum.TipoContribuicaoApurada); }
        }

        /// <summary>
        /// Tipo de controle do saldo de crédito - ICMS.
        /// </summary>
        public static Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms TipoControleSaldoCreditoIcms
        {
            get { return Config.GetConfigItem<Sync.Fiscal.EFD.DataSources.TipoControleSaldoCreditoIcms>(Config.ConfigEnum.TipoControleSaldoCreditoIcms); }
        }

        /// <summary>
        /// Percentual de aproveitamento de crédito de ICMS.
        /// </summary>
        public static float PercAproveitamentoCreditoIcms
        {
            get { return Config.GetConfigItem<float>(Config.ConfigEnum.PercAproveitamentoCreditoIcms); }
        }

        /// <summary>
        /// Código do ajuste de aproveitamento de crédito de ICMS.
        /// </summary>
        public static string CodigoAjusteAproveitamentoCreditoIcms
        {
            get
            {
                uint? idAjBenInc = Config.GetConfigItem<uint?>(Config.ConfigEnum.CodigoAjusteAproveitamentoCreditoIcms);
                return idAjBenInc > 0 ? AjusteBeneficioIncentivoDAO.Instance.ObtemValorCampo<string>("codigo",
                    "idAjBenInc=" + idAjBenInc) : null;
            }
        }

        /// <summary>
        /// Define que os dados do IPI serão zerados caso o CST seja 49
        /// </summary>
        public static bool ZerarDadosIpiCstIgual49RegistroC190
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ZerarDadosIpiCstIgual49RegistroC190); }
        }

        /// <summary>
        /// Define que os dados do ICMS serão zerados no registro C170 caso o CST seja 60.
        /// </summary>
        public static bool ZerarDadosRegistroC170NotaSaidaSeCst60
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ZerarDadosRegistroC170NotaSaidaSeCst60); }
        }

        /// <summary>
        /// Define que os dados do ICMS serão zerados no registro C190 caso o CST seja 60.
        /// </summary>
        public static bool ZerarDadosIcmsRegistroC190NotaEntradaSaidaSeCst60
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ZerarDadosIcmsRegistroC190NotaEntradaSaidaSeCst60); }
        }

        /// <summary>
        /// Define que ao buscar inventário de estoque, o grupo Uso e Consumo não será buscado
        /// </summary>
        public static bool IgnorarUsoEConsumoSPED
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.IgnorarUsoEConsumoSPED); }
        }

        /// <summary>
        /// Perfil de geração do arquivo do EFD Fiscal.
        /// </summary>
        public static Sync.Fiscal.EFD.Configuracao.PerfilArquivo PerfilArquivoEfdFiscal
        {
            get { return Config.GetConfigItem<Sync.Fiscal.EFD.Configuracao.PerfilArquivo>(Config.ConfigEnum.PerfilArquivoEfdFiscal); }
        }

        /// <summary>
        /// A empresa utiliza o FCI?
        /// </summary>
        public static bool UtilizaFCI
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UtilizaFCI); }
        }

        /// <summary>
        /// As notas fiscais devem ser emitidas apenas para pedidos liberados?
        /// </summary>
        public static bool BloquearEmissaoNFeApenasPedidosLiberados
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearEmissaoNFeApenasPedidosLiberados); }
        }

        /// <summary>
        /// Permite que seja gerada nota de pedido conferido
        /// </summary>
        public static bool PermitirGerarNotaPedidoConferido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirGerarNotaPedidoConferido); }
        }

        /// <summary>
        /// Alíquota do PIS para a loja.
        /// </summary>
        public static Dictionary<CrtLoja, float> AliquotaPis
        {
            get
            {
                Dictionary<CrtLoja, float> retorno = new Dictionary<CrtLoja, float>();
                
                string[] itens = Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaPis) != null ?
                    Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaPis).Split(';') : null;
 
                if (itens != null && itens.Length > 0)
                {
                    int index = 0;
                    foreach (GenericModel m in DataSources.Instance.GetCrtLoja())
                        retorno.Add((CrtLoja)m.Id.Value, itens.Length <= index ? 0 :
                            Glass.Conversoes.StrParaFloat(itens[index++]));
                }

                return retorno;
            }
        }

        /// <summary>
        /// Alíquota do Cofins para a loja.
        /// </summary>
        public static Dictionary<CrtLoja, float> AliquotaCofins
        {
            get
            {
                Dictionary<CrtLoja, float> retorno = new Dictionary<CrtLoja, float>();
                
                string[] itens = Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaCofins) != null ?
                    Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaCofins).Split(';') : null;
 
                if (itens != null && itens.Length > 0)
                {
                    int index = 0;
                    foreach (GenericModel m in DataSources.Instance.GetCrtLoja())
                        retorno.Add((CrtLoja)m.Id.Value, itens.Length <= index ? 0 :
                            Glass.Conversoes.StrParaFloat(itens[index++]));
                }

                return retorno;
            }
        }

        /// <summary>
        /// Define se será utilizado o controle de centro de custo
        /// </summary>
        public static bool UsarControleCentroCusto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleCentroCusto); }
        }

        /// <summary>
        /// Define se será somado o valor dos impostos no valor unitario do produto na movimentação de estoque fiscal
        /// </summary>
        public static bool SomarImpostosValorUnMovFiscal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SomarImpostosValorUnMovFiscal); }
        }

        /// <summary>
        /// Define se será subtraído o valor dos impostos no valor unitario do produto na movimentação de estoque fiscal
        /// </summary>
        public static bool SubtrairImpostosValorUnMovFiscal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SubtrairImpostosValorUnMovFiscal); }
        }

        /// <summary>
        /// A empresa utiliza NFC-e?
        /// </summary>
        public static bool UtilizaNFCe
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UtilizaNFCe); }
        }

        /// <summary>
        /// Gera o código de barras do DANFE de uma forma diferente para resolver um problema que ocorre nas empresas definidas nesta config
        /// </summary>
        public static bool CorrecaoGeracaoCodigoBarrasDanfe
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CorrecaoGeracaoCodigoBarrasDanfe); }
        }

        /// <summary>
        /// Percentual do ICMS relativo ao Fundo de Combate à Pobreza (FCP) na UF de destino
        /// </summary>
        public static decimal PercentualFundoPobreza
        {
            get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.PercentualFundoPobreza); }
        }

        /// <summary>
        /// A empresa utiliza NFC-e?
        /// </summary>
        public static bool ExibirCheckGerarProdutoConjunto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCheckGerarProdutoConjunto); }
        }

        /// <summary>
        /// Estados que devem ser considerados para o cálculo do Difal do artigo 7°, incisos XIII e XIV e 7° c/c Artigo 16 do RICMS/PR.
        /// </summary>
        public static string[] EstadosConsiderarRicmsPr
        {
            get
            {
                return Config.GetConfigItem<string>(Config.ConfigEnum.EstadosConsiderarRicmsPr).Split(',');
            }
        }
        public static bool UsarTLS12NFe
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarTLS12NFe); }
        }
    }
}
