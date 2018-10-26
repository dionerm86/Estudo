// <copyright file="GerenciadorOperacaoIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o gerenciador de operações de integração.
    /// </summary>
    public class GerenciadorOperacaoIntegracao
    {
        private readonly Dictionary<string, Operacao> operacoes = new Dictionary<string, Operacao>();

        /// <summary>
        /// Obtém as operações registradas no gerenciador.
        /// </summary>
        public IEnumerable<OperacaoIntegracao> Operacoes => this.operacoes.Select(f => f.Value.Definicao);

        /// <summary>
        /// Adiciona a operação para o gerenciador.
        /// </summary>
        /// <param name="operacao">Definição da operação.</param>
        /// <param name="chamada">Chamada da operação.</param>
        public void Adicionar(OperacaoIntegracao operacao, Delegate chamada)
        {
            if (operacao == null)
            {
                throw new ArgumentNullException(nameof(operacao));
            }

            if (chamada == null)
            {
                throw new ArgumentNullException(nameof(chamada));
            }

            if (this.operacoes.ContainsKey(operacao.Nome))
            {
                throw new ArgumentException($"Já existem uma operação com o nome {operacao.Nome}.");
            }

            this.operacoes.Add(operacao.Nome, new Operacao(operacao, chamada));
        }

        /// <summary>
        /// Executa a operação.
        /// </summary>
        /// <param name="operacao">Nome da operação.</param>
        /// <param name="parametros">Parâmetros da operação.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<object> Executar(string operacao, object[] parametros)
        {
            Operacao definicao;
            if (!this.operacoes.TryGetValue(operacao, out definicao))
            {
                throw new InvalidOperationException($"Não foi encontrada a operação \"{operacao}\"");
            }

            object resultado = null;

            try
            {
                resultado = definicao.Chamada.DynamicInvoke(parametros);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            var task = resultado as Task;
            if (task != null)
            {
                await task;

                if (task.GetType().IsGenericType)
                {
                    var taskType = typeof(Task<>).MakeGenericType(task.GetType().GetGenericParameterConstraints().First());
                    var property = taskType.GetProperty(nameof(Task<object>.Result));

                    return property.GetValue(task);
                }

                return null;
            }
            else
            {
                return resultado;
            }
        }

        /// <summary>
        /// Representa o operação.
        /// </summary>
        private class Operacao
        {
            public Operacao(OperacaoIntegracao definicao, Delegate chamada)
            {
                this.Definicao = definicao;
                this.Chamada = chamada;
            }

            /// <summary>
            /// Obtém a definição da operação.
            /// </summary>
            public OperacaoIntegracao Definicao { get; }

            /// <summary>
            /// Obtém a chamada da operação.
            /// </summary>
            public Delegate Chamada { get; }
        }
    }
}
