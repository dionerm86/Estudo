using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class SaidaEstoqueDAO : BaseDAO<SaidaEstoque, SaidaEstoqueDAO>
    {
        #region Busca padrão

        private string Sql(uint idSaidaEstoque, uint idPedido, uint idLiberarPedido, uint idVolume,
            uint idFunc, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "se.*, f.nome as nomeFunc, '$$$' as criterio" : "count(*)";
            string criterio = "";

            string sql = @"
                select " + campos + @"
                from saida_estoque se
                    left join funcionario f on (se.idFunc=f.idFunc)
                where 1";

            if (idSaidaEstoque > 0)
            {
                sql += " and se.idSaidaEstoque=" + idSaidaEstoque;
                criterio += "Código: " + idSaidaEstoque + "    ";
            }

            if (idPedido > 0)
            {
                sql += " and se.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                sql += " and se.idLiberarPedido=" + idLiberarPedido;
                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            if (idVolume > 0)
            {
                sql += " and se.idVolume=" + idVolume;
                criterio += "Volume: " + idVolume + "    ";
            }

            if (idFunc > 0)
            {
                sql += " and se.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and se.dataCad>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and se.dataCad<=?dataFim";
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

        public IList<SaidaEstoque> GetList(uint idPedido, uint idLiberarPedido, uint idVolume,
            uint idFunc, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "se.DataCad desc";
            return LoadDataWithSortExpression(Sql(0, idPedido, idLiberarPedido, idVolume,
                idFunc, dataIni, dataFim, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(uint idPedido, uint idLiberarPedido, uint idVolume, uint idFunc, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido, idLiberarPedido, idVolume,
                idFunc, dataIni, dataFim, false), GetParams(dataIni, dataFim));
        }

        public IList<SaidaEstoque> GetForRpt(uint idPedido, uint idLiberarPedido, uint idVolume, uint idFunc, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(0, idPedido, idLiberarPedido, idVolume, idFunc, dataIni, dataFim, true) + " order by se.DataCad desc", 
                GetParams(dataIni, dataFim)).ToList();
        }

        public SaidaEstoque GetElement(uint idSaidaEstoque)
        {
            return objPersistence.LoadOneData(Sql(idSaidaEstoque, 0, 0, 0, 0, null, null, true));
        }

        public SaidaEstoque GetByLiberacao(uint idLiberarPedido)
        {
            return GetByLiberacao(null, idLiberarPedido);
        }

        public SaidaEstoque GetByLiberacao(GDASession sessao, uint idLiberarPedido)
        {
            List<SaidaEstoque> saidas = objPersistence.LoadData(sessao, Sql(0, 0, idLiberarPedido, 0, 0, null, null, true));
            return saidas.Count > 0 ? saidas[0] : null;
        }

        public SaidaEstoque GetByVolume(uint idVolume)
        {
            List<SaidaEstoque> saidas = objPersistence.LoadData(Sql(0, 0, 0, idVolume, 0, null, null, true));
            return saidas.Count > 0 ? saidas[0] : null;
        }

        public SaidaEstoque GetByPedido(uint idPedido)
        {
            List<SaidaEstoque> saidas = objPersistence.LoadData(Sql(0, idPedido, 0, 0, 0, null, null, true));
            return saidas.Count > 0 ? saidas[0] : null;
        }

        #endregion

        #region Cria uma nova saída

        public uint GetNewSaidaEstoque(uint idLoja, uint? idPedido, uint? idLiberarPedido, uint? idVolume, bool manual)
        {
            return GetNewSaidaEstoque(null, idLoja, idPedido, idLiberarPedido, idVolume, manual);
        }

        public uint GetNewSaidaEstoque(GDASession sessao, uint idLoja, uint? idPedido, uint? idLiberarPedido, uint? idVolume, bool manual)
        {
            SaidaEstoque nova = new SaidaEstoque();
            nova.IdLoja = idLoja;
            nova.IdPedido = idPedido;
            nova.IdLiberarPedido = idLiberarPedido;
            nova.IdVolume = idVolume;
            nova.IdFunc = UserInfo.GetUserInfo.CodUser;
            nova.DataCad = DateTime.Now;
            nova.Manual = manual;

            return Insert(sessao, nova);
        }

        #endregion

        #region Marca estorno da saída de estoque

        /// <summary>
        /// Marca o estorno da saída de estoque
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idVolume"></param>
        public void MarcaEstorno(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint? idVolume)
        {
            if (!idPedido.HasValue && !idLiberarPedido.HasValue && !idVolume.HasValue)
                throw new Exception("Nenhum identificador for informado.");

            string sql = @"UPDATE saida_estoque SET estornado=true WHERE 1";

            if (idPedido.HasValue)
                sql += " AND idPedido=" + idPedido.Value;

            if (idLiberarPedido.HasValue)
                sql += " AND idLiberarPedido=" + idLiberarPedido.Value;

            if (idVolume.HasValue)
                sql += " AND idVolume=" + idVolume.Value;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Cancela uma saída de estoque

        private static readonly object _cancelarLock = new object();

        /// <summary>
        /// Cancela uma saída de estoque.
        /// </summary>
        public void Cancelar(uint idSaidaEstoque)
        {
            lock(_cancelarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var saida = GetElementByPrimaryKey(transaction, idSaidaEstoque);

                        if (!saida.PodeCancelar)
                            throw new Exception("Não é possível cancelar essa saída de estoque.");

                        MarcaEstorno(transaction, saida.IdPedido, saida.IdLiberarPedido, saida.IdVolume);

                        var produtos = ProdutoSaidaEstoqueDAO.Instance.GetForRpt(transaction, idSaidaEstoque);

                        var idsProdQtde = new Dictionary<int, float>();
                        var lstIdsPedidos = new List<uint>();

                        var tipoCalcMLAL = new List<int>
                        {
                            (int) TipoCalculoGrupoProd.MLAL0,
                            (int) TipoCalculoGrupoProd.MLAL05,
                            (int) TipoCalculoGrupoProd.MLAL1,
                            (int) TipoCalculoGrupoProd.MLAL6,
                            (int) TipoCalculoGrupoProd.ML
                        };

                        foreach (var prod in produtos)
                        {
                            var prodPed = ProdutosPedidoDAO.Instance.GetElementFluxoLite(transaction, prod.IdProdPed);
                            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdGrupoProd,
                                (int?)prodPed.IdSubgrupoProd);

                            if (!lstIdsPedidos.Contains(prodPed.IdPedido))
                                lstIdsPedidos.Add(prodPed.IdPedido);

                            var qtdSaidaEstoque = prod.QtdeSaida;
                            if (tipoCalcMLAL.Contains(tipoCalculo))
                                qtdSaidaEstoque *= prodPed.Altura;

                            // Marca a entrada dos produtos no pedido
                            ProdutosPedidoDAO.Instance.MarcarSaida(transaction, prod.IdProdPed, -prod.QtdeSaida, idSaidaEstoque);

                            // Faz a movimentação de estorno no estoque
                            if (saida.IdPedido > 0)
                            {
                                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(transaction, (int)prodPed.IdProd);

                                MovEstoqueDAO.Instance.CreditaEstoquePedido(transaction, prodPed.IdProd, saida.IdLoja,
                                    saida.IdPedido.Value,
                                    prodPed.IdProdPed,
                                    (decimal)qtdSaidaEstoque,
                                    tipoSubgrupo != TipoSubgrupoProd.ChapasVidro &&
                                    tipoSubgrupo != TipoSubgrupoProd.ChapasVidroLaminado);
                            }
                            else if (saida.IdLiberarPedido > 0)
                            {
                                MovEstoqueDAO.Instance.CreditaEstoqueLiberacao(transaction, prodPed.IdProd, saida.IdLoja,
                                    saida.IdLiberarPedido.Value, prodPed.IdPedido,
                                    ProdutosLiberarPedidoDAO.Instance.ObtemIdProdLiberarPedido(transaction, saida.IdLiberarPedido.Value,
                                        prodPed.IdProdPed), (decimal)qtdSaidaEstoque);
                            }

                            if (!PedidoDAO.Instance.IsProducao(transaction, prodPed.IdPedido))
                            {
                                if (!idsProdQtde.ContainsKey((int)prodPed.IdProd))
                                    idsProdQtde.Add((int)prodPed.IdProd, qtdSaidaEstoque);
                                else
                                    idsProdQtde[(int)prodPed.IdProd] += qtdSaidaEstoque;
                            }
                        }

                        if (!PedidoConfig.LiberarPedido)
                            ProdutoLojaDAO.Instance.ColocarReserva(transaction, (int)saida.IdLoja, idsProdQtde, (int)idSaidaEstoque, null,
                                null, null, null, null, null, "SaidaEstoqueDAO - Cancelar");
                        else
                            ProdutoLojaDAO.Instance.ColocarLiberacao(transaction, (int)saida.IdLoja, idsProdQtde, (int)idSaidaEstoque, null,
                                null, null, null, null, null, "SaidaEstoqueDAO - Cancelar");

                        foreach (var idPedido in lstIdsPedidos)
                            PedidoDAO.Instance.AtualizaSituacaoProducao(transaction, idPedido, null, DateTime.Now);
                        
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw ex;
                    }
                }
            }
        }

        #endregion
    }
}
