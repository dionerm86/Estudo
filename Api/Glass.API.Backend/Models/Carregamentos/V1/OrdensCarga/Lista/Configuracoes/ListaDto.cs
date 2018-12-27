// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de ordens de carga.
    /// </summary>
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ControlarPedidosImportados = Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se no carregamento serão controlados os pedidos importados de outro sistema.
        /// </summary>
        [DataMember]
        [JsonProperty("controlarPedidosImportados")]
        public bool ControlarPedidosImportados { get; set; }
    }
}