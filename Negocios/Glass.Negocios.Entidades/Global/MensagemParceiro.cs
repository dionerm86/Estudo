using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa uma mensagem para parceiro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(MensagemParceiroLoader))]
    public class MensagemParceiro : Colosoft.Business.Entity<Glass.Data.Model.MensagemParceiro>
    {
        #region Tipos Aninhados

        class MensagemParceiroLoader : Colosoft.Business.EntityLoader<MensagemParceiro, Glass.Data.Model.MensagemParceiro>
        {
            public MensagemParceiroLoader()
            {
                Configure()
                    .Uid(f => f.IdMensagemParceiro)
                    .Child<DestinatarioParceiroFuncionario, Glass.Data.Model.DestinatarioParceiroFuncionario>
                        ("DestinatariosFuncionario", f => f.DestinatariosFuncionario, f => f.IdMensagemParceiro)
                    .Child<DestinatarioParceiroCliente, Glass.Data.Model.DestinatarioParceiroCliente>
                        ("DestinatariosCliente", f => f.DestinatariosCliente, f => f.IdMensagemParceiro)
                    .Creator(f => new MensagemParceiro(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<DestinatarioParceiroCliente> _destinatariosCliente;
        private Colosoft.Business.IEntityChildrenList<DestinatarioParceiroFuncionario> _destinatariosFuncionario;

        #endregion

        #region Propriedades

        /// <summary>
        /// Destinatários da mensagem que são clientes.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<DestinatarioParceiroCliente> DestinatariosCliente
        {
            get { return _destinatariosCliente; }
        }

        /// <summary>
        /// Destinatários da mensagem que são funcionários.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<DestinatarioParceiroFuncionario> DestinatariosFuncionario
        {
            get { return _destinatariosFuncionario; }
            set { _destinatariosFuncionario = value; }
        }

        /// <summary>
        /// Identificador da mensagem do parceiro.
        /// </summary>
        public int IdMensagemParceiro
        {
            get { return DataModel.IdMensagemParceiro; }
            set
            {
                if (DataModel.IdMensagemParceiro != value &&
                    RaisePropertyChanging("IdMensagemParceiro", value))
                {
                    DataModel.IdMensagemParceiro = value;
                    RaisePropertyChanged("IdMensagemParceiro");
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
        /// Data de cadastro.
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

        /// <summary>
        /// Identifica se uma mensagem enviada por um funcionario.
        /// </summary>
        public bool IsFunc
        {
            get { return DataModel.IsFunc; }
            set
            {
                if (DataModel.IsFunc != value &&
                    RaisePropertyChanging("IsFunc", value))
                {
                    DataModel.IsFunc = value;
                    RaisePropertyChanged("IsFunc");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public MensagemParceiro()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected MensagemParceiro(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.MensagemParceiro> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _destinatariosFuncionario = GetChild<DestinatarioParceiroFuncionario>(args.Children, "DestinatariosFuncionario");
            _destinatariosCliente = GetChild<DestinatarioParceiroCliente>(args.Children, "DestinatariosCliente");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public MensagemParceiro(Glass.Data.Model.MensagemParceiro dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _destinatariosFuncionario = CreateChild<Colosoft.Business.IEntityChildrenList<DestinatarioParceiroFuncionario>>("DestinatariosFuncionario");
            _destinatariosCliente = CreateChild<Colosoft.Business.IEntityChildrenList<DestinatarioParceiroCliente>>("DestinatariosCliente");
        }

        #endregion
    }
}
