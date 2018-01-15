
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConfigMenuLoader))]
    public class ConfigMenu : Colosoft.Business.Entity<Glass.Data.Model.ConfigMenu>
    {
        #region Tipos Aninhados

        class ConfigMenuLoader : Colosoft.Business.EntityLoader<ConfigMenu, Data.Model.ConfigMenu>
        {
            public ConfigMenuLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdConfigMenu)
                    .Creator(f => new ConfigMenu(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigMenu()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigMenu(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConfigMenu> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigMenu(Glass.Data.Model.ConfigMenu dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdMenu.
        /// </summary>
        public int IdConfigMenu
        {
            get { return DataModel.IdConfigMenu; }
            set
            {
                if (DataModel.IdConfigMenu != value &&
                    RaisePropertyChanging("IdConfigMenu", value))
                {
                    DataModel.IdConfigMenu = value;
                    RaisePropertyChanged("IdConfigMenu");
                }
            }
        }

        /// <summary>
        /// IdMenuPai.
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
        /// IdMenu.
        /// </summary>
        public int IdConfig
        {
            get { return DataModel.IdConfig; }
            set
            {
                if (DataModel.IdConfig != value &&
                    RaisePropertyChanging("IdConfig", value))
                {
                    DataModel.IdConfig = value;
                    RaisePropertyChanged("IdConfig");
                }
            }
        }

        #endregion
    }
}
