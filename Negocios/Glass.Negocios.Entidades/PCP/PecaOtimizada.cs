using System;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de uma peça otimizada no sistema
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PecaOtimizadaLoader))]
    public class PecaOtimizada : Colosoft.Business.Entity<Glass.Data.Model.PecaOtimizada>
    {
        #region Tipos Aninhados

        class PecaOtimizadaLoader : Colosoft.Business.EntityLoader<PecaOtimizada, Glass.Data.Model.PecaOtimizada>
        {
            public PecaOtimizadaLoader()
            {
                Configure()
                    .Uid(f => f.IdPecaOtimizada)
                     .Reference<Pedido.Negocios.Entidades.ProdutosPedido, Glass.Data.Model.ProdutosPedido>("ProdutosPedido", f => f.ProdutosPedido, f => f.IdProdPed)
                     .Reference<Orcamento.Negocios.Entidades.ProdutosOrcamento, Glass.Data.Model.ProdutosOrcamento>("ProdutosOrcamento", f => f.ProdutosOrcamento, f => f.IdProdOrcamento)
                    .Creator(f => new PecaOtimizada(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do layout.
        /// </summary>
        public LayoutPecaOtimizada LayoutPecaOtimizada
        {
            get
            {
                // Recupera o pedido pai
                var LayoutPecaOtimizada = Owner as LayoutPecaOtimizada;

                if (LayoutPecaOtimizada == null)
                    throw new InvalidOperationException("Só é possível salvar a peça otimizada do layout com a instancia associada ao layout");

                return LayoutPecaOtimizada;
            }
        }

        /// <summary>
        /// Produto pedido da peça
        /// </summary>
        public Pedido.Negocios.Entidades.ProdutosPedido ProdutosPedido
        {
            get { return GetReference<Pedido.Negocios.Entidades.ProdutosPedido>("ProdutosPedido", true); }
        }

        /// <summary>
        /// Produto pedido da peça
        /// </summary>
        public Orcamento.Negocios.Entidades.ProdutosOrcamento ProdutosOrcamento
        {
            get { return GetReference<Orcamento.Negocios.Entidades.ProdutosOrcamento>("ProdutosOrcamento", true); }
        }

        /// <summary>
        /// Identificador da peça.
        /// </summary>
        public int IdPecaOtimizada
        {
            get { return DataModel.IdPecaOtimizada; }
            set
            {
                if (DataModel.IdPecaOtimizada != value &&
                    RaisePropertyChanging("IdPecaOtimizada", value))
                {
                    DataModel.IdPecaOtimizada = value;
                    RaisePropertyChanged("IdPecaOtimizada");
                }
            }
        }

        /// <summary>
        /// Identificador do layout da otimização.
        /// </summary>
        public int IdLayoutPecaOtimizada
        {
            get { return DataModel.IdLayoutPecaOtimizada; }
            set
            {
                if (DataModel.IdLayoutPecaOtimizada != value &&
                    RaisePropertyChanging("IdLayoutPecaOtimizada", value))
                {
                    DataModel.IdLayoutPecaOtimizada = value;
                    RaisePropertyChanged("IdLayoutPecaOtimizada");
                }
            }
        }

        /// <summary>
        /// Identificador do produto pedido.
        /// </summary>
        public int? IdProdPed
        {
            get { return DataModel.IdProdPed; }
            set
            {
                if (DataModel.IdProdPed != value &&
                    RaisePropertyChanging("IdProdPed", value))
                {
                    DataModel.IdProdPed = value;
                    RaisePropertyChanged("IdProdPed");
                }
            }
        }

        /// <summary>
        /// Identificador do produto orcamento.
        /// </summary>
        public int? IdProdOrcamento
        {
            get { return DataModel.IdProdOrcamento; }
            set
            {
                if (DataModel.IdProdOrcamento != value &&
                    RaisePropertyChanging("IdProdOrcamento", value))
                {
                    DataModel.IdProdOrcamento = value;
                    RaisePropertyChanged("IdProdOrcamento");
                }
            }
        }

        /// <summary>
        /// Grau.
        /// </summary>
        public Glass.Data.Model.GrauCorteEnum GrauCorte
        {
            get { return DataModel.GrauCorte; }
            set
            {
                if (DataModel.GrauCorte != value &&
                    RaisePropertyChanging("GrauCorte", value))
                {
                    DataModel.GrauCorte = value;
                    RaisePropertyChanged("GrauCorte");
                }
            }
        }

        /// <summary>
        /// Indica que essa peça e uma sobra.
        /// </summary>
        public bool Sobra
        {
            get { return DataModel.Sobra; }
            set
            {
                if (DataModel.Sobra != value &&
                    RaisePropertyChanging("Sobra", value))
                {
                    DataModel.Sobra = value;
                    RaisePropertyChanged("Sobra");
                }
            }
        }

        /// <summary>
        /// Posição da peça.
        /// </summary>
        public decimal PosicaoX
        {
            get { return DataModel.PosicaoX; }
            set
            {
                if (DataModel.PosicaoX != value &&
                    RaisePropertyChanging("PosicaoX", value))
                {
                    DataModel.PosicaoX = value;
                    RaisePropertyChanged("PosicaoX");
                }
            }
        }

        /// <summary>
        /// Comprimento.
        /// </summary>
        public decimal Comprimento
        {
            get { return DataModel.Comprimento; }
            set
            {
                if (DataModel.Comprimento != value &&
                    RaisePropertyChanging("Comprimento", value))
                {
                    DataModel.Comprimento = value;
                    RaisePropertyChanged("Comprimento");
                }
            }
        }

        /// <summary>
        /// Identificador do produto
        /// </summary>
        public int IdProd
        {
            get
            {
                if (Sobra || (ProdutosPedido == null && ProdutosOrcamento == null))
                    return LayoutPecaOtimizada.IdProd;

                return ProdutosPedido != null ? (int)ProdutosPedido.IdProd : ProdutosOrcamento != null ? (int)ProdutosOrcamento.IdProd : 0;
            }
        }

        /// <summary>
        /// Peso da peça
        /// </summary>
        public decimal Peso
        {
            get
            {
                if (Sobra || ProdutosPedido == null)
                    return (decimal)LayoutPecaOtimizada.Produto.Peso * (Comprimento / 1000);

                return (decimal)ProdutosPedido.Produto.Peso * (Comprimento / 1000);
            }
        }

        /// <summary>
        /// Descrição do grau do corte
        /// </summary>
        public string GrauCorteDescr
        {
            get { return Colosoft.Translator.Translate(GrauCorte).Format(); }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public PecaOtimizada()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PecaOtimizada(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.PecaOtimizada> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PecaOtimizada(Glass.Data.Model.PecaOtimizada dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
