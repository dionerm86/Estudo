using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Global
{
    /// <summary>
    /// Representa entidade de negocio para tipo de entrega.
    /// </summary>
    public class TipoEntrega : Glass.Api.Global.ITipoEntrega
    {
        #region Propriedades

        /// <summary>
        /// Identificador.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão para tipo de entrega vazia.
        /// </summary>
        public TipoEntrega()
        {
            
        }

        /// <summary>
        /// Construtor da entidade de tipo de entrega.
        /// </summary>
        /// <param name="modelo"></param>
        public TipoEntrega(Glass.Data.Model.Pedido.TipoEntregaPedido modelo)
        {
            Id = (int)modelo;
            Descricao = modelo.ToString();
        }

        /// <summary>
        /// Construtor da entidade de tipo de entrega.
        /// </summary>
        /// <param name="modelo"></param>
        public TipoEntrega(GenericModel modelo)
        {
            Id = (int)modelo.Id;
            Descricao = modelo.Descr;
        }

        #endregion
    }

    public class TipoEntregaFluxo : Glass.Api.Global.ITipoEntregaFluxo
    {
        /// <summary>
        /// Recupera os tipos de entrega.
        /// </summary>
        /// <returns></returns>
        public IList<Glass.Api.Global.ITipoEntrega> ObterTiposEntrega()
        {
            var retorno = new List<Glass.Api.Global.ITipoEntrega>();

            retorno.AddRange(Data.Helper.DataSources.Instance.GetTipoEntrega().Select(f => new TipoEntrega(f)));

            return retorno;
        }

        /// <summary>
        /// Recupera os tipos de entrega de parceiros.
        /// </summary>
        /// <returns></returns>
        public IList<Glass.Api.Global.ITipoEntrega> ObterTiposEntregaParceiros()
        {
            var retorno = new List<Glass.Api.Global.ITipoEntrega>();

            retorno.AddRange(Glass.Data.Helper.DataSources.Instance.GetTipoEntrega().Select(f => new TipoEntrega(f)));

            return retorno;
        }

        /// <summary>
        /// Recupera o tipo de entrega padrão.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <returns></returns>
        public int ObterTipoEntregaPadrao()
        {
            var rota = Glass.Data.DAL.RotaDAO.Instance.GetByCliente(null, Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente.Value);
            return rota != null && rota.EntregaBalcao ?
                   (int)Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao :
                   (int)Glass.Configuracoes.PedidoConfig.TipoEntregaPadraoPedido;
        }
    }
}
