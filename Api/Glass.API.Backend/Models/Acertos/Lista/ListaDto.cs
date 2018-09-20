// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Acertos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um acerto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Acerto")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="acerto">A model de acerto.</param>
        internal ListaDto(Acerto acerto)
        {
            this.Id = (int)acerto.IdAcerto;
            this.Referencia = acerto.Referencia;
            this.Cliente = new IdNomeDto
            {
                Id = (int)acerto.IdCli,
                Nome = acerto.NomeCliente,
            };

            this.NomeFuncionario = acerto.Funcionario;
            this.Total = acerto.TotalAcerto;
            this.Situacao = acerto.DescrSituacao;
            this.DataCadastro = acerto.DataCad;
            this.Observacao = acerto.Obs;
            this.CorLinha = this.ObterCorLinha(acerto.PossuiContaJuridico, acerto.Renegociacao);

            this.Permissoes = new PermissoesDto()
            {
                Cancelar = acerto.Situacao != (int)Acerto.SituacaoEnum.Cancelado,
                ExibirNotaPromissoria = acerto.Renegociacao && FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0,
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.Acerto, acerto.IdAcerto),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a referência do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o cliente do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do funcionário que efetuou o acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeFuncionario")]
        public string NomeFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o total do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a situação do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a observação do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(bool possuiContaProtestada, bool renegociacao)
        {
            if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico && possuiContaProtestada)
            {
                return Color.FromArgb(225, 200, 0).ToString();
            }
            else if (renegociacao)
            {
                return Color.Blue.ToString();
            }

            return Color.Black.ToString();
        }
    }
}
