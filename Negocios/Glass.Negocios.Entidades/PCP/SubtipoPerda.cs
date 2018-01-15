using Colosoft;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do subtipo de perda.
    /// </summary>
    public interface IValidadorSubtipoPerda
    {
        /// <summary>
        /// Valida a existencia do subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(SubtipoPerda subtipoPerda);
    }

    /// <summary>
    /// Representa a entidade de negócio do subtipo de perda.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SubtipoPerdaLoader))]
    [Glass.Negocios.ControleAlteracao(Glass.Data.Model.LogAlteracao.TabelaAlteracao.SubtipoPerda)]
    public class SubtipoPerda : Colosoft.Business.Entity<Glass.Data.Model.SubtipoPerda>
    {
        #region Tipos Aninhados

        class SubtipoPerdaLoader : Colosoft.Business.EntityLoader<SubtipoPerda, Glass.Data.Model.SubtipoPerda>
        {
            public SubtipoPerdaLoader()
            {
                Configure()
                    .Uid(f => f.IdSubtipoPerda)
                    .FindName(f => f.Descricao)
                    .Creator(f => new SubtipoPerda(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do subtipo de perda.
        /// </summary>
        public int IdSubtipoPerda
        {
            get { return DataModel.IdSubtipoPerda; }
            set
            {
                if (DataModel.IdSubtipoPerda != value &&
                    RaisePropertyChanging("IdSubtipoPerda", value))
                {
                    DataModel.IdSubtipoPerda = value;
                    RaisePropertyChanged("IdSubtipoPerda");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de perda associado.
        /// </summary>
        public int IdTipoPerda
        {
            get { return DataModel.IdTipoPerda; }
            set
            {
                if (DataModel.IdTipoPerda != value &&
                    RaisePropertyChanging("IdTipoPerda", value))
                {
                    DataModel.IdTipoPerda = value;
                    RaisePropertyChanged("IdTipoPerda");
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

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public SubtipoPerda()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected SubtipoPerda(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.SubtipoPerda> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public SubtipoPerda(Glass.Data.Model.SubtipoPerda dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Apaga o subtipo de perda.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorSubtipoPerda>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
