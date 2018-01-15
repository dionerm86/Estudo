
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RelatorioDinamicoLoader))]
    public class RelatorioDinamico : Colosoft.Business.Entity<Glass.Data.Model.RelatorioDinamico>
    {
        #region Tipos Aninhados

        class RelatorioDinamicoLoader : Colosoft.Business.EntityLoader<RelatorioDinamico, Data.Model.RelatorioDinamico>
        {
            public RelatorioDinamicoLoader()
            {
                Configure()
                    .Uid(f => f.IdRelatorioDinamico)
                    .Child<RelatorioDinamicoFiltro, Data.Model.RelatorioDinamicoFiltro>("Filtros", f => f.Filtros, f => f.IdRelatorioDinamico)
                    .Child<RelatorioDinamicoIcone, Data.Model.RelatorioDinamicoIcone>("Icones", f => f.Icones, f => f.IdRelatorioDinamico)
                    .Creator(f => new RelatorioDinamico(f));
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<RelatorioDinamicoFiltro> _filtros;
        private Colosoft.Business.IEntityChildrenList<RelatorioDinamicoIcone> _icones;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RelatorioDinamico()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RelatorioDinamico(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RelatorioDinamico> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _filtros = GetChild<RelatorioDinamicoFiltro>(args.Children, "Filtros");
            _icones = GetChild<RelatorioDinamicoIcone>(args.Children, "Icones");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RelatorioDinamico(Glass.Data.Model.RelatorioDinamico dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _filtros = CreateChild<Colosoft.Business.IEntityChildrenList<RelatorioDinamicoFiltro>>("Filtros");
            _icones = CreateChild<Colosoft.Business.IEntityChildrenList<RelatorioDinamicoIcone>>("Icones");
        }

        #endregion

        #region Propriedades

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
        /// NomeRelatorio.
        /// </summary>
        public string NomeRelatorio
        {
            get { return DataModel.NomeRelatorio; }
            set
            {
                if (DataModel.NomeRelatorio != value &&
                    RaisePropertyChanging("NomeRelatorio", value))
                {
                    DataModel.NomeRelatorio = value;
                    RaisePropertyChanged("NomeRelatorio");
                }
            }
        }

        /// <summary>
        /// ComandoSql.
        /// </summary>
        public string ComandoSql
        {
            get { return DataModel.ComandoSql; }
            set
            {
                if (DataModel.ComandoSql != value &&
                    RaisePropertyChanging("ComandoSql", value))
                {
                    DataModel.ComandoSql = value;
                    RaisePropertyChanged("ComandoSql");
                }
            }
        }

        /// <summary>
        /// Situacao.
        /// </summary>
        public Situacao Situacao
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
        /// Nome utilizado no link de inserção de um novo registro.
        /// </summary>
        public string LinkInsercaoNome
        {
            get { return DataModel.LinkInsercaoNome; }
            set
            {
                if (DataModel.LinkInsercaoNome != value &&
                    RaisePropertyChanging("LinkInsercaoNome", value))
                {
                    DataModel.LinkInsercaoNome = value;
                    RaisePropertyChanged("LinkInsercaoNome");
                }
            }
        }

        /// <summary>
        /// Url de inserção de um novo registro.
        /// </summary>
        public string LinkInsersaoUrl
        {
            get { return DataModel.LinkInsersaoUrl; }
            set
            {
                if (DataModel.LinkInsersaoUrl != value &&
                    RaisePropertyChanging("LinkInsersaoUrl", value))
                {
                    DataModel.LinkInsersaoUrl = value;
                    RaisePropertyChanged("LinkInsersaoUrl");
                }
            }
        }

        /// <summary>
        /// Quantidade de registros da grid por pagina.
        /// </summary>
        public int QuantidadeRegistrosPorPagina
        {
            get { return DataModel.QuantidadeRegistrosPorPagina; }
            set
            {
                if (DataModel.QuantidadeRegistrosPorPagina != value &&
                    RaisePropertyChanging("QuantidadeRegistrosPorPagina", value))
                {
                    DataModel.QuantidadeRegistrosPorPagina = value;
                    RaisePropertyChanged("QuantidadeRegistrosPorPagina");
                }
            }
        }

        /// <summary>
        /// Filtros do relatório
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<RelatorioDinamicoFiltro> Filtros
        {
            get { return _filtros; }
        }

        /// <summary>
        /// Filtros do relatório
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<RelatorioDinamicoIcone> Icones
        {
            get { return _icones; }
        }

        #endregion
    }
}
