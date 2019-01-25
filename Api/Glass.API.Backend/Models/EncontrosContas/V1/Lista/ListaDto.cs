// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Acertos.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.EncontrosContas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados dos encontros de contas para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="encontroContas">A model de encontros de contas.</param>
        internal ListaDto(EncontroContas encontroContas)
        {
            this.Id = (int?)encontroContas.IdEncontroContas;
            this.Cliente = new IdNomeDto
            {
                Id = (int?)encontroContas.IdCliente,
                Nome = encontroContas.NomeCliente,
            };

            this.Fornecedor = new IdNomeDto
            {
                Id = (int?)encontroContas.IdFornecedor,
                Nome = encontroContas.NomeFornecedor,
            };

            this.Valores = new ValoresDto
            {
                Pagar = encontroContas.ValorPagar,
                Receber = encontroContas.ValorReceber,
                Saldo = encontroContas.Saldo,
            };

            this.DataCadastro = encontroContas.DataCad;
            this.Situacao = encontroContas.SituacaoStr;
            this.Observacao = encontroContas.Obs;
            this.Permissoes = new PermissoesDto
            {
                Editar = encontroContas.EditarVisible,
                Excluir = encontroContas.ExcluirVisible,
                Imprimir = encontroContas.RelIndVisible,
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.EncontroContas, encontroContas.IdEncontroContas),
            };
        }

        /// <summary>
        /// Obtém ou define dados básicos do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define dados básicos do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define os valores.
        /// </summary>
        [DataMember]
        [JsonProperty("valores")]
        public ValoresDto Valores { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro dos encontros de contas.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da lista de encontro de contas.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}