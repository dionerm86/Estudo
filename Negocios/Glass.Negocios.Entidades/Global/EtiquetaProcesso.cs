using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da EtiquetaProcesso.
    /// </summary>
    public interface IValidadorEtiquetaProcesso
    {
        #region Métodos 

        /// <summary>
        /// Valida a existema da EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        Colosoft.IMessageFormattable[] ValidaExistencia(EtiquetaProcesso etiquetaProcesso);

        #endregion
    }

    /// <summary>
    /// Representa a entidade de negócio da EtiquetaProcesso.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(EtiquetaProcessoLoader))]
    public class EtiquetaProcesso : Colosoft.Business.Entity<Data.Model.EtiquetaProcesso>
    {
        #region Tipos Aninhados

        class EtiquetaProcessoLoader : Colosoft.Business.EntityLoader<EtiquetaProcesso, Data.Model.EtiquetaProcesso>
        {
            public EtiquetaProcessoLoader()
            {
                Configure()
                    .Uid(f => f.IdProcesso)
                    .FindName(f => f.CodInterno)
                    .Description(f => f.Descricao)
                    .Creator(f => new EtiquetaProcesso(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do processo.
        /// </summary>
        public int IdProcesso
        {
            get { return DataModel.IdProcesso; }
            set
            {
                if (DataModel.IdProcesso != value &&
                    RaisePropertyChanging("IdProcesso", value))
                {
                    DataModel.IdProcesso = value;
                    RaisePropertyChanged("IdProcesso");
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
        /// Identificador da etiqueta de aplicação associada.
        /// </summary>
        public int? IdAplicacao
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
        /// Identifica se é para destacar etiqueta.
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
        /// Define se a peça com este processo irá gerar uma forma inexistente na exportação para o Optyway 
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

        /// <summary>
        /// Define se a peça com esse processo irá gerar Arquivo de Mesa
        /// </summary>
        public bool GerarArquivoDeMesa
        {
            get { return DataModel.GerarArquivoDeMesa; }
            set
            {
                if(DataModel.GerarArquivoDeMesa != value && RaisePropertyChanging("GerarArquivoDeMesa", value))
                {
                    DataModel.GerarArquivoDeMesa = value;
                    RaisePropertyChanged("GerarArquivoDeMesa");
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
        /// Tipo processo.
        /// </summary>
        public Data.Model.EtiquetaTipoProcesso? TipoProcesso
        {
            get { return DataModel.TipoProcesso; }
            set
            {
                if (DataModel.TipoProcesso != value &&
                    RaisePropertyChanging("TipoProcesso", value))
                {
                    DataModel.TipoProcesso = value;
                    RaisePropertyChanged("TipoProcesso");
                }
            }
        }

        /// <summary>
        /// Tipos de Pedido.
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public EtiquetaProcesso()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected EtiquetaProcesso(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.EtiquetaProcesso> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public EtiquetaProcesso(Data.Model.EtiquetaProcesso dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado quando a instancia for apagada.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorEtiquetaProcesso>();

            var resultado = validador.ValidaExistencia(this);

            if (resultado.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultado.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
