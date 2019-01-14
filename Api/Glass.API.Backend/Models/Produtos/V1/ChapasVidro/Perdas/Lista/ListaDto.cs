// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.ChapasVidro.Perdas.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de perdas de chapa de vidro.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="perdaChapaVidro">A model da perda de chapa de vidro.</param>
        internal ListaDto(PerdaChapaVidro perdaChapaVidro)
        {
            this.Id = (int)perdaChapaVidro.IdPerdaChapaVidro;
            this.Produto = perdaChapaVidro.DescrProd;
            this.DadosPerda = new DadosPerdaDto
            {
                Tipo = perdaChapaVidro.TipoPerda,
                Subtipo = perdaChapaVidro.SubtipoPerda,
                Data = perdaChapaVidro.DataPerda,
                Funcionario = perdaChapaVidro.FuncPerda,
            };

            this.Observacao = perdaChapaVidro.Obs;
            this.CorLinha = this.ObterCorLinha(perdaChapaVidro);
            this.Permissoes = new PermissoesDto
            {
                Cancelar = !perdaChapaVidro.Cancelado,
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.PerdaChapaVidro, perdaChapaVidro.IdPerdaChapaVidro),
            };
        }

        /// <summary>
        /// Obtém ou define a descrição do produto associado a perda.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public string Produto { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes a perda.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosPerda")]
        public DadosPerdaDto DadosPerda { get; set; }

        /// <summary>
        /// Obtém ou define a observação da perda.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define as pemissões concebidas a perda de chapa de vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(PerdaChapaVidro perdaChapaVidro)
        {
            return perdaChapaVidro.Cancelado ? "Red" : string.Empty;
        }
    }
}