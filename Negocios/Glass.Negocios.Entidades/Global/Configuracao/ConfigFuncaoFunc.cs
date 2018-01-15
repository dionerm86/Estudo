
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConfigFuncaoFuncLoader))]
    public class ConfigFuncaoFunc : Colosoft.Business.Entity<Glass.Data.Model.ConfigFuncaoFunc>
    {
        #region Tipos Aninhados

        class ConfigFuncaoFuncLoader : Colosoft.Business.EntityLoader<ConfigFuncaoFunc, Data.Model.ConfigFuncaoFunc>
        {
            public ConfigFuncaoFuncLoader()
            {
                Configure()
                    .Uid(f => f.IdConfigFuncaoFunc)
                    .FindName(new ConfigFuncaoFuncFindNameConverter(), f => f.IdFuncaoMenu)
                    .Reference<FuncaoMenu, Data.Model.FuncaoMenu>("FuncaoMenu", f => f.FuncaoMenu, f => f.IdFuncaoMenu)
                    .Creator(f => new ConfigFuncaoFunc(f));
            }

            class ConfigFuncaoFuncFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome do cliente.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var idFuncaoMenu = (int)baseInfo[0];

                    var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IProvedorConfigFuncaoFunc>();

                    return provedor.ObtemIdentificacaoFuncaoMenu(idFuncaoMenu);
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigFuncaoFunc()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigFuncaoFunc(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConfigFuncaoFunc> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigFuncaoFunc(Glass.Data.Model.ConfigFuncaoFunc dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdConfigFuncaoFunc.
        /// </summary>
        public int IdConfigFuncaoFunc
        {
            get { return DataModel.IdConfigFuncaoFunc; }
            set
            {
                if (DataModel.IdConfigFuncaoFunc != value &&
                    RaisePropertyChanging("IdConfigFuncaoFunc", value))
                {
                    DataModel.IdConfigFuncaoFunc = value;
                    RaisePropertyChanged("IdConfigFuncaoFunc");
                }
            }
        }

        /// <summary>
        /// IdFuncaoMenu.
        /// </summary>
        public int IdFuncaoMenu
        {
            get { return DataModel.IdFuncaoMenu; }
            set
            {
                if (DataModel.IdFuncaoMenu != value &&
                    RaisePropertyChanging("IdFuncaoMenu", value))
                {
                    DataModel.IdFuncaoMenu = value;
                    RaisePropertyChanged("IdFuncaoMenu");
                }
            }
        }

        /// <summary>
        /// IdFunc.
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
        /// Referência à função de menu.
        /// </summary>
        public FuncaoMenu FuncaoMenu
        {
            get { return GetReference<FuncaoMenu>("FuncaoMenu"); }
        }

        #endregion
    }
}
