using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class LogMDFeDAO : BaseDAO<LogMDFe, LogMDFeDAO>
    {
        private string SqlList(int idManifestoEletronico, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "*" : "COUNT(*)";

            string sql = "SELECT " + campos + " FROM log_mdfe WHERE 1";

            if (idManifestoEletronico > 0)
            {
                sql += " AND IdManifestoEletronico=" + idManifestoEletronico;
                temFiltro = true;
            }

            return sql;
        }

        public IList<LogMDFe> GetList(int idManifestoEletronico, string sortExpression, int startRow, int pageSize)
        {
            string filtro = string.IsNullOrEmpty(sortExpression) ? "idLogMDFe desc" : sortExpression;

            bool temFiltro;
            return LoadDataWithSortExpression(SqlList(idManifestoEletronico, true, out temFiltro), filtro, startRow, pageSize, temFiltro);
        }

        public int GetCount(int idManifestoEletronico)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(SqlList(idManifestoEletronico, true, out temFiltro), temFiltro);
        }

        /// <summary>
        /// Obtém o último código retornado pela receita/sistema registrado para a nota passada
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <returns></returns>
        public int ObtemUltimoCodigo(int idManifestoEletronico)
        {
            return ExecuteScalar<int>("SELECT codigo FROM log_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico + " ORDER BY idLogMDFe DESC LIMIT 1");
        }

        /// <summary>
        /// Insere um novo log do MDFe passado
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="evento"></param>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        public uint NewLog(int idManifestoEletronico, string evento, int codigo, string descricao)
        {
            LogMDFe log = new LogMDFe();
            log.IdManifestoEletronico = idManifestoEletronico;
            log.Evento = evento;
            log.Codigo = codigo;
            log.Descricao = !string.IsNullOrEmpty(descricao) && descricao.Length > 255 ? descricao.Substring(0, 255) : descricao;
            log.DataHora = DateTime.Now;

            return Insert(log);
        }
    }
}
