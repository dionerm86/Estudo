using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados da cor do alumínio.
    /// </summary>
    public interface IValidadorCorAluminio
    {
        /// <summary>
        /// Valida a existencia do dados da cor do alumínio.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(CorAluminio corAluminio);
    }

    /// <summary>
    /// Representa a entidade de negócio da cor do aluminio.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CorAluminioLoader))]
    public class CorAluminio : Colosoft.Business.Entity<Data.Model.CorAluminio>
    {
        #region Tipos Aninhados

        class CorAluminioLoader : Colosoft.Business.EntityLoader<CorAluminio, Data.Model.CorAluminio>
        {
            public CorAluminioLoader()
            {
                Configure()
                    .Uid(f => f.IdCorAluminio)
                    .FindName(f => f.Descricao)
                    .Creator(f => new CorAluminio(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da cor d aluminio.
        /// </summary>
        public int IdCorAluminio
        {
            get { return DataModel.IdCorAluminio; }
            set
            {
                if (DataModel.IdCorAluminio != value &&
                    RaisePropertyChanging("IdCorAluminio", value))
                {
                    DataModel.IdCorAluminio = value;
                    RaisePropertyChanged("IdCorAluminio");
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
        /// Sigla.
        /// </summary>
        public string Sigla
        {
            get { return DataModel.Sigla; }
            set
            {
                if (DataModel.Sigla != value &&
                    RaisePropertyChanging("Sigla", value))
                {
                    DataModel.Sigla = value;
                    RaisePropertyChanged("Sigla");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CorAluminio()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CorAluminio(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CorAluminio> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CorAluminio(Data.Model.CorAluminio dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove a cor do alumínio.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorCorAluminio>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
