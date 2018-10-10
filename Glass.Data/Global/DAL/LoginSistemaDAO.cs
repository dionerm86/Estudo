using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class LoginSistemaDAO : BaseDAO<LoginSistema, LoginSistemaDAO>
    {
        //private LoginSistemaDAO() { }

        #region Busca padrão

        private string Sql(uint idFunc, int tipo, string dataIni, string dataFim, bool selecionar,
            out string filtroAdicional)
        {
            string criterio = string.Empty;
            filtroAdicional = "";
            string campos = selecionar ? "l.*, f.nome as nomeFunc, '$$$' as Criterio" : "count(*)";

            string sql = "select " + campos + @"
                from login_sistema l
                    inner join funcionario f on (l.idFunc=f.idFunc)
                where 1 ?filtroAdicional?";

            if (idFunc > 0)
            {
                filtroAdicional += " and l.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }
            if (tipo > 0)
            {
                filtroAdicional += " and l.tipo=" + tipo;
                criterio += "Tipo: " + (tipo == 1 ? "Entrou" : "Saiu") + "    ";
            }
            if (!String.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " and l.data>=?dataIni";
                criterio += "Data Inicial: " + dataIni + "    ";
            }
            if (!String.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " and l.data<=?dataFim";
                criterio += "Data Final: " + dataFim + "    ";
            }
            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
            {
                if (!dataIni.Contains(":")) dataIni += " 00:00";
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                if (!dataFim.Contains(":")) dataFim += " 23:59";
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));
            }

            return lst.ToArray();
        }

        public IList<LoginSistema> GetList(uint idFunc, int tipo, string dataIni, string dataFim, 
            string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(idFunc, tipo, dataIni, dataFim, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "l.data desc";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, 
                GetParams(dataIni, dataFim));
        }

        public IList<LoginSistema> GetForRpt(uint idFunc, int tipo, string dataIni, string dataFim)
        {
            string filtroAdicional;
            string sql = Sql(idFunc, tipo, dataIni, dataFim, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);


            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim)).ToList();
        }

        public int GetCount(uint idFunc, int tipo, string dataIni, string dataFim)
        {
            string filtroAdicional;
            string sql = Sql(idFunc, tipo, dataIni, dataFim, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(dataIni, dataFim));
        }

        #endregion

        #region Métodos de cadastro

        /// <summary>
        /// Indica a entrada de um usuário no sistema.
        /// </summary>
        /// <param name="idFunc"></param>
        public void Entrar(uint idFunc, string usuarioSync)
        {
            if (IsUsuarioLogado(idFunc))
                return;

            LoginSistema novo = new LoginSistema();
            novo.IdFunc = idFunc;
            novo.Data = DateTime.Now;
            novo.Tipo = LoginSistema.TipoEnum.Entrou;
            novo.UsuarioSync = usuarioSync;

            Insert(novo);
        }

        /// <summary>
        /// Indica a saída de um usuário do sistema.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="manual">A saída foi manual? (Através do botão Logout)</param>
        public void Sair(uint idFunc, string usuarioSync, bool manual)
        {
            if (!IsUsuarioLogado(idFunc))
                return;

            LoginSistema novo = new LoginSistema();
            novo.IdFunc = idFunc;
            novo.Data = DateTime.Now;
            novo.Tipo = LoginSistema.TipoEnum.Saiu;
            novo.Manual = manual;
            novo.UsuarioSync = usuarioSync;

            Insert(novo);
        }

        #endregion

        /// <summary>
        /// Verifica se um usuário está marcado como logado.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public bool IsUsuarioLogado(uint idFunc)
        {
            string sql = "select tipo from login_sistema where idFunc=" + idFunc +
                " order by data desc limit 1";

            return ExecuteScalar<LoginSistema.TipoEnum>(sql) == LoginSistema.TipoEnum.Entrou;
        }
    }
}
