// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Veiculos.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de veículos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarVeiculo = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarVeiculo);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode cadastrar/editar/exluir veículos.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarVeiculo")]
        public bool CadastrarVeiculo { get; set; }
    }
}
