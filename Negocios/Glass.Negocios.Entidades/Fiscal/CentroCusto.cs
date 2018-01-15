namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do centro de custo.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CentroCustoLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.CentroCusto)]
    public class CentroCusto : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.CentroCusto>
    {
        #region Tipos aninhados

        class CentroCustoLoader : Colosoft.Business.EntityLoader<CentroCusto, Data.Model.CentroCusto>
        {
            public CentroCustoLoader()
            {
                Configure()
                    .Uid(f => f.IdCentroCusto)
                    .FindName(f => f.Descricao)
                    .Creator(f => new CentroCusto(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do centro de custo.
        /// </summary>
        public int IdCentroCusto
        {
            get { return DataModel.IdCentroCusto; }
            set
            {
                if (DataModel.IdCentroCusto != value &&
                    RaisePropertyChanging("IdCentroCusto", value))
                {
                    DataModel.IdCentroCusto = value;
                    RaisePropertyChanged("IdCentroCusto");
                }
            }
        }

        /// <summary>
        /// Código da loja do centro de custo.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Descrição do centro de custo.
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
        /// Tipo de centro de custo.
        /// </summary>
        public Sync.Fiscal.Enumeracao.CentroCusto.Codigo CodigoTipo
        {
            get { return (Sync.Fiscal.Enumeracao.CentroCusto.Codigo)DataModel.CodigoTipo; }
            set
            {
                if (DataModel.CodigoTipo != (int)value &&
                    RaisePropertyChanging("CodigoTipo", value))
                {
                    DataModel.CodigoTipo = (int)value;
                    RaisePropertyChanged("CodigoTipo");
                }
            }
        }

        #endregion

        #region Construtor

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CentroCusto()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CentroCusto(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CentroCusto> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CentroCusto(Data.Model.CentroCusto dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
