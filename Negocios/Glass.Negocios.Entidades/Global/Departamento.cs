using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos departamentos.
    /// </summary>
    public interface IValidadorDepartamento
    {
        /// <summary>
        /// Valida a existencia do departamento.
        /// </summary>
        /// <param name="departamento"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Departamento departamento);
    }

    /// <summary>
    /// Representa a entidade de negócio do departamento.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(DepartamentoLoader))]
    public class Departamento : Colosoft.Business.Entity<Data.Model.Departamento>
    {
        #region Tipos Aninhados

        class DepartamentoLoader : Colosoft.Business.EntityLoader<Departamento, Data.Model.Departamento>
        {
            public DepartamentoLoader()
            {
                Configure()
                    .Uid(f => f.IdDepartamento)
                    .FindName(f => f.Nome)
                    .Creator(f => new Departamento(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do departamento.
        /// </summary>
        public int IdDepartamento
        {
            get { return DataModel.IdDepartamento; }
            set
            {
                if (DataModel.IdDepartamento != value &&
                    RaisePropertyChanging("IdDepartamento", value))
                {
                    DataModel.IdDepartamento = value;
                    RaisePropertyChanged("IdDepartamento");
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
        public Departamento()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Departamento(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Departamento> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Departamento(Data.Model.Departamento dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Apaga o departamento.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorDepartamento>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
