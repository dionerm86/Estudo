using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    public class PedidosOCFluxo : BaseFluxo<PedidosOCFluxo>
    {
        private PedidosOCFluxo() { }

        #region Lista de Pedidos que podem gerar OC

        /// <summary>
        /// Recupera uma lista de pedidos que podem gerar ordem de carga (Venda / Transferência)
        /// </summary>
        /// <param name="tipoOC"></param>
        /// <param name="idCliente"></param>
        /// <param name="idRota"></param>
        /// <param name="idLoja"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="pedidosObs"></param>
        /// <param name="codRotasExternas"></param>
        /// <param name="idClienteExterno"></param>
        /// <param name="nomeClienteExterno"></param>
        /// <returns></returns>
        public Glass.Data.Model.Pedido[] GetPedidosForItensOC(Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, uint idCliente, uint idRota,
            uint idLoja, string dtEntPedidoIni, string dtEntPedidoFin, bool pedidosObs, string codRotasExternas,
            uint idClienteExterno, string nomeClienteExterno, bool fastDelivery, string obsLiberacao)
        {
            var idsPedidos = PedidoDAO.Instance.GetIdsPedidosForOC(tipoOC, idCliente, null, idRota, null, idLoja,
                dtEntPedidoIni, dtEntPedidoFin, false, pedidosObs, codRotasExternas, idClienteExterno, nomeClienteExterno, fastDelivery, obsLiberacao);

            if (idsPedidos == null || idsPedidos.Count == 0)
                return null;

            var dados =  PedidoDAO.Instance.GetPedidosForOC(idsPedidos[0].Split(';')[2], 0, true)
                .OrderBy(p=>p.TipoPedido).ThenByDescending(p => p.IdPedido).ToArray();

            return dados;
        }

        #endregion

        #region Remover pedido da OC

        static volatile object _removerPedidoOCLock = new object();

        /// <summary>
        /// Remove um pedido da OC
        /// </summary>
        public void RemoverPedido(uint idOC, uint idPedido)
        {
            lock(_removerPedidoOCLock)
            {
                using (var trans = new GDA.GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        if (idPedido == 0)
                            throw new Exception("Pedido não encontrado.");

                        var idCarregamento = OrdemCargaDAO.Instance.GetIdCarregamento(trans, idOC);

                        if (idCarregamento.GetValueOrDefault(0) > 0 && PedidoDAO.Instance.PossuiPecaCarregada(trans, idPedido, idCarregamento.Value))
                            throw new Exception("Não é possível remover este Pedido da OC, pois o mesmo já possui peças carregadas.");

                        if (PedidoOrdemCargaDAO.Instance.ObtemQtdePedidosOC(trans, idOC) == 1)
                            throw new Exception("A OC só possiu este Pedido, não é possível removê-lo.");

                        //Se for OC de transferência é ja tiver gerado uma OC de venda, tem que deletar 
                        //a de venda primeiro.
                        var tipoOC = OrdemCargaDAO.Instance.GetTipoOrdemCarga(trans, idOC);
                        if (tipoOC == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia)
                        {
                            var pedidosNfs = PedidosNotaFiscalDAO.Instance.GetByPedido(trans, idPedido);
                            foreach (var pedidoNf in pedidosNfs)
                            {
                                var situacao = NotaFiscalDAO.Instance.ObtemSituacao(trans, pedidoNf.IdNf);

                                if (situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Cancelada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Denegada &&
                                    situacao != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Inutilizada)
                                    throw new Exception("Não é possível remover este Pedido da OC, pois o mesmo está vinculado a uma nota fiscal.");
                            }

                            if (OrdemCargaDAO.Instance.GetIdsPedidosOC(trans, idOC, Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda).Any(f => f == idPedido))
                                throw new Exception("Não é possível remover este Pedido da OC, pois o mesmo está vinculado a uma OC de venda.");
                        }
                        else
                        {
                            if (LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(trans, idPedido).Any())
                                throw new Exception("Não é possível remover este Pedido da OC, pois o mesmo está vinculado a uma Liberação.");
                        }

                        var pedidoOC = PedidoOrdemCargaDAO.Instance.GetElement(trans, tipoOC, idPedido);

                        //Registra o log de remoção
                        LogCancelamentoDAO.Instance.LogPedidoOC(trans, pedidoOC, string.Format("Remoção do pedido: {0},", idPedido), true);

                        //Remove os itens do pedido do carregamento
                        if (idCarregamento.GetValueOrDefault(0) > 0)
                            ItemCarregamentoDAO.Instance.DeleteByPedido(trans, idPedido, idCarregamento.Value);

                        //Remove o vinculo do pedido com a OC
                        PedidoOrdemCargaDAO.Instance.DeleteByPedido(trans, idPedido, idOC);

                        if (idCarregamento.GetValueOrDefault(0) > 0)
                            CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento.Value, null);

                        trans.Commit();
                        trans.Close();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Produtos do Pedido

        /// <summary>
        /// Busca todos os produtos relacionados ao pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="produtosEstoque"></param>
        /// <returns></returns>
        public ProdutosPedido[] GetProdutosByPedido(uint idPedido, bool produtosEstoque)
        {
            return ProdutosPedidoDAO.Instance.GetByPedidoForOC(idPedido, produtosEstoque).ToArray();
        }

        #endregion

        #region Volumes do Pedido

        /// <summary>
        /// Busca todos os volumes relacionados ao pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Volume[] GetVolumesByPedido(uint idPedido)
        {
            return VolumeDAO.Instance.ObterVolumesParaGerarOrdemCarga(idPedido);
        }

        #endregion
    }
}
