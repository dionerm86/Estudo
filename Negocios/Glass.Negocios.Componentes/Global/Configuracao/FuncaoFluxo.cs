using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    public class FuncaoFluxo : IFuncaoFluxo
    {
        /// <summary>
        /// Recuper as funções do sistema
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<Entidades.FuncaoMenu> ObterFuncoes()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FuncaoMenu>()
                .ProcessResult<Entidades.FuncaoMenu>()
                .ToList();
        }

        /// <summary>
        /// Recupera as funções do funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<Entidades.ConfigFuncaoFunc> ObterFuncoesFuncioario(int idFunc)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ConfigFuncaoFunc>()
                .Where("IdFunc=?id")
                .Add("?id", idFunc)
                .ProcessResult<Entidades.ConfigFuncaoFunc>()
                .ToList();
        }
    }
}
