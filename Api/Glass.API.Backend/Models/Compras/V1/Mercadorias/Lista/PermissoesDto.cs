// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Mercadorias.Lista
{
    /// <summary>
    /// Classe que encapsula as permissões para a lista da tela de compras de mercadorias.
    /// </summary>
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se pode ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode gerenciar as fotos.
        /// </summary>
        [DataMember]
        [JsonProperty("gerenciarFotos")]
        public bool GerenciarFotos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser gerada a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarNotaFiscal")]
        public bool GerarNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser reaberta.
        /// </summary>
        [DataMember]
        [JsonProperty("reabrir")]
        public bool Reabrir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto chegou.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirProdutoChegou")]
        public bool ExibirProdutoChegou { get; set; }
    }
}
