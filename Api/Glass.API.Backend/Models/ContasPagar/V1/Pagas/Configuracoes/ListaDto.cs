// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de contas pagas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.GerarArquivoGCon = FinanceiroConfig.GerarArquivoGCon;
            this.GerarArquivoProsoft = FinanceiroConfig.GerarArquivoProsoft;
            this.GerarArquivoDominio = FinanceiroConfig.GerarArquivoDominio;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o link para geração do arquivo GCON estará disponível.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoGCon")]
        public bool? GerarArquivoGCon { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o link para geração do arquivo PROSOFT estará disponível.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoProsoft")]
        public bool? GerarArquivoProsoft { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o link para geração do arquivo de exportação para domínio sistemas estará disponível.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDominio")]
        public bool? GerarArquivoDominio { get; set; }
    }
}