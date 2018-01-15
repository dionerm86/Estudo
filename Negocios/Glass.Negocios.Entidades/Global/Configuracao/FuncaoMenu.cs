
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(FuncaoMenuLoader))]
    public class FuncaoMenu : Colosoft.Business.Entity<Glass.Data.Model.FuncaoMenu>
    {
        #region Tipos Aninhados

        class FuncaoMenuLoader : Colosoft.Business.EntityLoader<FuncaoMenu, Data.Model.FuncaoMenu>
        {
            public FuncaoMenuLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdFuncaoMenu)
                    .Creator(f => new FuncaoMenu(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FuncaoMenu()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FuncaoMenu(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FuncaoMenu> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FuncaoMenu(Glass.Data.Model.FuncaoMenu dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

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
        /// IdFuncao.
        /// </summary>
        public int IdFuncao
        {
            get { return DataModel.IdFuncao; }
            set
            {
                if (DataModel.IdFuncao != value &&
                    RaisePropertyChanging("IdFuncao", value))
                {
                    DataModel.IdFuncao = value;
                    RaisePropertyChanged("IdFuncao");
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
        /// Descricao.
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

        #endregion
    }
}
