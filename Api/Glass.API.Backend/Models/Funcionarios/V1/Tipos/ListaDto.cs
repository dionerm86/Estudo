// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Tipos
{
    /// <summary>
    /// Classe que encapsula os dados de um tipo de funcionário para a tela de listagem.
    /// </summary>
    [DataContract(Name = "TipoFuncionario")]
    public class ListaDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="tipoFuncionario">A entidade de tipos de funcionário.</param>
        internal ListaDto(TipoFuncionario tipoFuncionario)
        {
            this.Id = tipoFuncionario.IdTipoFuncionario;
            this.Descricao = tipoFuncionario.Descricao;

            this.Permissoes = new PermissoesDto
            {
                Excluir = tipoFuncionario.TipoSistema,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Glass.Data.Model.LogAlteracao.TabelaAlteracao.TipoFuncionario, (uint)this.Id, null),
            };
        }

        /// <summary>
        /// Obtém ou define descrição do tipo de funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao tipo de funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}