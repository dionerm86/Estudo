// <copyright file="BaseCadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.CadastroAtualizacao;
using Glass.Comum.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Glass.API.Backend.Models.Genericas.CadastroAtualizacao
{
    /// <summary>
    /// Classe base para os DTO de cadastro e atualização.
    /// </summary>
    /// <typeparam name="T">O tipo do DTO.</typeparam>
    public class BaseCadastroAtualizacaoDto<T> : IDictionary<string, object>
        where T : BaseCadastroAtualizacaoDto<T>
    {
        private static CacheMemoria<object, string> CACHE_CONVERSORES;
        private readonly IDictionary<string, object> valores;

        static BaseCadastroAtualizacaoDto()
        {
            CACHE_CONVERSORES = new CacheMemoria<object, string>("cacheConversores", 10);
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseCadastroAtualizacaoDto{T}"/>.
        /// </summary>
        public BaseCadastroAtualizacaoDto()
        {
            this.valores = new Dictionary<string, object>();
        }

        /// <inheritdoc/>
        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return this.valores.Keys; }
        }

        /// <inheritdoc/>
        ICollection<object> IDictionary<string, object>.Values
        {
            get { return this.valores.Values; }
        }

        /// <inheritdoc/>
        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return this.valores.Count; }
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return this.valores.IsReadOnly; }
        }

        /// <inheritdoc/>
        object IDictionary<string, object>.this[string key]
        {
            get
            {
                object valor;
                return this.valores.TryGetValue(key, out valor)
                    ? valor
                    : null;
            }

            set
            {
                this.valores[key] = value;
            }
        }

        private static ConversorValoresDtoModelo<T, U> ObterConversor<U>(Expression<Func<T, U>> campoDto)
        {
            var id = $"{typeof(T).FullName}-{typeof(U).FullName}-{campoDto.ToString()}";
            var conversorCache = CACHE_CONVERSORES.RecuperarDoCache(id);

            if (conversorCache == null)
            {
                conversorCache = new ConversorValoresDtoModelo<T, U>(campoDto);
                CACHE_CONVERSORES.AtualizarItemNoCache(conversorCache, id);
            }

            return (ConversorValoresDtoModelo<T, U>)conversorCache;
        }

        /// <summary>
        /// Verifica se um campo foi informado no JSON.
        /// </summary>
        /// <typeparam name="U">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <returns>Verdadeiro, se o campo está presente no JSON.</returns>
        internal bool VerificarCampoInformado<U>(Expression<Func<T, U>> campoEnviado)
        {
            var conversor = ObterConversor(campoEnviado);
            return conversor.VerificarCampoInformado(this.valores);
        }

        /// <summary>
        /// Recupera o valor que será utilizado para a conversão do DTO para o modelo.
        /// </summary>
        /// <typeparam name="U">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <param name="valorModelo">O valor atual do campo no modelo.</param>
        /// <returns>O valor normalizado para uso no modelo.</returns>
        internal U ObterValorNormalizado<U>(Expression<Func<T, U>> campoEnviado, U valorModelo)
        {
            var conversor = ObterConversor(campoEnviado);

            if (!conversor.VerificarCampoInformado(this.valores))
            {
                return valorModelo;
            }

            var valorInformado = conversor.ObterValorPropriedade(this);
            return valorInformado;
        }

        /// <summary>
        /// Recupera o valor do dicionário interno.
        /// </summary>
        /// <typeparam name="U">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <returns>O valor convertido.</returns>
        protected U ObterValor<U>(Expression<Func<T, U>> campoEnviado)
        {
            var conversor = ObterConversor(campoEnviado);
            return conversor.ObterValor(this.valores);
        }

        /// <summary>
        /// Adiciona um valor correspondente a um campo do DTO no dicionário interno.
        /// </summary>
        /// <typeparam name="U">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <param name="valor">O valor que será adicionado ao dicionário.</param>
        protected void AdicionarValor<U>(Expression<Func<T, U>> campoEnviado, U valor)
        {
            var conversor = ObterConversor(campoEnviado);
            conversor.AdicionarValor(this.valores, valor);
        }

        /// <inheritdoc/>
        void IDictionary<string, object>.Add(string key, object value)
        {
            this.valores.Add(key, value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            this.valores.Add(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            this.valores.Clear();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return this.valores.Contains(item);
        }

        /// <inheritdoc/>
        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return this.valores.ContainsKey(key);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this.valores.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool IDictionary<string, object>.Remove(string key)
        {
            return this.valores.Remove(key);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return this.valores.Remove(item);
        }

        /// <inheritdoc/>
        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return this.valores.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this.valores.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.valores as IEnumerable).GetEnumerator();
        }
    }
}
