// <copyright file="TipoParticipanteDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.TiposParticipantes
{
    /// <summary>
    /// Classe que encapsula os dados de um participante fiscal.
    /// </summary>
    [DataContract(Name = "TipoParticipante")]
    public class TipoParticipanteDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TipoParticipanteDto"/>.
        /// </summary>
        /// <param name="dados">Os dados convertidos do enum.</param>
        internal TipoParticipanteDto(IdNomeDto dados)
        {
            this.Id = dados.Id;
            this.Nome = dados.Nome;
            this.UrlTelaBusca = this.ObterUrlBuscaTipoParticipante();
        }

        /// <summary>
        /// Obtém ou define a URL para busca do participante deste tipo.
        /// </summary>
        [DataMember]
        [JsonProperty("urlTelaBusca")]
        public string UrlTelaBusca { get; set; }

        private string ObterUrlBuscaTipoParticipante()
        {
            if (!this.Id.HasValue)
            {
                return null;
            }

            switch ((Data.EFD.DataSourcesEFD.TipoPartEnum)this.Id.Value)
            {
                case Data.EFD.DataSourcesEFD.TipoPartEnum.AdministradoraCartao:
                    return "../Utils/SelAdminCartao.aspx";

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Cliente:
                    return "../Utils/SelCliente.aspx";

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Fornecedor:
                    return "../Utils/SelFornec.aspx";

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Loja:
                    return "../Utils/SelLoja.aspx";

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Transportador:
                    return "../Utils/SelTransp.aspx";
            }

            throw new NotImplementedException("Tipo de participante inválido.");
        }
    }
}
