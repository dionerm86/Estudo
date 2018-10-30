// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Fiscal.Negocios.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma regra de natureza de operação para a tela de listagem.
    /// </summary>
    [DataContract(Name = "RegraNaturezaOperacao")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="regra">A model de regra de natureza de operação.</param>
        internal ListaDto(RegraNaturezaOperacaoPesquisa regra)
        {
            this.Id = regra.IdRegraNaturezaOperacao;
            this.Loja = new IdNomeDto
            {
                Id = regra.IdLoja,
                Nome = regra.NomeLoja,
            };

            this.TipoCliente = new IdNomeDto
            {
                Id = regra.IdTipoCliente,
                Nome = regra.DescricaoTipoCliente,
            };

            this.Produto = new ProdutoDto
            {
                GrupoProduto = new IdNomeDto
                {
                    Id = regra.IdGrupoProd,
                    Nome = regra.DescricaoGrupoProduto,
                },

                SubgrupoProduto = new IdNomeDto
                {
                    Id = regra.IdSubgrupoProd,
                    Nome = regra.DescricaoSubgrupoProduto,
                },

                Cores = new CoresDto
                {
                    Vidro = new IdNomeDto
                    {
                        Id = regra.IdCorVidro,
                        Nome = regra.DescricaoCorVidro,
                    },

                    Ferragem = new IdNomeDto
                    {
                        Id = regra.IdCorFerragem,
                        Nome = regra.DescricaoCorFerragem,
                    },

                    Aluminio = new IdNomeDto
                    {
                        Id = regra.IdCorAluminio,
                        Nome = regra.DescricaoCorAluminio,
                    },
                },
            };

            this.UfsDestino = !string.IsNullOrEmpty(regra.UfDest) ? regra.UfDest.Split(',') : null;
            this.NaturezaOperacaoProducao = new NaturezasOperacaoDto
            {
                Intraestadual = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoProdIntra,
                    Nome = regra.DescricaoNaturezaOperacaoProducaoIntra,
                },

                Interestadual = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoProdInter,
                    Nome = regra.DescricaoNaturezaOperacaoProducaoInter,
                },

                IntraestadualComSt = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoProdStIntra,
                    Nome = regra.DescricaoNaturezaOperacaoProducaoStIntra,
                },

                InterestadualComSt = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoProdStInter,
                    Nome = regra.DescricaoNaturezaOperacaoProducaoStInter,
                },
            };

            this.NaturezaOperacaoRevenda = new NaturezasOperacaoDto
            {
                Intraestadual = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoRevIntra,
                    Nome = regra.DescricaoNaturezaOperacaoRevendaIntra,
                },

                Interestadual = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoRevInter,
                    Nome = regra.DescricaoNaturezaOperacaoRevendaInter,
                },

                IntraestadualComSt = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoRevStInter,
                    Nome = regra.DescricaoNaturezaOperacaoRevendaStIntra,
                },

                InterestadualComSt = new IdNomeDto
                {
                    Id = regra.IdNaturezaOperacaoRevStIntra,
                    Nome = regra.DescricaoNaturezaOperacaoRevendaStInter,
                },
            };

            this.Permissoes = new PermissoesDto()
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.RegraNaturezaOperacao, (uint)regra.IdRegraNaturezaOperacao, null),
            };
        }

        /// <summary>
        /// Obtém ou define dados da loja da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cliente da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCliente")]
        public IdNomeDto TipoCliente { get; set; }

        /// <summary>
        /// Obtém ou define dados de produto associados à regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define dados de produto associados à regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ufsDestino")]
        public IEnumerable<string> UfsDestino { get; set; }

        /// <summary>
        /// Obtém ou define naturezas de operação de produção associadas à regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("naturezaOperacaoProducao")]
        public NaturezasOperacaoDto NaturezaOperacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define naturezas de operação de revenda associadas à regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("naturezaOperacaoRevenda")]
        public NaturezasOperacaoDto NaturezaOperacaoRevenda { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
