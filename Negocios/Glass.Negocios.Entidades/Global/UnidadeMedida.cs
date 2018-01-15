using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da unidade de medida.
    /// </summary>
    public interface IValidadorUnidadeMedida
    {
        /// <summary>
        /// Valida a existencia da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(UnidadeMedida unidadeMedida);

        /// <summary>
        /// Valida a atualização da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(UnidadeMedida unidadeMedida);
    }

    /// <summary>
    /// Representa uma entidade de negócio da unidade de medida.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(UnidadeMedidaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.UnidadeMedida)]
    public class UnidadeMedida : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.UnidadeMedida>
    {
        #region Tipos Aninhados

        class UnidadeMedidaLoader : Colosoft.Business.EntityLoader<UnidadeMedida, Data.Model.UnidadeMedida>
        {
            public UnidadeMedidaLoader()
            {
                Configure()
                    .Uid(f => f.IdUnidadeMedida)
                    .FindName(f => f.Codigo)
                    .Description(f => f.Descricao)
                    .Creator(f => new UnidadeMedida(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da unidade de medida.
        /// </summary>
        public int IdUnidadeMedida
        {
            get { return DataModel.IdUnidadeMedida; }
            set
            {
                if (DataModel.IdUnidadeMedida != value &&
                    RaisePropertyChanging("IdUnidadeMedida", value))
                {
                    DataModel.IdUnidadeMedida = value;
                    RaisePropertyChanged("IdUnidadeMedida");
                }
            }
        }

        /// <summary>
        /// Código da unidade de medida.
        /// </summary>
        public string Codigo
        {
            get { return DataModel.Codigo; }
            set
            {
                if (DataModel.Codigo != value &&
                    RaisePropertyChanging("Codigo", value))
                {
                    DataModel.Codigo = value;
                    RaisePropertyChanged("Codigo");
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
        public UnidadeMedida()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected UnidadeMedida(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.UnidadeMedida> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public UnidadeMedida(Data.Model.UnidadeMedida dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Apaga os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorUnidadeMedida>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        /// <summary>
        /// Salva os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (this.Codigo == this.Descricao)
                return new Colosoft.Business.SaveResult(false, 
                    "O código e a descrição da unidade de medida não podem ser os mesmos.".GetFormatter());

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
               .Current.GetInstance<IValidadorUnidadeMedida>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        #endregion
    }
}
