// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Rotas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de rotas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="rota">A rota que será retornada.</param>
        public ListaDto(RotaPesquisa rota)
        {
            this.Id = rota.IdRota;
            this.Nome = rota.Descricao;
            this.Codigo = rota.CodInterno;
            this.Situacao = Translator.Translate(rota.Situacao).Format();
            this.Distancia = rota.Distancia;
            this.DiasSemana = Translator.Translate(rota.DiasSemana).Format();
            this.MinimoDiasEntrega = rota.NumeroMinimoDiasEntrega;
            this.Observacao = rota.Obs;
            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    Data.Model.LogAlteracao.TabelaAlteracao.Rota,
                    (uint)rota.IdRota,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define o código da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a situação da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a distância da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public int Distancia { get; set; }

        /// <summary>
        /// Obtém ou define os dias de semana da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("diasSemana")]
        public string DiasSemana { get; set; }

        /// <summary>
        /// Obtém ou define o mínimo de dias para entrega na rota.
        /// </summary>
        [DataMember]
        [JsonProperty("minimoDiasEntrega")]
        public int MinimoDiasEntrega { get; set; }

        /// <summary>
        /// Obtém ou define a observação da rota.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para a rota.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
