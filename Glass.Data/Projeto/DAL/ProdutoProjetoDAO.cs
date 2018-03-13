using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoProjetoDAO : BaseDAO<ProdutoProjeto, ProdutoProjetoDAO>
    {
        //private ProdutoProjetoDAO() { }

        #region Busca Padrão

        public string Sql(uint idProdProj, string codInterno, string descricao, string codInternoAssoc, string descricaoAssoc, int tipo, bool selecionar)
        {
            string campos = selecionar ? "pp.*, p.descricao as ProdutoAssociado" : "Count(*)";

            string sql = "Select " + campos + @" From produto_projeto pp
                    Left Join produto p On (pp.idProd=p.idProd)
                Where 1";

            if (idProdProj > 0)
                sql += " And pp.idProdProj=" + idProdProj;

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And pp.codInterno Like ?codInterno";

            if (!String.IsNullOrEmpty(descricao))
                sql += " And pp.descricao Like ?descricao";

            if (!string.IsNullOrEmpty(codInternoAssoc))
                sql += @" AND (pp.IdProdProj IN (SELECT DISTINCT ppc1.IdProdProj FROM produto_projeto_config ppc1 WHERE ppc1.IdProd IN (SELECT p1. IdProd FROM produto p1 WHERE p1.CodInterno=?codInternoAssoc))
                    OR pp.IdProd IN (SELECT p1.IdProd FROM produto p1 WHERE p1.CodInterno=?codInternoAssoc))";
            
            if (!string.IsNullOrEmpty(descricaoAssoc))
                sql += @" AND (pp.IdProdProj IN (SELECT DISTINCT ppc1.IdProdProj FROM produto_projeto_config ppc1 WHERE ppc1.IdProd IN (SELECT p1. IdProd FROM produto p1 WHERE p1.Descricao LIKE ?descricaoAssoc))
                    OR pp.IdProd IN (SELECT p1.IdProd FROM produto p1 WHERE p1.Descricao LIKE ?descricaoAssoc))";

            if (tipo > 0)
                sql += " And pp.tipo=" + tipo;

            return sql;
        }

        public IList<ProdutoProjeto> GetList(string codInterno, string descricao, string codInternoAssoc, string descricaoAssoc, int tipo,
            string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "pp.tipo, pp.codInterno" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, codInterno, descricao, codInternoAssoc, descricaoAssoc, tipo, true), sort, startRow, pageSize,
                GetParam(codInterno, descricao, codInternoAssoc, descricaoAssoc));
        }

        public int GetCount(string codInterno, string descricao, string codInternoAssoc, string descricaoAssoc, int tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, codInterno, descricao, codInternoAssoc, descricaoAssoc, tipo, false),
                GetParam(codInterno, descricao, codInternoAssoc, descricaoAssoc));
        }

        public ProdutoProjeto GetElement(uint idProdProj)
        {
            List<ProdutoProjeto> item = objPersistence.LoadData(Sql(idProdProj, null, null, null, null, 0, true));
            return item.Count > 0 ? item[0] : null;
        }

        private GDAParameter[] GetParam(string codInterno, string descricao, string codInternoAssoc, string descricaoAssoc)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", "%" + codInterno + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            if (!String.IsNullOrEmpty(codInternoAssoc))
                lstParam.Add(new GDAParameter("?codInternoAssoc", codInternoAssoc));

            if (!String.IsNullOrEmpty(descricaoAssoc))
                lstParam.Add(new GDAParameter("?descricaoAssoc", "%" + descricaoAssoc + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        /// <summary>
        /// Busca o produto pelo código interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public ProdutoProjeto GetByCodInterno(string codInterno)
        {
            string sql = "Select pp.*, p.descricao as ProdutoAssociado From produto_projeto pp " + 
                "Left Join produto p On (pp.idProd=p.idProd) Where pp.codInterno=?codInterno";

            List<ProdutoProjeto> lstProdProj = objPersistence.LoadData(sql, new GDAParameter("?codInterno", codInterno));

            return lstProdProj.Count > 0 ? lstProdProj[0] : null;
        }

        /// <summary>
        /// Associa o produto passado ao produto do projeto
        /// </summary>
        /// <param name="idProdProj"></param>
        /// <param name="idProd"></param>
        public void AssociaProduto(uint idProdProj, string codInterno)
        {
            string sql = "Update produto_projeto Set idProd=(Select idProd from produto Where codInterno='" + codInterno + "' limit 0,1) " +
                " Where idProdProj=" + idProdProj;

            if (objPersistence.ExecuteCommand(sql) <= 0)
                throw new Exception("Falha ao associar produto. Atualização retornou 0.");
        }

        /// <summary>
        /// Retorna o id do produto relacionado à "ESCOVA"
        /// </summary>
        /// <returns></returns>
        public uint GetEscovaId()
        {
            return GetEscovaId(null);
        }

        /// <summary>
        /// Retorna o id do produto relacionado à "ESCOVA"
        /// </summary>
        /// <returns></returns>
        public uint GetEscovaId(GDASession sessao)
        {
            string sql = "Select coalesce(idProd,0) From produto_projeto Where codInterno='ESCOVA'";

            return Convert.ToUInt32(objPersistence.ExecuteScalar(sessao, sql));
        }

        /// <summary>
        /// Retorna o id do produto relacionado à mão de obra
        /// </summary>
        /// <returns></returns>
        public uint GetMaoDeObraId()
        {
            return GetMaoDeObraId(null);
        }


        /// <summary>
        /// Retorna o id do produto relacionado à mão de obra
        /// </summary>
        /// <returns></returns>
        public uint GetMaoDeObraId(GDASession sessao)
        {
            string sql = "Select coalesce(idProd,0) From produto_projeto Where codInterno='MO'";

            return Convert.ToUInt32(objPersistence.ExecuteScalar(sessao, sql));
        }

        /// <summary>
        /// Desvincula produtos associados ao produtoProjeto passado
        /// </summary>
        /// <param name="prodProj"></param>
        /// <returns></returns>
        public int DesvincularItens(uint idProdProj)
        {
            string sql = "Update produto_projeto Set idProd=null Where idProdProj=" + idProdProj + ";" +
                "Delete from produto_projeto_config Where idProdProj=" + idProdProj;

            return objPersistence.ExecuteCommand(sql);
        }

        public uint? FindByCodInterno(GDASession session, string codInterno)
        {
            List<ProdutoProjeto> lstProdProj = objPersistence.LoadData(session, "select * from produto_projeto where codInterno=?codInterno",
                new GDAParameter("?codInterno", codInterno)).ToList();

            if (lstProdProj.Count == 0)
                return null;

            if (lstProdProj.Count > 2)
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Existem mais de um produto de projeto cadastrado com o código interno " + codInterno, null));

            return lstProdProj[0].IdProdProj;
        }

        #region Obtém valor de campos do produto de projeto

        /// <summary>
        /// Obtém o tipo do produto de projeto.
        /// </summary>
        public int ObterTipo(GDASession session, int idProdProj)
        {
            return ObtemValorCampo<int>(session, "Tipo", "IdProdProj=" + idProdProj);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(ProdutoProjeto objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutoProjeto objInsert)
        {
            // Verifica se já existe um produto com este código
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, "Select Count(*) From produto_projeto Where codInterno=?cod",
                new GDAParameter("?cod", objInsert.CodInterno)).ToString()) > 0)
                throw new Exception("Já existe um produto cadastrado com este código.");

            return base.Insert(objInsert);
        }

        public override int Update(ProdutoProjeto objUpdate)
        {
            // Verifica se já existe um produto com este código
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From produto_projeto Where codInterno=?cod And idProdProj<>" +
                objUpdate.IdProdProj, new GDAParameter("?cod", objUpdate.CodInterno)).ToString()) > 0)
                throw new Exception("Já existe um produto cadastrado com este código.");

            LogAlteracaoDAO.Instance.LogProdutoProjeto(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(ProdutoProjeto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProdProj);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se há algum material associado à este projeto e não permite excluir caso haja
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_projeto_modelo Where idProdProj=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído, pois existem materiais de projetos associados ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogProdutoProjeto(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
	}
}