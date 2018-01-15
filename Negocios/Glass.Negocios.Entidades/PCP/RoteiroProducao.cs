namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RoteiroProducaoLoader))]
    public class RoteiroProducao : Colosoft.Business.Entity<Glass.Data.Model.RoteiroProducao>
    {
        #region Tipos Aninhados

        class RoteiroProducaoLoader : Colosoft.Business.EntityLoader<RoteiroProducao, Glass.Data.Model.RoteiroProducao>
        {
            public RoteiroProducaoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdRoteiroProducao)
                    //.Child<RoteiroProducaoSetor, Glass.Data.Model.RoteiroProducaoSetor>("RoteiroProducaoSetor", f => f.RoteiroProducaoSetor, f => f.IdRoteiroProducao)
                    .Creator(f => new RoteiroProducao(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RoteiroProducao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RoteiroProducao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RoteiroProducao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RoteiroProducao(Glass.Data.Model.RoteiroProducao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
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
        /// Identificador do grupo.
        /// </summary>
        public uint? IdGrupoProd
        {
            get { return DataModel.IdGrupoProd; }
            set
            {
                if (DataModel.IdGrupoProd != value &&
                    RaisePropertyChanging("IdGrupoProd", value))
                {
                    DataModel.IdGrupoProd = value;
                    RaisePropertyChanged("IdGrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador do Subgripo.
        /// </summary>
        public uint? IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador do Processo.
        /// </summary>
        public uint? IdProcesso
        {
            get { return DataModel.IdProcesso; }
            set
            {
                if (DataModel.IdProcesso != value &&
                    RaisePropertyChanging("IdProcesso", value))
                {
                    DataModel.IdProcesso = value;
                    RaisePropertyChanged("IdProcesso");
                }
            }
        }

        /// <summary>
        /// Identificador da classificação.
        /// </summary>
        public int? IdClassificacaoRoteiroProducao
        {
            get { return DataModel.IdClassificacaoRoteiroProducao; }
            set
            {
                if (DataModel.IdClassificacaoRoteiroProducao != value &&
                    RaisePropertyChanging("IdClassificacaoRoteiroProducao", value))
                {
                    DataModel.IdClassificacaoRoteiroProducao = value;
                    RaisePropertyChanged("IdClassificacaoRoteiroProducao");
                }
            }
        }

        #endregion
    }
}
