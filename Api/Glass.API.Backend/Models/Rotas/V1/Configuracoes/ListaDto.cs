// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Rotas.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de rotas.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.CadastrarRota = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarRota);
            this.UsarDiasCorridosCalculoRota = RotaConfig.UsarDiasCorridosCalculoRota;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cadastrar rotas.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarRota")]
        public bool CadastrarRota { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão usados dias corridos no cálculo da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDiasCorridosCalculoRota")]
        public bool UsarDiasCorridosCalculoRota { get; set; }
    }
}
