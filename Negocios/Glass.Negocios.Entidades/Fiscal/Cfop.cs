using System.Collections.Generic;

namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do Código Fiscal de Operações e Prestações.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CfopLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Cfop)]
    public class Cfop : Colosoft.Business.Entity<Data.Model.Cfop>, Sync.Fiscal.EFD.Entidade.ICfop
    {
        #region Tipos Aninhados

        class CfopLoader : Colosoft.Business.EntityLoader<Cfop, Data.Model.Cfop>
        {
            public CfopLoader()
            {
                Configure()
                    .Uid(f => f.IdCfop)
                    .FindName(f => f.Descricao)
                    .Child<NaturezaOperacao, Data.Model.NaturezaOperacao>(
                        "NaturezasOperacao", f => f.NaturezasOperacao, f => f.IdCfop)
                    .Creator(f => new Cfop(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<NaturezaOperacao> _naturezasOperacao;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do Cfop.
        /// </summary>
        public int IdCfop
        {
            get { return DataModel.IdCfop; }
            set
            {
                if (DataModel.IdCfop != value &&
                    RaisePropertyChanging("IdCfop", value))
                {
                    DataModel.IdCfop = value;
                    RaisePropertyChanged("IdCfop");
                }
            }
        }

        /// <summary>
        /// Identificador do tipo de Cfop.
        /// </summary>
        public int? IdTipoCfop
        {
            get { return DataModel.IdTipoCfop; }
            set
            {
                if (DataModel.IdTipoCfop != value &&
                    RaisePropertyChanging("IdTipoCfop", value))
                {
                    DataModel.IdTipoCfop = value;
                    RaisePropertyChanged("IdTipoCfop");
                }
            }
        }

        /// <summary>
        /// Tipo de mercadoria.
        /// </summary>
        public Data.Model.TipoMercadoria? TipoMercadoria
        {
            get { return DataModel.TipoMercadoria; }
            set
            {
                if (DataModel.TipoMercadoria != value &&
                    RaisePropertyChanging("TipoMercadoria", value))
                {
                    DataModel.TipoMercadoria = value;
                    RaisePropertyChanged("TipoMercadoria");
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
        /// Alterar estoque terceiros.
        /// </summary>
        public bool AlterarEstoqueTerceiros
        {
            get { return DataModel.AlterarEstoqueTerceiros; }
            set
            {
                if (DataModel.AlterarEstoqueTerceiros != value &&
                    RaisePropertyChanging("AlterarEstoqueTerceiros", value))
                {
                    DataModel.AlterarEstoqueTerceiros = value;
                    RaisePropertyChanged("AlterarEstoqueTerceiros");
                }
            }
        }

        /// <summary>
        /// Alterar estoque de vidros de cliente.
        /// </summary>
        public bool AlterarEstoqueCliente
        {
            get { return DataModel.AlterarEstoqueCliente; }
            set
            {
                if (DataModel.AlterarEstoqueCliente != value &&
                    RaisePropertyChanging("AlterarEstoqueCliente", value))
                {
                    DataModel.AlterarEstoqueCliente = value;
                    RaisePropertyChanged("AlterarEstoqueCliente");
                }
            }
        }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs
        {
            get { return DataModel.Obs; }
            set
            {
                if (DataModel.Obs != value &&
                    RaisePropertyChanging("Obs", value))
                {
                    DataModel.Obs = value;
                    RaisePropertyChanged("Obs");
                }
            }
        }

        /// <summary>
        /// Naturezas de operação associadas.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<NaturezaOperacao> NaturezasOperacao
        {
            get { return _naturezasOperacao; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Cfop()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Cfop(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Cfop> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _naturezasOperacao = GetChild<NaturezaOperacao>(args.Children, "NaturezasOperacao");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Cfop(Data.Model.Cfop dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _naturezasOperacao = CreateChild<Colosoft.Business.IEntityChildrenList<NaturezaOperacao>>("NaturezasOperacao");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public static bool VerificaCfopDevolucao(string codInterno)
        {
            List<string> cfopDevolucao = new List<string>() { "1201", "1202", "1203",
                "1204", "1208", "1209", "1410", "1411", "1503", "1504", "1505", "1506",
                "1553", "1660", "1661", "1662", "1918", "1919", "2201", "2202", "2203", "2204", "2208",
                "2209", "2410", "2411", "2503", "2504", "2505", "2506", "2553", "2660", "2661", "2662", "2918", "2919",
                "3201", "3202", "3211", "3503", "3553", "5201", "5202", "5208", "5209", "5210", "5410", "5411", "5412", "5413",
                "5503", "5553", "5555", "5556", "5660", "5661", "5662", "5918", "5919", "5921", "6201", "6202","6208", "6209", "6210",
                "6410", "6411", "6412", "6413", "6503", "6553", "6555", "6556", "6660", "6661", "6662", "6918", "6919", "6921", "7201",
                "7202", "7210", "7211", "7553", "7556",
                "7930" };

            return cfopDevolucao.Contains(codInterno);
        }

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        /// <returns></returns>
        public bool VerificaCfopDevolucao()
        {
            return VerificaCfopDevolucao(this.CodInterno);
        }

        #endregion

        #region ICfop Members

        int Sync.Fiscal.EFD.Entidade.ICfop.CodigoCfop
        {
            get { return IdCfop; }
        }

        string Sync.Fiscal.EFD.Entidade.ICfop.CodigoInterno
        {
            get { return CodInterno; }
        }

        bool Sync.Fiscal.EFD.Entidade.ICfop.Devolucao
        {
            get { return VerificaCfopDevolucao(CodInterno); }
        }

        bool Sync.Fiscal.EFD.Entidade.ICfop.CalculaCofins(int codigoLoja)
        {
            return false;
        }

        bool Sync.Fiscal.EFD.Entidade.ICfop.CalculaPis(int codigoLoja)
        {
            return false;
        }

        #endregion
    }
}
