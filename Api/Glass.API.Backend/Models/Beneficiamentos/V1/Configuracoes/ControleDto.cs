// <copyright file="ControleDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Beneficiamentos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para o controle de beneficiamentos.
    /// </summary>
    [DataContract(Name = "Controle")]
    internal class ControleDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ControleDto"/>.
        /// </summary>
        public ControleDto()
        {
            this.TiposControle = new ConversorEnum<TipoControleBenef>()
                .ObterObjetoComTraducao();

            this.TiposCalculo = new ConversorEnum<TipoCalculoBenef>()
                .ObterObjetoComTraducao();

            this.EmpresaTrabalhaComAlturaELargura = PedidoConfig.EmpresaTrabalhaAlturaLargura;
            this.CobrancaOpcionalMarcada = !OrcamentoConfig.CheckBenefOpcionalDesmascadoPadrao;
        }

        /// <summary>
        /// Obtém ou define os tipos de controle de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposControle")]
        public IDictionary<string, IdNomeDto> TiposControle { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de cálculo de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposCalculo")]
        public IDictionary<string, IdNomeDto> TiposCalculo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com a ordem 'Altura X Largura'.
        /// Caso contrário, considera-se 'Largura x Altura'.
        /// </summary>
        [DataMember]
        [JsonProperty("empresaTrabalhaComAlturaELargura")]
        public bool EmpresaTrabalhaComAlturaELargura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o campo de cobrança opcional é marcado por padrão
        /// no controle de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("cobrancaOpcionalMarcada")]
        public bool CobrancaOpcionalMarcada { get; set; }
    }
}
