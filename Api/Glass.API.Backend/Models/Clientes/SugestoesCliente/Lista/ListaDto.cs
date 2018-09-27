// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.SugestoesCliente.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma sugestão do cliente para a tela de listagem.
    /// </summary>
    [DataContract(Name = "SugestaoCliente")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="sugestaoCliente">A model de sugestões de clientes.</param>
        internal ListaDto(SugestaoClientePesquisa sugestaoCliente)
        {
            this.Id = sugestaoCliente.IdSugestao;
            this.IdPedido = sugestaoCliente.IdPedido;
            this.IdOrcamento = sugestaoCliente.IdOrcamento;
            this.DescricaoRota = sugestaoCliente.DescricaoRota;
            this.Tipo = sugestaoCliente.DescricaoTipoSugestao;
            this.Descricao = sugestaoCliente.Descricao;
            this.Situacao = sugestaoCliente.Cancelada ? "Cancelada" : "Ativa";
            this.Cliente = new IdNomeDto
            {
                Id = sugestaoCliente.IdCliente.GetValueOrDefault(),
                Nome = sugestaoCliente.Cliente,
            };
            this.Cadastro = new CadastroDto
            {
                DataCadastro = sugestaoCliente.DataCad,
                NomeFuncionario = sugestaoCliente.Funcionario,
            };
            this.Permissoes = new PermissoesDto
            {
                Cancelar = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes) && !sugestaoCliente.Cancelada,
                PodeInserirNova = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do cliente associado a sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido associado.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public uint? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento associado.
        /// </summary>
        [DataMember]
        [JsonProperty("idOrcamento")]
        public uint? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("rota")]
        public string DescricaoRota { get; set; }

        /// <summary>
        /// Obtém ou define as informações de cadastro da sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public CadastroDto Cadastro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas a sugestão do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
