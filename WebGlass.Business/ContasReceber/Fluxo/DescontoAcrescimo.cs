using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.ContasReceber.Fluxo
{
    public sealed class DescontoAcrescimo : BaseFluxo<DescontoAcrescimo>
    {
        private DescontoAcrescimo() { }

        #region Ajax

        private static Ajax.IDescontoAcrescimo _ajax;

        public static Ajax.IDescontoAcrescimo Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.DescontoAcrescimo();

                return _ajax;
            }
        }

        #endregion

        public void Validar(uint idPedidoOuLiberarPedido)
        {
            if (!PedidoConfig.LiberarPedido)
            {
                if (!PedidoDAO.Instance.PedidoExists(idPedidoOuLiberarPedido))
                    throw new Exception("Não existe nenhum pedido com o número passado.");
                else
                {
                    Glass.Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(idPedidoOuLiberarPedido);

                    if (situacao == Glass.Data.Model.Pedido.SituacaoPedido.Cancelado)
                        throw new Exception("Este pedido foi cancelado.");

                    else if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                        throw new Exception("Este pedido ainda não foi confirmado.");

                    else if (PedidoDAO.Instance.ObtemTipoVenda(idPedidoOuLiberarPedido) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista)
                        throw new Exception("Este pedido não possui contas a receber.");
                }
            }
            else
            {
                if (!LiberarPedidoDAO.Instance.LiberacaoExists(idPedidoOuLiberarPedido))
                    throw new Exception("Não existe nenhuma liberação com o número passado.");
                else
                {
                    var liberacao = LiberarPedidoDAO.Instance.GetElement(idPedidoOuLiberarPedido);

                    if (liberacao.Situacao == (uint)Glass.Data.Model.LiberarPedido.SituacaoLiberarPedido.Cancelado)
                        throw new Exception("Esta liberação foi cancelada.");

                    else if (liberacao.TipoPagto == (int)Glass.Data.Model.LiberarPedido.TipoPagtoEnum.AVista)
                        throw new Exception("Esta liberação não possui contas a receber.");
                }
            }
        }
    }
}
