namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RoteiroProducaoSetorLoader))]
    public class RoteiroProducaoSetor : Colosoft.Business.Entity<Glass.Data.Model.RoteiroProducaoSetor>
    {
        #region Tipos Aninhados

        class RoteiroProducaoSetorLoader : Colosoft.Business.EntityLoader<RoteiroProducaoSetor, Glass.Data.Model.RoteiroProducaoSetor>
        {
            public RoteiroProducaoSetorLoader()
            {
                Configure()
                    .Keys(f => f.IdRoteiroProducao, f => f.IdSetor)
                    .Creator(f => new RoteiroProducaoSetor(f));
            }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RoteiroProducaoSetor()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RoteiroProducaoSetor(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RoteiroProducaoSetor> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RoteiroProducaoSetor(Glass.Data.Model.RoteiroProducaoSetor dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Identificador do Roteiro.
        /// </summary>
        public int IdRoteiroProducao
        {
            get { return DataModel.IdRoteiroProducao; }
            set
            {
                if (DataModel.IdRoteiroProducao != value &&
                    RaisePropertyChanging("IdRoteiroProducao", value))
                {
                    DataModel.IdRoteiroProducao = value;
                    RaisePropertyChanged("IdRoteiroProducao");
                }
            }
        }

        /// <summary>
        /// Identificador do Setor.
        /// </summary>
        public uint IdSetor
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

        #endregion
    }
}
