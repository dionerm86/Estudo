using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados do grupo de cliente.
    /// </summary>
    public interface IValidadorGrupoCliente
    {
        /// <summary>
        /// Valida a existencia do dados do grupo de cliente.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(GrupoCliente GrupoCliente);

        /// <summary>
        /// Valida a existencia do dados do grupo de cliente.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaInsercao(GrupoCliente GrupoCliente);
    }

    /// <summary>
    /// Representa a entidade de negócio do grupo de cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(GrupoClienteLoader))]
    public class GrupoCliente : Colosoft.Business.Entity<Data.Model.GrupoCliente>
    {
        #region Tipos Aninhados

        class GrupoClienteLoader : Colosoft.Business.EntityLoader<GrupoCliente, Data.Model.GrupoCliente>
        {
            public GrupoClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdGrupoCliente)
                    .FindName(f => f.Descricao)
                    .Creator(f => new GrupoCliente(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do grupo de cliente.
        /// </summary>
        public int IdGrupoCliente
        {
            get { return DataModel.IdGrupoCliente; }
            set
            {
                if (DataModel.IdGrupoCliente != value &&
                    RaisePropertyChanging("IdGrupoCliente", value))
                {
                    DataModel.IdGrupoCliente = value;
                    RaisePropertyChanged("IdGrupoCliente");
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public GrupoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected GrupoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.GrupoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public GrupoCliente(Data.Model.GrupoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove o grupo de cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorGrupoCliente>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        /// <summary>
        /// Sobrescreve o método que adiciona o grupo de cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorGrupoCliente>();
            var resultadoValidacao = validador.ValidaInsercao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));
            return base.Save(session);
        }

        #endregion
    }
}
