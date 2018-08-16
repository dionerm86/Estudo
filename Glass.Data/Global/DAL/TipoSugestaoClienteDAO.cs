using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class TipoSugestaoClienteDAO : BaseDAO<TipoSugestaoCliente, TipoSugestaoClienteDAO>
    {
        public IList<TipoSugestaoCliente> ObterTiposSugestaoCliente()
        {
            var sql = @"SELECT * from tipo_sugestao_cliente";

            return objPersistence.LoadData(sql).ToList();
        }
    }
}