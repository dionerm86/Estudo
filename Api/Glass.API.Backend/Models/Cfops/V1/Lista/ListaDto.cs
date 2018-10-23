// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Fiscal.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um CFOP para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Cfop")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="cfop">A model de CFOP's.</param>
        internal ListaDto(CfopPesquisa cfop)
        {
            this.Id = cfop.IdCfop;
            this.Nome = cfop.Descricao;
            this.Codigo = cfop.CodInterno;

            this.TipoMercadoria = new IdNomeDto()
            {
                Id = (int)cfop.TipoMercadoria.GetValueOrDefault(),
                Nome = Colosoft.Translator.Translate(cfop.TipoMercadoria.GetValueOrDefault()).Format(),
            };

            this.AlterarEstoqueTerceiros = cfop.AlterarEstoqueTerceiros;
            this.AlterarEstoqueCliente = cfop.AlterarEstoqueCliente;
            this.Obs = cfop.Obs;

            this.TipoCfop = new TipoCfopDto()
            {
                IdTipoCfop = cfop.IdTipoCfop,
                Descricao = cfop.Tipo,
            };

            this.Permissoes = new PermissoesDto()
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Cfop, (uint)cfop.IdCfop, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define dados do tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCfop")]
        public TipoCfopDto TipoCfop { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoMercadoria")]
        public IdNomeDto TipoMercadoria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque de terceiros deve ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueTerceiros")]
        public bool AlterarEstoqueTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque do cliente deve ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueCliente")]
        public bool AlterarEstoqueCliente { get; set; }

        /// <summary>
        /// Obtém ou define a observação do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("obs")]
        public string Obs { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
