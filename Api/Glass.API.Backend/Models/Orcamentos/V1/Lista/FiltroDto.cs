// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Orcamentos;
using Glass.API.Backend.Models.Genericas.V1;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Orcamentos.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de orçamentos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaOrcamentos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor.
        /// </summary>
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define o telefone do orçamento.
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cidade do orçamento.
        /// </summary>
        public int? IdCidade { get; set; }

        /// <summary>
        /// Obtém ou define o nome do bairro do orçamento.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do orçamento.
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Obtém ou define o complemento do endereço do orçamento.
        /// </summary>
        public string Complemento { get; set; }

        /// <summary>
        /// Obtém ou define a situação do orçamento.
        /// </summary>
        public IEnumerable<Data.Model.Orcamento.SituacaoOrcamento> Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja emitente.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de cadastro do pedido.
        /// </summary>
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de cadastro do pedido.
        /// </summary>
        public DateTime? PeriodoCadastroFim { get; set; }
    }
}
