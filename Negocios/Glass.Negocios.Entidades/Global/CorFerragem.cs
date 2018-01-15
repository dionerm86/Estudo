using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados da cor da ferragem.
    /// </summary>
    public interface IValidadorCorFerragem
    {
        /// <summary>
        /// Valida a existencia do dados da cor da ferragem.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(CorFerragem corFerragem);
    }

    /// <summary>
    /// Representa a entidade de negócio da cor da ferragem.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CorFerragemLoader))]
    public class CorFerragem : Colosoft.Business.Entity<Data.Model.CorFerragem>
    {
        #region Tipos Aninhados

        class CorFerragemLoader : Colosoft.Business.EntityLoader<CorFerragem, Data.Model.CorFerragem>
        {
            public CorFerragemLoader()
            {
                Configure()
                    .Uid(f => f.IdCorFerragem)
                    .FindName(f => f.Descricao)
                    .Creator(f => new CorFerragem(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da cor da ferragem.
        /// </summary>
        public int IdCorFerragem
        {
            get { return DataModel.IdCorFerragem; }
            set
            {
                if (DataModel.IdCorFerragem != value &&
                    RaisePropertyChanging("IdCorFerragem", value))
                {
                    DataModel.IdCorFerragem = value;
                    RaisePropertyChanged("IdCorFerragem");
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
        public CorFerragem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CorFerragem(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CorFerragem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CorFerragem(Data.Model.CorFerragem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove a cor da ferragem.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorCorFerragem>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
