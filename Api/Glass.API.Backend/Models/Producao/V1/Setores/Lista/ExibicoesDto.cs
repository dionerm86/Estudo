// <copyright file="ExibicoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula visibilidade do setor nas telas do sistema.
    /// </summary>
    [DataContract(Name = "Exibicoes")]
    public class ExibicoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se os setores que a peças deve passar serão exibidos ao efetuar a leitura na produção.
        /// </summary>
        [DataMember]
        [JsonProperty("setoresLeituraPeca")]
        public bool SetoresLeituraPeca { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido na consulta de produção e nos relatórios.
        /// </summary>
        [DataMember]
        [JsonProperty("listaERelatorio")]
        public bool ListaERelatorio { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido no painel comercial.
        /// </summary>
        [DataMember]
        [JsonProperty("painelComercial")]
        public bool PainelComercial { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido no painel de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("painelProducao")]
        public bool PainelProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se ao ler peças neste setor, será exibida a imagem completa do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("imagemCompleta")]
        public bool ImagemCompleta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se ao ler peças neste setor, será feita uma consulta antes da leitura.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarAntesDaLeitura")]
        public bool ConsultarAntesDaLeitura { get; set; }
    }
}
