namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da categoria de conta.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CategoriaContaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.CategoriaConta)]
    public class CategoriaConta : Colosoft.Business.Entity<Data.Model.CategoriaConta>
    {
        #region Tipos Aninhados

        class CategoriaContaLoader : Colosoft.Business.EntityLoader<CategoriaConta, Data.Model.CategoriaConta>
        {
            public CategoriaContaLoader()
            {
                Configure()
                    .Uid(f => f.IdCategoriaConta)
                    .FindName(f => f.Descricao)
                    .Creator(f => new CategoriaConta(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da categoria.
        /// </summary>
        public int IdCategoriaConta
        {
            get { return DataModel.IdCategoriaConta; }
            set
            {
                if (DataModel.IdCategoriaConta != value &&
                    RaisePropertyChanging("IdCategoriaConta", value))
                {
                    DataModel.IdCategoriaConta = value;
                    RaisePropertyChanged("IdCategoriaConta");
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
        /// Tipo da categoria.
        /// </summary>
        public Data.Model.TipoCategoriaConta? Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Número da sequência.
        /// </summary>
        public int NumeroSequencia
        {
            get { return DataModel.NumeroSequencia; }
            set
            {
                if (DataModel.NumeroSequencia != value &&
                    RaisePropertyChanging("NumeroSequencia", value))
                {
                    DataModel.NumeroSequencia = value;
                    RaisePropertyChanged("NumeroSequencia");
                }
            }
        }

        /// <summary>
        /// Situacao
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CategoriaConta()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CategoriaConta(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CategoriaConta> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CategoriaConta(Data.Model.CategoriaConta dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
