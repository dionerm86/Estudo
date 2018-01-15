using System.Linq;

namespace Glass.Financeiro.Negocios.Componentes
{
    /// <summary>
    /// Representa o fluxo de negócio dos juros do sistema.
    /// </summary>
    public class JurosFluxo : Entidades.IProvedorJuros
    {
        #region Membros de IProvedorJuros

        /// <summary>
        /// Calcula a taxa de juros que ser aplica para o tipo de cartão.
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <param name="idLoja"></param>
        /// <param name="numeroParcelas"></param>
        /// <returns></returns>
        decimal Entidades.IProvedorJuros.CalcularTaxaJuros(Entidades.TipoCartaoCredito tipoCartao, int? idLoja, int numeroParcelas)
        {
            var itens = SourceContext.Instance.CreateQuery()
                .From<Data.Model.JurosParcelaCartao>()
                .Where("IdTipoCartao=?idTipoCartao AND NumParc=?numParc AND (IdLoja=?idLoja OR IdLoja IS NULL)")
                .Add("?idTipoCartao", tipoCartao.IdTipoCartao)
                .Add("?numParc", numeroParcelas)
                .Add("?idLoja", idLoja)
                .Select("Juros, IdLoja")
                .Execute()
                .Select(f => new 
                { 
                    Juros = f.GetDecimal("Juros"), 
                    IdLoja = f.IsDBNull("IdLoja") ? null : (int?)f.GetInt32("Juros") 
                })
                .ToArray();

            var item = itens.FirstOrDefault(f => f.IdLoja == idLoja) ?? itens.FirstOrDefault(f => !f.IdLoja.HasValue);

            return item != null ? item.Juros : 0m;
        }

        #endregion
    }
}
