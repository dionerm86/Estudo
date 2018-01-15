using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.PedidoEspelho.Fluxo
{
    public sealed class Gerar : BaseFluxo<Gerar>
    {
        private Gerar() { }

        public string GerarVarios(IEnumerable<string> idsPedidos, bool alterarDataEntrega, DateTime? dataEntrega, bool finalizar)
        {
            // Alterar a data do pedido antes de gerar conferência para que a data de fábrica fique correta
            if (dataEntrega != null)
                PedidoDAO.Instance.AlteraDataEntrega(String.Join(",", idsPedidos.ToArray()), dataEntrega.Value);

            string idsOrcamentosGerados = String.Empty;

            foreach (string id in idsPedidos)
            {
                uint idPedido = Glass.Conversoes.StrParaUint(id);

                PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao(idPedido);

                if (finalizar)
                {
                    uint idOrcamento = PedidoEspelhoDAO.Instance.FinalizarPedidoComTransacao(idPedido);

                    if (idOrcamento > 0)
                        idsOrcamentosGerados += idOrcamento + ",";
                }
            }

            return "Conferência dos pedidos geradas com sucesso." + 
                (!String.IsNullOrEmpty(idsOrcamentosGerados) ? " Orçamento(s) gerado(s): " + 
                idsOrcamentosGerados.TrimEnd(',') : String.Empty);
        }
    }
}
