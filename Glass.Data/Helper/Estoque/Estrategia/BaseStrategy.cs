// <copyright file="BaseStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper.Estoque.Estrategia.Models;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Estoque.Estrategia
{
    /// <summary>
    /// Classe que implementa controles do estoque comuns às estratégias.
    /// </summary>
    internal abstract class BaseStrategy : IEstoqueStrategy
    {
        public void Baixar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            movimentacao.Tipo = MovEstoque.TipoMovEnum.Saida;
            this.Movimentar(sessao, movimentacao);
        }

        public void Creditar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            movimentacao.Tipo = MovEstoque.TipoMovEnum.Entrada;
            this.Movimentar(sessao, movimentacao);
        }

        public void ValidarMovimentacao(GDASession sessao, MovimentacaoDto movimentacao)
        {
            throw new NotImplementedException();
        }

        private void Movimentar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            movimentacao.Usuario = movimentacao.Usuario != null ? movimentacao.Usuario : UserInfo.GetUserInfo;

            if (!GrupoProdDAO.Instance.AlterarEstoque(sessao, (int)movimentacao.IdProduto) && !movimentacao.LancamentoManual)
            {
                return;
            }

            if (movimentacao.IdLoja == 0)
            {
                throw new InvalidOperationException("A loja da movimentação de estoque não foi informada.");
            }

            uint idMovEstoque = 0;

            try
            {
                ProdutoBaixaEstoque[] produtosBaixaEstoque;

                if (movimentacao.AlterarMateriaPrima)
                {
                    produtosBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(
                        sessao,
                        movimentacao.IdProduto,
                        movimentacao.BaixarProprioProdutoSeNaoTiverMateriaPrima);
                }
                else
                {
                    produtosBaixaEstoque = new ProdutoBaixaEstoque[] 
                    {
                        new ProdutoBaixaEstoque{
                            IdProd = (int)movimentacao.IdProduto,
                            IdProdBaixa = (int)movimentacao.IdProduto,
                            Qtde = 1,
                        },
                    };
                }

                foreach (var item in produtosBaixaEstoque)
                {
                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)movimentacao.IdProduto);

                    // Se não for lançamento manual, não for mov. de produção e o produto for chapa de vidro mov. a matéria-prima
                    if (!movimentacao.LancamentoManual
                        && movimentacao.AlterarProdutoBase
                        && (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado))
                    {
                        var m2Chapa = ProdutoDAO.Instance.ObtemM2Chapa(sessao, item.IdProdBaixa);
                        var idProdBase = ProdutoDAO.Instance.ObterProdutoBase(sessao, item.IdProdBaixa);

                        if (idProdBase == item.IdProdBaixa)
                        {
                            throw new InvalidOperationException("O produto base não pode ser o próprio produto.");
                        }

                        if (idProdBase.HasValue)
                        {
                            var movimentacaoDto = movimentacao;
                            movimentacaoDto.IdProduto = (uint)idProdBase.Value;
                            movimentacaoDto.Quantidade = movimentacaoDto.Quantidade * m2Chapa;
                            movimentacaoDto.BaixarProprioProdutoSeNaoTiverMateriaPrima = true;

                            this.Movimentar(sessao, movimentacaoDto);
                        }
                    }

                    var qtde = movimentacao.Quantidade * (decimal)item.Qtde;
                    decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;

                    MovEstoqueDAO.Instance.ValidarMovimentarEstoque(
                        sessao,
                        item.IdProdBaixa,
                        (int)movimentacao.IdLoja,
                        movimentacao.Data,
                        movimentacao.Tipo,
                        qtde,
                        ref saldoQtdeAnterior,
                        ref saldoValorAnterior,
                        ref saldoQtdeValidar,
                        false);

                    // Registra a alteração do estoque
                    MovEstoque movEstoque = new MovEstoque();
                    movEstoque.IdProd = (uint)item.IdProdBaixa;
                    movEstoque.IdLoja = movimentacao.IdLoja;
                    movEstoque.IdFunc = movimentacao.Usuario?.CodUser ?? 0;
                    movEstoque.IdPedido = movimentacao.IdPedido;
                    movEstoque.IdCompra = movimentacao.IdCompra;
                    movEstoque.IdLiberarPedido = movimentacao.IdLiberarPedido;
                    movEstoque.IdProdPedProducao = movimentacao.IdProdPedProducao;
                    movEstoque.IdTrocaDevolucao = movimentacao.IdTrocaDevolucao;
                    movEstoque.IdNf = movimentacao.IdNf;
                    movEstoque.IdPedidoInterno = movimentacao.IdPedidoInterno;
                    movEstoque.IdProdPed = movimentacao.IdProdPed;
                    movEstoque.IdProdCompra = movimentacao.IdProdCompra;
                    movEstoque.IdProdLiberarPedido = movimentacao.IdProdLiberarPedido;
                    movEstoque.IdProdTrocaDev = movimentacao.IdProdTrocaDev;
                    movEstoque.IdProdTrocado = movimentacao.IdProdTrocado;
                    movEstoque.IdProdNf = movimentacao.IdProdNf;
                    movEstoque.IdProdPedInterno = movimentacao.IdProdPedInterno;
                    movEstoque.IdRetalhoProducao = movimentacao.IdRetalhoProducao;
                    movEstoque.IdPerdaChapaVidro = movimentacao.IdPerdaChapaVidro;
                    movEstoque.IdCarregamento = movimentacao.IdCarregamento;
                    movEstoque.IdVolume = movimentacao.IdVolume;
                    movEstoque.IdInventarioEstoque = movimentacao.IdInventarioEstoque;
                    movEstoque.IdProdImpressaoChapa = movimentacao.IdProdImpressaoChapa;
                    movEstoque.LancManual = movimentacao.LancamentoManual;
                    movEstoque.TipoMov = (int)movimentacao.Tipo;
                    movEstoque.DataMov = movimentacao.Data.AddSeconds(1);
                    movEstoque.QtdeMov = qtde;
                    movEstoque.Obs = movimentacao.Observacao;
                    movEstoque.SaldoQtdeMov = Math.Round(saldoQtdeAnterior + (movimentacao.Tipo == MovEstoque.TipoMovEnum.Entrada ? qtde : -qtde), Configuracoes.Geral.NumeroCasasDecimaisTotM);

                    if (movimentacao.Data.Date != DateTime.Now.Date)
                    {
                        movEstoque.DataCad = DateTime.Now;
                    }

                    if (movEstoque.SaldoQtdeMov < 0)
                    {
                        movEstoque.ValorMov = 0;
                        movEstoque.SaldoValorMov = 0;
                    }
                    else if (movimentacao.Tipo == MovEstoque.TipoMovEnum.Entrada && movimentacao.Total > 0)
                    {
                        var saldoQuantidadeMovimentada = movEstoque.SaldoQtdeMov > 0 ? movEstoque.SaldoQtdeMov : 1;
                        var perc = movimentacao.Quantidade > movEstoque.SaldoQtdeMov ? movimentacao.Quantidade / saldoQuantidadeMovimentada : 1;

                        movEstoque.ValorMov = Math.Abs(movimentacao.Total);
                        movEstoque.SaldoValorMov = saldoValorAnterior + (movEstoque.ValorMov * perc);
                    }
                    else
                    {
                        var valorUnit = saldoValorAnterior / (saldoQtdeAnterior > 0 ? saldoQtdeAnterior : 1);

                        movEstoque.ValorMov = Math.Abs(valorUnit * qtde);
                        movEstoque.SaldoValorMov = saldoValorAnterior - (valorUnit * qtde);
                    }

                    idMovEstoque = MovEstoqueDAO.Instance.Insert(sessao, movEstoque);

                    MovEstoqueDAO.Instance.AtualizaSaldo(sessao, movEstoque.IdMovEstoque);
                    ProdutoLojaDAO.Instance.AtualizarProdutoLoja(sessao, (int)movEstoque.IdProd, (int)movEstoque.IdLoja);

                    if (ProdutoDAO.Instance.IsProdutoProducao(sessao, item.IdProdBaixa))
                    {
                        var metroQuadrado = ProdutoDAO.Instance.ObtemM2BoxPadrao(sessao, item.IdProdBaixa);

                        if (metroQuadrado > 0)
                        {
                            ProdutoLojaDAO.Instance.AtualizarTotalM2(sessao, item.IdProdBaixa, (int)movimentacao.IdLoja, metroQuadrado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException($"MovEstoque - IdMovEstoque:{idMovEstoque}' IdProd:{movimentacao.IdProduto}' IdLoja:{movimentacao.IdLoja}", ex);
                throw;
            }
        }
    }
}
