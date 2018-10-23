// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Glass.Fiscal.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um CFOP para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Cfop")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="cfop">A model de CFOP's.</param>
        internal ListaDto(CfopPesquisa cfop)
        {
            this.Id = cfop.IdCfop;
            this.Nome = cfop.Descricao;
            this.Codigo = cfop.CodInterno;
            this.IdTipoCfop = cfop.IdTipoCfop;
            this.TipoMercadoria = cfop.TipoMercadoria;
            this.AlterarEstoqueTerceiros = cfop.AlterarEstoqueTerceiros;
            this.AlterarEstoqueCliente = cfop.AlterarEstoqueCliente;
            this.Obs = cfop.Obs;
        }

        /// <summary>
        /// Obtém ou define o código do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCfop")]
        public int? IdTipoCfop { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoMercadoria")]
        public TipoMercadoria? TipoMercadoria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque de terceiros deve ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueTerceiros")]
        public bool AlterarEstoqueTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque do cliente deve ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueCliente")]
        public bool AlterarEstoqueCliente { get; set; }

        /// <summary>
        /// Obtém ou define a observação do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("obs")]
        public string Obs { get; set; }
    }
}
