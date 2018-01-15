using System;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PedidoCorteDAO : BaseDAO<PedidoCorte, PedidoCorteDAO>
	{
        //private PedidoCorteDAO() { }

        /// <summary>
        /// Altera a situação do pedido passado, criando um novo pedido corte, caso o mesmo
        /// </summary>
        public void AlteraSituacao(uint idFunc, uint idPedido, int situacao)
        {
            AlteraSituacao(null, idFunc, idPedido, situacao);
        }

        /// <summary>
        /// Altera a situação do pedido passado, criando um novo pedido corte, caso o mesmo
        /// </summary>
        public void AlteraSituacao(GDASession session, uint idFunc, uint idPedido, int situacao)
        {
            bool existsPedidoCorte = ExistsByPedido(session, idPedido);

            PedidoCorte pedidoCorte = new PedidoCorte();

            // Se o pedido corte já existir, busca o mesmo
            if (existsPedidoCorte)
                pedidoCorte = GetByIdPedido(session, idPedido);
            
            pedidoCorte.IdPedido = idPedido;
            pedidoCorte.Situacao = situacao;

            switch (situacao)
            {
                case 2: // Produção
                    pedidoCorte.IdFuncProducao = idFunc;
                    pedidoCorte.DataProducao = DateTime.Now;
                    break;
                case 3: // Pronto
                    pedidoCorte.DataPronto = DateTime.Now;
                    break;
                case 4: // Entregue
                    pedidoCorte.IdFuncEntregue = idFunc;
                    pedidoCorte.DataEntregue = DateTime.Now;
                    break;
            }

            // Se já existir, atualiza, senão, insere
            if (existsPedidoCorte)
                Update(session, pedidoCorte);
            else
                Insert(session, pedidoCorte);
        }

        /// <summary>
        /// Busca o PedidoCorte pelo idPedido
        /// </summary>
        public PedidoCorte GetByIdPedido(uint idPedido)
        {
            return GetByIdPedido(null, idPedido);
        }

        /// <summary>
        /// Busca o PedidoCorte pelo idPedido
        /// </summary>
        public PedidoCorte GetByIdPedido(GDASession session, uint idPedido)
        {
            string sql = "Select * From pedido_corte Where idPedido=" + idPedido;

            return objPersistence.LoadOneData(session, sql);
        }

        /// <summary>
        /// Verifica se existe um PedidoCorte com o idPedido passado
        /// </summary>
        public bool ExistsByPedido(uint idPedido)
        {
            return ExistsByPedido(null, idPedido);
        }

        /// <summary>
        /// Verifica se existe um PedidoCorte com o idPedido passado
        /// </summary>
        public bool ExistsByPedido(GDASession session, uint idPedido)
        {
            string sql = "Select count(*) From pedido_corte Where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }
	}
}