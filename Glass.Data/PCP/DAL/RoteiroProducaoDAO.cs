using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class RoteiroProducaoDAO : BaseDAO<RoteiroProducao, RoteiroProducaoDAO>
    {
        private string Sql(int idRoteiroProducao, uint idGrupoProd, uint idSubgrupoProd, uint idProcesso, string codProcesso,
            int idClassificacaoRoteiroProducao, bool semClassificacao, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "rp.*, ep.codInterno as codProcesso, '$$$' as criterio" : "count(*)");

            sql.AppendFormat(@"
                from roteiro_producao rp
                    left join etiqueta_processo ep on (rp.idProcesso=ep.idProcesso)
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder(), criterio = new StringBuilder();

            if (idRoteiroProducao > 0)
                fa.AppendFormat(" and rp.idRoteiroProducao={0}", idRoteiroProducao);

            if (idGrupoProd > 0)
            {
                fa.AppendFormat(" and rp.idGrupoProd={0}", idGrupoProd);
                criterio.AppendFormat("Grupo de Produto: {0}    ", GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd));
            }

            if (idSubgrupoProd > 0)
            {
                fa.AppendFormat(" and rp.idSubgrupoProd={0}", idSubgrupoProd);
                criterio.AppendFormat("Subgrupo de Produto: {0}    ", SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd));
            }

            if (idProcesso > 0)
            {
                fa.AppendFormat(" and rp.idProcesso={0}", idProcesso);
                criterio.AppendFormat("Processo: {0}    ", EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso));
            }

            if (!string.IsNullOrEmpty(codProcesso))
            {
                fa.Append(" AND ep.codInterno like ?codProcesso");
                criterio.AppendFormat("Processo: {0}    ", codProcesso);
            }

            if (idClassificacaoRoteiroProducao > 0)
            {
                fa.AppendFormat(" AND rp.idClassificacaoRoteiroProducao={0}", idClassificacaoRoteiroProducao);
            }

            if (semClassificacao)
            {
                fa.AppendFormat(" AND COALESCE(rp.idClassificacaoRoteiroProducao, 0)=0");
            }

            filtroAdicional = fa.ToString();
            return sql.Replace("$$$", criterio.ToString()).ToString();
        }

        public IList<RoteiroProducao> ObtemLista(int idRoteiroProducao, uint idGrupoProd, uint idSubgrupoProd, uint idProcesso, string codProcesso,
            int idClassificacaoRoteiroProducao, bool semClassificacao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(idRoteiroProducao, idGrupoProd, idSubgrupoProd, idProcesso, codProcesso, idClassificacaoRoteiroProducao, semClassificacao, true, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional); ;

            var dados = objPersistence.LoadData(sql, GetParams(codProcesso)).AsEnumerable();

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "CodProcesso" : sortExpression;

            var desc = sortExpression.ToLower().Contains("desc");

            if (sortExpression != "CodProcesso")
            {
                var prop = sortExpression.Split(' ')[0];
                var propertyInfo = typeof(RoteiroProducao).GetProperty(prop);

                if (propertyInfo != null)
                    dados = desc ? dados.OrderByDescending(f => propertyInfo.GetValue(f, null)) : 
                        dados.OrderBy(f => propertyInfo.GetValue(f, null));
            }
            else
                dados = desc ? dados.OrderByDescending(f => f.CodProcesso, new AlphaNumericComparer()) : 
                    dados.OrderBy(f => f.CodProcesso, new AlphaNumericComparer());


           return dados.Skip(startRow).Take(pageSize).ToList();
        }

        public int ObtemNumeroRegistros(int idRoteiroProducao, uint idGrupoProd, uint idSubgrupoProd, uint idProcesso, string codProcesso,
            int idClassificacaoRoteiroProducao, bool semClassificacao)
        {
            string filtroAdicional;
            string sql = Sql(idRoteiroProducao, idGrupoProd, idSubgrupoProd, idProcesso, codProcesso, idClassificacaoRoteiroProducao, semClassificacao, true, out filtroAdicional);

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(codProcesso));
        }

        public IList<RoteiroProducao> ObtemParaRelatorio(int idRoteiroProducao, uint idGrupoProd, uint idSubgrupoProd, uint idProcesso)
        {
            string filtroAdicional, sql = Sql(idRoteiroProducao, idGrupoProd, idSubgrupoProd, idProcesso, null, 0, false, true, out filtroAdicional)
                .Replace(FILTRO_ADICIONAL, filtroAdicional);

            var dados = objPersistence.LoadData(sql).ToList();

            dados = dados.OrderBy(f => f.CodProcesso, new AlphaNumericComparer()).ToList();

            return dados;
        }

        public RoteiroProducao ObtemElemento(GDASession sessao, int idRoteiroProducao)
        {
            string filtroAdicional, sql = Sql(idRoteiroProducao, 0, 0, 0, null, 0, false, true, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadOneData(sessao, sql);
        }

        private GDAParameter[] GetParams(string codProcesso)
        {
            var lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codProcesso))
                lstParams.Add(new GDAParameter("?codProcesso", "%"+codProcesso+"%"));

            return lstParams.ToArray();
        }

        #region Obtem dados do roteiro

        public string ObtemDescricao(int idRoteiroProducao)
        {
            return ObtemDescricao(null, idRoteiroProducao);
        }

        public string ObtemDescricao(GDASession session, int idRoteiroProducao)
        {
            uint idProcesso = ObtemValorCampo<uint>(session, "idProcesso", "idRoteiroProducao=" + idRoteiroProducao);
            return "Processo: " + EtiquetaProcessoDAO.Instance.ObtemCodInterno(session, idProcesso);

            /*
            uint idGrupoProd = ObtemValorCampo<uint>("idGrupoProd", "idRoteiroProducao=" + idRoteiroProducao);
            uint idSubgrupoProd = ObtemValorCampo<uint>("idSubgrupoProd", "idRoteiroProducao=" + idRoteiroProducao);

            return GrupoProdDAO.Instance.GetDescricao(idGrupoProd) + (idSubgrupoProd == 0 ? String.Empty :
                " / " + SubgrupoProdDAO.Instance.GetDescricao(idSubgrupoProd));
            */
        }

        #endregion

        /* public uint? ObtemRoteiroProduto(uint idProd)
        {
            uint idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(idProd);
            uint idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd).GetValueOrDefault();

            string filtroAdicional, sql = "select idRoteiroProducao from (" + Sql(0, idGrupoProd,
                idSubgrupoProd, true, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional) + ") as temp";

            var retorno = ExecuteScalar<uint?>(sql);

            if (retorno.GetValueOrDefault() == 0 && idSubgrupoProd > 0)
            {
                sql = "select idRoteiroProducao from (" + Sql(0, idGrupoProd, 0, 0, true, out filtroAdicional).
                    Replace(FILTRO_ADICIONAL, filtroAdicional) + ") as temp";

                retorno = ExecuteScalar<uint?>(sql);
            }

            return retorno;
        }

        public uint? ObtemIdProcesso(uint idRoteiroProducao)
        {
            return ObtemValorCampo<uint?>("idProcesso", "idRoteiroProducao=" + idRoteiroProducao);
        } */

        #region Metodos Sobrescritos

        public override uint Insert(RoteiroProducao objInsert)
        {
            // Verifica se já foi cadastrado um roteiro com o idProcesso informado
            if (objInsert.IdProcesso > 0 &&
                ExecuteScalar<bool>("Select Count(*) > 0 From roteiro_producao Where idProcesso=" + objInsert.IdProcesso))
                throw new Exception("Já existe um roteiro cadastrado com este processo.");

            return base.Insert(objInsert);
        }

        public override int Update(RoteiroProducao objUpdate)
        {
            if (objUpdate.IdProcesso > 0 &&
                ExecuteScalar<bool>("Select Count(*) > 0 From roteiro_producao Where idProcesso=" + objUpdate.IdProcesso + " AND IdRoteiroProducao <>" + objUpdate.IdRoteiroProducao))
                            throw new Exception("Já existe um roteiro cadastrado com este processo.");

            LogAlteracaoDAO.Instance.LogRoteiroProducao(objUpdate);
            return base.Update(objUpdate);
        }

        #endregion

        #region Classificação Roteiro

        public void AssociaRoteiroClassificacao(int idRoteiro, int idClassificacao)
        {
            objPersistence.ExecuteCommand(@"
                UPDATE roteiro_producao 
                SET idClassificacaoRoteiroProducao=" + idClassificacao + @"
                WHERE idRoteiroProducao=" + idRoteiro);
        }

        public void DesassociarRoteiroClassificacao(int idRoteiro)
        {
            objPersistence.ExecuteCommand(@"
                UPDATE roteiro_producao 
                SET idClassificacaoRoteiroProducao=0
                WHERE idRoteiroProducao=" + idRoteiro);
        }

        #endregion
    }
}