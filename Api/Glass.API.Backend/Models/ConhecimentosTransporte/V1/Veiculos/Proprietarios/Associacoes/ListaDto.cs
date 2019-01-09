// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Associacoes
{
    /// <summary>
    /// Classe que encapsula um item para a lista de associação de proprietários e veículos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="proprietarioVeiculo">A model da associação de proprietário com veículo que será retornada.</param>
        public ListaDto(Data.Model.Cte.ProprietarioVeiculo_Veiculo associacaoProprietarioVeiculoAVeiculo)
        {
            this.Proprietario = new IdNomeDto
            {
                Id = (int)associacaoProprietarioVeiculoAVeiculo.IdPropVeic,
                Nome = associacaoProprietarioVeiculoAVeiculo.Nome,
            };

            this.PlacaVeiculo = associacaoProprietarioVeiculoAVeiculo.Placa;
        }

        /// <summary>
        /// Obtém ou define os dados referentes ao proprietário do veículo.
        /// </summary>
        [JsonProperty("proprietario")]
        public IdNomeDto Proprietario { get; set; }

        /// <summary>
        /// Obtém ou define a placa do veículo associado ao proprietário.
        /// </summary>
        [JsonProperty("placaVeiculo")]
        public string PlacaVeiculo { get; set; }
    }
}