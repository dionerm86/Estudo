using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using Glass.Data.NFeUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ProdutoLojaDAO : BaseDAO<ProdutoLoja, ProdutoLojaDAO>
    {
        //private ProdutoLojaDAO() { }

        #region Busca padrão

        private string Sql(uint idLoja, uint idProd, bool forEFD, bool selecionar)
        {
            string campos = selecionar ? @"pl.*, p.Descricao as DescrProduto, p.IdGrupoProd, p.IdSubgrupoProd,
                g.Descricao as DescrGrupoProd, u.codigo as unidadeProd, pcc.codInterno as codInternoContaContabil,
                p.idContaContabil, c.nome as nomeCliente, f.nomeFantasia as nomeFornec, t.nome as nomeTransportador,
                l.nomeFantasia as nomeLoja, a.nome as nomeAdminCartao" : "Count(*)";

            if (selecionar && !forEFD)
                campos += ", pc.totMComprando, pc.qtdeComprando, pped.totMProduzindo, pped.qtdeProduzindo";

            bool agruparEstoqueLoja = idLoja > 0;
            string sql = "Select " + campos + @" From produto_loja pl
                Left Join produto p On (pl.idProd=p.idProd)
                Left Join plano_conta_contabil pcc On (p.idContaContabil=pcc.idContaContabil)
                Left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                Left join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd)
                Left join unidade_medida u On (p.idUnidadeMedida=u.idUnidadeMedida)
                Left join cliente c on (pl.id_Cli=c.id_Cli)
                Left join fornecedor f on (pl.idFornec=f.idFornec)
                Left join transportador t on (pl.idTransportador=t.idTransportador)
                Left join loja l on (pl.idLojaTerc=l.idLoja)
                Left join administradora_cartao a on (pl.idAdminCartao=a.idAdminCartao)
                " + (!forEFD ? ProdutoDAO.Instance.SqlPendenteCompra("p", agruparEstoqueLoja ? "pl" : null) : "") + @"
                " + (!forEFD ? ProdutoDAO.Instance.SqlPendenteProducao("p", agruparEstoqueLoja ? "pl" : null, null) : "") + @"
                Where 1";

            if (idLoja > 0)
                sql += " and pl.idLoja=" + idLoja;

            if (idProd > 0)
                sql += " and pl.idProd=" + idProd;

            return sql;
        }

        public IList<ProdutoLoja> GetList(uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idLoja, 0, false, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idLoja, 0, false, false), null);
        }

        public ProdutoLoja GetElement(uint idLoja, uint idProd)
        {
            return GetElement(null, idLoja, idProd);
        }

        public ProdutoLoja GetElement(GDASession session, uint idLoja, uint idProd)
        {
            return GetElement(session, idLoja, idProd, false);
        }

        public ProdutoLoja GetElement(uint idLoja, uint idProd, bool forEFD)
        {
            return GetElement(null, idLoja, idProd, forEFD);
        }

        public ProdutoLoja GetElement(GDASession session, uint idLoja, uint idProd, bool forEFD)
        {
            List<ProdutoLoja> item = objPersistence.LoadData(session, Sql(idLoja, idProd, forEFD, true));
            return item.Count > 0 ? item[0] : null;
        }

        #endregion

        #region Novo ProdutoLoja

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o produto existe, se não existir, insere
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        public void NewProd(int idProd, int idLoja)
        {
            NewProd(null, idProd, idLoja);
        }

        /// <summary>
        /// Verifica se o produto existe, se não existir, insere
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        public void NewProd(GDASession sessao, int idProd, int idLoja)
        {
            // Se o produtos ainda não estiver vinculado com a loja, faz o vínculo
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produto_loja where idProd=" + idProd + " And idLoja=" + idLoja) <= 0)
            {
                ProdutoLoja prodLoja = new ProdutoLoja();
                prodLoja.EstMinimo = 0;
                prodLoja.IdLoja = idLoja;
                prodLoja.IdProd = idProd;
                prodLoja.M2 = 0;
                prodLoja.QtdEstoque = 0;
                prodLoja.Reserva = 0;
                prodLoja.EstoqueFiscal = 0;
                prodLoja.Liberacao = 0;

                Insert(sessao, prodLoja);
            }
        }

        #endregion

        #region Busca para Estoque inicial

        private string SqlEstoque(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd, string idsSubgrupoProd,
            bool exibirApenasComEstoque, bool exibirApenasPosseTerceiros, bool exibirApenasProdutosProjeto, uint? idCorVidro,
            uint? idCorFerragem, uint? idCorAluminio, int situacao, bool estoqueFiscal, bool aguardandoSaidaEstoque, bool selecionar)
        {
            string tipoCalculo = String.Format("coalesce({0}sg.tipoCalculo, g.tipoCalculo)",
                estoqueFiscal ? "sg.tipoCalculoNf, g.tipoCalculoNf, " : "");

            string tipoCalcMLAL = (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6;

            string campos = selecionar ? "p.IdProd as IdProd, " + idLoja + @" as IdLoja, coalesce(sum(pl.QtdEstoque), 0) as QtdEstoque,
                coalesce(sum(pl.EstMinimo), 0) as EstMinimo, coalesce(sum(pl.Liberacao), 0) as Liberacao, coalesce(sum(pl.Reserva), 0) as Reserva,
                coalesce(sum(pl.M2), 0) as M2, coalesce(sum(pl.EstoqueFiscal), 0) as EstoqueFiscal, p.IdGrupoProd, p.IdSubgrupoProd,
                p.Descricao as DescrProduto, p.CodInterno as CodInternoProd, g.Descricao as DescrGrupoProd,
                sg.Descricao as DescrSubgrupoProd, p.Ncm as NcmProd, p.situacao as Situacao, um.codigo as UnidadeProd, '$$$' as Criterio,
                " + tipoCalculo + @" as tipoCalc, (mef.valorUnit/if(" + tipoCalculo + " in (" + tipoCalcMLAL + @"), 6, 1)) as ValorUnitProd,
                (p.custoCompra/if(" + tipoCalculo + " in (" + tipoCalcMLAL + @"), 6, 1)) as CustoUnitProd, coalesce(sum(pl.defeito),0) as defeito,
                sum(pl.qtdePosseTerceiros) as qtdePosseTerceiros, pl.id_Cli, pl.idFornec, pl.idLojaTerc, pl.idTransportador,
                pl.idAdminCartao, c.nome as nomeCliente, f.nomeFantasia as nomeFornec, t.nome as nomeTransportador,
                l.nomeFantasia as nomeLoja, a.nome as nomeAdminCartao, p.CustoFabBase as CustoProd, p.ValorAtacado as ValorProd" : "Count(*)";

            string sql = @"
                Select " + campos + @" From produto p
                    Left join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida)
                    Left join produto_loja pl on (p.IdProd=pl.idProd)
                    Left Join (
                        select idProd, idLoja, if(saldoQtdeMov>0 and saldoValorMov>0, saldoValorMov/saldoQtdeMov,
                            if(qtdeMov>0, valorMov/qtdeMov, 0)) as valorUnit
                        from (
                            select * from mov_estoque_fiscal
                            order by dataMov desc, idMovEstoqueFiscal desc
                        ) as temp
                        group by idProd, idLoja
                    ) mef On (pl.idProd=mef.idProd and pl.idLoja=mef.idLoja)
                    Left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd)
                    Left join cliente c on (pl.id_Cli=c.id_Cli)
                    Left join fornecedor f on (pl.idFornec=f.idFornec)
                    Left join transportador t on (pl.idTransportador=t.idTransportador)
                    Left join loja l on (pl.idLojaTerc=l.idLoja)
                    Left join administradora_cartao a on (pl.idAdminCartao=a.idAdminCartao)
                Where 1 ";

            if (idLoja > 0)
                sql += " And pl.idLoja=" + idLoja;

            string criterio = "";

            if (!String.IsNullOrEmpty(codInternoProd))
            {
                sql += " and p.CodInterno=?codInterno";
                criterio += "Cód. Produto: " + codInternoProd + "    ";
            }
            else if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                sql += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descricao + "    ";
            }

            if (idGrupoProd > 0)
            {
                sql += " and p.IdGrupoProd=" + idGrupoProd;
                criterio += "Grupo Prod.: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd))
            {
                sql += $" and p.IdSubgrupoProd IN ({idsSubgrupoProd})";
                var subgrupos = string.Empty;
                foreach (var id in idsSubgrupoProd.Split(','))
                {
                    subgrupos += SubgrupoProdDAO.Instance.GetDescricao(null, id.StrParaInt()) + ", ";
                }

                criterio += "Subgrupo(s): " + subgrupos.TrimEnd(' ', ',') + "    ";
            }

            if (exibirApenasComEstoque)
            {
                sql += estoqueFiscal ? " and pl.EstoqueFiscal>0" : " and pl.QtdEstoque>0";
                criterio += "Apenas produtos com estoque " + (estoqueFiscal ? "fiscal" : "") + "    ";
            }

            if (exibirApenasPosseTerceiros)
            {
                sql += " and pl.qtdePosseTerceiros>0";
                criterio += "Apenas produtos em posse de terceiros    ";
            }

            if (exibirApenasProdutosProjeto)
            {
                string idsProdProjConfig = ProdutoProjetoConfigDAO.Instance.GetValoresCampo("Select idProd From produto_projeto_config", "idProd");
                sql += " And pl.idProd In (" + idsProdProjConfig + ")";
                criterio += "Apenas produtos associados a projetos    ";
            }

            if (aguardandoSaidaEstoque)
            {
                sql += PedidoConfig.LiberarPedido ? " and pl.liberacao>0" : " and pl.reserva>0";
                criterio += "Produtos aguardando saída de estoque    ";
            }

            if (idCorVidro > 0)
            {
                sql += " and p.idCorVidro=" + idCorVidro;
                criterio += "Cor vidro: " + CorVidroDAO.Instance.GetNome(idCorVidro.Value) + "    ";
            }

            if (idCorFerragem > 0)
            {
                sql += " and p.idCorFerragem=" + idCorFerragem;
                criterio += "Cor ferragem: " + CorFerragemDAO.Instance.GetNome(idCorFerragem.Value) + "    ";
            }

            if (idCorAluminio > 0)
            {
                sql += " and p.idCorAluminio=" + idCorAluminio;
                criterio += "Cor alumínio: " + CorAluminioDAO.Instance.GetNome(idCorAluminio.Value) + "    ";
            }

            if (situacao > 0)
            {
                sql += " and p.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Ativo" : "Inativo") + "    ";
            }

            sql += " group by p.idProd";

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] EstoqueParam(string codInternoProd, string descricao, string tipoBox)
        {
            GDAParameter[] p = new GDAParameter[3];
            p[0] = new GDAParameter("?codInterno", codInternoProd);
            p[1] = new GDAParameter("?descricao", "%" + descricao + "%");
            p[2] = new GDAParameter("?tipoBox", "%" + tipoBox + "%");

            return p;
        }

        public IList<ProdutoLoja> GetForEstoque(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd,
            string idsSubgrupoProd, bool exibirApenasComEstoque, bool exibirApenasPosseTerceiros, bool exibirApenasProdutosProjeto, uint? idCorVidro, uint? idCorFerragem,
            uint? idCorAluminio, int situacao, int estoqueFiscal, bool aguardandoSaidaEstoque, int ordenacao, string sortExpression, int startRow,
            int pageSize)
        {
            string order = String.Empty;

            switch (ordenacao)
            {
                case 1:
                    order = "p.CodInterno asc"; break;
                case 2:
                    order = estoqueFiscal == 1 ? "coalesce(sum(pl.EstoqueFiscal), 0) asc" : "coalesce(sum(pl.QtdEstoque), 0) asc"; break;
                case 3:
                    order = estoqueFiscal == 1 ? "coalesce(sum(pl.EstoqueFiscal), 0) desc" : "coalesce(sum(pl.QtdEstoque), 0) desc"; break;
            }

            sortExpression = string.IsNullOrEmpty(sortExpression) ? order : sortExpression;
            return objPersistence.LoadDataWithSortExpression(SqlEstoque(idLoja, codInternoProd, descricao, idGrupoProd, idsSubgrupoProd,
                exibirApenasComEstoque, exibirApenasPosseTerceiros, exibirApenasProdutosProjeto, idCorVidro, idCorFerragem, idCorAluminio, situacao,
                estoqueFiscal == 1, aguardandoSaidaEstoque, true) + $" ORDER BY {sortExpression}", null, new InfoPaging(startRow, pageSize), EstoqueParam(codInternoProd, descricao, null)).ToList();
        }

        public int GetForEstoqueCount(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd, string idsSubgrupoProd,
            bool exibirApenasComEstoque, bool exibirApenasPosseTerceiros, bool exibirApenasProdutosProjeto, uint? idCorVidro,
            uint? idCorFerragem, uint? idCorAluminio, int situacao, int estoqueFiscal, bool aguardandoSaidaEstoque, int ordenacao)
        {
            int i = objPersistence.ExecuteSqlQueryCount(SqlEstoque(idLoja, codInternoProd, descricao, idGrupoProd, idsSubgrupoProd,
                exibirApenasComEstoque, exibirApenasPosseTerceiros, exibirApenasProdutosProjeto, idCorVidro, idCorFerragem, idCorAluminio,
                situacao, estoqueFiscal == 1, aguardandoSaidaEstoque, false), EstoqueParam(codInternoProd, descricao, null));

            return i;
        }

        public ProdutoLoja[] GetForRptEstoque(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd,
            string idsSubgrupoProd, bool exibirApenasComEstoque, bool exibirApenasPosseTerceiros, bool exibirApenasProdutosProjeto,
            uint? idCorVidro, uint? idCorFerragem, uint? idCorAluminio, int situacao, int estoqueFiscal, bool aguardandoSaidaEstoque, int ordenacao)
        {
            string order = String.Empty;

            switch (ordenacao)
            {
                case 1:
                    order = " Order by p.Descricao asc"; break;
                case 2:
                    order = estoqueFiscal == 1 ? " Order by coalesce(sum(pl.EstoqueFiscal), 0) asc" : " Order by coalesce(sum(pl.QtdEstoque), 0) asc"; break;
                case 3:
                    order = estoqueFiscal == 1 ? " Order by coalesce(sum(pl.EstoqueFiscal), 0) desc" : " Order by coalesce(sum(pl.QtdEstoque), 0) desc"; break;
            }

            return objPersistence.LoadData(SqlEstoque(idLoja, codInternoProd, descricao, idGrupoProd, idsSubgrupoProd,
                exibirApenasComEstoque, exibirApenasPosseTerceiros, exibirApenasProdutosProjeto, idCorVidro, idCorFerragem, idCorAluminio, situacao, estoqueFiscal == 1,
                aguardandoSaidaEstoque, true) + order, EstoqueParam(codInternoProd, descricao, null)).ToList().ToArray();
        }

        #endregion

        #region Busca para Estoque mínimo

        private string SqlEstoqueMin(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd, uint idSubgrupoProd,
            bool abaixoEstMin, uint? idCorVidro, uint? idCorFerragem, uint? idCorAluminio, string tipoBox, bool selecionar)
        {
            string campos = selecionar ? "p.IdProd as IdProd, " + idLoja + @" as IdLoja, coalesce(pl.QtdEstoque, 0) as QtdEstoque,
                coalesce(pl.EstMinimo, 0) as EstMinimo, coalesce(pl.Reserva, 0) as Reserva, coalesce(pl.M2, 0) as M2,
                coalesce(pl.EstoqueFiscal, 0) as EstoqueFiscal, p.IdGrupoProd, p.IdSubgrupoProd,
                p.Descricao as DescrProduto, p.CodInterno as CodInternoProd, g.Descricao as DescrGrupoProd,
                sg.Descricao as DescrSubgrupoProd, coalesce(pl.Liberacao, 0) as Liberacao, coalesce(pl.defeito,0) as defeito,
                null as QTDEPOSSETERCEIROS, null as id_Cli, null as idFornec, null as idTransportador, null as idLojaTerc,
                null as idAdminCartao, pc.totMComprando, pc.qtdeComprando, pped.totMProduzindo, pped.qtdeProduzindo" : "Count(*)";

            bool agruparEstoqueLoja = idLoja > 0;
            string sql = @"
                Select " + campos + @" From produto p
                    Left join (select * from produto_loja Where idLoja=" + idLoja + @") pl on (p.IdProd=pl.idProd)
                    Left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd)
                    " + ProdutoDAO.Instance.SqlPendenteCompra("p", agruparEstoqueLoja ? "pl" : null) + @"
                    " + ProdutoDAO.Instance.SqlPendenteProducao("p", agruparEstoqueLoja ? "pl" : null, null) + @"
                Where p.situacao=" + (int)Situacao.Ativo + " And Coalesce(pl.EstMinimo, 0)>0 ";

            if (abaixoEstMin)
                sql += @" And (pl.estMinimo=0 Or if(coalesce(sg.tipoCalculo, g.tipoCalculo)
                    in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + ", " + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                    pl.m2 + coalesce(pped.qtdeProduzindo,0) < pl.estMinimo, pl.qtdEstoque + coalesce(pped.qtdeProduzindo,0) < pl.estMinimo))";

            if (!String.IsNullOrEmpty(codInternoProd))
                sql += " and p.CodInterno=?codInterno";

            if (!String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                sql += " And p.idProd In (" + ids + ")";
            }

            if (idGrupoProd > 0)
                sql += " and p.IdGrupoProd=" + idGrupoProd;

            if (idSubgrupoProd > 0)
                sql += " and p.IdSubgrupoProd=" + idSubgrupoProd;

            if (idCorVidro > 0)
                sql += " and p.idCorVidro=" + idCorVidro;

            if (idCorFerragem > 0)
                sql += " and p.idCorFerragem=" + idCorFerragem;

            if (idCorAluminio > 0)
                sql += " and p.idCorAluminio=" + idCorAluminio;

            if (!String.IsNullOrEmpty(tipoBox))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, tipoBox);
                sql += " And p.idProd In (" + ids + ")";
            }

            return sql;
        }

        public ProdutoLoja[] GetEstoqueMinRpt(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd,
            uint idSubgrupoProd, bool abaixoEstMin, uint? idCorVidro, uint? idCorFerragem, uint? idCorAluminio, string tipoBox)
        {
            string sort = " order by p.CodInterno asc";
            return objPersistence.LoadData(SqlEstoqueMin(idLoja, codInternoProd, descricao, idGrupoProd, idSubgrupoProd, abaixoEstMin,
                idCorVidro, idCorFerragem, idCorAluminio, tipoBox, true) + sort, EstoqueParam(codInternoProd, descricao, tipoBox)).ToList().ToArray();
        }

        public IList<ProdutoLoja> GetForEstoqueMin(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd,
            uint idSubgrupoProd, bool abaixoEstMin, uint? idCorVidro, uint? idCorFerragem, uint? idCorAluminio, string tipoBox,
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "p.Descricao asc" : sortExpression;
            return LoadDataWithSortExpression(SqlEstoqueMin(idLoja, codInternoProd, descricao, idGrupoProd, idSubgrupoProd, abaixoEstMin,
                idCorVidro, idCorFerragem, idCorAluminio, tipoBox, true), sortExpression, startRow, pageSize, EstoqueParam(codInternoProd, descricao, tipoBox));
        }

        public int GetForEstoqueMinCount(uint idLoja, string codInternoProd, string descricao, uint idGrupoProd, uint idSubgrupoProd,
            bool abaixoEstMin, uint? idCorVidro, uint? idCorFerragem, uint? idCorAluminio, string tipoBox)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlEstoqueMin(idLoja, codInternoProd, descricao, idGrupoProd, idSubgrupoProd,
                abaixoEstMin, idCorVidro, idCorFerragem, idCorAluminio, tipoBox, false), EstoqueParam(codInternoProd, descricao, tipoBox));
        }

        #endregion

        #region Retorna a quantidade em estoque

        /// <summary>
        /// Retorna a quantidade em estoque do produto passado na loja passada, considerando o que estiver na reserva e na liberação
        /// </summary>
        public float GetEstoque(GDASession sessao, uint idLoja, uint idProd)
        {
            return GetEstoque(sessao, idLoja, idProd, null, false, false, true);
        }

        /// <summary>
        /// Retorna a quantidade em estoque do produto passado na loja passada, considerando o que estiver na reserva e na liberação
        /// </summary>
        public float GetEstoque(uint idLoja, uint idProd, bool isPedidoProducao)
        {
            return GetEstoque(null, idLoja, idProd, isPedidoProducao);
        }

        public float GetEstoque(GDASession sessao, uint idLoja, uint idProd, bool isPedidoProducao)
        {
            return GetEstoque(sessao, idLoja, idProd, null, isPedidoProducao, false, true);
        }

        /// <summary>
        /// Retorna a quantidade em estoque do produto passado na loja passada, considerando o que estiver na reserva e na liberação
        /// </summary>
        public float GetEstoque(GDASession sessao, uint idLoja, uint idProd, uint? idPedidoIgnorar, bool isPedidoProducao, bool estoqueReal, bool considerarProdBaixa)
        {
            if (idProd == 0 && idLoja == 0)
                return 0f;

            string tipoCalcM2 = "(select coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                from produto p inner join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                left join subgrupo_prod s on (p.idSubgrupoProd=s.idSubgrupoProd) where p.idProd={0}.idProd)
                in (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ")";

            string sqlPedidoIgnorar = idPedidoIgnorar == null || idPedidoIgnorar == 0 ? "" :
                "+coalesce((select sum(if(" + String.Format(tipoCalcM2, "pp") + @", totM, qtde)) from produtos_pedido pp
                where idProd=pl.idProd and coalesce(invisivelPedido,false)=false and idPedido=" + idPedidoIgnorar + "), 0)";

            string sql = "Select round(Coalesce(QtdEstoque, 0) " +
                (!estoqueReal ? "- Coalesce(Reserva, 0)" + (PedidoConfig.LiberarPedido ? " - Coalesce(Liberacao, 0)" : "") + sqlPedidoIgnorar : String.Empty) +
                ", 2), idProd From produto_loja pl Where 1";

            if (idLoja > 0)
                sql += " and idLoja=" + idLoja;

            // Verifica se é necessário retornar o estoque da matéria-prima
            // Retorna o menor valor em estoque dentre as matérias-primas configuradas
            if (considerarProdBaixa && idProd > 0 && ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(sessao, idProd) &&
                ((isPedidoProducao && SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)idProd)) || (!isPedidoProducao && !SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)idProd))))
            {
                var produtosBaixa = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, idProd);
                sql += $" and idProd in ({string.Join(",", produtosBaixa.Select(pbe => pbe.IdProdBaixa))})";

                var estoque = this.objPersistence.LoadResult(sessao, sql)
                    .Select(resultado => new
                    {
                        Estoque = resultado.GetFloat(0),
                        IdProd = resultado.GetInt32(1),
                    })
                    .Select(item =>
                    {
                        var produtoBaixa = produtosBaixa.FirstOrDefault(pbe => pbe.IdProdBaixa == item.IdProd)
                            ?? new ProdutoBaixaEstoque();

                        return produtoBaixa.Qtde > 0
                            ? (float)Math.Round(item.Estoque / produtoBaixa.Qtde, 2)
                            : 0;
                    })
                    .ToList();

                return estoque.Min();
            }
            else
            {
                // Retorna o valor em estoque do produto
                if (idProd > 0)
                    sql += " and idProd=" + idProd;

                return ExecuteScalar<float>(sessao, sql);
            }
        }

        /// <summary>
        /// Retorna a quantidade em estoque do produto passado na loja passada, considerando o que estiver na reserva e na liberação
        /// </summary>
        public float GetEstoque(uint idLoja, uint idProd, uint? idPedidoIgnorar, bool isPedidoProducao, bool estoqueReal, bool considerarProdBaixa)
        {
            return GetEstoque(null, idLoja, idProd, idPedidoIgnorar, isPedidoProducao, estoqueReal, considerarProdBaixa);
        }

        /// <summary>
        /// Retorna o estoque mínimo do produto passado na loja passada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int GetEstoqueMin(uint idLoja, uint idProd)
        {
            string sql = "Select Coalesce(EstMinimo, 0) From produto_loja Where idProd=" + idProd + " And idLoja=" + idLoja;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return (int)Single.Parse(obj.ToString().Replace('.', ','));
        }

        #endregion

        #region Atualiza reserva/liberação

        #region Credita estoque reserva

        /// <summary>
        /// Credita a quantidade em reserva do produto na loja informada.
        /// </summary>
        public void ColocarReserva(GDASession sessao, int idLoja, Dictionary<int, float> idsProdQtde, int? idSaidaEstoque, int? idLiberarPedido,
            int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            foreach (var idLojaSistema in LojaDAO.Instance.GetIdsLojasAtivas())
            {
                AtualizaReservaLiberacao(sessao, (int)idLojaSistema, idsProdQtde, idSaidaEstoque, idLiberarPedido, idPedidoEspelho, idProdPedProducao,
                    idPedido, idsPedido, idProdPed, classeMetodo, false, true);
            }
        }

        #endregion

        #region Baixa estoque reserva

        /// <summary>
        /// Baixa a quantidade em reserva do produto na loja informada.
        /// </summary>
        public void TirarReserva(GDASession sessao, int idLoja, Dictionary<int, float> idsProdQtde, int? idSaidaEstoque, int? idLiberarPedido,
            int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            foreach (var idLojaSistema in LojaDAO.Instance.GetIdsLojasAtivas())
            {
                AtualizaReservaLiberacao(sessao, (int)idLojaSistema, idsProdQtde, idSaidaEstoque, idLiberarPedido, idPedidoEspelho, idProdPedProducao,
                    idPedido, idsPedido, idProdPed, classeMetodo, false, false);
            }
        }

        #endregion

        #region Credita estoque liberação

        /// <summary>
        /// Credita a quantidade em liberação do produto na loja informada.
        /// </summary>
        public void ColocarLiberacao(GDASession sessao, int idLoja, Dictionary<int, float> idsProdQtde, int? idSaidaEstoque, int? idLiberarPedido,
            int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            foreach (var idLojaSistema in LojaDAO.Instance.GetIdsLojasAtivas())
            {
                AtualizaReservaLiberacao(sessao, (int)idLojaSistema, idsProdQtde, idSaidaEstoque, idLiberarPedido, idPedidoEspelho, idProdPedProducao,
                    idPedido, idsPedido, idProdPed, classeMetodo, true, true);
            }
        }

        #endregion

        #region Baixa estoque liberação

        /// <summary>
        /// Baixa a quantidade em liberação do produto na loja informada.
        /// </summary>
        public void TirarLiberacao(GDASession sessao, int idLoja, Dictionary<int, float> idsProdQtde, int? idSaidaEstoque, int? idLiberarPedido,
            int? idPedidoEspelho, int? idProdPedProducao, int? idPedido, string idsPedido, int? idProdPed, string classeMetodo)
        {
            foreach (var idLojaSistema in LojaDAO.Instance.GetIdsLojasAtivas())
            {
                AtualizaReservaLiberacao(sessao, (int)idLojaSistema, idsProdQtde, idSaidaEstoque, idLiberarPedido, idPedidoEspelho, idProdPedProducao,
                    idPedido, idsPedido, idProdPed, classeMetodo, true, false);
            }
        }

        #endregion

        static volatile object _atualizarReservaLiberacaoLock = new object();

        /// <summary>
        /// Credita/Baixa a quantidade do produto na loja informada.
        /// </summary>
        private void AtualizaReservaLiberacao(GDASession sessao, int idLoja, Dictionary<int, float> idsProdQtde, int? idSaidaEstoque,
            int? idLiberarPedido, int? idPedidoEspelho, int? idProdPedProducao, int? idPedidoParam, string idsPedido, int? idProdPedParam,
            string classeMetodo, bool atualizarLiberacao, bool creditar)
        {
            lock (_atualizarReservaLiberacaoLock)
            {
                var lstTipoCalculo = new List<int> {
                    (int)TipoCalculoGrupoProd.Qtd,
                    (int)TipoCalculoGrupoProd.QtdDecimal,
                };

                // Controla reserva/liberação apenas de produtos calculados por QTD e QTD Decimal
                var idsProd = idsProdQtde.Keys
                    .Distinct()
                    .Where(f => lstTipoCalculo.Contains(GrupoProdDAO.Instance.TipoCalculo(sessao, f)))
                    .ToList();

                if (idsProd.Count == 0)
                {
                    return;
                }

                var invisivelFluxoPedido = PCPConfig.UsarConferenciaFluxo ? "InvisivelFluxo" : "InvisivelPedido";

                // Atualiza a reserva e liberação de cada produto
                foreach (var idProd in idsProd)
                {
                    // Cria um registro na tabela produto_loja caso não exista.
                    NewProd(sessao, idProd, idLoja);

                    var reserva = objPersistence.ExecuteScalar(sessao,
                        $@"SELECT COALESCE(SUM(Qtde-Qtdsaida), 0)
	                    FROM produtos_pedido pp
		                    LEFT JOIN pedido p ON (pp.IdPedido=p.IdPedido)
	                    WHERE pp.Qtde<>pp.QtdSaida
                            AND (pp.{ invisivelFluxoPedido } IS NULL OR pp.{ invisivelFluxoPedido }=0)
                            AND p.Situacao IN ({(PedidoConfig.LiberarPedido ? $"{(int)Pedido.SituacaoPedido.ConfirmadoLiberacao},{(int)Pedido.SituacaoPedido.LiberadoParcialmente}" : ((int)Pedido.SituacaoPedido.Confirmado).ToString())})
                            AND p.TipoPedido<>{ (int)Pedido.TipoPedidoEnum.Producao }
		                    AND pp.IdProd={ idProd }
                            { (idLoja > 0 ? string.Format("AND IdLoja={0}", idLoja) : string.Empty) };");

                    // Atualiza a coluna RESERVA
                    objPersistence.ExecuteCommand(sessao, string.Format(
                        $@"UPDATE produto_loja SET
                            Reserva=?reserva
                        WHERE IdProd={ idProd }
                            { (idLoja > 0 ? string.Format("AND IdLoja={0}", idLoja) : string.Empty) };"),
                        new GDAParameter("?reserva", reserva));

                    // Atualiza a coluna LIBERACAO
                    if (PedidoConfig.LiberarPedido && atualizarLiberacao)
                    {
                        var liberacao = objPersistence.ExecuteScalar(sessao,
                            $@"SELECT COALESCE(SUM(Qtde-Qtdsaida), 0)
	                        FROM produtos_pedido pp
		                        LEFT JOIN pedido p ON (pp.IdPedido=p.IdPedido)
	                        WHERE pp.Qtde<>pp.QtdSaida
                                AND (pp.{ invisivelFluxoPedido } IS NULL OR pp.{ invisivelFluxoPedido }=0)
		                        AND p.Situacao IN ({ (int)Pedido.SituacaoPedido.Confirmado }, { (int)Pedido.SituacaoPedido.LiberadoParcialmente })
                                AND p.SituacaoProducao<>{ (int)Pedido.SituacaoProducaoEnum.Entregue }
                                AND p.TipoPedido<>{ (int)Pedido.TipoPedidoEnum.Producao }
		                        AND pp.IdProd={ idProd }
                                { (idLoja > 0 ? string.Format("AND IdLoja={0}", idLoja) : string.Empty) };");

                        objPersistence.ExecuteCommand(sessao,
                            $@"UPDATE produto_loja SET
                                Liberacao=?liberacao
                            WHERE IdProd={ idProd }
                                { (idLoja > 0 ? string.Format("AND IdLoja={0}", idLoja) : string.Empty) }",
                            new GDAParameter("?liberacao", liberacao));
                    }
                }
            }
        }

        #endregion

        #region Estoque Fiscal/Terceiros

        /// <summary>
        /// Busca a quantidade existente no estoque fiscal do produto passado na loja passada
        /// </summary>
        public float GetEstoqueFiscal(uint idProd, uint idLoja)
        {
            return GetEstoqueFiscal(null, idProd, idLoja);
        }

        /// <summary>
        /// Busca a quantidade existente no estoque fiscal do produto passado na loja passada
        /// </summary>
        public float GetEstoqueFiscal(GDASession session, uint idProd, uint idLoja)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)idProd);

            if (GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(session, idGrupoProd, idSubgrupoProd) || ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
                return 100000f;

            var prodBaixaEstFisc = ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd(session, idProd);
            var qtdePosseterceiros = 0.0F;

            foreach (var pbef in prodBaixaEstFisc)
                qtdePosseterceiros += ExecuteScalar<float>(session, "Select Round(Coalesce(EstoqueFiscal, 0), 2) From produto_loja Where idProd=" +
                    idProd + " And idLoja=" + idLoja);

            return qtdePosseterceiros;
        }

        /// <summary>
        /// Obtém a quantidade do produtos passado em posse de terceiros
        /// </summary>
        public float ObtemEstoqueTerceiros(uint idProd, uint idLoja)
        {
            return ObtemEstoqueTerceiros(null, idProd, idLoja);
        }

        /// <summary>
        /// Obtém a quantidade do produtos passado em posse de terceiros
        /// </summary>
        public float ObtemEstoqueTerceiros(GDASession session, uint idProd, uint idLoja)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)idProd);

            if (GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(session, idGrupoProd, idSubgrupoProd) || ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
                return 100000f;

            var prodBaixaEstFisc = ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd(session, idProd);
            var qtdePosseterceiros = 0.0F;

            foreach (var pbef in prodBaixaEstFisc)
                qtdePosseterceiros += ExecuteScalar<float>(session, "Select Round(Coalesce(QtdePosseTerceiros, 0), 2) From produto_loja Where idProd=" +
                    pbef.IdProdBaixa + " And idLoja=" + idLoja);

            return qtdePosseterceiros;
        }

        #endregion

        #region Defeito

        /// <summary>
        /// Dá baixa no estoque de defeito no produto da loja passados
        /// </summary>
        public int BaixaDefeito(uint idProd, uint idLoja, Single qtdeBaixa)
        {
            return BaixaDefeito(null, idProd, idLoja, qtdeBaixa);
        }

        /// <summary>
        /// Dá baixa no estoque de defeito no produto da loja passados
        /// </summary>
        public int BaixaDefeito(GDASession session, uint idProd, uint idLoja, Single qtdeBaixa)
        {
            try
            {
                NewProd(session, (int)idProd, (int)idLoja);
            }
            catch { }

            var sql = "Update produto_loja Set Defeito=Coalesce(Defeito, 0)-" + qtdeBaixa.ToString().Replace(',', '.') +
                " Where idProd=" + idProd + " And idLoja=" + idLoja;

            return objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Credita quantidade do produto passado ao estoque de defeito da loja
        /// </summary>
        public int CreditaDefeito(uint idProd, uint idLoja, Single qtdeProd)
        {
            return CreditaDefeito(null, idProd, idLoja, qtdeProd);
        }

        /// <summary>
        /// Credita quantidade do produto passado ao estoque de defeito da loja
        /// </summary>
        public int CreditaDefeito(GDASession session, uint idProd, uint idLoja, Single qtdeProd)
        {
            NewProd(session, (int)idProd, (int)idLoja);

            string sql = "Update produto_loja Set Defeito=Coalesce(Defeito, 0)+" + qtdeProd.ToString().Replace(',', '.') +
                " Where idProd=" + idProd + " And idLoja=" + idLoja;

            return objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Atualiza estoque

        public int AtualizaEstoque(ProdutoLoja objUpdate)
        {
            return AtualizaEstoque(null, objUpdate);
        }

        /// <summary>
        /// Método usado apenas na tela de Lançamento inicial de estoque (LstEstoque.aspx)
        /// </summary>
        public int AtualizaEstoque(GDASession sessao, ProdutoLoja objUpdate)
        {
            int retorno;
            ProdutoLoja atual;

            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, "select coalesce(Count(*), 0) from produto_loja where idLoja=" + objUpdate.IdLoja + " and idProd=" + objUpdate.IdProd).ToString()) > 0)
            {
                atual = GetElement(sessao, (uint)objUpdate.IdLoja, (uint)objUpdate.IdProd);

                // Não atualiza o estoque/estoque fiscal, o mesmo será modificado na função da MovEstoqueDAO logo abaixo
                string sql = "Update produto_loja Set estMinimo=" + objUpdate.EstMinimo.ToString().Replace(",", ".") +
                    ", M2=" + objUpdate.M2.ToString().Replace(',', '.') +
                    /* ", EstoqueFiscal=" + objUpdate.EstoqueFiscal.ToString().Replace(",", ".") +
                    ", QtdEstoque=" + objUpdate.QtdEstoque.ToString().Replace(",", ".") +
                    ", Reserva=" + objUpdate.Reserva.ToString().Replace(',', '.') +
                    ", Liberacao=" + objUpdate.Liberacao.ToString().Replace(',', '.') +*/
                    ", QtdePosseTerceiros=" + objUpdate.QtdePosseTerceiros.ToString().Replace(',', '.') +
                    ", Defeito=" + objUpdate.Defeito.ToString().Replace(',', '.') +
                    ", Id_Cli=" + (objUpdate.IdCliente != null ? objUpdate.IdCliente.ToString() : "null") +
                    ", IdFornec=" + (objUpdate.IdFornec != null ? objUpdate.IdFornec.ToString() : "null") +
                    ", IdLojaTerc=" + (objUpdate.IdLojaTerc != null ? objUpdate.IdLojaTerc.ToString() : "null") +
                    ", IdTransportador=" + (objUpdate.IdTransportador != null ? objUpdate.IdTransportador.ToString() : "null") +
                    ", IdAdminCartao=" + (objUpdate.IdAdminCartao != null ? objUpdate.IdAdminCartao.ToString() : "null") +
                    " Where idLoja=" + objUpdate.IdLoja +
                    " and idProd=" + objUpdate.IdProd;

                retorno = objPersistence.ExecuteCommand(sessao, sql);
            }
            else
            {
                atual = new ProdutoLoja();
                atual.IdLoja = objUpdate.IdLoja;
                atual.IdProd = objUpdate.IdProd;
                retorno = (int)base.Insert(objUpdate);
            }

            var possuiMovReal = MovEstoqueDAO.Instance.VerificarIdProdIdLojaPossuiMovimentacao(sessao, objUpdate.IdProd, objUpdate.IdLoja);
            bool possuiMovFiscal = ExecuteScalar<bool>(sessao, "Select Count(*)>0 From mov_estoque_fiscal Where idProd=" + objUpdate.IdProd + " And idLoja=" + objUpdate.IdLoja);

            // Se a quantidade alterada for maior que a quantidade atual, gera uma movimentação de estoque creditando o estoque,
            // mas caso a quantidade modificada seja menor que a atual, gera uma movimentação de estoque debitando o estoque
            if (Math.Round(objUpdate.QtdEstoque, 2) > Math.Round(atual.QtdEstoque, 2) || (!possuiMovReal && objUpdate.QtdEstoque > 0))
            {
                decimal qtdMov = possuiMovReal ? (decimal)(objUpdate.QtdEstoque - atual.QtdEstoque) : (decimal)objUpdate.QtdEstoque;

                MovEstoqueDAO.Instance.CreditaEstoqueManual(sessao, (uint)objUpdate.IdProd, (uint)objUpdate.IdLoja,
                    qtdMov, null, DateTime.Now, null);
            }
            else if (Math.Round(objUpdate.QtdEstoque, 2) < Math.Round(atual.QtdEstoque, 2))
            {
                decimal qtdMov = possuiMovReal ? (decimal)(atual.QtdEstoque - objUpdate.QtdEstoque) : (decimal)objUpdate.QtdEstoque;

                MovEstoqueDAO.Instance.BaixaEstoqueManual(sessao, (uint)objUpdate.IdProd, (uint)objUpdate.IdLoja,
                    qtdMov, null, DateTime.Now, null);
            }

            // Se a quantidade alterada for maior que a quantidade atual, gera uma movimentação de estoque creditando o estoque,
            // mas caso a quantidade modificada seja menor que a atual, gera uma movimentação de estoque debitando o estoque
            if (Math.Round(objUpdate.EstoqueFiscal, 2) > Math.Round(atual.EstoqueFiscal, 2) || (!possuiMovFiscal && objUpdate.EstoqueFiscal > 0))
            {
                decimal qtdMov = possuiMovFiscal ? (decimal)(objUpdate.EstoqueFiscal - atual.EstoqueFiscal) : (decimal)objUpdate.EstoqueFiscal;

                MovEstoqueFiscalDAO.Instance.CreditaEstoqueManual(sessao, (uint)objUpdate.IdProd, (uint)objUpdate.IdLoja,
                    qtdMov, null, DateTime.Now, null);
            }
            else if (Math.Round(objUpdate.EstoqueFiscal, 2) < Math.Round(atual.EstoqueFiscal, 2))
            {
                decimal qtdMov = possuiMovFiscal ? (decimal)(atual.EstoqueFiscal - objUpdate.EstoqueFiscal) : (decimal)objUpdate.EstoqueFiscal;

                MovEstoqueFiscalDAO.Instance.BaixaEstoqueManual(sessao, (uint)objUpdate.IdProd, (uint)objUpdate.IdLoja, qtdMov, null, DateTime.Now, null);
            }

            LogAlteracaoDAO.Instance.LogProdutoLoja(sessao, atual);
            return retorno;
        }

        /// <summary>
        /// Método utilizado apenas na tela de Estoque Mínimo (ListaEstoqueMinimo.aspx) e ajuste de estoque mínimo
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <returns></returns>
        public int AtualizaEstoqueMinimo(ProdutoLoja objUpdate)
        {
            int retorno;
            ProdutoLoja atual;

            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("select coalesce(Count(*), 0) from produto_loja where idLoja=" + objUpdate.IdLoja + " and idProd=" + objUpdate.IdProd).ToString()) > 0)
            {
                atual = GetElement((uint)objUpdate.IdLoja, (uint)objUpdate.IdProd);
                retorno = objPersistence.ExecuteCommand("Update produto_loja Set estMinimo=" + objUpdate.EstMinimo + " Where idLoja=" + objUpdate.IdLoja + " and idProd=" + objUpdate.IdProd);
            }
            else
            {
                atual = new ProdutoLoja();
                atual.IdLoja = objUpdate.IdLoja;
                atual.IdProd = objUpdate.IdProd;
                retorno = (int)base.Insert(objUpdate);
            }

            LogAlteracaoDAO.Instance.LogProdutoLoja(atual);
            return retorno;
        }

        /// <summary>
        /// Método utlizado para limpar as quantidades em estoque dos produtos do grupo/subgrupo desejado.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <param name="idSubgrupoProd"></param>
        /// <returns></returns>
        public int ZeraEstoque(uint idLoja, uint idGrupoProd, string idsSubgrupoProd)
        {
            var where = "pl.idLoja=" + idLoja + " and p.idGrupoProd=" + idGrupoProd;
            if (!string.IsNullOrEmpty(idsSubgrupoProd))
                where += $" and p.idSubgrupoProd IN({idsSubgrupoProd})";

            var sql = "select pl.* from produto_loja pl left join produto p on (pl.idProd=p.idProd) where " + where;
            var prod = objPersistence.LoadData(sql).ToList().ToArray();

            // Limpa o estoque
            sql = "update produto_loja pl left join produto p on (pl.idProd=p.idProd) set qtdEstoque=0, m2=0 where " + where;
            var retorno = objPersistence.ExecuteCommand(sql);

            // Atualiza os logs
            foreach (ProdutoLoja p in prod)
                LogAlteracaoDAO.Instance.LogProdutoLoja(p);

            return retorno;
        }

        /// <summary>
        /// Atualiza a tabela produto_loja com o saldo do estoque
        /// </summary>
        public void AtualizarProdutoLoja(GDASession sessao, int idProd, int idLoja)
        {
            NewProd(sessao, idProd, idLoja);

            var saldoQtde = MovEstoqueDAO.Instance.ObtemSaldoQtdeMov(sessao, null, (uint)idProd, (uint)idLoja, null, false);

            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_loja SET QtdEstoque=?qtd WHERE IdProd={ idProd } AND IdLoja={ idLoja }", new GDAParameter("?qtd", saldoQtde));
        }

        /// <summary>
        /// Atualiza o total de M2 do produto loja.
        /// </summary>
        public void AtualizarTotalM2(GDASession session, int idProd, int idLoja, decimal totalM2)
        {
            objPersistence.ExecuteCommand(session, $"UPDATE produto_loja SET M2=QtdEstoque*{ totalM2.ToString().Replace(',', '.') } WHERE IdProd={ idProd } AND IdLoja={ idLoja }");
        }

        #endregion

        #region Atualiza o estoque mínimo por grupo/subgrupo

        /// <summary>
        /// Atualiza o estoque mínimo por grupo/subgrupo.
        /// </summary>
        public void AtualizaEstoqueMinimo(float qtdeMin, uint idLoja, uint idGrupo, uint? idSubgrupo)
        {
            foreach (Produto p in ProdutoDAO.Instance.GetByGrupoSubgrupo((int)idGrupo, (int?)idSubgrupo))
            {
                ProdutoLoja prod = new ProdutoLoja();
                prod.IdProd = p.IdProd;
                prod.IdLoja = (int)idLoja;
                prod.EstMinimo = qtdeMin;

                AtualizaEstoqueMinimo(prod);
            }
        }

        #endregion

        #region Obtém a quantidade de produtos abaixo do/no estoque mínimo

        /// <summary>
        /// Obtém a quantidade de produtos abaixo do/no estoque mínimo.
        /// </summary>
        public int ObtemQuantidadeProdutosAbaixoOuNoEstoqueMinimo(int idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(
                string.Format(
                    @"SELECT COUNT(*) FROM produto p
                        LEFT JOIN (SELECT * FROM produto_loja WHERE IdLoja={0}) pl ON (p.IdProd=pl.IdProd)
                        LEFT JOIN grupo_prod g ON (p.IdGrupoProd=g.IdGrupoProd)
                        LEFT JOIN subgrupo_prod sg ON (p.IdSubgrupoProd=sg.IdSubgrupoProd)
                        {1}
                    WHERE pl.IdLoja={0} AND p.Situacao={2} AND COALESCE(pl.EstMinimo, 0)>0 AND (pl.EstMinimo=0 OR IF(COALESCE(sg.TipoCalculo, g.TipoCalculo) IN ({3},{4}),
                        pl.M2 + COALESCE(pped.QtdeProduzindo,0) < pl.EstMinimo, pl.QtdEstoque + COALESCE(pped.QtdeProduzindo,0) < pl.EstMinimo))",
                    idLoja, ProdutoDAO.Instance.SqlPendenteProducao("p", "pl", null), (int)Situacao.Ativo,
                    (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto));
        }

        #endregion
    }
}
