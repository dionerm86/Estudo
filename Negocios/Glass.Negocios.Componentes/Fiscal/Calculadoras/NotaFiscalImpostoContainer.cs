using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação do container de itens de imposto da nota fiscal.
    /// </summary>
    class NotaFiscalImpostoContainer : IItemImpostoContainer
    {
        #region Propriedades

        /// <summary>
        /// Pedido.
        /// </summary>
        public Data.Model.NotaFiscal NotaFiscal { get; }

        /// <summary>
        /// Identificador da nota fiscal.
        /// </summary>
        public int? IdNf => (int)NotaFiscal.IdNf;

        /// <summary>
        /// Cliente.
        /// </summary>
        public Global.Negocios.Entidades.Cliente Cliente { get; }

        /// <summary>
        /// Loja.
        /// </summary>
        public Global.Negocios.Entidades.Loja Loja { get; }

        /// <summary>
        /// Fornecedor.
        /// </summary>
        public Global.Negocios.Entidades.Fornecedor Fornecedor { get; }

        /// <summary>
        /// Tipo de documento.
        /// </summary>
        public Sync.Fiscal.Enumeracao.NFe.TipoDocumento TipoDocumento => 
            (Sync.Fiscal.Enumeracao.NFe.TipoDocumento)NotaFiscal.TipoDocumento;

        /// <summary>
        /// Finalidade de emissão
        /// </summary>
        public Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao FinalidadeEmissao => 
            (Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao)NotaFiscal.FinalidadeEmissao;

        /// <summary>
        /// Valor do frete
        /// </summary>
        public decimal ValorFrete => NotaFiscal.ValorFrete;

        /// <summary>
        /// Modalidade do frete
        /// </summary>
        public Data.Model.ModalidadeFrete? ModalidadeFrete => NotaFiscal.ModalidadeFrete;

        /// <summary>
        /// Valor do seguro.
        /// </summary>
        public decimal ValorSeguro => NotaFiscal.ValorSeguro;

        /// <summary>
        /// Valor do desconto
        /// </summary>
        public decimal ValorDesconto => NotaFiscal.Desconto;

        /// <summary>
        /// Outras Despesas
        /// </summary>
        public decimal OutrasDespesas => NotaFiscal.OutrasDespesas;

        /// <summary>
        /// Valor do IPI devolvido
        /// </summary>
        public decimal ValorIpiDevolvido => NotaFiscal.ValorIpiDevolvido;

        /// <summary>
        /// Identifica se deve calcular a aliquota do ICMS.
        /// </summary>
        public bool CalcularAliquotaIcmsSt => true;

        /// <summary>
        /// Indica se a nota fiscal está sendo importada pelo sistema.
        /// </summary>
        public bool NotaFiscalImportadaSistema =>
            TipoDocumento == Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Entrada &&
            Fornecedor?.Cidade?.CodIbgeCidade == "99999";

        /// <summary>
        /// Itens filhos.
        /// </summary>
        public IEnumerable<IItemImposto> Itens { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="notaFiscal"></param>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="itens"></param>
        public NotaFiscalImpostoContainer(
            Data.Model.NotaFiscal notaFiscal,
            Global.Negocios.Entidades.Cliente cliente,
            Global.Negocios.Entidades.Loja loja,
            Global.Negocios.Entidades.Fornecedor fornecedor,
            IEnumerable<IItemImposto> itens)
        {
            NotaFiscal = notaFiscal;
            Cliente = cliente;
            Loja = loja;
            Fornecedor = fornecedor;
            Itens = itens;
        }

        #endregion
    }
}
