// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Genericas.Venda;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados básicos de produto para os controles de filtro.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ProdutoDto"/>.
        /// </summary>
        /// <param name="sessao">A transação atual com o banco de dados.</param>
        /// <param name="produto">A model de produtos.</param>
        public ProdutoDto(GDASession sessao, Produto produto)
        {
            var tipoCalculo = (TipoCalculoGrupoProd)produto.TipoCalculo;

            this.Id = produto.IdProd;
            this.Codigo = produto.CodInterno;
            this.Descricao = produto.Descricao;
            this.IdGrupo = produto.IdGrupoProd;
            this.IdSubgrupo = produto.IdSubgrupoProd;
            this.IdCor = produto.IdCorVidro ?? produto.IdCorAluminio ?? produto.IdCorFerragem;
            this.Espessura = produto.Espessura;
            this.Altura = this.ObterAltura(produto, tipoCalculo);
            this.Largura = this.ObterLargura(produto);
            this.Quantidade = this.ObterQuantidade(tipoCalculo);
            this.Custo = produto.CustoCompra;
            this.ExibirBeneficiamentos = this.ObterExibirBeneficiamentos(produto, tipoCalculo);
            this.Beneficiamentos = produto.Beneficiamentos?.ObterListaBeneficiamentos();
            this.ExigirProcessoEAplicacao = this.ObterExigirProcessoEAplicacao(produto, tipoCalculo);

            var produtoComposicao = this.IsProdLamComposicao(sessao, produto);

            this.Composicao = new ComposicaoDto
            {
                PossuiFilhos = produtoComposicao,
                IdsSubgruposProdutosFilhos = this.ObterSubgruposFilhos(sessao, produtoComposicao, produto),
            };
        }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupo")]
        public int IdGrupo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idSubgrupo")]
        public int? IdSubgrupo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idCor")]
        public int? IdCor { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double Espessura { get; set; }

        /// <summary>
        /// Obtém ou define um valor com informações sobre a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public AlturaDto Altura { get; set; }

        /// <summary>
        /// Obtém ou define um valor com informações sobre a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public LarguraDto Largura { get; set; }

        /// <summary>
        /// Obtém ou define um valor com informações sobre a quantidade do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public QuantidadeDto Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o custo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("custo")]
        public decimal Custo { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o valor mínimo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorMinimo")]
        public decimal ValorMinimo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o valor unitário do produto pode ser editado.
        /// </summary>
        [DataMember]
        [JsonProperty("podeEditarValorUnitario")]
        public bool PodeEditarValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto permite a aplicação de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirBeneficiamentos")]
        public bool ExibirBeneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define os dados de desconto e acréscimo por cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoAcrescimoCliente")]
        public DescontoAcrescimoClienteDto DescontoAcrescimoCliente { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos padrão do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public IEnumerable<ItemBeneficiamentoDto> Beneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os dados de etiqueta (processo e aplicação) são obrigatórios.
        /// </summary>
        [DataMember]
        [JsonProperty("exigirProcessoEAplicacao")]
        public bool ExigirProcessoEAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o tamanho máximo restante para venda na obra.
        /// </summary>
        [DataMember]
        [JsonProperty("tamanhoMaximoObra")]
        public double? TamanhoMaximoObra { get; set; }

        /// <summary>
        /// Obtém ou define os dados de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoque")]
        public EstoqueDto Estoque { get; set; }

        /// <summary>
        /// Obtém ou define os dados de composição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("composicao")]
        public ComposicaoDto Composicao { get; set; }

        private AlturaDto ObterAltura(Produto produto, TipoCalculoGrupoProd tipoCalculo)
        {
            var tiposCalculoAlturaDecimal = new[]
            {
                new { TipoCalculo = TipoCalculoGrupoProd.ML, Arredondamento = (double?)null },
                new { TipoCalculo = TipoCalculoGrupoProd.MLAL0, Arredondamento = (double?)null },
                new { TipoCalculo = TipoCalculoGrupoProd.MLAL05, Arredondamento = (double?)0.5 },
                new { TipoCalculo = TipoCalculoGrupoProd.MLAL1, Arredondamento = (double?)1 },
                new { TipoCalculo = TipoCalculoGrupoProd.MLAL6, Arredondamento = (double?)6 },
            };

            var tiposCalculoAlturaNaoEditavel = new[]
            {
                TipoCalculoGrupoProd.Qtd,
                TipoCalculoGrupoProd.QtdDecimal,
                TipoCalculoGrupoProd.QtdM2,
            };

            var tipoCalculoDecimal = tiposCalculoAlturaDecimal.FirstOrDefault(t => t.TipoCalculo == tipoCalculo);

            return new AlturaDto
            {
                PermiteDecimal = tipoCalculoDecimal != null,
                PodeEditar = !tiposCalculoAlturaNaoEditavel.Contains(tipoCalculo),
                Valor = produto.Altura,
                FatorArredondamento = tipoCalculoDecimal?.Arredondamento,
            };
        }

        private LarguraDto ObterLargura(Produto produto)
        {
            return new LarguraDto
            {
                PermiteDecimal = false,
                PodeEditar = this.Altura.PodeEditar && !this.Altura.PermiteDecimal,
                Valor = produto.Largura,
            };
        }

        private QuantidadeDto ObterQuantidade(TipoCalculoGrupoProd tipoCalculo)
        {
            var tiposCalculoQuantidadeDecimal = new[]
            {
                TipoCalculoGrupoProd.QtdDecimal,
            };

            return new QuantidadeDto
            {
                PermiteDecimal = tiposCalculoQuantidadeDecimal.Contains(tipoCalculo),
            };
        }

        private bool ObterExibirBeneficiamentos(Produto produto, TipoCalculoGrupoProd tipoCalculo)
        {
            if (Geral.UsarBeneficiamentosTodosOsGrupos)
            {
                return true;
            }
            else if (produto.IdGrupoProd != (int)NomeGrupoProd.Vidro)
            {
                return false;
            }

            var tiposCalculoBeneficiamento = new[]
            {
                TipoCalculoGrupoProd.M2,
                TipoCalculoGrupoProd.M2Direto,
            };

            return tiposCalculoBeneficiamento.Contains(tipoCalculo);
        }

        private bool ObterExigirProcessoEAplicacao(Produto produto, TipoCalculoGrupoProd tipoCalculo)
        {
            var chapaDeVidro = produto.IdSubgrupoProd.HasValue
                ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(produto.IdProd) == TipoSubgrupoProd.ChapasVidro
                : false;

            if (chapaDeVidro)
            {
                return false;
            }

            var roteiroProducao = PCPConfig.ControlarProducao
                && Data.Helper.Utils.GetSetores.Any(x => x.SetorPertenceARoteiro)
                && produto.IdGrupoProd == (int)NomeGrupoProd.Vidro;

            var tiposCalculoObrigarProcessoEAplicacao = new[]
            {
                TipoCalculoGrupoProd.M2,
                TipoCalculoGrupoProd.M2Direto,
            };

            var obrigarDadosEtiquetaVidrosBeneficiaveis = this.ExibirBeneficiamentos
                && PedidoConfig.DadosPedido.ObrigarProcAplVidros;

            return tiposCalculoObrigarProcessoEAplicacao.Contains(tipoCalculo)
                && (roteiroProducao
                    || obrigarDadosEtiquetaVidrosBeneficiaveis);
        }

        private bool IsProdLamComposicao(GDASession sessao, Produto produto)
        {
            var subGrupos = new List<int>() { (int)TipoSubgrupoProd.VidroLaminado, (int)TipoSubgrupoProd.VidroDuplo };
            return subGrupos.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)produto.IdProd));
        }

        private IEnumerable<int> ObterSubgruposFilhos(GDASession sessao, bool produtoComposicao, Produto produto)
        {
            return produtoComposicao
                ? ProdutoBaixaEstoqueDAO.Instance.ObterIdsSubgruposProdutosBaixa(sessao, produto.IdProd)
                : null;
        }
    }
}
