namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do produto.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(GeneroProdutoLoader))]
    public class GeneroProduto : Colosoft.Business.Entity<Data.Model.GeneroProduto>
    {
        #region Tipos Aninhados

        class GeneroProdutoLoader : Colosoft.Business.EntityLoader<GeneroProduto, Data.Model.GeneroProduto>
        {
            public GeneroProdutoLoader()
            {
                Configure()
                    .Uid(f => f.IdGeneroProduto)
                    .FindName(f => f.Codigo)
                    .Description(f => f.Descricao)
                    .Creator(f => new GeneroProduto(f));
            }            
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do genero.
        /// </summary>
        public int IdGeneroProduto
        {
            get { return DataModel.IdGeneroProduto; }
            set
            {
                if (DataModel.IdGeneroProduto != value &&
                    RaisePropertyChanging("IdGeneroProduto", value))
                {
                    DataModel.IdGeneroProduto = value;
                    RaisePropertyChanged("IdGeneroProduto");
                }
            }
        }

        /// <summary>
        /// Código.
        /// </summary>
        public string Codigo
        {
            get { return DataModel.Codigo; }
            set
            {
                if (DataModel.Codigo != value &&
                    RaisePropertyChanging("Codigo", value))
                {
                    DataModel.Codigo = value;
                    RaisePropertyChanged("Codigo");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Propriedade que representa a junção do código com a descrição.
        /// </summary>
        public string CodigoDescricao
        {
            get { return Codigo + " - " + (Descricao.Length > 50 ? Descricao.Substring(0, 40) + "..." : Descricao); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public GeneroProduto()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected GeneroProduto(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.GeneroProduto> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public GeneroProduto(Data.Model.GeneroProduto dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
