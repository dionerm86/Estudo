using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados da tabela de desconto/acréscimo.
    /// </summary>
    public interface IValidadorTabelaDescontoAcrescimoCliente
    {
        /// <summary>
        /// Valida a existencia do dados da tabela de desconto/acréscimo.
        /// </summary>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(TabelaDescontoAcrescimoCliente tabelaDesconto);
    }

    /// <summary>
    /// Representa a entidade de negócio da tabela de desconto/acréscimo do cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TabelaDescontoAcrescimoClienteLoader))]
    public class TabelaDescontoAcrescimoCliente : Colosoft.Business.Entity<Data.Model.TabelaDescontoAcrescimoCliente>
    {
        #region Tipos Aninhados

        class TabelaDescontoAcrescimoClienteLoader : Colosoft.Business.EntityLoader<TabelaDescontoAcrescimoCliente, Data.Model.TabelaDescontoAcrescimoCliente>
        {
            public TabelaDescontoAcrescimoClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdTabelaDesconto)
                    .FindName(f => f.Descricao)
                    .Child<DescontoAcrescimoCliente, Data.Model.DescontoAcrescimoCliente>("DescontosAcrescimos",
                        f => f.DescontosAcrescimos, f => f.IdTabelaDesconto)
                    .Creator(f => new TabelaDescontoAcrescimoCliente(f));
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente> _descontosAcrescimos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da tabela de desconto/acréscimo do cliente.
        /// </summary>
        public int IdTabelaDesconto
        {
            get { return DataModel.IdTabelaDesconto; }
            set
            {
                if (DataModel.IdTabelaDesconto != value &&
                    RaisePropertyChanging("IdTabelaDesconto", value))
                {
                    DataModel.IdTabelaDesconto = value;
                    RaisePropertyChanged("IdTabelaDesconto");
                }
            }
        }

        /// <summary>
        /// Descrição para a tabela de desconto/acréscimo do cliente.
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

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// Descontos/acréscimos da tabela.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente> DescontosAcrescimos
        {
            get { return _descontosAcrescimos; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TabelaDescontoAcrescimoCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TabelaDescontoAcrescimoCliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.TabelaDescontoAcrescimoCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _descontosAcrescimos = GetChild<DescontoAcrescimoCliente>(args.Children, "DescontosAcrescimos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TabelaDescontoAcrescimoCliente(Data.Model.TabelaDescontoAcrescimoCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _descontosAcrescimos = CreateChild<Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente>>("DescontosAcrescimos");
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método que remove a tabela de desconto/acréscimo cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorTabelaDescontoAcrescimoCliente>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
