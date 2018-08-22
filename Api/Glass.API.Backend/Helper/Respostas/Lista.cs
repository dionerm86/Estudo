// <copyright file="Lista.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que contém uma resposta com uma lista não paginável de itens.
    /// </summary>
    /// <typeparam name="T">O tipo de item da lista.</typeparam>
    internal class Lista<T> : ListaPaginada<T>
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Lista{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="lista">A lista de itens que será retornada.</param>
        public Lista(ApiController apiController, IEnumerable<T> lista)
            : base(apiController, lista, 1, lista.Count(), null)
        {
        }
    }
}
