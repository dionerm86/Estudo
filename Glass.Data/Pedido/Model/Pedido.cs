using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Drawing;
using System.Xml.Serialization;
using Glass.Configuracoes;
using Glass.Log;
using System.ComponentModel;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoDAO))]
	[PersistenceClass("pedido")]
	public class Pedido : ModelBaseCadastro, IContainerCalculo
	{
        /*
            Criação de campos novos nesta model devem ser incluídos nos métodos SqlComissao() e SqlRptSit(), na PedidoDAO
         */

        #region Construtores

        public Pedido()
        {
            // Define valores padrão para as variáveis
            _tipoAcrescimo = 2;
            _tipoDesconto = 2;
            SituacaoProducao = 1;
        }

        #endregion

        #region Enumeradores

        public enum SituacaoPedido : int
        {
            Ativo                               = 1,
            AtivoConferencia,
            EmConferencia,
            Conferido,
            Confirmado,                         // 5
            Cancelado,
            ConfirmadoLiberacao,
            LiberadoParcialmente,
            AguardandoFinalizacaoFinanceiro,
            AguardandoConfirmacaoFinanceiro     // 10
        }

        public enum TipoVendaPedido : int
        {
            AVista = 1,
            APrazo,
            Reposição,
            Garantia,
            Obra,
            Funcionario
        }

        public enum TipoEntregaPedido : int
        {
            Balcao = 1,
            Comum,
            Temperado,
            Entrega,
            ManutencaoTemperado,
            Esquadria
        }

        public enum TipoComissao : long
        {
            Todos = -1,
            Funcionario,
            Comissionado,
            Instalador,
            Gerente
        }

        public enum SituacaoProducaoEnum
        {
            NaoEntregue = 1,
            Pendente,
            Pronto,
            Entregue,
            Instalado
        }

        /// <summary>
        /// Possíveis tipos de pedido.
        /// </summary>
        public enum TipoPedidoEnum
        {
            Selecione = 0,
            /// <summary>
            /// Venda.
            /// </summary>
            [Description("Venda")]
            Venda = 1,
            /// <summary>
            /// Revenda.
            /// </summary>
            [Description("Revenda")]
            Revenda,
            /// <summary>
            /// Mão de obra.
            /// </summary>
            [Description("Mão de Obra")]
            MaoDeObra,
            /// <summary>
            /// Produção.
            /// </summary>
            [Description("Produção")]
            Producao,
            /// <summary>
            /// Mão de obra especial.
            /// </summary>
            [Description("Mão de Obra Especial")]
            MaoDeObraEspecial
        }

        public enum SituacaoVolumeEnum : int
        {
            SemVolume = 1,
            Pendente,
            Finalizado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDPROJETO")]
        public uint? IdProjeto { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDFUNCCONFIRMARFINANC")]
        public uint IdFuncConfirmarFinanc { get; set; }

        [PersistenceProperty("IDFUNCFINALIZARFINANC")]
        public uint IdFuncFinalizarFinanc { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLI")]
        public uint IdCli { get; set; }

        [Log(TipoLog.Cancelamento, "Comissionado", "Nome", typeof(ComissionadoDAO))]
        [PersistenceProperty("IDCOMISSIONADO")]
        public uint? IdComissionado { get; set; }

        [Log(TipoLog.Cancelamento, "Medidor", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDMEDIDOR")]
        public uint? IdMedidor { get; set; }

        [Log("Forma Pagto.", "Descricao", typeof(FormaPagtoDAO))]
        [PersistenceProperty("IDFORMAPAGTO")]
        public uint? IdFormaPagto { get; set; }

        [Log(TipoLog.Cancelamento, "Forma Pagto. 2", "Descricao", typeof(FormaPagtoDAO))]
        [PersistenceProperty("IDFORMAPAGTO2")]
        public uint? IdFormaPagto2 { get; set; }

        [Log("Tipo Cartão", "Descricao", typeof(TipoCartaoCreditoDAO))]
        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [Log(TipoLog.Cancelamento, "Tipo Cartão 2", "Descricao", typeof(TipoCartaoCreditoDAO))]
        [PersistenceProperty("IDTIPOCARTAO2")]
        public uint? IdTipoCartao2 { get; set; }

        [Log("Orçamento")]
        [PersistenceProperty("IDORCAMENTO")]
        public uint? IdOrcamento { get; set; }

        [Log("Obra")]
        [PersistenceProperty("IDOBRA")]
        public uint? IdObra { get; set; }

        [Log("Parcela", "Descricao", typeof(ParcelasDAO))]
        [PersistenceProperty("IDPARCELA")]
        public uint? IdParcela { get; set; }

        [Log(TipoLog.Cancelamento, "Sinal")]
        [PersistenceProperty("IDSINAL")]
        [XmlIgnore]
        public uint? IdSinal { get; set; }

        [Log(TipoLog.Cancelamento, "Pagto. Antecipado")]
        [PersistenceProperty("IDPAGAMENTOANTECIPADO")]
        [XmlIgnore]
        public uint? IdPagamentoAntecipado { get; set; }

        [Log(TipoLog.Cancelamento, "Cód. Ped. Cli.")]
        [PersistenceProperty("CODCLIENTE")]
        public string CodCliente { get; set; }

        [Log(TipoLog.Cancelamento, "Prazo Entrega")]
        [PersistenceProperty("PRAZOENTREGA")]
        public string PrazoEntrega { get; set; }

        [PersistenceProperty("TIPOENTREGA")]
        public int? TipoEntrega { get; set; }

        /// <summary>
        /// 1-À Vista
        /// 2-À Prazo
        /// 3-Reposição
        /// 4-Garantia
        /// 5-Obra
        /// </summary>
        [PersistenceProperty("TIPOVENDA")]
        public int? TipoVenda { get; set; }

        [Log("Data de Entrega")]
        [PersistenceProperty("DATAENTREGA")]
        public DateTime? DataEntrega { get; set; }

        [Log(TipoLog.Cancelamento, "Data de Entrega Original")]
        [PersistenceProperty("DATAENTREGAORIGINAL")]
        public DateTime? DataEntregaOriginal { get; set; }

        [Log("Valor do Frete")]
        [PersistenceProperty("VALORENTREGA")]
        public decimal ValorEntrega { get; set; }

        private SituacaoPedido _situacao = SituacaoPedido.Ativo;

		[PersistenceProperty("SITUACAO")]
        public SituacaoPedido Situacao
		{
			get { return _situacao; }
			set { _situacao = value; }
		}

        [Log("Valor Entrada")]
        [PersistenceProperty("VALORENTRADA")]
        public decimal ValorEntrada { get; set; }

        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        private int _tipoDesconto;

        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto
        {
            get
            {
                if (_tipoDesconto == 0) _tipoDesconto = 2;
                return _tipoDesconto;
            }
            set { _tipoDesconto = value; }
        }

        [PersistenceProperty("IDFUNCDESC", DirectionParameter.Input)]
        public uint? IdFuncDesc { get; set; }

        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        private int _tipoAcrescimo;

        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo
        {
            get
            {
                if (_tipoAcrescimo == 0) _tipoAcrescimo = 2;
                return _tipoAcrescimo;
            }
            set { _tipoAcrescimo = value; }
        }

        [Log(TipoLog.Cancelamento, "Perc. Comissão")]
        [PersistenceProperty("PERCCOMISSAO")]
        public float PercComissao { get; set; }

        [Log(TipoLog.Cancelamento, "Valor Comissão")]
        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [Log(TipoLog.Cancelamento, "Total")]
        [PersistenceProperty("TOTAL", DirectionParameter.OutputOnlyInsert)]
        public decimal Total { get; set; }

        [Log(TipoLog.Cancelamento, "Peso")]
        [PersistenceProperty("PESO", DirectionParameter.Input)]
        public float Peso { get; set; }

        [Log(TipoLog.Cancelamento, "Total m²")]
        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        private int _numParc = 1;

        [Log("Núm. Parcelas")]
        [PersistenceProperty("NUMPARC")]
        public int NumParc
        {
            get { return _numParc; }
            set { _numParc = value; }
        }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("TAXAPRAZO")]
        public float TaxaPrazo { get; set; }

        [Log(TipoLog.Cancelamento, "Custo")]
        [PersistenceProperty("CUSTOPEDIDO")]
        public decimal CustoPedido { get; set; }

        [Log(TipoLog.Cancelamento, "Data Confirmação")]
        [PersistenceProperty("DATACONF")]
        public DateTime? DataConf { get; set; }

        [Log(TipoLog.Cancelamento, "Usuário Confirmou", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUCONF")]
        public uint? UsuConf { get; set; }

        [PersistenceProperty("DATACANC")]
        public DateTime? DataCanc { get; set; }

        [PersistenceProperty("USUCANC")]
        public uint? UsuCanc { get; set; }

        /// <summary>
        /// Identifica se o local da obra do pedido foi especificado
        /// </summary>
        [Log(TipoLog.Cancelamento, "Local Obra")]
        [PersistenceProperty("LOCALOBRA")]
        public bool LocalObra { get; set; }

        [Log(TipoLog.Cancelamento, "Endereço Obra")]
        [PersistenceProperty("ENDERECOOBRA")]
        public string EnderecoObra { get; set; }

        [Log(TipoLog.Cancelamento, "CEP Obra")]
        [PersistenceProperty("CEPOBRA")]
        public string CepObra { get; set; }

        [Log(TipoLog.Cancelamento, "Bairro Obra")]
        [PersistenceProperty("BAIRROOBRA")]
        public string BairroObra { get; set; }

        [Log(TipoLog.Cancelamento, "Cidade Obra")]
        [PersistenceProperty("CIDADEOBRA")]
        public string CidadeObra { get; set; }

        [Log(TipoLog.Cancelamento, "Num. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        /// <summary>
        /// IdPedido original ao qual a reposição se refere
        /// </summary>
        [Log(TipoLog.Cancelamento, "Pedido Original")]
        [PersistenceProperty("IDPEDIDOANTERIOR")]
        public uint? IdPedidoAnterior { get; set; }

        [Log("Fast Delivery")]
        [PersistenceProperty("FASTDELIVERY")]
        public bool FastDelivery { get; set; }

        [Log(TipoLog.Cancelamento, "Taxa Fast Delivery")]
        [PersistenceProperty("TAXAFASTDELIVERY")]
        public float TaxaFastDelivery { get; set; }

        private DateTime _dataPedido;

        [Log(TipoLog.Cancelamento, "Data do Pedido")]
        [PersistenceProperty("DATAPEDIDO")]
        public DateTime DataPedido
        {
            get
            {
                if (_dataPedido.Ticks == 0)
                    _dataPedido = DataCad;
                return _dataPedido;
            }
            set { _dataPedido = value; }
        }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAICMS")]
        public float AliquotaIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliquotaIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }
        
        [Log(TipoLog.Atualizacao, "Tipo do Pedido")]
        [PersistenceProperty("TIPOPEDIDO")]
        public int TipoPedido { get; set; }

        [Log("Têmpera Fora")]
        [PersistenceProperty("TEMPERAFORA")]
        public bool TemperaFora { get; set; }

        [PersistenceProperty("SITUACAOPRODUCAO")]
        public int SituacaoProducao { get; set; }

        [Log(TipoLog.Cancelamento, "Funcionário Venda", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCVENDA")]
        public uint? IdFuncVenda { get; set; }

        [Log(TipoLog.Cancelamento, "Gerado WebGlass Parceiros")]
        [PersistenceProperty("GERADOPARCEIRO")]
        public bool GeradoParceiro { get; set; }

        [PersistenceProperty("VALORCREDITOAOCONFIRMAR")]
        public decimal? ValorCreditoAoConfirmar { get; set; }

        [PersistenceProperty("CREDITOGERADOCONFIRMAR")]
        public decimal? CreditoGeradoConfirmar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCONFIRMAR")]
        public decimal? CreditoUtilizadoConfirmar { get; set; }

        [Log("Valor Pagamento Antecipado")]
        [PersistenceProperty("VALORPAGAMENTOANTECIPADO")]
        public decimal ValorPagamentoAntecipado { get; set; }

        [Log("Data Pedido Pronto")]
        [PersistenceProperty("DATAPRONTO")]
        public DateTime? DataPronto { get; set; }

        [Log("Liber. Financ.")]
        [PersistenceProperty("LIBERADOFINANC", DirectionParameter.InputOptional)]
        public bool LiberadoFinanc { get; set; }

        [Log("Obs. Liberação")]
        [PersistenceProperty("OBSLIBERACAO")]
        public string ObsLiberacao { get; set; }

        [PersistenceProperty("IMPORTADO")]
        public bool Importado { get; set; }

        /// <summary>
        /// Define o valor percentual da comissão
        /// </summary>
        [Log("Perc. Comissão Func.")]
        [PersistenceProperty("PERCENTUALCOMISSAO")]
        public float PercentualComissao { get; set; }

        /// <summary>
        /// Indica se esse pedido deve ter uma OC de transferência antes de uma de venda
        /// </summary>
        [Log("Deve Transferir?")]
        [PersistenceProperty("DEVETRANSFERIR")]
        public bool DeveTransferir { get; set; }

        /// <summary>
        /// Indica se esse pedido deve ter OC apenas de transferência
        /// </summary>
        [PersistenceProperty("APENASTRANSFERENCIA")]
        public bool ApenasTransferencia { get; set; }

        [Log("Data Finalização")]
        [PersistenceProperty("DATAFIN", DirectionParameter.Input)]
        public DateTime? DataFin { get; set; }

        [Log("Usuário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUFIN", DirectionParameter.Input)]
        public uint? UsuFin { get; set; }

        [PersistenceProperty("OrdemCargaParcial")]
        public bool OrdemCargaParcial { get; set; }

        [Log("Ignorar o pedido na comissão")]
        [PersistenceProperty("IgnorarComissao")]
        public bool IgnorarComissao { get; set; }

        [Log("Motivo de ignorar o pedido na comissão")]
        [PersistenceProperty("MotivoIgnorarComissao")]
        public string MotivoIgnorarComissao { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR")]
        public int? IdTransportador { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE", Direction = DirectionParameter.Input)]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA", Direction = DirectionParameter.Input)]
        public decimal RentabilidadeFinanceira { get; set; }

        [PersistenceProperty("TRANSACAO")]
        public bool Transacao { get; set; }

        #region Dados para exportação

        [PersistenceProperty("IdPedidoExterno")]
        public uint IdPedidoExterno { get; set; }

        [PersistenceProperty("IdClienteExterno")]
        public uint IdClienteExterno { get; set; }

        [PersistenceProperty("ROTAEXTERNA")]
        public string RotaExterna { get; set; }

        [PersistenceProperty("CLIENTEEXTERNO")]
        public string ClienteExterno { get; set; }

        [PersistenceProperty("PEDCLIEXTERNO")]
        public string PedCliExterno { get; set; }

        [PersistenceProperty("CELCLIEXTERNO")]
        public string CelCliExterno { get; set; }

        [PersistenceProperty("TOTALPEDIDOEXTERNO")]
        public decimal TotalPedidoExterno { get; set; }

        [PersistenceProperty("CidadeClienteExterno")]
        public string CidadeClienteExterno { get; set; }

        [PersistenceProperty("UfClienteExterno")]
        public string UfClienteExterno { get; set; }

        [PersistenceProperty("EnderecoClienteExterno")]
        public string EnderecoClienteExterno { get; set; }

        [PersistenceProperty("GERARPEDIDOPRODUCAOCORTE")]
        public bool GerarPedidoProducaoCorte { get; set; }

        [PersistenceProperty("IDPEDIDOREVENDA")]
        public int? IdPedidoRevenda { get; set; }



        #endregion

        #endregion

        #region Produção

        public static string GetDescrSituacaoProducao(int tipoPedido, int situacao, int tipoEntrega, LoginUsuario login)
        {
            switch (situacao)
            {
                case (int)SituacaoProducaoEnum.Pendente:
                    return "Pendente";
                case (int)SituacaoProducaoEnum.Pronto:
                    return "Pronto";
                case (int)SituacaoProducaoEnum.Entregue:
                    return "Entregue";
                case (int)SituacaoProducaoEnum.Instalado:
                    return "Instalado";
                default:
                    string descrNaoImpressa = !PedidoConfig.TelaListagem.ExibirSituacaoPendenteECommerce ||
                        (login != null ? !login.IsCliente : true) ? "Etiqueta não impressa" :
                        "Pendente";

                    return !PedidoConfig.LiberarPedido || tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda ? "-" : descrNaoImpressa;
            }
        }

        [XmlIgnore]
        [PersistenceProperty("DataProducao", DirectionParameter.InputOptional)]
        public DateTime? DataProducao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DataProntoCorte", DirectionParameter.InputOptional)]
        public DateTime? DataProntoCorte { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DataEntregue", DirectionParameter.InputOptional)]
        public DateTime? DataEntregue { get; set; }

        private string _funcProd;

        [XmlIgnore]
        [PersistenceProperty("FuncProd", DirectionParameter.InputOptional)]
        public string FuncProd
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_funcProd); }
            set { _funcProd = value; }
        }

        private string _funcEntregue;

        [XmlIgnore]
        [PersistenceProperty("FuncEntregue", DirectionParameter.InputOptional)]
        public string FuncEntregue
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_funcEntregue); }
            set { _funcEntregue = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("SitProducao", DirectionParameter.InputOptional)]
        public int SitProducao { get; set; }

        [XmlIgnore]
        public string DescrSitProducao
        {
            get
            {
                switch (SitProducao)
                {
                    case 1:
                        return "Confirmado";
                    case 2:
                        return "Produção";
                    case 3:
                        return "Pronto";
                    case 4:
                        return "Entregue";
                    default:
                        return String.Empty;
                }
            }
        }

        [XmlIgnore]
        public string DescrSitProdRpt
        {
            get
            {
                switch (SitProducao)
                {
                    case 1:
                        return "Confirmados";
                    case 2:
                        return "Em Produção";
                    case 3:
                        return "Prontos";
                    case 4:
                        return "Entregues";
                    default:
                        return String.Empty;
                }
            }
        }

        [XmlIgnore]
        internal LoginUsuario Login { get; set; }

        [XmlIgnore]
        [Log("Situação Produção")]
        public string DescrSituacaoProducao
        {
            get { return GetDescrSituacaoProducao(TipoPedido, SituacaoProducao, TipoEntrega != null ? TipoEntrega.Value : 0, Login ?? UserInfo.GetUserInfo); }
        }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("ExibirFinalizacoesFinanceiro", DirectionParameter.InputOptional)]
        public bool ExibirFinalizacoesFinanceiro { get; set; }

        private decimal _totalReal;
        
        [XmlIgnore]
        [PersistenceProperty("TOTALREAL", DirectionParameter.InputOptional)]
        public decimal TotalReal
        {
            get { return _totalReal > 0 ? _totalReal : Total; }
            set { _totalReal = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("DATAENTRADA", DirectionParameter.InputOptional)]
        public DateTime? DataEntrada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("USUENTRADA", DirectionParameter.InputOptional)]
        public uint? UsuEntrada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORCREDITOAORECEBERSINAL", DirectionParameter.InputOptional)]
        public decimal? ValorCreditoAoReceberSinal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CREDITOGERADORECEBERSINAL", DirectionParameter.InputOptional)]
        public decimal? CreditoGeradoReceberSinal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CREDITOUTILIZADORECEBERSINAL", DirectionParameter.InputOptional)]
        public decimal? CreditoUtilizadoReceberSinal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PAGAMENTOANTECIPADO", DirectionParameter.InputOptional)]
        public bool PagamentoAntecipado { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdItensProjeto", DirectionParameter.InputOptional)]
        public string IdItensProjeto { get; set; }

        private string _nomeCli;

        [XmlIgnore]
        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCli
        {
            get { return _nomeCli != null ? _nomeCli.Replace("'", "").Replace("\"", "") : String.Empty; }
            set { _nomeCli = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("CodRota", DirectionParameter.InputOptional)]
        public string CodRota { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CpfCnpjCliente", DirectionParameter.InputOptional)]
        public string CpfCnpjCliente { get; set; }

        [XmlIgnore]
        public string TituloCpfCnpjCliente
        {
            get { return TipoPessoaCliente == "F" ? "CPF: " : "CNPJ: "; }
        }

        [XmlIgnore]
        [PersistenceProperty("RgInscrEstadualCliente", DirectionParameter.InputOptional)]
        public string RgInscrEstadualCliente { get; set; }

        [XmlIgnore]
        public string TituloRgInscrEstadualCliente
        {
            get { return TipoPessoaCliente == "F" ? "RG: " : "Inscrição Estadual: "; }
        }

        [XmlIgnore]
        [PersistenceProperty("TipoPessoaCliente", DirectionParameter.InputOptional)]
        public string TipoPessoaCliente { get; set; }

        [XmlIgnore]
        public string NomeCliente
        {
            get { return IdCli + " - " + _nomeCli; }
        }

        [XmlIgnore]
        public string NomeInicialCli
        {
            get { return IdCli + " - " + BibliotecaTexto.GetThreeFirstWords(_nomeCli); }
        }

        [XmlIgnore]
        [PersistenceProperty("ContatoCliente", DirectionParameter.InputOptional)]
        public string ContatoCliente { get; set; }

        private string _nomeFunc;

        [XmlIgnore]
        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get
            {
                return BibliotecaTexto.GetTwoFirstNames(_nomeFunc) + (base.Usucad != IdFunc ? " (Cad.: " + BibliotecaTexto.GetFirstName(base.DescrUsuCad) + ")" : "");
            }
            set { _nomeFunc = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("NomeFuncPedidoAnterior", DirectionParameter.InputOptional)]
        public string NomeFuncPedidoAnterior { get; set; }

        private string _nomeLoja;

        [XmlIgnore]
        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja : String.Empty; }
            set { _nomeLoja = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("DADOSLOJA", DirectionParameter.InputOptional)]
        public string DadosLoja { get; set; }

        private string _formaPagto;

        [XmlIgnore]
        [PersistenceProperty("FormaPagto", DirectionParameter.InputOptional)]
        public string FormaPagto
        {
            get { return _formaPagto != null ? _formaPagto : String.Empty; }
            set { _formaPagto = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("CliRevenda", DirectionParameter.InputOptional)]
        public bool CliRevenda { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMECOMISSIONADO", DirectionParameter.InputOptional)]
        public string NomeComissionado { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMEMEDIDOR", DirectionParameter.InputOptional)]
        public string NomeMedidor { get; set; }

        /// <summary>
        /// Valor da comissão do funcionário
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("Comissao", DirectionParameter.InputOptional)]
        public decimal Comissao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SaldoObra", DirectionParameter.InputOptional)]
        public decimal SaldoObra { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TotalPedidosAbertosObra", DirectionParameter.InputOptional)]
        public decimal TotalPedidosAbertosObra { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DescrObra", DirectionParameter.InputOptional)]
        public string DescrObra { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NomeUsuEntrada", DirectionParameter.InputOptional)]
        public string NomeUsuEntrada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TOTALESPELHO", DirectionParameter.InputOptional)]
        public decimal TotalEspelho { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ValorIpiEspelho", DirectionParameter.InputOptional)]
        public decimal ValorIpiEspelho { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ValorIcmsEspelho", DirectionParameter.InputOptional)]
        public decimal ValorIcmsEspelho { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TOTMESPELHO", DirectionParameter.InputOptional)]
        public double TotmEspelho { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PESOESPELHO", DirectionParameter.InputOptional)]
        public double PesoEspelho { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DATAFINALIZACAOINST", DirectionParameter.InputOptional)]
        public DateTime? DataFinalizacaoInst { get; set; }

        private string _nomeInst;

        [XmlIgnore]
        [PersistenceProperty("NOMEINST", DirectionParameter.InputOptional)]
        public string NomeInst
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeInst); }
            set { _nomeInst = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("NOMEFUNCVENDA", DirectionParameter.InputOptional)]
        public string NomeFuncVenda { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORPAGOCOMISSAO", DirectionParameter.InputOptional)]
        public decimal ValorPagoComissao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TEMALTERACAOPCP", DirectionParameter.InputOptional)]
        public bool TemAlteracaoPcp { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DADOSVIDROSVENDIDOS", DirectionParameter.InputOptional)]
        public string DadosVidrosVendidos { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDFUNCCLIENTE", DirectionParameter.InputOptional)]
        public uint? IdFuncCliente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMEFUNCCLIENTE", DirectionParameter.InputOptional)]
        public string NomeFuncCliente { get; set; }

        private bool? _temEspelho;

        [XmlIgnore]
        [PersistenceProperty("TEMESPELHO", DirectionParameter.InputOptional)]
        public bool TemEspelho
        {
            get 
            {
                if (_temEspelho == null)
                    _temEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(IdPedido);

                return _temEspelho != null ? _temEspelho.Value : false;
            }
            set { _temEspelho = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("CLIENTEPAGAANTECIPADO", DirectionParameter.InputOptional)]
        public bool ClientePagaAntecipado { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PERCSINALMINCLIENTE", DirectionParameter.InputOptional)]
        public float? PercSinalMinCliente { get; set; }

        private string _nomeUsuFin;

        [XmlIgnore]
        [PersistenceProperty("NOMEUSUFIN", DirectionParameter.InputOptional)]
        public string NomeUsuFin
        {
            get { return BibliotecaTexto.GetFirstName(_nomeUsuFin); }
            set { _nomeUsuFin = value; }
        }

        private string _nomeUsuConf;

        [XmlIgnore]
        [PersistenceProperty("NOMEUSUCONF", DirectionParameter.InputOptional)]
        public string NomeUsuConf
        {
            get { return BibliotecaTexto.GetFirstName(_nomeUsuConf); }
            set { _nomeUsuConf = value; }
        }

        private string _nomeUsuLib;

        [XmlIgnore]
        [PersistenceProperty("NOMEUSULIB", DirectionParameter.InputOptional)]
        public string NomeUsuLib
        {
            get { return BibliotecaTexto.GetFirstName(_nomeUsuLib); }
            set { _nomeUsuLib = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("IDSOCS", DirectionParameter.InputOptional)]
        public string IdsOCs { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TEMRECEBIMENTO", DirectionParameter.InputOptional)]
        public bool TemRecebimento { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMETRANSPORTADOR", DirectionParameter.InputOptional)]
        public string NomeTransportador { get; set; }

        public string CodCliComCodCliExterno
        {
            get
            { return CodCliente + " - " + PedCliExterno; }
        }

        #region Finalização / Confirmação do Financeiro

        [PersistenceProperty("MOTIVOERROFINALIZARFINANC", DirectionParameter.InputOptional)]
        public string MotivoErroFinalizarFinanc { get; set; }

        [PersistenceProperty("MOTIVOERROCONFIRMARFINANC", DirectionParameter.InputOptional)]
        public string MotivoErroConfirmarFinanc { get; set; }

        [PersistenceProperty("ObsLiberacaoCliente", DirectionParameter.InputOptional)]
        public string ObsLiberacaoCliente { get; set; }

        #endregion

        #endregion

        #region Propriedades de Suporte

        #region Métodos estáticos internos

        internal static decimal GetValorPerc(int tipoRetorno, int tipo, decimal valor, decimal total)
        {
            if (tipoRetorno == 1)
                return tipo == 1 ? valor : valor / (total > 0 ? total : 1) * 100;
            else
                return tipo == 1 ? total * valor / 100 : valor;
        }

        internal static string GetTextoPerc(int tipo, decimal valor, decimal total)
        {
            if (valor == 0)
                return 0.ToString("C") + " (0%)";

            decimal valorPorc = GetValorPerc(1, tipo, valor, total);
            decimal valorReal = GetValorPerc(2, tipo, valor, total);

            return valorReal.ToString("C") + " (" + (PedidoConfig.Comissao.ExibirPercentualCom2CasasDecimais ? valorPorc.ToString("0.##") : valorPorc.ToString("0.#####")) + "%)";
        }

        #endregion

        public byte[] BarCodeImage
        {
            get
            {
                return Utils.GetBarCode(IdPedido.ToString());
            }
        }

        [XmlIgnore]
        public string IdPedidoCodCliente
        {
            get { return IdPedido + (!String.IsNullOrEmpty(CodCliente) ? " (" + CodCliente + ")" : String.Empty); }
        }

        [XmlIgnore]
        public string TelVendedor
        {
            get
            {
                var retorno = "";

                if (IdFunc > 0)
                {
                    retorno = FuncionarioDAO.Instance.ObtemTelCel(IdFunc);
                }

                return retorno;
            }
        }

        [XmlIgnore]
        public string DescrSaldoObraPedidos
        {
            get
            {
                string retorno = "";

                if (IdObra > 0)
                {
                    Obra o = new Obra();
                    o.Saldo = SaldoObra;
                    o.TotalPedidosAbertos = TotalPedidosAbertosObra;

                    retorno = o.DescrSaldoPedidos.Replace("\n", "<br />");
                }

                return retorno;
            }
        }

        [XmlIgnore]
        public long QtdePecas
        {
            get { return PedidoDAO.Instance.ObtemQuantidadePecas(null, IdPedido); }
        }

        public string NfeAssociada { get { return PedidosNotaFiscalDAO.Instance.NotasFiscaisGeradas(null, IdPedido); } }

        [XmlIgnore]
        public DateTime? DataLiberacao
        {
            get { return IdLiberarPedido > 0 ? LiberarPedidoDAO.Instance.ObtemDataLiberacao(IdLiberarPedido.Value) : (DateTime?)null; }
        }

        [XmlIgnore]
        public bool RecebeuSinal
        {
            get { return IdSinal > 0; }
        }

        public decimal TotalRecebSinalPagtoAntecip
        {
            get
            {
                decimal totalReceb = IdPagamentoAntecipado > 0 ? ValorPagamentoAntecipado : 0;

                if (IdSinal > 0)
                    totalReceb += ValorEntrada;

                return totalReceb;
            }
        }

        [XmlIgnore]
        public bool MaoDeObra
        {
            get { return TipoPedido == (int)TipoPedidoEnum.MaoDeObra; }
        }

        [XmlIgnore]
        public bool Producao
        {
            get { return TipoPedido == (int)TipoPedidoEnum.Producao; }
        }

        [XmlIgnore]
        public bool MaoDeObraEspecial
        {
            get { return TipoPedido == (int)TipoPedidoEnum.MaoDeObraEspecial; }
        }

        [XmlIgnore]
        public Color CorLinhaLista
        {
            get
            {
                if (PedidoConfig.TelaListagem.ExibirLinhaAzulSePedidoPronto && SituacaoProducao == (int)Pedido.SituacaoProducaoEnum.Pronto)
                    return Color.Blue;
                
                if (PedidoConfig.TelaListagem.ExibirLinhaPretaSeRevenda && TipoPedido == (int)TipoPedidoEnum.Revenda)
                    return Color.Black;

                if (PedidoConfig.TelaListagem.ExibirLinhaVermelhaSePendenteOuTemAlteracaoPCP && (SituacaoProducao == (int)SituacaoProducaoEnum.Pendente || TemAlteracaoPcp))
                    return Color.Red;

                if (GeradoParceiro || Importado)
                    return Color.FromName(PedidoConfig.TelaListagem.CorLinhaSeImportadoOuGeradoParceiro);

                return Color.Black;
            }
        }

        [XmlIgnore]
        public decimal DescontoReal
        {
            get { return _tipoDesconto == 1 ? Desconto * 100 / TotalSemDesconto : Desconto; }
        }

        [XmlIgnore]
        public decimal AcrescimoReal
        {
            get { return _tipoAcrescimo == 1 ? Acrescimo * 100 / TotalSemAcrescimo : Acrescimo; }
        }

        [XmlIgnore]
        public string TotalComDescontoConcatenado
        {
            get
            {
                return Total.ToString("C");
            }
        }

        [XmlIgnore]
        public decimal TotalSemDesconto
        {
            get { return PedidoDAO.Instance.GetTotalSemDesconto(null, IdPedido, Total); }
        }

        [XmlIgnore]
        public decimal TotalSemAcrescimo
        {
            get { return PedidoDAO.Instance.GetTotalSemAcrescimo(IdPedido, Total); }
        }

        [XmlIgnore]
        public string TotalRealComDescontoConcatenado
        {
            get
            {
                return TotalReal.ToString("C");
            }
        }

        [XmlIgnore]
        public decimal TotalRealSemDesconto
        {
            get { return PedidoDAO.Instance.GetTotalSemDesconto(null, IdPedido, TotalReal); }
        }

        [XmlIgnore]
        public decimal TotalBruto
        {
            get { return PedidoDAO.Instance.GetTotalBruto(IdPedido, false); }
        }

        [XmlIgnore]
        public decimal TotalBrutoEspelho
        {
            get { return PedidoDAO.Instance.GetTotalBruto(IdPedido, true); }
        }

        [XmlIgnore]
        public decimal TotalPedidoFluxo
        {
            get 
            { 
                return !Glass.Configuracoes.Geral.NaoVendeVidro() ? (TemEspelho ? (decimal)TotalEspelho : Total) :
                    TemEspelho ? TotalBrutoEspelho : TotalBruto;
            }
        }

        private decimal _totalParaLiberacao = 0;

        [XmlIgnore]
        public decimal TotalParaLiberacao
        {
            get 
            {
                if (_totalParaLiberacao > 0)
                    return _totalParaLiberacao;

                _totalParaLiberacao = PedidoDAO.Instance.GetTotalParaLiberacao(null, IdPedido);

                return _totalParaLiberacao;
            }
        }


        [XmlIgnore]
        public decimal ValorASerPagoLiberacao
        {
            get 
            {
                decimal total = PedidoDAO.Instance.GetTotalParaLiberacao(null, IdPedido);

                if (IdPagamentoAntecipado > 0)
                    total -= ValorPagamentoAntecipado;

                if (IdSinal > 0)
                    total -= ValorEntrada;

                if (IdObra > 0)
                    return 0;

                return total - DescontoTotal;
            }
        }

        [XmlIgnore]
        public string ValorNegativoLiberar
        {
            get
            {
                if (IdObra > 0)
                    return "";

                var valorNegativoLiberar = new System.Text.StringBuilder("Valor Negativo para liberar. ");

                decimal total = PedidoDAO.Instance.GetTotalParaLiberacao(null, IdPedido);

                if (PCPConfig.UsarConferenciaFluxo)
                    valorNegativoLiberar.Append("Sistema está configurado para considerar o valor confirmado para liberação. ");

                if (IdPagamentoAntecipado > 0 && total > 0 && total - ValorPagamentoAntecipado < 0)
                {
                    total -= ValorPagamentoAntecipado;
                    valorNegativoLiberar.Append("O valor do pagamento antecipado do pedido supera o valor a liberar. ");
                }

                if (IdSinal > 0 && total > 0 && total - ValorEntrada < 0)
                {
                    total -= ValorEntrada;
                    valorNegativoLiberar.Append("O valor da entrada do pedido supera o valor a liberar. ");
                }

                if (DescontoTotal > 0 && total - DescontoTotal < 0)
                    valorNegativoLiberar.Append("O valor do desconto total aplicado ao pedido supera o valor a liberar. ");

                return valorNegativoLiberar.ToString();
            }
        }

        [XmlIgnore]
        public bool UsarControleReposicao
        {
            get { return (TipoVenda == 3) && PCPConfig.ControlarProducao && 
                PedidoReposicaoDAO.Instance.IsPedidoReposicao(IdPedido); }
        }

        /// <summary>
        /// Identifica que este pedido está sendo inserido a partir do orçamento rápido
        /// </summary>
        [XmlIgnore]
        public bool FromOrcamentoRapido { get; set; }

        [XmlIgnore]
        public string ImpressoPor { get; set; }

        [XmlIgnore]
        public string FastDeliveryString
        {
            get { return FastDelivery ? "Sim" : "Não"; }
        }

        [XmlIgnore]
        public decimal ValorFastDeliveryLiberacao
        {
            get { return FastDelivery ? TotalParaLiberacao - (TotalParaLiberacao / (1 + ((decimal)TaxaFastDelivery / 100))) : 0; }
        }

        [XmlIgnore]
        public string TemperaForaString
        {
            get { return TemperaFora ? "Sim" : "Não"; }
        }

        [XmlIgnore]
        [Log(TipoLog.Cancelamento, "Tipo Pedido")]
        public string DescricaoTipoPedido
        {
            get
            {
                switch (TipoPedido)
                {
                    case (int)TipoPedidoEnum.MaoDeObraEspecial: return "Mão de obra Especial";
                    case (int)TipoPedidoEnum.MaoDeObra: return "Mão de obra";
                    case (int)TipoPedidoEnum.Producao: return "Produção";
                    case (int)TipoPedidoEnum.Revenda: return "Revenda";
                    default: return TipoVenda == 3 ? "Reposição" : TipoVenda == 4 ? "Garantia" : "Venda";
                }
            }
        }

        [XmlIgnore]
        [Log("Desconto")]
        public string TextoDesconto
        {
            get { return _tipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        private bool _descontoTotalPcp = false;

        [XmlIgnore]
        public bool DescontoTotalPcp
        {
            get { return _descontoTotalPcp; }
            set { _descontoTotal = null; _descontoTotalPcp = value; }
        }

        private decimal? _descontoTotal = null;

        [XmlIgnore]
        public decimal DescontoTotal
        {
            get
            {
                if (_descontoTotal == null)
                {
                    decimal descontoProdutos, descontoPedido;

                    if (_descontoTotalPcp)
                    {
                        descontoProdutos = PedidoEspelhoDAO.Instance.GetDescontoProdutos(null, IdPedido);
                        descontoPedido = PedidoEspelhoDAO.Instance.GetDescontoPedido(null, IdPedido, descontoProdutos);
                    }
                    else
                    {
                        descontoProdutos = PedidoDAO.Instance.GetDescontoProdutos(null, IdPedido);
                        descontoPedido = PedidoDAO.Instance.GetDescontoPedido(null, IdPedido, descontoProdutos);
                    }

                    _descontoTotal = descontoPedido + descontoProdutos;
                }

                return _descontoTotal.Value;
            }
        }

        private decimal? _descontoExibirLib = null;

        [XmlIgnore]
        public decimal DescontoExibirLib
        {
            get
            {
                if (_descontoExibirLib == null)
                {
                    decimal descontoProdutos;

                    if (_descontoTotalPcp)
                    {
                        descontoProdutos = PedidoEspelhoDAO.Instance.GetDescontoProdutos(null, IdPedido);
                        _descontoExibirLib = PedidoEspelhoDAO.Instance.GetDescontoPedido(null, IdPedido, descontoProdutos);
                    }
                    else
                    {
                        descontoProdutos = PedidoDAO.Instance.GetDescontoProdutos(null, IdPedido);
                        _descontoExibirLib = PedidoDAO.Instance.GetDescontoPedido(null, IdPedido, descontoProdutos);
                    }
                }

                return _descontoExibirLib.Value;
            }
        }

        [XmlIgnore]
        public string TextoDescontoTotal
        {
            get
            {
                return DescontoTotal.ToString("C");
            }
        }

        [XmlIgnore]
        public string TextoDescontoPerc
        {
            get { return GetTextoPerc(_tipoDesconto, (decimal)Desconto, TotalSemDesconto); }
        }

        [XmlIgnore]
        public string TextoDescontoTotalPerc
        {
            get { return GetTextoPerc(2, DescontoTotal, TotalSemDesconto); }
        }

        [XmlIgnore]
        public string TextoPercDescontoTotal
        {
            get
            {
                if (DescontoTotal == 0)
                    return "";

                decimal valorPerc = GetValorPerc(1, 2, DescontoTotal, TotalSemDesconto);
                return "(" + valorPerc.ToString("0.##") + ")%";
            }
        }

        [XmlIgnore]
        public string TextoPercDescontoTotalReal
        {
            get
            {
                if (DescontoTotal == 0)
                    return "";

                decimal valorPerc = GetValorPerc(1, 2, DescontoTotal, TotalRealSemDesconto);
                return "(" + valorPerc.ToString("0.##") + ")%";
            }
        }

        private bool _buscarDescontoFluxoParaLiberacao;

        [XmlIgnore]
        public bool BuscarDescontoFluxoParaLiberacao
        {
            get { return _buscarDescontoFluxoParaLiberacao; }
            set { _buscarDescontoFluxoParaLiberacao = value; }
        }

        #region Relatório de comissão

        [XmlIgnore]
        public decimal Comissao_DescontoTotal { get; set; }

        [XmlIgnore]
        public decimal Comissao_TotalPedidos { get; set; }

        [XmlIgnore]
        public string Comissao_TextoDescontoPerc
        {
            get { return GetTextoPerc(2, Comissao_DescontoTotal, Comissao_TotalPedidos); }
        }

        #endregion

        [XmlIgnore]
        [Log("Acréscimo")]
        public string TextoAcrescimo
        {
            get { return _tipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        private decimal? _acrescimoTotal = null;

        [XmlIgnore]
        public decimal AcrescimoTotal
        {
            get
            {
                if (_acrescimoTotal == null)
                    _acrescimoTotal = PedidoDAO.Instance.GetAcrescimoPedido(IdPedido) + PedidoDAO.Instance.GetAcrescimoProdutos(IdPedido);

                return _acrescimoTotal.Value;
            }
        }

        [XmlIgnore]
        public string TextoAcrescimoTotal
        {
            get { return AcrescimoTotal.ToString("C"); }
        }

        [XmlIgnore]
        public string TextoAcrescimoPerc
        {
            get { return GetTextoPerc(_tipoAcrescimo, (decimal)Acrescimo, TotalSemAcrescimo + DescontoTotal); }
        }

        [XmlIgnore]
        public string TextoAcrescimoTotalPerc
        {
            get { return GetTextoPerc(2, PedidoDAO.Instance.GetAcrescimoPedido(IdPedido) + PedidoDAO.Instance.GetAcrescimoProdutos(IdPedido), TotalSemAcrescimo + DescontoTotal); }
        }

        [XmlIgnore]
        public decimal TotalSemIcms
        {
            get
            {
                decimal perc = Total / (_totalReal > 0 ? _totalReal : Total > 0 ? Total : 1);
                return Total - Math.Round(ValorIcms * perc, 2);
            }
        }

        [XmlIgnore]
        public decimal TotalSemIcmsReal
        {
            get { return TotalReal - ValorIcms; }
        }

        [XmlIgnore]
        public decimal TotalSemImpostos
        {
            get
            {
                decimal perc = Total / (_totalReal > 0 ? _totalReal : Total > 0 ? Total : 1);
                return Total - Math.Round(ValorIcms * perc, 2) - Math.Round(ValorIpi * perc, 2);
            }
        }

        [XmlIgnore]
        public decimal TotalComImpostos
        {
            get
            {
                decimal perc = Total / (_totalReal > 0 ? _totalReal : Total > 0 ? Total : 1);
                return Total;
            }
        }

        [XmlIgnore]
        public decimal TotalParaComissao
        {
            get 
            {
                switch (Configuracoes.ComissaoConfig.TotalParaComissao)
                {
                    case Glass.Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemIcms:
                        return TotalSemIcms;
                    case Glass.Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalComImpostos:
                        return TotalComImpostos;
                    case Glass.Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemImpostos:
                        return TotalSemImpostos;
                    default:
                        return 0;
                }
            }
        }

        [XmlIgnore]
        public decimal TotalRealParaComissao
        {
            get 
            { 
                return TotalReal - (FinanceiroConfig.FinanceiroPagto.SubtrairICMSCalculoComissao ? ValorIcms : 0); 
            }
        }

        [XmlIgnore]
        public decimal TotalParaComissaoProdutoInstalado{ get; set; }

        [XmlIgnore]
        public int NumDias { get; set; }

        [XmlIgnore]
        public bool VendidoFuncionario
        {
            get { return IdFuncVenda > 0 && TipoVenda == (int)TipoVendaPedido.Funcionario; }
        }

        [XmlIgnore]
        public string InfoAdicional { get; set; }

        [XmlIgnore]
        public string CidadeData
        {
            get { return LojaDAO.Instance.GetElement(IdLoja).Cidade + ", " + Formatacoes.DataExtenso(DateTime.Now); }
        }

        [XmlIgnore]
        public string MotivoFinanceiro
        {
            get
            {
                return Situacao == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro ? MotivoErroFinalizarFinanc :
                    Situacao == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro ? MotivoErroConfirmarFinanc : null;
            }
        }

        #region Parcelas

        [XmlIgnore]
        public string DescricaoParcelas
        {
            get
            {
                if (TipoVenda == 2)
                {
                    if (IdParcela > 0)
                    {
                        foreach (var p in ParcelasDAO.Instance.GetAll())
                            if (p.IdParcela == IdParcela)
                                return " - " + p.Descricao + (!PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido ? " parcela" + (_numParc > 1 ? "s" : "") : "");
                    }
                    else
                    {
                        foreach (GenericModel p in DataSources.Instance.GetNumParc())
                            if ((int)p.Id == _numParc)
                                return " - " + p.Descr + (!PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido ? " parcela" + (_numParc > 1 ? "s" : "") : "");
                    }
                }

                return "";
            }
        }

        [XmlIgnore]
        public string PagtoParcela
        {
            get
            {
                return !String.IsNullOrEmpty(_descrParcelas) ? (_formaPagto + " - " + _descrParcelas) : _formaPagto;
            }
        }

        private string _descrParcelas;

        [XmlIgnore]
        public string DescrParcelas
        {
            get { return _descrParcelas != null ? _descrParcelas : String.Empty; }
            set { _descrParcelas = value; }
        }

        /// <summary>
        /// Usado para calcular comissão sobre valor recebido
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("TOTALPARCELASRECEBIDAS", DirectionParameter.InputOptional)]
        public decimal TotalParcelasRecebidas { get; set; }

        [XmlIgnore]
        public decimal[] ValoresParcelas { get; set; }

        [XmlIgnore]
        public DateTime[] DatasParcelas { get; set; }

        #endregion

        #region Valores

        [XmlIgnore]
        public decimal Lucro
        {
            get { return Total - CustoPedido; }
        }

        #endregion

        #region Datas

        [XmlIgnore]
        public string DataEntregaExibicao
        {
            get
            {
                return DataEntregaString + (DataEntrega != DataEntregaOriginal && DataEntregaOriginal != null ? " (" + 
                    Conversoes.ConverteData(DataEntregaOriginal, false) + ")" : "");
            }
        }

        [XmlIgnore]
        public string DataEntregaString
        {
            get { return Conversoes.ConverteData(DataEntrega, false); }
            set { DataEntrega = Conversoes.ConverteData(value); }
        }

        [XmlIgnore]
        public string DataConfString
        {
            get { return Conversoes.ConverteData(DataConf, false); }
        }

        [XmlIgnore]
        public DateTime? DataConfLib
        {
            get
            {
                if (!PedidoConfig.LiberarPedido)
                    return Situacao == SituacaoPedido.Confirmado ? DataConf : null;
                else
                    return Situacao == SituacaoPedido.ConfirmadoLiberacao ? DataConf :
                        Situacao == SituacaoPedido.LiberadoParcialmente || Situacao == SituacaoPedido.Confirmado ? DataLiberacao : null;
            }
        }

        [XmlIgnore]
        public string DataPedidoString
        {
            get { return Conversoes.ConverteData(_dataPedido, false); }
            set { _dataPedido = Conversoes.ConverteData(value).Value; }
        }

        #endregion

        #region Tipo Venda

        public static string GetDescrTipoVenda(int? tipoVenda)
        {
            return tipoVenda == (int)TipoVendaPedido.AVista ? "À Vista" :
                tipoVenda == (int)TipoVendaPedido.APrazo ? "À Prazo" :
                tipoVenda == (int)TipoVendaPedido.Reposição ? "Reposição" :
                tipoVenda == (int)TipoVendaPedido.Garantia ? "Garantia" :
                tipoVenda == (int)TipoVendaPedido.Obra ? "Obra" :
                tipoVenda == (int)TipoVendaPedido.Funcionario ? "Funcionário" :
                String.Empty;
        }

        [XmlIgnore]
        [Log("Tipo de venda")]
        public string DescrTipoVenda
        {
            get 
            {
                string descrFormaPagto = IdFormaPagto > 0 &&
                    (TipoVenda == (int)TipoVendaPedido.AVista || TipoVenda == (int)TipoVendaPedido.APrazo) ?
                    " - " + (IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Credito ? "Crédito" : FormaPagto) : String.Empty;

                return GetDescrTipoVenda(TipoVenda) + descrFormaPagto; 
            }
        }

        #endregion

        #region Situação Pedido

        [XmlIgnore]
        [Log("Situação")]
        public string DescrSituacaoPedido
        {
            get { return PedidoDAO.Instance.GetSituacaoPedido((int)_situacao); }
        }

        [XmlIgnore]
        public int IntSituacao
        {
            get { return (int)_situacao; }
        }

        #endregion

        #region Tipo Entrega

        [XmlIgnore]
        [Log("Tipo Entrega")]
        public string DescrTipoEntrega
        {
            get { return Utils.GetDescrTipoEntrega(TipoEntrega); }
        }

        #endregion

        #region Visibilidade/Enabled de itens da grid/detailview

        /// <summary>
        /// Define qual tipo de usuário pode alterar o funcionário ao cadastrar o pedido
        /// </summary>
        [XmlIgnore]
        public bool SelVendEnabled
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);
            }
        }

        [XmlIgnore]
        public bool ExibirReabrir
        {
            get { return PedidoDAO.Instance.PodeReabrir(null, IdPedido, ValorPagamentoAntecipado, _situacao, GeradoParceiro, IdCli, TemEspelho, IdObra > 0, (Pedido.TipoPedidoEnum)TipoPedido, Importado, RecebeuSinal); }
        }

        [XmlIgnore]
        public bool ExibirRelatorio
        {
            get { return true; }
        }

        [XmlIgnore]
        public bool ExibirNotaPromissoria
        {
            get { return PedidoDAO.Instance.ExibirNotaPromissoria(TipoVenda.GetValueOrDefault(), _situacao); }
        }

        [XmlIgnore]
        public bool ExibirRelatorioCalculo
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuPedido.VisualizarMemoriaCalculo);
            }
        }

        // Controla visibilidade da opcao editar na grid
        [XmlIgnore]
        public bool EditVisible
        {
            get 
            {
                LoginUsuario login = UserInfo.GetUserInfo;
                bool flagSituacao;
                bool flagVendedor = true;
                bool flagAuxAdm = true;
                bool flagSupervisorTemperado = false;
                bool flagCliente = true;

                // Apenas Ativo e Conferido podem ser editados, mas se estiver Ativo/Em Conferência ou Em Conferencia
                // e não tiver ido para conferência, pode editar.
                flagSituacao = (_situacao == SituacaoPedido.Ativo) ||
                    (!PedidoEmConfer && (_situacao == SituacaoPedido.AtivoConferencia || _situacao == SituacaoPedido.EmConferencia));

                // Se não for Gerente/Auxiliar verifica se o pedido é do usuário logado
                if (!GeradoParceiro &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.Gerente &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.AuxAdministrativo &&
                    login.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador)
                    flagVendedor = IdFunc == login.CodUser || base.Usucad == login.CodUser;

                // Se for Auxiliar Adm., só pode alterar pedidos da loja dele
                if (PedidoConfig.TelaListagem.AuxAdministrativoAlteraPedidoLojaDele &&
                    (login.TipoUsuario == (uint)Utils.TipoFuncionario.AuxAdministrativo ||
                    login.TipoUsuario == (uint)Utils.TipoFuncionario.AuxEtiqueta))
                    flagAuxAdm = IdLoja == login.IdLoja;

                // Se for supervisor temperado, pode editar pedido e mandá-lo para conferência
                if (Config.PossuiPermissao(Config.FuncaoMenuConferencia.ControleConferenciaMedicao))
                    flagSupervisorTemperado = (_situacao == SituacaoPedido.Ativo || _situacao == SituacaoPedido.AtivoConferencia)
                        && (TipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado || 
                        TipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado);

                // Se o pedido for de cliente
                if (GeradoParceiro)
                    flagCliente = PedidoConfig.PodeEditarPedidoGeradoParceiro || (PedidoConfig.ParceiroPodeEditarPedido && IdCli == UserInfo.GetUserInfo.IdCliente);

                return (flagSituacao && flagVendedor && flagAuxAdm && flagCliente) || (flagSupervisorTemperado && !GeradoParceiro);
            }
        }

        /// <summary>
        /// Controla a visibilidade do botão "Em Conferência" no cadastro de pedidos
        /// </summary>
        [XmlIgnore]
        public bool ConferenciaVisible
        {
            get
            {
                return TipoVenda == (int)TipoVendaPedido.APrazo &&
                    Geral.ControleConferencia &&
                    (_situacao == SituacaoPedido.Ativo || 
                    _situacao == SituacaoPedido.EmConferencia || 
                    _situacao == SituacaoPedido.AtivoConferencia ||
                    _situacao == SituacaoPedido.Conferido);
            }
        }

        [XmlIgnore]
        public bool DescontoVisible
        {
            get 
            {
                List<SituacaoPedido> situacoes = new List<SituacaoPedido>(new SituacaoPedido[] {
                    SituacaoPedido.ConfirmadoLiberacao
                });

                if (!PedidoConfig.LiberarPedido || TipoPedido == (int)TipoPedidoEnum.Producao)
                    situacoes.Add(SituacaoPedido.Confirmado);

                // Desconto por tipo de produto (vendedor)
                if (PedidoConfig.DescontoPedidoVendedorUmProduto && situacoes.Contains(_situacao) &&
                    ProdutosPedidoDAO.Instance.PodeAplicarDescontoVendedor(IdPedido))
                    return true;

                return situacoes.Contains(_situacao) &&
                    (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador ||
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos) ||
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor) ||
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarDataEntregaPedidoListaPedidos));
            }
        }

        /// <summary>
        /// Indica se o tipo de pedido pode ser alterado na tela de cadastro.
        /// </summary>
        [XmlIgnore]
        public bool TipoPedidoEnabled
        {
            get { return !PedidoConfig.DadosPedido.BloquearItensTipoPedido || 
                ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(IdPedido, 0) == 0; }
        }

        /// <summary>
        /// Controla se o cliente pode ser alterado ao atualizar pedido
        /// </summary>
        [XmlIgnore]
        public bool ClienteEnabled { get; set; }

        private bool? _pedidoEmConfer;

        [XmlIgnore]
        public bool PedidoEmConfer
        { 
            get 
            { 
                if (_pedidoEmConfer != null)
                    return _pedidoEmConfer.Value;

                _pedidoEmConfer = PedidoDAO.Instance.EstaEmConferencia(IdPedido);

                return _pedidoEmConfer.Value;
            }
        }

        // Apenas gerentes podem cancelar pedidos
        [XmlIgnore]
        public bool CancelarVisible
        {
            get 
            { 
                return Config.PossuiPermissao(Config.FuncaoMenuPedido.CancelarPedido) &&
                    _situacao != SituacaoPedido.Cancelado; 
            }
        }

        public bool EditObsLiberacaoVisible
        {
            get
            {
                return Situacao == SituacaoPedido.ConfirmadoLiberacao || Situacao == SituacaoPedido.Conferido;
            }
        }

        [XmlIgnore]
        public bool ComissaoVisible
        {
            get { return PedidoConfig.Comissao.ComissaoPedido && 
                !PedidoConfig.Comissao.UsarComissionadoCliente; }
        }

        [XmlIgnore]
        public bool DescontoEnabled
        {
            get 
            { 
                return true;
            }
        }

        [XmlIgnore]
        public bool ExibirImpressaoProjeto
        {
            get { return !string.IsNullOrEmpty(IdItensProjeto); }
        }

        [XmlIgnore]
        public bool ExibirImpressaoPcp
        {
            get { return PCPConfig.ExibirImpressaoPcpListaPedidos && TemEspelho; }
        }

        [XmlIgnore]
        public bool ExibirTotalEspelho
        {
            get { return PCPConfig.ExibirDadosPcpListaAposConferencia && TemEspelho; }
        }
        
         [XmlIgnore]
        public bool ExibirTotalEspelhoGerarNfe
        {
            get { return PCPConfig.UsarConferenciaFluxo && TemEspelho; }
        }

        [XmlIgnore]
        public bool ExibirImpressaoItensLiberar
        {
            get { return PedidoConfig.LiberarPedido && _situacao == SituacaoPedido.LiberadoParcialmente; }
        }

        public bool AlterarProcessoAplicacaoVisible
        {
            get
            {
                var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(null, IdPedido);
                var situacaoProducao = PedidoDAO.Instance.ObtemSituacaoProducao(null, IdPedido);
                var tipoPedido = PedidoDAO.Instance.GetTipoPedido(null, IdPedido);
                var tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(IdPedido);

                return tipoPedido == Pedido.TipoPedidoEnum.Venda && (situacaoPedido == Pedido.SituacaoPedido.Conferido || situacaoPedido == Pedido.SituacaoPedido.ConfirmadoLiberacao) &&
                    Pedido.GetDescrSituacaoProducao((int)tipoPedido, (int)situacaoProducao, tipoEntrega, UserInfo.GetUserInfo).ToLower() == "etiqueta não impressa";
            }
        }

        #endregion

        #region Campos usados no relatório

        [XmlIgnore]
        public string LocalizacaoObra
        {
            get 
            {
                string localizacao = EnderecoObra + ", " + BairroObra + " - " + CidadeObra + 
                    (!String.IsNullOrEmpty(CepObra) ? " - CEP: " + CepObra : "");

                return localizacao.Length < 10 ? String.Empty : localizacao;
            }
        }

        [XmlIgnore]
        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        private string _confirmouRecebeuSinal;

        [XmlIgnore]
        public string ConfirmouRecebeuSinal
        { 
            get { return _confirmouRecebeuSinal + " " + MovimentacaoCreditoSinal; }
            set { _confirmouRecebeuSinal = value; }
        }

        [XmlIgnore]
        public string RptDataPedido
        {
            get { return DateTime.Now.ToString("dd/MM/yy HH:mm"); }
        }

        [XmlIgnore]
        public string RptObs
        {
            get { return "Observações: "; }
        }

        [XmlIgnore]
        public bool RptIsCliente { get; set; }

        [XmlIgnore]
        public string RptPagto
        {
            get 
            {
                if (RptIsCliente)
                {
                    Parcelas parc = ParcelasDAO.Instance.GetPadraoCliente(IdCli);
                    return parc != null ? parc.DescrCompleta : "";
                }
                else
                {
                    string fp = "";

                    if (ClientePagaAntecipado)
                    {
                        fp += " - Pagto. Antecipado";
                        if (ValorEntrada > 0)
                        {
                            float? percMinSinal = ClienteDAO.Instance.GetPercMinSinalPedido(IdCli);
                            if (percMinSinal > 0)
                                fp += " (" + percMinSinal.Value.ToString("0.##") + "%)";
                        }
                    }

                    fp += !String.IsNullOrEmpty(_formaPagto) ? " - " + _formaPagto : "";

                    // À Vista
                    if (TipoVenda == (int)TipoVendaPedido.AVista)
                        return "À Vista" + fp;
                    else if (TipoVenda == (int)TipoVendaPedido.APrazo)
                        return "À Prazo" + fp;
                    else if (TipoVenda == (int)TipoVendaPedido.Reposição)
                        return "Reposição";
                    else if (TipoVenda == (int)TipoVendaPedido.Garantia)
                        return "Garantia";
                    else if (TipoVenda == (int)TipoVendaPedido.Obra)
                        return "Obra";
                    else if (TipoVenda == (int)TipoVendaPedido.Funcionario && IdFuncVenda > 0)
                        return "Funcionário (" + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(IdFuncVenda.Value)) + ")";
                    else
                        return String.Empty;
                }
            }
        }

        /// <summary>
        /// Descrição completa da parcela
        /// </summary>
        [XmlIgnore]
        public string DescricaoCompletaParcela
        {
            get
            {
                if (TipoVenda == (int)TipoVendaPedido.AVista)
                    return string.Empty;

                var parc = IdParcela > 0 ? ParcelasDAO.Instance.GetElementByPrimaryKey(IdParcela.Value) : ParcelasDAO.Instance.GetByNumeroParcelas(_numParc);

                return parc != null ? parc.DescrCompleta : string.Empty;
            }
        }

        /// <summary>
        /// Descrição completa da parcela
        /// </summary>
        [XmlIgnore]
        public string DescricaoSimplificadaParcela
        {
            get
            {
                if (TipoVenda == (int)TipoVendaPedido.AVista)
                    return string.Empty;

                var parc = IdParcela > 0 ? ParcelasDAO.Instance.GetElementByPrimaryKey(IdParcela.Value) : ParcelasDAO.Instance.GetByNumeroParcelas(_numParc);

                return parc != null ? parc.Descricao : string.Empty;
            }
        }

        [XmlIgnore]
        public string RptDataEntrega
        {
            get { return DataEntrega != null ? DataEntrega.Value.ToString("dd/MM/yy") : String.Empty; }
        }

        [XmlIgnore]
        public string RptDesconto
        {
            get { return Desconto.ToString("F2"); }
        }

        [XmlIgnore]
        public string RptSinal
        {
            get { return ValorEntrada.ToString("F2"); }
        }

        [XmlIgnore]
        public string RptTotal
        {
            get { return Total.ToString("F2"); }
        }

        [XmlIgnore]
        public decimal RptTotalPedidoObra
        {
            get { return TemEspelho ? PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("total", "idPedido=" + IdPedido) : Total; }
        }

        [XmlIgnore]
        public string RptDataConf
        {
            get 
            { 
                return DataConf != null ? DataConf.Value.ToString("dd/MM/yyyy") : String.Empty;
            }
        }

        #region Cliente

        [XmlIgnore]
        [PersistenceProperty("TIPO_PESSOA", DirectionParameter.InputOptional)]
        public string RptTipoPessoa { get; set; }

        private string _rptCpfCnpj;

        [XmlIgnore]
        [PersistenceProperty("CPF_CNPJ", DirectionParameter.InputOptional)]
        public string RptCpfCnpj
        {
            get { return _rptCpfCnpj != null ? (RptTipoPessoa == "J" ? "CNPJ: " + _rptCpfCnpj : "CPF: " + _rptCpfCnpj) : null; }
            set { _rptCpfCnpj = value; }
        }

        private string _rptRgEscinst;

        [XmlIgnore]
        [PersistenceProperty("RG_ESCINST", DirectionParameter.InputOptional)]
        public string RptRgEscinst
        {
            get { return _rptRgEscinst != null ? (RptTipoPessoa == "J" ? "Insc. Est.: " + _rptRgEscinst : "RG: " + _rptRgEscinst) : null; }
            set { _rptRgEscinst = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("RPTTELCONT", DirectionParameter.InputOptional)]
        public string RptTelCont { get; set; }

        [XmlIgnore]
        [PersistenceProperty("RPTTELRES", DirectionParameter.InputOptional)]
        public string RptTelRes { get; set; }

        [XmlIgnore]
        [PersistenceProperty("RPTTELCEL", DirectionParameter.InputOptional)]
        public string RptTelCel { get; set; }

        [XmlIgnore]
        public string RptTelContCli
        {
            get 
            {
                string tel = String.Empty;
                bool telCont = !String.IsNullOrEmpty(RptTelCont);
                bool telCel = !String.IsNullOrEmpty(RptTelCel);
                bool telRes = !String.IsNullOrEmpty(RptTelRes);

                if (telCont)
                    tel += RptTelCont;

                if (telCel)
                    tel += (tel != String.Empty ? " / " : String.Empty) + RptTelCel;

                if ((!telCont || !telCel) && telRes)
                    tel += (tel != String.Empty ? " / " : String.Empty) + RptTelRes;

                return tel;
            }
        }

        private string _rptEndereco;

        [XmlIgnore]
        [PersistenceProperty("ENDERECO", DirectionParameter.InputOptional)]
        public string RptEndereco
        {
            get { return _rptEndereco == null ? String.Empty : _rptEndereco; }
            set { _rptEndereco = value; }
        }

        private string _rptCompl;

        [XmlIgnore]
        [PersistenceProperty("COMPL", DirectionParameter.InputOptional)]
        public string RptCompl
        {
            get { return String.IsNullOrEmpty(_rptCompl) ? String.Empty : " - " + _rptCompl; }
            set { _rptCompl = value; }
        }

        private string _rptNumero;

        [XmlIgnore]
        [PersistenceProperty("NUMERO", DirectionParameter.InputOptional)]
        public string RptNumero
        {
            get { return !String.IsNullOrEmpty(_rptNumero) ? ", " + _rptNumero : String.Empty; }
            set { _rptNumero = value; }
        }

        private string _rptBairro;

        [XmlIgnore]
        [PersistenceProperty("BAIRRO", DirectionParameter.InputOptional)]
        public string RptBairro
        {
            get { return _rptBairro == null ? String.Empty : _rptBairro; }
            set { _rptBairro = value; }
        }

        private string _rptCidade;

        [XmlIgnore]
        [PersistenceProperty("CIDADE", DirectionParameter.InputOptional)]
        public string RptCidade
        {
            get { return _rptCidade == null ? String.Empty : _rptCidade; }
            set { _rptCidade = value; }
        }

        private string _rptUf;

        [XmlIgnore]
        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string RptUf
        {
            get { return _rptUf == null ? String.Empty : _rptUf; }
            set { _rptUf = value; }
        }

        private string _rptCep;

        [XmlIgnore]
        [PersistenceProperty("CEP", DirectionParameter.InputOptional)]
        public string RptCep
        {
            get { return _rptCep == null ? String.Empty : _rptCep; }
            set { _rptCep = value; }
        }

        Cliente _cliente = new Cliente();

        [XmlIgnore]
        public string RptEnderecoCobranca
        {
            get
            {
                _cliente = ClienteDAO.Instance.GetElement(IdCli);
                return _cliente.EnderecoCobranca;
            }
        }

        [XmlIgnore]
        public string RptBairroCobranca
        {
            get
            {                
                return _cliente.BairroCobranca;
            }
        }

        [XmlIgnore]
        public string RptCidadeCobranca
        {
            get
            {
                return _cliente.CidadeCobranca;
            }
        }

        [XmlIgnore]
        public string RptCepCobranca
        {
            get
            {
                return _cliente.CepCobranca;
            }
        }

        [XmlIgnore]
        [PersistenceProperty("Email", DirectionParameter.InputOptional)]
        public string RptEmail { get; set; }

        [XmlIgnore]
        public string EnderecoCompletoCliente
        {
            get
            {
                return _rptEndereco + ", " + _rptBairro + (!String.IsNullOrEmpty(_rptCompl) ? "(" + _rptCompl + ")" : String.Empty) +
                    " - " + _rptCidade + "/" + _rptUf + " " + _rptCep;
            }
        }

        [XmlIgnore]
        [PersistenceProperty("RptRotaCliente", DirectionParameter.InputOptional)]
        public string RptRotaCliente { get; set; }

        #endregion

        #region Loja

        [XmlIgnore]
        [PersistenceProperty("EMAILLOJA", DirectionParameter.InputOptional)]
        public string EmailLoja { get; set; }

        [XmlIgnore]
        public string RptNomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja.Replace("VIDRALIA ", String.Empty).Replace("Glass ", String.Empty) : ""; }
        }

        private string _rptTelefoneLoja;

        [XmlIgnore]
        [PersistenceProperty("TelefoneLoja", DirectionParameter.InputOptional)]
        public string RptTelefoneLoja
        {
            get { return _rptTelefoneLoja != null ? _rptTelefoneLoja : String.Empty; }
            set { _rptTelefoneLoja = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("FaxLoja", DirectionParameter.InputOptional)]
        public string RptFaxLoja { get; set; }

        [XmlIgnore]
        public string FoneFaxLoja
        {
            get { return "Fone: " + _rptTelefoneLoja + (!String.IsNullOrEmpty(RptFaxLoja) ? " Fax: " + RptFaxLoja : String.Empty); }
        }

        [XmlIgnore]
        [PersistenceProperty("EnderecoLoja", DirectionParameter.InputOptional)]
        public string RptEnderecoLoja { get; set; }

        private string _rptComplLoja;

        [XmlIgnore]
        [PersistenceProperty("ComplLoja", DirectionParameter.InputOptional)]
        public string RptComplLoja
        {
            get { return _rptComplLoja != null ? _rptComplLoja : String.Empty; }
            set { _rptComplLoja = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("BairroLoja", DirectionParameter.InputOptional)]
        public string RptBairroLoja { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CidadeLoja", DirectionParameter.InputOptional)]
        public string RptCidadeLoja { get; set; }

        [XmlIgnore]
        [PersistenceProperty("UfLoja", DirectionParameter.InputOptional)]
        public string RptUfLoja { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CepLoja", DirectionParameter.InputOptional)]
        public string RptCepLoja { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CnpjLoja", DirectionParameter.InputOptional)]
        public string CnpjLoja { get; set; }

        #endregion

        #region Valor com Icms

        [XmlIgnore]
        public string RptTituloValorIcms
        {
            get
            {
                string retorno = "";

                //if (PedidoConfig.Impostos.CalcularIcmsPedido)
                    retorno += "VALOR ICMS ST R$\n";

                retorno += "TOTAL R$";

                return retorno;
            }
        }

        [XmlIgnore]
        public string RptValorIcms
        {
            get
            {
                string retorno = "";

                //if (PedidoConfig.Impostos.CalcularIcmsPedido)
                    retorno += ValorIcms.ToString("N") + "\n";

                retorno += Total.ToString("N"); 
                
                return retorno;
            }
        }

        [XmlIgnore]
        public string RptValorIcmsPcp
        {
            get
            {
                string retorno = "";

                //if (PedidoConfig.Impostos.CalcularIcmsPedido)
                    retorno += ValorIcms.ToString("N") + "\n";

                retorno += TotalEspelho.ToString("N");

                return retorno;
            }
        }

        #endregion

        #endregion

        #region Comissão

        [XmlIgnore]
        [PersistenceProperty("IDINSTALADOR", DirectionParameter.InputOptional)]
        public uint? IdInstalador { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDEQUIPE", DirectionParameter.InputOptional)]
        public uint? IdEquipe { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NUMEROINTEGRANTESEQUIPE", DirectionParameter.InputOptional)]
        public long NumeroIntegrantesEquipe { get; set; }

        [XmlIgnore]
        public decimal ValorComissaoRpt { get; set; }

        private decimal? _valorComissaoTotal = null;

        /// <summary>
        /// Retorna o valor total para o cálculo da comissão.
        /// </summary>
        [XmlIgnore]
        public decimal ValorComissaoTotal
        {
            get
            {
                if (_valorComissaoTotal.HasValue)
                    return _valorComissaoTotal.Value;

                switch (ComissaoFuncionario)
                {
                    case TipoComissao.Funcionario:
                        if (ComissaoConfig != null)
                            _valorComissaoTotal = ComissaoConfigDAO.Instance.GetComissaoValor(ValorBaseCalcComissao, IdFunc, IdPedido, ComissaoConfig, ComissaoFuncionario);
                        else
                            _valorComissaoTotal = ComissaoConfigDAO.Instance.GetComissaoValor(ValorBaseCalcComissao, IdFunc, IdPedido, ComissaoFuncionario);
                        break;
                    case TipoComissao.Comissionado:
                        _valorComissaoTotal = TotalRealParaComissao > 0 ?
							Math.Round(ValorComissao * (ValorBaseCalcComissao / TotalRealParaComissao), 2) : 0; break;
                    case TipoComissao.Instalador:
                        if (IdEquipe > 0 && IdInstalador > 0)
                            _valorComissaoTotal = ComissaoConfigDAO.Instance.GetComissaoValor(ValorBaseCalcComissao, IdInstalador.Value, IdPedido, ComissaoFuncionario); break;
                }

                return _valorComissaoTotal.HasValue ? _valorComissaoTotal.Value : 0;
            }
        }

        [XmlIgnore]
        public Glass.Data.Model.ComissaoConfig ComissaoConfig { get; set; }

        /// <summary>
        /// Retorna o valor já pago para a comissão.
        /// </summary>
        [XmlIgnore]
        public decimal ValorComissaoRecebida
        {
            get
            {
                switch (ComissaoFuncionario)
                {
                    case TipoComissao.Funcionario: return ValorComissaoRecebidaFunc;
                    case TipoComissao.Comissionado: return ValorComissaoRecebidaComissionado;
                    case TipoComissao.Instalador: return ValorComissaoRecebidaInstalador;
                    case TipoComissao.Gerente: return ValorComissaoGerentePago;
                    default: return 0;
                }
            }
        }

        [XmlIgnore]
        [PersistenceProperty("VALORCOMISSAORECEBIDAFUNC", DirectionParameter.InputOptional)]
        public decimal ValorComissaoRecebidaFunc { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORCOMISSAORECEBIDACOM", DirectionParameter.InputOptional)]
        public decimal ValorComissaoRecebidaComissionado { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORCOMISSAORECEBIDAINST", DirectionParameter.InputOptional)]
        public decimal ValorComissaoRecebidaInstalador { get; set; }

        /// <summary>
        /// Retorna o valor a pagar da comissão.
        /// </summary>
        [XmlIgnore]
        public decimal ValorComissaoPagar
        {
            get 
            {
                if (ComissaoFuncionario == TipoComissao.Gerente)
                    return ValorComissaoGerentePagar - ValorComissaoGerentePago;

                decimal retorno = ValorComissaoTotal - ValorComissaoRecebida;

                // Se o retorno for negativo, provavelmente este pedido foi liberado parcial e não está sendo considerando seu valor 
                // completo ao calcular o ValorBaseCalcComissao, que é usado dentro de ValorComissaoTotal, neste caso, é necessário considerar
                // o valor total já liberado deste pedido para aí sim debitar o valor da comissão total do que já foi recebido
                if (Math.Round(ValorComissaoTotal, 2) - Math.Round(ValorComissaoRecebida, 2) < 0 && PedidoConfig.LiberarPedido && ComissaoFuncionario == TipoComissao.Funcionario)
                    return ComissaoConfigDAO.Instance.GetComissaoValor((decimal)PedidoDAO.Instance.GetTotalLiberado(IdPedido, null), IdFunc, IdPedido, ComissaoFuncionario) - ValorComissaoRecebida;

                return retorno;
            }
        }

        /// <summary>
        /// Retorna o valor do débito de comissão que deverá ser gerado.
        /// </summary>
        [XmlIgnore]
        public decimal ValorComissaoPagarTrocaDevolucao
        {
            get
            {
                var idTrocaDevolucao = TrocaDevolucaoDAO.Instance.ObtemIdTrocaDevolucaoPorPedido(null, IdPedido);

                if (idTrocaDevolucao == 0)
                    return 0;

                var creditoGerado = TrocaDevolucaoDAO.Instance.ObterCreditoGerado(null, idTrocaDevolucao);

                if (creditoGerado == 0)
                    return 0;

                switch (ComissaoFuncionario)
                {
                    case TipoComissao.Funcionario:
                        if (ComissaoConfig != null)
                            return ComissaoConfigDAO.Instance.GetComissaoValor(creditoGerado, IdFunc, IdPedido, ComissaoConfig, ComissaoFuncionario);
                        else
                            return ComissaoConfigDAO.Instance.GetComissaoValor(creditoGerado, IdFunc, IdPedido, ComissaoFuncionario);

                    case TipoComissao.Comissionado:
                        return Math.Round(creditoGerado * (ValorComissao / TotalRealParaComissao), 2);

                    case TipoComissao.Instalador:
                        if (IdEquipe > 0 && IdInstalador > 0)
                        {
                            var valorBaseCalcComissao = Math.Round(creditoGerado / (NumeroIntegrantesEquipe > 0 ? NumeroIntegrantesEquipe : 1), 2);

                            return ComissaoConfigDAO.Instance.GetComissaoValor(valorBaseCalcComissao, IdInstalador.Value, IdPedido, ComissaoFuncionario);
                        }

                        return 0;
                }

                return 0;
            }
        }

        private decimal _valorBaseCalcComissao = 0;

        /// <summary>
        /// Retorna o valor usado para o cálculo da comissão.
        /// </summary>
        [XmlIgnore]
        public decimal ValorBaseCalcComissao
        {
            get
            {
                if (_valorBaseCalcComissao > 0)
                    return _valorBaseCalcComissao;

                _valorBaseCalcComissao = TotalParaComissao;

                if (ComissaoFuncionario == Pedido.TipoComissao.Instalador && Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado)
                    _valorBaseCalcComissao = TotalParaComissaoProdutoInstalado;

                if (ComissaoFuncionario == Pedido.TipoComissao.Instalador)
                {
                    _valorBaseCalcComissao = Math.Round(_valorBaseCalcComissao / (NumeroIntegrantesEquipe > 0 ? NumeroIntegrantesEquipe : 1), 2);

                    var baseCalcRecebido =
                        Convert.ToDecimal(ComissaoPedidoDAO.Instance.GetTotalBaseCalcComissaoPedido(IdPedido,
                            (int)ComissaoFuncionario, IdInstalador.GetValueOrDefault()));

                    _valorBaseCalcComissao -= (PedidoConfig.LiberarPedido ? 0 : baseCalcRecebido);
                }

                return _valorBaseCalcComissao;
            }
        }

        /// <summary>
        /// Ainda há comissão a pagar para esse pedido?
        /// </summary>
        [XmlIgnore]
        public bool ComissaoAPagar
        {
            get { return Math.Round(ValorComissaoPagar, 2) > 0; }
        }

        /// <summary>
        /// Define para qual tipo de funcionário será feito o cálculo da comissão.
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("ComissaoFuncionario", DirectionParameter.InputOptional)]
        public TipoComissao ComissaoFuncionario { get; set; }

        #endregion

        #region Código de barras

        [XmlIgnore]
        public byte[] BarCode
        {
            get { return Utils.GetBarCode("P" + IdPedido.ToString()); }
        }

        #endregion

        #region Atualizar valor do pedido

        /// <summary>
        /// Define se o desconto e o acréscimo serão bloqueados ao atualizar.
        /// </summary>
        [XmlIgnore]
        public bool BloquearDescontoAcrescimoAtualizar
        {
            get { return false; }
        }

        #endregion

        #region Pedido de parceiro

        [XmlIgnore]
        public string IdPedidoExibir
        {
            get { return (GeradoParceiro || Importado ? "W" : "") + IdPedido; }
        }

        #endregion

        #region Movimentação de crédito

        [XmlIgnore]
        public string MovimentacaoCreditoSinal
        {
            get
            {
                if (!RecebeuSinal)
                    return "";

                decimal utilizado = CreditoUtilizadoReceberSinal != null ? CreditoUtilizadoReceberSinal.Value : 0;
                decimal gerado = CreditoGeradoReceberSinal != null ? CreditoGeradoReceberSinal.Value : 0;

                if (ValorCreditoAoReceberSinal == null || (ValorCreditoAoReceberSinal == 0 && (utilizado + gerado) == 0))
                    return "";

                return "\nCrédito inicial: " + ValorCreditoAoReceberSinal.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoReceberSinal.Value - utilizado + gerado).ToString("C");
            }
        }

        [XmlIgnore]
        [Log(TipoLog.Cancelamento, "Movimentação Crédito")]
        public string MovimentacaoCreditoConf
        {
            get
            {
                if (PedidoConfig.LiberarPedido || _situacao != SituacaoPedido.Confirmado)
                    return "";

                decimal utilizado = CreditoUtilizadoConfirmar != null ? CreditoUtilizadoConfirmar.Value : 0;
                decimal gerado = CreditoGeradoConfirmar != null ? CreditoGeradoConfirmar.Value : 0;

                if (ValorCreditoAoConfirmar == null || (ValorCreditoAoConfirmar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoConfirmar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoConfirmar.Value - utilizado + gerado).ToString("C");
            }
        }

        #endregion

        #region Desconto de administrador (log)

        /// <summary>
        /// Usado para log na tela de desconto de administrador.
        /// </summary>
        [Log(TipoLog.Atualizacao, "Produto alterado", false)]
        internal string DiferencaProdutoAdmin { get; set; }

        #endregion

        #region Ordem de Carga

        [XmlIgnore]
        public uint IdOrdemCarga { get; set; }

        /// <summary>
        /// Total peso OC
        /// </summary>
        [PersistenceProperty("PesoOC", DirectionParameter.InputOptional)]
        public double PesoOC { get; set; }

        /// <summary>
        /// Total m² OC
        /// </summary>
        [PersistenceProperty("TotMOC", DirectionParameter.InputOptional)]
        public double TotMOC { get; set; }

        /// <summary>
        /// Peso pedente para produzir
        /// </summary>
        [PersistenceProperty("PesoPendente", DirectionParameter.InputOptional)]
        public double PesoPendenteProducao { get; set; }

        /// <summary>
        /// Total de m² pedente para produzir
        /// </summary>
        [PersistenceProperty("TotMPendente", DirectionParameter.InputOptional)]
        public double TotMPendenteProducao { get; set; }

        /// <summary>
        /// Qtde de peças que não geram volume
        /// </summary>
        [PersistenceProperty("QtdePecasVidro", DirectionParameter.InputOptional)]
        public double QtdePecasVidro { get; set; }

        /// <summary>
        /// Qtde peças pedente para produzir
        /// </summary>
        [PersistenceProperty("QtdePendente", DirectionParameter.InputOptional)]
        public double QtdePecaPendenteProducao { get; set; }

        /// <summary>
        /// Quantidade total de peças do pedido.
        /// </summary>
        [PersistenceProperty("QUANTIDADEPECASPEDIDO", DirectionParameter.InputOptional)]
        public double QuantidadePecasPedido { get; set; }

        [PersistenceProperty("ValorTotalOC", DirectionParameter.InputOptional)]
        public decimal ValorTotalOC { get; set; }

        #endregion

        #region Volume

        /// <summary>
        /// Qtde volumes do pedido
        /// </summary>
        [PersistenceProperty("QtdeVolume", DirectionParameter.InputOptional)]
        public double QtdeVolume { get; set; }

        /// <summary>
        /// Total de M² de volume
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("TotMVolume", DirectionParameter.InputOptional)]
        public double TotMVolume { get; set; }

        /// <summary>
        /// Peso Total de Volume
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("PesoVolume", DirectionParameter.InputOptional)]
        public double PesoVolume { get; set; }

        /// <summary>
        /// Qtde. de peças com volume gerado
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("QtdePecasVolume", DirectionParameter.InputOptional)]
        public double QtdePecasVolume { get; set; }

        [XmlIgnore]
        public bool GerouTodosVolumes
        {
            get 
            {
                if (IdPedido == 0)
                    return false;

                return PedidoDAO.Instance.GerouTodosVolumes(null, IdPedido);
            }
        }

        [XmlIgnore]
        public string VolumesPendentes
        {
            get { return GerouTodosVolumes ? "" : "Sim"; }
        }

        [XmlIgnore]
        public double QtdePecasPendenteVolume
        {
            get
            {
                return Convert.ToDouble(QuantidadePecasPedido) - QtdePecasVolume;
            }
        }

        [XmlIgnore]
        public SituacaoVolumeEnum SituacaoVolume
        {
            get 
            {
                if (!PedidoDAO.Instance.TemVolume(null, IdPedido))
                    return SituacaoVolumeEnum.SemVolume;
                else if (PedidoDAO.Instance.TemVolumeAberto(IdPedido) || QtdePecasPendenteVolume > 0)
                    return SituacaoVolumeEnum.Pendente;
                else
                    return SituacaoVolumeEnum.Finalizado;
            }
        }

        [XmlIgnore]
        public string SituacaoVolumeStr
        {
            get 
            {
                switch (SituacaoVolume)
                {
                    case SituacaoVolumeEnum.SemVolume:
                        return "Sem Volume";
                    case SituacaoVolumeEnum.Pendente:
                        return "Pendente";
                    case SituacaoVolumeEnum.Finalizado:
                        return "Finalizado";
                    default:
                        throw new Exception("Situação do volume não encontrada.");
                }
            }
        }

        [XmlIgnore]
        public bool GerarVolumeVisible
        {
            get { return QtdePecasPendenteVolume > 0; }
        }

        public bool RelatorioVolumeVisible
        {
            get { return SituacaoVolume != SituacaoVolumeEnum.SemVolume; }
        }

        #endregion

        /// <summary>
        /// Indica se esse pedido deve ter uma OC de transferência antes de uma de venda
        /// </summary>
        public string DeveTransferirStr
        {
            get
            {
                if (DeveTransferir)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public string TemRecebimentoString { get { return TemRecebimento ? "Sim" : "Não"; } }

        public string IdPedidoClienteExterno
        {
            get
            {
                return PedCliExterno + " - " + ClienteExterno;
            }
        }

        public string ObsCliente
        {
            get
            {
                var obs = ClienteDAO.Instance.ObterObsPedido(IdCli);

                if (obs.Split(';')[0] == "Erro")
                    return obs.Split(';')[1];

                return obs;
            }
        }

        public bool TemProdutoLamComposicao
        {
            get
            {
                return ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(IdPedido);
            }
        }

        public decimal ValorComissaoGerentePagar { get; set; }

        public decimal ValorComissaoGerentePago { get; set; }

        public bool ExibirImagemPeca
        {
            get
            {
                return TemProdutoLamComposicao || TemEspelho;
            }
        }

        public PedidoExportacao.SituacaoExportacaoEnum SituacaoExportacao
        {
            get
            {
                return PedidoExportacaoDAO.Instance.GetSituacaoExportacao(IdPedido);
            }
        }

        public string DescricaoSituacaoExportacao
        {
            get
            {
                if (SituacaoExportacao == 0)
                    return "Não Exportado";

                return SituacaoExportacao.ToString();
            }
        }

        #endregion

        #region IContainerCalculo

        uint IContainerCalculo.Id
        {
            get { return IdPedido; }
        }

        private IDadosCliente cliente;

        IDadosCliente IContainerCalculo.Cliente
        {
            get
            {
                if (cliente == null)
                {
                    cliente = new ClienteDTO(() => IdCli);
                }

                return cliente;
            }
        }

        private IDadosAmbiente ambientes;

        IDadosAmbiente IContainerCalculo.Ambientes
        {
            get
            {
                if (ambientes == null)
                {
                    ambientes = new DadosAmbienteDTO(
                        this,
                        () => AmbientePedidoDAO.Instance.GetByPedido(IdPedido)
                    );
                }

                return ambientes;
            }
        }

        uint? IContainerCalculo.IdObra
        {
            get { return IdObra; }
        }

        int? IContainerCalculo.TipoEntrega
        {
            get { return TipoEntrega; }
        }

        int? IContainerCalculo.TipoVenda
        {
            get { return TipoVenda; }
        }

        bool IContainerCalculo.Reposicao
        {
            get { return TipoVenda == (int)Pedido.TipoVendaPedido.Reposição; }
        }

        bool IContainerCalculo.MaoDeObra
        {
            get { return MaoDeObra; }
        }

        bool IContainerCalculo.IsPedidoProducaoCorte
        {
            get
            {
                return IdPedidoRevenda.HasValue
                    && TipoPedido == (int)TipoPedidoEnum.Producao;
            }
        }

        uint? IContainerCalculo.IdParcela
        {
            get { return IdParcela; }
        }

        #endregion
    }
}