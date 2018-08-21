using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.EFD;
using Glass.Data.Helper;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Poss�veis tipos de mercadorias.
    /// </summary>
    public enum TipoMercadoria
    {
        /// <summary>
        /// Mercador Revenda.
        /// </summary>
        [Description("Mercadoria Revenda")]
        MercadoriaRevenda,
        /// <summary>
        /// Mat�ria Prima.
        /// </summary>
        [Description("Mat�ria Prima")]
        MateriaPrima,
        /// <summary>
        /// Embalagem.
        /// </summary>
        [Description("Embalagem")]
        Embalagem,
        /// <summary>
        /// Produto em Processo.
        /// </summary>
        [Description("Produto em Processo")]
        ProdutoEmProcesso,
        /// <summary>
        /// Produto acabado.
        /// </summary>
        [Description("Produto Acabado")]
        ProdutoAcabado,
        /// <summary>
        /// Subproduto.
        /// </summary>
        [Description("Subproduto")]
        Subproduto,
        /// <summary>
        /// Produto Intermedi�rio.
        /// </summary>
        [Description("Produto Intermedi�rio")]
        ProdutoIntermediario,
        /// <summary>
        /// Material de uso e consumo.
        /// </summary>
        [Description("Material de Uso e Consumo")]
        MaterialUsoConsumo,
        /// <summary>
        /// Ativo imobilizado.
        /// </summary>
        [Description("Ativo Imobilizado")]
        AtivoImobilizado,
        /// <summary>
        /// Servi�os.
        /// </summary>
        [Description("Servi�os")]
        Servicos,
        /// <summary>
        /// Outros insumos.
        /// </summary>
        [Description("Outros Insumos")]
        OutrosInsumos,
        /// <summary>
        /// Outros.
        /// </summary>
        [Description("Outras")]
        Outras = 99
    }

    /// <summary>
    /// Poss�veis c�digos ds situa��o tribut�ria do Ipi do produto.
    /// </summary>
    public enum ProdutoCstIpi
    {
        /// <summary>
        /// Entrada recuperando cr�dito.
        /// </summary>
        [Description("Entrada recuperando cr�dito")]
        EntradaRecuperandoCredito,
        /// <summary>
        /// Entrada Tributada Al�quota Zero.
        /// </summary>
        [Description("Entrada tributada com al�quota zero")]
        EntradaTributadaAliquotaZero,
        /// <summary>
        /// Entrada Isenta
        /// </summary>
        [Description("Entrada isenta")]
        EntradaIsenta,
        /// <summary>
        /// Entrada n�o Tributada.
        /// </summary>
        [Description("Entrada n�o-tributada")]
        EntradaNaoTributada,
        /// <summary>
        /// Entrada Imenu.
        /// </summary>
        [Description("Entrada Imune")]
        EntradaImune,
        /// <summary>
        /// Entrada com suspen��o.
        /// </summary>
        [Description("Entrada com suspens�o")]
        EntradaComSuspensao,
        /// <summary>
        /// Outras entradas.
        /// </summary>
        [Description("Outras Entradas")]
        OutrasEntradas = 49,
        /// <summary>
        /// Sa�da Tributada.
        /// </summary>
        [Description("Sa�da Tributada")]
        SaidaTributada,
        /// <summary>
        /// Sa�da Tributada Al�quota Zero.
        /// </summary>
        [Description("Sa�da tributada com al�quota zero")]
        SaidaTributadaAliquotaZero,
        /// <summary>
        /// Sa�da Isenta
        /// </summary>
        [Description("Sa�da Isenta")]
        SaidaIsenta,
        /// <summary>
        /// Sa�da n�o Tributada.
        /// </summary>
        [Description("Sa�da n�o-tributada")]
        SaidaNaoTributada,
        /// <summary>
        /// Sa�da Imune.
        /// </summary>
        [Description("Sa�da Imune")]
        SaidaImune,
        /// <summary>
        /// Sa�da com Suspen��o.
        /// </summary>
        [Description("Sa�da com suspens�o")]
        SaidaComSuspensao,
        /// <summary>
        /// Outras Sa�das.
        /// </summary>
        [Description("Outras Sa�das")]
        OutrasSaidas = 99
    }

    [PersistenceBaseDAO(typeof(ProdutoDAO))]
    [PersistenceClass("produto")]
    [Colosoft.Data.Schema.Cache]
    public class Produto : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IProduto, IProdutoIcmsSt
    {
        #region Propriedades

        [PersistenceProperty("IDPROD", PersistenceParameterType.IdentityKey)]
        public int IdProd { get; set; }

        [Log("Fornecedor", "Nome", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        [PersistenceForeignKey(typeof(Fornecedor), "IdFornec")]
        public int IdFornec { get; set; }

        [Log("Subgrupo", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        [PersistenceForeignKey(typeof(SubgrupoProd), "IdSubgrupoProd")]
        public int? IdSubgrupoProd { get; set; }

        [Log("Grupo", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        [PersistenceForeignKey(typeof(GrupoProd), "IdGrupoProd")]
        public int IdGrupoProd { get; set; }

        [Log("Cor do Vidro", "Descricao", typeof(CorVidroDAO))]
        [PersistenceProperty("IDCORVIDRO")]
        [PersistenceForeignKey(typeof(CorVidro), "IdCorVidro")]
        public int? IdCorVidro { get; set; }

        [Log("Cor da Ferragem", "Descricao", typeof(CorFerragemDAO))]
        [PersistenceProperty("IDCORFERRAGEM")]
        [PersistenceForeignKey(typeof(CorFerragem), "IdCorFerragem")]
        public int? IdCorFerragem { get; set; }

        [Log("Cor do Alum�nio", "Descricao", typeof(CorAluminioDAO))]
        [PersistenceProperty("IDCORALUMINIO")]
        [PersistenceForeignKey(typeof(CorAluminio), "IdCorAluminio")]
        public int? IdCorAluminio { get; set; }

        [Log("Unidade de Medida", "Codigo", typeof(UnidadeMedidaDAO))]
        [PersistenceProperty("IDUNIDADEMEDIDA")]
        [PersistenceForeignKey(typeof(UnidadeMedida), "IdUnidadeMedida", 1)]
        public int IdUnidadeMedida { get; set; }

        [Log("Unidade de Medida Trib.", "Codigo", typeof(UnidadeMedidaDAO))]
        [PersistenceProperty("IDUNIDADEMEDIDATRIB")]
        [PersistenceForeignKey(typeof(UnidadeMedida), "IdUnidadeMedida", 2)]
        public int IdUnidadeMedidaTrib { get; set; }

        [Log("Genero Produto", "Codigo", typeof(GeneroProdutoDAO))]
        [PersistenceProperty("IDGENEROPRODUTO")]
        [PersistenceForeignKey(typeof(GeneroProduto), "IdGeneroProduto")]
        public int? IdGeneroProduto { get; set; }

        [PersistenceProperty("IDARQUIVOMESACORTE")]
        [PersistenceForeignKey(typeof(ArquivoMesaCorte), "IdArquivoMesaCorte")]
        public int? IdArquivoMesaCorte { get; set; }

        [Log("Tipo Arquivo")]
        [PersistenceProperty("TIPOARQUIVO")]
        public TipoArquivoMesaCorte? TipoArquivo { get; set; }

        [Log("Tipo Mercadoria", "Descr", typeof(DataSourcesEFD), "Id", "GetTipoMercadoria", true)]
        [PersistenceProperty("TIPOMERCADORIA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public TipoMercadoria? TipoMercadoria { get; set; }

        private string _codInterno;

        [Log("C�d. Produto")]
        [PersistenceProperty("CODINTERNO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string CodInterno
        {
            get { return !String.IsNullOrEmpty(_codInterno) ? _codInterno.ToUpper() : _codInterno; }
            set { _codInterno = !String.IsNullOrEmpty(value) ? value.ToUpper() : value; }
        }

        [Log("C�d. EX")]
        [PersistenceProperty("CODIGOEX")]
        public string CodigoEX { get; set; }

        [Log("GTIN Produto")]
        [PersistenceProperty("GTINPRODUTO")]
        public string GTINProduto { get; set; }

        [Log("GTIN Unid. Trib.")]
        [PersistenceProperty("GTINUNIDTRIB")]
        public string GTINUnidTrib { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public Glass.Situacao Situacao { get; set; }

        private string _descricao;

        [Log("Descri��o", true)]
        [PersistenceProperty("DESCRICAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Descricao
        {
            get { return _descricao != null ? _descricao.ToUpper().Replace("\t", "") : _descricao; }
            set { _descricao = !String.IsNullOrEmpty(value) ? value.ToUpper() : value; }
        }

        [Log("Custo Fab. Base")]
        [PersistenceProperty("CUSTOFABBASE")]
        public decimal Custofabbase { get; set; }

        [Log("Custo Compra")]
        [PersistenceProperty("CUSTOCOMPRA")]
        public decimal CustoCompra { get; set; }

        [Log("Valor Atacado")]
        [PersistenceProperty("VALORATACADO")]
        public decimal ValorAtacado { get; set; }

        [Log("Valor Balc�o")]
        [PersistenceProperty("VALORBALCAO")]
        public decimal ValorBalcao { get; set; }

        [Log("Valor Obra")]
        [PersistenceProperty("VALOROBRA")]
        public decimal ValorObra { get; set; }

        [Log("Valor Reposi��o")]
        [PersistenceProperty("VALORREPOSICAO")]
        public decimal ValorReposicao { get; set; }

        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [PersistenceProperty("UNIDADETRIB", DirectionParameter.InputOptional)]
        public string UnidadeTrib { get; set; }

        [Log("Valor M�nimo")]
        [PersistenceProperty("Valor_Minimo")]
        public decimal ValorMinimo { get; set; }

        [Log("Valor de Transfer�ncia")]
        [PersistenceProperty("VALORTRANSFERENCIA")]
        public decimal ValorTransferencia { get; set; }

        /// <summary>
        /// 00 - Tributada integralmente
        /// 10 - Tributada e com cobran�a de ICMS por substitui��o tribut�ria
        /// 20 - Com redu��o de base de c�lculo
        /// 30 - Isenta ou n�o tributada e com cobran�a de ICMS por substitui��o tribut�ria
        /// 40 - Isenta
        /// 41 - N�o tributada
        /// 50 - Suspens�o
        /// 51 - Diferimento. A exig�ncia do preenchimento das informa��es do ICMS diferido fica � crit�rio de cada UF.
        /// 60 - ICMS cobrado anteriormente por substitui��o tribut�ria
        /// 70 - Com redu��o de base de c�lculo e cobran�a do ICMS por substitui��o tribut�ria
        /// 90 - Outros
        /// </summary>
        [Log("CST")]
        [PersistenceProperty("CST")]
        public string Cst { get; set; }

        [Log("CSOSN")]
        [PersistenceProperty("CSOSN")]
        public string Csosn { get; set; }

        [Log("NCM")]
        [PersistenceProperty("NCM")]
        public string Ncm { get; set; }

        [Log("Al�quota IPI")]
        [PersistenceProperty("AliqIPI")]
        public float AliqIPI { get; set; }

        [Log("C�d. Otimiza��o")]
        [PersistenceProperty("CODOTIMIZACAO")]
        public string CodOtimizacao { get; set; }

        [Log("Ativar M�nimo")]
        [PersistenceProperty("AtivarMin")]
        public bool AtivarMin { get; set; }

        [Log("Espessura")]
        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [Log("Peso")]
        [PersistenceProperty("PESO")]
        public float Peso { get; set; }

        [Log("�rea M�nima")]
        [PersistenceProperty("AREAMINIMA")]
        public float AreaMinima { get; set; }

        [Log("Ativar �rea M�nima")]
        [PersistenceProperty("ATIVARAREAMINIMA")]
        public bool AtivarAreaMinima { get; set; }

        [Log("Compra")]
        [PersistenceProperty("COMPRA")]
        public bool Compra { get; set; }

        [Log("Item gen�rico")]
        [PersistenceProperty("ITEMGENERICO")]
        public bool ItemGenerico { get; set; }

        [PersistenceProperty("DATAALT")]
        [Colosoft.Data.Schema.CacheIndexed]
        public DateTime? DataAlt { get; set; }

        [PersistenceProperty("USUALT")]
        [Colosoft.Data.Schema.CacheIndexed]
        public int? UsuAlt { get; set; }

        [Log("Observa��o")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Altura")]
        [PersistenceProperty("ALTURA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public int? Altura { get; set; }

        [Log("Largura")]
        [PersistenceProperty("LARGURA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public int? Largura { get; set; }

        [Log("Redondo")]
        [PersistenceProperty("REDONDO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public bool Redondo { get; set; }

        [Log("Forma")]
        [PersistenceProperty("FORMA")]
        public string Forma { get; set; }

        [Log("CST IPI", "Descr", typeof(DataSourcesEFD), "Id", "GetCstIpi", true)]
        [PersistenceProperty("CSTIPI")]
        public ProdutoCstIpi? CstIpi { get; set; }

        [Log("Plano de Conta Cont�bil", "Descricao", typeof(PlanoContaContabilDAO))]
        [PersistenceProperty("IDCONTACONTABIL")]
        [PersistenceForeignKey(typeof(PlanoContaContabil), "IdContaContabil")]
        public int? IdContaContabil { get; set; }

        [Log("Local de Armazenagem")]
        [PersistenceProperty("LOCALARMAZENAGEM")]
        public string LocalArmazenagem { get; set; }

        [Log("Processo", "CodInterno", typeof(EtiquetaProcessoDAO))]
        [PersistenceProperty("IDPROCESSO")]
        [PersistenceForeignKey(typeof(EtiquetaProcesso), "IdProcesso")]
        public int? IdProcesso { get; set; }

        [Log("Aplica��o", "CodInterno", typeof(EtiquetaAplicacaoDAO))]
        [PersistenceProperty("IDAPLICACAO")]
        [PersistenceForeignKey(typeof(EtiquetaAplicacao), "IdAplicacao")]
        public int? IdAplicacao { get; set; }

        [Log("Valor Fiscal")]
        [PersistenceProperty("VALORFISCAL")]
        public decimal ValorFiscal { get; set; }

        [PersistenceProperty("IDPRODORIG")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", 3)]
        public int? IdProdOrig { get; set; }

        [Log("Produto Base", "CodInterno", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPRODBASE")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", 4)]
        public int? IdProdBase { get; set; }

        [Log("CEST", "Codigo", typeof(CestDAO))]
        [PersistenceProperty("IdCest")]
        [PersistenceForeignKey(typeof(Cest), "IdCest")]
        public int? IdCest { get; set; }

        [PersistenceProperty("MarkUp")]
        public decimal MarkUp { get; set; }

        [PersistenceProperty("MVAPRODUTONF", DirectionParameter.InputOptional)]
        public float MvaProdutoNf { get; set; }

        [PersistenceProperty("CodigoCest", DirectionParameter.InputOptional)]
        public string CodigoCest { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia da margem inferior da chapa.
        /// </summary>
        [Log("Recorte X1")]
        [PersistenceProperty("RecorteX1")]
        public double RecorteX1 { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia da margem esquerda da chapa.
        /// </summary>
        [Log("Recorte Y1")]
        [PersistenceProperty("RecorteY1")]
        public double RecorteY1 { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia da margem superior da chapa.
        /// </summary>
        [Log("Recorte X2")]
        [PersistenceProperty("RecorteX2")]
        public double RecorteX2 { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia da margem direita da chapa.
        /// </summary>
        [Log("Recorte Y2")]
        [PersistenceProperty("RecorteY2")]
        public double RecorteY2 { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia m�xima de lado X da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        [Log("Transversal M�xima X")]
        [PersistenceProperty("TransversalMaxX")]
        public double TransversalMaxX { get; set; } = 9999.0;

        /// <summary>
        /// Obt�m ou define a dist�ncia m�xima de lado Y da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        [Log("Transversal M�xima Y")]
        [PersistenceProperty("TransversalMaxY")]
        public double TransversalMaxY { get; set; } = 9999.0;

        /// <summary>
        /// Obt�m ou define a dimens�o m�nima em X da superf�cie de desperd�cio
        /// geradas pelo programa de otimiza��o que, ao serem suficientemente 
        /// grandes, se podem considerar reutiliz�veis e � desej�vel introduzi-las
        /// de novo no estoque para otimiza��es posteriores
        /// </summary>
        [Log("Desperd�cio M�nimo X")]
        [PersistenceProperty("DesperdicioMinX")]
        public double DesperdicioMinX { get; set; }

        /// <summary>
        /// Obt�m ou define a dimens�o m�nima em Y da superf�cie de desperd�cio
        /// geradas pelo programa de otimiza��o que, ao serem suficientemente 
        /// grandes, se podem considerar reutiliz�veis e � desej�vel introduzi-las
        /// de novo no estoque para otimiza��es posteriores
        /// </summary>
        [Log("Desperd�cio M�nimo Y")]
        [PersistenceProperty("DesperdicioMinY")]
        public double DesperdicioMinY { get; set; }

        /// <summary>
        /// Obt�m ou define a dist�ncia minima aceitavel durante a otimiza��o 
        /// entre dois cortes paralelos, com o intuito de facilitar ou tornar 
        /// poss�vel a abertura dos cortes.
        /// </summary>
        /// <example>
        /// Ao configurar o valor em 20mm, ser� imposs�vel encontrar no interior
        /// de um plano de corte duas pe�as ou dois cortes pr�ximos um do outro,
        /// de dist�ncia inferior � anteriormente introduzida (20mm).
        /// </example>
        /// <remarks>
        /// Evidentemente, esta dist�ncia n�o � tida em conta nos casos em que 
        /// 2 pe�as compartilham o mesmo corte.
        /// </remarks>
        [Log("Dist�ncia M�nima")]
        [PersistenceProperty("DistanciaMin")]
        public double DistanciaMin { get; set; }

        /// <summary>
        /// Obt�m ou define a configura��o do valor de recorte que deve
        /// introduzir-se nas formas no caso de esta conter �ngulos 
        /// inferiores ao configurado no campo "AnguloRecorteAutomatico"
        /// </summary>
        [Log("Recorte Autom�tico da Forma")]
        [PersistenceProperty("RecorteAutomaticoForma")]
        public double RecorteAutomaticoForma { get; set; }

        /// <summary>
        /// Obt�m ou define o valor do �ngulo ao qual o recorte deve
        /// ser introduzido de forma autom�tica.
        /// </summary>
        [Log("�ngulo de Recorte Autom�tico")]
        [PersistenceProperty("AnguloRecorteAutomatico")]
        public double AnguloRecorteAutomatico { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("IDSCOMPRAS", DirectionParameter.InputOptional)]
        public string IdsCompras { get; set; }

        public string DescrTipoMercadoria
        {
            get
            {
                return TipoMercadoria != null ? DataSourcesEFD.Instance.GetDescrTipoMercadoria((int)TipoMercadoria) : "";
            }
        }

        public string DescrCstIpi { get { return Colosoft.Translator.Translate(CstIpi).Format(); } }

        [PersistenceProperty("DescrContaContabil", DirectionParameter.InputOptional)]
        public string DescrContaContabil { get; set; }

        [PersistenceProperty("DescrTipoProduto", DirectionParameter.InputOptional)]
        public string DescrTipoProduto { get; set; }

        [PersistenceProperty("NomeFornecedor", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        [PersistenceProperty("TotalCusto", DirectionParameter.InputOptional)]
        public decimal TotalCusto { get; set; }

        [PersistenceProperty("TotalVend", DirectionParameter.InputOptional)]
        public decimal TotalVend { get; set; }

        [PersistenceProperty("DataFiltro", DirectionParameter.InputOptional)]
        public DateTime? DataFiltro { get; set; }

        [PersistenceProperty("TotalQtde", DirectionParameter.InputOptional)]
        public double TotalQtde { get; set; }

        [PersistenceProperty("TotalQtdeLong", DirectionParameter.InputOptional)]
        public long TotalQtdeLong
        {
            get { return (long)TotalQtde; }
            set { TotalQtde = (double)value; }
        }

        [PersistenceProperty("TotalM2", DirectionParameter.InputOptional)]
        public double TotalM2 { get; set; }

        [PersistenceProperty("TotalML", DirectionParameter.InputOptional)]
        public double TotalML { get; set; }

        [PersistenceProperty("QtdeEstoque", DirectionParameter.InputOptional)]
        public double QtdeEstoque { get; set; }

        [PersistenceProperty("QtdeEntrega", DirectionParameter.InputOptional)]
        public double QtdEntrega { get; set; }

        [PersistenceProperty("Liberacao", DirectionParameter.InputOptional)]
        public double Liberacao { get; set; }

        [PersistenceProperty("Reserva", DirectionParameter.InputOptional)]
        public double Reserva { get; set; }

        [PersistenceProperty("M2Estoque", DirectionParameter.InputOptional)]
        public double M2Estoque { get; set; }

        [PersistenceProperty("EstoqueFiscal", DirectionParameter.InputOptional)]
        public double EstoqueFiscal { get; set; }

        [PersistenceProperty("EstoqueMinimo", DirectionParameter.InputOptional)]
        public double EstoqueMinimo { get; set; }

        [PersistenceProperty("M2Entrega", DirectionParameter.InputOptional)]
        public double M2Entrega { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        private string _descrUsuAlt;

        [PersistenceProperty("DescrUsuAlt", DirectionParameter.InputOptional)]
        public string DescrUsuAlt
        {
            get
            {
                try
                {
                    return !String.IsNullOrEmpty(_descrUsuAlt) ? BibliotecaTexto.GetTwoFirstNames(_descrUsuAlt) : _descrUsuAlt;
                }
                catch
                {
                    return _descrUsuAlt;
                }
            }
            set { _descrUsuAlt = value; }
        }

        [PersistenceProperty("DescrParent", DirectionParameter.InputOptional)]
        public string DescrParent { get; set; }

        [PersistenceProperty("CodInternoParent", DirectionParameter.InputOptional)]
        public string CodInternoParent { get; set; }

        [PersistenceProperty("DescrBaixaEstFiscal", DirectionParameter.InputOptional)]
        public string DescrBaixaEstFiscal { get; set; }

        [PersistenceProperty("CodInternoBaixaEstFiscal", DirectionParameter.InputOptional)]
        public string CodInternoBaixaEstFiscal { get; set; }

        [PersistenceProperty("DescrGrupo", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        [PersistenceProperty("DescrSubgrupo", DirectionParameter.InputOptional)]
        public string DescrSubgrupo { get; set; }

        [PersistenceProperty("DescrGeneroProd", DirectionParameter.InputOptional)]
        public string DescrGeneroProd { get; set; }

        [PersistenceProperty("DESCRCOR", DirectionParameter.InputOptional)]
        public string DescrCor { get; set; }

        [PersistenceProperty("ESTOQUEFISCALPERIODO", DirectionParameter.InputOptional)]
        public double EstoqueFiscalPeriodo { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public int NumeroNfe { get; set; }

        [PersistenceProperty("DATANF", DirectionParameter.InputOptional)]
        public DateTime? DataNf { get; set; }

        [PersistenceProperty("TIPODOCUMENTONF", DirectionParameter.InputOptional)]
        public int TipoDocumentoNf { get; set; }

        [PersistenceProperty("IDCLIENTEVEND", DirectionParameter.InputOptional)]
        public uint? IdClienteVend { get; set; }

        [PersistenceProperty("NOMECLIENTEVEND", DirectionParameter.InputOptional)]
        public string NomeClienteVend { get; set; }

        [PersistenceProperty("CLIENTEREVENDAVEND", DirectionParameter.InputOptional)]
        public bool ClienteRevendaVend { get; set; }

        [PersistenceProperty("IDFORNECCOMP", DirectionParameter.InputOptional)]
        public UInt64 IdFornecComp { get; set; }

        [PersistenceProperty("NOMEFORNECCOMP", DirectionParameter.InputOptional)]
        public string NomeFornecComp { get; set; }

        [PersistenceProperty("TOTMCOMPRANDO", DirectionParameter.InputOptional)]
        public double TotMComprando { get; set; }

        [PersistenceProperty("QTDECOMPRANDO", DirectionParameter.InputOptional)]
        public double QtdeComprando { get; set; }

        [PersistenceProperty("TOTMPRODUZINDO", DirectionParameter.InputOptional)]
        public double TotMProduzindo { get; set; }

        [PersistenceProperty("QTDEPRODUZINDO", DirectionParameter.InputOptional)]
        public double QtdeProduzindo { get; set; }

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IdAmbiente", DirectionParameter.InputOptional)]
        public int? IdAmbiente { get; set; }

        [PersistenceProperty("Ambiente", DirectionParameter.InputOptional)]
        public string Ambiente { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("VALORVENDIDO", DirectionParameter.InputOptional)]
        public decimal ValorVendido { get; set; }

        [PersistenceProperty("TOTALM2CHAPA", DirectionParameter.InputOptional)]
        public decimal TotalM2Chapa { get; set; }

        [PersistenceProperty("QTDECHAPA", DirectionParameter.InputOptional)]
        public double QtdeChapa { get; set; }

        [PersistenceProperty("SugestaoCompraMensal", DirectionParameter.InputOptional)]
        public double SugestaoCompraMensal { get; set; }

        [PersistenceProperty("MediaVendaMensal", DirectionParameter.InputOptional)]
        public double MediaVendaMensal { get; set; }

        [PersistenceProperty("IdCfop", DirectionParameter.InputOptional)]
        public int? IdCfop { get; set; }

        [PersistenceProperty("ProdutoNfCst", DirectionParameter.InputOptional)]
        public string ProdutoNfCst { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situa��o")]
        public string DescrSituacao
        {
            get
            {
                return Colosoft.Translator.Translate(Situacao).Format();
            }
        }

        #region Propriedades para a busca de al�quota de ICMS interna

        private int? _idNfIcms;

        internal int? IdNfIcms
        {
            get { return _idNfIcms; }
            set
            {
                _idNfIcms = value;
                _aliqIcmsInterna = null;
                _aliqIcmsInternaComIpiNoCalculo = null;
            }
        }

        private uint _idLojaIcms;

        internal uint IdLojaIcms
        {
            get { return _idLojaIcms; }
            set
            {
                _idLojaIcms = value;
                _aliqIcmsInterna = null;
                _aliqIcmsInternaComIpiNoCalculo = null;
            }
        }

        internal uint? _idFornecIcms;

        internal uint? IdFornecIcms
        {
            get { return _idFornecIcms; }
            set
            {
                _idFornecIcms = value;
                _aliqIcmsInterna = null;
                _aliqIcmsInternaComIpiNoCalculo = null;
            }
        }

        private uint? _idClienteIcms;

        internal uint? IdClienteIcms
        {
            get { return _idClienteIcms; }
            set
            {
                _idClienteIcms = value;
                _aliqIcmsInterna = null;
                _aliqIcmsInternaComIpiNoCalculo = null;
            }
        }

        private bool _saidaIcms;

        internal bool SaidaIcms
        {
            get { return _saidaIcms; }
            set
            {
                _saidaIcms = value;
                _aliqIcmsInterna = null;
                _aliqIcmsInternaComIpiNoCalculo = null;
            }
        }

        #endregion

        private float? _aliqIcmsStInterna;

        public float AliqIcmsStInterna
        {
            get
            {
                if (_aliqIcmsStInterna == null && IdLojaIcms > 0)
                    _aliqIcmsStInterna = IcmsProdutoUfDAO.Instance.ObterAliquotaIcmsSt(null, (uint)IdProd, IdLojaIcms, IdFornecIcms, IdClienteIcms);

                return _aliqIcmsStInterna.GetValueOrDefault();
            }
        }

        private decimal? _aliqIcmsInterna;

        public decimal AliqICMSInterna
        {
            get
            {
                if (_aliqIcmsInterna == null)
                {
                    if (IdLojaIcms == 0)
                    {
                        throw new Exception("Indique a Loja para buscar a al�quota de ICMS interna.");
                    }

                    var lojaCalculaIpi = IdLojaIcms > 0 ? LojaDAO.Instance.ObtemCalculaIpiPedido(null, IdLojaIcms) : false;
                    var clienteCalculaIpi = IdClienteIcms > 0 ? ClienteDAO.Instance.IsCobrarIpi(null, IdClienteIcms.Value) : false;
                    var calcularIpi = false;

                    if (IdNaturezaOperacaoParaAliqICMSInternaComIpiNoCalculo > 0)
                    {
                        calcularIpi = NaturezaOperacaoDAO.Instance.CalculaIpi(null, IdNaturezaOperacaoParaAliqICMSInternaComIpiNoCalculo.Value) && AliqIPI > 0;
                    }
                    else
                    {
                        calcularIpi = lojaCalculaIpi && clienteCalculaIpi && AliqIPI > 0;
                    }

                    _aliqIcmsInterna = (decimal)CalculoIcmsStFactory.ObtemInstancia(null, (int)IdLojaIcms, (int?)IdClienteIcms, (int?)IdFornecIcms, IdCfop, ProdutoNfCst, IdNfIcms, calcularIpi)
                        .ObtemAliquotaInternaIcmsSt(this, SaidaIcms);
                }

                return _aliqIcmsInterna.GetValueOrDefault();
            }
        }

        private decimal? _aliqIcmsInternaComIpiNoCalculo;

        public decimal AliqICMSInternaComIpiNoCalculo
        {
            get
            {
                if (_aliqIcmsInternaComIpiNoCalculo == null)
                {
                    if (IdLojaIcms == 0)
                    {
                        throw new Exception("Indique a Loja para buscar a al�quota de ICMS interna.");
                    }

                    var lojaCalculaIpi = IdLojaIcms > 0 ? LojaDAO.Instance.ObtemCalculaIpiPedido(null, IdLojaIcms) : false;
                    var clienteCalculaIpi = IdClienteIcms > 0 ? ClienteDAO.Instance.IsCobrarIpi(null, IdClienteIcms.Value) : false;
                    var calcularIpi = false;

                    if (IdNaturezaOperacaoParaAliqICMSInternaComIpiNoCalculo > 0)
                    {
                        calcularIpi = NaturezaOperacaoDAO.Instance.CalculaIpi(null, IdNaturezaOperacaoParaAliqICMSInternaComIpiNoCalculo.Value) && AliqIPI > 0;
                    }
                    else
                    {
                        calcularIpi = lojaCalculaIpi && clienteCalculaIpi && AliqIPI > 0;
                    }

                    _aliqIcmsInternaComIpiNoCalculo = (decimal)CalculoIcmsStFactory.ObtemInstancia(null, (int)IdLojaIcms, (int?)IdClienteIcms, (int?)IdFornecIcms, IdCfop, ProdutoNfCst, IdNfIcms, calcularIpi)
                        .ObtemAliquotaInternaIcmsSt(this, SaidaIcms);
                }

                return _aliqIcmsInternaComIpiNoCalculo.GetValueOrDefault();
            }
        }

        internal uint? IdNaturezaOperacaoParaAliqICMSInternaComIpiNoCalculo { get; set; }

        public string DescrEstoque
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd, IdSubgrupoProd);
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(tipoCalc, true);
                return Disponivel.ToString() + descrTipoCalculo;
            }
        }

        public string DescrEntrega
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd, IdSubgrupoProd);
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(tipoCalc, true);
                return QtdEntrega.ToString() + descrTipoCalculo;
            }
        }

        public string DescrEstoqueFiscal
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd, IdSubgrupoProd);
                string estoque = Math.Round(EstoqueFiscal, 2).ToString();
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(tipoCalc, true);
                return estoque + descrTipoCalculo;
            }
        }

        public string DescrEstoqueFiscalPeriodo
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd, IdSubgrupoProd);
                string estoque = Math.Round(EstoqueFiscalPeriodo, 2).ToString();
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(tipoCalc, true);
                return estoque + descrTipoCalculo;
            }
        }

        public decimal Lucro
        {
            get { return TotalVend - TotalCusto; }
        }

        public bool PrecoAnteriorVisible
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Administrador; }
        }

        public string DescrAtacadoRepos
        {
            get { return "Atacado"; }
        }

        public decimal ValorAtacadoRepos
        {
            get { return ValorAtacado; }
            set
            {
                ValorAtacado = value;
            }
        }

        private double Disponivel
        {
            get
            {
                return Math.Round(QtdeEstoque - Reserva - (PedidoConfig.LiberarPedido ? Liberacao : 0), 2);
            }
        }

        public string EstoqueDisponivel
        {
            get
            {
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(TipoCalculo, true);
                return Disponivel + descrTipoCalculo;
            }
        }

        [Log("Arq. Mesa Corte")]
        public string CodigoArquivoMesaCorte
        {
            get
            {
                if (IdArquivoMesaCorte > 0)
                {
                    var idArquivoCalcEngine = ArquivoMesaCorteDAO.Instance.ObtemIdArquivoCalcEngine((uint)IdArquivoMesaCorte.Value);

                    if (idArquivoCalcEngine > 0)
                        return ArquivoCalcEngineDAO.Instance.ObtemNomeArquivo(null, idArquivoCalcEngine);
                }

                return string.Empty;
            }
        }

        public string DescrTipoDocumentoNf
        {
            get
            {
                NotaFiscal nf = new NotaFiscal();
                nf.TipoDocumento = TipoDocumentoNf;
                return nf.TipoDocumentoString;
            }
        }

        public string ImagemUrl
        {
            get { return Utils.GetProdutosVirtualPath + IdProd + ".jpg"; }
        }

        public bool TemImagem
        {
            get { return Utils.ArquivoExiste(ImagemUrl); }
        }

        private float? _percDescAcrescimo;

        public float PercDescAcrescimo
        {
            get
            {
                if (_percDescAcrescimo == null && IdClienteVend > 0)
                    _percDescAcrescimo = (float)DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(IdClienteVend.Value, IdGrupoProd, IdSubgrupoProd, IdProd, null, null).PercMultiplicar;

                return _percDescAcrescimo != null ? _percDescAcrescimo.Value : 1;
            }
        }

        public string DescrPercDescAcrescimo
        {
            get
            {
                float perc = PercDescAcrescimo;

                if (perc > 1)
                    return "+" + ((perc - 1) * 100).ToString("0.##") + "%";
                else if (perc < 1)
                    return "-" + ((1 - perc) * 100).ToString("0.##") + "%";

                return "0%";
            }
        }

        private float? _percDescAcrescimoAVista;

        public float PercDescAcrescimoAVista
        {
            get
            {
                if (_percDescAcrescimoAVista == null && IdClienteVend > 0)
                    _percDescAcrescimoAVista = (float)DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(IdClienteVend.Value, IdGrupoProd, IdSubgrupoProd, IdProd, null, null).PercMultiplicarAVista;

                return _percDescAcrescimoAVista != null ? _percDescAcrescimoAVista.Value : 1;
            }
        }

        public string DescrPercDescAcrescimoAVista
        {
            get
            {
                float perc = PercDescAcrescimoAVista;

                if (perc > 1)
                    return "+" + ((perc - 1) * 100).ToString("0.##") + "%";
                else if (perc < 1)
                    return "-" + ((1 - perc) * 100).ToString("0.##") + "%";

                return "0%";
            }
        }

        /// <summary>
        /// 0 - Padr�o
        /// 1 - Atacado
        /// 2 - Balc�o
        /// 3 - Obra
        /// </summary>
        [PersistenceProperty("TIPOVALORTABELA", DirectionParameter.InputOptional)]
        public long TipoValorTabela { get; set; }

        public decimal ValorTabelaUtilizado
        {
            get { return ValorOriginalUtilizado * (decimal)PercDescAcrescimo; }
        }

        public decimal ValorOriginalUtilizado
        {
            get
            {
                if (TipoValorTabela == 0)
                    return ClienteRevendaVend ? ValorAtacado : ValorBalcao;
                else
                    return TipoValorTabela == 1 ? ValorAtacado : TipoValorTabela == 2 ? ValorBalcao : ValorObra;
            }
        }

        public string TituloValorTabelaUtilizado
        {
            get
            {
                if (TipoValorTabela == 0)
                    return ClienteRevendaVend ? "Valor Atacado" : "Valor Balc�o";
                else
                    return TipoValorTabela == 1 ? "Valor Atacado" : TipoValorTabela == 2 ? "Valor Balc�o" : "Valor Obra";
            }
        }

        private int? _tipoCalculo = null;

        public int TipoCalculo
        {
            get
            {
                if (_tipoCalculo == null)
                    _tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd, IdSubgrupoProd);

                return _tipoCalculo != null ? _tipoCalculo.Value : (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd;
            }
        }

        public string TotalM2ML
        {
            get
            {
                return (TipoCalculo == (int)TipoCalculoGrupoProd.ML || TipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || TipoCalculo == (int)TipoCalculoGrupoProd.MLAL6) ? TotalML.ToString("N2") + "ml" :
                    TipoCalculo == (int)TipoCalculoGrupoProd.M2 || TipoCalculo == (int)TipoCalculoGrupoProd.M2Direto ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.QtdM2 || GrupoProdDAO.Instance.IsVidro(IdGrupoProd) ?
                    TotalM2.ToString("N2") + "m�" : "0";
            }
        }

        public string TotalM2Rel
        {
            get
            {
                return
                    TipoCalculo == (int)TipoCalculoGrupoProd.M2 || TipoCalculo == (int)TipoCalculoGrupoProd.M2Direto ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.QtdM2 || GrupoProdDAO.Instance.IsVidro(IdGrupoProd) ?
                    TotalM2.ToString("N2") + "m�" : "";
            }
        }

        public string TotalMLRel
        {
            get
            {
                return (TipoCalculo == (int)TipoCalculoGrupoProd.ML || TipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || TipoCalculo == (int)TipoCalculoGrupoProd.MLAL6) ? TotalML.ToString("N2") + "ml" : "";
            }
        }

        public bool UsarEstoqueM2
        {
            get { return TipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || TipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto; }
        }

        public float SugestaoCompra
        {
            get
            {
                double retorno = (2 * EstoqueMinimo) + Reserva + (PedidoConfig.LiberarPedido ? Liberacao : 0) -
                    (UsarEstoqueM2 ? M2Estoque + TotMComprando : QtdeEstoque + QtdeComprando);

                return (float)Math.Round(retorno, 2);
            }
        }

        public float SugestaoProducao
        {
            get
            {
                double retorno = (2 * EstoqueMinimo) + Reserva + (PedidoConfig.LiberarPedido ? Liberacao : 0) -
                    (UsarEstoqueM2 ? M2Estoque + TotMProduzindo : QtdeEstoque + QtdeProduzindo);

                return (float)Math.Round(retorno, 2);
            }
        }

        public string ReferenciaCompra
        {
            get { return !String.IsNullOrEmpty(IdsCompras) ? "Compra(s): " + IdsCompras : "NF: " + NumeroNfe; }
        }

        private Dictionary<uint, float> _dadosBaixaEstoque = null;

        public Dictionary<uint, float> DadosBaixaEstoque
        {
            get
            {
                if (_dadosBaixaEstoque == null)
                {
                    _dadosBaixaEstoque = new Dictionary<uint, float>();
                    foreach (ProdutoBaixaEstoque pbe in ProdutoBaixaEstoqueDAO.Instance.GetByProd((uint)IdProd, false))
                    {
                        if (_dadosBaixaEstoque.ContainsKey((uint)pbe.IdProdBaixa))
                        {
                            _dadosBaixaEstoque[(uint)pbe.IdProdBaixa] += pbe.Qtde;
                            continue;
                        }

                        _dadosBaixaEstoque.Add((uint)pbe.IdProdBaixa, pbe.Qtde);
                    }
                }

                return _dadosBaixaEstoque;
            }
            set { _dadosBaixaEstoque = value; }
        }

        public string DescrMateriaPrima
        {
            get
            {
                string retorno = "";
                foreach (uint idProd in DadosBaixaEstoque.Keys)
                    if (idProd > 0 && DadosBaixaEstoque[idProd] > 0)
                        retorno += ProdutoDAO.Instance.ObtemDescricao((int)idProd) + " x " + DadosBaixaEstoque[idProd] + ", ";

                return retorno.TrimEnd(',', ' ');
            }
        }

        private Dictionary<uint, float> _dadosBaixaEstoqueFiscal = null;

        public Dictionary<uint, float> DadosBaixaEstoqueFiscal
        {
            get
            {
                if (_dadosBaixaEstoqueFiscal == null)
                {
                    _dadosBaixaEstoqueFiscal = new Dictionary<uint, float>();
                    foreach (ProdutoBaixaEstoqueFiscal pbef in ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd((uint)IdProd, false))
                    {
                        if (_dadosBaixaEstoqueFiscal.ContainsKey((uint)pbef.IdProdBaixa))
                        {
                            _dadosBaixaEstoqueFiscal[(uint)pbef.IdProdBaixa] += pbef.Qtde;
                            continue;
                        }

                        _dadosBaixaEstoqueFiscal.Add((uint)pbef.IdProdBaixa, pbef.Qtde);
                    }
                }

                return _dadosBaixaEstoqueFiscal;
            }
            set { _dadosBaixaEstoqueFiscal = value; }
        }

        [Log("Estoque Fiscal - Produto para Baixa")]
        public string DescrEstoqueFiscalProdBaixa
        {
            get
            {
                string retorno = "";
                foreach (uint idProd in DadosBaixaEstoqueFiscal.Keys)
                    if (idProd > 0 && DadosBaixaEstoqueFiscal[idProd] > 0)
                        retorno += ProdutoDAO.Instance.ObtemDescricao((int)idProd) + " x " + DadosBaixaEstoqueFiscal[idProd] + ", ";

                return retorno.TrimEnd(',', ' ');
            }
        }

        private IList<RelModel.ControleIcmsProdutoPorUf> _aliqIcms = null;

        public IList<RelModel.ControleIcmsProdutoPorUf> AliqICMS
        {
            get
            {
                if (_aliqIcms == null)
                    _aliqIcms = IcmsProdutoUfDAO.Instance.ObtemParaControle((uint)IdProd);

                return _aliqIcms;
            }
            set { _aliqIcms = value; }
        }

        [Log("Al�quota ICMS")]
        public string DescrAliqIcms
        {
            get
            {
                StringBuilder retorno = new StringBuilder();
                foreach (var dados in AliqICMS)
                {
                    if (!String.IsNullOrEmpty(dados.UfOrigem))
                        retorno.AppendFormat("UF Origem: {0} ", dados.UfOrigem);

                    if (!String.IsNullOrEmpty(dados.UfDestino))
                        retorno.AppendFormat("UF Destino: {0} ", dados.UfDestino);

                    retorno.AppendFormat("Al�q. ICMS Intraestadual: {0} Al�q. ICMS Interestadual: {1}, Al�q. ICMS Interna Destinat�rio: {2}, Al�q. FCP Intraestadual: {3} Al�q. FCP Interestadual: {4} ",
                        dados.AliquotaIntraestadual, dados.AliquotaInterestadual, dados.AliquotaInternaDestinatario, dados.AliquotaFCPIntraestadual, dados.AliquotaFCPInterestadual);
                }

                return retorno.ToString().TrimEnd(',', ' ');
            }
        }

        private IList<RelModel.ControleMvaProdutoPorUf> _mva = null;

        public IList<RelModel.ControleMvaProdutoPorUf> Mva
        {
            get
            {
                if (_mva == null)
                    _mva = MvaProdutoUfDAO.Instance.ObtemParaControle(IdProd);

                return _mva;
            }
            set { _mva = value; }
        }

        [Log("MVA")]
        public string DescrMva
        {
            get
            {
                StringBuilder retorno = new StringBuilder();
                foreach (var dados in Mva)
                {
                    if (!String.IsNullOrEmpty(dados.UfOrigem))
                        retorno.AppendFormat("UF Origem: {0} ", dados.UfOrigem);

                    if (!String.IsNullOrEmpty(dados.UfDestino))
                        retorno.AppendFormat("UF Destino: {0} ", dados.UfDestino);

                    retorno.AppendFormat("MVA Original: {0} MVA Simples: {1}, ", dados.MvaOriginal, dados.MvaSimples);
                }

                return retorno.ToString().TrimEnd(',', ' ');
            }
        }

        /// <summary>
        /// Flags do produto
        /// </summary>
        public int[] FlagsArqMesa { get; set; }

        /// <summary>
        /// Descri��o da do arquuivo de mesa
        /// </summary>
        public string FlagsArqMesaDescricao { get; set; }

        #endregion

        #region Propriedades do Beneficiamento

        public bool SalvarBeneficiamentos { get; set; }

        private List<ProdutoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdProd == 0 || !ProdutoDAO.Instance.CalculaBeneficiamento(IdProd))
                        _beneficiamentos = new List<ProdutoBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<ProdutoBenef>(ProdutoBenefDAO.Instance.GetByProduto((uint)IdProd));
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoBenef>();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        public string DescricaoBeneficiamentos
        {
            get
            {
                var beneficiamentos = string.Empty;

                foreach (var produtoBenef in Beneficiamentos)
                {
                    var parenteId = BenefConfigDAO.Instance.GetParentId(produtoBenef.IdBenefConfig);
                    beneficiamentos += (!string.IsNullOrEmpty(beneficiamentos) ? " - " : "") + BenefConfigDAO.Instance.GetElement(parenteId).DescricaoCompleta +
                       " " + BenefConfigDAO.Instance.GetElementByPrimaryKey(produtoBenef.IdBenefConfig).DescricaoCompleta;
                }
                return beneficiamentos;
            }
        }

        public string DescricaoProdutoBeneficiamento
        {
            get
            {
                return !String.IsNullOrEmpty(Descricao) ?
                    (Descricao.ToUpper() + (!String.IsNullOrEmpty(DescricaoBeneficiamentos) ? " ( " + DescricaoBeneficiamentos + " )" : "")) : Descricao.ToUpper();
            }

        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        [Log("Beneficiamentos")]
        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion

        #region IProduto Members

        int? Sync.Fiscal.EFD.Entidade.IProduto.CodigoGeneroProduto
        {
            get { return IdGeneroProduto; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProduto.CodigoUnidadeMedida
        {
            get { return IdUnidadeMedida; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProduto.CodigoContaContabil
        {
            get { return IdContaContabil; }
        }

        string Sync.Fiscal.EFD.Entidade.IProduto.CodigoInterno
        {
            get { return CodInterno; }
        }

        string Sync.Fiscal.EFD.Entidade.IProduto.NCM
        {
            get { return Ncm; }
        }

        Sync.Fiscal.Enumeracao.Produto.TipoMercadoria? Sync.Fiscal.EFD.Entidade.IProduto.TipoMercadoria
        {
            get { return (Sync.Fiscal.Enumeracao.Produto.TipoMercadoria?)TipoMercadoria; }
        }

        string Sync.Fiscal.EFD.Entidade.IProduto.GTIN
        {
            get { return GTINProduto; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.IProduto.DataCadastro
        {
            get { return DataCad; }
        }

        string Sync.Fiscal.EFD.Entidade.IProduto.UfIcms { get; set; }

        #endregion

        #region IBuscarAPartirDoLog Members

        int Sync.Fiscal.EFD.Entidade.IBuscarAPartirDoLog.Codigo
        {
            get { return IdProd; }
        }

        #endregion

        #region IProdutoIcmsSt Members

        decimal IProdutoIcmsSt.Total
        {
            get { return ValorTabelaUtilizado; }
        }

        float IProdutoIcmsSt.AliquotaIcms
        {
            get
            {
                if (IdLojaIcms > 0)
                    return IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)IdProd, IdLojaIcms, IdFornecIcms, IdClienteIcms);
                else
                    return 0;
            }
        }

        decimal IProdutoIcmsSt.ValorIcms
        {
            get
            {
                return (this as Glass.Data.Model.IProdutoIcmsSt).Total *
                    (decimal)((this as Glass.Data.Model.IProdutoIcmsSt).AliquotaIcms / 100);
            }
        }

        float IProdutoIcmsSt.AliquotaIpi
        {
            get { return AliqIPI; }
        }

        decimal IProdutoIcmsSt.ValorIpi
        {
            get
            {
                return (this as Glass.Data.Model.IProdutoIcmsSt).Total *
                    (decimal)((this as Glass.Data.Model.IProdutoIcmsSt).AliquotaIpi / 100);
            }
        }

        float IProdutoIcmsSt.AliquotaIcmsSt
        {
            get
            {
                if (IdLojaIcms > 0)
                    return IcmsProdutoUfDAO.Instance.ObterAliquotaIcmsSt(null, (uint)IdProd, IdLojaIcms, IdFornecIcms, IdClienteIcms);
                else
                    return 0;
            }
        }

        decimal IProdutoIcmsSt.ValorDesconto
        {
            get { return 0; }
        }

        decimal IProdutoIcmsSt.ValorFrete
        {
            get { return 0; }
        }

        decimal IProdutoIcmsSt.ValorSeguro
        {
            get { return 0; }
        }

        decimal IProdutoIcmsSt.ValorOutrasDespesas
        {
            get { return 0; }
        }

        float IProdutoIcmsSt.PercentualReducaoBaseCalculo
        {
            get { return 0; }
        }

        #endregion
    }
}