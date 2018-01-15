using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class EtiquetaProcessoDAO : BaseDAO<EtiquetaProcesso, EtiquetaProcessoDAO>
	{
        //private EtiquetaProcessoDAO() { }

        private string SqlList(string codInterno, string descricao, uint idAplicacao, int situacao, uint idSubgrupo, bool selecionar)
        {
            string campos = selecionar ? "p.*, a.CodInterno as CodAplicacao, a.Descricao as DescrAplicacao" : "Count(*)";
            string sql = "Select " + campos + " From etiqueta_processo p left join etiqueta_aplicacao a on (p.idAplicacao=a.idAplicacao) Where 1";

            if (!String.IsNullOrEmpty(codInterno))
                sql += " and p.CodInterno=?codInterno";

            if (!String.IsNullOrEmpty(descricao))
                sql += " and p.Descricao LIKE ?descricao";

            if (idAplicacao > 0)
                sql += " AND p.idAplicacao=" + idAplicacao;

            if (situacao > 0)
                sql += " and p.situacao=" + situacao;

            if (idSubgrupo > 0)
                sql += @" AND IF ((SELECT COUNT(*) FROM classificacao_subgrupo) > 0, p.IdProcesso IN (Select rp.IdProcesso from classificacao_subgrupo cs 
                          LEFT JOIN classificacao_roteiro_producao crp ON (cs.IdClassificacaoRoteiroProducao = crp.IdClassificacaoRoteiroProducao)
                          LEFT JOIN roteiro_producao rp ON (crp.IdClassificacaoRoteiroProducao = rp.IdClassificacaoRoteiroProducao) 
                          WHERE cs.idsubgrupoprod = " + idSubgrupo + "), 1)";

            return sql;
        }

        public IList<EtiquetaProcesso> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                var lst = new List<EtiquetaProcesso>();
                lst.Add(new EtiquetaProcesso());
                return lst.ToArray();
            }

            string orderBy = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(null, null, 0, 0, 0, true), orderBy, startRow, pageSize, null);
        }

        public IList<EtiquetaProcesso> GetForFilter()
        {
            return objPersistence.LoadData(SqlList(null, null, 0, 0, 0, true) + " order by p.codInterno").ToList();
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(null, null, 0, 0, 0, false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(null, null, 0, 0, 0, false), null);

            return count == 0 ? 1 : count;
        }

        public IList<EtiquetaProcesso> GetForSel(string sortExpression, int startRow, int pageSize)
        {
            string orderBy = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(null, null, 0, (int)Glass.Situacao.Ativo, 0, true), orderBy, startRow, pageSize, null);
        }

        public int GetForSelCount()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(null, null, 0, (int)Glass.Situacao.Ativo, 0, false), null);
        }

        public IList<EtiquetaProcesso> GetForSel(string codProcesso, string descricao, uint idAplicacao, uint idSubgrupo,
            string sortExpression, int startRow, int pageSize)
        {
            string orderBy = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(codProcesso, descricao, idAplicacao, (int)Glass.Situacao.Ativo, idSubgrupo, true), 
                orderBy, startRow, pageSize, GetParams(codProcesso, descricao));
        }

        public int GetForSelCount(string codProcesso, string descricao, uint idAplicacao, uint idSubgrupo)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(codProcesso, descricao, idAplicacao, (int)Glass.Situacao.Ativo, idSubgrupo, false),
                GetParams(codProcesso, descricao));
        }

        public string GetCodInternoByIds(string idsProcesso)
        {
            return GetCodInternoByIds(null, idsProcesso);
        }

        public string GetCodInternoByIds(GDASession session, string idsProcesso)
        {
            return objPersistence.ExecuteScalar(session, "select cast(group_concat(codInterno separator ', ') as char) from " +
                "etiqueta_processo where idProcesso in (" + idsProcesso + ")").ToString();
        }

        public GDAParameter[] GetParams(string codProcesso, string descricao)
        {
            var p = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(codProcesso))
                p.Add(new GDAParameter("?codInterno", codProcesso));

            if (!string.IsNullOrEmpty(descricao))
                p.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return p.ToArray();
        }

        #region Obtem valores dos campos

        public uint? ObtemIdProcessoAtivo(string codInterno)
        {
            return ObtemIdProcessoAtivo(null, codInterno);
        }

        public uint? ObtemIdProcessoAtivo(GDASession session, string codInterno)
        {
            return ObtemValorCampo<uint?>(session, "idProcesso", "codInterno=?codInterno and situacao=" + (int)Glass.Situacao.Ativo,
                new GDAParameter("?codInterno", codInterno));
        }

        public uint? ObtemIdProcesso(string codInterno)
        {
            return ObtemIdProcesso(null, codInterno);
        }

        public uint? ObtemIdProcesso(GDASession session, string codInterno)
        {
            return ObtemValorCampo<uint?>(session, "idProcesso", "codInterno=?codInterno", new GDAParameter("?codInterno", codInterno));
        }

        public string ObtemCodInterno(uint idProcesso)
        {
            return ObtemCodInterno(null, idProcesso);
        }

        public string ObtemCodInterno(GDASession session, uint idProcesso)
        {
            return ObtemValorCampo<string>(session, "codInterno", "idProcesso=" + idProcesso);
        }

        public bool ObtemGerarFormaInexistente(uint idProcesso)
        {
            return ObtemGerarFormaInexistente(null, idProcesso);
        }

        public bool ObtemGerarFormaInexistente(GDASession session, uint idProcesso)
        {
            return ObtemValorCampo<bool>(session, "gerarFormaInexistente", "idProcesso=" + idProcesso);
        }

        public int ObtemTipoProcesso(uint idProcesso)
        {
            return ObtemTipoProcesso(null, idProcesso);
        }

        public int ObtemTipoProcesso(GDASession sessao, uint idProcesso)
        {
            return ObtemValorCampo<int>(sessao, "tipoProcesso", "idProcesso=" + idProcesso);
        }

        public uint? ObtemIdAplicacao(uint idProcesso)
        {
            return ObtemValorCampo<uint?>("idAplicacao", "idProcesso=" + idProcesso);
        }

        public bool ObterGerarArquivoDeMesa(uint idProcesso)
        {
            return ObtemValorCampo<bool>("GerarArquivoDeMesa", "idProcesso=" + idProcesso);
        }

        public string ObtemTipoPedido(uint idProcesso)
        {
            return ObtemValorCampo<string>("TipoPedido", "idProcesso=" + idProcesso);
        }

        /// <summary>
        /// Retorna o número mínimo de dias úteis para a data de entrega.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public int ObterNumeroDiasUteisDataEntrega(GDASession sessao, uint idProcesso)
        {
            return ObtemValorCampo<int>(sessao, "NumeroDiasUteisDataEntrega", "IdProcesso=" + idProcesso);
        }

        #endregion

        #region Busca processo pelo seu código interno

        /// <summary>
        /// Busca processo pelo seu código interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public EtiquetaProcesso GetByCodInterno(string codInterno)
        {
            List<EtiquetaProcesso> lst = objPersistence.LoadData(SqlList(codInterno, null, 0, (int)Glass.Situacao.Ativo, 0, true),
                new GDAParameter[] { new GDAParameter("?codInterno", codInterno) });

            return lst.Count > 0 ? lst[0] : null;
        }

        #endregion

        #region Métodos Sobrescritos

        public override int Update(EtiquetaProcesso objUpdate)
        {
            string codInterno = ObtemCodInterno((uint)objUpdate.IdProcesso);

            // Atualiza a configuração para ficar igual à esta alteração
            if (objUpdate.CodInterno != codInterno)
            {
                objPersistence.ExecuteCommand("Update config_loja set valorTexto=?codProcNovo Where idConfig In (19,21) And valorTexto=?codProcAntigo", 
                    new GDAParameter("?codProcNovo", objUpdate.CodInterno), new GDAParameter("?codProcAntigo", codInterno));
            }

            return base.Update(objUpdate);
        }

        public override int Delete(EtiquetaProcesso objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProcesso);
        }

        public override int DeleteByPrimaryKey(uint idProcesso)
        {
            // Verifica se esta aplicação está sendo usada em alguma parte do sistema
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_pedido Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver pedidos relacionados ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_pedido_espelho Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver conferências relacionadas ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_item_projeto Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver projetos relacionados ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From ambiente_pedido Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver pedidos de mão de obra relacionados ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From ambiente_pedido_espelho Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver pedidos de mão de obra relacionados ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From benef_config Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver beneficiamentos relacionados ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_troca_dev Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver trocas relacionadas ao mesmo.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_trocado Where idProcesso=" + idProcesso) > 0)
                throw new Exception("Este Processo não pode ser excluído por haver trocas relacionadas ao mesmo.");

            return base.DeleteByPrimaryKey(idProcesso);
        }

        #endregion
    }
}