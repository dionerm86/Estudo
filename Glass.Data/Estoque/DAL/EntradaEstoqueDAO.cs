using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class EntradaEstoqueDAO : BaseDAO<EntradaEstoque, EntradaEstoqueDAO>
    {
        //private EntradaEstoqueDAO() { }

        #region Busca padrão

        private string Sql(uint idEntradaEstoque, uint idCompra, uint numeroNfe, uint idFunc, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "ee.*, f.nome as nomeFunc, '$$$' as criterio" : "count(*)";
            string criterio = "";

            string sql = @"
                select " + campos + @"
                from entrada_estoque ee
                    left join funcionario f on (ee.idFunc=f.idFunc)
                where 1";

            if (idEntradaEstoque > 0)
            {
                sql += " and ee.idEntradaEstoque=" + idEntradaEstoque;
                criterio += "Código: " + idEntradaEstoque + "    ";
            }

            if (idCompra > 0)
            {
                sql += " and ee.idCompra=" + idCompra;
                criterio += "Compra: " + idCompra + "    ";
            }

            if (numeroNfe > 0)
            {
                sql += " and ee.numeroNfe=" + numeroNfe;
                criterio += "Nota Fiscal: " + numeroNfe + "    ";
            }

            if (idFunc > 0)
            {
                sql += " and ee.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and ee.dataCad>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and ee.dataCad<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        public IList<EntradaEstoque> GetList(uint idCompra, uint numeroNfe, uint idFunc, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "ee.DataCad desc";
            return LoadDataWithSortExpression(Sql(0, idCompra, numeroNfe, idFunc, dataIni, dataFim, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(uint idCompra, uint numeroNfe, uint idFunc, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idCompra, numeroNfe, idFunc, dataIni, dataFim, false), GetParams(dataIni, dataFim));
        }

        public IList<EntradaEstoque> GetForRpt(uint idCompra, uint numeroNfe, uint idFunc, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(0, idCompra, numeroNfe, idFunc, dataIni, dataFim, true) + " order by ee.DataCad desc", 
                GetParams(dataIni, dataFim)).ToList();
        }

        public EntradaEstoque GetElement(uint idEntradaEstoque)
        {
            return objPersistence.LoadOneData(Sql(idEntradaEstoque, 0, 0, 0, null, null, true));
        }

        #endregion

        #region Cria uma nota entrada

        public uint GetNewEntradaEstoque(uint idLoja, uint? idCompra, uint? numeroNFe, bool manual)
        {
            return GetNewEntradaEstoque(null, idLoja, idCompra, numeroNFe, manual, (int)UserInfo.GetUserInfo.CodUser);
        }

        public uint GetNewEntradaEstoque(GDASession session, uint idLoja, uint? idCompra, uint? numeroNFe, bool manual,
            int idFunc)
        {
            EntradaEstoque nova = new EntradaEstoque();
            nova.IdLoja = idLoja;
            nova.IdCompra = idCompra;
            nova.NumeroNFe = numeroNFe;
            nova.IdFunc = (uint)idFunc;
            nova.DataCad = DateTime.Now;
            nova.Manual = manual;

            return Insert(session, nova);
        }

        #endregion

        #region Marca estorno da entrada de estoque

        /// <summary>
        /// Marca o estorno da entrada de estoque
        /// </summary>
        public void MarcaEstorno(GDASession session, uint? idCompra, uint? numeroNFe)
        {
            if (!idCompra.HasValue && !numeroNFe.HasValue)
                throw new Exception("Nenhum identificador for informado.");

            string sql = @"UPDATE entrada_estoque SET estornado=true WHERE 1";

            if (idCompra.HasValue)
                sql += " AND idCompra=" + idCompra.Value;

            if (numeroNFe.HasValue)
                sql += " AND numeroNFe=" + numeroNFe.Value;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Cancela uma entrada de estoque

        static volatile object _cancelarEntradaEstoqueLock = new object();

        /// <summary>
        /// Cancela uma entrada de estoque.
        /// </summary>
        public void Cancelar(uint idEntradaEstoque)
        {
            lock(_cancelarEntradaEstoqueLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var entrada = GetElementByPrimaryKey(transaction, idEntradaEstoque);

                        if (!entrada.PodeCancelar)
                            throw new Exception("Não é possível cancelar essa entrada de estoque.");

                        MarcaEstorno(transaction, entrada.IdCompra, entrada.NumeroNFe);

                        var produtos = ProdutoEntradaEstoqueDAO.Instance.GetForRpt(transaction, idEntradaEstoque);

                        var tipoCalcM2 = new List<int> { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto };
                        var tipoCalcMLAL = new List<int> { (int)TipoCalculoGrupoProd.MLAL0, (int)TipoCalculoGrupoProd.MLAL05,
                        (int)TipoCalculoGrupoProd.MLAL1, (int)TipoCalculoGrupoProd.MLAL6, (int)TipoCalculoGrupoProd.ML };

                        foreach (var prod in produtos)
                        {
                            var prodCompra = prod.IdProdCompra > 0 ? ProdutosCompraDAO.Instance.GetElementByPrimaryKey(transaction, prod.IdProdCompra.Value) : null;
                            var prodNf = prod.IdProdNf > 0 ? ProdutosNfDAO.Instance.GetElementByPrimaryKey(transaction, prod.IdProdNf.Value) : null;

                            int tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, prodCompra != null ? (int)prodCompra.IdProd : (int)prodNf.IdProd);

                            // Remove a peça da reserva e a coloca na liberação
                            float m2Calc = prodCompra != null ?
                                Global.CalculosFluxo.ArredondaM2(transaction, prodCompra.Largura, (int)prodCompra.Altura, prod.QtdeEntrada, 0, prodCompra.Redondo, 0,
                                tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto) :
                                Global.CalculosFluxo.ArredondaM2(transaction, prodNf.Largura, (int)prodNf.Altura, prod.QtdeEntrada, 0, false, 0,
                                tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                            bool m2 = tipoCalcM2.Contains(tipoCalculo);

                            float qtdEntradaEstoque = prod.QtdeEntrada;
                            if (tipoCalcMLAL.Contains(tipoCalculo))
                                qtdEntradaEstoque *= prodCompra != null ? prodCompra.Altura : prodNf.Altura;

                            // Faz a movimentação de estorno no estoque
                            if (entrada.IdCompra > 0)
                            {
                                ProdutosCompraDAO.Instance.MarcarEntrada(transaction, prod.IdProdCompra.Value, -prod.QtdeEntrada, idEntradaEstoque);

                                MovEstoqueDAO.Instance.BaixaEstoqueCompra(transaction, prodCompra.IdProd, entrada.IdLoja, prodCompra.IdCompra,
                                    prodCompra.IdProdCompra, (decimal)qtdEntradaEstoque);
                            }
                            else if (entrada.NumeroNFe > 0)
                            {
                                ProdutosNfDAO.Instance.MarcarEntrada(transaction, prod.IdProdNf.Value, -prod.QtdeEntrada, idEntradaEstoque);

                                MovEstoqueDAO.Instance.BaixaEstoqueNotaFiscal(transaction, prodNf.IdProd, entrada.IdLoja, prodNf.IdNf,
                                    prodNf.IdProdNf, (decimal)qtdEntradaEstoque);
                            }
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("CancelarEntradaEstoque", ex, idEntradaEstoque);
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Verificar nota possui entrada estoque ativa

        /// <summary>
        /// Verifica se a nota fiscal possui alguma entrada de estoque ativa.
        /// </summary>
        public bool VerificarNotaFiscalPossuiEntradaEstoqueAtiva(GDASession session, int idNf)
        {
            var sql = string.Format(@"SELECT COUNT(*)>0 FROM entrada_estoque ee
	                INNER JOIN produto_entrada_estoque pee ON (ee.IdEntradaEstoque=pee.IdEntradaEstoque)
	                INNER JOIN produtos_nf pnf ON (pee.IdProdNf=pnf.IdProdNf)
                WHERE pnf.IdNf={0} AND (ee.Estornado IS NULL OR ee.Estornado=0);", idNf);

            return ExecuteScalar<bool>(session, sql);
        }

        #endregion
    }
}
