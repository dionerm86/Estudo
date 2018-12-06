using Sync.Fiscal.EFD.Entidade;
using System;

namespace Glass.Data.RelModel
{
    /// <summary>
    /// Classe criada para preencher os dados dos insumos, itens produzidos, para a geração do EFD.
    /// </summary>
    public class InsumoConsumidoEFD : IInsumoConsumido
    {
        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        public int CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a data de saída do estoque.
        /// </summary>
        public DateTime DataSaidaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade consumida.
        /// </summary>
        public decimal QtdeConsumida { get; set; }
    }
}
