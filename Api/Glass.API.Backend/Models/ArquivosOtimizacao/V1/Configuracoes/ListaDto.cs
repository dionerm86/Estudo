// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Web;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de arquivos de otimização.
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
        /// Obtém ou define o endereço do ECutter.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoECutter")]
        public string EnderecoECutter { get; set; }

        private string ObterEnderecoECutter()
        {
            var endereco = HttpContext.Current.Request.Url.AbsoluteUri;

            endereco = endereco.Substring(0, endereco.IndexOf("/backend", StringComparison.InvariantCultureIgnoreCase));
            var token = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value;

            var uri = new Uri($"{endereco}/handlers/ecutteroptimizationservice.ashx?token={token}&id=");

            return $"ecutter-opt{uri.AbsoluteUri.Substring(uri.Scheme.Length)}";
        }
    }
}
