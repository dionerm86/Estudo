// <copyright file="ProdutosOrcamentoDAO.Ambiente.cs" company="Sync Softwares">
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
        #region Busca ambientes orçamento

        /// <summary>
        /// SQL usado para recuperar os ambientes do orçamento.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="selecionar">selecionar.</param>
        /// <returns>Retorna um SQL base para a busca de produtos ambiente de orçamento.</returns>
        private string SqlProdutoAmbienteOrcamento(int? idOrcamento, bool selecionar)
        {
            var campos = string.Empty;
            var sqlProdutos = string.Empty;

            if (selecionar)
            {
                campos = "poa.*, po.TotalProdutos, po.ValorAcrescimo, po.ValorDesconto, po.TotM";
                sqlProdutos = $@"LEFT JOIN (
                        SELECT po.IdProdParent, COUNT(*) AS QtdeProd, SUM(po.TotM) AS TotM,
                            CAST(SUM(po.Total + COALESCE(po.ValorBenef, 0)) AS DECIMAL(12, 2)) AS TotalProdutos,
                            CAST(SUM(COALESCE(po.ValorAcrescimoProd, 0)) AS DECIMAL(12, 2)) AS ValorAcrescimo,
                            CAST(SUM(COALESCE(po.ValorDescontoProd, 0)) AS DECIMAL(12, 2)) AS ValorDesconto
                        FROM produtos_orcamento po
                            LEFT JOIN produtos_orcamento poa ON (po.IdProdParent = poa.IdProd AND po.IdOrcamento = poa.IdOrcamento)
                        WHERE po.IdProdOrcamentoParent IS NULL AND po.IdOrcamento = {idOrcamento}
                        GROUP BY po.IdProdParent
                    ) po ON (poa.IdProd = po.IdProdParent)";
            }
            else
            {
                campos = "COUNT(*)";
            }

            var sql = $@"SELECT {campos} FROM produtos_orcamento poa
                    {sqlProdutos}
                WHERE (poa.IdProdParent IS NULL OR poa.IdProdParent = 0)";

            if (idOrcamento > 0)
            {
                sql += $" AND poa.IdOrcamento = {idOrcamento}";
            }

            return sql;
        }

        /// <summary>
        /// Pesquisar os ambientes de um orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="sortExpression">sortExpression.</param>
        /// <param name="startRow">startRow.</param>
        /// <param name="pageSize">pageSize.</param>
        /// <returns>Retorna uma lista com produtos de orçamento, buscados com base nos parâmetros informados.</returns>
        public List<ProdutosOrcamento> PesquisarProdutosAmbienteOrcamento(
            GDASession session,
            int? idOrcamento,
            string sortExpression,
            int startRow,
            int pageSize)
        {
            if (idOrcamento.GetValueOrDefault() == 0)
            {
                return new List<ProdutosOrcamento>();
            }

            if (this.PesquisarProdutosAmbienteOrcamentoCount(session, idOrcamento) == 0)
            {
                return new List<ProdutosOrcamento> { new ProdutosOrcamento() };
            }

            var sqlProdutoAmbienteOrcamento = this.SqlProdutoAmbienteOrcamento(idOrcamento, true);

            return this.LoadDataWithSortExpression(session, sqlProdutoAmbienteOrcamento, sortExpression, startRow, pageSize)?.ToList();
        }

        /// <summary>
        /// Pesquisa a quantidade de ambientes de um orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna a quantidade de produtos ambiente de um orçamento.
        /// A quantidade retornada não pode ser menor ou igual à zero, para que a grid esconda a primeira linha do Item Template, caso o orçamento não possua ambiente.</returns>
        public int PesquisarProdutosAmbienteOrcamentoCountGrid(GDASession session, int? idOrcamento)
        {
            if (idOrcamento.GetValueOrDefault() == 0)
            {
                return 0;
            }

            var quantidadeRegistros = this.PesquisarProdutosAmbienteOrcamentoCount(session, idOrcamento);

            return quantidadeRegistros > 0 ? quantidadeRegistros : 1;
        }

        /// <summary>
        /// Pesquisa a quantidade real de ambientes de um orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna a quantidade de produtos ambiente de um orçamento.</returns>
        public int PesquisarProdutosAmbienteOrcamentoCount(GDASession session, int? idOrcamento)
        {
            if (idOrcamento.GetValueOrDefault() == 0)
            {
                return 0;
            }

            return this.objPersistence.ExecuteSqlQueryCount(session, this.SqlProdutoAmbienteOrcamento(idOrcamento, false));
        }

        /// <summary>
        /// Obtém o objeto de todos os ambientes de um orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna uma lista de produtos ambiente de um orçamento.</returns>
        public List<ProdutosOrcamento> ObterProdutosAmbienteOrcamento(GDASession session, int? idOrcamento)
        {
            if (idOrcamento.GetValueOrDefault() == 0)
            {
                return new List<ProdutosOrcamento>();
            }

            return this.objPersistence.LoadData(session, this.SqlProdutoAmbienteOrcamento(idOrcamento, true)).ToList();
        }

        #endregion

        #region Atualiza campos

        /// <summary>
        /// Atualiza o valor do campo Negociar, de acordo com o parâmetro informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdOrcamento">idProdOrcamento.</param>
        /// <param name="negociar">negociar.</param>
        public void AtualizarNegociar(GDASession session, int idProdOrcamento, bool negociar)
        {
            if (idProdOrcamento == 0)
            {
                return;
            }

            this.objPersistence.ExecuteCommand(session, $"UPDATE produto_orcamento SET Negociar = {(negociar ? "1" : "0")} WHERE IdProd = {idProdOrcamento};");
        }

        #endregion

        #region Obtém campos

        /// <summary>
        /// Recupera o ID do ambiente, pelo ID do item de projeto associado a ele.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idItemProjeto">idItemProjeto.</param>
        /// <returns>Retorna o ID de produto de orçamento associado ao item de projeto.</returns>
        public int? ObterIdProdOrcamentoPeloIdItemProjeto(GDASession session, int idItemProjeto)
        {
            if (idItemProjeto == 0)
            {
                return null;
            }

            var sqlIdProdOrcamentoPeloIdItemProjeto = $@"SELECT IdProd FROM produtos_orcamento
                WHERE (IdMaterItemProj IS NULL OR IdMaterItemProj = 0) AND
                    IdItemProjeto = {idItemProjeto};";

            return this.ExecuteScalar<int?>(session, sqlIdProdOrcamentoPeloIdItemProjeto);
        }

        /// <summary>
        /// Recupera o ID do item de projeto associado ao ambiente informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProd">idProd.</param>
        /// <returns>Retorna o ID de item de projeto associado ao produto de orçamento.</returns>
        public int? ObterIdItemProjeto(GDASession session, int idProdOrcamento)
        {
            if (idProdOrcamento == 0)
            {
                return 0;
            }

            return this.ExecuteScalar<int?>(session, $"SELECT COALESCE(IdItemProjeto, 0) FROM produtos_orcamento WHERE IdProd = {idProdOrcamento}");
        }

        /// <summary>
        /// Recupera o tipo de acréscimo do ambiente.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna o tipo de acréscimo do ambiente.</returns>
        public int ObterTipoAcrescimo(GDASession session, int idProdAmbienteOrcamento)
        {
            if (idProdAmbienteOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(session, "TipoAcrescimo", $"IdProd = {idProdAmbienteOrcamento}");
        }

        /// <summary>
        /// Recupera o valor/percentual de acréscimo do ambiente.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna o acréscimo do ambiente.</returns>
        public decimal ObterAcrescimo(GDASession session, int idProdAmbienteOrcamento)
        {
            if (idProdAmbienteOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<decimal>(session, "Acrescimo", $"IdProd = {idProdAmbienteOrcamento}");
        }

        /// <summary>
        /// Recupera o tipo de desconto do ambiente.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna o tipo de desconto do ambiente.</returns>
        public int ObterTipoDesconto(GDASession session, int idProdAmbienteOrcamento)
        {
            if (idProdAmbienteOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(session, "TipoDesconto", $"IdProd = {idProdAmbienteOrcamento}");
        }

        /// <summary>
        /// Recupera o valor/percentual de desconto do ambiente.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna o desconto do ambiente.</returns>
        public decimal ObterDesconto(GDASession session, int idProdAmbienteOrcamento)
        {
            if (idProdAmbienteOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<decimal>(session, "Desconto", $"IdProd = {idProdAmbienteOrcamento}");
        }

        /// <summary>
        /// Recupera todos os ID's de produto orçamento associados ao ambiente informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idProdAmbienteOrcamento">idProdAmbienteOrcamento.</param>
        /// <returns>Retorna os ID's de produto de orçamento pelo ID do ambiente de orçamento.</returns>
        public List<int> ObterIdsProdOrcamentoPeloIdProdAmbiente(GDASession session, int idProdAmbienteOrcamento)
        {
            if (idProdAmbienteOrcamento == 0)
            {
                return new List<int>();
            }

            var sqlIdsProdOrcamentoPeloIdProdAmbiente = $@"SELECT po.IdProd
                FROM produtos_orcamento po
                WHERE po.IdProdParent = {idProdAmbienteOrcamento};";

            return this.ExecuteMultipleScalar<int>(session, sqlIdsProdOrcamentoPeloIdProdAmbiente);
        }

        #endregion

        #region Verifica campos

        /// <summary>
        /// Verifica se o ambiente possui produtos.
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idProdOrcamentoAmbiente">idProdOrcamentoAmbiente</param>
        /// <returns>True: ambiente possui produtos;
        /// False: orçamento não possui produtos.</returns>
        public bool VerificarPossuiProduto(GDASession session, int idProdOrcamentoAmbiente)
        {
            if (idProdOrcamentoAmbiente == 0)
            {
                return false;
            }

            var sql = $"SELECT COUNT(*) FROM produtos_orcamento WHERE IdProdParent = {idProdOrcamentoAmbiente}";

            return this.objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Acréscimo e Desconto

        #region Acréscimo

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="produto">produto.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: acréscimo aplicado;
        /// False: acréscimo não aplicado.</returns>
        internal bool AplicarAcrescimoAmbiente(
            GDASession session,
            ProdutosOrcamento produto,
            Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            // Deve-se recalcular o total bruto sempre, pois caso tenha sido adicionado mais algum produto após o desconto/acréscimo,
            // o total bruto ficaria errado, causando erros ao reaplicar desconto/acréscimo
            var tabela = produto.IdItemProjeto > 0 ? "material_item_projeto" : "produtos_orcamento";
            var where = produto.IdItemProjeto > 0 ? "idItemProjeto=" + produto.IdItemProjeto : "idProdParent=" + produto.IdProd;

            produto.TotalBruto = this.ExecuteScalar<decimal>(session, "select sum(totalBruto) from " + tabela + " where " + where);
            produto.TotalBruto += this.ExecuteScalar<decimal>(session, "select sum(valorBenef" + (produto.IdItemProjeto > 0 ? string.Empty : "-coalesce(valorComissao,0)") + ") from " + tabela + " where " + where);

            var aplicado = DescontoAcrescimo.Instance.AplicarAcrescimoAmbiente(
                session,
                orcamento,
                produto.TipoAcrescimo,
                produto.Acrescimo,
                produtosOrcamento
            );

            if (aplicado)
            {
                produto.ValorAcrescimoProd = produtosOrcamento.Sum(p => p.ValorAcrescimoProd);
            }

            return aplicado;
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="produto">produto.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: acréscimo removido;
        /// False: acréscimo não removido.</returns>
        internal bool RemoverAcrescimoAmbiente(
            GDASession session,
            ProdutosOrcamento produto,
            Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimoAmbiente(
                session,
                orcamento,
                produtosOrcamento
            );
        }

        #endregion

        #region Desconto

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="produto">produto.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: desconto aplicado;
        /// False: desconto não aplicado.</returns>
        internal bool AplicarDescontoAmbiente(
            GDASession session,
            ProdutosOrcamento produto,
            Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            // Deve-se recalcular o total bruto sempre, pois caso tenha sido adicionado mais algum produto após o desconto/acréscimo,
            // o total bruto ficaria errado, causando erros ao reaplicar desconto/acréscimo
            var tabela = produto.IdItemProjeto > 0 ? "material_item_projeto" : "produtos_orcamento";
            var where = produto.IdItemProjeto > 0 ? "idItemProjeto=" + produto.IdItemProjeto : "idProdParent=" + produto.IdProd;

            produto.TotalBruto = this.ExecuteScalar<decimal>(session, "select sum(totalBruto) from " + tabela + " where " + where);
            produto.TotalBruto += this.ExecuteScalar<decimal>(session, "select sum(valorBenef" + (produto.IdItemProjeto > 0 ? string.Empty : "-coalesce(valorComissao,0)") + ") from " + tabela + " where " + where);

            var aplicado = DescontoAcrescimo.Instance.AplicarDescontoAmbiente(
                session,
                orcamento,
                produto.TipoDesconto,
                produto.Desconto,
                produtosOrcamento
            );

            if (aplicado)
            {
                produto.ValorDescontoProd = produtosOrcamento.Sum(p => p.ValorDescontoProd);
            }

            return aplicado;
        }

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="produto">produto.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: desconto removido;
        /// False: desconto não removido.</returns>
        internal bool RemoverDescontoAmbiente(
            GDASession sessao,
            ProdutosOrcamento produto,
            Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverDescontoAmbiente(
                sessao,
                orcamento,
                produtosOrcamento
            );
        }

        #endregion

        #region Finalização

        /// <summary>
        /// Finaliza a aplicação de desconto/acréscimo.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtoPai">produtoPai.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <param name="atualizar">atualizar.</param>
        internal void FinalizarAplicacaoAcrescimoDescontoAmbiente(
            GDASession sessao,
            Orcamento orcamento,
            ProdutosOrcamento produtoPai,
            IEnumerable<ProdutosOrcamento> produtosOrcamento,
            bool atualizar)
        {
            if (atualizar)
            {
                foreach (var produto in produtosOrcamento)
                {
                    this.UpdateBase(sessao, produto, orcamento);
                    this.AtualizarBenef(sessao, (int)produto.IdProd, produto.Beneficiamentos);
                }

                this.UpdateBase(sessao, produtoPai, orcamento);
                this.AtualizarBenef(sessao, (int)produtoPai.IdProd, produtoPai.Beneficiamentos);
                this.UpdateTotaisProdutoOrcamento(sessao, produtoPai);
            }
        }

        #endregion

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Cria um ambiente com transação.
        /// </summary>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do produto ambiente inserido.</returns>
        public uint InsertProdutoAmbienteComTransacao(ProdutosOrcamento objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.InsertProdutoAmbiente(transaction, objInsert);

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
        /// Cria um ambiente, dentro da sessão informada.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do produto ambiente inserido.</returns>
        public uint InsertProdutoAmbiente(GDASession session, ProdutosOrcamento objInsert)
        {
            if (objInsert.Descricao?.Length > 1500)
            {
                objInsert.Descricao = objInsert.Descricao.Substring(0, 1500);
            }

            objInsert.Negociar = true;

            objInsert.IdProd = base.Insert(session, objInsert);

            return objInsert.IdProd;
        }

        /// <summary>
        /// Atualiza um ambiente com transação.
        /// </summary>
        /// <param name="objUpdate">objUpdate.</param>
        /// <returns>Retorna o ID do produto ambiente atualizado.</returns>
        public int UpdateProdutoAmbienteComTransacao(ProdutosOrcamento objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.UpdateProdutoAmbiente(transaction, objUpdate);

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
        /// Atualiza um ambiente, dentro da sessão informada.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objUpdate">objUpdate.</param>
        /// <returns>Retorna o ID do produto ambiente atualizado.</returns>
        public int UpdateProdutoAmbiente(GDASession session, ProdutosOrcamento objUpdate)
        {
            var produtoAmbiente = this.GetElementByPrimaryKey(session, objUpdate.IdProd);
            objUpdate.IdOrcamento = produtoAmbiente.IdOrcamento;
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, produtoAmbiente.IdOrcamento);

            var tipoDescontoAmbiente = 2;
            decimal descontoAmbiente = 0;
            var tipoAcrescimoAmbiente = 2;
            decimal acrescimoAmbiente = 0;

            // Atualiza o acréscimo e o desconto
            if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
            {
                tipoDescontoAmbiente = produtoAmbiente.TipoDesconto;
                descontoAmbiente = produtoAmbiente.Desconto;
                tipoAcrescimoAmbiente = produtoAmbiente.TipoAcrescimo;
                acrescimoAmbiente = produtoAmbiente.Acrescimo;

                produtoAmbiente.TipoDesconto = objUpdate.TipoDesconto;
                produtoAmbiente.Desconto = objUpdate.Desconto;
                produtoAmbiente.TipoAcrescimo = objUpdate.TipoAcrescimo;
                produtoAmbiente.Acrescimo = objUpdate.Acrescimo;
            }
            else
            {
                produtoAmbiente.TipoDesconto = 2;
                produtoAmbiente.Desconto = 0;
                produtoAmbiente.TipoAcrescimo = 2;
                produtoAmbiente.Acrescimo = 0;
            }

            // Atualiza o produto.
            produtoAmbiente.Descricao = objUpdate.Descricao;
            produtoAmbiente.Ambiente = objUpdate.Ambiente;
            produtoAmbiente.Total = null;

            produtoAmbiente.IdProd = (uint)base.Update(session, produtoAmbiente);

            var produtosOrcamento = this.ObterProdutosOrcamento(session, (int)produtoAmbiente.IdOrcamento, (int)produtoAmbiente.IdProd);
            var atualizarAcrescimo = objUpdate.TipoAcrescimo != tipoAcrescimoAmbiente || objUpdate.Acrescimo != acrescimoAmbiente;
            var atualizarDesconto = objUpdate.TipoDesconto != tipoDescontoAmbiente || objUpdate.Desconto != descontoAmbiente;

            if (atualizarAcrescimo)
            {
                this.RemoverAcrescimoAmbiente(session, objUpdate, orcamento, produtosOrcamento);
                this.AplicarAcrescimoAmbiente(session, objUpdate, orcamento, produtosOrcamento);
            }

            if (atualizarDesconto)
            {
                this.RemoverDescontoAmbiente(session, objUpdate, orcamento, produtosOrcamento);
                this.AplicarDescontoAmbiente(session, objUpdate, orcamento, produtosOrcamento);
            }

            if (atualizarDesconto || atualizarAcrescimo)
            {
                this.FinalizarAplicacaoAcrescimoDescontoAmbiente(session, orcamento, produtoAmbiente, produtosOrcamento, true);

                if (produtoAmbiente.TemItensProdutoSession(session) || produtoAmbiente.IdItemProjeto > 0)
                {
                    produtoAmbiente = this.GetElementByPrimaryKey(session, produtoAmbiente.IdProd);
                }

                this.UpdateTotaisProdutoOrcamento(session, produtoAmbiente);
            }

            orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, produtoAmbiente.IdOrcamento);
            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, orcamento, true, false);

            return (int)produtoAmbiente.IdProd;
        }

        /// <summary>
        /// Apaga o ambiente informado e seus produtos (caso seja um ambiente de projeto).
        /// </summary>
        /// <param name="objDelete">objDelete.</param>
        /// <returns>Retorna o ID do produto ambiente removido.</returns>
        public int DeleteProdutoAmbienteComTransacao(ProdutosOrcamento objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = this.DeleteProdutoAmbiente(transaction, objDelete);

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
        /// Apaga o ambiente informado e seus produtos (caso seja um ambiente de projeto).
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objDelete">objDelete.</param>
        /// <returns>Retorna o ID do produto ambiente removido.</returns>
        public int DeleteProdutoAmbiente(GDASession session, ProdutosOrcamento objDelete)
        {
            if (!this.Exists(session, objDelete.IdProd))
            {
                return 0;
            }

            var idsProdOrcamento = this.ObterIdsProdOrcamentoPeloIdProdAmbiente(session, (int)objDelete.IdProd);
            var idItemProjeto = this.ObterIdItemProjeto(session, (int)objDelete.IdProd);
            var idOrcamento = this.ObterIdOrcamento(session, (int)objDelete.IdProd);
            objDelete.IdOrcamento = (uint)idOrcamento;
            var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, idOrcamento);
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

            // Exclui item_projeto associado à este produto.
            if (idItemProjeto > 0)
            {
                var sqlItemProjetoOrcamentoExiste = $@"SELECT COUNT(*) FROM item_projeto 
                    WHERE IdOrcamento = {objDelete.IdOrcamento} AND
                        IdItemProjeto = {idItemProjeto} AND
                        (IdProjeto IS NULL OR IdProjeto = 0)";

                var itemProjetoOrcamentoExiste = this.objPersistence.ExecuteSqlQueryCount(session, sqlItemProjetoOrcamentoExiste) > 0;

                if (itemProjetoOrcamentoExiste)
                {
                    ItemProjetoDAO.Instance.DeleteByPrimaryKey(session, (int)idItemProjeto);
                }
            }
            else
            {
                if (this.VerificarPossuiProduto(session, (int)objDelete.IdProd))
                {
                    throw new Exception("Esse ambiente possui produtos. Exclua-os antes de excluir o ambiente.");
                }
            }

            if (idsProdOrcamento.Any(f => f > 0))
            {
                var sqlApagarProdutosOrcamentoBenef = $@"DELETE FROM produto_orcamento_benef WHERE IdProd IN ({string.Join(",", idsProdOrcamento)});";
                var sqlApagarProdutosOrcamento = $@"DELETE FROM produtos_orcamento WHERE IdProd = {objDelete.IdProd};";

                // Exclui os beneficiamentos deste ambiente.
                this.objPersistence.ExecuteCommand(session, sqlApagarProdutosOrcamentoBenef);

                // Exclui os produtos deste ambiente.
                this.objPersistence.ExecuteCommand(session, sqlApagarProdutosOrcamento);
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

            var retorno = base.Delete(session, objDelete);

            orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(session, objDelete.IdOrcamento);
            OrcamentoDAO.Instance.UpdateTotaisOrcamento(session, orcamento, true, false);

            return retorno;
        }

        #endregion
    }
}
