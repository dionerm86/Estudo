// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Transportadores.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um transportador para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Transportador")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="transportador">A model de transportador.</param>
        internal ListaDto(Global.Negocios.Entidades.Transportador transportador)
        {
            this.Id = transportador.IdTransportador;
            this.RazaoSocial = transportador.Nome;
            this.NomeFantasia = transportador.NomeFantasia;
            this.CpfCnpj = transportador.CpfCnpj;
            this.Placa = transportador.Placa;
            this.Telefone = transportador.Telefone;

            this.Permissoes = new PermissoesDto()
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Transportador, (uint)transportador.IdTransportador, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a razão social do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("razaoSocial")]
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Obtém ou define o nome fantasia do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeFantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Obtém ou define o CPF/CNPJ do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define a placa do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("placa")]
        public string Placa { get; set; }

        /// <summary>
        /// Obtém ou define o telefone do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
