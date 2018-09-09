using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ProdutoDAO : BaseCadastroDAO<Produto, ProdutoDAO>
    {
        //private ProdutoDAO() { }

        #region Listagem padrão

        private enum TipoBusca
        {
            Normal,
            SugestaoCompra,
            SugestaoProducao
        }



        internal string SqlPendenteCompra(string aliasProduto, string aliasProdutoLoja)
        {
            return @"Left Join (
                    select pc.idProd, c.idLoja, sum(pc.totM) as totMComprando, sum(pc.qtde) as qtdeComprando
                    from produtos_compra pc
                        inner Join compra c On (pc.idCompra=c.idCompra)
                    where c.situacao in (" + (int)Compra.SituacaoEnum.Ativa + "," + (int)Compra.SituacaoEnum.Finalizada + "," + (int)Compra.SituacaoEnum.AguardandoEntrega + @")
                        and (c.estoqueBaixado=false or c.estoqueBaixado is null)
                    group by pc.idProd" +
                        (!String.IsNullOrEmpty(aliasProdutoLoja) ? ", c.idLoja" : "") + @"
                ) as pc On (" + aliasProduto + ".idProd=pc.idProd" +
                (!String.IsNullOrEmpty(aliasProdutoLoja) ? " and " + aliasProdutoLoja + ".idLoja=pc.idLoja" : "") + ") ";
        }

        internal string SqlPendenteProducao(string aliasProduto, string aliasProdutoLoja, string aliasPedido)
        {
            // Define que a peça não entrará na coluna Produzindo se a mesma tiver passado no setor do tipo "Forno";
            bool desconsiderarProduzindoPosForno = ProducaoConfig.SairDeProduzindoSePassarNoForno;

            string idsSetorForno = SetorDAO.Instance.ObtemIdsSetorForno();

            string sql = @"Left Join (
                    select idProd, idLoja, idPedido, sum(coalesce(totMProduzindo,0)) as totMProduzindo,
                        sum(coalesce(qtdeProduzindo,0)) as qtdeProduzindo
                    from (
                        select pp.idProd, ped.idLoja, ped.idPedido, greatest(sum(pp.totM)-(
                                select sum(p2.totM/p2.qtde) as numero
                                from produto_pedido_producao p1
	                                inner join produtos_pedido_espelho p2 on (p1.idProdPed=p2.idProdPed)
                                where p2.idProdPed=pp.idProdPed and (p1.entrouEstoque {0})
                                    and p1.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                            ), 0) as totMProduzindo, greatest(sum(pp.qtde)-(
                                select count(*) as numero
                                from produto_pedido_producao p1
	                                inner join produtos_pedido_espelho p2 on (p1.idProdPed=p2.idProdPed)
                                where p2.idProdPed=pp.idProdPed and (p1.entrouEstoque {0})
                                    and p1.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                            ), 0) as qtdeProduzindo
                        from produtos_pedido_espelho pp
                            inner join pedido_espelho pe on (pp.idPedido=pe.idPedido)
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                        where pe.situacao<>" + (int)PedidoEspelho.SituacaoPedido.Cancelado + " and pe.situacao<>" +
                            (int)PedidoEspelho.SituacaoPedido.Processando + " and ped.situacao<>" +
                            (int)Pedido.SituacaoPedido.Cancelado + " and ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @"
                            and coalesce(pp.invisivelFluxo, false)=false
                        group by pp.idProdPed, ped.idLoja, ped.idPedido
                    ) as temp
                    group by idProd" + (!String.IsNullOrEmpty(aliasProdutoLoja) ? ", idLoja" : "") +
                        (!String.IsNullOrEmpty(aliasPedido) ? ", idPedido" : "") + @"
                ) as pped On (" + aliasProduto + ".idProd=pped.idProd" +
                    (!String.IsNullOrEmpty(aliasProdutoLoja) ? " and " + aliasProdutoLoja + ".idLoja=pped.idLoja" : "") +
                    (!String.IsNullOrEmpty(aliasPedido) ? " and " + aliasPedido + ".idPedido=pped.idPedido" : "") + ") ";

            return String.Format(sql, desconsiderarProduzindoPosForno ? "Or p1.idSetor In (" + idsSetorForno + ")" : String.Empty);
        }

        private string Sql(uint idProd, uint idLoja, string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool apenasProdutosEstoqueBaixa,
            bool agruparEstoqueLoja, string ncmInicio, string ncmFim, decimal alturaInicio, decimal alturaFim,
            decimal larguraInicio, decimal larguraFim, bool selecionar, TipoBusca tipoBusca, bool mostrarProdutosCompra, out bool temFiltro)
        {
            temFiltro = true;
            var campos = selecionar ? @"
                p.IdProd, p.IdFornec, p.IdSubgrupoProd, p.Descricao, p.CustoFabBase, p.CustoCompra, p.ValorAtacado, p.ValorBalcao, p.ValorObra,
                p.DataCad, p.UsuCad, p.CodInterno, p.Valor_Minimo, p.ValorTransferencia, p.AliqIpi, p.AtivarMin, p.IdGrupoProd, p.Espessura,
                p.Compra, p.ItemGenerico, p.AreaMinima, p.AtivarAreaMinima, p.Peso, p.Cst, p.ValorReposicao, p.Situacao, p.DataAlt,
                p.UsuAlt, p.Obs, p.CodOtimizacao, p.IdCorVidro, p.Altura, p.Largura, p.Csosn, p.Redondo, p.Forma, p.IdUnidadeMedida, p.CodigoEx,
                p.IdGeneroProduto, p.TipoMercadoria, p.CstIpi, p.IdContaContabil, p.IdCorAluminio, p.IdCorFerragem, p.IdArquivoMesaCorte,
                p.GtinProduto, p.GtinUnidTrib, p.IdUnidadeMedidaTrib, p.LocalArmazenagem, p.IdProcesso, p.IdAplicacao, p.ValorFiscal, p.IdProdOrig,
                p.IdProdBase, CONCAT(g.Descricao, ' ', COALESCE(sg.Descricao, '')) AS DescrTipoProduto,
                f.NomeFantasia AS NomeFornecedor, pbe.Descricao AS DescrParent, pbe.CodInterno AS CodInternoParent,
                pbef.Descricao AS DescrBaixaEstFiscal, pbef.CodInterno AS CodInternoBaixaEstFiscal, cv.Descricao AS DescrCor,
                pl.QtdeEstoque, pl.Reserva, pl.Liberacao, pl.M2 AS M2Estoque, ff.Nome AS DescrUsuCad, fff.Nome AS DescrUsuAlt,
                pl.EstoqueMinimo, g.Descricao AS DescrGrupo, sg.Descricao AS DescrSubgrupo, um.Codigo AS Unidade, umt.Codigo AS UnidadeTrib,
                apl.CodInterno AS CodAplicacao, prc.CodInterno AS CodProcesso, gp.Descricao AS DescrGeneroProd,
                pcc.Descricao AS DescrContaContabil, COALESCE(ncm.Ncm, p.Ncm) AS Ncm, p.IdCest" : "COUNT(*)";

            switch (tipoBusca)
            {
                case TipoBusca.SugestaoCompra:
                    campos += ", pc.qtdeComprando, pc.totMComprando";
                    break;

                case TipoBusca.SugestaoProducao:
                    campos += ", pped.qtdeProduzindo, pped.totMProduzindo";
                    break;
            }

            string sql = @"
                Select " + campos + @"
                From produto p
                    Left Join (
                        Select pbe.idProd, pp.Descricao, pp.CodInterno
                        From produto_baixa_estoque pbe
                            Left Join produto pp On (pbe.idProdBaixa=pp.idProd)
                        Group By pbe.idProd
                    ) As pbe ON (p.idProd=pbe.idProd)
                    Left Join (
                        Select pbef.idProd, ppp.Descricao, ppp.codInterno
                        From produto_baixa_estoque pbef
                            Left Join produto ppp on (pbef.idProdBaixa=ppp.idProd)
                        Group By pbef.idProd
                    ) As pbef On (p.idProd=pbef.idProd)
                    Inner Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left Join unidade_medida umt On (p.idUnidadeMedidaTrib=umt.idUnidadeMedida)
                    Left Join fornecedor f On (p.idfornec=f.idfornec)
                    Left Join funcionario ff On (p.UsuCad=ff.idFunc)
                    Left Join funcionario fff On (p.UsuAlt=fff.idFunc)
                    Left Join etiqueta_aplicacao apl On (p.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc On (p.idProcesso=prc.idProcesso)
                    Left Join genero_produto gp On (p.idGeneroProduto=gp.idGeneroProduto)
                    Left Join plano_conta_contabil pcc On (p.idContaContabil=pcc.idContaContabil)
                    Left Join (
                        select idProd, idLoja, sum(Coalesce(qtdEstoque,0)) as qtdeEstoque, sum(Coalesce(reserva,0)) as reserva,
                            sum(Coalesce(liberacao, 0)) as liberacao, sum(Coalesce(m2, 0)) as m2, sum(Coalesce(estMinimo, 0)) as estoqueMinimo
                        from produto_loja
                        Where 1 " + (idLoja > 0 ? "And idLoja=" + idLoja : String.Empty) + @"
                        group by idProd" + (agruparEstoqueLoja ? ", idLoja" : "") + @"
                    ) as pl On (p.idProd=pl.idProd)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                        WHERE idLoja = " + idLoja + @"
                    ) as ncm ON (p.IdProd = ncm.IdProd) ";

            switch (tipoBusca)
            {
                case TipoBusca.SugestaoCompra:
                    sql += SqlPendenteCompra("p", agruparEstoqueLoja ? "pl" : null);
                    break;

                case TipoBusca.SugestaoProducao:
                    sql += SqlPendenteProducao("p", agruparEstoqueLoja ? "pl" : null, null);
                    break;
            }

            sql += " Where 1 ";

            if (idProd > 0)
                sql += " And p.idProd=" + idProd;

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And p.codInterno like ?codInterno";

            if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                sql += " and p.idProd In (" + ids + ")";
            }

            if (situacao > 0)
                sql += " And p.situacao=" + situacao;

            if (idSubgrupo > 0)
                sql += " And p.IdSubgrupoProd=" + idSubgrupo;

            if (idGrupo > 0)
                sql += " And p.IdGrupoProd=" + idGrupo;

            if (apenasProdutosEstoqueBaixa)
                sql += " And p.idProd in (select * from (select distinct idProd from produto_baixa_estoque where idProdBaixa>0) as temp)";

            if (!String.IsNullOrEmpty(ncmInicio))
                sql += " and cast(replace(p.ncm, '.', '') as signed)>=?ncmInicio";

            if (!String.IsNullOrEmpty(ncmFim))
                sql += " and cast(replace(p.ncm, '.', '') as signed)<=?ncmFim";

            if (alturaInicio > 0 || alturaFim > 0)
            {
                sql += " and p.altura >= " + alturaInicio +
                    (alturaFim > 0 ? " AND p.altura <= " + alturaFim : "");
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                sql += " and p.largura >= " + larguraInicio +
                    (larguraFim > 0 ? " AND p.largura <= " + larguraFim : "");
            }

            switch (tipoBusca)
            {
                case TipoBusca.SugestaoCompra:
                case TipoBusca.SugestaoProducao:
                    sql += " and pl.estoqueMinimo>0";
                    temFiltro = true;
                    break;
            }

            if (!mostrarProdutosCompra)
                sql += " And p.Compra=0";

            switch (ordenar)
            {
                case 1:
                    sql += " order by p.Descricao";
                    break;
                case 2:
                    sql += " order by p.CodInterno";
                    break;
                case 3:
                    sql += " order by p.idSubgrupoProd";
                    break;
            }

            return sql;
        }

        public IList<Produto> GetForParent(string descricao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "Descricao asc" : sortExpression;

            bool temFiltro;
            string sql = Sql(0, 0, null, descricao, 0, (int)Glass.Data.Model.NomeGrupoProd.Vidro, 0, 0, null, null, null, null, false, false, null, null,
                0, 0, 0, 0, true, TipoBusca.Normal, true,
                out temFiltro) + " and (p.IdSubgrupoProd in (select idSubgrupoProd from subgrupo_prod where IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                " and produtosEstoque=true) or p.IdSubgrupoProd is null)";

            temFiltro = true;
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, GetParam(null, descricao, null, null, null, null, null, null));
        }

        public int GetForParentCount(string descricao)
        {
            bool temFiltro;
            string sql = Sql(0, 0, null, descricao, 0, (int)Glass.Data.Model.NomeGrupoProd.Vidro, 0, 0, null,
                null, null, null, false, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true,
                out temFiltro) + " and (p.IdSubgrupoProd in (select idSubgrupoProd from subgrupo_prod where IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                " and produtosEstoque=true) or p.IdSubgrupoProd is null)";

            return GetCountWithInfoPaging(sql, temFiltro, null, GetParam(null, descricao, null, null, null, null, null, null));
        }

        #region Usado apenas para tela de listagem de produtos

        public IList<Produto> GetListFilter(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo,
            decimal alturaInicio, decimal alturaFim, decimal larguraInicio, decimal larguraFim, int ordenar,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            return LoadDataWithSortExpression(Sql(0, 0, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, null, null, null, null, false,
                false, null, null, alturaInicio, alturaFim, larguraInicio, larguraFim, true, TipoBusca.Normal, true, out temFiltro), ordenar > 0 ? null : sortExpression, startRow, pageSize, temFiltro,
                GetParam(codInterno, descricao, null, null, null, null, null, null));
        }

        public int GetCountFilter(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            decimal alturaInicio, decimal alturaFim, decimal larguraInicio, decimal larguraFim)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(0, 0, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, null, null, null, null,
                false, false, null, null, alturaInicio, alturaFim, larguraInicio, larguraFim, true, TipoBusca.Normal, true, out temFiltro), temFiltro, GetParam(codInterno, descricao, null, null, null, null, null, null));
        }

        #endregion

        public IList<Produto> GetList()
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(0, 0, null, null, 0, 0, 0, 0, null, null, null,
                null, false, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro)).ToList();
        }

        public IList<Produto> GetList(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool apenasProdutosEstoqueBaixa,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            return LoadDataWithSortExpression(Sql(0, 0, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib,
                dataFimLib, apenasProdutosEstoqueBaixa, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro), ordenar > 0 ? null : sortExpression, startRow, pageSize,
                temFiltro, GetParam(codInterno, descricao, dataIni, dataFim, dataIniLib, dataFimLib, null, null));
        }

        public int GetCount(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool apenasProdutosEstoqueBaixa)
        {
            bool temFiltro;
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib, dataFimLib,
                apenasProdutosEstoqueBaixa, false, null, null, 0, 0, 0, 0, false, TipoBusca.Normal, true, out temFiltro), GetParam(codInterno, descricao, dataIni, dataFim,
                dataIniLib, dataFimLib, null, null));
        }

        public IList<Produto> GetListConsulta(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool apenasProdutosEstoqueBaixa,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;

            return objPersistence.LoadDataWithSortExpression(Sql(0, UserInfo.GetUserInfo.IdLoja, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib,
                dataFimLib, apenasProdutosEstoqueBaixa, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, false, out temFiltro),
                new InfoSortExpression(sortExpression),
                new InfoPaging(startRow, pageSize), GetParam(codInterno, descricao, dataIni, dataFim, dataIniLib, dataFimLib, null, null)).ToList();
        }

        public int GetCountConsulta(string codInterno, string descricao, int situacao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool apenasProdutosEstoqueBaixa)
        {
            bool temFiltro;
            return objPersistence.ExecuteSqlQueryCount(Sql(0, UserInfo.GetUserInfo.IdLoja, codInterno, descricao, situacao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib, dataFimLib,
                apenasProdutosEstoqueBaixa, false, null, null, 0, 0, 0, 0, false, TipoBusca.Normal, false, out temFiltro), GetParam(codInterno, descricao,
                dataIni, dataFim, dataIniLib, dataFimLib, null, null));
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna um elemento Produto, se o id passado não existir, uma exceção será lançada
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Produto GetElement(uint idProd)
        {
            return GetElement(null, idProd);
        }

        /// <summary>
        /// Retorna um elemento Produto, se o id passado não existir, uma exceção será lançada
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Produto GetElement(GDASession sessao, uint idProd)
        {
            bool temFiltro;
            return objPersistence.LoadOneData(sessao, Sql(idProd, 0, null, null, 0, 0, 0, 0, null, null, null, null, false,
                false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro));
        }

        /// <summary>
        /// Retorna um elemento Produto, se o id passado não existir, uma exceção será lançada.
        /// (Inclui os dados para cálculo da alíquota de ICMS interna)
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Produto GetElement(GDASession sessao, uint idProd, int? idNf, uint idLoja, uint? idCliente, uint? idFornec, bool saida)
        {
            var retorno = GetElement(sessao, idProd);
            if (retorno != null)
            {
                retorno.IdNfIcms = idNf;
                retorno.IdLojaIcms = idLoja;
                retorno.IdClienteIcms = idCliente;
                retorno.IdFornecIcms = idFornec;
                retorno.SaidaIcms = saida;

                if (idLoja > 0)
                    retorno.Ncm = ObtemNcm(retorno.IdProd, idLoja);
            }

            return retorno;
        }

        /// <summary>
        /// Retorna um elemento Produto, se o id passado não existir, retorna NULL
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Produto GetElementIfExist(uint idProd)
        {
            if (idProd == 0)
                return null;

            bool temFiltro;
            List<Produto> lstProd = objPersistence.LoadData(Sql(idProd, 0, null, null, 0, 0, 0, 0, null, null, null,
                null, false, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro));
            return lstProd.Count > 0 ? lstProd[0] : null;
        }

        public IList<Produto> GetByGrupoSubgrupoCodInterno(string codInterno, string descricao, int idGrupoProd, int idSubgrupoProd,
            string ncmInicio, string ncmFim)
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(0, 0, codInterno, descricao, 0, idGrupoProd, idSubgrupoProd, 0, null, null,
                null, null, false, false, ncmInicio, ncmFim, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro), GetParam(codInterno, descricao,
                null, null, null, null, ncmInicio, ncmFim)).ToList();
        }

        private GDAParameter[] GetParam(string codInterno, string descricao, string dataIni, string dataFim, string dataIniLib, string dataFimLib,
            string ncmInicio, string ncmFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", "%" + codInterno + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descr", "%" + descricao + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniLib))
                lstParam.Add(new GDAParameter("?dataIniLib", DateTime.Parse(dataIniLib + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimLib))
                lstParam.Add(new GDAParameter("?dataFimLib", DateTime.Parse(dataFimLib + " 23:59")));

            if (!String.IsNullOrEmpty(ncmInicio))
                lstParam.Add(new GDAParameter("?ncmInicio", ncmInicio.Replace(".", "")));

            if (!String.IsNullOrEmpty(ncmFim))
                lstParam.Add(new GDAParameter("?ncmFim", ncmFim.Replace(".", "")));

            return lstParam.ToArray();
        }

        #endregion

        #region Listagem padrão de produtos compra

        private string SqlCompra(string codInterno, string descricao, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and Compra=1";

            string campos = selecionar ? "p.*, Concat(g.Descricao, if(sg.Descricao is null, '', Concat(' - ', sg.descricao))) as DescrTipoProduto, f.NomeFantasia as NomeFornecedor" : "Count(*)";

            string sql = "Select " + campos + " From produto p " +
                "Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd) " +
                "Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd) " +
                "Left Join fornecedor f On (p.idfornec=f.idfornec) Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And p.codInterno=?codInterno";

            if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                sql += " and p.idProd In (" + ids + ")";
            }

            return sql;
        }

        public IList<Produto> GetListCompra(string codInterno, string descricao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlCompra(codInterno, descricao, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParam(codInterno, descricao, null, null, null, null, null, null));
        }

        public int GetCountCompra(string codInterno, string descricao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlCompra(codInterno, descricao, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, GetParam(codInterno, descricao, null, null, null, null, null, null));
        }

        #endregion

        #region Retorna o último código interno cadastrado

        /// <summary>
        /// Retorna o último código interno cadastrado acrescentado de 1
        /// </summary>
        public string GetLastId()
        {
            object idProd = objPersistence.ExecuteScalar("Select Coalesce(idProd,0)+1 From produto Order By idprod desc limit 0,1");

            if (idProd == null || String.IsNullOrEmpty(idProd.ToString()))
                return String.Empty;

            int cont = 1;

            // Enquanto não achar um código interno válido continua procurando com base no idProd
            while (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto where codInterno='" + idProd + "'") > 0)
                idProd = objPersistence.ExecuteScalar("Select Coalesce(idProd,0)+" + (++cont) + " From produto Order By idprod desc limit 0,1");

            return idProd.ToString();
        }

        #endregion

        #region Busca produto pelo CodInterno/IdProd

        /// <summary>
        /// Busca produto pelo seu código interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Produto GetByCodInterno(string codInterno)
        {
            return GetByCodInterno(null, codInterno);
        }

        /// <summary>
        /// Busca produto pelo seu código interno
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Produto GetByCodInterno(GDASession sessao, string codInterno)
        {
            if (String.IsNullOrEmpty(codInterno))
                return null;

            string sql = "Select * From produto Where codInterno=?codInterno And situacao=1";

            List<Produto> lstProd = objPersistence.LoadData(sessao, sql, new GDAParameter("?codInterno", codInterno));

            return lstProd.Count > 0 ? lstProd[0] : null;
        }

        public uint ObterIdPorCodInterno(GDASession sessao, string codInterno)
        {
            if (string.IsNullOrEmpty(codInterno))
                return 0;

            string sql = "Select IdProd From produto Where codInterno=?codInterno And situacao=1";
            return ExecuteScalar<uint>(sessao, sql, new GDAParameter("?codInterno", codInterno));
        }

        /// <summary>
        /// Busca produto pelo seu código interno.
        /// (Inclui os dados para cálculo da alíquota de ICMS interna)
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Produto GetByCodInterno(string codInterno, int? idNf, uint idLoja, uint? idCliente, uint? idFornec, bool saida)
        {
            var retorno = GetByCodInterno(codInterno);
            if (retorno != null)
            {
                retorno.IdNfIcms = idNf;
                retorno.IdLojaIcms = idLoja;
                retorno.IdClienteIcms = idCliente;
                retorno.IdFornecIcms = idFornec;
                retorno.SaidaIcms = saida;
            }

            return retorno;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca produto pelo idProd
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Produto GetByIdProd(uint idProd)
        {
            return GetByIdProd(null, idProd);
        }

        /// <summary>
        /// Busca produto pelo idProd
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Produto GetByIdProd(GDASession sessao, uint idProd)
        {
            try
            {
                return objPersistence.LoadOneData(sessao, "Select * From produto Where idProd=" + idProd);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Recuperar os produtos pelos identificadores

        /// <summary>
        /// Recupera os produtos com base no identificadores informados
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        public IEnumerable<Produto> ObterProdutos(GDA.GDASession sessao, IEnumerable<uint> idsProd)
        {
            var ids = string.Join(",", idsProd);

            if (!string.IsNullOrEmpty(ids))
                return objPersistence.LoadData(sessao,
                    string.Format("SELECT * FROM produto WHERE IdProd IN ({0})", ids));
            else
                return new Produto[0];
        }

        #endregion

        #region Busca os produtos com a qtde que foram vendidos

        internal enum TipoBuscaMateriaPrima
        {
            ApenasProduto,
            ApenasMateriaPrima,
            Ambos,
            ApenasProdutoMateriaPrima
        }

        internal string SqlVendasProd(uint idCliente, string nomeCliente, string codRota, string idLoja, string idsGrupos, string idsSubgrupo,
            string codInterno, string descrProd, TipoBuscaMateriaPrima buscaMateriaPrima, string dtIni, string dtFim, string dtIniPed,
            string dtFimPed, string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoPedido, string tipoVendaPedido,
            uint idFunc, uint idFuncCliente, int tipoFastDelivery, uint idPedido, int tipoDesconto, bool agruparCliente, bool agruparPedido,
            bool semValor, bool agruparLiberacao, bool agruparAmbiente, int buscarNotaFiscal, int idLiberacao, ref List<GDAParameter> lstParam, bool selecionar)
        {
            return SqlVendasProd(idCliente, nomeCliente, codRota, idLoja, idsGrupos, idsSubgrupo, codInterno, descrProd, buscaMateriaPrima,
                dtIni, dtFim, dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, tipoPedido, tipoVendaPedido, idFunc,
                idFuncCliente, tipoFastDelivery, idPedido, tipoDesconto, agruparCliente, agruparPedido, semValor, agruparLiberacao,
                agruparAmbiente, buscarNotaFiscal, idLiberacao, ref lstParam, null, selecionar);
        }

        internal string SqlVendasProd(uint idCliente, string nomeCliente, string codRota, string idLoja, string idsGrupos, string idsSubgrupo,
            string codInterno, string descrProd, TipoBuscaMateriaPrima buscaMateriaPrima, string dtIni, string dtFim, string dtIniPed,
            string dtFimPed, string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoPedido, string tipoVendaPedido,
            uint idFunc, uint idFuncCliente, int tipoFastDelivery, uint idPedido, int tipoDesconto, bool agruparCliente, bool agruparPedido,
            bool semValor, bool agruparLiberacao, bool agruparAmbiente, int buscarNotaFiscal, int idLiberacao, ref List<GDAParameter> lstParam,
            LoginUsuario login, bool selecionar)
        {
            string criterio = String.Empty;
            bool calcularLiberados = PedidoConfig.LiberarPedido;
            var calcularValorPedComercial = !PedidoConfig.LiberarPedido && !PCPConfig.UsarConferenciaFluxo;

            if (PedidoConfig.LiberarPedido && !string.IsNullOrEmpty(situacao) && !agruparLiberacao && buscarNotaFiscal == 0 &&
                !("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.Confirmado + ",") &&
                !("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ","))
            {
                calcularLiberados = false;
            }

            if (!agruparLiberacao && buscarNotaFiscal == 0 && (("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.Ativo + ",") ||
                ("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.AtivoConferencia + ",") ||
                ("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + ",") ||
                ("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ",") ||
                ("," + situacao + ",").Contains("," + (int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro + ",")))
                calcularLiberados = false;

            string campoData = calcularLiberados ? "lp.dataLiberacao" : "ped.dataConf";

            string campoTotal = !semValor ? PedidoDAO.Instance.SqlCampoTotalLiberacao(selecionar && calcularLiberados,
                "((pp.total + coalesce(pp.valorBenef,0))/pp1.totalProd*ped.total)", "totalVend1", "ped", "pe", "ap", "plp", false) : "0 As totalVend1";

            string campoCusto = !semValor ? PedidoDAO.Instance.SqlCampoCustoLiberacao(selecionar && calcularLiberados,
                "pp.custoProd", "totalCusto1", "ped", "pe", "ap", "plp", true) : "0 As totalCusto1";

            // O valor unitário está sendo calculado de outra forma, dividindo o total pela qtd ou m²
            string campoValorUnitario = "0"; /*campoTotal.IndexOf(" as totalVend1") > 0 ? campoTotal.Remove(campoTotal.Length - " as totalVend1".Length) : campoTotal;
            campoValorUnitario = campoValorUnitario + "/if(coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + "), pp.TotM" +
                (calcularLiberados ? "/pp.qtde*plp.qtdeCalc" : "") + ", " + (calcularLiberados ? "plp.QtdeCalc" : "pp.qtde") + ")";*/

            string campoCalcTotM2 = @"if(p.idGrupoProd=1 and Coalesce(pp.TotM/pp.qtde,0)=0, (if(pp.altura>0, pp.altura, p.altura)/1000)*
                (if(pp.largura>0, pp.largura, p.largura)/1000), " + (calcularLiberados ?

                // Calcula m² considerando que o pedido possa ter mais de uma liberação
                (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ?
                    "if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" and ap.idAmbientePedido is not null,
                        (pp.TotM2Calc/Coalesce(ap.qtde,1))*Sum(plp.qtdeCalc), (pp.TotM2Calc/pp.Qtde)*Sum(plp.qtdeCalc))" :
                    "if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" and ap.idAmbientePedido is not null,
                        pp.TotM*Sum(plp.qtdeCalc), (pp.TotM/pp.Qtde)*Sum(plp.qtdeCalc))") :

                // Calcula m² considerando que o pedido não tenha mais de uma liberação
                (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "pp.totm2Calc" :
                    "pp.totm*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + " and ap.idAmbientePedido is not null, ap.qtde, 1)")) + ")";

            string camposInt = selecionar ? "p.*, " + campoCusto + ", " + campoData + @" as dataFiltro,
                (pp.altura* If(pp.totM > 0, 0, (" + (calcularLiberados ? "Sum(plp.QtdeCalc)" : "pp.qtde") + @"))) as totalML1,
                (pp.altura) as totalAltura1, " + campoTotal + @", cast(" + campoValorUnitario + @" as decimal(12,2)) as valorVendido1,
                ped.idFunc, g.Descricao as DescrGrupo, s.Descricao as DescrSubgrupo, Coalesce(" + campoCalcTotM2 + @", 0) as totalM21,
                cast((" + (calcularLiberados ? "Sum(plp.QtdeCalc)" : "pp.qtde") + @") as signed) as totalQtdeLong1, coalesce(s.tipoCalculo, g.tipoCalculo,
                " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @") as tipoCalc, '$$$' as Criterio, ped.idCli as idClienteVend, c.nome as nomeClienteVend,
                pp.idPedido," + (calcularLiberados ? " plp.idLiberarPedido," : "") + @" coalesce(cv.descricao, ca.descricao, cf.descricao, '-') as descrCor, ped.idCli,
                p.codInterno As codigoProduto, ap.IdAmbientePedido as IdAmbiente, ap.Ambiente" :
                string.Format("pp.idProd, ped.IdCli, pp.IdPedido, ap.IdAmbientePedido{0}", calcularLiberados ? ", plp.idLiberarPedido" : string.Empty);

            // Ao calcular o valor vendido dos produtos estava sendo utilizado a função AVG, porém a mesma não pode ser usada neste caso,
            // devido ao fato de existir por exemplo 10 unidade de um produto no valor R$50,00 (no mesmo produto_pedido) e 1 unidade deste produto
            // no valor de R$200,00, como o AVG não considera a qtde do produtos_pedido mas sim a quantidade de resultados do SQL (neste caso 2),
            // faz com que o cálculo fique incorreto, por isso foi alterado para calcular o valor vendido com base no total e na qtde.
            string campos = selecionar ? @"*, sum(totalAltura1) as totalAltura, round(sum(totalVend1),2) as totalVend,
                round(sum(totalCusto1),2) as totalCusto, round(sum(totalM21),2) as totalM2, round(sum(totalML1),2) as totalML,
                cast(sum(Coalesce(totalQtdeLong1,0)) as signed) as totalQtdeLong,
                Cast(sum(totalVend1)/if(tipoCalc in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                sum(totalM21), cast(sum(Coalesce(totalQtdeLong1,0)) as signed)) as decimal(12,2)) as valorVendido" : "Count(*)";

            if (semValor && String.IsNullOrEmpty(tipoPedido))
                tipoPedido = "1,2,3,4";

            string tipoPed = !semValor ? (int)Pedido.TipoPedidoEnum.Venda + "," + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.MaoDeObra : tipoPedido.ToString();

            // Se o tipo de venda não tiver sido informado, preenche automaticamente com à vista, à prazo e obra.
            if (String.IsNullOrEmpty(tipoVendaPedido))
                tipoVendaPedido = (int)Pedido.TipoVendaPedido.AVista + "," + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.Obra;

            string sql = @"
                select " + campos + @"
                from (
                    Select " + camposInt + @"
                    From produtos_pedido pp

                        Inner Join (select idpedido, sum(total + coalesce(valorBenef, 0)) as totalProd, sum(custoProd) as custoProd from produtos_pedido
		                    where invisivel" + (calcularValorPedComercial ? "Pedido" : "Fluxo") + @"=false or invisivel" + (calcularValorPedComercial ? "Pedido" : "Fluxo") + @" is null group by idpedido) pp1 on (pp.idpedido=pp1.idPedido)

                        Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                        Inner Join produto p On (pp.idProd=p.idProd)
                        Left Join (select idprodped, sum(custo) as custo from produto_pedido_benef) ppb On (ppb.idProdPed=pp.idProdPed)
                        Left Join cor_vidro cv on (p.idCorVidro=cv.idCorVidro)
                        Left Join cor_aluminio ca on (p.idCorAluminio=ca.idCorAluminio)
                        Left Join cor_ferragem cf on (p.idCorFerragem=cf.idCorFerragem)
                        Inner Join pedido ped on (pp.idPedido=ped.idPedido)
                        Left Join pedido_espelho pe On (ped.idPedido=pe.idPedido)
                        Left Join cliente c on (ped.idCli=c.id_Cli)
                        " + (calcularLiberados ? @"Left Join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                        Left Join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)" : "") + @"
                        Inner Join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                        Left Join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                    Where 1 ";

            if (calcularLiberados)
                sql += " and plp.idProdLiberarPedido is not null";
            else
                sql += string.Format(" and (invisivel{0}=false or invisivel{0} is null)",
                    calcularValorPedComercial ? "Pedido" : "Fluxo");

            #region Filtros por pedido

            var sqlPedido = @"
                Select Cast(ped.idPedido as char)
                From pedido ped
                    Left Join produtos_liberar_pedido plp on (ped.idPedido=plp.idPedido)
                    Left Join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido AND lp.Situacao IS NOT NULL AND lp.Situacao=1)
                Where 1";

            if (idLoja != "0" && !String.IsNullOrEmpty(idLoja))
            {
                sqlPedido += " And ped.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(Conversoes.StrParaUint(idLoja)) + "    ";
            }
            else
                criterio += "Loja: Todas    ";

            #region Filtro por data de situação

            PedidoDAO.DadosFiltroDataSituacao filtro = PedidoDAO.Instance.FiltroDataSituacao(dtIni, dtFim, situacao, "?dtIni", "?dtFim", "ped", "lp", " Sit.", calcularLiberados);

            // Chamado 15183: Necessário aplicar este filtro no sql principal também, para que filtre corretamente pedidos liberados parcialmente
            if (calcularLiberados)
                sql += filtro.Sql;

            if (filtro.Sql.Contains("?dtIni"))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00:00")));

            if (filtro.Sql.Contains("?dtFim"))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59:59")));

            sqlPedido += filtro.Sql;
            criterio += filtro.Criterio;
            List<GDAParameter> lstParamPed = new List<GDAParameter>(filtro.Parametros);

            #endregion

            if (!String.IsNullOrEmpty(dtIniPed))
            {
                sqlPedido += " And ped.DataPedido>=?dtIniPed";
                lstParamPed.Add(new GDAParameter("?dtIniPed", DateTime.Parse(dtIniPed + " 00:00")));

                criterio += "Data Início: " + dtIniPed + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimPed))
            {
                sqlPedido += " And ped.DataPedido<=?dtFimPed";
                lstParamPed.Add(new GDAParameter("?dtFimPed", DateTime.Parse(dtFimPed + " 23:59")));

                criterio += "Data Fim: " + dtFimPed + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniEnt))
            {
                sqlPedido += " And ped.DataEntrega>=?dtIniEnt";
                lstParamPed.Add(new GDAParameter("?dtIniEnt", DateTime.Parse(dtIniEnt + " 00:00")));

                criterio += "Data Entrega Início: " + dtIniEnt + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimEnt))
            {
                sqlPedido += " And ped.DataEntrega<=?dtFimEnt";
                lstParamPed.Add(new GDAParameter("?dtFimEnt", DateTime.Parse(dtFimEnt + " 23:59")));

                criterio += "Data Entrega Fim: " + dtFimEnt + "    ";
            }

            if (!String.IsNullOrEmpty(situacao))
            {
                sqlPedido += " And ped.situacao in (" + situacao + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacao) + "    ";
            }
            else
                sql += " And ped.Situacao=" + (int)Pedido.SituacaoPedido.Confirmado;

            if (!String.IsNullOrEmpty(situacaoProd))
            {
                sqlPedido += " And ped.situacaoProducao in (" + situacaoProd + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoProdPedido(situacaoProd, login) + "    ";
            }

            if (!String.IsNullOrEmpty(tipoPed))
            {
                sqlPedido += " And ped.tipoPedido in (" + tipoPed + ")";

                if (semValor)
                {
                    criterio += "Tipo de pedido: ";

                    if (("," + tipoPed + ",").Contains("1"))
                        criterio += "Venda, ";

                    if (("," + tipoPed + ",").Contains("2"))
                        criterio += "Revenda, ";

                    if (("," + tipoPed + ",").Contains("3"))
                        criterio += "Mão de obra, ";

                    if (("," + tipoPed + ",").Contains("4"))
                        criterio += "Produção, ";

                    criterio = criterio.TrimEnd(',', ' ') + "    ";
                }
            }

            if (idFunc > 0)
            {
                sqlPedido += " and ped.usuCad=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            switch (tipoFastDelivery)
            {
                case 1:
                    sqlPedido += " and ped.fastDelivery=true";
                    criterio += "Fast Delivery: Sim    ";
                    break;
                case 2:
                    sqlPedido += " and (ped.fastDelivery=false or ped.fastDelivery is null)";
                    criterio += "Fast Delivery: Não    ";
                    break;
            }

            if (!string.IsNullOrEmpty(tipoVendaPedido))
            {
                sqlPedido += " and ped.tipoVenda in(" + tipoVendaPedido + ")";
                string[] itens = tipoVendaPedido.Split(',');
                criterio += "Tipo de venda: ";

                foreach (var tipo in itens)
                    switch (Conversoes.StrParaUint(tipo))
                    {
                        case (int)Pedido.TipoVendaPedido.APrazo:
                            criterio += "À Prazo, "; break;
                        case (int)Pedido.TipoVendaPedido.AVista:
                            criterio += "À Vista, "; break;
                        case (int)Pedido.TipoVendaPedido.Funcionario:
                            criterio += "Funcionário, "; break;
                        case (int)Pedido.TipoVendaPedido.Garantia:
                            criterio += "Garantia, "; break;
                        case (int)Pedido.TipoVendaPedido.Obra:
                            criterio += "Obra, "; break;
                        case (int)Pedido.TipoVendaPedido.Reposição:
                            criterio += "Reposição, "; break;
                    }

                criterio = criterio.Remove(criterio.LastIndexOf(',')) + "     ";
            }

            var idsPedido = String.Join(",", ExecuteMultipleScalar<string>(sqlPedido, lstParamPed.ToArray()).ToArray());

            if (String.IsNullOrEmpty(idsPedido))
                sql += " And false";
            else
                sql += " And pp.idPedido In (" + idsPedido + ")";

            #endregion

            if (idCliente > 0)
            {
                sql += " And c.id_Cli=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                sql += @"
                    AND c.Id_Cli IN
                        (SELECT rc.IdCliente From rota_cliente rc
                        Where rc.IdRota IN
                            (SELECT r.IdRota From rota r
                            WHERE r.CodInterno LIKE ?codRota))";

                lstParam.Add(new GDAParameter("?codRota", "%" + codRota + "%"));
                criterio += "Rota: " + codRota + "    ";
            }

            if (buscaMateriaPrima == TipoBuscaMateriaPrima.ApenasProdutoMateriaPrima)
            {
                sql += " and p.idProd in (select distinct idProdBaixa from produto_baixa_estoque)";
                criterio += "Apenas matéria-prima    ";
            }
            else if (!String.IsNullOrEmpty(codInterno))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInterno, null);

                switch (buscaMateriaPrima)
                {
                    case TipoBuscaMateriaPrima.ApenasProduto:
                        sql += " and p.idProd In (" + ids + ")";
                        criterio += "Produto: " + descrProd + "    ";
                        break;

                    case TipoBuscaMateriaPrima.ApenasMateriaPrima:
                        sql += " and p.idProd in (select distinct idProd from produto_baixa_estoque where idProdBaixa in (" + ids + "))";
                        criterio += "Matéria-prima: " + descrProd + "    ";
                        break;

                    case TipoBuscaMateriaPrima.Ambos:
                        sql += " And (p.idProd In (" + ids + @") or p.idProd in (select * from (select distinct idProd from produto_baixa_estoque
                            where idProdBaixa in (" + ids + ")) as temp))";
                        criterio += "Produto/Matéria-prima: " + descrProd + "    ";
                        break;
                }
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);

                switch (buscaMateriaPrima)
                {
                    case TipoBuscaMateriaPrima.ApenasProduto:
                        sql += " and p.idProd In (" + ids + ")";
                        criterio += "Produto: " + descrProd + "    ";
                        break;

                    case TipoBuscaMateriaPrima.ApenasMateriaPrima:
                        sql += " and p.idProd in (select distinct idProd from produto_baixa_estoque where idProdBaixa in (" + ids + "))";
                        criterio += "Matéria-prima: " + descrProd + "    ";
                        break;

                    case TipoBuscaMateriaPrima.Ambos:
                        sql += " And (p.idProd In (" + ids + @") or p.idProd in (select * from (select distinct idProd from produto_baixa_estoque
                            where idProdBaixa in (" + ids + ")) as temp))";
                        criterio += "Produto/Matéria-prima: " + descrProd + "    ";
                        break;
                }
            }

            if (!String.IsNullOrEmpty(idsGrupos) && idsGrupos != "0")
            {
                sql += " And p.idGrupoProd in (" + idsGrupos + ")";
                criterio += "Grupos: ";

                foreach (string id in idsGrupos.Split(','))
                    criterio += GrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(id)) + ", ";

                criterio = criterio.TrimEnd(' ', ',') + "    ";
            }

            if (!String.IsNullOrEmpty(idsSubgrupo) && idsSubgrupo != "0")
            {
                sql += " AND p.IdSubgrupoProd IN (" + idsSubgrupo + ")";
                var criterioSubgrupo = "Subgrupo(s): ";

                foreach (var id in idsSubgrupo.Split(','))
                    criterioSubgrupo += SubgrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(id)) + ", ";

                criterio += criterioSubgrupo.TrimEnd(' ').TrimEnd(',') + "    ";
            }

            if (idFuncCliente > 0)
            {
                sql += " and c.idFunc=" + idFuncCliente;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idFuncCliente) + "    ";
            }

            switch (tipoDesconto)
            {
                case 1: // Com desconto
                    sql += " And (Coalesce(ped.desconto, 0)>0 Or Coalesce(pp.valorDesconto, 0)>0 Or Coalesce(pp.valorDescontoQtde, 0)>0 Or Coalesce(pp.valorDescontoProd, 0)>0) ";
                    criterio += "Produtos com desconto    ";
                    break;
                case 2: // Sem desconto
                    sql += " And (Coalesce(ped.desconto, 0)=0 And Coalesce(pp.valorDesconto, 0)=0 And Coalesce(pp.valorDescontoQtde, 0)=0 And Coalesce(pp.valorDescontoProd, 0)=0) ";
                    criterio += "Produtos sem desconto    ";
                    break;
            }

            if (idPedido > 0)
            {
                sql += " and pp.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (calcularLiberados)
                sql += String.Format(" and coalesce(lp.situacao, {0})={0}", (int)LiberarPedido.SituacaoLiberarPedido.Liberado);

            if (agruparLiberacao && buscarNotaFiscal > 0)
            {
                sql += String.Format(" and {0} exists (select * from pedidos_nota_fiscal where idLiberarPedido=plp.idLiberarPedido)",
                    buscarNotaFiscal == 1 ? "not" : "");
                criterio += String.Format("Apenas produtos {0} Nota Fiscal gerada    ", buscarNotaFiscal == 1 ? "sem" : "com");
            }

            if (idLiberacao > 0)
            {
                sql += " AND plp.IdLiberarPedido = " + idLiberacao;
                criterio += "Liberação: " + idLiberacao + "   ";
            }

            if (lstParam.Count == 0)
                lstParam = null;

            // Este group by deve ser por pp.idProdPed, porque se for por plp.idProdLiberarPedido caso a liberação de algum pedido tenha
            // sido feita por peça, o sql irá multiplicar o total do produto pela quantidade de registros na tabela produtos_liberar_pedido
            // que este produto tiver
            sql += @"
                    Group By pp.idProdPed
                ) as temp";

            sql += " GROUP BY ";

            if (agruparCliente)
                sql += "IdCli, ";

            if (agruparPedido)
                sql += "IdPedido, ";

            if (agruparLiberacao)
                sql += "IdLiberarPedido, ";

            if (agruparAmbiente)
                sql += "IdAmbientePedido, ";

            sql += "IdProd";

            if (!selecionar)
                sql = string.Format("SELECT COUNT(*) FROM ({0}) AS tbl", sql);

            return sql.Replace("$$$", criterio);
        }

        public IList<Produto> GetRptVendasProd(uint idCliente, string nomeCliente, string codRota, string idLoja, string idsGrupos, string idsSubgrupo,
            string codInterno, string descrProd, bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed,
            string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoVendaPedido, uint idFunc, uint idFuncCliente,
            int tipoFastDelivery, string idPedido, int tipoDesconto, bool agruparCliente, bool agruparPedido, bool agruparLiberacao, bool agruparAmbiente,
            int buscarNotaFiscal, int idLiberacao, int ordenacao)
        {
            var lstParam = new List<GDAParameter>();
            var tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            string ordenarPor = " Order By" +
                (ordenacao == 1 ? " valorVendido Desc" :
                ordenacao == 2 ? " codigoProduto Asc" :
                " totalVend Desc");

            uint idPed = idPedido == "" ? 0 : Glass.Conversoes.StrParaUint(idPedido);

            string sql = SqlVendasProd(idCliente, nomeCliente, codRota, idLoja, idsGrupos, idsSubgrupo, codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, null, tipoVendaPedido, idFunc, idFuncCliente, tipoFastDelivery, idPed,
                tipoDesconto, agruparCliente, agruparPedido, false, agruparLiberacao, agruparAmbiente, buscarNotaFiscal, idLiberacao, ref lstParam, true) + ordenarPor;

            return objPersistence.LoadData(sql, lstParam != null ? lstParam.ToArray() : null).ToList();
        }

        public IList<Produto> GetListaVendasProd(uint idCliente, string nomeCliente, string codRota, string idLoja, string idsGrupos, string idsSubgrupo,
            string codInterno, string descrProd, bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed,
            string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoVendaPedido, uint idFunc, uint idFuncCliente,
            int tipoFastDelivery, uint idPedido, int tipoDesconto, bool agruparCliente, bool agruparPedido, bool agruparLiberacao, bool agruparAmbiente,
            int buscarNotaFiscal, int idLiberacao, int ordenacao, string sortExpression, int startRow, int pageSize)
        {
            var lstParam = new List<GDAParameter>();
            var tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression :
                ordenacao == 1 ? " valorVendido Desc" :
                ordenacao == 2 ? " codigoProduto Asc" :
                " totalVend Desc";

            string sql = SqlVendasProd(idCliente, nomeCliente, codRota, idLoja, idsGrupos, idsSubgrupo, codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, null, tipoVendaPedido, idFunc, idFuncCliente, tipoFastDelivery, idPedido, tipoDesconto,
                agruparCliente, agruparPedido, false, agruparLiberacao, agruparAmbiente, buscarNotaFiscal, idLiberacao, ref lstParam, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, lstParam != null ? lstParam.ToArray() : null);
        }

        public int GetListaVendasProdCount(uint idCliente, string nomeCliente, string codRota, string idLoja, string idsGrupos, string idsSubgrupo,
            string codInterno, string descrProd, bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed,
            string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoVendaPedido, uint idFunc, uint idFuncCliente,
            int tipoFastDelivery, uint idPedido, int tipoDesconto, bool agruparCliente, bool agruparPedido, bool agruparLiberacao, bool agruparAmbiente,
            int buscarNotaFiscal, int idLiberacao, int ordenacao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();
            TipoBuscaMateriaPrima tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            string sql = SqlVendasProd(idCliente, nomeCliente, codRota, idLoja, idsGrupos, idsSubgrupo, codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, null, tipoVendaPedido, idFunc, idFuncCliente, tipoFastDelivery, idPedido,
                tipoDesconto, agruparCliente, agruparPedido, false, agruparLiberacao, agruparAmbiente, buscarNotaFiscal, idLiberacao, ref lstParam, false);

            return objPersistence.ExecuteSqlQueryCount(sql, lstParam != null ? lstParam.ToArray() : null);
        }

        public IList<Produto> GetRptProducaoProd(string idLoja, string idsGrupos, uint idSubgrupo, string codInterno, string descrProd,
            bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed, string dtIniEnt, string dtFimEnt,
            string situacao, string situacaoProd, string tipoPedido, uint idFuncCliente, int tipoFastDelivery, string idPedido,
            bool agruparPedido, LoginUsuario login)
        {
            var lstParam = new List<GDAParameter>();
            var tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            uint idPed = idPedido == "" ? 0 : Glass.Conversoes.StrParaUint(idPedido);

            string sql = SqlVendasProd(0, null, null, idLoja, idsGrupos, idSubgrupo.ToString(), codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, tipoPedido, null, 0, idFuncCliente, tipoFastDelivery, idPed,
                0, false, agruparPedido, true, false, false, 0, 0, ref lstParam, login, true);

            return objPersistence.LoadData(sql, lstParam != null ? lstParam.ToArray() : null).ToList();
        }

        public IList<Produto> GetListaProducaoProd(string idLoja, string idsGrupos, uint idSubgrupo,
            string codInterno, string descrProd, bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed,
            string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoPedido, uint idFuncCliente,
            int tipoFastDelivery, uint idPedido, bool agruparPedido, string sortExpression, int startRow, int pageSize)
        {
            var lstParam = new List<GDAParameter>();
            TipoBuscaMateriaPrima tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            string sql = SqlVendasProd(0, null, null, idLoja, idsGrupos, idSubgrupo.ToString(), codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, tipoPedido, null, 0, idFuncCliente, tipoFastDelivery, idPedido,
                0, false, agruparPedido, true, false, false, 0, 0, ref lstParam, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, lstParam != null ? lstParam.ToArray() : null);
        }

        public int GetListaProducaoProdCount(string idLoja, string idsGrupos, uint idSubgrupo,
            string codInterno, string descrProd, bool incluirMateriaPrima, string dtIni, string dtFim, string dtIniPed, string dtFimPed,
            string dtIniEnt, string dtFimEnt, string situacao, string situacaoProd, string tipoPedido, uint idFuncCliente,
            int tipoFastDelivery, uint idPedido, bool agruparPedido)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();
            TipoBuscaMateriaPrima tipoBuscaMP = incluirMateriaPrima ? TipoBuscaMateriaPrima.Ambos : TipoBuscaMateriaPrima.ApenasProduto;

            string sql = SqlVendasProd(0, null, null, idLoja, idsGrupos, idSubgrupo.ToString(), codInterno, descrProd, tipoBuscaMP, dtIni, dtFim,
                dtIniPed, dtFimPed, dtIniEnt, dtFimEnt, situacao, situacaoProd, tipoPedido, null, 0, idFuncCliente, tipoFastDelivery, idPedido,
                0, false, agruparPedido, true, false, false, 0, 0, ref lstParam, false);

            return objPersistence.ExecuteSqlQueryCount(sql, lstParam != null ? lstParam.ToArray() : null);
        }

        #endregion

        #region Busca os produtos com a qtde que foram comprados

        private string SqlComprasProd(uint idFornec, string nomeFornec, string idLoja, uint idGrupo, uint idSubgrupo,
            string codInterno, string descrProd, string dtIni, string dtFim, uint idFunc, bool agruparFornecedor, string tipoCfop, string comSemNf,
            ref List<GDAParameter> lstParam, bool selecionar, bool forRpt, out bool temFiltro)
        {
            temFiltro = false;
            string criterio = String.Empty;

            string campos = selecionar ? String.Format(@"p.*, pc.dataFiltro, {0}(pc.altura) as totalAltura,
                Round({0}(pc.total + coalesce(pc.valorBenef,0)), 2) as totalCusto, pc.idFunc, pc.idsCompras, pc.numeroNfe,
                g.Descricao as DescrGrupo, s.Descricao as DescrSubgrupo, Round({0}(pc.TotM), 2) as totalM2, {0}(pc.Qtde) as totalQtde,
                coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @") as tipoCalc, '$$$' as Criterio,
                f.idFornec as idFornecComp, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornecComp", !forRpt ? "sum" : "") :
                "count(distinct p.idProd)";

            string whereCfop = String.IsNullOrEmpty(tipoCfop) ? "" :
                @" and nf.idNaturezaOperacao in (select idNaturezaOperacao from natureza_operacao no
                inner join cfop on (no.idCfop=cfop.idCfop) where idTipoCfop in (" + tipoCfop + "))";

            if (!String.IsNullOrEmpty(whereCfop))
                criterio += "Tipos de CFOP: " + TipoCfopDAO.Instance.GetDescrByString(tipoCfop) + "    ";

            string sql = @"
                Select " + campos + @"
                From (
                    select idProd, cast(pc.idCompra as char) as idsCompras, null as numeroNfe, altura, totM, qtde, pc.total, valorBenef,
                        comp.dataFinalizada as dataFiltro, comp.usuCad as idFunc, comp.idFornec, comp.idLoja, null as idNf
                    from produtos_compra pc
                        inner join compra comp on (pc.idCompra=comp.idCompra)
                        left join compra_nota_fiscal cnf on (comp.idCompra=cnf.idCompra)
                    where cnf.idCompra is null and comp.situacao In (" + (int)Compra.SituacaoEnum.Finalizada + "," + (int)Compra.SituacaoEnum.AguardandoEntrega + @")

                    union all select idProd, group_concat(cnf.idCompra order by cnf.idCompra asc separator ', ') as idsCompras, nf.numeroNfe,
                        altura, totM, qtde, pnf.total, null as valorBenef, nf.dataEmissao as dataFiltro, nf.usuCad as idFunc, nf.idFornec,
                        nf.idLoja, pnf.idNf
                    from produtos_nf pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        inner join compra_nota_fiscal cnf on (nf.idNf=cnf.idNf)
                    where nf.situacao=" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + whereCfop + @"
                    Group By pnf.idprodnf
                ) as pc
                    Inner Join produto p On (pc.idProd=p.idProd)
                    left join fornecedor f on (pc.idFornec=f.idFornec)
                    Inner Join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                Where 1";

            if (idFornec > 0)
            {
                sql += " And f.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                sql += " And f.idFornec In (" + ids + ")";
                temFiltro = true;

                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " And p.CodInterno=?codInterno ";
                lstParam.Add(new GDAParameter("?codInterno", codInterno));
                temFiltro = true;

                criterio += "Produto: " + descrProd + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                sql += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProd + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idLoja) && idLoja != "0")
            {
                sql += " And pc.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idLoja)) + "    ";
                temFiltro = true;
            }
            else
                criterio += "Loja: Todas    ";

            if (idGrupo > 0)
            {
                sql += " And p.idGrupoProd=" + idGrupo;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupo) + "    ";
                temFiltro = true;
            }

            if (idSubgrupo > 0)
            {
                sql += " And p.idSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                sql += " And pc.dataFiltro>=?dtIni";
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));
                temFiltro = true;

                criterio += "Data Início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                sql += " And pc.dataFiltro<=?dtFim";
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));
                temFiltro = true;

                criterio += "Data Fim: " + dtFim + "    ";
            }

            if (idFunc > 0)
            {
                sql += " and pc.idFunc=" + idFunc;
                temFiltro = true;
            }

            if (comSemNf == "1") // Com NF gerada
            {
                sql += " And pc.idNf is not null";
                criterio += "Produtos com nota fiscal gerada    ";
                temFiltro = true;
            }
            else if (comSemNf == "2") // Sem NF gerada
            {
                sql += " And pc.idNf is null";
                criterio += "Produtos sem nota fiscal gerada    ";
                temFiltro = true;
            }

            if (lstParam.Count == 0)
                lstParam = null;

            if (selecionar && !forRpt)
            {
                sql += " Group By pc.idProd";

                if (agruparFornecedor)
                    sql += ", f.idFornec";
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<Produto> GetRptComprasProd(uint idFornec, string nomeFornec, string idLoja, uint idGrupo, uint idSubgrupo, string codInterno,
            string descrProd, string dtIni, string dtFim, uint idFunc, bool agruparFornecedor, string tipoCfop, bool exibirDetalhes, string comSemNf)
        {
            bool temFiltro;
            var lstParam = new List<GDAParameter>();
            string sql = SqlComprasProd(idFornec, nomeFornec, idLoja, idGrupo, idSubgrupo, codInterno, descrProd, dtIni, dtFim, idFunc,
                agruparFornecedor, tipoCfop, comSemNf, ref lstParam, true, exibirDetalhes, out temFiltro) + " Order By totalCusto Desc";

            return objPersistence.LoadData(sql, lstParam != null ? lstParam.ToArray() : null).ToList();
        }

        public IList<Produto> GetListaComprasProd(uint idFornec, string nomeFornec, string idLoja, uint idGrupo, uint idSubgrupo, string codInterno,
            string descrProd, string dtIni, string dtFim, uint idFunc, bool agruparFornecedor, string tipoCfop, string comSemNf,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "totalCusto Desc";

            var lstParam = new List<GDAParameter>();
            string sql = SqlComprasProd(idFornec, nomeFornec, idLoja, idGrupo, idSubgrupo, codInterno, descrProd, dtIni, dtFim, idFunc,
                agruparFornecedor, tipoCfop, comSemNf, ref lstParam, true, false, out temFiltro);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, lstParam != null ? lstParam.ToArray() : null);
        }

        public int GetListaComprasProdCount(uint idFornec, string nomeFornec, string idLoja, uint idGrupo, uint idSubgrupo, string codInterno,
            string descrProd, string dtIni, string dtFim, uint idFunc, bool agruparFornecedor, string tipoCfop, string comSemNf)
        {
            bool temFiltro;
            List<GDAParameter> lstParam = new List<GDAParameter>();
            string sql = SqlComprasProd(idFornec, nomeFornec, idLoja, idGrupo, idSubgrupo, codInterno, descrProd, dtIni, dtFim, idFunc,
                agruparFornecedor, tipoCfop, comSemNf, ref lstParam, true, false, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro, lstParam != null ? lstParam.ToArray() : null);
        }

        #endregion

        #region Busca produtos por Grupo/Subgrupo e que não estejam relacionados a uma Loja/Compra

        private string SqlProd(int idGrupo, int idSubgrupo, string descr, int idPedido, int idLoja, int idCompra, bool paraPedidoProducao,
            bool paraPedidoInterno, bool selecionar, out string filtroAdicional)
        {
            return SqlProd(idGrupo, idSubgrupo, descr, 0, 0, idPedido, idLoja, idCompra, paraPedidoProducao,
            paraPedidoInterno, false, 0, 0, 0, selecionar, out filtroAdicional);
        }

        private string SqlProd(int idGrupo, int idSubgrupo, string descr, int altura, int largura,
            int idPedido, int idLoja, int idCompra, bool paraPedidoProducao, bool paraPedidoInterno, bool parceiro, int idItemProjeto, uint idCliente, int idOrcamento,
            bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = " and p.situacao=" + (int)Glass.Situacao.Ativo;

            var parametroIdLoja = string.Empty;

            if (idOrcamento > 0)
            {
                var idLojaOrcamento = OrcamentoDAO.Instance.GetIdLoja(null, (uint)idOrcamento);

                if (idLojaOrcamento > 0)
                    parametroIdLoja = $" AND pl.IdLoja={idLojaOrcamento} ";
            }
            else if (idPedido > 0) // Busca os produtos que não forem compras
            {
                // Define que caso seja passado o pedido, busque estoque somente estoque disponível da loja do pedido passado.
                parametroIdLoja = " AND pl.IdLoja=" + PedidoDAO.Instance.ObtemIdLoja(null, (uint)idPedido);
                filtroAdicional += " And (p.compra is null or p.compra=0)";
            }
            else if (idLoja > 0)
            {
                parametroIdLoja = " AND pl.IdLoja=" + idLoja;
                filtroAdicional += " And (p.compra is null or p.compra=0)";
            }

            string campos = selecionar ? @"
                p.*, Concat(g.Descricao, if(sg.Descricao is null, '', Concat(' - ', sg.descricao))) as DescrTipoProduto,
                (Select Sum(QtdEstoque) From produto_loja pl Where pl.idProd=p.idProd " + parametroIdLoja + @" {0}) as QtdeEstoque,
                (Select Sum(Reserva) From produto_loja pl Where pl.idProd=p.idProd " + parametroIdLoja + @" {0}) as Reserva,
                (Select Sum(Liberacao) From produto_loja pl Where pl.idProd=p.idProd " + parametroIdLoja + @" {0}) as Liberacao,
                (Select Sum(M2) From produto_loja pl Where pl.idProd=p.idProd " + parametroIdLoja + @" {0}) as M2Estoque,
                 f.NomeFantasia as NomeFornecedor" : "Count(*)";

            // Se for para nota fiscal, busca estoque fiscal
            if (idPedido == 0 && idCompra == 0 && !paraPedidoProducao && selecionar)
                campos += ", (Select Sum(EstoqueFiscal) From produto_loja pl Where pl.idProd=p.idProd) as EstoqueFiscal ";

            string sql = @"
                Select " + campos + @"
                From produto p
                    Left Join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg On (p.idSubgrupoProd=sg.idSubgrupoProd)
                    Left Join fornecedor f On (p.idfornec=f.idfornec)
                Where 1 ?filtroAdicional?";

            if (idSubgrupo > 0)
                filtroAdicional += " And p.IdSubgrupoProd=" + idSubgrupo;
            else if (paraPedidoProducao)
                filtroAdicional += " and 1<0";

            if (idGrupo > 0)
                filtroAdicional += " And p.IdGrupoProd=" + idGrupo;

            // Filtra pela descrição
            if (!String.IsNullOrEmpty(descr))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descr);
                filtroAdicional += " And p.idProd In (" + ids + ")";
            }

            if (altura > 0)
                filtroAdicional += string.Format(" AND p.Altura={0}", altura);

            if (largura > 0)
                filtroAdicional += string.Format(" AND p.Largura={0}", largura);

            /*Chamado 63721 Verifica se idPedido e idloja é 0, para filtrar pela loja do funcionario */
            if (idOrcamento == 0 && idPedido == 0 && idLoja == 0 && !UserInfo.GetUserInfo.IsAdministrador)
                sql = String.Format(sql, " And pl.idLoja=" + UserInfo.GetUserInfo.IdLoja);

            if (idLoja > 0)
                filtroAdicional += " And p.IdProd Not In (Select IdProd From produto_loja Where IdLoja=" + idLoja + ")";

            if (idSubgrupo == (int)Utils.SubgrupoProduto.RetalhosProducao)
                filtroAdicional +=
                    string.Format(" AND p.IdProd IN (SELECT rp.IdProd FROM retalho_producao rp WHERE rp.Situacao={0})",
                        (int)SituacaoRetalhoProducao.Disponivel);

            if (paraPedidoInterno)
                filtroAdicional += " And p.compra=true";

            if (parceiro)
            {
                filtroAdicional += " And p.compra=false";

                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                    filtroAdicional += " AND sg.BloquearEcommerce=0 ";
            }

            if (sql.Contains("{0}"))
                sql = string.Format(sql, string.Empty);

            if (idPedido > 0 && PedidoDAO.Instance.GetTipoPedido(null, (uint)idPedido) == Pedido.TipoPedidoEnum.Producao)
            {
                var idPedidoRevenda = PedidoDAO.Instance.ObterIdPedidoRevenda(null, idPedido);

                if (idPedidoRevenda.GetValueOrDefault() > 0)
                    filtroAdicional = string.Format(" AND p.IdProd IN (SELECT IdProdBase FROM produto WHERE IdProd IN (SELECT IdProd FROM produtos_pedido WHERE IdPedido = {0}))", idPedidoRevenda);
            }

            if (idItemProjeto > 0)
            {
                var idProjetoModelo = ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, (uint)idItemProjeto);
                var idsCor = ProjetoModeloDAO.Instance.ObtemIdsCorVidro(idProjetoModelo);

                if (!string.IsNullOrWhiteSpace(idsCor))
                    filtroAdicional += " AND p.IdCorVidro IN (" + idsCor + ")";
            }

            if (idCliente > 0)
            {
                var idsSubgrupo = ClienteDAO.Instance.ObtemIdsSubgrupo(idCliente);
                if (!string.IsNullOrWhiteSpace(idsSubgrupo))
                    filtroAdicional += " AND p.IdSubgrupoProd IN (" + idsSubgrupo + ")";
            }

            return sql;
        }

        public IList<Produto> GetProdutos(int idGrupo, int idSubgrupo, string descr, int idPedido, int idLoja, int idCompra, int pedidoInterno, string sortExpression, int startRow, int pageSize)
        {
            return GetProdutos(idGrupo, idSubgrupo, descr, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno), sortExpression, startRow, pageSize);
        }

        public int GetProdutosCount(int idGrupo, int idSubgrupo, string descr, int idPedido, int idLoja, int idCompra, int pedidoInterno)
        {
            return GetProdutosCount(idGrupo, idSubgrupo, descr, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno));
        }

        public IList<Produto> GetProdutos(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra, int pedidoInterno, string sortExpression, int startRow, int pageSize)
        {
            return GetProdutos(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno), false, sortExpression, startRow, pageSize);
        }

        public int GetProdutosCount(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra, int pedidoInterno)
        {
            return GetProdutosCount(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno), false);
        }

        public IList<Produto> GetProdutos(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra, int pedidoInterno, int parceiro, int idItemProjeto, uint idCliente, int idOrcamento,
            string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlProd(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno), Convert.ToBoolean(parceiro), idItemProjeto, idCliente, idOrcamento, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParamProd(descr));
        }

        public int GetProdutosCount(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra, int pedidoInterno, int parceiro, int idItemProjeto, uint idCliente, int idOrcamento)
        {
            string filtroAdicional;
            string sql = SqlProd(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, false, Convert.ToBoolean(pedidoInterno), Convert.ToBoolean(parceiro), idItemProjeto, idCliente, idOrcamento, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamProd(descr));
        }

        public IList<Produto> GetProdutos(int idGrupo, int idSubgrupo, string descr, int idPedido, int idLoja, int idCompra, bool paraPedidoProducao, bool paraPedidoInterno, string sortExpression, int startRow, int pageSize)
        {
            return GetProdutos(idGrupo, idSubgrupo, descr, 0, 0, idPedido, idLoja, idCompra, paraPedidoProducao, paraPedidoInterno, false, sortExpression, startRow, pageSize);
        }

        public IList<Produto> GetProdutos(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra, bool paraPedidoProducao,
            bool paraPedidoInterno, bool parceiro, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlProd(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, paraPedidoProducao, paraPedidoInterno, parceiro, 0, 0, 0, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParamProd(descr));
        }

        public int GetProdutosCount(int idGrupo, int idSubgrupo, string descr, int idPedido, int idLoja, int idCompra, bool paraPedidoProducao, bool paraPedidoInterno)
        {
            return GetProdutosCount(idGrupo, idSubgrupo, descr, 0, 0, idPedido, idLoja, idCompra, paraPedidoProducao, paraPedidoInterno, false);
        }

        public int GetProdutosCount(int idGrupo, int idSubgrupo, string descr, int altura, int largura, int idPedido, int idLoja, int idCompra,
            bool paraPedidoProducao, bool paraPedidoInterno, bool parceiro)
        {
            string filtroAdicional;
            string sql = SqlProd(idGrupo, idSubgrupo, descr, altura, largura, idPedido, idLoja, idCompra, paraPedidoProducao, paraPedidoInterno, parceiro, 0, 0, 0, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamProd(descr));
        }

        public GDAParameter[] GetParamProd(string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca produtos para relatório de listagem

        private string SqlRpt(uint idFornec, string nomeFornec, uint idGrupo, uint idSubgrupo, string codInterno, string descricao,
            int tipoProduto, int situacao, bool apenasProdutosEstoqueBaixa, decimal alturaInicio,
            decimal alturaFim, decimal larguraInicio, decimal larguraFim, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";

            string campos = selecionar ? @"
                p.*, g.Descricao as DescrGrupo, s.Descricao as DescrSubgrupo,
                (Select Sum(QtdEstoque) From produto_loja pl Where pl.idProd=p.idProd) as QtdeEstoque,
                (Select Sum(Reserva) From produto_loja pl Where pl.idProd=p.idProd) as Reserva,
                (Select Sum(Liberacao) From produto_loja pl Where pl.idProd=p.idProd) as Liberacao,
                f.NomeFantasia as NomeFornecedor, gp.Descricao as DescrGeneroProd,
                pcc.Descricao as DescrContaContabil, cv.Descricao as DescrCor,
                um.codigo as Unidade, umt.codigo as UnidadeTrib, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @"
                From produto p
                    Left Join fornecedor f on (p.idFornec=f.idFornec)
                    Left Join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                    Left Join genero_produto gp On (p.idGeneroProduto=gp.idGeneroProduto)
                    Left Join plano_conta_contabil pcc On (p.idContaContabil=pcc.idContaContabil)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left Join unidade_medida umt On (p.idUnidadeMedidaTrib=umt.idUnidadeMedida)
                Where 1 ?filtroAdicional?";

            if (idFornec > 0)
            {
                filtroAdicional += " and p.idFornec=" + idFornec;
                criterio += "Fornecedor: " + idFornec + " - " + nomeFornec;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " and p.idFornec in (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec;
            }

            if (idGrupo > 0)
            {
                filtroAdicional += " And p.IdGrupoProd=" + idGrupo;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupo) + "    ";
            }

            if (idSubgrupo > 0)
            {
                filtroAdicional += " And p.IdSubGrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
            }

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And p.codInterno like ?codInterno";

            if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                filtroAdicional += " And p.idProd In (" + ids + ")";
                criterio += "Descrição: " + descricao + "    ";
            }

            if (tipoProduto == 1)
            {
                filtroAdicional += " And (p.compra=false Or p.compra is null)";
                criterio += "Produtos de venda    ";
            }
            else if (tipoProduto == 2)
            {
                filtroAdicional += " And p.compra=true";
                criterio += "Produtos de compra    ";
            }
            else
                criterio += "Produtos de compra e venda    ";

            if (situacao > 0)
            {
                filtroAdicional += " And p.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Ativo" : situacao == 2 ? "Inativo" : "N/D") + "    ";
            }

            if (apenasProdutosEstoqueBaixa)
            {
                filtroAdicional += " And p.idProd in (select * from (select distinct idProd from produto_baixa_estoque where idProdBaixa>0) as temp)";
                criterio += "Apenas produtos que indicam produto para baixa    ";
            }

            if (alturaInicio > 0 || alturaFim > 0)
            {
                filtroAdicional += " and p.altura >= " + alturaInicio +
                    (alturaFim > 0 ? " AND p.altura <= " + alturaFim : "");
                criterio += "Altura: " + alturaInicio + "Até" + alturaFim;
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                filtroAdicional += " and p.largura >= " + larguraInicio +
                    (larguraFim > 0 ? " AND p.largura <= " + larguraFim : "");
                criterio += "Largura: " + larguraInicio + "Até" + larguraFim;
            }

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParamRpt(string codInterno, string nomeFornec, string descricao)
        {
            List<GDAParameter> p = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                p.Add(new GDAParameter("?codInterno", "%" + codInterno + "%"));

            if (!String.IsNullOrEmpty(nomeFornec))
                p.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(descricao))
                p.Add(new GDAParameter("?descr", "%" + descricao + "%"));

            return p.ToArray();
        }

        public IList<Produto> GetForRpt(uint idFornec, string nomeFornec, uint idGrupo, uint idSubgrupo, string codInterno, string descricao,
            int tipoProduto, int situacao, bool apenasProdutosEstoqueBaixa, decimal alturaInicio,
            decimal alturaFim, decimal larguraInicio, decimal larguraFim, int orderBy)
        {
            string sort = orderBy == 1 ? " order by p.descricao" : orderBy == 2 ? " order by p.codInterno" : orderBy == 3 ? " order by p.idProd" : String.Empty;

            string filtroAdicional;
            string sql = SqlRpt(idFornec, nomeFornec, idGrupo, idSubgrupo, codInterno, descricao, tipoProduto, situacao,
                apenasProdutosEstoqueBaixa, alturaInicio, alturaFim, larguraInicio, larguraFim, true,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql + sort, GetParamRpt(codInterno, nomeFornec, descricao)).ToList();
        }

        public IList<Produto> GetForListRpt(uint idFornec, string nomeFornec, uint idGrupo, uint idSubgrupo, string codInterno, string descricao,
            int tipoProduto, int situacao, bool apenasProdutosEstoqueBaixa, int orderBy, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = orderBy == 1 ? "p.descricao" : orderBy == 2 ? "p.codInterno" : orderBy == 3 ? "p.idProd" : String.Empty;

            string filtroAdicional;
            string sql = SqlRpt(idFornec, nomeFornec, idGrupo, idSubgrupo, codInterno, descricao, tipoProduto, situacao,
                apenasProdutosEstoqueBaixa, 0, 0, 0, 0, true,
                out filtroAdicional).Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParamRpt(codInterno, nomeFornec, descricao));
        }

        public int GetListRptCount(uint idFornec, string nomeFornec, uint idGrupo, uint idSubgrupo, string codInterno, string descricao,
            int tipoProduto, int situacao, bool apenasProdutosEstoqueBaixa, int orderBy)
        {
            string filtroAdicional;
            string sql = SqlRpt(idFornec, nomeFornec, idGrupo, idSubgrupo, codInterno, descricao, tipoProduto, situacao,
                apenasProdutosEstoqueBaixa, 0, 0, 0, 0, true,
                out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamRpt(codInterno, nomeFornec, descricao));
        }

        #endregion

        #region Busca produtos para tela de reajuste (Reajustados)

        private string SqlReajustado(string codInterno, string descricao, uint idGrupo, uint idSubgrupo, string idsProd, int situacao,
            bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";
            string sql = "Select " + (selecionar ? "p.*" : "Count(*)") + " From produto p Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(codInterno))
                filtroAdicional += " And p.codInterno=?codInterno";
            else if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                filtroAdicional += " And p.idProd In (" + ids + ")";
            }

            if (idGrupo > 0)
                filtroAdicional += " And IdGrupoProd=" + idGrupo;

            if (idSubgrupo > 0)
                filtroAdicional += " And IdSubGrupoProd=" + idSubgrupo;

            if (!String.IsNullOrEmpty(idsProd) && !String.IsNullOrEmpty(idsProd.Trim(',')))
                filtroAdicional += " And idProd Not In (" + idsProd.TrimEnd(',') + ")";

            if (situacao > 0)
                filtroAdicional += " AND situacao =" + situacao;

            return sql;
        }

        public IList<Produto> GetListReajustado(string codInterno, string descricao, uint idGrupo, uint idSubgrupo,
            string idsProd, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlReajustado(codInterno, descricao, idGrupo, idSubgrupo, idsProd, situacao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParamReajustado(codInterno, descricao));
        }

        public int GetReajustadoCount(string codInterno, string descricao, uint idGrupo, uint idSubgrupo, string idsProd, int situacao)
        {
            string filtroAdicional;
            string sql = SqlReajustado(codInterno, descricao, idGrupo, idSubgrupo, idsProd, situacao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamReajustado(codInterno, descricao));
        }

        private GDAParameter[] GetParamReajustado(string codInterno, string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", codInterno));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca produtos para tela de reajuste (Não reajustados)

        private string SqlNaoReajustado(string idsProd, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";
            string sql = "Select " + (selecionar ? "*" : "Count(*)") + " From produto p Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(idsProd) && !String.IsNullOrEmpty(idsProd.Trim(',')))
                filtroAdicional += " And idProd In (" + idsProd.TrimEnd(',') + ")";
            else
                filtroAdicional += " And 0>1";

            return sql;
        }

        public IList<Produto> GetListNaoReajustado(string idsProd, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlNaoReajustado(idsProd, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional);
        }

        public int GetNaoReajustadoCount(string idsProd)
        {
            string filtroAdicional;
            string sql = SqlNaoReajustado(idsProd, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional);
        }

        #endregion

        #region Busca produtos para EFD

        /// <summary>
        /// Busca produtos para EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Produto GetForEFD(string codInterno, uint idLoja, uint? idCliente, uint? idFornec, bool saida)
        {
            uint idProd = ExecuteScalar<uint>("select idProd from produto where codInterno=?cod",
                new GDAParameter("?cod", codInterno));

            var produto = GetByCodInterno(codInterno, null, idLoja, idCliente, idFornec, saida);
            produto.Unidade = UnidadeMedidaDAO.Instance.ObtemCodigo((uint)produto.IdUnidadeMedida);

            return produto;
        }

        #endregion

        #region Busca descrição/cod interno do produto

        public string GetDescrProduto(string codInterno)
        {
            return GetDescrProduto((GDASession)null, codInterno);
        }

        public string GetDescrProduto(GDASession session, string codInterno)
        {
            var idProd = ObtemIdProd(session, codInterno);
            return GetDescrProduto(session, idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna a descricao do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string GetDescrProduto(int idProd)
        {
            return GetDescrProduto(null, idProd);
        }

        /// <summary>
        /// Retorna a descricao do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string GetDescrProduto(GDASession sessao, int idProd)
        {
            string descricao = ObtemValorCampo<string>(sessao, "descricao", "idProd=" + idProd);
            return GetDescrProduto(descricao, String.Empty);
        }

        public string GetDescrProduto(string descricao, string descricaoItemGenerico)
        {
            descricao = (descricao ?? String.Empty).Trim();

            if (!String.IsNullOrEmpty(descricaoItemGenerico))
            {
                return descricaoItemGenerico /*+ (!String.IsNullOrEmpty(descricao) ?
                    String.Format(" ({0})", descricao) : String.Empty) */;
            }
            else
                return descricao;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o código interno do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string GetCodInterno(int idProd)
        {
            return GetCodInterno(null, idProd);
        }

        /// <summary>
        /// Obtém o código interno do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string GetCodInterno(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<string>(sessao, "coalesce(codInterno, '')", "idProd=" + idProd);
        }

        #endregion

        #region Retorna o NCM de um produto

        /// <summary>
        /// Retorna o MVA do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        //public string GetNCM(int idProd)
        //{
        //    string sql = "Select Coalesce(ncm, '') from produto Where idProd=" + idProd;

        //    return objPersistence.ExecuteScalar(sql).ToString();
        //}

        #endregion

        #region Aplica reajustes

        /// <summary>
        /// Retorna o novo valor, com o reajuste aplicado.
        /// </summary>
        /// <param name="valorBase"></param>
        /// <param name="tipoReajuste"></param>
        /// <param name="valorReajuste"></param>
        /// <returns></returns>
        private decimal GetValorReajustado(decimal valorBase, int tipoReajuste, decimal valorReajuste)
        {
            if (valorReajuste == 0)
                return valorBase;

            decimal retorno = 0;
            if (tipoReajuste == 1) // Reajuste para porcentagem (funciona para valores positivos e negativos)
                retorno = valorBase * ((valorReajuste / 100) + 1);
            else // Reajuste em reais
                retorno = valorBase + valorReajuste;

            return Math.Round(retorno, 2);
        }

        /// <summary>
        /// Aplica reajuste de preços aos produtos do grupo/subgrupo informados, que não tenham seu id na variável
        /// idsProdNaoReajustado.
        /// </summary>
        /// <param name="idGrupo">Grupo de Produto que sofrerá reajuste</param>
        /// <param name="idSubgrupo">Subgrupo de Produto que sofrerá reajuste</param>
        /// <param name="idsProdNaoReajustado">Ids de produtos específicos que não serão reajustados</param>
        /// <param name="valorReajuste">Valor do reajuste</param>
        /// <param name="tipoReajuste">Tipo do reajuste (1)% (2)R$</param>
        /// <param name="tipoPrecoReajuste">Preços do produto que sofrerão reajuste (1)Compra e venda (2)Compra (3)Venda</param>
        /// <param name="codInterno">Código interno para filtro dos produtos.</param>
        /// <param name="descricao">Descrição para filtro dos produtos.</param>
        public void AplicaReajuste(uint idGrupo, uint idSubgrupo, string idsProdNaoReajustado, decimal valorReajuste, int tipoReajuste,
            bool custoFabBase, bool custoCompra, bool balcao, bool obra, bool atacado, bool reposicao, bool fiscal, string codInterno, string descricao,
            int situacao)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Só executa se o valor de reajuste for diferente de zero
                    if (valorReajuste == 0)
                        return;

                    // Variável com os parâmetros do SQL
                    List<GDAParameter> lstParam = new List<GDAParameter>();

                    // Cláusulas Where
                    string where = "";

                    if (idGrupo > 0)
                        where += " And IdGrupoProd=" + idGrupo;

                    if (idSubgrupo > 0)
                        where += " And IdSubGrupoProd=" + idSubgrupo;

                    if (!String.IsNullOrEmpty(idsProdNaoReajustado) &&
                        !String.IsNullOrEmpty(idsProdNaoReajustado.Trim(',')))
                        where += " And idProd Not In (" + idsProdNaoReajustado.TrimEnd(',') + ")";

                    if (!String.IsNullOrEmpty(codInterno))
                    {
                        where += " And codInterno=?codInterno";
                        lstParam.Add(new GDAParameter("?codInterno", codInterno));
                    }

                    if (!String.IsNullOrEmpty(descricao))
                    {
                        string ids = ProdutoDAO.Instance.ObtemIds(transaction, null, descricao);
                        where += " And idProd In (" + ids + ")";
                    }

                    if (situacao > 0)
                        where += " AND situacao=" + situacao;

                    var prod =
                        objPersistence.LoadData(transaction, "select * from produto where 1" + where, lstParam.ToArray()).ToList();

                    var login = UserInfo.GetUserInfo;

                    for (var i = 0; i < prod.Count; i++)
                    {
                        var lstParamAtualiz = new List<GDAParameter>();
                        var sql = "UPDATE produto SET ";

                        if (custoFabBase)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Custo Fab. Base";
                            logAlteracao.ValorAnterior = prod[i].Custofabbase.ToString();
                            prod[i].Custofabbase = GetValorReajustado(prod[i].Custofabbase, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].Custofabbase.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "CustoFabBase=?custoFabBase,";
                            lstParamAtualiz.Add(new GDAParameter("?custoFabBase", prod[i].Custofabbase));
                        }

                        if (custoCompra)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Custo Compra";
                            logAlteracao.ValorAnterior = prod[i].CustoCompra.ToString();
                            prod[i].CustoCompra = GetValorReajustado(prod[i].CustoCompra, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].CustoCompra.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "CustoCompra=?CustoCompra,";
                            lstParamAtualiz.Add(new GDAParameter("?CustoCompra", prod[i].CustoCompra));
                        }

                        if (atacado)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Valor Atacado";
                            logAlteracao.ValorAnterior = prod[i].ValorAtacado.ToString();
                            prod[i].ValorAtacado = GetValorReajustado(prod[i].ValorAtacado, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].ValorAtacado.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "ValorAtacado=?ValorAtacado,";
                            lstParamAtualiz.Add(new GDAParameter("?ValorAtacado", prod[i].ValorAtacado));
                        }

                        if (balcao)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Valor Balcão";
                            logAlteracao.ValorAnterior = prod[i].ValorBalcao.ToString();
                            prod[i].ValorBalcao = GetValorReajustado(prod[i].ValorBalcao, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].ValorBalcao.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "ValorBalcao=?ValorBalcao,";
                            lstParamAtualiz.Add(new GDAParameter("?ValorBalcao", prod[i].ValorBalcao));
                        }

                        if (obra)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Valor Obra";
                            logAlteracao.ValorAnterior = prod[i].ValorObra.ToString();
                            prod[i].ValorObra = GetValorReajustado(prod[i].ValorObra, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].ValorObra.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "ValorObra=?ValorObra,";
                            lstParamAtualiz.Add(new GDAParameter("?ValorObra", prod[i].ValorObra));
                        }

                        if (reposicao)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Valor Reposição";
                            logAlteracao.ValorAnterior = prod[i].ValorReposicao.ToString();
                            prod[i].ValorReposicao = GetValorReajustado(prod[i].ValorReposicao, tipoReajuste,
                                valorReajuste);
                            logAlteracao.ValorAtual = prod[i].ValorReposicao.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "ValorReposicao=?ValorReposicao,";
                            lstParamAtualiz.Add(new GDAParameter("?ValorReposicao", prod[i].ValorReposicao));
                        }

                        if (fiscal)
                        {
                            var logAlteracao = new LogAlteracao();
                            logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Produto;
                            logAlteracao.IdRegistroAlt = prod[i].IdProd;
                            logAlteracao.DataAlt = DateTime.Now;
                            logAlteracao.IdFuncAlt = login.CodUser;
                            logAlteracao.Referencia = LogAlteracao.GetReferencia(transaction, logAlteracao.Tabela,
                                (uint)logAlteracao.IdRegistroAlt);

                            logAlteracao.Campo = "Valor Fiscal";
                            logAlteracao.ValorAnterior = prod[i].ValorFiscal.ToString();
                            prod[i].ValorFiscal = GetValorReajustado(prod[i].ValorFiscal, tipoReajuste, valorReajuste);
                            logAlteracao.ValorAtual = prod[i].ValorFiscal.ToString();

                            LogAlteracaoDAO.Instance.Insert(transaction, logAlteracao);

                            sql += "ValorFiscal=?ValorFiscal,";
                            lstParamAtualiz.Add(new GDAParameter("?ValorFiscal", prod[i].ValorFiscal));
                        }

                        sql = sql.TrimEnd(',') + " WHERE IdProd=" + prod[i].IdProd;

                        var prodOld = GetByIdProd((uint)prod[i].IdProd);

                        objPersistence.ExecuteCommand(transaction, sql, lstParamAtualiz.ToArray());

                        var prodNew = GetByIdProd((uint)prod[i].IdProd);
                        /* Chamado 43961 */
                        MensagemDAO.Instance.EnviarMsgPrecoProdutoAlterado(prodOld, prodNew);
                        Email.EnviaEmailAdministradorPrecoProdutoAlterado(transaction, prodOld, prodNew);
                    }


                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(
                        string.Format(@"AplicaReajuste(idGrupo({0}), idSubgrupo({1}), idsProdNaoReajustado({2}), valorReajuste({3}), tipoReajuste({4}),
                            custoFabBase({5}), custoCompra({6}), balcao({7}), obra({8}), atacado({9}), reposicao({10}), fiscal({11}), codInterno({12}), descricao({13}),
                            situacao({14}))", idGrupo, idSubgrupo, idsProdNaoReajustado, valorReajuste, tipoReajuste, custoFabBase, custoCompra, balcao, obra,
                            atacado, reposicao, fiscal, codInterno, descricao, situacao), ex);
                }
            }
        }

        #endregion

        #region Inserção de produtos compra

        public uint InsertCompra(Produto objInsert)
        {
            objInsert.Compra = true;

            return base.Insert(objInsert);
        }

        #endregion

        #region Atualiza preço de custo

        /// <summary>
        /// Atualiza o preço de um produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="precoCusto"></param>
        public void AtualizaPreco(uint idProd, int tipoPreco, Single preco)
        {
            using (var trans = new GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    Produto prod = GetElementByPrimaryKey(idProd);

                    string campo = tipoPreco == 0 ? "custoFabBase" :
                        tipoPreco == 1 ? "custoCompra" :
                        tipoPreco == 2 ? "valorAtacado" :
                        tipoPreco == 3 ? "valorBalcao" :
                        tipoPreco == 4 ? "valorObra" : "";

                    string sql = "Update produto set " + campo + "=" + preco.ToString().Replace(',', '.') + " Where idProd=" + idProd;

                    objPersistence.ExecuteCommand(trans, sql);

                    /* Chamado 43961 */
                    var produtoAtualizado = GetElementByPrimaryKey(trans, idProd);
                    MensagemDAO.Instance.EnviarMsgPrecoProdutoAlterado(prod, produtoAtualizado);
                    Email.EnviaEmailAdministradorPrecoProdutoAlterado(trans, prod, produtoAtualizado);

                    LogAlteracaoDAO.Instance.LogProduto(trans, prod, LogAlteracaoDAO.SequenciaObjeto.Atual);

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();
                    throw ex;
                }
            }
        }

        public void AtualizaPreco(GDASession sessao, ProdutosCompra pc)
        {
            if (pc.Total <= 0)
                return;

            var prodAtual = GetElement(sessao, pc.IdProd);

            var sql = @"
            UPDATE produto
            SET custoCompra = ?custoCompra,
                valorAtacado = IF(MarkUp > 0, ?custoCompra * (1 + (MarkUp / 100)), valorAtacado),
                valorBalcao = IF(MarkUp > 0, ?custoCompra * (1 + (MarkUp / 100)), valorBalcao),
                valorObra = IF(MarkUp > 0, ?custoCompra * (1 + (MarkUp / 100)), valorObra),
                valorReposicao = IF(MarkUp > 0, ?custoCompra * (1 + (MarkUp / 100)), valorReposicao)
            WHERE IdProd = " + pc.IdProd;

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?custoCompra", pc.Valor));

            LogAlteracaoDAO.Instance.LogProduto(sessao, prodAtual, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Verificações do produto

        /// <summary>
        /// Verifica se o produto passado é do grupo vidro
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool IsVidro(string codInterno)
        {
            string sql = "Select Count(*) From produto Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                " And codInterno=?cod";

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?cod", codInterno)) > 0;
        }

        /// <summary>
        /// Verifica se o produto passado é do grupo vidro
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsVidro(GDASession sessao, int idProd)
        {
            string sql = "Select Count(*) From produto Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro +
                " And idProd=" + idProd;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o produto passado é do grupo alumínio
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsAluminio(uint idProd)
        {
            string sql = "Select Count(*) From produto Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Alumínio +
                " And idProd=" + idProd;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o produto pode ser usado para o Pedido Produção.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsProdutoProducao(int idProd)
        {
            return IsProdutoProducao(null, idProd);
        }

        /// <summary>
        /// Verifica se o produto pode ser usado para o Pedido Produção.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsProdutoProducao(GDASession sessao, int idProd)
        {
            string sql = @"select count(*) from produto where idSubgrupoProd in (
                select idSubgrupoProd from subgrupo_prod where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"
                and produtosEstoque=true) and idProd=" + idProd;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o produto é de venda
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsProdutoVenda(GDASession sessao, int idProd)
        {
            var idGrupoProd = ObtemValorCampo<int>(sessao, "idGrupoProd", "idProd=" + idProd);
            var idSubGrupoProduto = ObtemValorCampo<int>(sessao, "idSubGrupoProd", "idProd=" + idProd);

            return idGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra ||
                (idGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro &&
                !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, idGrupoProd, idSubGrupoProduto));
        }

        public bool IsRedondo(uint idProd)
        {
            return ObtemValorCampo<bool>("redondo", "idProd=" + idProd);
        }

        /// <summary>
        /// Verifica se o produto é de composição
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool IsProdutoLamComposicao(GDASession sessao, int idProd)
        {
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProd);

            return tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
        }

        #endregion

        #region Retorna o primeiro código interno da tabela de produto (usado para os beneficiamentos na tela de cadastro de produto)

        /// <summary>
        /// Retorna o código interno do primeiro produto cadastrado.
        /// </summary>
        /// <returns></returns>
        public string GetFirstProdutoCodInterno()
        {
            return GetFirstProdutoCodInterno(null);
        }

        /// <summary>
        /// Retorna o código interno do primeiro produto cadastrado de um grupo especificado.
        /// </summary>
        /// <returns></returns>
        public string GetFirstProdutoCodInterno(int? idGrupoProd)
        {
            string sql = "select CodInterno from produto " + (idGrupoProd > 0 ? "where idGrupoProd=" + idGrupoProd + " AND SITUACAO=1 " +
                (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(idGrupoProd.Value) ? " and espessura is not null and espessura>0" : "") : "") + " limit 1";

            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null ? retorno.ToString() : null;
        }

        #endregion

        #region Retorna o valor de campos do produto

        /// <summary>
        /// Retorna a área mínima do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public float ObtemAreaMinima(int? idProd)
        {
            return idProd > 0 ? ObtemValorCampo<float>(null, "areaMinima", "idProd=" + idProd) : 0;
        }

        /// <summary>
        /// Retorna a área mínima do produto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public float ObtemAreaMinima(GDASession sessao, int? idProd)
        {
            return idProd > 0 ? ObtemValorCampo<float>(sessao, "areaMinima", "idProd=" + idProd) : 0;
        }

        /// <summary>
        /// Retorna a descrição do produto.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public string ObtemDescricao(string codInterno)
        {
            var idProd = ObtemIdProd(codInterno);
            return ObtemDescricao(idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna a descrição do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string ObtemDescricao(int idProd)
        {
            return ObtemDescricao(null, idProd);
        }

        /// <summary>
        /// Retorna a descrição do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string ObtemDescricao(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<string>(sessao, "descricao", "idProd=" + idProd);
        }

        /// <summary>
        /// Retorna o valor do campo forma
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public string ObtemForma(string codInterno)
        {
            var idProd = ObtemIdProd(codInterno);
            return ObtemForma(null, idProd, null);
        }

        /// <summary>
        /// Retorna o valor do campo forma
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public string ObtemForma(GDASession session, int idProd, int? idProdBaixa)
        {
            //Recupera a forma do produto de baixa ou do produto caso o produto de baixa não possua forma
            var formaProdBaixa = "";
            if (idProdBaixa > 0)
                formaProdBaixa = ProdutoBaixaEstoqueDAO.Instance.ObtemValorCampo<string>(session, "Forma", "IdProdBaixaEst=" + idProdBaixa);

            var formaProd = ObtemValorCampo<string>(session, "Forma", "idProd=" + idProd);

            return string.IsNullOrEmpty(formaProdBaixa) ? formaProd : formaProdBaixa;
        }

        /// <summary>
        /// Retorna o valor do campo espessura
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public float ObtemEspessura(int idProd)
        {
            return ObtemEspessura(null, idProd);
        }

        /// <summary>
        /// Retorna o valor do campo espessura
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public float ObtemEspessura(GDASession session, int idProd)
        {
            return ObtemValorCampo<float>(session, "espessura", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o código da unidade de medida de um produto
        /// </summary>
        public string ObtemUnidadeMedida(int idProd)
        {
            return ObtemUnidadeMedida(null, idProd);
        }

        /// <summary>
        /// Obtém o código da unidade de medida de um produto
        /// </summary>
        public string ObtemUnidadeMedida(GDASession session, int idProd)
        {
            string sql = @"
                Select codigo From unidade_medida
                Where idUnidadeMedida=(
                    Select idUnidadeMedida From produto
                    Where idProd=" + idProd + @"
                )";

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString() == String.Empty ? String.Empty : obj.ToString();
        }

        /// <summary>
        /// Obtém o código da unidade de medida tributada de um produto
        /// </summary>
        public string ObtemUnidadeMedidaTrib(uint idProd)
        {
            return ObtemUnidadeMedidaTrib(null, idProd);
        }

        /// <summary>
        /// Obtém o código da unidade de medida tributada de um produto
        /// </summary>
        public string ObtemUnidadeMedidaTrib(GDASession session, uint idProd)
        {
            string sql = @"
                Select codigo From unidade_medida
                Where idUnidadeMedida=(
                    Select idUnidadeMedidaTrib From produto
                    Where idProd=" + idProd + @"
                )";

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString() == String.Empty ? String.Empty : obj.ToString();
        }

        /// <summary>
        /// Obtém o arquivo de mesa de corte associado à este produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public uint? ObtemIdArquivoMesaCorte(uint idProd)
        {
            return ObtemIdArquivoMesaCorte(null, idProd);
        }

        /// <summary>
        /// Obtém o arquivo de mesa de corte associado à este produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public uint? ObtemIdArquivoMesaCorte(GDASession sessao, uint idProd)
        {
            string sql = "Select idArquivoMesaCorte From produto Where idProd=" + idProd;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && !String.IsNullOrEmpty(obj.ToString()) ? (uint?)Glass.Conversoes.StrParaUint(obj.ToString()) : null;
        }

        /// <summary>
        /// Obtém o tipo do arquivo de mesa de corte associado à este produto.
        /// </summary>
        public TipoArquivoMesaCorte? ObterTipoArquivoMesaCorte(GDASession sessao, int idProd)
        {
            var sql = string.Format("SELECT TipoArquivo FROM produto WHERE IdProd={0}", idProd);

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && !string.IsNullOrEmpty(obj.ToString()) ? (TipoArquivoMesaCorte?)obj.ToString().StrParaInt() : null;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o id do grupo do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int ObtemIdGrupoProd(int idProd)
        {
            return ObtemIdGrupoProd(null, idProd);
        }

        /// <summary>
        /// Obtém o id do grupo do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int ObtemIdGrupoProd(GDASession sessao, int idProd)
        {
            string sql = "Select idGrupoProd From produto Where idProd=" + idProd;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaInt(obj.ToString()) : 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o id do subgrupo do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int? ObtemIdSubgrupoProd(int idProd)
        {
            return ObtemIdSubgrupoProd(null, idProd);
        }

        /// <summary>
        /// Obtém o id do subgrupo do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int? ObtemIdSubgrupoProd(GDASession sessao, int idProd)
        {
            string sql = "Select idSubgrupoProd From produto Where idProd=" + idProd;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && !String.IsNullOrEmpty(obj.ToString()) ? (int?)Glass.Conversoes.StrParaInt(obj.ToString()) : null;
        }

        /// <summary>
        /// Retorna o produto do subgrupo "Sobras de Vidro (Produção)" que possui a espessura e a cor do vidro passadas.
        /// </summary>
        /// <param name="espessura"></param>
        /// <param name="idCorVidro"></param>
        /// <returns></returns>
        public uint ObtemIdProdSobra(float espessura, uint idCorVidro)
        {
            return ObtemValorCampo<uint>("idProd", "espessura=" + espessura.ToString().Replace(",", ".") + " and idCorVidro=" + idCorVidro + " and idSubgrupoProd=" +
                (uint)Utils.SubgrupoProduto.SobrasDeVidro);
        }

        public bool ObtemCompra(uint idProd)
        {
            return ObtemValorCampo<bool>("compra", "idProd=" + idProd);
        }

        public bool ObtemItemGenerico(uint idProd)
        {
            return ObtemValorCampo<bool>("itemGenerico", "idProd=" + idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o custo de compra do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemCustoCompra(int idProd)
        {
            return ObtemCustoCompra(null, idProd);
        }

        /// <summary>
        /// Obtém o custo de compra do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemCustoCompra(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<decimal>(sessao, "custoCompra", "idProd=" + idProd);
        }

        /// <summary>
        /// Retornar o custo do fornecedor
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemCustoForn(int idProd)
        {
            return ObtemValorCampo<decimal>("custoFabBase", "idProd=" + idProd);
        }

        /// <summary>
        /// Retorna o valor fiscal do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemValorFiscal(int idProd)
        {
            return ObtemValorCampo<decimal>("valorFiscal", "idProd=" + idProd);
        }

        /// <summary>
        /// A área mínima do produto deve ser cobrada?
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool ObtemAtivarAreaMinima(GDASession sessao, int idProd)
        {
            string sql = "Select count(*) From produto Where ativarAreaMinima=true and idProd=" + idProd;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Recupera o id do produto base
        /// </summary>
        public int? ObterProdutoBase(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<int?>(sessao, "IdProdBase", "idProd=" + idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a metragem do produto com base na altura/largura inserida
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemM2BoxPadrao(int idProd)
        {
            return ObtemM2BoxPadrao(null, idProd);
        }

        /// <summary>
        /// Obtém a metragem do produto com base na altura/largura inserida
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemM2BoxPadrao(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<decimal>(sessao, "if(altura>1850 and altura<1900, 1.9, altura/1000)*(largura/1000)", "idProd=" + idProd);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a metragem do produto que for chapa de vidro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemM2Chapa(int idProd)
        {
            return ObtemM2Chapa(null, idProd);
        }

        /// <summary>
        /// Obtém a metragem do produto que for chapa de vidro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemM2Chapa(GDASession sessao, int idProd)
        {
            string sql = @"
                SELECT (p.altura * p.largura) / 1000000
                FROM produto p
                    INNER JOIN subgrupo_prod sgp ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
                WHERE p.idProd=" + idProd + @"
                    AND sgp.tipoSubgrupo IN(" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + ")";

            return ExecuteScalar<decimal>(sessao, sql);
        }

        /// <summary>
        /// Obtém o id da cor do vidro do produto.
        /// </summary>
        public int? ObtemIdCorVidro(int idProd)
        {
            return ObtemIdCorVidro(null, idProd);
        }

        /// <summary>
        /// Obtém o id da cor do vidro do produto.
        /// </summary>
        public int? ObtemIdCorVidro(GDASession session, int idProd)
        {
            return ObtemValorCampo<int?>(session, "idCorVidro", "idProd=" + idProd);
        }

        /// <summary>
        /// Retorna o valor para transferência do produto
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public decimal ObtemValorTransferencia(GDASession sessao, int idProd)
        {
            return ObtemValorCampo<decimal>(sessao, "ValorTransferencia", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o id da cor do alumínio do produto.
        /// </summary>
        public uint? ObtemIdCorAluminio(GDASession session, int idProd)
        {
            return ObtemValorCampo<uint?>(session, "idCorAluminio", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o id da cor da ferragem do produto.
        /// </summary>
        public uint? ObtemIdCorFerragem(GDASession session, int idProd)
        {
            return ObtemValorCampo<uint?>(session, "idCorFerragem", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o id do fornecedor.
        /// </summary>
        public uint? ObtemIdFornec(int idProd)
        {
            return ObtemValorCampo<uint?>("idFornec", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o id da aplicação.
        /// </summary>
        public uint? ObtemIdAplicacao(int idProd)
        {
            return ObtemValorCampo<uint?>("idAplicacao", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtém o id do processo.
        /// </summary>
        public uint? ObtemIdProcesso(int idProd)
        {
            return ObtemValorCampo<uint?>("idProcesso", "idProd=" + idProd);
        }

        /// <summary>
        /// Obtem o NCM por loja, caso não tenha por loja busca o padrão do produto
        /// </summary>
        public string ObtemNcm(int idProd, uint idLoja)
        {
            return ObtemNcm(null, idProd, idLoja);
        }

        /// <summary>
        /// Obtem o NCM por loja, caso não tenha por loja busca o padrão do produto
        /// </summary>
        public string ObtemNcm(GDASession session, int idProd, uint idLoja)
        {
            var sql = @"
                SELECT COALESCE(tmp.ncm, p.ncm)
                FROM produto p
                    LEFT JOIN
                    (
                    	SELECT idProd, ncm
                        FROM produto_ncm
                        WHERE IdProd = ?idProd AND idLoja = ?idLoja
                    ) as tmp ON (p.IdProd = tmp.IdProd)
                WHERE p.IdProd = ?idProd";

            return ExecuteScalar<string>(session, sql, new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        #endregion

        #region Obtem alíquota de IPI

        public float ObtemAliqIpi(uint idProd)
        {
            return ObtemValorCampo<float>("AliqIpi", "idProd=" + idProd);
        }

        #endregion

        #region Busca para movimentação do estoque fiscal

        private string SqlMovEstoqueFiscal(string dataIni, string dataFim, string codInterno, string descrProduto, uint idGrupoProd, uint idSubgrupoProd,
            uint idLoja, bool isRpt, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string criterio = "";

            string campoData = "coalesce(if(nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + ", nf.dataSaidaEnt, null), nf.dataEmissao)";
            string campoEstFiscPeriodo = "if(coalesce(s.tipoCalculoNf, g.tipoCalculoNf, s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd +
                ") in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + ", " + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ", " + (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2 +
                "), pnf.totM, pnf.qtde)*if(nf.tipoDocumento=" + (int)NotaFiscal.TipoDoc.Saída + ", -1, 1)";

            string campos = selecionar ? "p.*, concat(g.descricao, if(s.idSubgrupoProd is not null, concat(' - ', s.descricao), '')) as descrTipoProduto, " +
                "(select sum(estoqueFiscal) from produto_loja where idProd=p.idProd) as estoqueFiscal, sum(" + campoEstFiscPeriodo + ") as estoqueFiscalPeriodo, " +
                "'$$$' as criterio, nf.numeroNfe, " + campoData + " as dataNf, nf.tipoDocumento as tipoDocumentoNf" : "p.idProd";

            string sql = @"
                select " + campos + @"
                from produto p
                    left join produtos_nf pnf on (p.idProd=pnf.idProd)
                    left join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                where 1";

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and " + campoData + ">=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and " + campoData + "<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " and p.codInterno=?codInterno";
                criterio += "Produto: " + codInterno + " - " + descrProduto + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(descrProduto))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProduto);
                sql += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProduto + "    ";
                temFiltro = true;
            }

            if (idGrupoProd > 0)
            {
                sql += " and p.idGrupoProd=" + idGrupoProd;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
                temFiltro = true;
            }

            if (idSubgrupoProd > 0)
            {
                sql += " and p.idSubgrupoProd=" + idSubgrupoProd;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd) + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql += " and nf.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
                temFiltro = true;
            }

            sql += " group by p.idProd" + (isRpt ? ", pnf.idNf" : "");

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParamsMovEstoqueFiscal(string dataIni, string dataFim, string codInterno, string descrProduto)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(codInterno))
                lstParams.Add(new GDAParameter("?codInterno", codInterno));
            else if (!String.IsNullOrEmpty(descrProduto))
                lstParams.Add(new GDAParameter("?descricao", "%" + descrProduto + "%"));

            return lstParams.ToArray();
        }

        public IList<Produto> GetListMovEstoqueFiscal(string dataIni, string dataFim, string codInterno, string descrProduto, uint idGrupoProd, uint idSubgrupoProd,
            uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            return LoadDataWithSortExpression(SqlMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto, idGrupoProd, idSubgrupoProd, idLoja, false, true, out temFiltro),
                sortExpression, startRow, pageSize, temFiltro, GetParamsMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto));
        }

        public int GetListMovEstoqueFiscalCount(string dataIni, string dataFim, string codInterno, string descrProduto, uint idGrupoProd, uint idSubgrupoProd, uint idLoja)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(SqlMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto, idGrupoProd, idSubgrupoProd, idLoja, false, true, out temFiltro),
                temFiltro, GetParamsMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto));
        }

        public IList<Produto> GetForRptMovEstoqueFiscal(string dataIni, string dataFim, string codInterno, string descrProduto, uint idGrupoProd, uint idSubgrupoProd, uint idLoja)
        {
            bool temFiltro;
            return objPersistence.LoadData(SqlMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto, idGrupoProd, idSubgrupoProd, idLoja, true, true, out temFiltro),
                GetParamsMovEstoqueFiscal(dataIni, dataFim, codInterno, descrProduto)).ToList();
        }

        #endregion

        #region Recupera o valor de tabela de um produto

        /// <summary>
        /// Recupera o valor de tabela de um produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <param name="revenda"></param>
        /// <param name="reposicao"></param>
        /// <param name="percDescontoQtde"></param>
        /// <returns></returns>
        public decimal GetValorTabela(int idProd, int? tipoEntrega, uint? idCliente, bool revenda, bool reposicao, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return GetValorTabela(null, idProd, tipoEntrega, idCliente, revenda, reposicao, percDescontoQtde, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recupera o valor de tabela de um produto.
        /// </summary>
        public decimal GetValorTabela(GDASession sessao, int idProd, int? tipoEntrega, uint? idCliente, bool revenda, bool reposicao, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if ((idCliente ?? 0) == 0)
            {
                idCliente = idPedido > 0
                    ? PedidoDAO.Instance.GetIdCliente(sessao, (uint)idPedido)
                    : 0;
            }

            revenda = idCliente > 0 ? ClienteDAO.Instance.IsRevenda(sessao ,idCliente) : revenda;

            int id = 0;
            ContainerCalculoDTO.TipoContainer? tipo = null;
            var tipoVenda = 0;
            var idParcela = 0;

            #region Recuperação dos dados do pedido, projeto e orçamento

            if (idPedido > 0)
            {
                id = idPedido.Value;
                tipo = ContainerCalculoDTO.TipoContainer.Pedido;
                tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(sessao, (uint)idPedido.Value);
                idParcela = (int)PedidoDAO.Instance.ObtemIdParcela(sessao, (uint)idPedido.Value).GetValueOrDefault();
            }
            else if (idProjeto > 0)
            {
                id = idProjeto.Value;
                tipo = ContainerCalculoDTO.TipoContainer.Projeto;
                var idClienteProjeto = ProjetoDAO.Instance.ObtemIdCliente(sessao, (uint)idProjeto.Value);
                tipoVenda = (int)ProjetoDAO.Instance.GetTipoVenda(sessao, (uint)idProjeto.Value);
                idParcela = idClienteProjeto > 0 ? (int)ClienteDAO.Instance.ObtemTipoPagto(sessao, idClienteProjeto.Value).GetValueOrDefault() : 0;
            }
            else if (idOrcamento > 0)
            {
                id = idOrcamento.Value;
                tipo = ContainerCalculoDTO.TipoContainer.Orcamento;
                tipoVenda = OrcamentoDAO.Instance.ObterTipoVenda(sessao, idOrcamento.Value).GetValueOrDefault();
                idParcela = OrcamentoDAO.Instance.ObterIdParcela(sessao, idOrcamento.Value).GetValueOrDefault();
            }

            #endregion

            var containerCalculo = new ContainerCalculoDTO()
            {
                Id = (uint)id,
                Tipo = tipo,
                TipoVenda = tipoVenda,
                IdParcela = (uint)idParcela,
                TipoEntrega = tipoEntrega.GetValueOrDefault(),
                Reposicao = reposicao,
                Cliente = new ClienteDTO(idCliente ?? 0, revenda)
            };

            var produtoCalculo = new ProdutoCalculoDTO()
            {
                IdProduto = (uint)idProd,
                PercDescontoQtde = percDescontoQtde
            };

            produtoCalculo.InicializarParaCalculo(sessao, containerCalculo);

            return produtoCalculo.DadosProduto.ValorTabela();
        }

        #endregion

        #region Recupera o valor mínimo para um produto

        public enum TipoBuscaValorMinimo
        {
            ProdutoOrcamento,
            ProdutoPedido,
            ProdutoPedidoEspelho,
            ProdutoTrocaDevolucao,
            ProdutoTrocado,
            MaterialItemProjeto
        }

        /// <summary>
        /// Recupera o valor mínimo para um produto.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public decimal GetValorMinimo(uint id, TipoBuscaValorMinimo tipoBusca, bool revenda, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return GetValorMinimo(null, id, tipoBusca, revenda, percDescontoQtde, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recupera o valor mínimo para um produto.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public decimal GetValorMinimo(GDASession sessao, uint id, TipoBuscaValorMinimo tipoBusca, bool revenda, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if (id == 0)
                return 0;

            int idProd = 0;
            uint idParent = 0;
            int? tipoEntrega = null;
            uint? idCliente = null, idPed = null;
            float percDescontoQtdeAtual = 0;
            decimal valorUnit = 0;
            bool reposicao = false;

            switch (tipoBusca)
            {
                case TipoBuscaValorMinimo.ProdutoOrcamento:
                    idProd = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<int>(sessao, "idProduto", "idProd=" + id);
                    valorUnit = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorProd", "idProd=" + id);
                    percDescontoQtdeAtual = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<float>(sessao, "percDescontoQtde", "idProd=" + id);
                    idParent = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<uint>(sessao, "idOrcamento", "idProd=" + id);
                    tipoEntrega = OrcamentoDAO.Instance.ObtemValorCampo<int?>(sessao, "tipoEntrega", "idOrcamento=" + idParent);
                    idCliente = OrcamentoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idCliente", "idOrcamento=" + idParent);
                    reposicao = false;
                    break;

                case TipoBuscaValorMinimo.ProdutoPedido:
                    idProd = (int)ProdutosPedidoDAO.Instance.ObtemIdProd(sessao, id);
                    valorUnit = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorVendido", "idProdPed=" + id);
                    percDescontoQtdeAtual = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(sessao, "percDescontoQtde", "idProdPed=" + id);
                    idParent = ProdutosPedidoDAO.Instance.ObtemIdPedido(sessao, id);
                    tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(sessao, idParent);
                    idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idParent);
                    reposicao = PedidoDAO.Instance.IsPedidoReposicao(sessao, idParent.ToString());
                    break;

                case TipoBuscaValorMinimo.ProdutoPedidoEspelho:
                    idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<int>(sessao, "idProd", "idProdPed=" + id);
                    valorUnit = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorVendido", "idProdPed=" + id);
                    percDescontoQtdeAtual = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>(sessao, "percDescontoQtde", "idProdPed=" + id);
                    idParent = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(sessao, id);
                    tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(sessao, idParent);
                    idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idParent);
                    reposicao = PedidoDAO.Instance.IsPedidoReposicao(sessao, idParent.ToString());
                    break;

                case TipoBuscaValorMinimo.ProdutoTrocaDevolucao:
                    idProd = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<int>(sessao, "idProd", "idProdTrocaDev=" + id);
                    valorUnit = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorVendido", "idProdTrocaDev=" + id);
                    percDescontoQtdeAtual = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<float>(sessao, "percDescontoQtde", "idProdTrocaDev=" + id);
                    idParent = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idTrocaDevolucao", "idProdTrocaDev=" + id);
                    idPed = TrocaDevolucaoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idPedido", "idTrocaDevolucao=" + idParent);
                    tipoEntrega = idPed > 0 ? (int?)PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPed.Value) : null;
                    idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente(sessao, idParent);
                    reposicao = false;
                    break;

                case TipoBuscaValorMinimo.ProdutoTrocado:
                    idProd = ProdutoTrocadoDAO.Instance.ObtemValorCampo<int>(sessao, "idProd", "idProdTrocado=" + id);
                    valorUnit = ProdutoTrocadoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorVendido", "idProdTrocado=" + id);
                    percDescontoQtdeAtual = ProdutoTrocadoDAO.Instance.ObtemValorCampo<float>(sessao, "percDescontoQtde", "idProdTrocado=" + id);
                    idParent = ProdutoTrocadoDAO.Instance.ObtemValorCampo<uint>(sessao, "idTrocaDevolucao", "idProdTrocado=" + id);
                    idPed = TrocaDevolucaoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idPedido", "idTrocaDevolucao=" + idParent);
                    tipoEntrega = idPed > 0 ? (int?)PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPed.Value) : null;
                    idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente(sessao, idParent);
                    break;

                case TipoBuscaValorMinimo.MaterialItemProjeto:
                    idProd = MaterialItemProjetoDAO.Instance.ObtemValorCampo<int>(sessao, "idProd", "idMaterItemProj=" + id);
                    valorUnit = MaterialItemProjetoDAO.Instance.ObtemValorCampo<decimal>(sessao, "valor", "idMaterItemProj=" + id);
                    percDescontoQtdeAtual = 0;
                    idParent = MaterialItemProjetoDAO.Instance.ObtemValorCampo<uint>(sessao, "idItemProjeto", "idMaterItemProj=" + id);
                    ItemProjetoDAO.Instance.GetTipoEntregaCliente(sessao, idParent, out tipoEntrega, out idCliente, out reposicao);
                    break;
            }

            return GetValorMinimo(sessao, idProd, tipoEntrega, idCliente, revenda, reposicao,
                Math.Max(percDescontoQtde, percDescontoQtdeAtual), valorUnit, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recupera o valor mínimo para um produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <param name="desconto"></param>
        /// <param name="acrescimo"></param>
        /// <returns></returns>
        public decimal GetValorMinimo(int idProd, int? tipoEntrega, uint? idCliente, bool revenda,
            bool reposicao, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return GetValorMinimo(null, idProd, tipoEntrega, idCliente, revenda, reposicao, percDescontoQtde,
                idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recupera o valor mínimo para um produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="tipoEntrega"></param>
        /// <param name="idCliente"></param>
        /// <param name="desconto"></param>
        /// <param name="acrescimo"></param>
        /// <returns></returns>
        public decimal GetValorMinimo(GDASession sessao, int idProd, int? tipoEntrega, uint? idCliente, bool revenda,
            bool reposicao, float percDescontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return GetValorMinimo(sessao, idProd, tipoEntrega, idCliente, revenda, reposicao, percDescontoQtde, 0, idPedido, idProjeto, idOrcamento);
        }

        private decimal GetValorMinimo(GDASession sessao, int idProd, int? tipoEntrega, uint? idCliente, bool revenda, bool reposicao,
            float percDescontoQtde, decimal valorNegociado, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            var idGrupoProd = ObtemValorCampo<int>(sessao, "idGrupoProd", "idProd=" + idProd);
            var idSubgrupoProd = ObtemValorCampo<int?>(sessao, "idSubgrupoProd", "idProd=" + idProd);

            var desc = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(sessao, idCliente.GetValueOrDefault() > 0 ?
                idCliente.Value : 0, idGrupoProd, idSubgrupoProd, idProd, idPedido, idProjeto);
            decimal valorUnitario = GetValorTabela(sessao, idProd, tipoEntrega, idCliente, revenda, reposicao, percDescontoQtde, idPedido, idProjeto, idOrcamento);

            if (valorNegociado == 0)
                return valorUnitario;

            // Chamado 60618: Não deve deduzir o percentual de desconto por qtd, pq por algum motivo o desconto por qtd não reduz mais o valor unitário do produto
            return Math.Round(Math.Min(valorUnitario, valorNegociado) /* * (1 - ((decimal)percDescontoQtde / 100))*/, 2);
        }

        #endregion

        #region Atualiza dados de tributação do produto (NFe > Produtos)

        /// <summary>
        /// Atualiza dados de tributação do produto (NFe > Produtos)
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <returns></returns>
        public int AtualizaTrib(Produto objUpdate)
        {
            string sql = "Update produto Set /*AliqIcms=" + objUpdate.AliqICMS.ToString().Replace(",", ".") +
                ",*/ AliqIpi=" + objUpdate.AliqIPI.ToString().Replace(",", ".") + ", Cst=?cst, " + "Ncm=?ncm, /*mva=" +
                objUpdate.Mva.ToString().Replace(",", ".");

            sql += objUpdate.CstIpi != null ? ", CstIpi=" + objUpdate.CstIpi : "";
            sql += objUpdate.Csosn != null ? ", Csosn=" + objUpdate.Csosn : "";
            sql += objUpdate.CodigoEX != null ? ", CodigoEx=" + objUpdate.CodigoEX : "";
            sql += objUpdate.IdGeneroProduto != null ? ", IdGeneroProduto=" + objUpdate.IdGeneroProduto : "";
            sql += objUpdate.TipoMercadoria != null ? ", TipoMercadoria=" + (int)objUpdate.TipoMercadoria : "";
            sql += objUpdate.IdContaContabil != null ? ", IdContaContabil=" + objUpdate.IdContaContabil : "";

            sql += " Where idProd=" + objUpdate.IdProd;

            return objPersistence.ExecuteCommand(sql, new GDAParameter("?cst", objUpdate.Cst),
                new GDAParameter("?ncm", objUpdate.Ncm));
        }

        #endregion

        #region Alterar dados fiscais de produtos

        /// <summary>
        /// Altera os dados fiscais de todos os produtos passados.
        /// </summary>
        public void AlteraDadosFiscais(string idsProd, string dadosAliqIcms, float aliqIpi,
            string dadosMva, string ncm, string cst, string cstIpi, string csosn, string codEx, string genProd, string tipoMerc,
            string planoContabil, bool substituirICMS, bool substituirMVA, bool AlterarICMS, bool alterarMVA, string cest)
        {
            var mensagemErro = String.Empty;
            try
            {
                FilaOperacoes.AlteraDadosFiscaisProduto.AguardarVez();

                if (String.IsNullOrEmpty(idsProd))
                    return;

                var prod = objPersistence.LoadData("select * from produto where idProd in (" +
                    idsProd.TrimEnd(',') + ")").ToList();

                foreach (Produto p in prod)
                {
                    using (var transaction = new GDA.GDATransaction())
                    {
                        try
                        {
                            transaction.BeginTransaction();

                            // Se tiver Alíquota ICMS ou FCP para ser alterada ou substituida
                            if (dadosAliqIcms != "-1")
                            {
                                // Altera as alíquotas padrões
                                string[] dados = dadosAliqIcms.Split('/');

                                // Se for substituir as alíquotas padrões existentes
                                if (substituirICMS)
                                {
                                    p.AliqICMS = new List<RelModel.ControleIcmsProdutoPorUf>();

                                    p.AliqICMS.Add(new RelModel.ControleIcmsProdutoPorUf()
                                    {
                                        AliquotaIntraestadual = dados[0].StrParaFloat(),
                                        AliquotaInterestadual = dados[1].StrParaFloat(),
                                        AliquotaInternaDestinatario = dados[2].StrParaFloat(),
                                        AliquotaFCPIntraestadual = dados[3].StrParaFloat(),
                                        AliquotaFCPInterestadual = dados[4].StrParaFloat()
                                    });
                                }
                                // Se form alterar as alíquotas padrões existentes
                                else
                                {
                                    // Busca a alíquota padrão
                                    var aliqIcms = p.AliqICMS
                                        .Where(f => f.TipoCliente == null && string.IsNullOrEmpty(f.UfDestino) && string.IsNullOrEmpty(f.UfOrigem))
                                        .FirstOrDefault();

                                    // Se não tiver alíquota padrão cadastrada
                                    if (aliqIcms == null)
                                    {
                                        // Adiciona nova alíquota padrão
                                        p.AliqICMS.Add(new RelModel.ControleIcmsProdutoPorUf()
                                        {
                                            AliquotaIntraestadual = dados[0].StrParaFloat(),
                                            AliquotaInterestadual = dados[1].StrParaFloat(),
                                            AliquotaInternaDestinatario = dados[2].StrParaFloat(),
                                            AliquotaFCPIntraestadual = dados[3].StrParaFloat(),
                                            AliquotaFCPInterestadual = dados[4].StrParaFloat()
                                        });
                                    }
                                    // Se tiver, substitui os dados da alíquota padrão
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(dados[0]))
                                            aliqIcms.AliquotaIntraestadual = dados[0].StrParaFloat();

                                        if (!string.IsNullOrEmpty(dados[1]))
                                            aliqIcms.AliquotaInterestadual = dados[1].StrParaFloat();

                                        if (!string.IsNullOrEmpty(dados[2]))
                                            aliqIcms.AliquotaInternaDestinatario = dados[2].StrParaFloat();

                                        if (!string.IsNullOrEmpty(dados[3]))
                                            aliqIcms.AliquotaFCPIntraestadual = dados[3].StrParaFloat();

                                        if (!string.IsNullOrEmpty(dados[4]))
                                            aliqIcms.AliquotaFCPInterestadual = dados[4].StrParaFloat();
                                    }
                                }

                                // Altera as alíquotas das exceções por estado
                                for (int i = 5; i < dados.Length; i++)
                                {
                                    if (string.IsNullOrEmpty(dados[i]))
                                        continue;

                                    string[] item = dados[i].Split('|');

                                    // Se form alterar as alíquotas existentes
                                    if (AlterarICMS)
                                    {
                                        // Busca as alíquotas correspondentes a UF Origem, UF Destino e Tipo Cliente
                                        var aliqIcms = p.AliqICMS
                                        .Where(f => f.UfOrigem == item[1] && f.UfDestino == item[2] && f.TipoCliente == item[0].StrParaIntNullable())
                                        .FirstOrDefault();

                                        // Se encontrar alíquota correspondente
                                        if (aliqIcms != null)
                                        {
                                            aliqIcms.AliquotaIntraestadual = item[3].StrParaFloat();
                                            aliqIcms.AliquotaInterestadual = item[4].StrParaFloat();
                                            aliqIcms.AliquotaInternaDestinatario = item[5].StrParaFloat();
                                            aliqIcms.AliquotaFCPIntraestadual = item[6].StrParaFloat();
                                            aliqIcms.AliquotaFCPInterestadual = item[7].StrParaFloat();
                                        }
                                    }
                                    // Se não encontrar alíquota, adiciona uma nova
                                    else
                                    {
                                        p.AliqICMS.Add(new Glass.Data.RelModel.ControleIcmsProdutoPorUf()
                                        {
                                            TipoCliente = item[0].StrParaIntNullable(),
                                            UfOrigem = item[1],
                                            UfDestino = item[2],
                                            AliquotaIntraestadual = item[3].StrParaFloat(),
                                            AliquotaInterestadual = item[4].StrParaFloat(),
                                            AliquotaInternaDestinatario = item[5].StrParaFloat(),
                                            AliquotaFCPIntraestadual = item[6].StrParaFloat(),
                                            AliquotaFCPInterestadual = item[7].StrParaFloat()
                                        });
                                    }
                                }
                            }

                            if (aliqIpi > -1)
                                p.AliqIPI = aliqIpi;

                            if (dadosMva != "-1")
                            {
                                string[] dados = dadosMva.Split('/');

                                if (substituirMVA)
                                {
                                    p.Mva = new List<RelModel.ControleMvaProdutoPorUf>();

                                    p.Mva.Add(new RelModel.ControleMvaProdutoPorUf()
                                    {
                                        MvaOriginal = Glass.Conversoes.StrParaFloat(dados[0]),
                                        MvaSimples = Glass.Conversoes.StrParaFloat(dados[1])
                                    });
                                }
                                else
                                {
                                    var mva = p.Mva
                                        .Where(f => string.IsNullOrEmpty(f.UfDestino) && string.IsNullOrEmpty(f.UfOrigem))
                                        .FirstOrDefault();

                                    if (mva == null)
                                    {
                                        p.Mva.Add(new RelModel.ControleMvaProdutoPorUf()
                                        {
                                            MvaOriginal = Glass.Conversoes.StrParaFloat(dados[0]),
                                            MvaSimples = Glass.Conversoes.StrParaFloat(dados[1])
                                        });
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(dados[0]))
                                            mva.MvaOriginal = Glass.Conversoes.StrParaFloat(dados[0]);

                                        if (!string.IsNullOrEmpty(dados[1]))
                                            mva.MvaSimples = Glass.Conversoes.StrParaFloat(dados[1]);
                                    }
                                }

                                for (int i = 2; i < dados.Length; i++)
                                {
                                    if (String.IsNullOrEmpty(dados[i]))
                                        continue;

                                    string[] item = dados[i].Split('|');

                                    if (alterarMVA)
                                    {
                                        var mva = p.Mva
                                        .Where(f => f.UfOrigem == item[0] && f.UfDestino == item[1])
                                        .FirstOrDefault();

                                        if (mva != null)
                                        {
                                            mva.MvaOriginal = item[2].StrParaFloat();
                                            mva.MvaSimples = item[3].StrParaFloat();
                                        }
                                    }
                                    else
                                    {
                                        p.Mva.Add(new Glass.Data.RelModel.ControleMvaProdutoPorUf()
                                        {
                                            UfOrigem = item[0],
                                            UfDestino = item[1],
                                            MvaOriginal = Glass.Conversoes.StrParaFloat(item[2]),
                                            MvaSimples = Glass.Conversoes.StrParaFloat(item[3])
                                        });
                                    }
                                }
                            }

                            if (!ncm.Equals("-1"))
                                p.Ncm = ncm;

                            if (!cst.Equals("-1"))
                                p.Cst = cst;

                            if (!cstIpi.Equals("-1"))
                                p.CstIpi = (ProdutoCstIpi)Glass.Conversoes.StrParaIntNullable(cstIpi);

                            if (!csosn.Equals("-1"))
                                p.Csosn = csosn;

                            if (!codEx.Equals("-1"))
                                p.CodigoEX = codEx;

                            if (!genProd.Equals("-1"))
                                p.IdGeneroProduto = Glass.Conversoes.StrParaIntNullable(genProd);

                            if (!tipoMerc.Equals("-1"))
                            {
                                var valor = Glass.Conversoes.StrParaIntNullable(tipoMerc);

                                if (valor != null)
                                    p.TipoMercadoria = (TipoMercadoria)valor.Value;
                                else
                                    p.TipoMercadoria = null;
                            }

                            if (!planoContabil.Equals("-1"))
                                p.IdContaContabil = Glass.Conversoes.StrParaIntNullable(planoContabil);

                            if (!cest.Equals("-1"))
                                p.IdCest = cest.StrParaIntNullable();

                            Update(transaction, p, false);

                            transaction.Commit();
                            transaction.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            transaction.Close();

                            mensagemErro += p.CodInterno + ": " + ex.Message + "|";
                        }
                    }
                }
            }
            finally
            {
                FilaOperacoes.AlteraDadosFiscaisProduto.ProximoFila();

                if (!String.IsNullOrEmpty(mensagemErro))
                    throw new Exception(mensagemErro);
            }
        }

        #endregion

        #region Verificações de produto

        /// <summary>
        /// Verifica se o produto é bisavô, ou seja, possui 3 níveis de baixa de estoque real ou fiscal.
        /// </summary>
        public bool VerificarProdutoAvo(GDASession session, int idProd)
        {
            // Verifica se o produto possui ligação de bisavô em sua configuração de baixa de estoque.
            var baixaEstoque = objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM produto p
	                    INNER JOIN subgrupo_prod sp ON (p.IdSubgrupoProd=sp.IdSubgrupoProd)

	                    INNER JOIN
		                    (SELECT primeironivelbaixa.IdProd FROM produto_baixa_estoque primeironivelbaixa
			                    INNER JOIN produto primeironivelprod ON (primeironivelbaixa.IdProdBaixa=primeironivelprod.IdProd)
			                    INNER JOIN subgrupo_prod primeironivelsubgrupo ON (primeironivelprod.IdSubgrupoProd=primeironivelsubgrupo.IdSubgrupoProd)

		                    WHERE primeironivelsubgrupo.TipoSubgrupo IN ({1},{2})) AS baixaprimeironivel ON (p.IdProd=baixaprimeironivel.IdProd)

                    WHERE p.IdProd={0} AND sp.TipoSubgrupo IN ({1},{2})",
                idProd, (int)TipoSubgrupoProd.VidroDuplo, (int)TipoSubgrupoProd.VidroLaminado));

            return baixaEstoque > 0;
        }

        /// <summary>
        /// Verifica se o produto é de um grupo específico.
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public bool ProdutoExisteGrupo(uint idProd, Glass.Data.Model.NomeGrupoProd grupo)
        {
            string sql = "select count(*) from produto where idProd=" + idProd + " and idGrupoProd=" + (int)grupo;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se o produto passado possui o valor de tabela especificado
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="valorTabel"></param>
        /// <returns></returns>
        public bool ProdutoPossuiValorTabela(GDASession sessao, uint idProd, decimal valorTabela)
        {
            const decimal TOLERANCIA = 0.01M;
            string sql = "select count(*) from produto where idProd=" + idProd + @"
                and ((valorBalcao>=?valorInf and valorBalcao<=?valorSup) Or (valorObra>=?valorInf and valorObra<=?valorSup)
                Or (valorAtacado>=?valorInf and valorAtacado<=?valorSup))";

            // Pesquisa primeiro nos valores atuais dos produtos
            bool possuiValor = objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?valorInf", valorTabela - TOLERANCIA),
                new GDAParameter("?valorSup", valorTabela + TOLERANCIA)) > 0;

            // Se não possuir o valor, pesquisa a última alteração feitas nestes campos
            if (!possuiValor)
            {
                sql = @"
                    Select count(*) From (
                        Select idLog From log_alteracao
                        Where tabela=" + (int)LogAlteracao.TabelaAlteracao.Produto + @"
                            And idRegistroAlt=" + idProd + @"
                            And campo in ('Valor Balcão', 'Valor Obra', 'Valor Atacado')
                            And (valorAnterior>=?valorInf and valorAnterior<=?valorSup)
                        Order by idLog desc Limit 3
                    ) as tbl";

                possuiValor = objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?valorInf", valorTabela - TOLERANCIA),
                    new GDAParameter("?valorSup", valorTabela + TOLERANCIA)) > 0; ;
            }

            return possuiValor;
        }

        #endregion

        #region Relatório de preços de tabela por cliente

        private string SqlPrecoTab(uint idCliente, string nomeCliente, uint idGrupoProd, string idsSubgrupoProd,
            string codInterno, string descrProduto, int tipoValor, decimal alturaInicio, decimal alturaFim, decimal larguraInicio,
            decimal larguraFim, bool selecionar, out string filtroAdicional, string ordenacao, bool ecommerce)
        {
            filtroAdicional = " and p.situacao=" + (int)Glass.Situacao.Ativo;

            string criterio = "";
            string campos = selecionar ? @"p.*, c.id_Cli as idClienteVend, c.nome as nomeClienteVend,
                c.revenda as clienteRevendaVend, g.descricao as descrGrupo, s.descricao as descrSubgrupo, '$$$' as criterio, " +
                tipoValor + " as tipoValorTabela" : "count(*)";

            string whereJoin;
            if (idCliente > 0)
            {
                whereJoin = "c.id_Cli=" + idCliente;
                criterio += "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                whereJoin = "c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }
            else
                whereJoin = "c.id_Cli=0";

            string sql = "select " + campos + @"
                from produto p
                    inner join cliente c On (" + whereJoin + @")
                    inner join grupo_prod g On (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s On (p.idSubgrupoProd=s.idSubgrupoProd)
                where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(codInterno))
            {
                filtroAdicional += " and p.codInterno=?codInterno";
                criterio += "Produto: " + GetDescrProduto(codInterno) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProduto))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProduto);
                filtroAdicional += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProduto + "    ";
            }

            if (idGrupoProd > 0)
            {
                filtroAdicional += " and p.idGrupoProd=" + idGrupoProd;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (!String.IsNullOrEmpty(idsSubgrupoProd) && !new List<string>(idsSubgrupoProd.Split(',')).Contains("0"))
            {
                filtroAdicional += " and p.idSubgrupoProd in (" + idsSubgrupoProd + ")";
                criterio += "Subgrupos: " + SubgrupoProdDAO.Instance.GetDescricao(idsSubgrupoProd) + "    ";
            }
            else if (ecommerce)
            {
                filtroAdicional += " AND (s.IdCli IS NULL OR s.IdCli = c.Id_Cli)";
            }


            if (alturaInicio > 0 || alturaFim > 0)
            {
                filtroAdicional += " and p.altura >= " + alturaInicio +
                    (alturaFim > 0 ? " AND p.altura <= " + alturaFim : "");
                criterio += "Altura: " + alturaInicio + "Até" + alturaFim;
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                filtroAdicional += " and p.largura >= " + larguraInicio +
                    (larguraFim > 0 ? " AND p.largura <= " + larguraFim : "");
                criterio += "Largura: " + larguraInicio + "Até" + larguraFim;
            }

            sql += !string.IsNullOrEmpty(ordenacao) ? " ORDER BY " + ordenacao : "";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParamsPrecoTab(string nomeCliente, string codInterno, string descrProduto)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(codInterno))
                lst.Add(new GDAParameter("?codInterno", codInterno));

            if (!String.IsNullOrEmpty(descrProduto))
                lst.Add(new GDAParameter("?descrProduto", "%" + descrProduto + "%"));

            return lst.ToArray();
        }

        public IList<Produto> GetListPrecoTab(uint idCliente, string nomeCliente, uint idGrupoProd, string idsSubgrupoProd,
            string codInterno, string descrProduto, int tipoValor, decimal alturaInicio, decimal alturaFim,
            decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto, bool ecommerce, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression :
                ordenacao == 0 ? "p.codInterno" :
                ordenacao == 1 ? "p.descricao, p.espessura, p.idcorvidro, p.idcoraluminio, p.idcorferragem" :
                ordenacao == 2 ? "g.descricao, s.descricao" :
                "s.descricao";

            string sql = SqlPrecoTab(idCliente, nomeCliente, idGrupoProd, idsSubgrupoProd, codInterno,
                descrProduto, tipoValor, alturaInicio, alturaFim, larguraInicio, larguraFim, true, out filtroAdicional, sortExpression, ecommerce)
                .Replace("?filtroAdicional?", filtroAdicional);

            var dados = objPersistence.LoadData(sql, GetParamsPrecoTab(nomeCliente, codInterno, descrProduto)).ToList();

            // Caso esteja buscando apenas produtos com desconto, busca todos que o
            // percentual de desconto acréscimo for diferente de 1 (sem desconto e sem acréscimo)
            if (produtoDesconto)
                dados = dados.Where(f => DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(null, (int)idCliente, f.IdProd)).ToList();

            return dados.Skip(startRow).Take(pageSize).ToList();
        }

        public int GetCountPrecoTab(uint idCliente, string nomeCliente, uint idGrupoProd, string idsSubgrupoProd,
            string codInterno, string descrProduto, int tipoValor, decimal alturaInicio, decimal alturaFim,
            decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto, bool ecommerce)
        {
            string filtroAdicional;
            string sql = SqlPrecoTab(idCliente, nomeCliente, idGrupoProd, idsSubgrupoProd, codInterno,
                descrProduto, tipoValor, alturaInicio, larguraFim, larguraInicio, larguraFim, true, out filtroAdicional, null, ecommerce)
                .Replace("?filtroAdicional?", filtroAdicional);

            var dados = objPersistence.LoadData(sql, GetParamsPrecoTab(nomeCliente, codInterno, descrProduto)).ToList();

            // Caso esteja buscando apenas produtos com desconto, busca todos que o
            // percentual de desconto acréscimo for diferente de 1 (sem desconto e sem acréscimo)
            if (produtoDesconto)
                dados = dados.Where(f => DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(null, (int)idCliente, f.IdProd)).ToList();

            return dados.Count();
        }

        public IList<Produto> GetForRptPrecoTab(uint idCliente, string nomeCliente, uint idGrupoProd, string idsSubgrupoProd,
            string codInterno, string descrProduto, int tipoValor, decimal alturaInicio, decimal alturaFim,
            decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto)
        {
            string filtroAdicional;

            string ordenar = (
                ordenacao == 0 ? "Trim(p.codInterno)" :
                ordenacao == 1 ? "Trim(p.descricao)" :
                ordenacao == 2 ? "g.descricao, s.descricao" :
                "s.descricao");

            string sql = SqlPrecoTab(idCliente, nomeCliente, idGrupoProd, idsSubgrupoProd, codInterno,
                descrProduto, tipoValor, alturaInicio, alturaFim, larguraInicio, larguraFim, true, out filtroAdicional, ordenar, false)
                .Replace("?filtroAdicional?", filtroAdicional);

            var dados = objPersistence.LoadData(sql, GetParamsPrecoTab(nomeCliente, codInterno, descrProduto)).ToList();

            // Caso esteja buscando apenas produtos com desconto, busca todos que o
            // percentual de desconto acréscimo for diferente de 1 (sem desconto e sem acréscimo)
            if (produtoDesconto)
                dados = dados.Where(f => DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(null, (int)idCliente, f.IdProd)).ToList();

            return dados;
        }

        #endregion

        #region Recupera todos os produtos de um grupo/subgrupo

        /// <summary>
        /// Recupera todos os produtos de um grupo/subgrupo.
        /// </summary>
        public IList<Produto> GetByGrupoSubgrupo(int idGrupoProd, int? idSubgrupoProd)
        {
            return GetByGrupoSubgrupo(idGrupoProd, idSubgrupoProd, null);
        }

        /// <summary>
        /// Recupera todos os produtos de um grupo/subgrupo.
        /// </summary>
        public IList<Produto> GetByGrupoSubgrupo(int idGrupoProd, int? idSubgrupoProd, int? situacao)
        {
            string sql = "select * from produto where 1";

            if (idGrupoProd > 0)
                sql += " and idGrupoProd=" + idGrupoProd;

            if (idSubgrupoProd > 0)
                sql += " and idSubgrupoProd=" + idSubgrupoProd;

            if (situacao.GetValueOrDefault() > 0)
                sql += " and situacao=" + situacao.Value;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Duplicar produtos

        /// <summary>
        /// Duplica uma lista de produtos.
        /// </summary>
        public void Duplicar(string idsProd, uint idNovoGrupo, uint? idNovoSubgrupo, string codInternoRemover,
            string codInternoSubstituir, string descricaoRemover, string descricaoSubstituir, string novaAltura, string novaLargura)
        {
            if (idNovoSubgrupo == 0) idNovoSubgrupo = null;

            List<Produto> prod = objPersistence.LoadData("select * from produto where idProd in (" + idsProd + ")").ToList();

            int idProd = 0;

            foreach (Produto p in prod)
            {
                //Apaga os dados de arquivo de mesa para não duplicar de forma incorreta
                p.IdArquivoMesaCorte = null;
                p.FlagsArqMesaDescricao = null;
                p.TipoArquivo = null;
                p.FlagsArqMesa = null;

                idProd = p.IdProd;

                // Carrega os beneficiamentos do produto
                p.SalvarBeneficiamentos = true;
                p.Beneficiamentos = p.Beneficiamentos;

                p.IdProd = 0;
                p.IdGrupoProd = (int)idNovoGrupo;
                p.IdSubgrupoProd = (int?)idNovoSubgrupo;
                p.Altura = string.IsNullOrWhiteSpace(novaAltura) ? p.Altura : novaAltura.StrParaInt();
                p.Largura = string.IsNullOrWhiteSpace(novaLargura) ? p.Largura : novaLargura.StrParaInt();

                if (!string.IsNullOrEmpty(codInternoRemover))
                    p.CodInterno = p.CodInterno.ToUpper().Replace(codInternoRemover.ToUpper(), codInternoSubstituir != null ? codInternoSubstituir.ToUpper() : string.Empty).Trim();
                else
                    p.CodInterno += codInternoSubstituir.ToUpper().Trim();

                if (!string.IsNullOrEmpty(descricaoRemover))
                    p.Descricao = p.Descricao.ToUpper().Replace(descricaoRemover.ToUpper(), descricaoSubstituir != null ? descricaoSubstituir.ToUpper() : string.Empty).Trim();
                else
                    p.Descricao = (p.Descricao + " " + (descricaoSubstituir != null ? descricaoSubstituir.ToUpper().Trim() : string.Empty)).Trim();

                var idProdNovo = Insert(p);

                try
                {
                    //Faz a copia do MVA
                    string sqlMva = @"
                    INSERT INTO mva_produto_uf (idProd, ufOrigem, ufDestino, mvaOriginal, mvaSimples)
                    SELECT {0} as idProd, ufOrigem, ufDestino, mvaOriginal, mvaSimples
                    FROM mva_produto_uf
                    WHERE idProd = {1}";
                    objPersistence.ExecuteCommand(string.Format(sqlMva, idProdNovo, idProd));

                    //Faz a copia do ICMS
                    string sqlIcms = @"
                    INSERT INTO icms_produto_uf (idProd, ufOrigem, ufDestino, aliquotaIntra, aliquotaInter, idTipoCliente)
                    SELECT {0} as idProd, ufOrigem, ufDestino, aliquotaIntra, aliquotaInter, idTipoCliente
                    FROM icms_produto_uf
                    WHERE idProd = {1}";
                    objPersistence.ExecuteCommand(string.Format(sqlIcms, idProdNovo, idProd));

                    //Faz a copia da materia-prima
                    string sqlMP = @"
                    INSERT INTO {0} (idProd, idProdBaixa, qtde)
                    SELECT {1} as idProd, idProdBaixa, qtde
                    FROM {0}
                    WHERE idProd = {2}";
                    objPersistence.ExecuteCommand(string.Format(sqlMP, "produto_baixa_estoque", idProdNovo, idProd));
                    objPersistence.ExecuteCommand(string.Format(sqlMP, "produto_baixa_estoque_fiscal", idProdNovo, idProd));
                }
                catch (Exception ex)
                {
                    DeleteByPrimaryKey(idProdNovo);
                    throw ex;
                }
            }
        }

        #endregion

        #region Sugestão de compra/produção

        private string SqlSugestaoCompra(uint idLoja, uint idGrupoProd, uint idSubgrupoProd,
            string codInterno, string descricao, string idsProd, bool forProducao)
        {
            TipoBusca tipoBusca = forProducao == true ? TipoBusca.SugestaoProducao : TipoBusca.SugestaoCompra;

            bool temFiltro;
            string sql = String.IsNullOrEmpty(idsProd) ?
                Sql(0, 0, codInterno, descricao, 0, (int)idGrupoProd, (int)idSubgrupoProd, 0, null, null, null, null, false, idLoja > 0, null, null, 0, 0, 0, 0, true, tipoBusca, true, out temFiltro) :
                Sql(0, 0, null, null, 0, 0, 0, 0, null, null, null, null, false, idLoja > 0, null, null, 0, 0, 0, 0, true, tipoBusca, true, out temFiltro);

            string isTipoCalcM2 = "coalesce(sg.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                (int)TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ")";

            if (PCPConfig.ControlarProducao && forProducao)
            {
                sql += " and p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" and sg.produtosEstoque=true
                    and (if(" + isTipoCalcM2 + @", pl.m2 + coalesce(pped.totMProduzindo,0), pl.qtdeEstoque +
                    coalesce(pped.qtdeProduzindo,0))) < (Coalesce(pl.estoqueMinimo, 0) + Coalesce(pl.reserva, 0) + Coalesce(pl.liberacao, 0))";
            }
            else
            {
                sql += string.Format(" and (coalesce(sg.produtosEstoque, false)=false Or sg.tipoSubgrupo In({0},{1})) and (if(" + isTipoCalcM2 + @", coalesce(pl.m2,0) +
                    coalesce(pc.totMComprando,0), coalesce(pl.qtdeEstoque,0) + coalesce(pc.qtdeComprando,0))) <
                    (coalesce(pl.estoqueMinimo,0) + coalesce(pl.reserva,0) + coalesce(pl.liberacao,0))",
                    (int)TipoSubgrupoProd.ChapasVidro,
                    (int)TipoSubgrupoProd.ChapasVidroLaminado);
            }

            if (idLoja > 0)
                sql += " and pl.idLoja=" + idLoja;

            if (!String.IsNullOrEmpty(idsProd))
                sql += " and p.idProd in (" + idsProd.TrimEnd(',', ' ') + ")";

            return sql;
        }

        private string SqlSugestaoCompraPorVendas(int idLoja, int idGrupoProd, int idSubgrupoProd,
            string codInterno, int mesesVendas, int mesesEstoque, string idsProd)
        {

            var isTipoCalcM2 = "coalesce(sg.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
               (int)TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ")";

            var sql = @"
                SELECT tmp.*, (MediaVendaMensal * " + mesesEstoque + @" - tmp.QtdeEstoque) as SugestaoCompraMensal
                FROM (
                        SELECT p.*, pp.MediaVendaMensal,
                            g.Descricao AS DescrGrupo, sg.Descricao AS DescrSubgrupo, pl.Reserva, pl.Liberacao, pl.M2 AS M2Estoque,
                            SUM(coalesce(pl.qtdEstoque, 0) + IF(" + isTipoCalcM2 + @", coalesce(pc.totMComprando, 0), coalesce(pc.qtdeComprando, 0))) as QtdeEstoque
                        FROM produto p
	                        INNER JOIN produto_loja pl ON (p.IdProd = pl.IdProd)
	                        INNER JOIN grupo_prod g ON (p.IdGrupoProd = g.IdGrupoProd)
	                        LEFT JOIN subgrupo_prod sg ON (p.IdSubgrupoProd = sg.IdSubgrupoProd)
                            LEFT JOIN
                            (
		                        SELECT pp.IdProd, (SUM(pp.Qtde) / " + mesesVendas + @") as MediaVendaMensal
		                        FROM produtos_pedido pp
			                        INNER JOIN pedido p ON (pp.IdPedido = p.IdPedido)
			                        INNER JOIN produtos_liberar_pedido plp ON (pp.IdProdPed = plp.IdProdPed)
			                        INNER JOIN liberarpedido lp ON (plp.IdLiberarPedido = lp.IdLiberarPedido)
		                        WHERE COALESCE(pp.InvisivelFluxo, 0) = 0
			                        AND p.Situacao = " + (int)Pedido.SituacaoPedido.Confirmado + @"
			                        AND lp.Situacao = " + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + @"
			                        AND lp.DataLiberacao >= ?dtIni
		                        GROUP BY pp.IdProd
                            ) as pp ON (p.IdProd = pp.IdProd)
                            LEFT JOIN
                            (
                                SELECT pc.idProd, c.idLoja, SUM(pc.totM) AS totMComprando, SUM(pc.qtde) AS qtdeComprando
                                FROM produtos_compra pc
                                    INNER JOIN compra c ON (pc.idCompra = c.idCompra)
                                WHERE c.situacao IN (" + (int)Compra.SituacaoEnum.Ativa + @" , " + (int)Compra.SituacaoEnum.Finalizada + @") AND COALESCE(c.estoqueBaixado, 0) = 0
                                GROUP BY pc.idProd , c.idLoja
                            ) AS pc ON (p.idProd = pc.idProd AND pl.idLoja = pc.idLoja)
                        WHERE (COALESCE(sg.produtosEstoque, 0) = 0 OR sg.tipoSubgrupo In(" + (int)TipoSubgrupoProd.ChapasVidro + @"," + (int)TipoSubgrupoProd.ChapasVidroLaminado + @"))
                            {0}
                        GROUP BY p.IdProd
                    ) as tmp
                WHERE (MediaVendaMensal * " + mesesEstoque + @") > QtdeEstoque";

            var where = "";

            if (idLoja > 0)
                where += " AND pl.IdLoja =" + idLoja;

            if (idGrupoProd > 0)
                where += " AND  g.IdGrupoProd =" + idGrupoProd;

            if (idSubgrupoProd > 0)
                where += " AND  sg.IdSubgrupoProd =" + idSubgrupoProd;

            if (!string.IsNullOrEmpty(codInterno))
                where += " AND  p.CodInterno like ?codInterno";

            if (!string.IsNullOrEmpty(idsProd))
                where += " AND p.idProd in (" + idsProd.TrimEnd(',', ' ') + ")";

            sql = string.Format(sql, where);


            return sql;
        }

        /// <summary>
        /// Busca produtos para sugestão de compra.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idGrupoProd"></param>
        /// <param name="idSubgrupoProd"></param>
        /// <param name="codInterno"></param>
        /// <param name="descricao"></param>
        /// <param name="forProducao"></param>
        /// <param name="porVenda"></param>
        /// <returns></returns>
        public IList<Produto> GetForSugestaoCompra(uint idLoja, uint idGrupoProd, uint idSubgrupoProd,
            string codInterno, string descricao, bool forProducao, int mesesVendas, int mesesEstoque)
        {
            if (mesesVendas > 0)
            {
                var dataIni = DateTime.Now.AddMonths(-mesesVendas);

                return objPersistence.LoadData(SqlSugestaoCompraPorVendas((int)idLoja, (int)idGrupoProd, (int)idSubgrupoProd, codInterno, mesesVendas, mesesEstoque, null),
                    new GDAParameter("?dtIni", dataIni), new GDAParameter("?codInterno", codInterno)).ToList();
            }

            return objPersistence.LoadData(SqlSugestaoCompra(idLoja, idGrupoProd, idSubgrupoProd, codInterno, descricao, null, forProducao),
                GetParam(codInterno, descricao, null, null, null, null, null, null)).ToList();
        }

        /// <summary>
        /// Busca produtos para sugestão de compra.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        public IList<Produto> GetForSugestaoCompra(uint idLoja, string idsProd, bool forProducao, int mesesVendas, int mesesEstoque)
        {
            if (mesesVendas > 0)
            {
                var dataIni = DateTime.Now.AddMonths(-mesesVendas);

                return objPersistence.LoadData(SqlSugestaoCompraPorVendas((int)idLoja, 0, 0, null, mesesVendas, mesesEstoque, idsProd),
                    new GDAParameter("?dtIni", dataIni), new GDAParameter("?codInterno", null)).ToList();
            }

            return objPersistence.LoadData(SqlSugestaoCompra(idLoja, 0, 0, null, null, idsProd, forProducao)).ToList();
        }

        /// <summary>
        /// Gera uma compra com os produtos passados e com as quantidades sugeridas (est. mínimo x 2).
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        public uint GerarCompraSugerida(uint idLoja, string idsProd, int mesesVendas, int mesesEstoque)
        {
            uint idCompra = 0;
            List<uint> idsProdCompra = new List<uint>();

            try
            {
                // Cadastra a compra
                Compra compra = new Compra();
                compra.IdLoja = idLoja;
                compra.TipoCompra = (int)Compra.TipoCompraEnum.AVista;
                compra.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro;

                idCompra = CompraDAO.Instance.Insert(compra);
                if (idCompra == 0)
                    throw new Exception("Compra não gerada corretamente.");

                // Cadastra os produtos
                foreach (Produto prod in ProdutoDAO.Instance.GetForSugestaoCompra(idLoja, idsProd.TrimEnd(',', ' '), false, mesesVendas, mesesEstoque))
                {
                    ProdutosCompra p = new ProdutosCompra();
                    p.IdCompra = idCompra;
                    p.IdProd = (uint)prod.IdProd;
                    p.Qtde = !prod.UsarEstoqueM2 ? mesesVendas > 0 ? (float)prod.SugestaoCompraMensal : prod.SugestaoCompra : 1;
                    p.TotM = prod.UsarEstoqueM2 ? mesesVendas > 0 ? (float)prod.SugestaoCompraMensal : prod.SugestaoCompra : 0;
                    p.Valor = prod.CustoCompra;
                    p.Altura = prod.UsarEstoqueM2 || prod.TipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro ? 1000 :
                        prod.TipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd || prod.TipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal ? 0 : 1;
                    p.Largura = p.Altura > 1 ? (int)p.Altura : 0;
                    p.Redondo = prod.Redondo;
                    p.Beneficiamentos = prod.Beneficiamentos;
                    p.Espessura = prod.Espessura;

                    idsProdCompra.Add(ProdutosCompraDAO.Instance.Insert(p));
                }

                return idCompra;
            }
            catch (Exception ex)
            {
                if (idCompra > 0)
                    CompraDAO.Instance.DeleteByPrimaryKey(idCompra);

                foreach (uint id in idsProdCompra)
                    ProdutosCompraDAO.Instance.DeleteByPrimaryKey(id);

                throw ex;
            }
        }

        /// <summary>
        /// Gera um pedido com os produtos passados e com as quantidades sugeridas (est. mínimo x 2).
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        public uint GerarPedidoSugerido(uint idLoja, string idsProd)
        {
            uint idPedido = 0;
            List<uint> idsProdPedido = new List<uint>();

            try
            {
                // Cadastra o pedido
                Pedido pedido = new Pedido();
                pedido.TipoPedido = (int)Pedido.TipoPedidoEnum.Producao;
                pedido.IdLoja = idLoja;
                pedido.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;
                pedido.TipoEntrega = (int)Pedido.TipoEntregaPedido.Balcao;
                pedido.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro;
                pedido.IdCli = ClienteDAO.Instance.GetClienteProducao();
                pedido.DataEntrega = DateTime.Now.AddDays(7);
                pedido.IdFunc = UserInfo.GetUserInfo.CodUser;

                idPedido = PedidoDAO.Instance.Insert(pedido);
                if (idPedido == 0)
                    throw new Exception("Pedido não gerado corretamente.");

                uint? idAmbientePedido = null;
                if (PedidoConfig.DadosPedido.AmbientePedido)
                {
                    AmbientePedido a = new AmbientePedido();
                    a.IdPedido = idPedido;
                    a.Ambiente = "Produção";

                    idAmbientePedido = AmbientePedidoDAO.Instance.Insert(a);
                }

                // Cadastra os produtos
                if (!String.IsNullOrEmpty(idsProd))
                {
                    foreach (Produto prod in ProdutoDAO.Instance.GetForSugestaoCompra(idLoja, idsProd.TrimEnd(',', ' '), true, 0, 0))
                    {
                        ProdutosPedido p = new ProdutosPedido();
                        p.IdPedido = idPedido;
                        p.IdAmbientePedido = idAmbientePedido;
                        p.IdProd = (uint)prod.IdProd;
                        p.Qtde = !prod.UsarEstoqueM2 ? prod.SugestaoProducao : 1;
                        p.TotM = prod.UsarEstoqueM2 ? prod.SugestaoProducao : 0;
                        p.ValorVendido = prod.ValorBalcao;
                        p.Altura = prod.Altura != null ? prod.Altura.Value : 0;
                        p.Largura = prod.Largura != null ? prod.Largura.Value : 0;
                        p.Redondo = prod.Redondo;
                        p.Beneficiamentos = prod.Beneficiamentos;
                        p.Espessura = prod.Espessura;
                        p.IdAplicacao = (uint?)prod.IdAplicacao;
                        p.IdProcesso = (uint?)prod.IdProcesso;

                        idsProdPedido.Add(ProdutosPedidoDAO.Instance.Insert(p));
                    }
                }

                return idPedido;
            }
            catch (Exception ex)
            {
                if (idPedido > 0)
                {
                    // Ao invés de excluir o pedido, marca-o como cancelado
                    PedidoDAO.Instance.AlteraSituacao(null, idPedido, Pedido.SituacaoPedido.Cancelado);
                    PedidoDAO.Instance.AtualizaObs(idPedido, Glass.MensagemAlerta.FormatErrorMsg("Pedido cancelado por falha ao gerar pedido sugerido.", ex));
                }

                throw ex;
            }
        }

        #endregion

        #region Exibir mensagem de estoque

        /// <summary>
        /// Verifica se a mensagem de estoque deve ser exibida.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool ExibirMensagemEstoque(int idProd)
        {
            var idGrupoProd = ObtemIdGrupoProd(idProd);
            var idSubgrupoProd = ObtemIdSubgrupoProd(idProd);

            return ExibirMensagemEstoque(null, idGrupoProd, idSubgrupoProd);
        }

        /// <summary>
        /// Verifica se a mensagem de estoque deve ser exibida.
        /// </summary>
        /// <returns></returns>
        public bool ExibirMensagemEstoque(GDASession sessao, int idGrupoProd, int? idSubgrupoProd)
        {
            return idSubgrupoProd > 0
                ? SubgrupoProdDAO.Instance.ObtemValorCampo<bool>(sessao, "exibirMensagemEstoque", "idSubgrupoProd=" + idSubgrupoProd.Value)
                : GrupoProdDAO.Instance.ObtemValorCampo<bool>(sessao, "exibirMensagemEstoque", "idGrupoProd=" + idGrupoProd);
        }

        #endregion

        #region Alterar grupo/subgrupo

        /// <summary>
        /// Altera o grupo/subgrupo dos produtos indicados.
        /// </summary>
        /// <param name="idsProd"></param>
        /// <param name="idGrupoProd"></param>
        /// <param name="idSubgrupoProd"></param>
        public void AlterarGrupoSubgrupo(string idsProd, uint idGrupoProd, uint? idSubgrupoProd)
        {
            if (String.IsNullOrEmpty(idsProd))
                return;

            if (idSubgrupoProd == 0)
                idSubgrupoProd = null;

            var prod = objPersistence.LoadData("select * from produto where idProd in (" + idsProd.TrimEnd(',') + ")").ToList();

            foreach (Produto p in prod)
            {
                // Caso o grupo/subgrupo tenha sido alterado, atualiza os valores na tabela de desconto/acréscimo
                if (idGrupoProd != p.IdGrupoProd)
                    objPersistence.ExecuteCommand("Update desconto_acrescimo_cliente Set idGrupoProd=" + idGrupoProd + " Where idProd=" + p.IdProd);

                if (idSubgrupoProd.GetValueOrDefault(0) != p.IdSubgrupoProd.GetValueOrDefault(0))
                    objPersistence.ExecuteCommand("Update desconto_acrescimo_cliente Set idSubGrupoProd=" +
                        (idSubgrupoProd == null ? "null" : idSubgrupoProd.Value.ToString()) + " Where idProd=" + p.IdProd);

                p.IdGrupoProd = (int)idGrupoProd;
                p.IdSubgrupoProd = (int?)idSubgrupoProd;

                // Salva alterações no log e atualiza o produto
                LogAlteracaoDAO.Instance.LogProduto(p, LogAlteracaoDAO.SequenciaObjeto.Novo);
                base.Update(p);
            }
        }

        #endregion

        #region Busca o CST IPI do produto

        /// <summary>
        /// Busca o CST IPI do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int? GetCstIpi(uint idProd)
        {
            string sql = "select cstIpi from produto where idProd=" + idProd;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (int?)Glass.Conversoes.StrParaInt(retorno.ToString()) : null;
        }

        #endregion

        #region Busca o plano de conta contábil do produto

        /// <summary>
        /// Busca o plano de conta contábil do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public uint? GetIdContaContabil(uint idProd)
        {
            string sql = "select idContaContabil from produto where idProd=" + idProd;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        #endregion

        #region Obtém ids dos produtos

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public int ObtemIdProd(string codInterno)
        {
            return ObtemIdProd(null, codInterno);
        }

        public int ObtemIdProd(GDASession sessao, string codInterno)
        {
            return ObtemValorCampo<int>(sessao, "idProd", "codInterno=?codInterno", new GDAParameter("?codInterno", codInterno));
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint ObtemIdProdByEtiqueta(string codEtiqueta)
        {
            return ObtemIdProdByEtiqueta(null, codEtiqueta);
        }

        public uint ObtemIdProdByEtiqueta(GDASession sessao, string codEtiqueta)
        {
            string sql = @"
                SELECT idProd
        	    FROM produto_pedido_producao ppp
                INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                WHERE ppp.numEtiqueta = ?codEtiqueta";

            return ExecuteScalar<uint>(sessao, sql, new GDAParameter("?codEtiqueta", codEtiqueta));
        }

        public Tuple<int, decimal, decimal> ObtemAlturaLarguraByEtiqueta(GDASession sessao, string codEtiqueta)
        {
            string sql = @"
                SELECT CONCAT(p.idProd, ',', pp.Altura, ',', pp.Largura)
        	    FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                    INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                WHERE ppp.numEtiqueta = ?codEtiqueta";

            var retorno = ExecuteScalar<string>(sessao, sql, new GDAParameter("?codEtiqueta", codEtiqueta));

            if (retorno == null)
                return null;

            var dados = retorno.Split(',');

            return new Tuple<int, decimal, decimal>(dados[0].StrParaInt(), dados[1].StrParaDecimal(), dados[2].StrParaDecimal());
        }

        /// <summary>
        /// Recupera o código de cada produto associado às informações passadas por parâmetro.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <param name="descricao"></param>
        public string ObtemIds(string codInterno, string descricao)
        {
            return ObtemIds((GDASession)null, codInterno, descricao);
        }

        /// <summary>
        /// Recupera o código de cada produto associado às informações passadas por parâmetro.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="codInterno"></param>
        /// <param name="descricao"></param>
        public string ObtemIds(GDASession session, string codInterno, string descricao)
        {
            return ObtemIds(session, codInterno, descricao, null);
        }

        /// <summary>
        /// Recupera o código de cada produto associado às informações passadas por parâmetro.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <param name="descricao"></param>
        /// <param name="codOtimizacao"></param>
        public string ObtemIds(string codInterno, string descricao, string codOtimizacao)
        {
            return ObtemIds(null, codInterno, descricao, codOtimizacao);
        }

        /// <summary>
        /// Recupera o código de cada produto associado às informações passadas por parâmetro.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="codInterno"></param>
        /// <param name="descricao"></param>
        /// <param name="codOtimizacao"></param>
        public string ObtemIds(GDASession session, string codInterno, string descricao, string codOtimizacao)
        {
            // Caso todos os campos estejam vazios, retorna nulo.
            if (String.IsNullOrEmpty(codInterno) && String.IsNullOrEmpty(descricao) && String.IsNullOrEmpty(codOtimizacao))
                return null;
            // Lista criada para setar os parâmetros incluídos no sql.
            List<GDAParameter> lstParam = new List<GDAParameter>();

            var sql = "Select p.idProd From produto p Where 1";

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " And p.codInterno=?codInterno";
                lstParam.Add(new GDAParameter("?codInterno", codInterno));
            }
            else if (!String.IsNullOrEmpty(descricao))
            {
                sql += " And p.descricao Like ?descr";
                lstParam.Add(new GDAParameter("?descr", "%" + descricao + "%"));
            }

            if (!String.IsNullOrEmpty(codOtimizacao))
            {
                sql += " And p.codOtimizacao=?codOtimizacao";
                lstParam.Add(new GDAParameter("?codOtimizacao", codOtimizacao));
            }

            // Recupera todos os códigos dos produtos retornados através do sql executado.
            var ids = ExecuteMultipleScalar<uint>(session, sql, lstParam.ToArray());

            if (ids.Count == 0)
                return "0";
            // Retorna os códigos dos produtos recuperados, separados por vírgula.
            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate (uint x)
                {
                    return x.ToString();
                }
            )));
        }

        #endregion

        #region Atualiza Valor Fiscal

        /// <summary>
        /// Atualiza o valor fiscal do produto
        /// </summary>
        /// <param name="idProduto">Chave do produto</param>
        /// <param name="valorFiscal">Valor fiscal alterado</param>
        /// <returns>int</returns>
        public int AtualizaValorFiscal(uint idProduto, decimal valorFiscal)
        {
            string sql = "Update produto set ValorFiscal = '" + valorFiscal.ToString().Replace(',', '.') + "' where idProd= " + idProduto;

            return objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Verifica se um produto calcula beneficiamento

        /// <summary>
        /// Verifica se um produto calcula beneficiamento.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool CalculaBeneficiamento(int idProd)
        {
            return CalculaBeneficiamento(null, idProd);
        }

        /// <summary>
        /// Verifica se um produto calcula beneficiamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool CalculaBeneficiamento(GDASession sessao, int idProd)
        {
            if (!Geral.UsarBeneficiamentosTodosOsGrupos)
            {
                var idGrupoProd = ObtemIdGrupoProd(sessao, idProd);
                return new List<int> { (int)Glass.Data.Model.NomeGrupoProd.Vidro }.Contains(idGrupoProd);
            }
            else
                return true;
        }

        #endregion

        #region Verifica produto na nota fiscal

        public bool ProdutoEstaNaNotaFiscal(string codInterno, uint idNf)
        {
            string idsProd = ObtemIds(codInterno, null);

            string sql = @"select count(*) from produtos_nf
                where idNf=" + idNf + " and idProd in (" + idsProd + ")";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(Produto objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, Produto objInsert)
        {
            // Verifica se o codInterno que se pretende cadastrar já está sendo usado por outro produto
            string sql = "Select Count(*) From produto Where codInterno=?codInterno";
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, sql, new GDAParameter[] { new GDAParameter("?codInterno", objInsert.CodInterno) }).ToString()) > 0)
                throw new Exception("Já existe um produto cadastrado com o código informado.");

            /* Chamado 22919. */
            if (objInsert.Descricao.Contains("'") ||
                objInsert.CodInterno.Contains("'") ||
                objInsert.CodInterno.Contains('"'))
                throw new Exception("Retire os caracteres ' e " + '"' + " do código e descrição do produto.");

            if (objInsert.Ncm != null)
                objInsert.Ncm = objInsert.Ncm.Replace("\t", "");

            objInsert.Usucad = objInsert.Usucad > 0 ? objInsert.Usucad : UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            if (objInsert.CodOtimizacao != null)
                objInsert.CodOtimizacao = objInsert.CodOtimizacao.ToUpper();

            // Não permite que o nome do produto possua ' ou " ou \t ou \n
            objInsert.Descricao = objInsert.Descricao.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "");

            uint retorno = base.Insert(session, objInsert);
            objInsert.IdProd = (int)retorno;

            foreach (Loja loja in LojaDAO.Instance.GetAll(session))
            {
                ProdutoBenefDAO.Instance.DeleteByProd(session, retorno);
                ProdutoLojaDAO.Instance.NewProd(session, objInsert.IdProd, loja.IdLoja);
            }

            if (objInsert.SalvarBeneficiamentos)
                foreach (ProdutoBenef pb in objInsert.Beneficiamentos.ToProdutos(retorno))
                    ProdutoBenefDAO.Instance.Insert(session, pb);

            objInsert.RefreshBeneficiamentos();

            // Reinsere produtos para baixa
            objInsert.DadosBaixaEstoque = objInsert.DadosBaixaEstoque;
            ProdutoBaixaEstoqueDAO.Instance.DeleteByProd(session, (uint)objInsert.IdProd);
            foreach (uint idProdBaixa in objInsert.DadosBaixaEstoque.Keys)
            {
                ProdutoBaixaEstoque pbe = new ProdutoBaixaEstoque()
                {
                    IdProd = (int)retorno,
                    IdProdBaixa = (int)idProdBaixa,
                    Qtde = objInsert.DadosBaixaEstoque[idProdBaixa]
                };

                ProdutoBaixaEstoqueDAO.Instance.Insert(session, pbe);
            }

            // Reinsere produtos para baixa fiscal.
            objInsert.DadosBaixaEstoqueFiscal = objInsert.DadosBaixaEstoqueFiscal;
            ProdutoBaixaEstoqueFiscalDAO.Instance.DeleteByProd(session, (uint)objInsert.IdProd);
            foreach (var idProdBaixaFiscal in objInsert.DadosBaixaEstoqueFiscal.Keys)
            {
                var pbef = new ProdutoBaixaEstoqueFiscal()
                {
                    IdProd = (int)retorno,
                    IdProdBaixa = (int)idProdBaixaFiscal,
                    Qtde = objInsert.DadosBaixaEstoqueFiscal[idProdBaixaFiscal]
                };

                ProdutoBaixaEstoqueFiscalDAO.Instance.Insert(session, pbef);
            }

            // Salva o ICMS e MVA por UF
            IcmsProdutoUfDAO.Instance.SalvarDadosControle(session, retorno, objInsert.AliqICMS);
            MvaProdutoUfDAO.Instance.SalvarDadosControle(session, retorno, objInsert.Mva);

            return retorno;
        }

        public int UpdateBase(Produto objUpdate)
        {
            return UpdateBase((GDASession)null, objUpdate);
        }

        public int UpdateBase(GDASession session, Produto objUpdate)
        {
            if (objUpdate.Ncm != null)
                objUpdate.Ncm = objUpdate.Ncm.Replace("\t", "");

            // Inclui as informações de alteração
            objUpdate.DataAlt = DateTime.Now;
            objUpdate.UsuAlt = (int?)UserInfo.GetUserInfo.CodUser;

            if (objUpdate.CodOtimizacao != null)
                objUpdate.CodOtimizacao = objUpdate.CodOtimizacao.ToUpper();

            // Não permite que o nome do produto possua ' ou " ou \t ou \n
            objUpdate.Descricao = objUpdate.Descricao.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "");

            LogAlteracaoDAO.Instance.LogProduto(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
            return base.Update(session, objUpdate);
        }

        /// <summary>
        /// Se algum preço do produto tiver sofrido alguma alteração, altera os valores de
        /// todos os kits e produtos kits
        /// </summary>
        /// <param name="objUpdate"></param>
        public override int Update(Produto objUpdate)
        {
            return Update((GDASession)null, objUpdate);
        }

        /// <summary>
        /// Se algum preço do produto tiver sofrido alguma alteração, altera os valores de
        /// todos os kits e produtos kits
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objUpdate"></param>
        public override int Update(GDASession session, Produto objUpdate)
        {
            return Update(session, objUpdate, true);
        }

        /// <summary>
        /// Se algum preço do produto tiver sofrido alguma alteração, altera os valores de
        /// todos os kits e produtos kits
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <param name="atualizarBenef">Define se o beneficiamento do produto será atualizado ou não.</param>
        public int Update(Produto objUpdate, bool atualizarBenef)
        {
            return Update((GDASession)null, objUpdate, atualizarBenef);
        }

        /// <summary>
        /// Se algum preço do produto tiver sofrido alguma alteração, altera os valores de
        /// todos os kits e produtos kits
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objUpdate"></param>
        /// <param name="atualizarBenef">Define se o beneficiamento do produto será atualizado ou não.</param>
        public int Update(GDASession session, Produto objUpdate, bool atualizarBenef)
        {
            if (objUpdate.IdProd == objUpdate.IdProdBase)
                throw new Exception("O produto base não pode ser o próprio produto.");

            // Verifica se o codInterno que se pretende cadastrar já está sendo usado por outro produto que não seja este
            string sql = "Select Count(*) From produto Where codInterno=?codInterno And idProd<>" + objUpdate.IdProd;
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, sql,
                new GDAParameter[] { new GDAParameter("?codInterno", objUpdate.CodInterno) }).ToString()) > 0)
                throw new Exception("Existe outro produto cadastrado com o código informado.");

            /* Chamado 22919. */
            if (objUpdate.Descricao.Contains("'") ||
                objUpdate.CodInterno.Contains("'") ||
                objUpdate.CodInterno.Contains('"'))
                throw new Exception("Retire os caracteres ' e " + '"' + " do código e descrição do produto.");

            // Chamado 13679.
            // O usuário estava associando matéria prima em um produto associado ao grupo de retalho de produção,
            // como isso não pode ser feito a matéria prima não era salva e para o usuário aparentemente era um erro.
            if (objUpdate.DadosBaixaEstoque != null && objUpdate.DadosBaixaEstoque.Count > 0 &&
                objUpdate.IdSubgrupoProd == (uint)Utils.SubgrupoProduto.RetalhosProducao)
                throw new Exception("Não é possível associar matéria prima em produtos associados ao subgrupo Retalhos de Produção.");

            // Chamado 43961 - produto recuperado para verificar alteração de preço
            var produtoAntigo = GetElement((uint)objUpdate.IdProd);
            var idGrupoProdAntigo = produtoAntigo.IdGrupoProd;
            var idSubgrupoProdAntigo = produtoAntigo.IdSubgrupoProd;

            // Se o subgrupo escolhido não pertencer ao grupo escolhido, apaga idSubgrupoProd
            if (objUpdate.IdSubgrupoProd > 0 &&
                objUpdate.IdGrupoProd != SubgrupoProdDAO.Instance.ObtemValorCampo<int>(session,
                "idGrupoProd", "idSubgrupoProd=" + objUpdate.IdSubgrupoProd.Value))
                objUpdate.IdSubgrupoProd = null;

            int retorno = UpdateBase(session, objUpdate);

            // Chamado 43961
            MensagemDAO.Instance.EnviarMsgPrecoProdutoAlterado(produtoAntigo, objUpdate);
            Email.EnviaEmailAdministradorPrecoProdutoAlterado(session, produtoAntigo, objUpdate);

            // Reinsere beneficiamentos
            if (atualizarBenef)
            {
                objUpdate.Beneficiamentos = objUpdate.Beneficiamentos;
                ProdutoBenefDAO.Instance.DeleteByProd(session, (uint)objUpdate.IdProd);
                if (objUpdate.SalvarBeneficiamentos)
                    foreach (ProdutoBenef pb in objUpdate.Beneficiamentos.ToProdutos((uint)objUpdate.IdProd))
                        ProdutoBenefDAO.Instance.Insert(session, pb);

                objUpdate.RefreshBeneficiamentos();
            }

            // Caso o grupo/subgrupo tenha sido alterado, atualiza os valores na tabela de desconto/acréscimo
            if (objUpdate.IdGrupoProd != idGrupoProdAntigo)
                objPersistence.ExecuteCommand(session, "Update desconto_acrescimo_cliente Set idGrupoProd=" + objUpdate.IdGrupoProd + " Where idProd=" + objUpdate.IdProd);

            if (objUpdate.IdSubgrupoProd.GetValueOrDefault(0) != idSubgrupoProdAntigo.GetValueOrDefault(0))
                objPersistence.ExecuteCommand(session, "Update desconto_acrescimo_cliente Set idSubGrupoProd=" +
                    (objUpdate.IdSubgrupoProd == null ? "null" : objUpdate.IdSubgrupoProd.ToString()) + " Where idProd=" + objUpdate.IdProd);

            // Reinsere produtos para baixa
            objUpdate.DadosBaixaEstoque = objUpdate.DadosBaixaEstoque;
            ProdutoBaixaEstoqueDAO.Instance.DeleteByProd(session, (uint)objUpdate.IdProd);
            foreach (uint idProdBaixa in objUpdate.DadosBaixaEstoque.Keys)
            {
                ProdutoBaixaEstoque pbe = new ProdutoBaixaEstoque()
                {
                    IdProd = objUpdate.IdProd,
                    IdProdBaixa = (int)idProdBaixa,
                    Qtde = objUpdate.DadosBaixaEstoque[idProdBaixa]
                };

                ProdutoBaixaEstoqueDAO.Instance.Insert(session, pbe);
            }

            // Reinsere produtos para baixa fiscal.
            objUpdate.DadosBaixaEstoqueFiscal = objUpdate.DadosBaixaEstoqueFiscal;
            ProdutoBaixaEstoqueFiscalDAO.Instance.DeleteByProd(session, (uint)objUpdate.IdProd);
            foreach (var idProdBaixaFiscal in objUpdate.DadosBaixaEstoqueFiscal.Keys)
            {
                var pbef = new ProdutoBaixaEstoqueFiscal()
                {
                    IdProd = objUpdate.IdProd,
                    IdProdBaixa = (int)idProdBaixaFiscal,
                    Qtde = objUpdate.DadosBaixaEstoqueFiscal[idProdBaixaFiscal]
                };

                ProdutoBaixaEstoqueFiscalDAO.Instance.Insert(session, pbef);
            }

            // Salva o ICMS e MVA por UF
            IcmsProdutoUfDAO.Instance.SalvarDadosControle(session, (uint)objUpdate.IdProd, objUpdate.AliqICMS);
            MvaProdutoUfDAO.Instance.SalvarDadosControle(session, (uint)objUpdate.IdProd, objUpdate.Mva);

            return retorno;
        }

        public override int Delete(Produto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProd);
        }

        public override int DeleteByPrimaryKey(uint key)
        {
            /*
            // Verifica se o produto está sendo usado em algum pedido
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_pedido Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem pedidos utilizando-o.");

            // Verifica se o produto está sendo usado em algum orçamento
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_orcamento Where idProduto=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem orçamentos utilizando-o.");

            // Verifica se o produto está sendo usado em alguma compra
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_compra Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem compras utilizando-o.");

            // Verifica se o produto está sendo usado em alguma nota fiscal
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_nf Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem notas fiscais utilizando-o.");

            // Verifica se o produto está sendo usado em algum material de projeto
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_item_projeto Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem cálculos de projeto utilizando-o.");

            // Verifica se o produto está sendo na configuração do projeto
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_projeto Where idProd=" + Key) > 0 ||
                objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_projeto_config Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem produtos no projeto utilizando-o.");

            // Verifica se o produto está sendo usado para baixa de estoque fiscal de outro
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_baixa_estoque_fiscal Where idProd=" + Key + " Or idProdBaixa=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem outros produtos associados ao mesmo como baixa de estoque.");

            // Verifica se o produto está sendo usado para baixa de estoque de outro
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_baixa_estoque Where idProd=" + Key + " Or idProdBaixa=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem outros produtos associados ao mesmo como baixa de estoque.");

            // Verifica se o produto está sendo usado para baixa de estoque de outro
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_loja Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem dados de estoque relacionados ao mesmo.");

            // Verifica se o produto está sendo usado em alguma chapa de vidro
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From chapa_vidro Where idProd=" + Key) > 0)
                throw new Exception("Este produto não pode ser excluído pois existem chapas de vidro relacionadas ao mesmo.");

            ProdutoBenefDAO.Instance.DeleteByProd(Key);
            ProdutoBaixaEstoqueDAO.Instance.DeleteByProd(Key);
            ProdutoBaixaEstoqueFiscalDAO.Instance.DeleteByProd(Key);
            IcmsProdutoUfDAO.Instance.DeleteByProd(Key);
            MvaProdutoUfDAO.Instance.DeleteByProd(Key);

            LogAlteracaoDAO.Instance.ApagaLogProduto(Key);
            int retorno = base.DeleteByPrimaryKey(Key);

            // Apaga os registros da tabela produto_fornecedor
            objPersistence.ExecuteCommand("delete from produto_fornecedor where idProd=" + Key);

            return retorno;*/

            throw new NotSupportedException();
        }

        #endregion

        public string ObterVendaPontoEquilibrio(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            string sql = SqlVendasProd(0, null, null, "0", null, null, null, null, TipoBuscaMateriaPrima.ApenasProduto, dataIni, dataFim, null, null, null, null, "5",
                null, null, "1,2,5", 0, 0, 0, 0, 0, false, false, false, false, false, 0, 0, ref lstParam, true);
            return sql;//objPersistence.LoadData(sql, new GDAParameter("?dtIni", dataIni), new GDAParameter("?dtFim", dataFim)).ToArray();
        }

        public Produto ObterProduto(string codRetalho)
        {
            return ObterProduto(null, codRetalho);
        }

        public Produto ObterProduto(GDASession session, string codRetalho)
        {
            try
            {
                return objPersistence.LoadOneData(session, "select * from produto where CodInterno=?codInterno and IdGrupoProd=1 and IdSubgrupoProd=4", new GDAParameter("?codInterno", codRetalho));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IList<Produto> ObterProdutosNota(uint idNf)
        {
            bool temFiltro;

            string sql = "Select e.* From (" + Sql(0, 0, null, null, 0, 0, 0, 0, null, null, null, null,
                false, false, null, null, 0, 0, 0, 0, true, TipoBusca.Normal, true, out temFiltro) + ") as e Where e.IdProd in (Select IdProd From produtos_nf Where IdNf = " + idNf + ")";

            return objPersistence.LoadData(sql).ToList();
        }

        #region Calcula totais de um item produto

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        public void CalcTotaisItemProd(uint idCliente, int idProd, int largura, int qtde, int qtdeAmbiente, decimal valorVendido, float espessura, bool redondo,
            int arredondarAluminio, bool compra, ref decimal custoProd, ref Single altura, ref Single totM2, ref decimal total, bool nf, int numeroBenef)
        {
            CalcTotaisItemProd(idCliente, idProd, largura, (float)qtde, (float)qtdeAmbiente, valorVendido, espessura, redondo,
                arredondarAluminio, compra, ref custoProd, ref altura, ref totM2, ref total, nf, numeroBenef);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        public void CalcTotaisItemProd(uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido, float espessura, bool redondo,
            int arredondarAluminio, bool compra, ref decimal custoProd, ref Single altura, ref Single totM2, ref decimal total, bool nf, int numeroBenef)
        {
            float totM2Calc = 0;
            CalcTotaisItemProd(null, idCliente, idProd, largura, qtde, qtdeAmbiente, valorVendido, espessura, redondo, arredondarAluminio, compra, true, ref custoProd, ref altura,
                ref totM2, ref totM2Calc, ref total, 2, 2, nf, numeroBenef, true);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="calcMult5">Verifica se produtos com cálculo de m² será calculado o mult. de 5</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        public void CalcTotaisItemProd(uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido, float espessura,
            bool redondo, int arredondarAluminio, bool compra, bool calcMult5, ref decimal custoProd, ref Single altura, ref Single totM2,
            ref float totM2Calc, ref decimal total, bool nf, int numeroBenef)
        {
            CalcTotaisItemProd(null, idCliente, idProd, largura, qtde, qtdeAmbiente, valorVendido, espessura,
                redondo, arredondarAluminio, compra, calcMult5, ref custoProd, ref altura, ref totM2,
                ref totM2Calc, ref total, nf, numeroBenef);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="calcMult5">Verifica se produtos com cálculo de m² será calculado o mult. de 5</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        internal void CalcTotaisItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido, float espessura,
            bool redondo, int arredondarAluminio, bool compra, bool calcMult5, ref decimal custoProd, ref Single altura, ref Single totM2,
            ref float totM2Calc, ref decimal total, bool nf, int numeroBenef)
        {
            CalcTotaisItemProd(sessao, idCliente, idProd, largura, qtde, qtdeAmbiente, valorVendido, espessura, redondo, arredondarAluminio, compra, true,
                ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, 2, 2, nf, numeroBenef, true);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="calcMult5">Verifica se produtos com cálculo de m² será calculado o mult. de 5</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        internal void CalcTotaisItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido, float espessura,
            bool redondo, int arredondarAluminio, bool compra, bool calcMult5, ref decimal custoProd, ref Single altura, ref Single totM2,
            ref float totM2Calc, ref decimal total, bool nf, int numeroBenef, bool calcularAreaMinima)
        {
            CalcTotaisItemProd(sessao, idCliente, idProd, largura, qtde, qtdeAmbiente, valorVendido, espessura, redondo, arredondarAluminio, compra, true,
                ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, 2, 2, nf, numeroBenef, true, calcularAreaMinima);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="calcMult5">Verifica se produtos com cálculo de m² será calculado o mult. de 5</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        internal void CalcTotaisItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido,
            float espessura, bool redondo, int arredondarAluminio, bool compra, bool calcMult5, ref decimal custoProd, ref Single altura, ref Single totM2,
            ref float totM2Calc, ref decimal total, int alturaML, int larguraML, bool nf, int numeroBenef, bool usarChapaVidro)
        {
            CalcTotaisItemProd(sessao, idCliente, idProd, largura, qtde, qtdeAmbiente, valorVendido, espessura, redondo, arredondarAluminio,
                compra, calcMult5, ref custoProd, ref altura, ref totM2, ref totM2Calc, ref total, alturaML, larguraML, nf, numeroBenef,
                usarChapaVidro, false);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto que é item de um:
        /// Pedido, ItemProjeto, PedidoEspelho, NF
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="largura"></param>
        /// <param name="qtde"></param>
        /// <param name="valorVendido"></param>
        /// <param name="redondo">Identifica se o vidro é redondo</param>
        /// <param name="arredondarAluminio">0-Não arredondar  1-Arredondar alterando a altura  2-Arredondar apenas para cálculo</param>
        /// <param name="compra">identifica se o produto vem de compra</param>
        /// <param name="calcMult5">Verifica se produtos com cálculo de m² será calculado o mult. de 5</param>
        /// <param name="custoProd"></param>
        /// <param name="altura"></param>
        /// <param name="totM2"></param>
        /// <param name="total"></param>
        internal void CalcTotaisItemProd(GDASession sessao, uint idCliente, int idProd, int largura, float qtde, float qtdeAmbiente, decimal valorVendido, float espessura,
            bool redondo, int arredondarAluminio, bool compra, bool calcMult5, ref decimal custoProd, ref Single altura, ref Single totM2,
            ref float totM2Calc, ref decimal total, int alturaML, int larguraML, bool nf, int numeroBenef, bool usarChapaVidro, bool calcularAreaMinima)
        {
            var produto = new ProdutoCalculoDTO()
            {
                IdProduto = (uint)idProd,
                Largura = largura,
                Qtde = qtde,
                QtdeAmbiente = (int)qtdeAmbiente,
                ValorUnit = valorVendido,
                Espessura = espessura,
                Redondo = redondo,
                CustoProd = custoProd,
                Altura = altura,
                TotM = totM2,
                TotM2Calc = totM2Calc,
                Total = total,
                AlturaBenef = alturaML,
                LarguraBenef = larguraML
            };

            var container = new ContainerCalculoDTO()
            {
                Cliente = new ClienteDTO(() => idCliente)
            };

            ValorTotal.Instance.Calcular(
                 sessao,
                 container,
                 produto,
                 (ArredondarAluminio)arredondarAluminio,
                 calcMult5,
                 nf,
                 compra,
                 numeroBenef,
                 usarChapaVidro
            );

            custoProd = produto.CustoProd;
            altura = produto.Altura;
            totM2 = produto.TotM;
            totM2Calc = produto.TotM2Calc;
            total = produto.Total;
        }

        #endregion

        #region Verifica se um preço é tabelado para um produto

        /// <summary>
        /// Verifica se um preço é tabelado para um produto.
        /// </summary>
        public bool IsPrecoTabela(uint idProd, decimal valor)
        {
            string sql = @"select count(*)>0 from produto where idProd=?id and (
                valorBalcao=?valor or valorAtacado=?valor or valorObra=?valor)";

            return ExecuteScalar<bool>(sql, new GDAParameter("?id", idProd),
                new GDAParameter("?valor", valor));
        }

        #endregion

        #region Ativa / Inativa Produtos

        public void AlterarSituacaoProduto(Glass.Situacao situacao, int? idGrupoProd, int? idSubgrupoProd)
        {
            if (idGrupoProd.GetValueOrDefault(0) == 0 && idSubgrupoProd.GetValueOrDefault(0) == 0)
                throw new Exception("Nenhum filtro foi informado para alterar a situação dos produtos.");

            var sql = @"
                UPDATE produto
                SET situacao = ?sit
                WHERE ";

            if (idGrupoProd.GetValueOrDefault(0) > 0)
                sql += "IdGrupoProd = " + idGrupoProd.Value;

            if (idSubgrupoProd.GetValueOrDefault(0) > 0)
                sql += "IdSubgrupoProd = " + idSubgrupoProd.Value;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?sit", situacao));
        }

        #endregion

        #region Valor de tabela do produto com base no tipo de entrega do pedido

        /// <summary>
        /// Atualiza o valor de tabela do produto e verifica se ele deve ser atualizado.
        /// </summary>
        internal bool VerificarAtualizarValorTabelaProduto(GDASession session, Pedido antigo, Pedido novo, IProdutoCalculo produto)
        {
            #region Declaração de variáveis

            decimal valorAtacado = 0;
            decimal valorBalcao = 0;
            decimal valorObra = 0;

            #endregion

            #region Recuperação do valor de tabela com base no tipo de entrega do pedido

            if (novo.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
            {
                produto.InicializarParaCalculo(session, antigo);

                produto.ValorUnit = produto.DadosProduto.ValorTabela();
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = 0;

                return true;
            }
            else
            {
                var container = new ContainerCalculoDTO(novo);
                int? tipoEntregaOriginal = novo.TipoEntrega;
                bool revendaOriginal = container.Cliente.Revenda;

                produto.InicializarParaCalculo(session, container);

                Func<int, bool, decimal> valorTabelaTipoEntregaERevenda = (tipoEntrega, revenda) =>
               {
                   container.TipoEntrega = tipoEntrega;
                   (container.Cliente as ClienteDTO).Revenda = revenda;

                   var valor = produto.DadosProduto.ValorTabela();

                   (container.Cliente as ClienteDTO).Revenda = revendaOriginal;
                   container.TipoEntrega = tipoEntregaOriginal;

                   return valor;
               };

                valorAtacado = valorTabelaTipoEntregaERevenda((int)Pedido.TipoEntregaPedido.Balcao, true);
                valorBalcao = valorTabelaTipoEntregaERevenda((int)Pedido.TipoEntregaPedido.Balcao, revendaOriginal);
                valorObra = valorTabelaTipoEntregaERevenda((int)Pedido.TipoEntregaPedido.Comum, revendaOriginal);

                var tipoEntregaDiferencaCliente = novo.TipoEntrega;

                // Se o cliente é revenda.
                if (container.Cliente.Revenda && (produto.ValorUnit < valorAtacado || antigo.IdCli != novo.IdCli))
                {
                    produto.ValorUnit = valorAtacado;
                }
                // Se o tipo de entrega for balcão, traz preço de balcão.
                else if (novo.TipoEntrega == (int)Pedido.TipoEntregaPedido.Balcao)
                {
                    produto.ValorUnit = valorBalcao;
                }
                // Se o tipo de entrega for entrega, traz preço de obra.
                else if (novo.TipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega || novo.TipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado)
                {
                    produto.ValorUnit = valorObra;
                    tipoEntregaDiferencaCliente = (int)Pedido.TipoEntregaPedido.Entrega;
                }
                // Verifica se o valor é permitido, se não for atualiza o valor para o mínimo.
                else if (produto.ValorUnit < valorObra)
                {
                    produto.ValorUnit = valorObra;
                    tipoEntregaDiferencaCliente = (int)Pedido.TipoEntregaPedido.Comum;
                }
                else
                {
                    return false;
                }

                container.TipoEntrega = tipoEntregaDiferencaCliente;
                DiferencaCliente.Instance.Calcular(session, container, produto);

                return true;
            }

            #endregion
        }

        #endregion
    }
}
