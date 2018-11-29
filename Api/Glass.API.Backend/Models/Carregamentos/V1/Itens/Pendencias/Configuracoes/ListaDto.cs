// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Itens.Pendencias.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de pendencias de carregamentos.
    /// </summary>
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.UsarOpcaoDeveTransferir = Glass.Configuracoes.PedidoConfig.ExibirOpcaoDeveTransferir;
            this.ControlarPedidosImportados = Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados;
        }

        [DataMember]
        [JsonProperty("usarOpcaoDeveTransferir")]
        public bool UsarOpcaoDeveTransferir { get; set; }

        [DataMember]
        [JsonProperty("controlarPedidosImportados")]
        public bool ControlarPedidosImportados { get; set; }
    }
}
