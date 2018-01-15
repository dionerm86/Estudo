using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Pedido
{
    /// <summary>
    /// Representa a descrição do pedido.
    /// </summary>
    public class PedidoDescritor : Glass.Api.Pedido.IPedidoDescritor
    {
        #region Properties

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public int Pedido { get; set; }

        /// <summary>
        /// Identificador do projeto.
        /// </summary>
        public int IdProjeto { get; set; }        

        /// <summary>
        /// Código do pedido do cliente.
        /// </summary>
        public string PedidoCli { get; set; }

        /// <summary>
        /// Situação produção.
        /// </summary>
        public string SituacaoProd { get; set; }

        /// <summary>
        /// Valor total do pedido.
        /// </summary>
        public string Total { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="pedido"></param>
        public PedidoDescritor(Glass.Data.Model.Pedido pedido)
        {
            Pedido = (int)pedido.IdPedido;
            PedidoCli = pedido.CodCliente;
            SituacaoProd = pedido.DescrSituacaoProducao;
            Total = pedido.Total.ToString("c", new System.Globalization.CultureInfo("pt-BR"));
            try
            {
                IdProjeto = (int)pedido.IdProjeto.Value;
            }
            catch { }
        }

        #endregion
    }
}
