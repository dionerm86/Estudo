// <copyright file="ConversorValorTipo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.CadastroAtualizacao
{
    /// <summary>
    /// Classe responsável pela conversão de tipos do JSON para o DTO.
    /// </summary>
    /// <typeparam name="T">O tipo de valor do DTO para conversão.</typeparam>
    internal class ConversorValorTipo<T>
    {
        private readonly Type tipoConverter;
        private readonly Type tipoEnum;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorValorTipo{T}"/>.
        /// </summary>
        public ConversorValorTipo()
        {
            this.tipoConverter = typeof(T);
            this.tipoEnum = null;

            if (this.tipoConverter.IsGenericType)
            {
                this.tipoConverter = this.tipoConverter.GenericTypeArguments[0];
            }

            if (this.tipoConverter.IsEnum)
            {
                this.tipoEnum = this.tipoConverter;
            }

            if (typeof(T).UnderlyingSystemType.Name == typeof(Nullable<>).Name)
            {
                this.tipoConverter = typeof(T);
            }
        }

        /// <summary>
        /// Converte o valor atual para o tipo desejado.
        /// </summary>
        /// <param name="valorInformado">O objeto que será convertido.</param>
        /// <returns>O valor convertido.</returns>
        public T Converter(object valorInformado)
        {
            if (valorInformado == null)
            {
                return default(T);
            }

            var valorArray = valorInformado as JArray;

            if (valorArray != null)
            {
                return this.ConverterArray(valorArray);
            }

            var valorObjeto = valorInformado as JObject;

            if (valorObjeto != null)
            {
                return this.ConverterObjeto(valorObjeto);
            }

            return this.ConverterValor(valorInformado);
        }

        private T ConverterArray(JArray valorInformado)
        {
            var tipo = this.tipoEnum ?? this.tipoConverter;
            var conversor = Activator.CreateInstance(typeof(ConversorValorTipo<>).MakeGenericType(tipo));
            var metodo = conversor.GetType().GetMethod("Converter");

            var itens = valorInformado.Values()
                .Select(item => metodo.Invoke(conversor, new[] { item }));

            var itensConvertidos = typeof(Enumerable).GetMethod("Cast")
                .MakeGenericMethod(tipo)
                .Invoke(null, new[] { itens });

            return (T)itensConvertidos;
        }

        private T ConverterObjeto(JObject valorInformado)
        {
            return default(T);
        }

        private T ConverterValor(object valorInformado)
        {
            var valor = valorInformado as JValue;
            var valorUsar = valor != null
                ? valor.Value
                : valorInformado;

            if (this.tipoEnum != null)
            {
                return this.ConverterEnum(valorUsar);
            }

            return (T)valorUsar;
        }

        private T ConverterEnum(object valorInformado)
        {
            try
            {
                return (T)Enum.Parse(this.tipoEnum, valorInformado.ToString(), true);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
