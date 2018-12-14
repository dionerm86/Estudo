// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Fiscais.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de movimentações do estoque fiscal.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CodigoTabela = (int)LogCancelamento.TabelaCancelamento.MovEstoqueFiscal;
        }

        /// <summary>
        /// Obtém ou define o código da tabela de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoTabela")]
        public int CodigoTabela { get; set; }
    }
}