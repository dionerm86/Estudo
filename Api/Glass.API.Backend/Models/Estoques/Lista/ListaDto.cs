// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.NotasFiscais.TiposParticipantes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um estoque de produto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Estoque")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="estoqueProduto">A model de estoque de produto.</param>
        internal ListaDto(ProdutoLoja estoqueProduto)
        {
            this.IdProduto = estoqueProduto.IdProd;
            this.IdLoja = estoqueProduto.IdLoja;
            this.IdLog = (int)estoqueProduto.IdLog;
            this.CodigoInternoProduto = estoqueProduto.CodInternoProd;
            this.DescricaoProduto = estoqueProduto.DescrProduto;
            this.DescricaoGrupoProduto = estoqueProduto.DescrGrupoProd;
            this.DescricaoTipoCalculo = estoqueProduto.DescrEstoque;
            this.DescricaoSubgrupoProduto = estoqueProduto.DescrSubgrupoProd;
            this.EstoqueMinimo = (decimal)estoqueProduto.EstMinimo;
            this.DescricaoEstoqueMinimo = estoqueProduto.EstoqueMinimoString;
            this.EstoqueM2 = (decimal)estoqueProduto.M2;
            this.QuantidadeReserva = (decimal)estoqueProduto.Reserva;
            this.QuantidadeLiberacao = (decimal)estoqueProduto.Liberacao;
            this.QuantidadeEstoque = (decimal)estoqueProduto.QtdEstoque;
            this.DescricaoQuantidadeEstoque = estoqueProduto.QtdEstoqueStringLabel;
            this.DescricaoEstoqueDisponivel = estoqueProduto.EstoqueDisponivel;
            this.QuantidadeEstoqueFiscal = (decimal)estoqueProduto.EstoqueFiscal;
            this.QuantidadeDefeito = (decimal)estoqueProduto.Defeito;
            this.QuantidadePosseTerceiros = (decimal)estoqueProduto.QtdePosseTerceiros;

            var conversor = new ConversorParticipante(estoqueProduto);
            this.Participante = conversor.ObterParticipante();
            this.TipoParticipante = conversor.ObterTipoParticipante();

            this.Permissoes = new PermissoesDto
            {
                Editar = estoqueProduto.EditVisible,
                ExibirLinkReserva = estoqueProduto.TipoCalc == (int)TipoCalculoGrupoProd.Qtd || estoqueProduto.TipoCalc == (int)TipoCalculoGrupoProd.QtdDecimal,
                ExibirLinkLiberacao = estoqueProduto.TipoCalc == (int)TipoCalculoGrupoProd.Qtd || estoqueProduto.TipoCalc == (int)TipoCalculoGrupoProd.QtdDecimal,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.ProdutoLoja, estoqueProduto.IdLog, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProduto")]
        public int IdProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do log do estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("idLog")]
        public int IdLog { get; set; }

        /// <summary>
        /// Obtém ou define o código interno do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoInternoProduto")]
        public string CodigoInternoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoGrupoProduto")]
        public string DescricaoGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do subgrupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoSubgrupoProduto")]
        public string DescricaoSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do tipo de cálculo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoTipoCalculo")]
        public string DescricaoTipoCalculo { get; set; }

        /// <summary>
        /// Obtém ou define o estoque mínimo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueMinimo")]
        public decimal EstoqueMinimo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do estoque mínimo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoEstoqueMinimo")]
        public string DescricaoEstoqueMinimo { get; set; }

        /// <summary>
        /// Obtém ou define o estoque em m2 do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueM2")]
        public decimal EstoqueM2 { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculo")]
        public IdNomeDto TipoCalculo { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em reserva do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeReserva")]
        public decimal QuantidadeReserva { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em liberação do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeLiberacao")]
        public decimal QuantidadeLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoque")]
        public decimal QuantidadeEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a descrição completa do estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoQuantidadeEstoque")]
        public string DescricaoQuantidadeEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a descrição completa do estoque disponível do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoEstoqueDisponivel")]
        public string DescricaoEstoqueDisponivel { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public decimal QuantidadeEstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do estoque do produto com defeito.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeDefeito")]
        public decimal QuantidadeDefeito { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do estoque do produto em posse de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePosseTerceiros")]
        public decimal QuantidadePosseTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define o participante (estoque em posse de terceitos).
        /// </summary>
        [DataMember]
        [JsonProperty("participante")]
        public IdNomeDto Participante { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do participante (estoque em posse de terceitos).
        /// </summary>
        [DataMember]
        [JsonProperty("tipoParticipante")]
        public TipoParticipanteDto TipoParticipante { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
