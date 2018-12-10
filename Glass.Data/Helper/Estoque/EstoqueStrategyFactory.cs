// <copyright file="EstoqueStrategyFactory.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper.Estoque.Estrategia;
using Glass.Data.Helper.Estoque.Estrategia.Cenarios;

namespace Glass.Data.Helper.Estoque
{
    /// <summary>
    /// Factory das estratégias de estoque.
    /// </summary>
    public class EstoqueStrategyFactory
    {
        /// <summary>
        /// Recupera a estratégia a ser usada para controle de estoque.
        /// </summary>
        /// <param name="cenario">Cenário que será utilizado.</param>
        /// <returns>Estratégia de controle de estoque.</returns>
        public IEstoqueStrategy RecuperaEstrategia(Cenario cenario)
        {
            switch (cenario)
            {
                case Cenario.Generica:
                    return new GenericaStrategy();

                default:
                    return null;
            }
        }
    }
}