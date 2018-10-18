// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de notas fiscais.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.AtivarContingencia = Config.PossuiPermissao(Config.FuncaoMenuFiscal.AtivarContingenciaNFe);
            this.UfUsuario = UserInfo.GetUserInfo.UfLoja;
            this.QuantidadeNotasFsda = NotaFiscalDAO.Instance.GetCountEmitirFs();
            this.TipoContingenciaScan = DataSources.TipoContingenciaNFe.SCAN;
            this.TipoContingenciaFsda = DataSources.TipoContingenciaNFe.FSDA;
            this.TipoContingenciaNaoUtilizar = DataSources.TipoContingenciaNFe.NaoUtilizar;
            this.SituacaoFinalizada = NotaFiscal.SituacaoEnum.FinalizadaTerceiros;
            this.SituacaoAutorizada = NotaFiscal.SituacaoEnum.Autorizada;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário logado possui permissão de ativar contingência de nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("ativarContingencia")]
        public bool AtivarContingencia { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica a UF do usuário logado.
        /// </summary>
        [DataMember]
        [JsonProperty("ufUsuario")]
        public string UfUsuario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica a quantidade de notas FS-DA.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeNotasFsda")]
        public int QuantidadeNotasFsda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o tipo de contingência SCAN.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoContingenciaScan")]
        public DataSources.TipoContingenciaNFe TipoContingenciaScan { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o tipo de contingência FS-DA.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoContingenciaFsda")]
        public DataSources.TipoContingenciaNFe TipoContingenciaFsda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o tipo de contingência "Não Utilizar".
        /// </summary>
        [DataMember]
        [JsonProperty("tipoContingenciaNaoUtilizar")]
        public DataSources.TipoContingenciaNFe TipoContingenciaNaoUtilizar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual a situação finalizada da nota.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoFinalizada")]
        public NotaFiscal.SituacaoEnum SituacaoFinalizada { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual a situação autorizada da nota.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoAutorizada")]
        public NotaFiscal.SituacaoEnum SituacaoAutorizada { get; set; }
    }
}
