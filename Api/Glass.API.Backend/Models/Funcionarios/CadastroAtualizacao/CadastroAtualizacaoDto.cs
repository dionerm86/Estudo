// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Funcionarios.Detalhe;
using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização do funcionário.
    /// </summary>
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do tipo do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoFuncionario")]
        public int IdTipoFuncionario
        {
            get { return this.ObterValor(c => c.IdTipoFuncionario); }
            set { this.AdicionarValor(c => c.IdTipoFuncionario, value); }
        }

        /// <summary>
        /// Obtém ou define os identificadores dos setores.
        /// </summary>
        [DataMember]
        [JsonProperty("idsSetores")]
        public IEnumerable<int> IdsSetores
        {
            get { return this.ObterValor(c => c.IdsSetores); }
            set { this.AdicionarValor(c => c.IdsSetores, value); }
        }

        /// <summary>
        /// Obtém ou define o endereço do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public Genericas.EnderecoDto Endereco
        {
            get { return this.ObterValor(c => c.Endereco); }
            set { this.AdicionarValor(c => c.Endereco, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int IdLoja
        {
            get { return this.ObterValor(c => c.IdLoja); }
            set { this.AdicionarValor(c => c.IdLoja, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("contatos")]
        public ContatosDto Contatos
        {
            get { return this.ObterValor(c => c.Contatos); }
            set { this.AdicionarValor(c => c.Contatos, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de documentação e pessoais do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("documentosEDadosPessoais")]
        public DocumentosEDadosPessoaisDto DocumentosEDadosPessoais
        {
            get { return this.ObterValor(c => c.DocumentosEDadosPessoais); }
            set { this.AdicionarValor(c => c.DocumentosEDadosPessoais, value); }
        }

        /// <summary>
        /// Obtém ou define a situacao do funcionários.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Glass.Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de acesso do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("acesso")]
        public AcessoDto Acesso
        {
            get { return this.ObterValor(c => c.Acesso); }
            set { this.AdicionarValor(c => c.Acesso, value); }
        }

        /// <summary>
        /// Obtém ou define os identificadores do tipos de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idsTiposPedidos")]
        public IEnumerable<int> IdsTiposPedidos
        {
            get { return this.ObterValor(c => c.IdsTiposPedidos); }
            set { this.AdicionarValor(c => c.IdsTiposPedidos, value); }
        }

        /// <summary>
        /// Obtém ou define o numero de dias para atrasar pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasParaAtrasarPedidos")]
        public int NumeroDiasParaAtrasarPedidos
        {
            get { return this.ObterValor(c => c.NumeroDiasParaAtrasarPedidos); }
            set { this.AdicionarValor(c => c.NumeroDiasParaAtrasarPedidos, value); }
        }

        /// <summary>
        /// Obtém ou define o numero do ponto de venda (frente de caixa).
        /// </summary>
        [DataMember]
        [JsonProperty("numeroPdv")]
        public string NumeroPdv
        {
            get { return this.ObterValor(c => c.NumeroPdv); }
            set { this.AdicionarValor(c => c.NumeroPdv, value); }
        }

        /// <summary>
        /// Obtém ou define as permissões do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermisoesDto Permissoes
        {
            get { return this.ObterValor(c => c.Permissoes); }
            set { this.AdicionarValor(c => c.Permissoes, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("string")]
        public string Obsercavao
        {
            get { return this.ObterValor(c => c.Obsercavao); }
            set { this.AdicionarValor(c => c.Obsercavao, value); }
        }
    }
}
