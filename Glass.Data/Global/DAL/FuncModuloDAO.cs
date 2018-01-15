using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FuncModuloDAO : BaseDAO<FuncModulo, FuncModuloDAO>
    {
        //private FuncModuloDAO() { }

        public FuncModulo GetByModuloFunc(uint idModulo, uint idFunc)
        {
            return GetByModuloFunc(null, idModulo, idFunc);
        }

        public FuncModulo GetByModuloFunc(GDASession session, uint idModulo, uint idFunc)
        {
            string sql = "Select m.idModulo, (select fmp.idFunc from func_modulo fmp where " +
                "fmp.idModulo=m.idModulo and fmp.idFunc=" + idFunc + ") as idFunc, (select fmp.permitir from func_modulo fmp where " +
                "fmp.idModulo=m.idModulo and fmp.idFunc=" + idFunc + ") as Permitir, m.Descricao as DescrModulo From modulo m " +
                "Where m.idModulo=" + idModulo + " Order By Descricao";

            return objPersistence.LoadOneData(session, sql);
        }

        /// <summary>
        /// Busca os módulos para serem associados ao funcionário passado
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<FuncModulo> GetByFunc(uint idFunc)
        {
            if (idFunc == 0)
                return null;

            string sql = "Select m.idModulo, (select fmp.idFunc from func_modulo fmp where " +
                "fmp.idModulo=m.idModulo and fmp.idFunc=" + idFunc + ") as idFunc, (select fmp.permitir from func_modulo fmp where " +
                "fmp.idModulo=m.idModulo and fmp.idFunc=" + idFunc + ") as Permitir, m.Descricao as DescrModulo, m.grupo as GrupoModulo "+
                "From modulo m " +
                "Where m.Situacao=" + (int)Glass.Situacao.Ativo +
                " Order By GrupoModulo, Descricao";
             
            return objPersistence.LoadData(sql).ToList();
        }

        public uint[] GetModuleIdByFunc(uint idFunc)
        {
            string sql = "Select * From func_modulo Where permitir=1 And idFunc=" + idFunc;

            var retorno = new List<uint>();
            foreach (FuncModulo f in objPersistence.LoadData(sql))
                retorno.Add((uint)f.IdModulo);

            return retorno.ToArray();
        }

        public FuncModulo GetByLog(uint idLog)
        {
            return GetByLog(null, idLog);
        }

        public FuncModulo GetByLog(GDASession session, uint idLog)
        {
            string dadosLog = idLog.ToString();
            uint idModulo = Glass.Conversoes.StrParaUint(dadosLog.Substring(0, dadosLog.Length - 4));
            uint idFunc = Glass.Conversoes.StrParaUint(dadosLog.Substring(dadosLog.Length - 4));

            return GetByModuloFunc(session, idModulo, idFunc);
        }
    }
}
