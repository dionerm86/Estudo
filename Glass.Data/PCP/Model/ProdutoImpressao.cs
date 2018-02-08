using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoImpressaoDAO))]
	[PersistenceClass("produto_impressao")]
	public class ProdutoImpressao
    {
        #region Propriedades

        [PersistenceProperty("IDPRODIMPRESSAO", PersistenceParameterType.IdentityKey)]
        public uint IdProdImpressao { get; set; }

        [Log(TipoLog.Cancelamento, "Impressão")]
        [PersistenceProperty("IDIMPRESSAO")]
        public uint? IdImpressao { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IdProdPedBox")]
        public int? IdProdPedBox { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDO")]
        public uint? IdAmbientePedido { get; set; }

        [Log(TipoLog.Cancelamento, "Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [Log(TipoLog.Cancelamento, "NF-e", "NumeroNFe", typeof(NotaFiscalDAO))]
        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [Log(TipoLog.Cancelamento, "Posição produto")]
        [PersistenceProperty("POSICAOPROD")]
        public int PosicaoProd { get; set; }

        [Log(TipoLog.Cancelamento, "Item Etiqueta")]
        [PersistenceProperty("ITEMETIQUETA")]
        public int ItemEtiqueta { get; set; }

        [Log(TipoLog.Cancelamento, "Quantidade Produto")]
        [PersistenceProperty("QTDEPROD")]
        public int QtdeProd { get; set; }

        [Log(TipoLog.Cancelamento, "Plano de Corte")]
        [PersistenceProperty("PLANOCORTE")]
        public string PlanoCorte { get; set; }

        [Log(TipoLog.Cancelamento, "Posição Arquivo Otimização")]
        [PersistenceProperty("POSICAOARQOTIMIZ")]
        public int PosicaoArqOtimiz { get; set; }

        [Log(TipoLog.Cancelamento, "Número Sequência")]
        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [Log(TipoLog.Cancelamento, "Lote")]
        [PersistenceProperty("LOTE")]
        public string Lote { get; set; }

        [PersistenceProperty("CANCELADO")]
        public bool Cancelado { get; set; }

        [Log(TipoLog.Cancelamento, "Retalho Produção", "NumeroEtiqueta", typeof(RetalhoProducaoDAO), "IdRetalhoProducao", "Obter", true)]
        [PersistenceProperty("IDRETALHOPRODUCAO")]
        public uint? IdRetalhoProducao { get; set; }

        [PersistenceProperty("IDPEDIDOEXPEDICAO")]
        public uint? IdPedidoExpedicao { get; set; }

        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("FORMA")]
        public string Forma { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("Qtde", DirectionParameter.InputOptional)]
        public double Qtde { get; set; }

        [PersistenceProperty("ALTURA", DirectionParameter.InputOptional)]
        public double Altura { get; set; }

        [PersistenceProperty("LARGURA", DirectionParameter.InputOptional)]
        public long Largura { get; set; }

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [Log(TipoLog.Atualizacao, "Observação")]
        [PersistenceProperty("Obs", DirectionParameter.InputOptional)]
        public string Obs { get; set; }

        [PersistenceProperty("Cor", DirectionParameter.InputOptional)]
        public ulong Cor { get; set; }

        [PersistenceProperty("Espessura", DirectionParameter.InputOptional)]
        public float Espessura { get; set; }

        [PersistenceProperty("DescrCor", DirectionParameter.InputOptional)]
        public string DescrCor { get; set; }

        [PersistenceProperty("DATAENTREGA", DirectionParameter.InputOptional)]
        public DateTime DataEntrega { get; set; }

        [PersistenceProperty("DATAENTREGAORIGINAL", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaOriginal { get; set; }

        [PersistenceProperty("DATAFABRICA", DirectionParameter.InputOptional)]
        public DateTime? DataFabrica { get; set; }

        [PersistenceProperty("TIPODATA", DirectionParameter.InputOptional)]
        public long? TipoData { get; set; }

        [PersistenceProperty("PEDCLI", DirectionParameter.InputOptional)]
        public string PedCli { get; set; }

        [PersistenceProperty("AMBIENTE", DirectionParameter.InputOptional)]
        public string Ambiente { get; set; }

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public ulong IdProd { get; set; }

        [PersistenceProperty("IDGRUPOPROD", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public uint NumeroNFe { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("CodInternoProd", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float TotM2
        {
            get { return Glass.Global.CalculosFluxo.ArredondaM2((int)Largura, (int)Altura, (int)Qtde, (int)IdProd, false); }
        }

        public string TituloDataEntrega
        {
            get { return "Data Entrega" + (TipoData == (long)DataSources.TipoDataEtiquetaEnum.Fábrica ? " Fábrica" : ""); }
        }

        public string DataEntregaExibicao
        {
            get
            {
                if (TipoData == (long)DataSources.TipoDataEtiquetaEnum.Entrega)
                    return Conversoes.ConverteData(DataEntrega, false) + (DataEntrega != DataEntregaOriginal && DataEntregaOriginal != null ? " (" +
                        Conversoes.ConverteData(DataEntregaOriginal, false) + ")" : "");
                else
                    return Conversoes.ConverteData(DataFabrica, false);
            }
        }

        public string ObsEditar { get; set; }

        public bool CancelarVisible
        {
            get { return !Cancelado && !ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(IdProdImpressao); }
        }

        public bool AddObsVisible 
        {
            get { return !Cancelado && IdProdPed.HasValue && IdProdPed.Value > 0; }
        }

        #endregion
    }
}