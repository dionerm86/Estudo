// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Geral.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de movimentações do caixa geral.
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
            this.DescricaoContaContabil = FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil;
            this.DescricaoContaNaoContabil = FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil;
            this.ExibirTotalCumulativo = FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo;
            this.PermitirFiltrarFuncionario = !FinanceiroConfig.FinanceiroRec.ApenasFinancGeralAdminSelFuncCxGeral || (this.ControleFinanceiroRecebimento && this.ControleFinanceiroPagamento);
            this.ExibirInformacoesContasRecebidas = FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber;
            this.LojaUsuario = new IdNomeDto
            {
                Id = (int)UserInfo.GetUserInfo.IdLoja,
                Nome = UserInfo.GetUserInfo.NomeLoja,
            };

            this.Usuario = new IdNomeDto
            {
                Id = (int)UserInfo.GetUserInfo.CodUser,
                Nome = UserInfo.GetUserInfo.Nome,
            };
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
        /// Obtém ou define a descrição a ser adicionada no tooltip do totalizador de contas contábeis.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContaContabil")]
        public string DescricaoContaContabil { get; set; }

        /// <summary>
        /// Obtém ou define a descrição a ser adicionada no tooltip do totalizador de contas não contábeis.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContaNaoContabil")]
        public string DescricaoContaNaoContabil { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os totais cumulativos em dinheiro e cheque serão exibidos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirTotalCumulativo")]
        public bool ExibirTotalCumulativo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido utilizar o filtro por funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirFiltrarFuncionario")]
        public bool PermitirFiltrarFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão exibidas informações de contas recebidas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirInformacoesContasRecebidas")]
        public bool ExibirInformacoesContasRecebidas { get; set; }

        /// <summary>
        /// Obtém ou define a loja do usuário logado.
        /// </summary>
        [DataMember]
        [JsonProperty("lojaUsuario")]
        public IdNomeDto LojaUsuario { get; set; }

        /// <summary>
        /// Obtém ou define o usuário logado.
        /// </summary>
        [DataMember]
        [JsonProperty("Usuario")]
        public IdNomeDto Usuario { get; set; }
    }
}
