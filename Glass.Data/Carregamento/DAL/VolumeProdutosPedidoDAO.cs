using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class VolumeProdutosPedidoDAO : BaseDAO<VolumeProdutosPedido, VolumeProdutosPedidoDAO>
    {
        //private VolumeProdutosPedidoDAO() { }

        #region Busca de Itens

        private string Sql(string idsVolumes, uint idVolume, uint idProdPed, bool selecionar)
        {
            string campos = selecionar ? "vpp.*, p.CodInterno, p.Descricao, pp.Altura, pp.Largura, pp.Qtde as QtdeProdPed, pp.idProd, sgp.descricao AS NomeSubGrupoProd" : "COUNT(*)";

            string sql = @"
                SELECT " + campos + @"
                FROM volume_produtos_pedido vpp
                    LEFT JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    LEFT JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod ON (p.IdSubGrupoProd = sbg.IdSubGrupoProd)
                WHERE 1";

            if (idVolume > 0)
                sql += " AND vpp.idVolume=" + idVolume;

            if (idProdPed > 0)
                sql += " AND vpp.idProdPed=" + idProdPed;

            if (!string.IsNullOrEmpty(idsVolumes))
                sql += " AND vpp.idVolume IN(" + idsVolumes + ")";

            if(idVolume == 0 && idProdPed == 0 && string.IsNullOrEmpty(idsVolumes))
                sql += " AND vpp.idVolume=0";


            return sql;
        }

        /// <summary>
        /// Recupera a lista de itens de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<VolumeProdutosPedido> GetList(uint idVolume, string sortExpression, int startRow, int pageSize)
        {
            if (GetListCountReal(idVolume) == 0)
            {
                List<VolumeProdutosPedido> lst = new List<VolumeProdutosPedido>();
                lst.Add(new VolumeProdutosPedido());
                return lst.ToArray();
            }

            return LoadDataWithSortExpression(Sql(null, idVolume, 0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetListCount(uint idVolume)
        {
            int count = objPersistence.ExecuteSqlQueryCount(Sql(null, idVolume, 0, false));

            return count == 0 ? 1 : count;
        }

        public int GetListCountReal(uint idVolume)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(null, idVolume, 0, false));
        }

        /// <summary>
        /// Recupera os produtos do volume para o relatório
        /// </summary>
        /// <param name="idsVolumes"></param>
        /// <returns></returns>
        public VolumeProdutosPedido[] GetListForRpt(string idsVolumes)
        {
            return objPersistence.LoadData(Sql(idsVolumes, 0, 0, true)).ToArray();
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera os produtos do volume
        /// </summary>
        /// <param name="idsVolumes"></param>
        /// <returns></returns>
        public VolumeProdutosPedido[] GetList(string idsVolumes)
        {
            return GetList(null, idsVolumes);
        }

        /// <summary>
        /// Recupera os produtos do volume
        /// </summary>
        /// <param name="idsVolumes"></param>
        /// <returns></returns>
        public VolumeProdutosPedido[] GetList(GDASession sessao, string idsVolumes)
        {
            return objPersistence.LoadData(sessao, Sql(idsVolumes, 0, 0, true)).ToArray();
        }

        /// <summary>
        /// Recupera um produto do Volume
        /// </summary>
        public VolumeProdutosPedido GetElement(uint idVolume, uint idProdPed)
        {
            return GetElement(null, idVolume, idProdPed);
        }

        /// <summary>
        /// Recupera um produto do Volume
        /// </summary>
        public VolumeProdutosPedido GetElement(GDASession session, uint idVolume, uint idProdPed)
        {
            if (!Exists(session, idProdPed, idVolume))
                return null;

            return objPersistence.LoadOneData(session, Sql(null, idVolume, idProdPed, true));
        }

        /// <summary>
        /// Recupera o código dos produtos do volume.
        /// </summary>
        public string ObterCodigoProdutosVolume(GDASession session, int idVolume)
        {
            var retorno =
                ExecuteMultipleScalar<string>(session,
                    string.Format(
                        @"SELECT p.CodInterno FROM volume_produtos_pedido vpp
                            INNER JOIN produtos_pedido pp ON (vpp.IdProdPed=pp.IdProdPed)
                            INNER JOIN produto p ON (pp.IdProd=p.IdProd)
                        WHERE vpp.IdVolume={0} GROUP BY p.IdProd", idVolume));

            return retorno != null && retorno.Count > 0 ?
                string.Join(", ", retorno) : string.Empty;
        }

        /// <summary>
        /// Recupera a quantidade de produtos do pedido associados à volumes.
        /// </summary>
        public float ObterQuantidadeProdutoPedidoVolumeGerado(GDASession session, int? idVolume, int? idProdPed)
        {
            var sql = "SELECT CAST(SUM(vpp.Qtde) AS DECIMAL(12,2)) FROM volume_produtos_pedido vpp WHERE 1";

            if (idVolume > 0)
                sql += string.Format(" AND vpp.IdVolume={0}", idVolume);

            if (idProdPed > 0)
                sql += string.Format(" AND vpp.IdProdPed={0}", idProdPed);

            return ExecuteScalar<float>(session, sql);
        }
        
        #endregion

        #region Excluir Itens

        /// <summary>
        /// Deleta todos os itens de um volume
        /// </summary>
        public void DeleteByVolume(GDASession session, uint idVolume)
        {
            string sql = "DELETE FROM volume_produtos_pedido WHERE idVolume=" + idVolume;
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion
    }
}
