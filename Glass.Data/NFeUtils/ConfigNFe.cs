using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.EFD;
using Glass.Configuracoes;

namespace Glass.Data.NFeUtils
{
    public static class ConfigNFe
    {
        #region Enumeradores

        public enum TipoAmbienteNfe
        {
            Producao = 1,
            Homologacao
        }

        public enum TipoCalculoIcmsSt
        {
            SemIpi,

            /// <summary>
            /// Considera IPI no cálculo do ICMS ST ao calcular valor do ICMS ST
            /// </summary>
            ComIpiNoCalculo,

            /// <summary>
            /// A empresa optou por alterar seu preço de tabela para incluir o IPI.
            /// Esta opção considera a alíquota do IPI no cálculo removendo-o ao subtrair o ICMS
            /// </summary>
            ComIpiEmbutidoNoPreco
        }

        public enum TipoCalculoIcmsStNf
        {
            NaoCalcular,
            CalculoPadrao,
            AliquotaIcmsStComIpi
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Ambiente que a NF-e será operada
        /// </summary>
        public static TipoAmbienteNfe TipoAmbiente
        {
            get 
            { 
                return Config.GetConfigItem<bool>(Config.ConfigEnum.NFeModoProducao) ?
                    TipoAmbienteNfe.Producao : TipoAmbienteNfe.Homologacao;
            }
        }

        public static string Modelo(bool nfc)
        {
            return nfc ? "65" : "55";
        }

        public static ProdutoCstIpi CstIpi(uint idProdNf)
        {
            uint idNf = ProdutosNfDAO.Instance.ObtemValorCampo<uint>("idNf", "idProdNf=" + idProdNf);
            uint idNaturezaOperacao = ProdutosNfDAO.Instance.ObtemValorCampo<uint>("idNaturezaOperacao", "idProdNf=" + idProdNf);
            string codCfop = CfopDAO.Instance.ObtemValorCampo<string>("codInterno", "idCfop=" +
                NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNaturezaOperacao));

            return CstIpi(idNf, codCfop);
        }

        public static ProdutoCstIpi CstIpi(uint idNf, string codCfop)
        {
            ProdutoCstIpi cst;
            bool saida = NotaFiscalDAO.Instance.GetTipoDocumento(idNf) == (int)NotaFiscal.TipoDoc.Saída;

            if (saida)
            {
                cst = ProdutoCstIpi.SaidaTributada;
                if (NotaFiscalDAO.Instance.IsNotaFiscalImportacao(idNf) && !",3201,3202,3211,3503,3553,".Contains("," + codCfop.Replace(".", "") + ","))
                    cst = ProdutoCstIpi.OutrasEntradas;
            }
            else
                cst = (ProdutoCstIpi)Enum.Parse(typeof(ProdutoCstIpi), FiscalConfig.NotaFiscalConfig.CstIpiPadraoNotaEntrada.ToString());

            return cst;
        }

        public static int CstPisCofins(uint idNf)
        {
            uint idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idNf);
            var tipoDoc = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);
            int crt = LojaDAO.Instance.BuscaCrtLoja(null, idLoja);

