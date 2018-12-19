using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class MovimentacaoEstoqueDAO : BaseDAO<MovimentacaoEstoque, MovimentacaoEstoqueDAO>
    {
        //private MovimentacaoEstoqueDAO() { }

        #region SQL

        #region Métodos de suporte

        private class Complementos
        {
            public string Tabela { get; private set; }
            public string Campo { get; private set; }

            private Complementos(string tabela, string campo)
            {
                Tabela = tabela;
                Campo = campo;
            }

            public static Complementos Obtem(bool fiscal, bool cliente)
            {
                if (fiscal)
                    return new Complementos("_fiscal", "Fiscal");
                else if (cliente)
                    return new Complementos("_cliente", "Cliente");
                else
                    return new Complementos(String.Empty, String.Empty);
            }
        }

        private string GetNumeroDocumento(bool fiscal, bool cliente)
        {
            if (fiscal)
            {
                return @"if(me.lancManual, cast('Lanc. Manual' as char CHARACTER SET utf8),
                    if(me.idNf > 0, cast(concat('NF-e: ', nf.numeroNFe) as char CHARACTER SET utf8), null))";
            }
            else if (cliente)
            {
                return @"if(me.lancManual, cast(CONCAT('Lanc. Manual', ' (', me.observacao, ')') as char CHARACTER SET utf8),
                    if(me.idLiberarPedido > 0, cast(concat('Liberação: ', me.idLiberarPedido, if(me.idPedido > 0, concat(', Pedido: ', me.idPedido), '')) as char CHARACTER SET utf8), 
                    if(me.idPedido > 0, cast(concat('Pedido: ', me.idPedido) as char CHARACTER SET utf8),
                    if(me.idProdPedProducao > 0, (select cast(concat('Pedido: ', idPedido) as char CHARACTER SET utf8) from produtos_pedido pp
                                                        inner join produto_pedido_producao ppp ON (pp.idProdPedEsp = ppp.idProdPed)
                                                    where ppp.idProdPedProducao = me.IDPRODPEDPRODUCAO
                                                    group by idProdPedProducao),
                    if(me.idNf > 0, cast(concat('NF-e: ', nf.numeroNFe) as char CHARACTER SET utf8), null)))))";
            }
            else
            {
                return @"if(me.lancManual, cast('Lanc. Manual' as char CHARACTER SET utf8),
                    if(me.idLiberarPedido > 0, cast(concat('Liberação: ', me.idLiberarPedido, if(me.idPedido > 0, concat(', Pedido: ', me.idPedido), '')) as char CHARACTER SET utf8), 
                    if(me.idPedido > 0, cast(concat('Pedido: ', me.idPedido) as char CHARACTER SET utf8),
                    if(me.idCompra > 0, cast(concat('Compra: ', me.idCompra) as char CHARACTER SET utf8),
                    if(me.idProdPedProducao > 0, cast(concat('Etiqueta: ', coalesce(ppp.numEtiqueta, ppp.numEtiquetaCanc)) as char CHARACTER SET utf8),
                    if(me.idTrocaDevolucao > 0, cast(concat(if(td.tipo=1, 'Troca: ', 'Devolução: '), me.idTrocaDevolucao) as char CHARACTER SET utf8),
                    if(me.idNf > 0, cast(concat('NF-e: ', nf.numeroNFe) as char CHARACTER SET utf8),
                    if(me.idPedidoInterno > 0, cast(concat('Pedido Interno: ', me.idPedidoInterno) as char CHARACTER SET utf8), 
                    if(me.idRetalhoProducao > 0, cast(concat('Etiqueta: R', me.idRetalhoProducao, '-', pi.itemEtiqueta, '/', pi.qtdeProd) as char CHARACTER SET utf8), null)))))))))";
            }
        }

        private string GetTabelas(bool fiscal, bool cliente)
        {
            var tabela = Complementos.Obtem(fiscal, cliente).Tabela;

            if (fiscal)
            {
                return String.Format(@"mov_estoque{0} me
                    left join nota_fiscal nf on (me.idNf=nf.idNf)
                    left join produtos_nf pnf on (me.idProdNf=pnf.idProdNf)
                    left join natureza_operacao no on (pnf.idNaturezaOperacao=no.idNaturezaOperacao)",
                                                                                                     
                    tabela);
            }
            else if (cliente)
            {
                return String.Format(@"mov_estoque{0} me
                    left join nota_fiscal nf on (me.idNf=nf.idNf)
                    left join produtos_nf pnf on (me.idProdNf=pnf.idProdNf)
                    left join natureza_operacao no on (pnf.idNaturezaOperacao=no.idNaturezaOperacao)
                    left join produto_pedido_producao ppp on (me.idProdPedProducao=ppp.idProdPedProducao)",

                    tabela);
            }
            else
            {
                return @"mov_estoque me
                    left join nota_fiscal nf on (me.idNf=nf.idNf)
                    left join troca_devolucao td on (me.idTrocaDevolucao=td.idTrocaDevolucao)
                    left join produto_pedido_producao ppp on (me.idProdPedProducao=ppp.idProdPedProducao)
                    left join produto_impressao pi on (me.idRetalhoProducao=pi.idRetalhoProducao)";
            }
        }

        private string GetCriterio(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string ncm, int? numeroNfe, string codOtimizacao, string dataIni,
            string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem,
            uint idCorAluminio, bool fiscal, bool cliente)
        {
            StringBuilder criterio = new StringBuilder();

            if (cliente && idCliente > 0)
                criterio.AppendFormat("Cliente: {0}    ", ClienteDAO.Instance.GetNome(idCliente));

            if (idLoja > 0)
                criterio.AppendFormat("Loja: {0}    ", LojaDAO.Instance.GetNome(idLoja));

            if (!String.IsNullOrEmpty(codInternoProd) || !String.IsNullOrEmpty(descrProd))
            {
                if (!String.IsNullOrEmpty(descrProd))
                    criterio.AppendFormat("Produto: {0}    ", descrProd);
                else
                    criterio.AppendFormat("Produto: {0}    ", ProdutoDAO.Instance.ObtemDescricao(codInternoProd));
            }

            if (!string.IsNullOrEmpty(ncm))
                criterio.AppendFormat("NCM: {0}    ", ncm);

            if (numeroNfe > 0)
                criterio.AppendFormat("Nota Fiscal: {0}    ", numeroNfe);

            if (!String.IsNullOrEmpty(codOtimizacao))
                criterio.AppendFormat("Cod. Otimização: {0}    ", codOtimizacao);

            if (!String.IsNullOrEmpty(dataIni) && DateTime.Parse(dataIni).Year != 2000)
                criterio.AppendFormat("Data início: {0}    ", dataIni);

            if (!String.IsNullOrEmpty(dataFim))
                criterio.AppendFormat("Data término: {0}    ", dataFim);

            if (tipoMov > 0)
            {
                MovEstoque me = new MovEstoque();
                me.TipoMov = tipoMov;
                criterio.AppendFormat("Tipo: {0}    ", me.DescrTipoMov);
            }

            if (situacaoProd > 0)
                criterio.AppendFormat("Situação: {0}    ", situacaoProd == 1 ? "Ativos" : "Inativos");

            if ((fiscal || cliente) && idCfop > 0)
                criterio.AppendFormat("CFOP: {0}    ", CfopDAO.Instance.ObtemCodInterno(idCfop));

            if (!String.IsNullOrEmpty(idsGrupoProd))
            {
                var grupos = String.Empty;

                foreach (var id in idsGrupoProd.Split(','))
                    grupos += GrupoProdDAO.Instance.GetDescricao(Conversoes.StrParaInt(id)) + ", ";

                criterio.AppendFormat("Grupo(s): {0}    ", grupos.TrimEnd(' ', ','));
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd))
            {
                var subgrupos = string.Empty;
                foreach (var id in idsSubgrupoProd.Split(','))
                    subgrupos += SubgrupoProdDAO.Instance.GetDescricao(Conversoes.StrParaInt(id)) + ", ";

                criterio.AppendFormat("Subgrupo(s): {0}    ", subgrupos.TrimEnd(' ', ','));
            }

            if (idCorVidro > 0)
                criterio.AppendFormat("Cor Vidro: {0}    ", CorVidroDAO.Instance.GetNome(idCorVidro));

            if (idCorFerragem > 0)
                criterio.AppendFormat("Cor Ferragem: {0}    ", CorFerragemDAO.Instance.GetNome(idCorFerragem));

            if (idCorAluminio > 0)
                criterio.AppendFormat("Cor Alumínio: {0}    ", CorAluminioDAO.Instance.GetNome(idCorAluminio));

            return criterio.ToString();
        }

        #endregion

        internal string SqlEstoqueInicialIdsProd(
            int idCliente,
            int idLoja,
            string codInternoProd,
            string descrProd,
            string ncm,
            int? numeroNfe,
            string codOtimizacao,
            string dataIni,
            string dataFim,
            int tipoMov,
            int situacaoProd,
            int idCfop,
            string idsGrupoProd,
            string idsSubgrupoProd,
            int idCorVidro,
            int idCorFerragem,
            int idCorAluminio,
            bool fiscal,
            bool cliente,
            bool ignorarUsoConsumo,
            List<TipoMercadoria> tipoMercadoria,
            out string filtroAdicional,
            out bool temFiltro)
        {
            filtroAdicional = string.Empty;
            temFiltro = false;

            var sql = $@"SELECT DISTINCT(me.IdProd)
                FROM {this.GetTabelas(fiscal, cliente)}
                    INNER JOIN produto p ON (me.IdProd = p.IdProd)
                    INNER JOIN grupo_prod g ON (p.IdGrupoProd = g.IdGrupoProd)
                WHERE 1";

            var filtroSql = this.ObterFiltroSql(
                idCliente,
                idLoja,
                codInternoProd,
                descrProd,
                ncm,
                numeroNfe,
                codOtimizacao,
                dataIni,
                dataFim,
                tipoMov,
                situacaoProd,
                idCfop,
                idsGrupoProd,
                idsSubgrupoProd,
                idCorVidro,
                idCorFerragem,
                idCorAluminio,
                fiscal,
                cliente,
                ignorarUsoConsumo,
                tipoMercadoria,
                out filtroAdicional,
                out temFiltro);

            sql += $"{filtroSql}{filtroAdicional}";

            return sql;
        }

        internal string SqlEstoqueInicial(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string ncm, int? numeroNfe, string codOtimizacao, string dataIni,
            string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem,
            uint idCorAluminio, bool fiscal, bool cliente, bool ignorarUsoConsumo, List<TipoMercadoria> tipoMercadoria,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            string criterio = GetCriterio(idCliente, idLoja, codInternoProd, descrProd, ncm, numeroNfe, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd, idCfop,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente);

            var compl = Complementos.Obtem(fiscal, cliente);

            StringBuilder sql = new StringBuilder("select ");
            sql.AppendFormat(selecionar ? @"idMovEstoque, temp.idProd, date(?dataIni) as data,
                'Estoque inicial' as numeroDocumento, 1 as tipo, sum(qtdeSaldo) as qtde, sum(valor) as valor,
                p.codInterno as codInternoProd, p.descricao as descrProd, qtdeSaldo as qtdeSaldo,
                sum(valorSaldo) as valorSaldo, true as estoqueInicial, '{0}' as criterio, 
                concat(g.descricao, if(s.idSubgrupoProd is not null, concat(' - ', s.descricao), '')) as descrTipoProd,
                coalesce({1}s.tipoCalculo, g.tipoCalculo) as tipoCalc, u.codigo as unidadeProd, COALESCE(ncm.ncm, p.ncm) as ncm" : "sum(num)",
                criterio, 
                fiscal ? "s.tipoCalculoNf, g.tipoCalculoNf, " : string.Empty);

            var idsProd = this.ExecuteMultipleScalar<int>(
                this.SqlEstoqueInicialIdsProd(
                    (int)idCliente,
                    (int)idLoja,
                    codInternoProd,
                    descrProd,
                    ncm,
                    numeroNfe,
                    codOtimizacao,
                    dataIni,
                    dataFim,
                    tipoMov,
                    situacaoProd,
                    (int)idCfop,
                    idsGrupoProd,
                    idsSubgrupoProd,
                    (int)idCorVidro,
                    (int)idCorFerragem,
                    (int)idCorAluminio,
                    fiscal,
                    cliente,
                    ignorarUsoConsumo,
                    tipoMercadoria,
                    out filtroAdicional,
                    out temFiltro),
                this.GetParams(dataIni, dataFim, ncm));

            if (!idsProd.Any(f => f > 0))
            {
                idsProd = new List<int> { 0 };
            }

            sql.Append(" from (select ");
            sql.AppendFormat(selecionar ? @"me.idMovEstoque{0} as idMovEstoque, me.idProd, 
                me.qtdeMov as qtde, me.saldoValorMov as valor, me.saldoQtdeMov as qtdeSaldo,
                me.saldoValorMov as valorSaldo" : "count(*) as num", compl.Campo);

            sql.AppendFormat(@"
                from mov_estoque{0} me
                    left join (
                        select * from (
                            select idMovEstoque{1} as idMovEstoque, idProd, idLoja
                            from mov_estoque{0}
                            where idProd in ({2}) and idLoja={3} and dataMov < ?dataIni {4}
                            order by dataMov desc, idMovEstoque desc
                        ) as temp
                        group by idProd, idLoja
                    ) id on (me.idProd=id.idProd and me.idLoja=id.idLoja)
                where me.idMovEstoque{1}=id.idMovEstoque and me.idProd in ({2}) and me.idLoja={3}",

                compl.Tabela,
                compl.Campo,
                string.Join(",", idsProd), idLoja, (cliente && idCliente > 0 ? " AND idCliente=" + idCliente : ""));

            sql.Append(" union all select ");
            sql.Append(selecionar ? @"null as idMovEstoque, idProd, cast(0 as decimal(12,2)) as qtde, 
                cast(0 as decimal(12,2)) as valor, cast(0 as decimal(12,2)) as qtdeSaldo,
                cast(0 as decimal(12,2)) as valorSaldo" : "count(*) as num");

            sql.AppendFormat(@"
                from produto
                where idProd in ({0})", string.Join(",", idsProd));

            sql.AppendFormat(@"
                ) as temp
                    left join produto p on (temp.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                    left join unidade_medida u on (p.idUnidadeMedida=u.idUnidadeMedida)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                    ) as ncm ON ({1} = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                where {0}
                group by idProd", "g.tipoGrupo<>" + (int)TipoGrupoProd.UsoConsumo, idLoja);

            return sql.ToString();
        }

        internal string ObterFiltroSql(
            int idCliente,
            int idLoja,
            string codInternoProd,
            string ncm,
            string descrProd,
            int? numeroNfe,
            string codOtimizacao,
            string dataIni,
            string dataFim,
            int tipoMov,
            int situacaoProd,
            int idCfop,
            string idsGrupoProd,
            string idsSubgrupoProd,
            int idCorVidro,
            int idCorFerragem,
            int idCorAluminio,
            bool fiscal,
            bool cliente,
            bool ignorarUsoConsumo,
            List<TipoMercadoria> tipoMercadoria,
            out string filtroAdicional,
            out bool temFiltro)
        {
            var filtroSql = string.Empty;
            filtroAdicional = string.Empty;
            temFiltro = false;

            if (cliente && idCliente > 0)
            {
                filtroAdicional += $" AND me.IdCliente = {idCliente}";
            }

            if (idLoja > 0)
            {
                filtroAdicional += $" AND me.IdLoja = {idLoja}";
            }

            if (!string.IsNullOrWhiteSpace(codInternoProd)
                || !string.IsNullOrWhiteSpace(descrProd)
                || !string.IsNullOrWhiteSpace(codOtimizacao))
            {
                var idsProduto = ProdutoDAO.Instance.ObtemIds(codInternoProd, descrProd, codOtimizacao);
                filtroAdicional += $" AND me.IdProd IN ({idsProduto})";
            }

            if (!string.IsNullOrWhiteSpace(ncm))
            {
                filtroAdicional += " AND (p.Ncm = ?ncm OR ncm.Ncm = ?ncm)";
            }

            if (numeroNfe > 0)
            {
                var idsNf = NotaFiscalDAO.Instance.ObterIdsNf(null, numeroNfe.Value);
                filtroAdicional += $" AND me.IdNf IN ({idsNf})";
            }

            if (!string.IsNullOrWhiteSpace(dataIni))
            {
                filtroAdicional += " AND me.DataMov >= ?dataIni";
            }

            if (!string.IsNullOrWhiteSpace(dataFim))
            {
                filtroAdicional += " AND me.DataMov <= ?dataFim";
            }

            if (tipoMov > 0)
            {
                filtroAdicional += $" AND me.TipoMov = {tipoMov}";
            }

            if (situacaoProd > 0)
            {
                filtroSql += $" AND p.Situacao = {situacaoProd}";
                temFiltro = true;
            }

            if ((fiscal || cliente) && idCfop > 0)
            {
                filtroSql += $" AND no.IdCfop = {idCfop}";
                temFiltro = true;
            }

            if (!string.IsNullOrWhiteSpace(idsGrupoProd) && idsGrupoProd != "0")
            {
                filtroSql += $" AND p.IdGrupoProd IN ({idsGrupoProd})";
                temFiltro = true;
            }

            if (!string.IsNullOrWhiteSpace(idsSubgrupoProd) && idsSubgrupoProd != "0")
            {
                filtroSql += $" AND p.IdSubgrupoProd IN ({idsSubgrupoProd})";
                temFiltro = true;
            }

            if (idCorVidro > 0)
            {
                filtroSql += $" AND p.IdCorVidro = {idCorVidro}";
                temFiltro = true;
            }

            if (idCorFerragem > 0)
            {
                filtroSql += $" AND p.IdCorFerragem = {idCorFerragem}";
                temFiltro = true;
            }

            if (idCorAluminio > 0)
            {
                filtroSql += $" AND p.IdCorAluminio = {idCorAluminio}";
                temFiltro = true;
            }

            if (ignorarUsoConsumo)
            {
                filtroSql += $" AND g.TipoGrupo <> {(int)TipoGrupoProd.UsoConsumo}";
                temFiltro = true;
            }

            if (tipoMercadoria?.Count > 0)
            {
                filtroSql += $" AND p.TipoMercadoria IN ({string.Join(",", tipoMercadoria.Select(f => ((int)f).ToString()).ToArray())})";
                temFiltro = true;
            }

            return filtroSql;
        }

        internal string Sql(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string ncm, int? numeroNfe, string codOtimizacao, string dataIni, string dataFim,
            int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio,
            bool fiscal, bool cliente, bool ignorarUsoConsumo, List<Glass.Data.Model.TipoMercadoria> tipoMercadoria,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            var criterio = GetCriterio(idCliente, idLoja, codInternoProd, descrProd, ncm, numeroNfe, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd, idCfop,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente);

            var compl = Complementos.Obtem(fiscal, cliente);
            var sql = new StringBuilder("select ");
            sql.AppendFormat(selecionar ? @"me.idMovEstoque{0} as idMovEstoque, me.idProd, me.dataMov as data, 
                {1} as numeroDocumento, me.tipoMov as tipo, me.qtdeMov as qtde, me.valorMov as valor, 
                p.codInterno as codInternoProd, p.descricao as descrProd, me.saldoQtdeMov as qtdeSaldo, 
                me.saldoValorMov as valorSaldo, false as estoqueInicial, '{2}' as criterio,
                concat(g.descricao, if(s.idSubgrupoProd is not null, concat(' - ', s.descricao), '')) as descrTipoProd,
                coalesce({3}s.tipoCalculo, g.tipoCalculo) as tipoCalc, u.codigo as unidadeProd, COALESCE(ncm.ncm, p.ncm) as ncm" : "count(*)",
                compl.Campo, 
                GetNumeroDocumento(fiscal, cliente), criterio, 
                fiscal ? "s.tipoCalculoNf, g.tipoCalculoNf, " : "");
            
            sql.AppendFormat(@"
                from {0}
                    left join produto p on (me.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd)
                    left join unidade_medida u on (p.idUnidadeMedida=u.idUnidadeMedida)
                    Left Join produto_loja pl on (pl.idProd=me.idProd and pl.idLoja=me.idLoja)
                    LEFT JOIN
                    (
                        SELECT *
                        FROM produto_ncm
                    ) as ncm ON (me.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                where exists (select * from produto where idProd=me.idProd) {1}",
                GetTabelas(fiscal, cliente),
                FILTRO_ADICIONAL);

            var filtroSql = ObterFiltroSql(
                (int)idCliente,
                (int)idLoja,
                codInternoProd,
                descrProd,
                ncm,
                numeroNfe,
                codOtimizacao,
                dataIni,
                dataFim,
                tipoMov,
                situacaoProd,
                (int)idCfop,
                idsGrupoProd,
                idsSubgrupoProd,
                (int)idCorVidro,
                (int)idCorFerragem,
                (int)idCorAluminio,
                fiscal,
                cliente,
                ignorarUsoConsumo,
                tipoMercadoria,
                out filtroAdicional,
                out temFiltro);

            sql.AppendFormat(filtroSql);

            return sql.ToString();
        }

        internal string SqlComparativo(uint idCliente, uint idLoja, string codInternoProd, string descrProd, int? numeroNfe, string dataIni, string dataFim,
            int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio,
            bool fiscal, bool cliente, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            string criterio = GetCriterio(idCliente, idLoja, codInternoProd, descrProd, null, numeroNfe, null, dataIni, dataFim, tipoMov, situacaoProd, idCfop,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente);

            var compl = Complementos.Obtem(fiscal, cliente);

            StringBuilder sql = new StringBuilder("Select ");
            sql.AppendFormat(selecionar ? @"me.idMovEstoque{0} as idMovEstoque, me.idProd, me.saldoQtdeMov as qtdeSaldo" :
                "count(*)", compl.Campo);

            sql.AppendFormat(@"
                from mov_estoque{0} me
                    left join nota_fiscal nf on (me.idNf=nf.idNf)
                    left join produtos_nf pnf on (me.idProdNf=pnf.idProdNf)
                    left join natureza_operacao no on (pnf.idNaturezaOperacao=no.idNaturezaOperacao)
                    left join produto p on (me.idProd=p.idProd)
                    left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left Join produto_loja pl on (pl.idProd=me.idProd and pl.idLoja=me.idLoja)
                where g.tipoGrupo<>{1} {2}",
                
                compl.Tabela,
                (int)TipoGrupoProd.UsoConsumo,
                FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (cliente && idCliente > 0)
                fa.AppendFormat(" and me.idCliente={0}", idCliente);

            if (idLoja > 0)
                fa.AppendFormat(" and me.idLoja={0}", idLoja);

            if (!String.IsNullOrEmpty(codInternoProd) || !String.IsNullOrEmpty(descrProd))
                fa.AppendFormat(" and me.idProd in ({0})", ProdutoDAO.Instance.ObtemIds(codInternoProd, descrProd));

            if (numeroNfe > 0)
                fa.AppendFormat(" AND me.IdNf IN ({0})", NotaFiscalDAO.Instance.ObterIdsNf(null, numeroNfe.Value));

            if (!String.IsNullOrEmpty(dataIni))
                fa.Append(" and me.dataMov>=?dataIni");

            if (!String.IsNullOrEmpty(dataFim))
                fa.Append(" and me.dataMov<=?dataFim");

            if (tipoMov > 0)
                fa.AppendFormat(" and me.tipoMov={0}", tipoMov);

            if (situacaoProd > 0)
            {
                sql.AppendFormat(" and p.situacao={0}", situacaoProd);
                temFiltro = true;
            }

            if ((fiscal || cliente) && idCfop > 0)
            {
                sql.AppendFormat(" and no.idCfop={0}", idCfop);
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idsGrupoProd))
            {
                sql.AppendFormat(" and p.idGrupoProd IN ({0})", idsGrupoProd);
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd))
            {
                sql.AppendFormat(" and p.idSubgrupoProd IN({0})", idsSubgrupoProd);
                temFiltro = true;
            }

            if (idCorVidro > 0)
            {
                sql.AppendFormat(" and p.idCorVidro={0}", idCorVidro);
                temFiltro = true;
            }

            if (idCorFerragem > 0)
            {
                sql.AppendFormat(" and p.idCorFerragem={0}", idCorFerragem);
                temFiltro = true;
            }

            if (idCorAluminio > 0)
            {
                sql.AppendFormat(" and p.idCorAluminio={0}", idCorAluminio);
                temFiltro = true;
            }

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string ncm)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(ncm))
                lst.Add(new GDAParameter("?ncm", ncm));

            return lst.ToArray();
        }

        #endregion

        public IList<MovimentacaoEstoque> GetList(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio,
            bool fiscal, bool cliente, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idCliente, idLoja, codInternoProd, descrProd, null, null, null, dataIni, dataFim, tipoMov, situacaoProd, idCfop,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente, false, null, true, out temFiltro, out filtroAdicional);

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "idProd, data";
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParams(dataIni, dataFim, null));
        }

        public int GetCount(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem,
            uint idCorAluminio, bool fiscal, bool cliente)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idCliente, idLoja, codInternoProd, descrProd, null, null, null, dataIni, dataFim, tipoMov, situacaoProd, idCfop,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente, false, null, true, out temFiltro, out filtroAdicional);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParams(dataIni, dataFim, null));
        }

        public IList<MovimentacaoEstoque> GetForRpt(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string ncm, int? numeroNfe, string codOtimizacao,
            string dataIni, string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro,
            uint idCorFerragem, uint idCorAluminio, bool fiscal, bool cliente, bool naoBuscarEstoqueZero, bool ignorarUsoConsumo,
            List<Glass.Data.Model.TipoMercadoria> tipoMercadoria)
        {
            bool temFiltro;
            string filtroAdicional, sql = @"
                set @saldo := 0.0;
                select idMovEstoque, idProd, codInternoProd, descrProd, descrTipoProd, data, numeroDocumento,
                    tipo, qtde, valor, estoqueInicial, tipoCalc, unidadeProd, criterio, qtdeSaldo, 
                    @saldo as valorSaldoAnt, (@saldo := valorSaldo) as valorSaldo, ncm
                from (
                    select idMovEstoque, idProd, codInternoProd, descrProd, descrTipoProd, data, numeroDocumento,
                        tipo, sum(coalesce(qtde,0)) as qtde, sum(coalesce(valor,0)) as valor, estoqueInicial,
                        tipoCalc, unidadeProd, criterio, qtdeSaldo, valorSaldo, ncm
                    from (
                        " + SqlEstoqueInicial(idCliente, idLoja, codInternoProd, descrProd, ncm, numeroNfe, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd,
                            idCfop, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente, ignorarUsoConsumo, tipoMercadoria,
                            true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional) + @"
                        union all " + Sql(idCliente, idLoja, codInternoProd, descrProd, ncm, numeroNfe, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd,
                            idCfop, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente, ignorarUsoConsumo, tipoMercadoria,
                            true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional) + @"
                        order by data desc, idMovEstoque desc
                    ) as m";

            if (cliente)
                sql += " group by idProd, date(data), numeroDocumento, tipo, estoqueInicial";
            else
                sql += " group by idMovEstoque";

            sql += @" order by codInternoProd asc, idProd asc, data asc, idMovEstoque asc, !estoqueInicial asc) as final";

            var movs = objPersistence.LoadData(sql, GetParams(dataIni, dataFim, ncm)).ToList();

            if (cliente)
            {
                decimal qtdeSaldo = 0;

                foreach (var mov in movs)
                {
                    if (mov.EstoqueInicial)
                        qtdeSaldo = 0;

                    qtdeSaldo += mov.QtdeEntrada - mov.QtdeSaida;
                    mov.QtdeSaldo = qtdeSaldo;
                }
            }

            return movs;
        }

        public IList<MovimentacaoEstoque> GetForRptComparativo(uint idCliente, uint idLoja, string codInternoProd, string descrProd, int? numeroNfe,
            string dataIni, string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro,
            uint idCorFerragem, uint idCorAluminio, bool fiscal, bool cliente, bool naoBuscarEstoqueZero)
        {
            bool temFiltro;
            string filtroAdicional, sql = @"
                Select idMovEstoque, idProd, qtdeSaldo, Null As codInternoProd, Null As descrProd, Null As descrTipoProd, Null As Data, Null As numeroDocumento,
                    Null As tipo, Null As qtde, Null As valor, Null As estoqueInicial, Null As tipoCalc, Null As unidadeProd, Null As criterio, Null As valorSaldo,
                    Null As valorSaldoAnt, null as ncm
                From (
                    Select idMovEstoque, idProd, qtdeSaldo
                    From (
                        " + SqlComparativo(idCliente, idLoja, codInternoProd, descrProd, numeroNfe, dataIni, dataFim, tipoMov, situacaoProd,
                            idCfop, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, fiscal, cliente,
                            true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional) + @"
                        Order By idMovEstoque Desc
                    ) As m
                    Order By idProd Asc, idMovEstoque Desc
                ) As final Group By idProd";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim, null)).ToList();
        }

        public MovimentacaoEstoque[] GetForRptTotal(uint idCliente, uint idLoja, string codInternoProd, string descrProd, string ncm, int? numeroNfe, string codOtimizacao,
            string dataIni, string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro,
            uint idCorFerragem, uint idCorAluminio, bool fiscal, bool cliente, bool naoBuscarEstoqueZero, bool usarValorFiscal, bool ignorarUsoConsumo,
            List<Glass.Data.Model.TipoMercadoria> tipoMercadoria)
        {
            List<MovimentacaoEstoque> retorno = new List<MovimentacaoEstoque>(GetForRpt(idCliente, idLoja, codInternoProd, descrProd, ncm, numeroNfe,
                codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd, idCfop, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, fiscal, cliente, naoBuscarEstoqueZero, ignorarUsoConsumo, tipoMercadoria));

            uint idProd = 0;
            for (int i = retorno.Count - 1; i >= 0; i--)
            {
                if (retorno[i].IdProd != idProd)
                {
                    idProd = retorno[i].IdProd;

                    if (retorno[i].TipoCalc == (int) Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 ||
                        retorno[i].TipoCalc == (int) Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        retorno[i].TipoCalc == (int) Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                        retorno[i].TipoCalc == (int) Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                    {
                        retorno[i].ComplementoQtd = " (" + Math.Round(retorno[i].QtdeSaldo/6, 2) + " barras)";
                    }
                }
                else
                {
                    retorno.RemoveAt(i);
                    continue;
                }

                /* Chamado 22802. */
                if (fiscal && GrupoProdDAO.Instance.ObtemValorCampo<TipoGrupoProd>("TipoGrupo",
                    string.Format("IdGrupoProd={0}", ProdutoDAO.Instance.ObtemIdGrupoProd((int)retorno[i].IdProd))) ==
                    TipoGrupoProd.UsoConsumo)
                    retorno.RemoveAt(i);
            }

            // Remove os resultados que ficaram com valores zerados após a soma
            if (naoBuscarEstoqueZero)
                for (int i = retorno.Count - 1; i >= 0; i--)
                    if (retorno[i].QtdeSaldo == 0)
                        retorno.RemoveAt(i);

            // Calcula o valor do saldo usando o campo valor fiscal se o checkbox estiver marcado
            if (usarValorFiscal)
                for (int i = 0; i < retorno.Count; i++)
                {
                    // Altera o valor para o valor fiscal para que mesmo que a quantidade seja 0, o valor unitário seja exibido
                    retorno[i].Valor = ProdutoDAO.Instance.ObtemValorFiscal((int)retorno[i].IdProd);
                    retorno[i].ValorSaldo = retorno[i].Valor * retorno[i].QtdeSaldo;

                    retorno[i].ValorVenda = ProdutoDAO.Instance.ObtemValorCampo<decimal>("ValorBalcao", "IdProd = " + retorno[i].IdProd);
                    retorno[i].ValorTotalVenda = retorno[i].ValorVenda * retorno[i].QtdeSaldo;
                }

            if (fiscal)
                for (int i = 0; i < retorno.Count; i++)
                {
                    retorno[i].ValorVenda = ProdutoDAO.Instance.ObtemValorCampo<decimal>("ValorBalcao", "IdProd = " + retorno[i].IdProd);
                    retorno[i].ValorTotalVenda = retorno[i].ValorVenda * retorno[i].QtdeSaldo;
                }

            return retorno.ToArray();
        }

        public MovimentacaoEstoque[] GetForRptTotalComparativo(uint idCliente, uint idLoja, string codInternoProd, string descrProd, int? numeroNfe,
            string dataIni, string dataFim, int tipoMov, int situacaoProd, uint idCfop, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro,
            uint idCorFerragem, uint idCorAluminio, bool fiscal, bool naoBuscarEstoqueZero, bool usarValorFiscal)
        {
            List<MovimentacaoEstoque> retorno = new List<MovimentacaoEstoque>(GetForRptComparativo(idCliente, idLoja, codInternoProd, descrProd,
                numeroNfe, dataIni, dataFim, tipoMov, situacaoProd, idCfop, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, fiscal, false, naoBuscarEstoqueZero));

            uint idProd = 0;
            for (int i = retorno.Count - 1; i >= 0; i--)
            {
                if (retorno[i].IdProd != idProd)
                {
                    idProd = retorno[i].IdProd;

                    if (retorno[i].TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || retorno[i].TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        retorno[i].TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || retorno[i].TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                        retorno[i].ComplementoQtd = " (" + Math.Round(retorno[i].QtdeSaldo / 6, 2) + " barras)";
                }
                else
                    retorno.RemoveAt(i);
            }

            // Remove os resultados que ficaram com valores zerados após a soma
            if (naoBuscarEstoqueZero)
                for (int i = retorno.Count - 1; i >= 0; i--)
                    if (retorno[i].QtdeSaldo == 0)
                        retorno.RemoveAt(i);

            return retorno.ToArray();
        }

        public MovimentacaoEstoque[] GetForEFD(uint idLoja, DateTime dataInventario, bool fiscal, bool ignorarUsoConsumo)
        {
            string dataIni = "01/01/2000"; // Alterado para que fique igual ao inventário
            string dataFim = dataInventario.ToString("dd/MM/yyyy");
            return GetForRptTotal(0, idLoja, null, null, null, null, null, dataIni, dataFim, 0, 0, 0, "", "", 0, 0, 0, fiscal, false, true, false, ignorarUsoConsumo, null);
        }

        public MovimentacaoEstoque[] GetForEFD(int idLoja, DateTime inicio, DateTime fim)
        {
            var tipoMercadoria = new List<Glass.Data.Model.TipoMercadoria>() 
            {
                Glass.Data.Model.TipoMercadoria.MercadoriaRevenda,
                Glass.Data.Model.TipoMercadoria.MateriaPrima,
                Glass.Data.Model.TipoMercadoria.Embalagem,
                Glass.Data.Model.TipoMercadoria.ProdutoEmProcesso,
                Glass.Data.Model.TipoMercadoria.ProdutoAcabado,
                Glass.Data.Model.TipoMercadoria.Subproduto,
                Glass.Data.Model.TipoMercadoria.ProdutoIntermediario,
                Glass.Data.Model.TipoMercadoria.OutrosInsumos
            };

            var dados = GetForRptTotal(0, (uint)idLoja, null, null, null, null, null, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), 0, 0, 0, "", "", 0, 0, 0, true, false, true, false, true, tipoMercadoria);

            return dados;
        }
    }
}
