// <copyright file="ListaObservacaoFinanceiroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ObservacoesFinanceiro.Lista
{
    /// <summary>
    /// Classe que encapsula os dados da lista de observações do financeiro.
    /// </summary>
    [DataContract(Name = "ObservacaoFinanceiro")]
    public class ListaObservacaoFinanceiroDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaObservacaoFinanceiroDto"/>.
        /// </summary>
        /// <param name="observacao">A model de observações do financeiro.</param>
        internal ListaObservacaoFinanceiroDto(ObservacaoFinalizacaoFinanceiro observacao)
        {
            this.Id = (int)observacao.IdObsFinanc;
            this.Motivo = observacao.DescrMotivo;
            this.Observacao = observacao.Observacao;
            this.CorLinha = this.ObterCorLinha(observacao.Motivo);
            this.Cadastro = new DataFuncionarioDto
            {
                Funcionario = observacao.NomeFuncCad,
                Data = observacao.DataCad,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da observação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o motivo da observação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("motivo")]
        public string Motivo { get; set; }

        /// <summary>
        /// Obtém ou define a observação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da observação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define os dados de cadastro da observação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public DataFuncionarioDto Cadastro { get; set; }

        private string ObterCorLinha(ObservacaoFinalizacaoFinanceiro.MotivoEnum motivo)
        {
            switch (motivo)
            {
                case ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoFinalizar:
                case ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoConfirmar:
                    return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.Red);

                default:
                    return null;
            }
        }
    }
}
