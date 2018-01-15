using System;
using Glass.Data.DAL;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class ReabrirCotacaoCompra : BaseFluxo<ReabrirCotacaoCompra>
    {
        private ReabrirCotacaoCompra() { }

        /// <summary>
        /// Reabre uma cotação de compra.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        public void Reabrir(uint codigoCotacaoCompra)
        {
            // Garante que a cotação de compra pode ser reaberta
            var cotacao = CRUD.Instance.ObtemCotacaoCompra(codigoCotacaoCompra);
            if (!cotacao.PodeReabrir)
                throw new Exception("Não é possível reabrir esta cotação de compra. Verifique se ela está " + 
                    "finalizada ou se possui compras em aberto.");

            // Reabre a cotação de compra
            CotacaoCompraDAO.Instance.Reabrir(codigoCotacaoCompra);
        }
    }
}
