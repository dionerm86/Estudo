
namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(RelatorioDinamicoIconeLoader))]
    public class RelatorioDinamicoIcone : Colosoft.Business.Entity<Glass.Data.Model.RelatorioDinamicoIcone>
    {
        #region Tipos Aninhados

        class RelatorioDinamicoIconeLoader : Colosoft.Business.EntityLoader<RelatorioDinamicoIcone, Data.Model.RelatorioDinamicoIcone>
        {
            public RelatorioDinamicoIconeLoader()
            {
                Configure()
                    .Uid(f => f.IdRelatorioDinamicoIcone)
                    .Creator(f => new RelatorioDinamicoIcone(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RelatorioDinamicoIcone()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RelatorioDinamicoIcone(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.RelatorioDinamicoIcone> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RelatorioDinamicoIcone(Glass.Data.Model.RelatorioDinamicoIcone dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// IdRelatorioDinamicoIcone.
        /// </summary>
        public int IdRelatorioDinamicoIcone
        {
            get { return DataModel.IdRelatorioDinamicoIcone; }
            set
            {
                if (DataModel.IdRelatorioDinamicoIcone != value &&
                    RaisePropertyChanging("IdRelatorioDinamicoIcone", value))
                {
                    DataModel.IdRelatorioDinamicoIcone = value;
                    RaisePropertyChanged("IdRelatorioDinamicoIcone");
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
        /// NomeIcone.
        /// </summary>
        public string NomeIcone
        {
            get { return DataModel.NomeIcone; }
            set
            {
                if (DataModel.NomeIcone != value &&
                    RaisePropertyChanging("NomeIcone", value))
                {
                    DataModel.NomeIcone = value;
                    RaisePropertyChanged("NomeIcone");
                }
            }
        }

        /// <summary>
        /// FuncaoJavaScript.
        /// </summary>
        public string FuncaoJavaScript
        {
            get { return DataModel.FuncaoJavaScript; }
            set
            {
                if (DataModel.FuncaoJavaScript != value &&
                    RaisePropertyChanging("FuncaoJavaScript", value))
                {
                    DataModel.FuncaoJavaScript = value;
                    RaisePropertyChanged("FuncaoJavaScript");
                }
            }
        }

        /// <summary>
        /// Icone.
        /// </summary>
        public byte[] Icone
        {
            get { return DataModel.Icone; }
            set
            {
                if (DataModel.Icone != value &&
                    RaisePropertyChanging("Icone", value))
                {
                    DataModel.Icone = value;
                    RaisePropertyChanged("Icone");
                }
            }
        }

        /// <summary>
        /// NumSeq.
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

        /// <summary>
        /// Método que ira controlar a visibilidade do icone.
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
        /// Indica se o icone deve ser mostrado no final da grid.
        /// </summary>
        public bool MostrarFinalGrid
        {
            get { return DataModel.MostrarFinalGrid; }
            set
            {
                if (DataModel.MostrarFinalGrid != value &&
                    RaisePropertyChanging("MostrarFinalGrid", value))
                {
                    DataModel.MostrarFinalGrid = value;
                    RaisePropertyChanged("MostrarFinalGrid");
                }
            }
        }

        /// <summary>
        /// Renderização da imagem do ícone
        /// </summary>
        public string ImagemIcone
        {
            get
            {
                if (Icone == null || Icone.Length == 0)
                    return string.Empty;

                return string.Format("data:image/png;base64,{0}", System.Convert.ToBase64String(Icone, 0, Icone.Length));
            }
        }

        #endregion
    }
}
