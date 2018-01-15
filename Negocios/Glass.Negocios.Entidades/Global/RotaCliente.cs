namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da associação entre a rota e o cliente.
    /// </summary>
    public interface IValidadorRotaCliente
    {
        /// <summary>
        /// Recupera um novo número de sequencia para a associação da rota com o cliente.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        int ObtemNumeroSequencia(RotaCliente rotaCliente);
    }

    /// <summary>
    /// Representa a entidade de negócio da rota do cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(RotaClienteLoader))]
    public class RotaCliente : Colosoft.Business.Entity<Data.Model.RotaCliente>
    {
        #region Tipos aninhados

        class RotaClienteLoader : Colosoft.Business.EntityLoader<RotaCliente, Data.Model.RotaCliente>
        {
            public RotaClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdRotaCliente)
                    .Creator(f => new RotaCliente(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da rota de cliente.
        /// </summary>
        public int IdRotaCliente
        {
            get { return DataModel.IdRotaCliente; }
            set
            {
                if (DataModel.IdRotaCliente != value &&
                    RaisePropertyChanging("IdRotaCliente", value))
                {
                    DataModel.IdRotaCliente = value;
                    RaisePropertyChanged("IdRotaCliente");
                }
            }
        }

        /// <summary>
        /// Código da rota associada ao cliente.
        /// </summary>
        public int IdRota
        {
            get { return DataModel.IdRota; }
            set
            {
                if (DataModel.IdRota != value &&
                    RaisePropertyChanging("IdRota", value))
                {
                    DataModel.IdRota = value;
                    RaisePropertyChanged("IdRota");
                }
            }
        }

        /// <summary>
        /// Código do cliente.
        /// </summary>
        public int IdCliente
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
        /// Número de sequência do cliente na rota.
        /// </summary>
        public int NumSeq
        {
            get { return DataModel.NumSeq; }
            set
            {
                if (DataModel.NumSeq != value &&
                    RaisePropertyChanging("NumSeq", value))
                {
                    DataModel.NumSeq = value;
                    RaisePropertyChanged("NumSeq");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RotaCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RotaCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.RotaCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RotaCliente(Data.Model.RotaCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Método Públicos

        /// <summary>
        /// Salva os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (!ExistsInStorage && NumSeq == 0)
            {
                var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IValidadorRotaCliente>();

                // Recupera o número de sequencia.
                NumSeq = validador.ObtemNumeroSequencia(this);
            }

            return base.Save(session);
        }

        #endregion
    }
}
