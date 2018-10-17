// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Aplicacoes.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de aplicações de etiqueta.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="aplicacao">A aplicação que será retornada.</param>
        public ListaDto(EtiquetaAplicacao aplicacao)
        {
            this.Id = aplicacao.IdAplicacao;
            this.Codigo = aplicacao.CodInterno;
            this.Descricao = aplicacao.Descricao;
            this.DestacarNaEtiqueta = aplicacao.DestacarEtiqueta;
            this.GerarFormaInexistente = aplicacao.GerarFormaInexistente;
            this.NaoPermitirFastDelivery = aplicacao.NaoPermitirFastDelivery;
            this.NumeroDiasUteisDataEntrega = aplicacao.DiasMinimos;

            this.TiposPedidos = (aplicacao.TipoPedido ?? string.Empty)
                .Split(',')
                .Select(tipo =>
                {
                    var tipoPedido = (Data.Model.Pedido.TipoPedidoEnum?)tipo.StrParaIntNullable();
                    string descricaoTipoPedido = tipoPedido?.Translate().Format();

                    return IdNomeDto.TentarConverter((int?)tipoPedido, descricaoTipoPedido);
                })
                .Where(tipoPedido => tipoPedido != null);

            string descricaoSituacao = aplicacao.Situacao.Translate().Format();
            this.Situacao = IdNomeDto.TentarConverter((int)aplicacao.Situacao, descricaoSituacao);
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    Data.Model.LogAlteracao.TabelaAlteracao.Aplicacao,
                    (uint)aplicacao.IdAplicacao,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define a descrição da aplicação de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a aplicação de etiqueta exibe um destaque na etiqueta impressa.
        /// </summary>
        [DataMember]
        [JsonProperty("destacarNaEtiqueta")]
        public bool DestacarNaEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a aplicação de etiqueta gera forma inexistente.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarFormaInexistente")]
        public bool GerarFormaInexistente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a aplicação impede que os pedidos sejam 'fast delivery'.
        /// </summary>
        [DataMember]
        [JsonProperty("naoPermitirFastDelivery")]
        public bool NaoPermitirFastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define o número de dias úteis para calcular data de entrega da aplicação de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntrega")]
        public int NumeroDiasUteisDataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de pedido da aplicação de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidos")]
        public IEnumerable<IdNomeDto> TiposPedidos { get; set; }

        /// <summary>
        /// Obtém ou define a situação da aplicação de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para a aplicação de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
