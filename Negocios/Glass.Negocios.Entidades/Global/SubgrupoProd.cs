using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do subgrupo de produtos.
    /// </summary>
    public interface IValidadorSubgrupoProd
    {
        /// <summary>
        /// Valida a existecia do subgrupo de produtos.
        /// </summary>
        /// <param name="subgrupoProduto"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(SubgrupoProd subgrupoProduto);
        
        /// <summary>
        /// Valida a atualização do cliente.
        /// </summary>
        IMessageFormattable[] ValidaAtualizacao(SubgrupoProd subgrupoProduto);
    }

    /// <summary>
    /// Representa a entidade de negócio
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SubgrupoProdLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.SubgrupoProduto)]
    public class SubgrupoProd : Colosoft.Business.Entity<Data.Model.SubgrupoProd>
    {
        #region Tipos Aninhados

        class SubgrupoProdLoader : Colosoft.Business.EntityLoader<SubgrupoProd, Data.Model.SubgrupoProd>
        {
            public SubgrupoProdLoader()
            {
                Configure()
                    .Uid(f => f.IdSubgrupoProd)
                    .FindName(f => f.Descricao)
                    .Child<Glass.Global.Negocios.Entidades.SubgrupoProdLoja, Data.Model.SubgrupoProdLoja>("SubgruposProdLoja", f => f.SubgruposProdLoja, f => f.IdSubgrupoProd)
                    .Creator(f => new SubgrupoProd(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<SubgrupoProdLoja> _subgruposLoja;

        #endregion

        #region Propriedades

        public Colosoft.Business.IEntityChildrenList<Glass.Global.Negocios.Entidades.SubgrupoProdLoja> SubgruposProdLoja
        {
            get
            {
                return _subgruposLoja;
            }
        }

        /// <summary>
        /// Identificador do subgrupo.
        /// </summary>
        public int IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd", "SubgrupoSistema");
                }
            }
        }

        /// <summary>
        /// Identificador do grupo associado.
        /// </summary>
        public int IdGrupoProd
        {
            get { return DataModel.IdGrupoProd; }
            set
            {
                if (DataModel.IdGrupoProd != value &&
                    RaisePropertyChanging("IdGrupoProd", value))
                {
                    DataModel.IdGrupoProd = value;
                    RaisePropertyChanged("IdGrupoProd");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Tipo do calculo.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculo
        {
            get { return DataModel.TipoCalculo; }
            set
            {
                if (DataModel.TipoCalculo != value &&
                    RaisePropertyChanging("TipoCalculo", value))
                {
                    DataModel.TipoCalculo = value;

                    if (value.HasValue && value.Value == Data.Model.TipoCalculoGrupoProd.Qtd)
                        this.ProdutosEstoque = false;

                    RaisePropertyChanged("TipoCalculo");
                }
            }
        }

        /// <summary>
        /// Tipo do cálculo de nota fiscal.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculoNf
        {
            get { return DataModel.TipoCalculoNf; }
            set
            {
                if (DataModel.TipoCalculoNf != value &&
                    RaisePropertyChanging("TipoCalculoNf", value))
                {
                    DataModel.TipoCalculoNf = value;
                    RaisePropertyChanged("TipoCalculoNf");
                }
            }
        }

        /// <summary>
        /// Identifica se é bloquear o estoque.
        /// </summary>
        public bool BloquearEstoque
        {
            get { return DataModel.BloquearEstoque; }
            set
            {
                if (DataModel.BloquearEstoque != value &&
                    RaisePropertyChanging("BloquearEstoque", value))
                {
                    DataModel.BloquearEstoque = value;
                    RaisePropertyChanged("BloquearEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica que não é para alterar o estoque.
        /// </summary>
        public bool NaoAlterarEstoque
        {
            get { return DataModel.NaoAlterarEstoque; }
            set
            {
                if (DataModel.NaoAlterarEstoque != value &&
                    RaisePropertyChanging("NaoAlterarEstoque", value))
                {
                    DataModel.NaoAlterarEstoque = value;
                    RaisePropertyChanged("NaoAlterarEstoque", "AlterarEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica que é para alterar o estoque fiscal.
        /// </summary>
        public bool AlterarEstoque
        {
            get { return !NaoAlterarEstoque; }
            set { NaoAlterarEstoque = !value; }
        }

        /// <summary>
        /// Identifica que não é para alterar o estoque fiscal.
        /// </summary>
        public bool NaoAlterarEstoqueFiscal
        {
            get { return DataModel.NaoAlterarEstoqueFiscal; }
            set
            {
                if (DataModel.NaoAlterarEstoqueFiscal != value &&
                    RaisePropertyChanging("NaoAlterarEstoqueFiscal", value))
                {
                    DataModel.NaoAlterarEstoqueFiscal = value;
                    RaisePropertyChanged("NaoAlterarEstoqueFiscal", "AlterarEstoqueFiscal");
                }
            }
        }

        /// <summary>
        /// Identifica que é para alterar o estoque fiscal.
        /// </summary>
        public bool AlterarEstoqueFiscal
        {
            get { return !NaoAlterarEstoqueFiscal; }
            set { NaoAlterarEstoqueFiscal = !value; }
        }

        /// <summary>
        /// Produtos para estoque.
        /// </summary>
        public bool ProdutosEstoque
        {
            get { return DataModel.ProdutosEstoque; }
            set
            {
                if (DataModel.ProdutosEstoque != value &&
                    RaisePropertyChanging("ProdutosEstoque", value))
                {
                    DataModel.ProdutosEstoque = value;
                    RaisePropertyChanged("ProdutosEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica se é vidro temperado.
        /// </summary>
        public bool IsVidroTemperado
        {
            get { return DataModel.IsVidroTemperado; }
            set
            {
                if (DataModel.IsVidroTemperado != value &&
                    RaisePropertyChanging("IsVidroTemperado", value))
                {
                    DataModel.IsVidroTemperado = value;
                    RaisePropertyChanged("IsVidroTemperado");
                }
            }
        }

        /// <summary>
        /// Exibir mensagem estoque.
        /// </summary>
        public bool ExibirMensagemEstoque
        {
            get { return DataModel.ExibirMensagemEstoque; }
            set
            {
                if (DataModel.ExibirMensagemEstoque != value &&
                    RaisePropertyChanging("ExibirMensagemEstoque", value))
                {
                    DataModel.ExibirMensagemEstoque = value;
                    RaisePropertyChanged("ExibirMensagemEstoque");
                }
            }
        }

        /// <summary>
        /// Número mínimo de dias para entrega.
        /// </summary>
        public int? NumeroDiasMinimoEntrega
        {
            get { return DataModel.NumeroDiasMinimoEntrega; }
            set
            {
                if (DataModel.NumeroDiasMinimoEntrega != value &&
                    RaisePropertyChanging("NumeroDiasMinimoEntrega", value))
                {
                    DataModel.NumeroDiasMinimoEntrega = value;
                    RaisePropertyChanged("NumeroDiasMinimoEntrega");
                }
            }
        }

        /// <summary>
        /// Dia semana entrega.
        /// </summary>
        public int? DiaSemanaEntrega
        {
            get { return DataModel.DiaSemanaEntrega; }
            set
            {
                if (DataModel.DiaSemanaEntrega != value &&
                    RaisePropertyChanging("DiaSemanaEntrega", value))
                {
                    DataModel.DiaSemanaEntrega = value;
                    RaisePropertyChanged("DiaSemanaEntrega");
                }
            }
        }

        /// <summary>
        /// Identifica se gera volume.
        /// </summary>
        public bool GeraVolume
        {
            get { return DataModel.GeraVolume; }
            set
            {
                if (DataModel.GeraVolume != value &&
                    RaisePropertyChanging("GeraVolume", value))
                {
                    DataModel.GeraVolume = value;
                    RaisePropertyChanged("GeraVolume");
                }
            }
        }

        /// <summary>
        /// Tipo do subgrupo.
        /// </summary>
        public Data.Model.TipoSubgrupoProd TipoSubgrupo
        {
            get { return DataModel.TipoSubgrupo; }
            set
            {
                if (DataModel.TipoSubgrupo != value &&
                    RaisePropertyChanging("TipoSubgrupo", value))
                {
                    DataModel.TipoSubgrupo = value;
                    RaisePropertyChanged("TipoSubgrupo");
                }
            }
        }

        /// <summary>
        /// Identifica o id do cliente.
        /// </summary>
        public int? IdCli
        {
            get { return DataModel.IdCli; }
            set
            {
                if (DataModel.IdCli != value &&
                    RaisePropertyChanging("IdCli", value))
                {
                    DataModel.IdCli = value;
                    RaisePropertyChanged("IdCli");
                }
            }
        }

        /// <summary>
        /// Indica se os produtos desse subgrupo podem ser liberados com a produção pendente.
        /// </summary>
        public bool LiberarPendenteProducao
        {
            get { return DataModel.LiberarPendenteProducao; }
            set
            {
                if (DataModel.LiberarPendenteProducao != value &&
                    RaisePropertyChanging("LiberarPendenteProducao", value))
                {
                    DataModel.LiberarPendenteProducao = value;
                    RaisePropertyChanged("LiberarPendenteProducao");
                }
            }
        }

        /// <summary>
        /// Indica se é permitida a revenda de produtos do tipo venda (solução para inclusão de embalagem no pedido de venda)
        /// </summary>
        public bool PermitirItemRevendaNaVenda
        {
            get { return DataModel.PermitirItemRevendaNaVenda; }
            set
            {
                if (DataModel.PermitirItemRevendaNaVenda != value &&
                    RaisePropertyChanging("PermitirItemRevendaNaVenda", value))
                {
                    DataModel.PermitirItemRevendaNaVenda = value;
                    RaisePropertyChanged("PermitirItemRevendaNaVenda");
                }
            }
        }

        /// <summary>
        /// Bloquear Ecommerce.
        /// </summary>
        public bool BloquearEcommerce
        {
            get { return DataModel.BloquearEcommerce; }
            set
            {
                if (DataModel.BloquearEcommerce != value &&
                    RaisePropertyChanging("BloquearEcommerce", value))
                {
                    DataModel.BloquearEcommerce = value;
                    RaisePropertyChanged("BloquearEcommerce");
                }
            }
        }

        /// <summary>
        /// Identifica se é um subgrupo do sistema.
        /// </summary>
        public bool SubgrupoSistema
        {
            get { return IdSubgrupoProd > 0 && IdSubgrupoProd <= 8; }
        }

        public int[] IdsLojaAssociacao
        {
            get
            {
#pragma warning disable S2365 // Properties should not make collection or array copies
                return SubgruposProdLoja.Select(f => f.IdLoja).ToArray();
#pragma warning restore S2365 // Properties should not make collection or array copies
            }
            set
            {
                SubgruposProdLoja.Clear();

                foreach (var loja in value)
                {
                    if (SubgruposProdLoja.Any(f => f.IdLoja == loja && f.IdSubgrupoProd == IdSubgrupoProd))
                        continue;

                    var novo = new SubgrupoProdLoja();
                    novo.IdSubgrupoProd = IdSubgrupoProd;
                    novo.IdLoja = loja;

                    SubgruposProdLoja.Add(novo);
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public SubgrupoProd()
			: this(null, null, null)
		{

		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="args"></param>
		protected SubgrupoProd(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.SubgrupoProd> args)
			: base(args.DataModel, args.UIContext, args.TypeManager)
		{
            _subgruposLoja = GetChild<Global.Negocios.Entidades.SubgrupoProdLoja>(args.Children, "SubgruposProdLoja");
		}

		/// <summary>
		/// Cria a instancia com os dados do modelo de dados.
		/// </summary>
		/// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
		public SubgrupoProd(Data.Model.SubgrupoProd dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
			: base(dataModel, uiContext, entityTypeManager)
		{
            _subgruposLoja = CreateChild<Colosoft.Business.IEntityChildrenList<Glass.Global.Negocios.Entidades.SubgrupoProdLoja>>("SubgruposProdLoja");
		}

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado para apagar a entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            if (SubgrupoSistema)
                return new Colosoft.Business.DeleteResult(false, "Não é possível apagar um subgrupo do sistema".GetFormatter());

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorSubgrupoProd>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método de salvamento da entidade de SubgrupoProd.
        /// </summary>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorSubgrupoProd>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaAtualizacao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            // Salva o subgrupo.
            var resultado = base.Save(session);

            // Retorna em caso de erro.
            if (!resultado)
                return resultado;

            return resultado;
        }

        #endregion
    }
}
