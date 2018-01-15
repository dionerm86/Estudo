using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados do tipo de cliente.
    /// </summary>
    public interface IValidadorTipoCliente
    {
        /// <summary>
        /// Valida a existencia do dados do tipo de cliente.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(TipoCliente tipoCliente);
    }

    /// <summary>
    /// Representa a entidade de negócio do tipo de cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoClienteLoader))]
    public class TipoCliente : Colosoft.Business.Entity<Data.Model.TipoCliente>
    {
        #region Tipos Aninhados

        class TipoClienteLoader : Colosoft.Business.EntityLoader<TipoCliente, Data.Model.TipoCliente>
        {
            public TipoClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdTipoCliente)
                    .FindName(f => f.Descricao)
                    .Creator(f => new TipoCliente(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do tipo de cliente.
        /// </summary>
        public int IdTipoCliente
        {
            get { return DataModel.IdTipoCliente; }
            set
            {
                if (DataModel.IdTipoCliente != value &&
                    RaisePropertyChanging("IdTipoCliente", value))
                {
                    DataModel.IdTipoCliente = value;
                    RaisePropertyChanged("IdTipoCliente");
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
        /// Identifica se é para cobrar a área mínima.
        /// </summary>
        public bool CobrarAreaMinima
        {
            get { return DataModel.CobrarAreaMinima; }
            set
            {
                if (DataModel.CobrarAreaMinima != value &&
                    RaisePropertyChanging("CobrarAreaMinima", value))
                {
                    DataModel.CobrarAreaMinima = value;
                    RaisePropertyChanged("CobrarAreaMinima");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TipoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.TipoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoCliente(Data.Model.TipoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove o tipo de cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorTipoCliente>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
