using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoFornecedorDAO : BaseDAO<ProdutoFornecedor, ProdutoFornecedorDAO>
    {
        //private ProdutoFornecedorDAO() { }

        private string Sql(uint idProdFornec, uint idFornec, uint idProd, bool? exibirSemDataVigencia, bool selecionar,
            bool descricao, string codInterno, string descricaoProd)
        {
            return Sql(null, idProdFornec, idFornec, idProd, exibirSemDataVigencia, selecionar, descricao, codInterno, descricaoProd);
        }

        private string Sql(GDASession session, uint idProdFornec, uint idFornec, uint idProd, bool? exibirSemDataVigencia, bool selecionar, bool descricao,
            string codInterno, string descricaoProd)
        {
            string campos = descricao ? "concat(" + FornecedorDAO.Instance.GetNomeFornecedor("f") + ", '/', p.descricao)" :
                selecionar ? "pf.*, " + FornecedorDAO.Instance.GetNomeFornecedor("f") + @" as nomeFornec, 
                p.descricao as descrProduto, '$$$' as criterio" : "count(*)";

            string criterio = String.Empty;
            string sql = @"
                select " + campos + @"
                from produto_fornecedor pf
                    inner join fornecedor f on (pf.idFornec=f.idFornec)
                    inner join produto p on (pf.idProd=p.idProd)
                where 1";

            if (idProdFornec > 0)
                sql += " and pf.idProdFornec=" + idProdFornec;

            if (idFornec > 0)
            {
                sql += " and pf.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(session, idFornec) + "    ";
            }

            if (idProd > 0)
            {
                sql += " and pf.idProd=" + idProd;
                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(session, (int)idProd) + "    ";
            }

            if (exibirSemDataVigencia != null)
            {
                if (!exibirSemDataVigencia.Value)
                {
                    sql += " and pf.dataVigencia is not null";
                    criterio += "Com data de vigência    ";
                }
                else
                {
                    sql += " and pf.dataVigencia is null";
                    criterio += "Sem data de vigência    ";
                }
            }

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " And p.codInterno like ?codInterno";
                criterio += "Produto: " + codInterno + (!String.IsNullOrEmpty(descricaoProd) ? " - " + descricaoProd : "") + "   ";
            }
            else if (!String.IsNullOrEmpty(descricaoProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(session, null, descricaoProd);
                sql += " and p.idProd In (" + ids + ")";
                criterio += "Produto: " + descricaoProd + "   ";
            }

            return sql.Replace("$$$", criterio);
        }

        public ProdutoFornecedor GetElement(uint idProdFornec)
        {
            List<ProdutoFornecedor> p = objPersistence.LoadData(Sql(idProdFornec, 0, 0, null, true, false, null, null));
            return p.Count > 0 ? p[0] : null;
        }

        public string ObtemDescricao(uint idProdFornec)
        {
            return ObtemDescricao(null, idProdFornec);
        }

        public string ObtemDescricao(GDASession session, uint idProdFornec)
        {
            string sql = Sql(session, idProdFornec, 0, 0, null, true, true, null, null);
            return ExecuteScalar<string>(session, sql);
        }

        public IList<ProdutoFornecedor> GetList(uint idFornec, uint idProd, bool exibirSemDataVigencia, string codProd, string descrProd,
            string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idFornec, idProd, exibirSemDataVigencia, codProd, descrProd) == 0)
                return new ProdutoFornecedor[] { new ProdutoFornecedor() };

            return LoadDataWithSortExpression(Sql(0, idFornec, idProd, exibirSemDataVigencia, true, false, codProd, descrProd),
                sortExpression, startRow, pageSize, GetParam(codProd));
        }

        public int GetCount(uint idFornec, uint idProd, bool exibirSemDataVigencia, string codProd, string descrProd)
        {
            int retorno = GetCountReal(idFornec, idProd, exibirSemDataVigencia, codProd, descrProd);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idFornec, uint idProd, bool exibirSemDataVigencia, string codProd, string descrProd)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idFornec, idProd, exibirSemDataVigencia, false, false, codProd, descrProd),
                GetParam(codProd));
        }

        private GDAParameter[] GetParam(string codInterno)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", codInterno));

            return lstParam.ToArray();
        }

        public IList<ProdutoFornecedor> ObtemParaRelatorio(uint idProd, uint idFornec, string codProd, string descrProd)
        {
            return objPersistence.LoadData(Sql(0, idFornec, idProd, null, true, false, codProd, descrProd), GetParam(codProd)).ToList();
        }

        /// <summary>
        /// Retorna o preço usado pelo fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal GetCustoCompra(int idFornec, int idProd)
        {
            string sql = @"select custoCompra from produto_fornecedor 
                where idFornec=" + idFornec + " and idProd=" + idProd + @"
                    and date(coalesce(dataVigencia, now()))>=date(now())
                order by dataVigencia is null asc, dataVigencia asc
                limit 1";

            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Obtém o ID do Produto pelo ID do Fornecedor e Código Interno do Fornecedor
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="codFornec"></param>
        /// <returns></returns>
        public uint GetIdProdByIdFornecCodFornec(uint idFornec, string codFornec)
        {
            return ObtemValorCampo<uint>("idProd", "idFornec=" + idFornec + " and codFornec=?codFornec", new GDAParameter("?codFornec", codFornec));
        }

        /// <summary>
        /// Desassocia um produto de fornecedor
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="codFornec"></param>
        public void DesassociaProduto(uint idFornec, string codFornec)
        {
            objPersistence.ExecuteCommand("Delete from produto_fornecedor Where idFornec=" + idFornec + " And codFornec=?codFornec", 
                new GDAParameter("?codFornec", codFornec));
        }

        public override int Update(ProdutoFornecedor objUpdate)
        {
            LogAlteracaoDAO.Instance.LogProdutoFornecedor(objUpdate);
            return base.Update(objUpdate);
        }
    }
}
