using Colosoft;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos dados do setor.
    /// </summary>
    public interface IProvedorSetor
    {
        /// <summary>
        /// Valida a atualização dos dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        IMessageFormattable[] ValidaAtualizacao(Setor setor);
    }

    /// <summary>
    /// Representa a entidade de negócios dos setores do sistema.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SetorLoader))]
    [Glass.Negocios.ControleAlteracao(Glass.Data.Model.LogAlteracao.TabelaAlteracao.Setor)]
    public class Setor : Colosoft.Business.Entity<Glass.Data.Model.Setor>
    {
        #region Tipos Aninhados

        class SetorLoader : Colosoft.Business.EntityLoader<Setor, Glass.Data.Model.Setor>
        {
            public SetorLoader()
            {
                Configure()
                    .Uid(f => f.IdSetor)
                    .FindName(f => f.Descricao)
                    .Descriptor(f => new SetorDescritor(f))
                    .Child<SetorBenef, Glass.Data.Model.SetorBenef>("SetorBeneficiamentos", f => f.SetorBeneficiamentos, f => f.IdSetor)
                    .Creator(f => new Setor(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<SetorBenef> _setorBeneficiamentos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Relação dos beneficiamentos do setor.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<SetorBenef> SetorBeneficiamentos
        {
            get { return _setorBeneficiamentos; }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public int IdSetor
        {
            get { return DataModel.IdSetor; }
            set
            {
                if (DataModel.IdSetor != value &&
                    RaisePropertyChanging("IdSetor", value))
                {
                    DataModel.IdSetor = value;
                    RaisePropertyChanged("IdSetor");
                }
            }
        }

        /// <summary>
        /// Identificador do CNC.
        /// </summary>
        public int? IdCnc
        {
            get { return DataModel.IdCnc; }
            set
            {
                if (DataModel.IdCnc != value &&
                    RaisePropertyChanging("IdCnc", value))
                {
                    DataModel.IdCnc = value;
                    RaisePropertyChanged("IdCnc");
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
        /// Sigla.
        /// </summary>
        public string Sigla
        {
            get { return DataModel.Sigla; }
            set
            {
                if (DataModel.Sigla != value &&
                    RaisePropertyChanging("Sigla", value))
                {
                    DataModel.Sigla = value;
                    RaisePropertyChanged("Sigla");
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
        /// Tipo.
        /// </summary>
        public Glass.Data.Model.TipoSetor Tipo
        {
            get { return DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Cor.
        /// </summary>
        public Glass.Data.Model.CorSetor Cor
        {
            get { return DataModel.Cor; }
            set
            {
                if (DataModel.Cor != value &&
                    RaisePropertyChanging("Cor", value))
                {
                    DataModel.Cor = value;
                    RaisePropertyChanged("Cor");
                }
            }
        }

        /// <summary>
        /// Cor da tela.
        /// </summary>
        public Glass.Data.Model.CorTelaSetor CorTela
        {
            get { return DataModel.CorTela; }
            set
            {
                if (DataModel.CorTela != value &&
                    RaisePropertyChanging("CorTela", value))
                {
                    DataModel.CorTela = value;
                    RaisePropertyChanged("CorTela");
                }
            }
        }

        /// <summary>
        /// Número de sequência.
        /// </summary>
        public int NumeroSequencia
        {
            get { return DataModel.NumeroSequencia; }
            set
            {
                if (DataModel.NumeroSequencia != value &&
                    RaisePropertyChanging("NumeroSequencia", value))
                {
                    DataModel.NumeroSequencia = value;
                    RaisePropertyChanged("NumeroSequencia");
                }
            }
        }

        /// <summary>
        /// Entrada estoque.
        /// </summary>
        public bool EntradaEstoque
        {
            get { return DataModel.EntradaEstoque; }
            set
            {
                if (DataModel.EntradaEstoque != value &&
                    RaisePropertyChanging("EntradaEstoque", value))
                {
                    DataModel.EntradaEstoque = value;
                    RaisePropertyChanged("EntradaEstoque");
                }
            }
        }

        /// <summary>
        /// Impedir Avanço.
        /// </summary>
        public bool ImpedirAvanco
        {
            get { return DataModel.ImpedirAvanco; }
            set
            {
                if (DataModel.ImpedirAvanco != value &&
                    RaisePropertyChanging("ImpedirAvanco", value))
                {
                    DataModel.ImpedirAvanco = value;
                    RaisePropertyChanged("ImpedirAvanco");
                }
            }
        }

        /// <summary>
        /// Informar Rota.
        /// </summary>
        public bool InformarRota
        {
            get { return DataModel.InformarRota; }
            set
            {
                if (DataModel.InformarRota != value &&
                    RaisePropertyChanging("InformarRota", value))
                {
                    DataModel.InformarRota = value;
                    RaisePropertyChanged("InformarRota");
                }
            }
        }

        /// <summary>
        /// Corte.
        /// </summary>
        public bool Corte
        {
            get { return DataModel.Corte; }
            set
            {
                if (DataModel.Corte != value &&
                    RaisePropertyChanging("Corte", value))
                {
                    DataModel.Corte = value;
                    RaisePropertyChanged("Corte");
                }
            }
        }

        /// <summary>
        /// Forno.
        /// </summary>
        public bool Forno
        {
            get { return DataModel.Forno; }
            set
            {
                if (DataModel.Forno != value &&
                    RaisePropertyChanging("Forno", value))
                {
                    DataModel.Forno = value;
                    RaisePropertyChanged("Forno");
                }
            }
        }

        /// <summary>
        /// Laminado.
        /// </summary>
        public bool Laminado
        {
            get { return DataModel.Laminado; }
            set
            {
                if (DataModel.Laminado != value &&
                    RaisePropertyChanging("Laminado", value))
                {
                    DataModel.Laminado = value;
                    RaisePropertyChanged("Laminado");
                }
            }
        }

        /// <summary>
        /// Exibir setores.
        /// </summary>
        public bool ExibirSetores
        {
            get { return DataModel.ExibirSetores; }
            set
            {
                if (DataModel.ExibirSetores != value &&
                    RaisePropertyChanging("ExibirSetores", value))
                {
                    DataModel.ExibirSetores = value;
                    RaisePropertyChanged("ExibirSetores");
                }
            }
        }

        /// <summary>
        /// Exibir imagem completa.
        /// </summary>
        public bool ExibirImagemCompleta
        {
            get { return DataModel.ExibirImagemCompleta; }
            set
            {
                if (DataModel.ExibirImagemCompleta != value &&
                    RaisePropertyChanging("ExibirImagemCompleta", value))
                {
                    DataModel.ExibirImagemCompleta = value;
                    RaisePropertyChanged("ExibirImagemCompleta");
                }
            }
        }

        /// <summary>
        /// Tempo de login.
        /// </summary>
        public int TempoLogin
        {
            get { return DataModel.TempoLogin; }
            set
            {
                if (DataModel.TempoLogin != value &&
                    RaisePropertyChanging("TempoLogin", value))
                {
                    DataModel.TempoLogin = value;
                    RaisePropertyChanged("TempoLogin");
                }
            }
        }

        /// <summary>
        /// Consulta antes.
        /// </summary>
        public bool ConsultarAntes
        {
            get { return DataModel.ConsultarAntes; }
            set
            {
                if (DataModel.ConsultarAntes != value &&
                    RaisePropertyChanging("ConsultarAntes", value))
                {
                    DataModel.ConsultarAntes = value;
                    RaisePropertyChanged("ConsultarAntes");
                }
            }
        }

        /// <summary>
        /// Exibir relatorio.
        /// </summary>
        public bool ExibirRelatorio
        {
            get { return DataModel.ExibirRelatorio; }
            set
            {
                if (DataModel.ExibirRelatorio != value &&
                    RaisePropertyChanging("ExibirRelatorio", value))
                {
                    DataModel.ExibirRelatorio = value;
                    RaisePropertyChanged("ExibirRelatorio");
                }
            }
        }

        /// <summary>
        /// Desafio perda.
        /// </summary>
        public double DesafioPerda
        {
            get { return DataModel.DesafioPerda; }
            set
            {
                if (DataModel.DesafioPerda != value &&
                    RaisePropertyChanging("DesafioPerda", value))
                {
                    DataModel.DesafioPerda = value;
                    RaisePropertyChanged("DesafioPerda");
                }
            }
        }

        /// <summary>
        /// Metda de perda mensalç.
        /// </summary>
        public double MetaPerda
        {
            get { return DataModel.MetaPerda; }
            set
            {
                if (DataModel.MetaPerda != value &&
                    RaisePropertyChanging("MetaPerda", value))
                {
                    DataModel.MetaPerda = value;
                    RaisePropertyChanged("MetaPerda");
                }
            }
        }

        /// <summary>
        /// Capacidade diária.
        /// </summary>
        public int? CapacidadeDiaria
        {
            get { return DataModel.CapacidadeDiaria; }
            set
            {
                if (DataModel.CapacidadeDiaria != value &&
                    RaisePropertyChanging("CapacidadeDiaria", value))
                {
                    DataModel.CapacidadeDiaria = value;
                    RaisePropertyChanged("CapacidadeDiaria");
                }
            }
        }

        /// <summary>
        /// Ignorar capacidade diária.
        /// </summary>
        public bool IgnorarCapacidadeDiaria
        {
            get { return DataModel.IgnorarCapacidadeDiaria; }
            set
            {
                if (DataModel.IgnorarCapacidadeDiaria != value &&
                    RaisePropertyChanging("IgnorarCapacidadeDiaria", value))
                {
                    DataModel.IgnorarCapacidadeDiaria = value;
                    RaisePropertyChanged("IgnorarCapacidadeDiaria");
                }
            }
        }

        /// <summary>
        /// Permitir leitura fora do roteiro.
        /// </summary>
        public bool PermitirLeituraForaRoteiro
        {
            get { return DataModel.PermitirLeituraForaRoteiro; }
            set
            {
                if (DataModel.PermitirLeituraForaRoteiro != value &&
                    RaisePropertyChanging("PermitirLeituraForaRoteiro", value))
                {
                    DataModel.PermitirLeituraForaRoteiro = value;
                    RaisePropertyChanged("PermitirLeituraForaRoteiro");
                }
            }
        }

        /// <summary>
        /// Exibir no painel comercial.
        /// </summary>
        public bool ExibirPainelComercial
        {
            get { return DataModel.ExibirPainelComercial; }
            set
            {
                if (DataModel.ExibirPainelComercial != value &&
                    RaisePropertyChanging("ExibirPainelComercial", value))
                {
                    DataModel.ExibirPainelComercial = value;
                    RaisePropertyChanged("ExibirPainelComercial");
                }
            }
        }        
 
        /// <summary>
        /// Exibir no painel da produÃ§Ã£o.
        /// </summary>
        public bool ExibirPainelProducao
        {
            get { return DataModel.ExibirPainelProducao; }
            set
            {
                if (DataModel.ExibirPainelProducao != value &&
                    RaisePropertyChanging("ExibirPainelProducao", value))
                {
                    DataModel.ExibirPainelProducao = value;
                    RaisePropertyChanged("ExibirPainelProducao");
                }
            }
        }

        /// <summary>
        /// Tempo para alerta de inatividade.
        /// </summary>
        public int TempoAlertaInatividade
        {
            get { return DataModel.TempoAlertaInatividade; }
            set
            {
                if (DataModel.TempoAlertaInatividade != value &&
                    RaisePropertyChanging("TempoAlertaInatividade", value))
                {
                    DataModel.TempoAlertaInatividade = value;
                    RaisePropertyChanged("TempoAlertaInatividade");
                }
            }
        }

        /// <summary>
        /// Indica se deve ser informado o numero do cavelate na leitura do setor.
        /// </summary>
        public bool InformarCavalete
        {
            get { return DataModel.InformarCavalete; }
            set
            {
                if (DataModel.InformarCavalete != value &&
                    RaisePropertyChanging("InformarCavalete", value))
                {
                    DataModel.InformarCavalete = value;
                    RaisePropertyChanged("InformarCavalete");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Setor()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Setor(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Setor> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _setorBeneficiamentos = GetChild<SetorBenef>(args.Children, "SetorBeneficiamentos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Setor(Glass.Data.Model.Setor dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _setorBeneficiamentos = CreateChild<Colosoft.Business.IEntityChildrenList<SetorBenef>>("SetorBeneficiamentos");
        }

        #endregion

        #region Métodos Publicos

        /// <summary>
        /// Método acionado para salvar as alterações do setor.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            // Verifica se o setor não for do tipo por beneficiamento
            if (Tipo != Glass.Data.Model.TipoSetor.PorBenef)
                // Limpa os beneficiamentos do setor
                SetorBeneficiamentos.Clear();

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IProvedorSetor>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            var salvar = base.Save(session);

            // Reseta a lista de setores
            Glass.Data.Helper.Utils.GetSetores = null;

            return salvar;
        }

        #endregion
    }
}
