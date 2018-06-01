using System;
using System.Web;
using Glass.Data.Helper;
using GDA;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(EtiquetaDAO))]
    public class Etiqueta
    {
        #region Propriedades

        public string CodCliente { get; set; }

        private string _nomeCliente;

        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        public string NomeFantasiaCliente { get; set; }

        public string RazaoSocialCliente { get; set; }

        public string IdPedido { get; set; }

        public string IdNf { get; set; }

        public uint IdCliente { get; set; }

        public uint IdLoja { get; set; }

        public uint IdImpressao { get; set; }

        public uint? IdSubgrupoProd { get; set; }

        public int? TipoPedido { get; set; }

        public int? TipoVendaPedido { get; set; }

        public string NumItem { get; set; }

        public uint IdProdPedEsp { get; set; }

        public int IdProdPedBox { get; set; }

        public uint? IdArquivoMesaCorte { get; set; }

        public DateTime? DataEntrega { get; set; }

        public DateTime? DataFabrica { get; set; }

        public DateTime DataImpressao { get; set; }

        public DateTime? DataEntradaSaidaNFe { get; set; }

        public DateTime? DataEmissaoNFe { get; set; }

        public string DescrProd { get; set; }

        public string DescrProdParent { get; set; }

        public string DescrBenef { get; set; }

        public string DescricaoSubgrupo { get; set; }

        public string CodInterno { get; set; }

        public string CodInternoParent { get; set; }

        public string CodApl { get; set; }

        public string CodProc { get; set; }

        public int TipoProcesso { get; set; }

        public string CodOtimizacao { get; set; }

        public string Largura { get; set; }

        public string Altura { get; set; }

        public string AlturaLargura { get; set; }

        public string Ambiente { get; set; }

        public string CodigoProjeto { get; set; }

        public string Obs { get; set; }

        public string ObsItemProjeto { get; set; }

        public string BarCodeData { get; set; }

        public string ImpressoPor { get; set; }

        public string Flags { get; set; }

        /// <summary>
        /// Dados do Cód. de Barras
        /// </summary>
        public string BarCode
        {
            get
            {
                var barCodeData = BarCodeData.Replace(" ", "");

                barCodeData = barCodeData.Replace("/", ";");

                if (PCPConfig.ConcatenarEspAltLargAoNumEtiqueta)
                    barCodeData = barCodeData.Replace(".", "a").Replace(";", "b").Replace("-", "d").Replace("_", "c");

                return barCodeData;
            }
        }

        /// <summary>
        /// Gera o código de barras
        /// Padrão Utilizado: Code128
        /// </summary>
        public byte[] BarCodeImage
        {
            get 
            {
                Utils.GirarImagem girar = 
                    (EtiquetaConfig.Girar90GrausCodigoDeBarras && (IdRetalhoProducao != null || IdPedido != null || IdNf != null)) ? Utils.GirarImagem.Girar90Graus :
                    Utils.GirarImagem.Normal;

                return Utils.GetBarCode(BarCode, girar); 
            }
        }

        public DateTime? DataPedido { get; set; }

        public uint? IdCorVidro { get; set; }

        public float Espessura { get; set; }

        public string PedidoRepos { get; set; }

        public string PlanoCorte { get; set; }

        public string DescrMaterial { get; set; }

        public int QtdEtiquetaPlanoCorte { get; set; }

        public DateTime? DataProducao { get; set; }

        /// <summary>
        /// Criado em 11/01/11
        /// </summary>
        public string NumEtiqueta { get; set; }

        // Propriedade criada para exibir o número da etiqueta sem o ID do pedido (Temper Mat).
        public string NumEtiquetaSemIdPed { get; set; }

        /// <summary>
        /// Utilizado para ordenar as etiquetas geradas pelo optyway na impressão da etiqueta
        /// </summary>
        public int PosicaoArqOtimiz { get; set; }

        public int NumSeq { get; set; }

        /// <summary>
        /// Campo cadastrado no cadastro de produto e utilizado no opty-way
        /// </summary>
        public string Forma { get; set; }

        /// <summary>
        /// Nome do arquivo de corte.
        /// </summary>
        public string NomeArquivoCorte { get; set; }

        public string ComplementoCliente { get; set; }

        public string NomeCidade { get; set; }

        public string DescrRota { get; set; }

        public string CodRota { get; set; }

        public bool DestacarEtiqueta { get; set; }

        public bool PecaReposta { get; set; }

        public int QtdPecasPedido { get; set; }

        public bool DestacarVidroTemperadoEtiqueta { get; set; }

        public string TelefoneLoja { get; set; }

        public string TelefoneCliente { get; set; }

        public string DescrTipoEntrega { get; set; }

        public string NumeroNFe { get; set; }

        public string Lote { get; set; }

        public bool FastDelivery { get; set; }

        public string RotaExterna { get; set; }

        public string ClienteExterno { get; set; }

        public string PedCliExterno { get; set; }

        public bool MaoDeObra { get; set; }

        public string NomeFuncCadPedido { get; set; }

        public DateTime? DataCadPedido { get; set; }

        public string SetoresRoteiro { get; set; }

        public bool PossuiFml { get; set; }

        public bool PossuiDxf { get; set; }

        public bool PossuiSGlass { get; set; }

        public bool PossuiIntermac { get; set; }

        /// <summary>
        /// Campo usado pela Modelo, para identifcar peças com aresta 0 ou benef "CNC" das outras
        /// </summary>
        public bool DestacarAlturaLargura { get; set; }

        public float Peso { get; set; }

        public string LogomarcaLojaCliente
        {
            get
            {
                var arqLogotipo = "logo" + System.Configuration.ConfigurationManager.AppSettings["sistema"];
                var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(IdCliente);

                arqLogotipo += idLojaCliente > 0 ? idLojaCliente.ToString() : "";

                if (Logotipo.EsconderLogotipoRelatorios)
                    arqLogotipo = "logoEmBranco";

                return "file:///" + HttpContext.Current.Request.PhysicalApplicationPath.Replace('\\', '/') + "Images/" +
                    arqLogotipo + ".png";
            }
        }

        /// <summary>
        /// Campo criado para a etiqueta da Personal Glass, é o número da etiqueta sem as pontuações e com 3 casas decimais
        /// após cada pontuação.
        /// </summary>
        public string Cad { get; set; }
 
        public string DescrGrupoProj { get; set; }

        /// <summary>
        /// Criado para a etiqueta da EM Vidros, e o número de sequencial de todas as etiquetas
        /// </summary>
        public int Sequencial
        {
            get
            {
                if (EtiquetaConfig.RelatorioEtiqueta.UsarNumeroSequencial && NumEtiqueta != null)
                    return ProdutoPedidoProducaoDAO.Instance.ObtemNumSequencia(NumEtiqueta);
                else
                    return 0;
            }
        }

        public string PosEtiquetaParent { get; set; }

        public string Contato2 { get; set; }

        public int ItemEtiqueta { get; set; }

        public int QtdeProd { get; set; }

        #endregion

        #region Propriedades Estendidas

        public string DescrAlturaLargura { get; set; }

        public string DescrCorVidro
        {
            get { return IdCorVidro > 0 ? CorVidroDAO.Instance.GetNome((uint)IdCorVidro) : ""; }
        }

        public string NomeLoja { get { return IdLoja > 0 ? LojaDAO.Instance.GetNome(IdLoja) : string.Empty; } }

        public uint? IdRetalhoProducao { get; set; }

        //Exibe o numero de etiqueta do cliente
        // Altera o idpedido no numero etiqueta atual para o idpedido do pedido importado.
        public string NumEtiquetaCliente
        {
            get
            {
                if (!PedidoDAO.Instance.IsPedidoImportado(IdPedido.StrParaUint()))
                    return "";

                var produtoPedidoProducao = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(NumEtiqueta);

                return produtoPedidoProducao.NumEtiquetaCliente;
            }
        }

        /// <summary>
        /// Dados do Cód. de Barras da etiqueta do cliente
        /// </summary>
        public string BarCodeCliente
        {
            get
            {
                var barCodeCliente = NumEtiquetaCliente.Replace(" ", "");

                barCodeCliente = barCodeCliente.Replace("/", ";");

                return barCodeCliente;
            }
        }

        /// <summary>
        /// Gera o código de barras da etiqueta do cliente
        /// Padrão Utilizado: Code128
        /// </summary>
        public byte[] BarCodeImageCliente
        {
            get
            {
                var barCodeCliente = BarCodeCliente;
                if (string.IsNullOrEmpty(barCodeCliente))
                    return null;

                return Utils.GetBarCode(barCodeCliente);
            }
        }

        /// <summary>
        /// Dados do Cód. de Barras.
        /// </summary>
        public string BarCodeDataIntermac
        {
            get
            {
                var barCodeDataIntermac = string.Empty;

                if (!string.IsNullOrEmpty(NumEtiqueta) && IdProdPedEsp > 0 && PossuiIntermac)
                {
                    barCodeDataIntermac = string.Format("{0}.cni{1}00010000", BarCodeData.Replace(" ", string.Empty).Replace("/", ";"), Espessura.ToString().PadLeft(4, '0'));
                }

                return barCodeDataIntermac;
            }
        }

        /// <summary>
        /// Gera o código de barras.
        /// Padrão Utilizado: Code128.
        /// </summary>
        public byte[] BarCodeImageIntermac
        {
            get
            {
                var girar = (EtiquetaConfig.Girar90GrausCodigoDeBarras && (IdRetalhoProducao != null || IdPedido != null || IdNf != null)) ? Utils.GirarImagem.Girar90Graus : Utils.GirarImagem.Normal;
                return Utils.GetBarCode(BarCodeDataIntermac, girar);
            }
        }

        #endregion
    }
}