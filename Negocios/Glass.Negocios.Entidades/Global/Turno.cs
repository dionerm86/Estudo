using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de turnos.
    /// </summary>
    public interface IValidadorTurno
    {
        /// <summary>
        /// Valida a atualização do turno.
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Turno turno);
    }

    /// <summary>
    /// Representa a entidade de negócio do turno.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TurnoLoader))]
    public class Turno : Colosoft.Business.Entity<Glass.Data.Model.Turno>
    {
        #region Tipos Aninhados

        class TurnoLoader : Colosoft.Business.EntityLoader<Turno, Glass.Data.Model.Turno>
        {
            public TurnoLoader()
            {
                Configure()
                    .Uid(f => f.IdTurno)
                    .FindName(f => f.Descricao)
                    .Creator(f => new Turno(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do turno.
        /// </summary>
        public int IdTurno
        {
            get { return DataModel.IdTurno; }
            set
            {
                if (DataModel.IdTurno != value &&
                    RaisePropertyChanging("IdTurno", value))
                {
                    DataModel.IdTurno = value;
                    RaisePropertyChanged("IdTurno");
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
        /// Número de sequencia.
        /// </summary>
        public Glass.Data.Model.TurnoSequencia NumSeq
        {
            get { return DataModel.NumSeq; }
            set
            {
                if (DataModel.NumSeq != value &&
                    RaisePropertyChanging("NumSeq", value))
                {
                    DataModel.NumSeq = value;
                    RaisePropertyChanged("NumSeq");
                }
            }
        }

        /// <summary>
        /// Início.
        /// </summary>
        public string Inicio
        {
            get { return DataModel.Inicio; }
            set
            {
                if (DataModel.Inicio != value &&
                    RaisePropertyChanging("Inicio", value))
                {
                    DataModel.Inicio = value;
                    RaisePropertyChanged("Inicio");
                }
            }
        }

        /// <summary>
        /// Término.
        /// </summary>
        public string Termino
        {
            get { return DataModel.Termino; }
            set
            {
                if (DataModel.Termino != value &&
                    RaisePropertyChanging("Termino", value))
                {
                    DataModel.Termino = value;
                    RaisePropertyChanged("Termino");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Turno()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Turno(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Turno> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Turno(Glass.Data.Model.Turno dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva o turno.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorTurno>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        #endregion
    }
}
