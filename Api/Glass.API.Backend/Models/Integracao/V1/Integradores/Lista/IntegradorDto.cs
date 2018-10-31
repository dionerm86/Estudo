// <copyright file="IntegradorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um integradores para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Integrador")]
    public class IntegradorDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegradorDto"/>.
        /// </summary>
        /// <param name="integrador">Integrador cujo os dados serão encapsulados.</param>
        internal IntegradorDto(Glass.Integracao.IIntegrador integrador)
        {
            this.Nome = integrador.Nome;
            this.Ativo = integrador.Ativo;
            this.Configuracao = integrador.Configuracao.NomesParametro
                .Select(f => new ConfiguracaoIntegradorDto(f, integrador.Configuracao[f], integrador.Configuracao.VerificarSomenteLeitura(f)));
            this.Operacoes = integrador.Operacoes.Select(f => new OperacaoIntegracaoDto(f)).ToList();
            this.Jobs = integrador.Jobs.Select(f => new JobIntegracaoDto(f)).ToList();
            this.EsquemaHistorico = integrador.EsquemaHistorico != null ? new EsquemaHistoricoDto(integrador.EsquemaHistorico) : null;
        }

        /// <summary>
        /// Obtém o nome do integrador.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém um valor que indica se o integrador está ativo.
        /// </summary>
        [DataMember]
        [JsonProperty("ativo")]
        public bool Ativo { get; }

        /// <summary>
        /// Obtém as entradas de configuração do integrador.
        /// </summary>
        [DataMember]
        [JsonProperty("configuracao")]
        public IEnumerable<ConfiguracaoIntegradorDto> Configuracao { get; }

        /// <summary>
        /// Obtém as operações do integrador.
        /// </summary>
        [DataMember]
        [JsonProperty("operacoes")]
        public IEnumerable<OperacaoIntegracaoDto> Operacoes { get; }

        /// <summary>
        /// Obtém os jobs do integrador.
        /// </summary>
        [DataMember]
        [JsonProperty("jobs")]
        public IEnumerable<JobIntegracaoDto> Jobs { get; }

        /// <summary>
        /// Obtém o esquema do histórico.
        /// </summary>
        [DataMember]
        [JsonProperty("esquemaHistorico")]
        public EsquemaHistoricoDto EsquemaHistorico { get; }
    }
}
