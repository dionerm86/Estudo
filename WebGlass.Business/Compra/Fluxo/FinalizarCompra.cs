﻿using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.Compra.Fluxo
{
    public sealed class FinalizarCompra : BaseFluxo<FinalizarCompra>
    {
        private FinalizarCompra() { }

        private void ValidarCompra(Glass.Data.Model.Compra compra)
        {
            // Verifica se o Pedido possui produtos
            if (ProdutosCompraDAO.Instance.CountInCompra(compra.IdCompra) == 0)
                throw new Exception("Inclua pelo menos um produto na compra para finalizá-la.");

            int pagtoFornecedor = ParcelasDAO.Instance.GetCountByFornecedor(compra.IdFornec.GetValueOrDefault(), ParcelasDAO.TipoConsulta.Todos);

            if (pagtoFornecedor > 0)
            {
                pagtoFornecedor = ParcelasDAO.Instance.GetCountByFornecedor(compra.IdFornec.GetValueOrDefault(), ParcelasDAO.TipoConsulta.Prazo);
                if (pagtoFornecedor == 0 && compra.TipoCompra != (int)Glass.Data.Model.Compra.TipoCompraEnum.AVista)
                    throw new Exception("Esse fornecedor aceita apenas compras à vista.");
                else
                {
                    pagtoFornecedor = ParcelasDAO.Instance.GetNumParcByFornecedor(compra.IdFornec.GetValueOrDefault());
                    if (pagtoFornecedor < compra.NumParc)
                        throw new Exception("Esse fornecedor aceita apenas " + pagtoFornecedor + " parcela(s).");
                }
            }

            // Se for compra à prazo
            if (compra.TipoCompra == (int)Glass.Data.Model.Compra.TipoCompraEnum.APrazo)
            {
                // Calcula o valor de entrada + o valor à prazo
                decimal valorTotalPago = (decimal)compra.ValorEntrada;

                // Se forem mais de (FinanceiroConfig.Compra.NumeroParcelasCompra) parcelas, o cálculo de parcelas é automático
                if (compra.NumParc > FinanceiroConfig.Compra.NumeroParcelasCompra)
                    valorTotalPago += (compra.NumParc * (decimal)compra.ValorParc);

                // Cálculo feito a partir das parcelas salvas
                else
                {
                    // Busca o valor das parcelas
                    var lstParc = ParcelasCompraDAO.Instance.GetByCompra(compra.IdCompra);
                    foreach (var p in lstParc)
                        valorTotalPago += (decimal)p.Valor;
                }

                // Verifica se o valor total da compra bate com o valorTotalPago
                if (Math.Round(compra.Total, 2) != Math.Round(valorTotalPago, 2) && compra.ValorTributado == 0)
                    throw new Exception("O valor total da compra não bate com o valor do pagamento da mesma. Valor Compra: " +
                        Math.Round(compra.Total, 2).ToString("C") + " Valor Pago: " + Math.Round(valorTotalPago, 2).ToString("C"));

                // Verifica se a data base de vencimento das parcelas foi informado
                if (compra.NumParc > FinanceiroConfig.Compra.NumeroParcelasCompra && compra.DataBaseVenc == null)
                    throw new Exception("Informe a data base de vencimento das parcelas.");
            }
            else if (compra.TipoCompra == (int)Glass.Data.Model.Compra.TipoCompraEnum.AntecipFornec)
            {
                try
                {
                    if (!compra.IdAntecipFornec.HasValue)
                        throw new Exception("Informe a Antecipação.");

                    uint idFornec = AntecipacaoFornecedorDAO.Instance.GetIdFornec(compra.IdAntecipFornec.Value);

                    if (compra.IdFornec != idFornec)
                        throw new Exception("O fornecedor da antecipação deve ser o mesmo da compra.");

                    decimal saldoAntecip = AntecipacaoFornecedorDAO.Instance.GetSaldo(compra.IdAntecipFornec.Value);

                    if (saldoAntecip < compra.Total && compra.Situacao == Glass.Data.Model.Compra.SituacaoEnum.Ativa)
                        throw new Exception("O saldo da antecipação é menor que o valor desta compra. Saldo da antecipação: " + saldoAntecip.ToString("C"));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Finalizar(uint idCompra, bool isPcp, bool alterarDadosFinanceiro, out string scriptExecutar)
        {
            var compra = CompraDAO.Instance.GetElementByPrimaryKey(idCompra);

            if (compra.IdFornec == 0)
                throw new Exception("Selecione o fornecedor da compra.");

            if (compra.IdConta == 0)
                throw new Exception("Selecione o plano de conta da compra.");

            ValidarCompra(compra);

            if (FinanceiroConfig.Compra.UsarControleCompraContabilNF)
            {
                CompraDAO.Instance.AlteraSituacao(compra.IdCompra, Glass.Data.Model.Compra.SituacaoEnum.EmAndamento);
                scriptExecutar = "redirectUrl('../Listas/LstCompras.aspx');";
                return;
            }
            else
            {
                if (alterarDadosFinanceiro)
                {
                    if (compra.TipoCompra == (int)Glass.Data.Model.Compra.TipoCompraEnum.APrazo)
                    {
                        //Verifica se a compra tem sinal e não foi pago,
                        //para redirecionar para tela de pagamento de sinal
                        if (compra.ValorEntrada > 0 && CompraDAO.Instance.TemSinalPagar(compra.IdCompra))
                        {
                            scriptExecutar = "redirectUrl('../Cadastros/CadPagSinalCompra.aspx?idCompra=" + compra.IdCompra + "');";
                            return;
                        }
                    }

                    // Altera a situação da compra para finalizada
                    CompraDAO.Instance.FinalizarCompraComTransacao(compra.IdCompra);
                }

                scriptExecutar = String.Empty;
                string pathName = "../Listas/LstCompras.aspx";

                if ((compra.TipoCompra == (int)Glass.Data.Model.Compra.TipoCompraEnum.AVista || compra.ValorEntrada > 0) &&
                    ContasPagarDAO.Instance.TemContaAVista(compra.IdCompra))
                    pathName = "../Cadastros/CadContaPagar.aspx?idCompra=" + compra.IdCompra;

                else if (isPcp)
                    pathName = "../Listas/LstCompraPcp.aspx";

                scriptExecutar += "redirectUrl('" + pathName + "');";
            }
        }

        public string FinalizarVarias(IEnumerable<uint> idsCompras, DateTime[] datasParcelas, int numeroParcelas)
        {
            string retorno = "";

            //Verifica se as compras que estão sendo finalizadas tem sinal a pagar
            string idsComprasSinalPagar = "";
            foreach (uint idCompra in idsCompras)
                if (CompraDAO.Instance.TemSinalPagar(idCompra))
                    idsComprasSinalPagar += idCompra + ", ";

            if (!string.IsNullOrEmpty(idsComprasSinalPagar))
                throw new Exception("A(s) compra(s): " + idsComprasSinalPagar.TrimEnd(' ').TrimEnd(',') + " possui(em) sinal a pagar.");

            foreach (uint idCompra in idsCompras)
            {
                CompraDAO.Instance.AlteraParcelas(null, idCompra, numeroParcelas, datasParcelas);
                CompraDAO.Instance.FinalizarCompraComTransacao(idCompra);

                retorno += idCompra + ", ";
            }

            return retorno;
        }
    }
}