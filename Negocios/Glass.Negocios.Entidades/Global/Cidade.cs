namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da cidade.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CidadeLoader))]
    public class Cidade : Colosoft.Business.Entity<Data.Model.Cidade>
    {
        #region Tipos Aninhados

        class CidadeLoader : Colosoft.Business.EntityLoader<Cidade, Data.Model.Cidade>
        {
            public CidadeLoader()
            {
                Configure()
                    .Uid(f => f.IdCidade)
                    .FindName(f => f.NomeCidade)
                    .Creator(f => new Cidade(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da cidade.
        /// </summary>
        public int IdCidade
        {
            get { return DataModel.IdCidade; }
            set
            {
                if (DataModel.IdCidade != value &&
                    RaisePropertyChanging("IdCidade", value))
                {
                    DataModel.IdCidade = value;
                    RaisePropertyChanged("IdCidade");
                }
            }
        }

        /// <summary>
        /// Nome da cidade.
        /// </summary>
        public string NomeCidade
        {
            get { return DataModel.NomeCidade; }
            set
            {
                if (DataModel.NomeCidade != value &&
                    RaisePropertyChanging("NomeCidade", value))
                {
                    DataModel.NomeCidade = value;
                    RaisePropertyChanged("NomeCidade");
                }
            }
        }

        /// <summary>
        /// Código IBGE da cdiade.
        /// </summary>
        public string CodIbgeCidade
        {
            get { return DataModel.CodIbgeCidade; }
            set
            {
                if (DataModel.CodIbgeCidade != value &&
                    RaisePropertyChanging("CodIbgeCidade", value))
                {
                    DataModel.CodIbgeCidade = value;
                    RaisePropertyChanged("CodIbgeCidade");
                }
            }
        }

        /// <summary>
        /// Nome UF.
        /// </summary>
        public string NomeUf
        {
            get { return DataModel.NomeUf; }
            set
            {
                if (DataModel.NomeUf != value &&
                    RaisePropertyChanging("NomeUf", value))
                {
                    DataModel.NomeUf = value;
                    RaisePropertyChanged("NomeUf");
                }
            }
        }

        /// <summary>
        /// Código IBGE do estado.
        /// </summary>
        public string CodIbgeUf
        {
            get { return DataModel.CodIbgeUf; }
            set
            {
                if (DataModel.CodIbgeUf != value &&
                    RaisePropertyChanging("CodIbgeUf", value))
                {
                    DataModel.CodIbgeUf = value;
                    RaisePropertyChanged("CodIbgeUf");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Cidade()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Cidade(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Cidade> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Cidade(Data.Model.Cidade dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
