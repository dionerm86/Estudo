using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class TipoCfopDAO : BaseDAO<TipoCfop, TipoCfopDAO>
    {
        //private TipoCfopDAO() { }

        public override int Delete(TipoCfop objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdTipoCfop);
        }

        public override int DeleteByPrimaryKey(uint idTipoCfop)
        {
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From cfop Where idTipoCfop=" + idTipoCfop) > 0)
                throw new Exception("Este tipo de cfop não pode ser excluído pois existem cfops relacionados ao mesmo.");

            return GDAOperations.Delete(new TipoCfop { IdTipoCfop = (int)idTipoCfop });
        }

        public IList<TipoCfop> GetByString(string idsTipoCfop)
        {
            return GetByString(null, idsTipoCfop);
        }

        public IList<TipoCfop> GetByString(GDASession session, string idsTipoCfop)
        {
            return objPersistence.LoadData(session, "select * from tipo_cfop where idTipoCfop in (" + idsTipoCfop + ")").ToList();
        }

        public string GetDescrByString(string idsTipoCfop)
        {
            return GetDescrByString(null, idsTipoCfop);
        }

        public string GetDescrByString(GDASession session, string idsTipoCfop)
        {
            string descr = "";
            foreach (TipoCfop t in GetByString(session, idsTipoCfop))
                descr += t.Descricao + ", ";
            
            return descr.TrimEnd(',', ' ');
        }

        public bool IsDevolucao(uint idCfop)
        {
            uint idTipoCfop = CfopDAO.Instance.ObtemValorCampo<uint>("idTipoCfop", "idCfop=" + idCfop);
            if (idTipoCfop == 0)
                return false;

            return ObtemValorCampo<bool>("devolucao", "idTipoCfop=" + idTipoCfop);
        }
    }
}
