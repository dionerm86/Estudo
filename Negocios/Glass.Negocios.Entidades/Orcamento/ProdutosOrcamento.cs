
namespace Glass.Orcamento.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade do produto do pedido.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutosOrcamentoLoader))]
    public class ProdutosOrcamento : Colosoft.Business.Entity<Data.Model.ProdutosOrcamento>
    {
        #region Tipos Aninhados

        class ProdutosOrcamentoLoader : Colosoft.Business.EntityLoader<ProdutosOrcamento, Data.Model.ProdutosOrcamento>
        {
            public ProdutosOrcamentoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdProd)
                    .Reference<Glass.Global.Negocios.Entidades.Produto, Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProd)
                    .Creator(f => new ProdutosOrcamento(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Referencia do produto associado.
        /// </summary>
        public Glass.Global.Negocios.Entidades.Produto Produto
        {
            get { return GetReference<Glass.Global.Negocios.Entidades.Produto>("Produto", true); }
        }

        /// <summary>
        /// Identificador do ambiente do pedido.
        /// </summary>
        public int? IdAmbientePedido
        {
            get { return DataModel.IdAmbientePedido; }
            set
            {
                if (DataModel.IdAmbientePedido != value &&
                    RaisePropertyChanging("IdAmbientePedido", value))
                {
                    DataModel.IdAmbientePedido = value;
                    RaisePropertyChanged("IdAmbientePedido");
                }
            }
        }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public uint? IdProd
        {
            get { return DataModel.IdProduto; }
            set
            {
                if (DataModel.IdProduto != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProduto = value;
                    RaisePropertyChanged("IdProduto", "Total", "AliqIpi", "AliqIcms");
                }
            }
        }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public uint IdOrcamento
        {
            get { return DataModel.IdOrcamento; }
            set
            {
                if (DataModel.IdOrcamento != value &&
                    RaisePropertyChanging("IdPedido", value))
                {
                    DataModel.IdOrcamento = value;
                    RaisePropertyChanged("IdPedido");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutosOrcamento()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutosOrcamento(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ProdutosOrcamento> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutosOrcamento(Glass.Data.Model.ProdutosOrcamento dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
