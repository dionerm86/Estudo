
namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio dos juros da parcela do cartão.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(JurosParcelaCartaoLoader))]
    public class JurosParcelaCartao : Colosoft.Business.Entity<Glass.Data.Model.JurosParcelaCartao>
    {
        #region Tipos Aninhados

        class JurosParcelaCartaoLoader : Colosoft.Business.EntityLoader<JurosParcelaCartao, Glass.Data.Model.JurosParcelaCartao>
        {
            public JurosParcelaCartaoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdJurosParcela)
                    .Creator(f => new JurosParcelaCartao(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public JurosParcelaCartao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected JurosParcelaCartao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.JurosParcelaCartao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public JurosParcelaCartao(Glass.Data.Model.JurosParcelaCartao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da instanci.
        /// </summary>
        public uint IdJurosParcela
        {
            get { return DataModel.IdJurosParcela; }
            set
            {
                if (DataModel.IdJurosParcela != value &&
                    RaisePropertyChanging("IdJurosParcela", value))
                {
                    DataModel.IdJurosParcela = value;
                    RaisePropertyChanged("IdJurosParcela");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de cartão.
        /// </summary>
        public int IdTipoCartao
        {
            get { return DataModel.IdTipoCartao; }
            set
            {
                if (DataModel.IdTipoCartao != value &&
                    RaisePropertyChanging("IdTipoCartao", value))
                {
                    DataModel.IdTipoCartao = value;
                    RaisePropertyChanged("IdTipoCartao");
                }
            }
        }

        /// <summary>
        /// Identificador do loja.
        /// </summary>
        public uint? IdLoja
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
        /// Número de parcelas.
        /// </summary>
        public int NumParc
        {
            get { return DataModel.NumParc; }
            set
            {
                if (DataModel.NumParc != value &&
                    RaisePropertyChanging("NumParc", value))
                {
                    DataModel.NumParc = value;
                    RaisePropertyChanged("NumParc");
                }
            }
        }

        /// <summary>
        /// Juros.
        /// </summary>
        public decimal Juros
        {
            get { return (decimal)DataModel.Juros; }
            set
            {
                if ((decimal)DataModel.Juros != value &&
                    RaisePropertyChanging("Juros", value))
                {
                    DataModel.Juros = (float)value;
                    RaisePropertyChanged("Juros");
                }
            }
        }

        #endregion
    }
}
