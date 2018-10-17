// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Clientes;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Clientes.V1.SugestoesCliente.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma sugestão de cliente para a tela de listagem.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaSugestoesClientes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da Sugestão.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o nome do funcionário.
        /// </summary>
        [JsonProperty("nomeFuncionario")]
        public string NomeFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de cadastro do cliente.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de cadastro do cliente.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo da Sugestão.
        /// </summary>
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da sugestão.
        /// </summary>
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a situação da situação.
        /// </summary>
        [JsonProperty("situacao")]
        public int? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota.
        /// </summary>
        [JsonProperty("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        [JsonProperty("idOrcamento")]
        public uint? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do endedor Associado.
        /// </summary>
        [JsonProperty("idVendedorAssociado")]
        public int? IdVendedorAssociado { get; set; }

    }
}
