// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Produtos.GruposProduto.Lista;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.SubgruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de subgrupos de produto.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="subgrupoProduto">O subgrupo de produto que será retornado.</param>
        public ListaDto(Global.Negocios.Entidades.SubgrupoProdPesquisa subgrupoProduto)
        {
            this.Id = subgrupoProduto.IdSubgrupoProd;
            this.Nome = subgrupoProduto.Descricao;
            this.VidroTemperado = subgrupoProduto.IsVidroTemperado;
            this.LiberarPendenteProducao = subgrupoProduto.LiberarPendenteProducao;
            this.PermitirItemRevendaNaVenda = subgrupoProduto.PermitirItemRevendaNaVenda;
            this.Tipo = new IdNomeDto()
            {
                Id = (int)subgrupoProduto.TipoSubgrupo,
                Nome = Colosoft.Translator.Translate(subgrupoProduto.TipoSubgrupo).Format(),
            };

            this.Cliente = new IdNomeDto()
            {
                Id = subgrupoProduto.IdCli,
                Nome = subgrupoProduto.IdCli > 0 ? ClienteDAO.Instance.GetNome((uint)subgrupoProduto.IdCli.Value) : string.Empty,
            };

            this.LojasAssociadas = this.ObterLojasAssociadas(subgrupoProduto.IdsLojaAssociacao, subgrupoProduto.Lojas);

            this.Entrega = new EntregaDto()
            {
                DiaSemanaEntrega = subgrupoProduto.DiaSemanaEntrega,
                DiasMinimoEntrega = subgrupoProduto.NumeroDiasMinimoEntrega,
            };

            this.TiposCalculo = new TiposCalculoDto()
            {
                Pedido = new IdNomeDto()
                {
                    Id = (int?)subgrupoProduto.TipoCalculo,
                    Nome = Colosoft.Translator.Translate(subgrupoProduto.TipoCalculo).Format(),
                },
                NotaFiscal = new IdNomeDto()
                {
                    Id = (int?)subgrupoProduto.TipoCalculoNf,
                    Nome = Colosoft.Translator.Translate(subgrupoProduto.TipoCalculoNf).Format(),
                },
            };

            this.Estoque = new EstoqueDto()
            {
                BloquearEstoque = subgrupoProduto.BloquearEstoque,
                AlterarEstoque = subgrupoProduto.AlterarEstoque,
                AlterarEstoqueFiscal = subgrupoProduto.AlterarEstoqueFiscal,
                ExibirMensagemEstoque = subgrupoProduto.ExibirMensagemEstoque,
                GeraVolume = subgrupoProduto.GeraVolume,
                BloquearVendaECommerce = subgrupoProduto.BloquearEcommerce,
                ProdutoParaEstoque = subgrupoProduto.ProdutosEstoque,
            };

            this.Permissoes = new PermissoesDto()
            {
                Excluir = !subgrupoProduto.SubgrupoSistema,
                GrupoVidro = subgrupoProduto.IdGrupoProd == (int)Glass.Data.Model.NomeGrupoProd.Vidro,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.SubgrupoProduto, (uint)subgrupoProduto.IdSubgrupoProd, null),
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto é de vidros temperados.
        /// </summary>
        [DataMember]
        [JsonProperty("vidroTemperado")]
        public bool VidroTemperado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos desse subgrupo podem ser liberados com a produção pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("liberarPendenteProducao")]
        public bool LiberarPendenteProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é permitida a revenda de produtos do tipo venda (solução para inclusão de embalagem no pedido de venda).
        /// </summary>
        [DataMember]
        [JsonProperty("permitirItemRevendaNaVenda")]
        public bool PermitirItemRevendaNaVenda { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define as lojas associadas à este subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("lojasAssociadas")]
        public IEnumerable<IdNomeDto> LojasAssociadas { get; set; }

        /// <summary>
        /// Obtém ou define dados de entrega do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public EntregaDto Entrega { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de cálculo do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposCalculo")]
        public TiposCalculoDto TiposCalculo { get; set; }

        /// <summary>
        /// Obtém ou define dados de estoque do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoque")]
        public EstoqueDto Estoque { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private IEnumerable<IdNomeDto> ObterLojasAssociadas(int[] idsLojaAssociacao, string lojas)
        {
            var listaLojas = lojas?.Split(',');

            for (var i = 0; i < idsLojaAssociacao.Length; i++)
            {
                yield return new IdNomeDto()
                {
                    Id = idsLojaAssociacao[i],
                    Nome = listaLojas[i],
                };
            }
        }
    }
}
