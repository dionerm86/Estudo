using Colosoft;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da classificação de roteiro de produção.
    /// </summary>
    public interface IValidadorClassificacaoRoteiroProducao
    {
        /// <summary>
        /// Valida a existência da classificação.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(ClassificacaoRoteiroProducao classificacao);
    }

    [Colosoft.Business.EntityLoader(typeof(ClassificacaoRoteiroProducaoLoader))]
    [Glass.Negocios.ControleAlteracao(Glass.Data.Model.LogAlteracao.TabelaAlteracao.ClassificacaoRoteiroProducao)]
    public class ClassificacaoRoteiroProducao : Colosoft.Business.Entity<Data.Model.ClassificacaoRoteiroProducao>
    {
        #region Tipos Aninhados

        class ClassificacaoRoteiroProducaoLoader : Colosoft.Business.EntityLoader<ClassificacaoRoteiroProducao, Data.Model.ClassificacaoRoteiroProducao>
        {
            public ClassificacaoRoteiroProducaoLoader()
            {
                Configure()
                    .Uid(f => f.IdClassificacaoRoteiroProducao)
                    .FindName(f => f.Descricao)
                    .Child<RoteiroProducao, Glass.Data.Model.RoteiroProducao>("Roteiros", f => f.Roteiros, f => f.IdClassificacaoRoteiroProducao)
                    .Creator(f => new ClassificacaoRoteiroProducao(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<RoteiroProducao> _roteiros;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ClassificacaoRoteiroProducao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ClassificacaoRoteiroProducao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ClassificacaoRoteiroProducao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _roteiros = GetChild<RoteiroProducao>(args.Children, "Roteiros");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ClassificacaoRoteiroProducao(Data.Model.ClassificacaoRoteiroProducao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _roteiros = CreateChild<Colosoft.Business.IEntityChildrenList<RoteiroProducao>>("Roteiros");
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da Classificação.
        /// </summary>
        public int IdClassificacaoRoteiroProducao
        {
            get { return DataModel.IdClassificacaoRoteiroProducao; }
            set
            {
                if (DataModel.IdClassificacaoRoteiroProducao != value &&
                    RaisePropertyChanging("IdClassificacaoRoteiroProducao", value))
                {
                    DataModel.IdClassificacaoRoteiroProducao = value;
                    RaisePropertyChanged("IdClassificacaoRoteiroProducao");
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
        /// Capacidade Diária.
        /// </summary>
        public int CapacidadeDiaria
        {
            get { return DataModel.CapacidadeDiaria; }
            set
            {
                if (DataModel.CapacidadeDiaria != value &&
                    RaisePropertyChanging("CapacidadeDiaria", value))
                {
                    DataModel.CapacidadeDiaria = value;
                    RaisePropertyChanged("CapacidadeDiaria");
                }
            }
        }

        /// <summary>
        /// MetaDiaria.
        /// </summary>
        public decimal MetaDiaria
        {
            get { return DataModel.MetaDiaria; }
            set
            {
                if (DataModel.MetaDiaria != value &&
                    RaisePropertyChanging("MetaDiaria", value))
                {
                    DataModel.MetaDiaria = value;
                    RaisePropertyChanged("MetaDiaria");
                }
            }
        }

        /// <summary>
        /// Roteiros associados a classificação
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<RoteiroProducao> Roteiros
        {
            get { return _roteiros; }
        }

        #endregion

        #region Metodos Sobrescritos

        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorClassificacaoRoteiroProducao>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            var deleteResult = OnDeleting();

            if (!deleteResult)
                return deleteResult;

            foreach (var r in Roteiros)
            {
                r.IdClassificacaoRoteiroProducao = 0;
                var saveResult = r.Save(session);
                if (!saveResult)
                    return new Colosoft.Business.DeleteResult(false, saveResult.Message);
            }

            session.Delete(DataModel, (action, result) =>
            {
                if (result.Success)
                {
                    Roteiros.Clear();
                    OnDeleted(true, null);
                }
                else
                    OnDeleted(false, result.FailureMessage.GetFormatter());
            });

            return new Colosoft.Business.DeleteResult(true, null);
        }

        #endregion
    }
}
