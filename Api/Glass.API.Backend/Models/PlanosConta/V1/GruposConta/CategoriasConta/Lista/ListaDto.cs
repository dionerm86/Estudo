// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.CategoriasConta.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de categorias de conta.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="categoriaConta">A categoria de conta que será retornada.</param>
        public ListaDto(Financeiro.Negocios.Entidades.CategoriaConta categoriaConta)
        {
            this.Id = categoriaConta.IdCategoriaConta;
            this.Nome = categoriaConta.Descricao;
            this.Tipo = new IdNomeDto
            {
                Id = (int?)categoriaConta.Tipo,
                Nome = categoriaConta.Tipo != null ? Colosoft.Translator.Translate(categoriaConta.Tipo).Format() : string.Empty,
            };

            this.Situacao = new IdNomeDto
            {
                Id = (int)categoriaConta.Situacao,
                Nome = Colosoft.Translator.Translate(categoriaConta.Situacao).Format(),
            };

            this.NumeroSequencia = categoriaConta.NumeroSequencia;
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.CategoriaConta, (uint)categoriaConta.IdCategoriaConta, null),
            };
        }

        /// <summary>
        /// Obtém ou define o tipo da categoria de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação da categoria de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o número sequencial da categoria de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroSequencia")]
        public int NumeroSequencia { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da categoria de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
