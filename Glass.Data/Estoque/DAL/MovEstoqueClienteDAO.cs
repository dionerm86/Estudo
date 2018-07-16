using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueClienteDAO : BaseDAO<MovEstoqueCliente, MovEstoqueClienteDAO>
    {
        //private MovEstoqueClienteDAO() { }

        #region Recupera listagem

        private string Sql(uint idLoja, uint idCliente, string codInterno, string descricao, string dataIni, 
            string dataFim, int tipoMov, int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, 
            uint idCorFerragem, uint idCorAluminio, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;

            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? @"me.*, f.nome as nomeFunc, u.codigo as codUnidade, '$$$' as criterio" : "Count(*)");

            sql.AppendFormat(@"
                From mov_estoque_cliente me
                    Left Join produto p On (me.idProd=p.idProd) 
                    Left Join produtos_nf pnf On (me.idProdNf=pnf.idProdNf)
                    Left Join funcionario f On (me.idFunc=f.idFunc)
                    Left Join unidade_medida u On (p.idUnidadeMedida=u.idUnidadeMedida)
                Where 1 {0}", FILTRO_ADICIONAL);

            // Retorna movimentação apenas se a loja, o cliente e o produto tiverem sido informados
            if (idLoja == 0 || idCliente == 0 || String.IsNullOrEmpty(codInterno))
            {
                filtroAdicional = String.Empty;
                return sql.ToString() + " And false";
            }

            StringBuilder fa = new StringBuilder();
            StringBuilder criterio = new StringBuilder();

            if (idLoja > 0)
            {
                fa.AppendFormat(" And me.idLoja={0}", idLoja);
                criterio.AppendFormat("Loja: {0}    ", LojaDAO.Instance.GetNome(idLoja));
            }

            if (idCliente > 0)
            {
                fa.AppendFormat(" and me.idCliente={0}", idCliente);
                criterio.AppendFormat("Cliente: {0}    ", ClienteDAO.Instance.GetNome(idCliente));
            }

            if (!String.IsNullOrEmpty(codInterno) || !String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInterno, descricao);
                fa.AppendFormat(" And me.idProd In ({0})", ids);

                if (!String.IsNullOrEmpty(descricao))
                    criterio.AppendFormat("Produto: {0}    ", descricao);
                else
                    criterio.AppendFormat("Produto: {0}    ", ProdutoDAO.Instance.GetDescrProduto(codInterno));
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                fa.Append(" And me.dataMov>=?dataIni");
                criterio.AppendFormat("Período: {0}", dataIni);
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                fa.Append(" And me.dataMov<=?dataFim");
                criterio.AppendFormat(" até: {0}    ", dataFim);
            }

            if (tipoMov > 0)
            {
                fa.AppendFormat(" And me.tipoMov={0}", tipoMov);
                criterio.AppendFormat("Apenas movimentações de {0}    ", (tipoMov == (int)MovEstoque.TipoMovEnum.Entrada ? "entrada" : "saída"));
            }

            if (situacaoProd > 0)
            {
                fa.AppendFormat(" and p.situacao={0}", situacaoProd);
                criterio.AppendFormat("Situação: {0}    ", (situacaoProd == 1 ? "Ativos" : "Inativos"));
            }

            if (idCfop > 0)
            {
                string ids = NaturezaOperacaoDAO.Instance.GetValoresCampo(@"select idNaturezaOperacao 
                    from natureza_operacao where idCfop=" + idCfop, "idNaturezaOperacao");

                fa.AppendFormat(" and pnf.idNaturezaOperacao in ({0})", ids);
                temFiltro = true;
                criterio.AppendFormat("CFOP: {0}    ", CfopDAO.Instance.ObtemCodInterno(idCfop));
            }

            if (idGrupoProd > 0)
            {
                fa.AppendFormat(" and p.idGrupoProd={0}", idGrupoProd);
                temFiltro = true;
                criterio.AppendFormat("Grupo: {0}    ", GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd));
            }

            if (idSubgrupoProd > 0)
            {
                fa.AppendFormat(" and p.idSubgrupoProd={0}", idSubgrupoProd);
                temFiltro = true;
                criterio.AppendFormat("Subgrupo: {0}    ", SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd));
            }

            if (idCorVidro > 0)
            {
                fa.AppendFormat(" and p.idCorVidro={0}", idCorVidro);
                temFiltro = true;
                criterio.AppendFormat("Cor Vidro: {0}    ", CorVidroDAO.Instance.GetNome(idCorVidro));
            }

            if (idCorFerragem > 0)
            {
                fa.AppendFormat(" and p.idCorFerragem={0}", idCorFerragem);
                temFiltro = true;
                criterio.AppendFormat("Cor Ferragem: {0}    ", CorFerragemDAO.Instance.GetNome(idCorFerragem));
            }

            if (idCorAluminio > 0)
            {
                fa.AppendFormat(" and p.idCorAluminio={0}", idCorAluminio);
                temFiltro = true;
                criterio.AppendFormat("Cor Alumínio: {0}    ", CorAluminioDAO.Instance.GetNome(idCorAluminio));
            }

            filtroAdicional = fa.ToString();
            return sql.ToString().Replace("$$$", criterio.ToString());
        }

        public IList<MovEstoqueCliente> GetForRpt(uint idLoja, uint idCliente, string codInterno, string descricao, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idLoja, idCliente, codInterno, descricao, dataIni, dataFim, tipoMov, situacaoProd,
                idCfop, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, true, out temFiltro, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional) + " Order By me.dataMov asc, me.idMovEstoqueCliente Asc";

            return objPersistence.LoadData(sql, GetParam(dataIni, dataFim)).ToList();
        }

        public IList<MovEstoqueCliente> GetList(uint idLoja, uint idCliente, string codInterno, string descricao, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idLoja, idCliente, codInterno, descricao, dataIni, dataFim, tipoMov, situacaoProd,
                idCfop, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, true, out temFiltro, out filtroAdicional);

            string sortExpression = "me.dataMov asc, me.idMovEstoqueCliente Asc";
            return LoadDataWithSortExpression(sql, sortExpression, 0, int.MaxValue, temFiltro, filtroAdicional, GetParam(dataIni, dataFim)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Obtém o saldo do produto

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        private uint? ObtemUltimoIdMovEstoqueCliente(uint idCliente, uint idProd, uint idLoja)
        {
            return ObtemUltimoIdMovEstoqueCliente(null, idCliente, idProd, idLoja);
        }

        private uint? ObtemUltimoIdMovEstoqueCliente(GDASession sessao, uint idCliente, uint idProd, uint idLoja)
        {
            return ExecuteScalar<uint?>(sessao, @"select idMovEstoqueCliente from mov_estoque_cliente where idCliente=?idCli 
                and idProd=?idProd and idLoja=?idLoja order by dataMov desc, idMovEstoqueCliente desc limit 1",
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja), new GDAParameter("?idCli", idCliente));
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem o id da movimentação anterior.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        private uint? ObtemIdMovAnterior(uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov)
        {
            return ObtemIdMovAnterior(null, idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov);
        }

        /// <summary>
        /// Obtem o id da movimentação anterior.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        private uint? ObtemIdMovAnterior(GDASession sessao, uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov)
        {
            // Remove os milisegundos da hora da movimentação
            dataMov = dataMov.AddMilliseconds(-dataMov.Millisecond);

            if (idMovEstoqueCliente.GetValueOrDefault() == 0)
            {
                idMovEstoqueCliente = ExecuteScalar<uint>(sessao, @"select idMovEstoqueCliente from mov_estoque_cliente
                    where idCliente=?idCli and idProd=?idProd and idLoja=?idLoja and dataMov=?data order by dataMov asc, 
                    idMovEstoqueCliente asc limit 1", new GDAParameter("?idProd", idProd),
                    new GDAParameter("?idLoja", idLoja), new GDAParameter("?data", dataMov));
            }

            return ExecuteScalar<uint?>(sessao, "select idMovEstoqueCliente from mov_estoque_cliente me where idCliente=" + idCliente + 
                " and idProd=" + idProd + " and idLoja=" + idLoja + " and (dataMov<?data or (dataMov=?data and idMovEstoqueCliente<" + idMovEstoqueCliente + @"))
                order by dataMov desc, idMovEstoqueCliente desc limit 1", new GDAParameter("?data", dataMov));
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoQtdeMov(idMovEstoqueCliente, idCliente, idProd, idLoja, DateTime.Now, anterior);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            return ObtemSaldoQtdeMov(null, idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov, anterior);
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(GDASession sessao, uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoqueCliente = ObtemIdMovAnterior(sessao, idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov);
            else if (idMovEstoqueCliente.GetValueOrDefault() == 0)
                idMovEstoqueCliente = ObtemUltimoIdMovEstoqueCliente(sessao, idCliente, idProd, idLoja);

            return ObtemValorCampo<decimal>(sessao, "saldoQtdeMov", "idMovEstoqueCliente=" + idMovEstoqueCliente.GetValueOrDefault());
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoValorMov(idMovEstoqueCliente, idCliente, idProd, idLoja, DateTime.Now, anterior);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            return ObtemSaldoValorMov(null, idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov, anterior);
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(GDASession sessao, uint? idMovEstoqueCliente, uint idCliente, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoqueCliente = ObtemIdMovAnterior(sessao, idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov);
            else if (idMovEstoqueCliente.GetValueOrDefault() == 0)
                idMovEstoqueCliente = ObtemUltimoIdMovEstoqueCliente(sessao, idCliente, idProd, idLoja);

            return ObtemValorCampo<decimal>(sessao, "saldoValorMov", "idMovEstoqueCliente=" + idMovEstoqueCliente.GetValueOrDefault());
        }

        #endregion

        #region Baixa Estoque

        public void BaixaEstoquePedido(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima)
        {
            var totalProdPed = MovEstoqueDAO.Instance.GetTotalProdPed(sessao, (int)idProdPed);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, idPedido, idProdPed, null, null, null, false, qtdeBaixa, qtdeBaixaAreaMinima,
                totalProdPed, DateTime.Now, null);
        }

        public void BaixaEstoqueLiberacao(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido,
            uint idProdLiberarPedido, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima)
        {
            var totalProdLiberarPedido = MovEstoqueDAO.Instance.GetTotalProdLiberarPedido(sessao, (int)idProdLiberarPedido);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, idPedido, null, idLiberarPedido, idProdLiberarPedido, null, false, qtdeBaixa,
                qtdeBaixaAreaMinima, totalProdLiberarPedido, DateTime.Now, null);
        }

        public void BaixaEstoqueManual(uint idCliente, uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string obs)
        {
            var totalProdManual = MovEstoqueDAO.Instance.GetTotalEstoqueManual(null, (int)idProd, qtdeBaixa);

            MovimentaEstoqueCliente(idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, true, qtdeBaixa, 0, valor.GetValueOrDefault(totalProdManual), dataMov, obs);
        }

        public void BaixaEstoqueProducao(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima)
        {
            var totalProdPedProducao = MovEstoqueDAO.Instance.GetTotalProdPedProducao(sessao, (int)idProdPedProducao);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, idProdPedProducao, false, qtdeBaixa, qtdeBaixaAreaMinima,
                totalProdPedProducao, DateTime.Now, null);
        }

        #endregion

        #region Credita Estoque

        public void CreditaEstoqueNotaFiscal(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idNaturezaOperacao, uint idNf, uint idProdNf, decimal qtdeEntrada)
        {
            var tipoDoc = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(idNf);
            if (tipoDoc != NotaFiscal.TipoDoc.NotaCliente)
                return;

            DateTime dataMov = NotaFiscalDAO.Instance.ObtemValorCampo<DateTime?>(sessao, "dataSaidaEnt", "idNf=" + idNf).GetValueOrDefault(DateTime.Now);
            var totalProdNf = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)idProdNf);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idNf, idProdNf, null, null, null, null, null,
                false, qtdeEntrada, 0, totalProdNf, dataMov, null);
        }
        
        public void CreditaEstoquePedido(GDASession session, uint idCliente, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeEntrada)
        {
            var totalProdPed = MovEstoqueDAO.Instance.GetTotalProdPed(session, (int)idProdPed);

            MovimentaEstoqueCliente(session, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, idPedido, idProdPed, null, null, null, false, qtdeEntrada, 0, totalProdPed,
                DateTime.Now, null);
        }

        public void CreditaEstoqueLiberacao(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido, uint idProdLiberarPedido, decimal qtdeEntrada)
        {
            var totalProdLiberarPedido = MovEstoqueDAO.Instance.GetTotalProdLiberarPedido(sessao, (int)idProdLiberarPedido);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, idPedido, null, idLiberarPedido, idProdLiberarPedido, null, false, qtdeEntrada,
                0, totalProdLiberarPedido, DateTime.Now, null);
        }

        public void CreditaEstoqueManual(uint idCliente, uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string obs)
        {
            var totalProdManual = MovEstoqueDAO.Instance.GetTotalEstoqueManual(null, (int)idProd, qtdeEntrada);

            MovimentaEstoqueCliente(idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, true, qtdeEntrada, 0,
                valor.GetValueOrDefault(totalProdManual), dataMov, obs);
        }
        
        public void CreditaEstoqueProducao(GDASession sessao, uint idCliente, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeEntrada)
        {
            var totalProdPedProducao = MovEstoqueDAO.Instance.GetTotalProdPedProducao(sessao, (int)idProdPedProducao);

            MovimentaEstoqueCliente(sessao, idCliente, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, idProdPedProducao, false, qtdeEntrada, 0,
                totalProdPedProducao, DateTime.Now, null);
        }

        #endregion

        #region Verifica se há uma movimentação posterior

        /// <summary>
        /// Verifica se há uma movimentação posterior.
        /// </summary>
        /// <param name="idMovEstoqueCliente"></param>
        /// <returns></returns>
        public bool TemMovimentacaoPosterior(uint idMovEstoqueCliente)
        {
            return TemMovimentacaoPosterior(null, idMovEstoqueCliente);
        }

        /// <summary>
        /// Verifica se há uma movimentação posterior.
        /// </summary>
        /// <param name="idMovEstoqueCliente"></param>
        /// <returns></returns>
        public bool TemMovimentacaoPosterior(GDASession sessao, uint idMovEstoqueCliente)
        {
            DateTime dataMov = ObtemValorCampo<DateTime>("dataMov", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            string sql = @"select count(*) from mov_estoque_cliente where dataMov>?data
                or (dataMov=?data and idMovEstoqueCliente>" + idMovEstoqueCliente + ")";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?data", dataMov)) > 0;
        }

        #endregion

        #region Movimenta Estoque

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Dá baixa no estoque no produto da loja passados
        /// </summary>
        private void MovimentaEstoqueCliente(uint idCliente, uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idNf, uint? idProdNf,
            uint? idPedido, uint? idProdPed, uint? idLiberarPedido, uint? idProdLiberarPedido, uint? idProdPedProducao, bool lancManual, decimal qtdeMov,
            decimal qtdeMovAreaMinima, decimal total, DateTime dataMov, string obs)
        {
            MovimentaEstoqueCliente(null, idCliente, idProd, idLoja, tipoMov, idNf, idProdNf, idPedido, idProdPed, idLiberarPedido,
                idProdLiberarPedido, idProdPedProducao, lancManual, qtdeMov, qtdeMovAreaMinima, total, dataMov, obs);
        }

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados
        /// </summary>
        private void MovimentaEstoqueCliente(GDASession sessao, uint idCliente, uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idNf, uint? idProdNf,
            uint? idPedido, uint? idProdPed, uint? idLiberarPedido, uint? idProdLiberarPedido, uint? idProdPedProducao, bool lancManual, decimal qtdeMov,
            decimal qtdeMovAreaMinima, decimal total, DateTime dataMov, string obs)
        {
            if (!EstoqueConfig.ControlarEstoqueVidrosClientes)
                return;
            
            ProdutoBaixaEstoque[] prodBaixEst;

            if (!lancManual)
                prodBaixEst = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, idProd);
            else
                prodBaixEst = new ProdutoBaixaEstoque[] 
                {
                    new ProdutoBaixaEstoque()
                    {
                        IdProd = (int)idProd,
                        IdProdBaixa = (int)idProd,
                        Qtde = 1
                    }
                };

            foreach (var pbe in prodBaixEst)
            {
                bool executou = false;
                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)idProd);

                //Se não for lançamento manual e o produto for chapa de vidro mov. a matéria-prima
                if (!lancManual && (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado))
                {
                    decimal m2Chapa = ProdutoDAO.Instance.ObtemM2Chapa(sessao, (int)pbe.IdProdBaixa);
                    decimal qtdeMovMateriaPrima = (decimal)pbe.Qtde * m2Chapa;

                    uint? idProdBase = ProdutoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idProdBase", "idProd=" + pbe.IdProdBaixa);

                    if (idProdBase.HasValue && idProd != idProdBase)
                    {
                        MovimentaEstoqueCliente(sessao, idCliente, idProdBase.Value, idLoja, tipoMov, idNf, idProdNf, idPedido, idProdPed,
                            idLiberarPedido, idProdLiberarPedido, idProdPedProducao, lancManual, qtdeMov, qtdeMovAreaMinima, total, dataMov, obs);

                        executou = true;
                    }
                }

                if (!executou)
                {
                    decimal qtde = (qtdeMovAreaMinima > 0 ? qtdeMovAreaMinima : qtdeMov) * (decimal)pbe.Qtde;
                    decimal totalMov = pbe.IdProdBaixa != idProd ? MovEstoqueDAO.Instance.GetTotalEstoqueManual(sessao, (int)pbe.IdProdBaixa, qtde) :
                        total * (decimal)pbe.Qtde;

                    // Registra a alteração do estoque
                    MovEstoqueCliente movEstoque = new MovEstoqueCliente();
                    movEstoque.IdCliente = idCliente;
                    movEstoque.IdProd = (uint)pbe.IdProdBaixa;
                    movEstoque.IdLoja = idLoja;
                    movEstoque.IdFunc = UserInfo.GetUserInfo.CodUser;
                    movEstoque.IdNf = idNf;
                    movEstoque.IdProdNf = idProdNf;
                    movEstoque.IdPedido = idPedido;
                    movEstoque.IdProdPed = idProdPed;
                    movEstoque.IdLiberarPedido = idLiberarPedido;
                    movEstoque.IdProdLiberarPedido = idProdLiberarPedido;
                    movEstoque.IdProdPedProducao = idProdPedProducao;
                    movEstoque.LancManual = lancManual;
                    movEstoque.TipoMov = (int)tipoMov;
                    movEstoque.DataMov = dataMov;
                    if (dataMov.Date != DateTime.Now.Date) movEstoque.DataCad = DateTime.Now;
                    movEstoque.QtdeMov = qtde;
                    movEstoque.Observacao = obs;

                    movEstoque.IdMovEstoqueCliente = Insert(sessao, movEstoque);

                    // Recupera os dados da movimentação anterior
                    decimal saldoQtdeAnterior = ObtemSaldoQtdeMov(sessao, movEstoque.IdMovEstoqueCliente, idCliente, (uint)pbe.IdProdBaixa, idLoja, dataMov, true);
                    decimal saldoValorAnterior = ObtemSaldoValorMov(sessao, movEstoque.IdMovEstoqueCliente, idCliente, (uint)pbe.IdProdBaixa, idLoja, dataMov, true);

                    movEstoque.SaldoQtdeMov = Math.Round(saldoQtdeAnterior + (tipoMov == MovEstoque.TipoMovEnum.Entrada ? qtde : -qtde), Geral.NumeroCasasDecimaisTotM);

                    if (movEstoque.SaldoQtdeMov < 0)
                    {
                        movEstoque.ValorMov = 0;
                        movEstoque.SaldoValorMov = 0;
                    }
                    else if (tipoMov == MovEstoque.TipoMovEnum.Entrada)
                    {
                        decimal perc = tipoMov == MovEstoque.TipoMovEnum.Entrada && qtde > movEstoque.SaldoQtdeMov ?
                            qtde / (movEstoque.SaldoQtdeMov > 0 ? movEstoque.SaldoQtdeMov : 1) : 1;

                        movEstoque.ValorMov = Math.Abs(totalMov);
                        movEstoque.SaldoValorMov = saldoValorAnterior + (movEstoque.ValorMov * perc);
                    }
                    else
                    {
                        decimal valorUnit = saldoValorAnterior / (saldoQtdeAnterior > 0 ? saldoQtdeAnterior : 1);
                        movEstoque.ValorMov = Math.Abs(valorUnit * qtde);
                        movEstoque.SaldoValorMov = saldoValorAnterior - (valorUnit * qtde);
                    }

                    base.Update(sessao, movEstoque);

                    if (TemMovimentacaoPosterior(sessao, movEstoque.IdMovEstoqueCliente))
                        AtualizaSaldo(sessao, movEstoque.IdMovEstoqueCliente);
                }
            }
        }

        #endregion

        #region Atualiza o saldo de estoque

        private void AtualizaSaldoQtd(uint idMovEstoqueCliente)
        {
            AtualizaSaldoQtd(null, idMovEstoqueCliente);
        }

        private void AtualizaSaldoQtd(GDASession sessao, uint idMovEstoqueCliente)
        {
            uint idCliente = ObtemValorCampo<uint>("idCliente", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            uint idProd = ObtemValorCampo<uint>("idProd", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            uint idLoja = ObtemValorCampo<uint>("idLoja", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            DateTime dataMov = ObtemValorCampo<DateTime>("dataMov", "idMovEstoqueCliente=" + idMovEstoqueCliente);

            string sql = @"
                set @saldo := coalesce((select saldoQtdeMov from mov_estoque_cliente
                    where (dataMov<?data or (dataMov=?data and idMovEstoqueCliente<?id))
                    and idCliente=?idCli and idProd=?idProd and idLoja=?idLoja
                    order by dataMov desc, idMovEstoqueCliente desc limit 1), 0);
                
                update mov_estoque_cliente set saldoQtdeMov=(@saldo := @saldo + if(tipoMov=1, qtdeMov, -qtdeMov))
                where (dataMov>?data or (dataMov=?data and idMovEstoqueCliente>=?id)) and idCliente=?idCli and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoqueCliente asc";

            objPersistence.ExecuteCommand(sessao, sql,
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoqueCliente),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja),
                new GDAParameter("?idCli", idCliente));
        }

        private void AtualizaSaldoTotal(uint idMovEstoqueCliente)
        {
            AtualizaSaldoTotal(null, idMovEstoqueCliente);
        }

        private void AtualizaSaldoTotal(GDASession sessao, uint idMovEstoqueCliente)
        {
            uint idCliente = ObtemValorCampo<uint>("idCliente", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            uint idProd = ObtemValorCampo<uint>("idProd", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            uint idLoja = ObtemValorCampo<uint>("idLoja", "idMovEstoqueCliente=" + idMovEstoqueCliente);
            DateTime dataMov = ObtemValorCampo<DateTime>("dataMov", "idMovEstoqueCliente=" + idMovEstoqueCliente);

            uint idMovEstoqueClienteAnt = ObtemIdMovAnterior(idMovEstoqueCliente, idCliente, idProd, idLoja, dataMov).GetValueOrDefault();

            string sql = @"
                /**
                 * Recupera algumas variáveis para uso durante o cálculo dos valores das movimentações:
                 * @saldo - o saldo de valor da movimentação anterior à movimentação que está sendo alterada
                 * @valorUnit - o valor unitário da movimentação anterior à movimentação que está sendo alterada
                 * @valorProd - o maior valor salvo para o produto (para normalização de valores, ver abaixo)
                 */
                set @saldo := coalesce((
                    select if(saldoQtdeMov<0, 0, Coalesce(saldoValorMov, 0))
                    from mov_estoque_cliente where idMovEstoqueCliente=?idAnt
                ), 0), 

                @valorUnit := coalesce((
                    select if(saldoQtdeMov<0, 0, abs(coalesce(saldoValorMov/if(saldoQtdeMov<>0, saldoQtdeMov, 1), 0)))
                    from mov_estoque_cliente where idMovEstoqueCliente=?idAnt
                ), 0),

                @valorProd := 0 /* Removido - erro no cálculo de produtos com valor muito baixo - coalesce((
                    select greatest(valorAtacado, valorBalcao, valorObra, custoCompra, custoFabBase)
                    from produto where idProd=?idProd
                ), 0) */;

                update mov_estoque_cliente
                set valorMov=abs(
                    
                    /**
                     * Verifica se a movimentação é de entrada: se for, o valor da movimentação é o próprio valor (calcula o 
                     * valor unitário com base no saldo anterior, no valor da movimentação e no saldo de quantidade);
                     * caso não seja, o valor é calculado com base no valor unitário anterior * a quantidade movimentada -
                     * esse valor é armazenado para o próximo cálculo
                     */
                    if(tipoMov=1, (@valorUnit := (@saldo + coalesce(valorMov, 0)) / if(saldoQtdeMov <> 0, saldoQtdeMov, 1)) * 0 + coalesce(valorMov, 0), (@valorUnit := 
                        
                        /**
                         * Verifica se o saldo da movimentação atual é negativo: caso seja, o valor fica zerado (evita saldo negativo);
                         * se não for, o valor unitário é calculado com base no saldo de valor e no saldo de quantidade
                         */
                        if(saldoQtdeMov<0, 0,
                            
                            /**
                             * Verifica se o saldo da movimentação é menor que a quantidade da movimentação (apenas para movimentações 
                             * de entrada): calcula o valor da movimentação apenas do percentual que ficou positivo, desprezando o 
                             * restante (o saldo anterior era negativo); caso contrário, calcula o valor integral da movimentação
                             */
                            if(saldoQtdeMov<qtdeMov and tipoMov=1, 
                                
                                /**
                                 * Soma o saldo anterior (variável @saldo) ao valor da movimentação atual, dividindo pelo saldo de 
                                 * quantidade (valor vendido), dividindo novamente pela quantidade da movimentação (valor unitário) e 
                                 * multiplicando pelo saldo de quantidade
                                 */
                                ((@saldo+coalesce(valorMov,0)) / if(saldoQtdeMov<>0, saldoQtdeMov, 1)) / if(qtdeMov<>0, qtdeMov, 1) * saldoQtdeMov, 
                                
                                /**
                                 * Calcula um valor unitário (temporário) para uso no cálculo
                                 */
                                if((@valorUnit := abs(coalesce((
                                        
                                        /**
                                         * Ao calcular o valor da movimentação, é verificado o seu tipo: caso seja movimentação de entrada
                                         * é somado o saldo anterior (variável @saldo) com o valor da movimentação atual, dividindo pelo 
                                         * saldo de quantidade (valor vendido); mas se a movimentação for de saída, utiliza-se o valor 
                                         * unitário calculado anteriormente
                                         */
                                        if(tipoMov=1, (@saldo+coalesce(valorMov,0))/if(saldoQtdeMov<>0, saldoQtdeMov, 1), @valorUnit)
                                
                                    ), 0)))>
                                    
                                    /**
                                     * Garante que o valor calculado seja, no máximo, 5 vezes o maior valor de tabela do produto
                                     * (tentativa de que não haja valores exorbitantes durante o cálculo - apenas se houver algum valor
                                     * de tabela recuperado para o produto)
                                     */
                                    if(@valorProd>0, @valorProd*5, @valorUnit), @valorProd, @valorUnit)
                                )
                            )
                        
                        /**
                         * Multiplica o valor calculado (unitário) pela quantidade movimentada
                         */   
                        ) * qtdeMov
                    )),
                    
                    /**
                     * Atualiza o novo saldo na variável @saldo para que esse valor seja usado para o cálculo do valor da 
                     * próxima movimentação
                     */
                    saldoValorMov=(@saldo := 
                        
                        /**
                         * Verifica se o saldo da movimentação ficou negativo, alterando para 0.
                         */
                        if(saldoQtdeMov<0, 0, 
                            
                            /**
                             * Verifica se o saldo da movimentação é menor que a quantidade movimentada (apenas para
                             * movimentações de entrada): caso seja, calcula o novo valor com base no percentual da movimentação
                             * que ficou positiva; se não, apenas soma/subtrai o valor da movimentação ao saldo
                             */
                            if(saldoQtdeMov<qtdeMov and tipoMov=1, 
                                
                                /**
                                 * Divide o valor da movimentação pela quantidade (valor unitário) e então multiplica
                                 * pelo saldo atual de quantidade (apenas a parte positiva)
                                 */
                                valorMov / if(qtdeMov<>0, qtdeMov, 1) * saldoQtdeMov, 
                                
                                /**
                                 * Soma ou subtrai o valor da movimentação ao saldo anterior, com base no tipo de movimentação
                                 * realizada (soma para movimentação de entrada, subtrai para movimentação de saída)
                                 */ 
                                @saldo + if(tipoMov=1, valorMov, -valorMov)
                            )
                        )
                    )

                where (dataMov>?data or (dataMov=?data and idMovEstoqueCliente>=?id)) and idCliente=?idCli and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoqueCliente asc";

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?idAnt", idMovEstoqueClienteAnt),
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoqueCliente),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja),
                new GDAParameter("?idCli", idCliente));

            if (idMovEstoqueClienteAnt == 0)
            {
                idMovEstoqueCliente = ExecuteScalar<uint>(@"select idMovEstoqueCliente from mov_estoque_cliente where idCliente=?idCli
                    and idProd=?idProd and idLoja=?idLoja and idMovEstoqueCliente<>?id order by dataMov asc, idMovEstoqueCliente asc limit 1",
                    new GDAParameter("?id", idMovEstoqueCliente), new GDAParameter("?idProd", idProd),
                    new GDAParameter("?idLoja", idLoja), new GDAParameter("?idCli", idCliente));

                if (idMovEstoqueCliente > 0)
                    AtualizaSaldoTotal(sessao, idMovEstoqueCliente);
            }
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        /// <param name="idMovEstoqueCliente"></param>
        public void AtualizaSaldo(uint idMovEstoqueCliente)
        {
            AtualizaSaldo(null, idMovEstoqueCliente);
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        /// <param name="idMovEstoqueCliente"></param>
        public void AtualizaSaldo(GDASession sessao, uint idMovEstoqueCliente)
        {
            AtualizaSaldoQtd(sessao, idMovEstoqueCliente);
            AtualizaSaldoTotal(sessao, idMovEstoqueCliente);
        }

        #endregion

        #region Apaga as movimentações de NF-e

        /// <summary>
        /// Apaga as movimentações de NF-e.
        /// </summary>
        internal void DeleteByNf(GDASession sessao, uint idNf)
        {
            foreach (var movEstoque in ExecuteMultipleScalar<uint>(sessao, "select idMovEstoqueCliente from mov_estoque_cliente where idNf=" + idNf))
                DeleteByPrimaryKey(sessao, movEstoque);
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(MovEstoqueCliente objUpdate)
        {
            LogAlteracaoDAO.Instance.LogMovEstoqueCliente(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(MovEstoqueCliente objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdMovEstoqueCliente);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            MovEstoqueCliente mov = GetElementByPrimaryKey(sessao, Key);
            LogCancelamentoDAO.Instance.LogMovEstoqueCliente(sessao, mov, null, true);

            // Zera a movimentação para recalcular o saldo
            objPersistence.ExecuteCommand(sessao, "update mov_estoque_cliente set qtdeMov=0, valorMov=0 where idMovEstoqueCliente=" + Key);
            AtualizaSaldo(sessao, Key);

            return base.DeleteByPrimaryKey(sessao, Key);
        }

        #endregion
    }
}
