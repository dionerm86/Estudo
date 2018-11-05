// <copyright file="ItensHistoricoFiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe com os dados do filtro para a pesquisa dos itens do histórico.
    /// </summary>
    public class ItensHistoricoFiltroDto
    {
        /// <summary>
        /// Obtém ou define o tipo dos itens que serão filtrados.
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores do item que está sendo pesquisado.
        /// </summary>
        public IEnumerable<string> Identificadores { get; set; } = new List<string>();
    }
}
