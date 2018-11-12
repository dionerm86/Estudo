// <copyright file="IProvedorHistorico.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Assinatura do provedor de histórico.
    /// </summary>
    public interface IProvedorHistorico
    {
        /// <summary>
        /// Registra o item no histórico.
        /// </summary>
        /// <param name="item">Item que será registrado.</param>
        void RegistrarItem(Item item);

        /// <summary>
        /// Obtém os itens de histórico associados com o item do esquema de histório.
        /// </summary>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="tipo">Tipo dos itens que serão recuperados.</param>
        /// <param name="identificadores">Identificadores que serão usados para filtrar os itens.</param>
        /// <returns>Itens de histórico.</returns>
        IEnumerable<Item> ObterItens(ItemEsquema itemEsquema, TipoItemHistorico? tipo, IEnumerable<object> identificadores);
    }
}
