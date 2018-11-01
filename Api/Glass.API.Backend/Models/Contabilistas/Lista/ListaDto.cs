// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Contabilistas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de contabilistas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="contabilista">O contabilista que será retornado.</param>
        public ListaDto(Contabilista contabilista)
        {
            this.Id = (int)contabilista.IdContabilista;
            this.Nome = contabilista.Nome;
            this.CpfCnpj = contabilista.CpfCnpj;
            this.Crc = contabilista.Crc;
            this.Situacao = new IdNomeDto
            {
                Id = contabilista.Situacao,
                Nome = ((Situacao)contabilista.Situacao).ToString(),
            };

            this.DadosContato = new DadosContatoDto
            {
                Telefone = contabilista.TelCont,
                Fax = contabilista.Fax,
                Email = contabilista.Email,
            };

            this.Endereco = new EnderecoDto
            {
                Logradouro = contabilista.Endereco,
                Numero = contabilista.Numero,
                Complemento = contabilista.Compl,
                Bairro = contabilista.Bairro,
                Cidade = new CidadeDto
                {
                    Id = (int)contabilista.IdCidade,
                    Nome = contabilista.NomeCidade,
                    Uf = contabilista.NomeUf,
                },

                Cep = contabilista.Cep,
            };
        }

        /// <summary>
        /// Obtém ou define o tipo de pessoa do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPessoa")]
        public string TipoPessoa { get; set; }

        /// <summary>
        /// Obtém ou define o CNPJ do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o CRC do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("crc")]
        public string Crc { get; set; }

        /// <summary>
        /// Obtém ou define a situação do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define dados de contato do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosContato")]
        public DadosContatoDto DadosContato { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public EnderecoDto Endereco { get; set; }
    }
}
