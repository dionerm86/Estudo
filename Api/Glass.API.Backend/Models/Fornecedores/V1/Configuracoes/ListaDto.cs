// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Fornecedores.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de fornecedores.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarFornecedor = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFornecedor);
            this.UsarCreditoFornecedor = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            this.AtivarInativarFornecedor = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarFornecedor);
            this.AnexarArquivosFornecedor = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosFornecedor);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode cadastrar/editar/exluir fornecedores.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarFornecedor")]
        public bool CadastrarFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com crédito de fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("usarCreditoFornecedor")]
        public bool UsarCreditoFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode ativar/inativar fornecedores.
        /// </summary>
        [DataMember]
        [JsonProperty("ativarInativarFornecedor")]
        public bool AtivarInativarFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode anexar arquivos aos fornecedores.
        /// </summary>
        [DataMember]
        [JsonProperty("anexarArquivosFornecedor")]
        public bool AnexarArquivosFornecedor { get; set; }
    }
}
