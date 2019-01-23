// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.DevolucoesPagamento.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados da devolução do pagamento para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="devolucaoPagto">A model de devolução de pagamento.</param>
        internal ListaDto(DevolucaoPagto devolucaoPagto)
        {
            this.Id = (int?)devolucaoPagto.IdDevolucaoPagto;
            this.Cliente = new IdNomeDto
            {
                Id = (int?)devolucaoPagto.IdCliente,
                Nome = devolucaoPagto.NomeCliente,
            };

            this.Valor = devolucaoPagto.Valor;
            this.Situacao = devolucaoPagto.DescrSituacao;
            this.DataCadastro = devolucaoPagto.DataCad;
            this.UsuarioCadastro = devolucaoPagto.DescrUsuCad;
            this.Permissoes = new PermissoesDto
            {
                Cancelar = devolucaoPagto.PodeCancelar,
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.DevolucaoPagamento, devolucaoPagto.IdDevolucaoPagto),
            };
        }

        /// <summary>
        /// Obtém ou define o cliente referente a devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o valor da devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define a situação da devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro da devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o usuário cadastrado na devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da lista da devolução do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}