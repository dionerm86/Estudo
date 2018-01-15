namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da baixa do estoque fiscal do produto.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutoBaixaEstoqueFiscalLoader))]
    public class ProdutoBaixaEstoqueFiscal : Colosoft.Business.Entity<Glass.Data.Model.ProdutoBaixaEstoqueFiscal>
    {
        #region Tipos Aninhados

        class ProdutoBaixaEstoqueFiscalLoader : Colosoft.Business.EntityLoader<ProdutoBaixaEstoqueFiscal, Glass.Data.Model.ProdutoBaixaEstoqueFiscal>
        {
            public ProdutoBaixaEstoqueFiscalLoader()
            {
                Configure()
                    .Uid(f => f.IdProdBaixaEstFisc)
                    .FindName(new ProdutoBaixaEstoqueFiscalFindNameConverter(), f => f.IdProdBaixa, f => f.Qtde)
                    .Creator(f => new ProdutoBaixaEstoqueFiscal(f));
            }
        }

        /// <summary>
        /// Implementação do conversor do nome.
        /// </summary>
        class ProdutoBaixaEstoqueFiscalFindNameConverter : Colosoft.IFindNameConverter
        {
            /// <summary>
            /// Converte os valores informados para o nome de pesquisa.
            /// </summary>
            /// <param name="baseInfo"></param>
            /// <returns></returns>
            public string Convert(object[] baseInfo)
            {
                var idProdBaixa = (int)baseInfo[0];
                var qtde = (float)baseInfo[1];

                var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorProdutoBaixaEstoque>();

                return provedor.ObtemIdentificacao(idProdBaixa, qtde);
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da baixa do estoque fiscal.
        /// </summary>
        public int IdProdBaixaEstFisc
        {
            get { return DataModel.IdProdBaixaEstFisc; }
            set
            {
                if (DataModel.IdProdBaixaEstFisc != value &&
                    RaisePropertyChanging("IdProdBaixaEstFisc", value))
                {
                    DataModel.IdProdBaixaEstFisc = value;
                    RaisePropertyChanged("IdProdBaixaEstFisc");
                }
            }
        }

        /// <summary>
        /// Identificador do produto associado.
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
        /// Identificador da baixaa associada.
        /// </summary>
        public int IdProdBaixa
        {
            get { return DataModel.IdProdBaixa; }
            set
            {
                if (DataModel.IdProdBaixa != value &&
                    RaisePropertyChanging("IdProdBaixa", value))
                {
                    DataModel.IdProdBaixa = value;
                    RaisePropertyChanged("IdProdBaixa");
                }
            }
        }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public float Qtde
        {
            get { return DataModel.Qtde; }
            set
            {
                if (DataModel.Qtde != value &&
                    RaisePropertyChanging("Qtde", value))
                {
                    DataModel.Qtde = value;
                    RaisePropertyChanged("Qtde");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoBaixaEstoqueFiscal()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoBaixaEstoqueFiscal(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ProdutoBaixaEstoqueFiscal> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoBaixaEstoqueFiscal(Glass.Data.Model.ProdutoBaixaEstoqueFiscal dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
