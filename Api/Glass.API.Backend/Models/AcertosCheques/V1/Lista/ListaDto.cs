// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.AcertosCheques.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de acertos de cheques.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="acertoCheque">O acerto do cheque que será retornado.</param>
        public ListaDto(AcertoCheque acertoCheque)
        {
            this.Id = (int)acertoCheque.IdAcertoCheque;
            this.Funcionario = acertoCheque.NomeFunc;
            this.Data = acertoCheque.DataAcerto;
            this.Valor = acertoCheque.ValorAcerto;
            this.Situacao = acertoCheque.DescrSituacao;
            this.Observacao = acertoCheque.Obs;
            this.Permissoes = new PermissoesDto
            {
                Cancelar = acertoCheque.CancelVisible,
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.AcertoCheque, acertoCheque.IdAcertoCheque),
            };
        }

        /// <summary>
        /// Obtém ou define o nome do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a data de acerto do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime? Data { get; set; }

        /// <summary>
        /// Obtém ou define o valor de acerto do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define a situação de acerto do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação do acerto do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da lista de acertos de cheques.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}