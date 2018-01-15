using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.NotaFiscal.Ajax
{
    public interface IGerar
    {
        string GerarNf(string idsPedidos, string idsLiberarPedidos, string idNaturezaOperacao, string idLoja,
            string percReducao, string percReducaoRevenda, string dadosNaturezasOperacao, string idCliente,
            string transferencia, string idCarregamento, string transferirNf, string nfce);

        string GerarNf(string idsCompras, string idNaturezaOperacao, string idLoja, string dadosNaturezasOperacao,
            string idFornecedor, string tipoCompra, string idConta, string numeroNFe);
    }

    internal class Gerar : IGerar
    {
        public string GerarNf(string idsPedidos, string idsLiberarPedidos, string idNaturezaOperacao, string idLoja,
            string percReducao, string percReducaoRevenda, string dadosNaturezasOperacao, string idCliente,
            string transferencia, string idCarregamento, string transferirNf, string nfce)
        {
            try
            {
                Dictionary<uint, uint> naturezasOperacao = null;

                if (!String.IsNullOrEmpty(dadosNaturezasOperacao))
                {
                    naturezasOperacao = new Dictionary<uint, uint>();
                    foreach (string s in dadosNaturezasOperacao.Split('-'))
                    {
                        uint[] n = Array.ConvertAll(s.Split(','), x => Glass.Conversoes.StrParaUint(x));

                        if (!naturezasOperacao.ContainsKey(n[0]))
                            naturezasOperacao.Add(n[0], n[1]);
                    }
                }

                uint idCli = !string.IsNullOrEmpty(idCliente) ? Glass.Conversoes.StrParaUint(idCliente) : 0;

                uint idNf = NotaFiscalDAO.Instance.GerarNf(idsPedidos, idsLiberarPedidos, Glass.Conversoes.StrParaUintNullable(idNaturezaOperacao),
                    Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaFloat(percReducao), Glass.Conversoes.StrParaFloat(percReducaoRevenda), naturezasOperacao, idCli,
                    bool.Parse(transferencia), Glass.Conversoes.StrParaUintNullable(idCarregamento), bool.Parse(transferirNf), bool.Parse(nfce));

                if (FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD)
                    return "Exp;";

                return "Ok;" + idNf;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar Nota Fiscal.", ex);
            }
        }

        public string GerarNf(string idsCompras, string idNaturezaOperacao, string idLoja, string dadosNaturezasOperacao,
            string idFornecedor, string tipoCompra, string idConta, string numeroNFe)
        {
            try
            {
                Dictionary<uint, uint> naturezasOperacao = null;

                if (!String.IsNullOrEmpty(dadosNaturezasOperacao))
                {
                    naturezasOperacao = new Dictionary<uint, uint>();
                    foreach (string s in dadosNaturezasOperacao.Split('-'))
                    {
                        uint[] n = Array.ConvertAll(s.Split(','), x => Glass.Conversoes.StrParaUint(x));

                        if (!naturezasOperacao.ContainsKey(n[0]))
                            naturezasOperacao.Add(n[0], n[1]);
                    }
                }

                uint idFornec = !string.IsNullOrEmpty(idFornecedor) ? Glass.Conversoes.StrParaUint(idFornecedor) : 0;

                var ids = idsCompras.Split(',').Select(x => Glass.Conversoes.StrParaUint(x));

                uint idNf = NotaFiscalDAO.Instance.GerarNfCompraComTransacao(ids, idFornec, Glass.Conversoes.StrParaUintNullable(idNaturezaOperacao),
                    naturezasOperacao, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaInt(tipoCompra), Glass.Conversoes.StrParaUint(idConta), Glass.Conversoes.StrParaUint(numeroNFe), null,
                    ProdutosCompraDAO.Instance.GetByVariasCompras(idsCompras, FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFeTerceiros, false));

                return "Ok;" + idNf;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar Nota Fiscal.", ex);
            }
        }
    }
}
