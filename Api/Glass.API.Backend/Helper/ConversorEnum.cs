// <copyright file="ConversorEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe base para a tradução de campos para ordenação.
    /// </summary>
    /// <typeparam name="T">O tipo do Enum que será convertido.</typeparam>
    internal class ConversorEnum<T>
        where T : struct
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorEnum{T}"/>.
        /// </summary>
        public ConversorEnum()
        {
            if (!typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException("Tipo T precisa ser um Enum.", "T");
            }
        }

        /// <summary>
        /// Obtém uma lista com o ID e nome dos possíveis valores de um enum.
        /// </summary>
        /// <returns>Uma lista de IdNomeDto com os dados do enum.</returns>
        public IEnumerable<IdNomeDto> ObterTraducao()
        {
            var lista = new List<IdNomeDto>();

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                lista.Add(new IdNomeDto
                {
                    Id = (int)item,
                    Nome = this.ObterDescricao(item as Enum),
                });
            }

            return lista;
        }

        /// <summary>
        /// Obtém um dicionário (que é convertido para um objeto ao serializar o JSON) onde
        /// os nomes dos campos são os nomes dos enums, e os valores são os dados "traduzidos" (ID e descrição).
        /// </summary>
        /// <returns>Um dicionário com os dados para criação de objeto JSON.</returns>
        public IDictionary<string, IdNomeDto> ObterObjetoComTraducao()
        {
            var dicionario = new Dictionary<string, IdNomeDto>();

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                dicionario.Add(
                    this.FormatarParaCamelCase(item.ToString()),
                    new IdNomeDto
                    {
                        Id = (int)item,
                        Nome = this.ObterDescricao(item as Enum),
                    });
            }

            return dicionario;
        }

        private string ObterDescricao(Enum valor)
        {
            if (valor == null)
            {
                return null;
            }

            var atributo = typeof(T).GetField(valor.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return atributo != null && atributo.Length > 0
                ? atributo[0].Description
                : null;
        }

        private string FormatarParaCamelCase(string nome)
        {
            return string.IsNullOrWhiteSpace(nome)
                ? nome
                : nome[0].ToString().ToLower() + nome.Substring(1);
        }
    }
}
