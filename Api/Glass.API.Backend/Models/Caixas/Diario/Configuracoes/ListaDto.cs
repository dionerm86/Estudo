// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de movimentações do caixa diário.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ControleFinanceiroRecebimento = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
            this.ControleFinanceiroPagamento = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
            this.AlterarDataConsulta = FinanceiroConfig.TelaFechamentoCaixaDiario.PermitirCaixaAlterarDataConsulta;
            this.ControleCaixaDiario = Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario);
            this.UsuarioAdministrador = UserInfo.GetUserInfo.IsAdministrador;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem acesso às funções de recebimento do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("controleFinanceiroRecebimento")]
        public bool ControleFinanceiroRecebimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem acesso às funções de pagamento do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("controleFinanceiroPagamento")]
        public bool ControleFinanceiroPagamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de alterar a data da consulta.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarDataConsulta")]
        public bool AlterarDataConsulta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem acesso às funções de caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("controleCaixaDiario")]
        public bool ControleCaixaDiario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário é administrador.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioAdministrador")]
        public bool UsuarioAdministrador { get; set; }
    }
}
