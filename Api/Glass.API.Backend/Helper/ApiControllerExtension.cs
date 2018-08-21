// <copyright file="ApiControllerExtension.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend
{
    /// <summary>
    /// Classe com métodos de extensão para o tipo <see cref="ApiController"/>.
    /// </summary>
    internal static class ApiControllerExtension
    {
        /// <summary>
        /// Retorna uma resposta para um único item, com código de resposta HTTP 200.
        /// </summary>
        /// <typeparam name="T">O tipo do item.</typeparam>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="item">O item que será retornado.</param>
        /// <returns>A resposta HTTP.</returns>
        public static Item<T> Item<T>(this ApiController apiController, T item)
        {
            return new Item<T>(apiController, item);
        }

        /// <summary>
        /// Retorna uma resposta para uma lista não paginável.
        /// </summary>
        /// <typeparam name="T">O tipo do item.</typeparam>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="lista">A lista de itens que será retornada.</param>
        /// <returns>A resposta HTTP.</returns>
        public static Lista<T> Lista<T>(this ApiController apiController, IEnumerable<T> lista)
        {
            return new Lista<T>(apiController, lista);
        }

        /// <summary>
        /// Retorna uma resposta para uma lista paginável.
        /// </summary>
        /// <typeparam name="T">O tipo do item.</typeparam>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="lista">A lista de itens que será retornada</param>
        /// <param name="paginacao">O objeto com os dados de paginação.</param>
        /// <param name="contagemRegistros">Função que retorna o número total de registros encontrados.</param>
        /// <returns>A resposta HTTP.</returns>
        public static ListaPaginada<T> ListaPaginada<T>(this ApiController apiController, IEnumerable<T> lista, PaginacaoDto paginacao, Func<long> contagemRegistros)
        {
            return new ListaPaginada<T>(apiController, lista, paginacao.Pagina, paginacao.NumeroRegistros, contagemRegistros);
        }

        /// <summary>
        /// Retorna uma resposta para um erro de validação.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <param name="erro">O erro ocorrido.</param>
        /// <returns>A resposta HTTP.</returns>
        public static ErroValidacao ErroValidacao(this ApiController apiController, string mensagem, Exception erro = null)
        {
            return new ErroValidacao(apiController, mensagem, erro);
        }

        /// <summary>
        /// Retorna uma resposta para um erro interno do servidor.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <param name="erro">O erro ocorrido.</param>
        /// <returns>A resposta HTTP.</returns>
        public static ErroInternoServidor ErroInternoServidor(this ApiController apiController, string mensagem, Exception erro = null)
        {
            return new ErroInternoServidor(apiController, mensagem, erro);
        }

        /// <summary>
        /// Retorna uma resposta para um item aceito.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de sucesso.</param>
        /// <returns>A resposta HTTP.</returns>
        public static Aceito Aceito(this ApiController apiController, string mensagem)
        {
            return new Aceito(apiController, mensagem);
        }

        /// <summary>
        /// Retorna uma resposta para um item criado.
        /// </summary>
        /// <typeparam name="T">O tipo do identificador do item.</typeparam>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de sucesso.</param>
        /// <param name="identificadorItem">O identificador do item criado.</param>
        /// <returns>A resposta HTTP.</returns>
        public static Criado<T> Criado<T>(this ApiController apiController, string mensagem, T identificadorItem)
        {
            return new Criado<T>(apiController, mensagem, identificadorItem);
        }

        /// <summary>
        /// Retorna uma resposta para um item com múltipla escolha.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de sucesso.</param>
        /// <returns>A resposta HTTP.</returns>
        public static MultiplaEscolha MultiplaEscolha(this ApiController apiController, string mensagem)
        {
            return new MultiplaEscolha(apiController, mensagem);
        }

        /// <summary>
        /// Retorna uma resposta para um item não encontrado.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <returns>A resposta HTTP.</returns>
        public static NaoEncontrado NaoEncontrado(this ApiController apiController, string mensagem)
        {
            return new NaoEncontrado(apiController, mensagem);
        }

        /// <summary>
        /// Retorna uma resposta para uma lista vazia.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <returns>A resposta HTTP.</returns>
        public static SemConteudo SemConteudo(this ApiController apiController)
        {
            return new SemConteudo(apiController);
        }

        /// <summary>
        /// Retorna uma resposta para um único item, com código de resposta HTTP 406.
        /// </summary>
        /// <typeparam name="T">O tipo do item.</typeparam>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="item">O item que será retornado.</param>
        /// <returns>A resposta HTTP.</returns>
        public static NaoAceitavel<T> NaoAceitavel<T>(this ApiController apiController, T item)
        {
            return new NaoAceitavel<T>(apiController, item);
        }
    }
}
