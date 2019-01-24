// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Individual.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de impressões individuais de etiquetas.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.PossuiPermissaoImprimirEtiquetas = Config.PossuiPermissao(Config.FuncaoMenuPCP.ReimprimirEtiquetas) || UserInfo.GetUserInfo.IsAdministrador;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário possui permissão para imprimir etiquetas.
        /// </summary>
        [JsonProperty("possuiPermissaoImprimirEtiquetas")]
        public bool PossuiPermissaoImprimirEtiquetas { get; set; }
    }
}