using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class CorVidroDAO : BaseDAO<CorVidro, CorVidroDAO>
	{
        //private CorVidroDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From cor_vidro";

            return sql;
        }

        public IList<CorVidro> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                List<CorVidro> lst = new List<CorVidro>();
                lst.Add(new CorVidro());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(true), filtro, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false), null);

            return count == 0 ? 1 : count;
        }
 
        /// <summary>
        /// Retorna as cores de vidro cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<CorVidro> GetForFiltro()
        {
            return objPersistence.LoadData(SqlList(true) + " Order By descricao").ToList();
        }

        public uint? GetIdByDescr(string descricao)
        {
            return ObtemValorCampo<uint?>("idCorVidro", "descricao=?descr", new GDAParameter("?descr", descricao));
        }

        public string GetNome(GDASession sessao, uint idCorVidro)
        {
            return ExecuteScalar<string>("select descricao from cor_vidro where idCorVidro=" + idCorVidro);
        }

        public string GetNome(uint idCorVidro)
        {
            return GetNome(null, idCorVidro);
        }

        public string GetSigla(uint idCorVidro)
        {
            return ObtemValorCampo<string>("coalesce(sigla, descricao)", "idCorVidro=" + idCorVidro);
        }

        public CorVidro[] GetForProjeto(uint? idProjetoModelo)
        {
            string sql = "SELECT * FROM cor_vidro WHERE 1";

            if (idProjetoModelo.GetValueOrDefault(0) > 0)
            {
                var ids = ProjetoModeloDAO.Instance.ObtemIdsCorVidro(idProjetoModelo.Value);
                if (!string.IsNullOrWhiteSpace(ids))
                    sql += " AND IdCorVidro IN (" + ids + ")";
            }

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(CorVidro objDelete)
        {
            // Verifica se existem produtos associados à esta cor
            if (objPersistence.ExecuteSqlQueryCount("Select * From produto where idCorVidro=" + objDelete.IdCorVidro) > 0)
                throw new Exception("Existem produtos associados à esta cor de vidro, exclua os mesmos antes de excluir esta cor.");
            
            // Verifica se existem cálculos de projeto associados à esta cor
            if (objPersistence.ExecuteSqlQueryCount("Select * From item_projeto where idCorVidro=" + objDelete.IdCorVidro) > 0)
                throw new Exception("Esta cor não pode ser excluída, pois existem cálculos de projeto associados à mesma.");

            return base.Delete(objDelete);
        }
	}
}