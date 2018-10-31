// <copyright file="ItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Integracao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do item de integração.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ItemIntegracaoLoader))]
    public class ItemIntegracao : Colosoft.Business.Entity<Data.Model.ItemIntegracao>
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemIntegracao"/>.
        /// </summary>
        public ItemIntegracao()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemIntegracao"/>.
        /// </summary>
        /// <param name="args">Argumentos do loader.</param>
        protected ItemIntegracao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ItemIntegracao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            this.Eventos = this.GetChild<EventoItemIntegracao>(args.Children, nameof(this.Eventos));
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemIntegracao"/>
        /// com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel">Modelo de dados.</param>
        /// <param name="uiContext">Contexto da interface com o usuário.</param>
        /// <param name="entityTypeManager">Gerencidor de tipos de entidade.</param>
        public ItemIntegracao(Data.Model.ItemIntegracao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            this.Eventos = this.CreateChild<Colosoft.Business.IEntityChildrenList<EventoItemIntegracao>>(nameof(this.Eventos));
        }

        /// <summary>
        /// Obtém os eventos do item.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<EventoItemIntegracao> Eventos { get; }

        /// <summary>
        /// Obtém ou define o identificador do itme de integração.
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
        /// Obtém ou define o identificador do esquema do histórico.
        /// </summary>
        public int IdEsquemaHistorico
        {
            get
            {
                return this.DataModel.IdEsquemaHistorico;
            }

            set
            {
                if (this.DataModel.IdEsquemaHistorico != value &&
                    this.RaisePropertyChanging(nameof(this.IdEsquemaHistorico), value))
                {
                    this.DataModel.IdEsquemaHistorico = value;
                    this.RaisePropertyChanged(nameof(this.IdEsquemaHistorico));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do item do esquema do histórico.
        /// </summary>
        public int IdItemEsquemaHistorico
        {
            get
            {
                return this.DataModel.IdItemEsquemaHistorico;
            }

            set
            {
                if (this.DataModel.IdItemEsquemaHistorico != value &&
                    this.RaisePropertyChanging(nameof(this.IdItemEsquemaHistorico), value))
                {
                    this.DataModel.IdItemEsquemaHistorico = value;
                    this.RaisePropertyChanged(nameof(this.IdItemEsquemaHistorico));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o primeiro identificador inteiro do item.
        /// </summary>
        public int IdInteiro1
        {
            get
            {
                return this.DataModel.IdInteiro1;
            }

            set
            {
                if (this.DataModel.IdInteiro1 != value &&
                    this.RaisePropertyChanging(nameof(this.IdInteiro1), value))
                {
                    this.DataModel.IdInteiro1 = value;
                    this.RaisePropertyChanged(nameof(this.IdInteiro1));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o segundo identificador inteiro do item.
        /// </summary>
        public int IdInteiro2
        {
            get
            {
                return this.DataModel.IdInteiro2;
            }

            set
            {
                if (this.DataModel.IdInteiro2 != value &&
                    this.RaisePropertyChanging(nameof(this.IdInteiro2), value))
                {
                    this.DataModel.IdInteiro2 = value;
                    this.RaisePropertyChanged(nameof(this.IdInteiro2));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador textual do item.
        /// </summary>
        public string IdTextual
        {
            get
            {
                return this.DataModel.IdTextual;
            }

            set
            {
                if (this.DataModel.IdTextual != value &&
                    this.RaisePropertyChanging(nameof(this.IdTextual), value))
                {
                    this.DataModel.IdTextual = value;
                    this.RaisePropertyChanged(nameof(this.IdTextual));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a situação.
        /// </summary>
        public Data.Model.SituacaoItemIntegracao Situacao
        {
            get
            {
                return this.DataModel.Situacao;
            }

            set
            {
                if (this.DataModel.Situacao != value &&
                    this.RaisePropertyChanging(nameof(this.Situacao), value))
                {
                    this.DataModel.Situacao = value;
                    this.RaisePropertyChanged(nameof(this.Situacao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a data da última atualização.
        /// </summary>
        public DateTime UltimaAtualizacao
        {
            get
            {
                return this.DataModel.UltimaAtualizacao;
            }

            set
            {
                if (this.DataModel.UltimaAtualizacao != value &&
                    this.RaisePropertyChanging(nameof(this.UltimaAtualizacao), value))
                {
                    this.DataModel.UltimaAtualizacao = value;
                    this.RaisePropertyChanged(nameof(this.UltimaAtualizacao));
                }
            }
        }

        private class ItemIntegracaoLoader : Colosoft.Business.EntityLoader<ItemIntegracao, Data.Model.ItemIntegracao>
        {
            public ItemIntegracaoLoader()
            {
                this.Configure()
                    .Uid(f => f.IdItemIntegracao)
                    .Child<EventoItemIntegracao, Data.Model.EventoItemIntegracao>(
                        nameof(ItemIntegracao.Eventos), f => f.Eventos, f => f.IdItemIntegracao, Colosoft.Business.LoadOptions.Lazy)
                    .Creator(f => new ItemIntegracao(f));
            }
        }
    }
}
