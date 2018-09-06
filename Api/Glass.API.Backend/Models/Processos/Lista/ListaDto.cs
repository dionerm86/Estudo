// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de processos de etiqueta.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdCodigoDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListaDto"/> class.
        /// </summary>
        /// <param name="processo">O processo que será retornado.</param>
        public ListaDto(EtiquetaProcessoPesquisa processo)
        {
            this.Id = processo.IdProcesso;
            this.Codigo = processo.CodInterno;
            this.Descricao = processo.Descricao;
            this.Aplicacao = IdCodigoDto.TentarConverter(processo.IdAplicacao, processo.CodInternoAplicacao);
            this.DestacarNaEtiqueta = processo.DestacarEtiqueta;
            this.GerarFormaInexistente = processo.GerarFormaInexistente;
            this.GerarArquivoDeMesa = processo.GerarArquivoDeMesa;
            this.NumeroDiasUteisDataEntrega = processo.NumeroDiasUteisDataEntrega;

            string descricaoTipoProcesso = processo.TipoProcesso?.Translate().Format();
            this.TipoProcesso = IdNomeDto.TentarConverter((int?)processo.TipoProcesso, descricaoTipoProcesso);

            this.TiposPedidos = (processo.TipoPedido ?? string.Empty)
                .Split(',')
                .Select(tipo =>
                {
                    var tipoPedido = (Data.Model.Pedido.TipoPedidoEnum?)tipo.StrParaIntNullable();
                    string descricaoTipoPedido = tipoPedido?.Translate().Format();

                    return IdNomeDto.TentarConverter((int?)tipoPedido, descricaoTipoPedido);
                })
                .Where(tipoPedido => tipoPedido != null);

            string descricaoSituacao = processo.Situacao.Translate().Format();
            this.Situacao = IdNomeDto.TentarConverter((int)processo.Situacao, descricaoSituacao);
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    Data.Model.LogAlteracao.TabelaAlteracao.Processo,
                    (uint)processo.IdProcesso,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define a descrição do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a aplicação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo de etiqueta exibe um destaque na etiqueta impressa.
        /// </summary>
        [DataMember]
        [JsonProperty("destacarNaEtiqueta")]
        public bool DestacarNaEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo de etiqueta gera forma inexistente.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarFormaInexistente")]
        public bool GerarFormaInexistente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo de etiqueta gera arquivo de mesa.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDeMesa")]
        public bool GerarArquivoDeMesa { get; set; }

        /// <summary>
        /// Obtém ou define o número de dias úteis para calcular data de entrega do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntrega")]
        public int NumeroDiasUteisDataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoProcesso")]
        public IdNomeDto TipoProcesso { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de pedido do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidos")]
        public IEnumerable<IdNomeDto> TiposPedidos { get; set; }

        /// <summary>
        /// Obtém ou define a situação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para o processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
