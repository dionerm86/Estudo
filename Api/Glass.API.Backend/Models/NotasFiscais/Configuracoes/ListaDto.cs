﻿// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de notas fiscais.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.AtivarContingencia = Config.PossuiPermissao(Config.FuncaoMenuFiscal.AtivarContingenciaNFe);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário logado possui permissão de ativar contingência de nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("ativarContingencia")]
        public bool AtivarContingencia { get; set; }
    }
}
