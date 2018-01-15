using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Data.Helper;

namespace Glass.Api.Projeto
{
    /// <summary>
    /// Representa o projeto.
    /// </summary>
    public class Projeto : IProjeto
    {
        #region Variáveis Locais

        private List<ItemProjeto> _itens = new List<ItemProjeto>();
        
        #endregion

        #region Propriedades

        /// <summary>
        /// Itens.
        /// </summary>
        public List<ItemProjeto> Itens
        {
            get { return _itens; }
        }

        /// <summary>
        /// Identificador do projeto.
        /// </summary>
        public Guid IdProjeto { get; set; }

        /// <summary>
        /// Identificador do tipo de entrega.
        /// </summary>
        public int IdTipoEntrega { get; set; }

        /// <summary>
        /// Pedido.
        /// </summary>
        public string Pedido { get; set; }

        /// <summary>
        /// Data de cadastro.
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Custo total.
        /// </summary>
        public decimal CustoTotal { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        IEnumerable<IItemProjeto> IProjeto.Itens
        {
            get
            {
                return Itens;
            }

        }

        #endregion
    }
}