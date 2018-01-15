using System;
using System.Collections.Generic;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL.CTe
{
    public sealed class LogCteDAO : BaseDAO<LogCte, LogCteDAO>
    {
        //private LogCteDAO() { }

        private string SqlList(uint idCte, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From log_cte Where 1";

            if (idCte > 0)
            {
                sql += " And idCte=" + idCte;
                temFiltro = true;
            }

            return sql;
        }

        public IList<LogCte> GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "idLogCte desc" : sortExpression;

            bool temFiltro;
            return LoadDataWithSortExpression(SqlList(idCte, true, out temFiltro), filtro, startRow, pageSize, temFiltro);
        }

        public int GetCount(uint idCte)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(SqlList(idCte, true, out temFiltro), temFiltro);
        }

        /// <summary>
        /// Obtém o último código retornado pela receita/sistema registrado para a nota passada
        /// </summary>
        /// <param name="idCTe"></param>
        /// <returns></returns>
        public int ObtemUltimoCodigo(uint idCte)
        {
            return ExecuteScalar<int>("Select codigo From log_cte Where idCte=" + idCte + " Order By idLogCte desc Limit 1");
        }

        /// <summary>
        /// Insere um novo log do CTe passado
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="evento"></param>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        public uint NewLog(uint idCte, string evento, int codigo, string descricao)
        {
            LogCte log = new LogCte();
            log.IdCte = idCte;
            log.Evento = evento;
            log.Codigo = codigo;
            log.Descricao = !String.IsNullOrEmpty(descricao) && descricao.Length > 255 ? descricao.Substring(0, 255) : descricao;
            log.DataHora = DateTime.Now;

            return Insert(log);
        }
    }
}