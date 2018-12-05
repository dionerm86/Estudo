// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de carregamentos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.UsarFaturamentoCarregamento = Glass.Configuracoes.PCPConfig.HabilitarFaturamentoCarregamento;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá utilizar a função faturamento na lista de carregamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarFaturamentoCarregamento")]
        public bool UsarFaturamentoCarregamento { get; set; }
    }
}
