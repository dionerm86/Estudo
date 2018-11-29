// <copyright file="PermisoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de entrega do pedido.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermisoesDto : BaseCadastroAtualizacaoDto<PermisoesDto>
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário pode utilizar o chat.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizarChat")]
        public bool UtilizarChat
        {
            get { return this.ObterValor(c => c.UtilizarChat); }
            set { this.AdicionarValor(c => c.UtilizarChat, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário pode acessar o controle de usuários.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarControleUsuarios")]
        public bool HabilitarControleUsuarios
        {
            get { return this.ObterValor(c => c.HabilitarControleUsuarios); }
            set { this.AdicionarValor(c => c.HabilitarControleUsuarios, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve enviar e-mail ao finalizar PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmailPedidoConfirmadoVendedor")]
        public bool EnviarEmailPedidoConfirmadoVendedor
        {
            get { return this.ObterValor(c => c.EnviarEmailPedidoConfirmadoVendedor); }
            set { this.AdicionarValor(c => c.EnviarEmailPedidoConfirmadoVendedor, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário é administrador sync.
        /// </summary>
        [DataMember]
        [JsonProperty("adminSync")]
        public bool AdminSync
        {
            get { return this.ObterValor(c => c.AdminSync); }
            set { this.AdicionarValor(c => c.AdminSync, value); }
        }
    }
}
