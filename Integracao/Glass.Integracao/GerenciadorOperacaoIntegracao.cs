// <copyright file="GerenciadorOperacaoIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o gerenciador de operações de integração.
    /// </summary>
    public class GerenciadorOperacaoIntegracao
    {
        private readonly Colosoft.Logging.ILogger logger;
        private readonly Dictionary<string, Operacao> operacoes = new Dictionary<string, Operacao>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="GerenciadorOperacaoIntegracao"/>.
        /// </summary>
        /// <param name="logger">Logger que será usado pela instância.</param>
        public GerenciadorOperacaoIntegracao(Colosoft.Logging.ILogger logger)
        {
            this.logger = logger;
        }

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

            this.logger.Info($"Solicitação de execução da operação '{operacao}'...".GetFormatter());

            object resultado = null;

            try
            {
                resultado = definicao.Chamada.DynamicInvoke(parametros);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                this.logger.Error($"Falha na execução da opereação '{operacao}'.".GetFormatter(), ex.InnerException);
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                this.logger.Error($"Falha na execução da opereação '{operacao}'.".GetFormatter(), ex);
                throw;
            }

            this.logger.Info($"Operação '{operacao}' executada!".GetFormatter());

            var task = resultado as Task;
            if (task != null)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    this.logger.Error($"Falha na execução da opereação '{operacao}'.".GetFormatter(), ex);
                    throw;
                }

                if (task.GetType().IsGenericType)
                {
                    var genericArgument = task.GetType().GetGenericArguments().First();
                    var taskType = typeof(Task<>).MakeGenericType(genericArgument);
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
