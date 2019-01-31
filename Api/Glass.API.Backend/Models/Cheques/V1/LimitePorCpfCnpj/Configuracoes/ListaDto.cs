// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;
using Glass.Data.Helper;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de limites de cheques por cpf/cnpj.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.AlterarLimiteDeChequesPorCpfCnpj = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) || UserInfo.GetUserInfo.IsCaixaDiario;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá alterar o valor do limite de cheques.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarLimiteDeChequesPorCpfCnpj")]
        public bool AlterarLimiteDeChequesPorCpfCnpj { get; set; }
    }
}