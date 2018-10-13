// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que contém os dados de uma peça para a lista de produção.
    /// </summary>
    [DataContract(Name = "Peca")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="sessao">A sessão atual com o banco de dados.</param>
        /// <param name="peca">A peça que será representada pelo DTO.</param>
        public ListaDto(GDASession sessao, ProdutoPedidoProducao peca)
        {
            this.Id = (int)peca.IdProdPedProducao;
            this.Pedido = new PedidoDto
            {
                Id = (int)peca.IdPedido,
                IdExibir = peca.IdPedidoExibir,
                SiglaTipoPedido = peca.SiglaTipoPedido,
                Cancelado = peca.PedidoCancelado,
                CodigoPedidoCliente = peca.CodCliente,
                DataLiberacao = peca.DataLiberacaoPedido,
                Cliente = new IdNomeDto
                {
                    Id = (int?)peca.IdCliente,
                    Nome = peca.NomeCliente,
                },
            };

            this.ProdutoPedido = new ProdutoPedidoDto
            {
                Id = (int?)peca.IdProdPed,
                Composto = peca.IsProdutoLaminadoComposicao,
                Descricao = peca.DescrProduto,
                DescricaoBeneficiamentos = peca.DescrBeneficiamentos,
                DescricaoCompleta = ProdutoPedidoProducaoDAO.Instance.ObterDescrProdEtiqueta(sessao, peca.IdProdPedProducao),
            };

            this.PossuiImagemSvg = peca.TemSvgAssociado;
            this.SituacaoProducao = new SituacaoProducaoDto
            {
                PecaParada = peca.PecaParadaProducao,
                PecaReposta = peca.PecaReposta,
                PossuiLeituraSetorOculto = peca.TemLeituraSetorOculto,
                Perda = !peca.DataPerda.HasValue
                    ? null
                    : new PerdaDto
                    {
                        Tipo = peca.DescrTipoPerdaLista,
                        Data = peca.DataPerda.Value,
                    },
            };

            this.Altura = (decimal)peca.Altura;
            this.Largura = (int)peca.Largura;
            this.CodigoAplicacao = peca.CodAplicacao;
            this.CodigoProcesso = peca.CodProcesso;
            this.DataEntrega = new DataEntregaDto
            {
                Fabrica = peca.DataEntregaFabrica,
                Exibicao = peca.DataEntregaExibicao,
            };

            this.PlanoCorte = peca.PlanoCorte;
            this.NumeroEtiqueta = peca.NumEtiqueta;
            this.NumeroCavalete = peca.NumCavalete;
            this.IdImpressao = (int?)peca.IdImpressao;
            this.Cancelada = peca.PecaCancelada;
            this.Leituras = this.ObterLeituras(peca);
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.ProdPedProducao, peca.IdProdPedProducao, null),
                DesfazerLeitura = peca.RemoverSituacaoVisible,
                RelatorioPedido = peca.ExibirRelatorioPedido,
                PararPecaProducao = peca.ExibirPararPecaProducao,
                LogEstornoCarregamento = peca.EstornoCarregamentoVisible,
            };

            this.CorLinha = peca.CorLinha;
        }

        /// <summary>
        /// Obtém ou define os dados do pedido da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("pedido")]
        public PedidoDto Pedido { get; set; }

        /// <summary>
        /// Obtém ou define os dados do produto de pedido da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoPedido")]
        public ProdutoPedidoDto ProdutoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça possui arquivo SVG associado.
        /// </summary>
        [DataMember]
        [JsonProperty("possuiImagemSvg")]
        public bool PossuiImagemSvg { get; set; }

        /// <summary>
        /// Obtém ou define os dados de situação de produção da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoProducao")]
        public SituacaoProducaoDto SituacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define a altura da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define o código da aplicação da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoAplicacao")]
        public string CodigoAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoProcesso")]
        public string CodigoProcesso { get; set; }

        /// <summary>
        /// Obtém ou define os dados da data de entrega da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrega")]
        public DataEntregaDto DataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o plano de corte da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("planoCorte")]
        public string PlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o número de etiqueta da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroEtiqueta")]
        public string NumeroEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o número de cavalete da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCavalete")]
        public string NumeroCavalete { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da impressão da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("idImpressao")]
        public int? IdImpressao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça está cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelada")]
        public bool Cancelada { get; set; }

        /// <summary>
        /// Obtém ou define os dados das leituras da peça nos setores de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("leituras")]
        public IEnumerable<LeituraDto> Leituras { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da peça na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        private IEnumerable<LeituraDto> ObterLeituras(ProdutoPedidoProducao peca)
        {
            var idsSetoresPeca = peca.GroupIdSetor?.Split(',')
                .Select(x => x.StrParaInt())
                .ToList();

            return Utils.GetSetores
                .Select(setor =>
                {
                    var posicaoSetor = idsSetoresPeca.IndexOf(setor.IdSetor);
                    if (posicaoSetor < 0)
                    {
                        return new LeituraDto();
                    }

                    var dataLeitura = peca.VetDataLeitura[posicaoSetor]?.Length > 0
                        ? DateTime.Parse(peca.VetDataLeitura[posicaoSetor])
                        : (DateTime?)null;

                    return new LeituraDto
                    {
                        Setor = new SetorDto
                        {
                            Id = setor.IdSetor,
                            Nome = setor.Descricao,
                            Obrigatorio = peca.SetorNaoObrigatorio[posicaoSetor]?.Length == 0,
                        },
                        Data = dataLeitura,
                        Funcionario = peca.VetNomeFuncLeitura[posicaoSetor],
                        Chapa = !(dataLeitura.HasValue && peca.VetSetorCorte[posicaoSetor])
                            ? null
                            : new ChapaVidroDto
                            {
                                Lote = peca.LoteChapa,
                                NumeroEtiqueta = peca.NumEtiquetaChapa,
                                NumeroNotaFiscal = peca.NumeroNFeChapa,
                            },
                    };
                })
                .Where(leitura => leitura.Setor != null);
        }
    }
}
