using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class ProdutoTrocaDevolucaoDAO : BaseDAO<ProdutoTrocaDevolucao, ProdutoTrocaDevolucaoDAO>
    {
        //private ProdutoTrocaDevolucaoDAO() { }

        #region Busca padrão

        private string Sql(uint idTrocaDev, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? @"ptd.*, td.idCliente, p.codInterno, p.descricao as descrProduto, ep.codInterno as codProcesso, 
                ea.codInterno as codAplicacao, p.idGrupoProd, p.idSubgrupoProd, um.codigo as unidade, p.custoCompra as custoCompraProduto,
                (td.tipo=" + (int)TrocaDevolucao.TipoTrocaDev.Troca + ") as isTroca, pp.idPedido" : "count(*)";

            string sql = @"
                select " + campos + @"
                from produto_troca_dev ptd
                    left join produtos_pedido pp on (ptd.idProdPed=pp.idProdPed)
                    left join produto p on (ptd.idProd=p.idProd)
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida) 
                    left join etiqueta_processo ep on (ptd.idProcesso=ep.idProcesso)
                    left join etiqueta_aplicacao ea on (ptd.idAplicacao=ea.idAplicacao)
                    left join troca_devolucao td on (ptd.idTrocaDevolucao=td.idTrocaDevolucao)
                where 1";

            if (idTrocaDev > 0)
            {
                sql += " and ptd.idTrocaDevolucao=" + idTrocaDev;
                temFiltro = true;
            }

            return sql;
        }

        public IList<ProdutoTrocaDevolucao> GetList(uint idTrocaDev, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idTrocaDev) == 0)
                return new ProdutoTrocaDevolucao[] { new ProdutoTrocaDevolucao() };

            bool temFiltro;
            return LoadDataWithSortExpression(Sql(idTrocaDev, true, out temFiltro), sortExpression, startRow, pageSize, temFiltro);
        }

        public int GetCount(uint idTrocaDev)
        {
            int retorno = GetCountReal(idTrocaDev);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idTrocaDev)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(idTrocaDev, true, out temFiltro), temFiltro);
        }

        public IList<ProdutoTrocaDevolucao> GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            return GetByTrocaDevolucao(null, idTrocaDevolucao);
        }

        public IList<ProdutoTrocaDevolucao> GetByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            bool temFiltro;
            return objPersistence.LoadData(session, Sql(idTrocaDevolucao, true, out temFiltro)).ToList();
        }

        #endregion

        #region Atualiza o valor do beneficiamento do produto

        /// <summary>
        /// Atualiza o valor do beneficiamento do produto.
        /// </summary>
        public void UpdateValorBenef(uint idProdTrocaDev)
        {
            UpdateValorBenef(null, idProdTrocaDev);
        }

        /// <summary>
        /// Atualiza o valor do beneficiamento do produto.
        /// </summary>
        public void UpdateValorBenef(GDASession session, uint idProdTrocaDev)
        {
            var idProd = ObtemValorCampo<int>(session, "idProd", "idProdTrocaDev=" + idProdTrocaDev);
            if (Glass.Configuracoes.Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(session, idProd))
                return;

            objPersistence.ExecuteCommand(session, "update produto_troca_dev ptd set valorBenef=(select sum(coalesce(valor,0)) from " +
                "produto_troca_dev_benef where idProdTrocaDev=ptd.idProdTrocaDev) where idProdTrocaDev=" + idProdTrocaDev);

            // Recalcula o total bruto/valor unitário bruto
            ProdutoTrocaDevolucao pt = GetElementByPrimaryKey(session, idProdTrocaDev);
            UpdateBase(session, pt);
        }

        #endregion

        #region Apaga os produtos de uma troca/devolução

        public void DeleteByTrocaDevolucao(uint idTrocaDevolucao)
        {
            DeleteByTrocaDevolucao(null, idTrocaDevolucao);
        }

        public void DeleteByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            objPersistence.ExecuteCommand(session, "delete from produto_troca_dev where idTrocaDevolucao=" + idTrocaDevolucao);
        }

        #endregion

        #region Busca dados do produto troca/devolução

        public decimal ObterQtde(GDASession session, int idProdTrocaDev)
        {
            return ObtemValorCampo<decimal>(session, "Qtde", string.Format("IdProdTrocaDev={0}", idProdTrocaDev));
        }

        #endregion

        #region Métodos sobrescritos

        public uint InsertComTransacao(ProdutoTrocaDevolucao objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);
                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override uint Insert(ProdutoTrocaDevolucao objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutoTrocaDevolucao objInsert)
        {
            if (TrocaDevolucaoDAO.Instance.IsDevolucao(session, objInsert.IdTrocaDevolucao))
                return 0;

            if (objInsert.IdProd > 0)
            {
                uint idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente(session, objInsert.IdTrocaDevolucao);
                float altura = objInsert.Altura, totM2 = objInsert.TotM, totM2Calc = objInsert.TotM2Calc;
                decimal custo = objInsert.CustoProd, total = objInsert.Total;

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, idCliente, (int)objInsert.IdProd, objInsert.Largura,
                    objInsert.Qtde, 1, objInsert.ValorVendido, objInsert.Espessura, objInsert.Redondo, 2, false, true, ref custo,
                    ref altura, ref totM2, ref totM2Calc, ref total, false, objInsert.Beneficiamentos.CountAreaMinimaSession(session));

                objInsert.CustoProd = custo;
                objInsert.Altura = altura;
                objInsert.TotM = totM2;
                objInsert.TotM2Calc = totM2Calc;
                objInsert.Total = total;
            }

            CalculaDescontoEValorBrutoProduto(session, objInsert);

            uint retorno = base.Insert(session, objInsert);

            ProdutoTrocaDevolucaoBenefDAO.Instance.DeleteByProdutoTrocaDev(session, retorno);
            foreach (ProdutoTrocaDevolucaoBenef p in objInsert.Beneficiamentos.ToProdutosTrocaDevolucao(retorno))
                ProdutoTrocaDevolucaoBenefDAO.Instance.Insert(session, p);

            UpdateValorBenef(session, retorno);
            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, objInsert.IdTrocaDevolucao);
            return retorno;
        }

        public uint InsertFromPedido(int idTrocaDev, int idProdPed, decimal qtde)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    ProdutosPedido prodPed = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(transaction, idProdPed);
                    Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodPed.IdPedido);
                    var qtdeOriginal = prodPed.Qtde;
                    List<ProdutoTrocadoBenef> lstProdTrocBenef = new List<ProdutoTrocadoBenef>();

                    // Se a quantidade disponível para ser trocada for diferente da qtd do produto recalcula o total e os beneficiamentos
                    if (prodPed.Qtde != (float)qtde)
                    {
                        // Recalcula o metro quadrado
                        prodPed.TotM = (prodPed.TotM / prodPed.Qtde) * (float)qtde;
                        prodPed.TotM2Calc = prodPed.TotM2Calc > 0 ? (prodPed.TotM2Calc / prodPed.Qtde) * (float)qtde : prodPed.TotM;

                        prodPed.Qtde = (float)qtde;
                        int tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdProd);

                        if (tipoCalc == (uint)TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)TipoCalculoGrupoProd.QtdM2 || tipoCalc == (uint)TipoCalculoGrupoProd.QtdDecimal)
                            prodPed.Total = (decimal)prodPed.Qtde * prodPed.ValorVendido;
                        /* Chamado 62311. */
                        else if (prodPed.TotM2Calc > 0)
                            prodPed.Total = (decimal)prodPed.TotM2Calc * prodPed.ValorVendido;
                        else if (tipoCalc == (int)TipoCalculoGrupoProd.ML)
                            prodPed.Total = prodPed.ValorVendido * (decimal)(prodPed.Altura * prodPed.Qtde);
                        else if (prodPed.Altura > 0)
                            prodPed.Total = (prodPed.ValorVendido * (decimal)(prodPed.Altura * prodPed.Qtde)) / 6;
                        else
                            prodPed.Total = prodPed.ValorVendido * (decimal)prodPed.Qtde;

                        // Recalcula o valor dos beneficiamentos para considerar apenas a quantidade a ser trocada escolhido
                        foreach (ProdutoTrocadoBenef ptb in prodPed.Beneficiamentos.ToProdutosTrocado())
                        {
                            ptb.Valor = (ptb.Valor / (decimal)qtdeOriginal) * qtde;
                            lstProdTrocBenef.Add(ptb);
                        }
                    }
                    else
                        // Se a quantidade a ser trocada for a mesma quantidade do produto original, insere todos os beneficiamentos sem recalcular
                        lstProdTrocBenef = prodPed.Beneficiamentos;

                    ProdutoTrocaDevolucao objInsert = new ProdutoTrocaDevolucao();
                    objInsert.IdTrocaDevolucao = (uint)idTrocaDev;
                    objInsert.IdProdPed = (uint)idProdPed;
                    objInsert.IdProd = prodPed.IdProd;
                    objInsert.Qtde = (float)qtde;
                    objInsert.Altura = prodPed.Altura;
                    objInsert.AlturaReal = prodPed.AlturaReal;
                    objInsert.Beneficiamentos = lstProdTrocBenef;
                    objInsert.CustoProd = prodPed.CustoProd;
                    objInsert.Espessura = prodPed.Espessura;
                    objInsert.IdAplicacao = prodPed.IdAplicacao;
                    objInsert.IdProcesso = prodPed.IdProcesso;
                    objInsert.Largura = prodPed.Largura;
                    objInsert.PedCli = prodPed.PedCli;
                    objInsert.PercDescontoQtde = prodPed.PercDescontoQtde;
                    objInsert.Redondo = prodPed.Redondo;
                    objInsert.Total = prodPed.Total;
                    objInsert.TotM = prodPed.TotM;
                    objInsert.TotM2Calc = prodPed.TotM2Calc;
                    objInsert.ValorAcrescimo = prodPed.ValorAcrescimo;
                    objInsert.ValorAcrescimoProd = prodPed.ValorAcrescimoProd;
                    objInsert.ValorDesconto = prodPed.ValorDesconto;
                    objInsert.ValorDescontoProd = prodPed.ValorDescontoProd;
                    objInsert.ValorDescontoQtde = prodPed.ValorDescontoQtde;
                    objInsert.ValorVendido = prodPed.ValorVendido;
                    objInsert.AlterarEstoque = true;
                    objInsert.ValorDescontoCliente = prodPed.ValorDescontoCliente;
                    objInsert.ValorAcrescimoCliente = prodPed.ValorAcrescimoCliente;
                    objInsert.ValorUnitarioBruto = prodPed.ValorUnitarioBruto;
                    objInsert.TotalBruto = prodPed.TotalBruto;
                    objInsert.ValorBenef = prodPed.ValorBenef;

                    // Soma o ICMS e IPI ao produto da troca, caso o pedido tenha cobrado
                    if (ped.ValorIcms > 0 || ped.ValorIpi > 0)
                    {
                        if (ped.ValorIcms > 0)
                            objInsert.Total += prodPed.ValorIcms / (decimal)prodPed.Qtde * (decimal)objInsert.Qtde;

                        if (ped.ValorIpi > 0)
                            objInsert.Total += prodPed.ValorIpi / (decimal)prodPed.Qtde * (decimal)objInsert.Qtde;

                        decimal valorUnit = 0;
                        CalculosFluxo.CalcValorUnitItemProd(transaction, ped.IdCli, (int)prodPed.IdProd, objInsert.Largura,
                            objInsert.Qtde, objInsert.QtdeAmbiente, objInsert.Total,
                            objInsert.Espessura, objInsert.Redondo, 1, false, true, objInsert.Altura, objInsert.TotM,
                            ref valorUnit, objInsert.Beneficiamentos.CountAreaMinimaSession(transaction), 0, 0);

                        objInsert.ValorVendido = valorUnit;
                    }

                    if (objInsert.IdProdPed == null)
                        return 0;

                    objInsert.IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(transaction, objInsert.IdProdPed.Value);

                    if (!PedidoConfig.RatearDescontoProdutos && objInsert.IdPedido > 0)
                    {
                        float percDesc = PedidoDAO.Instance.GetPercDesc(transaction, objInsert.IdPedido.Value);

                        if (percDesc > 0)
                        {
                            int tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)objInsert.IdProd);

                            objInsert.Total -= (objInsert.Total + objInsert.ValorBenef) * (decimal)percDesc;

                            if (tipoCalc == (uint)TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)TipoCalculoGrupoProd.QtdM2 || tipoCalc == (uint)TipoCalculoGrupoProd.QtdDecimal)
                                objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.Qtde;
                            else if (objInsert.TotM2Calc > 0)
                                objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.TotM2Calc;
                            else if (tipoCalc == (int)TipoCalculoGrupoProd.ML)
                                objInsert.ValorVendido = objInsert.Total / (decimal)(objInsert.Altura * objInsert.Qtde);
                            else if (objInsert.Altura > 0)
                                objInsert.ValorVendido = (objInsert.Total * 6) / (decimal)(objInsert.Altura * objInsert.Qtde);
                            else
                                objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.Qtde;
                        }
                    }

                    uint retorno = base.Insert(transaction, objInsert);

                    ProdutoTrocaDevolucaoBenefDAO.Instance.DeleteByProdutoTrocaDev(transaction, retorno);
                    foreach (ProdutoTrocaDevolucaoBenef p in objInsert.Beneficiamentos.ToProdutosTrocaDevolucao(retorno)
                        )
                        ProdutoTrocaDevolucaoBenefDAO.Instance.Insert(transaction, p);

                    UpdateValorBenef(transaction, retorno);
                    TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(transaction, objInsert.IdTrocaDevolucao);

                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Delete(ProdutoTrocaDevolucao objDelete)
        {
            uint idTrocaDevolucao = ObtemValorCampo<uint>("idTrocaDevolucao", "idProdTrocaDev=" + objDelete.IdProdTrocaDev);

            ProdutoTrocaDevolucaoBenefDAO.Instance.DeleteByProdutoTrocaDev(objDelete.IdProdTrocaDev);
            int retorno = base.Delete(objDelete);

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(idTrocaDevolucao);
            return retorno;
        }

        internal int UpdateBase(ProdutoTrocaDevolucao objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        internal int UpdateBase(GDASession session, ProdutoTrocaDevolucao objUpdate)
        {
            CalculaDescontoEValorBrutoProduto(session, objUpdate);
            return base.Update(session, objUpdate);
        }

        public int UpdateComTransacao(ProdutoTrocaDevolucao objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);
                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Update(ProdutoTrocaDevolucao objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, ProdutoTrocaDevolucao objUpdate)
        {
            if (TrocaDevolucaoDAO.Instance.IsDevolucao(session, objUpdate.IdTrocaDevolucao))
                return 0;

            if (objUpdate.IdProd > 0)
            {
                uint idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente(session, objUpdate.IdTrocaDevolucao);
                float altura = objUpdate.Altura, totM2 = objUpdate.TotM, totM2Calc = objUpdate.TotM2Calc;
                decimal custo = objUpdate.CustoProd, total = objUpdate.Total;

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, idCliente, (int)objUpdate.IdProd,
                    objUpdate.Largura, objUpdate.Qtde, 1, objUpdate.ValorVendido, objUpdate.Espessura, objUpdate.Redondo, 2,
                    false, true, ref custo, ref altura, ref totM2, ref totM2Calc, ref total, false,
                    objUpdate.Beneficiamentos.CountAreaMinimaSession(session));

                objUpdate.CustoProd = custo;
                objUpdate.Altura = altura;
                objUpdate.TotM = totM2;
                objUpdate.TotM2Calc = totM2Calc;
                objUpdate.Total = total;
            }
            else
                throw new System.Exception("Informe o código de produto associado ao produto novo.");

            /* Chamado 44968. */
            if (objUpdate.IdProd != ObtemValorCampo<int>("IdProd", "IdProdTrocaDev=" + objUpdate.IdProdTrocaDev))
                throw new System.Exception("Não é permitido alterar o produto caso já tenha sido inserido. Apague-o e insira novamente.");

            int retorno = UpdateBase(session, objUpdate);

            ProdutoTrocaDevolucaoBenefDAO.Instance.DeleteByProdutoTrocaDev(session, objUpdate.IdProdTrocaDev);
            foreach (ProdutoTrocaDevolucaoBenef p in objUpdate.Beneficiamentos.ToProdutosTrocaDevolucao(objUpdate.IdProdTrocaDev))
                ProdutoTrocaDevolucaoBenefDAO.Instance.Insert(session, p);

            UpdateValorBenef(session, objUpdate.IdProdTrocaDev);
            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, objUpdate.IdTrocaDevolucao);
            return retorno;
        }

        #endregion

        private void CalculaDescontoEValorBrutoProduto(GDASession session, ProdutoTrocaDevolucao produto)
        {
            var pedido = produto.IdPedido.HasValue
                ? PedidoDAO.Instance.GetElementByPrimaryKey(session, produto.IdPedido.Value)
                : null;

            DescontoAcrescimo.Instance.RemoveDescontoQtde(produto, pedido);
            DescontoAcrescimo.Instance.AplicaDescontoQtde(produto, pedido);
            DiferencaCliente.Instance.Calcular(produto, pedido);
            ValorBruto.Instance.Calcular(produto, pedido);
        }
    }
}
