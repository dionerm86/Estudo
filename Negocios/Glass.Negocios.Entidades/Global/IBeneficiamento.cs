using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura dos classes de beneficiamento.
    /// </summary>
    public interface IBeneficiamento : IEquatable<IBeneficiamento>
    {
        #region Propriedades

        /// <summary>
        /// Tipo do produto do beneficiamento.
        /// </summary>
        Glass.Data.TipoProdutoBeneficiamento TipoProdutoBenef { get; }

        /// <summary>
        /// Identificador da configuração do beneficiamento.
        /// </summary>
        int IdBenefConfig { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        int Qtd { get; set; }

        /// <summary>
        /// Valor unitário.
        /// </summary>
        decimal ValorUnit { get; set; }

        /// <summary>
        /// Valor.
        /// </summary>
        decimal Valor { get; set; }

        /// <summary>
        /// Custo.
        /// </summary>
        decimal Custo { get; set; }

        /// <summary>
        /// Largura da lapidação.
        /// </summary>
        int LapLarg { get; set; }

        /// <summary>
        /// Altura da lapidação.
        /// </summary>
        int LapAlt { get; set; }

        /// <summary>
        /// Largura do bisotado.
        /// </summary>
        int BisLarg { get; set; }

        /// <summary>
        /// Altura do bisotado.
        /// </summary>
        int BisAlt { get; set; }

        /// <summary>
        /// Espessura do Bisote.
        /// </summary>
        float EspBisote { get; set; }

        /// <summary>
        /// Espessura do furo.
        /// </summary>
        int EspFuro { get; set; }

        /// <summary>
        /// Padrão.
        /// </summary>
        bool Padrao { get; set; }

        /// <summary>
        /// Valor da comissão.
        /// </summary>
        decimal ValorComissao { get; set; }

        /// <summary>
        /// Valor do acrescimo.
        /// </summary>
        decimal ValorAcrescimo { get; set; }

        /// <summary>
        /// Valor do acrescimo
        /// </summary>
        decimal ValorAcrescimoProd { get; set; }

        /// <summary>
        /// Valor do desconto.
        /// </summary>
        decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor do desconto.
        /// </summary>
        decimal ValorDescontoProd { get; set; }

        #endregion
    }
}
