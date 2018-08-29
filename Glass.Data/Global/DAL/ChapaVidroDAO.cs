using System;
using System.Linq;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ChapaVidroDAO : BaseDAO<ChapaVidro, ChapaVidroDAO>
	{
        //private ChapaVidroDAO() { }

        #region Listagem de chapas

        private string Sql(uint idProd, string codInterno, string produto, uint idSubgrupo, bool ativas, bool selecionar,
            out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "c.*, p.Descricao as DescrProd, p.CodInterno as CodInternoProd, '$$$' as criterio" : "Count(*)";
            string criterio = String.Empty;

            string sql = "select " + campos + @"
                from chapa_vidro c 
                left join produto p on (c.idProd=p.idProd) where 1";

            if (idProd > 0)
            {
                sql += " and p.IdProd=" + idProd;
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " and p.CodInterno=?codInterno";
                criterio += "Código: " + codInterno + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(produto))
            {
                sql += " and p.Descricao like ?produto";
                criterio += "Produto: " + produto + "    ";
                temFiltro = true;
            }

            if (idSubgrupo > 0)
            {
                sql += " and p.IdSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
                temFiltro = true;
            }

            if (ativas)
                sql += " and c.Situacao=" + (int)Situacao.Ativo;

            return sql.Replace("$$$", criterio);
        }

        public IList<ChapaVidro> GetForRpt(string codInterno, string produto, uint idSubgrupo)
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(0, codInterno, produto, idSubgrupo, false, true, out temFiltro), GetParam(codInterno, produto)).ToList();
        }

        public IList<ChapaVidro> GetList(string codInterno, string produto, uint idSubgrupo, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            return LoadDataWithSortExpression(Sql(0, codInterno, produto, idSubgrupo, false, true, out temFiltro), 
                sortExpression, startRow, pageSize, temFiltro, GetParam(codInterno, produto));
        }

        public int GetListCount(string codInterno, string produto, uint idSubgrupo)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(0, codInterno, produto, idSubgrupo, false, true, out temFiltro),
                temFiltro, null, GetParam(codInterno, produto));
        }

        public GDAParameter[] GetParam(string codInterno, string produto)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", codInterno));

            if (!String.IsNullOrEmpty(produto))
                lstParam.Add(new GDAParameter("?produto", "%" + produto + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        /// <summary>
        /// Verifica se um produto já possui uma chapa de vidro associada.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool IsProdChapa(string codInterno, string codIgnorar)
        {
            string ignorar = !String.IsNullOrEmpty(codIgnorar) ? " and p.idProd<>'" + codIgnorar + "'" : String.Empty;
            string sql = @"select Count(*) from chapa_vidro c, produto p 
                where c.idProd=p.idProd and p.codInterno=?codInterno" + ignorar;

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?codInterno", codInterno)) > 0;
        }

        /// <summary>
        /// Retorna uma chapa de vidro pelo id do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ChapaVidro GetElement(uint idProd)
        {
            return GetElement(null, idProd);
        }

        /// <summary>
        /// Retorna uma chapa de vidro pelo id do produto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public ChapaVidro GetElement(GDASession sessao, uint idProd)
        {
            bool temFiltro;
            var chapa = objPersistence.LoadData(sessao, Sql(idProd, null, null, 0, true, true, out temFiltro)).ToList();
            return chapa.Count > 0 ? chapa[0] : null;
        }

        public IList<ChapaVidro> ObterListaChapasOtimizacao(string idsPedido, string idsOrcamento)
        {
            bool temFiltro;

            // Busca as chapas de vidro pelo produto do Pedido
            string sql = Sql(0, null, null, 0, true, true, out temFiltro);
            sql += @" and c.IdProd in(select coalesce(pbe.IdProdBaixa, pp.idProd) from produtos_pedido pp
                    left join produto_baixa_estoque pbe on(pp.IDPROD=pbe.IDPROD)
                     where pp.idPedido in(" + (!string.IsNullOrEmpty(idsPedido) ? idsPedido : "0") + "))";

            // Busca as chapas de vidro pelo produto do Orcamento
            sql += " union ";
            sql += Sql(0, null, null, 0, true, true,  out temFiltro);
            sql += @" and c.IdProd in(SELECT coalesce(pbe.IdProdBaixa, po.idProduto) from produtos_orcamento po
                     LEFT JOIN produto_baixa_estoque pbe ON(po.idProduto=pbe.IDPROD)
                     where po.idOrcamento in(" + (!string.IsNullOrEmpty(idsOrcamento) ? idsOrcamento : "0") +
                   ") and po.Altura > 0 and po.Largura > 0 and po.IdProduto > 0)";

            // Busca as chapas de vidro pelo produto do Projeto
            sql += " UNION ";
            sql += Sql(0, null, null, 0, true, true, out temFiltro);
            sql += @" AND c.IdProd IN (SELECT coalesce(pbe.IdProdBaixa, mip.IdProd) FROM material_item_projeto mip
					    LEFT JOIN item_projeto ip ON (ip.IdItemProjeto=mip.IdItemProjeto)
					    LEFT JOIN produto_baixa_estoque pbe ON(mip.IdProd=pbe.IDPROD)
					    WHERE ip.idOrcamento IN (" + (!string.IsNullOrEmpty(idsOrcamento) ? idsOrcamento : "0") +
                        ") AND mip.Altura > 0 AND mip.Largura > 0 AND mip.IdProd > 0)";

            var chapas = objPersistence.LoadData(sql).ToList();

            foreach (var chapa in chapas)
                chapa.DescrProduto = chapa.DescrProduto.Replace("(", "")
                    .Replace(")", "")
                    .Replace("+", "")
                    .Replace("-", "");

            return chapas;
        }

        public List<ChapaVidro> ObterListaOtimizacao(string ids)
        {
            string sql = @"select c.*, p.Descricao as DescrProd, p.CodInterno as CodInternoProd from chapa_vidro c 
                left join produto p on (c.idProd=p.idProd) where c.IdChapaVidro IN(" + ids + ") AND c.Situacao=" + (int)Glass.Situacao.Ativo;

            return objPersistence.LoadData(sql);
        }

        /// <summary>
        /// Altera a situação de um cliente, ativando ou inativando o cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void AlteraSituacao(uint idChapa)
        {
            var chapaVidro = GetElementByPrimaryKey(idChapa);
            chapaVidro.Situacao = chapaVidro.Situacao == Situacao.Ativo ? Situacao.Inativo : Situacao.Ativo;
            Update(chapaVidro);
        }

        #region Métodos sobescritos

        public override uint Insert(ChapaVidro objInsert)
        {
            objInsert.Situacao = Situacao.Ativo;
            // Verifica se já foi cadastrada uma chapa de vidro para este produto
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From chapa_vidro Where idProd=" + objInsert.IdProd) > 0)
                throw new Exception("Já existe uma chapa de vidro cadastrada para este produto.");

            /* Chamado 48889. */
            if (objInsert.Quantidade > 100)
                throw new Exception("A quantidade máxima são 100 chapas.");

            return base.Insert(objInsert);
        }

        public override int Update(ChapaVidro objUpdate)
        {
            // Verifica se já foi cadastrada uma chapa de vidro para este produto
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From chapa_vidro Where idProd=" + objUpdate.IdProd) > 1)
                throw new Exception("Já existe uma chapa de vidro cadastrada para este produto.");

            /* Chamado 48889. */
            if (objUpdate.Quantidade > 100)
                throw new Exception("A quantidade máxima são 100 chapas.");

            LogAlteracaoDAO.Instance.LogChapaVidro(objUpdate);
            return base.Update(objUpdate);
        }

        #endregion
    }
}