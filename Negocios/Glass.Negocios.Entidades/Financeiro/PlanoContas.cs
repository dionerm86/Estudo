using Colosoft;
using System.Linq;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de plano de contas.
    /// </summary>
    public interface IValidadorPlanoContas
    {
        /// <summary>
        /// Valida a existencia do plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(PlanoContas planoContas);
    }

    /// <summary>
    /// Representa a entidade do plano de contas.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PlanoContasLoader))]
    public class PlanoContas : Colosoft.Business.Entity<Data.Model.PlanoContas>
    {
        #region Tipos Aninhados

        class PlanoContasLoader : Colosoft.Business.EntityLoader<PlanoContas, Data.Model.PlanoContas>
        {
            public PlanoContasLoader()
            {
                Configure()
                    .Uid(f => f.IdConta)
                    .FindName(f => f.Descricao)
                    .Descriptor(f => new PlanoContasDescritor(f))
                    .Reference<GrupoConta, Data.Model.GrupoConta>("Grupo", f => f.Grupo, f => f.IdGrupo)
                    .Creator(f => new PlanoContas(f));
            }
        }

        #endregion

        #region Propriedades

        public GrupoConta Grupo
        {
            get { return GetReference<GrupoConta>("Grupo", true); }
        }

        /// <summary>
        /// Identificador da conta.
        /// </summary>
        public int IdConta
        {
            get { return DataModel.IdConta; }
            set
            {
                if (DataModel.IdConta != value &&
                    RaisePropertyChanging("IdConta", value))
                {
                    DataModel.IdConta = value;
                    RaisePropertyChanged("IdConta");
                }
            }
        }

        /// <summary>
        /// Identificador da conta dentro do grupo.
        /// </summary>
        public int IdContaGrupo
        {
            get { return DataModel.IdContaGrupo; }
            set
            {
                if (DataModel.IdContaGrupo != value &&
                    RaisePropertyChanging("IdContaGrupo", value))
                {
                    DataModel.IdContaGrupo = value;
                    RaisePropertyChanged("IdContaGrupo");
                }
            }
        }

        /// <summary>
        /// Identificador do grupo associado.
        /// </summary>
        public int IdGrupo
        {
            get { return DataModel.IdGrupo; }
            set
            {
                if (DataModel.IdGrupo != value &&
                    RaisePropertyChanging("IdGrupo", value))
                {
                    DataModel.IdGrupo = value;
                    RaisePropertyChanged("IdGrupo");
                }
            }
        }

        /// <summary>
        /// Identificador do plano contabil vinculado
        /// </summary>
        public int? IdContaContabil
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
        /// Situação do plano de contas.
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
        /// Descrição do plano de contas.
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
        /// Exibir no DRE.
        /// </summary>
        public bool ExibirDre
        {
            get { return DataModel.ExibirDre; }
            set
            {
                if (DataModel.ExibirDre != value && RaisePropertyChanging("ExibirDre", value))
                {
                    DataModel.ExibirDre = value;
                    RaisePropertyChanged("ExibirDre");
                }
            }
        }

        /// <summary>
        /// Fixo.
        /// </summary>
        public bool Fixo
        {
            get { return DataModel.Fixo; }
            set
            {
                if (DataModel.Fixo != value &&
                    RaisePropertyChanging("Fixo", value))
                {
                    DataModel.Fixo = value;
                    RaisePropertyChanged("Fixo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public PlanoContas()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PlanoContas(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.PlanoContas> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PlanoContas(Data.Model.PlanoContas dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado para apagar o plano de contas.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorPlanoContas>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
