using System;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class OtimizacaoBancoDAO : BaseDAO<OtimizacaoBanco, OtimizacaoBancoDAO>
    {
        //private OtimizacaoBancoDAO() { }

        public bool Otimizou(DateTime data)
        {
            string sql = "select count(*) from otimizacao_banco where data=?data";
            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?data", data.Date)) > 0;
        }

        public void Otimizado()
        {
            OtimizacaoBanco novo = new OtimizacaoBanco();
            novo.Data = DateTime.Now.Date;
            Insert(novo);
        }

        public string[] GetTabelasBanco()
        {
            string sql = @"select table_name as nomeTabela
                from information_schema.tables where table_schema='" + DBUtils.GetDBName + "'";

            return ExecuteMultipleScalar<string>(sql).ToArray();
        }

        public void OtimizaBanco()
        {
            string sqlBase = "repair table {0}; optimize table {0}; flush table {0}; ";
            StringBuilder sql = new StringBuilder();

            int numeroTabelas = 0;
            foreach (string tabela in GetTabelasBanco())
            {
                sql.AppendFormat(sqlBase, tabela);
                if (++numeroTabelas == 5)
                {
                    objPersistence.ExecuteCommand(sql.ToString());
                    numeroTabelas = 0;
                    sql.Length = 0;
                }
            }

            if (numeroTabelas > 0)
                objPersistence.ExecuteCommand(sql.ToString());
        }
    }
}
