// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PedidosConferencia.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de pedidos em conferencia.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ExibirRentabilidade = RentabilidadeConfig.ExibirRentabilidadePedidoEspelho;
            this.SituacaoCncProjetado = PedidoEspelho.SituacaoCncEnum.Projetado;
            this.SituacaoCncSemNecessidadeNaoConferido = PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido;
            this.UsarControleGerenciamentoProjetoCnc = PCPConfig.UsarControleGerenciamentoProjCnc;
            this.GerarArquivoDxf = PCPConfig.EmpresaGeraArquivoDxf;
            this.GerarArquivoFml = PCPConfig.EmpresaGeraArquivoFml;
            this.GerarArquivoSGlass = PCPConfig.EmpresaGeraArquivoSGlass;
            this.GerarArquivoIntermac = PCPConfig.EmpresaGeraArquivoIntermac;
            this.PermitirImpressaoDePedidosImportadosApenasConferidos = PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o controle de rentabilidade será exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRentabilidade")]
        public bool ExibirRentabilidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o valor da situação "CNC projetado".
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoCncProjetado")]
        public PedidoEspelho.SituacaoCncEnum SituacaoCncProjetado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o valor da situação "CNC sem necessidade não conferido".
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoCncSemNecessidadeNaoConferido")]
        public PedidoEspelho.SituacaoCncEnum SituacaoCncSemNecessidadeNaoConferido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o controle de gerenciamento de projeto de CNC será usado.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleGerenciamentoProjetoCnc")]
        public bool UsarControleGerenciamentoProjetoCnc { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo DXF.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDxf")]
        public bool GerarArquivoDxf { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo FML.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoFml")]
        public bool GerarArquivoFml { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo SGlass.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoSGlass")]
        public bool GerarArquivoSGlass { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo Intermac.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoIntermac")]
        public bool GerarArquivoIntermac { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitida a impressão de pedidos importados apenas se tiverem sido conferidos.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirImpressaoDePedidosImportadosApenasConferidos")]
        public bool PermitirImpressaoDePedidosImportadosApenasConferidos { get; set; }
    }
}
