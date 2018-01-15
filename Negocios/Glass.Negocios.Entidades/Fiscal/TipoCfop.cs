namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócios do tipo de
    /// Código Fiscal de Operações e Prestações.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoCfpoLoader))]
    public class TipoCfop : Colosoft.Business.Entity<Data.Model.TipoCfop>
    {
        #region Tipos Aninhados

        class TipoCfpoLoader : Colosoft.Business.EntityLoader<TipoCfop, Data.Model.TipoCfop>
        {
            public TipoCfpoLoader()
            {
                Configure()
                    .Uid(f => f.IdTipoCfop)
                    .FindName(f => f.Descricao)
                    .Creator(f => new TipoCfop(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do tipo.
        /// </summary>
        public int IdTipoCfop
        {
            get { return DataModel.IdTipoCfop; }
            set
            {
                if (DataModel.IdTipoCfop != value &&
                    RaisePropertyChanging("IdTipoCfop", value))
                {
                    DataModel.IdTipoCfop = value;
                    RaisePropertyChanged("IdTipoCfop");
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
        /// Industrialização.
        /// </summary>
        public bool Industrializacao
        {
            get { return DataModel.Industrializacao; }
            set
            {
                if (DataModel.Industrializacao != value &&
                    RaisePropertyChanging("Industrializacao", value))
                {
                    DataModel.Industrializacao = value;
                    RaisePropertyChanged("Industrializacao");
                }
            }
        }

        /// <summary>
        /// Devolução.
        /// </summary>
        public bool Devolucao
        {
            get { return DataModel.Devolucao; }
            set
            {
                if (DataModel.Devolucao != value &&
                    RaisePropertyChanging("Devolucao", value))
                {
                    DataModel.Devolucao = value;
                    RaisePropertyChanged("Devolucao");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TipoCfop()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoCfop(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.TipoCfop> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoCfop(Data.Model.TipoCfop dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
