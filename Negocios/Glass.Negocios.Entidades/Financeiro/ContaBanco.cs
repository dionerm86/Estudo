using Colosoft;
using System.Linq;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do ContaBanco.
    /// </summary>
    public interface IValidadorContaBanco
    {
        /// <summary>
        /// Valida a existencia da conta banco.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(ContaBanco contaBanco);
    }

    /// <summary>
    /// Representa a entidade de negócio da conta bancária.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ContaBancoLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.ContaBanco)]
    public class ContaBanco : Colosoft.Business.Entity<Glass.Data.Model.ContaBanco>
    {
        #region Tipos Aninhados

        class ContaBancoLoader : Colosoft.Business.EntityLoader<ContaBanco, Glass.Data.Model.ContaBanco>
        {
            public ContaBancoLoader()
            {
                Configure()
                    .Uid(f => f.IdContaBanco)
                    .FindName(new ContaBancoFindNameConverter(), f => f.CodConvenio, f => f.Nome, f => f.Agencia, f => f.Conta)
                    .Creator(f => new ContaBanco(f));
            }
        }

        class ContaBancoFindNameConverter : IFindNameConverter
        {
            public string Convert(object[] baseInfo)
            {
                var codConvenio = baseInfo[0];
                var nome = baseInfo[1];
                var agencia = baseInfo[2];
                var conta = baseInfo[3];

                return nome + " Agência: " + agencia + " Conta: " + conta;
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da conta do banco.
        /// </summary>
        public int IdContaBanco
        {
            get { return DataModel.IdContaBanco; }
            set
            {
                if (DataModel.IdContaBanco != value &&
                    RaisePropertyChanging("IdContaBanco", value))
                {
                    DataModel.IdContaBanco = value;
                    RaisePropertyChanged("IdContaBanco");
                }
            }
        }

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Código do banco.
        /// </summary>
        public int? CodBanco
        {
            get { return DataModel.CodBanco; }
            set
            {
                if (DataModel.CodBanco != value &&
                    RaisePropertyChanging("CodBanco", value))
                {
                    DataModel.CodBanco = value;
                    RaisePropertyChanged("CodBanco");
                }
            }
        }

        /// <summary>
        /// Código do Convenio.
        /// </summary>
        public string CodConvenio
        {
            get { return DataModel.CodConvenio; }
            set
            {
                if (DataModel.CodConvenio != value &&
                    RaisePropertyChanging("CodConvenio", value))
                {
                    DataModel.CodConvenio = value;
                    RaisePropertyChanged("CodConvenio");
                }
            }
        }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        /// <summary>
        /// Agência.
        /// </summary>
        public string Agencia
        {
            get { return DataModel.Agencia; }
            set
            {
                if (DataModel.Agencia != value &&
                    RaisePropertyChanging("Agencia", value))
                {
                    DataModel.Agencia = value;
                    RaisePropertyChanged("Agencia");
                }
            }
        }

        /// <summary>
        /// Conta.
        /// </summary>
        public string Conta
        {
            get { return DataModel.Conta; }
            set
            {
                if (DataModel.Conta != value &&
                    RaisePropertyChanging("Conta", value))
                {
                    DataModel.Conta = value;
                    RaisePropertyChanged("Conta");
                }
            }
        }

        /// <summary>
        /// Posto.
        /// </summary>
        public int? Posto
        {
            get { return DataModel.Posto; }
            set
            {
                if (DataModel.Posto != value &&
                    RaisePropertyChanging("Posto", value))
                {
                    DataModel.Posto = value;
                    RaisePropertyChanged("Posto");
                }
            }
        }

        /// <summary>
        /// Titular.
        /// </summary>
        public string Titular
        {
            get { return DataModel.Titular; }
            set
            {
                if (DataModel.Titular != value &&
                    RaisePropertyChanging("Titular", value))
                {
                    DataModel.Titular = value;
                    RaisePropertyChanged("Titular");
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

        /// <summary>
        /// Cód do cliente no banco.
        /// </summary>
        public string CodCliente
        {
            get { return DataModel.CodCliente; }
            set
            {
                if (DataModel.CodCliente != value &&
                    RaisePropertyChanging("CodCliente", value))
                {
                    DataModel.CodCliente = value;
                    RaisePropertyChanged("CodCliente");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ContaBanco()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ContaBanco(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ContaBanco> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ContaBanco(Glass.Data.Model.ContaBanco dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método usado para apagar a conta.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorContaBanco>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
