// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de impressões de etiquetas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.EnderecoECutter = this.ObterEnderecoECutter();
        }

        /// <summary>
        /// Obtém ou define o endereço do e-Cutter.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoECutter")]
        public string EnderecoECutter { get; set; }

        private string ObterEnderecoECutter()
        {
            var address = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;

            address = address.Substring(0, address.IndexOf("/backend", System.StringComparison.InvariantCultureIgnoreCase));
            var token = System.Web.HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value;

            var uri = new System.Uri($"{address}/handlers/ecutteroptimizationservice.ashx?token={token}&id=");

            return $"ecutter-opt{uri.AbsoluteUri.Substring(uri.Scheme.Length)}";
        }
    }
}
