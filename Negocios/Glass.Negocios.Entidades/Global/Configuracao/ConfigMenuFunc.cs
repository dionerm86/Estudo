
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConfigMenuFuncLoader))]
    public class ConfigMenuFunc : Colosoft.Business.Entity<Glass.Data.Model.ConfigMenuFunc>
    {
        #region Tipos Aninhados

        class ConfigMenuFuncLoader : Colosoft.Business.EntityLoader<ConfigMenuFunc, Data.Model.ConfigMenuFunc>
        {
            public ConfigMenuFuncLoader()
            {
                Configure()
                    .Uid(f => f.IdConfigMenuFunc)
                    .FindName(new ConfigMenuFuncFindNameConverter(), f => f.IdMenu)
                    .Reference<Menu, Data.Model.Menu>("Menu", f => f.Menu, f => f.IdMenu)
                    .Creator(f => new ConfigMenuFunc(f));
            }

            class ConfigMenuFuncFindNameConverter : Colosoft.IFindNameConverter
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
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IProvedorConfigMenuFunc>();

                    return provedor.ObtemIdentificacaoMenu(idMenu);
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigMenuFunc()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConfigMenuFunc(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConfigMenuFunc> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConfigMenuFunc(Glass.Data.Model.ConfigMenuFunc dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdConfigMenuFunc.
        /// </summary>
        public int IdConfigMenuFunc
        {
            get { return DataModel.IdConfigMenuFunc; }
            set
            {
                if (DataModel.IdConfigMenuFunc != value &&
                    RaisePropertyChanging("IdConfigMenuFunc", value))
                {
                    DataModel.IdConfigMenuFunc = value;
                    RaisePropertyChanged("IdConfigMenuFunc");
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
        /// Referência ao menu.
        /// </summary>
        public Menu Menu
        {
            get { return GetReference<Menu>("Menu", true); }
        }

        #endregion
    }
}
