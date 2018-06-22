using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do container de itens de imposto.
    /// </summary>
    public interface IItemImpostoContainer
    {
        #region Propriedades

        /// <summary>
        /// Identificador da nota fiscal.
        /// </summary>
        int? IdNf { get; }

        /// <summary>
        /// Cliente.
        /// </summary>
        Global.Negocios.Entidades.Cliente Cliente { get; }

        /// <summary>
        /// Loja.
        /// </summary>
        Global.Negocios.Entidades.Loja Loja { get; }

        /// <summary>
        /// Fornecedor.
        /// </summary>
        Global.Negocios.Entidades.Fornecedor Fornecedor { get; }

        /// <summary>
        /// Tipo de documento.
        /// </summary>
        Sync.Fiscal.Enumeracao.NFe.TipoDocumento TipoDocumento { get; }

        /// <summary>
        /// Finalidade de emissão
        /// </summary>
        Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao FinalidadeEmissao { get; }

        /// <summary>
        /// Valor do frete
        /// </summary>
        decimal ValorFrete { get; }

        /// <summary>
        /// Modalidade do frete
        /// </summary>
        Data.Model.ModalidadeFrete? ModalidadeFrete { get; }

        /// <summary>
        /// Valor do seguro.
        /// </summary>
        decimal ValorSeguro { get; }

        /// <summary>
        /// Outras Despesas
        /// </summary>
        decimal OutrasDespesas { get; }

        /// <summary>
        /// Valor do IPI devolvido
        /// </summary>
        decimal ValorIpiDevolvido { get; }

        /// <summary>
        /// Valor do desconto
        /// </summary>
        decimal ValorDesconto { get; }

        /// <summary>
        /// Indica se a nota fiscal está sendo importada pelo sistema.
        /// </summary>
        bool NotaFiscalImportadaSistema { get; }

        /// <summary>
        /// Identifica se deve calcular a aliquota do ICMS ST.
        /// </summary>
        bool CalcularAliquotaIcmsSt { get; }

        /// <summary>
        /// Itens filhos.
        /// </summary>
        IEnumerable<IItemImposto> Itens { get; }

        #endregion
    }
}
