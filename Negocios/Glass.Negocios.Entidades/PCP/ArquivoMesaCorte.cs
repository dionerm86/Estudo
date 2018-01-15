namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do arquivo de mesa de corte.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ArquivoMesaCorteLoader))]
    public class ArquivoMesaCorte : Colosoft.Business.Entity<Glass.Data.Model.ArquivoMesaCorte>
    {
        #region Tipos Aninhados

        class ArquivoMesaCorteLoader : Colosoft.Business.EntityLoader<ArquivoMesaCorte, Glass.Data.Model.ArquivoMesaCorte>
        {
            public ArquivoMesaCorteLoader()
            {
                Configure()
                    .Uid(f => f.IdArquivoMesaCorte)
                    .Creator(f => new ArquivoMesaCorte(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do arquivo.
        /// </summary>
        public int IdArquivoMesaCorte
        {
            get { return DataModel.IdArquivoMesaCorte; }
            set
            {
                if (DataModel.IdArquivoMesaCorte != value &&
                    RaisePropertyChanging("IdArquivoMesaCorte", value))
                {
                    DataModel.IdArquivoMesaCorte = value;
                    RaisePropertyChanged("IdArquivoMesaCorte");
                }
            }
        }

        /// <summary>
        /// Arquivo.
        /// </summary>
        public string Arquivo
        {
            get { return DataModel.Arquivo; }
            set
            {
                if (DataModel.Arquivo != value &&
                    RaisePropertyChanging("Arquivo", value))
                {
                    DataModel.Arquivo = value;
                    RaisePropertyChanged("Arquivo");
                }
            }
        }

        /// <summary>
        /// Tipo de arquivp.
        /// </summary>
        public Glass.Data.Model.TipoArquivoMesaCorte TipoArquivo
        {
            get { return DataModel.TipoArquivo; }
            set
            {
                if (DataModel.TipoArquivo != value &&
                    RaisePropertyChanging("TipoArquivo", value))
                {
                    DataModel.TipoArquivo = value;
                    RaisePropertyChanged("TipoArquivo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ArquivoMesaCorte()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ArquivoMesaCorte(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ArquivoMesaCorte> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ArquivoMesaCorte(Glass.Data.Model.ArquivoMesaCorte dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
