// <copyright file="ProvedorHistorico.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Colosoft.Collections;
using Glass.Integracao.Historico;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Integracao.Negocios.Componentes
{
    /// <summary>
    /// Representa o provedor de histórico do sistema.
    /// </summary>
    public class ProvedorHistorico : IProvedorHistorico
    {
        private static Data.Model.TipoEventoItemIntegracao ObterTipoEvento(TipoItemHistorico tipoItem)
        {
            switch (tipoItem)
            {
                case TipoItemHistorico.Integrando:
                    return Data.Model.TipoEventoItemIntegracao.Integrando;
                case TipoItemHistorico.Integrado:
                    return Data.Model.TipoEventoItemIntegracao.ItemIntegrado;
                case TipoItemHistorico.Informativo:
                    return Data.Model.TipoEventoItemIntegracao.Informativo;
                case TipoItemHistorico.Falha:
                    return Data.Model.TipoEventoItemIntegracao.Falha;
                default:
                    return Data.Model.TipoEventoItemIntegracao.Informativo;
            }
        }

        private static TipoItemHistorico ObterTipoItemHistorico(Data.Model.TipoEventoItemIntegracao tipo)
        {
            switch (tipo)
            {
                case Data.Model.TipoEventoItemIntegracao.Falha:
                    return TipoItemHistorico.Falha;
                case Data.Model.TipoEventoItemIntegracao.Informativo:
                    return TipoItemHistorico.Informativo;
                case Data.Model.TipoEventoItemIntegracao.Integrando:
                    return TipoItemHistorico.Integrando;
                case Data.Model.TipoEventoItemIntegracao.ItemIntegrado:
                    return TipoItemHistorico.Integrado;
                default:
                    return TipoItemHistorico.Informativo;
            }
        }

        private Entidades.ItemIntegracao ObterItemIntegracao(Item item)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ItemIntegracao>()
                .Where("IdEsquemaHistorico=?idEsquemaHistorico AND IdItemEsquemaHistorico=?idItemEsquemaHistorico")
                .Add("?idEsquemaHistorico", item.ItemEsquema.Esquema.Id)
                .Add("?idItemEsquemaHistorico", item.ItemEsquema.Id);

            var index = 0;
            foreach (var identificador in item.ItemEsquema.Identificadores)
            {
                if (identificador.Tipo == typeof(int) || identificador.Tipo == typeof(uint))
                {
                    switch (index)
                    {
                        case 0:
                            consulta.WhereClause
                                .And("IdInteiro1=?idInteiro1")
                                .Add("?idInteiro1", item.Identificadores.ElementAt(index));
                            break;
                        case 1:
                            consulta.WhereClause
                                 .And("IdInteiro2=?idInteiro2")
                                 .Add("?idInteiro2", item.Identificadores.ElementAt(index));
                            break;
                        default:
                            throw new InvalidOperationException("A quantidade de identificador é maior que a suportada.");
                    }
                }
                else
                {
                    consulta.WhereClause
                        .And("IdTextual=?idTextual")
                        .Add("?idTextual", item.Identificadores.ElementAt(index));
                }

                index++;
            }

            return consulta.ProcessResult<Entidades.ItemIntegracao>().FirstOrDefault();
        }

        private Entidades.ItemIntegracao CriarItemIntegracao(Item item)
        {
            var itemIntegracao = SourceContext.Instance.Create<Entidades.ItemIntegracao>();
            itemIntegracao.IdEsquemaHistorico = item.ItemEsquema.Esquema.Id;
            itemIntegracao.IdItemEsquemaHistorico = item.ItemEsquema.Id;
            itemIntegracao.Situacao = Data.Model.SituacaoItemIntegracao.Integrado;
            itemIntegracao.UltimaAtualizacao = DateTime.Now;

            var index = 0;
            foreach (var identificador in item.ItemEsquema.Identificadores)
            {
                if (identificador.Tipo == typeof(int) || identificador.Tipo == typeof(uint))
                {
                    switch (index)
                    {
                        case 0:
                            itemIntegracao.IdInteiro1 = Convert.ToInt32(item.Identificadores.ElementAt(index));
                            break;
                        case 1:
                            itemIntegracao.IdInteiro2 = Convert.ToInt32(item.Identificadores.ElementAt(index));
                            break;
                        default:
                            throw new InvalidOperationException("A quantidade de identificador é maior que a suportada.");
                    }
                }
                else
                {
                    var id = item.Identificadores.ElementAt(index);
                    itemIntegracao.IdTextual = id as string ?? id.ToString();
                }

                index++;
            }

            return itemIntegracao;
        }

        private Entidades.FalhaIntegracao CriarFalha(Falha falha)
        {
            var falha1 = SourceContext.Instance.Create<Entidades.FalhaIntegracao>();
            falha1.Tipo = falha.Tipo;
            falha1.Mensagem = falha.Mensagem;
            falha1.PilhaChamada = falha.PilhaChamada;

            if (falha.FalhaInterna != null)
            {
                falha1.FalhaInterna = this.CriarFalha(falha.FalhaInterna);
            }

            return falha1;
        }

        private Entidades.EventoItemIntegracao CriarEvento(Item item)
        {
            var evento = SourceContext.Instance.Create<Entidades.EventoItemIntegracao>();
            evento.Tipo = ObterTipoEvento(item.Tipo);
            evento.Mensagem = item.Mensagem;
            evento.Data = item.Data;

            if (item.Falha != null)
            {
                evento.Falha = this.CriarFalha(item.Falha);
            }

            return evento;
        }

        /// <inheritdoc />
        public void RegistrarItem(Item item)
        {
            var itemIntegracao = this.ObterItemIntegracao(item) ?? this.CriarItemIntegracao(item);

            itemIntegracao.Situacao = item.Tipo == TipoItemHistorico.Falha ?
                Data.Model.SituacaoItemIntegracao.Falha : Data.Model.SituacaoItemIntegracao.Integrado;
            itemIntegracao.UltimaAtualizacao = DateTime.Now;

            var evento = this.CriarEvento(item);
            itemIntegracao.Eventos.Add(evento);

            SourceContext.Instance.ExecuteSave(itemIntegracao).ThrowInvalid();
        }

        private VirtualListLoaderResult<Item> ProcessarItens(object sender, VirtualListLoaderEventArgs e)
        {
            var info = (ProcessadorItensInfo)e.ReferenceObject;

            if (e.NeedItemsCount)
            {
                return new VirtualListLoaderResult<Item>(null, info.ItensIntegracao.Count());
            }

            var items = info.ItensIntegracao
                .Skip(e.StartRow)
                .Take(e.PageSize)
                .Select(item => item.CriarItem(info.ItemEsquema))
                .ToList();

            return new VirtualListLoaderResult<Item>(items);
        }

        /// <inheritdoc />
        public IEnumerable<Item> ObterItens(ItemEsquema itemEsquema, TipoItemHistorico? tipo, IEnumerable<object> identificadores)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.EventoItemIntegracao>("ev")
                .InnerJoin<Data.Model.ItemIntegracao>("ev.IdItemIntegracao=i.IdItemIntegracao", "i")
                .LeftJoin<Data.Model.FalhaIntegracao>("ev.IdEventoItemIntegracao=f.IdEventoItemIntegracao", "f")
                .Where("i.IdEsquemaHistorico=?idEsquemaHistorico AND i.IdItemEsquemaHistorico=?idItemEsquemaHistorico")
                .Add("?idEsquemaHistorico", itemEsquema.Esquema.Id)
                .Add("?idItemEsquemaHistorico", itemEsquema.Id)
                .Select(@"i.IdInteiro1, i.IdInteiro2, i.IdTextual, ev.Tipo, ev.Mensagem, ev.Data,
                          f.Tipo AS TipoFalha, f.Mensagem AS MensagemFalha, f.PilhaChamada AS PilhaChamadaFalha")
                .OrderBy("Data DESC");

            if (identificadores != null)
            {
                var index = 0;

                using (var enumeradorIdentificadoresEsquema = itemEsquema.Identificadores.GetEnumerator())
                using (var enumeradorIdentificadores = identificadores.GetEnumerator())
                {
                    while (enumeradorIdentificadoresEsquema.MoveNext() && enumeradorIdentificadores.MoveNext())
                    {
                        var identificadorEsquema = enumeradorIdentificadoresEsquema.Current;
                        var identificador = enumeradorIdentificadores.Current;

                        if (identificador == null || (identificador as string) == string.Empty)
                        {
                            continue;
                        }

                        if (identificadorEsquema.Tipo == typeof(int) || identificadorEsquema.Tipo == typeof(uint))
                        {
                            switch (index)
                            {
                                case 0:
                                    consulta
                                        .WhereClause
                                        .And("i.IdInteiro1=?idInteiro1")
                                        .Add("?idInteiro1", Convert.ToInt32(identificador, System.Globalization.CultureInfo.InvariantCulture));
                                    break;
                                case 1:
                                    consulta
                                        .WhereClause
                                       .And("i.IdInteiro2=?idInteiro2")
                                       .Add("?idInteiro2", Convert.ToInt32(identificador, System.Globalization.CultureInfo.InvariantCulture));
                                    break;
                                default:
                                    throw new InvalidOperationException("A quantidade de identificador é maior que a suportada.");
                            }
                        }
                        else
                        {
                            var valor = identificador;
                            var convertible = valor as IConvertible;
                            if (convertible != null)
                            {
                                valor = convertible.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            }

                            consulta
                                .Where("i.IdTextual=?idTextual")
                                .Add("?idTextual", valor?.ToString());
                        }
                    }
                }
            }

            if (tipo.HasValue)
            {
                var tipoEvento = ObterTipoEvento(tipo.Value);

                consulta
                    .WhereClause
                    .And("ev.Tipo=?tipoEvento")
                    .Add("?tipoEvento", tipoEvento);
            }

            var itensIntegracao = consulta
                .ToVirtualResultLazy<ItemInfo>();

            return new VirtualList<Item>(
                20,
                new VirtualListLoaderHandler<Item>(this.ProcessarItens),
                new ProcessadorItensInfo
                {
                    ItemEsquema = itemEsquema,
                    ItensIntegracao = itensIntegracao,
                });
        }

        private class ItemInfo
        {
            public int IdInteiro1 { get; set; }

            public int IdInteiro2 { get; set; }

            public string IdTextual { get; set; }

            public Data.Model.TipoEventoItemIntegracao Tipo { get; set; }

            public string Mensagem { get; set; }

            public DateTime Data { get; set; }

            public string TipoFalha { get; set; }

            public string MensagemFalha { get; set; }

            public string PilhaChamadaFalha { get; set; }

            private IEnumerable<object> ObterIdentificadores(ItemEsquema itemEsquema)
            {
                var identificadores = new List<object>();

                var index = 0;
                foreach (var identificador in itemEsquema.Identificadores)
                {
                    if (identificador.Tipo == typeof(int) || identificador.Tipo == typeof(uint))
                    {
                        switch (index)
                        {
                            case 0:
                                identificadores.Add(this.IdInteiro1);
                                break;
                            case 1:
                                identificadores.Add(this.IdInteiro2);
                                break;
                            default:
                                throw new InvalidOperationException("A quantidade de identificador é maior que a suportada.");
                        }
                    }
                    else
                    {
                        identificadores.Add(this.IdTextual);
                    }

                    index++;
                }

                return identificadores;
            }

            public Item CriarItem(ItemEsquema itemEsquema)
            {
                if (this.Tipo == Glass.Data.Model.TipoEventoItemIntegracao.Falha)
                {
                    Falha falha = null;

                    if (!string.IsNullOrEmpty(this.TipoFalha))
                    {
                        falha = new Falha(this.TipoFalha, this.MensagemFalha, this.PilhaChamadaFalha);
                    }

                    return new Item(itemEsquema, this.Mensagem, falha, this.ObterIdentificadores(itemEsquema), this.Data);
                }
                else
                {
                    return new Item(itemEsquema, ObterTipoItemHistorico(this.Tipo), this.Mensagem, this.ObterIdentificadores(itemEsquema), this.Data);
                }
            }
        }

        private class ProcessadorItensInfo
        {
            public ItemEsquema ItemEsquema { get; set; }

            public IEnumerable<ItemInfo> ItensIntegracao { get; set; }
        }
    }
}
