﻿namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do baixa de estoque do produto.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ProdutoBaixaEstoqueLoader))]
    public class ProdutoBaixaEstoque : Colosoft.Business.Entity<Glass.Data.Model.ProdutoBaixaEstoque>
    {
        #region Tipos Aninhados

        class ProdutoBaixaEstoqueLoader : Colosoft.Business.EntityLoader<ProdutoBaixaEstoque, Glass.Data.Model.ProdutoBaixaEstoque>
        {
            public ProdutoBaixaEstoqueLoader()
            {
                Configure()
                    .Uid(f => f.IdProdBaixaEst)
                    .FindName(new ProdutoBaixaEstoqueFindNameConverter(), f => f.IdProdBaixa, f => f.Qtde)
                    .Child<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef, Data.Model.ProdutoBaixaEstoqueBenef>("ProdutoBaixaEstBeneficiamentos", f => f.ProdutoBaixaEstBeneficiamentos, f => f.IdProdBaixaEst)
                    .Creator(f => new ProdutoBaixaEstoque(f));
            }
        }

        /// <summary>
        /// Implementação do conversor do nome.
        /// </summary>
        class ProdutoBaixaEstoqueFindNameConverter : Colosoft.IFindNameConverter
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

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef> _produtoBaixaEstBeneficiamentos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Relação dos beneficiamentos do produto.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef> ProdutoBaixaEstBeneficiamentos
        {
            get { return _produtoBaixaEstBeneficiamentos; }
        }

        /// <summary>
        /// Identificador da baixa.
        /// </summary>
        public int IdProdBaixaEst
        {
            get { return DataModel.IdProdBaixaEst; }
            set
            {
                if (DataModel.IdProdBaixaEst != value &&
                    RaisePropertyChanging("IdProdBaixaEst", value))
                {
                    DataModel.IdProdBaixaEst = value;
                    RaisePropertyChanged("IdProdBaixaEst");
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
        /// Identificador do produto associado a baixa.
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

        /// <summary>
        /// Processo.
        /// </summary>
        public int IdProcesso
        {
            get { return DataModel.IdProcesso; }
            set
            {
                if (DataModel.IdProcesso != value &&
                    RaisePropertyChanging("IdProcesso", value))
                {
                    DataModel.IdProcesso = value;
                    RaisePropertyChanged("IdProcesso");
                }
            }
        }

        /// <summary>
        /// Aplicação.
        /// </summary>
        public int IdAplicacao
        {
            get { return DataModel.IdAplicacao; }
            set
            {
                if (DataModel.IdAplicacao != value &&
                    RaisePropertyChanging("IdAplicacao", value))
                {
                    DataModel.IdAplicacao = value;
                    RaisePropertyChanged("IdAplicacao");
                }
            }
        }

        /// <summary>
            /// Altura.
            /// </summary>
        public int Altura
        {
            get { return DataModel.Altura; }
            set
            {
                if (DataModel.Altura != value &&
                    RaisePropertyChanging("Altura", value))
                {
                    DataModel.Altura = value;
                    RaisePropertyChanged("Altura");
                }
            }
        }

        /// <summary>
            /// Largura.
            /// </summary>
        public int Largura
        {
            get { return DataModel.Largura; }
            set
            {
                if (DataModel.Largura != value &&
                    RaisePropertyChanging("Largura", value))
                {
                    DataModel.Largura = value;
                    RaisePropertyChanged("Largura");
                }
            }
        }

        /// <summary>
            /// Forma.
            /// </summary>
        public string Forma
        {
            get { return DataModel.Forma; }
            set
            {
                if (DataModel.Forma != value &&
                    RaisePropertyChanging("Forma", value))
                {
                    DataModel.Forma = value;
                    RaisePropertyChanged("Forma");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoBaixaEstoque()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoBaixaEstoque(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ProdutoBaixaEstoque> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _produtoBaixaEstBeneficiamentos = GetChild<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef>(args.Children, "ProdutoBaixaEstBeneficiamentos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoBaixaEstoque(Data.Model.ProdutoBaixaEstoque dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _produtoBaixaEstBeneficiamentos = CreateChild<Colosoft.Business.IEntityChildrenList<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef>>("ProdutoBaixaEstBeneficiamentos");
        }

        #endregion
    }
}
