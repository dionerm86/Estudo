namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do preço da configuração do beneficiamento.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(BenefConfigPrecoLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.BenefConfigPreco)]
    public class BenefConfigPreco : Colosoft.Business.Entity<Glass.Data.Model.BenefConfigPreco>
    {
        #region Tipos Aninhados

        class BenefConfigPrecoLoader : Colosoft.Business.EntityLoader<BenefConfigPreco, Glass.Data.Model.BenefConfigPreco>
        {
            public BenefConfigPrecoLoader()
            {
                Configure()
                    .Uid(f => f.IdBenefConfigPreco)
                    .Creator(f => new BenefConfigPreco(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do preço.
        /// </summary>
        public int IdBenefConfigPreco
        {
            get { return DataModel.IdBenefConfigPreco; }
            set
            {
                if (DataModel.IdBenefConfigPreco != value &&
                    RaisePropertyChanging("IdBenefConfigPreco", value))
                {
                    DataModel.IdBenefConfigPreco = value;
                    RaisePropertyChanged("IdBenefConfigPreco");
                }
            }
        }

        /// <summary>
        /// Identificador da configuração do beneficiamento associado.
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

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        public int? IdSubgrupoProd
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
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro
        {
            get { return DataModel.IdCorVidro; }
            set
            {
                if (DataModel.IdCorVidro != value &&
                    RaisePropertyChanging("IdCorVidro", value))
                {
                    DataModel.IdCorVidro = value;
                    RaisePropertyChanged("IdCorVidro");
                }
            }
        }

        /// <summary>
        /// Espessura do vidro.
        /// </summary>
        public float? Espessura
        {
            get { return DataModel.Espessura; }
            set
            {
                if (DataModel.Espessura != value &&
                    RaisePropertyChanging("Espessura", value))
                {
                    DataModel.Espessura = value;
                    RaisePropertyChanged("Espessura");
                }
            }
        }

        /// <summary>
        /// Valor atacado.
        /// </summary>
        public decimal ValorAtacado
        {
            get { return DataModel.ValorAtacado; }
            set
            {
                if (DataModel.ValorAtacado != value &&
                    RaisePropertyChanging("ValorAtacado", value))
                {
                    DataModel.ValorAtacado = value;
                    RaisePropertyChanged("ValorAtacado");
                }
            }
        }

        /// <summary>
        /// Valor balcão.
        /// </summary>
        public decimal ValorBalcao
        {
            get { return DataModel.ValorBalcao; }
            set
            {
                if (DataModel.ValorBalcao != value &&
                    RaisePropertyChanging("ValorBalcao", value))
                {
                    DataModel.ValorBalcao = value;
                    RaisePropertyChanged("ValorBalcao");
                }
            }
        }

        /// <summary>
        /// Valor obra.
        /// </summary>
        public decimal ValorObra
        {
            get { return DataModel.ValorObra; }
            set
            {
                if (DataModel.ValorObra != value &&
                    RaisePropertyChanging("ValorObra", value))
                {
                    DataModel.ValorObra = value;
                    RaisePropertyChanged("ValorObra");
                }
            }
        }

        /// <summary>
        /// Custo.
        /// </summary>
        public decimal Custo
        {
            get { return DataModel.Custo; }
            set
            {
                if (DataModel.Custo != value &&
                    RaisePropertyChanging("Custo", value))
                {
                    DataModel.Custo = value;
                    RaisePropertyChanged("Custo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public BenefConfigPreco()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected BenefConfigPreco(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.BenefConfigPreco> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public BenefConfigPreco(Glass.Data.Model.BenefConfigPreco dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
