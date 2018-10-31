// <copyright file="ProvedorHistoricoExtensions.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Classe com os métodos de extensão para trabalhar com o provedor de histórico.
    /// </summary>
    public static class ProvedorHistoricoExtensions
    {
        /// <summary>
        /// Registra um item informativo.
        /// </summary>
        /// <typeparam name="T">Tipo da classe que o item representa.</typeparam>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="referencia">Referência do item.</param>
        /// <param name="mensagem">Mensagem.</param>
        public static void RegistrarInformativo<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia, string mensagem)
        {
            var item = itemEsquema.CriarItemHistorico(referencia, TipoItemHistorico.Informativo, mensagem);
            provedor.RegistrarItem(item);
        }

        /// <summary>
        /// Registra um item de falha.
        /// </summary>
        /// <typeparam name="T">Tipo da classe que o item representa.</typeparam>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="referencia">Referência do item.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="falha">Falha associada.</param>
        public static void RegistrarFalha<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia, string mensagem, Exception falha)
        {
            var item = itemEsquema.CriarItemHistorico(referencia, mensagem, new Falha(falha));
            provedor.RegistrarItem(item);
        }

        /// <summary>
        /// Notifica a integração do item.
        /// </summary>
        /// <typeparam name="T">Tipo da classe que o item representa.</typeparam>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="referencia">Referência do item.</param>
        public static void NotificarIntegracao<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia)
        {
            var item = itemEsquema.CriarItemHistorico(referencia, TipoItemHistorico.Integrado, null);
            provedor.RegistrarItem(item);
        }
    }
}
