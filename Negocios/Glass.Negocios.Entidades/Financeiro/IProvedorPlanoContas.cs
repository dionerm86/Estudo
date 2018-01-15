using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    public interface IProvedorPlanoContas
    {
        /// <summary>
        /// Recupera os dados do plano de contas.
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        Entidades.PlanoContas ObtemPlanoContas(int idConta);

        /// <summary>
        /// Recupera os dados do grupo de conta.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo.</param>
        /// <returns></returns>
        Entidades.GrupoConta ObtemGrupoConta(int idGrupo);
    }
}
