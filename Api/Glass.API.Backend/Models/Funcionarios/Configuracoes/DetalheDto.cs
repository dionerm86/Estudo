// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para tela de cadastro de funcionário.
    /// </summary>
    [DataContract(Name = "Detalhe")]
    public class DetalheDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        internal DetalheDto()
        {
            var permissao = Glass.Data.Helper.UserInfo.GetUserInfo != null && Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync;

            this.HabilitarChat = permissao;
            this.HabilitarControleUsuarios = permissao;
            this.EnviarEmailPedidoConfirmado = Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o ICMS é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarChat")]
        public bool HabilitarChat { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o IPI é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmailPedidoConfirmado")]
        public bool EnviarEmailPedidoConfirmado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa não vende vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarControleUsuarios")]
        public bool HabilitarControleUsuarios { get; set; }
    }
}
