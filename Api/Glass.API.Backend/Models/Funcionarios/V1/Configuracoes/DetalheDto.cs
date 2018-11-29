// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Configuracoes
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
            var permissao = UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IsAdminSync;

            this.HabilitarChat = permissao;
            this.HabilitarControleUsuarios = permissao;
            this.EnviarEmailPedidoConfirmado = Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmadoVendedor;
            this.PodeCadastrarFuncionario = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario);
            this.IdsTiposFuncionariosComSetor = new int[] { 196 };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o ICMS é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarChat")]
        public bool HabilitarChat { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve enviar email ao finalizar PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmailPedidoConfirmado")]
        public bool EnviarEmailPedidoConfirmado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário pode habilitar o controle de usuários.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarControleUsuarios")]
        public bool HabilitarControleUsuarios { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode cadastrar funcionários.
        /// </summary>
        [DataMember]
        [JsonProperty("podeCadastrarFuncionario")]
        public bool PodeCadastrarFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os valores dos tipos de funcionário que possuem setor.
        /// </summary>
        [DataMember]
        [JsonProperty("idsTiposFuncionariosComSetor")]
        public IEnumerable<int> IdsTiposFuncionariosComSetor { get; set; }
    }
}