            if (tipoDoc == (int)NotaFiscal.TipoDoc.Saída || tipoDoc == (int)NotaFiscal.TipoDoc.Entrada)
                return crt == (int)CrtLoja.LucroPresumido ? (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoTributavelAliquotaBasica :
                    crt == (int)CrtLoja.LucroReal ? (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoTributavelAliquotaBasica :
                    (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoSemIncidenciaContribuicao;
            else
                return crt == (int)CrtLoja.LucroPresumido ? (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaMercadoInterno :
                    crt == (int)CrtLoja.LucroReal ? (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoDireitoCreditoReceitaTributadaMercadoInterno :
                    (int)DataSourcesEFD.CstPisCofinsEnum.OperacaoSemIncidenciaContribuicao;
        }

        /// <summary>
        /// Retorna a alíquota de PIS para a loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static float AliqPis(uint idLoja)
        {
            int crt = LojaDAO.Instance.BuscaCrtLoja(null, idLoja);

            if (crt == (int)CrtLoja.LucroPresumido)
                return FiscalConfig.AliquotaPis.Count == 0 ? 0F : FiscalConfig.AliquotaPis[CrtLoja.LucroPresumido];
            else if (crt == (int)CrtLoja.LucroReal)
                return FiscalConfig.AliquotaPis.Count == 0 ? 0F : FiscalConfig.AliquotaPis[CrtLoja.LucroReal];
            else
                return FiscalConfig.AliquotaPis.Count == 0 ? 0F : FiscalConfig.AliquotaPis[CrtLoja.SimplesNacional];
        }

        /// <summary>
        /// Retorna a alíquota de COFINS para a loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static float AliqCofins(uint idLoja)
        {
            int crt = LojaDAO.Instance.BuscaCrtLoja(null, idLoja);

            if (crt == (int)CrtLoja.LucroPresumido)
                return FiscalConfig.AliquotaCofins.Count == 0 ? 0F : FiscalConfig.AliquotaCofins[CrtLoja.LucroPresumido];
            else if (crt == (int)CrtLoja.LucroReal)
                return FiscalConfig.AliquotaCofins.Count == 0 ? 0F : FiscalConfig.AliquotaCofins[CrtLoja.LucroReal];
            else
                return FiscalConfig.AliquotaCofins.Count == 0 ? 0F : FiscalConfig.AliquotaCofins[CrtLoja.SimplesNacional];
        }

        #endregion

        #region Métodos para Config

        public static GenericModel[] GetTipoCalculoIcmsStNf()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoCalculoIcmsStNf);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(ConfigNFe.TipoCalculoIcmsStNf), d, false).ToArray();
        }

        public static string GetDescrTipoCalculoIcmsStNf(int tipoCalculoIcmsSt)
        {
            switch (tipoCalculoIcmsSt)
            {
                case (int)ConfigNFe.TipoCalculoIcmsStNf.NaoCalcular: return "Não calcular";
                case (int)ConfigNFe.TipoCalculoIcmsStNf.CalculoPadrao: return "Alíquota padrão ICMS ST";
                case (int)ConfigNFe.TipoCalculoIcmsStNf.AliquotaIcmsStComIpi: return "Alíquota ICMS ST com IPI no cálculo";
                default: return "";
            }
        }

        public static GenericModel[] GetTipoCalculoIcmsSt()
        {
            Converter<int, string> d = new Converter<int, string>(GetDescrTipoCalculoIcmsSt);
            return DataSourcesEFD.Instance.GetFromEnum(typeof(ConfigNFe.TipoCalculoIcmsSt), d, false).ToArray();
        }

        public static string GetDescrTipoCalculoIcmsSt(int tipoCalculoIcms)
        {
            switch (tipoCalculoIcms)
            {
                case (int)ConfigNFe.TipoCalculoIcmsSt.SemIpi: return "Alíquota sem IPI";
                case (int)ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo: return "Alíquota com IPI no cálculo";
                case (int)ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco: return "Alíquota com IPI no cálculo e embutido no preço";
                default: return "";
            }
        }

        /// <summary>
        /// Obtém o tipo de contingência a ser utilizado de acordo com a UF de emissão
        /// </summary>
        /// <param name="uf"></param>
        /// <returns></returns>
        public static NotaFiscal.TipoEmissao ObtemTipoContingencia(string uf)
        {
            if (String.IsNullOrEmpty(uf))
                throw new Exception("A UF do ambiente de contingência não foi definida.");

            switch (uf.ToUpper())
            {
                case "AM":
                case "BA":
                case "CE":
                case "GO":
                case "MA":
                case "MS":
                case "MT":
                case "PA":
                case "PE":
                case "PI":
                case "PR":
                    return NotaFiscal.TipoEmissao.ContingenciaSVCRS;
                default:
                    return NotaFiscal.TipoEmissao.ContingenciaSVCAN;
            }
        }

        #endregion

        #region Versão de Layouts

        public static string VersaoCancelamento 
        { 
            get { return "1.00"; } 
        }

        public static string VersaoConsulta
        {
            get { return "4.00"; }
        }

        public static string VersaoConsCad
        {
            get { return "2.00"; }
        }

        public static string VersaoInutilizacao
        {
            get { return "4.00"; }
        }

        public static string VersaoNFe
        {
            get { return "4.00"; }
        }

        public static string VersaoLoteNFe
        {
            get { return "4.00"; }
        }

        public static string VersaoPedRecibo
        {
            get { return "2.00"; }
        }

        public static string VersaoPedSituacao
        {
            get { return "2.00"; }
        }

        public static string VersaoRetAutorizacao
        {
            get { return "4.00"; }
        }

        public static string VersaoCabecMsg
        {
            get { return "3.10"; }
        }

        public static string VersaoLoteCce
        {
            get { return "1.00"; }
        }

        #endregion
    }
}