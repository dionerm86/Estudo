using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.EFD;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoLojaDAO))]
	[PersistenceClass("produto_loja")]
	public class ProdutoLoja : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.IProdutoLoja
    {
        #region Propriedades

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        public int IdLoja { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int IdProd { get; set; }

        [Log("Qtde. Estoque")]
        [PersistenceProperty("QTDESTOQUE")]
        public double QtdEstoque { get; set; }

        [Log("Estoque Mínimo")]
        [PersistenceProperty("ESTMINIMO")]
        public double EstMinimo { get; set; }

        [PersistenceProperty("Reserva")]
        public double Reserva { get; set; }

        [PersistenceProperty("Liberacao")]
        public double Liberacao { get; set; }

        [Log("M²")]
        [PersistenceProperty("M2")]
        public double M2 { get; set; }

        [Log("Estoque Fiscal")]
        [PersistenceProperty("ESTOQUEFISCAL")]
        public double EstoqueFiscal { get; set; }

        [PersistenceProperty("DEFEITO")]
        public double Defeito { get; set; }

        [Log("Qtde. Posse Terceiros")]
        [PersistenceProperty("QTDEPOSSETERCEIROS")]
        public double QtdePosseTerceiros { get; set; }

        [PersistenceProperty("ID_CLI")]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int? IdCliente { get; set; }

        [PersistenceProperty("IDFORNEC")]
        [PersistenceForeignKey(typeof(Fornecedor), "IdFornec")]
        public int? IdFornec { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        [PersistenceForeignKey(typeof(Transportador), "IdTransportador")]
        public int? IdTransportador { get; set; }

        [PersistenceProperty("IDLOJATERC")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja", "Terceiros")]
        public int? IdLojaTerc { get; set; }

        [PersistenceProperty("IDADMINCARTAO")]
        [PersistenceForeignKey(typeof(AdministradoraCartao), "IdAdminCartao")]
        public int? IdAdminCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("CodInternoProd", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("DescrGrupoProd", DirectionParameter.InputOptional)]
        public string DescrGrupoProd { get; set; }

        [PersistenceProperty("DescrSubgrupoProd", DirectionParameter.InputOptional)]
        public string DescrSubgrupoProd { get; set; }

        [PersistenceProperty("NcmProd", DirectionParameter.InputOptional)]
        public string NcmProd { get; set; }

        [PersistenceProperty("UnidadeProd", DirectionParameter.InputOptional)]
        public string UnidadeProd { get; set; }

        [PersistenceProperty("ValorUnitProd", DirectionParameter.InputOptional)]
        public decimal ValorUnitProd { get; set; }

        [PersistenceProperty("CustoUnitProd", DirectionParameter.InputOptional)]
        public decimal CustoUnitProd { get; set; }

        [PersistenceProperty("ValorProd", DirectionParameter.InputOptional)]
        public decimal ValorProd { get; set; }

        [PersistenceProperty("CustoProd", DirectionParameter.InputOptional)]
        public decimal CustoProd { get; set; }

        [PersistenceProperty("Situacao", DirectionParameter.InputOptional)]
        public int Situacao { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("QtdeComprando", DirectionParameter.InputOptional)]
        public double QtdeComprando { get; set; }

        [PersistenceProperty("TotMComprando", DirectionParameter.InputOptional)]
        public double TotMComprando { get; set; }

        [PersistenceProperty("QtdeProduzindo", DirectionParameter.InputOptional)]
        public double QtdeProduzindo { get; set; }

        [PersistenceProperty("TotMProduzindo", DirectionParameter.InputOptional)]
        public double TotMProduzindo { get; set; }

        [PersistenceProperty("NOMELOJATERC", DirectionParameter.InputOptional)]
        public string NomeLojaTerc { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMETRANSP", DirectionParameter.InputOptional)]
        public string NomeTransportador { get; set; }

        [PersistenceProperty("NOMEADMINCARTAO", DirectionParameter.InputOptional)]
        public string NomeAdminCartao { get; set; }

        [PersistenceProperty("IDCONTACONTABIL", DirectionParameter.InputOptional)]
        public uint? IdContaContabil { get; set; }

        [PersistenceProperty("CODINTERNOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        private long? _tipoCalc = null;

        [PersistenceProperty("TipoCalc", DirectionParameter.InputOptional)]
        public long TipoCalc
        {
            get
            {
                if (_tipoCalc == null)
                    _tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd);

                return _tipoCalc.GetValueOrDefault((int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd);
            }
            set { _tipoCalc = value; }
        }

        [PersistenceProperty("SugestaoCompraMensal", DirectionParameter.InputOptional)]
        public decimal SugestaoCompraMensal { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrEstoque
        {
            get { return Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true); }
        }

        public string DescrGrupoSubgrupoProd
        {
            get
            {
                string retorno = DescrGrupoProd;
                if (!String.IsNullOrEmpty(DescrSubgrupoProd))
                    retorno += " - " + DescrSubgrupoProd;

                return retorno;
            }
        }

        public float Disponivel
        {
            get { return (float)Math.Round(QtdEstoque - Reserva - (PedidoConfig.LiberarPedido ? Liberacao : 0), 2); }
        }

        public string QtdEstoqueStringLabel
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);

                return QtdEstoque + descrTipoCalculo + ((TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05
                    || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6) ?
                    " (" + Math.Round(QtdEstoque / 6, 2) + " br)" : "");
            }
        }

        public string EstoqueDisponivel
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);
                float disponivel = Disponivel;

                return disponivel + descrTipoCalculo + ((TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05
                    || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6) ? 
                    " (" + Math.Round(disponivel / 6, 2) + " br)" : 
                    "");
            }
        }

        public string EstoqueMinimoString
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);

                return EstMinimo + descrTipoCalculo + ((TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05
                    || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6) ?
                    " (" + Math.Round(EstMinimo / 6, 2) + " br)" : "");
            }
        }

        public string ReservaString
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);

                return Reserva + descrTipoCalculo + ((TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05
                    || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6) ?
                    " (" + Math.Round(Reserva / 6, 2) + " br)" : "");
            }
            set { Reserva = Glass.Conversoes.StrParaFloat(value); }
        }

        public string LiberacaoString
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);

                return Liberacao + descrTipoCalculo + ((TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05
                    || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6) ?
                    " (" + Math.Round(Liberacao / 6, 2) + " br)" : "");
            }
            set { Liberacao = Glass.Conversoes.StrParaFloat(value); }
        }

        public string EstoqueFiscalUnidade
        {
            get { return EstoqueFiscal.ToString("0.##") + " " + UnidadeProd; }
        }

        public decimal ValorTotalProdFiscal
        {
            get { return ValorUnitProd * (decimal)EstoqueFiscal; }
        }

        public decimal CustoTotalProdFiscal
        {
            get { return CustoUnitProd * (decimal)EstoqueFiscal; }
        }

        public decimal ValorTotalProdReal
        {
            get { return ValorProd * (decimal)Disponivel; }
        }

        public decimal CustoTotalProdReal
        {
            get { return CustoProd * (decimal)Disponivel; }
        }

        public uint IdLog
        {
            get { return Glass.Conversoes.StrParaUint(IdProd + IdLoja.ToString("0##")); }
            internal set
            {
                IdProd = value.ToString().Remove(value.ToString().Length - 3).StrParaInt();
                IdLoja = value.ToString().Substring(value.ToString().Length - 3).StrParaInt();
            }
        }

        private bool? _produtoProducao = null;

        public bool ProdutoProducao
        {
            get
            {
                if (_produtoProducao == null)
                    _produtoProducao = Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) && SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)IdGrupoProd, (int?)IdSubgrupoProd);

                return _produtoProducao.GetValueOrDefault();
            }
        }

        public float Comprando
        {
            get
            {
                if (ProdutoProducao)
                    return 0;

                return (float)(TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                    TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? TotMComprando : QtdeComprando);
            }
        }

        public string DescricaoComprando
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);
                return Comprando + descrTipoCalculo;
            }
        }

        public float Produzindo
        {
            get
            {
                if (!ProdutoProducao)
                    return 0;

                return (float)(TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                    TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? TotMProduzindo : QtdeProduzindo);
            }
        }

        public string DescricaoProduzindo
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);
                return Produzindo + descrTipoCalculo;
            }
        }

        public float PendenteCompra
        {
            get
            {
                if (ProdutoProducao)
                    return 0;

                return Math.Max((float)EstMinimo - Disponivel - Comprando, 0);
            }
        }

        public string DescricaoPendenteCompra
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);
                return PendenteCompra + descrTipoCalculo;
            }
        }

        public float PendenteProducao
        {
            get
            {
                if (!ProdutoProducao)
                    return 0;

                return Math.Max((float)EstMinimo - Disponivel - Produzindo, 0);
            }
        }

        public string DescricaoPendenteProducao
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true);
                return PendenteProducao + descrTipoCalculo;
            }
        }

        private DataSourcesEFD.TipoPartEnum? _tipoPart;

        public int? TipoPart
        {
            get
            {
                if (_tipoPart == null)
                {
                    _tipoPart = IdLojaTerc > 0 ? (DataSourcesEFD.TipoPartEnum?)DataSourcesEFD.TipoPartEnum.Loja :
                        IdFornec > 0 ? (DataSourcesEFD.TipoPartEnum?)DataSourcesEFD.TipoPartEnum.Fornecedor :
                        IdTransportador > 0 ? (DataSourcesEFD.TipoPartEnum?)DataSourcesEFD.TipoPartEnum.Transportador :
                        IdCliente > 0 ? (DataSourcesEFD.TipoPartEnum?)DataSourcesEFD.TipoPartEnum.Cliente :
                        IdAdminCartao > 0 ? (DataSourcesEFD.TipoPartEnum?)DataSourcesEFD.TipoPartEnum.AdministradoraCartao : null;
                }

                return (int?)_tipoPart;
            }
            set { _tipoPart = (DataSourcesEFD.TipoPartEnum?)value; }
        }

        public int? IdPart
        {
            get
            {
                switch ((DataSourcesEFD.TipoPartEnum?)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return IdFornec;
                    case DataSourcesEFD.TipoPartEnum.Loja: return IdLojaTerc;
                    case DataSourcesEFD.TipoPartEnum.Transportador: return IdTransportador;
                    case DataSourcesEFD.TipoPartEnum.Cliente: return IdCliente;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return IdAdminCartao;
                    default: return null;
                }
            }
            set
            {
                switch ((DataSourcesEFD.TipoPartEnum?)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Cliente:
                        IdCliente = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.Fornecedor:
                        IdFornec = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.Loja:
                        IdLojaTerc = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.Transportador:
                        IdTransportador = value;
                        break;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao:
                        IdAdminCartao = value;
                        break;
                }
            }
        }

        [Log("Tipo Portador Item")]
        public string DescrTipoPart
        {
            get { return TipoPart != null ? DataSourcesEFD.Instance.GetDescrTipoParticipante(TipoPart.Value) : ""; }
        }

        [Log("Portador Item")]
        public string DescrPart
        {
            get
            {
                switch ((DataSourcesEFD.TipoPartEnum?)TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return NomeFornec;
                    case DataSourcesEFD.TipoPartEnum.Loja: return NomeLojaTerc;
                    case DataSourcesEFD.TipoPartEnum.Transportador: return NomeTransportador;
                    case DataSourcesEFD.TipoPartEnum.Cliente: return NomeCliente;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return NomeAdminCartao;
                    default: return "";
                }
            }
        }

        public bool EditVisible
        {
            get 
            {
                return Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente);
            }
        }

        public string DescrQtdeEstoque
        {
            get
            {
                return (float)Math.Round(TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ?
                    M2 : QtdEstoque, 2) + DescrEstoque;
            }
        }

        public string DescrSituacao
        {
            get { return Situacao == 1 ? "Ativo" : "Inativo"; }
        }

        #endregion

        #region IProdutoLoja Members

        int Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoProduto
        {
            get { return IdProd; }
        }

        int Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoLoja
        {
            get { return IdLoja; }
        }

        float Sync.Fiscal.EFD.Entidade.IProdutoLoja.QuantidadeEmPosseDeTerceiros
        {
            get { return (float)QtdePosseTerceiros; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoCliente
        {
            get { return IdCliente; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoFornecedor
        {
            get { return IdFornec; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoLojaTerceiro
        {
            get { return IdLojaTerc; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoLoja.CodigoTransportador
        {
            get { return IdTransportador; }
        }

        #endregion
    }
}