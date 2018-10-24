// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Roteiros.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de roteiros.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="roteiro">O roteiro que será retornado.</param>
        public ListaDto(RoteiroProducao roteiro)
        {
            this.Id = roteiro.IdRoteiroProducao;
            this.Processo = roteiro.CodProcesso;
            this.Setores = roteiro.Setores?.Split(',').Select(f => f.Trim());
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    LogAlteracao.TabelaAlteracao.RoteiroProducao,
                    (uint)roteiro.IdRoteiroProducao,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define o processo do roteiro.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public string Processo { get; set; }

        /// <summary>
        /// Obtém ou define os setores do roteiro.
        /// </summary>
        [DataMember]
        [JsonProperty("setores")]
        public IEnumerable<string> Setores { get; set; }

        /// <summary>
        /// Obtém ou define permissões do roteiro.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
