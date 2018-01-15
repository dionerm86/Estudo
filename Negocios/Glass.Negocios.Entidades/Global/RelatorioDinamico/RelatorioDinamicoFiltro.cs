
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RelatorioDinamicoFiltroLoader))]
    public class RelatorioDinamicoFiltro : Colosoft.Business.Entity<Glass.Data.Model.RelatorioDinamicoFiltro>
    {
        #region Tipos Aninhados

        class RelatorioDinamicoFiltroLoader : Colosoft.Business.EntityLoader<RelatorioDinamicoFiltro, Data.Model.RelatorioDinamicoFiltro>
        {
            public RelatorioDinamicoFiltroLoader()
            {
                Configure()
                    .Uid(f => f.IdRelatorioDinamicoFiltro)
                    .Creator(f => new RelatorioDinamicoFiltro(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RelatorioDinamicoFiltro()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RelatorioDinamicoFiltro(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RelatorioDinamicoFiltro> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RelatorioDinamicoFiltro(Glass.Data.Model.RelatorioDinamicoFiltro dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdRelatorioDinamicoFiltro.
        /// </summary>
        public int IdRelatorioDinamicoFiltro
        {
            get { return DataModel.IdRelatorioDinamicoFiltro; }
            set
            {
                if (DataModel.IdRelatorioDinamicoFiltro != value &&
                    RaisePropertyChanging("IdRelatorioDinamicoFiltro", value))
                {
                    DataModel.IdRelatorioDinamicoFiltro = value;
                    RaisePropertyChanged("IdRelatorioDinamicoFiltro");
                }
            }
        }

        /// <summary>
        /// IdRelatorioDinamico.
        /// </summary>
        public int IdRelatorioDinamico
        {
            get { return DataModel.IdRelatorioDinamico; }
            set
            {
                if (DataModel.IdRelatorioDinamico != value &&
                    RaisePropertyChanging("IdRelatorioDinamico", value))
                {
                    DataModel.IdRelatorioDinamico = value;
                    RaisePropertyChanged("IdRelatorioDinamico");
                }
            }
        }

        /// <summary>
        /// NomeFiltro.
        /// </summary>
        public string NomeFiltro
        {
            get { return DataModel.NomeFiltro; }
            set
            {
                if (DataModel.NomeFiltro != value &&
                    RaisePropertyChanging("NomeFiltro", value))
                {
                    DataModel.NomeFiltro = value;
                    RaisePropertyChanged("NomeFiltro");
                }
            }
        }

        /// <summary>
        /// NomeColunaSql.
        /// </summary>
        public string NomeColunaSql
        {
            get { return DataModel.NomeColunaSql; }
            set
            {
                if (DataModel.NomeColunaSql != value &&
                    RaisePropertyChanging("NomeColunaSql", value))
                {
                    DataModel.NomeColunaSql = value;
                    RaisePropertyChanged("NomeColunaSql");
                }
            }
        }

        /// <summary>
        /// TipoControle.
        /// </summary>
        public Data.Model.TipoControle TipoControle
        {
            get { return DataModel.TipoControle; }
            set
            {
                if (DataModel.TipoControle != value &&
                    RaisePropertyChanging("TipoControle", value))
                {
                    DataModel.TipoControle = value;
                    RaisePropertyChanged("TipoControle");
                }
            }
        }

        /// <summary>
        /// Opcoes.
        /// </summary>
        public string Opcoes
        {
            get { return DataModel.Opcoes; }
            set
            {
                if (DataModel.Opcoes != value &&
                    RaisePropertyChanging("Opcoes", value))
                {
                    DataModel.Opcoes = value;
                    RaisePropertyChanged("Opcoes");
                }
            }
        }

        /// <summary>
        /// Valor padrão.
        /// </summary>
        public string ValorPadrao
        {
            get { return DataModel.ValorPadrao; }
            set
            {
                if (DataModel.ValorPadrao != value &&
                    RaisePropertyChanging("ValorPadrao", value))
                {
                    DataModel.ValorPadrao = value;
                    RaisePropertyChanged("ValorPadrao");
                }
            }
        }

        /// <summary>
        /// Núm. de Sequência.
        /// </summary>
        public int NumSeq
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

        #endregion
    }
}
