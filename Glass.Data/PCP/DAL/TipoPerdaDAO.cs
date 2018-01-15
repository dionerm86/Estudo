using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TipoPerdaDAO : BaseDAO<TipoPerda, TipoPerdaDAO>
	{
        //private TipoPerdaDAO() { }

        private string SqlList(bool selecionar, uint idSetor)
        {
            string campos = selecionar ? "tp.*, s.descricao as descrsetor" : "count(*)";

            string sql = "Select " + campos + " From tipo_perda tp left join setor s on tp.IdSetor=s.IdSetor";

            if (idSetor > 0)
            {
                sql += " Where s.idSetor=" + idSetor;
            }

            return sql;
        }

        public string SqlFilter(bool selecionar, uint idSetor)
        {
            string campos = selecionar ? "tp.*, s.descricao as descrsetor" : "count(*)";

            string sql = "Select " + campos + " From tipo_perda tp left join setor s on tp.IdSetor=s.IdSetor Where tp.IdSetor in (" + idSetor + ",0)";

            return sql;
        }

        public IList<TipoPerda> GetList(string sortExpression, int startRow, int pageSize)
        {

            if (GetCountReal() == 0)
            {
                List<TipoPerda> lst = new List<TipoPerda>();
                lst.Add(new TipoPerda());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(true, 0), filtro, startRow, pageSize);
        }

        public TipoPerda[] GetOrderedList()
        {
            return GetOrderedList(0);
        }

        public TipoPerda[] GetOrderedList(int? idSetor)
        {
            string sql = SqlList(true, (uint)idSetor.GetValueOrDefault());
            List<TipoPerda> lstTipoPerda = objPersistence.LoadData(sql).ToList();

            lstTipoPerda.Sort(
                    new Comparison<TipoPerda>(
                            delegate(TipoPerda x, TipoPerda y)
                            {
                                return x.Descricao.CompareTo(y.Descricao);
                            }
                        )
                );

            return lstTipoPerda.ToArray();
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false, 0), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false, 0), null);

            return count == 0 ? 1 : count;
        }

        public string GetNome(uint idTipoPerda)
        {
            return GetNome(null, idTipoPerda);
        }

        public string GetNome(GDASession session, uint idTipoPerda)
        {
            if (idTipoPerda > 0)
            {
                string sql = "select descricao from tipo_perda where idTipoPerda=" + idTipoPerda;
                return objPersistence.ExecuteScalar(session, sql).ToString();
            }
            else
            {
                return "ERRO AO RECUPERAR TIPO DA PERDA";
            }
        }

        public IList<TipoPerda> GetByNome(string nome)
        {
            string sql = "select idTipoPerda from tipo_perda where descricao like ?nome";

            return objPersistence.LoadData(sql, new GDAParameter("?nome", "%" + nome + "%")).ToList();
        }

        public uint GetIDByNomeExato(string nome)
        {
            return ExecuteScalar<uint>("select idTipoPerda from tipo_perda where descricao=?nome", new GDAParameter("?nome", nome));
        }

        /// <summary>
        /// Retorna tipos de perda existentes no pedido
        /// </summary>
        /// <param name="idPedidoRepos"></param>
        /// <returns></returns>
        public TipoPerda[] GetListByPedido(uint idPedidoRepos)
        {
            var tipoPerdas = TipoPerdaDAO.Instance.GetOrderedList();
            var produtos = ProdutosPedidoDAO.Instance.GetByPedido(idPedidoRepos);

            var lstRetorno = new List<TipoPerda>();

            foreach (ProdutosPedido p in produtos)
            {
                try
                {
                    ProdutoPedidoProducao ppp = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(p.NumEtiquetaRepos);
                    if (ppp.TipoPerda == null)
                        continue;

                    foreach (TipoPerda tp in tipoPerdas)
                    {
                        if ((uint)ppp.TipoPerda.Value == tp.IdTipoPerda && !lstRetorno.Contains(tp))
                            lstRetorno.Add(tp);
                    }
                }
                catch { }
            }

            return lstRetorno.ToArray();
        }

        /// <summary>
        /// Retorna tipos de perda identificando qual deve ser marcado no relatório.
        /// </summary>
        /// <param name="idPedidoRepos"></param>
        /// <returns></returns>
        public TipoPerda[] GetByPedidoRepos(uint idPedidoRepos)
        {
            var tipoPerdas = TipoPerdaDAO.Instance.GetOrderedList();
            
            var produtos = ProdutosPedidoDAO.Instance.GetByPedido(idPedidoRepos);

            foreach (ProdutosPedido p in produtos)
            {
                try
                {
                    ProdutoPedidoProducao ppp = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(p.NumEtiquetaRepos);
                    if (ppp.TipoPerda == null)
                        continue;

                    foreach (TipoPerda tp in tipoPerdas)
                    {
                        if ((uint)ppp.TipoPerda.Value == tp.IdTipoPerda)
                        {
                            tp.MarcarRelatorio = true;
                        }
                    }
                }
                catch { }
            }

            return tipoPerdas;
        }

        public IList<TipoPerda> GetBySetor(uint idSetor)
        {
            return objPersistence.LoadData(SqlFilter(true, idSetor)).ToList();
        }

        #region Métodos sobrescritos

        public override int Update(TipoPerda objUpdate)
        {
            LogAlteracaoDAO.Instance.LogTipoPerda(objUpdate);
            return base.Update(objUpdate);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se o tipo de perda esta sendo usado
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_pedido_producao Where tipoPerda=" + Key + " Or tipoPerdaRepos=" + Key) > 0)
                throw new Exception("Não é possível excluir este tipo de perda pois o mesmo está sendo usado em algumas peças da produção.");

            LogAlteracaoDAO.Instance.ApagaLogTipoPerda(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        public override int Delete(TipoPerda objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdTipoPerda);
        }

        #endregion
    }
}