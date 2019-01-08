using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class GrupoProdDAO : BaseDAO<GrupoProd, GrupoProdDAO>
    {
        //private GrupoProdDAO() { }

        #region Busca padrão

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From grupo_prod";

            return sql;
        }

        public IList<GrupoProd> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                List<GrupoProd> lst = new List<GrupoProd>();
                lst.Add(new GrupoProd());
                return lst.ToArray();
            }

            return LoadDataWithSortExpression(SqlList(true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false), null);

            return count == 0 ? 1 : count;
        }

        #endregion

        /// <summary>
        /// Verifica se a venda de produto do grupo/subgrupo passado será bloqueada caso não exista em estoque.
        /// </summary>
        public bool BloquearEstoque(int idGrupoProd, int? idSubgrupoProd)
        {
            return BloquearEstoque(null, idGrupoProd, idSubgrupoProd);
        }

        /// <summary>
        /// Verifica se a venda de produto do grupo/subgrupo passado será bloqueada caso não exista em estoque.
        /// </summary>
        public bool BloquearEstoque(GDASession session, int idGrupoProd, int? idSubgrupoProd)
        {
            // Verifica se o subgrupo de produto passado bloqueia estoque
            if (idSubgrupoProd.GetValueOrDefault() > 0)
            {
                if (objPersistence.ExecuteSqlQueryCount(session, @"
                    SELECT COUNT(*) FROM subgrupo_prod sgp
                    WHERE sgp.BloquearEstoque IS NOT NULL AND sgp.BloquearEstoque AND sgp.IdSubGrupoProd=" + idSubgrupoProd.Value) > 0)
                    return true;
                
                return false;
            }

            // Verifica se o grupo de produto passado bloqueia estoque
            if (objPersistence.ExecuteSqlQueryCount(session, @"
                SELECT COUNT(*) FROM grupo_prod gp
                WHERE gp.BloquearEstoque IS NOT NULL AND gp.BloquearEstoque AND gp.IdGrupoProd=" + idGrupoProd) > 0)
                 return true;
 
             return false;
        }

        public GrupoProd[] GetForFilter()
        {
            return GetForFilter(true, false);
        }

        public GrupoProd[] GetForFilter(bool incluirTodos)
        {
            return GetForFilter(incluirTodos, false);
        }

        public GrupoProd[] GetForFilter(bool incluirTodos, bool paraPedidoInterno)
        {
            var bloquearGrupo = UserInfo.GetUserInfo.IsCliente ? 
                @" AND (SELECT COUNT(*) FROM subgrupo_prod sgp WHERE sgp.IdGrupoProd=gp.IdGrupoProd AND BloquearEcommerce = 0) > 0 " : 
                string.Empty;

            string sql = "Select * From grupo_prod gp Where 1 " + bloquearGrupo +
                (paraPedidoInterno ? "and gp.idGrupoProd in (select idGrupoProd from produto where compra=true) " : "") + 
                "Order By Descricao";          

            List<GrupoProd> lst = objPersistence.LoadData(sql);

            if (incluirTodos)
            {
                GrupoProd grupo = new GrupoProd();
                grupo.IdGrupoProd = 0;
                grupo.Descricao = "Todos";
                lst.Insert(0, grupo);
            }

            return lst.ToArray();
        }

        public string GetDescricao(int idGrupoProd)
        {
            return GetDescricao(null, idGrupoProd);
        }

        public string GetDescricao(GDASession session, int idGrupoProd)
        {
            if (idGrupoProd == 0)
                return "N/D";

            return ObtemValorCampo<string>(session, "descricao", "idGrupoProd=" + idGrupoProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <param name="nf"></param>
        /// <returns></returns>
        public int? ObtemTipoCalculo(int idGrupoProd, bool nf)
        {
            return ObtemTipoCalculo(null, idGrupoProd, nf);
        }

        public int? ObtemTipoCalculo(GDASession sessao, int idGrupoProd, bool nf)
        {
            string campo = nf ? "coalesce(tipoCalculoNf, tipoCalculo)" : "tipoCalculo";
            return ObtemValorCampo<int?>(sessao, campo, "idGrupoProd=" + idGrupoProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <returns></returns>
        public bool ObtemNaoAlterarEstoque(int idGrupoProd)
        {
            return ObtemNaoAlterarEstoque(null, idGrupoProd);
        }

        public bool ObtemNaoAlterarEstoque(GDASession sessao, int idGrupoProd)
        {
            string sql = "select count(*) from grupo_prod where naoAlterarEstoque=true and idGrupoProd=" + idGrupoProd;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        public bool ObtemNaoAlterarEstoqueFiscal(int idGrupoProd)
        {
            return ObtemNaoAlterarEstoqueFiscal(null, idGrupoProd);
        }

        public bool ObtemNaoAlterarEstoqueFiscal(GDASession session, int idGrupoProd)
        {
            string sql = "select count(*) from grupo_prod where naoAlterarEstoqueFiscal=true and idGrupoProd=" + idGrupoProd;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public override int Update(GrupoProd objUpdate)
        {
            throw new NotImplementedException();
            /*LogAlteracaoDAO.Instance.LogGrupoProduto(objUpdate);
            return base.Update(objUpdate);*/
        }

        public override int Delete(GrupoProd objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdGrupoProd);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            throw new NotImplementedException();
            /*if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From produto Where idGrupoProd=" + Key) > 0)
                throw new Exception("Este grupo não pode ser excluído. Existem produtos relacionados ao mesmo.");

            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From subgrupo_prod Where idGrupoProd=" + Key) > 0)
                throw new Exception("Este grupo não pode ser excluído. Existem subgrupos relacionados ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogGrupoProduto(Key);
            return GDAOperations.Delete(new GrupoProd { IdGrupoProd = (int)Key });*/
        }

        /// <summary>
        /// Retorna o código do grupo passado
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <returns></returns>
        public static uint GetGrupoProduto(NomeGrupoProd grupoProduto)
        {
            switch (grupoProduto)
            {
                case NomeGrupoProd.Vidro:
                    return 1;
                default:
                    throw new Exception("Grupo de Produtos não definido.");
            }
        }

        #region Verifica se Grupo é Vidro/Alumínio/Ferragem

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int TipoCalculo(int idProd)
        {
            return TipoCalculo(null, idProd);
        }

        /// <summary>
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int TipoCalculo(GDASession sessao, int idProd)
        {
            return TipoCalculo(sessao, idProd, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="nf"></param>
        /// <returns></returns>
        public int TipoCalculo(int idProd, bool nf)
        {
            return TipoCalculo(null, idProd, nf);
        }

        /// <summary>
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="nf"></param>
        /// <returns></returns>
        public int TipoCalculo(GDASession sessao, int idProd, bool nf)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, idProd);

            return TipoCalculo(sessao, idGrupoProd, idSubgrupoProd, nf);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica o tipo de cálculo que será aplicado no produto
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        public int TipoCalculo(int idGrupo, int? idSubgrupo)
        {
            return TipoCalculo(null, idGrupo, idSubgrupo);
        }

        /// <summary>
        /// Verifica o tipo de cálculo que será aplicado no produto
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        public int TipoCalculo(GDASession sessao, int idGrupo, int? idSubgrupo)
        {
            return TipoCalculo(sessao, idGrupo, idSubgrupo, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica o tipo de cálculo que será aplicado no produto
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        public int TipoCalculo(int idGrupo, int? idSubgrupo, bool nf)
        {
            return TipoCalculo(null, idGrupo, idSubgrupo, nf);
        }

        /// <summary>
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idGrupo">idGrupo.</param>
        /// <param name="idSubgrupo">idSubgrupo.</param>
        /// <param name="nf">nf.</param>
        /// <returns>Retorna o tipo de cálculo que será aplicado no produto.</returns>
        public int TipoCalculo(GDASession session, int idGrupo, int? idSubgrupo, bool nf)
        {
            try
            {
                TipoCalculoGrupoProd? tipoCalculoGrupo = null;
                TipoCalculoGrupoProd? tipoCalculoNfGrupo = null;
                TipoCalculoGrupoProd? tipoCalculoSubgrupo = null;
                TipoCalculoGrupoProd? tipoCalculoNfSubgrupo = null;

                if (idGrupo > 0)
                {
                    tipoCalculoGrupo = (TipoCalculoGrupoProd?)this.ObtemTipoCalculo(session, idGrupo, false);
                    tipoCalculoNfGrupo = (TipoCalculoGrupoProd?)this.ObtemTipoCalculo(session, idGrupo, true);
                }

                if (idSubgrupo > 0)
                {
                    tipoCalculoSubgrupo = (TipoCalculoGrupoProd?)SubgrupoProdDAO.Instance.ObtemTipoCalculo(session, idSubgrupo.Value, false);
                    tipoCalculoNfSubgrupo = (TipoCalculoGrupoProd?)SubgrupoProdDAO.Instance.ObtemTipoCalculo(session, idSubgrupo.Value, true);
                }

                return this.TipoCalculo(
                    idGrupo,
                    idSubgrupo,
                    nf,
                    tipoCalculoGrupo,
                    tipoCalculoNfGrupo,
                    tipoCalculoSubgrupo,
                    tipoCalculoNfSubgrupo);
            }
            catch
            {
                return (int)TipoCalculoGrupoProd.Qtd;
            }
        }

        /// <summary>
        /// Verifica o tipo de cálculo que será aplicado no produto.
        /// </summary>
        /// <param name="idGrupo">idGrupo.</param>
        /// <param name="idSubgrupo">idSubgrupo.</param>
        /// <param name="nf">nf.</param>
        /// <param name="tipoCalculoGrupo">tipoCalculoGrupo.</param>
        /// <param name="tipoCalculoNfGrupo">tipoCalculoNfGrupo.</param>
        /// <param name="tipoCalculoSubgrupo">tipoCalculoSubgrupo.</param>
        /// <param name="tipoCalculoNfSubgrupo">tipoCalculoNfSubgrupo.</param>
        /// <returns>Retorna o tipo de cálculo que será aplicado no produto.</returns>
        public int TipoCalculo(
            int idGrupo,
            int? idSubgrupo,
            bool nf,
            TipoCalculoGrupoProd? tipoCalculoGrupo,
            TipoCalculoGrupoProd? tipoCalculoNfGrupo,
            TipoCalculoGrupoProd? tipoCalculoSubgrupo,
            TipoCalculoGrupoProd? tipoCalculoNfSubgrupo)
        {
            TipoCalculoGrupoProd? tipoCalculo = null;

            if (nf)
            {
                if (idSubgrupo > 0)
                {
                    tipoCalculo = tipoCalculoNfSubgrupo;
                }

                if (!tipoCalculo.HasValue)
                {
                    tipoCalculo = tipoCalculoNfGrupo;
                }
            }

            if (!tipoCalculo.HasValue)
            {
                if (idSubgrupo > 0)
                {
                    tipoCalculo = tipoCalculoSubgrupo;
                }

                if (!tipoCalculo.HasValue)
                {
                    tipoCalculo = tipoCalculoGrupo;
                }
            }

            return (int)tipoCalculo;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o estoque será alterado para o produto.
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        public bool NaoAlterarEstoque(int idGrupo, int? idSubgrupo)
        {
            return NaoAlterarEstoque(null, idGrupo, idSubgrupo);
        }

        /// <summary>
        /// Verifica se o estoque será alterado para o produto.
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="idSubgrupo"></param>
        public bool NaoAlterarEstoque(GDASession sessao, int idGrupo, int? idSubgrupo)
        {
            if (idSubgrupo > 0)
                return SubgrupoProdDAO.Instance.ObtemNaoAlterarEstoque(sessao, idSubgrupo.Value);

            if (idGrupo > 0)
                return GrupoProdDAO.Instance.ObtemNaoAlterarEstoque(sessao, idGrupo);

            return false;
        }

        /// <summary>
        /// Verifica se o estoque fiscal será alterado para o produto.
        /// </summary>
        public bool NaoAlterarEstoqueFiscal(int idGrupo, int? idSubgrupo)
        {
            return NaoAlterarEstoqueFiscal(null, idGrupo, idSubgrupo);
        }

        /// <summary>
        /// Verifica se o estoque fiscal será alterado para o produto.
        /// </summary>
        public bool NaoAlterarEstoqueFiscal(GDASession session, int idGrupo, int? idSubgrupo)
        {
            if (idSubgrupo > 0)
                return SubgrupoProdDAO.Instance.ObtemNaoAlterarEstoqueFiscal(session, idSubgrupo.Value);

            if (idGrupo > 0)
                return GrupoProdDAO.Instance.ObtemNaoAlterarEstoqueFiscal(session, idGrupo);

            return false;
        }

        /// <summary>
        /// Verifica se Grupo passado é Vidro
        /// </summary>
        /// <param name="idGrupo"></param>
        public bool IsVidro(int idGrupo)
        {
            return idGrupo == (int)NomeGrupoProd.Vidro;
        }

        /// <summary>
        /// Verifica se Grupo passado é Alumínio
        /// </summary>
        /// <param name="idGrupo"></param>
        public bool IsAluminio(int idGrupo)
        {
            return idGrupo == (int)NomeGrupoProd.Alumínio;
        }

        /// <summary>
        /// Verifica se Grupo passado é Ferragem
        /// </summary>
        /// <param name="idGrupo"></param>
        public bool IsFerragem(int idGrupo)
        {
            return idGrupo == (int)NomeGrupoProd.Ferragem;
        }

        /// <summary>
        /// Verifica se produto é mão de obra
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public bool IsMaoDeObra(int idGrupo)
        {
            return idGrupo == (int)NomeGrupoProd.MaoDeObra;
        }

        /// <summary>
        /// Verifica se produto é vidro temperado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public bool IsVidroTemperado(GDASession sessao, int idGrupo, int? idSubgrupo)
        {
            return idGrupo == (int)NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.ObtemVidroTemperado(sessao, idSubgrupo.GetValueOrDefault());
        }

        /// <summary>
        /// Retorna os ids de Subgrupo que devem ser marcados
        /// </summary>
        /// <returns></returns>
        public string ObtemSubgruposMarcadosFiltro(GDASession sessao, int idCli)
        {
            return SubgrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(sessao, idCli);
        }

        #endregion

        #region Produto

        public List<int> ObterIdProdGrupoProd(GDASession session, uint idGrupo, uint? idSubGrupoProd)
        {
            var sql = "SELECT IDPROD FROM PRODUTO WHERE IDGRUPOPROD="  + idGrupo;

            if (idSubGrupoProd > 0)
                sql += " AND IDSUBGRUPOPROD= " + idSubGrupoProd;

            return ExecuteMultipleScalar<int>(session, sql);
        }

        #endregion
    }
}
