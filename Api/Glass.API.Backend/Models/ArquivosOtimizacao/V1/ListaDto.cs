// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1
{
    /// <summary>
    /// Classe que encapsula um item para a lista de arquivos de otimização.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="arquivoOtimizacao">O arquivoOtimizacao que será retornado.</param>
        public ListaDto(ArquivoOtimizacao arquivoOtimizacao)
        {
            this.Id = (int)arquivoOtimizacao.IdArquivoOtimizacao;
            this.Direcao = new IdNomeDto
            {
                Id = (int)arquivoOtimizacao.IdArquivoOtimizacao,
                Descricao = arquivoOtimizacao.DescrDirecao,
            };
            this.Arquivo = new ArquivoOtimizacao
            {
                Caminho = arquivoOtimizacao.CaminhoArquivo,
                Extensao = arquivoOtimizacao.ExtensaoArquivo,
            };
            this.Funcionario = arquivoOtimizacao.NomeFunc;
            this.DataCadastro = arquivoOtimizacao.DataCad;
        }

        /// <summary>
        /// Obtém ou define a descrição do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("direcao")]
        public CodigoNomeDto Direcao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("arquivo")]
        public CodigoNomeDto Arquivo { get; set; }

        /// <summary>
        /// Obtém ou define o caminho do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a extensão do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }
    }
}
