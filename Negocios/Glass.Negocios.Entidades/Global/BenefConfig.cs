using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da configuração do beneficiamento.
    /// </summary>
    public interface IValidadorBenefConfig
    {
        /// <summary>
        /// Recupera o número de sequencia para a configuração informada.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        int ObtemNumeroSequencia(BenefConfig benefConfig);

        /// <summary>
        /// Valida a atualização da configuração do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(BenefConfig benefConfig);

        /// <summary>
        /// Verifica se a configuração do beneficiamento está em uso
        /// em alguma parte do sistema.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        bool EmUso(BenefConfig benefConfig);
    }

    /// <summary>
    /// Representa a entidade de negócio da configuração do beneficiamento.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(BenefConfigLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.BenefConfig)]
    public class BenefConfig : Colosoft.Business.Entity<Glass.Data.Model.BenefConfig>
    {
        #region Tipos Aninhados

        class BenefConfigLoader : Colosoft.Business.EntityLoader<BenefConfig, Glass.Data.Model.BenefConfig>
        {
            public BenefConfigLoader()
            {
                Configure()
                    .Uid(f => f.IdBenefConfig)
                    .FindName(f => f.Nome)
                    .Description(f => f.Descricao)
                    .Child<BenefConfig, Glass.Data.Model.BenefConfig>("Filhos", f => f.Filhos, f => f.IdParent, Colosoft.Business.LoadOptions.Lazy)
                    .Child<BenefConfigPreco, Glass.Data.Model.BenefConfigPreco>("Precos", f => f.Precos, f => f.IdBenefConfig)                        
                    .Reference<EtiquetaProcesso, Glass.Data.Model.EtiquetaProcesso>("Processo", f => f.Processo, f => f.IdProcesso)
                    .Reference<EtiquetaAplicacao, Glass.Data.Model.EtiquetaAplicacao>("Aplicacao", f => f.Aplicacao, f => f.IdAplicacao)
                    .Reference<Produto, Glass.Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProd)
                    .Creator(f => new BenefConfig(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<BenefConfig> _filhos;
        private Colosoft.Business.IEntityChildrenList<BenefConfigPreco> _precos;        

        #endregion

        #region Propriedades

        /// <summary>
        /// Beneficiamentos filhos associados.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<BenefConfig> Filhos
        {
            get { return _filhos; }
        }

        /// <summary>
        /// Preços associados.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<BenefConfigPreco> Precos
        {
            get { return _precos; }
        }

        /// <summary>
        /// Dados da Etiqueta processo associada.
        /// </summary>
        public EtiquetaProcesso Processo
        {
            get { return GetReference<EtiquetaProcesso>("Processo", true); }
        }

        /// <summary>
        /// Dados da Etiqueta aplicação associada.
        /// </summary>
        public EtiquetaAplicacao Aplicacao
        {
            get { return GetReference<EtiquetaAplicacao>("Aplicacao", true); }
        }

        /// <summary>
        /// Dados do produto associado.
        /// </summary>
        public Produto Produto
        {
            get { return GetReference<Produto>("Produto", true); }
        }

        /// <summary>
        /// Identificador da configuração do beneficiamento.
        /// </summary>
        public int IdBenefConfig
        {
            get { return DataModel.IdBenefConfig; }
            set
            {
                if (DataModel.IdBenefConfig != value &&
                    RaisePropertyChanging("IdBenefConfig", value))
                {
                    DataModel.IdBenefConfig = value;
                    RaisePropertyChanged("IdBenefConfig");
                }
            }
        }

        /// <summary>
        /// Identificador do pai associado.
        /// </summary>
        public int? IdParent
        {
            get { return DataModel.IdParent; }
            set
            {
                if (DataModel.IdParent != value &&
                    RaisePropertyChanging("IdParent", value))
                {
                    DataModel.IdParent = value;
                    RaisePropertyChanged("IdParent");
                }
            }
        }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
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
        /// Tipo de controle.
        /// </summary>
        public Glass.Data.Model.TipoControleBenef TipoControle
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
        /// Tipo de calculo.
        /// </summary>
        public Glass.Data.Model.TipoCalculoBenef TipoCalculo
        {
            get { return DataModel.TipoCalculo; }
            set
            {
                if (DataModel.TipoCalculo != value &&
                    RaisePropertyChanging("TipoCalculo", value))
                {
                    DataModel.TipoCalculo = value;
                    RaisePropertyChanged("TipoCalculo");
                }
            }
        }

        /// <summary>
        /// Tipo de espessura.
        /// </summary>
        public Glass.Data.Model.TipoEspessuraBenef TipoEspessura
        {
            get { return DataModel.TipoEspessura; }
            set
            {
                if (DataModel.TipoEspessura != value &&
                    RaisePropertyChanging("TipoEspessura", value))
                {
                    DataModel.TipoEspessura = value;
                    RaisePropertyChanged("TipoEspessura");
                }
            }
        }

        /// <summary>
        /// Cobrança opcional.
        /// </summary>
        public bool CobrancaOpcional
        {
            get { return DataModel.CobrancaOpcional; }
            set
            {
                if (DataModel.CobrancaOpcional != value &&
                    RaisePropertyChanging("CobrancaOpcional", value))
                {
                    DataModel.CobrancaOpcional = value;
                    RaisePropertyChanged("CobrancaOpcional");
                }
            }
        }

        /// <summary>
        /// Número de Sequencia.
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
        /// Cobrar área mínima.
        /// </summary>
        public bool CobrarAreaMinima
        {
            get { return DataModel.CobrarAreaMinima; }
            set
            {
                if (DataModel.CobrarAreaMinima != value &&
                    RaisePropertyChanging("CobrarAreaMinima", value))
                {
                    DataModel.CobrarAreaMinima = value;
                    RaisePropertyChanged("CobrarAreaMinima");
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
        /// Identificador da etiqueta de processo associada.
        /// </summary>
        public int? IdProcesso
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
        /// Identificador do produto associado.
        /// </summary>
        public int? IdProd
        {
            get { return DataModel.IdProd; }
            set
            {
                if (DataModel.IdProd != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProd = value;
                    RaisePropertyChanged("IdProd");
                }
            }
        }

        /// <summary>
        /// Acréscimo altura.
        /// </summary>
        public int AcrescimoAltura
        {
            get { return DataModel.AcrescimoAltura; }
            set
            {
                if (DataModel.AcrescimoAltura != value &&
                    RaisePropertyChanging("AcrescimoAltura", value))
                {
                    DataModel.AcrescimoAltura = value;
                    RaisePropertyChanged("AcrescimoAltura");
                }
            }
        }

        /// <summary>
        /// Acréscimo de largura.
        /// </summary>
        public int AcrescimoLargura
        {
            get { return DataModel.AcrescimoLargura; }
            set
            {
                if (DataModel.AcrescimoLargura != value &&
                    RaisePropertyChanging("AcrescimoLargura", value))
                {
                    DataModel.AcrescimoLargura = value;
                    RaisePropertyChanged("AcrescimoLargura");
                }
            }
        }

        /// <summary>
        /// Identifica se não é para exibir descrição do beneficiamento na etiqueta.
        /// </summary>
        public bool NaoExibirEtiqueta
        {
            get { return DataModel.NaoExibirEtiqueta; }
            set
            {
                if (DataModel.NaoExibirEtiqueta != value &&
                    RaisePropertyChanging("NaoExibirEtiqueta", value))
                {
                    DataModel.NaoExibirEtiqueta = value;
                    RaisePropertyChanged("NaoExibirEtiqueta");
                }
            }
        }

        /// <summary>
        /// Tipo do beneficiamento.
        /// </summary>
        public Glass.Data.Model.TipoBenef TipoBenef
        {
            get { return DataModel.TipoBenef; }
            set
            {
                if (DataModel.TipoBenef != value &&
                    RaisePropertyChanging("TipoBenef", value))
                {
                    DataModel.TipoBenef = value;
                    RaisePropertyChanged("TipoBenef");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public BenefConfig()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected BenefConfig(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.BenefConfig> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _filhos = GetChild<BenefConfig>(args.Children, "Filhos");
            _precos = GetChild<BenefConfigPreco>(args.Children, "Precos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public BenefConfig(Glass.Data.Model.BenefConfig dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _filhos = CreateChild<Colosoft.Business.IEntityChildrenList<BenefConfig>>("Filhos");
            _precos = CreateChild<Colosoft.Business.IEntityChildrenList<BenefConfigPreco>>("Precos");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            // Não permite inserir/atualizar beneficiamento que seja "Seleção simples" e que o cálculo seja "Qtd"
            if (TipoControle == Glass.Data.Model.TipoControleBenef.SelecaoSimples &&
                TipoCalculo == Glass.Data.Model.TipoCalculoBenef.Quantidade)
                return new Colosoft.Business.SaveResult(false, 
                    "Não é possível cadastrar beneficiamento que seja do tipo seleção simples e calculado por quantidade.".GetFormatter());

            // Não permite inserir/atualizar beneficiamento que seja "Quantidade" ou "Lista Seleção c/ Qtd." e que o cálculo não seja Qtd.
            if (Configuracoes.Beneficiamentos.ControleBeneficiamento.BloquearControleQuantidadeCalculoQuantidade && 
                (TipoControle == Data.Model.TipoControleBenef.Quantidade || TipoControle == Data.Model.TipoControleBenef.ListaSelecaoQtd) &&
                TipoCalculo != Data.Model.TipoCalculoBenef.Quantidade)
                return new Colosoft.Business.SaveResult(false,
                    "Beneficiamentos que sejam do tipo Quantidade ou Lista Seleção c Qtd. devem ser calculados por quantidade.".GetFormatter());

            /* Chamado 45472. */
            if (Nome.Contains("$") || Descricao.Contains("$"))
                return new Colosoft.Business.SaveResult(false,
                    "O Nome/Descrição do beneficiamento não pode conter o caractere Cifrão.".GetFormatter());

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorBenefConfig>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            // Calcula a sequência somente quando o beneficiamento não existir no banco.
            if (!this.ExistsInStorage)
                // Recupera o número da sequencia
                this.NumSeq = validador.ObtemNumeroSequencia(this);

            return base.Save(session);
        }

        /// <summary>
        /// Apaga a configuração do beneficiamento.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorBenefConfig>();

            // Verifica se este beneficiamento ou seus filhos estão sendo usados em alguma tabela, 
            // se estiverem, não permite que este beneficiamento seja excluído
            if (validador.EmUso(this))
            {
                // Altera a situação para inativo
                this.Situacao = Glass.Situacao.Inativo;
                var resultado = Save(session);
                if (!resultado)
                    return new Colosoft.Business.DeleteResult(false, resultado.Message);

                return new Colosoft.Business.DeleteResult(true, null);
            }
            
            return base.Delete(session);
        }

        #endregion
    }
}
