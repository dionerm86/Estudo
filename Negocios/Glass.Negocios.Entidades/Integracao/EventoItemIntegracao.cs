// <copyright file="EventoItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Integracao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do evento do item de integração.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(EventoItemIntegracaoLoader))]
    public class EventoItemIntegracao : Colosoft.Business.Entity<Data.Model.EventoItemIntegracao>
    {
        private FalhaIntegracao falha;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EventoItemIntegracao"/>.
        /// </summary>
        public EventoItemIntegracao()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EventoItemIntegracao"/>.
        /// </summary>
        /// <param name="args">Argumentos do loader.</param>
        protected EventoItemIntegracao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.EventoItemIntegracao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            this.falha = this.GetSingleChild<FalhaIntegracao>(args.Children, nameof(this.Falha));
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EventoItemIntegracao"/>
        /// com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel">Modelo de dados.</param>
        /// <param name="uiContext">Contexto da interface com o usuário.</param>
        /// <param name="entityTypeManager">Gerencidor de tipos de entidade.</param>
        public EventoItemIntegracao(Data.Model.EventoItemIntegracao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
        }

        /// <summary>
        /// Obtém ou define a falha associada.
        /// </summary>
        public FalhaIntegracao Falha
        {
            get
            {
                return this.falha;
            }

            set
            {
                if (this.falha != value &&
                   this.RaisePropertyChanging(nameof(this.Falha), value))
                {
                    if (this.falha != null)
                    {
                        this.UnregisterChild(value);
                    }

                    if (value != null)
                    {
                        value.IdEventoItemIntegracao = this.IdEventoItemIntegracao;
                        this.RegisterChild(value);
                    }

                    this.falha = value;
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do evento do item de integração.
        /// </summary>
        public int IdEventoItemIntegracao
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
        /// Obtém ou define o identificador do item de integração.
        /// </summary>
        public int IdItemIntegracao
        {
            get
            {
                return this.DataModel.IdItemIntegracao;
            }

            set
            {
                if (this.DataModel.IdItemIntegracao != value &&
                    this.RaisePropertyChanging(nameof(this.IdItemIntegracao), value))
                {
                    this.DataModel.IdItemIntegracao = value;
                    this.RaisePropertyChanged(nameof(this.IdItemIntegracao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define tipo de evento.
        /// </summary>
        public Data.Model.TipoEventoItemIntegracao Tipo
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
        /// Obtém ou define a mensagem.
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
        /// Obtém ou define a data que o evento ocorreu.
        /// </summary>
        public DateTime Data
        {
            get
            {
                return this.DataModel.Data;
            }

            set
            {
                if (this.DataModel.Data != value &&
                    this.RaisePropertyChanging(nameof(this.Data), value))
                {
                    this.DataModel.Data = value;
                    this.RaisePropertyChanged(nameof(this.Data));
                }
            }
        }

        private class EventoItemIntegracaoLoader : Colosoft.Business.EntityLoader<EventoItemIntegracao, Data.Model.EventoItemIntegracao>
        {
            public EventoItemIntegracaoLoader()
            {
                this.Configure()
                    .Uid(f => f.IdEventoItemIntegracao)
                    .SingleChild<FalhaIntegracao, Glass.Data.Model.FalhaIntegracao>(nameof(EventoItemIntegracao.Falha), f => f.Falha, f => f.IdEventoItemIntegracao)
                    .Creator(f => new EventoItemIntegracao(f));
            }
        }
    }
}
