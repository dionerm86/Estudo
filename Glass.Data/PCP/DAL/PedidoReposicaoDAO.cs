using System;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class PedidoReposicaoDAO : BaseDAO<PedidoReposicao, PedidoReposicaoDAO>
    {
        //private PedidoReposicaoDAO() { }

        public PedidoReposicao GetByPedido(uint idPedido)
        {
            string sql = "select * from pedido_reposicao where IdPedido=" + idPedido;
            return objPersistence.LoadOneData(sql);
        }

        public bool IsPedidoReposicao(uint idPedido)
        {
            return IsPedidoReposicao(null, idPedido);
        }

        public bool IsPedidoReposicao(GDASession session, uint idPedido)
        {
            string sql = "select count(*) from pedido_reposicao where idPedido=" + idPedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Retorna o número do pedido original (no caso de uma reposição) ou o próprio número do pedido.
        /// </summary>
        public uint GetPedidoOriginal(GDASession session, uint idPedido)
        {
            string sql = "select coalesce(idPedidoAnterior, idPedido) from pedido where idPedido=" + idPedido;
            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null && retorno.ToString() != null ? Glass.Conversoes.StrParaUint(retorno.ToString()) : idPedido;
        }

        /// <summary>
        /// Retorna o número do produto pedido original (no caso de uma reposição) ou o próprio número do produto pedido.
        /// </summary>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public uint GetProdPedEspOriginal(uint idProdPedEsp)
        {
            string sql = @"
                select coalesce(ppp.idProdPed, idProdPedEsp) 
                from produtos_pedido pp 
                    left join produto_pedido_producao ppp on (pp.idProdPedProdRepos=ppp.idProdPedProducao)
                where idProdPedEsp=" + idProdPedEsp;

            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno.ToString() != null ? Glass.Conversoes.StrParaUint(retorno.ToString()) : idProdPedEsp;
        }

        /// <summary>
        /// Salva as datas do pedido de reposição.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="dataEntrega"></param>
        /// <param name="dataCliente"></param>
        public void SalvarDatas(uint idPedido, DateTime dataEntrega, DateTime? dataCliente)
        {
            string sql = "update pedido set dataEntrega=?de where idPedido=" + idPedido + 
                "; update pedido_reposicao set dataClienteInformado=?dc where idPedido=" + idPedido;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?de", dataEntrega), new GDAParameter("?dc", dataCliente));
        }

        /// <summary>
        /// Verifica se o pedido é utilizado para troca.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoParaTroca(uint idPedido)
        {
            return PedidoConfig.PermitirTrocaPorPedido && 
                ObtemValorCampo<bool>("podeUtilizarTroca", "idPedido=" + idPedido);
        }
    }
}
