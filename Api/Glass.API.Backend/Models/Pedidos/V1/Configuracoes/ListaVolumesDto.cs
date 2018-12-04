// <copyright file="ListaVolumesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de pedidos para volume.
    /// </summary>
    [DataContract(Name = "ListaVolumes")]
    public class ListaVolumesDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaVolumesDto"/>.
        /// </summary>
        internal ListaVolumesDto()
        {
            this.ControlarPedidosImportados = OrdemCargaConfig.ControlarPedidosImportados;
            this.SituacoesPedidoVolume = new List<Data.Model.Pedido.SituacaoVolumeEnum>
            {
                Data.Model.Pedido.SituacaoVolumeEnum.SemVolume,
                Data.Model.Pedido.SituacaoVolumeEnum.Pendente,
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com pedidos importados.
        /// </summary>
        [DataMember]
        [JsonProperty("controlarPedidosImportados")]
        public bool ControlarPedidosImportados { get; set; }

        /// <summary>
        /// Obtém ou define as situações de volume a serem filtradas por padrão na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("situacoesPedidoVolume")]
        public IEnumerable<Data.Model.Pedido.SituacaoVolumeEnum> SituacoesPedidoVolume { get; set; }
    }
}
