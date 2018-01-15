
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(LayoutPecaOtimizadaLoader))]
    public class LayoutPecaOtimizada : Colosoft.Business.Entity<Glass.Data.Model.LayoutPecaOtimizada>
    {
        #region Tipos Aninhados

        class LayoutPecaOtimizadaLoader : Colosoft.Business.EntityLoader<LayoutPecaOtimizada, Glass.Data.Model.LayoutPecaOtimizada>
        {
            public LayoutPecaOtimizadaLoader()
            {
                Configure()
                    .Uid(f => f.IdLayoutPecaOtimizada)
                    .Child<PecaOtimizada, Glass.Data.Model.PecaOtimizada>("PecasOtimizadas", f => f.PecasOtimizadas, f => f.IdLayoutPecaOtimizada)
                    .Reference<Global.Negocios.Entidades.Produto, Glass.Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProd)
                    .Creator(f => new LayoutPecaOtimizada(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<PecaOtimizada> _pecasOtimizadas;

        #endregion

        #region Propiedades

        /// <summary>
        /// Pecas otimizadas no layout
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<PecaOtimizada> PecasOtimizadas
        {
            get { return _pecasOtimizadas; }
        }

        /// <summary>
        /// Materia-prima do layout
        /// </summary>
        public Global.Negocios.Entidades.Produto Produto
        {
            get { return GetReference<Global.Negocios.Entidades.Produto>("Produto", true); }
        }

        /// <summary>
        /// Identificador do layout.
        /// </summary>
        public int IdLayoutPecaOtmizada
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
        /// Identificador da otimização.
        /// </summary>
        public int IdOtimizacao
        {
            get { return DataModel.IdOtimizacao; }
            set
            {
                if (DataModel.IdOtimizacao != value &&
                    RaisePropertyChanging("IdOtimizacao", value))
                {
                    DataModel.IdOtimizacao = value;
                    RaisePropertyChanged("IdOtimizacao");
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
        /// Quantidade de materia-prima utilizada nesse layout.
        /// </summary>
        public int Qtde
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
        /// Peso de uma materia-prima do layout
        /// </summary>
        public decimal PesoUn
        {
            get { return (decimal)Produto.Peso * 6; }
        }

        /// <summary>
        /// Peso total de materia-prima do layout
        /// </summary>
        public decimal PesoTotal
        {
            get { return PesoUn * Qtde; }
        }

        /// <summary>
        /// Descrição do produto
        /// </summary>
        public string DescrProduto
        {
            get { return Produto.CodInterno + " - " + Produto.Descricao; }
        }

        /// <summary>
        /// Imagem do layout
        /// </summary>
        public byte[] LayoutImage
        {
            get
            {
                return CriaImagemLayout();
            }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public LayoutPecaOtimizada()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected LayoutPecaOtimizada(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.LayoutPecaOtimizada> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _pecasOtimizadas = GetChild<PecaOtimizada>(args.Children, "PecasOtimizadas");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public LayoutPecaOtimizada(Glass.Data.Model.LayoutPecaOtimizada dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _pecasOtimizadas = CreateChild<Colosoft.Business.IEntityChildrenList<PecaOtimizada>>("PecasOtimizadas");
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria a imagem do layout
        /// </summary>
        /// <returns></returns>
        private byte[] CriaImagemLayout()
        {
            var lstPecas = new List<string>();

            var svg = @"<svg xmlns='http://www.w3.org/2000/svg' width='683' height='38' viewBox='-1 -1 683 38'>
                            {0}
                        </svg>";

            var strPeca = @"
                <rect width='{0}' height='35' x ='{1}' y ='1' fill='#{2}' stroke ='#000080' stroke-width='0.5'></rect>
                <text x='{3}' y='20' fill='#000080' font-family='Helvetica' font-size= '8px' font-weight='bold'>{4}</text>
                ";

            foreach (var p in PecasOtimizadas.OrderBy(f => f.PosicaoX))
            {
                decimal comp, x, posTexto;
                string cor;

                comp = AplicaEscala(p.Comprimento);
                x = AplicaEscala(p.PosicaoX);
                posTexto = x + (comp / 2) - 8;
                cor = p.Sobra ? "E0FFFF" : "87CEEB";

                lstPecas.Add(string.Format(strPeca, comp.ToString().Replace(",", "."), x.ToString().Replace(",", "."), cor, posTexto.ToString().Replace(",", "."), Math.Round(p.Comprimento, 0)));
            }

            svg = string.Format(svg, string.Join(Environment.NewLine, lstPecas));

            return Conversoes.ConverteSvgParaPng(svg);
        }

        /// <summary>
        /// Aplica a escala para gerar a imagem com a barra tendo 18cm
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private decimal AplicaEscala(decimal x)
        {
            if (x == 0)
                return 0;

            return ((x * 18) / 6000) * 37.795276m;
        }

        #endregion
    }
}
