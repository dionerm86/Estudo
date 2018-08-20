namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da sugestão do cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SugestaoClienteLoader))]
    public class SugestaoCliente : Glass.Negocios.Entidades.EntidadeBaseCadastro<Glass.Data.Model.SugestaoCliente>
    {
        #region Tipos Aninhados

        class SugestaoClienteLoader : Colosoft.Business.EntityLoader<SugestaoCliente, Glass.Data.Model.SugestaoCliente>
        {
            public SugestaoClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdSugestao)
                    .FindName(f => f.Descricao)
                    .Reference<Cliente, Data.Model.Cliente>("Cliente", f => f.Cliente, f => f.IdCliente)
                    .Reference<Funcionario, Data.Model.Funcionario>("Funcionario", f => f.Funcionario, f => f.Usucad)
                    .Creator(f => new SugestaoCliente(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do cliente associado.
        /// </summary>
        public Cliente Cliente
        {
            get { return GetReference<Cliente>("Cliente", true); }
        }
        
        /// <summary>
        /// Instancia do funcionário associado.
        /// </summary>
        public Funcionario Funcionario
        {
            get { return GetReference<Funcionario>("Funcionario", true); }
        }

        /// <summary>
        /// Identificador da sugestão.
        /// </summary>
        public int IdSugestao
        {
            get { return DataModel.IdSugestao; }
            set
            {
                if (DataModel.IdSugestao != value &&
                    RaisePropertyChanging("IdSugestao", value))
                {
                    DataModel.IdSugestao = value;
                    RaisePropertyChanged("IdSugestao");
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
        /// Tipo de sugestão.
        /// </summary>
        public int TipoSugestao
        {
            get { return DataModel.TipoSugestao; }
            set
            {
                if (DataModel.TipoSugestao != value &&
                    RaisePropertyChanging("TipoSugestao", value))
                {
                    DataModel.TipoSugestao = value;
                    RaisePropertyChanged("TipoSugestao");
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
        /// Identifica se a sugestão foi cancelada.
        /// </summary>
        public bool Cancelada
        {
            get { return DataModel.Cancelada; }
            set
            {
                if (DataModel.Cancelada != value &&
                    RaisePropertyChanging("Cancelada", value))
                {
                    DataModel.Cancelada = value;
                    RaisePropertyChanged("Cancelada");
                }
            }
        }

        /// <summary>
        /// Id do pedido.
        /// </summary>
        public int? IdPedido
        {
            get { return DataModel.IdPedido; }
            set
            {
                if (DataModel.IdPedido != value &&
                    RaisePropertyChanging("IdPedido", value))
                {
                    DataModel.IdPedido = value;
                    RaisePropertyChanged("IdPedido");
                }
            }
        }

        public uint? IdOrcamento
        {
            get { return DataModel.IdOrcamento; }
            set
            {
                if(DataModel.IdOrcamento != value && RaisePropertyChanging("IdOrcamento", value))
                {
                    DataModel.IdOrcamento = value;
                    RaisePropertyChanged("IdOrcamento");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public SugestaoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected SugestaoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.SugestaoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public SugestaoCliente(Glass.Data.Model.SugestaoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
