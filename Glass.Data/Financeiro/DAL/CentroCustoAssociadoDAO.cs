using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class CentroCustoAssociadoDAO : BaseDAO<CentroCustoAssociado, CentroCustoAssociadoDAO>
    {
        #region Busca padrão

        private string Sql(int idCentroCusto, int idCompra, int idImpostoServ, int idNf, int idContaPg, bool selecionar)
        {
            var campos = selecionar ? "cca.*, cc.Descricao as DescricaoCentroCusto" : "count(*)";

            var sql = @"
                SELECT " + campos + @"
                FROM centro_custo_associado cca
                    INNER JOIN centro_custo cc ON (cca.IdCentroCusto = cc.IdCentroCusto)
                WHERE 1";

            if (idCompra > 0)
                sql += " AND cca.IdCompra = " + idCompra;
            else
                sql += " AND COALESCE(cc.Tipo, 0) <> " + (int)TipoCentroCusto.Estoque;

            if (idImpostoServ > 0)
                sql += " AND cca.IdImpostoServ = " + idImpostoServ;

            if (idNf > 0)
                sql += " AND (cca.IdNf = " + idNf + " OR cca.IdCompra IN(SELECT idCompra from compra_nota_fiscal WHERE idNf = " + idNf + "))";

            if (idContaPg > 0)
                sql += " AND cca.IdContaPg = " + idContaPg;

            if (idCentroCusto > 0)
                sql += " AND cca.IdCentroCusto = " + idCentroCusto;

            return sql;
        }

        /// <summary>
        /// Busca os centros de custo
        /// </summary>
        /// <param name="idCompra"></param>
        /// <param name="idImpostoServ"></param>
        /// <param name="idNf"></param>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public IList<CentroCustoAssociado> ObtemDadosCentroCusto(int idCompra, int idImpostoServ, int idNf, int idContaPg)
        {
            if (idCompra == 0 && idImpostoServ == 0 && idNf == 0 && idContaPg == 0)
                return new List<CentroCustoAssociado>() { new CentroCustoAssociado() };

            var retorno = objPersistence.LoadData(Sql(0, idCompra, idImpostoServ, idNf, idContaPg, true)).ToList();

            if (retorno.Count == 0 && CentroCustoDAO.Instance.GetCountReal() > 0)
                retorno.Add(new CentroCustoAssociado());

            return retorno;
        }

        public int ObtemDadosCentroCustoCount(int idCompra, int idImportoServ, int idNf, int idContaPg)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idCompra, idImportoServ, idNf, idContaPg, false));
        }

        public DetalhesCentroCustoAssociado ObtemDetalhesCentroCustoAssociado(int idCompra, int idImpostoServ, int idNf, int idContaPg)
        {
            var retorno = new DetalhesCentroCustoAssociado();

            if (idCompra > 0)
            {
                retorno.IdCompra = idCompra;
                retorno.ValorAssociacao = CompraDAO.Instance.ObtemValorCampo<decimal>("Total", "IdCompra = " + idCompra);
                retorno.ValorTotal = ObtemTotalPorCompra(idCompra);
            }
            else if (idImpostoServ > 0)
            {
                retorno.IdImpostoServ = idImpostoServ;
                retorno.ValorAssociacao = ImpostoServDAO.Instance.ObtemValorCampo<decimal>("Total", "idImpostoServ = " + idImpostoServ);
                retorno.ValorTotal = ObtemTotalPorImpostoServ(idImpostoServ);
            }
            else if (idNf > 0)
            {
                retorno.IdNf = idNf;
                retorno.ValorAssociacao = NotaFiscalDAO.Instance.ObtemTotal((uint)idNf);
                retorno.ValorTotal = ObtemTotalPorNotaFiscal(idNf);
            }
            else if (idContaPg > 0)
            {
                retorno.IdContaPg = idContaPg;
                retorno.ValorAssociacao = ContasPagarDAO.Instance.ObtemValorCampo<decimal>("valorvenc", "idcontapg = " + idContaPg);
                retorno.ValorTotal = ObtemTotalPorContaPagar(idContaPg);
            }

            return retorno;
        }

        #endregion   

        #region Busca o total

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma compra
        /// </summary>
        public decimal ObtemTotalPorCompra(int idCompra)
        {
            return ObtemTotalPorCompra(null, idCompra);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma compra
        /// </summary>
        public decimal ObtemTotalPorCompra(GDASession session, int idCompra)
        {
            return ExecuteScalar<decimal>(session, "SELECT sum(valor) FROM centro_custo_associado WHERE IdCompra = " + idCompra);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de um importo e serviço
        /// </summary>
        public decimal ObtemTotalPorImpostoServ(int idImpostoServ)
        {
            return ObtemTotalPorImpostoServ(null, idImpostoServ);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de um importo e serviço
        /// </summary>
        public decimal ObtemTotalPorImpostoServ(GDASession session, int idImpostoServ)
        {
            return ExecuteScalar<decimal>(session, "SELECT sum(valor) FROM centro_custo_associado WHERE IdImpostoServ = " + idImpostoServ);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma nota fiscal
        /// </summary>
        public decimal ObtemTotalPorNotaFiscal(int idNf)
        {
            return ObtemTotalPorNotaFiscal(null, idNf);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma nota fiscal
        /// </summary>
        public decimal ObtemTotalPorNotaFiscal(GDASession session, int idNf)
        {
            var total =  ExecuteScalar<decimal>(session, "SELECT sum(valor) FROM centro_custo_associado WHERE idNf = " + idNf);

            foreach (var id in CompraNotaFiscalDAO.Instance.ObtemLstIdCompras(session, (uint)idNf))
                total += ObtemTotalPorCompra(session, (int)id);

            return total;
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma conta a pagar
        /// </summary>
        public decimal ObtemTotalPorContaPagar(int idContaPg)
        {
            return ObtemTotalPorContaPagar(null, idContaPg);
        }

        /// <summary>
        /// Obtem o valor total dos centros de custo de uma conta a pagar
        /// </summary>
        public decimal ObtemTotalPorContaPagar(GDASession session, int idContaPg)
        {
            return ExecuteScalar<decimal>(session, "SELECT sum(valor) FROM centro_custo_associado WHERE idContaPg = " + idContaPg);
        }

        #endregion

        #region Obtem dados do centro de custo associado

        /// <summary>
        /// Verifica se a compra ja tem um centro de custo do tipo estoque associado
        /// </summary>
        public int? TemCentroCustoEstoque(int idCompra)
        {
            return TemCentroCustoEstoque(null, idCompra);
        }

        /// <summary>
        /// Verifica se a compra ja tem um centro de custo do tipo estoque associado
        /// </summary>
        public int? TemCentroCustoEstoque(GDASession session, int idCompra)
        {
            var sql = @"
                SELECT cca.IdCentroCustoAssociado
                FROM centro_custo_associado cca
                    INNER JOIN centro_custo cc ON (cca.IdCentroCusto = cc.IdCentroCusto)
                WHERE cc.Tipo = " + (int)TipoCentroCusto.Estoque + @"
                    AND cca.IdCompra=" + idCompra;

            return ExecuteScalar<int?>(session, sql);
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(CentroCustoAssociado objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, CentroCustoAssociado objInsert)
        {
            //Valida a inserção
            ValidaInsertUpdate(session, objInsert);

            #region Busca o plano de contas

            if (objInsert.IdCompra.GetValueOrDefault(0) > 0)
                objInsert.IdConta = CompraDAO.Instance.ObtemIdConta(session, objInsert.IdCompra.Value);
            else if (objInsert.IdImpostoServ.GetValueOrDefault(0) > 0)
                objInsert.IdConta = ImpostoServDAO.Instance.ObtemIdConta(session, objInsert.IdImpostoServ.Value);
            else if (objInsert.IdNf.GetValueOrDefault(0) > 0)
                objInsert.IdConta = NotaFiscalDAO.Instance.ObtemIdConta(session, objInsert.IdNf.Value);
            else if (objInsert.IdContaPg.GetValueOrDefault(0) > 0)
                objInsert.IdConta = ContasPagarDAO.Instance.ObtemIdConta(session, objInsert.IdContaPg.Value);

            #endregion

            var id = base.Insert(session, objInsert);

            //Se for compra credita o estoque do centro de custo.
            if (objInsert.IdCompra.GetValueOrDefault(0) > 0 &&TemCentroCustoEstoque(session, objInsert.IdCompra.Value).GetValueOrDefault(0) > 0)
                MovEstoqueCentroCustoDAO.Instance.CreditaEstoqueCompra(session, objInsert.IdCompra.Value);

            return id;
        }

        public override int Update(CentroCustoAssociado objUpdate)
        {
            ValidaInsertUpdate(null, objUpdate);

            var centroCustoAntigo = GetElementByPrimaryKey(objUpdate.IdCentroCustoAssociado);
            if (centroCustoAntigo.IdCompra.GetValueOrDefault(0) > 0 &&TemCentroCustoEstoque(centroCustoAntigo.IdCompra.Value).GetValueOrDefault(0) > 0)
                MovEstoqueCentroCustoDAO.Instance.DeleteByCompra(centroCustoAntigo.IdCompra.Value);

            var id = base.Update(objUpdate);

            if (objUpdate.IdCompra.GetValueOrDefault(0) > 0 && TemCentroCustoEstoque(objUpdate.IdCompra.Value).GetValueOrDefault(0) > 0)
                MovEstoqueCentroCustoDAO.Instance.CreditaEstoqueCompra(null, objUpdate.IdCompra.Value);

            return id;
        }

        public override int Delete(CentroCustoAssociado objDelete)
        {
            var idCompra = ObtemValorCampo<int?>("IdCompra", "IdCentroCustoAssociado=" + objDelete.IdCentroCustoAssociado);

            if (idCompra.GetValueOrDefault(0) > 0 && TemCentroCustoEstoque(idCompra.Value).GetValueOrDefault(0) > 0)
                MovEstoqueCentroCustoDAO.Instance.DeleteByCompra(idCompra.Value);

            return base.Delete(objDelete);
        }

        #endregion

        #region Validações

        /// <summary>
        /// Valida a inserção ou atualização de um centro de custo da compra
        /// </summary>
        private void ValidaInsertUpdate(GDASession session, CentroCustoAssociado obj)
        {
            if (obj.Valor <= 0 && obj.IdPedidoInterno.GetValueOrDefault(0) == 0)
                throw new Exception("Informe o valor.");

            if (!CentroCustoDAO.Instance.Exists(session, obj.IdCentroCusto))
                throw new Exception("O centro de custo informado não existe.");

            //Caso seja update busca o antigo
            var centroCustoAntigo = obj.IdCentroCustoAssociado > 0 ? GetElementByPrimaryKey(session, obj.IdCentroCustoAssociado) : null;
            var valorJaAssociado = centroCustoAntigo != null ? centroCustoAntigo.Valor : 0;

            //Sql para verificar se um centro de custo de foi inserido
            var sqlCentroCustoExistente = @"
                SELECT count(*) 
                FROM centro_custo_associado 
                WHERE idCentroCusto=" + obj.IdCentroCusto;

            #region Compra

            if (obj.IdCompra > 0)
            {
                var valorCompra = CompraDAO.Instance.ObtemValorCampo<decimal>(session, "Total", "IdCompra = " + obj.IdCompra);
                var valorAssociacao = ObtemTotalPorCompra(session, obj.IdCompra.Value);
                var idLoja = CompraDAO.Instance.ObtemIdLoja(session, (uint)obj.IdCompra);
                var centroCustoEstoque = CentroCustoDAO.Instance.ObtemCentroCustoEstoque(session, (int)idLoja);
                var temCentroCustoEstoque = TemCentroCustoEstoque(session, obj.IdCompra.Value);

                if (!CompraDAO.Instance.Exists(session, obj.IdCompra))
                    throw new Exception("A compra informada não existe.");

                if (obj.Valor > valorCompra)
                    throw new Exception("O valor do centro de custo informado é maior que o valor da compra.");

                if (valorAssociacao + obj.Valor - valorJaAssociado > valorCompra)
                    throw new Exception("A soma dos centros de custos informados é maior que o valor da compra.");

                if ((centroCustoAntigo == null || centroCustoAntigo.IdCentroCusto != obj.IdCentroCusto) &&
                    objPersistence.ExecuteSqlQueryCount(session, sqlCentroCustoExistente + " AND idCompra=" + obj.IdCompra) > 0)
                    throw new Exception("O centro de custo informado já foi inserido.");

                if (centroCustoAntigo == null && temCentroCustoEstoque.GetValueOrDefault(0) > 0)
                    throw new Exception("A compra ja possui um centro de custo do tipo estoque associado.");
                else if (centroCustoAntigo != null && centroCustoEstoque.IdCentroCusto == obj.IdCentroCusto &&
                    temCentroCustoEstoque != null && obj.IdCentroCustoAssociado != temCentroCustoEstoque.Value)
                    throw new Exception("A compra ja possui um centro de custo do tipo estoque associado.");

                if (centroCustoEstoque != null && obj.Valor != valorCompra)
                    throw new Exception("O valor do centro de custo informado é diferente do valor da compra.");

            }

            #endregion

            #region Imp/Serv

            if (obj.IdImpostoServ > 0)
            {
                var valorImpostoServ = ImpostoServDAO.Instance.ObtemValorCampo<decimal>(session, "Total", "idImpostoServ = " + obj.IdImpostoServ);
                var valorAssociacao = ObtemTotalPorImpostoServ(session, (int)obj.IdImpostoServ);

                if (!ImpostoServDAO.Instance.Exists(session, obj.IdImpostoServ))
                    throw new Exception("O imposto/serviço avulso informado não existe.");

                if (obj.Valor > valorImpostoServ)
                    throw new Exception("O valor do centro de custo informado é maior que o valor do imposto/serviço avulso.");

                if (valorAssociacao + obj.Valor - valorJaAssociado > valorImpostoServ)
                    throw new Exception("A soma dos centros de custos informados é maior que o valor do imposto/serviço avulso.");

                if ((centroCustoAntigo == null || centroCustoAntigo.IdCentroCusto != obj.IdCentroCusto) &&
                    objPersistence.ExecuteSqlQueryCount(session, sqlCentroCustoExistente + " AND IdImpostoServ=" + obj.IdImpostoServ) > 0)
                    throw new Exception("O centro de custo informado já foi inserido.");
            }

            #endregion

            #region Nota Fiscal

            if (obj.IdNf > 0)
            {
                var valorNf = NotaFiscalDAO.Instance.ObtemTotal(session, (uint)obj.IdNf);
                var valorAssociacao = ObtemTotalPorNotaFiscal(session, (int)obj.IdNf);

                if (!NotaFiscalDAO.Instance.Exists(session, obj.IdNf))
                    throw new Exception("A nota fiscal informada não existe.");

                if (obj.Valor > valorNf)
                    throw new Exception("O valor do centro de custo informado é maior que o valor da nota fiscal.");

                if (valorAssociacao + obj.Valor - valorJaAssociado > valorNf)
                    throw new Exception("A soma dos centros de custos informados é maior que o valor da nota fiscal.");

                if ((centroCustoAntigo == null || centroCustoAntigo.IdCentroCusto != obj.IdCentroCusto) &&
                    objPersistence.ExecuteSqlQueryCount(session, sqlCentroCustoExistente + " AND IdNf=" + obj.IdNf) > 0)
                    throw new Exception("O centro de custo informado já foi inserido.");
            }

            #endregion

            #region Custo Fixo

            if (obj.IdContaPg > 0)
            {
                var valorContaPg = ContasPagarDAO.Instance.ObtemValorCampo<decimal>(session, "valorvenc", "idcontapg = " + obj.IdContaPg);
                var valorAssociacao = ObtemTotalPorContaPagar(session, (int)obj.IdContaPg);

                if (!NotaFiscalDAO.Instance.Exists(session, obj.IdContaPg))
                    throw new Exception("A conta a pagar do custo fixo informado não existe.");

                if (obj.Valor > valorContaPg)
                    throw new Exception("O valor do centro de custo informado é maior que o valor do custo fixo.");

                if (valorAssociacao + obj.Valor - valorJaAssociado > valorContaPg)
                    throw new Exception("A soma dos centros de custos informados é maior que o valor do custo fixo.");

                if ((centroCustoAntigo == null || centroCustoAntigo.IdCentroCusto != obj.IdCentroCusto) &&
                    objPersistence.ExecuteSqlQueryCount(session, sqlCentroCustoExistente + " AND IdContaPg=" + obj.IdContaPg) > 0)
                    throw new Exception("O centro de custo informado já foi inserido.");
            }

            #endregion

            #region Pedido Interno

            if (obj.IdPedidoInterno > 0)
            {

            }

            #endregion
        }

        #endregion

        #region Remove associação do centro de custo

        /// <summary>
        /// Apaga as associações de um pedido interno
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        public void DeleteByPedidoInterno(int idPedidoInterno)
        {
            objPersistence.ExecuteCommand("DELETE FROM centro_custo_associado WHERE IdPedidoInterno = " + idPedidoInterno);
        }

        #endregion
    }
}
