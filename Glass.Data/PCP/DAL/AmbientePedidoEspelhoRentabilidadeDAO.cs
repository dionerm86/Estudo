﻿using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa o objeto de acesso a dados da rentabilidade do ambiente do pedido.
    /// </summary>
    public class AmbientePedidoEspelhoRentabilidadeDAO : BaseDAO<AmbientePedidoEspelhoRentabilidade, AmbientePedidoEspelhoRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pelo ambiente do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idAmbientePedido"></param>
        /// <returns></returns>
        public IList<AmbientePedidoEspelhoRentabilidade> ObterPorAmbiente(GDA.GDASession sessao, uint idAmbientePedido)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM ambiente_pedido_espelho_rentabilidade WHERE IdAmbientePedido=?id",
                new GDA.GDAParameter("?id", idAmbientePedido))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        #endregion
    }
}
