namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio que liga o funcionário ao departamento.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FuncDepartamentoLoader))]
    public class FuncDepartamento : Colosoft.Business.Entity<Glass.Data.Model.FuncDepartamento>
    {
        #region Tipos Aninhados

        class FuncDepartamentoLoader : Colosoft.Business.EntityLoader<FuncDepartamento, Glass.Data.Model.FuncDepartamento>
        {
            public FuncDepartamentoLoader()
            {
                Configure()
                    .Keys(f => f.IdFunc, f => f.IdDepartamento)
                    .Creator(f => new FuncDepartamento(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do departamento associado.
        /// </summary>
        public int IdDepartamento
        {
            get { return DataModel.IdDepartamento; }
            set
            {
                if (DataModel.IdDepartamento != value &&
                    RaisePropertyChanging("IdDepartamento", value))
                {
                    DataModel.IdDepartamento = value;
                    RaisePropertyChanged("IdDepartamento");
                }
            }
        }

        /// <summary>
        /// Identificador do funcionário associado.
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FuncDepartamento()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FuncDepartamento(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FuncDepartamento> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FuncDepartamento(Glass.Data.Model.FuncDepartamento dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
