using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Collections;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ProdutosNfDAO : BaseDAO<ProdutosNf, ProdutosNfDAO>
	{
        //private ProdutosNfDAO() { }

        #region Busca produtos para listagem padrão

        private string Sql(uint idNf, bool selecionar)
        {
            string campos = selecionar ? @"pn.*, p.Descricao as DescrProduto, p.CodInterno, p.IdGrupoProd, 
                p.idSubgrupoProd, if(p.AtivarAreaMinima=1, Cast(p.AreaMinima as char), '0') as AreaMinima, 
                coalesce(no.codInterno, c.codInterno) as CodNaturezaOperacao, c.CodInterno as CodCfop, 
                (mbai.idProdNf is not null) as temMovimentacaoBemAtivoImob, pcc.descricao as descrPlanoContaContabil, 
                um.Codigo As Unidade, n.TipoDocumento" : "Count(*)";

            string sql = @"
                Select " + campos + @" From produtos_nf pn 
                    Left Join produto p On (pn.idProd=p.idProd) 
                    Left Join natureza_operacao no On (pn.idNaturezaOperacao=no.idNaturezaOperacao)
                    Left Join cfop c On (no.idCfop=c.idCfop) 
                    Left Join movimentacao_bem_ativo_imob mbai on (pn.idProdNf=mbai.idProdNf)   
                    Left Join plano_conta_contabil pcc on (pn.idContaContabil=pcc.idContaContabil)
                    Left Join unidade_medida um On(um.IdUnidadeMedida=p.IdUnidadeMedida)
                    Left Join nota_fiscal n On(n.IdNF=pn.IdNF)
                Where 1"; 
            
            if (idNf > 0)
                sql += " and pn.idNf=" + idNf;

            return sql;
        }

        public ProdutosNf[] GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            if (CountInNf(idNf) == 0)
                return new ProdutosNf[] { new ProdutosNf() };

            string sort = String.IsNullOrEmpty(sortExpression) ? " Order By pn.IdProdNf asc" : "";

            var lstProd = LoadDataWithSortExpression(Sql(idNf, true) + sort, sortExpression, startRow, pageSize, null).ToArray();

            // Verifica se há algum beneficiamento de bisote ou lapidação
            if (FiscalConfig.NotaFiscalConfig.AcrescentarLapBisProdutoNota)
                foreach (ProdutosNf pnf in lstProd)
                {
                    if (ProdutoNfBenefDAO.Instance.PossuiLapidacao(null, pnf.IdProdNf))
                        pnf.DescrProduto += " LAP.";

                    if (ProdutoNfBenefDAO.Instance.PossuiBisote(null, pnf.IdProdNf))
                        pnf.DescrProduto += " BIS.";
                }

            return lstProd;
        }

        public int GetCount(uint idNf)
        {
            int count = objPersistence.ExecuteSqlQueryCount(Sql(idNf, false), null);

            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Retorna a quantidade de produtos relacionados à NF passada
        /// </summary>
        /// <param name="idNf"></param>
        public int CountInNf(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, false), null);
        }

        public ProdutosNf GetElement(uint idProdNf)
        {
            return GetElement(null, idProdNf);
        }

        public ProdutosNf GetElement(GDASession sessao, uint idProdNf)
        {
            string sql = @"
                Select pnf.*, p.codInterno, p.descricao as DescrProduto, um.codigo as unidade, umt.codigo as unidadeTrib 
                From produtos_nf pnf 
                    Left Join produto p On (pnf.idProd=p.idProd) 
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left Join unidade_medida umt On (p.idUnidadeMedidaTrib=umt.idUnidadeMedida)
                Where IdProdNf=" + idProdNf;

            return objPersistence.LoadOneData(sessao, sql);
        }

        public ProdutosNf[] ObterProdutosNota(uint numeroNfe)
        {
            return ObterProdutosNota(numeroNfe, false, false);
        }

        public ProdutosNf[] ObterProdutosNota(uint numeroNfe, bool apenasEntradas, bool apenasVidros)
        {
            string sql = @"
                select pnf.*, p.Descricao as DescrProduto, p.CodInterno, 
                    concat(g.Descricao, ' ', coalesce(sg.descricao, '')) as DescrTipoProduto
                from produtos_nf pnf 
                    inner join nota_fiscal n on(n.IdNf=pnf.IdNf)
                    inner join produto p on(p.IdProd=pnf.IdProd)
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) 
                    left join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd) 
                where n.NumeroNFE=" + (numeroNfe == 0 ? -1 : (int)numeroNfe);

            if (apenasEntradas)
                sql += " and n.TipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída;

            if (apenasVidros)
                sql += " and p.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro;

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Busca todos os produtos relacionados à NF

        /// <summary>
        /// Busca todos os produtos relacionados à NF
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public ProdutosNf[] GetByNf(uint idNf)
        {
            return GetByNf(null, idNf);
        }

        /// <summary>
        /// Busca todos os produtos relacionados à NF
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public ProdutosNf[] GetByNf(GDASession sessao, uint idNf)
        {
            List<ProdutosNf> lstProd = objPersistence.LoadData(sessao, Sql(idNf, true)).ToList();

            // Verifica se há algum beneficiamento de bisote ou lapidação
            foreach (ProdutosNf pnf in lstProd)
            {
                if (ProdutoNfBenefDAO.Instance.PossuiLapidacao(sessao, pnf.IdProdNf))
                    pnf.DescrProduto += " LAP.";

                if (ProdutoNfBenefDAO.Instance.PossuiBisote(sessao, pnf.IdProdNf))
                    pnf.DescrProduto += " BIS.";
            }

            return lstProd.ToArray();
        }

        /// <summary>
        /// Busca produtos da NF com alguns campos a mais da tabela de produtos
        /// </summary>
        public ProdutosNf[] GetByNfExtended(uint idNf)
        {
            return GetByNfExtended(null, idNf);
        }

        /// <summary>
        /// Busca produtos da NF com alguns campos a mais da tabela de produtos
        /// </summary>
        public ProdutosNf[] GetByNfExtended(GDASession session, uint idNf)
        {
            string sql = @"
                Select pn.*, p.CodInterno, p.Descricao as DescrProduto, p.IdGrupoProd, p.idSubgrupoProd, 
                    um.codigo as Unidade, umt.codigo as UnidadeTrib, p.Espessura 
                From produtos_nf pn 
                    Left Join produto p On (pn.idProd=p.idProd) 
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left Join unidade_medida umt On (p.idUnidadeMedidaTrib=umt.idUnidadeMedida)
                Where pn.idNf=" + idNf + @"
                Order by pn.idProdNf asc";

            List<ProdutosNf> lstProd = objPersistence.LoadData(session, sql).ToList();
            
            // Verifica se há algum beneficiamento de bisote ou lapidação
            if (FiscalConfig.NotaFiscalConfig.AcrescentarLapBisProdutoNota)
            {
                foreach (ProdutosNf pnf in lstProd)
                {
                    if (ProdutoNfBenefDAO.Instance.PossuiLapidacao(session, pnf.IdProdNf))
                        pnf.DescrProduto += " LAP.";

                    if (ProdutoNfBenefDAO.Instance.PossuiBisote(session, pnf.IdProdNf))
                        pnf.DescrProduto += " BIS.";
                }
            }

            return lstProd.ToArray();
        }

        /// <summary>
        /// Busca produtos de uma NF para usar na EFD.
        /// </summary>
        public IList<ProdutosNf> GetForEFD(IEnumerable<Sync.Fiscal.EFD.Entidade.INFe> nf)
        {
            return GetForEFD(nf, false);
        }

        /// <summary>
        /// Busca produtos de uma NF para usar na EFD.
        /// </summary>
        public IList<ProdutosNf> GetForEFD(IEnumerable<Sync.Fiscal.EFD.Entidade.INFe> nf, bool efdContribuicoes)
        {
            var idsNf = string.Join(",", Array.ConvertAll(MetodosExtensao.ToArray(nf), x => x.Codigo.ToString()));
            var sql =
                @"select pnf.*, u.codigo as unidade, c.codInterno as codCfop,
                    p.codInterno as codInterno, p.descricao as descrProduto,
                    round((pnf.total/nf.totalProd)*nf.valorFrete, 2) as valorFrete,
                    round((pnf.total/nf.totalProd)*nf.valorSeguro, 2) as valorSeguro,
                    round((pnf.total/nf.totalProd)*nf.outrasDespesas, 2) as valorOutrasDesp,
                    round((pnf.total/nf.totalProd)*nf.desconto, 2) as valorDesconto,
                    round(pnf.total, 2) as totalEfd, round(pnf.bcPis, 2) as bcPisEfd,
                    round(pnf.bcCofins, 2) as bcCofinsEfd, pcc.codInterno as codInternoContaContabil,
                    p.espessura
                from produtos_nf pnf
                    left join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    left join produto p on (pnf.idProd=p.idProd)
                    left join unidade_medida u on (p.idUnidadeMedida=u.idUnidadeMedida)
                    left join natureza_operacao no on (coalesce(pnf.idNaturezaOperacao, nf.idNaturezaOperacao)=no.idNaturezaOperacao)
                    left join cfop c on (no.idCfop=c.idCfop)
                    left join plano_conta_contabil pcc on (pnf.idContaContabil=pcc.idContaContabil)
                where pnf.idNf in (" + idsNf + ")";

            if (efdContribuicoes)
                sql += " AND pnf.CstCofins <> 70 AND pnf.CstPis <> 70";

            return string.IsNullOrEmpty(idsNf) ? new List<ProdutosNf>() : objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Verifica se produto já foi adicionado

        /// <summary>
        /// Verifica se produto com a altura e largura passadas já foi adicionado
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idProd"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <returns></returns>
        public bool ExistsInNf(uint idNf, uint idProd, Single altura, int largura)
        {
            string sql = "Select count(*) From produtos_nf where idNf=" + idNf + " And idProd=" + idProd +
                " And altura=" + altura.ToString().Replace(',', '.') + " And largura=" + largura;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        /// <summary>
        /// Verifica se produto com a altura e largura passadas já foi adicionado, não sendo o ProdutoNf passado
        /// </summary>
        /// <param name="idProdPed">Item do pedido</param>
        /// <param name="idPedido"></param>
        /// <param name="idProd"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <returns></returns>
        public bool ExistsInNfUpdate(uint idProdNf, uint idNf, uint idProd, Single altura, int largura)
        {
            string sql = "Select count(*) From produtos_nf where idNf=" + idNf + " And idProd=" + idProd +
                " And altura=" + altura.ToString().Replace(',', '.') + " And largura=" + largura + " And idProdNf<>" + idProdNf;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Retorna o total dos produtos da NF

        /// <summary>
        /// Retorna o total dos produtos da NF
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string GetTotalByNf(uint idNf)
        {
            return objPersistence.ExecuteScalar("Select Sum(Round(Total, 2)) From produtos_nf where idNf=" + idNf).ToString();
        }

        /// <summary>
        /// Retorna o total dos produtos da NF por CST
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal GetTotalByNfCST(uint idNf, string cst)
        {
            return ObtemValorCampo<decimal>("Round(Sum(Total), 2)",  "idNf=" + idNf + " and cst=?cst", new GDAParameter("?cst", cst));
        }

        /// <summary>
        /// Retorna o total dos produtos da NF para cálculo do ICMS do simples
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string ObtemTotalCalcSN(uint idNf)
        {
            float percDesconto = NotaFiscalDAO.Instance.ObtemPercDescontoProd(idNf);

            string sql = @"
                Select Round(Sum(Total-(Total*" + percDesconto.ToString().Replace(',', '.') + @")), 2) 
                From produtos_nf 
                Where Csosn In ('101', '201') And idNf=" + idNf;

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Obtém o total de II da nota passada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal ObtemTotalII(uint idNf)
        {
            string sql = "Select Sum(Coalesce(valorIi, 0)) From produtos_nf Where idNf=" + idNf;

            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Obtém o valor de ICMS do produto de nota fiscal informado.
        /// </summary>
        public decimal ObterValorIcms(GDASession session, int idProdNf)
        {
            var sql = string.Format("SELECT COALESCE(ValorIcms, 0) From produtos_nf WHERE IdProdNf={0}", idProdNf);

            return ExecuteScalar<decimal>(session, sql);
        }

        /// <summary>
        /// Obtém o total do produto de nota fiscal informado.
        /// </summary>
        public decimal ObterTotal(GDASession session, int idProdNf)
        {
            return ObtemValorCampo<decimal>(session, "Total", $"IdProdNf={ idProdNf }");
        }

        #endregion

        #region Retorna a última BCICMSST/ICMSST utilizada

        /// <summary>
        /// Retorna a última BCICMSST utilizada no produto passado
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public decimal GetLastBcIcmsSt(uint idProdNf)
        {
            ProdutosNf prodNf = GetElement(idProdNf);

            //if (false)
            //{
            //    string sql = "Select BcIcmsSt From produtos_nf Where idProd=" + prodNf.IdProd + " And idProdNf<>" + prodNf.IdProdNf +
            //        " Order By idProdNf descr limit 1";

            //    return ExecuteScalar<decimal?>(sql).GetValueOrDefault(prodNf.BcIcmsSt);
            //}

            return prodNf.BcIcmsSt;
        }

        /// <summary>
        /// Retorna o último valor de ICMS ST utilizado no produto passado
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public decimal GetLastIcmsSt(uint idProdNf)
        {
            ProdutosNf prodNf = GetElement(idProdNf);

            //if (false)
            //{
            //    string sql = "Select valorIcmsSt From produtos_nf Where idProd=" + prodNf.IdProd + " And idProdNf<>" + prodNf.IdProdNf +
            //        " Order By idProdNf descr limit 1";

            //    return ExecuteScalar<decimal?>(sql).GetValueOrDefault(prodNf.ValorIcmsSt);
            //}

            return prodNf.ValorIcmsSt;
        }

        #endregion

        #region Calcula os impostos do produto e da NFe

        /// <summary>
        /// Calcula o ICMS/IPI/ICMS ST de uma lista de produtos de NF
        /// </summary>
        internal void CalcImposto(GDASession sessao, int idNf, bool atualizarSeNecessario, bool forcarCalculoIcmsSt)
        {
            var produtosNf = GetByNf(sessao, (uint)idNf).ToList();
            CalcImposto(sessao, ref produtosNf, atualizarSeNecessario, forcarCalculoIcmsSt);
        }

        /// <summary>
        /// Calcula o ICMS/IPI/ICMS ST de uma lista de produtos de NF
        /// </summary>
        internal void CalcImposto(GDASession sessao, ref List<ProdutosNf> lstProdNf, bool atualizarSeNecessario, bool forcarCalculoIcmsSt)
        {
            if (lstProdNf == null || lstProdNf.Count == 0)
                return;

            // Busca a Nota Fiscal
            var nf = NotaFiscalDAO.Instance.GetElement(sessao, lstProdNf[0].IdNf);

            // Calcula os impostos dos produtos do pedido
            var impostos = (Data.ICalculoImpostoResultado<Data.Model.ProdutosNf>) 
                CalculadoraImpostoHelper.ObterCalculadora<Model.NotaFiscal, Model.ProdutosNf>()
                    .Calcular(sessao, nf, lstProdNf);

            if (atualizarSeNecessario && !forcarCalculoIcmsSt)
                // Salva os dados dos impostos calculados
                impostos.Salvar(sessao);
        }

        #endregion

        #region Calculo Impostos

        /// <summary>
        /// Atualiza os valores de impostos associados com a instancia informada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoNf"></param>
        public void AtualizarImpostos(GDASession sessao, ProdutosNf produtoNf)
        {
            if (produtoNf.IdProdNf <= 0) return;

            // Relação das propriedades que devem ser atualizadas
            var propriedades = new[]
            {
                //nameof(ProdutosNf.IdNaturezaOperacao),
                nameof(ProdutosNf.AliqIpi),
                nameof(ProdutosNf.ValorIpi),
                nameof(ProdutosNf.AliqIcms),
                nameof(ProdutosNf.BcIcms),
                nameof(ProdutosNf.ValorIcms),
                nameof(ProdutosNf.AliqFcp),
                nameof(ProdutosNf.BcFcp),
                nameof(ProdutosNf.ValorFcp),
                nameof(ProdutosNf.AliqIcmsSt),
                nameof(ProdutosNf.BcIcmsSt),
                nameof(ProdutosNf.ValorIcmsSt),
                nameof(ProdutosNf.AliqFcpSt),
                nameof(ProdutosNf.BcFcpSt),
                nameof(ProdutosNf.ValorFcpSt),
                nameof(ProdutosNf.AliqPis),
                nameof(ProdutosNf.BcPis),
                nameof(ProdutosNf.ValorPis),
                nameof(ProdutosNf.AliqCofins),
                nameof(ProdutosNf.BcCofins),
                nameof(ProdutosNf.ValorCofins),
            };

            objPersistence.Update(sessao, produtoNf, string.Join(",", propriedades), DirectionPropertiesName.Inclusion);
        }

        #endregion

        #region Exclui todos os produtos de uma nota fiscal

        /// <summary>
        /// Exclui todos os produtos de uma nota fiscal
        /// </summary>
        /// <param name="idNf"></param>
        public void DeleteByNotaFiscal(GDASession sessao, uint idNf)
        {
            string sql = "Delete From produtos_nf Where idNf=" + idNf;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Recupera a quantidade total de produtos da nota fiscal

        /// <summary>
        /// Recupera a quantidade total de produtos de uma nota fiscal.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public float QtdeTotal(uint idNf)
        {
            object retorno = objPersistence.ExecuteScalar("select sum(coalesce(qtde, 0)) from produtos_nf where idNf=" + idNf);
            if (retorno == null || retorno.ToString() == "")
                return 0;

            return float.Parse(retorno.ToString());
        }

        #endregion

        #region Recupera todos os produtos em um período de tempo de todas as notas fiscais

        /// <summary>
        /// Retorna todos os produtos cadastrados nas notas fiscais em um período de tempo.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public ProdutosNf[] GetByIdProdPeriodo(uint idProd, DateTime inicio, DateTime fim)
        {
            string sql = Sql(0, true);
            sql += " and pn.idProd=" + idProd + " and pn.idNf in (select idNf from nota_fiscal where dataEmissao>=?inicio and dataEmissao<=?fim)";

            return objPersistence.LoadData(sql, new GDAParameter("?inicio", inicio.Date), new GDAParameter("?fim", fim.Date.AddDays(1).AddMilliseconds(-1))).ToList().ToArray();
        }

        #endregion

        #region Retorna produtos com CFOPs/Aliq ICMS diferentes

        /// <summary>
        /// Retorna produtos com CFOPs/Aliq ICMS diferentes
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IList<ProdutosNf> GetProdNfByCfopAliq(uint idNf)
        {
            var lstProdNf = objPersistence.LoadData(
                @"Select pnf.* From produtos_nf pnf
                    Left Join natureza_operacao no ON (pnf.idNaturezaOperacao=no.idNaturezaOperacao)
                Where idNf=" + idNf + " Group By no.idCfop, pnf.aliqIcms").ToList();

            // Motivo da retirada: Mesmo que a nota possuir apenas um produto, retorna esta listagem, 
            // para que o valor da alíquota do icms não fique errado e gere o sintegra incorretamente.
            //if (lstProdNf.Length <= 1)
            //    return null;

            return lstProdNf;
        }

        #endregion

        #region Retorna o total dos produtos por CFOP

        /// <summary>
        /// Retorna o total dos produtos por CFOP/Aliq. ICMS
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public decimal GetTotalByCfopAliq(uint idNf, uint idCfop, float aliqIcms)
        {
            var valorFreteNf = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>("valorFrete", "idNf=" + idNf);
            var valorSeguroNf = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>("valorSeguro", "idNf=" + idNf);
            var outrasDespesasNf = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>("outrasDespesas", "idNf=" + idNf);
            var descontoNf = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>("desconto", "idNf=" + idNf);
            var totalNotaNf = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>("totalNota", "idNf=" + idNf);

            var lstProdNf = objPersistence.LoadData("Select * From produtos_nf Where idNf=" + idNf + " And idNaturezaOperacao in (" +
                "select idNaturezaOperacao from natureza_operacao where idCfop=" + idCfop + ") And aliqIcms=" + aliqIcms.ToString().Replace(",", ".")).ToArray();

            int qtdProd = objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_nf Where idNf=" + idNf);
            int qtdProdFrete = objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_nf Where idNf=" + idNf + 
                " And idNaturezaOperacao in (select idNaturezaOperacao from natureza_operacao where idCfop=" + idCfop + " And calcIcms=true)");

            var vFreteRateado = qtdProdFrete > 0 ? (valorFreteNf / qtdProdFrete) * lstProdNf.Length : (valorFreteNf / qtdProd) * lstProdNf.Length;
            var vSeguroRateado = (valorSeguroNf / qtdProd) * lstProdNf.Length;
            var vOutrDespRateado = (outrasDespesasNf / qtdProd) * lstProdNf.Length;
            var percDesconto = descontoNf / (totalNotaNf + descontoNf > 0 ? totalNotaNf + descontoNf : 1);

            var totalByCfop = vFreteRateado + vSeguroRateado + vOutrDespRateado;

            // Calcula o total da nota baseado apenas nos produtos com o CFOP passado
            foreach (ProdutosNf pnf in lstProdNf)
                totalByCfop += pnf.Total + pnf.ValorIcmsSt + pnf.ValorIpi;

            return totalByCfop - (totalByCfop * percDesconto);
        }

        /// <summary>
        /// Retorna a BcIcms por CFOP/Aliq. ICMS
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public decimal GetBcIcmsByCfopAliq(uint idNf, uint idCfop, float aliqIcms)
        {
            object bcIcmsByCfop = objPersistence.ExecuteScalar("Select Sum(Coalesce(bcIcms, 0)) From produtos_nf Where idNf=" + idNf + 
                " and idNaturezaOperacao in (select idNaturezaOperacao from natureza_operacao where idCfop=" + idCfop + ")" +
                " And aliqIcms=" + aliqIcms.ToString().Replace(",", "."));

            return bcIcmsByCfop != null ? Glass.Conversoes.StrParaDecimal(bcIcmsByCfop.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o valor do ICMS por CFOP/Aliq. ICMS
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idCfop"></param>
        /// <param name="aliqIcms"></param>
        /// <returns></returns>
        public decimal GetValorIcmsByCfopAliq(uint idNf, uint idCfop, float aliqIcms)
        {
            object valorIcms = objPersistence.ExecuteScalar("Select Sum(Coalesce(valorIcms, 0)) From produtos_nf Where idNf=" + idNf + 
                " and idNaturezaOperacao in (select idNaturezaOperacao from natureza_operacao where idCfop=" + idCfop + ")" + 
                " And aliqIcms=" + aliqIcms.ToString().Replace(",", "."));

            return valorIcms != null ? Glass.Conversoes.StrParaDecimal(valorIcms.ToString()) : 0;
        }

        #endregion

        #region Retorna a quantidade que será usada no DANFE

        /// <summary>
        /// Retorna a quantidade que será usada no DANFE
        /// </summary>
        public float ObtemQtdDanfe(ProdutosNf pnf)
        {
            return ObtemQtdDanfe(null, pnf);
        }

        /// <summary>
        /// Retorna a quantidade que será usada no DANFE
        /// </summary>
        public float ObtemQtdDanfe(GDASession session, ProdutosNf pnf)
        {
            return ObtemQtdDanfe(session, pnf, false);
        }

        public float ObtemQtdDanfe(ProdutosNf pnf, bool nfQtdBaixaM2)
        {
            return ObtemQtdDanfe(null, pnf, nfQtdBaixaM2);
        }

        public float ObtemQtdDanfe(GDASession session, ProdutosNf pnf, bool nfQtdBaixaM2)
        {
            return ObtemQtdDanfe(session, pnf.IdProd, pnf.TotM, pnf.Qtde, pnf.Altura, pnf.Largura, nfQtdBaixaM2, true);
        }

        /// <summary>
        /// Retorna a quantidade que será usada no DANFE
        /// </summary>
        public float ObtemQtdDanfe(uint idProd, float totM2, float qtde, float altura, int largura, bool nfQtdBaixaM2, bool tipoCalcFiscal)
        {
            return ObtemQtdDanfe(null, idProd, totM2, qtde, altura, largura, nfQtdBaixaM2, tipoCalcFiscal);
        }

        /// <summary>
        /// Retorna a quantidade que será usada no DANFE
        /// </summary>
        public float ObtemQtdDanfe(GDASession session, uint idProd, float totM2, float qtde, float altura, int largura, bool nfQtdBaixaM2, bool tipoCalcFiscal)
        {
            int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)idProd, tipoCalcFiscal);

            if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto || (nfQtdBaixaM2 && tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2))
                return totM2 > 0 ? totM2 : Glass.Global.CalculosFluxo.ArredondaM2(session, largura, (int)altura, qtde, (int)idProd, false);
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                return altura * qtde;
            else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro)
                return ((2 * (altura + largura)) / 1000f) * qtde;
            else // Qtd/QtdM2
                return qtde;
        }

        #endregion

        #region Recupera informações dos campos

        /// <summary>
        /// Recupera o id da nota fiscal.
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public uint ObtemIdNf(uint idProdNf)
        {
            return ObtemIdNf(null, idProdNf);
        }

        /// <summary>
        /// Recupera o id da nota fiscal.
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public uint ObtemIdNf(GDASession session, uint idProdNf)
        {
            return ObtemValorCampo<uint>(session, "idNf", "idProdNf=" + idProdNf);
        }
        
        public decimal ObterQtde(GDASession session, int idProdNf)
        {
            return ObtemValorCampo<decimal>(session, "Qtde", string.Format("IdProdNf={0}", idProdNf));
        }

        /// <summary>
        /// Obtém a quantidade que foi dado entrada do produto passado
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public uint ObtemQtdeEntrada(uint idProdNf)
        {
            return ObtemValorCampo<uint>("qtdeEntrada", "idProdNf=" + idProdNf);
        }

        public float ObtemPercentualReducaoBaseCalculo(uint idProdNF)
        {
            return ObtemValorCampo<float>("PERCREDBCICMS", "IDPRODNF =" + idProdNF);
        }

        public float ObtemPercentualReducaoBaseCalculoST(uint idProdNF)
        {
            return ObtemValorCampo<float>("PERCREDBCICMSST", "IDPRODNF =" + idProdNF);
        }

        public float ObtemMVA(uint idProdNF)
        {
            return ObtemValorCampo<float>("MVA", "IDPRODNF =" + idProdNF);
        }

        public Single ObtemAliqICMSST(uint idProdNF)
        {
            return ObtemValorCampo<float>("ALIQICMSST", "IDPRODNF =" + idProdNF);
        }

        #endregion

        #region Busca a posição do produto da nota fiscal

        /// <summary>
        /// Busca a posição do produto da nota fiscal.
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public int GetProdPosition(uint idNf, uint idProdNf)
        {
            return GetProdPosition(null, idNf, idProdNf);
        }

        /// <summary>
        /// Busca a posição do produto da nota fiscal.
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public int GetProdPosition(GDASession session, uint idNf, uint idProdNf)
        {
            string sql = @"Select count(*) From produtos_nf pnf 
                Left Join produto p On (pnf.idProd=p.idProd)
                Left Join nota_fiscal nf On (pnf.idNf=nf.idNf)
                Where pnf.idProdNf<=" + idProdNf + " And pnf.idNf=" + idNf + 
                " And p.tipoMercadoria in (" + (int)TipoMercadoria.MateriaPrima + ")";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, sql).ToString());
        }

        #endregion

        #region Recupera os produtos para entrada de estoque

        /// <summary>
        /// Recupera os produtos para entrada de estoque.
        /// </summary>
        public IList<ProdutosNf> GetForEntradaEstoque(uint numeroNFe)
        {
            var sql = string.Format(@"SELECT pnf.*, p.Descricao AS DescrProduto FROM produtos_nf pnf 
                    INNER JOIN nota_fiscal nf ON (pnf.IdNf=nf.IdNf) INNER JOIN produto p ON (pnf.IdProd=p.IdProd)
                WHERE nf.NumeroNFe={0} AND nf.TipoDocumento IN ({1},{2}) AND pnf.QtdeEntrada < pnf.Qtde",
                numeroNFe, (int)NotaFiscal.TipoDoc.Entrada, (int)NotaFiscal.TipoDoc.EntradaTerceiros);

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Salva informação adicional

        /// <summary>
        /// Salva informação adicional
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <param name="infAdic"></param>
        public void SalvaInfAdic(uint idProdNf, string infAdic)
        {
            string sql = "Update produtos_nf Set infAdic=?infAdic Where idProdNf=" + idProdNf;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?infAdic", infAdic));
        }

        #endregion

        #region Marca entrada de produtos

        /// <summary>
        /// Marca entrada de produto na tabela produtos_nf
        /// </summary>
        public void MarcarEntrada(GDASession session, uint idProdNf, float qtdEntrada, uint idEntradaEstoque)
        {
            string sql = "Update produtos_nf set qtdeEntrada=Coalesce(qtdeEntrada, 0)+" + qtdEntrada.ToString().Replace(",", ".") + " Where idProdNf=" + idProdNf;
            objPersistence.ExecuteCommand(session, sql);

            // Insere um registro na tabela indicando que o produto foi baixado
            if (idEntradaEstoque > 0)
            {
                ProdutoEntradaEstoque novo = new ProdutoEntradaEstoque();
                novo.IdEntradaEstoque = idEntradaEstoque;
                novo.IdProdNf = idProdNf;
                novo.QtdeEntrada = qtdEntrada;

                ProdutoEntradaEstoqueDAO.Instance.Insert(session, novo);
            }
        }

        #endregion

        #region Busca produtos para relatório da lista de notas fiscais por produto

        public IList<ProdutosNf> GetForRptFiscal(NotaFiscal[] nfs, int ordenar)
        {
            if (nfs == null || nfs.Length == 0)
                return null;

            string ids = "";
            foreach (NotaFiscal nf in nfs)
                ids += nf.IdNf + ",";

            string sql = @"
                Select pn.*, p.CodInterno, p.Descricao as DescrProduto, p.IdGrupoProd, p.idSubgrupoProd, nf.numeroNfe, 
                    nf.dataEmissao as dataEmissaoNfe, um.codigo as Unidade, umt.codigo as UnidadeTrib, p.Espessura, 
                    if(nf.tipodocumento not in (3,4),l.razaoSocial,f.razaoSocial) as emitenteNfe, cf.codInterno as CodCfop,
                    coalesce(no.codInterno, cf.codInterno) as CodNaturezaOperacao, if(nf.tipoDocumento=1,f.razaoSocial,if(nf.tipodocumento in (2,4), " + 
                    (FiscalConfig.NotaFiscalConfig.UsarNomeFantasiaNotaFiscal ? "c.nomeFantasia" : "c.nome") +
                    @", l.razaoSocial)) as destinatarioNfe, func.nome as DescrUsuCad
                From produtos_nf pn 
                    Left Join produto p On (pn.idProd=p.idProd) 
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left Join unidade_medida umt On (p.idUnidadeMedidaTrib=umt.idUnidadeMedida)
                    Left Join nota_fiscal nf on (pn.idNf=nf.idNf)
                    Left Join natureza_operacao no on (coalesce(pn.idNaturezaOperacao, nf.idNaturezaOperacao)=no.idNaturezaOperacao)
                    Left Join cfop cf ON (no.idCfop=cf.idCfop)
                    Left Join loja l On (nf.idLoja=l.idLoja) 
                    Left Join fornecedor f On (nf.idFornec=f.idFornec) 
                    Left Join cliente c On (nf.idCliente=c.id_Cli)
                    Left Join funcionario func On (nf.usuCad=func.idFunc)
                Where pn.idNf in (" + ids.TrimEnd(',') + ")";

            switch (ordenar)
            {
                case 1:
                    sql += " order by nf.NumeroNFE desc";
                    break;
                case 2:
                    sql += " order by nf.NumeroNFE asc";
                    break;
                case 3:
                    sql += " order by nf.DataEmissao desc";
                    break;
                case 4:
                    sql += " order by nf.DataEmissao asc";
                    break;
                case 5:
                    sql += " order by nf.DataSaidaEnt desc";
                    break;
                case 6:
                    sql += " order by nf.DataSaidaEnt asc";
                    break;
            }

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca produtos para impressão de etiquetas

        private string SqlImpressaoEtiqueta(uint idNf, uint numeroNFe, uint idFornecedor, string descricaoProd, 
            string dataEmissaoIni, string dataEmissaoFim, uint idCorVidro, float espessura, float alturaMin, float alturaMax, 
            int larguraMin, int larguraMax, bool selecionar)
        {
            string sql = "select " + (selecionar ? "pnf.*, p.descricao as descrProduto, " +
                    FornecedorDAO.Instance.GetNomeFornecedor("f") + " as emitenteNfe, nf.numeroNFe" : "count(*)") + @"
                from produtos_nf pnf
                    inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    inner join fornecedor f on (nf.idFornec=f.idFornec)
                    inner join produto p on (pnf.idProd=p.idProd)
                    inner join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                where nf.situacao In (" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + "," + (int)NotaFiscal.SituacaoEnum.Autorizada + @") 
                    and coalesce(pnf.qtdImpresso,0)<pnf.qtde and pnf.altura>0 and pnf.largura>0 and pnf.qtde>0  and nf.GerarEtiqueta
                    and ((p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + " and p.tipoMercadoria=" +
                    (int)TipoMercadoria.MateriaPrima + ") or s.tipoSubgrupo=" + (int)TipoSubgrupoProd.PVB + @") 
                    and nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída;

            if (idNf > 0)
                sql += " and pnf.idNf=" + idNf;
            else
            {
                if (numeroNFe > 0)
                    sql += " and nf.numeroNFe=" + numeroNFe;

                if (idFornecedor > 0)
                    sql += " and nf.idFornec=" + idFornecedor;

                if (!String.IsNullOrEmpty(descricaoProd))
                    sql += " and pnf.idProd in (" + ProdutoDAO.Instance.ObtemIds(null, descricaoProd) + ")";

                if (!String.IsNullOrEmpty(dataEmissaoIni))
                    sql += " and nf.dataEmissao>=?dataIni";

                if (!String.IsNullOrEmpty(dataEmissaoFim))
                    sql += " and nf.dataEmissao<=?dataFim";
            }

            if (idCorVidro > 0)
                sql += " and p.idCorVidro=" + idCorVidro;

            if (espessura > 0)
                sql += " and p.espessura=" + espessura.ToString().Replace(",", ".");

            if (alturaMin > 0)
                sql += " and pnf.altura>=" + alturaMin.ToString().Replace(",", ".");

            if (alturaMax > 0)
                sql += " and pnf.altura<=" + alturaMax.ToString().Replace(",", ".");

            if (larguraMin > 0)
                sql += " and pnf.largura>=" + larguraMin;

            if (larguraMax > 0)
                sql += " and pnf.largura<=" + larguraMax;

            return sql;
        }

        private GDAParameter[] GetParamsImpressaoEtiqueta(string dataEmissaoIni, string dataEmissaoFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataEmissaoIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataEmissaoIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataEmissaoFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataEmissaoFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<ProdutosNf> GetForImpressaoEtiqueta(uint idNf, uint idCorVidro, float espessura, float alturaMin, 
            float alturaMax, int larguraMin, int larguraMax)
        {
            return objPersistence.LoadData(SqlImpressaoEtiqueta(idNf, 0, 0, null, null, null, idCorVidro, espessura,
                alturaMin, alturaMax, larguraMin, larguraMax, true)).ToList();
        }

        public IList<ProdutosNf> GetListImpressaoEtiqueta(uint numeroNFe, uint idFornecedor,
            string descricaoProd, string dataEmissaoIni, string dataEmissaoFim,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlImpressaoEtiqueta(0, numeroNFe, idFornecedor, descricaoProd,
                dataEmissaoIni, dataEmissaoFim, 0, 0, 0, 0, 0, 0, true), sortExpression, startRow, pageSize, 
                GetParamsImpressaoEtiqueta(dataEmissaoIni, dataEmissaoFim));
        }

        public int GetCountImpressaoEtiqueta(uint numeroNFe, uint idFornecedor,
            string descricaoProd, string dataEmissaoIni, string dataEmissaoFim)
        {
            return GetCountWithInfoPaging(SqlImpressaoEtiqueta(0, numeroNFe, idFornecedor, descricaoProd,
                dataEmissaoIni, dataEmissaoFim, 0, 0, 0, 0, 0, 0, true), GetParamsImpressaoEtiqueta(dataEmissaoIni, dataEmissaoFim));
        }

        public IList<ProdutosNf> GetForImpressaoEtiquetaOrdered(uint numeroNFe, uint idFornecedor,
            string descricaoProd, string dataEmissaoIni, string dataEmissaoFim)
        {
            var param = new ArrayList();
            string sql = SqlImpressaoEtiqueta(0, numeroNFe, idFornecedor, descricaoProd, dataEmissaoIni, dataEmissaoFim, 
                0, 0, 0, 0, 0, 0, true) + " order by p.idCorVidro, p.espessura";

            return objPersistence.LoadData(sql, GetParamsImpressaoEtiqueta(dataEmissaoIni, dataEmissaoFim)).ToList();
        }

        #endregion

        #region Marca a quantidade de determinado item que foi impresso

        /// <summary>
        /// Marca a quantidade de determinado produto que foi impresso
        /// </summary>
        public void MarcarImpressao(GDASession session, uint idProdNf, int qtdImpresso, string obs)
        {
            string sql = "Update produtos_nf set qtdImpresso=coalesce(qtdImpresso,0)+" + qtdImpresso +
                ", obs=?obs Where idProdNf=" + idProdNf;

            objPersistence.ExecuteCommand(session, sql, new GDAParameter[] { new GDAParameter("?obs", obs) });
        }

        /// <summary>
        /// Atualiza a observação da peça
        /// </summary>
        public void AtualizaObs(GDASession session, uint idProdNf, string obs)
        {
            string sql = "Update produtos_pedido_espelho set obs=?obs Where idProdNf=" + idProdNf;

            objPersistence.ExecuteCommand(session, sql, new GDAParameter("?obs", obs));
        }

        #endregion

        #region Retorna a quantidade de peças da nota fiscal

        /// <summary>
        /// Retorna a quantidade de peças da nota fiscal.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemQtdPecasNf(uint idNf)
        {
            var qtdMateriaPrima = ExecuteScalar<decimal>(string.Format(@"
                Select Sum(pnf.qtde) 
                From produtos_nf pnf 
                    Inner Join produto p On (pnf.idProd=p.idProd)
                Where p.tipoMercadoria={0} 
                    And idNf={1}",
                (int)TipoMercadoria.MateriaPrima,
                idNf));
            
            if (qtdMateriaPrima > 0)
                return (int)qtdMateriaPrima;
            else
                throw new Exception("Esta impressão não possui matéria-prima.");

        }

        #endregion

        #region Busca o código do produto a partir da etiqueta

        /// <summary>
        /// Retorna o id de um produto nota fiscal a partir do número da etiqueta, sem joins
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint GetIdByEtiquetaFast(GDASession sessao, string codEtiqueta)
        {
            // Pega o idPedido pelo código da etiqueta
            uint idNf = Glass.Conversoes.StrParaUint(codEtiqueta.Substring(1, codEtiqueta.IndexOf('-') - 1));

            // Pega a posição do produto no pedido pelo código da etiqueta
            int posicao = Glass.Conversoes.StrParaInt(codEtiqueta.Substring(codEtiqueta.IndexOf('-') + 1, codEtiqueta.IndexOf('.') - codEtiqueta.IndexOf('-') - 1));

            string sql = @"
                Select pnf.idProdNf From produtos_nf pnf
                    Inner Join produto p On (pnf.idProd=p.idProd) 
                Where pnf.idNf=" + idNf + @" And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro;

            sql += " Order by pnf.IdProdNf Asc";

            List<uint> lstProd = objPersistence.LoadResult(sessao, sql, null).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            if (lstProd.Count < posicao)
                throw new Exception("Produto da etiqueta não encontrado.");

            return lstProd[posicao - 1];
        }

        /// <summary>
        /// Retorna o id de um produto nota fiscal a partir do número da etiqueta
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint GetIdProdByEtiqueta(GDASession sessao, string codEtiqueta)
        {
            string sql = @"
                SELECT pnf.idProd
                FROM produtos_nf pnf
                    INNER JOIN produto_impressao pi ON (pnf.idProdNf = pi.idProdNf)
                WHERE pi.numEtiqueta = ?numEtq";

            return ExecuteScalar<uint>(sessao, sql, new GDAParameter("?numEtq", codEtiqueta));
        }

        public uint GetIdProdByEtiquetaAtiva(GDASession sessao, string codEtiqueta)
        {
            string sql = @"
                SELECT pnf.idProd
                FROM produtos_nf pnf
                    INNER JOIN produto_impressao pi ON (pnf.idProdNf = pi.idProdNf)
                WHERE !coalesce(cancelado, false) And pi.numEtiqueta = ?numEtq";

            return ExecuteScalar<uint>(sessao, sql, new GDAParameter("?numEtq", codEtiqueta));
        }

        /// <summary>
        /// Retorna o id de um produto nota fiscal a partir do número da etiqueta
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint GetIdProdByEtiqueta(string codEtiqueta)
        {
            return GetIdProdByEtiqueta(null, codEtiqueta);
        }

        /// <summary>
        /// Retorna um produto nota fiscal a partir do número da etiqueta
        /// </summary>
        public ProdutosNf GetProdNfByEtiqueta(GDASession session, string codEtiqueta)
        {
            string sql = @"
                SELECT pnf.*, p.descricao as DescrProduto, p.codInterno
                FROM produtos_nf pnf
                    INNER JOIN produto_impressao pi ON (pnf.idProdNf = pi.idProdNf)
                    INNER JOIN produto p ON (pnf.idProd = p.idProd)
                WHERE pi.idNf = ?idNf 
                    AND pi.posicaoProd = ?posicaoProd 
                    AND pi.itemEtiqueta = ?itemEtiqueta
                    AND pi.qtdeProd = ?qtdeProd";

            var idNf = codEtiqueta.Substring(1, codEtiqueta.IndexOf('-') - 1);
            var posicaoProd = codEtiqueta.Substring(codEtiqueta.IndexOf('-') + 1, codEtiqueta.IndexOf('.') - codEtiqueta.IndexOf('-') - 1);
            var itemEtiqueta = codEtiqueta.Substring(codEtiqueta.IndexOf('.') + 1, codEtiqueta.IndexOf('/') - codEtiqueta.IndexOf('.') - 1);
            var qtdeProd = codEtiqueta.Substring(codEtiqueta.IndexOf('/') + 1);

            return objPersistence.LoadOneData(session, sql, new GDAParameter("?idNf", idNf), new GDAParameter("?posicaoProd", posicaoProd),
                new GDAParameter("?itemEtiqueta", itemEtiqueta), new GDAParameter("?qtdeProd", qtdeProd));
        }

        #endregion

        #region Métodos sobrescritos

        #region Insert

        public uint InsertBase(ProdutosNf objInsert)
        {
            return base.Insert(objInsert);
        }

        /// <summary>
        /// Atualiza o valor da NF ao incluir um produto à mesma
        /// </summary>
        public uint InsertComTransacao(ProdutosNf objInsert)
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

        /// <summary>
        /// Atualiza o valor da NF ao incluir um produto à mesma
        /// </summary>
        public override uint Insert(ProdutosNf objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutosNf objInsert)
        {
            uint returnValue = 0;

            try
            {
                /* Chamado 34268. */
                if (ProdutoImpressaoDAO.Instance.NfPossuiPecaImpressa(null, (int)objInsert.IdNf))
                    throw new Exception("Não é possível inserir produtos nesta nota fiscal porque existem etiquetas associadas à ela.");

                // Se esta NF não puder ser editada, emite o erro.
                int situacao = NotaFiscalDAO.Instance.ObtemSituacao(session, objInsert.IdNf);
                if (situacao != (int)NotaFiscal.SituacaoEnum.Aberta && situacao != (int)NotaFiscal.SituacaoEnum.FalhaEmitir &&
                    situacao != (int)NotaFiscal.SituacaoEnum.NaoEmitida && situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros)
                    throw new Exception("Apenas Notas Fiscais nas situações: Aberta, Não Emitida e Falha ao emitir podem ser alteradas.");

                var tipoDocumentoNotaFiscal = NotaFiscalDAO.Instance.GetTipoDocumento(session, objInsert.IdNf);

                if (!NaturezaOperacaoDAO.Instance.ValidarCfop((int)objInsert.IdNaturezaOperacao.GetValueOrDefault(0), tipoDocumentoNotaFiscal))
                    throw new Exception("A Natureza de operação selecionada não pode ser utilizada em notas desse tipo.");

                uint idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(session, objInsert.IdNf).GetValueOrDefault();
                float totM2 = objInsert.TotM, altura = objInsert.Altura, totM2Calc = 0;
                decimal total = objInsert.Total, custoProd = 0;

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente, (int)objInsert.IdProd, objInsert.Largura, objInsert.Qtde, 1, objInsert.ValorUnitario, 0, false, 0,
                    false, false, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, true, 0);

                objInsert.TotM = totM2;

                // Chamado 15025: Arredondamento criado para resolver diferença da BC ICMS para o Total Prod
                objInsert.Total = Math.Round(total, 2);

                // Calcula o peso do produto
                objInsert.Peso = Utils.CalcPeso((int)objInsert.IdProd, objInsert.Espessura, objInsert.TotM, objInsert.Qtde, objInsert.Altura, true);

                // Se o NCM não tiver sido informado, busca do produto
                if (String.IsNullOrEmpty(objInsert.Ncm))
                {
                    var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(objInsert.IdNf);
                    objInsert.Ncm = ProdutoDAO.Instance.ObtemNcm((int)objInsert.IdProd, idLoja);
                }

                // Informa a altura e largura do item nas observações dos produtos na nota
                var tipoDoc = NotaFiscalDAO.Instance.GetTipoDocumento(session, objInsert.IdNf);
                if (String.IsNullOrEmpty(objInsert.InfAdic) && (tipoDoc == (int)NotaFiscal.TipoDoc.Saída || tipoDoc == (int)NotaFiscal.TipoDoc.Entrada) &&
                    Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(ProdutoDAO.Instance.ObtemIdGrupoProd((int)objInsert.IdProd)))
                {
                    if (FiscalConfig.NotaFiscalConfig.ExibirLarguraEAlturaInfAdicProduto)
                        objInsert.InfAdic = objInsert.Largura + "x" + objInsert.Altura;
                    else if (FiscalConfig.NotaFiscalConfig.ExibirQtdLarguraEAlturaInfAdicProduto &&
                        !Glass.Configuracoes.FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe)
                        objInsert.InfAdic = objInsert.Qtde + " - " + objInsert.Altura + "x" + objInsert.Largura;
                }

                if (objInsert.IdNaturezaOperacao.GetValueOrDefault() == 0)
                    objInsert.IdNaturezaOperacao = NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(session, objInsert.IdNf);

                // Se o MVA não tiver sido informado, busca do produto
                if (objInsert.Mva == 0)
                    objInsert.Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(session, (int)objInsert.IdProd,
                        NotaFiscalDAO.Instance.ObtemIdLoja(session, objInsert.IdNf),
                        (int?)NotaFiscalDAO.Instance.ObtemIdFornec(session, objInsert.IdNf),
                        NotaFiscalDAO.Instance.ObtemIdCliente(session, objInsert.IdNf),
                        (tipoDoc == (int)NotaFiscal.TipoDoc.Saída ||
                        /* Chamado 32984 e 39660. */
                        (tipoDoc == (int)NotaFiscal.TipoDoc.Entrada &&
                        CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(session, objInsert.IdNaturezaOperacao.Value)))));

                objInsert.Mva = (float)Math.Round((decimal)objInsert.Mva, 2);

                //Se nao for cst de origem 3, 5 ou 8 apaga o numero de controle da FCI caso esteja preenchido
                if (objInsert.CstOrig != 3 && objInsert.CstOrig != 5 && objInsert.CstOrig != 8)
                    objInsert.NumControleFciStr = null;

                objInsert.CstCofins = objInsert.CstPis;
                objInsert.BcCofins = objInsert.BcPis;

                var produtosNf = GetByNf(session, objInsert.IdNf).ToList();
                produtosNf.Add(objInsert);

                // Calcula Impostos
                CalcImposto(session, ref produtosNf, true, false);

                /* Chamado 63976. */
                objInsert = produtosNf.Where(f => f.IdProdNf <= 0).FirstOrDefault();

                returnValue = base.Insert(session, objInsert);
                //LogAlteracaoDAO.Instance.LogProdutoNotaFiscal(new ProdutosNf(), LogAlteracaoDAO.SequenciaObjeto.Atual);

                /* Chamado 14947.
                 * É necessário que a nota de devolução do tipo EntradaTerceiros atualize o valor total da nota. */
                // Atualiza os totais da nota fiscal se for emissão normal
                if (NotaFiscalDAO.Instance.ObtemFinalidade(session, objInsert.IdNf) == (int)NotaFiscal.FinalidadeEmissaoEnum.Normal ||
                    NotaFiscalDAO.Instance.ObtemFinalidade(session, objInsert.IdNf) == (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao)
                    NotaFiscalDAO.Instance.UpdateTotalNf(session, objInsert.IdNf);

                // Busca observação da CFOP do produto e salva nas informações complementares da nota
                if (NotaFiscalDAO.Instance.GetTipoDocumento(session, objInsert.IdNf) == (int)NotaFiscal.TipoDoc.Saída && objInsert.IdCfop > 0 &&
                    objInsert.IdCfop != NotaFiscalDAO.Instance.GetIdCfop(session, objInsert.IdNf))
                {
                    string obsCfop = CfopDAO.Instance.GetObs(session, objInsert.IdCfop.Value);
                    string infCompl = NotaFiscalDAO.Instance.ObtemValorCampo<string>(session, "infCompl", "idNf=" + objInsert.IdNf);

                    if (!String.IsNullOrEmpty(obsCfop) && (infCompl == null || !infCompl.Contains(obsCfop)))
                        NotaFiscalDAO.Instance.InsereInfCompl(session, objInsert.IdNf, (!String.IsNullOrEmpty(infCompl) ? ". " : "") + obsCfop);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Produto na NF. Erro: " + ex.Message);
            }

            return returnValue;
        }

        #endregion

        #region Update

        public int UpdateComTransacao(ProdutosNf objUpdate)
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

        public override int Update(ProdutosNf objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, ProdutosNf objUpdate)
        {
            try
            {
                // Recuperar possíveis valores referente à nota de importação, para não perdê-los
                ProdutosNf prodNfOld = GetElement(session, objUpdate.IdProdNf);
                /* Chamado 34268. */
                if (ProdutoImpressaoDAO.Instance.VerificarPossuiImpressao(session, (int)objUpdate.IdProdNf))
                {
                    /* Chamado 46033. */
                    if (prodNfOld.IdProd != objUpdate.IdProd ||
                        prodNfOld.Qtde != objUpdate.Qtde ||
                        prodNfOld.Altura != objUpdate.Altura ||
                        prodNfOld.Largura != objUpdate.Largura ||
                        prodNfOld.TotM != objUpdate.TotM ||
                        ((string.IsNullOrEmpty(prodNfOld.Lote) && string.IsNullOrEmpty(objUpdate.Lote)) == true ? false : (prodNfOld.Lote != objUpdate.Lote)) ||
                        prodNfOld.TipoMercadoria != objUpdate.TipoMercadoria)
                        throw new Exception("Não é possível alterar a quantidade, altura, largura, " +
                            "M2, lote ou tipo de mercadoria do produto, pois, existem etiquetas associadas à nota fiscal. " +
                            "Cancele as etiquetas da nota para conseguir alterá-lo.");
                }

                // Se esta NF não puder ser editada, emite o erro.
                int situacao = NotaFiscalDAO.Instance.ObtemSituacao(session, objUpdate.IdNf);
                if (situacao != (int)NotaFiscal.SituacaoEnum.Aberta && situacao != (int)NotaFiscal.SituacaoEnum.FalhaEmitir &&
                    situacao != (int)NotaFiscal.SituacaoEnum.NaoEmitida && situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros &&
                    (situacao == (int)NotaFiscal.SituacaoEnum.Autorizada && !NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(session, objUpdate.IdNf)))
                    throw new Exception("Apenas Notas Fiscais nas situações: Aberta, Não Emitida e Falha ao emitir podem ser alteradas.");

                var tipoDocumentoNotaFiscal = NotaFiscalDAO.Instance.GetTipoDocumento(session, objUpdate.IdNf);

                if (!NaturezaOperacaoDAO.Instance.ValidarCfop((int)objUpdate.IdNaturezaOperacao.GetValueOrDefault(0), tipoDocumentoNotaFiscal))
                    throw new Exception("A Natureza de operação selecionada não pode ser utilizada em notas desse tipo.");

                if (objUpdate.Cst != "20" && objUpdate.Cst != "70")
                {
                    if (objUpdate.CodValorFiscal != 1)
                    {
                        objUpdate.PercRedBcIcms = 0;
                        objUpdate.PercRedBcIcmsSt = 0;
                    }
                }

                // Calcula o peso do produto
                objUpdate.Peso = Utils.CalcPeso((int)objUpdate.IdProd, objUpdate.Espessura, objUpdate.TotM, objUpdate.Qtde, objUpdate.Altura, true);

                objUpdate.NumDocImp = prodNfOld.NumDocImp;
                objUpdate.DataRegDocImp = prodNfOld.DataRegDocImp;
                objUpdate.LocalDesembaraco = prodNfOld.LocalDesembaraco;
                objUpdate.UfDesembaraco = prodNfOld.UfDesembaraco;
                objUpdate.DataDesembaraco = prodNfOld.DataDesembaraco;
                objUpdate.CodExportador = prodNfOld.CodExportador;
                objUpdate.BcIi = prodNfOld.BcIi;
                objUpdate.DespAduaneira = prodNfOld.DespAduaneira;
                objUpdate.ValorIi = prodNfOld.ValorIi;
                objUpdate.ValorIof = prodNfOld.ValorIof;
                objUpdate.InfAdic = prodNfOld.InfAdic;

                objUpdate.CstCofins = objUpdate.CstPis;
                objUpdate.BcCofins = objUpdate.BcPis;
                objUpdate.VAFRMM = prodNfOld.VAFRMM;
                objUpdate.TpViaTransp = prodNfOld.TpViaTransp;
                objUpdate.TpIntermedio = prodNfOld.TpIntermedio;

                // Verifica se a nota não está finalizada (não é correção manual da nota) ou se não é de importação
                if (!NotaFiscalDAO.Instance.IsFinalizada(session, objUpdate.IdNf) && !NotaFiscalDAO.Instance.IsNotaFiscalImportacao(session, objUpdate.IdNf))
                {
                    uint idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(session, objUpdate.IdNf).GetValueOrDefault();
                    Single totM2 = objUpdate.TotM, altura = objUpdate.Altura, totM2Calc = 0;
                    decimal total = objUpdate.Total, custoProd = 0;

                    Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, idCliente, (int)objUpdate.IdProd, objUpdate.Largura, objUpdate.Qtde, 1, objUpdate.ValorUnitario, 0, false, 0, false,
                        false, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, true, 0);

                    objUpdate.TotM = totM2;

                    // Chamado 15025: Arredondamento criado para resolver diferença da BC ICMS para o Total Prod
                    objUpdate.Total = Math.Round(total, 2);                    
                    //objUpdate.Total = Math.Round(total,4);

                    // Se o NCM não tiver sido informado, busca de produto
                    if (String.IsNullOrEmpty(objUpdate.Ncm))
                    {
                        var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(objUpdate.IdNf);
                        objUpdate.Ncm = ProdutoDAO.Instance.ObtemNcm((int)objUpdate.IdProd, idLoja);
                    }

                    var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(session, objUpdate.IdNf);

                    // Se o MVA não tiver sido informado, busca do produto
                    if (objUpdate.Mva == 0)
                        objUpdate.Mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(session, (int)objUpdate.IdProd,
                            NotaFiscalDAO.Instance.ObtemIdLoja(session, objUpdate.IdNf),
                            (int?)NotaFiscalDAO.Instance.ObtemIdFornec(session, objUpdate.IdNf),
                            NotaFiscalDAO.Instance.ObtemIdCliente(session, objUpdate.IdNf),
                            (tipoDocumento == (int)NotaFiscal.TipoDoc.Saída ||
                            /* Chamado 32984 e 39660. */
                            (tipoDocumento == (int)NotaFiscal.TipoDoc.Entrada &&
                            CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(session, objUpdate.IdNaturezaOperacao.Value)))));

                    objUpdate.Mva = (float)Math.Round((decimal)objUpdate.Mva, 2);

                    // Calcula os impostos do produto já atualizando o mesmo e os totais da Nota, desde que não seja nota de ajuste
                    if (NotaFiscalDAO.Instance.ObtemFinalidade(session, objUpdate.IdNf) != (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste)
                    {
                        var produtosNf = GetByNf(session, objUpdate.IdNf).ToList();

                        //Remove o produto da lista que foi adicionado para o calculo do imposto.
                        var prodRemover = produtosNf.Where(f => f.IdProdNf == objUpdate.IdProdNf).FirstOrDefault();
                        produtosNf.Remove(prodRemover);

                        produtosNf.Add(objUpdate);

                        CalcImposto(session, ref produtosNf, true, false);

                        /* Chamado 63976. */
                        objUpdate = produtosNf.Where(f => f.IdProdNf == objUpdate.IdProdNf).FirstOrDefault();
                    }
                }

                //Se nao for cst de origem 3, 5 ou 8 apaga o numero de controle da FCI caso esteja preenchido
                if (objUpdate.CstOrig != 3 && objUpdate.CstOrig != 5 && objUpdate.CstOrig != 8)
                    objUpdate.NumControleFciStr = null;

                // Atualiza o produto
                LogAlteracaoDAO.Instance.LogProdutoNotaFiscal(prodNfOld, LogAlteracaoDAO.SequenciaObjeto.Atual);
                base.Update(session, objUpdate);

                /* Chamado 14947.
                 * É necessário que a nota de devolução do tipo EntradaTerceiros atualize o valor total da nota. */
                // Atualiza os totais da nota fiscal se for emissão normal
                if (NotaFiscalDAO.Instance.ObtemFinalidade(session, objUpdate.IdNf) == (int)NotaFiscal.FinalidadeEmissaoEnum.Normal ||
                    NotaFiscalDAO.Instance.ObtemFinalidade(session, objUpdate.IdNf) == (int)NotaFiscal.FinalidadeEmissaoEnum.Devolucao)
                    NotaFiscalDAO.Instance.UpdateTotalNf(session, objUpdate.IdNf);

                // Busca observação da CFOP do produto e salva nas informações complementares da nota
                if (NotaFiscalDAO.Instance.GetTipoDocumento(session, objUpdate.IdNf) == (int)NotaFiscal.TipoDoc.Saída && objUpdate.IdCfop > 0 &&
                    objUpdate.IdCfop != NotaFiscalDAO.Instance.GetIdCfop(session, objUpdate.IdNf))
                {
                    string obsCfop = CfopDAO.Instance.GetObs(session, objUpdate.IdCfop.Value);
                    string infCompl = NotaFiscalDAO.Instance.ObtemValorCampo<string>(session, "infCompl", "idNf=" + objUpdate.IdNf);

                    if (!String.IsNullOrEmpty(obsCfop) && (infCompl == null || !infCompl.Contains(obsCfop)))
                        NotaFiscalDAO.Instance.InsereInfCompl(session, objUpdate.IdNf, (!String.IsNullOrEmpty(infCompl) ? ". " : "") + obsCfop);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Produto da NF. Erro: " + ex.Message);
            }

            return 1;
        }

        public void UpdateInfoAdic(ProdutosNf objUpdate)
        {
            string sql = @"update produtos_nf set numDocImp=?di, dataRegDocImp=?dataDi, localDesembaraco=?ld,
                ufDesembaraco=?ufd, dataDesembaraco=?dd, codExportador=?ce, bcii=?bcii, despAduaneira=?da,
                valorIi=?ii, valorIof=?iof, tpViaTransp=?tpViaTransp, vAFRMM=?vAFRMM,
                cnpjAdquirenteEncomendante=?cnpjAdquirenteEncomendante, tpIntermedio=?tpIntermedio,
                ufTerceiro=?ufTerceiro where idProdNf=" + objUpdate.IdProdNf;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?di", objUpdate.NumDocImp),
                new GDAParameter("?dataDi", objUpdate.DataRegDocImp), new GDAParameter("?ld", objUpdate.LocalDesembaraco),
                new GDAParameter("?ufd", objUpdate.UfDesembaraco), new GDAParameter("?dd", objUpdate.DataDesembaraco),
                new GDAParameter("?ce", objUpdate.CodExportador), new GDAParameter("?bcii", objUpdate.BcIi),
                new GDAParameter("?da", objUpdate.DespAduaneira), new GDAParameter("?ii", objUpdate.ValorIi),
                new GDAParameter("?iof", objUpdate.ValorIof), new GDAParameter("?tpViaTransp", objUpdate.TpViaTransp),
                new GDAParameter("?vAFRMM", objUpdate.VAFRMM), new GDAParameter("?cnpjAdquirenteEncomendante", objUpdate.CnpjAdquirenteEncomendante),
                new GDAParameter("?tpIntermedio", objUpdate.TpIntermedio),
                new GDAParameter("?ufTerceiro", objUpdate.UfTerceiro));
        }

        #endregion

        #region Delete

        public int DeleteProdutoNf(GDASession sessao, uint idProdNf)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produtos_nf Where IdProdNf=" + idProdNf) == 0)
                return 0;

            int returnValue = 0;
            uint idNf = ObtemValorCampo<uint>(sessao, "idNf", "idProdNf=" + idProdNf);

            var prod = GetElement(sessao, idProdNf);

            // Se esta NF não puder ser editada, emite o erro.
            int situacao = NotaFiscalDAO.Instance.ObtemSituacao(sessao, idNf);
            if (situacao != (int)NotaFiscal.SituacaoEnum.Aberta && situacao != (int)NotaFiscal.SituacaoEnum.FalhaEmitir &&
                situacao != (int)NotaFiscal.SituacaoEnum.NaoEmitida && situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros)
                throw new Exception("Apenas Notas Fiscais nas situações: Aberta, Não Emitida e Falha ao emitir podem ser alteradas.");

            try
            {
                LogAlteracaoDAO.Instance.ApagaLogProdutoNotaFiscal(idProdNf);
                ProdutoNfCustoDAO.Instance.ApagarPorProdutoNf(sessao, idProdNf);
                ProdutoNfBenefDAO.Instance.DeleteByProdNf(sessao, idProdNf);
                ProdutoNfRentabilidadeDAO.Instance.ApagarPorProdutoNf(sessao, idProdNf);
                returnValue = GDAOperations.Delete(sessao, prod);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir Produto da NF. Erro: " + ex.Message);
            }

            try
            {
                /* Chamado 63976. */
                CalcImposto(sessao, (int)idNf, true, false);

                NotaFiscalDAO.Instance.UpdateTotalNf(sessao, idNf);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Valor da NF. Erro: " + ex.Message);
            }
            
            return returnValue;
        }

        /// <summary>
        /// Atualiza o valor da NF ao excluir um produto da mesma
        /// </summary>
        public int DeleteComTransacao(ProdutosNf objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = DeleteProdutoNf(transaction, objDelete.IdProdNf);

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

        /// <summary>
        /// Atualiza o valor da NF ao excluir um produto da mesma
        /// </summary>
        public override int Delete(ProdutosNf objDelete)
        {
            return DeleteComTransacao(objDelete);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteComTransacao(GetElement(Key));
        }

        #endregion

        #endregion

        #region FCI

        #region Calcula o conteudo de importação do produto

        /// <summary>
        /// Realiza o calculo do conteudo de importação de um produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="periodoApuracao"></param>
        /// <returns></returns>
        public decimal CalculaConteudoImportacao(uint idProd, DateTime periodoApuracao)
        {
            return Math.Round((CalculaValorParcelaImportada(idProd, periodoApuracao) / CalculaValorSaidaInterestadual(idProd, periodoApuracao)) * 100, 2);
        }

        /// <summary>
        /// Realiza o calculo do valor da parcela importada de um produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="periodoApuracao"></param>
        /// <returns></returns>
        public decimal CalculaValorParcelaImportada(uint idProd, DateTime periodoApuracao)
        {
            #region Variaveis

            decimal valorTotal = 0, valorParcelaImportada = 0;
            float qtdeTotal = 0;
            List<ProdutoBaixaEstoque> pbes;
            uint? idProdBase = null;
            List<uint> idsChapas = null;

            int mes = 0, ano = 0;

            if (periodoApuracao.Month == 1)
            {
                mes = 11;
                ano = periodoApuracao.Year - 1;
            }
            else
            {
                mes = periodoApuracao.Month - 2;
                ano = periodoApuracao.Year;
            }

            #endregion

            #region Sql

            string sqlParcelaImportada = @"
                SELECT pnf.*
                FROM produtos_nf pnf
                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                WHERE nf.situacao IN(" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + @")
                    AND nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + @"
                    AND pnf.cstOrig<> 0 AND pnf.cstOrig<> 4
                    AND pnf.idProd IN({0})
                    AND MONTH(nf.dataEmissao)={1}
                    AND YEAR(nf.dataEmissao)={2}";

            string sqlProdsChapa = @"
                SELECT p.idProd
                FROM produto p
                WHERE p.idProdBase={0}";

            #endregion

            //Materias-primas
            idsChapas = new List<uint>();
            pbes = new List<ProdutoBaixaEstoque>(ProdutoBaixaEstoqueDAO.Instance.GetByProd(idProd, false));
            idProdBase = ProdutoDAO.Instance.ObtemValorCampo<uint?>("idProdBase", "idProd=" + idProd);

            //Se o produto for um comum cortado ou laminado cortado
            if (idProdBase.GetValueOrDefault(0) == 0 && pbes.Count == 0)
            {
                idsChapas = objPersistence.LoadResult(string.Format(sqlProdsChapa, idProd)).Select(f=>f.GetUInt32(0)).ToList();

            }//Se o produto for um temperado ou laminado temperado
            else if (idProdBase.GetValueOrDefault(0) == 0 && pbes.Count == 1)
            {
                idsChapas = objPersistence.LoadResult(string.Format(sqlProdsChapa, pbes[0].IdProdBaixa)).Select(f => f.GetUInt32(0)).ToList();

            }//Se o produto for uma chapa comum
            else if (idProdBase.GetValueOrDefault(0) > 0 && pbes.Count == 0)
            {
                idsChapas.Add(idProd);

            }//Se o produto for uma chapa de laminado
            else if (idProdBase.GetValueOrDefault(0) > 0 && pbes.Count > 1)
            {
                foreach (var pbe in pbes)
                {
                    #region Busca entradas

                    //Busca as importações que o produto teve
                    var prodsNfImp = objPersistence.LoadData(string.Format(sqlParcelaImportada, pbe.IdProdBaixa, mes, ano)).ToList();

                    //verifica se houve imporação no mês ou busca nos ultimos meses
                    if (prodsNfImp == null || prodsNfImp.Count == 0)
                    {
                        int count = 0;

                        //Se não encontrar vai buscando nos ultimos 24 meses
                        while (count < 24 && (prodsNfImp == null || prodsNfImp.Count == 0))
                        {
                            if (mes == 1) { mes = 12; ano--; }
                            else { mes--; }

                            prodsNfImp = objPersistence.LoadData(string.Format(sqlParcelaImportada, pbe.IdProdBaixa, mes, ano)).ToList();

                            count++;
                        }
                    }

                    #endregion

                    #region Calculo das entradas

                    if (prodsNfImp != null && prodsNfImp.Count > 0)
                    {
                        foreach (var prodNf in prodsNfImp)
                        {
                            if (prodNf.CstOrig == 3)
                                valorTotal += prodNf.Total * 50 / 100;
                            else if (prodNf.CstOrig == 5)
                                valorTotal += 0;
                            else
                                valorTotal += prodNf.Total;

                            qtdeTotal += ObtemQtdDanfe(prodNf);
                        }

                        valorParcelaImportada += (valorTotal / Convert.ToDecimal(qtdeTotal)) * Convert.ToDecimal(pbe.Qtde);
                    }
                    else
                    {
                        var produto = ProdutoDAO.Instance.GetByIdProd((uint)pbe.IdProdBaixa);

                        if (produto.CustoCompra > 0)
                            valorParcelaImportada += produto.CustoCompra * Convert.ToDecimal(pbe.Qtde);
                        else if (produto.Custofabbase > 0)
                            valorParcelaImportada += produto.Custofabbase * Convert.ToDecimal(pbe.Qtde);
                    }

                    #endregion
                }

                return Math.Round(valorParcelaImportada, 2);
            }

            foreach (var idChapa in idsChapas)
            {
                pbes = new List<ProdutoBaixaEstoque>(ProdutoBaixaEstoqueDAO.Instance.GetByProd(idChapa, false));

                //Verifica se é uma chapa de laminado
                if (pbes.Count > 1)
                {
                    valorParcelaImportada += CalculaValorParcelaImportada(idChapa, periodoApuracao);
                }
                else
                {
                    #region Busca entradas

                    //Busca as importações que o produto teve
                    var prodsNfImp = objPersistence.LoadData(string.Format(sqlParcelaImportada, idChapa, mes, ano)).ToList();

                    //verifica se houve imporação no mês ou busca nos ultimos meses
                    if (prodsNfImp == null || prodsNfImp.Count == 0)
                    {
                        int count = 0;

                        //Se não encontrar vai buscando nos ultimos 24 meses
                        while (count < 24 && (prodsNfImp == null || prodsNfImp.Count == 0))
                        {
                            if (mes == 1) { mes = 12; ano--; }
                            else { mes--; }

                            prodsNfImp = objPersistence.LoadData(string.Format(sqlParcelaImportada, idChapa, mes, ano)).ToList();

                            count++;
                        }
                    }

                    #endregion

                    #region Calculo das entradas

                    if (prodsNfImp != null && prodsNfImp.Count > 0)
                    {
                        foreach (var prodNf in prodsNfImp)
                        {
                            if (prodNf.CstOrig == 3)
                                valorTotal += prodNf.Total * 50 / 100;
                            else if (prodNf.CstOrig == 5)
                                valorTotal += 0;
                            else
                                valorTotal += prodNf.Total;

                            qtdeTotal += ObtemQtdDanfe(prodNf);
                        }

                        valorParcelaImportada += valorTotal / Convert.ToDecimal(qtdeTotal);
                    }
                    else
                    {
                        var produto = ProdutoDAO.Instance.GetByIdProd(idChapa);

                        if (produto.CustoCompra > 0)
                            valorParcelaImportada += produto.CustoCompra;
                        else if (produto.Custofabbase > 0)
                            valorParcelaImportada += produto.Custofabbase;
                    }

                    #endregion
                }
            }

            if (idsChapas.Count == 0)
                return 0;

            if (idsChapas.Count == 0)
                return 0;

            return Math.Round(valorParcelaImportada / idsChapas.Count, 2);
        }

        /// <summary>
        /// Realiza o calculo do valor da saída interestadual de um produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="periodoApuracao"></param>
        /// <returns></returns>
        public decimal CalculaValorSaidaInterestadual(uint idProd, DateTime periodoApuracao)
        {
            #region Variaveis

            decimal valorTotal = 0, valorOperacaoInterestadual = 0;
            float qtdeTotal = 0;

            int mes = 0, ano = 0;

            if (periodoApuracao.Month == 1)
            {
                mes = 11;
                ano = periodoApuracao.Year - 1;
            }
            else
            {
                mes = periodoApuracao.Month - 2;
                ano = periodoApuracao.Year;
            }

            #endregion

            #region Sql

            string sqlSaida = @"
                SELECT pnf.*
                FROM produtos_nf pnf
                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                WHERE nf.situacao =" + (int)NotaFiscal.SituacaoEnum.Autorizada + @"
                    AND nf.tipoDocumento=" + (int)NotaFiscal.TipoDoc.Saída + @"
                    AND pnf.idProd={0}
                    AND MONTH(nf.dataEmissao)={1}
                    AND YEAR(nf.dataEmissao)={2}";

            #endregion

            #region buca as saídas

            //Busca as saidas que o produto teve
            var prodsNfSaida = objPersistence.LoadData(string.Format(sqlSaida, idProd, mes, ano)).ToList();

            //verifica se houve imporação no mês ou busca nos ultimos meses
            if (prodsNfSaida == null || prodsNfSaida.Count == 0)
            {
                int count = 0;

                //Se não encontrar vai buscando nos ultimos 24 meses
                while (prodsNfSaida == null || count < 24)
                {
                    if (mes == 1) { mes = 12; ano--; }
                    else { mes--; }

                    prodsNfSaida = objPersistence.LoadData(string.Format(sqlSaida, idProd, mes, ano)).ToList();

                    count++;
                }
            }

            #endregion

            #region Calculo das saídas

            if (prodsNfSaida != null && prodsNfSaida.Count > 0)
            {
                foreach (var prodNf in prodsNfSaida)
                {
                    valorTotal += prodNf.Total;
                    qtdeTotal += ObtemQtdDanfe(prodNf);
                }

                valorOperacaoInterestadual += valorTotal / Convert.ToDecimal(qtdeTotal);
            }
            else
            {
                var produto = ProdutoDAO.Instance.GetByIdProd(idProd);

                if (produto.ValorBalcao > 0)
                    valorOperacaoInterestadual += produto.ValorBalcao;
                else if (produto.ValorAtacado > 0)
                    valorOperacaoInterestadual += produto.ValorAtacado;
                else if (produto.ValorObra > 0)
                    valorOperacaoInterestadual += produto.ValorObra;
            }

            #endregion

            return Math.Round(valorOperacaoInterestadual, 2);
        }

        #endregion

        #region Buscar Produtos da Nf para gerar FCI

        /// <summary>
        /// Buscar Produtos da Nf para gerar FCI
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public string GetIdsProdForGerarFci(uint idNf)
        {
            string sql = @"
                SELECT GROUP_CONCAT(CONCAT(pnf.idProd, ';', pnf.idProdNf))
                FROM produtos_nf pnf
                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                WHERE pnf.cstOrig IN (3, 5, 8) AND nf.idNf=" + idNf;

            return ExecuteScalar<string>(sql);

        }

        #endregion

        /// <summary>
        /// Atualiza os dados da fci do produto da nf
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <param name="parcelaImportada"></param>
        /// <param name="saidaInterestadual"></param>
        /// <param name="conteudoImportacao"></param>
        /// <param name="numControleFci"></param>
        public void AtualizaDadosFCIProdutoNf(uint idProdNf, decimal parcelaImportada, decimal saidaInterestadual,
            decimal conteudoImportacao, string numControleFci)
        {
            // Se esta NF não puder ser editada, emite o erro.
            var idNf = ObtemIdNf(idProdNf);
            int situacao = NotaFiscalDAO.Instance.ObtemSituacao(idNf);
            if (situacao != (int)NotaFiscal.SituacaoEnum.Aberta && situacao != (int)NotaFiscal.SituacaoEnum.FalhaEmitir &&
                situacao != (int)NotaFiscal.SituacaoEnum.NaoEmitida && situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros &&
                (situacao == (int)NotaFiscal.SituacaoEnum.Autorizada && !NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(null, idNf)))
                return;

            string sql = @"
                UPDATE produtos_nf
                SET parcelaImportada = ?percImp,
                    saidaInterEstadual = ?saidaInter,
                    conteudoImportacao = ?contImp,
                    numControleFci= ?fci
                WHERE idProdNf=" + idProdNf;

            var fci = new Guid(numControleFci).ToByteArray();

            objPersistence.ExecuteCommand(sql, new GDAParameter("?percImp", parcelaImportada),
                new GDAParameter("?saidaInter", saidaInterestadual), new GDAParameter("?contImp", conteudoImportacao), new GDAParameter("?fci", fci));
        }

        #endregion

        #region Faz o rateio do Frete/Seguro/Despesas/Descontos

        /// <summary>
        /// Calcula o Frete/Seguro/Despesas/Descontos de um produto de NF
        /// </summary>
        /// <param name="lstProdNf"></param>
        internal void CalcDespesas(GDASession sessao, ref ProdutosNf prodNf)
        {
            if (prodNf == null)
                return;

            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(sessao, prodNf.IdNf);

            var qtdeProd = ExecuteScalar<int>("SELECT count(*) FROM produtos_nf WHERE IdNf = " + prodNf.IdNf);

            var totalProd = ExecuteScalar<decimal>(sessao, "Select Sum(total) From produtos_nf Where idNf=" + nf.IdNf);
            decimal percDesconto = (nf.Desconto / (totalProd > 0 ? totalProd : 1));

            prodNf.ValorFrete = nf.TotalProd > 0 ? (prodNf.Total / nf.TotalProd) * nf.ValorFrete : nf.ValorFrete / qtdeProd;
            prodNf.ValorSeguro = nf.TotalProd > 0 ? (prodNf.Total / nf.TotalProd) * nf.ValorSeguro : nf.ValorSeguro / qtdeProd;
            prodNf.ValorOutrasDespesas = nf.TotalProd > 0 ? (prodNf.Total / nf.TotalProd) * nf.OutrasDespesas : nf.OutrasDespesas / qtdeProd;
            prodNf.ValorDesconto = Math.Round(percDesconto * Math.Round(prodNf.Total, 2), 2);
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Recupera os produtos da nota para o calculo da rentabilidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IList<ProdutosNf> ObterProdutosParaRentabilidade(GDA.GDASession sessao, uint idNf)
        {
            return objPersistence.LoadData(sessao,
                "SELECT * FROM produtos_nf WHERE IdNf=?id",
                new GDAParameter("?id", idNf))
                .ToList();
        }

        /// <summary>
        /// Atualiza a rentabilidade do produto da nota fiscal.
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            uint idProdNf, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE produtos_nf SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdProdNf=?id",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?id", idProdNf));
        }

        #endregion
    }
}