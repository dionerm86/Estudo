// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de ferragens.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.CadastrarFerragem = PCPConfig.ControleCavalete;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode cadastrar ferragens.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarFerragem")]
        public bool CadastrarFerragem { get; set; }
    }
}
