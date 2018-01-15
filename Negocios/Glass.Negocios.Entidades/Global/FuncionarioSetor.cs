namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade do relacionamento do funcionário com o setor.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FuncionarioSetorLoader))]
    public class FuncionarioSetor : Colosoft.Business.Entity<Glass.Data.Model.FuncionarioSetor>
    {
        #region Tipos Aninhados

        class FuncionarioSetorLoader : Colosoft.Business.EntityLoader<FuncionarioSetor, Glass.Data.Model.FuncionarioSetor>
        {
            public FuncionarioSetorLoader()
            {
                Configure()
                    .Keys(f => f.IdFunc, f => f.IdSetor)
                    .Creator(f => new FuncionarioSetor(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do funcionário.
        /// </summary>
        public int IdFunc
        {
            get { return DataModel.IdFunc; }
            set
            {
                if (DataModel.IdFunc != value &&
                    RaisePropertyChanging("IdFunc", value))
                {
                    DataModel.IdFunc = value;
                    RaisePropertyChanged("IdFunc");
                }
            }
        }

        /// <summary>
        /// Identificador do setor.
        /// </summary>
        public int IdSetor
        {
            get { return DataModel.IdSetor; }
            set
            {
                if (DataModel.IdSetor != value &&
                    RaisePropertyChanging("IdSetor", value))
                {
                    DataModel.IdSetor = value;
                    RaisePropertyChanged("IdSetor");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FuncionarioSetor()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FuncionarioSetor(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FuncionarioSetor> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FuncionarioSetor(Glass.Data.Model.FuncionarioSetor dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
