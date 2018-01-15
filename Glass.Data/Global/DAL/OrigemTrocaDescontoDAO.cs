using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class OrigemTrocaDescontoDAO : BaseCadastroDAO<OrigemTrocaDesconto,OrigemTrocaDescontoDAO>
    {
        #region Retorno de Itens

        private string Sql(int situacao, bool selecionar)
        {
            string campos = selecionar ? "otc.*" : "COUNT(*)";
            var sql = @"
                SELECT " + campos + @"
                FROM origem_troca_desconto otc
                WHERE 1";

            if (situacao > 0)
                sql += " AND situacao=" + situacao;

            return sql;
        }

        public OrigemTrocaDesconto[] GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetListCountReal() == 0)
            {
                var lst = new List<OrigemTrocaDesconto>();
                lst.Add(new OrigemTrocaDesconto());
                return lst.ToArray();
            }

            var sql = Sql(0, true);

            if(string.IsNullOrEmpty(sortExpression))
                sql+=" ORDER BY descricao";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize).ToArray();
        }

        public int GetListCount()
        {
            var count = GetListCountReal();

            return count > 0 ? count : 1;
        }

        public int GetListCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false));
        }

        public OrigemTrocaDesconto[] GetList()
        {
            var sql = Sql((int)Situacao.Ativo, true) + " ORDER BY descricao";

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Obtem dados da Origem

        public string ObtemDescricao(uint idOrigemTrocaDesconto)
        {
            return ObtemDescricao(null, idOrigemTrocaDesconto);
        }

        public string ObtemDescricao(GDA.GDASession session, uint idOrigemTrocaDesconto)
        {
            return ObtemValorCampo<string>(session, "Descricao", "idOrigemTrocaDesconto=" + idOrigemTrocaDesconto);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(OrigemTrocaDesconto objInsert)
        {
            objInsert.Situacao = Situacao.Ativo;
            return base.Insert(objInsert);
        }

        #endregion
    }
}
