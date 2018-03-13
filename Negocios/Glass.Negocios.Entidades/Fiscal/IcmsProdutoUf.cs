namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a classe de Icms do produto por estado.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(IcmsProdutoUfLoader))]
    public class IcmsProdutoUf : Colosoft.Business.Entity<Glass.Data.Model.IcmsProdutoUf>
    {
        #region Tipos Aninhados

        class IcmsProdutoUfLoader : Colosoft.Business.EntityLoader<IcmsProdutoUf, Glass.Data.Model.IcmsProdutoUf>
        {
            public IcmsProdutoUfLoader()
            {
                Configure()
                    .Uid(f => f.IdIcmsProdUf)
                    .Creator(f => new IcmsProdutoUf(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd
        {
            get { return DataModel.IdProd; }
            set
            {
                if (DataModel.IdProd != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProd = value;
                    RaisePropertyChanged("IdProd");
                }
            }
        }

        /// <summary>
        /// Estado de origem.
        /// </summary>
        public string UfOrigem
        {
            get { return DataModel.UfOrigem; }
            set
            {
                if (DataModel.UfOrigem != value &&
                    RaisePropertyChanging("UfOrigem", value))
                {
                    DataModel.UfOrigem = value;
                    RaisePropertyChanged("UfOrigem");
                }
            }
        }

        /// <summary>
        /// Estado de destino.
        /// </summary>
        public string UfDestino
        {
            get { return DataModel.UfDestino; }
            set
            {
                if (DataModel.UfDestino != value &&
                    RaisePropertyChanging("UfDestino", value))
                {
                    DataModel.UfDestino = value;
                    RaisePropertyChanged("UfDestino");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de cliente associado.
        /// </summary>
        public int? IdTipoCliente
        {
            get { return DataModel.IdTipoCliente; }
            set
            {
                if (DataModel.IdTipoCliente != value &&
                    RaisePropertyChanging("IdTipoCliente", value))
                {
                    DataModel.IdTipoCliente = value;
                    RaisePropertyChanged("IdTipoCliente");
                }
            }
        }

        /// <summary>
        /// Alíquota intraestadual.
        /// </summary>
        public float AliquotaIntraestadual
        {
            get { return DataModel.AliquotaIntraestadual; }
            set
            {
                if (DataModel.AliquotaIntraestadual != value &&
                    RaisePropertyChanging("AliquotaIntraestadual", value))
                {
                    DataModel.AliquotaIntraestadual = value;
                    RaisePropertyChanged("AliquotaIntraestadual");
                }
            }
        }

        /// <summary>
        /// Alíquota interestadual.
        /// </summary>
        public float AliquotaInterestadual
        {
            get { return DataModel.AliquotaInterestadual; }
            set
            {
                if (DataModel.AliquotaInterestadual != value &&
                    RaisePropertyChanging("AliquotaInterestadual", value))
                {
                    DataModel.AliquotaInterestadual = value;
                    RaisePropertyChanged("AliquotaInterestadual");
                }
            }
        }
 
        /// <summary>
        /// Alíquota interna destinatário.
        /// </summary>
        public float AliquotaInternaDestinatario
        {
            get { return DataModel.AliquotaInternaDestinatario; }
            set
            {
                if (DataModel.AliquotaInternaDestinatario != value &&
                    RaisePropertyChanging("AliquotaInternaDestinatario", value))
                {
                    DataModel.AliquotaInternaDestinatario = value;
                    RaisePropertyChanged("AliquotaInternaDestinatario");
                }
            }
        }

        /// <summary>
        /// Alíquota FCP intraestadual.
        /// </summary>
        public float AliquotaFCPIntraestadual
        {
            get { return DataModel.AliquotaFCPIntraestadual; }
            set
            {
                if (DataModel.AliquotaFCPIntraestadual != value &&
                    RaisePropertyChanging("AliquotaFCPIntraestadual", value))
                {
                    DataModel.AliquotaFCPIntraestadual = value;
                    RaisePropertyChanged("AliquotaFCPIntraestadual");
                }
            }
        }

        /// <summary>
        /// Alíquota FCP interestadual.
        /// </summary>
        public float AliquotaFCPInterestadual
        {
            get { return DataModel.AliquotaFCPInterestadual; }
            set
            {
                if (DataModel.AliquotaFCPInterestadual != value &&
                    RaisePropertyChanging("AliquotaFCPInterestadual", value))
                {
                    DataModel.AliquotaFCPInterestadual = value;
                    RaisePropertyChanged("AliquotaFCPInterestadual");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public IcmsProdutoUf()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected IcmsProdutoUf(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.IcmsProdutoUf> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public IcmsProdutoUf(Data.Model.IcmsProdutoUf dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
