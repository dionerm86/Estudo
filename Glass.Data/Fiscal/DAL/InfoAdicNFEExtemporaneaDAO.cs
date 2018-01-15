using System;
using System.Collections.Generic;
using Glass.Data.Model.EFD;
using GDA;

namespace Glass.Data.DAL.EFD
{
    public sealed class InfoAdicNFEExtemporaneaDAO : BaseDAO<InfoAdicNFEExtemporanea, InfoAdicNFEExtemporaneaDAO>
    {
        //private InfoAdicNFEExtemporaneaDAO() { }

        private GDAParameter[] GetParams(int idInfoAdicNFEExtemporanea, int idNfe, int tipoImposto)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (idInfoAdicNFEExtemporanea > 0)
                lst.Add(new GDAParameter("?idInfoAdicNFEExtemporanea", idInfoAdicNFEExtemporanea));
            else
            {
                if (idNfe > 0)
                    lst.Add(new GDAParameter("?idNfe", idNfe));

                if (tipoImposto > 0)
                    lst.Add(new GDAParameter("?tipoImposto", tipoImposto));
            }

            return lst.ToArray();
        }

        private string Sql(int idInfoAdicNFEExtemporanea, int idNfe, int tipoImposto)
        {
            string sql = "SELECT i.*, n.NUMERONFE FROM info_adic_nfe_extemporanea i LEFT JOIN nota_fiscal n ON(n.IDNF=i.IDNF) WHERE 1";

            if (idInfoAdicNFEExtemporanea > 0)
                sql += " AND IdInfoAdicNFEExt = ?idInfoAdicNFEExtemporanea";
            else
            {
                if (idNfe > 0)
                    sql += " AND i.IdNf=?idNfe";

                if (tipoImposto > 0)
                    sql += " AND i.TipoImposto = ?tipoImposto";
            }

            return sql;
        }

        #region Busca

        public IList<InfoAdicNFEExtemporanea> ObterLista(int idNfe, int tipoImposto, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, idNfe, tipoImposto), sortExpression, startRow, pageSize, GetParams(0, idNfe, tipoImposto));
        }

        public InfoAdicNFEExtemporanea Obter(int idInfoAdicNFEExtemporanea)
        {
            return objPersistence.LoadOneData(Sql(idInfoAdicNFEExtemporanea, 0, 0), GetParams(idInfoAdicNFEExtemporanea, 0, 0));
        }

        public InfoAdicNFEExtemporanea Obter(int idNfe, int tipoImposto)
        {
            return objPersistence.LoadOneData(Sql(0, idNfe, tipoImposto), GetParams(0, idNfe, tipoImposto));
        }

        #endregion

        #region CRUD

        public uint Inserir(InfoAdicNFEExtemporanea model)
        {
            try
            {
                return objPersistence.Insert(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro : " + ex.Message);
            }
        }

        public int Atualizar(InfoAdicNFEExtemporanea model)
        {
            try
            {
                return objPersistence.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro : " + ex.Message);
            }
        }

        public int Excluir(InfoAdicNFEExtemporanea model)
        {
            try
            {
                return objPersistence.Delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro : " + ex.Message);
            }
        }

        #endregion
    }
}
