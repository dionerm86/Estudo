// <copyright file="ProvedorHistoricoExtensions.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;

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
        /// <returns>Item gerado.</returns>
        public static Item RegistrarInformativo<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia, string mensagem)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(referencia, TipoItemHistorico.Informativo, mensagem);
            provedor.RegistrarItem(item);
            return item;
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
        /// <returns>Item gerado.</returns>
        public static Item RegistrarFalha<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia, string mensagem, Exception falha)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(referencia, mensagem, falha != null ? new Falha(falha) : null);
            provedor.RegistrarItem(item);
            return item;
        }

        /// <summary>
        /// Registra um item de falha.
        /// </summary>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="identificadores">Identificadores que representam o item.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="falha">Falha associada.</param>
        /// <returns>Item gerado.</returns>
        public static Item RegistrarFalha(this IProvedorHistorico provedor, ItemEsquema itemEsquema, IEnumerable<object> identificadores, string mensagem, Exception falha)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(identificadores, mensagem, falha != null ? new Falha(falha) : null);
            provedor.RegistrarItem(item);
            return item;
        }

        /// <summary>
        /// Notifica que o item foi integrado.
        /// </summary>
        /// <typeparam name="T">Tipo da classe que o item representa.</typeparam>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="referencia">Referência do item.</param>
        /// <returns>Item gerado.</returns>
        public static Item NotificarIntegrado<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(referencia, TipoItemHistorico.Integrado, null);
            provedor.RegistrarItem(item);
            return item;
        }

        /// <summary>
        /// Notifica que o item foi integrado.
        /// </summary>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="identificadores">Identificadores que representam o item.</param>
        /// <returns>Item gerado.</returns>
        public static Item NotificarIntegrado(this IProvedorHistorico provedor, ItemEsquema itemEsquema, IEnumerable<object> identificadores)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(identificadores, TipoItemHistorico.Integrado, null);
            provedor.RegistrarItem(item);
            return item;
        }

        /// <summary>
        /// Notifica que o item está sendo integrado.
        /// </summary>
        /// <typeparam name="T">Tipo da classe que o item representa.</typeparam>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="referencia">Referência do item.</param>
        /// <returns>Item gerado.</returns>
        public static Item NotificarIntegrando<T>(this IProvedorHistorico provedor, ItemEsquema<T> itemEsquema, T referencia)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(referencia, TipoItemHistorico.Integrando, null);
            provedor.RegistrarItem(item);
            return item;
        }

        /// <summary>
        /// Notifica que o item está sendo integrado.
        /// </summary>
        /// <param name="provedor">Provedor do histórico.</param>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="identificadores">Identificadores que representam o item.</param>
        /// <returns>Item gerado.</returns>
        public static Item NotificarIntegrando(this IProvedorHistorico provedor, ItemEsquema itemEsquema, IEnumerable<object> identificadores)
        {
            if (itemEsquema == null)
            {
                throw new ArgumentNullException(nameof(itemEsquema));
            }

            var item = itemEsquema.CriarItemHistorico(identificadores, TipoItemHistorico.Integrando, null);
            provedor.RegistrarItem(item);
            return item;
        }
    }
}
