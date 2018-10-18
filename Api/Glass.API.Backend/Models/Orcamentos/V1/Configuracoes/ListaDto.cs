// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Orcamentos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de orçamentos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarOrcamento = Config.PossuiPermissao(Config.FuncaoMenuOrcamento.EmitirOrcamento);
            this.ExibirColunaIdPedidoEspelho = LojaDAO.Instance.GetCount() > 1;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de cadastrar orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarOrcamento")]
        public bool CadastrarOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna com o id do pedido espelho na listagem.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaIdPedidoEspelho")]
        public bool ExibirColunaIdPedidoEspelho { get; set; }
    }
}
