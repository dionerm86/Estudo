// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Helper;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de processos de etiqueta.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListaDto"/> class.
        /// </summary>
        public ListaDto()
        {
            this.TiposPedidosIgnorar = new[]
            {
                Data.Model.Pedido.TipoPedidoEnum.Revenda,
            };

            this.SituacaoPadrao = new ConversorEnum<Glass.Situacao>()
                .ObterTraducao()
                .FirstOrDefault(s => s.Id == (int)Glass.Situacao.Ativo);
        }

        /// <summary>
        /// Obtém ou define os tipos de pedido que serão ignorados.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidosIgnorar")]
        public Data.Model.Pedido.TipoPedidoEnum[] TiposPedidosIgnorar { get; set; }

        /// <summary>
        /// Obtém ou define a situação padrão para o cadastro de processo.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoPadrao")]
        public IdNomeDto SituacaoPadrao { get; set; }
    }
}
