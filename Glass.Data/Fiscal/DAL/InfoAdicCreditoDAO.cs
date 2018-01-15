using System;
using System.Collections.Generic;
using Glass.Data.Model.EFD;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class InfoAdicCreditoDAO : BaseDAO<InfoAdicCredito, InfoAdicCreditoDAO>
    {
        //private InfoAdicCreditoDAO() { }

        private GDAParameter[] GetParams(int idInfoAdicCredito, int codCred, string periodo, int tipoImposto)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (idInfoAdicCredito > 0)
                lst.Add(new GDAParameter("?idInfoAdicCredito", idInfoAdicCredito));
            else
            {
                if (codCred > 0)
                    lst.Add(new GDAParameter("?codCred", codCred));

                if (!string.IsNullOrEmpty(periodo))
                    lst.Add(new GDAParameter("?periodo", periodo));

                if (tipoImposto > 0)
                    lst.Add(new GDAParameter("?tipoImposto", tipoImposto));
            }

            return lst.ToArray();
        }

        private string Sql(int idInfoAdicCredito, int codCred, string periodo, int tipoImposto)
        {
            string sql = "SELECT * FROM info_adic_creditos_efd WHERE 1";

            if (idInfoAdicCredito > 0)
                sql += " AND IdInfoAdicCred = ?idInfoAdicCredito";
            else
            {
                if (codCred > 0)
                    sql += " AND CodCred=?codCred";

                if (!string.IsNullOrEmpty(periodo))
                    sql += " AND Periodo = ?periodo";

                if (tipoImposto > 0)
                    sql += " AND TipoImposto = ?tipoImposto";
            }

            return sql;
        }
        
        #region Busca

        public IList<InfoAdicCredito> ObterLista(int codCred, string periodo, int tipoImposto, 
                                                    string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, codCred, periodo, tipoImposto), sortExpression, startRow, pageSize, GetParams(0, codCred, periodo, tipoImposto));
        }

        public InfoAdicCredito Obter(int idInfoAdicCredito)
        {
            return objPersistence.LoadOneData(Sql(idInfoAdicCredito, 0, null, 0), GetParams(idInfoAdicCredito, 0, null, 0));
        }

        public InfoAdicCredito Obter(int codCred, string periodo, int tipoImposto)
        {
            return objPersistence.LoadOneData(Sql(0, codCred, periodo, tipoImposto), GetParams(0, codCred, periodo, tipoImposto));
        }

        #endregion

        #region CRUD

        public uint Inserir(InfoAdicCredito model)
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

        public int Atualizar(InfoAdicCredito model)
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

        public int Excluir(InfoAdicCredito model)
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
