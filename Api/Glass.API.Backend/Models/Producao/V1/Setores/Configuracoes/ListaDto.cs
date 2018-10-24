// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de setores.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.UsarControleCavalete = PCPConfig.ControleCavalete;
            this.UsarGerenciamentoFornada = PCPConfig.GerenciamentoFornada;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com controle de cavalete.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleCavalete")]
        public bool UsarControleCavalete { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com gerenciamento de fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("usarGerenciamentoFornada")]
        public bool UsarGerenciamentoFornada { get; set; }
    }
}
