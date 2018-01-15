
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(FlagArqMesaProdutoLoader))]
    public class FlagArqMesaProduto : Colosoft.Business.Entity<Data.Model.FlagArqMesaProduto>
    {
        #region Tipos Aninhados

        class FlagArqMesaProdutoLoader : Colosoft.Business.EntityLoader<FlagArqMesaProduto, Data.Model.FlagArqMesaProduto>
        {
            public FlagArqMesaProdutoLoader()
            {
                Configure()
                    .Keys(f => f.IdProduto, f=> f.IdFlagArqMesa)
                    .Creator(f => new FlagArqMesaProduto(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do tipo de cliente.
        /// </summary>
        public int IdProduto
        {
            get { return DataModel.IdProduto; }
            set
            {
                if (DataModel.IdProduto != value &&
                    RaisePropertyChanging("IdProduto", value))
                {
                    DataModel.IdProduto = value;
                    RaisePropertyChanged("IdProduto");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public int IdFlagArqMesa
        {
            get { return DataModel.IdFlagArqMesa; }
            set
            {
                if (DataModel.IdFlagArqMesa != value &&
                    RaisePropertyChanging("IdFlagArqMesa", value))
                {
                    DataModel.IdFlagArqMesa = value;
                    RaisePropertyChanged("IdFlagArqMesa");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FlagArqMesaProduto()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FlagArqMesaProduto(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FlagArqMesaProduto> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FlagArqMesaProduto(Data.Model.FlagArqMesaProduto dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
