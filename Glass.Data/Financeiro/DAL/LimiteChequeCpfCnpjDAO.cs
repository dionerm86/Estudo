using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class LimiteChequeCpfCnpjDAO : BaseDAO<LimiteChequeCpfCnpj, LimiteChequeCpfCnpjDAO>
    {
        //private LimiteChequeCpfCnpjDAO() { }

        private string Sql(string cpfCnpj, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "distinct lcc.idLimiteCheque, c.cpfCnpj, lcc.limite, lcc.Observacao" : 
                "count(distinct c.cpfCnpj)");

            sql.AppendFormat(@"
                from cheques c
                    left join limite_cheque_cpfcnpj lcc on (c.cpfCnpj=lcc.cpfCnpj)
                where 1{0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder(" and c.cpfCnpj is not null");

            if (!String.IsNullOrEmpty(cpfCnpj))
                fa.Append(" and c.cpfCnpj=?cpfCnpj");

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParams(string cpfCnpj)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(cpfCnpj))
                lst.Add(new GDAParameter("?cpfCnpj", cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "")));

            return lst.ToArray();
        }

        public IList<LimiteChequeCpfCnpj> ObtemItens(string cpfCnpj, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "c.cpfCnpj asc";

            string filtroAdicional;
            return LoadDataWithSortExpression(Sql(cpfCnpj, true, out filtroAdicional), sortExpression, startRow, 
                pageSize, true, filtroAdicional, GetParams(cpfCnpj));
        }

        public int ObtemNumeroItens(string cpfCnpj)
        {
            string filtroAdicional;
            return GetCountWithInfoPaging(Sql(cpfCnpj, true, out filtroAdicional), true, filtroAdicional, GetParams(cpfCnpj));
        }

        public decimal ObtemLimite(string cpfCnpj)
        {
            return ObtemLimite(null, cpfCnpj);
        }

        public decimal ObtemLimite(GDASession session, string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return 0;

            cpfCnpj = cpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            return ObtemValorCampo<decimal>(session, "limite", "cpfCnpj=?c", new GDAParameter("?c", cpfCnpj));
        }

        private decimal ObtemValorChequesAbertos(string cpfCnpj)
        {
            return ObtemValorChequesAbertos((GDASession)null, cpfCnpj);
        }

        private decimal ObtemValorChequesAbertos(GDASession session, string cpfCnpj)
        {
            return ObtemValorChequesAbertos(session, cpfCnpj, null);
        }

        public decimal ObtemValorChequesAbertos(string cpfCnpj, uint? idCliente)
        {
            return ObtemValorChequesAbertos((GDASession)null, cpfCnpj, idCliente);
        }

        public decimal ObtemValorChequesAbertos(GDASession session, string cpfCnpj, uint? idCliente)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return 0;

            string sql = @"select sum(valor-coalesce(valorReceb,0)) from cheques where cpfCnpj=?c
                and situacao in (" + (int)Cheques.SituacaoCheque.EmAberto + "," + (int)Cheques.SituacaoCheque.Devolvido + "," +
                (int)Cheques.SituacaoCheque.Protestado + "," + (int)Cheques.SituacaoCheque.Trocado + ")";

            if (idCliente > 0)
                sql += " and idCliente=" + idCliente;

            cpfCnpj = cpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            return ExecuteScalar<decimal>(session, sql, new GDAParameter("?c", cpfCnpj));
        }

        public decimal ObtemValorRestanteLimite(string cpfCnpj, out bool validar)
        {
            return ObtemValorRestanteLimite(null, cpfCnpj, out validar);
        }

        public decimal ObtemValorRestanteLimite(GDASession session, string cpfCnpj, out bool validar)
        {
            decimal limite = ObtemLimite(session, cpfCnpj);
            validar = limite > 0;
            return limite - ObtemValorChequesAbertos(session, cpfCnpj);
        }

        public decimal ObtemValorRestanteLimiteCliente(string cpfCnpj, uint idCliente, out bool validar)
        {
            return ObtemValorRestanteLimiteCliente((GDASession)null, cpfCnpj, idCliente, out validar);
        }

        public decimal ObtemValorRestanteLimiteCliente(GDASession session, string cpfCnpj, uint idCliente, out bool validar)
        {
            decimal limite = ClienteDAO.Instance.ObtemLimiteCheques(session, idCliente);
            validar = limite > 0;
            return limite - ObtemValorChequesAbertos(session, cpfCnpj, idCliente);
        }

        public override void InsertOrUpdate(LimiteChequeCpfCnpj objUpdate)
        {
            objUpdate.Observacao =
                !string.IsNullOrEmpty(objUpdate.Observacao) && objUpdate.Observacao.Length > 300 ?
                    objUpdate.Observacao.Substring(0, 300) : objUpdate.Observacao;

            if (objUpdate.IdLimiteCheque == 0)
            {
                decimal limite = objUpdate.Limite;
                objUpdate.Limite = 0;
                objUpdate.IdLimiteCheque = base.Insert(objUpdate);
                objUpdate.Limite = limite;
            }

            LogAlteracaoDAO.Instance.LogLimiteChequeCpfCnpj(objUpdate);
            Update(objUpdate);
        }

        public string[] ObtemCpfCnpj()
        {
            string filtroAdicional, sql = "select distinct cpfCnpj from (" + Sql(null, true, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional) + ") as temp";

            return ExecuteMultipleScalar<string>(sql).ToArray();
        }
    }
}
