﻿using System;
using System.Linq;
using System.ComponentModel.Composition;
using Colosoft;
using Colosoft.Reports;

namespace Glass.Global.Relatorios.Clientes
{
    /// <summary>
    /// Representa o relatório da lista de clientes.
    /// </summary>
    [ExportaRelatorio("ListaClientes")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaClientes : ReportDocument
    {
        #region Variáveis Locais

        private Negocios.IClienteFluxo _clienteFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do cliente.
        /// </summary>
        [ReportDataSourceParameter("idCli")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Nome/apelido do cliente.
        /// </summary>
        [ReportDataSourceParameter("nome")]
        public string NomeOuApelido { get; set; }

        /// <summary>
        /// CPF/CNPJ do cliente.
        /// </summary>
        [ReportDataSourceParameter("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Código da loja do cliente.
        /// </summary>
        [ReportDataSourceParameter("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        [ReportDataSourceParameter("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Logradouro no endereço do cliente.
        /// </summary>
        [ReportDataSourceParameter("endereco")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Bairro no endereço do cliente.
        /// </summary>
        [ReportDataSourceParameter("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Código da cidade do cliente.
        /// </summary>
        [ReportDataSourceParameter("idCidade")]
        public int? IdCidade { get; set; }

        /// <summary>
        /// Código do tipo de cliente.
        /// </summary>
        [ReportDataSourceParameter("idTipoCliente")]
        public string IdTipoCliente { get; set; }

        /// <summary>
        /// Situação do cliente.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Código da rota do cliente;
        /// </summary>
        [ReportDataSourceParameter("codRota")]
        public string CodigoRota { get; set; }

        /// <summary>
        /// Código do vendedor associado ao cliente.
        /// </summary>
        [ReportDataSourceParameter("idFunc")]
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Tipos fiscais.
        /// </summary>
        [ReportDataSourceParameter("tipoFiscal")]
        public string TiposFiscais { get; set; }

        /// <summary>
        /// Formas Pagto.
        /// </summary>
        [ReportDataSourceParameter("formasPagto")]
        public string FormasPagto { get; set; }

        /// <summary>
        /// Data inicial de cadastro do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataCadIni")]
        public DateTime? DataCadastroIni { get; set; }

        /// <summary>
        /// Data final de cadastro do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataCadFim")]
        public DateTime? DataCadastroFim { get; set; }

        /// <summary>
        /// Data inicial de período de compra do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataSemCompraIni")]
        public DateTime? DataSemCompraIni { get; set; }

        /// <summary>
        /// Data final de período de compra do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataSemCompraFim")]
        public DateTime? DataSemCompraFim { get; set; }

        /// <summary>
        /// Data inicial de inativação do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataInativadoIni")]
        public DateTime? DataInativadoIni { get; set; }

        /// <summary>
        /// Data final de inativação do cliente.
        /// </summary>
        [ReportDataSourceParameter("dataInativadoFim")]
        public DateTime? DataInativadoFim { get; set; }

        /// <summary>
        /// Código da tabela de desconto/acréscimo do cliente.
        /// </summary>
        [ReportDataSourceParameter("idTabelaDesconto")]
        public int? IdTabelaDescontoAcrescimo { get; set; }

        /// <summary>
        /// Buscar apenas clientes sem rota?
        /// </summary>
        [ReportDataSourceParameter("apenasSemRota")]
        public bool ApenasSemRota { get; set; }
        
        /// <summary>
        /// Buscar apenas clientes com total comprado maior que zero?
        /// </summary>
        [ReportDataSourceParameter("comCompra")]
        public bool ComCompra { get; set; }

        /// <summary>
        /// Agrupar dados por vendedor?
        /// </summary>
        [ReportDataSourceParameter("agruparVend")]
        public bool AgruparVendedor { get; set; }

        /// <summary>
        /// Exibir o historico do cliente na listagem
        /// </summary>
        [ReportDataSourceParameter("exibirHistorico")]
        public bool ExibirHistorico { get; set; }

        /// <summary>
        /// Limite do cliente
        /// 1 - Acima do limite
        /// 2 - Dentro do limite
        /// 3 - Sem limite cadastrado
        /// </summary>
        [ReportDataSourceParameter("limite")]
        public int Limite { get; set; }
        
        [ReportDataSourceParameter("uf")]
        public string Uf { get; set; }
        
        [ReportDataSourceParameter("tipoPessoa")]
        public string TipoPessoa { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="clienteFluxo"></param>
        [ImportingConstructor]
        public ListaClientes(Negocios.IClienteFluxo clienteFluxo)
            : base("ListaClientes")
        {
            _clienteFluxo = clienteFluxo;
        }

        #endregion

        #region Métodos Protegidos
        public override System.IO.Stream GetDefinition()
        {
            var relatorio =
                RepositorioRelatorios.RecuperarArquivoRelatorio("", string.Format("rptListaClientes{0}.rdlc", System.Configuration.ConfigurationManager.AppSettings["sistema"]));

            if (relatorio != null)
                return relatorio;
            else
                return RepositorioRelatorios.RecuperarArquivoRelatorio("", "rptListaClientes.rdlc");
        }       

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var tiposFiscais = String.IsNullOrEmpty(TiposFiscais) ? null :
                TiposFiscais.Split(',')
                    .Select(x => (Data.Model.TipoFiscalCliente)x.StrParaInt())
                    .ToArray();

            var formasPagto = String.IsNullOrEmpty(FormasPagto) ? null :
                FormasPagto.Split(',')
                    .Select(x => x.StrParaInt())
                    .ToArray();

            var situacao = String.IsNullOrEmpty(Situacao) ? null :
                Situacao.Split(',')
                    .Select(x => x.StrParaInt())
                    .ToArray();

            var tiposCliente = String.IsNullOrEmpty(IdTipoCliente) ? null :
                IdTipoCliente.Split(',')
                    .Select(x => x.StrParaInt())
                    .ToArray();

            var clientes = _clienteFluxo.PesquisarClientes(IdCliente, NomeOuApelido, CpfCnpj, IdLoja, Telefone,
                Logradouro, Bairro, IdCidade, tiposCliente, situacao, CodigoRota, IdVendedor, tiposFiscais, formasPagto,
                DataCadastroIni, DataCadastroFim, DataSemCompraIni, DataSemCompraFim, DataInativadoIni, DataInativadoFim, 
                IdTabelaDescontoAcrescimo, ApenasSemRota, Limite, Uf, TipoPessoa, ComCompra);

            // Recupera o critério da pesquisa
            Parameters.Add("Criterio", clientes.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            Parameters.Add("AgruparVend", AgruparVendedor);
            Parameters.Add("ExibirHistorico", ExibirHistorico);
            Parameters.Add("ExibirCidade", true);

            DataSources.Add(new ReportDataSource("Cliente", clientes));
        }

        #endregion
    }
}
