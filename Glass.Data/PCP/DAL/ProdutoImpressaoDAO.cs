using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.EFD;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ProdutoImpressaoDAO : BaseDAO<ProdutoImpressao, ProdutoImpressaoDAO>
	{
        //private ProdutoImpressaoDAO() { }

        private static readonly object _insertOrUpdatePecaLock = new object();

        public enum TipoEtiqueta : long
        {
            Pedido = 1,
            NotaFiscal,
            Retalho,
            Box
        }

        #region Busca produtos relacionados à uma Impressão

        private string SqlImpressao(string idImpressao, string planoCorte, uint idPedido, uint numeroNFe, string descrProduto, string etiqueta, float? altura,
            int? largura, bool usarAgrupamentos, DataSources.TipoDataEtiquetaEnum tipoData, bool etiqReposicao,
            bool buscarCancelados, bool selecionar)
        {
            return SqlImpressao(null, idImpressao, planoCorte, idPedido, numeroNFe, descrProduto, etiqueta, altura, largura, usarAgrupamentos,
                tipoData, etiqReposicao, buscarCancelados, selecionar);
        }

        private string SqlImpressao(GDASession session, string idImpressao, string planoCorte, uint idPedido, uint numeroNFe, string descrProduto,
            string etiqueta, float? altura, int? largura, bool usarAgrupamentos, DataSources.TipoDataEtiquetaEnum tipoData, bool etiqReposicao, 
            bool buscarCancelados, bool selecionar)
        {
            string campos = selecionar ? "IF(pi.IdProdPedBox IS NOT NULL, ppb.IdPedido, pi.IdPedido) as IdPedido, pi.*, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", concat(a.Ambiente, if(a.redondo and " + 
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', '')), concat(p.Descricao, if(pp.redondo, ' REDONDO', ''))) as DescrProduto, p.CodInterno as CodInternoProd,
                apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, nf.numeroNFe,
                coalesce(if(pi.idRetalhoProducao is not null, p.altura, coalesce(pnf.altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Altura, if(pp.AlturaReal > 0, pp.alturareal, pp.Altura)))), ppb.altura) as Altura, 
                coalesce(if(pi.idRetalhoProducao is not null, p.largura, coalesce(pnf.largura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", if(a.redondo, 0, a.Largura), if(pp.redondo, 0, if(pp.larguraReal > 0, pp.larguraReal, pp.Largura))))), ppb.largura) as Largura, 
                if(pi.idRetalhoProducao is not null OR pi.IdProdPedBox is not null, 1, coalesce(pnf.qtde, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Qtde, pp.Qtde))) as Qtde, 
                coalesce(pnf.obs, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.obs, pp.obs)) as Obs, coalesce(pp.pedCli, ppb.pedCli) as pedCli, a.ambiente, p.idCorVidro as Cor, 
                p.Espessura as Espessura, c.Descricao as DescrCor, p.idProd, p.idGrupoProd, p.idSubgrupoProd, cast(" + (int)tipoData + @" as signed) as tipoData, 
                ped.DataEntrega, ped.dataEntregaOriginal, pedEsp.dataFabrica, " + ClienteDAO.Instance.GetNomeCliente("cli") + @" as nomeCliente, 
                cli.id_Cli as idCliente, " + FornecedorDAO.Instance.GetNomeFornecedor("f") + " as nomeFornec" : "Count(*)";

            string sql = "Select " + campos + @" From produto_impressao pi 
                Left Join retalho_producao rp on (pi.idRetalhoProducao=rp.idRetalhoProducao)
                Left Join produto_pedido_producao ppp on (rp.idProdPedProducaoOrig=ppp.idProdPedProducao)
                Left Join produtos_pedido_espelho pp On (coalesce(pi.idProdPed, ppp.idProdPed)=pp.idProdPed) 
                Left Join ambiente_pedido_espelho a On (coalesce(pi.idAmbientePedido, pp.idAmbientePedido)=a.idAmbientePedido) 
                Left Join produtos_pedido ppb ON (pi.idProdPedBox = ppb.idProdPed)
                Left Join pedido ped On (coalesce(pp.idPedido,a.idpedido, ppb.IdPedido)=ped.idPedido) 
                Left Join produtos_nf pnf on (coalesce(pi.idProdNf, rp.idProdNf)=pnf.idProdNf)
                Left Join nota_fiscal nf on (pnf.idNf=nf.idNf)
                Left Join fornecedor f on (nf.idFornec=f.idFornec)
                Left Join cliente cli On (ped.idCli=cli.id_Cli) 
                Left Join pedido_espelho pedEsp On (ped.idPedido=pedEsp.idPedido)
                Left Join produto p On (coalesce(rp.idProd, pp.idProd, a.idProd, pnf.idProd, ppb.IdProd)=p.idProd) 
                Left Join cor_vidro c On (p.idCorVidro=c.idCorVidro) 
                Left Join etiqueta_aplicacao apl On (coalesce(pp.idAplicacao, a.idAplicacao, ppb.IdAplicacao)=apl.idAplicacao) 
                Left Join etiqueta_processo prc On (coalesce(pp.idProcesso, a.idAplicacao, ppb.IdProcesso)=prc.idProcesso) 
                Where 1";

            if (!string.IsNullOrEmpty(idImpressao))
                sql += " AND pi.idImpressao In (" + idImpressao + ")";

            if (!buscarCancelados)
                sql += " and !coalesce(pi.cancelado,false)";

            if (idPedido > 0)
                sql += " And (pi.idPedido=" + idPedido + " OR ppb.IdPedido = " + idPedido + ")";

            if (numeroNFe > 0)
                sql += " And nf.numeroNFe=" + numeroNFe;

            if (!String.IsNullOrEmpty(planoCorte))
                sql += " And pi.planoCorte=?planoCorte";

            if (!String.IsNullOrEmpty(descrProduto))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(session, null, descrProduto);
                sql += " And p.idProd In (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(etiqueta))
                sql += " And pi.numEtiqueta=?etiqueta";

            if (altura.HasValue && altura.Value > 0)
                sql += " and coalesce(pnf.altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                    a.Altura, if(pp.AlturaReal > 0, pp.alturareal, pp.Altura)))=" + altura;

            if (largura.HasValue && largura.Value > 0)
                sql += " and coalesce(pnf.largura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                    if(a.redondo, 0, a.Largura), if(pp.redondo, 0, if(pp.larguraReal > 0, pp.larguraReal, pp.Largura))))=" + largura;

            return sql;
        }

        public IList<ProdutoImpressao> GetListImpressao(uint idImpressao, string planoCorte, uint idPedido, uint numeroNFe, string descrProduto,
            string etiqueta, float? altura, int? largura, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "COALESCE(pi.idPedido, pi.idNf), pi.posicaoProd, pi.itemEtiqueta";

            var lstProdImpr = LoadDataWithSortExpression(SqlImpressao(idImpressao.ToString(), planoCorte, idPedido, numeroNFe, descrProduto, etiqueta, 
                altura, largura, true, EtiquetaConfig.TipoDataEtiqueta, false, true, true), sortExpression, startRow, pageSize, GetParam(planoCorte, descrProduto, etiqueta));

            foreach (ProdutoImpressao pi in lstProdImpr)
                if (pi.IdProdPed > 0)
                {
                    pi.ObsEditar = pi.Obs;

                    var descrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(pi.IdProdPed.Value, true).ToUpper();
                    
                    pi.Obs = (!String.IsNullOrEmpty(descrBenef) ? descrBenef + " " : "") + pi.Obs;
                }

            return lstProdImpr;
        }

        public IList<ProdutoImpressao> GetListImpressao(string idImpressao)
        {
            return GetListImpressao(null, idImpressao);
        }

        public IList<ProdutoImpressao> GetListImpressao(GDASession session, string idImpressao)
        {
            return objPersistence.LoadData(session, SqlImpressao(idImpressao, null, 0, 0, null, null, null, null, true, EtiquetaConfig.TipoDataEtiqueta, false, false, true)).ToList();
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdPed
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        private string GetIdImpressaoByCampoId(GDASession sessao, string campo, uint id, bool apenasImpressoesFinalizadas)
        {
            string sql = @"
                Select pi.idImpressao
                From produto_impressao pi 
                    Inner Join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                Where pi.idImpressao is not null and pi." + campo + "=" + id + (!apenasImpressoesFinalizadas ? "" :
                    " and !coalesce(pi.cancelado,false) And ie.Situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa);

            return GetValoresCampo(sessao, sql, "idImpressao");
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdPed
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetIdImpressaoByProdPed(uint idProdPed)
        {
            return GetIdImpressaoByProdPed(null, idProdPed, false);
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdPed
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="apenasImpressoesFinalizadas"></param>
        /// <returns></returns>
        public string GetIdImpressaoByProdPed(uint idProdPed, bool apenasImpressoesFinalizadas)
        {
            return GetIdImpressaoByProdPed(null, idProdPed, apenasImpressoesFinalizadas);
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdPed
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        /// <param name="apenasImpressoesFinalizadas"></param>
        /// <returns></returns>
        public string GetIdImpressaoByProdPed(GDASession sessao, uint idProdPed, bool apenasImpressoesFinalizadas)
        {
            return GetIdImpressaoByCampoId(sessao, "idProdPed", idProdPed, apenasImpressoesFinalizadas);
        }

        public string GetIdImpressaoByAmbientePedido(uint idAmbientePedido, bool apenasImpressoesFinalizadas)
        {
            return GetIdImpressaoByCampoId(null, "idAmbientePedido", idAmbientePedido, apenasImpressoesFinalizadas);
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdNf
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public string GetIdImpressaoByProdNf(uint idProdNf)
        {
            return GetIdImpressaoByProdNf(idProdNf, false);
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idProdNf
        /// </summary>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public string GetIdImpressaoByProdNf(uint idProdNf, bool apenasImpressoesFinalizadas)
        {
            return GetIdImpressaoByCampoId(null, "idProdNf", idProdNf, apenasImpressoesFinalizadas);
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idRetalhoProducao
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <returns></returns>
        public string GetIdImpressaoByRetalhoProducao(uint idRetalhoProducao)
        {
            return GetIdImpressaoByRetalhoProducao(idRetalhoProducao, false);
        }

        /// <summary>
        /// Recupera os ids do prod impressao pelo id da impressao
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        public string GetIdsProdImpressaoByIdImpressao(uint idImpressao)
        {
            string sql = @"
                SELECT DISTINCT IdProdImpressao
                FROM produto_impressao
                WHERE idImpressao = " + idImpressao;

            return GetValoresCampo(sql, "IdProdImpressao");
        }

        /// <summary>
        /// Obtém o idImpressao a partir do idRetalhoProducao
        /// </summary>
        /// <param name="idRetalhoProducao"></param>
        /// <returns></returns>
        public string GetIdImpressaoByRetalhoProducao(uint idRetalhoProducao, bool apenasImpressoesFinalizadas)
        {
            return GetIdImpressaoByCampoId(null, "idRetalhoProducao", idRetalhoProducao, apenasImpressoesFinalizadas);
        }

        public int GetCountImpressao(uint idImpressao, string planoCorte, uint idPedido, uint numeroNFe, string descrProduto, string etiqueta, float? altura, int? largura)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlImpressao(idImpressao.ToString(), planoCorte, idPedido, numeroNFe, descrProduto, etiqueta, altura, 
                largura, true, EtiquetaConfig.TipoDataEtiqueta, false, true, false), GetParam(planoCorte, descrProduto, etiqueta));
        }

        private GDAParameter[] GetParam(string planoCorte, string descrProd, string etiqueta)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(planoCorte))
                lstParam.Add(new GDAParameter("?planoCorte", planoCorte));

            if (!String.IsNullOrEmpty(descrProd))
                lstParam.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            if (!String.IsNullOrEmpty(etiqueta))
                lstParam.Add(new GDAParameter("?etiqueta", etiqueta));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Verifica se um produtoImpressao já foi inserido

        /// <summary>
        /// Verifica se um produtoImpressao já foi inserido
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        public bool ProdutoImpressaoExists(uint idProdPed, uint idImpressao)
        {
            string sql = "Select Count(*) From produto_impressao Where idProdPed=" + idProdPed + " And idImpressao=" + idImpressao;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public bool ProdutoImpresso(uint idProdPed, int qtdImprimir)
        {
            string sql = "Select Count(*) From produto_impressao Where idProdPed=" + idProdPed;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possui alguma peça impressa
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoPossuiPecaImpressa(GDASession session, uint idPedido)
        {
            string sql = @"
                Select Count(*) From produto_impressao pi
                inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                Where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa +
                " and !coalesce(pi.cancelado,false) and pi.idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se a nota fiscal possui alguma peça impressa
        /// </summary>
        public bool NfPossuiPecaImpressa(GDASession session, int idNf)
        {
            string sql = @"
                Select Count(*) From produto_impressao pi
                inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                Where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa +
                " and !coalesce(pi.cancelado,false) and pi.idNf=" + idNf;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se a nota fiscal possui alguma peça impressa
        /// </summary>
        public bool VerificarPossuiImpressao(GDASession session, int idProdNf)
        {
            string sql = string.Format(@"
                SELECT COUNT(*) FROM produto_impressao pi 
                INNER JOIN impressao_etiqueta ie ON (pi.idImpressao=ie.idImpressao)
                WHERE ie.situacao={0} AND !coalesce(pi.cancelado,false) AND pi.idProdNf = {1}", (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa, idProdNf);

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se alguma etiqueta foi impressa a partir da nota fiscal passada
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public bool NotaFiscalPossuiPecaProducao(GDASession sessao, uint idNf)
        {
            string sql = @"Select Count(*) From produto_impressao pi Where pi.idNf=" + idNf;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se a etiqueta é de chapa de vidro

        /// <summary>
        /// Verifica se a etiqueta é de chapa de vidro
        /// </summary>
        public bool IsChapaVidro(GDASession session, string numEtiqueta)
        {
            var idProdNf = ObtemCampoByEtiqueta(session, numEtiqueta, ObtemTipoEtiqueta(numEtiqueta), "idProdNf");
            
            string sql = @"
                SELECT COUNT(*) 
                FROM produtos_nf pnf
                    INNER JOIN produto p on(pnf.idprod = p.idprod)
                    INNER JOIN subgrupo_prod sgp ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
                WHERE pnf.idProdNf=" + idProdNf + @"
                    AND sgp.tipoSubgrupo IN(" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + ")";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Busca produtos impressão pelos seus ids

        public ProdutoImpressao[] GetListByIds(string idsProdImpressao, bool apenasNaoCanceladas)
        {
            return GetListByIds(null, idsProdImpressao, apenasNaoCanceladas);
        }

        public ProdutoImpressao[] GetListByIds(GDASession session, string idsProdImpressao, bool apenasNaoCanceladas)
        {
            string sql = "Select * From produto_impressao Where idProdImpressao In (" + idsProdImpressao + ")";

            if (apenasNaoCanceladas)
                sql += " And !Coalesce(cancelado, False)";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca por etiqueta
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem um a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do campo.</returns>
        public uint ObtemCampoByEtiqueta(string numEtiqueta, TipoEtiqueta tipoEtiqueta, string campo)
        {
            return ObtemCampoByEtiqueta(null, numEtiqueta, tipoEtiqueta, campo);
        }

        /// <summary>
        /// Obtem um a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do campo.</returns>
        public uint ObtemCampoByEtiqueta(GDASession sessao, string numEtiqueta, TipoEtiqueta tipoEtiqueta, string campo)
        {
            string campoId = tipoEtiqueta == TipoEtiqueta.Pedido ? "idPedido" :
                tipoEtiqueta == TipoEtiqueta.NotaFiscal ? "idNf" : "idRetalhoProducao";

            List<string> dadosEtiqueta = new List<string>(numEtiqueta.Split('-', '.', '/'));
            if (tipoEtiqueta == TipoEtiqueta.Retalho)
                dadosEtiqueta.Insert(1, "1");

            string sql = @"select " + campo + @" from produto_impressao pi
                left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where coalesce(ie.situacao, " + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + ")=" +
                    (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + @" and !coalesce(pi.cancelado,false)
                    and " + campoId + "=" + dadosEtiqueta[0].ToUpper().TrimStart('N', 'R') + " and posicaoProd=" + dadosEtiqueta[1] +
                    " and itemEtiqueta=" + dadosEtiqueta[2] + " and qtdeProd=" + dadosEtiqueta[3];

            return ExecuteScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Retorna o tipo de etiqueta pelo seu número.
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public TipoEtiqueta ObtemTipoEtiqueta(string numEtiqueta)
        {
            return numEtiqueta[0].ToString().ToUpper() == "N" ? TipoEtiqueta.NotaFiscal :
                numEtiqueta[0].ToString().ToUpper() == "R" ? TipoEtiqueta.Retalho : TipoEtiqueta.Pedido;
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem o IdProdImpressao a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do produto impressão.</returns>
        public uint ObtemIdProdImpressao(string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return ObtemIdProdImpressao(null, numEtiqueta, tipoEtiqueta);
        }

        /// <summary>
        /// Obtem o IdProdImpressao a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do produto impressão.</returns>
        public uint ObtemIdProdImpressao(GDASession sessao, string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return ObtemCampoByEtiqueta(sessao, numEtiqueta, tipoEtiqueta, "idProdImpressao");
        }

        public uint? ObtemIdNf(GDASession sessao, uint idProdImpressao)
        {
            return ObtemValorCampo<uint?>(sessao, "idNf", "idProdImpressao=" + idProdImpressao);
        }

        /// <summary>
        /// Obtem o IdProdImpressao a partir de uma etiqueta para o carregamento. Não considera canceladas.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public uint? ObtemIdProdImpressaoParaCarregamento(GDASession sessao, string numEtiqueta)
        {
            return ObtemValorCampo<uint?>(sessao, "IdProdImpressao", "cancelado = 0 AND numEtiqueta = ?numEtq", new GDAParameter("?numEtq", numEtiqueta));
        }

        /// <summary>
        /// Busca o produto impressão para expedicao da chapa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public ProdutoImpressao ObtemProdImpressaoParaExpedicao(string numEtiqueta)
        {
            return ObtemProdImpressaoParaExpedicao(null, numEtiqueta);
        }

        /// <summary>
        /// Busca o produto impressão para expedicao da chapa
        /// </summary>
        /// <param name="session"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public ProdutoImpressao ObtemProdImpressaoParaExpedicao(GDASession session, string numEtiqueta)
        {
            return objPersistence.LoadOneData(SqlImpressao(session, null, null, 0, 0, null, numEtiqueta, null, null,
                false, DataSources.TipoDataEtiquetaEnum.Fábrica, false, false, true), new GDAParameter("?etiqueta", numEtiqueta));
        }

        /// <summary>
        /// Obtem o IdProdNf a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do produto impressão.</returns>
        public uint ObtemIdProdNf(string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return ObtemCampoByEtiqueta(numEtiqueta, tipoEtiqueta, "idProdNf");
        }

        /// <summary>
        /// Obtem o identificador do produto pedido
        /// </summary>
        /// <param name="idProdImpressao"></param>
        /// <returns></returns>
        public int ObtemIdProdPed(GDASession sessao, int idProdImpressao)
        {
            return ObtemValorCampo<int>(sessao, "IdProdPed", "IdProdImpressao=" + idProdImpressao);
        }

        /// <summary>
        /// Obtem o IdProdPed a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>O id do produto impressão.</returns>
        public uint ObtemIdProdPed(GDASession sessao, string numEtiqueta)
        {
            return ObtemCampoByEtiqueta(numEtiqueta, TipoEtiqueta.Pedido, "idProdPed");
        }

        /// <summary>
        /// Obtem o IdImpressao a partir de uma etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <returns>O id da impressão.</returns>
        public uint ObtemIdImpressao(string numEtiqueta)
        {
            TipoEtiqueta tipoEtiqueta = ObtemTipoEtiqueta(numEtiqueta);
            return ObtemCampoByEtiqueta(numEtiqueta, tipoEtiqueta, "pi.idImpressao");
        }

        public ProdutoImpressao GetElementByEtiqueta(string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return GetElementByEtiqueta(null, numEtiqueta, tipoEtiqueta);
        }

        /// <summary>
        /// Retorna um elemento a partir da etiqueta.
        /// </summary>
        /// <param name="numEtiqueta">O número da etiqueta.</param>
        /// <param name="tipoEtiqueta">O tipo de etiqueta que será buscada.</param>
        /// <returns>Um objeto ProdutoImpressao.</returns>
        public ProdutoImpressao GetElementByEtiqueta(GDASession session, string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            try
            {
                uint idProdImpressao = ObtemIdProdImpressao(session, numEtiqueta, tipoEtiqueta);
                ProdutoImpressao prodImp = Exists(session, idProdImpressao) ? GetElementByPrimaryKey(session, idProdImpressao) : null;

                // Preenche o campo espessura (Usado em algumas etiquetas)
                if (prodImp != null && prodImp.IdProdPed > 0)
                {
                    prodImp.IdProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, prodImp.IdProdPed.Value);
                    prodImp.Espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)prodImp.IdProd);
                }

                return prodImp;
            }
            catch (Exception)
            {
                return new ProdutoImpressao();
            }
        }

        /// <summary>
        /// Obtem o plano de corte de uma etiqueta
        /// </summary>
        /// <param name="idProdImpressao"></param>
        /// <returns></returns>
        public string ObtemPlanoCorte(GDASession sessao, uint idProdImpressao)
        {
            return ObtemValorCampo<string>(sessao, "planocorte", "IdProdImpressao=" + idProdImpressao);
        }

        public uint? ObtemIdRetalho(GDASession sessao, uint idProdImpressao)
        {
            return ObtemValorCampo<uint?>(sessao, "idRetalhoProducao", "IdProdImpressao=" + idProdImpressao);
        }

        public DateTime? ObtemDataFabrica(GDASession session, string planoCorte, string idImpressao)
        {
            var sql = @"
                        SELECT MIN(p.DataFabrica)
                         FROM produto_impressao pi
 	                        LEFT JOIN pedido_espelho p ON (pi.IdPedido = p.IdPedido)
                         WHERE PlanoCorte=?planoCorte
 	                         AND IdImpressao IN (" + idImpressao + ")";

            return ExecuteScalar<DateTime?>(sql, new GDAParameter("?planoCorte", planoCorte));
        }

        /// <summary>
        /// Obtem o IdPedidoExpedicao a partir de uma etiqueta.
        /// </summary>
        public int? ObterIdPedidoExpedicaoPelaEtiqueta(GDASession sessao, string numEtiqueta)
        {
            return (int?)ObtemCampoByEtiqueta(sessao, numEtiqueta, TipoEtiqueta.NotaFiscal, "IdPedidoExpedicao");
        }

        #endregion

        #region Marca uma peça como impressa

        /// <summary>
        /// Marcar em qual impressao a peça foi impressa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idImpressao"></param>
        /// <param name="idProdPed"></param>
        public void MarcaImpressao(string numEtiqueta, uint idImpressao, TipoEtiqueta tipoEtiqueta)
        {
            MarcaImpressao(null, numEtiqueta, idImpressao, tipoEtiqueta, null);
        }

        /// <summary>
        /// Marcar em qual impressao a peça foi impressa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idImpressao"></param>
        /// <param name="idProdPed"></param>
        public void MarcaImpressao(GDASession session, string numEtiqueta, uint idImpressao, TipoEtiqueta tipoEtiqueta, bool? isPecaReposta)
        {
            if (isPecaReposta == null)
                isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(session, numEtiqueta, false);

            if (isPecaReposta.Value && idImpressao == 0)
                idImpressao = ProdutoPedidoProducaoDAO.Instance.ObtemIdImpressaoByEtiqueta(session, numEtiqueta);

            uint idProdImpressao = ObtemIdProdImpressao(session, numEtiqueta, tipoEtiqueta);

            // Insere um registro nesta tabela, para identificar se a peça foi impressa (Caso não tenha sido impresso ainda)
            if (idProdImpressao == 0)
            {
                int[] dadosEtiqueta = Array.ConvertAll(numEtiqueta.Split('-', '.', '/'), x => Glass.Conversoes.StrParaInt(x));

                ProdutoImpressao pi = new ProdutoImpressao()
                {
                    PosicaoProd = dadosEtiqueta[1],
                    ItemEtiqueta = dadosEtiqueta[2],
                    QtdeProd = dadosEtiqueta[3]
                };

                switch (tipoEtiqueta)
                {
                    case TipoEtiqueta.Pedido:
                        pi.IdPedido = (uint)dadosEtiqueta[0];
                        break;

                    case TipoEtiqueta.NotaFiscal:
                        pi.IdNf = (uint)dadosEtiqueta[0];
                        break;
                }

                idProdImpressao = Insert(session, pi);
            }

            string sql = "Update produto_impressao Set idImpressao=" + idImpressao + " Where idProdImpressao=" + idProdImpressao;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Verifica se uma peça foi impressa

        /// <summary>
        /// Verifica se há alguma etiqueta otimizada para o produto pedido já foi impressa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool EstaImpressa(uint idProdPed)
        {
            return EstaImpressa(null, idProdPed);
        }

        /// <summary>
        /// Verifica se há alguma etiqueta otimizada para o produto pedido já foi impressa
        /// </summary>
        /// <param name="session"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool EstaImpressa(GDASession session, uint idProdPed)
        {
            string sql = @"Select Count(*) From produto_impressao Where idImpressao is not null
                and !coalesce(cancelado, false) and idProdPed=" + idProdPed;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Retorna a quantidade de peças impressas para um produto de pedido.
        /// </summary>
        public int QuantidadeImpressa(GDASession session, int idProdPed)
        {
            var sql = string.Format(@"SELECT COUNT(*) FROM produto_impressao WHERE IdImpressao IS NOT NULL
                AND !COALESCE(Cancelado, FALSE) AND IdProdPed={0}", idProdPed);

            return objPersistence.ExecuteSqlQueryCount(session, sql);
        }

        /// <summary>
        /// Verifica se a etiqueta já foi impressa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool EstaImpressa(GDASession sessao, string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            string sql = @"Select Count(*) From produto_impressao Where idImpressao is not null
                and !coalesce(cancelado, false) and idProdImpressao=" + ObtemIdProdImpressao(sessao, numEtiqueta, tipoEtiqueta);

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a etiqueta já foi impressa
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool EstaImpressa(string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return EstaImpressa(null, numEtiqueta, tipoEtiqueta);
        }

        #endregion

        #region Busca etiquetas repostas

        /// <summary>
        /// Recupera a lista de etiquetas que foram otimizadas e que são repostas.
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        public List<string> GetEtiquetasRepostas(GDASession session, uint idImpressao, TipoEtiqueta tipoEtiqueta)
        {
            string campoEtiqueta = "cast(concat(" + (tipoEtiqueta == TipoEtiqueta.Pedido ? "idPedido" : "idNf") +
                ", '-', posicaoProd, '.', itemEtiqueta, '/', qtdeProd) as char)";

            string sql = "select " + campoEtiqueta + " from produto_impressao where " + campoEtiqueta + @" not in (select * from (
                select numEtiqueta from produto_pedido_producao where idImpressao={0}) as temp) and idImpressao={0}";

            return ExecuteMultipleScalar<string>(session, String.Format(sql, idImpressao)).ToList();
        }

        /// <summary>
        /// Retorna as etiquetas que foram impressas pela primeira vez e repostas na impressão passada
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="tipoEtiqueta"></param>
        /// <returns></returns>
        public List<string> GetEtiquetasRepostasNaImpressao(uint idImpressao, TipoEtiqueta tipoEtiqueta)
        {
            return GetEtiquetasRepostasNaImpressao(idImpressao, null, tipoEtiqueta);
        }

        /// <summary>
        /// Retorna as etiquetas que foram impressas pela primeira vez e repostas na impressão passada
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="tipoEtiqueta"></param>
        /// <returns></returns>
        public List<string> GetEtiquetasRepostasNaImpressao(uint idImpressao, string idsProdImpressao, TipoEtiqueta tipoEtiqueta)
        {
            return GetEtiquetasRepostasNaImpressao(null, idImpressao, idsProdImpressao, tipoEtiqueta);
        }

        /// <summary>
        /// Retorna as etiquetas que foram impressas pela primeira vez e repostas na impressão passada
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="tipoEtiqueta"></param>
        /// <returns></returns>
        public List<string> GetEtiquetasRepostasNaImpressao(GDASession session, uint idImpressao, string idsProdImpressao, TipoEtiqueta tipoEtiqueta)
        {
            string campoEtiqueta = "cast(concat(" + (tipoEtiqueta == TipoEtiqueta.Pedido ? "idPedido" : "idNf") +
                ", '-', posicaoProd, '.', itemEtiqueta, '/', qtdeProd) as char)";

            string sql = "select " + campoEtiqueta + " from produto_impressao where " + campoEtiqueta + @" in (select * from (
                select numEtiqueta from produto_pedido_producao where idImpressao={0} and pecaReposta) as temp) and idImpressao={0}";

            if (!String.IsNullOrEmpty(idsProdImpressao))
                sql += " and idProdImpressao in (" + idsProdImpressao + ")";

            return ExecuteMultipleScalar<string>(session, String.Format(sql, idImpressao)).ToList();
        }

        #endregion

        #region Retorna material do plano de corte passado

        /// <summary>
        /// Retorna material do plano de corte passado
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        public string GetMaterialPlanoCorte(GDASession session, string planoCorte, string idImpressao)
        {
            string sql = "Select idProdPed From produto_impressao Where planoCorte=?planoCorte And idImpressao In (" + idImpressao +
                ") limit 1";

            uint idProdPed = ExecuteScalar<uint>(session, sql, new GDAParameter("?planoCorte", planoCorte));
            uint idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + idProdPed);

            uint idCorVidro = ProdutoDAO.Instance.ObtemValorCampo<uint>(session, "idCorVidro", "idProd=" + idProd);

            string descrCor = idCorVidro > 0 ? CorVidroDAO.Instance.GetNome(idCorVidro) : "";

            float espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)idProd);

            return (String.IsNullOrEmpty(descrCor) ? "Cor N/D" : descrCor.ToUpper()) + " " + espessura.ToString().PadLeft(2, '0') + "MM";
        }

        #endregion

        #region Obtém campos

        /// <summary>
        /// Retorna o menor NumSeq da impressão e do plano de corte passado
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        public int ObtemNumSeqPlanoCorte(GDASession session, uint idImpressao, string planoCorte)
        {
            string sql = "Select Min(Coalesce(numSeq, 0)) From produto_impressao Where idImpressao=" + idImpressao + " And planoCorte=?planoCorte";

            return ExecuteScalar<int>(session, sql, new GDAParameter("?planoCorte", planoCorte));
        }

        public string ObtemNumEtiqueta(uint idProdImpressao)
        {
            return ObtemNumEtiqueta(null, idProdImpressao);
        }

        public string ObtemNumEtiqueta(GDASession session, uint idProdImpressao)
        {
            return ObtemValorCampo<string>(session, "numEtiqueta", "idProdImpressao=" + idProdImpressao);
        }

        public string ObterEtiquetasImpressasPeloPedido(GDASession session, int idPedido)
        {
            var etiquetasImpressas =
                ExecuteMultipleScalar<string>(session,
                    string.Format("SELECT NumEtiqueta FROM produto_impressao WHERE (Cancelado IS NULL OR Cancelado=0) AND IdPedido={0}",
                        idPedido));

            return
                etiquetasImpressas != null && etiquetasImpressas.Count > 0 ?
                    string.Join(", ", etiquetasImpressas) :
                    string.Empty;
        }

        #endregion

        #region Retorna quantidade de etiquetas

        /// <summary>
        /// Retorna quantidade de etiquetas 
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        public int GetQtdEtiquetaPlanoCorte(GDASession session, string planoCorte, string idImpressao)
        {
            string sql = "Select Count(*) From produto_impressao Where planoCorte=?planoCorte And idImpressao In (" + idImpressao + ")";

            return objPersistence.ExecuteSqlQueryCount(session, sql, new GDAParameter("?planoCorte", planoCorte));
        }

        #endregion

        #region Posição do arquivo de otimização

        /// <summary>
        /// Retorna a posição no arquivo de otimização da etiqueta passada
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public int GetPosArqOtimiz(string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return ObtemValorCampo<int>("posicaoArqOtimiz", "idProdImpressao=" + ObtemIdProdImpressao(numEtiqueta, tipoEtiqueta));
        }

        public string ObtemForma(GDASession session, string numEtiqueta, TipoEtiqueta tipoEtiqueta)
        {
            return ObtemValorCampo<string>(session, "forma", "idProdImpressao=" + ObtemIdProdImpressao(session, numEtiqueta, tipoEtiqueta));
        }

        public List<ProdutoImpressao> GetByIdPedido(int posicao, int idPedido)
        {
            return GetByIdPedido(null, posicao, idPedido);
        }

        public List<ProdutoImpressao> GetByIdPedido(GDASession sessao, int posicao, int idPedido)
        {
            string sql = "Select * From produto_impressao Where PosicaoProd=" + posicao + " And idPedido=" + idPedido;

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Retorna as peças otimizadas de uma impressão

        /// <summary>
        /// Retorna as peças otimizadas de uma impressão
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public List<ProdutoImpressao> GetByProdPed(string idImpressao, uint idProdPed)
        {
            string sql = "Select * From produto_impressao Where idImpressao In (" + idImpressao + ") And idProdPed=" + idProdPed +
                " Order By planoCorte, Cast(REPLACE(SubString(planoCorte, InStr(planoCorte, '-')+1), '/', '') as SIGNED)";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna as peças otimizadas de uma impressão
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idProdPedEsp"></param>
        /// <returns></returns>
        public List<ProdutoImpressao> GetByAmbientePedido(string idImpressao, uint idAmbientePedido)
        {
            string sql = "Select * From produto_impressao Where idImpressao In (" + idImpressao + ") And idAmbientePedido=" + idAmbientePedido +
                " Order By planoCorte, Cast(REPLACE(SubString(planoCorte, InStr(planoCorte, '-')+1), '/', '') as SIGNED)";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Tipo de Etiqueta

        public GenericModel[] GetTiposEtiquetas()
        {
            Converter<int, string> d = delegate(int tipoEtiqueta)
            {
                ImpressaoEtiqueta ie = new ImpressaoEtiqueta();
                ie.TipoImpressao = (TipoEtiqueta)tipoEtiqueta;
                return ie.DescrTipoImpressao;
            };

            return DataSourcesEFD.Instance.GetFromEnum(typeof(TipoEtiqueta), d, false).ToArray();
        }

        #endregion

        #region Retorna o id dos produtos de impressão
        
        /// <summary>
        /// Retorna o id dos produtos de impressão de um determinado pedido e plano de corte.
        /// </summary>
        public string GetByIdPedidoPlanoCorte(GDASession session, uint idImpressao, uint? idPedido, uint? numeroNFe, string planoCorte)
        {
            var sql = string.Format("SELECT IdProdImpressao FROM produto_impressao WHERE (Cancelado IS NULL OR Cancelado=0) AND IdImpressao={0}", idImpressao);

            if (idPedido > 0)
                sql += string.Format(" AND IdPedido={0}", idPedido.Value);
            else if (numeroNFe > 0)
                sql += string.Format(@" AND IdNf IN (SELECT pnf.IdNf FROM nota_fiscal nf
                        INNER JOIN produtos_nf pnf ON (nf.IdNf=pnf.IdNf)
                    WHERE nf.NumeroNFe={0}
                    GROUP BY nf.IdNf HAVING SUM(pnf.QtdImpresso) > 0)", numeroNFe);
            
            if (!string.IsNullOrEmpty(planoCorte))
                sql += " AND PlanoCorte=?planoCorte";

            // Busca os dados
            return GetValoresCampo(session, sql, "IdProdImpressao", new GDAParameter("?planoCorte", planoCorte));
        }

        #endregion

        #region Insere ou atualiza peça

        /// <summary>
        /// Insere ou atualiza peça em relação ao seu plano de corte
        /// </summary>
        public void InsertOrUpdatePecaComTransacao(string numEtiqueta, string planoCorte, int posicaoArqOtimiz, int numSeq, TipoEtiqueta tipoEtiqueta, string forma)
        {
            lock(_insertOrUpdatePecaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        InsertOrUpdatePeca(transaction, numEtiqueta, planoCorte, posicaoArqOtimiz, numSeq, tipoEtiqueta, forma);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Insere ou atualiza peça em relação ao seu plano de corte
        /// </summary>
        public void InsertOrUpdatePeca(GDASession sessao, string numEtiqueta, string planoCorte, int posicaoArqOtimiz, int numSeq, TipoEtiqueta tipoEtiqueta, string forma)
        {
            ProdutoImpressao dados = GetElementByEtiqueta(sessao, numEtiqueta, tipoEtiqueta);

            if (dados == null)
            {
                int[] dadosEtiqueta = Array.ConvertAll(numEtiqueta.Split('-', '.', '/'), x => Glass.Conversoes.StrParaInt(x.TrimStart('N')));

                dados = new ProdutoImpressao();

                if (tipoEtiqueta == TipoEtiqueta.Pedido)
                {
                    dados.IdPedido = (uint)dadosEtiqueta[0];
                    dados.IdProdPed = ProdutosPedidoEspelhoDAO.Instance.GetIdProdPedByEtiqueta(sessao, numEtiqueta, true);
                }
                else
                {
                    dados.IdNf = (uint)dadosEtiqueta[0];
                    dados.IdProdNf = ProdutosNfDAO.Instance.GetIdByEtiquetaFast(sessao, numEtiqueta);
                }

                dados.PosicaoProd = dadosEtiqueta[1];
                dados.ItemEtiqueta = dadosEtiqueta[2];
                dados.QtdeProd = dadosEtiqueta[3];
            }

            if (!string.IsNullOrEmpty(forma))
                dados.Forma = forma;

            if (!String.IsNullOrEmpty(planoCorte))
                dados.PlanoCorte = planoCorte;

            if (posicaoArqOtimiz > 0)
                dados.PosicaoArqOtimiz = posicaoArqOtimiz;

            if (numSeq > 0)
                dados.NumSeq = numSeq;

            InsertOrUpdate(sessao, dados);

            //Estava ocorrendo um erro onde o plano de corte não estava sendo atualizado no produto impressao.
            //Foi adicionado o codigo abaixo para checar se foi realmente atualizado, e se não foi atualiza novamente.
            if (!string.IsNullOrEmpty(planoCorte))
            {
                if (dados.IdProdImpressao == 0)
                    ErroDAO.Instance.InserirFromException("Importar arquivo otimização", new Exception("Id do produto impressão não encontrado."));
                else if(string.IsNullOrEmpty(ObtemPlanoCorte(sessao, dados.IdProdImpressao)))
                    objPersistence.ExecuteCommand(sessao, @"update produto_impressao set planoCorte=?pc
                        where IdProdImpressao=" + dados.IdProdImpressao, new GDAParameter("?pc", planoCorte));
            }

            // Atualiza o plano de corte da etiqueta na produção
            if (tipoEtiqueta == TipoEtiqueta.Pedido)
            {
                uint? idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta);
                if (idProdPedProducao > 0)
                    objPersistence.ExecuteCommand(sessao, @"update produto_pedido_producao set planoCorte=?pc
                        where idProdPedProducao=" + idProdPedProducao, new GDAParameter("?pc", planoCorte));
                else
                    ErroDAO.Instance.InserirFromException("ProdutoImpressaoDAO(InsertOrUpdatePeca)",
                        new Exception(
                            string.Format("Não foi possível recuperar o produto de produção da etiqueta: {0} e plano de corte: {1}.",
                                numEtiqueta, planoCorte)));
            }
        }

        #endregion

        #region Métodos sobrescritos

        internal string GetNumeroEtiqueta(ProdutoImpressao prodImp)
        {
            if (prodImp.IdPedido > 0)
                return prodImp.IdPedido + "-" + prodImp.PosicaoProd + "." + prodImp.ItemEtiqueta + "/" + prodImp.QtdeProd;

            else if (prodImp.IdNf > 0)
                return "N" + prodImp.IdNf + "-" + prodImp.PosicaoProd + "." + prodImp.ItemEtiqueta + "/" + prodImp.QtdeProd;

            else if (prodImp.IdRetalhoProducao > 0)
                return "R" + prodImp.IdRetalhoProducao + "-1/1";

            return String.Empty;
        }

        public override uint Insert(GDASession session, ProdutoImpressao objInsert)
        {
            objInsert.NumEtiqueta = GetNumeroEtiqueta(objInsert);
            return base.Insert(session, objInsert);
        }

        public override uint Insert(ProdutoImpressao objInsert)
        {
            objInsert.NumEtiqueta = GetNumeroEtiqueta(objInsert);
            return base.Insert(objInsert);
        }

        public override int Update(GDASession session, ProdutoImpressao objUpdate)
        {
            objUpdate.NumEtiqueta = GetNumeroEtiqueta(objUpdate);
            return base.Update(session, objUpdate);
        }

        public override int Update(ProdutoImpressao objUpdate)
        {
            objUpdate.NumEtiqueta = GetNumeroEtiqueta(objUpdate);
            return base.Update(objUpdate);
        }

        public override void InsertOrUpdate(ProdutoImpressao objUpdate)
        {
            InsertOrUpdate(null, objUpdate);
        }

        public override void InsertOrUpdate(GDASession session, ProdutoImpressao objUpdate)
        {
            if (Exists(session, objUpdate))
                Update(session, objUpdate);
            else
            {
                uint key = GetKey(objUpdate);
                uint id = Insert(session, objUpdate);

                if (key > 0 && id > 0)
                {
                    objPersistence.ExecuteCommand(session, "update " + objPersistence.TableNameInfo.Name + " set " +
                        objPersistence.Keys[0].Name + "=" + key + " where " + objPersistence.Keys[0].Name + "=" + id);

                    objUpdate.IdProdImpressao = id;
                }
            }
        }

        #endregion

        #region Recupera o código do produto associado a um produto impresso

        /// <summary>
        /// Recupera o código do produto associado a um produto impresso.
        /// </summary>
        /// <param name="idProdImpressao">O código do produto impresso.</param>
        /// <returns></returns>
        public uint? GetIdProd(GDASession sessao, uint idProdImpressao)
        {
            ProdutoImpressao pi = GetElementByPrimaryKey(sessao, idProdImpressao);
            uint? idProd = null;

            if (pi == null)
                return null;

            if (pi.IdPedido > 0)
            {
                if (pi.IdProdPed > 0)
                    idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idProdPed=" + pi.IdProdPed);
                else if (pi.IdAmbientePedido > 0)
                    idProd = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idAmbientePedido=" + pi.IdAmbientePedido);
            }
            else if (pi.IdNf > 0)
            {
                if (pi.IdProdNf > 0)
                    idProd = ProdutosNfDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idProdNf=" + pi.IdProdNf);
            }
            else if (pi.IdRetalhoProducao > 0)
                idProd = RetalhoProducaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idRetalhoProducao=" + pi.IdRetalhoProducao);

            return idProd;
        }

        /// <summary>
        /// Recupera o código do produto associado a um produto impresso.
        /// </summary>
        /// <param name="idProdImpressao">O código do produto impresso.</param>
        /// <returns></returns>
        public uint? GetIdProd(uint idProdImpressao)
        {
            return GetIdProd(null, idProdImpressao);
        }

        #endregion

        #region Verifica se uma peça foi expedida


        /// <summary>
        /// Verifica se uma peça foi expedida
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressao"></param>
        /// <returns></returns>
        public bool EstaExpedida(GDASession sessao, uint idProdImpressao)
        {
            return ObtemValorCampo<uint>(sessao, "IdPedidoExpedicao", "IdProdImpressao=" + idProdImpressao) > 0;
        }

        // <summary>
        /// Verifica se uma peça foi expedida
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressao"></param>
        /// <returns></returns>
        public bool EstaExpedida(uint idProdImpressao)
        {
            return EstaExpedida(null, idProdImpressao);
        }


        #endregion

        #region Recupera a quantidade impressa de box de um impressão

        /// <summary>
        /// Recupera a quantidade impressa de box de um impressão
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        public string ObtemQtdeBoxImpresso(int idImpressao)
        {
            var sql = @"
                SELECT CAST(CONCAT(IdProdPedBox, ';', count(*)) AS CHAR)
                FROM produto_impressao
                WHERE !cancelado AND IdImpressao = " + idImpressao + @"
                GROUP BY IdProdPedBox";

            return string.Join("|", ExecuteMultipleScalar<string>(sql));
        }

        #endregion

        #region Atualiza o pedido expedição

        /// <summary>
        /// Atualiza o pedido expedição
        /// </summary>
        public void AtualizaPedidoExpedicao(GDASession sessao, uint? idPedidoExp, uint idProdImpressao)
        {
            objPersistence.ExecuteCommand(sessao, @"UPDATE produto_impressao SET idPedidoExpedicao=" + (idPedidoExp == null ? "null" : idPedidoExp.Value.ToString()) + @"
                WHERE idProdImpressao=" + idProdImpressao);
        }

        /// <summary>
        /// Atualiza o pedido expedição
        /// </summary>
        /// <param name="idPedidoExp"></param>
        /// <param name="idProdImpressao"></param>
        public void AtualizaPedidoExpedicao(uint? idPedidoExp, uint idProdImpressao)
        {
            AtualizaPedidoExpedicao(null, idPedidoExp, idProdImpressao);
        }

        #endregion
    }
}