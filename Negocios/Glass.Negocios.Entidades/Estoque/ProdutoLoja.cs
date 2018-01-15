namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do ProdutoLoja.
    /// </summary>
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.ProdutoLoja)]
    [Colosoft.Business.EntityLoader(typeof(ProdutoLojaLoader))]
    public class ProdutoLoja : Colosoft.Business.Entity<Data.Model.ProdutoLoja>
    {
        #region Tipos Aninhados

        class ProdutoLojaLoader : Colosoft.Business.EntityLoader<ProdutoLoja, Data.Model.ProdutoLoja>
        {
            public ProdutoLojaLoader()
            {
                Configure()
                    .Keys(f => f.IdLoja, f => f.IdProd)
                    .Creator(f => new ProdutoLoja(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da loja.
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
        /// Quantidade no estoque.
        /// </summary>
        public double QtdEstoque
        {
            get { return DataModel.QtdEstoque; }
            set
            {
                if (DataModel.QtdEstoque != value &&
                    RaisePropertyChanging("QtdEstoque", value))
                {
                    DataModel.QtdEstoque = value;
                    RaisePropertyChanged("QtdEstoque");
                }
            }
        }

        /// <summary>
        /// Estoque mínimo.
        /// </summary>
        public double EstMinimo
        {
            get { return DataModel.EstMinimo; }
            set
            {
                if (DataModel.EstMinimo != value &&
                    RaisePropertyChanging("EstMinimo", value))
                {
                    DataModel.EstMinimo = value;
                    RaisePropertyChanged("EstMinimo");
                }
            }
        }

        /// <summary>
        /// Reserva.
        /// </summary>
        public double Reserva
        {
            get { return DataModel.Reserva; }
            set
            {
                if (DataModel.Reserva != value &&
                    RaisePropertyChanging("Reserva", value))
                {
                    DataModel.Reserva = value;
                    RaisePropertyChanged("Reserva");
                }
            }
        }

        /// <summary>
        /// Liberação.
        /// </summary>
        public double Liberacao
        {
            get { return DataModel.Liberacao; }
            set
            {
                if (DataModel.Liberacao != value &&
                    RaisePropertyChanging("Liberacao", value))
                {
                    DataModel.Liberacao = value;
                    RaisePropertyChanged("Liberacao");
                }
            }
        }

        /// <summary>
        /// M2.
        /// </summary>
        public double M2
        {
            get { return DataModel.M2; }
            set
            {
                if (DataModel.M2 != value &&
                    RaisePropertyChanging("M2", value))
                {
                    DataModel.M2 = value;
                    RaisePropertyChanged("M2");
                }
            }
        }

        /// <summary>
        /// Estoque fical.
        /// </summary>
        public double EstoqueFiscal
        {
            get { return DataModel.EstoqueFiscal; }
            set
            {
                if (DataModel.EstoqueFiscal != value &&
                    RaisePropertyChanging("EstoqueFiscal", value))
                {
                    DataModel.EstoqueFiscal = value;
                    RaisePropertyChanged("EstoqueFiscal");
                }
            }
        }

        /// <summary>
        /// Defeito.
        /// </summary>
        public double Defeito
        {
            get { return DataModel.Defeito; }
            set
            {
                if (DataModel.Defeito != value &&
                    RaisePropertyChanging("Defeito", value))
                {
                    DataModel.Defeito = value;
                    RaisePropertyChanged("Defeito");
                }
            }
        }

        /// <summary>
        /// Quantidade em posse de terceiros.
        /// </summary>
        public double QtdePosseTerceiros
        {
            get { return DataModel.QtdePosseTerceiros; }
            set
            {
                if (DataModel.QtdePosseTerceiros != value &&
                    RaisePropertyChanging("QtdePosseTerceiros", value))
                {
                    DataModel.QtdePosseTerceiros = value;
                    RaisePropertyChanged("QtdePosseTerceiros");
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
        public int? IdFornec
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
        /// Identificador do transportador associado.
        /// </summary>
        public int? IdTransportador
        {
            get { return DataModel.IdTransportador; }
            set
            {
                if (DataModel.IdTransportador != value &&
                    RaisePropertyChanging("IdTransportador", value))
                {
                    DataModel.IdTransportador = value;
                    RaisePropertyChanged("IdTransportador");
                }
            }
        }

        /// <summary>
        /// Identificador da loja de terceiros.
        /// </summary>
        public int? IdLojaTerc
        {
            get { return DataModel.IdLojaTerc; }
            set
            {
                if (DataModel.IdLojaTerc != value &&
                    RaisePropertyChanging("IdLojaTerc", value))
                {
                    DataModel.IdLojaTerc = value;
                    RaisePropertyChanged("IdLojaTerc");
                }
            }
        }

        /// <summary>
        /// Identificador da administradora de cartão.
        /// </summary>
        public int? IdAdminCartao
        {
            get { return DataModel.IdAdminCartao; }
            set
            {
                if (DataModel.IdAdminCartao != value &&
                    RaisePropertyChanging("IdAdminCartao", value))
                {
                    DataModel.IdAdminCartao = value;
                    RaisePropertyChanged("IdAdminCartao");
                }
            }
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoLoja()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoLoja(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ProdutoLoja> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoLoja(Data.Model.ProdutoLoja dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
