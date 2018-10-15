// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.SugestoesCliente.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de sugestões.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarSugestoesCliente = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de cadastrar sugestões de clientes.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarSugestaoCliente")]
        public bool CadastrarSugestoesCliente { get; set; }
    }
}
