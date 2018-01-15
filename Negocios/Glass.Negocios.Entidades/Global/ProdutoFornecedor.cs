using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do produto do fornecedor.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutoFornecedorLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.ProdutoFornecedor)]
    public class ProdutoFornecedor : Colosoft.Business.Entity<Data.Model.ProdutoFornecedor>
    {
        #region Tipos Aninhados

        class ProdutoFornecedorLoader : Colosoft.Business.EntityLoader<ProdutoFornecedor, Data.Model.ProdutoFornecedor>
        {
            public ProdutoFornecedorLoader()
            {
                Configure()
                    .Uid(f => f.IdProdFornec)
                    .FindName(f => f.CodFornec)
                    .Creator(f => new ProdutoFornecedor(f));

            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto do fornecedor.
        /// </summary>
        public int IdProdFornec
        {
            get { return DataModel.IdProdFornec; }
            set
            {
                if (DataModel.IdProdFornec != value &&
                    RaisePropertyChanging("IdProdFornec", value))
                {
                    DataModel.IdProdFornec = value;
                    RaisePropertyChanged("IdProdFornec");
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
        /// Identificador do fornecedor associado.
        /// </summary>
        public int IdFornec
        {
            get { return DataModel.IdFornec; }
            set
            {
                if (DataModel.IdFornec != value &&
                    RaisePropertyChanging("IdFornec", value))
                {
                    DataModel.IdFornec = value;
                    RaisePropertyChanged("IdFornec");
                }
            }
        }

        /// <summary>
        /// Data de vigência.
        /// </summary>
        public DateTime? DataVigencia
        {
            get { return DataModel.DataVigencia; }
            set
            {
                if (DataModel.DataVigencia != value &&
                    RaisePropertyChanging("DataVigencia", value))
                {
                    DataModel.DataVigencia = value;
                    RaisePropertyChanged("DataVigencia");
                }
            }
        }

        /// <summary>
        /// Código do produto no fornecedor.
        /// </summary>
        public string CodFornec
        {
            get { return DataModel.CodFornec; }
            set
            {
                if (DataModel.CodFornec != value &&
                    RaisePropertyChanging("CodFornec", value))
                {
                    DataModel.CodFornec = value;
                    RaisePropertyChanged("CodFornec");
                }
            }
        }

        /// <summary>
        /// Custom de comra.
        /// </summary>
        public decimal CustoCompra
        {
            get { return DataModel.CustoCompra; }
            set
            {
                if (DataModel.CustoCompra != value &&
                    RaisePropertyChanging("CustoCompra", value))
                {
                    DataModel.CustoCompra = value;
                    RaisePropertyChanged("CustoCompra");
                }
            }
        }

        /// <summary>
        /// Prazo de entrega em dias.
        /// </summary>
        public int PrazoEntregaDias
        {
            get { return DataModel.PrazoEntregaDias; }
            set
            {
                if (DataModel.PrazoEntregaDias != value &&
                    RaisePropertyChanging("PrazoEntregaDias", value))
                {
                    DataModel.PrazoEntregaDias = value;
                    RaisePropertyChanged("PrazoEntregaDias");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoFornecedor()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoFornecedor(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ProdutoFornecedor> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoFornecedor(Data.Model.ProdutoFornecedor dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
