namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da regra da natureza de operação.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(RegraNaturezaOperacaoLoader))]
    [Glass.Negocios.ControleAlteracao(
        Data.Model.LogAlteracao.TabelaAlteracao.RegraNaturezaOperacao,
        Data.Model.LogCancelamento.TabelaCancelamento.RegraNaturezaOperacao)]
    public class RegraNaturezaOperacao : Colosoft.Business.Entity<Data.Model.RegraNaturezaOperacao>
    {
        #region Tipos Aninhados

        class RegraNaturezaOperacaoLoader : Colosoft.Business.EntityLoader<RegraNaturezaOperacao, Data.Model.RegraNaturezaOperacao>
        {
            public RegraNaturezaOperacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdRegraNaturezaOperacao)
                    .Creator(f => new RegraNaturezaOperacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da regra.
        /// </summary>
        public int IdRegraNaturezaOperacao
        {
            get { return DataModel.IdRegraNaturezaOperacao; }
            set
            {
                if (DataModel.IdRegraNaturezaOperacao != value &&
                    RaisePropertyChanging("IdRegraNaturezaOperacao", value))
                {
                    DataModel.IdRegraNaturezaOperacao = value;
                    RaisePropertyChanged("IdRegraNaturezaOperacao");
                }
            }
        }

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        public int? IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de cliente.
        /// </summary>
        public int? IdTipoCliente
        {
            get { return DataModel.IdTipoCliente; }
            set
            {
                if (DataModel.IdTipoCliente != value &&
                    RaisePropertyChanging("IdTipoCliente", value))
                {
                    DataModel.IdTipoCliente = value;
                    RaisePropertyChanged("IdTipoCliente");
                }
            }
        }

        /// <summary>
        /// Identificador do grupo de produção.
        /// </summary>
        public int? IdGrupoProd
        {
            get { return DataModel.IdGrupoProd; }
            set
            {
                if (DataModel.IdGrupoProd != value &&
                    RaisePropertyChanging("IdGrupoProd", value))
                {
                    DataModel.IdGrupoProd = value;
                    RaisePropertyChanged("IdGrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador do subgrupo de produção.
        /// </summary>
        public int? IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro
        {
            get { return DataModel.IdCorVidro; }
            set
            {
                if (DataModel.IdCorVidro != value &&
                    RaisePropertyChanging("IdCorVidro", value))
                {
                    DataModel.IdCorVidro = value;
                    RaisePropertyChanged("IdCorVidro");
                }
            }
        }

        /// <summary>
        /// Identificador da cor do aluminio.
        /// </summary>
        public int? IdCorAluminio
        {
            get { return DataModel.IdCorAluminio; }
            set
            {
                if (DataModel.IdCorAluminio != value &&
                    RaisePropertyChanging("IdCorAluminio", value))
                {
                    DataModel.IdCorAluminio = value;
                    RaisePropertyChanged("IdCorAluminio");
                }
            }
        }

        /// <summary>
        /// Identificador da cor da ferragem.
        /// </summary>
        public int? IdCorFerragem
        {
            get { return DataModel.IdCorFerragem; }
            set
            {
                if (DataModel.IdCorFerragem != value &&
                    RaisePropertyChanging("IdCorFerragem", value))
                {
                    DataModel.IdCorFerragem = value;
                    RaisePropertyChanged("IdCorFerragem");
                }
            }
        }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float? Espessura
        {
            get { return DataModel.Espessura; }
            set
            {
                if (DataModel.Espessura != value &&
                    RaisePropertyChanging("Espessura", value))
                {
                    DataModel.Espessura = value;
                    RaisePropertyChanged("Espessura");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção - Intraestadual.
        /// </summary>
        public int IdNaturezaOperacaoProdIntra
        {
            get { return DataModel.IdNaturezaOperacaoProdIntra; }
            set
            {
                if (DataModel.IdNaturezaOperacaoProdIntra != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoProdIntra", value))
                {
                    DataModel.IdNaturezaOperacaoProdIntra = value;
                    RaisePropertyChanged("IdNaturezaOperacaoProdIntra");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Revenda - Intraestadual.
        /// </summary>
        public int IdNaturezaOperacaoRevIntra
        {
            get { return DataModel.IdNaturezaOperacaoRevIntra; }
            set
            {
                if (DataModel.IdNaturezaOperacaoRevIntra != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoRevIntra", value))
                {
                    DataModel.IdNaturezaOperacaoRevIntra = value;
                    RaisePropertyChanged("IdNaturezaOperacaoRevIntra");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção - Interestadual.
        /// </summary>
        public int IdNaturezaOperacaoProdInter
        {
            get { return DataModel.IdNaturezaOperacaoProdInter; }
            set
            {
                if (DataModel.IdNaturezaOperacaoProdInter != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoProdInter", value))
                {
                    DataModel.IdNaturezaOperacaoProdInter = value;
                    RaisePropertyChanged("IdNaturezaOperacaoProdInter");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Revenda - Interestadual.
        /// </summary>
        public int IdNaturezaOperacaoRevInter
        {
            get { return DataModel.IdNaturezaOperacaoRevInter; }
            set
            {
                if (DataModel.IdNaturezaOperacaoRevInter != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoRevInter", value))
                {
                    DataModel.IdNaturezaOperacaoRevInter = value;
                    RaisePropertyChanged("IdNaturezaOperacaoRevInter");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção ST - Intraestadual.
        /// </summary>
        public int? IdNaturezaOperacaoProdStIntra
        {
            get { return DataModel.IdNaturezaOperacaoProdStIntra; }
            set
            {
                if (DataModel.IdNaturezaOperacaoProdStIntra != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoProdStIntra", value))
                {
                    DataModel.IdNaturezaOperacaoProdStIntra = value;
                    RaisePropertyChanged("IdNaturezaOperacaoProdStIntra");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção ST - Intraestadual.
        /// </summary>
        public int? IdNaturezaOperacaoRevStIntra
        {
            get { return DataModel.IdNaturezaOperacaoRevStIntra; }
            set
            {
                if (DataModel.IdNaturezaOperacaoRevStIntra != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoRevStIntra", value))
                {
                    DataModel.IdNaturezaOperacaoRevStIntra = value;
                    RaisePropertyChanged("IdNaturezaOperacaoRevStIntra");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção ST - Interestadual.
        /// </summary>
        public int? IdNaturezaOperacaoProdStInter
        {
            get { return DataModel.IdNaturezaOperacaoProdStInter; }
            set
            {
                if (DataModel.IdNaturezaOperacaoProdStInter != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoProdStInter", value))
                {
                    DataModel.IdNaturezaOperacaoProdStInter = value;
                    RaisePropertyChanged("IdNaturezaOperacaoProdStInter");
                }
            }
        }

        /// <summary>
        /// Natureza Operação Produção ST - Intrerstadual.
        /// </summary>
        public int? IdNaturezaOperacaoRevStInter
        {
            get { return DataModel.IdNaturezaOperacaoRevStInter; }
            set
            {
                if (DataModel.IdNaturezaOperacaoRevStInter != value &&
                    RaisePropertyChanging("IdNaturezaOperacaoRevStInter", value))
                {
                    DataModel.IdNaturezaOperacaoRevStInter = value;
                    RaisePropertyChanged("IdNaturezaOperacaoRevStInter");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RegraNaturezaOperacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RegraNaturezaOperacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.RegraNaturezaOperacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RegraNaturezaOperacao(Data.Model.RegraNaturezaOperacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
