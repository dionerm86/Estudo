using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class SubgrupoProdDAO : BaseDAO<SubgrupoProd, SubgrupoProdDAO>
	{
        /// <summary>
        /// Busca todos os Subgrupos de produto do Grupo passado
        /// </summary>
        public string Sql(string idGrupoProd, bool paraPedidoProducao, bool paraPedidoInterno, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            string filtroProducao = paraPedidoProducao && ("," + idGrupoProd + ",").Contains("," + (uint)Glass.Data.Model.NomeGrupoProd.Vidro + ",") ? " and produtosEstoque=1" : "";
            string filtroPedidoInterno = paraPedidoInterno ? " and s.idSubgrupoProd in (select s.idSubgrupoProd from produto where compra=true)" : "";
            
            return "Select " + campos + " From subgrupo_prod Where IdGrupoProd in (" + idGrupoProd  + ")" + filtroProducao + filtroPedidoInterno + " ORDER BY Descricao";
        }

        public string SqlTabelaCliente(int idGrupoProd, int idCli, bool selecionar)
        {
            string sql = @"
                Select " + (selecionar ? "*" : "count(*)") + @" 
                From subgrupo_prod s 
                    Left Join desconto_acrescimo_cliente d on (s.idsubgrupoprod = d.idsubgrupoprod) 
                Where s.IdGrupoProd in (" + idGrupoProd + ")";

            if (idCli > 0)
            {
                /* Chamado 52406.
                 * Caso o cliente possua associação com o subgrupo, em seu cadastro, é necessário que sejam exibidos somente esses subgrupos na listagem de preço de tabela por cliente. */
                var idsSubgrupoCliente = ClienteDAO.Instance.ObtemIdsSubgrupo((uint)idCli);

                // Esta associação é feita no cadastro do cliente.
                if (!string.IsNullOrEmpty(idsSubgrupoCliente))
                    sql += string.Format(" AND s.IdSubgrupoProd IN ({0})", idsSubgrupoCliente);
                // Esta associação é feita diretamente no cadastro do subgrupo.
                else
                    sql += string.Format(" AND (s.IdCli={0} OR s.IdCli IS NULL)", idCli);
            }

            sql += " Group By s.IDSUBGRUPOPROD Order By s.Descricao";

            return sql;
        }

        public IList<SubgrupoProd> GetList(int idGrupoProd)
        {
            var sql = Sql(idGrupoProd.ToString(), false, false, true);
            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna os grupos de produtos e seus subgrupos (se houver)
        /// </summary>
        /// <returns></returns>
        public SubgrupoProd[] GetRptSubGrupo()
        {
            return
                objPersistence.LoadData(
                    @"SELECT s.IdSubgrupoProd, COALESCE(s.IdGrupoProd, g.IdGrupoProd) AS IdGrupoProd, s.Descricao,
                        s.TipoCalculo, s.TipoCalculoNf, s.BloquearEstoque, s.NaoAlterarEstoque, s.NaoAlterarEstoqueFiscal,
                        s.ProdutosEstoque, s.IsVidroTemperado, s.ExibirMensagemEstoque, s.NumeroDiasMinimoEntrega, s.DiaSemanaEntrega,
                        s.GeraVolume, s.TipoSubgrupo, s.IdCli, s.LiberarPendenteProducao, g.Descricao AS DescrGrupo
                    FROM grupo_prod g
                        LEFT JOIN subgrupo_prod s ON (s.IdGrupoProd = g.IdGrupoProd)").ToList().ToArray();
        }

        public IList<SubgrupoProd> GetList(int idGrupoProd, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idGrupoProd) == 0)
            {
                List<SubgrupoProd> lst = new List<SubgrupoProd>();
                lst.Add(new SubgrupoProd());
                return lst.ToArray();
            }

            return LoadDataWithSortExpression(Sql(idGrupoProd.ToString(), false, false, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal(int idGrupoProd)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idGrupoProd.ToString(), false, false, false), null);
        }

        public int GetCount(int idGrupoProd)
        {
            int count = objPersistence.ExecuteSqlQueryCount(Sql(idGrupoProd.ToString(), false, false, false), null);

            return count == 0 ? 1 : count;
        }

        public SubgrupoProd[] GetForFilter(int idGrupo)
        {
            return GetForFilter(idGrupo.ToString(), false, false);
        }
        
        public SubgrupoProd[] GetForFilter(string idGrupos)
        {
            return GetForFilter(idGrupos, false, false);
        }

        public SubgrupoProd[] GetForFilter(string idGrupo, bool paraPedidoProducao, bool paraPedidoInterno)
        {
            List<SubgrupoProd> lst = new List<SubgrupoProd>();

            if (!String.IsNullOrEmpty(idGrupo) && idGrupo != "0")
                lst = objPersistence.LoadData(Sql(idGrupo, paraPedidoProducao, paraPedidoInterno, true));

            if (!paraPedidoProducao)
            {
                SubgrupoProd subgrupo = new SubgrupoProd();
                subgrupo.Descricao = "Todos";
                subgrupo.IdSubgrupoProd = 0;
                lst.Insert(0, subgrupo);
            }

            if (UserInfo.GetUserInfo.IsCliente)
                lst = lst.Where(f => !f.BloquearEcommerce).ToList();

            return lst.ToArray();
        }

        public SubgrupoProd[] GetForTabelaCliente(int idGrupo, int idCli)
        {
            List<SubgrupoProd> lst = objPersistence.LoadData(SqlTabelaCliente(idGrupo, idCli, true));

            SubgrupoProd subgrupo = new SubgrupoProd();
            subgrupo.Descricao = "Todos";
            subgrupo.IdSubgrupoProd = 0;
            lst.Insert(0, subgrupo);

            return lst.ToArray();
        }

        public SubgrupoProd[] GetForTabelaClienteEcommerce(int idCli)
        {
            if (idCli == 0)
                throw new Exception("Nenhum cliente informado na pesquisa de subgrupos");

            List<SubgrupoProd> lst = objPersistence.LoadData(SqlTabelaCliente((int)NomeGrupoProd.Vidro, idCli, true));

            SubgrupoProd subgrupo = new SubgrupoProd();
            subgrupo.Descricao = "Todos";
            subgrupo.IdSubgrupoProd = 0;
            lst.Insert(0, subgrupo);

            return lst.ToArray();
        }

        public IList<SubgrupoProd> GetForCadCliente()
        {
            var sql = @"
                SELECT sgp.*, gp.Descricao as DescrGrupo
                FROM subgrupo_prod sgp
                    INNER JOIN grupo_prod gp ON (sgp.IdGrupoProd = gp.IdGrupoProd)
                ORDER BY gp.Descricao, sgp.Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        public bool IsVidroTemperado(GDASession session, uint idProd)
        {
            string sql = @"
                Select count(*) 
                From subgrupo_prod 
                Where idGrupoProd=1 
                    And isVidroTemperado=true And idSubgrupoProd In (
                        Select idSubgrupoProd From produto Where idProd=" + idProd + @"
                    )";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <returns></returns>
        public string ObtemSubgruposMarcadosFiltro(int idCli)
        {
            return ObtemSubgruposMarcadosFiltro(null, idCli);
        }

        public string ObtemSubgruposMarcadosFiltro(GDASession sessao, int idCli)
        {
            var sql = "select cast(group_concat(idSubgrupoProd) as char) from subgrupo_prod where idCli=" + idCli;
            var subgrupoTemperado = objPersistence.ExecuteScalar(sessao, sql);

            return subgrupoTemperado != null ? subgrupoTemperado.ToString() : String.Empty;
        }

        /// <summary>
        /// Obtém subgrupos pelos idsSubGrupo informados
        /// </summary>
        public IList<Data.Model.SubgrupoProd> ObtemSubgrupos(GDASession sessao, List<int> idsSubGrupo)
        {
            if (idsSubGrupo == null || idsSubGrupo.Count == 0)
                return new List<SubgrupoProd>();

            string sql = string.Format("Select * From subgrupo_prod Where IdSubgrupoProd IN ({0})", string.Join(",", idsSubGrupo));

            var subGrupos = objPersistence.LoadData(sessao, sql).ToList();

            return subGrupos;
        }

        public string GetDescricao(int idSubgrupoProd)
        {
            return GetDescricao(null, idSubgrupoProd);
        }

        public string GetDescricao(GDASession sessao, int idSubgrupoProd)
        {
            return ObtemValorCampo<string>(sessao, "descricao", "idSubgrupoProd=" + idSubgrupoProd);
        }

        public string GetDescricao(string idsSubgrupoProd)
        {
            string sql = "select group_concat(descricao separator ', ') from subgrupo_prod where idSubgrupoProd in (" + idsSubgrupoProd + ")";
            return objPersistence.ExecuteScalar(sql).ToString();
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idSubgrupoProd"></param>
        /// <param name="nf"></param>
        /// <returns></returns>
        public int? ObtemTipoCalculo(int idSubgrupoProd, bool nf)
        {
            return ObtemTipoCalculo(null, idSubgrupoProd, nf);
        }

        public int? ObtemTipoCalculo(GDASession sessao, int idSubgrupoProd, bool nf)
        {
            string campo = nf ? "coalesce(tipoCalculoNf, tipoCalculo)" : "tipoCalculo";
            return ObtemValorCampo<int?>(sessao, campo, "idSubgrupoProd=" + idSubgrupoProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idSubgrupoProd"></param>
        /// <returns></returns>
        public bool ObtemNaoAlterarEstoque(int idSubgrupoProd)
        {
            return ObtemNaoAlterarEstoque(null, idSubgrupoProd);
        }

        public bool ObtemNaoAlterarEstoque(GDASession sessao, int idSubgrupoProd)
        {
            string sql = "select count(*) from subgrupo_prod where naoAlterarEstoque=true and idSubgrupoProd=" + idSubgrupoProd;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        public bool ObtemNaoAlterarEstoqueFiscal(GDASession session, int idSubgrupoProd)
        {
            string sql = "select count(*) from subgrupo_prod where naoAlterarEstoqueFiscal=true and idSubgrupoProd=" + idSubgrupoProd;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public bool ObtemVidroTemperado(GDASession sessao, int idSubgrupoProd)
        {
            return ObtemValorCampo<bool>(sessao, "isVidroTemperado", "idSubgrupoProd=" + idSubgrupoProd);
        }

        public string ObtemDescricao(int idSubgrupoProd)
        {
            return ObtemValorCampo<string>("Descricao", "idSubgrupoProd=" + idSubgrupoProd);
        }

        public bool ObtemPermitirItemRevendaNaVenda(int idSubgrupoProd)
        {
            return ObtemValorCampo<bool>("PermitirItemRevendaNaVenda", "idSubgrupoProd=" + idSubgrupoProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public TipoSubgrupoProd ObtemTipoSubgrupo(int idProd)
        {
            return ObtemTipoSubgrupo(null, idProd);
        }

        public bool IsProdutoEstoque(int idSubgrupoProd)
        {
            return ObtemValorCampo<bool>("produtosEstoque", "idSubgrupoProd=" + idSubgrupoProd);
        }

        public TipoSubgrupoProd ObtemTipoSubgrupo(GDASession sessao, int idProd)
        {
            string sql = @"
                SELECT tipoSubgrupo
                FROM subgrupo_prod sgp
                    INNER JOIN produto p ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
                WHERE p.idProd=" + idProd;

            return ExecuteScalar<TipoSubgrupoProd>(sessao, sql);
        }

        public TipoSubgrupoProd ObtemTipoSubgrupoPorSubgrupo(int idSubgrupoProd)
        {
            return ObtemTipoSubgrupoPorSubgrupo(null, idSubgrupoProd);
        }

        public TipoSubgrupoProd ObtemTipoSubgrupoPorSubgrupo(GDASession sessao, int idSubgrupoProd)
        {
            string sql = @"
                SELECT tipoSubgrupo
                FROM subgrupo_prod
                WHERE idSubgrupoProd=" + idSubgrupoProd;

            return ExecuteScalar<TipoSubgrupoProd>(sessao, sql);
        }

        public string GetSubgruposTemperados()
        {
            var sql = "select cast(group_concat(idSubgrupoProd) as char) from subgrupo_prod where idGrupoProd=1 and isVidroTemperado=true";
            var subgrupoTemperado = objPersistence.ExecuteScalar(sql);

            return subgrupoTemperado != null ? subgrupoTemperado.ToString() : String.Empty;
        }

        public int? ObterIdLojaPeloProduto(GDASession sessao, int idProd)
        {
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, idProd);

            if (idSubgrupoProd.GetValueOrDefault() == 0)
                return null;

            return ObterIdLoja(sessao, idSubgrupoProd.Value);
        }

        public int? ObterIdLoja(GDASession sessao, int idSubrupoProd)
        {
            return ExecuteScalar<int?>(sessao, "SELECT IdLoja FROM subgrupo_prod sgp WHERE IdSubgrupoProd=" + idSubrupoProd);
        }

        public bool ObterBloquearEcommerce(GDASession sessao, int idSubrupoProd)
        {
            return ExecuteScalar<bool>(sessao, "SELECT BloquearEcommerce FROM subgrupo_prod sgp WHERE IdSubgrupoProd=" + idSubrupoProd);
        }

        /// <summary>
        /// Recupera os ids dos subgrupos de chapa de vidro
        /// </summary>
        /// <param name="sessao"></param>
        /// <returns></returns>
        public List<uint> ObterSubgruposChapaVidro(GDASession sessao)
        {
            var sql = "SELECT IdSubgrupoProd FROM subgrupo_prod WHERE IdGrupoProd = 1 AND TipoSubgrupo IN (" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + ")";

            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        #region Verifica se o subgrupo é usado para produção

        /// <summary>
        /// SQL para verificar se um subgrupo é usado para revenda.
        /// (Utilizado para JOIN)
        /// </summary>
        /// <returns></returns>
        internal string SqlSubgrupoRevenda()
        {
            return SqlSubgrupoRevenda(null, null);
        }

        /// <summary>
        /// SQL para verificar se um subgrupo é usado para revenda.
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        /// <returns></returns>
        internal string SqlSubgrupoRevenda(string idGrupo, string idSubgrupo)
        {
            bool usarJoin = String.IsNullOrEmpty(idGrupo) && String.IsNullOrEmpty(idSubgrupo);
            string campos = (usarJoin ?
                "s1.idGrupoProd, s1.idSubgrupoProd" :
                "count(*) as contagem");

            string sql = @"
                select {0}
                from subgrupo_prod s1
                   inner join grupo_prod g1 on (s1.idGrupoProd=g1.idGrupoProd)
                where (s1.idGrupoProd not in ({1},{2})
                   or (coalesce(s1.tipoCalculo, g1.tipoCalculo, {3})={3}
                   and s1.produtosEstoque=true))";

            if (!String.IsNullOrEmpty(idGrupo))
                sql += " and s1.idGrupoProd=" + idGrupo;

            if (!String.IsNullOrEmpty(idSubgrupo))
                sql += " and s1.idSubgrupoProd=" + idSubgrupo;

            if (usarJoin)
                sql += " group by s1.idGrupoProd, s1.idSubgrupoProd";

            return String.Format(sql,
                campos,
                (int)Glass.Data.Model.NomeGrupoProd.Vidro,
                (int)Glass.Data.Model.NomeGrupoProd.MaoDeObra,
                (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd);
        }

        public bool IsSubgrupoProducao(GDASession sessao, int idGrupo, int? idSubgrupo)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, SqlSubgrupoRevenda(idGrupo.ToString(),
                idSubgrupo != null ? idSubgrupo.Value.ToString() : null)) > 0;
        }

        public bool IsSubgrupoProducao(int idGrupo, int? idSubgrupo)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlSubgrupoRevenda(idGrupo.ToString(), 
                idSubgrupo != null ? idSubgrupo.Value.ToString() : null)) > 0;
        }

        public bool IsSubgrupoProducao(int idProd)
        {
            int idGrupo = ProdutoDAO.Instance.ObtemIdGrupoProd(idProd);
            int? idSubgrupo = ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd);
            return IsSubgrupoProducao(idGrupo, idSubgrupo);
        }

        public bool IsSubgrupoGeraVolume(uint idGrupo, uint idSubgrupo)
        {
            return IsSubgrupoGeraVolume(null, idGrupo, idSubgrupo);
        }

        public bool IsSubgrupoGeraVolume(GDASession session, uint idGrupo, uint idSubgrupo)
        {
            var sql = @"
                SELECT count(*)
                FROM grupo_prod gp
                    LEFT JOIN subgrupo_prod sgp ON (sgp.idGrupoProd = gp.idGrupoProd)
                WHERE COALESCE(sgp.geraVolume, gp.geraVolume, false) = true
                    AND gp.idGrupoProd = {0}";

            if (idSubgrupo > 0)
                sql += " AND sgp.idSubgrupoProd = " + idSubgrupo;

            return ExecuteScalar<int>(session, string.Format(sql, idGrupo)) > 0;
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(SubgrupoProd objInsert)
        {
            // Verifica se existe algum desconto lançado no grupo deste subgrupo, se existir, não permite inserir.
            if (ExecuteScalar<bool>("Select Count(*)>0 From desconto_acrescimo_cliente Where Coalesce(idSubgrupoProd, 0) = 0 And idGrupoProd=" + objInsert.IdGrupoProd))
                throw new Exception("Não é possível inserir um subgrupo neste grupo pois já existem descontos por cliente configurados para o mesmo. Para inserir este subgrupo será necessário retirar os desconto deste grupo de produto.");

            return base.Insert(objInsert);
        }

        public override int Update(SubgrupoProd objUpdate)
        {
            if (objUpdate.TipoCalculo.GetValueOrDefault(0) != Glass.Data.Model.TipoCalculoGrupoProd.Qtd)
                objUpdate.ProdutosEstoque = false;
            
            // Chamado 13679.
            // O erro ocorreu porque o usuário editou a descrição de um subgrupo padrão do sistema.
            if (objUpdate.IsPadraoSistema &&
                objUpdate.Descricao != ObtemValorCampo<string>("Descricao", "IdSubgrupoProd=" + objUpdate.IdSubgrupoProd))
                throw new Exception("Não é possível editar a descrição de um subgrupo padrão do sistema.");

            LogAlteracaoDAO.Instance.LogSubgrupoProduto(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(SubgrupoProd objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdSubgrupoProd);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto Where idSubgrupoProd=" + Key) > 0)
                throw new Exception("Este subgrupo não pode ser excluído. Existem produtos relacionados ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogSubgrupoProduto(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
	}
}