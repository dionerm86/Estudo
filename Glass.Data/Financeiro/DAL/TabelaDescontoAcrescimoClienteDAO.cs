using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class TabelaDescontoAcrescimoClienteDAO : BaseDAO<TabelaDescontoAcrescimoCliente, TabelaDescontoAcrescimoClienteDAO>
    {
        //private TabelaDescontoAcrescimoClienteDAO() { }

        private string Sql(bool selecionar)
        {
            string campos = selecionar ? "tdac.*" : "count(*)";
            
            string sql = "select " + campos + @"
                from tabela_desconto_acrescimo_cliente tdac";

            return sql;
        }

        public IList<TabelaDescontoAcrescimoCliente> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new TabelaDescontoAcrescimoCliente[] { new TabelaDescontoAcrescimoCliente() };

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "tdac.descricao";
            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize, false);
        }


        public int GetCount()
        {
            int count = GetCountReal();
            return count > 0 ? count : 1;
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        public IList<TabelaDescontoAcrescimoCliente> GetSorted()
        {
            return objPersistence.LoadData(Sql(true)).ToList();
        }

        public string GetDescricao(uint idTabelaDesconto)
        {
            return ObtemValorCampo<string>("descricao", "idTabelaDesconto=" + idTabelaDesconto);
        }

        public IList<TabelaDescontoAcrescimoCliente> GetForFilter()
        {
            
            string sql = "SELECT * FROM tabela_desconto_acrescimo_cliente";

            List<TabelaDescontoAcrescimoCliente> lst = objPersistence.LoadData(sql);

            return lst.ToArray();
        }

        public override uint Insert(TabelaDescontoAcrescimoCliente objInsert)
        {
            // Verifica se já existe uma tabela com o nome dado à esta
            if (ExecuteScalar<bool>("Select Count(*) > 0 From tabela_desconto_acrescimo_cliente Where descricao=?descricao",
                new GDAParameter("?descricao", objInsert.Descricao)))
                throw new Exception("Já existe uma tabela de desconto/acréscimo com esta descrição.");

            return base.Insert(objInsert);
        }

        public override int Delete(TabelaDescontoAcrescimoCliente objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdTabelaDesconto);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            string sql = "select count(*) from cliente where idTabelaDesconto=" + Key;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há clientes associados à esta tabela de desconto.");
            
            return base.DeleteByPrimaryKey(Key);
        }
    }
}
