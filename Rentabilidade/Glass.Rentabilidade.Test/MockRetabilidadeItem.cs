using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Test
{
    class MockRetabilidadeItem : IItemRentabilidade
    {
        public string Descricao { get; set; }

        public decimal FatorICMSSubstituicao { get; set; }

        public decimal PercentualComissao { get; set; }

        public decimal PercentualICMSCompra { get; set; }

        public decimal PercentualICMSVenda { get; set; }

        public decimal PercentualIPICompra { get; set; }

        public decimal PercentualIPIVenda { get; set; }

        public int PrazoMedio { get; set; }

        public decimal PrecoCusto { get; set; }

        public decimal PrecoVendaSemIPI { get; set; }

        public decimal CustosExtras { get; set; }

        /// <summary>
        /// Percentual de rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Rentabilidade financeira.
        /// </summary>
        public decimal RentabilidadeFinanceira { get; set; }

        /// <summary>
        /// Registros de rentabilidade associados.
        /// </summary>
        public IEnumerable<IRegistroRentabilidade> RegistrosRentabilidade => new IRegistroRentabilidade[0];

        /// <summary>
        /// Cria uma instancia do registro.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="nome">Nome do registro.</param>
        /// <param name="valor">Valor do registro</param>
        /// <returns></returns>
        public IRegistroRentabilidade CriarRegistro(TipoRegistroRentabilidade tipo, string nome, decimal valor) => null;

        /// <summary>
        /// Limpa os registros
        /// </summary>
        public void LimparRegistros()
        {   
        }
    }
}
