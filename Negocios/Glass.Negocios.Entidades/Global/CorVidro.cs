using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados da cor do vidro.
    /// </summary>
    public interface IValidadorCorVidro
    {
        /// <summary>
        /// Valida a existencia do dados da cor do vidro.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(CorVidro corVidro);
    }

    /// <summary>
    /// Representa a entidade de negócio da cor do vidro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CorVidroLoader))]
    public class CorVidro : Colosoft.Business.Entity<Data.Model.CorVidro>
    {
        #region Tipos Aninhados

        class CorVidroLoader : Colosoft.Business.EntityLoader<CorVidro, Data.Model.CorVidro>
        {
            public CorVidroLoader()
            {
                Configure()
                    .Uid(f => f.IdCorVidro)
                    .FindName(f => f.Descricao)
                    .Creator(f => new CorVidro(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int IdCorVidro
        {
            get { return DataModel.IdCorVidro; }
            set
            {
                if (DataModel.IdCorVidro != value &&
                    RaisePropertyChanging("IdCorVidro", value))
                {
                    DataModel.IdCorVidro = value;
                    RaisePropertyChanged("IdCorVidro");
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
        public CorVidro()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CorVidro(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CorVidro> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CorVidro(Data.Model.CorVidro dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove a cor do vidro.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorCorVidro>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
