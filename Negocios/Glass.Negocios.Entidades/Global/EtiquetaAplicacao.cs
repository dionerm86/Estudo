using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura da classe que valida das EtiquetaAplicacao.
    /// </summary>
    public interface IValidadorEtiquetaAplicacao
    {
        /// <summary>
        /// Valida a existena dos dados.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        Colosoft.IMessageFormattable[] ValidaExistencia(EtiquetaAplicacao etiquetaAplicacao);
    }

    /// <summary>
    /// Representa a entidade de negócio da etiqueta de aplicação.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(EtiquetaAplicacaoLoader))]
    public class EtiquetaAplicacao : Colosoft.Business.Entity<Data.Model.EtiquetaAplicacao>
    {
        #region Tipos Aninhados

        class EtiquetaAplicacaoLoader : Colosoft.Business.EntityLoader<EtiquetaAplicacao, Data.Model.EtiquetaAplicacao>
        {
            public EtiquetaAplicacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdAplicacao)
                    .FindName(f => f.CodInterno)
                    .Description(f => f.Descricao)
                    .Creator(f => new EtiquetaAplicacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da etiqueta de aplicação.
        /// </summary>
        public int IdAplicacao
        {
            get { return DataModel.IdAplicacao; }
            set
            {
                if (DataModel.IdAplicacao != value &&
                    RaisePropertyChanging("IdAplicacao", value))
                {
                    DataModel.IdAplicacao = value;
                    RaisePropertyChanged("IdAplicacao");
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
        /// Identifica se é para destacar a etiqueta.
        /// </summary>
        public bool DestacarEtiqueta
        {
            get { return DataModel.DestacarEtiqueta; }
            set
            {
                if (DataModel.DestacarEtiqueta != value &&
                    RaisePropertyChanging("DestacarEtiqueta", value))
                {
                    DataModel.DestacarEtiqueta = value;
                    RaisePropertyChanged("DestacarEtiqueta");
                }
            }
        }

        /// <summary>
        /// Define se a peça com esta aplicação irá gerar uma forma inexistente na exportação para o Optyway 
        /// para o conferente saber que precisa criar uma forma para a mesma, desde que a mesma não possua forma.
        /// </summary>
        public bool GerarFormaInexistente
        {
            get { return DataModel.GerarFormaInexistente; }
            set
            {
                if (DataModel.GerarFormaInexistente != value &&
                    RaisePropertyChanging("GerarFormaInexistente", value))
                {
                    DataModel.GerarFormaInexistente = value;
                    RaisePropertyChanged("GerarFormaInexistente");
                }
            }
        }

        public bool NaoPermitirFastDelivery
        {
            get { return DataModel.NaoPermitirFastDelivery; }
            set
            {
                if (DataModel.NaoPermitirFastDelivery != value &&
                    RaisePropertyChanging("NaoPermitirFastDelivery", value))
                {
                    DataModel.NaoPermitirFastDelivery = value;
                    RaisePropertyChanged("NaoPermitirFastDelivery");
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
        /// Numero de dias minimos para entrega do pedido com produtos dessa aplicação.
        /// </summary>
        public int DiasMinimos
        {
            get { return DataModel.DiasMinimos; }
            set
            {
                if (DataModel.DiasMinimos != value &&
                    RaisePropertyChanging("DiasMinimos", value))
                {
                    DataModel.DiasMinimos = value;
                    RaisePropertyChanged("DiasMinimos");
                }
            }
        }

        /// <summary>
        /// Tipos de pedidos.
        /// </summary>
        public string TipoPedido
        {
            get { return DataModel.TipoPedido; }
            set
            {
                if (DataModel.TipoPedido != value &&
                    RaisePropertyChanging("TipoPedido", value))
                {
                    DataModel.TipoPedido = value;
                    RaisePropertyChanged("TipoPedido");
                }
            }
        }

        /// <summary>
        /// Descrição dos tipos de pedido
        /// </summary>
        public string DescricaoTipoPedido { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public EtiquetaAplicacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected EtiquetaAplicacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.EtiquetaAplicacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public EtiquetaAplicacao(Data.Model.EtiquetaAplicacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado para apagar os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorEtiquetaAplicacao>();

            var resultado = validador.ValidaExistencia(this);

            if (resultado.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultado.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
