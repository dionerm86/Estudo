// <copyright file="ContagemPecasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.ContagemPecas
{
    /// <summary>
    /// Classe que encapsula o resultado da contagem de peças por situação.
    /// </summary>
    [DataContract(Name = "ContagemPecas")]
    public class ContagemPecasDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContagemPecasDto"/>.
        /// </summary>
        /// <param name="contagem">A contagem realizada das peças.</param>
        public ContagemPecasDto(ProdutoPedidoProducaoDAO.ContagemPecas contagem)
        {
            this.Prontas = this.ObterContagemPorTipo(contagem.Prontas, contagem.TotMProntas, contagem.TotMProntasCalc);
            this.Pendentes = this.ObterContagemPorTipo(contagem.Pendentes, contagem.TotMPendentes, contagem.TotMPendentesCalc);
            this.Entregues = this.ObterContagemPorTipo(contagem.Entregues, contagem.TotMEntregues, contagem.TotMEntreguesCalc);
            this.Perdidas = this.ObterContagemPorTipo(contagem.Perdidas, contagem.TotMPerdidas, contagem.TotMPerdidasCalc);
            this.Canceladas = this.ObterContagemPorTipo(contagem.Canceladas, contagem.TotMCanceladas, contagem.TotMCanceladasCalc);
        }

        /// <summary>
        /// Obtém ou define a quantidade de peças prontas.
        /// </summary>
        [DataMember]
        [JsonProperty("prontas")]
        public DadosContagemDto Prontas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças pendentes.
        /// </summary>
        [DataMember]
        [JsonProperty("pendentes")]
        public DadosContagemDto Pendentes { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças entregues.
        /// </summary>
        [DataMember]
        [JsonProperty("entregues")]
        public DadosContagemDto Entregues { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças perdidas.
        /// </summary>
        [DataMember]
        [JsonProperty("perdidas")]
        public DadosContagemDto Perdidas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças canceladas.
        /// </summary>
        [DataMember]
        [JsonProperty("canceladas")]
        public DadosContagemDto Canceladas { get; set; }

        private DadosContagemDto ObterContagemPorTipo(long numero, double areaReal, double areaParaCalculo)
        {
            return new DadosContagemDto
            {
                Numero = numero,
                AreaEmM2 = new TotalAreaM2Dto
                {
                    Real = (decimal)areaReal,
                    ParaCalculo = (decimal)areaParaCalculo,
                },
            };
        }
    }
}
