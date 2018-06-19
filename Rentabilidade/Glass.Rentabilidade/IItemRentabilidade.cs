using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa um item sobre o qual a rentabilidade é calculada.
    /// </summary>
    public interface IItemRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Descrição do item.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Preço de venda sem o IPI.
        /// </summary>
        decimal PrecoVendaSemIPI { get; }

        /// <summary>
        /// Preço de custo do item.
        /// </summary>
        decimal PrecoCusto { get; }

        /// <summary>
        /// Prazo média em dias.
        /// </summary>
        int PrazoMedio { get; }

        /// <summary>
        /// Percentual do ICMS de compra.
        /// </summary>
        decimal PercentualICMSCompra { get; }

        /// <summary>
        /// Percentual do ICMS de venda.
        /// </summary>
        decimal PercentualICMSVenda { get; }

        /// <summary>
        /// Fator do ICMS de substituição.
        /// </summary>
        decimal FatorICMSSubstituicao { get; }

        /// <summary>
        /// Percentual do IPI de compra.
        /// </summary>
        decimal PercentualIPICompra { get; }

        /// <summary>
        /// Percentual do IPI de venda.
        /// </summary>
        decimal PercentualIPIVenda { get; }

        /// <summary>
        /// Percentual de comissão.
        /// </summary>
        decimal PercentualComissao { get; }

        /// <summary>
        /// Valor da comissão.
        /// </summary>
        decimal ValorComissao { get; }

        /// <summary>
        /// Custos extras.
        /// </summary>
        decimal CustosExtras { get; }

        /// <summary>
        /// Percentual de rentabilidade.
        /// </summary>
        decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        decimal RentabilidadeFinanceira { get; set; }

        /// <summary>
        /// Registros de rentabilidade associados.
        /// </summary>
        IEnumerable<IRegistroRentabilidade> RegistrosRentabilidade { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Limpa os registros de rentabilidade do item.
        /// </summary>
        void LimparRegistros();

        /// <summary>
        /// Cria uma instancia do registro.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="nome">Nome do registro.</param>
        /// <param name="valor">Valor do registro</param>
        /// <returns></returns>
        IRegistroRentabilidade CriarRegistro(TipoRegistroRentabilidade tipo, string nome, decimal valor);

        #endregion
    }
}
