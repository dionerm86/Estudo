﻿// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Glass.Data.Helper.Utils;

namespace Glass.API.Backend.Models.Funcionarios.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de funcionários.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.AlterarFuncionario = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario);
            this.IdsTiposFuncionariosComSetor = new[] { TipoFuncionario.MarcadorProducao };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de inserir funcionários.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarFuncionario")]
        public bool AlterarFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de editar funcionários.
        /// </summary>
        [DataMember]
        [JsonProperty("idsTiposFuncionariosComSetor")]
        public IEnumerable<TipoFuncionario> IdsTiposFuncionariosComSetor { get; set; }
    }
}
