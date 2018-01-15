using GDA;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public class ProtocoloMDFeDAO : BaseDAO<ProtocoloMDFe, ProtocoloMDFeDAO>
    {
        #region Busca padrão

        private string Sql(int idManifestoEletronico, int tipoProtocolo, bool selecionar)
        {
            var campos = selecionar ? "*" : "COUNT(*)";

            string sql = "SELECT " + campos + " FROM protocolo_mdfe WHERE 1";

            if (idManifestoEletronico > 0)
                sql += " AND IdManifestoEletronico=" + idManifestoEletronico;

            sql += " AND TipoProtocolo=" + tipoProtocolo;

            return sql;
        }

        public ProtocoloMDFe GetElement(int idManifestoEletronico, int tipoProtocolo)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idManifestoEletronico, tipoProtocolo, true));
            }
            catch (GDAException)
            {
                return null;
            }
        }

        public IList<ProtocoloMDFe> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize, null).ToList();
        }

        public List<ProtocoloMDFe> GetProtocolosById(int idManifestoEletronico)
        {
            return objPersistence.LoadData(Sql(idManifestoEletronico, 0, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        #endregion

        public override uint Insert(ProtocoloMDFe objInsert)
        {
            uint idManifestoEletronico = base.Insert(objInsert);

            return idManifestoEletronico;
        }

        public override int Update(ProtocoloMDFe objUpdate)
        {
            return base.Update(objUpdate);
        }

        public void Update(int idManifestoEletronico, string numProtocolo)
        {
            objPersistence.ExecuteCommand("UPDATE protocolo_mdfe SET NumProtocolo=?numProt WHERE IdManifestoEletronico=" + idManifestoEletronico,
                new GDAParameter[] { new GDAParameter("?numProt", numProtocolo) });
        }

        public void Delete(int idManifestoEletronico, int tipoProtocolo)
        {
            string sql = "DELETE FROM protocolo_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico + " AND TipoProtocolo=" + tipoProtocolo;
            objPersistence.ExecuteCommand(sql);
        }
    }
}
