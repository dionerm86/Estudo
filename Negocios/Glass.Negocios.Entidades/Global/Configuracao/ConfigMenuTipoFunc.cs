
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConfigMenuTipoFuncLoader))]
    public class ConfigMenuTipoFunc : Colosoft.Business.Entity<Glass.Data.Model.ConfigMenuTipoFunc>
    {
        #region Tipos Aninhados

        class ConfigMenuTipoFuncLoader : Colosoft.Business.EntityLoader<ConfigMenuTipoFunc, Data.Model.ConfigMenuTipoFunc>
        {
            public ConfigMenuTipoFuncLoader()
            {
                Configure()
                    .Uid(f => f.IdConfigMenuTipoFunc)
                    .FindName(new ConfigMenuTipoFuncFindNameConverter(), f => f.IdMenu)
                    .Reference<Menu, Data.Model.Menu>("Menu", f => f.Menu, f => f.IdMenu)
                    .Creator(f => new ConfigMenuTipoFunc(f));
            }

            class ConfigMenuTipoFuncFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome do cliente.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var idMenu = (int)baseInfo[0];

                    var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IProvedorConfigMenuTipoFunc>();

                    return provedor.ObtemIdentificacaoMenu(idMenu);
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigMenuTipoFunc()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigMenuTipoFunc(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConfigMenuTipoFunc> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigMenuTipoFunc(Glass.Data.Model.ConfigMenuTipoFunc dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdConfigMenuTipoFunc.
        /// </summary>
        public int IdConfigMenuTipoFunc
        {
            get { return DataModel.IdConfigMenuTipoFunc; }
            set
            {
                if (DataModel.IdConfigMenuTipoFunc != value &&
                    RaisePropertyChanging("IdConfigMenuTipoFunc", value))
                {
                    DataModel.IdConfigMenuTipoFunc = value;
                    RaisePropertyChanged("IdConfigMenuTipoFunc");
                }
            }
        }

        /// <summary>
        /// IdTipoFunc.
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
        /// IdMenu.
        /// </summary>
        public int IdMenu
        {
            get { return DataModel.IdMenu; }
            set
            {
                if (DataModel.IdMenu != value &&
                    RaisePropertyChanging("IdMenu", value))
                {
                    DataModel.IdMenu = value;
                    RaisePropertyChanged("IdMenu");
                }
            }
        }

        /// <summary>
        /// Referência ao menu.
        /// </summary>
        public Menu Menu
        {
            get { return GetReference<Menu>("Menu", true); }
        }

        #endregion
    }
}
