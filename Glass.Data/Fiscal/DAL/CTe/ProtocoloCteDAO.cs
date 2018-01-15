using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ProtocoloCteDAO : BaseDAO<ProtocoloCte, ProtocoloCteDAO>
    {
        //private ProtocoloCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, int tipoProtocolo, bool selecionar)
        {
            string sql = "Select * From protocolo_cte Where 1";

            if (!selecionar)
                sql = "Select count(*) from protocolo_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

                sql += " And TIPOPROTOCOLO=" + tipoProtocolo;

            return sql;
        }

        public ProtocoloCte GetElement(uint idCte, int tipoProtocolo)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, tipoProtocolo, true));
            }
            catch (GDAException)
            {
                return null;
            }
        }

        public IList<ProtocoloCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize, null).ToList();
        }

        public List<ProtocoloCte> GetProtocolosByIdCte(uint idCte)
        {
            return objPersistence.LoadData(Sql(idCte, 0, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        #endregion

        public override uint Insert(ProtocoloCte objInsert)
        {
            uint idCte = base.Insert(objInsert);

            return idCte;
        }

        public override int Update(ProtocoloCte objUpdate)
        {
            return base.Update(objUpdate);
        }

        public void Update(uint idCte, string numProtocolo)
        {
            objPersistence.ExecuteCommand("Update protocolo_cte set NUMPROTOCOLO=?numProt Where IDCTE=" + idCte, 
                        new GDAParameter[] { new GDAParameter("?numProt", numProtocolo) });
        }

        public void Delete(uint idCte, int tipoProtocolo)
        {
            string sql = "delete from protocolo_cte where IDCTE=" + idCte + " AND TIPOPROTOCOLO=" + tipoProtocolo;
            objPersistence.ExecuteCommand(sql);
        }        
    }
}
