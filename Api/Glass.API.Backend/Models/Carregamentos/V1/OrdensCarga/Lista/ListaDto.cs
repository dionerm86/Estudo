// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de ordens de carga na lista de carregamentos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="ordemCarga">A ordem de carga que será retornada.</param>
        internal ListaDto(OrdemCarga ordemCarga)
        {
            this.Id = (int)ordemCarga.IdOrdemCarga;
            this.Cliente = new IdNomeDto
            {
                Id = (int)ordemCarga.IdCliente,
                Nome = ordemCarga.NomeCliente,
            };

            this.Loja = ordemCarga.NomeLoja;
            this.Rota = ordemCarga.CodRota;
            this.Peso = (decimal)ordemCarga.Peso;
            this.PesoPendente = (decimal)ordemCarga.PesoPendenteProducao;
            this.TotaEmM2 = (decimal)ordemCarga.TotalM2;
            this.TotalEmM2Pendente = (decimal)ordemCarga.TotalM2PendenteProducao;
            this.QuantidadePecas = (int)ordemCarga.QtdePecasVidro;
            this.QuantidadePecasPendentes = (int)ordemCarga.QtdePecaPendenteProducao;
            this.ValorTotalPedidos = ordemCarga.ValorTotalPedidos;
            this.QuantidadeVolumes = (int)ordemCarga.QtdeVolumes;
            this.Tipo = ordemCarga.TipoOrdemCargaStr;
            this.Situacao = ordemCarga.SituacaoStr;
            this.ClienteExportaPedidos = Data.DAL.ClienteDAO.Instance.ClienteImportacao((uint)this.Cliente.Id);
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.OrdemCarga, ordemCarga.IdOrdemCarga, null),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.OrdemCarga, ordemCarga.IdOrdemCarga),
            };
        }

        /// <summary>
        /// Obtém ou define o cliente da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define a loja da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define a rota da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("rota")]
        public string Rota { get; set; }

        /// <summary>
        /// Obtém ou define o peso da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal? Peso { get; set; }

        /// <summary>
        /// Obtém ou define o peso pendente da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("pesoPendente")]
        public decimal? PesoPendente { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("totaEmM2")]
        public decimal? TotaEmM2 { get; set; }

        /// <summary>
        /// Obtém ou define o total pendente em metros quadrados da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("totalEmM2Pendente")]
        public decimal? TotalEmM2Pendente { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade total de peças da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecas")]
        public int? QuantidadePecas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade pendente de peças da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecasPendentes")]
        public int? QuantidadePecasPendentes { get; set; }

        /// <summary>
        /// Obtém ou define o valor total dos pedidos da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("valorTotalPedidos")]
        public decimal? ValorTotalPedidos { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de volumes da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeVolumes")]
        public decimal? QuantidadeVolumes { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente é externo.
        /// </summary>
        [DataMember]
        [JsonProperty("clienteExportaPedidos")]
        public bool? ClienteExportaPedidos { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para a tela de listagem de ordens de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
