using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using Glass.Data.Model.Cte;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(CTeDAO))]
    public class CTe
    {
        #region Identificação do Emitente

        public string RazaoSocialEmit { get; set; }

        public string EnderecoEmit { get; set; }

        public string CnpjCpfEmitente { get; set; }

        public string InscEstEmitente { get; set; }

        #endregion

        public int TipoAmbiente { get; set; }

        #region Modal

        public string Modal { get; set; }

        #endregion

        #region Modelo

        public string Modelo { get; set; }

        #endregion

        #region Série

        public string SerieCte { get; set; }

        #endregion

        #region Número

        public string NumeroCte { get; set; }

        #endregion

        #region Data e hora de emissão

        public string DHEmissao { get; set; }

        #endregion

        #region Inscrição Suframa

        public string InscSuframa { get; set; }

        #endregion
        
        #region Chave de acesso

        public string ChaveAcesso { get; set; }

        #endregion

        #region Protocolo de autorização de uso

        public string ProtocoloAutorizacao { get; set; }

        #endregion

        #region Tipo de Cte

        public string TipoCte { get; set; }

        #endregion

        #region Tipo Serviço

        public string TipoServico { get; set; }

        #endregion

        #region Dados complementares do CTe para fins operacionais ou comerciais

        public string InformacoesAdicionais { get; set; }

        #endregion

        #region Forma de pagamento

        public string FormaPagamento { get; set; }

        #endregion

        #region CFOP - Natureza da prestação

        public string NatOperacao { get; set; }

        public string ObservaocaoNatOperacao { get; set; }

        #endregion

        #region Origem da prestação

        public string OrigemPrestacao { get; set; }

        #endregion

        #region Destino da prestação

        public string DestinoPrestacao { get; set; }

        #endregion

        #region Forma Emissão

        public string FormaEmissao { get; set; }

        #endregion

        #region Dados do Remetente
        
        public string Remetente { get; set; }

        public string EnderecoRem { get; set; }

        public string MunicipioRem { get; set; }

        public string CepRem { get; set; }

        public string CnpjCpfRem { get; set; }

        public string InscEstRem { get; set; }

        public string UFRem { get; set; }

        public string PaisRem { get; set; }

        public string FoneRem { get; set; }        

        #endregion

        #region Destinatário

        public string Destinatario { get; set; }

        public string EnderecoDest { get; set; }

        public string MunicipioDest { get; set; }

        public string CepDest { get; set; }

        public string CnpjCpfDest { get; set; }

        public string InscEstDest { get; set; }

        public string UFDest { get; set; }

        public string PaisDest { get; set; }

        public string FoneDest { get; set; }

        #endregion

        #region Expedidor

        public string Expedidor { get; set; }

        public string EnderecoExped { get; set; }

        public string MunicipioExpd { get; set; }

        public string CepExped { get; set; }

        public string CnpjCpfExped { get; set; }

        public string InscEstExped { get; set; }

        public string UFExped { get; set; }

        public string PaisExped { get; set; }

        public string FoneExped { get; set; }

        #endregion

        #region Recebedor

        public string Recebedor { get; set; }

        public string EnderecoReceb { get; set; }

        public string MunicipioReceb { get; set; }

        public string CepReceb { get; set; }

        public string CnpjCpfReceb { get; set; }

        public string InscEstReceb { get; set; }

        public string UFReceb { get; set; }

        public string PaisReceb { get; set; }

        public string FoneReceb { get; set; }

        #endregion

        #region Tomador

        public string TipoTomador { get; set; }

        public string Tomador { get; set; }

        public string EnderecoToma { get; set; }

        public string MunicipioToma { get; set; }

        public string CepToma { get; set; }

        public string CnpjCpfToma { get; set; }

        public string InscEstToma { get; set; }

        public string UFToma { get; set; }

        public string PaisToma { get; set; }

        public string FoneToma { get; set; }

        #endregion               

        #region Produto predominante

        public string ProdutoPredominante { get; set; }

        #endregion

        #region Outras caracteristicas da carga

        public string OutCarctCarga { get; set; }

        public string ValorTotalMercadoria { get; set; }

        #endregion

        #region Peso bruto

        public string PesoBruto { get; set; }

        #endregion

        #region Peso Base Calculo

        public string PesoBC { get; set; }

        #endregion

        #region Peso aferido

        public string PesoAferido { get; set; }

        #endregion

        #region Cubagem

        public string Cubagem { get; set; }

        #endregion

        #region Quantidade volumes

        public string QtdVolumes { get; set; }

        #endregion

        public List<InfoCargaCte> ListaInfoCargaCte { get; set; }

        #region Seguro

        public List<Glass.Data.Model.Cte.SeguroCte> ListaSeguros { get; set; }

        public string NomeSeguradora { get; set; }

        public string ResponsavelSeguro { get; set; }

        public string NumApolice { get; set; }

        public string NumAverbacao { get; set; }

        #endregion

        #region Componentes de valor da prestação do serviço

        public List<ComponenteValorCte> ListaComponentes { get; set; }
        public string NomeComponente { get; set; }

        public string ValorComponente { get; set; }

        public decimal ValorTotalServicoComponente { get; set; }

        public decimal ValorReceberComponente { get; set; }

        #endregion

        #region Informações relativas ao imposto

        public string SubstituicaoTributaria { get; set; }

        public string BaseCalculo { get; set; }

        public string AliquotaIcms { get; set; }

        public string ValorIcms { get; set; }

        public string ReducaoBaseCalculo { get; set; }

        public string IcmsST { get; set; }

        private bool DestacaIcmsSt
        {
            get { return Glass.Conversoes.StrParaFloat(IcmsST) > 0; }
        }

        private bool DestacaIcmsProp
        {
            get { return !DestacaIcmsSt && Glass.Conversoes.StrParaFloat(ValorIcms) > 0; }
        }

        #endregion

        #region Documentos Originários

        public List<NotaFiscalCte> ListaDocumentosOriginarios { get; set; }                

        public string Observacoes { get; set; }

        #endregion

        #region Informações específicas do modal rodoviário

        public string RNTRCRodo { get; set; }

        public string CIOT { get; set; }

        public string Lotacao { get; set; }

        public string DataPrevistaEntrega { get; set; }

        #endregion

        #region Informações específicas do modal rodoviário lotação

        public string TipoVeiculo { get; set; }

        public string Placa { get; set; }

        public string UFVeiculo { get; set; }

        public string RNTRCProprietario { get; set; }

        public string RespValePedCnpj { get; set; }

        public string FornValePedagioCnpj { get; set; }

        public string NumeroComprovante { get; set; }

        public string NomeMotorista { get; set; }

        public string CpfMotorista { get; set; }

        public List<LacreCteRod> ListaNumeroLacre { get; set; }

        #endregion

        public string DadosAdicionaisFs
        {
            get
            {
                string dados = UFRem.ToString() + FormaEmissao.ToString() + CnpjCpfEmitente.Replace(".", "").Replace("/", "").Replace("-", "") +
                    Glass.Conversoes.StrParaFloat(ValorTotalServicoComponente.ToString()).ToString("0.00").Replace(",", "").PadLeft(14, '0') + (DestacaIcmsProp ? "1" : "2") +
                    (DestacaIcmsSt ? "1" : "2") + DHEmissao.Substring(0, 2);

                return Formatacoes.MascaraDadosAdicionaisCTe(dados + Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.CalculaDV(dados, 3));
            }
        }

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
    }
}
