
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConfigFuncaoTipoFuncLoader))]
    public class ConfigFuncaoTipoFunc : Colosoft.Business.Entity<Glass.Data.Model.ConfigFuncaoTipoFunc>
    {
        #region Tipos Aninhados

        class ConfigFuncaoTipoFuncLoader : Colosoft.Business.EntityLoader<ConfigFuncaoTipoFunc, Data.Model.ConfigFuncaoTipoFunc>
        {
            public ConfigFuncaoTipoFuncLoader()
            {
                Configure()
                    .Uid(f => f.IdConfigFuncaoTipoFunc)
                    .FindName(new ConfigFuncaoTipoFuncFindNameConverter(), f => f.IdFuncaoMenu)
                    .Reference<FuncaoMenu, Data.Model.FuncaoMenu>("FuncaoMenu", f => f.FuncaoMenu, f => f.IdFuncaoMenu)
                    .Creator(f => new ConfigFuncaoTipoFunc(f));
            }

            class ConfigFuncaoTipoFuncFindNameConverter : Colosoft.IFindNameConverter
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
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IProvedorConfigFuncaoTipoFunc>();

                    return provedor.ObtemIdentificacaoFuncaoMenu(idFuncaoMenu);
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigFuncaoTipoFunc()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigFuncaoTipoFunc(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConfigFuncaoTipoFunc> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigFuncaoTipoFunc(Glass.Data.Model.ConfigFuncaoTipoFunc dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdConfigFuncaoTipoFunc.
        /// </summary>
        public int IdConfigFuncaoTipoFunc
        {
            get { return DataModel.IdConfigFuncaoTipoFunc; }
            set
            {
                if (DataModel.IdConfigFuncaoTipoFunc != value &&
                    RaisePropertyChanging("IdConfigFuncaoTipoFunc", value))
                {
                    DataModel.IdConfigFuncaoTipoFunc = value;
                    RaisePropertyChanged("IdConfigFuncaoTipoFunc");
                }
            }
        }

        /// <summary>
        /// IdConfigMenuTipoFunc.
        /// </summary>
        public int IdTipoFunc
        {
            get { return DataModel.IdTipoFunc; }
            set
            {
                if (DataModel.IdTipoFunc != value &&
                    RaisePropertyChanging("IdTipoFunc", value))
                {
                    DataModel.IdTipoFunc = value;
                    RaisePropertyChanged("IdTipoFunc");
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
        /// Referência à função de menu.
        /// </summary>
        public FuncaoMenu FuncaoMenu
        {
            get { return GetReference<FuncaoMenu>("FuncaoMenu", true); }
        }

        #endregion
    }
}
