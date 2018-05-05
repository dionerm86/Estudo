using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoEspelhoDAO))]
	[PersistenceClass("pedido_espelho")]
	public class PedidoEspelho : IContainerCalculo
    {
        #region Enumeradores

        public enum SituacaoPedido : int
        {
            Processando = 0,
            Aberto,
            Finalizado,
            Impresso,
            ImpressoComum,
            Cancelado
        }

        public enum SituacaoCncEnum : int
        {
            SemNecessidadeNaoConferido = 1,
            NaoProjetado,
            Projetado,
            SemNecessidadeConferido
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDO", PersistenceParameterType.Key)]
        public uint IdPedido { get; set; }

        [Log("Funcionário Conferência", "Nome", typeof(FuncionarioDAO))]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        [PersistenceProperty("IDFUNCCONF")]
        public uint IdFuncConf { get; set; }

        [Log("Funcionário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Data Pedido")]
        [PersistenceProperty("DATAESPELHO")]
        public DateTime DataEspelho { get; set; }

        [Log("Total")]
        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [Log("Peso")]
        [PersistenceProperty("PESO", DirectionParameter.Input)]
        public float Peso { get; set; }

        [Log("Total M²")]
        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [Log("Valor do ICMS")]
        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [Log("Aliquota de ICMS")]
        [PersistenceProperty("ALIQUOTAICMS")]
        public float AliquotaIcms { get; set; }

        [Log("Aliquota de IPI")]
        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliquotaIpi { get; set; }

        [Log("Valor IPI")]
        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [Log("Data da Conferência")]
        [PersistenceProperty("DATACONF")]
        public DateTime? DataConf { get; set; }

        [Log("Data Fábrica", "DataFabrica", typeof(PedidoEspelhoDAO))]
        [PersistenceProperty("DATAFABRICA")]
        public DateTime? DataFabrica { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        private int _tipoDesconto;

        [Log("Tipo Desconto")]
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

        [Log("Acrescimo")]
        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        private int _tipoAcrescimo;

        [Log("Tipo do Acrescimo")]
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

        [Log("Obs.")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Percentual de Comissão")]
        [PersistenceProperty("PERCCOMISSAO")]
        public float PercComissao { get; set; }

        [Log("Valor da Comissão")]
        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("IDCOMISSIONADO")]
        public uint? IdComissionado { get; set; }

        [PersistenceProperty("SITUACAOCNC")]
        public int SituacaoCnc { get; set; }

        [Log("Data projeto CNC")]
        [PersistenceProperty("DATAPROJETOCNC")]
        public DateTime DataProjetoCnc { get; set; }
        
        [PersistenceProperty("USUPROJETOCNC")]
        public uint UsuProjetoCnc { get; set; }

        [PersistenceProperty("ValorEntrega")]
        public decimal ValorEntrega { get; set; }

        /// <summary>
        /// Define se o pedido IMPORTADO já foi conferido
        /// </summary>
        [Log("Pedido Conferido")]
        [PersistenceProperty("PedidoConferido")]
        public bool PedidoConferido { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE", Direction = DirectionParameter.OutputOnlyInsert)]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA", Direction = DirectionParameter.OutputOnlyInsert)]
        public decimal RentabilidadeFinanceira { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("IDPROJETO", DirectionParameter.InputOptional)]
        public uint? IdProjeto { get; set; }

        [PersistenceProperty("IDCLI", DirectionParameter.InputOptional)]
        public uint IdCli { get; set; }

        [PersistenceProperty("TIPOVENDA", DirectionParameter.InputOptional)]
        public int TipoVenda { get; set; }

        [PersistenceProperty("IDFORMAPAGTO", DirectionParameter.InputOptional)]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("DATAENTREGA", DirectionParameter.InputOptional)]
        public DateTime? DataEntrega { get; set; }

        [PersistenceProperty("DATAENTREGAORIGINAL", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaOriginal { get; set; }

        [PersistenceProperty("TIPOENTREGA", DirectionParameter.InputOptional)]
        public int TipoEntrega { get; set; }

        [PersistenceProperty("IDSINAL", DirectionParameter.InputOptional)]
        public uint? IdSinal { get; set; }

        [PersistenceProperty("VALORENTRADA", DirectionParameter.InputOptional)]
        public decimal ValorEntrada { get; set; }

        [PersistenceProperty("TOTALPEDIDO", DirectionParameter.InputOptional)]
        public decimal TotalPedido { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCli { get; set; }

        public string NomeInicialCli
        {
            get { return IdCli + " - " + BibliotecaTexto.GetThreeFirstWords(NomeCli); }
        }

        private string _nomeFunc;

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return !string.IsNullOrEmpty(_nomeFunc) ? _nomeFunc.Split(' ')[0] : String.Empty; }
            set { _nomeFunc = value; }
        }

        private string _conferente;

        [PersistenceProperty("CONFERENTE", DirectionParameter.InputOptional)]
        public string Conferente
        {
            get 
            {
                return BibliotecaTexto.GetTwoFirstNames(_conferente);
            }
            set { _conferente = value; }
        }

        private string _conferenteFin;

        [PersistenceProperty("CONFERENTEFIN", DirectionParameter.InputOptional)]
        public string ConferenteFin
        {
            get { return BibliotecaTexto.GetFirstName(_conferenteFin); }
            set { _conferenteFin = value; }
        }

        [PersistenceProperty("IDLOJA", DirectionParameter.InputOptional)]
        public ulong IdLoja { get; set; }

        private string _nomeLoja;

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja : String.Empty; }
            set { _nomeLoja = value; }
        }

        private string _formaPagto;

        [PersistenceProperty("FormaPagto", DirectionParameter.InputOptional)]
        public string FormaPagto
        {
            get { return _formaPagto != null ? _formaPagto : String.Empty; }
            set { _formaPagto = value; }
        }

        [PersistenceProperty("CliRevenda", DirectionParameter.InputOptional)]
        public bool CliRevenda { get; set; }

        [PersistenceProperty("QTDEPECAS", DirectionParameter.InputOptional)]
        public long QtdePecas { get; set; }

        [PersistenceProperty("FASTDELIVERY", DirectionParameter.InputOptional)]
        public bool FastDelivery { get; set; }

        [PersistenceProperty("IdItensProjeto", DirectionParameter.InputOptional)]
        public string IdItensProjeto { get; set; }

        [PersistenceProperty("TemProdutosComprar", DirectionParameter.InputOptional)]
        public bool TemProdutosComprar { get; set; }

        [PersistenceProperty("NomeComissionado", DirectionParameter.InputOptional)]
        public string NomeComissionado { get; set; }

        [PersistenceProperty("CompraGerada", DirectionParameter.InputOptional)]
        public string CompraGerada { get; set; }

        [PersistenceProperty("GerouCompra", DirectionParameter.InputOptional)]
        public bool GerouCompra { get; set; }

        [PersistenceProperty("TipoPedido", DirectionParameter.InputOptional)]
        public Pedido.TipoPedidoEnum? TipoPedido { get; set; }

        [PersistenceProperty("ProdutosBenefCompra", DirectionParameter.InputOptional)]
        public IList<ProdutosPedidoEspelho> ProdutosBenefCompra { get; set; }

        [PersistenceProperty("Importado", DirectionParameter.InputOptional)]
        public bool Importado { get; set; }

        [PersistenceProperty("GeradoParceiro", DirectionParameter.InputOptional)]
        public bool GeradoParceiro { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool RecebeuSinal
        {
            get { return IdSinal > 0; }
        }

        public string DataEntregaExibicao
        {
            get
            {
                return Conversoes.ConverteData(DataEntrega, false) + (DataEntrega != DataEntregaOriginal && DataEntregaOriginal != null ? " (" +
                    Conversoes.ConverteData(DataEntregaOriginal, false) + ")" : "");
            }
        }

        public string IdPedidoExibir
        {
            get { return (GeradoParceiro || Importado ? "W" : "") + IdPedido; }
        }

        public System.Drawing.Color CorLinhaLista
        {
            get
            {
                if (GeradoParceiro || Importado)
                    return System.Drawing.Color.FromName(PedidoConfig.TelaListagem.CorLinhaSeImportadoOuGeradoParceiro);

                return System.Drawing.Color.Black;
            }
        }

        public string ConfirmouRecebeuSinal { get; set; }

        [Log("Tipo Venda")]
        public string DescrTipoVenda
        {
            get
            {
                return TipoVenda == 1 ? "À Vista" :
                    TipoVenda == 2 ? "À Prazo" :
                    TipoVenda == 3 ? "Reposição" :
                    TipoVenda == 4 ? "Garantia" :
                    String.Empty;
            }
        }

        [Log("Tipo Entrega")]
        public string DescrTipoEntrega
        {
            get
            {
                switch (TipoEntrega)
                {
                    case 1:
                        return "Balcão";
                    case 2:
                        return "Colocação Comum";
                    case 3:
                        return "Colocação Temperado";
                    case 4:
                        return "Entrega";
                    case 5:
                        return "Manutenção Temperado";
                    case 6:
                        return "Colocação Esquadria";
                    default:
                        return String.Empty;
                }
            }
        }

        [Log("Situação", "Situacao", typeof(PedidoEspelhoDAO))]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoPedido.Processando:
                        return "Processando";
                    case (int)SituacaoPedido.Aberto:
                        return "Aberto";
                    case (int)SituacaoPedido.Finalizado:
                        return "Finalizado";
                    case (int)SituacaoPedido.Impresso:
                        return "Impresso";
                    case (int)SituacaoPedido.ImpressoComum:
                        return "Impresso Comum";
                    case (int)SituacaoPedido.Cancelado:
                        return "Cancelado";
                    default:
                        return String.Empty;
                }
            }
        }

        public bool CancelarVisible
        {
            get 
            {
                bool flag = ((PedidoConfig.LiberarPedido || Situacao == (int)SituacaoPedido.Processando) && !PedidoEspelhoDAO.Instance.IsPedidoImpresso(null, IdPedido) &&
                    (Situacao == (int)SituacaoPedido.Aberto || Situacao == (int)SituacaoPedido.Processando) &&
                    Config.PossuiPermissao(Config.FuncaoMenuPCP.EditarCancelarConferencia)) || (ExibirReabrir && PedidoEspelhoDAO.Instance.PossuiProdutosComposicao(null, (int)IdPedido));

                return flag;
            }
        }

        // Controla visibilidade da opcao editar na grid
        public bool EditVisible
        {
            get
            {
                bool flagSituacao;
                bool flagImpressoComum;
                bool moduloPcp;

                // Apenas Aberto
                flagSituacao = Situacao == (int)SituacaoPedido.Aberto;

                // Se a situação for impresso comum, o usuário poderá editar desde que não seja Aux. Adm. Etiqueta
                flagImpressoComum = Situacao == (int)SituacaoPedido.ImpressoComum && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.AuxEtiqueta;

                // Verifica se o usuário possui acesso ao módulo PCP
                moduloPcp = Config.PossuiPermissao(Config.FuncaoMenuPCP.EditarCancelarConferencia);

                return (flagSituacao || flagImpressoComum) && moduloPcp;
            }
        }

        // Exibe apenas para os pedidos de venda
        public bool ComprarVisible
        {
            get
            {
                bool flagSituacao;
                bool flagTipoPedido;

                // Apenas pedidos finalizados/impressos
                flagSituacao = Situacao == (int)SituacaoPedido.Finalizado || Situacao == (int)SituacaoPedido.Impresso;

                // Apenas pedidos de venda
                flagTipoPedido = PedidoDAO.Instance.IsVenda(IdPedido);

                return flagSituacao && flagTipoPedido && TemProdutosComprar;
            }
        }

        /// <summary>
        /// Retorna o valor da conferência - valor do pedido, valor este que será utilizado para gerar ou não contas a receber
        /// </summary>
        public decimal Diferenca
        {
            get { return Total - TotalPedido; }
        }

        [Log("Conferêcia")]
        public string ResponsavelConferecia
        {
            get { return IdFuncFin > 0 && IdFuncFin != IdFuncConf ? Conferente + " (" + ConferenteFin + ")" : Conferente; }
        }

        public bool UsarControleReposicao
        {
            get { return (TipoVenda == 3) && PCPConfig.ControlarProducao && 
                PedidoReposicaoDAO.Instance.IsPedidoReposicao(IdPedido); }
        }

        public bool ExibirReabrir
        {
            get 
            {
                if (PCPConfig.ReabrirPCPSomenteAdmin && !UserInfo.GetUserInfo.IsAdministrador)
                    return false;

                bool pedidoLiberado = new List<Pedido.SituacaoPedido> { Pedido.SituacaoPedido.Confirmado, 
                    Pedido.SituacaoPedido.LiberadoParcialmente }.Contains(PedidoDAO.Instance.ObtemSituacao(IdPedido));

                // Se o pedido estiver liberado não pode ser aberto, pois caso os clones sejam apagados por alterações nos produtos,
                // os produtos liberados perderão referência
                if (PedidoConfig.LiberarPedido && pedidoLiberado && PedidoDAO.Instance.GetTipoPedido(IdPedido) != Pedido.TipoPedidoEnum.Producao)
                     return false;

                //Valida se o pedido ja tem OC se tiver não pode reabrir o espelho
                if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(IdPedido))
                    return false;

                //Se usar o controle de gerenciamento de projeto cnc e o pedido ja tiver sido projetado não pode reabrir
                //o pedido espelho.
                if (PCPConfig.UsarControleGerenciamentoProjCnc && SituacaoCnc == (int)SituacaoCncEnum.Projetado)
                    return false;

                // A opção de reabrir pedido impresso não pode existir, pois caso o pedido tenha algum projeto,
                // ao recalcular ou confirmar o projeto, os produtos do pedido irão trocar de posição, alterando os números das etiquetas.
                return Situacao == (int)SituacaoPedido.Finalizado
                                                                                                        ;
            }
        }

        public bool ExibirRelatorioPedido
        {
            get { return Situacao != (int)SituacaoPedido.Processando; }
        }

        public bool ExibirRelatorioCalculo
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuPedido.VisualizarMemoriaCalculo);
            }
        }

        public bool ExibirImpressaoProjeto
        {
            /* Chamado 22367.
             * O pedido foi gerado a partir de um projeto, porém, o projeto foi excluso do pedido.
             * A prancheta deve ser exibida somente se o pedido possuir item de projeto. */
            //get { return Situacao != (int)SituacaoPedido.Processando && (!string.IsNullOrEmpty(IdItensProjeto) || IdProjeto > 0); }
            get { return Situacao != (int)SituacaoPedido.Processando && !string.IsNullOrEmpty(IdItensProjeto); }
        }

        public decimal TotalSemDesconto
        {
            get { return PedidoEspelhoDAO.Instance.GetTotalSemDesconto(IdPedido, Total); }
        }

        public decimal TotalSemAcrescimo
        {
            get { return PedidoEspelhoDAO.Instance.GetTotalSemAcrescimo(IdPedido, Total); }
        }

        public decimal TotalBruto
        {
            get { return PedidoEspelhoDAO.Instance.GetTotalBruto(IdPedido); }
        }

        public decimal DescontoTotal
        {
            get { return PedidoEspelhoDAO.Instance.GetDescontoPedido(IdPedido) + PedidoEspelhoDAO.Instance.GetDescontoProdutos(IdPedido); }
        }

        public decimal AcrescimoTotal
        {
            get { return PedidoEspelhoDAO.Instance.GetAcrescimoPedido(IdPedido) + PedidoEspelhoDAO.Instance.GetAcrescimoProdutos(IdPedido); }
        }
        
        public bool DescontoEnabled
        {
            get
            {
                return true;
            }
        }

        public string TextoDescontoTotalPerc
        {
            get { return Pedido.GetTextoPerc(2, DescontoTotal, TotalSemDesconto); }
        }

        public string TextoAcrescimoTotalPerc
        {
            get
            {
                return Pedido.GetTextoPerc(2, PedidoEspelhoDAO.Instance.GetAcrescimoPedido(IdPedido) + 
                    PedidoEspelhoDAO.Instance.GetAcrescimoProdutos(IdPedido), TotalSemAcrescimo);
            }
        }

        [Log("Situação projeto CNC")]
        public string DescrSituacaoCnc
        {
            get
            {
                switch (SituacaoCnc)
                {
                    case (int)SituacaoCncEnum.SemNecessidadeNaoConferido:
                        return "Sem necessidade (Não conferido)";
                    case (int)SituacaoCncEnum.NaoProjetado:
                        return "Não projetado";
                    case (int)SituacaoCncEnum.Projetado:
                        return "Projetado";
                    case (int)SituacaoCncEnum.SemNecessidadeConferido:
                        return "Sem necessidade (Conferido)";
                    default:
                        return "";
                }
            }
        }

        public bool ExibirSituacaoCnc
        {
            get
            {
                return SituacaoCnc == (int)SituacaoCncEnum.Projetado || SituacaoCnc == (int)SituacaoCncEnum.NaoProjetado;
            }
        }

        public bool ExibirSituacaoCncConferencia
        {
            get
            {
                return SituacaoCnc == (int)SituacaoCncEnum.SemNecessidadeNaoConferido || SituacaoCnc == (int)SituacaoCncEnum.SemNecessidadeConferido;
            }
        }

        [Log("Usuário projeto CNC")]
        public string IdNomeUsuProjCnc
        {
            get
            {
                return UsuProjetoCnc + " - " + FuncionarioDAO.Instance.GetNome(UsuProjetoCnc);
            }
        }

        [Log("Funcionário Conferência")]
        public string NomeFuncConferencia
        {
            get
            {
                return FuncionarioDAO.Instance.GetNome(IdFuncConf);
            }
        }

        public bool TemProdutoLamComposicao
        {
            get
            {
                return ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(IdPedido);
            }
        }

        /// <summary>
        /// Define se é possivel marcar ou não como conferido
        /// </summary>
        public bool ConferirPedidoVisible
        {
            get { return PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos && Importado && Situacao == (int)PedidoEspelho.SituacaoPedido.Finalizado; }
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
                        () => AmbientePedidoEspelhoDAO.Instance.GetByPedido(IdPedido)
                    );
                }

                return ambientes;
            }
        }

        private Lazy<uint?> idObra;

        uint? IContainerCalculo.IdObra
        {
            get
            {
                if (idObra == null)
                {
                    idObra = new Lazy<uint?>(() => PedidoDAO.Instance.GetIdObra(IdPedido));
                }

                return idObra.Value;
            }
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
            get { return TipoPedido == Pedido.TipoPedidoEnum.MaoDeObra; }
        }

        private Lazy<bool> isPedidoProducaoCorte;

        bool IContainerCalculo.IsPedidoProducaoCorte
        {
            get
            {
                if (isPedidoProducaoCorte == null)
                {
                    isPedidoProducaoCorte = new Lazy<bool>(() => {
                        return TipoPedido == Pedido.TipoPedidoEnum.Producao
                            && PedidoDAO.Instance.ObtemValorCampo<uint?>("idPedidoRevenda", "idPedido=" + IdPedido).HasValue;
                    });
                }

                return isPedidoProducaoCorte.Value;
            }
        }

        private Lazy<uint?> idParcela;

        uint? IContainerCalculo.IdParcela
        {
            get
            {
                if (idParcela == null)
                {
                    idParcela = new Lazy<uint?>(() => PedidoDAO.Instance.ObtemIdParcela(IdPedido));
                }

                return idParcela.Value;
            }
        }

        #endregion
    }
}