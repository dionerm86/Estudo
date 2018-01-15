namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do tipo de funcionário.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoFuncionarioLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.TipoFuncionario)]
    public class TipoFuncionario : Colosoft.Business.Entity<Data.Model.TipoFuncionario>
    {
        #region Tipos Aninhados

        class TipoFuncionarioLoader : Colosoft.Business.EntityLoader<TipoFuncionario, Data.Model.TipoFuncionario>
        {
            public TipoFuncionarioLoader()
            {
                Configure()
                    .Uid(f => f.IdTipoFuncionario)
                    .FindName(f => f.Descricao)
                    .Child<Global.Negocios.Entidades.ConfigMenuTipoFunc, Data.Model.ConfigMenuTipoFunc>("ConfigsMenuTipoFunc", f => f.ConfigsMenuTipoFunc, f => f.IdTipoFunc)
                    .Log("ConfigsMenuTipoFunc", "Menus")
                    .Child<Global.Negocios.Entidades.ConfigFuncaoTipoFunc, Data.Model.ConfigFuncaoTipoFunc>("ConfigsFuncaoTipoFunc", f => f.ConfigsFuncaoTipoFunc, f => f.IdTipoFunc)
                    .Log("ConfigsFuncaoTipoFunc", "Funções")
                    .Creator(f => new TipoFuncionario(f));
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigMenuTipoFunc> _configsMenuTipoFunc;
        private Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigFuncaoTipoFunc> _configsFuncaoTipoFunc;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do tipo de funcionario.
        /// </summary>
        public int IdTipoFuncionario
        {
            get { return DataModel.IdTipoFuncionario; }
            set
            {
                if (DataModel.IdTipoFuncionario != value &&
                    RaisePropertyChanging("IdTipoFuncionario", value))
                {
                    DataModel.IdTipoFuncionario = value;
                    RaisePropertyChanged("IdTipoFuncionario");
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
        /// Identifica se é um tipo do sistema.
        /// </summary>
        public bool TipoSistema
        {
            get
            {
                foreach (GenericModel g in
                    Glass.Data.EFD.DataSourcesEFD
                         .Instance.GetFromEnum(typeof(Glass.Data.Helper.Utils.TipoFuncionario), null, false))
                    if (g.Id == IdTipoFuncionario)
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Configuração dos menus
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigMenuTipoFunc> ConfigsMenuTipoFunc
        {
            get { return _configsMenuTipoFunc; }
        }

        /// <summary>
        /// Configuração das funções
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigFuncaoTipoFunc> ConfigsFuncaoTipoFunc
        {
            get { return _configsFuncaoTipoFunc; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TipoFuncionario()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoFuncionario(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.TipoFuncionario> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _configsMenuTipoFunc = GetChild<Global.Negocios.Entidades.ConfigMenuTipoFunc>(args.Children, "ConfigsMenuTipoFunc");
            _configsFuncaoTipoFunc = GetChild<Global.Negocios.Entidades.ConfigFuncaoTipoFunc>(args.Children, "ConfigsFuncaoTipoFunc");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoFuncionario(Data.Model.TipoFuncionario dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _configsMenuTipoFunc = CreateChild<Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigMenuTipoFunc>>("ConfigsMenuTipoFunc");
            _configsFuncaoTipoFunc = CreateChild<Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ConfigFuncaoTipoFunc>>("ConfigsFuncaoTipoFunc");
        }

        #endregion
    }
}

