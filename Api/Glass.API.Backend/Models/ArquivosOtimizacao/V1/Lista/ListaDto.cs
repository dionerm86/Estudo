// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1.Lista
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
            this.Direcao = new DirecaoDto
            {
                Id = arquivoOtimizacao.Direcao,
                Descricao = arquivoOtimizacao.DescrDirecao,
            };

            this.Arquivo = new ArquivoDto
            {
                Caminho = arquivoOtimizacao.CaminhoArquivo,
                Extensao = arquivoOtimizacao.ExtensaoArquivo,
            };

            this.Funcionario = arquivoOtimizacao.NomeFunc;
            this.DataCadastro = arquivoOtimizacao.DataCad;
            this.Permissoes = new PermissoesDto
            {
                ExibirLinkDownload = Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta != Data.Helper.DataSources.TipoExportacaoEtiquetaEnum.eCutter || arquivoOtimizacao.Direcao == 2,
                ExibirLinkECutter = Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == Data.Helper.DataSources.TipoExportacaoEtiquetaEnum.eCutter && arquivoOtimizacao.Direcao == 1,
            };
        }

        /// <summary>
        /// Obtém ou define a direção do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("direcao")]
        public DirecaoDto Direcao { get; set; }

        /// <summary>
        /// Obtém ou define o arquivo do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("arquivo")]
        public ArquivoDto Arquivo { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário para a lista de arquivos de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do funcionário para a lista de arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define permissões para o download na lista de arquivos de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
