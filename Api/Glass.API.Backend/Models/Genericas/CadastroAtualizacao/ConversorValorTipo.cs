// <copyright file="ConversorValorTipo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Glass.API.Backend.Models.Genericas.CadastroAtualizacao
{
    /// <summary>
    /// Classe responsável pela conversão de tipos do JSON para o DTO.
    /// </summary>
    internal class ConversorValorTipo
    {
        private static readonly MethodInfo METODO_CONVERTER;
        private static readonly MethodInfo METODO_CONVERTER_ARRAY;

        private Type tipoBaseConverter;
        private Type tipoBaseEnum;

        static ConversorValorTipo()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            METODO_CONVERTER = typeof(ConversorValorTipo).GetMethod("IniciarConversao", flags);
            METODO_CONVERTER_ARRAY = typeof(ConversorValorTipo).GetMethod("ConverterArray", flags);
        }

        /// <summary>
        /// Converte o valor atual para o tipo desejado.
        /// </summary>
        /// <typeparam name="T">O tipo de valor do DTO para conversão.</typeparam>
        /// <param name="valorInformado">O objeto que será convertido.</param>
        /// <returns>O valor convertido.</returns>
        public T Converter<T>(object valorInformado)
        {
            return this.IniciarConversao<T>(valorInformado);
        }

        private T IniciarConversao<T>(object valorInformado)
        {
            this.Iniciar(typeof(T));

            var valorArray = valorInformado as JArray;

            if (valorArray != null)
            {
                return (T)this.ConverterArray(valorArray, typeof(T));
            }

            var valor = valorInformado as JValue;

            var valorUsar = valor != null
                ? valor.Value
                : valorInformado;

            return this.ConverterItem<T>(valorUsar);
        }

        private void Iniciar(Type tipo)
        {
            this.tipoBaseConverter = tipo;
            this.tipoBaseEnum = null;

            if (this.tipoBaseConverter.IsGenericType)
            {
                this.tipoBaseConverter = this.tipoBaseConverter.GenericTypeArguments[0];
            }

            if (this.tipoBaseConverter.IsEnum)
            {
                this.tipoBaseEnum = typeof(long);
            }

            if (tipo.UnderlyingSystemType.Name == typeof(Nullable<>).Name)
            {
                this.tipoBaseConverter = tipo;
            }
        }

        private IEnumerable ConverterArray(JArray valorInformado, Type tipoInicial)
        {
            var tipo = this.tipoBaseEnum ?? this.tipoBaseConverter;
            var metodo = METODO_CONVERTER.MakeGenericMethod(tipo);

            var itens = valorInformado.Values()
                .Select(item => metodo.Invoke(this, new[] { item }))
                .ToList();

            this.Iniciar(tipoInicial);
            return itens;
        }

        private T ConverterItem<T>(object valorInformado)
        {
            var valorEnum = this.tipoBaseEnum != null
                ? Convert.ChangeType(valorInformado, this.tipoBaseEnum)
                : valorInformado;

            return (T)(object)valorEnum;
        }
    }

}
