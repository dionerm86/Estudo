// <copyright file="ProdutosOrcamentoDAO.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed partial class ProdutosOrcamentoDAO : BaseDAO<ProdutosOrcamento, ProdutosOrcamentoDAO>
    {
        #region Busca produtos de orçamento

        /// <summary>
        /// Verifica se algum filtro foi informado.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProd">idProd.</param>
        /// <param name="idProdParent">idProdParent.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <param name="idsProdutos">idsProdutos.</param>
        /// <returns>True: existem parâmetros vazios ou nulos; False: não existem paâmetros vazios ou nulos.</returns>
        internal bool ParametrosVazios(int? idOrcamento, int? idProd, int? idProdParent, int? idProdOrcamentoParent, string idsProdutos)
        {
            if (idOrcamento.GetValueOrDefault() == 0 &&
                idProd.GetValueOrDefault() == 0 &&
                idProdParent.GetValueOrDefault() == 0 &&
                idProdOrcamentoParent.GetValueOrDefault() == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// SQL base para a busca de produtos de orçamento.
        /// </summary>
        /// <param name="idOrca">idOrca.</param>
        /// <param name="showChildren">showChildren.</param>
        /// <param name="idProd">idProd.</param>
        /// <param name="idProdParent">idProdParent.</param>
        /// <param name="temOrdenacao">temOrdenacao.</param>
        /// <param name="selecionar">selecionar.</param>
        /// <returns>Retorna o SQL base para a busca de produtos de orçamento.</returns>
        private string Sql(uint? idOrca, bool showChildren, uint? idProd, uint? idProdParent, bool temOrdenacao, bool selecionar)
        {
            var sqlProdutoTabela = string.Empty;
            var sqlNumChild = string.Empty;
            var campos = string.Empty;

            if (selecionar)
            {
                sqlProdutoTabela = $"IF(o.TipoEntrega IN ({(int)Orcamento.TipoEntregaOrcamento.Balcao}, {(int)Orcamento.TipoEntregaOrcamento.Entrega}), p.ValorBalcao, p.ValorObra)";
                sqlNumChild = "SELECT COUNT(*) FROM produtos_orcamento WHERE IdProdParent = po.IdProd";
                campos = $@"po.*, ({sqlNumChild}) AS NumChild, p.CodInterno AS CodInterno, p.Descricao AS DescrProduto, {sqlProdutoTabela} AS ValorProdutoTabela,
                    um.Codigo AS Unidade, ip.Obs AS ObsProj, o.IdCliente, c.Nome AS NomeCliente, ep.CodInterno AS CodProcesso, ea.CodInterno AS CodAplicacao, sgp.descricao AS NomeSubGrupoProd,
                    gp.descricao As NomeGrupoProduto";
            }
            else
            {
                campos = "COUNT(*)";
            }

            var sql = $@"SELECT {campos} FROM produtos_orcamento po
                    LEFT JOIN orcamento o ON (po.IdOrcamento = o.IdOrcamento)
                    LEFT JOIN produto p ON (po.IdProduto = p.IdProd)
                    LEFT JOIN unidade_medida um ON (p.IdUnidadeMedida = um.IdUnidadeMedida)
                    LEFT JOIN item_projeto ip ON (po.IditemProjeto = ip.IdItemProjeto)
                    LEFT JOIN cliente c ON (c.Id_Cli = o.IdCliente)
                    LEFT JOIN etiqueta_processo ep ON (po.IdProcesso = ep.IdProcesso)
                    LEFT JOIN etiqueta_aplicacao ea ON (po.IdAplicacao = ea.IdAplicacao)
                    LEFT JOIN subgrupo_prod sgp ON (p.IdSubGrupoProd = sgp.IdSubGrupoProd)
                    LEFT JOIN grupo_prod gp ON (p.IdGrupoProd = gp.IdGrupoProd)
                WHERE 1";

            if (idOrca > 0)
            {
                sql += $" AND po.IdOrcamento = {idOrca}";
            }

            if (idProd > 0)
            {
                sql += $" AND po.IdProd = {idProd}";
            }

            if (idProdParent > 0)
            {
                sql += $" AND po.IdProdParent = {idProdParent}";
            }

            if (!showChildren)
            {
                sql += " AND (po.IdProdParent IS NULL OR po.IdProdParent = 0)";
            }

            if (!temOrdenacao)
            {
                sql += " ORDER BY po.IdProd";
            }

            return sql;
        }

        /// <summary>
        /// Obtém o objeto do produto de orçamento, referente ao ID informado.
        /// </summary>
        /// <param name="idProd">idProd.</param>
        /// <returns>Retorna o produto de orçamento referente ao ID informado.</returns>
        public ProdutosOrcamento GetElement(uint idProd)
        {
            return this.GetElement(null, idProd);
        }

        /// <summary>
        /// Obtém o objeto do produto de orçamento, referente ao ID informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProd">idProd.</param>
        /// <returns>Retorna o produto de orçamento referente ao ID informado.</returns>
        public ProdutosOrcamento GetElement(GDASession session, uint idProd)
        {
            if (this.ParametrosVazios(null, (int)idProd, null, null, string.Empty))
            {
                return new ProdutosOrcamento();
            }

            return this.objPersistence.LoadOneData(session, this.Sql(null, true, idProd, null, false, true));
        }

        /// <summary>
        /// Obtém produtos de orçamento, junto com seus beneficiamentos, para o relatório de orçamento.
        /// </summary>
        /// <param name="idOrca">idOrca.</param>
        /// <param name="showChildren">showChildren.</param>
        /// <param name="addTextoAmbiente">addTextoAmbiente.</param>
        /// <param name="incluirBeneficiamentos">incluirBeneficiamentos.</param>
        /// <returns>Retorna produtos de orçamento e beneficiamentos, para o relatório de orçamento.</returns>
        private IList<ProdutosOrcamento> GetReport(uint idOrca, bool showChildren, bool addTextoAmbiente, bool incluirBeneficiamentos)
        {
            if (this.ParametrosVazios((int)idOrca, null, null, null, string.Empty))
            {
                return new List<ProdutosOrcamento>();
            }

            var produtosOrcamento = this.objPersistence.LoadData(this.Sql(idOrca, showChildren, null, null, false, true)).ToList();

            if (showChildren && incluirBeneficiamentos)
            {
                var produtosOrcamentoTemp = new List<ProdutosOrcamento>();

                foreach (var produtoOrcamento in produtosOrcamento)
                {
                    if (produtoOrcamento.Redondo)
                    {
                        if (!BenefConfigDAO.Instance.CobrarRedondo() && !produtoOrcamento.DescrProduto.ToLower().Contains("redondo"))
                        {
                            produtoOrcamento.DescrProduto += " REDONDO";
                        }

                        produtoOrcamento.Largura = 0;
                    }

                    produtosOrcamentoTemp.Add(produtoOrcamento);

                    var beneficiamentos = produtoOrcamento.Beneficiamentos;

                    if (!PedidoConfig.RelatorioPedido.AgruparBenefRelatorio)
                    {
                        // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                        foreach (GenericBenef beneficiamento in beneficiamentos)
                        {
                            var descricaoLapBis = Utils.MontaDescrLapBis(
                                beneficiamento.BisAlt,
                                beneficiamento.BisLarg,
                                beneficiamento.LapAlt,
                                beneficiamento.LapLarg,
                                beneficiamento.EspBisote,
                                null,
                                null,
                                false);

                            var benef = new ProdutosOrcamento();
                            benef.IdOrcamento = idOrca;
                            benef.IdItemProjeto = produtoOrcamento.IdItemProjeto;
                            benef.Ambiente = produtoOrcamento.Ambiente;
                            benef.Qtde = beneficiamento.Qtd > 0 ? beneficiamento.Qtd : 1;
                            benef.ValorProd = beneficiamento.ValorUnit;
                            benef.Total = beneficiamento.Valor;
                            benef.ValorBenef = 0;
                            benef.DescrProduto = $" {beneficiamento.DescricaoBeneficiamento}{descricaoLapBis}";

                            // É necessário existir esta propriedade para que, no relatório da Vidraçaria Pestana, possamos identificar
                            // produtos de orçamento que são beneficiamentos e esconder somente os que são ambientes.
                            benef.IsBeneficiamento = true;

                            produtosOrcamentoTemp.Add(benef);
                        }
                    }
                    else
                    {
                        if (beneficiamentos.Count > 0)
                        {
                            var benef = new ProdutosOrcamento();
                            benef.IdOrcamento = idOrca;
                            benef.IdItemProjeto = produtoOrcamento.IdItemProjeto;
                            benef.Ambiente = produtoOrcamento.Ambiente;
                            benef.Qtde = 0;
                            benef.ValorBenef = 0;
                            benef.IsBeneficiamento = true;

                            // Para cada beneficiamento, adiciona o mesmo como um produto na listagem de produtos do pedido
                            foreach (GenericBenef beneficiamento in beneficiamentos)
                            {
                                var descricaoLapBis = Utils.MontaDescrLapBis(
                                    beneficiamento.BisAlt,
                                    beneficiamento.BisLarg,
                                    beneficiamento.LapAlt,
                                    beneficiamento.LapLarg,
                                    beneficiamento.EspBisote,
                                    null,
                                    null,
                                    false);

                                benef.ValorProd += beneficiamento.ValorUnit;
                                benef.Total = (benef.Total > 0 ? benef.Total.Value : 0) + beneficiamento.Valor;
                                var textoQuantidade = (beneficiamento.TipoCalculo == TipoCalculoBenef.Quantidade) ? $"{beneficiamento.Qtd.ToString()} " : string.Empty;
                                benef.DescrProduto += $"; {textoQuantidade}{beneficiamento.DescricaoBeneficiamento}{descricaoLapBis}";
                            }

                            benef.DescrProduto = $" {benef.DescrProduto.Substring(2)}";
                            produtosOrcamentoTemp.Add(benef);
                        }
                    }
                }

                produtosOrcamento = produtosOrcamentoTemp;
            }

            if (showChildren)
            {
                for (var i = produtosOrcamento.Count - 1; i >= 0; i--)
                {
                    if (produtosOrcamento[i].NumChild > 0)
                    {
                        produtosOrcamento.RemoveAt(i);
                    }
                }
            }

            var count = 1;

            foreach (var produtoOrcamento in produtosOrcamento)
            {
                produtoOrcamento.NumItem = (count++).ToString();

                try
                {
                    if (OrcamentoConfig.UploadImagensOrcamento && File.Exists(Utils.GetProdutosOrcamentoPath + produtoOrcamento.NomeImagem))
                    {
                        produtoOrcamento.ImagemProjModPath = $"file:///{Utils.GetProdutosOrcamentoPath.Replace("\\", "/")}{produtoOrcamento.NomeImagem}";
                    }
                    else if (produtoOrcamento.IdItemProjeto > 0 && produtoOrcamento.IdMaterItemProj == 0)
                    {
                        // Pega a imagem do modelo do projeto associada à este item.
                        var nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(null, produtoOrcamento.IdItemProjeto.Value);

                        if (!string.IsNullOrEmpty(nomeFigura))
                        {
                            produtoOrcamento.ImagemProjModPath = $"file:///{Utils.GetModelosProjetoPath.Replace("\\", "/")}{nomeFigura}";
                        }
                    }
                }
                catch
                {
                }

                // Quando os valores forem 0, não mostrar nada
                if (produtoOrcamento.Qtde == 0)
                {
                    produtoOrcamento.Qtde = null;
                }

                if (produtoOrcamento.Total == 0)
                {
                    produtoOrcamento.Total = null;
                }

                if (produtoOrcamento.ValorProd == 0)
                {
                    produtoOrcamento.ValorProd = null;
                }

                if (!showChildren && addTextoAmbiente && !string.IsNullOrWhiteSpace(produtoOrcamento.Ambiente))
                {
                    produtoOrcamento.Ambiente += ":\r\n";
                }
            }

            return produtosOrcamento.ToArray();
        }

        /// <summary>
        /// Obtém produtos de orçamento associados ao orçamento informado.
        /// </summary>
        /// <param name="idOrca">idOrca.</param>
        /// <param name="incluirBeneficiamentos">incluirBeneficiamentos.</param>
        /// <returns>Retorna produtos de orçamento, beneficiamentos dos produtos e as imagens de projeto associadas aos produtos.</returns>
        public ProdutosOrcamento[] GetForRpt(uint idOrca, bool incluirBeneficiamentos)
        {
            var imprimirProdutos = OrcamentoDAO.Instance.ObterImprimirProdutosOrcamento(null, (int)idOrca);
            var itens = new List<ProdutosOrcamento>(this.GetReport(idOrca, imprimirProdutos, true, incluirBeneficiamentos));

            for (var i = 1; i < itens.Count; i++)
            {
                this.ObterImagemProjeto(itens, i);
            }

            return itens.ToArray();
        }

        /// <summary>
        /// Obtém os produtos de orçamento, associados ao orçamento informado, para o relatório de memória de cálculo.
        /// </summary>
        /// <param name="idOrca">idOrca.</param>
        /// <returns>Retorna os produtos de orçamento, juntos com seus beneficiamentos, com base no orçamento informado.</returns>
        public IList<ProdutosOrcamento> GetForMemoriaCalculo(uint idOrca)
        {
            if (this.ParametrosVazios((int)idOrca, null, null, null, string.Empty))
            {
                return new List<ProdutosOrcamento>();
            }

            return this.GetReport(idOrca, true, false, false);
        }

        /// <summary>
        /// Obtém os produtos de orçamento do orçamento informado, para a funcionalidade de otimização de vidro.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna os produtos do orçamento informado, somente produtos do grupo vidro e que possuam cadastro de chapa de vidro.</returns>
        public IList<ProdutosOrcamento> GetByVidroOrcamento(uint idOrcamento)
        {
            var sql = this.Sql(idOrcamento, true, 0, 0, false, true);
            sql += $" AND p.IdGrupoProd = {(int)NomeGrupoProd.Vidro}";

            sql = sql.Substring(0, sql.ToUpper().IndexOf("WHERE", StringComparison.Ordinal));

            sql += $"WHERE po.IdOrcamento = {idOrcamento} AND po.Altura > 0 AND po.Largura > 0 AND po.IdProduto > 0";

            sql += @" AND po.IdProduto IN
                    (SELECT IdProd FROM chapa_vidro
                    UNION ALL
                    SELECT pbe.IdProd FROM produto_baixa_estoque pbe
                        INNER JOIN chapa_vidro c ON (pbe.IdProdBaixa=c.IdProd))
                AND po.IdProdParent IS NOT NULL";

            sql += " ORDER BY po.IdProdPed ASC";

            var produtosOrcamento = this.objPersistence.LoadData(sql).ToList();

            foreach (var produtoOrcamento in produtosOrcamento)
            {
                produtoOrcamento.DescrProduto = produtoOrcamento.DescrProduto.Replace("(", string.Empty).Replace(")", string.Empty).Replace("+", string.Empty).Replace("-", string.Empty);
            }

            return produtosOrcamento;
        }

        /// <summary>
        /// Obtém uma lista de produtos de orçamento para a impressão de recibo.
        /// </summary>
        /// <param name="idOrca">idOrca.</param>
        /// <returns>Retorna uma lista de produtos de orçamento para a impressão de recibo.</returns>
        public IList<ProdutosOrcamento> GetForRecibo(int idOrca)
        {
            return this.objPersistence.LoadData(Sql((uint?)idOrca, true, null, null, false, true)).ToList();
        }

        /// <summary>
        /// Obtém uma lista de produtos de orçamento do tipo alumínio, para a otimização de alumínios.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna uma lista de produtos de orçamento que possuem o tipo de cálculo de alumínio.</returns>
        public List<ProdutosOrcamento> ObterAluminiosParaOtimizacao(int idOrcamento)
        {
            var sql = @"
                SELECT po.IdOrcamento, po.IdProd, po.Altura, p.CodInterno, po.Qtde,
                    p.Descricao AS DescrProduto, CAST((p.peso * po.Altura) as DECIMAL(12, 2)) AS Peso,
                    (pot.IdPecaOtimizada IS NOT NULL) AS PecaOtimizada, po.IdProduto, mip.GrauCorte, gm.Esquadria AS ProjetoEsquadria
                FROM produtos_orcamento po
	                INNER JOIN produto p ON (po.IdProduto = p.IdProd)
                    LEFT JOIN item_projeto ip ON (po.IdItemProjeto = ip.IdItemProjeto)
                    LEFT JOIN material_item_projeto mip ON (ip.IdItemProjeto = mip.IdItemProjeto)
                    LEFT JOIN material_projeto_modelo mpm ON (mip.IdMaterProjMod = mpm.IdMaterProjMod)
                    LEFT JOIN projeto_modelo pm ON (mpm.IdProjetoModelo = pm.IdProjetoModelo)
                    LEFT JOIN grupo_modelo gm ON (pm.IdGrupoModelo = gm.IdGrupoModelo)
	                LEFT JOIN subgrupo_prod sp ON (p.IdSubgrupoProd = sp.IdSubgrupoProd)
                    LEFT JOIN grupo_prod gp ON (p.IdGrupoProd = gp.IdGrupoProd)
                    LEFT JOIN peca_otimizada pot ON (po.IdProd = pot.IdProdOrcamento)
                WHERE COALESCE(sp.TipoCalculo, gp.TipoCalculo) IN ({0})
                    AND gp.IdGrupoProd={1}
	                AND po.IdOrcamento = {2}
                GROUP BY po.IdProd";

            var tipoCalc = (int)TipoCalculoGrupoProd.Perimetro + "," +
                (int)TipoCalculoGrupoProd.ML + "," +
                (int)TipoCalculoGrupoProd.MLAL0 + "," +
                (int)TipoCalculoGrupoProd.MLAL05 + "," +
                (int)TipoCalculoGrupoProd.MLAL1 + "," +
                (int)TipoCalculoGrupoProd.MLAL6;

            sql = string.Format(sql, tipoCalc, (int)NomeGrupoProd.Alumínio, idOrcamento);

            return this.objPersistence.LoadData(sql);
        }

        #endregion

        #region Obtém dados

        /// <summary>
        /// Obtém os beneficiamentos do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna os beneficiamentos do produtos de orçamento informado.</returns>
        public GenericBenefCollection ObterBeneficiamentos(GDASession sessao, int idProdOrcamento)
        {
            var produtoOrcamento = new ProdutosOrcamento();
            produtoOrcamento.IdProd = (uint)idProdOrcamento;
            produtoOrcamento.IdProduto = (uint)this.ObterIdProduto(sessao, idProdOrcamento);

            return produtoOrcamento.Beneficiamentos;
        }

        /// <summary>
        /// Obtém o ID do produto de orçamento pelo ID do material de item de projeto associado a ele.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idMaterialItemProjeto">idMaterialItemProjeto.</param>
        /// <returns>Retorna o ID do produto de orçamento associado ao material de projeto informado.</returns>
        public int? ObterIdProdOrcamentoPeloIdMaterialItemProjeto(GDASession session, int idMaterialItemProjeto)
        {
            if (idMaterialItemProjeto == 0)
            {
                return null;
            }

            return this.ExecuteScalar<int?>(session, $"SELECT IdProd FROM produtos_orcamento WHERE IdMaterItemProj = {idMaterialItemProjeto}");
        }

        /// <summary>
        /// Obtém os ID's de material de item de projeto, dos produtos de orçamento, associados a um item de projeto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idItemProjeto">idItemProjeto.</param>
        /// <returns>Retorna os IDs de material de projeto associados ao item de projeto informado, desde que estejam associados à produtos de orçamento.</returns>
        public List<int> ObterIdsMaterialItemProjetoPeloItemProjeto(GDASession session, int idItemProjeto)
        {
            return this.ExecuteMultipleScalar<int>(session, $"SELECT IdMaterItemProj FROM produtos_orcamento WHERE IdItemProjeto = {idItemProjeto}");
        }

        /// <summary>
        /// Obtém o valor da propriedade Redondo, do produto de orçamento informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProd">idProd.</param>
        /// <returns>Retorna a propriedade Redondo do produto de orçamento informado.</returns>
        public bool ObterRedondo(GDASession session, int idProd)
        {
            return this.ObtemValorCampo<bool>(session, "Redondo", $"IdProd = {idProd}");
        }

        /// <summary>
        /// Obtém o valor da propriedade IdOrcamento, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o ID do orçamento do produto de orçamento informado.</returns>
        public int ObterIdOrcamento(GDASession sessao, int idProdOrcamento)
        {
            return this.ObtemValorCampo<int>(sessao, "IdOrcamento", $"IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Obtém o valor da propriedade IdProduto, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o produto associado ao produto de orçamento informado.</returns>
        public int ObterIdProduto(GDASession sessao, int idProdOrcamento)
        {
            return this.ObtemValorCampo<int>(sessao, "IdProduto", $"IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Obtém o valor da propriedade IdProdParent, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o produto ambiente associado ao produto de orçamento informado.</returns>
        public int? ObterIdProdAmbiente(GDASession sessao, int idProdOrcamento)
        {
            return this.ObtemValorCampo<int?>(sessao, "IdProdParent", $"IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Obtém o valor da propriedade IdProdOrcamentoParent, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o produto pai, de composição, associado ao produto de orçamento informado.</returns>
        public int? ObterIdProdOrcamentoParent(GDASession sessao, int idProdOrcamento)
        {
            return this.ObtemValorCampo<int?>(sessao, "IdProdOrcamentoParent", $"IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Obtém o valor da propriedade TotM, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o total de metro quadrado do produto de orçamento informado.</returns>
        public float ObterTotM(GDASession sessao, int idProdOrcamento)
        {
            return this.ExecuteScalar<float>(sessao, $"SELECT TotM FROM produtos_orcamento WHERE IdProd = {idProdOrcamento}").ToString()?.StrParaFloat() ?? 0;
        }

        /// <summary>
        /// Obtém o valor da propriedade Qtde, do produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna a quantidade do produto de orçamento informado.</returns>
        public float ObterQtde(GDASession sessao, int idProdOrcamento)
        {
            return this.objPersistence.ExecuteScalar(sessao, $"SELECT COALESCE(Qtde, 0) FROM produtos_pedido WHERE IdProd = {idProdOrcamento}")?.ToString()?.StrParaFloat() ?? 0;
        }

        /// <summary>
        /// Calcula o valor vendido do produto pai dos produtos da composição.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <returns>Retorna o valor vendido do produto de orçamento informado.</returns>
        public decimal ObterValorVendidoProdutoPai(GDASession sessao, int idProdOrcamentoParent)
        {
            var totalComposicao = this.ExecuteScalar<decimal>(sessao, $"SELECT SUM(Total) FROM produtos_orcamento WHERE IdProdOrcamentoParent = {idProdOrcamentoParent}");

            var sqlTotalBeneficiamentoComposicao = $@"SELECT SUM(pob.valor)
                FROM produto_orcamento_benef pob
                    INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                WHERE po.IdProdOrcamentoParent = {idProdOrcamentoParent}";
            var totalBeneficiamentoComposicao = this.ExecuteScalar<decimal>(sessao, sqlTotalBeneficiamentoComposicao);

            var totM = this.ObterTotM(sessao, idProdOrcamentoParent);
            var qtde = this.ObterQtde(sessao, idProdOrcamentoParent);

            if ((totalComposicao + totalBeneficiamentoComposicao) == 0 || totM == 0)
            {
                return 0;
            }

            return ((totalComposicao + totalBeneficiamentoComposicao) * (decimal)(qtde)) / (decimal)totM;
        }

        /// <summary>
        /// Obtém o nome do ambiente od produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o nome do ambiente associado ao produto de orçamento informado.</returns>
        public string ObterNomeAmbiente(GDASession sessao, int idProdOrcamento)
        {
            if (idProdOrcamento <= 0)
            {
                return string.Empty;
            }

            return this.ObtemValorCampo<string>(sessao, "Ambiente", $"IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Retorna o maior desconto em percentual aplicado nos produtos (ambiente) do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o maior desconto, de produtos orçamento, do orçamento.</returns>
        internal decimal ObterMaiorDesconto(GDASession sessao, uint idOrcamento)
        {
            return this.ObtemValorCampo<decimal>(sessao, "MAX(IF(TipoDesconto = 1, Desconto, (Desconto / Totalbruto) * 100))", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o caminho onde a imagem, do produto de orçamento, deve ser salva.
        /// </summary>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o caminho completo, juntamente com o nome do arquivo, em que a imagem do produto de orçamento deve ser salva.</returns>
        public string ObterUrlImagemSalvar(int idProdOrcamento)
        {
            return $"{Utils.GetProdutosOrcamentoPath}{idProdOrcamento.ToString().PadLeft(10, '0')}.jpg";
        }

        #endregion

        #region Valida dados

        /// <summary>
        /// Valida o valor de desconto informado, para o orçamento informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="totalBruto">totalBruto.</param>
        /// <returns>True: o desconto informado é válido;
        /// False: o desconto informado ultrapassa o percentual máximo permitido.</returns>
        private bool ValidarDesconto(GDASession session, uint idOrcamento, int tipoDesconto, decimal desconto, decimal totalBruto)
        {
            if (desconto > 0 && OrcamentoConfig.Desconto.DescontoMaximoOrcamento > 0 && OrcamentoConfig.Desconto.DescontoMaximoOrcamento <= 100)
            {
                // Calcula o desconto máximo permitido verificando se foi lançado algum desconto pelo administrador
                var idFunc = UserInfo.GetUserInfo.CodUser;

                if (Geral.ManterDescontoAdministrador)
                {
                    idFunc = (uint)OrcamentoDAO.Instance.ObterIdFuncDesc(session, (int)idOrcamento).GetValueOrDefault((int)idFunc);
                }

                var descMax = (decimal)OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(session, idFunc);
                var tipoDescontoOrca = OrcamentoDAO.Instance.ObterTipoDesconto(session, (int)idOrcamento);
                var descontoOrca = OrcamentoDAO.Instance.ObterDesconto(session, (int)idOrcamento);
                decimal percDescontoOrca;

                if (tipoDescontoOrca == 1)
                {
                    percDescontoOrca = descontoOrca;
                }
                else
                {
                    percDescontoOrca = (descontoOrca / OrcamentoDAO.Instance.ObterTotalBruto(session, (int)idOrcamento)) * 100;
                }

                if (tipoDesconto == 1)
                {
                    if (desconto + percDescontoOrca > descMax)
                    {
                        return false;
                    }
                }
                else
                {
                    if (((desconto / totalBruto) * 100) + percDescontoOrca > descMax)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Atualiza dados

        /// <summary>
        /// Atualiza os beneficiamentos de um produto.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProd">idProd.</param>
        /// <param name="beneficiamentos">beneficiamentos.</param>
        public void AtualizarBenef(GDASession sessao, int idProd, GenericBenefCollection beneficiamentos)
        {
            var temItens = this.VerificarPossuiProduto(sessao, idProd);
            var idOrcamento = this.ObterIdOrcamento(sessao, idProd);

            if (temItens)
            {
                ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(sessao, (uint)idProd);

                foreach (var beneficiamento in beneficiamentos.ToProdutosOrcamento((uint)idProd))
                {
                    ProdutoOrcamentoBenefDAO.Instance.Insert(sessao, beneficiamento);
                }

                this.UpdateValorBenef(sessao, idProd, OrcamentoDAO.Instance.GetElementByPrimaryKey(sessao, idOrcamento));
            }
            else
            {
                this.UpdateTotaisProdutoOrcamento(sessao, this.GetElementByPrimaryKey(sessao, (uint)idProd));
            }
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <param name="orcamento">orcamento.</param>
        public void UpdateValorBenef(GDASession sessao, int idProdOrcamento, Orcamento orcamento)
        {
            var idProduto = this.ObterIdProduto(sessao, idProdOrcamento);

            if (Geral.NaoVendeVidro() || !ProdutoDAO.Instance.CalculaBeneficiamento(sessao, idProduto))
            {
                return;
            }

            var sql = $@"UPDATE produtos_orcamento po
                    SET po.ValorBenef = COALESCE(ROUND((SELECT SUM(pob.Valor) FROM produto_orcamento_benef pob WHERE pob.IdProd = po.IdProd), 2), 0)
                WHERE po.IdProd = {idProduto}";
            this.objPersistence.ExecuteCommand(sessao, sql);

            var produtoOrcamento = this.GetElementByPrimaryKey(sessao, idProdOrcamento);

            if (produtoOrcamento.IdProdParent > 0)
            {
                var temItens = this.VerificarPossuiProduto(sessao, (int)produtoOrcamento.IdProdParent.Value);

                if (temItens)
                {
                    this.UpdateTotaisProdutoOrcamento(sessao, this.GetElementByPrimaryKey(sessao, produtoOrcamento.IdProdParent.Value));
                }
                else
                {
                    produtoOrcamento.IdProdParent = null;
                }
            }

            // Recalcula o total bruto/valor unitário bruto.
            this.UpdateBase(sessao, produtoOrcamento, orcamento);
        }

        /// <summary>
        /// Calcula o valor total do produto com o beneficiamento aplicado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idProdParent">idProdParent.</param>
        public void UpdateValorBenefChild(GDASession sessao, uint idProdParent)
        {
            if (Geral.NaoVendeVidro())
            {
                return;
            }

            var sql = $@"UPDATE produtos_orcamento po
                SET po.ValorBenef = COALESCE(ROUND((
                    SELECT SUM(pob.Valor)
                    FROM produto_orcamento_benef pob
                    WHERE pob.IdProd = po.IdProd), 2), 0)
                WHERE po.IdProdParent = {idProdParent}";

            this.objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Atualiza os totais do produto do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="prodOrca">prodOrca.</param>
        public void UpdateTotaisProdutoOrcamento(GDASession sessao, ProdutosOrcamento prodOrca)
        {
            string sql;

            if (prodOrca.TemItensProdutoSession(sessao))
            {
                this.UpdateValorBenefChild(sessao, prodOrca.IdProd);

                sql = $@"SELECT COALESCE(SUM(ROUND(po.Custo, 2)), 0) FROM produtos_orcamento po
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND po.IdProdParent = {prodOrca.IdProd}";
                var custo = this.objPersistence.ExecuteScalar(sessao, sql).ToString().StrParaDecimal();

                sql = $@"SELECT COALESCE(SUM(ROUND(po.Total + COALESCE(po.ValorBenef, 0), 2)), 0) FROM produtos_orcamento po
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND po.IdProdParent = {prodOrca.IdProd}";
                var total = this.objPersistence.ExecuteScalar(sessao, sql).ToString().StrParaDecimal();

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    var valorDesconto = prodOrca.Desconto;

                    if (prodOrca.TipoDesconto == 1)
                    {
                        valorDesconto = Math.Round(total * (valorDesconto / 100), 2);
                    }

                    total -= valorDesconto;
                }

                sql = $@"UPDATE produtos_orcamento po
                    SET po.Custo = ?custo, po.ValorProd = ?total / COALESCE(po.Qtde, 1), po.Total = ?total
                    WHERE po.IdProd = {prodOrca.IdProd}";
                this.objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?custo", custo), new GDAParameter("?total", total));
            }
            else
            {
                sql = $@"UPDATE produtos_orcamento po
                    SET po.Total = (po.ValorProd * po.Qtde), po.Totalbruto = (po.Valorprod * po.Qtde), po.ValorUnitBruto = po.ValorProd
                    WHERE (po.Total IS NULL OR po.Total = 0) AND po.IdProd = {prodOrca.IdProd}";
                this.objPersistence.ExecuteCommand(sessao, sql);

                if (prodOrca.IdProdParent > 0)
                {
                    var produtoOrcamento = this.GetElementByPrimaryKey(sessao, prodOrca.IdProdParent.Value);
                    this.UpdateTotaisProdutoOrcamento(sessao, produtoOrcamento);
                }
            }
        }

        /// <summary>
        /// Recalcula os valores unitários e totais brutos e líquidos.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="prod">prod.</param>
        /// <param name="somarAcrescimoDesconto">somarAcrescimoDesconto.</param>
        /// <param name="orcamento">orcamento.</param>
        public void RecalcularValores(GDASession session, ProdutosOrcamento prod, bool somarAcrescimoDesconto, Orcamento orcamento)
        {
            var benef = prod.Beneficiamentos;
            var valorBenef = prod.ValorBenef;

            try
            {
                prod.Beneficiamentos = new GenericBenefCollection();
                prod.ValorBenef = 0;

                ValorBruto.Instance.Calcular(session, orcamento, prod);

                if (prod.IdProduto > 0)
                {
                    var valorAtualTabelado = prod.ValorTabela == prod.ValorProd;
                    prod.ValorTabela = (prod as IProdutoCalculo).DadosProduto.ValorTabela();

                    var valorUnitario = ValorUnitario.Instance.RecalcularValor(session, orcamento, prod, !somarAcrescimoDesconto);
                    prod.ValorProd = valorAtualTabelado ? prod.ValorTabela : valorUnitario ?? Math.Max(prod.ValorTabela, prod.ValorProd.GetValueOrDefault());

                    ValorTotal.Instance.Calcular(
                        session,
                        orcamento,
                        prod,
                        Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                        true,
                        prod.Beneficiamentos.CountAreaMinimaSession(session));
                }
            }
            finally
            {
                prod.Beneficiamentos = benef;
                prod.ValorBenef = valorBenef;
            }
        }

        /// <summary>
        /// Atualiza os valores de impostos associados com a instancia informada.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="produtoOrcamento">produtoOrcamento.</param>
        public void AtualizarImpostos(GDASession sessao, ProdutosOrcamento produtoOrcamento)
        {
            // Relação das propriedades que devem ser atualizadas.
            var propriedades = new[]
            {
                nameof(ProdutosOrcamento.IdNaturezaOperacao),
                nameof(ProdutosOrcamento.Mva),
                nameof(ProdutosOrcamento.CodValorFiscal),
                nameof(ProdutosOrcamento.Csosn),
                nameof(ProdutosOrcamento.Cst),
                nameof(ProdutosOrcamento.PercRedBcIcms),
                nameof(ProdutosOrcamento.AliquotaIpi),
                nameof(ProdutosOrcamento.ValorIpi),
                nameof(ProdutosOrcamento.CstIpi),
                nameof(ProdutosOrcamento.AliquotaIcms),
                nameof(ProdutosOrcamento.BcIcms),
                nameof(ProdutosOrcamento.ValorIcms),
                nameof(ProdutosOrcamento.AliqFcp),
                nameof(ProdutosOrcamento.BcFcp),
                nameof(ProdutosOrcamento.ValorFcp),
                nameof(ProdutosOrcamento.AliqIcmsSt),
                nameof(ProdutosOrcamento.BcIcmsSt),
                nameof(ProdutosOrcamento.ValorIcmsSt),
                nameof(ProdutosOrcamento.AliqFcpSt),
                nameof(ProdutosOrcamento.BcFcpSt),
                nameof(ProdutosOrcamento.ValorFcpSt),
                nameof(ProdutosOrcamento.AliqPis),
                nameof(ProdutosOrcamento.BcPis),
                nameof(ProdutosOrcamento.ValorPis),
                nameof(ProdutosOrcamento.CstPis),
                nameof(ProdutosOrcamento.AliqCofins),
                nameof(ProdutosOrcamento.BcCofins),
                nameof(ProdutosOrcamento.ValorCofins),
                nameof(ProdutosOrcamento.CstCofins),
            };

            this.objPersistence.Update(
                sessao,
                produtoOrcamento,
                string.Join(",", propriedades),
                DirectionPropertiesName.Inclusion);
        }

        /// <summary>
        /// Salva a imagem do produto de orçamento filho.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamentoFilho">idProdOrcamentoFilho.</param>
        /// <param name="stream">stream.</param>
        public void SalvarImagemProdutoOrcamento(GDASession session, int idProdOrcamentoFilho, MemoryStream stream)
        {
            var imagemUrlSalvarItem = this.ObterUrlImagemSalvar(idProdOrcamentoFilho);
            ManipulacaoImagem.SalvarImagem(imagemUrlSalvarItem, stream);

            // Cria Log de alteração da Imagem do Produto Pedido
            // Apenas para controle
            LogAlteracaoDAO.Instance.Insert(session, new LogAlteracao
            {
                Tabela = (int)LogAlteracao.TabelaAlteracao.ImagemOrcamento,
                IdRegistroAlt = idProdOrcamentoFilho,
                Campo = "Imagem Produto Orçamento",
                ValorAtual = "Imagem da matéria prima",
                DataAlt = DateTime.Now,
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                Referencia = $"Imagem do Produto Orçamento {idProdOrcamentoFilho}",
                NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(
                    session,
                    LogAlteracao.TabelaAlteracao.ImagemOrcamento,
                    (int)idProdOrcamentoFilho),
            });
        }

        /// <summary>
        /// Calcula o desconto e o valor bruto do produto informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="produto">produto.</param>
        /// <param name="orcamento">orcamento.</param>
        private void CalcularDescontoEValorBrutoProduto(GDASession session, ProdutosOrcamento produto, Orcamento orcamento)
        {
            DescontoAcrescimo.Instance.RemoverDescontoQtde(session, orcamento, produto);
            DescontoAcrescimo.Instance.AplicarDescontoQtde(session, orcamento, produto);
            DiferencaCliente.Instance.Calcular(session, orcamento, produto);
            ValorBruto.Instance.Calcular(session, orcamento, produto);
        }

        #endregion

        #region Apaga dados

        /// <summary>
        /// Exclui todos os produtos de um orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        public void DeleteByOrcamento(GDASession sessao, uint idOrcamento)
        {
            var sql = $@"DELETE FROM produto_orcamento_benef WHERE IdProd IN (SELECT IdProd FROM produtos_orcamento WHERE IdOrcamento = {idOrcamento});
                DELETE FROM produtos_orcamento WHERE IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Insere um produto de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do produto de orçamento inserido.</returns>
        public uint InsertBase(GDASession session, ProdutosOrcamento objInsert)
        {
            var idOrcamento = this.ObterIdOrcamento(session, (int)objInsert.IdProd);
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
            this.CalcularDescontoEValorBrutoProduto(session, objInsert, orcamento);

            objInsert.IdProd = base.Insert(session, objInsert);

            this.AtualizarBenef(session, (int)objInsert.IdProd, objInsert.Beneficiamentos);
            objInsert.RefreshBeneficiamentos();

            return objInsert.IdProd;
        }

        /// <summary>
        /// Insere um produto de orçamento.
        /// </summary>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do produto de orçamento inserido.</returns>
        public override uint Insert(ProdutosOrcamento objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Insere um produto de orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do produto de orçamento inserido.</returns>
        public override uint Insert(GDASession sessao, ProdutosOrcamento objInsert)
        {
            var idOrcamento = this.ObterIdOrcamento(sessao, (int)objInsert.IdProd);
            return this.Insert(sessao, objInsert, OrcamentoDAO.Instance.GetElementByPrimaryKey(sessao, idOrcamento), false);
        }

        /// <summary>
        /// Insere um produto de orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="objInsert">objInsert.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="insercaoComposicao">insercaoComposicao.</param>
        /// <returns>Retorna o ID do produto de orçamento inserido.</returns>
        public uint Insert(GDASession sessao, ProdutosOrcamento objInsert, Orcamento orcamento, bool insercaoComposicao)
        {
            var tipoOrcamento = OrcamentoDAO.Instance.ObterTipoOrcamento(sessao, (int)objInsert.IdOrcamento).GetValueOrDefault();

            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && tipoOrcamento == 0)
            {
                throw new Exception("Informe o tipo do orçamento antes da inserção de produtos.");
            }

            var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;
            var produtosOrcamento = this.ObterProdutosOrcamento(sessao, (int)orcamento.IdOrcamento, null);

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.RemoverAcrescimo(sessao, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.RemoverDesconto(sessao, orcamento, produtosOrcamento);
            }

            if (objInsert.IdProdOrcamentoParent > 0)
            {
                var idProdProdOrcamentoParent = this.ObterIdProduto(sessao, objInsert.IdProdOrcamentoParent.Value);
                var tipoSubgrupoProdProdPedParent = objInsert.IdProdOrcamentoParent > 0
                    ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProdProdOrcamentoParent)
                    : 0;

                // O produto filho que está sendo inserido, não pode ser um produto avô, senão, seria criada uma ligação de bisavô após essa inserção.
                if (tipoSubgrupoProdProdPedParent == TipoSubgrupoProd.VidroDuplo && ProdutoDAO.Instance.VerificarProdutoAvo(sessao, (int)objInsert.IdProd))
                {
                    var codInterno = ProdutoDAO.Instance.GetCodInterno(sessao, (int)objInsert.IdProd);
                    var mensagemBloqueioSubgrupo = "O produto duplo/laminado pode possuir no máximo 2 produtos em sua hierarquia de composição. " +
                        $"Portanto, não é possível inserir o produto {codInterno}, pois, ele possui mais de 2 produtos em sua hierarquia de composição.";

                    throw new Exception();
                }
            }

            if (objInsert.Descricao != null && objInsert.Descricao.Length > 1500)
            {
                objInsert.Descricao = objInsert.Descricao.Substring(0, 1500);
            }

            // Verifica se o produto é do grupo vidro.
            if (ProdutoDAO.Instance.IsVidro(sessao, (int)objInsert.IdProduto.Value))
            {
                // Recupera o id do subgrupo do produto.
                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)objInsert.IdProduto.Value);

                if (idSubgrupoProd > 0)
                {
                    // Se o cálculo for qtde recupera os bneficiamentos inseridos no cadastro do produto.
                    if (SubgrupoProdDAO.Instance.ObtemTipoCalculo(sessao, idSubgrupoProd.Value, false) == (int)TipoCalculoGrupoProd.Qtd)
                    {
                        var prod = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, objInsert.IdProduto.Value);

                        if (prod.Beneficiamentos.Count > 0)
                        {
                            objInsert.Beneficiamentos = prod.Beneficiamentos;
                        }

                        // Busca novamente a altura e largura do produto, caso estejam definidas no cadastro de produto.
                        if (prod.Altura > 0 || prod.Largura > 0)
                        {
                            objInsert.Altura = (float)prod.Altura;
                            objInsert.Largura = prod.Largura.GetValueOrDefault();
                        }
                    }
                }
            }

            var idCliente = OrcamentoDAO.Instance.ObterIdCliente(sessao, (int)objInsert.IdOrcamento);

            if (idCliente > 0 && objInsert.IdProduto.Value > 0)
            {
                ValorTotal.Instance.Calcular(
                    sessao,
                    orcamento,
                    objInsert,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                    true,
                    objInsert.Beneficiamentos.CountAreaMinimaSession(sessao));
            }

            this.CalcularDescontoEValorBrutoProduto(sessao, objInsert, orcamento);

            objInsert.IdProd = base.Insert(sessao, objInsert);

            this.AtualizarBenef(sessao, (int)objInsert.IdProd, objInsert.Beneficiamentos);
            objInsert.RefreshBeneficiamentos();

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.AplicarAcrescimo(sessao, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.AplicarDesconto(sessao, orcamento, produtosOrcamento);
            }

            OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(
                sessao,
                orcamento,
                produtosOrcamento,
                alterarAcrescimoDesconto);

            #region Produtos de composição

            var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)objInsert.IdProduto.GetValueOrDefault());

            // Caso o produto seja do subgrupo de tipo laminado, insere os filhos.
            if (tipoSubgrupoProd == TipoSubgrupoProd.VidroLaminado || tipoSubgrupoProd == TipoSubgrupoProd.VidroDuplo)
            {
                var tipoEntrega = OrcamentoDAO.Instance.ObterTipoEntrega(sessao, (int)objInsert.IdOrcamento);

                foreach (var produtoBaixaEstoque in ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, objInsert.IdProduto.Value, false))
                {
                    var alturaFilho = produtoBaixaEstoque.Altura > 0
                        ? produtoBaixaEstoque.Altura
                        : objInsert.Altura;
                    var larguraFilho = produtoBaixaEstoque.Largura > 0
                        ? produtoBaixaEstoque.Largura
                        : objInsert.Largura;
                    var beneficiamentosComposicaoInserir = produtoBaixaEstoque.Beneficiamentos;

                    if (objInsert.AplicarBenefComposicao)
                    {
                        foreach (var item in objInsert.Beneficiamentos)
                        {
                            if (!beneficiamentosComposicaoInserir.Contains(item))
                            {
                                beneficiamentosComposicaoInserir.Add(item);
                            }
                        }
                    }

                    var valorTabelaProduto = ProdutoDAO.Instance.GetValorTabela(
                        sessao,
                        produtoBaixaEstoque.IdProdBaixa,
                        orcamento.TipoEntrega,
                        (uint?)orcamento.IdCliente,
                        false,
                        false,
                        0F,
                        null,
                        null,
                        (int)orcamento.IdOrcamento);

                    var produtoComposicao = new ProdutosOrcamento();

                    produtoComposicao.IdProdOrcamentoParent = (int)objInsert.IdProd;
                    produtoComposicao.IdProduto = (uint)produtoBaixaEstoque.IdProdBaixa;

                    produtoComposicao.IdProcesso = objInsert.IdProcessoFilhas > 0
                        ? (uint)objInsert.IdProcessoFilhas
                        : (uint)produtoBaixaEstoque.IdProcesso;
                    produtoComposicao.IdAplicacao = objInsert.IdAplicacaoFilhas > 0
                        ? (uint)objInsert.IdAplicacaoFilhas
                        : (uint)produtoBaixaEstoque.IdAplicacao;

                    produtoComposicao.IdOrcamento = objInsert.IdOrcamento;
                    produtoComposicao.IdProdParent = objInsert.IdProdParent;
                    produtoComposicao.Qtde = produtoBaixaEstoque.Qtde;
                    produtoComposicao.Altura = alturaFilho;
                    produtoComposicao.AlturaCalc = alturaFilho;
                    produtoComposicao.Largura = larguraFilho;
                    produtoComposicao.IdProdBaixaEst = produtoBaixaEstoque.IdProdBaixaEst;
                    produtoComposicao.ValorProd = valorTabelaProduto;
                    produtoComposicao.Beneficiamentos = beneficiamentosComposicaoInserir;

                    produtoComposicao.IdProd = this.Insert(sessao, produtoComposicao, orcamento, true);

                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IProdutoBaixaEstoqueRepositorioImagens>();
                    var stream = new MemoryStream();

                    // Verifica se a matéria prima possui imagem.
                    var possuiImagem = repositorio.ObtemImagem(produtoBaixaEstoque.IdProdBaixaEst, stream);

                    if (possuiImagem)
                    {
                        this.SalvarImagemProdutoOrcamento(sessao, (int)produtoComposicao.IdProd, stream);
                    }
                }
            }

            // Atualiza o produto pai.
            if (!insercaoComposicao && objInsert.IdProdOrcamentoParent > 0)
            {
                var produtoOrcamentoPai = this.GetElement(sessao, (uint)objInsert.IdProdOrcamentoParent);
                var idSubgrupoProdutoPai = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)produtoOrcamentoPai.IdProduto);

                if (idSubgrupoProdutoPai != TipoSubgrupoProd.VidroLaminado)
                {
                    produtoOrcamentoPai.ValorProd = this.ObterValorVendidoProdutoPai(sessao, objInsert.IdProdOrcamentoParent.Value);
                    produtoOrcamentoPai.Beneficiamentos = this.ObterBeneficiamentos(sessao, objInsert.IdProdOrcamentoParent.Value);

                    this.Update(sessao, produtoOrcamentoPai);
                }
            }

            #endregion

            this.UpdateTotaisProdutoOrcamento(sessao, this.GetElementByPrimaryKey(sessao, objInsert.IdProdParent.Value));

            orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(sessao, objInsert.IdOrcamento);
            OrcamentoDAO.Instance.UpdateTotaisOrcamento(sessao, orcamento, true, false);

            return objInsert.IdProd;
        }

        /// <summary>
        /// Atualiza o produto de orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="objUpdate">objUpdate.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <returns>Retorna o ID do produto atualizado.</returns>
        public int UpdateBase(GDASession sessao, ProdutosOrcamento objUpdate, Orcamento orcamento)
        {
            if (objUpdate.IdProduto > 0)
            {
                this.CalcularDescontoEValorBrutoProduto(sessao, objUpdate, orcamento);
            }

            return base.Update(sessao, objUpdate);
        }

        /// <summary>
        /// Atualiza o produto de orçamento informado.
        /// </summary>
        /// <param name="objUpdate">objUpdate.</param>
        /// <returns>Retorna o ID do produto de orçamento atualizado.</returns>
        public int UpdateComTransacao(ProdutosOrcamento objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Atualiza o produto de orçamento informado.
        /// </summary>
        /// <param name="objUpdate">objUpdate.</param>
        /// <returns>Retorna o ID do produto de orçamento atualizado.</returns>
        public override int Update(ProdutosOrcamento objUpdate)
        {
            return this.UpdateComTransacao(objUpdate);
        }

        /// <summary>
        /// Atualiza um produto de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objUpdate">objUpdate.</param>
        /// <returns>Retorna o ID do produto de orçamento atualizado.</returns>
        public override int Update(GDASession session, ProdutosOrcamento objUpdate)
        {
            var produtoOrcamento = this.GetElementByPrimaryKey(session, objUpdate.IdProd);
            objUpdate.IdOrcamento = produtoOrcamento.IdOrcamento;
            var orcamento = OrcamentoDAO.Instance.GetElement(session, objUpdate.IdOrcamento);

            produtoOrcamento.PercDescontoQtde = objUpdate.PercDescontoQtde;

            // Verifica se este orçamento pode ter desconto
            var msgErro = string.Empty;

            if (!OrcamentoDAO.Instance.DescontoPermitido(session, orcamento, out msgErro))
            {
                throw new Exception(msgErro);
            }

            var tamanhoMinimoBisote = PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComBisote;
            var tamanhoMinimoLapidacao = PedidoConfig.TamanhoVidro.AlturaELarguraMinimaParaPecasComLapidacao;
            var tamanhoMinimoTemperado = PedidoConfig.TamanhoVidro.AlturaELarguraMinimasParaPecasTemperadas;

            var retornoValidacao = string.Empty;

            if (objUpdate.Beneficiamentos != null)
            {
                foreach (var prodBenef in objUpdate.Beneficiamentos)
                {
                    if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == TipoControleBenef.Bisote &&
                        (objUpdate.Altura < tamanhoMinimoBisote || objUpdate.Largura < tamanhoMinimoBisote))
                    {
                        retornoValidacao += $"A altura ou largura minima para peças com bisotê é de {tamanhoMinimoBisote}mm.";
                    }

                    if (BenefConfigDAO.Instance.GetElement(prodBenef.IdBenefConfig).TipoControle == TipoControleBenef.Lapidacao &&
                        (objUpdate.Altura < tamanhoMinimoLapidacao || objUpdate.Largura < tamanhoMinimoLapidacao))
                    {
                        retornoValidacao += $"A altura ou largura minima para peças com lapidação é de {tamanhoMinimoLapidacao}mm.";
                    }
                }
            }

            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)objUpdate.IdProduto);
            var idSubGrupoProd = (int?)ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)objUpdate.IdProduto);

            if (GrupoProdDAO.Instance.IsVidroTemperado(session, idGrupoProd, idSubGrupoProd)
                && objUpdate.Altura < tamanhoMinimoTemperado && objUpdate.Largura < tamanhoMinimoTemperado)
            {
                retornoValidacao += $"A altura ou largura minima para peças com tempera é de {tamanhoMinimoTemperado}mm.";
            }

            if (!string.IsNullOrWhiteSpace(retornoValidacao))
            {
                throw new Exception(retornoValidacao);
            }

            var dadosProdutoOrcamentoForamAlterados = objUpdate.Qtde != produtoOrcamento.Qtde ||
                objUpdate.Altura != produtoOrcamento.Altura ||
                objUpdate.Largura != produtoOrcamento.Largura ||
                objUpdate.ValorProd != produtoOrcamento.ValorProd;
            var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;
            var produtosOrcamento = this.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.RemoverDesconto(session, orcamento, produtosOrcamento);
            }

            if (Geral.NaoVendeVidro())
            {
                var codInterno = ProdutoDAO.Instance.GetCodInterno(session, (int)objUpdate.IdProduto.Value);
                var descricao = ProdutoDAO.Instance.GetDescrProduto(session, (int)objUpdate.IdProduto.Value);

                objUpdate.Descricao = $"{codInterno} - {descricao}";

                ValorTotal.Instance.Calcular(
                    session,
                    orcamento,
                    objUpdate,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                    true,
                    0,
                    primeiroCalculo: true);
            }

            if (objUpdate.IdProdParent.GetValueOrDefault() == 0)
            {
                objUpdate.IdProdParent = produtoOrcamento.IdProdParent;
            }

            // Atualiza o produto.
            produtoOrcamento.Descricao = objUpdate.Descricao;
            produtoOrcamento.Qtde = objUpdate.Qtde;
            produtoOrcamento.ValorProd = objUpdate.ValorProd;
            produtoOrcamento.Ambiente = objUpdate.Ambiente;

            /* Chamado 37451.
             * Este campo é imprescindível para o cálculo do alumínio no pedido. */
            produtoOrcamento.Altura = objUpdate.Altura;
            produtoOrcamento.AlturaCalc = objUpdate.AlturaCalc;
            produtoOrcamento.Largura = objUpdate.Largura;
            produtoOrcamento.TotMCalc = objUpdate.TotMCalc;
            produtoOrcamento.Custo = objUpdate.Custo;
            produtoOrcamento.PercDescontoQtde = objUpdate.PercDescontoQtde;

            var idCliente = OrcamentoDAO.Instance.ObterIdCliente(session, (int)objUpdate.IdOrcamento);
            var qtde = objUpdate.Qtde.GetValueOrDefault();
            var altura = objUpdate.Altura;
            var totM2 = objUpdate.TotM;
            var totM2Calc = objUpdate.TotMCalc;
            var valorProd = objUpdate.ValorProd.GetValueOrDefault();
            var custoProd = objUpdate.Custo;
            var total = objUpdate.Total.GetValueOrDefault();
            var redondo = objUpdate.Redondo ||
                (objUpdate.IdProdParent > 0
                    ? this.ObterRedondo(session, (int)objUpdate.IdProdParent.Value)
                    : false);

            ProdutoDAO.Instance.CalcTotaisItemProd(
                session,
                (uint)idCliente,
                (int)objUpdate.IdProduto,
                objUpdate.Largura,
                qtde,
                1,
                valorProd,
                objUpdate.Espessura,
                redondo,
                0,
                false,
                true,
                ref custoProd,
                ref altura,
                ref totM2,
                ref totM2Calc,
                ref total,
                2,
                2,
                false,
                objUpdate.Beneficiamentos.CountAreaMinimaSession(session),
                true);

            produtoOrcamento.TotM = objUpdate.TotM = totM2;
            produtoOrcamento.Total = objUpdate.Total = total;
            produtoOrcamento.Custo = objUpdate.Custo = custoProd;
            produtoOrcamento.TotMCalc = objUpdate.TotMCalc = totM2Calc;

            if (ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)objUpdate.IdProduto) == (int)NomeGrupoProd.Vidro && objUpdate.Espessura == 0)
            {
                objUpdate.Espessura = ProdutoDAO.Instance.ObtemEspessura(session, (int)objUpdate.IdProd);
            }

            if (!OrcamentoDAO.Instance.DescontoPermitido(session, orcamento, out msgErro))
            {
                throw new Exception(msgErro);
            }

            if (!this.ValidarDesconto(
                session,
                objUpdate.IdOrcamento,
                objUpdate.TipoDesconto,
                objUpdate.Desconto,
                produtoOrcamento.TotalBruto))
            {
                throw new Exception("Desconto acima do permitido.");
            }

            if (objUpdate.Total > 0)
            {
                produtoOrcamento.Total = objUpdate.Total;
            }
            else if (objUpdate.ValorProd != null && objUpdate.Qtde != null)
            {
                produtoOrcamento.Total = (decimal)objUpdate.Qtde * objUpdate.ValorProd;
            }
            else
            {
                produtoOrcamento.Total = null;
            }

            produtoOrcamento.IdProd = (uint)this.UpdateBase(session, produtoOrcamento, orcamento);

            this.AtualizarBenef(session, (int)objUpdate.IdProd, objUpdate.Beneficiamentos);
            objUpdate.RefreshBeneficiamentos();

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.AplicarAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.AplicarDesconto(session, orcamento, produtosOrcamento);
            }

            OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(
                session,
                orcamento,
                produtosOrcamento,
                alterarAcrescimoDesconto);

            #region Produto composição

            var tipoSubgrupo = objUpdate.IdProdOrcamentoParent > 0
                ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, objUpdate.IdProdOrcamentoParent.Value)
                : (TipoSubgrupoProd?)null;

            // Se for produto de composição atualiza o valor do pai.
            if (objUpdate.IdProdOrcamentoParent > 0 &&
                !PedidoConfig.NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura &&
                dadosProdutoOrcamentoForamAlterados)
            {
                var produtoOrcamentoPai = this.GetElement(session, (uint)objUpdate.IdProdOrcamentoParent);

                if (tipoSubgrupo != TipoSubgrupoProd.VidroLaminado)
                {
                    produtoOrcamentoPai.ValorProd = this.ObterValorVendidoProdutoPai(session, objUpdate.IdProdOrcamentoParent.Value);
                    produtoOrcamentoPai.Beneficiamentos = this.ObterBeneficiamentos(session, objUpdate.IdProdOrcamentoParent.Value);

                    this.Update(session, produtoOrcamentoPai);
                }
            }

            if (tipoSubgrupo == TipoSubgrupoProd.VidroLaminado)
            {
                foreach (var produtoOrcamentoComposicao in this.ObterFilhosComposicao(session, new List<int> { (int)objUpdate.IdProd }))
                {
                    var composicaoAlterada = false;

                    if (produtoOrcamentoComposicao.Altura != objUpdate.Altura ||
                        produtoOrcamentoComposicao.Largura != objUpdate.Largura)
                    {
                        produtoOrcamentoComposicao.Altura = objUpdate.Altura;
                        produtoOrcamentoComposicao.Largura = objUpdate.Largura;
                        composicaoAlterada = true;

                        this.Update(session, produtoOrcamentoComposicao);
                    }

                    if (objUpdate.AplicarBenefComposicao)
                    {
                        produtoOrcamentoComposicao.Beneficiamentos = objUpdate.Beneficiamentos;
                    }

                    if (composicaoAlterada)
                    {
                        this.Update(session, produtoOrcamentoComposicao);
                    }
                }
            }

            #endregion

            this.AtualizarBenef(session, (int)objUpdate.IdProd, objUpdate.Beneficiamentos);
            objUpdate.RefreshBeneficiamentos();

            if (objUpdate.IdProdParent != null)
            {
                this.UpdateTotaisProdutoOrcamento(session, this.GetElementByPrimaryKey(session, objUpdate.IdProdParent.Value));
            }

            orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, produtoOrcamento.IdOrcamento);
            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, orcamento, true, false);

            return (int)produtoOrcamento.IdProd;
        }

        /// <summary>
        /// Apaga um produto de orçamento.
        /// </summary>
        /// <param name="objDelete">objDelete.</param>
        /// <returns>Retorna o ID do produto removido.</returns>
        public override int Delete(ProdutosOrcamento objDelete)
        {
            return this.DeleteByPrimaryKey(objDelete.IdProd);
        }

        /// <summary>
        /// Apaga um produto de orçamento.
        /// </summary>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o ID do produto removido.</returns>
        public override int DeleteByPrimaryKey(uint idProdOrcamento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.DeleteByPrimaryKey(transaction, idProdOrcamento);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception($"Falha ao remover o produto do orçamento. Erro: {ex.Message.Replace("'", string.Empty)}.");
                }
            }
        }

        /// <summary>
        /// Apaga um produto de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o ID do produto removido.</returns>
        public override int DeleteByPrimaryKey(GDASession session, uint idProdOrcamento)
        {
            int returnValue;

            if (!this.Exists(session, idProdOrcamento))
            {
                return 0;
            }

            var idOrcamento = this.ObterIdOrcamento(session, (int)idProdOrcamento);
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
            var idItemProjeto = this.ObterIdItemProjeto(session, (int)idProdOrcamento);
            var idProdParent = this.ObterIdProdAmbiente(session, (int)idProdOrcamento);
            var idProdOrcamentoParent = this.ObterIdProdOrcamentoParent(session, (int)idProdOrcamento);
            var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;
            var produtosOrcamento = this.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.RemoverDesconto(session, orcamento, produtosOrcamento);
            }

            // Se for o pai, do produto de composição, deleta os filhos.
            foreach (var produtoOrcamentoComposicao in this.ObterFilhosComposicao(session, new List<int> { (int)idProdOrcamento }))
            {
                this.DeleteByPrimaryKey(session, produtoOrcamentoComposicao.IdProd);
            }

            foreach (var produtoOrcamento in produtosOrcamento?.Where(f => f.IdProdParent == idProdOrcamento)?.ToList())
            {
                this.DeleteByPrimaryKey(session, produtoOrcamento.IdProd);
            }

            ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(session, idProdOrcamento);

            // Exclui item_projeto associado à este produto.
            if (idItemProjeto > 0)
            {
                var sqlItemProjetoOrcamentoExiste = $@"SELECT COUNT(*) FROM item_projeto 
                    WHERE IdOrcamento = {idOrcamento} AND
                        IdItemProjeto = {idItemProjeto} AND
                        (IdProjeto IS NULL OR IdProjeto = 0)";
                var itemProjetoOrcamentoExiste = this.objPersistence.ExecuteSqlQueryCount(session, sqlItemProjetoOrcamentoExiste) > 0;

                if (itemProjetoOrcamentoExiste)
                {
                    ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, idItemProjeto.Value);
                }
            }

            returnValue = GDAOperations.Delete(session, new ProdutosOrcamento { IdProd = idProdOrcamento });

            // Se estiver deletando um filho, recalcula o valor do pai.
            if (idProdOrcamentoParent > 0)
            {
                var produtoOrcamentoPai = this.GetElement(session, (uint)idProdOrcamentoParent);

                if (produtoOrcamentoPai?.IdProd > 0)
                {
                    produtoOrcamentoPai.ValorProd = this.ObterValorVendidoProdutoPai(session, (int)produtoOrcamentoPai.IdProd);
                    produtoOrcamentoPai.Beneficiamentos = this.ObterBeneficiamentos(session, (int)produtoOrcamentoPai.IdProd);

                    this.Update(session, produtoOrcamentoPai);
                }
            }

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.AplicarAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.AplicarDesconto(session, orcamento, produtosOrcamento);
            }

            OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(
                session,
                orcamento,
                produtosOrcamento,
                alterarAcrescimoDesconto);

            if (idProdParent > 0)
            {
                this.UpdateTotaisProdutoOrcamento(session, this.GetElementByPrimaryKey(session, (uint)idProdParent.Value));
            }

            orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, orcamento, true, false);

            return returnValue;
        }

        #endregion

        #region Projeto orçamento

        #region Insere/Atualiza Produto de Projeto

        /// <summary>
        /// Insere/atualiza um produto de orçamento, associado a um material de projeto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="itemProjeto">itemProjeto.</param>
        /// <param name="medidasAlteradas">medidasAlteradas.</param>
        /// <returns>Retorna o ID do produto inserido.</returns>
        public int InsereAtualizaProdProj(
            GDASession session,
            int idOrcamento,
            int? idProdAmbienteOrcamento,
            ItemProjeto itemProjeto,
            bool medidasAlteradas)
        {
            try
            {
                var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
                var idCliente = OrcamentoDAO.Instance.ObterIdCliente(session, idOrcamento);
                var tipoEntrega = OrcamentoDAO.Instance.ObterTipoEntrega(session, idOrcamento);
                var percComissao = OrcamentoDAO.Instance.RecuperaPercComissao(session, orcamento.IdOrcamento);

                if (idProdAmbienteOrcamento.GetValueOrDefault() == 0)
                {
                    idProdAmbienteOrcamento = this.ObterIdProdOrcamentoPeloIdItemProjeto(session, (int)itemProjeto.IdItemProjeto);
                }

                var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;
                var produtosOrcamento = this.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);

                if (orcamento.Acrescimo > 0)
                {
                    OrcamentoDAO.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
                }

                if (orcamento.Desconto > 0)
                {
                    OrcamentoDAO.Instance.RemoverDesconto(session, orcamento, produtosOrcamento);
                }

                var produtoAmbienteOrcamento = new ProdutosOrcamento();
                produtoAmbienteOrcamento.IdOrcamento = (uint)idOrcamento;
                produtoAmbienteOrcamento.IdItemProjeto = itemProjeto.IdItemProjeto;
                produtoAmbienteOrcamento.Ambiente = !string.IsNullOrWhiteSpace(itemProjeto.Ambiente) ? itemProjeto.Ambiente : "Cálculo Projeto";

                var descricao = UtilsProjeto.FormataTextoOrcamento(session, itemProjeto);

                if (!string.IsNullOrWhiteSpace(descricao))
                {
                    produtoAmbienteOrcamento.Descricao = descricao;
                }

                // Colocado para centerbox e megatemper ficar com valor correto de fechamentos na impressão.
                produtoAmbienteOrcamento.Qtde = produtoAmbienteOrcamento.Qtde > 0 ? produtoAmbienteOrcamento.Qtde : 1;

                // Se o ambiente não tiver sido informado, insere ambiente orçamento, senão apenas atualiza texto.
                if (idProdAmbienteOrcamento.GetValueOrDefault() == 0)
                {
                    produtoAmbienteOrcamento.IdProd = this.Insert(session, produtoAmbienteOrcamento);
                    idProdAmbienteOrcamento = (int)produtoAmbienteOrcamento.IdProd;
                }
                else
                {
                    var produtoOrcamentoAmbienteAtual = this.GetElementByPrimaryKey(session, idProdAmbienteOrcamento.Value);

                    produtoAmbienteOrcamento.TipoAcrescimo = produtoOrcamentoAmbienteAtual.TipoAcrescimo;
                    produtoAmbienteOrcamento.TipoDesconto = produtoOrcamentoAmbienteAtual.TipoDesconto;
                    produtoAmbienteOrcamento.Desconto = produtoOrcamentoAmbienteAtual.Desconto;
                    produtoAmbienteOrcamento.Acrescimo = produtoOrcamentoAmbienteAtual.Acrescimo;
                    produtoAmbienteOrcamento.IdProd = (uint)idProdAmbienteOrcamento.Value;

                    if (string.IsNullOrWhiteSpace(produtoAmbienteOrcamento.Descricao))
                    {
                        produtoAmbienteOrcamento.Descricao = produtoOrcamentoAmbienteAtual.Descricao;
                    }

                    this.Update(session, produtoAmbienteOrcamento);
                }

                var materiaisItemProjeto = MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, itemProjeto.IdItemProjeto);

                // Insere materiais do item projeto no ambiente.
                foreach (var materialItemProjeto in materiaisItemProjeto)
                {
                    var idProdOrcamento = this.ObterIdProdOrcamentoPeloIdMaterialItemProjeto(session, (int)materialItemProjeto.IdMaterItemProj);
                    var produtoOrcamento = new ProdutosOrcamento();

                    produtoOrcamento.IdProd = (uint)idProdOrcamento.GetValueOrDefault();
                    produtoOrcamento.IdOrcamento = (uint)idOrcamento;
                    produtoOrcamento.IdProdParent = produtoAmbienteOrcamento.IdProd;
                    produtoOrcamento.IdItemProjeto = itemProjeto.IdItemProjeto;
                    produtoOrcamento.IdMaterItemProj = (int)materialItemProjeto.IdMaterItemProj;
                    produtoOrcamento.IdProduto = materialItemProjeto.IdProd;
                    produtoOrcamento.IdProcesso = materialItemProjeto.IdProcesso;
                    produtoOrcamento.IdAplicacao = materialItemProjeto.IdAplicacao;
                    produtoOrcamento.Redondo = materialItemProjeto.Redondo;
                    produtoOrcamento.Qtde = materialItemProjeto.Qtde;
                    produtoOrcamento.AlturaCalc = materialItemProjeto.AlturaCalc;
                    produtoOrcamento.Altura = materialItemProjeto.Altura;
                    produtoOrcamento.Largura = materialItemProjeto.Largura;
                    produtoOrcamento.TotM = materialItemProjeto.TotM;
                    produtoOrcamento.TotMCalc = materialItemProjeto.TotM2Calc;
                    produtoOrcamento.ValorProd = materialItemProjeto.Valor;
                    produtoOrcamento.Total = materialItemProjeto.Total;
                    produtoOrcamento.Custo = materialItemProjeto.Custo;
                    produtoOrcamento.Espessura = materialItemProjeto.Espessura;
                    produtoOrcamento.AliquotaIcms = materialItemProjeto.AliqIcms;
                    produtoOrcamento.ValorIcms = materialItemProjeto.ValorIcms;
                    produtoOrcamento.AliquotaIpi = materialItemProjeto.AliquotaIpi;
                    produtoOrcamento.ValorIpi = materialItemProjeto.ValorIpi;
                    produtoOrcamento.ValorAcrescimo = materialItemProjeto.ValorAcrescimo;
                    produtoOrcamento.ValorDesconto = materialItemProjeto.ValorDesconto;
                    produtoOrcamento.Beneficiamentos = materialItemProjeto.Beneficiamentos;

                    ValorBruto.Instance.Calcular(session, orcamento, produtoOrcamento);
                    ValorUnitario.Instance.Calcular(session, orcamento, produtoOrcamento);

                    produtoOrcamento.IdProd = (uint)this.InsertOrUpdateFromProjeto(session, produtoOrcamento);

                    // Chamado 49030.
                    if (!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto && produtoOrcamento.ValorProd != materialItemProjeto.Valor)
                    {
                        MaterialItemProjeto material = materialItemProjeto;

                        // Verifica qual preço deverá ser utilizado
                        material.Valor = ProdutoDAO.Instance.GetValorTabela(
                            session,
                            (int)materialItemProjeto.IdProd,
                            tipoEntrega,
                            (uint)idCliente,
                            false,
                            itemProjeto.Reposicao,
                            0F,
                            null,
                            null,
                            idOrcamento);

                        MaterialItemProjetoDAO.Instance.CalcTotais(session, ref material, false);
                        MaterialItemProjetoDAO.Instance.UpdateBase(session, material);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(session, itemProjeto.IdItemProjeto);
                    }
                }

                // Aplica comissão.
                ProdutosOrcamento[] prod = { this.GetElementByPrimaryKey(session, idProdAmbienteOrcamento.GetValueOrDefault()) };

                if (PedidoConfig.Comissao.ComissaoAlteraValor)
                {
                    DescontoAcrescimo.Instance.AplicarComissao(session, orcamento, percComissao, prod);
                }

                if (orcamento.Acrescimo > 0)
                {
                    OrcamentoDAO.Instance.AplicarAcrescimo(session, orcamento, produtosOrcamento);
                }

                if (orcamento.Desconto > 0)
                {
                    OrcamentoDAO.Instance.AplicarDesconto(session, orcamento, produtosOrcamento);
                }

                OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(
                    session,
                    orcamento,
                    produtosOrcamento,
                    alterarAcrescimoDesconto);

                // Aplica acréscimo e desconto no ambiente.
                if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
                {
                    if (produtoAmbienteOrcamento.Acrescimo > 0)
                    {
                        this.AplicarAcrescimoAmbiente(
                            session,
                            produtoAmbienteOrcamento,
                            orcamento,
                            this.ObterProdutosOrcamento(session, (int)produtoAmbienteOrcamento.IdOrcamento, (int)produtoAmbienteOrcamento.IdProd));
                    }

                    if (produtoAmbienteOrcamento.Desconto > 0)
                    {
                        this.AplicarDescontoAmbiente(
                            session,
                            produtoAmbienteOrcamento,
                            orcamento,
                            this.ObterProdutosOrcamento(session, (int)produtoAmbienteOrcamento.IdOrcamento, (int)produtoAmbienteOrcamento.IdProd));
                    }
                }

                orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, (uint)idOrcamento);

                // Atualiza o total do orçamento.
                OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, orcamento, false, false);

                return (int)produtoAmbienteOrcamento.IdProd;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Insere/atualiza um produto de orçamento, associado a um material de projeto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="produtoOrcamento">produtoOrcamento.</param>
        /// <returns>Retorna o ID do produto de orçamento inserido/atualizado.</returns>
        public int InsertOrUpdateFromProjeto(GDASession session, ProdutosOrcamento produtoOrcamento)
        {
            try
            {
                var orcamento = PedidoDAO.Instance.GetElementByPrimaryKey(session, produtoOrcamento.IdOrcamento);

                ValorTotal.Instance.Calcular(
                    session,
                    orcamento,
                    produtoOrcamento,
                    Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarEAtualizarProduto,
                    true,
                    produtoOrcamento.Beneficiamentos.CountAreaMinimaSession(session)
                );

                if (produtoOrcamento.IdProd > 0 && this.Exists(session, produtoOrcamento.IdProd))
                {
                    this.AtualizarBenef(session, (int)produtoOrcamento.IdProd, produtoOrcamento.Beneficiamentos);
                    produtoOrcamento.RefreshBeneficiamentos();

                    return (int)produtoOrcamento.IdProd;
                }
                else
                {
                    return (int)this.InsertBase(session, produtoOrcamento);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao incluir Produto no Pedido. Erro: " + ex.Message);
            }
        }

        #endregion

        #region Inserir produtos pelo item de projeto (orçamentos antigos)

        /// <summary>
        /// Obtém os ID's de item de projeto sem produto de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna os ID's de itens de projeto que não possuem produtos de orçamento.</returns>
        internal List<int> ObterIdsItemProjetoSemProdutoOrcamento(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0 || !OrcamentoDAO.Instance.Exists(session, idOrcamento))
            {
                return new List<int>();
            }

            var sqlIdsItemProjetoSemProdutoOrcamento = $@"SELECT DISTINCT(ip.IdItemProjeto) FROM item_projeto ip
                    INNER JOIN material_item_projeto mip ON (ip.IdItemProjeto = mip.IdItemProjeto)
                    LEFT JOIN produtos_orcamento po ON (mip.IdMaterItemProj = po.IdMaterItemProj)
                WHERE po.IdProd IS NULL AND ip.IdOrcamento = {idOrcamento};";

            return this.ExecuteMultipleScalar<int>(session, sqlIdsItemProjetoSemProdutoOrcamento);
        }

        /// <summary>
        /// Insere produtos de orçamento para os itens de projeto que não possuem produtos de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        public void InserirProdutosOrcamentoPelosIdsItemProjeto(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0 || !OrcamentoDAO.Instance.Exists(session, idOrcamento))
            {
                return;
            }

            var idsItemProjeto = this.ObterIdsItemProjetoSemProdutoOrcamento(session, idOrcamento);

            if (!idsItemProjeto.Any(f => f > 0))
            {
                return;
            }

            foreach (var idItemProjeto in idsItemProjeto.Where(f => f > 0))
            {
                var idProdAmbiente = this.ObterIdProdOrcamentoPeloIdItemProjeto(session, idItemProjeto);

                if (idProdAmbiente.GetValueOrDefault() == 0 || !Exists(session, idProdAmbiente.Value))
                {
                    continue;
                }

                if ((this.ObterIdsMaterialItemProjetoPeloItemProjeto(session, idItemProjeto)?.Any(f => f > 0)).GetValueOrDefault())
                {
                    continue;
                }

                var itemProjeto = ItemProjetoDAO.Instance.GetElement(session, (uint)idItemProjeto);

                if ((itemProjeto?.IdItemProjeto).GetValueOrDefault() == 0)
                {
                    continue;
                }

                this.InsereAtualizaProdProj(session, idOrcamento, idProdAmbiente, itemProjeto, true);
            }
        }

        #endregion

        #region Imagem

        /// <summary>
        /// Obtém as imagens de projeto, associadas aos produtos informados.
        /// </summary>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <param name="indice">indice.</param>
        public void ObterImagemProjeto(List<ProdutosOrcamento> produtosOrcamento, int indice)
        {
            if (OrcamentoConfig.UploadImagensOrcamento && File.Exists($"{Utils.GetProdutosOrcamentoPath}{produtosOrcamento[indice].NomeImagem}"))
            {
                produtosOrcamento[indice].ImagemProjModPath = $"file:///{Utils.GetProdutosOrcamentoPath.Replace("\\", "/")}{produtosOrcamento[indice].NomeImagem}";
            }
            else if (produtosOrcamento[indice].IdItemProjeto > 0 && produtosOrcamento[indice].IdMaterItemProj.GetValueOrDefault() == 0)
            {
                // Pega a imagem do modelo do projeto associada à este item.
                var nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(null, produtosOrcamento[indice].IdItemProjeto.Value);

                if (!string.IsNullOrEmpty(nomeFigura))
                {
                    produtosOrcamento[indice].ImagemProjModPath = $"file:///{Utils.GetModelosProjetoPath.Replace("\\", "/")}{nomeFigura}";
                }
            }
        }

        #endregion

        #region Apagar produtos

        /// <summary>
        /// Exclui todos os produtos de um orçamento que vieram de um projeto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        public void DeleteFromProjeto(GDASession session, uint idOrcamento)
        {
            var sqlApagarProdutoOrcamentoBenef = $@"DELETE FROM produto_orcamento_benef
                WHERE IdProd IN 
                    (SELECT IdProd FROM produtos_orcamento
                    WHERE IdItemProjeto IS NOT NULL AND
                        IdItemProjeto > 0 AND
                        IdOrcamento = {idOrcamento});";
            var sqlApagarProdutoOrcamento = $@"DELETE FROM produtos_orcamento
                WHERE IdItemProjeto IS NOT NULL AND
                    IdItemProjeto > 0 AND
                    IdOrcamento = {idOrcamento};";

            this.objPersistence.ExecuteCommand(session, sqlApagarProdutoOrcamentoBenef);
            this.objPersistence.ExecuteCommand(session, sqlApagarProdutoOrcamento);
        }

        /// <summary>
        /// Apaga o produto de ambiente informado e todos os produtos associados a ele.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>Retorna o ID do produto de orçamento removido.</returns>
        public int DeleteByPrimaryKeyExcluirProjeto(GDASession session, uint idProdOrcamento)
        {
            int returnValue;

            if (!this.Exists(session, idProdOrcamento))
            {
                return 0;
            }

            var idOrcamento = this.ObterIdOrcamento(session, (int)idProdOrcamento);
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
            var idItemProjeto = this.ObterIdItemProjeto(session, (int)idProdOrcamento);
            var idProdParent = this.ObterIdProdAmbiente(session, (int)idProdOrcamento);
            var alterarAcrescimoDesconto = orcamento.Acrescimo > 0 || orcamento.Desconto > 0;
            var produtosOrcamento = alterarAcrescimoDesconto
                ? this.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null)
                : null;

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.RemoverDesconto(session, orcamento, produtosOrcamento);
            }

            ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(session, idProdOrcamento);

            foreach (var produtosOrcamentoDoAmbiente in this.ObterProdutosOrcamento(session, idOrcamento, (int)idProdOrcamento))
            {
                this.DeleteByPrimaryKey(session, produtosOrcamentoDoAmbiente.IdProd);
            }

            // Exclui item_projeto associado à este produto.
            if (idItemProjeto > 0)
            {
                var sqlExcluirItemProjeto = $@"SELECT COUNT(*) FROM item_projeto
                    WHERE IdOrcamento = {idOrcamento} AND
                        IdItemProjeto = {idItemProjeto} AND
                        (IdProjeto IS NULL OR IdProjeto = 0)";

                if (this.objPersistence.ExecuteSqlQueryCount(session, sqlExcluirItemProjeto) > 0)
                {
                    ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, idItemProjeto.Value);
                }
            }

            if (orcamento.Acrescimo > 0)
            {
                OrcamentoDAO.Instance.AplicarAcrescimo(session, orcamento, produtosOrcamento);
            }

            if (orcamento.Desconto > 0)
            {
                OrcamentoDAO.Instance.AplicarDesconto(session, orcamento, produtosOrcamento);
            }

            OrcamentoDAO.Instance.FinalizarAplicacaoComissaoAcrescimoDesconto(session, orcamento, produtosOrcamento, alterarAcrescimoDesconto);

            returnValue = GDAOperations.Delete(session, new ProdutosOrcamento { IdProd = idProdOrcamento });

            if (idProdParent != null)
            {
                this.UpdateTotaisProdutoOrcamento(session, this.GetElementByPrimaryKey(session, idProdParent.Value));
            }

            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento), true, false);

            return returnValue;
        }

        #endregion

        #endregion

        #region Produto orcamento

        /// <summary>
        /// Sql base para recuperar produtos de orçamento.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idsProdOrcamento">idsProdOrcamento.</param>
        /// <param name="selecionar">selecionar.</param>
        /// <returns>Retorna um SQL base para a busca de produtos de orçamento.</returns>
        private string SqlProdutoOrcamento(int? idOrcamento, int? idProdAmbienteOrcamento, List<int> idsProdOrcamento, bool selecionar)
        {
            var campos = string.Empty;

            if (selecionar)
            {
                var sqlValorTabelaProduto = $"IF(o.TipoEntrega IN ({(int)Orcamento.TipoEntregaOrcamento.Balcao}, {(int)Orcamento.TipoEntregaOrcamento.Entrega}), p.ValorBalcao, p.ValorObra)";
                campos = $@"po.*, p.CodInterno, p.Descricao AS DescrProduto, apl.CodInterno AS CodAplicacao, prc.CodInterno AS CodProcesso,
                    {sqlValorTabelaProduto} AS ValorProdutoTabela, IF(p.AtivarAreaMinima = 1, CAST(p.AreaMinima AS CHAR), '0') AS AreaMinima";
            }
            else
            {
                campos = "COUNT(*)";
            }

            var sql = $@"SELECT {campos} FROM produtos_orcamento po
                    INNER JOIN orcamento o ON (po.IdOrcamento = o.IdOrcamento)
                    LEFT JOIN produto p ON (po.IdProduto = p.IdProd)
                    LEFT JOIN etiqueta_aplicacao apl ON (po.IdAplicacao = apl.IdAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (po.IdProcesso = prc.IdProcesso)
                WHERE po.IdProdParent IS NOT NULL AND po.IdProdParent > 0
                    AND (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0)";

            if (idOrcamento > 0)
            {
                sql += $" AND po.IdOrcamento = {idOrcamento}";
            }

            if (idProdAmbienteOrcamento > 0)
            {
                sql += $" AND po.IdProdParent = {idProdAmbienteOrcamento}";
            }

            if ((idsProdOrcamento?.Any(f => f > 0)).GetValueOrDefault())
            {
                sql += $" AND po.IdProd IN ({string.Join(",", idsProdOrcamento)})";
            }

            return sql;
        }

        /// <summary>
        /// Pesquisa produtos de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="sortExpression">sortExpression.</param>
        /// <param name="startRow">startRow.</param>
        /// <param name="pageSize">pageSize.</param>
        /// <returns>Retorna uma lista de produtos de orçamento, buscados com base nos parâmetros preenchidos.</returns>
        public List<ProdutosOrcamento> PesquisarProdutosOrcamento(
            GDASession session,
            int? idOrcamento,
            int? idProdAmbienteOrcamento,
            string sortExpression,
            int startRow,
            int pageSize)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, null, null))
            {
                return new List<ProdutosOrcamento>();
            }

            var quantidadeRegistros = this.PesquisarProdutosOrcamentoCount(session, idOrcamento, idProdAmbienteOrcamento);

            if (quantidadeRegistros == 0)
            {
                return new List<ProdutosOrcamento> { new ProdutosOrcamento() };
            }

            var sqlProdutoOrcamento = this.SqlProdutoOrcamento(idOrcamento, idProdAmbienteOrcamento, null, true);

            return this.LoadDataWithSortExpression(session, sqlProdutoOrcamento, sortExpression, startRow, pageSize)?.ToList();
        }

        /// <summary>
        /// Obtém a quantidade, para grid, de produtos de orçamento pesquisados.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna a quantidade de produtos de orçamento, buscados de acordo com os parâmetros preenchidos.
        /// O retorno nunca será 0 (zero), para evitar problema de exibição na grid sem produtos.</returns>
        public int PesquisarProdutosOrcamentoCountGrid(GDASession session, int? idOrcamento, int? idProdAmbienteOrcamento)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, null, null))
            {
                return 1;
            }

            var quantidadeRegistros = this.PesquisarProdutosOrcamentoCount(session, idOrcamento, idProdAmbienteOrcamento);

            return quantidadeRegistros > 0 ? quantidadeRegistros : 1;
        }

        /// <summary>
        /// Obtém a quantidade, para grid, de produtos de orçamento pesquisados.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna a quantidade de produtos de orçamento, buscados de acordo com os parâmetros preenchidos.</returns>
        public int PesquisarProdutosOrcamentoCount(GDASession session, int? idOrcamento, int? idProdAmbienteOrcamento)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, null, null))
            {
                return 0;
            }

            var sqlProdutoOrcamento = this.SqlProdutoOrcamento(idOrcamento, idProdAmbienteOrcamento, null, false);

            return this.objPersistence.ExecuteSqlQueryCount(session, sqlProdutoOrcamento);
        }

        /// <summary>
        /// Obtém produtos de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna uma lista com produtos de orçamento, buscados com base nos parâmetros preenchidos.</returns>
        public List<ProdutosOrcamento> ObterProdutosOrcamento(GDASession session, int? idOrcamento, int? idProdAmbienteOrcamento)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, null, null))
            {
                return new List<ProdutosOrcamento>();
            }

            var sqlProdutoOrcamento = this.SqlProdutoOrcamento(idOrcamento, idProdAmbienteOrcamento, null, true);

            return this.objPersistence.LoadData(session, sqlProdutoOrcamento).ToList();
        }

        /// <summary>
        /// Obtém produtos de orçamento pelos IDs de produtos de orçamento informados.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idsProdOrcamento">idsProdOrcamento.</param>
        /// <returns>Retorna uma lista com produtos de orçamento, buscados com base nos parâmetros preenchidos.</returns>
        public List<ProdutosOrcamento> ObterProdutosOrcamentoPorIdsProdOrcamento(GDASession session, List<int> idsProdOrcamento)
        {
            if (!(idsProdOrcamento?.Any(f => f > 0)).GetValueOrDefault())
            {
                return new List<ProdutosOrcamento>();
            }

            var sqlProdutoOrcamento = this.SqlProdutoOrcamento(null, null, idsProdOrcamento.Distinct().ToList(), true);

            return this.objPersistence.LoadData(session, sqlProdutoOrcamento).ToList();
        }

        /// <summary>
        /// Obtém a quantidade de produtos de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna a quantidade de produtos de orçamento, com base nos parâmetros preenchidos.</returns>
        public int ObterQuantidadeProdutosOrcamento(GDASession session, int? idOrcamento, int? idProdAmbienteOrcamento)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, null, null))
            {
                return 0;
            }

            var sqlProdutoOrcamento = this.SqlProdutoOrcamento(idOrcamento, idProdAmbienteOrcamento, null, false);

            return this.objPersistence.ExecuteSqlQueryCount(sqlProdutoOrcamento, null);
        }

        #endregion

        #region Produto composição orcamento

        /// <summary>
        /// Sql base para recuperar produtos de orçamento de composição.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <param name="selecionar">selecionar.</param>
        /// <returns>Retorna um SQL base para a busca de produtos de composição.</returns>
        private string SqlProdutoComposicao(int? idOrcamento, int? idProdAmbienteOrcamento, int? idProdOrcamentoParent, bool selecionar)
        {
            var sqlValorTabelaProduto = $"IF(o.TipoEntrega IN ({(int)Orcamento.TipoEntregaOrcamento.Balcao}, {(int)Orcamento.TipoEntregaOrcamento.Entrega}), p.ValorBalcao, p.ValorObra)";
            var campos = selecionar ? $@"po.*, p.Descricao AS DescrProduto, p.CodInterno, mip.IdMaterProjMod, mip.IdPecaItemProj,
                apl.CodInterno AS CodAplicacao, prc.CodInterno AS CodProcesso, poa.Ambiente, {sqlValorTabelaProduto} AS ValorProdutoTabela" : "COUNT(*)";

            var sql = $@"SELECT {campos} FROM produtos_orcamento po
                    LEFT JOIN orcamento o ON (po.IdOrcamento = o.IdOrcamento)
                    LEFT JOIN produtos_orcamento poa ON (po.IdProdParent = poa.IdProd)
                    LEFT JOIN produto p ON (po.IdProduto = p.IdProd)
                    LEFT JOIN material_item_projeto mip ON (po.IdMaterItemProj = mip.IdMaterItemProj)
                    LEFT JOIN etiqueta_aplicacao apl ON (po.IdAplicacao = apl.IdAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (po.IdProcesso = prc.IdProcesso)
                WHERE po.IdProdOrcamentoParent IS NOT NULL";

            if (idOrcamento > 0)
            {
                sql += $" AND po.IdOrcamento = {idOrcamento}";
            }

            if (idProdAmbienteOrcamento > 0)
            {
                sql += $" AND po.IdProdParent = {idProdAmbienteOrcamento}";
            }

            if (idProdOrcamentoParent > 0)
            {
                sql += $" AND po.IdProdOrcamentoParent = {idProdOrcamentoParent}";
            }

            return sql;
        }

        /// <summary>
        /// Pesquisa produtos de orçamento de composição.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <param name="sortExpression">sortExpression.</param>
        /// <param name="startRow">startRow.</param>
        /// <param name="pageSize">pageSize.</param>
        /// <returns>Retorna uma lista de produtos de composição, buscados com base nos parâmetros preenchidos.</returns>
        public IList<ProdutosOrcamento> PesquisarProdutosComposicao(
            int? idOrcamento,
            int? idProdAmbienteOrcamento,
            int? idProdOrcamentoParent,
            string sortExpression,
            int startRow,
            int pageSize)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, idProdOrcamentoParent, null))
            {
                return new List<ProdutosOrcamento>();
            }

            if (this.ObterQuantidadeProdutoComposicao(idOrcamento, idProdAmbienteOrcamento, idProdOrcamentoParent) == 0)
            {
                return new List<ProdutosOrcamento>() { new ProdutosOrcamento() };
            }

            var sort = string.IsNullOrEmpty(sortExpression) ? "po.IdProd ASC" : sortExpression;
            var sqlProdutoComposicao = this.SqlProdutoComposicao(idOrcamento, idProdAmbienteOrcamento, idProdOrcamentoParent, true);

            return this.LoadDataWithSortExpression(sqlProdutoComposicao, sort, startRow, pageSize, null);
        }

        /// <summary>
        /// Obtém a quantidade, para grid, de produtos de composição de orçamento pesquisados.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <returns>Retorna a quantidade de produtos de composição, buscados de acordo com os parâmetros preenchidos.
        /// O retorno nunca será 0 (zero), para evitar problema de exibição na grid sem produtos.</returns>
        public int PesquisarProdutosComposicaoCountGrid(int? idOrcamento, int? idProdAmbienteOrcamento, int? idProdOrcamentoParent)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, idProdOrcamentoParent, null))
            {
                return 1;
            }

            var quantidadeRegistros = this.PesquisarProdutosComposicaoCount(idOrcamento, idProdAmbienteOrcamento, idProdOrcamentoParent);

            return quantidadeRegistros > 0 ? quantidadeRegistros : 1;
        }

        /// <summary>
        /// Obtém a quantidade, para grid, de produtos de composição de orçamento pesquisados.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <returns>Retorna a quantidade de produtos de composição, buscados de acordo com os parâmetros preenchidos.</returns>
        public int PesquisarProdutosComposicaoCount(int? idOrcamento, int? idProdAmbienteOrcamento, int? idProdOrcamentoParent)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, idProdOrcamentoParent, null))
            {
                return 0;
            }

            var sqlProdutoComposicao = this.SqlProdutoComposicao(idOrcamento, idProdAmbienteOrcamento, idProdOrcamentoParent, false);

            return this.objPersistence.ExecuteSqlQueryCount(sqlProdutoComposicao);
        }

        /// <summary>
        /// Obtém produtos de composição pelo IdProdAmbienteOrcamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna uma lista de produtos de composicao</returns>
        public List<ProdutosOrcamento> ObterProdutosComposicao(GDASession session, int? idProdAmbienteOrcamento)
        {
            if (this.ParametrosVazios(null, null, idProdAmbienteOrcamento, null, null))
            {
                return new List<ProdutosOrcamento>();
            }

            var sqlProdutoComposicao = this.SqlProdutoComposicao(null, idProdAmbienteOrcamento, null, true);

            return this.objPersistence.LoadData(session, sqlProdutoComposicao).ToList();
        }

        /// <summary>
        /// Obtém produtos filhos de composição de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idsProdOrcamento">idsProdOrcamento.</param>
        /// <returns>Retorna uma lista de produtos filhos de um produto composto.</returns>
        public IList<ProdutosOrcamento> ObterFilhosComposicao(GDASession session, List<int> idsProdOrcamento)
        {
            if (!(idsProdOrcamento?.Any(f => f > 0)).GetValueOrDefault())
            {
                return new List<ProdutosOrcamento>();
            }

            var sqlProdutoFilhoComposicao = $@"SELECT * FROM produtos_orcamento
                WHERE IdProdOrcamentoParent IN ({string.Join(",", idsProdOrcamento.Where(f => f > 0))});";

            return this.objPersistence.LoadData(session, sqlProdutoFilhoComposicao)?.ToList() ?? new List<ProdutosOrcamento>();
        }

        /// <summary>
        /// Obtém a quantidade de produtos de composição.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <param name="idProdOrcamentoParent">idProdOrcamentoParent.</param>
        /// <returns>Retorna a quantidade de produtos de composição, com base nos parâmetros preenchidos.</returns>
        public int ObterQuantidadeProdutoComposicao(int? idOrcamento, int? idProdAmbienteOrcamento, int? idProdOrcamentoParent)
        {
            if (this.ParametrosVazios(idOrcamento, null, idProdAmbienteOrcamento, idProdOrcamentoParent, null))
            {
                return 0;
            }

            var sqlProdutoComposicao = this.SqlProdutoComposicao(idOrcamento, idProdAmbienteOrcamento, idProdOrcamentoParent, false);

            return this.objPersistence.ExecuteSqlQueryCount(sqlProdutoComposicao, null);
        }

        /// <summary>
        /// Verifica se o orçamento informado possui produtos de composição.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento possui produtos compostos;
        /// False: o orçamento não possui produtos compostos.</returns>
        public bool VerificarTemProdutoLamComposicao(GDASession session, int idOrcamento)
        {
            var sql = $@"SELECT COUNT(*)
                FROM produtos_orcamento po
                    INNER JOIN produto p ON (po.IdProduto = p.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (p.IdSubgrupoProd = sgp.IdSubgrupoProd)
                WHERE sgp.TipoSubGrupo IN ({(int)TipoSubgrupoProd.VidroDuplo}, {(int)TipoSubgrupoProd.VidroLaminado})
                    AND po.IdOrcamento = {idOrcamento};";

            return this.objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se o produto de orçamento é um produto laminado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>True: o produto de orçamento é um produto laminado;
        /// False: o produto de orçamento não é um produto laminado.</returns>
        public bool VerificarProdutoLaminado(GDASession session, int idProdOrcamento)
        {
            var idProduto = this.ObterIdProduto(session, idProdOrcamento);
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, idProduto);

            return tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
        }

        /// <summary>
        /// Verifica se o produto de orçamento possui produtos de composição.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <returns>True: o produto de orçamento possui peças filhas em sua composição;
        /// False: o produto de orçamento não possui peças filhas em sua composição.</returns>
        public bool VerificarTemFilhoComposicao(GDASession session, int idProdOrcamento)
        {
            var sqlVerificaTemFilhoComposicao = $@"SELECT COUNT(*) FROM produtos_orcamento
                WHERE IdProdOrcamentoParent = {idProdOrcamento};";

            return this.objPersistence.ExecuteSqlQueryCount(session, sqlVerificaTemFilhoComposicao) > 0;
        }

        #endregion
    }
}
