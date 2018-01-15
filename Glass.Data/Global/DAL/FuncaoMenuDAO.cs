using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class FuncaoMenuDAO : BaseDAO<FuncaoMenu, FuncaoMenuDAO>
    {
        /// <summary>
        /// Recupera as funções do funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public List<FuncaoMenu> ObterFuncoesFuncioario(int idFunc)
        {
            return objPersistence.LoadData(string.Format("Select * From funcao_menu Where IdFuncaoMenu In (Select IdFuncaoMenu From config_funcao_func Where IdFunc={0})", idFunc));
        }

        /// <summary>
        /// Retorna o IdFuncaoMenu com base no IdFuncao e IdModulo
        /// </summary>
        /// <param name="idFuncao"></param>
        /// <param name="idModulo"></param>
        /// <returns></returns>
        public int ObterIdFuncaoMenu(int idFuncao, int idModulo)
        {
            return ExecuteScalar<int>(string.Format("Select IdFuncaoMenu From funcao_menu Where IdFuncao={0} And IdModulo={1}", idFuncao, idModulo));
        }
    }
}
