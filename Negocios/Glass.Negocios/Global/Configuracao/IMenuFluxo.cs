using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de menu.
    /// </summary>
    public interface IMenuFluxo
    {
        /// <summary>
        /// Recupera as funções do funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        IList<Entidades.Menu> ObterMenusPorConfig(int idLoja);

        /// <summary>
        /// Obtém os menus que o funcionário tem acesso
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        IEnumerable<Entidades.Menu> ObterMenusPorFuncionario(Entidades.Funcionario funcionario);

        /// <summary>
        /// Remove da memória os menus do funcionário passado
        /// </summary>
        /// <param name="idFunc"></param>
        void RemoveMenuFuncMemoria(int idFunc);

        /// <summary>
        /// Limpa o menu da memóra
        /// </summary>
        void RemoveMenuMemoria(int[] idConfig);
    }
}
