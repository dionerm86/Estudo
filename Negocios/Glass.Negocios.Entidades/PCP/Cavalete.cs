
namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(CavaleteLoader))]
    [Glass.Negocios.ControleAlteracao(Glass.Data.Model.LogAlteracao.TabelaAlteracao.Setor)]
    public class Cavalete : Colosoft.Business.Entity<Glass.Data.Model.Cavalete>
    {
        #region Tipos Aninhados

        class CavaleteLoader : Colosoft.Business.EntityLoader<Cavalete, Glass.Data.Model.Cavalete>
        {
            public CavaleteLoader()
            {
                Configure()
                    .Uid(f => f.IdCavalete)
                    .Creator(f => new Cavalete(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public Cavalete()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Cavalete(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Cavalete> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Cavalete(Glass.Data.Model.Cavalete dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do cavalete.
        /// </summary>
        public int IdCavalete
        {
            get { return DataModel.IdCavalete; }
            set
            {
                if (DataModel.IdCavalete != value &&
                    RaisePropertyChanging("IdCavalete", value))
                {
                    DataModel.IdCavalete = value;
                    RaisePropertyChanged("IdCavalete");
                }
            }
        }

        /// <summary>
        /// Cód interno.
        /// </summary>
        public string CodInterno
        {
            get { return DataModel.CodInterno; }
            set
            {
                if (DataModel.CodInterno != value &&
                    RaisePropertyChanging("CodInterno", value))
                {
                    DataModel.CodInterno = value;
                    RaisePropertyChanged("CodInterno");
                }
            }
        }

        /// <summary>
        /// Localização.
        /// </summary>
        public string Localizacao
        {
            get { return DataModel.Localizacao; }
            set
            {
                if (DataModel.Localizacao != value &&
                    RaisePropertyChanging("Localizacao", value))
                {
                    DataModel.Localizacao = value;
                    RaisePropertyChanged("Localizacao");
                }
            }
        }

        /// <summary>
        /// Gera o código de barras
        /// Padrão Utilizado: Code128
        /// </summary>
        public byte[] BarCodeImage
        {
            get
            {
                return Glass.Data.Helper.Utils.GetBarCode(CodInterno);
            }
        }

        #endregion
    }
}
