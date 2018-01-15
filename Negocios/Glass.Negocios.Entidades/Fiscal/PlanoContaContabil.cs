using System.Linq;
using Colosoft;

namespace Glass.Fiscal.Negocios.Entidades
{
    public interface IValidadorPlanoContaContabil
    {
        /// <summary>
        /// Valida a existencia do plano conta contabil.
        /// </summary>
        /// <param name="planoContaContabil"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidarExistencia(PlanoContaContabil planoContaContabil);

    }

    /// <summary>
    /// Representa a entidade de negócio do Plano de Conta Contabil.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PlanoContaContabilLoader))]
    public partial class PlanoContaContabil : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.PlanoContaContabil>
    {
        #region Tipos aninhados

        class PlanoContaContabilLoader : Colosoft.Business.EntityLoader<PlanoContaContabil, Data.Model.PlanoContaContabil>
        {
            public PlanoContaContabilLoader()
            {
                Configure()
                    .Uid(f => f.IdContaContabil)
                    .FindName(f => f.Descricao)
                    .Creator(f => new PlanoContaContabil(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do plano de conta contábil..
        /// </summary>
        public int IdContaContabil
        {
            get { return DataModel.IdContaContabil; }
            set
            {
                if (DataModel.IdContaContabil != value &&
                    RaisePropertyChanging("IdContaContabil", value))
                {
                    DataModel.IdContaContabil = value;
                    RaisePropertyChanged("IdContaContabil");
                }
            }
        }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno
        {
            get { return DataModel.CodInterno; }
            set
            {
                if (DataModel.CodInterno != value &&
                    RaisePropertyChanging("CodInterno", value))
                {
                    DataModel.CodInterno = value;
                    RaisePropertyChanged("CodInterno");
                }
            }
        }

        /// <summary>
        /// Descrição do plano de conta contábil.
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
        /// Natureza do plano de conta contábil.
        /// </summary>
        public int Natureza
        {
            get { return DataModel.Natureza; }
            set
            {
                if (DataModel.Natureza != value &&
                    RaisePropertyChanging("Natureza", value))
                {
                    DataModel.Natureza = value;
                    RaisePropertyChanged("Natureza");
                }
            }
        }

        /// <summary>
        /// Código conta.
        /// </summary>
        public string CodContaRfb
        {
            get { return DataModel.CodContaRfb; }
            set
            {
                if (DataModel.CodContaRfb != value &&
                    RaisePropertyChanging("CodContaRfb", value))
                {
                    DataModel.CodContaRfb = value;
                    RaisePropertyChanged("CodContaRfb");
                }
            }
        }

        #endregion

        #region Construtor

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public PlanoContaContabil()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PlanoContaContabil(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.PlanoContaContabil> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PlanoContaContabil(Data.Model.PlanoContaContabil dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Publicos
        /// <summary>
        /// Método acionado para apagar
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorPlanoContaContabil>();

            var resultadoValidacao = validador.ValidarExistencia(this);

            if (resultadoValidacao.Any())
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
