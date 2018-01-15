using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class LogNfDAO : BaseDAO<LogNf, LogNfDAO>
    {
        //private LogNfDAO() { }

        private string SqlList(uint idNf, bool separacaoValores, bool selecionar, out bool temFiltro, out List<GDAParameter> lstParams)
        {
            lstParams = new List<GDAParameter>();
            temFiltro = false;
            var campos = selecionar ? "ln.*" : "Count(*)";

            var sql = "SELECT " + campos + @", f.Nome AS NomeFuncionario
                FROM log_nf ln
                    LEFT JOIN funcionario f ON (ln.IdFuncLog=f.IdFunc)
                WHERE 1";

            if (idNf > 0)
            {
                sql += " AND ln.IdNf=" + idNf;
                temFiltro = true;
            }

            /* Chamado 14827.
             * Os logs referentes a separação de valores devem ser mostrados em uma grid separada. */
            if (separacaoValores)
            {
                sql += " AND ln.Evento=?Evento";
                lstParams.Add(new GDAParameter("?Evento", "Separação Valores"));
                temFiltro = true;
            }
            else
            {
                sql += " AND ln.Evento<>?Evento";
                lstParams.Add(new GDAParameter("?Evento", "Separação Valores"));
                temFiltro = true;
            }

            return sql;
        }

        public IList<LogNf> GetList(uint idNf, bool separacaoValores, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "idLogNf desc" : sortExpression;

            bool temFiltro;
            List<GDAParameter> lstParams;
            
            var sql = SqlList(idNf, separacaoValores, true, out temFiltro, out lstParams);
            return LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro, lstParams.ToArray());
        }

        public int GetCount(uint idNf, bool separacaoValores)
        {
            bool temFiltro;
            List<GDAParameter> lstParams;

            var sql = SqlList(idNf, separacaoValores, true, out temFiltro, out lstParams);
            return GetCountWithInfoPaging(sql, temFiltro, lstParams.ToArray());
        }

        /// <summary>
        /// Obtém o último código retornado pela receita/sistema registrado para a nota passada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemUltimoCodigo(uint idNf)
        {
            return ExecuteScalar<int>("Select codigo From log_nf Where idNf=" + idNf + " Order By idLogNf desc Limit 1");
        }

        /// <summary>
        /// Obtém a data de cancelamento da nota fiscal.
        /// </summary>
        public DateTime? ObtemDataCancelamento(GDASession session, int idNf)
        {
            return ExecuteScalar<DateTime?>(session,
                string.Format("SELECT DataHora FROM log_nf WHERE IdNf={0} AND Evento = ?evento;",
                    idNf), new GDAParameter("?evento", "Cancelamento"));
        }

        /// <summary>
        /// Insere um novo log da NF passada
        /// </summary>
        public uint NewLog(uint idNf, string evento, int codigo, string descricao)
        {
            return NewLog(null, idNf, evento, codigo, descricao);
        }

        /// <summary>
        /// Insere um novo log da NF passada
        /// </summary>
        public uint NewLog(GDASession session, uint idNf, string evento, int codigo, string descricao)
        {
            LogNf log = new LogNf();
            log.IdNf = idNf;
            log.Evento = evento;
            log.Codigo = codigo;
            log.Descricao = !string.IsNullOrEmpty(descricao) && descricao.Length > 255 ? descricao.Substring(0, 255) : descricao;
            /* Chamado 14827.
             * É necessário salvar o funcionário que provocou a inserção do log da nota fiscal. */
            log.IdFuncLog = UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.CodUser > 0 ? (int)UserInfo.GetUserInfo.CodUser : (int?)null;
            log.DataHora = DateTime.Now;

            return Insert(session, log);
        }
    }
}