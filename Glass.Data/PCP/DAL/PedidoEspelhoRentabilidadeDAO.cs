﻿using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa o objeto de acesso a dados da rentabilidade do pedido.
    /// </summary>
    public class PedidoEspelhoRentabilidadeDAO : BaseDAO<PedidoEspelhoRentabilidade, PedidoEspelhoRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pelo pedido informado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<PedidoEspelhoRentabilidade> ObterPorPedido(GDA.GDASession sessao, uint idPedido)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM pedido_espelho_rentabilidade WHERE IdPedido=?id",
                new GDA.GDAParameter("?id", idPedido))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        #endregion
    }
}
