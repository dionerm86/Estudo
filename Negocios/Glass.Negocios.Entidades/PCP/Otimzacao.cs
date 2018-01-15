using Colosoft.Business;
using System.Collections.Generic;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(OtimizacaoLoader))]
    public class Otimizacao : Glass.Negocios.Entidades.EntidadeBaseCadastro<Glass.Data.Model.Otimizacao>
    {
        #region Tipos Aninhados

        class OtimizacaoLoader : Colosoft.Business.EntityLoader<Otimizacao, Glass.Data.Model.Otimizacao>
        {
            public OtimizacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdOtimizacao)
                    .Child<LayoutPecaOtimizada, Glass.Data.Model.LayoutPecaOtimizada>("LayoutsOtimizacao", f => f.LayoutsOtimizacao, f => f.IdOtimizacao)
                    .Reference<Global.Negocios.Entidades.Funcionario, Glass.Data.Model.Funcionario>("Funcionario", f => f.Funcionario, f => f.Usucad)
                    .Creator(f => new Otimizacao(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<LayoutPecaOtimizada> _layoutsOtimizacao;

        #endregion

        #region Propriedades

        /// <summary>
        /// Layouts da otimização
        /// </summary>
        public IEntityChildrenList<LayoutPecaOtimizada> LayoutsOtimizacao
        {
            get { return _layoutsOtimizacao; }
        }

        /// <summary>
        /// Funcionário que realizou a otimização
        /// </summary>
        public Global.Negocios.Entidades.Funcionario Funcionario
        {
            get { return GetReference<Global.Negocios.Entidades.Funcionario>("Funcionario", true); }
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
        /// Tipo.
        /// </summary>
        public Glass.Data.Model.TipoOtimizacao Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Peso bruto da otimização (Peso total das materias-primas utilizadas)
        /// </summary>
        public decimal PesoBruto
        {
            get { return LayoutsOtimizacao.Sum(f => f.PesoTotal); }
        }

        /// <summary>
        /// Peso Liquido da otimização
        /// </summary>
        public decimal PesoLiquido
        {
            get { return LayoutsOtimizacao.Sum(f => f.PecasOtimizadas.Where(x => !x.Sobra).Sum(x => x.Peso) * f.Qtde); }
        }

        /// <summary>
        /// Peso dos retalhos da otimização
        /// </summary>
        public decimal PesoRetalho
        {
            get { return LayoutsOtimizacao.Sum(f => f.PecasOtimizadas.Where(x => x.Sobra).Sum(x => x.Peso) * f.Qtde); }
        }

        /// <summary>
        /// Peso do material que foi perdido com arestas
        /// </summary>
        public decimal PesoPerda
        {
            get { return PesoBruto - (PesoLiquido + PesoRetalho); }
        }

        /// <summary>
        /// Nome do funcionário que realizou a otimização
        /// </summary>
        public string NomeFuncionario
        {
            get { return Funcionario.Nome; }
        }

        /// <summary>
        /// Pedidos da otimização
        /// </summary>
        public string Pedidos
        {
            get
            {
                var lstPedidos = new List<uint>();

                foreach (var l in LayoutsOtimizacao)
                {
                    if (l.PecasOtimizadas == null || l.PecasOtimizadas.Count == 0)
                        continue;

                    lstPedidos.AddRange(l.PecasOtimizadas.Where(f => !f.Sobra && f.ProdutosPedido != null).Select(f => f.ProdutosPedido.IdPedido).ToList());
                }

                return string.Join(", ", lstPedidos.Distinct().Select(f => f.ToString()));
            }
        }

        /// <summary>
        /// Pedidos da otimização
        /// </summary>
        public string Orcamentos
        {
            get
            {
                var lstOrcamentos = new List<uint>();

                foreach (var l in LayoutsOtimizacao)
                {
                    if (l.PecasOtimizadas == null || l.PecasOtimizadas.Count == 0)
                        continue;

                    lstOrcamentos.AddRange(l.PecasOtimizadas.Where(f => !f.Sobra && f.ProdutosOrcamento != null).Select(f => f.ProdutosOrcamento.IdOrcamento).ToList());
                }

                return string.Join(", ", lstOrcamentos.Distinct().Select(f => f.ToString()));
            }
        }


        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Otimizacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Otimizacao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Otimizacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _layoutsOtimizacao = GetChild<LayoutPecaOtimizada>(args.Children, "LayoutsOtimizacao");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Otimizacao(Glass.Data.Model.Otimizacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _layoutsOtimizacao = CreateChild<Colosoft.Business.IEntityChildrenList<LayoutPecaOtimizada>>("LayoutsOtimizacao");
        }

        #endregion
    }
}
