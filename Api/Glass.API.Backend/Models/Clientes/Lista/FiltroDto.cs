// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Clientes;
using Glass.API.Backend.Models.Genericas;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Clientes.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de clientes.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaClientes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o Cpf/Cnpj do cliente.
        /// </summary>
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do cliente.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o telefone do cliente.
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define a UF do cliente.
        /// </summary>
        public string Uf { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cidade do cliente.
        /// </summary>
        public int? IdCidade { get; set; }

        /// <summary>
        /// Obtém ou define o nome do bairro do cliente.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do cliente.
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do cliente.
        /// </summary>
        public int[] Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cliente.
        /// </summary>
        public IEnumerable<Data.Model.SituacaoCliente> Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o código da rota do cliente.
        /// </summary>
        public string CodigoRota { get; set; }

        /// <summary>
        /// Obtém ou define o vendedor do cliente.
        /// </summary>
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define o tipo fiscal do cliente.
        /// </summary>
        public IEnumerable<Data.Model.TipoFiscalCliente> TipoFiscal { get; set; }

        /// <summary>
        /// Obtém ou define as formas de pagamento que serão filtradas por cliente.
        /// </summary>
        public int[] FormasPagamento { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de cadastro do cliente.
        /// </summary>
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de cadastro do cliente.
        /// </summary>
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a data incial que o cliente está sem comprar.
        /// </summary>
        public DateTime? PeriodoSemCompraInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final que o cliente está sem comprar.
        /// </summary>
        public DateTime? PeriodoSemCompraFim { get; set; }

        /// <summary>
        /// Obtém ou define a data incial que o cliente foi inativado.
        /// </summary>
        public DateTime? PeriodoInativadoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final que o cliente foi inativado.
        /// </summary>
        public DateTime? PeriodoInativadoFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da tabela de desconto do cliente.
        /// </summary>
        public int? IdTabelaDesconto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscados apenas clientes sem rota.
        /// </summary>
        public bool ApenasSemRota { get; set; }
    }
}
