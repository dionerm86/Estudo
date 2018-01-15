
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RelatorioDinamicoColunaLoader))]
    public class RelatorioDinamicoColuna : Colosoft.Business.Entity<Glass.Data.Model.RelatorioDinamicoColuna>
    {
        #region Tipo Aninhados

        class RelatorioDinamicoColunaLoader : Colosoft.Business.EntityLoader<RelatorioDinamicoColuna, Data.Model.RelatorioDinamicoColuna>
        {
            public RelatorioDinamicoColunaLoader()
            {
                Configure()
                    .Uid(f => f.IdRelatorioDinamicoColuna)
                    .Creator(f => new RelatorioDinamicoColuna(f));
            }
        }

        #endregion

        #region Contrutores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public RelatorioDinamicoColuna()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RelatorioDinamicoColuna(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RelatorioDinamicoColuna> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RelatorioDinamicoColuna(Glass.Data.Model.RelatorioDinamicoColuna dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da Coluna.
        /// </summary>
        public int IdRelatorioDinamicoColuna
        {
            get { return DataModel.IdRelatorioDinamicoColuna; }
            set
            {
                if (DataModel.IdRelatorioDinamicoColuna != value &&
                    RaisePropertyChanging("IdRelatorioDinamicoColuna", value))
                {
                    DataModel.IdRelatorioDinamicoColuna = value;
                    RaisePropertyChanged("IdRelatorioDinamicoColuna");
                }
            }
        }

        /// <summary>
        /// Identificador do relatorio.
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
        /// Nome da coluna.
        /// </summary>
        public string NomeColuna
        {
            get { return DataModel.NomeColuna; }
            set
            {
                if (DataModel.NomeColuna != value &&
                    RaisePropertyChanging("NomeColuna", value))
                {
                    DataModel.NomeColuna = value;
                    RaisePropertyChanged("NomeColuna");
                }
            }
        }

        /// <summary>
        /// Alias.
        /// </summary>
        public string Alias
        {
            get { return DataModel.Alias; }
            set
            {
                if (DataModel.Alias != value &&
                    RaisePropertyChanging("Alias", value))
                {
                    DataModel.Alias = value;
                    RaisePropertyChanged("Alias");
                }
            }
        }

        /// <summary>
        /// Valor do preenchimento da coluna.
        /// </summary>
        public string Valor
        {
            get { return DataModel.Valor; }
            set
            {
                if (DataModel.Valor != value &&
                    RaisePropertyChanging("Valor", value))
                {
                    DataModel.Valor = value;
                    RaisePropertyChanged("Valor");
                }
            }
        }

        /// <summary>
        /// Método de visibilidade.
        /// </summary>
        public string MetodoVisibilidade
        {
            get { return DataModel.MetodoVisibilidade; }
            set
            {
                if (DataModel.MetodoVisibilidade != value &&
                    RaisePropertyChanging("MetodoVisibilidade", value))
                {
                    DataModel.MetodoVisibilidade = value;
                    RaisePropertyChanged("MetodoVisibilidade");
                }
            }
        }

        /// <summary>
        /// Numero de sequencia.
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
