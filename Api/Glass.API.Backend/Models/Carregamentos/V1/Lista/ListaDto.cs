// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de carregamentos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="carregamento">O carregamento que será retornado.</param>
        public ListaDto(Carregamento carregamento)
        {
            this.Id = (int)carregamento.IdCarregamento;
            this.Motorista = new IdNomeDto
            {
                Id = (int)carregamento.IdMotorista,
                Nome = carregamento.NomeMotorista,
            };

            this.Veiculo = new CodigoNomeDto
            {
                Codigo = carregamento.Placa,
                Nome = carregamento.Veiculo,
            };

            this.DataPrevisaoSaida = carregamento.DataPrevistaSaida;
            this.Loja = carregamento.NomeLoja;
            this.Situacao = carregamento.SituacaoStr;
            this.Rotas = !string.IsNullOrEmpty(carregamento.DescrRotas) ? carregamento.DescrRotas.Split(',') : null;
            this.Peso = (decimal)carregamento.Peso;
            this.ValorTotalPedidos = carregamento.ValorTotalPedidos;
            this.SituacaoFaturamento = Colosoft.Translator.Translate(carregamento.SituacaoFaturamento).Format();

            this.Permissoes = new PermissoesDto
            {
                FaturarCarregamento = PCPConfig.HabilitarFaturamentoCarregamento && carregamento.SituacaoFaturamento != SituacaoFaturamentoEnum.Faturado,
                ImprimirFaturamento = PCPConfig.HabilitarFaturamentoCarregamento && carregamento.SituacaoFaturamento == SituacaoFaturamentoEnum.Faturado,
                VisualizarPendenciasCarregamento = PCPConfig.HabilitarFaturamentoCarregamento && carregamento.SituacaoFaturamento == SituacaoFaturamentoEnum.FaturadoParcialmente,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Carregamento, carregamento.IdCarregamento, null),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.OrdemCarga, carregamento.IdCarregamento),
            };
        }

        /// <summary>
        /// Obtém ou define o motorista do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("motorista")]
        public IdNomeDto Motorista { get; set; }

        /// <summary>
        /// Obtém ou define o veículo do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("veiculo")]
        public CodigoNomeDto Veiculo { get; set; }

        /// <summary>
        /// Obtém ou define a data de previsão de saída do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPrevisaoSaida")]
        public DateTime DataPrevisaoSaida { get; set; }

        /// <summary>
        /// Obtém ou define dados da loja do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define a situação do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as rotas do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("rotas")]
        public IEnumerable<string> Rotas { get; set; }

        /// <summary>
        /// Obtém ou define o peso total dos itens do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal Peso { get; set; }

        /// <summary>
        /// Obtém ou define o valor total dos pedidos do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorTotalPedidos")]
        public decimal ValorTotalPedidos { get; set; }

        /// <summary>
        /// Obtém ou define a situação do faturamento do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoFaturamento")]
        public string SituacaoFaturamento { get; set; }

        /// <summary>
        /// Obtém ou define permissões da lista de carregamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
