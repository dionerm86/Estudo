using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glass.Api.Projeto
{
    /// <summary>
    /// Representa a medida de um item de projeto.
    /// </summary>
    public class MedidaItemProjeto : IMedidaItemProjeto
    {
        #region Propriedades

        /// <summary>
        /// Identificador da medida de item.
        /// </summary>
        public Guid IdMedidaItemProjeto { get; set; }

        /// <summary>
        /// Identificador da medida do projeto.
        /// </summary>
        public int IdMedidaProjeto { get; set; }

        /// <summary>
        /// Valor da medida.
        /// </summary>
        public int Valor { get; set; }

        #endregion

        #region IMedidaItemProjeto Members

        /// <summary>
        /// Identificador do medida do projeto.
        /// </summary>
        uint IMedidaItemProjeto.IdMedidaProjeto
        {
            get
            {
                return (uint)IdMedidaProjeto;
            }

            set
            {
                IdMedidaProjeto = (int)value;
            }
        }


        #endregion
    }
}