using Colosoft;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do tipo de perda.
    /// </summary>
    public interface IValidadorTipoPerda
    {
        /// <summary>
        /// Valida a existencia do tipo de perda.
        /// </summary>
        /// <param name="tipoPerda"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(TipoPerda tipoPerda);
    }

    /// <summary>
    /// Representa a entidade de negócio dos tipos de perda.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoPerdaLoader))]
    [Glass.Negocios.ControleAlteracao(Glass.Data.Model.LogAlteracao.TabelaAlteracao.TipoPerda)]
    public class TipoPerda : Colosoft.Business.Entity<Glass.Data.Model.TipoPerda>
    {
        #region Tipos Aninhados

        class TipoPerdaLoader : Colosoft.Business.EntityLoader<TipoPerda, Glass.Data.Model.TipoPerda>
        {
            public TipoPerdaLoader()
            {
                Configure()
                    .Uid(f => f.IdTipoPerda)
                    .FindName(f => f.Descricao)
                    .Child<SubtipoPerda, Glass.Data.Model.SubtipoPerda>("Subtipos", f => f.Subtipos, f => f.IdTipoPerda)
                    .Creator(f => new TipoPerda(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<SubtipoPerda> _subtipos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Subtipos associados.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<SubtipoPerda> Subtipos
        {
            get { return _subtipos; }
        }

        /// <summary>
        /// Identificador do tipo de perda.
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

        /// <summary>
        /// Identificador do setor associado.
        /// </summary>
        public int? IdSetor
        {
            get { return DataModel.IdSetor; }
            set
            {
                if (DataModel.IdSetor != value &&
                    RaisePropertyChanging("IdSetor", value))
                {
                    DataModel.IdSetor = value;
                    RaisePropertyChanged("IdSetor");
                }
            }
        }
         
        /// <summary>
        /// Situação Tipo Perda
        /// </summary>
        public Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if(DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Exibir Painel Producao.
        /// </summary>
        public bool ExibirPainelProducao
        {
            get { return DataModel.ExibirPainelProducao; }
            set
            {
                if(DataModel.ExibirPainelProducao != value &&
                    RaisePropertyChanging("ExibirPainelProducao", value))
                {
                    DataModel.ExibirPainelProducao = value;
                    RaisePropertyChanged("ExibirPainelProducao");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public TipoPerda()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoPerda(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.TipoPerda> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _subtipos = GetChild<SubtipoPerda>(args.Children, "Subtipos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoPerda(Glass.Data.Model.TipoPerda dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _subtipos = CreateChild<Colosoft.Business.IEntityChildrenList<SubtipoPerda>>("Subtipos");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Apaga o tipo de perda.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorTipoPerda>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
