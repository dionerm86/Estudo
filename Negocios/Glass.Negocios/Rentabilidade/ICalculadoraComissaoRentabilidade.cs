using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios
{
    /// <summary>
    /// Representa o resultado do calculo dos valores da 
    /// rentabilidade sobre a comissão.
    /// </summary>
    public class ResultadoComissaoRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Identifica se o valor da comissão aplicado é válido.
        /// </summary>
        public bool Valido { get; }

        /// <summary>
        /// Percentual da comissão que deve ser aplicado.
        /// </summary>
        public decimal PercentualComissao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="valido"></param>
        /// <param name="percentualComissao"></param>
        public ResultadoComissaoRentabilidade(bool valido, decimal percentualComissao)
        {
            Valido = valido;
            PercentualComissao = percentualComissao;
        }

        #endregion
    }

    /// <summary>
    /// Assinatura da calculadora da comissão pelo percentual de rentabilidade.
    /// </summary>
    public interface ICalculadoraComissaoRentabilidade
    {
        /// <summary>
        /// Realiza o cálculo dos valores da rentabilidade sobre a comissão.
        /// </summary>
        /// <param name="itemRentabilidade">Item onde será aplicado o cálculo.</param>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <param name="idFunc">Identificador do funcionário que criou o item.</param>
        /// <param name="forcaPercentualComissao">Identifica se é para forçar a atualização do percentual de comissão.</param>
        /// <returns></returns>
        ResultadoComissaoRentabilidade Calcular(IItemRentabilidade itemRentabilidade, int idLoja, int? idFunc, bool forcaPercentualComissao);

        /// <summary>
        /// Força a atualização dos dados da calculadora.
        /// </summary>
        void AtualizarDados();
    }
}
