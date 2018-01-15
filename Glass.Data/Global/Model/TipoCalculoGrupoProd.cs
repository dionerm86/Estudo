using Colosoft.Globalization;
using System.Collections.Generic;
using Colosoft;

namespace Glass.Data.Model
{
    /// <summary>
    /// Tipos de cálculo que pode ser feito nos produtos
    /// </summary>
    [Colosoft.Translate(typeof(TipoCalculoGrupoProdTranslateProvider))]
    public enum TipoCalculoGrupoProd : int
    {
        /// <summary>
        /// Quantidade.
        /// </summary>
        Qtd = 1,
        /// <summary>
        /// M²
        /// </summary>
        M2,
        /// <summary>
        /// Perímetro.
        /// </summary>
        Perimetro,
        /// <summary>
        /// ML Barra 6m (0,5m).
        /// </summary>
        MLAL05,
        /// <summary>
        /// Qtd Decimal.
        /// </summary>
        QtdDecimal,   // 5
        /// <summary>
        /// ML Barra 6m (1m).
        /// </summary>
        MLAL1,
        /// <summary>
        /// ML Barra 6m (6m).
        /// </summary>
        MLAL6,
        /// <summary>
        /// ML direto
        /// </summary>
        ML,
        /// <summary>
        /// ML Barra 6m
        /// </summary>
        MLAL0,
        /// <summary>
        /// M² direto
        /// </summary>
        M2Direto,    // 10
        /// <summary>
        /// Qtd./m²
        /// </summary>
        QtdM2 = 11
    }

    /// <summary>
    /// Provedor de tradução do Tipo Calculo do grupo de produtos.
    /// </summary>
    class TipoCalculoGrupoProdTranslateProvider : Colosoft.Globalization.IMultipleTranslateProvider
    {
        #region Constantes

        private const string NOME_MLAL = "ML Barra 6m";

        #endregion

        /// <summary>
        /// Recupera as traduções do grupo de tradução informado.
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public IEnumerable<Colosoft.Globalization.TranslateInfo> GetTranslates(object groupKey)
        {
            // O retorno será da unidade do tipo de cálculo?
            if (groupKey != null)
                return new TranslateInfo[]
                {
                    new TranslateInfo(TipoCalculoGrupoProd.M2, "m²".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.ML, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.Perimetro, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL0, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL05, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL1, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL6, "ml".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.Qtd, "".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.QtdDecimal, "".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.M2Direto, "m²".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.QtdM2, "".GetFormatter()),
                };
            else
                return new TranslateInfo[]
                {
                    new TranslateInfo(TipoCalculoGrupoProd.M2, "M²".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.ML, "ML direto".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.Perimetro, "ML".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL0, NOME_MLAL.GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL05, (NOME_MLAL + " (0,5m)").GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL1, (NOME_MLAL + " (1m)").GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.MLAL6, (NOME_MLAL + " (6m)").GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.Qtd, "Qtd.".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.QtdDecimal, "Qtd. decimal".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.M2Direto, "M² direto".GetFormatter()),
                    new TranslateInfo(TipoCalculoGrupoProd.QtdM2, "Qtd./m²".GetFormatter()),
                };
        }

        /// <summary>
        /// Recupera as traduções.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.Globalization.TranslateInfo> GetTranslates()
        {
            return GetTranslates(null);
        }
    }
}
