using System;
using iTextSharp.text.pdf;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(NFeDAO))]
    public class NFe
    {
        #region Cabeçalho

        public string RazaoSocialEmit { get; set; }

        public string NumeroNfe { get; set; }

        public string SerieNfe { get; set; }

        public string ModeloNfe { get; set; }

        public string EnderecoEmit { get; set; }

        public string TipoNfe { get; set; }

        public string ChaveAcesso { get; set; }

        public string LinkQrCode { get; set; }

        public string UrlChave { get; set; }

        #endregion

        #region Dados da NF-e

        public string NatOperacao { get; set; }

        public string ProtAutorizacao { get; set; }

        public string InscEstEmit { get; set; }

        public string InscEstStEmit { get; set; }

        public string CnpjEmit { get; set; }

        public int TipoAmbiente { get; set; }

        public int UfNfe { get; set; }

        public int TipoEmissao { get; set; }

        private bool DestacaIcmsSt
        {
            get { return Glass.Conversoes.StrParaFloat(VlrIcmsSt) > 0; }
        }

        private bool DestacaIcmsProp
        {
            get { return !DestacaIcmsSt && Glass.Conversoes.StrParaFloat(VlrIcms) > 0; }
        }

        public string DadosAdicionaisFs
        {
            get
            {
                string dados = UfNfe.ToString() + TipoEmissao.ToString() + CnpjEmit.Replace(".", "").Replace("/", "").Replace("-", "") +
                    Glass.Conversoes.StrParaFloat(VlrTotalNota).ToString("0.00").Replace(",", "").PadLeft(14, '0') + (DestacaIcmsProp ? "1" : "2") +
                    (DestacaIcmsSt ? "1" : "2") + DataEmissao.Substring(0, 2);

                return Formatacoes.MascaraDadosAdicionaisNFe(dados + NotaFiscalDAO.Instance.CalculaDV(dados, 3));
            }
        }

        #endregion

        #region Destinatário/Remetente

        public string RazaoSocialRemet { get; set; }

        public string CpfCnpjRemet { get; set; }

        public string EnderecoRemet { get; set; }

        public string BairroRemet { get; set; }

        public string CepRemet { get; set; }

        public string MunicipioRemet { get; set; }

        public string FoneRemet { get; set; }

        public string UfRemet { get; set; }

        public string InscEstRemet { get; set; }

        public string DataEmissao { get; set; }

        public string DataEmissaoOriginal { get; set; }

        public string DataEntradaSaida { get; set; }

        public string HoraEntradaSaida { get; set; }

        public string EmailFiscal { get; set; }

        #endregion

        #region Fatura/Duplicatas

        public string Fatura { get; set; }

        #endregion

        #region Cálculo do Imposto

        public string BcIcms { get; set; }

        public string VlrIcms { get; set; }

        public string BcIcmsSt { get; set; }

        public string VlrIcmsSt { get; set; }

        public string VlrFrete { get; set; }

        public string VlrSeguro { get; set; }

        public string Desconto { get; set; }

        public string OutrasDespesas { get; set; }

        public string VlrIpi { get; set; }

        public string VlrTotalProd { get; set; }

        public string VlrTotalNota { get; set; }

        #endregion

        #region Transportador/Volumes Transportados

        public string RazaoSocialTransp { get; set; }

        public string CpfCnpjTransp { get; set; }

        public string FretePorConta { get; set; }

        public string CodAntt { get; set; }

        public string PlacaVeiculo { get; set; }

        public string UfVeiculo { get; set; }

        public string EnderecoTransp { get; set; }

        public string MunicipioTransp { get; set; }

        public string UfTransp { get; set; }

        public string InscEstTransp { get; set; }

        public string QtdTransp { get; set; }

        public string EspecieTransp { get; set; }

        public string MarcaTransp { get; set; }

        public string NumeracaoTransp { get; set; }

        public string PesoBruto { get; set; }

        public string PesoLiquido { get; set; }

        #endregion

        #region Cálculo do ISSQN

        public string InscMunicIssqn { get; set; }

        public string VlrTotalServicosIssqn { get; set; }

        public string BcIssqn { get; set; }

        public string VlrIssqn { get; set; }

        #endregion

        #region Dados Adicionais

        public string InformacoesCompl { get; set; }

        #endregion

        #region Dados de controle

        public string Crt { get; set; }

        #endregion

        #region Código de Barras

        private static byte[] GetBarCode(string texto)
        {
            if (String.IsNullOrEmpty(texto))
                return null;

            Barcode128 barCode = new Barcode128();
            barCode.CodeType = Barcode128.CODE_C;
            barCode.ChecksumText = true;
            barCode.GenerateChecksum = true;
            barCode.StartStopText = true;
            barCode.BarHeight = 60;
            barCode.Code = texto.Replace(" ", ""); //"31090807301671000131550010001000200905000585"

            System.Drawing.Image image = barCode.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Obtêm o número que será utilizado para gerar o código de barras incluindo os
        /// caracteres de início "{", de fim "~" e de verificação, sendo o último gerado
        /// pelo algoritmo abaixo
        /// Padrão Utilizado: Code128
        /// </summary>
        public byte[] BarCodeImage
        {
            get { return GetBarCode(ChaveAcesso); }
        }

        public byte[] BarCodeImageDadosAdicionais
        {
            get { return GetBarCode(DadosAdicionaisFs); }
        }

        #endregion

        #region Assinatura

        public string DigestValue { get; set; }

        #endregion
    }
}