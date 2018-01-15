namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Entidade responsavel por armazenar os dados da movimentação interna de estoque
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(MovInternaEstoqueFiscalLoader))]
    public class MovInternaEstoqueFiscal : Glass.Negocios.Entidades.EntidadeBaseCadastro<Glass.Data.Model.MovInternaEstoqueFiscal>
    {
        #region Tipos Aninhados

        class MovInternaEstoqueFiscalLoader : Colosoft.Business.EntityLoader<MovInternaEstoqueFiscal, Glass.Data.Model.MovInternaEstoqueFiscal>
        {
            public MovInternaEstoqueFiscalLoader()
            {
                Configure()
                    .Uid(f => f.IdMovInternaEstoqueFiscal)
                    .Reference<Global.Negocios.Entidades.Produto, Data.Model.Produto>("ProdutoOrigem", f => f.ProdutoOrigem, f => f.IdProdOrigem)
                    .Reference<Global.Negocios.Entidades.Produto, Data.Model.Produto>("ProdutoDestino", f => f.ProdutoDestino, f => f.IdProdDestino)
                    .Reference<Global.Negocios.Entidades.Loja, Data.Model.Loja>("Loja", f => f.Loja, f => f.IdLoja)
                    .Reference<Global.Negocios.Entidades.Funcionario, Data.Model.Funcionario>("Funcionario", f => f.Funcionario, f => f.Usucad)
                    .Creator(f => new MovInternaEstoqueFiscal(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public MovInternaEstoqueFiscal()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected MovInternaEstoqueFiscal(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.MovInternaEstoqueFiscal> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public MovInternaEstoqueFiscal(Glass.Data.Model.MovInternaEstoqueFiscal dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Produto origem associado
        /// </summary>
        public Global.Negocios.Entidades.Produto ProdutoOrigem
        {
            get { return GetReference<Global.Negocios.Entidades.Produto>("ProdutoOrigem", true); }
        }

        /// <summary>
        /// Produto destino associado
        /// </summary>
        public Global.Negocios.Entidades.Produto ProdutoDestino
        {
            get { return GetReference<Global.Negocios.Entidades.Produto>("ProdutoDestino", true); }
        }

        public Global.Negocios.Entidades.Loja Loja
        {
            get { return GetReference<Global.Negocios.Entidades.Loja>("Loja", true); }
        }

        public Global.Negocios.Entidades.Funcionario Funcionario
        {
            get { return GetReference<Global.Negocios.Entidades.Funcionario>("Funcionario", true); }
        }

        /// <summary>
        /// Identificador da movimentação.
        /// </summary>
        public int IdMovInternaEstoqueFiscal
        {
            get { return DataModel.IdMovInternaEstoqueFiscal; }
            set
            {
                if (DataModel.IdMovInternaEstoqueFiscal != value &&
                    RaisePropertyChanging("IdMovInternaEstoqueFiscal", value))
                {
                    DataModel.IdMovInternaEstoqueFiscal = value;
                    RaisePropertyChanged("IdMovInternaEstoqueFiscal");
                }
            }
        }

        /// <summary>
        /// Identificador do produto de origem.
        /// </summary>
        public int IdProdOrigem
        {
            get { return DataModel.IdProdOrigem; }
            set
            {
                if (DataModel.IdProdOrigem != value &&
                    RaisePropertyChanging("IdProdOrigem", value))
                {
                    DataModel.IdProdOrigem = value;
                    RaisePropertyChanged("IdProdOrigem");
                }
            }
        }

        /// <summary>
        /// Identificador do produto de destino.
        /// </summary>
        public int IdProdDestino
        {
            get { return DataModel.IdProdDestino; }
            set
            {
                if (DataModel.IdProdDestino != value &&
                    RaisePropertyChanging("IdProdDestino", value))
                {
                    DataModel.IdProdDestino = value;
                    RaisePropertyChanged("IdProdDestino");
                }
            }
        }

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Quantida movimentada do produto de origem.
        /// </summary>
        public decimal QtdeOrigem
        {
            get { return DataModel.QtdeOrigem; }
            set
            {
                if (DataModel.QtdeOrigem != value &&
                    RaisePropertyChanging("QtdeOrigem", value))
                {
                    DataModel.QtdeOrigem = value;
                    RaisePropertyChanged("QtdeOrigem");
                }
            }
        }

        /// <summary>
        /// Quantidade movimentada do produto de destino.
        /// </summary>
        public decimal QtdeDestino
        {
            get { return DataModel.QtdeDestino; }
            set
            {
                if (DataModel.QtdeDestino != value &&
                    RaisePropertyChanging("QtdeDestino", value))
                {
                    DataModel.QtdeDestino = value;
                    RaisePropertyChanged("QtdeDestino");
                }
            }
        }

        #endregion
    }
}
