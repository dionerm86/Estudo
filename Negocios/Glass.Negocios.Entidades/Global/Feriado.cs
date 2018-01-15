namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do feriado.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FeriadoLoader))]
    public class Feriado : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Feriado>
    {
        #region Tipos Aninhados

        class FeriadoLoader : Colosoft.Business.EntityLoader<Feriado, Data.Model.Feriado>
        {
            public FeriadoLoader()
            {
                Configure()
                    .Uid(f => f.IdFeriado)
                    .FindName(f => f.Descricao)
                    .Creator(f => new Feriado(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do feriado.
        /// </summary>
        public int IdFeriado
        {
            get { return DataModel.IdFeriado; }
            set
            {
                if (DataModel.IdFeriado != value &&
                    RaisePropertyChanging("IdFeriado", value))
                {
                    DataModel.IdFeriado = value;
                    RaisePropertyChanged("IdFeriado");
                }
            }
        }

        /// <summary>
        /// Dia.
        /// </summary>
        public int Dia
        {
            get { return DataModel.Dia; }
            set
            {
                if (DataModel.Dia != value &&
                    RaisePropertyChanging("Dia", value))
                {
                    DataModel.Dia = value;
                    RaisePropertyChanged("Dia");
                }
            }
        }

        /// <summary>
        /// Mês.
        /// </summary>
        public int Mes
        {
            get { return DataModel.Mes; }
            set
            {
                if (DataModel.Mes != value &&
                    RaisePropertyChanging("Mes", value))
                {
                    DataModel.Mes = value;
                    RaisePropertyChanged("Mes");
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Feriado()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Feriado(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Feriado> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Feriado(Data.Model.Feriado dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
