namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio de formas de pagamento de cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FormaPagtoClienteLoader))]
    public class FormaPagtoCliente : Colosoft.Business.Entity<Data.Model.FormaPagtoCliente>
    {
        #region Tipos Aninhados

        class FormaPagtoClienteLoader : Colosoft.Business.EntityLoader<FormaPagtoCliente, Glass.Data.Model.FormaPagtoCliente>
        {
            public FormaPagtoClienteLoader()
            {
                Configure()
                    .Keys(f => f.IdCliente, f => f.IdFormaPagto)
                    .FindName(new FormaPagtoClienteFindNameConverter(), f => f.IdFormaPagto)
                    .Creator(f => new FormaPagtoCliente(f));
            }
        }

        /// <summary>
        /// Implementação do conversor do nome.
        /// </summary>
        class FormaPagtoClienteFindNameConverter : Colosoft.IFindNameConverter
        {
            /// <summary>
            /// Converte os valores informados para o nome de pesquisa.
            /// </summary>
            /// <param name="baseInfo"></param>
            /// <returns></returns>
            public string Convert(object[] baseInfo)
            {
                var idFormaPagto = (int)baseInfo[0];

                var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.Entidades.IProvedorFormaPagtoCliente>();

                return provedor.ObtemIdentificacao(idFormaPagto);
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do cliente.
        /// </summary>
        public int IdCliente
        {
            get { return DataModel.IdCliente; }
            set
            {
                if (DataModel.IdCliente != value &&
                    RaisePropertyChanging("IdCliente", value))
                {
                    DataModel.IdCliente = value;
                    RaisePropertyChanged("IdCliente");
                }
            }
        }

        /// <summary>
        /// Código da forma de pagamento.
        /// </summary>
        public int IdFormaPagto
        {
            get { return DataModel.IdFormaPagto; }
            set
            {
                if (DataModel.IdFormaPagto != value &&
                    RaisePropertyChanging("IdFormaPagto", value))
                {
                    DataModel.IdFormaPagto = value;
                    RaisePropertyChanged("IdFormaPagto");
                }
            }
        }

        /// <summary>
        /// Tipo de venda.
        /// </summary>
        public int TipoVenda
        {
            get { return DataModel.TipoVenda; }
            set
            {
                if (DataModel.TipoVenda != value &&
                    RaisePropertyChanging("TipoVenda", value))
                {
                    DataModel.TipoVenda = value;
                    RaisePropertyChanged("TipoVenda");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FormaPagtoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FormaPagtoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FormaPagtoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FormaPagtoCliente(Data.Model.FormaPagtoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
