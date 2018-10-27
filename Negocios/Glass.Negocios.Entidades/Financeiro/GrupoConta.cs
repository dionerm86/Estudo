using Glass.Global.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do grupo de contas.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(GrupoContaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.GrupoConta)]
    public class GrupoConta : Colosoft.Business.Entity<Data.Model.GrupoConta>
    {
        #region Tipos Aninhados

        class GrupoContaLoader : Colosoft.Business.EntityLoader<GrupoConta, Data.Model.GrupoConta>
        {
            public GrupoContaLoader()
            {
                Configure()
                    .Uid(f => f.IdGrupo)
                    .FindName(f => f.Descricao)
                    .Reference<CategoriaConta, Data.Model.CategoriaConta>("Categoria", f => f.Categoria, f => f.IdCategoriaConta)
                    .Creator(f => new GrupoConta(f));
            }
        }

        #endregion

        #region Propriedades

        public CategoriaConta Categoria
        {
            get { return GetReference<CategoriaConta>("Categoria", true); }
        }

        /// <summary>
        /// Identificador do grupo.
        /// </summary>
        public int IdGrupo
        {
            get { return DataModel.IdGrupo; }
            set
            {
                if (DataModel.IdGrupo != value &&
                    RaisePropertyChanging("IdGrupo", value))
                {
                    DataModel.IdGrupo = value;
                    RaisePropertyChanged("IdGrupo");
                }
            }
        }

        /// <summary>
        /// Identificador da categoria de conta.
        /// </summary>
        public int? IdCategoriaConta
        {
            get { return DataModel.IdCategoriaConta; }
            set
            {
                if (DataModel.IdCategoriaConta != value &&
                    RaisePropertyChanging("IdCategoriaConta", value))
                {
                    DataModel.IdCategoriaConta = value;
                    RaisePropertyChanged("IdCategoriaConta");
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
        /// Ponto de equilibrio.
        /// </summary>
        public bool PontoEquilibrio
        {
            get { return DataModel.PontoEquilibrio; }
            set
            {
                if (DataModel.PontoEquilibrio != value &&
                    RaisePropertyChanging("PontoEquilibrio", value))
                {
                    DataModel.PontoEquilibrio = value;
                    RaisePropertyChanged("PontoEquilibrio");
                }
            }
        }

        /// <summary>
        /// Número da sequencia.
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public GrupoConta()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected GrupoConta(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.GrupoConta> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public GrupoConta(Data.Model.GrupoConta dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva o log de alteração da Categoria
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (ExistsInStorage)
            {
                #region Log

                if (ChangedProperties.Contains("IdCategoriaConta"))
                {
                    var grupoContaAnterior = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<IProvedorPlanoContas>()
                        .ObtemGrupoConta(IdGrupo);

                    var logAlteracao = ObterLogAlteracao();

                    logAlteracao.ValorAnterior = grupoContaAnterior.Categoria.Descricao;
                    logAlteracao.ValorAtual = Categoria?.Descricao;
                    logAlteracao.Campo = "Categoria";

                    logAlteracao.Save(session);
                }

                #endregion
            }

            return base.Save(session);
        }

        /// <summary>
        /// Método que retorna LogAlteracao com os dados comuns preenchidos.
        /// </summary>
        /// <returns></returns>
        private LogAlteracao ObterLogAlteracao()
        {
            var controleAlteracao = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Negocios.IControleAlteracao>();

            var logAlteracao = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Negocios.Entidades.ICriarEntidadeProvedor>()
                .CriarEntidade<LogAlteracao>() as LogAlteracao;

            logAlteracao.Tabela = (int)Data.Model.LogAlteracao.TabelaAlteracao.GrupoConta;
            logAlteracao.DataAlt = DateTime.Now;
            logAlteracao.IdRegistroAlt = IdGrupo;
            logAlteracao.IdFuncAlt = (int)Data.Helper.UserInfo.GetUserInfo.CodUser;
            logAlteracao.Referencia = IdGrupo.ToString();
            logAlteracao.NumEvento = controleAlteracao.ObterNumeroEventoAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.GrupoConta, IdGrupo);

            return logAlteracao;
        }

        #endregion
    }
}
