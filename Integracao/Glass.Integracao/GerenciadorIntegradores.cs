// <copyright file="GerenciadorIntegradores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o gerenciador dos integradores.
    /// </summary>
    public sealed class GerenciadorIntegradores : IDisposable
    {
        private readonly IProvedorIntegradores provedor;
        private readonly List<IIntegrador> integradores = new List<IIntegrador>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="GerenciadorIntegradores"/>.
        /// </summary>
        /// <param name="provedor">Provedor que será usado na inicialização.</param>
        public GerenciadorIntegradores(IProvedorIntegradores provedor)
        {
            if (provedor == null)
            {
                throw new ArgumentNullException(nameof(provedor));
            }

            this.provedor = provedor;
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="GerenciadorIntegradores"/>.
        /// </summary>
        ~GerenciadorIntegradores()
        {
            this.Dispose();
        }

        /// <summary>
        /// Obtém os integradores associados.
        /// </summary>
        public IEnumerable<IIntegrador> Integradores => this.integradores;

        /// <summary>
        /// Inicializa os integradores.
        /// </summary>
        /// <param name="logger">Logger que será usado na operação.</param>
        /// <returns>True se a inicialização foi bem sucedida.</returns>
        public async Task<bool> Inicializar(Colosoft.Logging.ILogger logger)
        {
            IEnumerable<IIntegrador> integradoresDisponiveis;

            try
            {
                integradoresDisponiveis = await this.provedor.ObterIntegradoresDisponiveis();
            }
            catch (Exception ex)
            {
                logger.Error("Não foi possível carrega os integradores disponíveis".GetFormatter(), ex);
                return false;
            }

            var inicializacaoSucedida = true;

            foreach (var integrador in integradoresDisponiveis)
            {
                if (integrador.Ativo)
                {
                    try
                    {
                        await integrador.Setup();
                        this.integradores.Add(integrador);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Ocorreu um erro no setup do integrador {integrador.Nome}".GetFormatter(), ex);
                        inicializacaoSucedida = false;
                        integrador.Dispose();
                    }
                }
            }

            return inicializacaoSucedida;
        }

        /// <summary>
        /// Executa a operação do integrador.
        /// </summary>
        /// <param name="integrador">Nome do integrador para o qual a operação será executada.</param>
        /// <param name="operacao">Nome da operação de integração.</param>
        /// <param name="parametros">Parâmetros que será usados na execução.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<object> ExecutarOperacao(string integrador, string operacao, object[] parametros)
        {
            if (string.IsNullOrEmpty(integrador))
            {
                throw new ArgumentNullException(nameof(integrador));
            }

            if (string.IsNullOrEmpty(operacao))
            {
                throw new ArgumentNullException(nameof(operacao));
            }

            var integrador1 = this.Integradores.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, integrador));
            if (integrador1 == null)
            {
                throw new InvalidOperationException($"O integrador '{integrador}' não foi encontrado");
            }

            var operacao1 = integrador1.Operacoes.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, operacao));
            if (operacao1 == null)
            {
                throw new InvalidOperationException($"A operação '{operacao}' não foi encontrada no integrador '{integrador}'.");
            }

            if ((parametros?.Length ?? 0) != operacao1.Parametros.Count())
            {
                throw new InvalidOperationException("A quantidade de parâmetros informados não é compatível com a operação.");
            }

            var parametros2 = new List<object>();

            if (parametros != null)
            {
                using (var valores = ((IEnumerable<object>)parametros).GetEnumerator())
                using (var descritores = operacao1.Parametros.GetEnumerator())
                {
                    while (valores.MoveNext())
                    {
                        descritores.MoveNext();

                        var valor = valores.Current;
                        var descritor = descritores.Current;

                        if (valor != null)
                        {
                            var tipoValor = valor.GetType();

                            if (tipoValor != descritor.Tipo)
                            {
                                var typeDescriptor = TypeDescriptor.GetConverter(descritor.Tipo);

                                if (!typeDescriptor.CanConvertFrom(tipoValor))
                                {
                                    throw new InvalidOperationException($"Não é possível converte o parâmetro '{descritor.Nome}' do tipo '{tipoValor.Name}' para '{descritor.Tipo.Name}'");
                                }
                                else
                                {
                                    valor = typeDescriptor.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, valor);
                                }
                            }
                        }

                        parametros2.Add(valor);
                    }
                }
            }

            return integrador1.ExecutarOperacao(operacao, parametros2.ToArray());
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            foreach (var integrador in this.integradores)
            {
                integrador.Dispose();
            }

            this.integradores.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
