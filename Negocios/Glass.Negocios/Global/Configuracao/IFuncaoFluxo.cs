using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de função.
    /// </summary>
    public interface IFuncaoFluxo
    {
        /// <summary>
        /// Recuper as funções do sistema
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        IList<Entidades.FuncaoMenu> ObterFuncoes();

        /// <summary>
        /// Recupera as funções do funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        IList<Entidades.ConfigFuncaoFunc> ObterFuncoesFuncioario(int idFunc);
    }
}
