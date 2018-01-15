using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PedidosCupomFiscalDAO : BaseDAO<PedidosCupomFiscal, PedidosCupomFiscalDAO>
    {
        //private PedidosCupomFiscalDAO() { }

        /// <summary>
        /// Retorna os pedidos relacionados à este cupom fiscal
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public Pedido[] GetPedidosByCupomFiscal(uint idCFe)
        {
            List<PedidosCupomFiscal> pCFe = objPersistence.LoadData("select * from pedidos_cupom_fiscal where idCupomFiscal=" + idCFe + " and idPedido is not null");
            string pedidos = "";
            foreach (PedidosCupomFiscal pedido in pCFe)
                pedidos += "," + pedido.IdPedido;

            return PedidoDAO.Instance.GetByString(null, pedidos.Substring(1));
        }

        /// <summary>
        /// Retorna os ids dos pedidos relacionados à este cupom fiscal
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IList<PedidosCupomFiscal> GetByCupomFiscal(uint idCFe)
        {
            return objPersistence.LoadData("Select * From pedidos_cupom_fiscal Where idCupomFiscal=" + idCFe).ToList();
        }

        public IList<PedidosCupomFiscal> GetByPedido(uint idPedido)
        {
            return objPersistence.LoadData("select * from pedidos_cupom_fiscal where idPedido=" + idPedido).ToList();
        }

        public string CuponsFiscaisGerados(uint idPedido)
        {
            string sql = "select cast(group_concat(cfe.chaveCupomSAT SEPARATOR ', ') as char) from pedidos_cupom_fiscal pcf left join cupom_fiscal_cfe cfe on (pcf.idCupomFiscal=cfe.idCupomFiscal) " +
                "where pcf.idPedido=" + idPedido + " and cfe.cancelado = 'N' order by cfe.idCupomFiscal";

            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null ? retorno.ToString() : null;
        }
    }
}
