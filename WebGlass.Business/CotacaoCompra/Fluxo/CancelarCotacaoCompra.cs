using System;
using Glass.Data.DAL;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class CancelarCotacaoCompra : BaseFluxo<CancelarCotacaoCompra>
    {
        private CancelarCotacaoCompra() { }

        /// <summary>
        /// Cancela uma cotação de compra.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <param name="motivo"></param>
        public void Cancelar(uint codigoCotacaoCompra, string motivo)
        {
            // Verifica se uma cotação de compra pode ser cancelada
            if (CotacaoCompraDAO.Instance.PossuiComprasNaoCanceladas(codigoCotacaoCompra))
                throw new Exception("Não é possível cancelar a cotação de compra porque há " +
                    "compras não canceladas geradas por ela. Cancele-as para continuar.");

            CotacaoCompraDAO.Instance.Cancelar(codigoCotacaoCompra, motivo, true);
        }
    }
}
