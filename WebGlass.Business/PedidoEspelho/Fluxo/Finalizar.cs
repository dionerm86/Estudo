using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.PedidoEspelho.Fluxo
{
    public sealed class Finalizar : BaseFluxo<Finalizar>
    {
        private Finalizar() { }

        public void FinalizarPedido(uint idPedido)
        {
            foreach (var item in ItemProjetoDAO.Instance.GetByPedidoEspelho(idPedido))
                if (!item.Conferido)
                    throw new Exception("Para finalizar o pedido confirme o projeto " + item.CodigoModelo + 
                        " (ambiente '" + item.Ambiente + "').");

            if (PedidoDAO.Instance.IsMaoDeObra(idPedido))
            {
                var ambientes = AmbientePedidoEspelhoDAO.Instance.GetByPedido(idPedido);

                foreach (var a in ambientes)
                    if (!AmbientePedidoEspelhoDAO.Instance.PossuiProdutos(a.IdAmbientePedido))
                    {
                        throw new Exception("O vidro " + a.PecaVidro + " não possui mão-de-obra cadastrada. " +
                            "Cadastre alguma mão-de-obra ou remova o vidro para continuar.");
                    }
            }

            PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao(idPedido);
        }

        public void GerarValorExcedente(uint idPedido)
        {
            var lstProd = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(idPedido, false);
            int countProdPed = lstProd.Count;

            // Verifica se o Pedido possui produtos
            if (countProdPed == 0)
                throw new Exception("Inclua pelo menos um produto no pedido.");
            
            else
            {
                var lstProdPed = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(idPedido, false);
                string descrProd;

                foreach (var p in lstProdPed)
                {
                    descrProd = p.DescrProduto;

                    if (!String.IsNullOrEmpty(descrProd) && (descrProd.Trim().ToLower() == "t o t a l" ||
                        descrProd.Trim().ToLower() == "total" || descrProd.Trim().ToLower() == "pedido em conferencia"))
                    {
                        throw new Exception("Inclua pelo menos um produto no pedido que não seja o produto TOTAL ou PEDIDO EM CONFERENCIA.");
                    }
                }
            }

            PedidoEspelhoDAO.Instance.GerarExcedente(idPedido);
        }

        public string FinalizarVarios(IEnumerable<string> idsPedidos, bool alterarDataEntrega, DateTime? dataEntrega)
        {
            // Alterar as datas de entrega e de fábrica do pedido
            if (alterarDataEntrega)
                PedidoDAO.Instance.AlteraDataEntrega(String.Join(",", idsPedidos.ToArray()), dataEntrega.GetValueOrDefault(), true);

            string idsOrcamentosGerados = String.Empty;

            foreach (string id in idsPedidos)
            {
                uint idPedido = Glass.Conversoes.StrParaUint(id);
                uint idOrcamento = PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao(idPedido);

                if (idOrcamento > 0)
                    idsOrcamentosGerados += idOrcamento + ",";
            }

            return "Conferência dos pedidos finalizadas com sucesso." + 
                (!String.IsNullOrEmpty(idsOrcamentosGerados) ? " Orçamento(s) gerado(s): " + 
                idsOrcamentosGerados.TrimEnd(',') : String.Empty);
        }
    }
}
