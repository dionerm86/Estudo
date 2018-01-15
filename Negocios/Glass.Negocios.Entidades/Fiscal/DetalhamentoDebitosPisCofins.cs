using System;

namespace Glass.Fiscal.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(DetalhamentoDebitosPisCofinsLoader))]
    public class DetalhamentoDebitosPisCofins : Colosoft.Business.Entity<Data.Model.DetalhamentoDebitosPisCofins>
    {
        #region Tipos aninhados

        class DetalhamentoDebitosPisCofinsLoader : Colosoft.Business.EntityLoader<DetalhamentoDebitosPisCofins, Data.Model.DetalhamentoDebitosPisCofins>
        {
            public DetalhamentoDebitosPisCofinsLoader()
            {
                Configure()
                    .Uid(f => f.IdDetalhamentoPisCofins)
                    .Creator(f => new DetalhamentoDebitosPisCofins(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do débito de PIS/Cofins.
        /// </summary>
        public int IdDetalhamentoPisCofins
        {
            get { return DataModel.IdDetalhamentoPisCofins; }
            set
            {
                if (DataModel.IdDetalhamentoPisCofins != value &&
                    RaisePropertyChanging("IdDetalhamentoPisCofins", value)) {
                    DataModel.IdDetalhamentoPisCofins = value;
                    RaisePropertyChanged("IdDetalhamentoPisCofins");
                }
            }
        }

        /// <summary>
        /// Data de pagamento.
        /// </summary>
        public DateTime DataPagamento
        {
            get { return DataModel.DataPagamento; }
            set
            {
                if (DataModel.DataPagamento != value &&
                    RaisePropertyChanging("DataPagamento", value)) {
                    DataModel.DataPagamento = value;
                    RaisePropertyChanged("DataPagamento");
                }
            }
        }

        /// <summary>
        /// Tipo de imposto.
        /// </summary>
        public Sync.Fiscal.Enumeracao.TipoImposto TipoImposto
        {
            get { return (Sync.Fiscal.Enumeracao.TipoImposto)DataModel.TipoImposto; }
            set
            {
                if (DataModel.TipoImposto != (int)value &&
                    RaisePropertyChanging("TipoImposto", value)) {
                    DataModel.TipoImposto = (int)value;
                    RaisePropertyChanged("TipoImposto");
                }
            }
        }

        /// <summary>
        /// Código de receita.
        /// </summary>
        public string CodigoReceita
        {
            get { return DataModel.CodigoReceita; }
            set
            {
                if (DataModel.CodigoReceita != value &&
                    RaisePropertyChanging("CodigoReceita", value)) {
                    DataModel.CodigoReceita = value;
                    RaisePropertyChanged("CodigoReceita");
                }
            }
        }

        /// <summary>
        /// Contribuição cumulativa?.
        /// </summary>
        public bool Cumulativo
        {
            get { return DataModel.Cumulativo; }
            set
            {
                if (DataModel.Cumulativo != value &&
                    RaisePropertyChanging("Cumulativo", value)) {
                    DataModel.Cumulativo = value;
                    RaisePropertyChanged("Cumulativo");
                }
            }
        }

        /// <summary>
        /// Valor do pagamento da contribuição.
        /// </summary>
        public decimal ValorPagamento
        {
            get { return DataModel.ValorPagamento; }
            set
            {
                if (DataModel.ValorPagamento != value &&
                    RaisePropertyChanging("ValorPagamento", value)) {
                    DataModel.ValorPagamento = value;
                    RaisePropertyChanged("ValorPagamento");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public DetalhamentoDebitosPisCofins()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected DetalhamentoDebitosPisCofins(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.DetalhamentoDebitosPisCofins> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public DetalhamentoDebitosPisCofins(Data.Model.DetalhamentoDebitosPisCofins dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
