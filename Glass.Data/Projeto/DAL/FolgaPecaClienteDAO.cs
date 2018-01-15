using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{

    public sealed class FolgaPecaClienteDAO : BaseDAO<FolgaPecaCliente, FolgaPecaClienteDAO>
    {
        /// <summary>
        /// Retorna a folga
        /// </summary>
        public FolgaPecaCliente GetElement(uint IdPecaProjetoModelo, uint idCliente)
        {
            var sql = string.Format("SELECT * FROM folga_peca_cliente WHERE IdPecaProjetoModelo={0} AND IdCliente={1}", IdPecaProjetoModelo, idCliente);
            return objPersistence.LoadOneData(sql, "");
        }
    }
}
