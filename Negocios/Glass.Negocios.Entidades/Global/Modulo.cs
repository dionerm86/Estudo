namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do módulo de segurança do sistema.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ModuloLoader))]
    public class Modulo : Colosoft.Business.Entity<Glass.Data.Model.Modulo>
    {
        #region Tipos Aninhados

        class ModuloLoader : Colosoft.Business.EntityLoader<Modulo, Glass.Data.Model.Modulo>
        {
            public ModuloLoader()
            {
                Configure()
                    .Uid(f => f.IdModulo)
                    .FindName(f => f.Descricao)
                    .Creator(f => new Modulo(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do módulo.
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
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Grupo.
        /// </summary>
        public string Grupo
        {
            get { return DataModel.Grupo; }
            set
            {
                if (DataModel.Grupo != value &&
                    RaisePropertyChanging("Grupo", value))
                {
                    DataModel.Grupo = value;
                    RaisePropertyChanged("Grupo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Modulo()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Modulo(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Modulo> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Modulo(Glass.Data.Model.Modulo dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
