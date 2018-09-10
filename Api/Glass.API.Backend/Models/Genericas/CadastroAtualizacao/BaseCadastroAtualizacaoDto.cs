// <copyright file="BaseCadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Glass.API.Backend.Models.Genericas.CadastroAtualizacao
{
    /// <summary>
    /// Classe base para os DTO de cadastro e atualização.
    /// </summary>
    /// <typeparam name="T">O tipo do DTO.</typeparam>
    public class BaseCadastroAtualizacaoDto<T> : IDictionary<string, object>
        where T : BaseCadastroAtualizacaoDto<T>
    {
        private readonly IDictionary<string, object> valores;

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

        /// <summary>
        /// Recupera o valor que será utilizado para a conversão do DTO para o modelo.
        /// </summary>
        /// <typeparam name="TPropriedade">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <param name="valorModelo">O valor atual do campo no modelo.</param>
        /// <returns>O valor normalizado para uso no modelo.</returns>
        internal TPropriedade ObterValorNormalizado<TPropriedade>(Expression<Func<T, TPropriedade>> campoEnviado, TPropriedade valorModelo)
        {
            var propriedade = this.ObterPropriedade(campoEnviado);
            var nomeCampo = this.ObterNomeCampoJSON(propriedade);

            var campoInformado = this.valores.ContainsKey(nomeCampo);

            if (!campoInformado)
            {
                return valorModelo;
            }

            var valorInformado = (TPropriedade)propriedade.GetValue(this);
            return valorInformado;
        }

        /// <summary>
        /// Recupera o valor do dicionário interno.
        /// </summary>
        /// <typeparam name="TPropriedade">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <returns>O valor convertido.</returns>
        protected TPropriedade ObterValor<TPropriedade>(Expression<Func<T, TPropriedade>> campoEnviado)
        {
            var propriedade = this.ObterPropriedade(campoEnviado);
            var nomeCampo = this.ObterNomeCampoJSON(propriedade);

            var valorInformado = this.valores[nomeCampo];

            if (valorInformado is JArray)
            {
                valorInformado = (valorInformado as JArray).Values<object>();
            }

            return (TPropriedade)valorInformado;
        }

        /// <summary>
        /// Adiciona um valor correspondente a um campo do DTO no dicionário interno.
        /// </summary>
        /// <typeparam name="TPropriedade">O tipo de retorno do método.</typeparam>
        /// <param name="campoEnviado">O campo do DTO que está sendo avaliado.</param>
        /// <param name="valor">O valor que será adicionado ao dicionário.</param>
        protected void AdicionarValor<TPropriedade>(Expression<Func<T, TPropriedade>> campoEnviado, TPropriedade valor)
        {
            var propriedade = this.ObterPropriedade(campoEnviado);
            var nomeCampo = this.ObterNomeCampoJSON(propriedade);

            this.valores[nomeCampo] = valor;
        }

        private PropertyInfo ObterPropriedade<TPropriedade>(Expression<Func<T, TPropriedade>> campoEnviado)
        {
            var type = typeof(T);
            var member = campoEnviado.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException($"Expressão '{campoEnviado}' é um método, e não uma propriedade.");
            }

            var propInfo = member.Member as PropertyInfo;

            if (propInfo == null)
            {
                throw new ArgumentException($"Expressão '{campoEnviado}' é um campo, e não uma propriedade.");
            }

            return propInfo;
        }

        private string ObterNomeCampoJSON(PropertyInfo propriedade)
        {
            var atributo = propriedade.GetCustomAttribute<JsonPropertyAttribute>(false);

            return atributo != null
                ? atributo.PropertyName
                : propriedade.Name;
        }
    }
}
