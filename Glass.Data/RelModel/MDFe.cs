using GDA;
using Glass.Data.RelDAL;
using iTextSharp.text.pdf;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MDFeDAO))]
    public class MDFe
    {
        #region IDENTIFICAÇÃO MDF-e

        public string ChaveAcesso { get; set; }

        public string Modelo { get; set; }

        public string Serie { get; set; }

        public string TipoEmissao { get; set; }

        public string NumeroManifestoEletronico { get; set; }

        public int TipoAmbiente { get; set; }

        public string DataEmissao { get; set; }

        public string UFInicio { get; set; }

        public string UFFim { get; set; }

        #region Protocolo de Autorização

        public string ProtocoloAutorizacao { get; set; }

        #endregion

        #region Emitente

        public string RazaoSocialEmitente { get; set; }

        public string EnderecoEmitente { get; set; }

        public string CNPJEmitente { get; set; }

        public string InscEstEmitente { get; set; }

        #endregion

        #endregion

        #region INFORMAÇÕES DO MODAL RODOVIARIO

        // CIOT
        public string CIOTs { get; set; }

        public string ResposaveisCIOTs { get; set; }

        // PEDÁGIO
        public string CNPJsResponsaveisPedagio { get; set; }

        public string CNPJsFornecedoresPedagio { get; set; }

        public string NumerosCompraPedagio { get; set; }

        // VEÍCULOS
        public string PlacasVeiculos { get; set; }

        public string RNTRCsVeiculos { get; set; }

        // CONDUTOR
        public string CPFsCondutores { get; set; }

        public string NomesCondutores { get; set; }

        #endregion

        #region INFORMAÇÕES DOS DOCUMENTOS FISCAIS VINCULADOS

        public string DocumentosFiscaisVinculados { get; set; }

        #endregion

        #region SEGURO CARGA

        #endregion

        #region TOTALIZADORES DA CARGA

        public string QuantidadeCTe { get; set; }

        public string QuantidadeNFe { get; set; }

        public string ValorCarga { get; set; }

        public string CodigoUnidade { get; set; }

        public string PesoTotalCarga { get; set; }

        #endregion

        #region INFORMAÇÕES ADICIONAIS

        public string InformacoesAdicionaisFisco { get; set; }

        public string InformacoesComplementares { get; set; }

        #endregion

        #region Código de Barras

        private static byte[] GetBarCode(string texto)
        {
            if (string.IsNullOrEmpty(texto))
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


        #endregion
    }
}
