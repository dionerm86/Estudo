// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de processos de etiqueta.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.TiposPedidosIgnorar = this.ObterTiposPedidosIgnorar();
            this.SituacaoPadrao = new ConversorEnum<Glass.Situacao>()
                .ObterTraducao()
                .FirstOrDefault(s => s.Id == (int)Glass.Situacao.Ativo);
        }

        /// <summary>
        /// Obtém ou define os tipos de pedido que serão ignorados.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidosIgnorar")]
        public IEnumerable<Data.Model.Pedido.TipoPedidoEnum> TiposPedidosIgnorar { get; set; }

        /// <summary>
        /// Obtém ou define a situação padrão para o cadastro de processo.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoPadrao")]
        public IdNomeDto SituacaoPadrao { get; set; }

        private IList<Data.Model.Pedido.TipoPedidoEnum> ObterTiposPedidosIgnorar()
        {
            var tiposPedidosIgnorar = new List<Data.Model.Pedido.TipoPedidoEnum>
            {
                Data.Model.Pedido.TipoPedidoEnum.Revenda,
            };

            if (!EstoqueConfig.ControlarEstoqueVidrosClientes)
            {
                tiposPedidosIgnorar.Add(Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial);
            }

            return tiposPedidosIgnorar;
        }
    }
}
