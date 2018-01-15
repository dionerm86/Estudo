namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do beneficiamento do setor.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SetorBenefLoader))]
    public class SetorBenef : Colosoft.Business.Entity<Glass.Data.Model.SetorBenef>
    {
        #region Tipos Aninhados

        class SetorBenefLoader : Colosoft.Business.EntityLoader<SetorBenef, Glass.Data.Model.SetorBenef>
        {
            public SetorBenefLoader()
            {
                Configure()
                    .Uid(f => f.IdSetorBenef)
                    .Creator(f => new SetorBenef(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do relacionamento.
        /// </summary>
        public int IdSetorBenef
        {
            get { return DataModel.IdSetorBenef; }
            set
            {
                if (DataModel.IdSetorBenef != value &&
                    RaisePropertyChanging("IdSetorBenef", value))
                {
                    DataModel.IdSetorBenef = value;
                    RaisePropertyChanged("IdSetorBenef");
                }
            }
        }

        /// <summary>
        /// Identificador do setor associado.
        /// </summary>
        public int IdSetor
        {
            get { return DataModel.IdSetor; }
            set
            {
                if (DataModel.IdSetor != value &&
                    RaisePropertyChanging("IdSetor", value))
                {
                    DataModel.IdSetor = value;
                    RaisePropertyChanged("IdSetor");
                }
            }
        }

        /// <summary>
        /// Identificador do beneficiamento.
        /// </summary>
        public int IdBenefConfig
        {
            get { return DataModel.IdBenefConfig; }
            set
            {
                if (DataModel.IdBenefConfig != value &&
                    RaisePropertyChanging("IdBenefConfig", value))
                {
                    DataModel.IdBenefConfig = value;
                    RaisePropertyChanged("IdBenefConfig");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public SetorBenef()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected SetorBenef(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.SetorBenef> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public SetorBenef(Glass.Data.Model.SetorBenef dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }
        
        #endregion
    }
}
