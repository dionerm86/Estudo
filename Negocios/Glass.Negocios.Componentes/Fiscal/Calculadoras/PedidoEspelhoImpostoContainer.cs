using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação do container de itens de impostos para o pedido.
    /// </summary>
    class PedidoEspelhoImpostoContainer : IItemImpostoContainer
    {
        #region Propriedades

        /// <summary>
        /// Pedido.
        /// </summary>
        public Data.Model.PedidoEspelho PedidoEspelho { get; }

        /// <summary>
        /// Identificador da nota fiscal.
        /// </summary>
        public int? IdNf => null;

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
        public Global.Negocios.Entidades.Fornecedor Fornecedor => null;

        /// <summary>
        /// Tipo de documento.
        /// </summary>
        public Sync.Fiscal.Enumeracao.NFe.TipoDocumento TipoDocumento => Sync.Fiscal.Enumeracao.NFe.TipoDocumento.Saida;

        /// <summary>
        /// Finalidade de emissão
        /// </summary>
        public Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao FinalidadeEmissao => Sync.Fiscal.Enumeracao.NFe.FinalidadeEmissao.Normal;

        /// <summary>
        /// Valor do frete
        /// </summary>
        public decimal ValorFrete => PedidoEspelho.ValorEntrega;

        /// <summary>
        /// Modalidade do frete
        /// </summary>
        public Data.Model.ModalidadeFrete? ModalidadeFrete => Data.Model.ModalidadeFrete.SemTransporte;

        /// <summary>
        /// Valor do seguro.
        /// </summary>
        public decimal ValorSeguro => 0m;

        /// <summary>
        /// Valor do desconto
        /// </summary>
        public decimal ValorDesconto => 0m;

        /// <summary>
        /// Outras Despesas
        /// </summary>
        public decimal OutrasDespesas => 0m;

        /// <summary>
        /// Valor do IPI devolvido
        /// </summary>
        public decimal ValorIpiDevolvido => 0;

        /// <summary>
        /// Indica se a nota fiscal está sendo importada pelo sistema.
        /// </summary>
        public bool NotaFiscalImportadaSistema => false;

        /// <summary>
        /// Identifica se deve calcular a aliquota do ICMS.
        /// </summary>
        public bool CalcularAliquotaIcmsSt => (Loja?.CalcularIcmsPedido ?? false) && (Cliente?.CobrarIcmsSt ?? false);

        /// <summary>
        /// Itens filhos.
        /// </summary>
        public IEnumerable<IItemImposto> Itens { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="pedidoEspelho"></param>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="itens"></param>
        public PedidoEspelhoImpostoContainer(
            Data.Model.PedidoEspelho pedidoEspelho,
            Global.Negocios.Entidades.Cliente cliente,
            Global.Negocios.Entidades.Loja loja,
            IEnumerable<IItemImposto> itens)
        {
            PedidoEspelho = pedidoEspelho;
            Cliente = cliente;
            Loja = loja;
            Itens = itens;
        }

        #endregion
    }
}
