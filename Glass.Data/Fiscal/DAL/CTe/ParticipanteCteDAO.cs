using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ParticipanteCteDAO : BaseDAO<ParticipanteCte, ParticipanteCteDAO>
    {
        //private ParticipanteCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, int tipoParticipante, bool selecionar)
        {
            string sql = "Select * From participante_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;
            if (tipoParticipante > 0)
                sql += " And TipoParticipante=" + tipoParticipante;

            return sql;
        }        

        public ParticipanteCte GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, 0, true));
            }
            catch
            {
                return new ParticipanteCte();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCte">id do cte</param>
        /// <param name="tipoParticipante">tipo de participante</param>
        /// <returns></returns>
        public ParticipanteCte GetParticipanteByIdCteTipo(uint idCte, int tipoParticipante)
        {
            return objPersistence.LoadOneData(Sql(idCte, tipoParticipante, true));
        }

        public List<ParticipanteCte> GetParticipanteByIdCte(uint idCte)
        {
            return GetParticipanteByIdCte(null, idCte);
        }

        public List<ParticipanteCte> GetParticipanteByIdCte(GDASession session, uint idCte)
        {
            return objPersistence.LoadData(session, Sql(idCte, 0, true)).ToList();
        }

        public IList<ParticipanteCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize, null).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        #endregion

        public List<uint> Insert(List<ParticipanteCte> listaObjInsert)
        {
            return Insert(null, listaObjInsert);
        }

        public List<uint> Insert(GDASession sessao, List<ParticipanteCte> listaObjInsert)
        {
            List<uint> listaIds = new List<uint>();
            foreach(var i in listaObjInsert)
                listaIds.Add(Insert(sessao, i));

            return listaIds;
        }        

        public void Delete(GDASession session, uint idCte)
        {
            string sql = "delete from participante_cte where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(session, sql);
        }

        public List<uint> GetLojas(IEnumerable<ConhecimentoTransporte> ctes)
        {
            if (ctes.Count() == 0)
                return new List<uint>();

            string idsCte = String.Join(",", ctes.Select(x => x.IdCte.ToString()).ToArray());

            string sql = "select idLoja from participante_cte where idLoja>0 and idCte in (" + idsCte + ")";
            return ExecuteMultipleScalar<uint>(sql);
        }

        internal List<ParticipanteCte> GetForEFD(IEnumerable<Sync.Fiscal.EFD.Entidade.ICTe> ctes)
        {
            if (ctes.Count() == 0)
                return new List<ParticipanteCte>();

            string idsCte = String.Join(",", ctes.Select(x => x.Codigo.ToString()).ToArray());

            string sql = "select * from participante_cte where idCte in (" + idsCte + ")";
            return objPersistence.LoadData(sql).ToList();
        }
    }
}
