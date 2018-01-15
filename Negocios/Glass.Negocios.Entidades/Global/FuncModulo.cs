namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócios que faz ligação entre o funcionário e o módulo.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FuncModuloLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.ControleUsuario, typeof(FuncModuloLogIdCreator))]
    public class FuncModulo : Colosoft.Business.Entity<Glass.Data.Model.FuncModulo>
    {
        #region Tipos Aninhados

        class FuncModuloLogIdCreator : Glass.Negocios.LogIdCreator<FuncModulo>
        {
            public override int Create(FuncModulo entity)
            {
                return Glass.Conversoes.StrParaInt(entity.IdModulo.ToString() + entity.IdFunc.ToString().PadLeft(4, '0'));
            }
        }

        class FuncModuloLoader : Colosoft.Business.EntityLoader<FuncModulo, Glass.Data.Model.FuncModulo>
        {
            public FuncModuloLoader()
            {
                Configure()
                    .Keys(f => f.IdFunc, f => f.IdModulo)
                    .Creator(f => new FuncModulo(f));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Identificador do módulo associado.
        /// </summary>
        public int IdModulo
        {
            get { return DataModel.IdModulo; }
            set
            {
                if (DataModel.IdModulo != value &&
                    RaisePropertyChanging("IdModulo", value))
                {
                    DataModel.IdModulo = value;
                    RaisePropertyChanged("IdModulo");
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

        /// <summary>
        /// Define se é permitir ou não o acesso ao módulo.
        /// </summary>
        public bool Permitir
        {
            get { return DataModel.Permitir; }
            set
            {
                if (DataModel.Permitir != value &&
                    RaisePropertyChanging("Permitir", value))
                {
                    DataModel.Permitir = value;
                    RaisePropertyChanged("Permitir");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FuncModulo()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FuncModulo(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FuncModulo> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FuncModulo(Glass.Data.Model.FuncModulo dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
