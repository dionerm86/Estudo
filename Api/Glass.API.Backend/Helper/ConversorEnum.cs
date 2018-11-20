// <copyright file="ConversorEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe base para a tradução de campos para ordenação.
    /// </summary>
    /// <typeparam name="T">O tipo do Enum que será convertido.</typeparam>
    internal class ConversorEnum<T>
        where T : struct
    {
        private readonly ISet<T> listaIgnorados;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorEnum{T}"/>.
        /// </summary>
        public ConversorEnum()
        {
            if (!typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException("Tipo T precisa ser um Enum.", "T");
            }

            this.listaIgnorados = new HashSet<T>();
        }

        /// <summary>
        /// Obtém uma lista com o ID e nome dos possíveis valores de um enum.
        /// </summary>
        /// <returns>Uma lista de IdNomeDto com os dados do enum.</returns>
        public IEnumerable<IdNomeDto> ObterTraducao()
        {
            var lista = new List<IdNomeDto>();

            foreach (var item in this.ObterItens())
            {
                lista.Add(this.ConverterItem(item));
            }

            return lista;
        }

        /// <summary>
        /// Obtém uma lista com o código e nome dos possíveis valores de um enum.
        /// </summary>
        /// <returns>Uma lista de CodigoNomeDto com os dados do enum.</returns>
        public IEnumerable<CodigoNomeDto> ObterTraducaoCodigoNome()
        {
            var lista = new List<CodigoNomeDto>();

            foreach (var item in this.ObterItens())
            {
                lista.Add(this.ConverterItemCodigoNome(item));
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

            foreach (var item in this.ObterItens())
            {
                dicionario.Add(
                    this.FormatarParaCamelCase(item.ToString()),
                    this.ConverterItem(item));
            }

            return dicionario;
        }

        /// <summary>
        /// Adiciona itens que devem ser ignorados no momento da tradução.
        /// </summary>
        /// <param name="itens">Os itens que devem ser ignorados.</param>
        /// <returns>O próprio objeto de conversão.</returns>
        public ConversorEnum<T> AdicionarIgnorados(params T[] itens)
        {
            foreach (var item in itens)
            {
                this.listaIgnorados.Add(item);
            }

            return this;
        }

        private IEnumerable ObterItens()
        {
            var validarItensIgnorados = this.listaIgnorados.Any();

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                bool retornar = true;

                if (validarItensIgnorados)
                {
                    var valorConvertido = (T)item;
                    retornar = !this.listaIgnorados.Contains(valorConvertido);
                }

                if (retornar)
                {
                    yield return item;
                }
            }
        }

        private IdNomeDto ConverterItem(object item)
        {
            return new IdNomeDto
            {
                Id = (int)Convert.ChangeType(item, typeof(int)),
                Nome = this.ObterDescricao(item as Enum) ?? item.ToString(),
            };
        }

        private CodigoNomeDto ConverterItemCodigoNome(object item)
        {
            return new CodigoNomeDto
            {
                Codigo = Convert.ChangeType(item, typeof(char))?.ToString(),
                Nome = this.ObterDescricao(item as Enum) ?? item.ToString(),
            };
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
