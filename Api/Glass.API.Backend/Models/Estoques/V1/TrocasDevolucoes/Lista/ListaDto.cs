// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.TrocasDevolucoes.Lista
{
    /// <summary>
    /// Classe com o DTO do serviço de trocas/devoluções.
    /// </summary>
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="trocaDevolucao">O objeto com os dados da troca/devolução do banco de dados.</param>
        internal ListaDto(TrocaDevolucao trocaDevolucao)
        {
            this.Id = (int)trocaDevolucao.IdTrocaDevolucao;
            this.IdPedido = (int?)trocaDevolucao.IdPedido;
            this.Funcionario = trocaDevolucao.NomeFunc;
            this.Loja = trocaDevolucao.Loja;
            this.Cliente = new IdNomeDto
            {
                Id = (int)trocaDevolucao.IdCliente,
                Nome = trocaDevolucao.NomeCliente,
            };

            this.Tipo = trocaDevolucao.DescrTipo;
            this.DataTrocaDevolucao = trocaDevolucao.DataTroca;
            this.DataErro = trocaDevolucao.DataErro;
            this.CreditoGerado = trocaDevolucao.CreditoGerado;
            this.ValorExcedente = trocaDevolucao.ValorExcedente;
            this.Situacao = trocaDevolucao.DescrSituacao;
            this.OrigemTrocaDevolucao = trocaDevolucao.DescrOrigemTrocaDevolucao;
            this.Setor = trocaDevolucao.Setor;
            this.Descricao = trocaDevolucao.Descricao;
            this.Observacao = trocaDevolucao.Obs;
            this.UsuarioCadastro = trocaDevolucao.NomeUsuCad;
            this.Permissoes = new PermissoesDto
            {
                Editar = trocaDevolucao.EditEnabled,
                Cancelar = trocaDevolucao.CancelEnabled,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.TrocaDev, trocaDevolucao.IdTrocaDevolucao, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o nome do funcionário da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a loja da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define os dados do cliente da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a data da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("dataTrocaDevolucao")]
        public DateTime DataTrocaDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define a data de erro que originou a troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("dataErro")]
        public DateTime? DataErro { get; set; }

        /// <summary>
        /// Obtém ou define o valor de crédito gerado pela troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("creditoGerado")]
        public decimal CreditoGerado { get; set; }

        /// <summary>
        /// Obtém ou define o valor excedente a ser pago pela troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("valorExcedente")]
        public decimal ValorExcedente { get; set; }

        /// <summary>
        /// Obtém ou define a situação da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a origem da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("origemTrocaDevolucao")]
        public string OrigemTrocaDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define o setor da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("setor")]
        public string Setor { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a observação da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o usuário que fez o cadastro da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões da troca/devolução.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
