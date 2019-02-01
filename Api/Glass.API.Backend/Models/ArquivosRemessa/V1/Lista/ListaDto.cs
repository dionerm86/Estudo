// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosRemessa.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de arquivos de remessa.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="arquivoRemessa">A model de arquivo de remessa preenchida.</param>
        public ListaDto(ArquivoRemessa arquivoRemessa)
        {
            this.Id = (int)arquivoRemessa.IdArquivoRemessa;
            this.NumeroArquivoRemessa = arquivoRemessa.NumRemessa ?? 0;
            this.Tipo = arquivoRemessa.Tipo.ToString();
            this.UsuarioCadastro = arquivoRemessa.DescrUsuCad;
            this.DataCadastro = arquivoRemessa.DataCad;
            this.Situacao = arquivoRemessa.Situacao.ToString();
            this.CorLinha = this.ObterCorLinha(arquivoRemessa);
            this.Permissoes = new PermissoesDto
            {
                Excluir = arquivoRemessa.DeletarVisivel,
                LogImportacao = arquivoRemessa.LogVisivel,
            };
        }

        /// <summary>
        /// Obtém ou define o número do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroArquivoRemessa")]
        public int? NumeroArquivoRemessa { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o usuário de cadastro do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha do arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas ao arquivo de remessa.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(ArquivoRemessa arquivoRemessa)
        {
            if (arquivoRemessa.Situacao == ArquivoRemessa.SituacaoEnum.Cancelado)
            {
                return "Red";
            }

            return string.Empty;
        }
    }
}