// <copyright file="ITraducaoOrdenacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Interface para os objetos de tradução de ordenação.
    /// </summary>
    public interface ITraducaoOrdenacao
    {
        /// <summary>
        /// Retorna o campo de ordenação traduzido para uso na consulta.
        /// </summary>
        /// <returns>O campo traduzido para a consulta no banco de dados.</returns>
        string ObterTraducaoOrdenacao();
    }
}
