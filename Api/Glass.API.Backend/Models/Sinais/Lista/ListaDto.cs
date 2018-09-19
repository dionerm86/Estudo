// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Sinais.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um sinal para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Sinal")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="sinal">A model de sinal.</param>
        internal ListaDto(Sinal sinal)
        {
            this.Id = (int)sinal.IdSinal;
            this.Cliente = new IdNomeDto
            {
                Id = (int)sinal.IdCliente,
                Nome = sinal.NomeCliente,
            };

            this.Total = sinal.TotalSinal;
            this.DataCadastro = sinal.DataCad;
            this.Situacao = sinal.DescrSituacao;
            this.Observacao = sinal.Obs;

            this.Permissoes = new PermissoesDto()
            {
                Cancelar = sinal.Situacao != (int)Sinal.SituacaoEnum.Cancelado,
                LogAlteracoes = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.Sinal, sinal.IdSinal),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.Sinal, sinal.IdSinal),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o cliente do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o total do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
