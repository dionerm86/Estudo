using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoSinalDAO : BaseDAO<PagtoSinal, PagtoSinalDAO>
    {
        //private PagtoSinalDAO() { }

        private string Sql(uint idSinal, uint? idFormaPagto, bool selecionar)
        {
            string campos = selecionar ? "ps.*, If(ps.idFormaPagto!=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Obra + ", fp.descricao, 'Obra') As DescrFormaPagto" : "Count(*)";
            string sql = "Select " + campos + @" From pagto_sinal ps 
                Left Join formapagto fp On (ps.idFormaPagto=fp.idFormaPagto)
                Where ps.idSinal=" + idSinal + (idFormaPagto >= 0 ? " And ps.idFormaPagto=" + idFormaPagto : "") +
                " Order By ps.idFormaPagto Asc";// " Order By ps.NumFormaPagto Asc"; Alterei a ordenação para o idFormaPagto pois desta forma o crédito é o primeiro a ser estornado.

            return sql;
        }

        public IList<PagtoSinal> GetBySinal(uint idSinal)
        {
            return GetBySinal(null, idSinal);
        }

        public IList<PagtoSinal> GetBySinal(GDASession session, uint idSinal)
        {
            return objPersistence.LoadData(session, Sql(idSinal, null, true)).ToList();
        }

        public IList<PagtoSinal> GetByIdFormaPagto(uint idSinal, uint idFormaPagto)
        {
            return objPersistence.LoadData(Sql(idSinal, idFormaPagto, true)).ToList();
        }

        public void DeleteBySinal(uint idSinal)
        {
            objPersistence.ExecuteCommand("Delete From pagto_sinal Where idSinal=" + idSinal);
        }

        public override uint Insert(GDASession session, PagtoSinal objInsert)
        {
            if (objInsert.IdFormaPagto == (uint)Pagto.FormaPagto.Deposito &&
                objInsert.IdContaBanco.GetValueOrDefault(0) <= 0)
                throw new Exception("A forma de pagamento infomada é depósito, porém nenhuma conta foi informada.");

            return base.Insert(session, objInsert);
        }

        public override uint Insert(PagtoSinal objInsert)
        {
            if (objInsert.IdFormaPagto == (uint)Pagto.FormaPagto.Deposito &&
               objInsert.IdContaBanco.GetValueOrDefault(0) <= 0)
                throw new Exception("A forma de pagamento infomada é depósito, porém nenhuma conta foi informada.");

            return base.Insert(objInsert);
        }
    }
}
