// <copyright file="FalhaIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.Integracao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da falha de integração.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FalhaIntegracaoLoader))]
    public class FalhaIntegracao : Colosoft.Business.Entity<Data.Model.FalhaIntegracao>
    {
        private FalhaIntegracao falhaInterna;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FalhaIntegracao"/>.
        /// </summary>
        public FalhaIntegracao()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FalhaIntegracao"/>.
        /// </summary>
        /// <param name="args">Argumentos do loader.</param>
        protected FalhaIntegracao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FalhaIntegracao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            this.falhaInterna = this.GetSingleChild<FalhaIntegracao>(args.Children, nameof(this.FalhaInterna));
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FalhaIntegracao"/>
        /// com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel">Modelo de dados.</param>
        /// <param name="uiContext">Contexto da interface com o usuário.</param>
        /// <param name="entityTypeManager">Gerencidor de tipos de entidade.</param>
        public FalhaIntegracao(Data.Model.FalhaIntegracao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
        }

        /// <summary>
        /// Obtém ou define a falha interna.
        /// </summary>
        public FalhaIntegracao FalhaInterna
        {
            get
            {
                return this.falhaInterna;
            }

            set
            {
                if (this.falhaInterna != value &&
                   this.RaisePropertyChanging(nameof(this.FalhaInterna), value))
                {
                    if (this.falhaInterna != null)
                    {
                        this.UnregisterChild(value);
                    }

                    if (value != null)
                    {
                        value.IdFalhaIntegracaoPai = this.IdFalhaIntegracao;
                        this.RegisterChild(value);
                    }

                    this.falhaInterna = value;
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador da falha.
        /// </summary>
        public int IdFalhaIntegracao
        {
            get
            {
                return this.DataModel.IdFalhaIntegracao;
            }

            set
            {
                if (this.DataModel.IdFalhaIntegracao != value &&
                    this.RaisePropertyChanging(nameof(this.IdFalhaIntegracao), value))
                {
                    this.DataModel.IdFalhaIntegracao = value;
                    this.RaisePropertyChanged(nameof(this.IdFalhaIntegracao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do evento do item de integração.
        /// </summary>
        public int? IdEventoItemIntegracao
        {
            get
            {
                return this.DataModel.IdEventoItemIntegracao;
            }

            set
            {
                if (this.DataModel.IdEventoItemIntegracao != value &&
                    this.RaisePropertyChanging(nameof(this.IdEventoItemIntegracao), value))
                {
                    this.DataModel.IdEventoItemIntegracao = value;
                    this.RaisePropertyChanged(nameof(this.IdEventoItemIntegracao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador da falha pai.
        /// </summary>
        public int? IdFalhaIntegracaoPai
        {
            get
            {
                return this.DataModel.IdFalhaIntegracaoPai;
            }

            set
            {
                if (this.DataModel.IdFalhaIntegracaoPai != value &&
                    this.RaisePropertyChanging(nameof(this.IdFalhaIntegracaoPai), value))
                {
                    this.DataModel.IdFalhaIntegracaoPai = value;
                    this.RaisePropertyChanged(nameof(this.IdFalhaIntegracaoPai));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o tipo da falha.
        /// </summary>
        public string Tipo
        {
            get
            {
                return this.DataModel.Tipo;
            }

            set
            {
                if (this.DataModel.Tipo != value &&
                    this.RaisePropertyChanging(nameof(this.Tipo), value))
                {
                    this.DataModel.Tipo = value;
                    this.RaisePropertyChanged(nameof(this.Tipo));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a mensagem da falha.
        /// </summary>
        public string Mensagem
        {
            get
            {
                return this.DataModel.Mensagem;
            }

            set
            {
                if (this.DataModel.Mensagem != value &&
                    this.RaisePropertyChanging(nameof(this.Mensagem), value))
                {
                    this.DataModel.Mensagem = value;
                    this.RaisePropertyChanged(nameof(this.Mensagem));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a pilha de chamada.
        /// </summary>
        public string PilhaChamada
        {
            get
            {
                return this.DataModel.PilhaChamada;
            }

            set
            {
                if (this.DataModel.PilhaChamada != value &&
                    this.RaisePropertyChanging(nameof(this.PilhaChamada), value))
                {
                    this.DataModel.PilhaChamada = value;
                    this.RaisePropertyChanged(nameof(this.PilhaChamada));
                }
            }
        }

        private class FalhaIntegracaoLoader : Colosoft.Business.EntityLoader<FalhaIntegracao, Data.Model.FalhaIntegracao>
        {
            public FalhaIntegracaoLoader()
            {
                this.Configure()
                    .Uid(f => f.IdFalhaIntegracao)
                    .SingleChild<FalhaIntegracao, Data.Model.FalhaIntegracao>(nameof(FalhaIntegracao.FalhaInterna), f => f.FalhaInterna, f => f.IdFalhaIntegracaoPai)
                    .Creator(f => new FalhaIntegracao(f));
            }
        }
    }
}
