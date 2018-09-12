// <copyright file="ConversorValoresDtoModelo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Glass.API.Backend.Helper.CadastroAtualizacao
{
    /// <summary>
    /// Classe responsável pelos métodos de conversão entre o DTO e o modelo.
    /// </summary>
    /// <typeparam name="T">O tipo do DTO.</typeparam>
    /// <typeparam name="U">O tipo da propriedade que será convertida.</typeparam>
    internal class ConversorValoresDtoModelo<T, U>
        where T : BaseCadastroAtualizacaoDto<T>
    {
        private readonly Expression<Func<T, U>> campoDto;
        private readonly PropertyInfo propriedade;
        private readonly string nomeCampo;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorValoresDtoModelo{T, U}"/>.
        /// </summary>
        /// <param name="campoDto">A expressão que indica o campo do DTO.</param>
        public ConversorValoresDtoModelo(Expression<Func<T, U>> campoDto)
        {
            this.campoDto = campoDto;
            this.propriedade = this.ObterPropriedade();
            this.nomeCampo = this.ObterNomeCampoJSON();
        }

        /// <summary>
        /// Verifica se o campo representado pela classe foi informado no JSON do DTO.
        /// </summary>
        /// <param name="valores">Os valores informados no JSON, convertidos para um dicionário.</param>
        /// <returns>Verdadeiro, se o campo foi informado.</returns>
        public bool VerificarCampoInformado(IDictionary<string, object> valores)
        {
            return valores.ContainsKey(this.nomeCampo);
        }

        /// <summary>
        /// Adiciona um valor para a propriedade atual à lista de valores informadas no JSON.
        /// </summary>
        /// <param name="valores">Os valores informados no JSON, convertidos para um dicionário.</param>
        /// <param name="valor">O valor que será adicionado.</param>
        public void AdicionarValor(IDictionary<string, object> valores, U valor)
        {
            valores[this.nomeCampo] = valor;
        }

        /// <summary>
        /// Recupera o valor que será utilizado para a conversão do DTO para o modelo.
        /// </summary>
        /// <param name="valores">Os valores informados no JSON, convertidos para um dicionário.</param>
        /// <param name="valorModelo">O valor atual do campo no modelo.</param>
        /// <returns>O valor normalizado para uso no modelo.</returns>
        public U ObterValorNormalizado(IDictionary<string, object> valores, U valorModelo)
        {
            var campoInformado = this.VerificarCampoInformado(valores);

            if (!campoInformado)
            {
                return valorModelo;
            }

            var valorInformado = (U)this.propriedade.GetValue(this);
            return valorInformado;
        }

        /// <summary>
        /// Recupera o valor do dicionário interno.
        /// </summary>
        /// <param name="valores">Os valores informados no JSON, convertidos para um dicionário.</param>
        /// <returns>O valor convertido.</returns>
        public U ObterValor(IDictionary<string, object> valores)
        {
            if (!this.VerificarCampoInformado(valores))
            {
                throw new ArgumentNullException("valores", "Campo não está presente nos valores informados.");
            }

            var valorInformado = valores[this.nomeCampo];
            return new ConversorValorTipo<U>().Converter(valorInformado);
        }

        /// <summary>
        /// Recupera o valor atual da propriedade para um item.
        /// </summary>
        /// <param name="item">O item que terá a propriedade lida.</param>
        /// <returns>O valor atual da propriedade.</returns>
        public U ObterValorPropriedade(object item)
        {
            return (U)this.propriedade.GetValue(item);
        }

        private PropertyInfo ObterPropriedade()
        {
            var membroExpressao = this.campoDto.Body as MemberExpression;

            if (membroExpressao == null)
            {
                throw new ArgumentException($"Expressão '{this.campoDto}' é um método, e não uma propriedade.");
            }

            var propriedadeExpressao = membroExpressao.Member as PropertyInfo;

            if (propriedadeExpressao == null)
            {
                throw new ArgumentException($"Expressão '{this.campoDto}' é um campo, e não uma propriedade.");
            }

            return propriedadeExpressao;
        }

        private string ObterNomeCampoJSON()
        {
            var atributo = this.propriedade.GetCustomAttribute<JsonPropertyAttribute>(false);

            return atributo != null
                ? atributo.PropertyName
                : this.propriedade.Name;
        }
    }
}
