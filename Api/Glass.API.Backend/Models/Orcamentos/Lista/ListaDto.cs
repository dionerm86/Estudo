// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Orcamentos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um orçamento para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Orcamento")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="orcamento">A model de orçamentos.</param>
        internal ListaDto(Data.Model.Orcamento orcamento)
        {
            this.Id = (int)orcamento.IdOrcamento;
            this.IdProjeto = (int?)orcamento.IdProjeto;
            this.IdPedidoEspelho = (int?)orcamento.IdPedidoEspelho;
            this.IdsMedicao = !string.IsNullOrEmpty(orcamento.IdsMedicao) ? orcamento.IdsMedicao.Split(',').Select(f => f.StrParaInt()) : new List<int>();
            this.Cliente = new IdNomeDto
            {
                Id = (int?)orcamento.IdCliente,
                Nome = orcamento.NomeCliente,
            };

            this.Vendedor = new IdNomeDto
            {
                Id = (int?)orcamento.IdFuncionario,
                Nome = orcamento.NomeFuncAbrv,
            };

            this.TelefoneOrcamento = orcamento.TelCliente;
            this.Total = orcamento.Total;
            this.DataCadastro = orcamento.DataCad;
            this.Situacao = orcamento.DescrSituacao;

            this.Permissoes = new PermissoesDto
            {
                Editar = orcamento.EditVisible,
                Imprimir = orcamento.ExibirImpressao,
                ImprimirMemoriaCalculo = orcamento.ExibirRelatorioCalculo,
                ImprimirProjeto = orcamento.ExibirImpressaoProjeto,
                CadastrarSugestao = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes),
                GerarPedido = orcamento.GerarPedidoVisible,
                EnviarEmail = OrcamentoConfig.MostrarIconeEnvioEmailListagem,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Orcamento, orcamento.IdOrcamento, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do projeto, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("idProjeto")]
        public int? IdProjeto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido espelho, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoEspelho")]
        public int? IdPedidoEspelho { get; set; }

        /// <summary>
        /// Obtém ou define a lista de medições associadas ao orçamento, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("idsMedicao")]
        public IEnumerable<int> IdsMedicao { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("vendedor")]
        public IdNomeDto Vendedor { get; set; }

        /// <summary>
        /// Obtém ou define o telefone do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneOrcamento")]
        public string TelefoneOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define o total do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
