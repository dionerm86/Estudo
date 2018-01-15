using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa uma mensagem.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(MensagemLoader))]
    public class Mensagem : Colosoft.Business.Entity<Glass.Data.Model.Mensagem>
    {
        #region Tipos Aninhados

        class MensagemLoader : Colosoft.Business.EntityLoader<Mensagem, Glass.Data.Model.Mensagem>
        {
            public MensagemLoader()
            {
                Configure()
                    .Uid(f => f.IdMensagem)
                    .Child<Destinatario, Glass.Data.Model.Destinatario>("Destinatarios", f => f.Destinatarios, f => f.IdMensagem)
                    .Creator(f => new Mensagem(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<Destinatario> _destinatarios;

        #endregion

        #region Propriedades

        /// <summary>
        /// Destinatários da mensagem.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Destinatario> Destinatarios
        {
            get { return _destinatarios; }
        }

        /// <summary>
        /// Identificador da mensagem.
        /// </summary>
        public int IdMensagem
        {
            get { return DataModel.IdMensagem; }
            set
            {
                if (DataModel.IdMensagem != value &&
                    RaisePropertyChanging("IdMensagem", value))
                {
                    DataModel.IdMensagem = value;
                    RaisePropertyChanged("IdMensagem");
                }
            }
        }

        /// <summary>
        /// Identificador do remetente..
        /// </summary>
        public int IdRemetente
        {
            get { return DataModel.IdRemetente; }
            set
            {
                if (DataModel.IdRemetente != value &&
                    RaisePropertyChanging("IdRemetente", value))
                {
                    DataModel.IdRemetente = value;
                    RaisePropertyChanged("IdRemetente");
                }
            }
        }

        /// <summary>
        /// Assunto.
        /// </summary>
        public string Assunto
        {
            get { return DataModel.Assunto; }
            set
            {
                if (DataModel.Assunto != value &&
                    RaisePropertyChanging("Assunto", value))
                {
                    DataModel.Assunto = value;
                    RaisePropertyChanged("Assunto");
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
        /// Data de Cadastro.
        /// </summary>
        public DateTime DataCad
        {
            get { return DataModel.DataCad; }
            set
            {
                if (DataModel.DataCad != value &&
                    RaisePropertyChanging("DataCad", value))
                {
                    DataModel.DataCad = value;
                    RaisePropertyChanged("DataCad");
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Mensagem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Mensagem(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Mensagem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _destinatarios = GetChild<Destinatario>(args.Children, "Destinatarios");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Mensagem(Glass.Data.Model.Mensagem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _destinatarios = CreateChild<Colosoft.Business.IEntityChildrenList<Destinatario>>("Destinatarios");
        }

        #endregion
    }
}
