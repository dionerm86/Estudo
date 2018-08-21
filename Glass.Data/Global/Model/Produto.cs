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
    /// Possíveis tipos de mercadorias.
    /// </summary>
    public enum TipoMercadoria
    {
        /// <summary>
        /// Mercador Revenda.
        /// </summary>
        [Description("Mercadoria Revenda")]
        MercadoriaRevenda,
        /// <summary>
        /// Matéria Prima.
        /// </summary>
        [Description("Matéria Prima")]
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
        /// Produto Intermediário.
        /// </summary>
        [Description("Produto Intermediário")]
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
        /// Serviços.
        /// </summary>
        [Description("Serviços")]
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
    /// Possíveis códigos ds situação tributária do Ipi do produto.
    /// </summary>
    public enum ProdutoCstIpi
    {
        /// <summary>
        /// Entrada recuperando crédito.
        /// </summary>
        [Description("Entrada recuperando crédito")]
        EntradaRecuperandoCredito,
        /// <summary>
        /// Entrada Tributada Alíquota Zero.
        /// </summary>
        [Description("Entrada tributada com alíquota zero")]
        EntradaTributadaAliquotaZero,
        /// <summary>
        /// Entrada Isenta
        /// </summary>
        [Description("Entrada isenta")]
        EntradaIsenta,
        /// <summary>
        /// Entrada não Tributada.
        /// </summary>
        [Description("Entrada não-tributada")]
        EntradaNaoTributada,
        /// <summary>
        /// Entrada Imenu.
        /// </summary>
        [Description("Entrada Imune")]
        EntradaImune,
        /// <summary>
        /// Entrada com suspenção.
        /// </summary>
        [Description("Entrada com suspensão")]
        EntradaComSuspensao,
        /// <summary>
        /// Outras entradas.
        /// </summary>
        [Description("Outras Entradas")]
        OutrasEntradas = 49,
        /// <summary>
        /// Saída Tributada.
        /// </summary>
        [Description("Saída Tributada")]
        SaidaTributada,
        /// <summary>
        /// Saída Tributada Alíquota Zero.
        /// </summary>
        [Description("Saída tributada com alíquota zero")]
        SaidaTributadaAliquotaZero,
        /// <summary>
        /// Saída Isenta
        /// </summary>
        [Description("Saída Isenta")]
        SaidaIsenta,
        /// <summary>
        /// Saída não Tributada.
        /// </summary>
        [Description("Saída não-tributada")]
        SaidaNaoTributada,
        /// <summary>
        /// Saída Imune.
        /// </summary>
        [Description("Saída Imune")]
        SaidaImune,
        /// <summary>
        /// Saída com Suspenção.
        /// </summary>
        [Description("Saída com suspensão")]
        SaidaComSuspensao,
        /// <summary>
        /// Outras Saídas.
        /// </summary>
        [Description("Outras Saídas")]
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

        [Log("Cor do Alumínio", "Descricao", typeof(CorAluminioDAO))]
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

        [Log("Cód. Produto")]
        [PersistenceProperty("CODINTERNO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string CodInterno
        {
            get { return !String.IsNullOrEmpty(_codInterno) ? _codInterno.ToUpper() : _codInterno; }
            set { _codInterno = !String.IsNullOrEmpty(value) ? value.ToUpper() : value; }
        }

        [Log("Cód. EX")]
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

        [Log("Descrição", true)]
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

        [Log("Valor Balcão")]
        [PersistenceProperty("VALORBALCAO")]
        public decimal ValorBalcao { get; set; }

        [Log("Valor Obra")]
        [PersistenceProperty("VALOROBRA")]
        public decimal ValorObra { get; set; }

        [Log("Valor Reposição")]
        [PersistenceProperty("VALORREPOSICAO")]
        public decimal ValorReposicao { get; set; }

        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [PersistenceProperty("UNIDADETRIB", DirectionParameter.InputOptional)]
        public string UnidadeTrib { get; set; }

        [Log("Valor Mínimo")]
        [PersistenceProperty("Valor_Minimo")]
        public decimal ValorMinimo { get; set; }

        [Log("Valor de Transferência")]
        [PersistenceProperty("VALORTRANSFERENCIA")]
        public decimal ValorTransferencia { get; set; }

        /// <summary>
        /// 00 - Tributada integralmente
        /// 10 - Tributada e com cobrança de ICMS por substituição tributária
        /// 20 - Com redução de base de cálculo
        /// 30 - Isenta ou não tributada e com cobrança de ICMS por substituição tributária
        /// 40 - Isenta
        /// 41 - Não tributada
        /// 50 - Suspensão
        /// 51 - Diferimento. A exigência do preenchimento das informações do ICMS diferido fica à critério de cada UF.
        /// 60 - ICMS cobrado anteriormente por substituição tributária
        /// 70 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária
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

        [Log("Alíquota IPI")]
        [PersistenceProperty("AliqIPI")]
        public float AliqIPI { get; set; }

        [Log("Cód. Otimização")]
        [PersistenceProperty("CODOTIMIZACAO")]
        public string CodOtimizacao { get; set; }

        [Log("Ativar Mínimo")]
        [PersistenceProperty("AtivarMin")]
        public bool AtivarMin { get; set; }

        [Log("Espessura")]
        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [Log("Peso")]
        [PersistenceProperty("PESO")]
        public float Peso { get; set; }

        [Log("Área Mínima")]
        [PersistenceProperty("AREAMINIMA")]
        public float AreaMinima { get; set; }

        [Log("Ativar Área Mínima")]
        [PersistenceProperty("ATIVARAREAMINIMA")]
        public bool AtivarAreaMinima { get; set; }

        [Log("Compra")]
        [PersistenceProperty("COMPRA")]
        public bool Compra { get; set; }

        [Log("Item genérico")]
        [PersistenceProperty("ITEMGENERICO")]
        public bool ItemGenerico { get; set; }

        [PersistenceProperty("DATAALT")]
        [Colosoft.Data.Schema.CacheIndexed]
        public DateTime? DataAlt { get; set; }

        [PersistenceProperty("USUALT")]
        [Colosoft.Data.Schema.CacheIndexed]
        public int? UsuAlt { get; set; }

        [Log("Observação")]
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

        [Log("Plano de Conta Contábil", "Descricao", typeof(PlanoContaContabilDAO))]
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

        [Log("Aplicação", "CodInterno", typeof(EtiquetaAplicacaoDAO))]
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
        /// Obtém ou define a distância da margem inferior da chapa.
        /// </summary>
        [Log("Recorte X1")]
        [PersistenceProperty("RecorteX1")]
        public double RecorteX1 { get; set; }

        /// <summary>
        /// Obtém ou define a distância da margem esquerda da chapa.
        /// </summary>
        [Log("Recorte Y1")]
        [PersistenceProperty("RecorteY1")]
        public double RecorteY1 { get; set; }

        /// <summary>
        /// Obtém ou define a distância da margem superior da chapa.
        /// </summary>
        [Log("Recorte X2")]
        [PersistenceProperty("RecorteX2")]
        public double RecorteX2 { get; set; }

        /// <summary>
        /// Obtém ou define a distância da margem direita da chapa.
        /// </summary>
        [Log("Recorte Y2")]
        [PersistenceProperty("RecorteY2")]
        public double RecorteY2 { get; set; }

        /// <summary>
        /// Obtém ou define a distância máxima de lado X da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        [Log("Transversal Máxima X")]
        [PersistenceProperty("TransversalMaxX")]
        public double TransversalMaxX { get; set; } = 9999.0;

        /// <summary>
        /// Obtém ou define a distância máxima de lado Y da chapa
        /// da qual deve ser criada uma transversal.
        /// </summary>
        [Log("Transversal Máxima Y")]
        [PersistenceProperty("TransversalMaxY")]
        public double TransversalMaxY { get; set; } = 9999.0;

        /// <summary>
        /// Obtém ou define a dimensão mínima em X da superfície de desperdício
        /// geradas pelo programa de otimização que, ao serem suficientemente 
        /// grandes, se podem considerar reutilizáveis e é desejável introduzi-las
        /// de novo no estoque para otimizações posteriores
        /// </summary>
        [Log("Desperdício Mínimo X")]
        [PersistenceProperty("DesperdicioMinX")]
        public double DesperdicioMinX { get; set; }

        /// <summary>
        /// Obtém ou define a dimensão mínima em Y da superfície de desperdício
        /// geradas pelo programa de otimização que, ao serem suficientemente 
        /// grandes, se podem considerar reutilizáveis e é desejável introduzi-las
        /// de novo no estoque para otimizações posteriores
        /// </summary>
        [Log("Desperdício Mínimo Y")]
        [PersistenceProperty("DesperdicioMinY")]
        public double DesperdicioMinY { get; set; }

        /// <summary>
        /// Obtém ou define a distância minima aceitavel durante a otimização 
        /// entre dois cortes paralelos, com o intuito de facilitar ou tornar 
        /// possível a abertura dos cortes.
        /// </summary>
        /// <example>
        /// Ao configurar o valor em 20mm, será impossível encontrar no interior
        /// de um plano de corte duas peças ou dois cortes próximos um do outro,
        /// de distância inferior à anteriormente introduzida (20mm).
        /// </example>
        /// <remarks>
        /// Evidentemente, esta distância não é tida em conta nos casos em que 
        /// 2 peças compartilham o mesmo corte.
        /// </remarks>
        [Log("Distância Mínima")]
        [PersistenceProperty("DistanciaMin")]
        public double DistanciaMin { get; set; }

        /// <summary>
        /// Obtém ou define a configuração do valor de recorte que deve
        /// introduzir-se nas formas no caso de esta conter ângulos 
        /// inferiores ao configurado no campo "AnguloRecorteAutomatico"
        /// </summary>
        [Log("Recorte Automático da Forma")]
        [PersistenceProperty("RecorteAutomaticoForma")]
        public double RecorteAutomaticoForma { get; set; }

        /// <summary>
        /// Obtém ou define o valor do ângulo ao qual o recorte deve
        /// ser introduzido de forma automática.
        /// </summary>
        [Log("Ângulo de Recorte Automático")]
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

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                return Colosoft.Translator.Translate(Situacao).Format();
            }
        }

        #region Propriedades para a busca de alíquota de ICMS interna

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
                        throw new Exception("Indique a Loja para buscar a alíquota de ICMS interna.");
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
                        throw new Exception("Indique a Loja para buscar a alíquota de ICMS interna.");
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
        /// 0 - Padrão
        /// 1 - Atacado
        /// 2 - Balcão
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
                    return ClienteRevendaVend ? "Valor Atacado" : "Valor Balcão";
                else
                    return TipoValorTabela == 1 ? "Valor Atacado" : TipoValorTabela == 2 ? "Valor Balcão" : "Valor Obra";
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
                    TotalM2.ToString("N2") + "m²" : "0";
            }
        }

        public string TotalM2Rel
        {
            get
            {
                return
                    TipoCalculo == (int)TipoCalculoGrupoProd.M2 || TipoCalculo == (int)TipoCalculoGrupoProd.M2Direto ||
                    TipoCalculo == (int)TipoCalculoGrupoProd.QtdM2 || GrupoProdDAO.Instance.IsVidro(IdGrupoProd) ?
                    TotalM2.ToString("N2") + "m²" : "";
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

        [Log("Alíquota ICMS")]
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

                    retorno.AppendFormat("Alíq. ICMS Intraestadual: {0} Alíq. ICMS Interestadual: {1}, Alíq. ICMS Interna Destinatário: {2}, Alíq. FCP Intraestadual: {3} Alíq. FCP Interestadual: {4} ",
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
        /// Descrição da do arquuivo de mesa
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