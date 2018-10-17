// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um ambiente de pedido.
    /// </summary>
    [DataContract(Name = "Ambiente")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="ambientePedido">A model de ambiente de pedido.</param>
        internal ListaDto(AmbientePedido ambientePedido)
        {
            this.Id = (int)ambientePedido.IdAmbientePedido;
            this.Projeto = !ambientePedido.IdItemProjeto.HasValue
                ? null
                : new DadosProjetoDto
                {
                    IdItemProjeto = (int)ambientePedido.IdItemProjeto.Value,
                    Descricao = ambientePedido.DescrObsProj,
                };

            this.Nome = ambientePedido.Ambiente;
            this.ProdutoMaoDeObra = !this.AmbienteEMaoDeObra(ambientePedido)
                ? null
                : new ProdutoMaoDeObraDto
                {
                    Id = (int)ambientePedido.IdProd.Value,
                    CodigoInterno = ambientePedido.CodInterno,
                    Altura = ambientePedido.Altura.Value,
                    Largura = ambientePedido.Largura.Value,
                    Quantidade = ambientePedido.Qtde.Value,
                    Redondo = ambientePedido.Redondo,
                    Aplicacao = this.RecuperaDadosProcessoAplicacao((int?)ambientePedido.IdAplicacao, ambientePedido.CodAplicacao),
                    Processo = this.RecuperaDadosProcessoAplicacao((int?)ambientePedido.IdProcesso, ambientePedido.CodProcesso),
                };

            this.Descricao = ambientePedido.Descricao;
            this.TotalProdutos = ambientePedido.TotalProdutos;
            this.Acrescimo = new AcrescimoDescontoDto
            {
                Tipo = ambientePedido.TipoAcrescimo,
                Valor = ambientePedido.Acrescimo,
            };

            this.Desconto = new AcrescimoDescontoDto
            {
                Tipo = ambientePedido.TipoDesconto,
                Valor = ambientePedido.Desconto,
            };

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.AmbientePedido, ambientePedido.IdAmbientePedido, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define os dados do projeto vinculado ao ambiente, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("projeto")]
        public DadosProjetoDto Projeto { get; set; }

        /// <summary>
        /// Obtém ou define o valor total dos produtos do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("totalProdutos")]
        public decimal TotalProdutos { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define o nome do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define os dados do produto mão-de-obra, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoMaoDeObra")]
        public ProdutoMaoDeObraDto ProdutoMaoDeObra { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o acréscimo aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public AcrescimoDescontoDto Acrescimo { get; set; }

        /// <summary>
        /// Obtém ou define o desconto aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public AcrescimoDescontoDto Desconto { get; set; }

        private bool AmbienteEMaoDeObra(AmbientePedido ambientePedido)
        {
            return ambientePedido.IdProd.HasValue
                && ambientePedido.Altura.HasValue
                && ambientePedido.Largura.HasValue
                && ambientePedido.Qtde.HasValue;
        }

        private IdCodigoDto RecuperaDadosProcessoAplicacao(int? id, string codigo)
        {
            return !id.HasValue
                ? null
                : new IdCodigoDto { Id = id.Value, Codigo = codigo };
        }
    }
}
