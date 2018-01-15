
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(MenuLoader))]
    public class Menu : Colosoft.Business.Entity<Glass.Data.Model.Menu>
    {
        #region Tipos Aninhados

        class MenuLoader : Colosoft.Business.EntityLoader<Menu, Data.Model.Menu>
        {
            public MenuLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdMenu)
                    .Child<ConfigMenu, Data.Model.ConfigMenu>("ConfigsMenu", f => f.ConfigsMenu, f => f.IdConfigMenu)
                    .Child<FuncaoMenu, Data.Model.FuncaoMenu>("FuncoesMenu", f => f.FuncoesMenu, f => f.IdFuncaoMenu)
                    .Creator(f => new Menu(f));
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<ConfigMenu> _configsMenu;
        private Colosoft.Business.IEntityChildrenList<FuncaoMenu> _funcoesMenu;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Menu()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Menu(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Menu> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _configsMenu = GetChild<ConfigMenu>(args.Children, "ConfigsMenu");
            _funcoesMenu = GetChild<FuncaoMenu>(args.Children, "FuncoesMenu");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Menu(Glass.Data.Model.Menu dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _configsMenu = CreateChild<Colosoft.Business.IEntityChildrenList<ConfigMenu>>("ConfigsMenu");
            _funcoesMenu = CreateChild<Colosoft.Business.IEntityChildrenList<FuncaoMenu>>("FuncoesMenu");
        }

        #endregion

        #region Propriedades

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
        /// IdMenuPai.
        /// </summary>
        public int? IdMenuPai
        {
            get { return DataModel.IdMenuPai; }
            set
            {
                if (DataModel.IdMenuPai != value &&
                    RaisePropertyChanging("IdMenuPai", value))
                {
                    DataModel.IdMenuPai = value;
                    RaisePropertyChanged("IdMenuPai");
                }
            }
        }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        /// <summary>
        /// URL.
        /// </summary>
        public string Url
        {
            get { return DataModel.Url; }
            set
            {
                if (DataModel.Url != value &&
                    RaisePropertyChanging("Url", value))
                {
                    DataModel.Url = value;
                    RaisePropertyChanged("Url");
                }
            }
        }

        /// <summary>
        /// IdModulo.
        /// </summary>
        public Data.Helper.Config.Modulo IdModulo
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
        /// NumSeq.
        /// </summary>
        public int NumSeq
        {
            get { return DataModel.NumSeq; }
            set
            {
                if (DataModel.NumSeq != value &&
                    RaisePropertyChanging("NumSeq", value))
                {
                    DataModel.NumSeq = value;
                    RaisePropertyChanged("NumSeq");
                }
            }
        }

        /// <summary>
        /// Observacao.
        /// </summary>
        public string Observacao
        {
            get { return DataModel.Observacao; }
            set
            {
                if (DataModel.Observacao != value &&
                    RaisePropertyChanging("Observacao", value))
                {
                    DataModel.Observacao = value;
                    RaisePropertyChanged("Observacao");
                }
            }
        }

        /// <summary>
        /// ExibirLite.
        /// </summary>
        public bool ExibirLite
        {
            get { return DataModel.ExibirLite; }
            set
            {
                if (DataModel.ExibirLite != value &&
                    RaisePropertyChanging("ExibirLite", value))
                {
                    DataModel.ExibirLite = value;
                    RaisePropertyChanged("ExibirLite");
                }
            }
        }

        /// <summary>
        /// Configuração dos menus
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<ConfigMenu> ConfigsMenu
        {
            get { return _configsMenu; }
        }

        /// <summary>
        /// Funções do menu
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<FuncaoMenu> FuncoesMenu
        {
            get { return _funcoesMenu; }
        }

        #endregion
    }
}
