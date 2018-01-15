using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Valida a origem de troca de devolução.
    /// </summary>
    public interface IValidadorOrigemTrocaDesconto
    {
        /// <summary>
        /// Valida a existencia da origem.
        /// </summary>
        /// <param name="origemTrocaDevolucao"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(OrigemTrocaDesconto origemTrocaDevolucao);
    }
    
    /// <summary>
    /// Representa a entidade de negócio da origem troca desconto.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(OrigemTrocaDescontoLoader))]
    public class OrigemTrocaDesconto : Glass.Negocios.Entidades.EntidadeBaseCadastro<Glass.Data.Model.OrigemTrocaDesconto>
    {
        #region Tipos Aninhados

        class OrigemTrocaDescontoLoader : Colosoft.Business.EntityLoader<OrigemTrocaDesconto, Data.Model.OrigemTrocaDesconto>
        {
            public OrigemTrocaDescontoLoader()
            {
                Configure()
                    .Uid(f => f.IdOrigemTrocaDesconto)
                    .FindName(f => f.Descricao)
                    .Creator(f => new OrigemTrocaDesconto(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da origem de troca desconto.
        /// </summary>
        public int IdOrigemTrocaDesconto
        {
            get { return DataModel.IdOrigemTrocaDesconto; }
            set
            {
                if (DataModel.IdOrigemTrocaDesconto != value &&
                    RaisePropertyChanging("IdOrigemTrocaDesconto", value))
                {
                    DataModel.IdOrigemTrocaDesconto = value;
                    RaisePropertyChanged("IdOrigemTrocaDesconto");
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
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public OrigemTrocaDesconto()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected OrigemTrocaDesconto(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.OrigemTrocaDesconto> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public OrigemTrocaDesconto(Data.Model.OrigemTrocaDesconto dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Apaga os dados da origem.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorOrigemTrocaDesconto>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
