namespace Glass.Pedido.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade do produto do pedido.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutosPedidoLoader))]
    public class ProdutosPedido : Colosoft.Business.Entity<Data.Model.ProdutosPedido>
    {
        #region Tipos Aninhados

        class ProdutosPedidoLoader : Colosoft.Business.EntityLoader<ProdutosPedido, Data.Model.ProdutosPedido>
        {
            public ProdutosPedidoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdProdPed)
                    .Reference<Glass.Global.Negocios.Entidades.Produto, Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProd)
                    .Creator(f => new ProdutosPedido(f));
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
        /// Identificador do produto do pedido.
        /// </summary>
        public uint IdProdPed
        {
            get { return DataModel.IdProdPed; }
            set
            {
                if (DataModel.IdProdPed != value &&
                    RaisePropertyChanging("IdProdPed", value))
                {
                    DataModel.IdProdPed = value;
                    RaisePropertyChanged("IdProdPed");
                }
            }
        }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public uint IdProd
        {
            get { return DataModel.IdProd; }
            set
            {
                if (DataModel.IdProd != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProd = value;
                    RaisePropertyChanged("IdProd", "Total", "AliqIpi", "AliqIcms");
                }
            }
        }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public uint IdPedido
        {
            get { return DataModel.IdPedido; }
            set
            {
                if (DataModel.IdPedido != value &&
                    RaisePropertyChanging("IdPedido", value))
                {
                    DataModel.IdPedido = value;
                    RaisePropertyChanged("IdPedido");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutosPedido()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutosPedido(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ProdutosPedido> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutosPedido(Glass.Data.Model.ProdutosPedido dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
