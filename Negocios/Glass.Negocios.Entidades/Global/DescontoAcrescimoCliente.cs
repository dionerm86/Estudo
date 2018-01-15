using System;
using Colosoft.Business;
using Colosoft.Data;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    public interface IProvedorDescontoAcrescimoCliente
    {
        /// <summary>
        /// Verifica se Existe Desconto na tabela de desconto acrescimo cliente.
        /// </summary>
        bool ExisteDescontoGrupoSubgrupoProdutoPorCliente(int idCliente, int? idGrupoProd, int? idSubgrupoProd, int? idProduto);
    }

    /// <summary>
    /// Assinatura dos métodos de recuperação da descrição de desconto/acréscimo cliente.
    /// </summary>
    public interface IDescricaoDescontoAcrescimoCliente
    {
        /// <summary>
        /// Obtém a descrição do item de desconto/acréscimo cliente.
        /// </summary>
        /// <returns></returns>
        string ObtemDescricao(int idDescontoAcrescimoCliente);

        /// <summary>
        /// Obtém a descrição do item de desconto/acréscimo cliente.
        /// </summary>
        /// <returns></returns>
        string ObtemDescricao(int? idCliente, int? idTabelaDescontoAcrescimo, int idGrupoProd, int? idSubgrupoProd, int? idProduto);
    }

    /// <summary>
    /// Representa a entidade de negócio do desconto/acréscimo de cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(DescontoAcrescimoClienteLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.DescontoAcrescimoCliente, true)]
    public class DescontoAcrescimoCliente : Colosoft.Business.Entity<Data.Model.DescontoAcrescimoCliente>
    {
        #region Tipos aninhados

        class DescontoAcrescimoClienteLoader : Colosoft.Business.EntityLoader<DescontoAcrescimoCliente, Data.Model.DescontoAcrescimoCliente>
        {
            public DescontoAcrescimoClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdDesconto)
                    .FindName(new DescontoAcrescimoClienteFindNameConverter(),
                        f => f.IdCliente, f => f.IdTabelaDesconto, f => f.IdGrupoProd,
                        f => f.IdSubgrupoProd, f => f.IdProduto)
                    .Reference<GrupoProd, Data.Model.GrupoProd>("GrupoProduto", f => f.GrupoProduto, f => f.IdGrupoProd)
                    .Reference<SubgrupoProd, Data.Model.SubgrupoProd>("SubgrupoProduto", f => f.SubgrupoProduto, f => f.IdSubgrupoProd)
                    .Reference<Produto, Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProduto)
                    .Creator(f => new DescontoAcrescimoCliente(f));
            }

            class DescontoAcrescimoClienteFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome do desconto/acréscimo de cliente.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var idCliente = (int?)baseInfo[0];
                    var idTabelaDescontoAcrescimo = (int?)baseInfo[1];
                    var idGrupoProd = (int)baseInfo[2];
                    var idSubgrupoProd = (int?)baseInfo[3];
                    var idProduto = (int?)baseInfo[4];

                    return Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                        .GetInstance<IDescricaoDescontoAcrescimoCliente>()
                        .ObtemDescricao(idCliente, idTabelaDescontoAcrescimo, idGrupoProd, idSubgrupoProd, idProduto);
                }
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do desconto/acréscimo do cliente.
        /// </summary>
        public int IdDesconto
        {
            get { return DataModel.IdDesconto; }
            set
            {
                if (DataModel.IdDesconto != value &&
                    RaisePropertyChanging("IdDesconto", value))
                {
                    DataModel.IdDesconto = value;
                    RaisePropertyChanged("IdDesconto");
                }
            }
        }

        /// <summary>
        /// Código do cliente no desconto/acréscimo.
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
        /// Código da tabela de desconto/acréscimo de cliente.
        /// </summary>
        public int? IdTabelaDesconto
        {
            get { return DataModel.IdTabelaDesconto; }
            set
            {
                if (DataModel.IdTabelaDesconto != value &&
                    RaisePropertyChanging("IdTabelaDesconto", value))
                {
                    DataModel.IdTabelaDesconto = value;
                    RaisePropertyChanged("IdTabelaDesconto");
                }
            }
        }

        /// <summary>
        /// Código do grupo do desconto/acréscimo de cliente.
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
        /// Código do subgrupo do desconto/acréscimo de cliente.
        /// </summary>
        public int? IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Código do produto do desconto/acréscimo de cliente.
        /// </summary>
        public int? IdProduto
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
        /// Percentual de desconto no desconto/acréscimo de cliente.
        /// </summary>
        public float Desconto
        {
            get { return DataModel.Desconto; }
            set
            {
                if (DataModel.Desconto != value &&
                    RaisePropertyChanging("Desconto", value))
                {
                    DataModel.Desconto = value;
                    RaisePropertyChanged("Desconto");
                }
            }
        }

        /// <summary>
        /// Percentual de acréscimo no desconto/acréscimo de cliente.
        /// </summary>
        public float Acrescimo
        {
            get { return DataModel.Acrescimo; }
            set
            {
                if (DataModel.Acrescimo != value &&
                    RaisePropertyChanging("Acrescimo", value))
                {
                    DataModel.Acrescimo = value;
                    RaisePropertyChanged("Acrescimo");
                }
            }
        }

        /// <summary>
        /// Aplicar desconto/acréscimo nos beneficiamentos?
        /// </summary>
        public bool AplicarBeneficiamentos
        {
            get { return DataModel.AplicarBeneficiamentos; }
            set
            {
                if (DataModel.AplicarBeneficiamentos != value &&
                    RaisePropertyChanging("AplicarBeneficiamentos", value))
                {
                    DataModel.AplicarBeneficiamentos = value;
                    RaisePropertyChanged("AplicarBeneficiamentos");
                }
            }
        }

        #endregion

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// Cliente referente ao desconto/acréscimo.
        /// </summary>
        public Cliente Cliente
        {
            get
            {
                // Recupera a tabela de desconto/acréscimo pai.
                var cliente = Owner as Cliente;

                if (cliente == null)
                    throw new InvalidOperationException("Só é possível salvar o desconto/acréscimo com a instância associada ao cliente.");

                return cliente;
            }
        }

        /// <summary>
        /// Tabela referente ao desconto/acréscimo.
        /// </summary>
        public TabelaDescontoAcrescimoCliente TabelaDescontoAcrescimo
        {
            get
            {
                // Recupera a tabela de desconto/acréscimo pai.
                var tabelaDescontoAcrescimoCliente = Owner as TabelaDescontoAcrescimoCliente;

                if (tabelaDescontoAcrescimoCliente == null)
                    throw new InvalidOperationException("Só é possível salvar o desconto/acréscimo com a instância associada à tabela.");

                return tabelaDescontoAcrescimoCliente;
            }
        }

        /// <summary>
        /// Grupo de produto referente ao desconto/acréscimo.
        /// </summary>
        public GrupoProd GrupoProduto
        {
            get { return GetReference<GrupoProd>("GrupoProduto", true); }
        }

        /// <summary>
        /// Subgrupo de produto referente ao desconto/acréscimo.
        /// </summary>
        public SubgrupoProd SubgrupoProduto
        {
            get { return GetReference<SubgrupoProd>("SubgrupoProduto", true); }
        }

        /// <summary>
        /// Produto referente ao desconto/acréscimo.
        /// </summary>
        public Produto Produto
        {
            get { return GetReference<Produto>("Produto", true); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public DescontoAcrescimoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected DescontoAcrescimoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.DescontoAcrescimoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public DescontoAcrescimoCliente(Data.Model.DescontoAcrescimoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
        
        static volatile object _salvarDescontoAcrescimoLock = new object();

        public override SaveResult Save(IPersistenceSession session)
        {
            /* Chamados 48638, 48919 e 48921.
             * Caso o cliente já possua acréscimo/desconto no grupo/subgrupo/produto informado, a exceção é lançada. */
            if ((IdDesconto == 0 || IdDesconto < 0) && IdCliente > 0)
            {
                var existeDesconto = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IProvedorDescontoAcrescimoCliente>()
                    .ExisteDescontoGrupoSubgrupoProdutoPorCliente(IdCliente.Value, IdGrupoProd, IdSubgrupoProd, IdProduto);

                if (existeDesconto)
                    return new SaveResult(false, ("Não é possivel salvar o desconto, pois, já existe registro na tabela " +
                        "deste grupo/subgrupo/produto para o cliente. Atualize a tela e tente novamente.").GetFormatter());
            }
            /* Chamado 45969.
             * É extremamente necessário que o cliente seja setado como nulo e o Owner seja alterado para a tabela,
             * senão, ao inserir um cliente novo e associá-lo a uma tabela, o sistema considera que o ID ainda não foi preenchido
             * no registro de desconto/acréscimo e salva o registro com ID tabela e ID cliente juntos, fazendo com que
             * o banco de dados fique incorreto.*/
            if (IdTabelaDesconto > 0)
                IdCliente = null;
            else if (IdCliente > 0)
                IdTabelaDesconto = null;

            return base.Save(session);
        }
    }
}
