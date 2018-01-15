using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio de loga alteração.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(LogAlteracaoLoader))]
    public class LogAlteracao : Colosoft.Business.Entity<Data.Model.LogAlteracao>, Sync.Fiscal.EFD.Entidade.IAlteracaoLog
    {
        #region Tipos Aninhados

        class LogAlteracaoLoader : Colosoft.Business.EntityLoader<LogAlteracao, Data.Model.LogAlteracao>
        {
            public LogAlteracaoLoader()
            {
                Configure()
                    .Uid(f => f.IdLog)
                    .Reference<Funcionario, Data.Model.Funcionario>("Funcionario", f => f.FuncionarioAlteracao, f => f.IdFuncAlt)
                    .Creator(f => new LogAlteracao(f));
            }
        }

        #endregion

        #region Propriedades

        public Funcionario FuncionarioAlteracao
        {
            get { return GetReference<Funcionario>("Funcionario", true); }
        }

        /// <summary>
        /// Identificador do log.
        /// </summary>
        public int IdLog
        {
            get { return DataModel.IdLog; }
            set
            {
                if (DataModel.IdLog != value &&
                    RaisePropertyChanging("IdLog", value))
                {
                    DataModel.IdLog = value;
                    RaisePropertyChanged("IdLog");
                }
            }
        }

        /// <summary>
        /// Tabela alterada.
        /// </summary>
        public int Tabela
        {
            get { return DataModel.Tabela; }
            set
            {
                if (DataModel.Tabela != value &&
                    RaisePropertyChanging("Tabela", value))
                {
                    DataModel.Tabela = value;
                    RaisePropertyChanged("Tabela");
                }
            }
        }

        /// <summary>
        /// Identificador do registro alterado.
        /// </summary>
        public int IdRegistroAlt
        {
            get { return DataModel.IdRegistroAlt; }
            set
            {
                if (DataModel.IdRegistroAlt != value &&
                    RaisePropertyChanging("IdRegistroAlt", value))
                {
                    DataModel.IdRegistroAlt = value;
                    RaisePropertyChanged("IdRegistroAlt");
                }
            }
        }

        /// <summary>
        /// Número do evento.
        /// </summary>
        public int NumEvento
        {
            get { return (int)DataModel.NumEvento; }
            set
            {
                if (DataModel.NumEvento != value &&
                    RaisePropertyChanging("NumEvento", value))
                {
                    DataModel.NumEvento = (uint)value;
                    RaisePropertyChanged("NumEvento");
                }
            }
        }

        /// <summary>
        /// Identificador do funcionário que alterou.
        /// </summary>
        public int IdFuncAlt
        {
            get { return (int)DataModel.IdFuncAlt; }
            set
            {
                if (DataModel.IdFuncAlt != value &&
                    RaisePropertyChanging("IdFuncAlt", value))
                {
                    DataModel.IdFuncAlt = (uint)value;
                    RaisePropertyChanged("IdFuncAlt");
                }
            }
        }

        /// <summary>
        /// Data da alteração
        /// </summary>
        public DateTime DataAlt
        {
            get { return DataModel.DataAlt; }
            set
            {
                if (DataModel.DataAlt != value &&
                    RaisePropertyChanging("DataAlt", value))
                {
                    DataModel.DataAlt = value;
                    RaisePropertyChanged("DataAlt");
                }
            }
        }

        /// <summary>
        /// Nome do campo
        /// </summary>
        public string Campo
        {
            get { return DataModel.Campo; }
            set
            {
                if (DataModel.Campo != value &&
                    RaisePropertyChanging("Campo", value))
                {
                    DataModel.Campo = value;
                    RaisePropertyChanged("Campo");
                }
            }
        }

        /// <summary>
        /// Valor anterior do campo
        /// </summary>
        public string ValorAnterior
        {
            get { return DataModel.ValorAnterior; }
            set
            {
                if (DataModel.ValorAnterior != value &&
                    RaisePropertyChanging("ValorAnterior", value))
                {
                    DataModel.ValorAnterior = value;
                    RaisePropertyChanged("ValorAnterior");
                }
            }
        }

        /// <summary>
        /// Valor atual do campo
        /// </summary>
        public string ValorAtual
        {
            get { return DataModel.ValorAtual; }
            set
            {
                if (DataModel.ValorAtual != value &&
                    RaisePropertyChanging("ValorAtual", value))
                {
                    DataModel.ValorAtual = value;
                    RaisePropertyChanged("ValorAtual");
                }
            }
        }

        /// <summary>
        /// Referência do log
        /// </summary>
        public string Referencia
        {
            get { return DataModel.Referencia; }
            set
            {
                if (DataModel.Referencia != value &&
                    RaisePropertyChanging("Referencia", value))
                {
                    DataModel.Referencia = value;
                    RaisePropertyChanged("Referencia");
                }
            }
        }

        #region membros de IAlteracaoLog

        public DateTime DataAlteracao
        {
            get
            {
                return DataAlt;
            }
        }

        public int NumeroCampoAlteracao
        {
            get
            {
                return NumEvento;
            }
        }

        public string ValorNovo
        {
            get
            {
                return ValorAtual;
            }
        }
        
        #endregion
        
        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public LogAlteracao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected LogAlteracao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.LogAlteracao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public LogAlteracao(Glass.Data.Model.LogAlteracao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
