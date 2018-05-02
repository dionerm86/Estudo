using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Argumentos do evento acionado quando o cálculo da rentabilidade for salvo.
    /// </summary>
    public class SalvarCalculoRentabilidadeEventArgs : EventArgs
    {
        #region Propriedades

        /// <summary>
        /// Sessão de persistencia do GDA.
        /// </summary>
        public GDA.GDASession Sessao { get; }

        /// <summary>
        /// Percentual de rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade { get; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        public decimal RentabilidadeFinanceira { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="percentualRentabilidade"></param>
        /// <param name="rentabilidadeFinanceira"></param>
        public SalvarCalculoRentabilidadeEventArgs(
            GDA.GDASession sessao, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            Sessao = sessao;
            PercentualRentabilidade = percentualRentabilidade;
            RentabilidadeFinanceira = rentabilidadeFinanceira;
        }

        #endregion

    }

    /// <summary>
    /// Assinatuar do resultado do calculo de rentabilidade.
    /// </summary>
    public interface ICalculoRentabilidadeResultado
    {
        #region Events

        /// <summary>
        /// Evento acionado quando o resultado estiver sendo salvo.
        /// </summary>
        event EventHandler<Data.SalvarCalculoRentabilidadeEventArgs> Salvando;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identifica se o calculo foi executado.
        /// </summary>
        bool Executado { get; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        decimal PercentualRentabilidade { get; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        decimal RentabilidadeFinanceira { get; }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados do resultado.
        /// </summary>
        /// <param name="sessao"></param>
        void Salvar(GDA.GDASession sessao);

        #endregion
    }
}
