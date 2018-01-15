using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class MaterialProjetoModeloDAO : BaseDAO<MaterialProjetoModelo, MaterialProjetoModeloDAO>
    {
        //private MaterialProjetoModeloDAO() { }

        #region Busca padrão

        private string Sql(uint idMaterProjMod, uint idProjetoModelo, int? espessura, bool selecionar)
        {
            string campos = selecionar ? @"mpm.*, pp.idProd, pp.tipo as tipoProd, pp.codInterno as codMaterial, 
                Concat(pp.CodInterno, ' - ', pp.Descricao) as DescrProdProj" : "Count(*)";
            
            string sql = "Select " + campos + " From material_projeto_modelo mpm " + 
                "Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) Where 1";

            if (idMaterProjMod > 0)
                sql += " And mpm.idMaterProjMod=" + idMaterProjMod;

            if (idProjetoModelo > 0)
                sql += " And mpm.idProjetoModelo=" + idProjetoModelo;

            if (espessura > 0 && Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto)
                sql += " AND (mpm.Espessuras LIKE ?espessura OR mpm.Espessuras IS NULL OR mpm.Espessuras='')";

            return sql;
        }

        private GDAParameter[] GetParam(int? espessura)
        {
            var lstParam = new List<GDAParameter>();

            if (espessura > 0 && Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto)
                lstParam.Add(new GDAParameter("?espessura", "%" + espessura + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public List<MaterialProjetoModelo> GetByProjetoModelo(uint idProjetoModelo, int? espessura)
        {
            return GetByProjetoModelo(null, idProjetoModelo, espessura);
        }

        public List<MaterialProjetoModelo> GetByProjetoModelo(GDASession sessao, uint idProjetoModelo, int? espessura)
        {
            return objPersistence.LoadData(sessao, Sql(0, idProjetoModelo, espessura, true), GetParam(espessura));
        }

        public MaterialProjetoModelo GetElement(uint idMaterProjMod)
        {
            return GetElement(null, idMaterProjMod);
        }

        public MaterialProjetoModelo GetElement(GDASession session, uint idMaterProjMod)
        {
            return objPersistence.LoadOneData(session, Sql(idMaterProjMod, 0, null, true));
        }

        #endregion

        #region Busca para inserção

        private string SqlIns(uint idProjetoModelo, bool selecionar)
        {
            string campos = selecionar ? "mpm.*, pp.idProd, pp.tipo as tipoProd, Concat(pp.CodInterno, ' - ', pp.Descricao) as DescrProdProj" : "Count(*)";

            string sql = "Select " + campos + " From material_projeto_modelo mpm " +
                "Left Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) Where mpm.idProjetoModelo=" + idProjetoModelo;

            return sql;
        }

        public List<MaterialProjetoModelo> GetListIns(uint idProjetoModelo)
        {
            if (GetCountRealIns(idProjetoModelo) == 0)
            {
                List<MaterialProjetoModelo> lst = new List<MaterialProjetoModelo>();
                lst.Add(new MaterialProjetoModelo());
                return lst;
            }

            return objPersistence.LoadData(SqlIns(idProjetoModelo, true));
        }

        public int GetCountRealIns(uint idProjetoModelo)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlIns(idProjetoModelo, false), null);
        }

        public int GetCountIns(uint idProjetoModelo)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlIns(idProjetoModelo, false), null);

            return count == 0 ? 1 : count;
        }

        #endregion

        /// <summary>
        /// Verifica se o modelo utilizado no itemProjeto passado requer kit
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool RequerKit(uint idItemProjeto)
        {
            string sql = @"
                Select Count(*) From material_projeto_modelo mpm 
                    Inner Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) 
                Where mpm.idProjetoModelo In (Select idProjetoModelo From item_projeto Where idItemProjeto=" + idItemProjeto + @") 
                    And pp.CodInterno='KIT'";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        /// <summary>
        /// Verifica se o modelo utilizado no itemProjeto passado requer Tubo
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        public bool RequerTubo(uint idItemProjeto)
        {
            string sql = "Select Count(*) From material_projeto_modelo mpm " +
                "Inner Join produto_projeto pp On (mpm.idProdProj=pp.idProdProj) " +
                "Where mpm.idProjetoModelo In (Select idProjetoModelo From item_projeto Where idItemProjeto=" + idItemProjeto + ") " +
                "And pp.CodInterno='TUBO'";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #region Métodos sobrescritos

        public override uint Insert(MaterialProjetoModelo objInsert)
        {
            /* Chamado 52016. */
            if (ProdutoProjetoDAO.Instance.ObterTipo(null, (int)objInsert.IdProdProj) != (int)ProdutoProjeto.TipoProduto.Aluminio && objInsert.GrauCorte > 0)
                throw new System.Exception("Somente produtos de projeto do tipo Alumínio podem possuir grau de corte.");
            
            return base.Insert(objInsert);
        }

        public override int Update(MaterialProjetoModelo objUpdate)
        {
            /* Chamado 52016. */
            if (ProdutoProjetoDAO.Instance.ObterTipo(null, (int)objUpdate.IdProdProj) != (int)ProdutoProjeto.TipoProduto.Aluminio && objUpdate.GrauCorte > 0)
                throw new System.Exception("Somente produtos de projeto do tipo Alumínio podem possuir grau de corte.");

            LogAlteracaoDAO.Instance.LogMaterialProjetoModelo(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(MaterialProjetoModelo objDelete)
        {
            LogAlteracaoDAO.Instance.ApagaLogMaterialProjetoModelo(objDelete.IdMaterProjMod);
            LogCancelamentoDAO.Instance.LogMaterialProjetoModelo(objDelete, objDelete.MotivoCancelamento, true);
            return GDAOperations.Delete(objDelete);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElement(Key));
        }

        #endregion
    }
}