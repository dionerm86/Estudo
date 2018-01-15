using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class ControleCreditoEfdDAO : BaseDAO<ControleCreditoEfd, ControleCreditoEfdDAO>
    {
        //private ControleCreditoEfdDAO() { }

        private string Sql(uint idLoja, int tipoImposto, string periodoAtual, int? codCred, bool apenasCreditoPositivo, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");
            
            sql.Append(selecionar || apenasCreditoPositivo ? @"cc.*, l.nomeFantasia as DescrLoja, coalesce(sum(uc.valorUsado), 0) as valorUsadoCredito" : 
                "count(distinct cc.idCredito)");

            sql.Append(@"
                from controle_credito_efd cc
                    LEFT JOIN loja l ON (cc.IdLoja = l.IdLoja)
                    LEFT JOIN
                    (
                	    SELECT IdCredito, valorUsado
                        FROM uso_credito_efd
                        WHERE 1 {0}
                    
                    ) uc on (cc.IdCredito = uc.IdCredito)
                where 1");

            if (!String.IsNullOrEmpty(periodoAtual))
            {
                sql.Append(@" and str_to_date(concat('01/', cc.periodoGeracao), '%d/%m/%Y') < ?periodoAtual");
                sql = new StringBuilder(string.Format(sql.ToString(), " AND str_to_date(concat('01/', periodoUso), '%d/%m/%Y') < ?periodoAtual"));
            }
            else
                sql.AppendFormat(sql.ToString(), "");

            if (idLoja > 0)
                sql.Append(" AND cc.IdLoja = " + idLoja);

            if (tipoImposto > 0)
                sql.AppendFormat(" and cc.tipoImposto={0}", tipoImposto);

            if (codCred > 0)
                sql.AppendFormat(" and cc.codCred={0}", codCred);

            if (selecionar || apenasCreditoPositivo)
            {
                sql.Append(@"
                    group by cc.idCredito");
            }

            if (apenasCreditoPositivo)
                sql.Append(" having cc.valorGerado-valorUsadoCredito>0");

            return selecionar || !apenasCreditoPositivo ? sql.ToString() :
                "select count(*) from (" + sql.ToString() + ") as temp";
        }

        private GDAParameter[] GetParams(string periodo, bool atual)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(periodo))
                lst.Add(new GDAParameter("?periodoAtual", DateTime.Parse("01/" + periodo).AddMonths(atual ? 0 : 1)));

            return lst.ToArray();
        }

        public IList<ControleCreditoEfd> GetList(uint idLoja, DateTime inicio, DataSourcesEFD.TipoImpostoEnum tipoImposto, DataSourcesEFD.CodCredEnum? codCred,
            bool apenasCreditoPositivo, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "str_to_date(concat('01/', cc.periodoGeracao), '%d/%m/%Y') asc, cc.codCred asc";

            string periodo = inicio.ToString("MM/yyyy");
            return LoadDataWithSortExpression(Sql(idLoja, (int)tipoImposto, periodo, (int?)codCred, apenasCreditoPositivo, true), 
                sortExpression, startRow, pageSize, true, GetParams(periodo, false));
        }

        public int GetCount(uint idLoja, DateTime inicio, DataSourcesEFD.TipoImpostoEnum tipoImposto, DataSourcesEFD.CodCredEnum? codCred, bool apenasCreditoPositivo)
        {
            string periodo = inicio.ToString("MM/yyyy");
            return GetCountWithInfoPaging(Sql(idLoja, (int)tipoImposto, periodo, (int?)codCred, apenasCreditoPositivo, true), true, GetParams(periodo, false));
        }

        public KeyValuePair<decimal, IEnumerable<Sync.Fiscal.EFD.Entidade.IControleCreditoEFD>> GetForEFD(uint idLoja, DateTime inicio, DataSourcesEFD.TipoImpostoEnum tipoImposto, DataSourcesEFD.CodCredEnum? codCred)
        {
            string periodo = inicio.ToString("MM/yyyy");

            var sql = Sql(idLoja, (int)tipoImposto, periodo, (int?)codCred, true, true) +
                " order by str_to_date(concat('01/', cc.periodoGeracao), '%d/%m/%Y') asc, cc.codCred asc";

            var itens = objPersistence.LoadData(sql, GetParams(periodo, true)).ToList().ToArray();

            decimal soma = 0;
            foreach (ControleCreditoEfd c in itens)
                soma += c.ValorRestanteCredito;

            return new KeyValuePair<decimal, IEnumerable<Sync.Fiscal.EFD.Entidade.IControleCreditoEFD>>(soma, itens);
        }

        public uint? ObterCodigoItem(uint idLoja, string periodo, int? codCred, int tipoImposto)
        {
            return ObtemValorCampo<uint?>("idCredito", "periodoGeracao=?periodo AND idLoja=" + idLoja +
                (codCred != null ? " and codCred=" + codCred : "") + " and tipoImposto=" + tipoImposto,
                new GDAParameter("?periodo", periodo));
        }

        public void InserirCredito(uint idLoja, DateTime inicio, DataSourcesEFD.TipoImpostoEnum tipoImposto, DataSourcesEFD.CodCredEnum? codCred, decimal valor)
        {
            string periodo = inicio.ToString("MM/yyyy");

            uint? idCreditoAtual = ObterCodigoItem(idLoja, periodo, (int?)codCred, (int)tipoImposto);

            if (idCreditoAtual > 0)
            {
                decimal totalUsado = UsoCreditoEfdDAO.Instance.ObtemValorCampo<decimal>("sum(valorUsado)", 
                    "idCredito=" + idCreditoAtual.Value);

                if (totalUsado > valor)
                    throw new Exception(string.Format("O crédito já usado no período ({0}) é maior que o crédito que está sendo gerado ({1}).", totalUsado.ToString("C"), valor.ToString("C")));

                DeleteByPrimaryKey(idCreditoAtual.Value);
            }

            ControleCreditoEfd novo = new ControleCreditoEfd();
            novo.PeriodoGeracao = periodo;
            novo.TipoImposto = (int)tipoImposto;
            novo.CodCred = (int?)codCred;
            novo.ValorGerado = valor;
            novo.IdLoja = idLoja;

            uint idNovoCredito = Insert(novo);

            if (idCreditoAtual > 0)
                UsoCreditoEfdDAO.Instance.AtualizaCreditoUsado(idCreditoAtual.Value, idNovoCredito);
        }

        public override uint Insert(ControleCreditoEfd objInsert)
        {
            uint? idCreditoAtual = ObterCodigoItem(objInsert.IdLoja, objInsert.PeriodoGeracao, (int?)objInsert.CodCred, 
                (int)objInsert.TipoImposto);
                
            if (idCreditoAtual > 0)
                throw new Exception("Já há um crédito gerado para esse período" + 
                    (objInsert.CodCred != null ? ", código de crédito" : "") + 
                    " e tipo de imposto. Só é possível que exista um registro desse tipo.");

            return base.Insert(objInsert);
        }

        public override int Update(ControleCreditoEfd objUpdate)
        {
            uint? idCreditoAtual = ObterCodigoItem(objUpdate.IdLoja, objUpdate.PeriodoGeracao, (int?)objUpdate.CodCred,
                (int)objUpdate.TipoImposto);

            if (idCreditoAtual > 0)
                throw new Exception("Já há um crédito gerado para esse período" +
                    (objUpdate.CodCred != null ? ", código de crédito" : "") +
                    " e tipo de imposto. Só é possível que exista um registro desse tipo.");

            LogAlteracaoDAO.Instance.LogControleCreditosEfd(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(ControleCreditoEfd objDelete)
        {
            LogCancelamentoDAO.Instance.LogControleCreditos(objDelete, "Cancelamento manual", true);
            return base.Delete(objDelete);
        }
    }
}
