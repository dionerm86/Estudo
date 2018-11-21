// <copyright file="ListaVolumesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
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
            this.SituacaoSemVolume = Data.Model.Pedido.SituacaoVolumeEnum.SemVolume;
            this.SituacaoPendente = Data.Model.Pedido.SituacaoVolumeEnum.Pendente;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com pedidos importados.
        /// </summary>
        [DataMember]
        [JsonProperty("controlarPedidosImportados")]
        public bool ControlarPedidosImportados { get; set; }

        /// <summary>
        /// Obtém ou define o valor da situação de volume "Sem volume".
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoSemVolume")]
        public Data.Model.Pedido.SituacaoVolumeEnum SituacaoSemVolume { get; set; }

        /// <summary>
        /// Obtém ou define o valor da situação de volume "Pendente".
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoPendente")]
        public Data.Model.Pedido.SituacaoVolumeEnum SituacaoPendente { get; set; }
    }
}
