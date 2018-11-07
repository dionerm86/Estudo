// <copyright file="IIntegrador.cs" company="Sync Softwares">
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
    /// Assinatura do integrador de um integrador do sistema.
    /// </summary>
    public interface IIntegrador : IDisposable
    {
        /// <summary>
        /// Obtém o nome do integrador.
        /// </summary>
        string Nome { get; }

        /// <summary>
        /// Obtém a configuração do integrador.
        /// </summary>
        ConfiguracaoIntegrador Configuracao { get; }

        /// <summary>
        /// Obtém um valor que indica se o integrador está ativo.
        /// </summary>
        bool Ativo { get; }

        /// <summary>
        /// Obtém as operações de integração.
        /// </summary>
        IEnumerable<OperacaoIntegracao> Operacoes { get; }

        /// <summary>
        /// Obtém os Jobs do integrador.
        /// </summary>
        IEnumerable<IJobIntegracao> Jobs { get; }

        /// <summary>
        /// Obtém o esquema do histório associado com o integrador.
        /// </summary>
        Historico.Esquema EsquemaHistorico { get; }

        /// <summary>
        /// Obtém o logger do integrador.
        /// </summary>
        LoggerIntegracao Logger { get; }

        /// <summary>
        /// Executa a operação de integração informada.
        /// </summary>
        /// <param name="operacao">Nome da operação que será executada.</param>
        /// <param name="parametros">Parâmetros da operação.</param>
        /// <returns>Resultado da operação.</returns>
        Task<object> ExecutarOperacao(string operacao, object[] parametros);

        /// <summary>
        /// Realiza o setup do integrador.
        /// </summary>
        /// <returns>Tarefa.</returns>
        Task Setup();
    }
}
