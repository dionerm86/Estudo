namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio de parcelas não usar.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ParcelasNaoUsarLoader))]
    public class ParcelasNaoUsar : Colosoft.Business.Entity<Glass.Data.Model.ParcelasNaoUsar>
    {
        #region Tipos Aninhados

        class ParcelasNaoUsarLoader : Colosoft.Business.EntityLoader<ParcelasNaoUsar, Glass.Data.Model.ParcelasNaoUsar>
        {
            public ParcelasNaoUsarLoader()
            {
                Configure()
                    .Keys(f => f.IdParcela, f => f.IdCliente, f => f.IdFornecedor)
                    .FindName(new ParcelaNaoUsarFindNameConverter(), f => f.IdParcela)
                    .Reference<Parcelas, Data.Model.Parcelas>("Parcela", f => f.Parcela, f => f.IdParcela)
                    .Creator(f => new ParcelasNaoUsar(f));
            }
        }

        /// <summary>
        /// Implementação do conversor do nome.
        /// </summary>
        class ParcelaNaoUsarFindNameConverter : Colosoft.IFindNameConverter
        {
            /// <summary>
            /// Converte os valores informados para o nome de pesquisa.
            /// </summary>
            /// <param name="baseInfo"></param>
            /// <returns></returns>
            public string Convert(object[] baseInfo)
            {
                var idParcela = (int)baseInfo[0];

                var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorParcelasNaoUsar>();

                return provedor.ObtemIdentificacao(idParcela);
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da parcela.
        /// </summary>
        public int IdParcela
        {
            get { return DataModel.IdParcela; }
            set
            {
                if (DataModel.IdParcela != value &&
                    RaisePropertyChanging("IdParcela", value))
                {
                    DataModel.IdParcela = value;
                    RaisePropertyChanged("IdParcela");
                }
            }
        }

        /// <summary>
        /// Identificador do cliente associado.
        /// </summary>
        public int? IdCliente
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
        /// Identificador do fornecedor associado.
        /// </summary>
        public int? IdFornecedor
        {
            get { return DataModel.IdFornecedor; }
            set
            {
                if (DataModel.IdFornecedor != value &&
                    RaisePropertyChanging("IdFornecedor", value))
                {
                    DataModel.IdFornecedor = value;
                    RaisePropertyChanged("IdFornecedor");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ParcelasNaoUsar()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ParcelasNaoUsar(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ParcelasNaoUsar> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ParcelasNaoUsar(Glass.Data.Model.ParcelasNaoUsar dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// 
        /// </summary>
        public Parcelas Parcela
        {
            get { return GetReference<Parcelas>("Parcela", true); }
        }

        #endregion
    }
}
