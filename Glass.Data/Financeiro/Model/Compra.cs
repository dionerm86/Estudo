using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Xml.Serialization;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CompraDAO))]
    [PersistenceClass("compra")]
    public class Compra : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoEnum : int
        {
            Ativa = 1,
            Finalizada,
            Cancelada,
            EmAndamento,
            AguardandoEntrega
        }

        public enum TipoCompraEnum : int
        {
            AVista = 1,
            APrazo,
            Estoque,
            Producao,
            AntecipFornec
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCOMPRA", PersistenceParameterType.IdentityKey)]
        public uint IdCompra { get; set; }

        [PersistenceProperty("IDLOJA")]
        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFORNEC")]
        [Log("Fornecedor", "Nomefantasia", typeof(FornecedorDAO))]
        public uint? IdFornec { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        [Log("Forma Pagto.", "Descricao", typeof(FormaPagtoDAO))]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [PersistenceProperty("IDPEDIDOESPELHO")]
        public uint? IdPedidoEspelho { get; set; }
        
        [PersistenceProperty("TIPOCOMPRA")]
        public int TipoCompra { get; set; }

        [PersistenceProperty("NF")]
        public string Nf { get; set; }

        [PersistenceProperty("VALORNF")]
        public decimal ValorNf { get; set; }

        private SituacaoEnum _situacao = SituacaoEnum.Ativa;

        /// <summary>
        /// 1 - Ativa
        /// 2 - Finalizada
        /// 3 - Cancelada
        /// </summary>.
        [PersistenceProperty("SITUACAO")]
        [Log("Situação")]
        public SituacaoEnum Situacao
        {
            get { return _situacao; }
            set { _situacao = value; }
        }

        [PersistenceProperty("VALORENTRADA")]
        [Log("Valor Entrada")]
        public decimal ValorEntrada { get; set; }

        [PersistenceProperty("NUMPARC")]
        [Log("Num. Parc.")]
        public int NumParc { get; set; }

        [Log("Valor Parc.")]
        [PersistenceProperty("VALORPARC")]
        public decimal ValorParc { get; set; }

        [PersistenceProperty("DATABASEVENC")]
        public DateTime? DataBaseVenc { get; set; }

        [PersistenceProperty("BOLETOCHEGOU")]
        public bool BoletoChegou { get; set; }

        [PersistenceProperty("DATAFINALIZADA")]
        [Log("Data de FinalizacãSo")]
        public DateTime? DataFinalizada { get; set; }

        [PersistenceProperty("DESCONTO")]
        [Log("Desconto")]
        public decimal Desconto { get; set; }

        [PersistenceProperty("ESTOQUEBAIXADO")]
        public bool EstoqueBaixado { get; set; }

        [PersistenceProperty("FRETE")]
        [Log("Frete")]
        public decimal Frete { get; set; }

        [PersistenceProperty("ICMS")]
        [Log("Icms")]
        public decimal Icms { get; set; }

        [PersistenceProperty("SEGURO")]
        [Log("Seguro")]
        public decimal Seguro { get; set; }

        [PersistenceProperty("IPI")]
        [Log("Ipi")]
        public decimal Ipi { get; set; }

        [PersistenceProperty("TOTAL")]
        [Log("Total")]
        public decimal Total { get; set; }

        [PersistenceProperty("OBS")]
        [Log("Obs")]
        public string Obs { get; set; }

        [PersistenceProperty("DATAFABRICA")]
        public DateTime? DataFabrica { get; set; }

        [PersistenceProperty("CONTABIL")]
        public bool Contabil { get; set; }

        [PersistenceProperty("IDFUNCFINAL")]
        [Log("Usuário Finalização", "Nome", typeof(FuncionarioDAO))]
        public uint? IdFuncFinal { get; set; }

        [PersistenceProperty("VALORTRIBUTADO")]
        public Decimal ValorTributado { get; set; }

        [PersistenceProperty("OUTRASDESPESAS")]
        public Decimal OutrasDespesas { get; set; }

        [Log(TipoLog.Cancelamento, "Sinal")]
        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [PersistenceProperty("DATASAIDA")]
        public DateTime? DataSaida { get; set; }

        [PersistenceProperty("IDCOTACAOCOMPRA", DirectionParameter.Input)]
        public uint? IdCotacaoCompra { get; set; }

        [PersistenceProperty("GERADABENEF")]
        public bool GeradaBenef { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }
                
        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("EMAILFORNEC", DirectionParameter.InputOptional)]
        public string EmailFornec { get; set; }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("FormaPagto", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("TIPOPAGTOFORNEC", DirectionParameter.InputOptional)]
        public uint? TipoPagtoFornec { get; set; }

        private string _enderecoFornec;

        [PersistenceProperty("ENDERECOFORNEC", DirectionParameter.InputOptional)]
        public string EnderecoFornec
        {
            get { return _enderecoFornec != null ? _enderecoFornec : String.Empty; }
            set { _enderecoFornec = value; }
        }

        private string _numeroFornec;

        [PersistenceProperty("NUMEROFORNEC", DirectionParameter.InputOptional)]
        public string NumeroFornec
        {
            get { return _numeroFornec != null ? _numeroFornec : String.Empty; }
            set { _numeroFornec = value; }
        }

        private string _complFornec;

        [PersistenceProperty("COMPLFORNEC", DirectionParameter.InputOptional)]
        public string ComplFornec
        {
            get { return _complFornec != null ? _complFornec : String.Empty; }
            set { _complFornec = value; }
        }

        private string _bairroFornec;

        [PersistenceProperty("BAIRROFORNEC", DirectionParameter.InputOptional)]
        public string BairroFornec
        {
            get { return _bairroFornec != null ? _bairroFornec : String.Empty; }
            set { _bairroFornec = value; }
        }

        private string _nomeCidadeFornec;

        [PersistenceProperty("NOMECIDADEFORNEC", DirectionParameter.InputOptional)]
        public string NomeCidadeFornec
        {
            get { return _nomeCidadeFornec != null ? _nomeCidadeFornec : String.Empty; }
            set { _nomeCidadeFornec = value; }
        }

        private string _nomeUfFornec;

        [PersistenceProperty("NOMEUFFORNEC", DirectionParameter.InputOptional)]
        public string NomeUfFornec
        {
            get { return _nomeUfFornec != null ? _nomeUfFornec : String.Empty; }
            set { _nomeUfFornec = value; }
        }

        private string _cepFornec;

        [PersistenceProperty("CEPFORNEC", DirectionParameter.InputOptional)]
        public string CepFornec
        {
            get { return _cepFornec != null ? _cepFornec : String.Empty; }
            set { _cepFornec = value; }
        }

        [PersistenceProperty("TELCONTFORNEC", DirectionParameter.InputOptional)]
        public string TelcontFornec { get; set; }

        [PersistenceProperty("FAXFORNEC", DirectionParameter.InputOptional)]
        public string FaxFornec { get; set; }

        [PersistenceProperty("ENDERECOLOJA", DirectionParameter.InputOptional)]
        public string EnderecoLoja { get; set; }

        [PersistenceProperty("NUMEROLOJA", DirectionParameter.InputOptional)]
        public string NumeroLoja { get; set; }

        [PersistenceProperty("COMPLLOJA", DirectionParameter.InputOptional)]
        public string ComplLoja { get; set; }

        [PersistenceProperty("BAIRROLOJA", DirectionParameter.InputOptional)]
        public string BairroLoja { get; set; }

        [PersistenceProperty("CIDADELOJA", DirectionParameter.InputOptional)]
        public string CidadeLoja { get; set; }

        [PersistenceProperty("UFLOJA", DirectionParameter.InputOptional)]
        public string UfLoja { get; set; }

        [PersistenceProperty("TELEFONELOJA", DirectionParameter.InputOptional)]
        public string TelefoneLoja { get; set; }

        [PersistenceProperty("FAXLOJA", DirectionParameter.InputOptional)]
        public string FaxLoja { get; set; }

        [PersistenceProperty("CNPJLOJA", DirectionParameter.InputOptional)]
        public string CnpjLoja { get; set; }

        [PersistenceProperty("IELOJA", DirectionParameter.InputOptional)]
        public string IeLoja { get; set; }

        [PersistenceProperty("TEMNFE", DirectionParameter.InputOptional)]
        public bool TemNFe { get; set; }

        [PersistenceProperty("POSCOMPRA", DirectionParameter.InputOptional)]
        public long PosCompra { get; set; }

        [PersistenceProperty("NOMEFUNCFINAL", DirectionParameter.InputOptional)]
        public string NomeFuncFinal { get; set; }

        [PersistenceProperty("TOTALPEDIDO", DirectionParameter.InputOptional)]
        public decimal? TotalPedido { get; set; }

        [PersistenceProperty("DATAENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaPedido { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DATAENTRADA", DirectionParameter.InputOptional)]
        public DateTime? DataEntrada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("USUENTRADA", DirectionParameter.InputOptional)]
        public uint? UsuEntrada { get; set; }

        private string _nomeFunc;

        [XmlIgnore]
        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get
            {
                return BibliotecaTexto.GetTwoFirstNames(_nomeFunc) + (base.Usucad != IdFuncFinal ? " (Cad.: " + BibliotecaTexto.GetFirstName(base.DescrUsuCad) + ")" : "");
            }
            set { _nomeFunc = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public string NumeroNfe { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDSPEDIDO", DirectionParameter.InputOptional)]
        public string IdsPedido { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Parcelas

        private DateTime[] _datasParcelas = null;

        public DateTime[] DatasParcelas
        {
            get
            {
                if (_datasParcelas == null)
                    _datasParcelas = new DateTime[FinanceiroConfig.Compra.NumeroParcelasCompra];

                return _datasParcelas;
            }
            set { _datasParcelas = value; }
        }

        private decimal[] _valoresParcelas = null;

        public decimal[] ValoresParcelas
        {
            get
            {
                if (_valoresParcelas == null)
                    _valoresParcelas = new decimal[FinanceiroConfig.Compra.NumeroParcelasCompra];

                return _valoresParcelas;
            }
            set { _valoresParcelas = value; }
        }

        private string[] _boletosParcelas = null;

        public string[] BoletosParcelas
        {
            get
            {
                if (_boletosParcelas == null)
                    _boletosParcelas = new string[FinanceiroConfig.Compra.NumeroParcelasCompra];

                return _boletosParcelas;
            }
            set { _boletosParcelas = value; }
        }

        private uint[] _formasPagtoParcelas = null;

        public uint[] FormasPagtoParcelas
        {
            get
            {
                if (_formasPagtoParcelas == null)
                    _formasPagtoParcelas = new uint[FinanceiroConfig.Compra.NumeroParcelasCompra];

                return _formasPagtoParcelas;
            }
            set { _formasPagtoParcelas = value; }
        }

        #endregion

        [XmlIgnore]
        public bool RecebeuSinal
        {
            get { return IdSinalCompra > 0; }
            
        }

        public string IdNomeFornec
        {
            get { return IdFornec + " - " + NomeFornec; }
        }

        public string DescrPagto { get; set; }

        public string DescrTipoCompra
        {
            get
            {
                switch (TipoCompra)
                {
                    case (int)TipoCompraEnum.AVista: return "À Vista";
                    case (int)TipoCompraEnum.Estoque: return "Estoque";
                    case (int)TipoCompraEnum.Producao: return "Produção";
                    case (int)TipoCompraEnum.AntecipFornec: return "Antecip. Fornecedor";
                    default: return "À Prazo";
                }
            }
        }

        [Log("Situacao")]
        public string DescrSituacao
        {
            get
            {
                switch (_situacao)
                {
                    case SituacaoEnum.Ativa: return "Ativa";
                    case SituacaoEnum.Finalizada: return "Finalizada";
                    case SituacaoEnum.Cancelada: return "Cancelada";
                    case SituacaoEnum.EmAndamento: return "Em andamento";
                    case SituacaoEnum.AguardandoEntrega: return "Aguardando Entrega";
                    default:
                        return "";
                }
            }
        }

        public bool EditVisible
        {
            get
            {
                return _situacao == SituacaoEnum.Ativa &&
                    (UserInfo.GetUserInfo.CodUser == base.Usucad || Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.EditarQualquerCompra));
            }
        }

        /// <summary>
        /// Apenas compras que não estejam canceladas e que sejam do usuário logado ou que o usuário logado 
        /// não seja do almoxarifado podem ser canceladas.
        /// </summary>
        public bool CancelVisible
        {
            get
            {
                return _situacao != SituacaoEnum.Cancelada && (UserInfo.GetUserInfo.CodUser == base.Usucad ||
                    UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.AuxAlmoxarifado) &&
                    !CompraNotaFiscalDAO.Instance.PossuiNFe(IdCompra);
            }
        }

        /// <summary>
        /// Apenas compras finalizadas e que sejam do usuário logado ou que o usuário logado
        /// não seja do almoxarifado podem ser canceladas.
        /// </summary>
        public bool CancelPagtoVisible
        {
            get
            {
                return false;

                //return _situacao == SituacaoEnum.Finalizada && (UserInfo.GetUserInfo.CodUser == base.Usucad ||
                //    UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.AuxAlmoxarifado);
            }
        }

        /// <summary>
        /// Apenas funcionários que não sejam do almoxarifado podem finalizar compras
        /// </summary>
        public bool FinalizarVisible
        {
            get
            {
                return UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.AuxAlmoxarifado ||
                    Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque);
            }
        }

        /// <summary>
        /// Apenas funcionários que não sejam do almoxarifado podem finalizar compras
        /// </summary>
        public bool FinalizarAguardandoEntregaVisible
        {
            get
            {
                return FinanceiroConfig.Compra.UsarControleFinalizacaoCompra
                        && (UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.AuxAlmoxarifado
                        || Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                        && Situacao == SituacaoEnum.AguardandoEntrega;
            }
        }

        // Apenas funcionários que sejam do almoxarifado podem baixar estoque de compras diretamente
        public bool BaixarEstoqueVisible
        {
            get
            {
                return !EstoqueConfig.EntradaEstoqueManual && (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.AuxAlmoxarifado ||
                    Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque)) && !FinalizarVisible;
            }
        }

        public bool ReabrirVisible
        {
            get
            {
                return (_situacao == SituacaoEnum.Finalizada || _situacao == SituacaoEnum.AguardandoEntrega) &&
                    ContasPagarDAO.Instance.GetPagasCount(IdCompra) == 0 && !CompraNotaFiscalDAO.Instance.PossuiNFe(IdCompra);
            }
        }

        public bool ProdutoChegouVisible
        {
            get { return _situacao == SituacaoEnum.EmAndamento; }
        }

        public string TituloRelatorio
        {
            get
            {
                return !string.IsNullOrEmpty(Nf) || IdPedidoEspelho > 0 ?
                    string.Format("NF/Pedido: {0}", !string.IsNullOrEmpty(Nf) ? Nf : IdPedidoEspelho > 0 ? IdPedidoEspelho.Value.ToString() : string.Empty) :
                    string.Empty;
            }
        }

        public string EnderecoCompletoFornec
        {
            get { return EnderecoFornec + " " + NumeroFornec + " " + BairroFornec + " - " + NomeCidadeFornec + "/" + NomeUfFornec + " - CEP " + CepFornec; }
        }

        public string FoneFaxFornec
        {
            get { return TelcontFornec + " / " + FaxFornec; }
        }

        public string EnderecoCompletoLoja
        {
            get
            {
                return EnderecoLoja + ", " + NumeroLoja + ", " + (!String.IsNullOrEmpty(ComplLoja) ? ComplLoja + ", " : "") +
                    BairroLoja + " - " + CidadeLoja + "/" + UfLoja;
            }
        }

        public string DadosLoja
        {
            get { return EnderecoCompletoLoja + "\n" + "Fone: " + TelefoneLoja + "    Fax: " + FaxLoja; }
        }

        public bool IsVendedor
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor; }
        }

        public bool IsFinanceiro
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão de fotos na grid
        /// </summary>
        public bool FotosVisible
        {
            get
            {
                return true;
                // Exibe apenas se a compra estiver "Aberta" ou se a compra estiver finalizada e puder ser reaberta
                //return _situacao == SituacaoEnum.Ativa || (_situacao == SituacaoEnum.Finalizada && ReabrirVisible);
            }
        }

        public bool GerarNFeVisible
        {
            get { return _situacao == SituacaoEnum.Finalizada && !TemNFe; }
        }

        public string ComplementoIdCompra
        {
            get
            {
                if (IdPedidoEspelho > 0)
                {
                    char retorno = 'A';
                    return ((char)((int)retorno + PosCompra)).ToString();
                }
                else
                    return "";
            }
        }

        private string _criterio;

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio
        {
            get { return _criterio; }
            set { _criterio = value; }
        }

        public bool IsCompraSemValores
        {
            get
            {
                if (TipoCompra != (int)TipoCompraEnum.Estoque && TipoCompra != (int)TipoCompraEnum.Producao)
                    return false;

                return FornecedorDAO.Instance.IsFornecedorProprio(IdFornec.GetValueOrDefault());
            }
        }

        /// <summary>
        /// Numero de dias que a entrega da compra esta atrasada.
        /// </summary>
        public int AtrasoEntrega
        {
            get
            {
                if (FinanceiroConfig.Compra.UsarControleFinalizacaoCompra && Situacao == SituacaoEnum.AguardandoEntrega &&
                    DataFabrica.HasValue && DataFabrica.Value.Date < DateTime.Now.Date)
                    return (DateTime.Now.Date - DataFabrica.Value.Date).Days;

                return 0;
            }
        }

        /// <summary>
        /// Exibir ícone com a informação do número da nota fiscal gerada.
        /// </summary>
        public bool ExibirNfeGerada
        {
            get { return !String.IsNullOrEmpty(NumeroNfe) && NumeroNfe != "0"; }
        }

        public string LogotipoLoja
        {
            get
            {
                var arqLogotipo = "logo" + System.Configuration.ConfigurationManager.AppSettings["sistema"];

                arqLogotipo += IdLoja > 0 ? IdLoja.ToString() : "";

                if (Logotipo.EsconderLogotipoRelatorios)
                    arqLogotipo = "logoEmBranco";

                return "file:///" + System.Web.HttpContext.Current.Request.PhysicalApplicationPath.Replace('\\', '/') + "Images/" +
                    arqLogotipo + ".png";
            }
        }

        /// <summary>
        /// Indica se é para exibir o centro de custo
        /// </summary>
        public bool ExibirCentroCusto
        {
            get
            {
                return FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0 && Situacao == SituacaoEnum.Finalizada;
            }
        }

        /// <summary>
        /// Verifica se o valor do centro de custo foi totalmente informado.
        /// </summary>
        public bool CentroCustoCompleto
        {
            get
            {
                return Total == CentroCustoAssociadoDAO.Instance.ObtemTotalPorCompra((int)IdCompra);
            }
        }

        #endregion
    }
}