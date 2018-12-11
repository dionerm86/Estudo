// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.TrocasDevolucoes.Configuracoes
{
    /// <summary>
    /// Classe com os dados para as configurações da tela de listagem de trocas/devoluções.
    /// </summary>
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.CadastrarTrocaDevolucao = Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é possível cadastrar troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarTrocaDevolucao")]
        public bool CadastrarTrocaDevolucao { get; set; }
    }
}
