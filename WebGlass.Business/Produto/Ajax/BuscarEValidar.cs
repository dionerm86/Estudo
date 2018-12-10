using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Data.Model;
using Glass;

namespace WebGlass.Business.Produto.Ajax
{
    public interface IBuscarEValidar
    {
        string GetProdutoCompra(string idLoja, string idFornec, string idProd);
        string GetProdutoChapaVidro(string codInterno);
        string GetDadosProduto(string codInterno);
        string BuscarProdutosAlterarDados(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr,
            string ncmInicial, string ncmFinal);
        string BuscarProdutosAlterarGrupo(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr);
        string GetProduto(string idLoja, string idFornec, string codInterno);
        string GetProdutosGrupoSubgrupo(string idGrupo, string idSubgrupo);
        string GetProdutosGrupoSubgrupo(string idGrupo, string idSubgrupo, int? situacao);
        string GetProdutoNotaFiscal(string idProd, string tipoEntrega, string idCliente, string idFornecedor, string idNf);
        string GetProdObra(string codInterno, string idClienteStr);
        string GetProdutoOrca(string codInterno, string tipoEntrega, string revenda, string idCliente,
            string percComissao, string percDescontoQtdeStr, string idLoja, string idOrcamento);
        string GetProdutoPedido(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string percComissao, string tipoPedidoStr, string tipoVendaStr, string ambienteMaoObra,
            string percDescontoQtdeStr, string idLoja, bool produtoComposto);
        string GetProdutoPcp(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string tipoPedido, string ambienteMaoObra, string percDescontoQtdeStr, string idLoja);
        string GetProdutoInterno(string codInterno);
        string UsarDiferencaM2Prod(string codInternoProd);
        string GetProdutoPreco(string codInterno, string tipoPreco);
        string GetProdutoProduto(string codInterno);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetProdutoCompra(string idLoja, string idFornec, string idProd)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByIdProd(Glass.Conversoes.StrParaUint(idProd));

                if (prod == null || prod.IdProd == 0)
                {
                    return "Erro|Não existe produto com o código informado.";
                }
                else if (prod.Situacao == Glass.Situacao.Inativo)
                {
                    return "Erro|Produto inativo." + (!string.IsNullOrWhiteSpace(prod.Obs) ? " Obs: " + prod.Obs : string.Empty);
                }
                else
                {
                    string infoEstoque = !Glass.Configuracoes.Geral.NaoVendeVidro() ? string.Empty :
                        " (Disp. Estoque: " + ProdutoLojaDAO.Instance.GetEstoque(null, Glass.Conversoes.StrParaUint(idLoja), (uint)prod.IdProd, null, false, false, false) +
                        " Estoque Mín.: " + ProdutoLojaDAO.Instance.GetEstoqueMin(Glass.Conversoes.StrParaUint(idLoja), (uint)prod.IdProd) + ")";

                    decimal precoForn = ProdutoFornecedorDAO.Instance.GetCustoCompra(Glass.Conversoes.StrParaInt(idFornec), prod.IdProd);
                    decimal custoCompra = precoForn > 0 ? precoForn : prod.Custofabbase > 0 ? prod.Custofabbase : prod.CustoCompra;

                    var tipoCalculoSubGrupo = CompraConfig.UsarTipoCalculoNfParaCompra
                        ? SubgrupoProdDAO.Instance.ObtemTipoCalculo(null, prod.IdSubgrupoProd.GetValueOrDefault(), true).GetValueOrDefault()
                        : SubgrupoProdDAO.Instance.ObtemTipoCalculo(null, prod.IdSubgrupoProd.GetValueOrDefault(), false).GetValueOrDefault();

                    return "Prod|" +
                           infoEstoque + "|" +
                           Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + "|" +
                           prod.Espessura + "|" +
                           custoCompra.ToString("F2") + "|" +
                           prod.IdCorVidro + "|" +
                           "false|" +
                           prod.Altura + "|" +
                           prod.Largura + "|" +
                           prod.CodInterno + "|" +
                           tipoCalculoSubGrupo;
                }
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar produto.", ex);
            }
        }

        public string GetProdutoChapaVidro(string codInterno)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
                if (prod == null)
                    throw new Exception("Produto não encontrado.");

                if (ChapaVidroDAO.Instance.GetElement((uint)prod.IdProd) != null)
                    throw new Exception("Chapa de vidro já cadastrada para esse produto.");

                if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd))
                    return "Ok|" + prod.IdProd + "|" + prod.Descricao;

                throw new Exception("Esse produto não é um vidro.");
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        public string GetDadosProduto(string codInterno)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
                if (prod == null)
                    throw new Exception("Produto não encontrado.");

                return "Ok##" + prod.IdProd + "##" + prod.Descricao + "##" + GrupoProdDAO.Instance.GetDescricao(prod.IdGrupoProd) + "##" +
                    (prod.IdSubgrupoProd > 0 ? SubgrupoProdDAO.Instance.GetDescricao(prod.IdSubgrupoProd.Value) : "") + "##" +
                    prod.IdGrupoProd + "##" + (prod.IdSubgrupoProd > 0 ? prod.IdSubgrupoProd : 0) + "##" +
                    (prod.IdSubgrupoProd > 0 ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(prod.IdProd) == Glass.Data.Model.TipoSubgrupoProd.ChapasVidro : false) + "##" +
                    prod.Altura + "##" + prod.Largura;
            }
            catch (Exception ex)
            {
                return "Erro##" + ex.Message;
            }
        }

        private string FormataStringControle(string texto, string texto0, string texto1)
        {
            if (String.IsNullOrEmpty(texto))
                return String.Empty;

            var nova = new List<string>();
            var temp = texto.Split('/');

            for (var i = 0; i < temp.Length; i++)
                nova.Add(temp[i]);

            texto = texto0 + ": " + nova[0] + " / " + texto1 + ": " + nova[1];
            if (nova.Count > 2)
            {
                nova.RemoveAt(0);
                nova.RemoveAt(0);
                texto += " / Exceções: " + String.Join(" / ", nova.ToArray());
            }

            nova.Clear();
            temp = texto.Split('|');

            for (var i = 0; i < temp.Length; i++)
                if (temp[i] != "")
                    nova.Add(temp[i]);

            return String.Join(", ", nova.ToArray());
        }

        public string BuscarProdutosAlterarDados(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr,
            string ncmInicial, string ncmFinal)
        {
            if ((!String.IsNullOrEmpty(ncmInicial) && String.IsNullOrEmpty(ncmFinal)) ||
                (String.IsNullOrEmpty(ncmInicial) && !String.IsNullOrEmpty(ncmFinal)))
                return "Erro~Para utilizar a faixa de NCM selecione os dois valores.";

            int idGrupo = Glass.Conversoes.StrParaInt(idGrupoProdStr);
            int idSubgrupo = Glass.Conversoes.StrParaInt(idSubgrupoProdStr);

            string retorno = "";
            try
            {
                foreach (var p in ProdutoDAO.Instance.GetByGrupoSubgrupoCodInterno(codInterno, descricao, idGrupo, idSubgrupo, ncmInicial, ncmFinal))
                {
                    string aliqIcms = String.Empty, mva = String.Empty;

                    var geralIcms = p.AliqICMS.Where(x => String.IsNullOrEmpty(x.UfOrigem)).FirstOrDefault();
                    if (geralIcms != null)
                        aliqIcms += geralIcms.AliquotaIntraestadual + "/" + geralIcms.AliquotaInterestadual;

                    var excecoesIcms = p.AliqICMS.Where(x => !String.IsNullOrEmpty(x.UfOrigem));
                    foreach (var e in excecoesIcms)
                        aliqIcms += "/" + e.UfOrigem + "|" + e.UfDestino + "|" + e.AliquotaIntraestadual + "|" + e.AliquotaInterestadual;

                    var geralMva = p.Mva.Where(x => String.IsNullOrEmpty(x.UfOrigem)).FirstOrDefault();
                    if (geralMva != null)
                        mva += geralMva.MvaOriginal + "/" + geralMva.MvaSimples;

                    var excecoesMva = p.Mva.Where(x => !String.IsNullOrEmpty(x.UfOrigem));
                    foreach (var e in excecoesMva)
                        mva += "/" + e.UfOrigem + "|" + e.UfDestino + "|" + e.MvaOriginal + "|" + e.MvaSimples;

                    retorno += p.IdProd + "#" +
                               p.CodInterno.Replace("#", "") + "#" +
                               p.Descricao.Replace("#", "") + "#" +
                               p.DescrGrupo + (!String.IsNullOrEmpty(p.DescrSubgrupo) ? " " + p.DescrSubgrupo : "") + "#" +
                               FormataStringControle(aliqIcms, "Aliq. Intra.", "Aliq. Inter.") + "#" +
                               p.AliqIPI.ToString().Replace("#", "") + "#" +
                               FormataStringControle(mva, "Original", "Simples") + "#" +
                               p.Ncm + "#" +
                               p.Cst + "#" +
                               p.CstIpi + "#" +
                               p.Csosn + "#" +
                               p.CodigoEX + "#" +
                               p.DescrGeneroProd + "#" +
                               p.DescrTipoMercadoria + "#" +
                               p.DescrContaContabil + "|";
                }
            }
            catch (Exception ex)
            {
                return "Erro~" + ex.Message;
            }

            return "Ok~" + retorno.TrimEnd('|');
        }

        public string BuscarProdutosAlterarGrupo(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr)
        {
            int idGrupo = Glass.Conversoes.StrParaInt(idGrupoProdStr);
            int idSubgrupo = Glass.Conversoes.StrParaInt(idSubgrupoProdStr);

            string retorno = "";

            foreach (var p in ProdutoDAO.Instance.GetByGrupoSubgrupoCodInterno(codInterno, descricao, idGrupo, idSubgrupo, null, null))
            {
                retorno += p.IdProd + "#" +
                           p.CodInterno.Replace("#", "") + "#" +
                           p.Descricao.Replace("#", "") + "#" +
                           p.DescrGrupo + (!String.IsNullOrEmpty(p.DescrSubgrupo) ? " " + p.DescrSubgrupo : "") + "~";
            }

            return retorno.TrimEnd('~');
        }

        public string GetProduto(string idLoja, string idFornec, string codInterno)
        {
            var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            try
            {
                if (prod == null || prod.IdProd == 0)
                    return "Erro;Não existe produto com o código informado.";

                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

                else
                {
                    string infoEstoque = !Geral.NaoVendeVidro() ? String.Empty :
                        " (Disp. Estoque: " + ProdutoLojaDAO.Instance.GetEstoque(null, Glass.Conversoes.StrParaUint(idLoja), (uint)prod.IdProd, null, false, false, false) +
                        " Estoque Mín.: " + ProdutoLojaDAO.Instance.GetEstoqueMin(Glass.Conversoes.StrParaUint(idLoja), (uint)prod.IdProd) + ")";

                    decimal precoForn = ProdutoFornecedorDAO.Instance.GetCustoCompra(Glass.Conversoes.StrParaInt(idFornec), prod.IdProd);
                    decimal custoCompra = precoForn > 0 ? precoForn : prod.Custofabbase > 0 ? prod.Custofabbase : prod.CustoCompra;
                    return "Prod|" + prod.IdProd + "|" + prod.Descricao + infoEstoque + "|" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false) + "|" +
                        Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + "|" + prod.Espessura + "|" + custoCompra.ToString("F2") + "|" +
                        prod.IdCorVidro;
                }
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar produto.", ex);
            }
        }

        public string GetProdutosGrupoSubgrupo(string idGrupo, string idSubgrupo)
        {
            return GetProdutosGrupoSubgrupo(idGrupo, idSubgrupo, null);
        }

        public string GetProdutosGrupoSubgrupo(string idGrupo, string idSubgrupo, int? situacao)
        {
            try
            {
                var g = Glass.Conversoes.StrParaInt(idGrupo);
                var s = Glass.Conversoes.StrParaIntNullable(idSubgrupo);

                string descrGrupo = GrupoProdDAO.Instance.GetDescricao(g);
                string descrSubgrupo = s > 0 ? SubgrupoProdDAO.Instance.GetDescricao(s.Value) : "";

                string produtos = "";
                foreach (var prod in ProdutoDAO.Instance.GetByGrupoSubgrupo(g, s, situacao))
                {
                    if ((s == null || s == 0) && prod.IdSubgrupoProd > 0)
                        descrSubgrupo = SubgrupoProdDAO.Instance.GetDescricao(prod.IdSubgrupoProd.Value);

                    produtos += prod.IdProd + "~" + prod.CodInterno + "~" + prod.Descricao + "~" + descrGrupo + "~" +
                        descrSubgrupo + "~" + prod.IdGrupoProd + "~" + (prod.IdSubgrupoProd > 0 ? prod.IdSubgrupoProd : 0) + "^";
                }

                return "Ok##" + produtos.TrimEnd('^');
            }
            catch (Exception ex)
            {
                return "Erro##" + ex.Message;
            }
        }

        public string GetProdutoNotaFiscal(string idProd, string tipoEntrega, string idCliente, string idFornecedor, string idNfStr)
        {
            uint idNf = Glass.Conversoes.StrParaUint(idNfStr);
            uint idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idNf);
            var tipoDocumento = (Glass.Data.Model.NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(idNf);
            int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
            uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
            uint idNatOp = RegraNaturezaOperacao.Fluxo.BuscarEValidar.Instance.BuscaCodigoNaturezaOperacaoPorRegra(idNf, tipoDocumento, idLoja, idCli, Glass.Conversoes.StrParaInt(idProd)) ??
                NotaFiscalDAO.Instance.GetIdNaturezaOperacao(null, idNf);
            var prod = ProdutoDAO.Instance.GetElement(null, Glass.Conversoes.StrParaUint(idProd), idNfStr.StrParaIntNullable(), idLoja, Glass.Conversoes.StrParaUintNullable(idCliente),
                Glass.Conversoes.StrParaUintNullable(idFornecedor),
                (tipoDocumento == Glass.Data.Model.NotaFiscal.TipoDoc.Saída ||
                /* Chamado 32984. */
                (tipoDocumento == Glass.Data.Model.NotaFiscal.TipoDoc.Entrada &&
                CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNatOp)))));

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";

            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

            try
            {
                string retorno = "Prod";
                int? idFornec = !String.IsNullOrEmpty(idFornecedor) ? (int?)Glass.Conversoes.StrParaInt(idFornecedor) : null;
                decimal precoForn = idFornec > 0 ? ProdutoFornecedorDAO.Instance.GetCustoCompra(idFornec.Value, prod.IdProd) : 0;
                decimal custoCompra = precoForn > 0 ? precoForn : prod.Custofabbase > 0 ? prod.Custofabbase : prod.CustoCompra;

                retorno += ";" + (tipoDocumento == Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros ?
                    custoCompra : ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, false, false, 0, null, null, null)).ToString("F2");

                // Busca o CST origem padrão configurado, caso seja nota de saída
                int cstOrig = FiscalConfig.NotaFiscalConfig.CstOrigPadraoNotaFiscalSaida;
                if (cstOrig > 0 && tipoDocumento != Glass.Data.Model.NotaFiscal.TipoDoc.Saída)
                    cstOrig = 0;

                string natOp = NaturezaOperacao.Fluxo.BuscarEValidar.Instance.ObtemCodigoControle(idNatOp);
                string cstIcms = NaturezaOperacao.Fluxo.BuscarEValidar.Instance.ObtemCstIcms(idNatOp);
                var cstIpi = NaturezaOperacao.Fluxo.BuscarEValidar.Instance.ObtemCstIpi(idNatOp);

                if (cstIpi == null)
                    cstIpi = prod.CstIpi;

                var mva = MvaProdutoUfDAO.Instance.ObterMvaPorProduto(null, prod.IdProd, idLoja, idFornec, idCli,
                    (tipoDocumento == Glass.Data.Model.NotaFiscal.TipoDoc.Saída ||
                    /* Chamado 32984. */
                    (tipoDocumento == Glass.Data.Model.NotaFiscal.TipoDoc.Entrada &&
                    CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNatOp)))));
                var icms = IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)prod.IdProd, idLoja, (uint?)idFornec, idCli);
                var fcp = IcmsProdutoUfDAO.Instance.ObterFCPPorProduto(null, (uint)prod.IdProd, idLoja, (uint?)idFornec, idCli);
                var fcpSt = IcmsProdutoUfDAO.Instance.ObterAliquotaFCPSTPorProduto(null, (uint)prod.IdProd, idLoja, (uint?)idFornec, idCli);

                var ncmNaturezaOp = idNatOp > 0 ? NaturezaOperacaoDAO.Instance.ObtemNcm(null, idNatOp) : null;
                var ncm = !string.IsNullOrEmpty(ncmNaturezaOp) ? ncmNaturezaOp : prod.Ncm;

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0") + ";" +
                    (cstIcms ?? prod.Cst ?? String.Empty) + ";" +
                    (!String.IsNullOrEmpty(prod.Csosn) ? prod.Csosn : String.Empty) + ";" + icms + ";" + fcp + ";" + prod.AliqIPI +
                    ";" + natOp + ";" + ncm + ";" + mva + ";" + prod.Altura + ";" +
                    prod.Largura + ";" + prod.AliqIcmsStInterna + ";" + cstIpi + ";" + prod.IdContaContabil + ";" +
                    (prod.IdUnidadeMedida != prod.IdUnidadeMedidaTrib).ToString().ToLower() + ";" + cstOrig + ";" + fcpSt;

                return retorno;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        public string GetProdObra(string codInterno, string idClienteStr)
        {
            var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";

            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : String.Empty);

            else
            {
                uint? idCliente = !String.IsNullOrEmpty(idClienteStr) ? (uint?)Glass.Conversoes.StrParaUint(idClienteStr) : null;
                return "Prod;" + prod.IdProd + ";" + prod.Descricao + ";" +
                    ProdutoDAO.Instance.GetValorTabela(prod.IdProd, null, idCliente, false, false, 0, null, null, null).ToString("0.00");
            }
        }

        public string GetProdutoOrca(string codInterno, string tipoEntrega, string revenda, string idCliente,
            string percComissao, string percDescontoQtdeStr, string idLoja, string idOrcamento)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, null, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUintNullable(idCliente), null, true);

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";

                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

                else if (prod.Compra)
                    return "Erro;Produto utilizado apenas na compra.";

                if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra && prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para orçamentos.";

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
                decimal valorProduto = 0;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true", false, percDescontoQtde, null, null, idOrcamento.StrParaIntNullable());

                if (PedidoConfig.Comissao.ComissaoPedido)
                    valorProduto = valorProduto / (decimal)((100 - float.Parse(percComissao)) / 100);

                retorno += ";" + valorProduto.ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(null, UserInfo.GetUserInfo.IdLoja,
                    (uint)prod.IdProd).ToString() : "999999999");

                // Verifica como deve ser feito o cálculo do produto
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);

                // Retorna a espessura do produto
                retorno += ";" + prod.Espessura;

                // Retorna a alíquota ICMS do produto
                retorno += ";" + prod.AliqICMSInterna.ToString().Replace(',', '.');

                //if (isPedidoProducao)
                retorno += ";" + (prod.Altura != null ? prod.Altura.Value.ToString() : "") + ";" + (prod.Largura != null ?
                    prod.Largura.Value.ToString() : "");

                retorno += ";" + prod.IdCorVidro + ";" + prod.Forma;

                return retorno;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro;Falha ao buscar produto.", ex);
            }
        }

        public string GetProdutoPedido(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string percComissao, string tipoPedidoStr, string tipoVendaStr, string ambienteMaoObra,
            string percDescontoQtdeStr, string idLoja, bool produtoComposto)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                int tipoPedido = Glass.Conversoes.StrParaInt(tipoPedidoStr);
                bool isAmbienteMaoObra = ambienteMaoObra.ToLower() == "true";

                bool isPedidoMaoObraEspecial = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
                bool isPedidoMaoObra = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra;
                bool isPedidoProducao = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao;
                int tipoVenda = Glass.Conversoes.StrParaInt(tipoVendaStr);

                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, null, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUintNullable(idCliente), null, true);

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";
                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
                else if (prod.Compra)
                    return "Erro;Produto utilizado apenas na compra.";

                var subGrupo = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(prod.IdSubgrupoProd.GetValueOrDefault()) ?? new Glass.Data.Model.SubgrupoProd();

                if (isPedidoMaoObra)
                {
                    if (!isAmbienteMaoObra)
                    {
                        if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                            return "Erro;Apenas produtos do grupo 'Mão de Obra Beneficiamento' podem ser incluídos nesse pedido.";
                    }
                    else if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                        return "Erro;Apenas produtos do grupo 'Vidro' podem ser usados como peça de vidro.";
                }
                else if (isPedidoProducao && !ProdutoPedidoProducaoDAO.Instance.PedidoProducaoGeradoPorPedidoRevenda(null, idPedido))
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro' marcados como 'Produtos para Estoque' podem ser incluídos nesse pedido.";

                    if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa((uint)prod.IdProd))
                        return "Erro;Esse produto ainda não possui um produto associado. Para usá-lo aqui é preciso que você altere o cadastro desse produto e associe o produto final.";
                }
                else if (isPedidoMaoObraEspecial)
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro', e que não são marcados como 'Produtos para Estoque', podem ser utilizados nesse pedido.";

                    if (PedidoConfig.DadosPedido.BloquearItensCorEspessura && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(null, Glass.Conversoes.StrParaUint(idLoja)))
                    {
                        var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(idPedido);

                        if (produtosPedido != null)
                            foreach (var prodped in produtosPedido.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0))
                                if (prodped.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                                    if (ProdutoDAO.Instance.ObtemIdCorVidro((int)prodped.IdProd) != prod.IdCorVidro ||
                                        ProdutoDAO.Instance.ObtemEspessura((int)prodped.IdProd) != prod.Espessura)
                                        return "Erro;Todos os produtos devem ter a mesma cor e espessura.";
                    }
                }
                else if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra &&
                    prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.";

                //Verifica se o produto é uma embalagem (Item de revenda que pode ser incluído em pedido de venda)
                else if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && !produtoComposto && !subGrupo.PermitirItemRevendaNaVenda)
                {
                    if (tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda &&
                        (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro ||
                        (prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))) &&
                        prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                        return "Erro;Produtos de revenda não podem ser incluídos em um pedido de venda.";

                    else if (tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda &&
                        ((prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd)) ||
                        prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra))
                        return "Erro;Produtos de venda não podem ser incluídos em um pedido de revenda.";

                    // Impede que sejam inseridos produtos de cor ou espessura diferentes dos que já foram inseridos.
                    else if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(null, Glass.Conversoes.StrParaUint(idLoja))) && tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda)
                    {
                        var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(idPedido);

                        if (produtosPedido != null)
                            foreach (var prodped in produtosPedido.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0))
                                if (prodped.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                                    if (ProdutoDAO.Instance.ObtemIdCorVidro((int)prodped.IdProd) != prod.IdCorVidro ||
                                        ProdutoDAO.Instance.ObtemEspessura((int)prodped.IdProd) != prod.Espessura)
                                        return "Erro;Todos os produtos devem ter a mesma cor e espessura.";
                    }

                    //Impede que sejam inseridos produtos comuns com vidros duplos e laminados
                    if (tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda && prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro &&
                        !PedidoConfig.TelaCadastro.PermitirInserirVidroComumComComposicao)
                    {
                        foreach (var prodped in ProdutosPedidoDAO.Instance.GetByPedido(idPedido))
                        {
                            var itemRevendaNaVenda = prodped.IdSubgrupoProd == 0 ?
                                false :
                                SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda((int)prodped.IdSubgrupoProd);

                            var tipo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)prodped.IdProd);
                            var tipoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)prod.IdProd);

                            if (((tipo != TipoSubgrupoProd.VidroDuplo && tipo != TipoSubgrupoProd.VidroLaminado && (tipoProd == TipoSubgrupoProd.VidroDuplo || tipoProd == TipoSubgrupoProd.VidroLaminado)) ||
                                ((tipo == TipoSubgrupoProd.VidroDuplo || tipo == TipoSubgrupoProd.VidroLaminado) && tipoProd != TipoSubgrupoProd.VidroDuplo && tipoProd != TipoSubgrupoProd.VidroLaminado)) &&
                                !itemRevendaNaVenda)
                                return "Erro;Não é possivel inserir produtos do tipo de subgrupo vidro duplo ou laminado junto com produtos comuns e temperados.";
                        }
                    }
                }
                // Se o pedido tiver forma de pagamento Obra, não permite inserir produto com tipo subgrupo VidroLaminado ou VidroDuplo sem produto de composição.
                if (PedidoDAO.Instance.GetIdObra(null, idPedido) > 0 &&
                    (subGrupo.TipoSubgrupo == TipoSubgrupoProd.VidroLaminado || subGrupo.TipoSubgrupo == TipoSubgrupoProd.VidroDuplo))
                {
                    if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa((uint)prod.IdProd))
                        return "Erro;Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.";
                }

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
                decimal valorProduto = 0;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true",
                    tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição, percDescontoQtde, (int?)idPedido, null, null);

                if (PedidoConfig.Comissao.ComissaoPedido && PedidoConfig.Comissao.ComissaoAlteraValor)
                    valorProduto = valorProduto / (decimal)((100 - float.Parse(percComissao)) / 100);

                retorno += ";" + valorProduto.ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd) && !isPedidoProducao;

                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(Glass.Conversoes.StrParaUint(idLoja),
                    (uint)prod.IdProd, isPedidoProducao).ToString() : "999999999");

                // Verifica como deve ser feito o cálculo do produto
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);

                // Retorna a espessura do produto
                retorno += ";" + prod.Espessura;

                var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(null, idPedido);
                // Retorna a alíquota ICMS do produto
                retorno += ";" + (LojaDAO.Instance.ObtemCalculaIcmsStPedido(null, idLojaPedido) ? prod.AliqICMSInterna.ToString().Replace(',', '.') : "0");

                //if (isPedidoProducao)
                retorno += ";" + (prod.Altura != null ? prod.Altura.Value.ToString() : "") + ";" + (prod.Largura != null ? prod.Largura.Value.ToString() : "");

                retorno += ";" + prod.IdCorVidro + ";" + prod.Forma;

                retorno += ";" + ProdutoDAO.Instance.ExibirMensagemEstoque(prod.IdProd).ToString().ToLower();
                retorno += ";" + ProdutoLojaDAO.Instance.GetEstoque(Glass.Conversoes.StrParaUint(idLoja), (uint)prod.IdProd, isPedidoProducao);

                // Retornar aplicação e processo associados
                if (prod.IdAplicacao > 0)
                    retorno += ";" + prod.IdAplicacao.Value + ";" + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno((uint)prod.IdAplicacao.Value);
                else
                    retorno += ";;";

                if (prod.IdProcesso > 0)
                    retorno += ";" + prod.IdProcesso + ";" + EtiquetaProcessoDAO.Instance.ObtemCodInterno((uint)prod.IdProcesso);
                else
                    retorno += ";;";

                retorno += ";" + ProdutoDAO.Instance.ObtemCustoCompra(prod.IdProd);

                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
                var configuracao = ComissaoConfigDAO.Instance.GetComissaoConfig(pedido.IdFunc);
                float percFaixa = 0;

                if (PedidoConfig.Comissao.PerComissaoPedido)
                {
                    if (pedido.Total < configuracao.FaixaUm)
                        percFaixa = configuracao.PercFaixaUm;
                    else if (pedido.Total < configuracao.FaixaDois)
                        percFaixa = configuracao.PercFaixaDois;
                    else if (pedido.Total < configuracao.FaixaTres)
                        percFaixa = configuracao.PercFaixaTres;
                    else if (pedido.Total < configuracao.FaixaQuatro)
                        percFaixa = configuracao.PercFaixaQuatro;
                    else
                        percFaixa = configuracao.PercFaixaCinco;
                }

                retorno += ";" + percFaixa;

                return retorno;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro;Falha ao buscar produto.", ex);
            }
        }

        public string GetProdutoPcp(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string tipoPedidoStr, string ambienteMaoObra, string percDescontoQtdeStr, string idLoja)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                int tipoPedido = Glass.Conversoes.StrParaInt(tipoPedidoStr);
                bool isAmbienteMaoObra = ambienteMaoObra.ToLower() == "true";

                bool isPedidoMaoObraEspecial = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
                bool isPedidoMaoObra = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra;
                bool isPedidoProducao = tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao;

                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, null, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUintNullable(idCliente), null, true);
                var subGrupo = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(prod.IdSubgrupoProd.GetValueOrDefault()) ?? new Glass.Data.Model.SubgrupoProd();

                if (prod == null || prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Não existe produto com o código informado.";

                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!string.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

                if (isPedidoMaoObra)
                {
                    if (!isAmbienteMaoObra)
                    {
                        if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                            return "Erro;Apenas produtos do grupo 'Mão de Obra Beneficiamento' podem ser incluídos nesse pedido.";
                    }
                    else if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                        return "Erro;Apenas produtos do grupo 'Vidro' podem ser usados como peça de vidro.";
                }
                else if (isPedidoProducao && !ProdutoPedidoProducaoDAO.Instance.PedidoProducaoGeradoPorPedidoRevenda(null, idPedido))
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro' marcados como 'Produtos para Estoque' podem ser incluídos nesse pedido.";

                    if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa((uint)prod.IdProd))
                        return "Erro;Esse produto ainda não possui um produto associado. Para usá-lo aqui é preciso que você altere o cadastro desse produto e associe o produto final.";
                }
                else if (isPedidoMaoObraEspecial)
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro', e que não são marcados como 'Produtos para Estoque', podem ser utilizados nesse pedido.";
                }
                else if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra && prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.";
                //Verifica se o produto é uma embalagem (Item de revenda que pode ser incluído em pedido de venda)
                else if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && !subGrupo.PermitirItemRevendaNaVenda)
                {
                    if (tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda &&
                        (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro ||
                        (prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))) &&
                        prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                        return "Erro;Produtos de revenda não podem ser incluídos em um pedido de venda.";

                    else if (tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda &&
                        ((prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd)) ||
                        prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra))
                        return "Erro;Produtos de venda não podem ser incluídos em um pedido de revenda.";

                    // Impede que sejam inseridos produtos de cor ou espessura diferentes dos que já foram inseridos.
                    else if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(null, Glass.Conversoes.StrParaUint(idLoja))) && tipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda)
                    {
                        var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(idPedido);

                        if (produtosPedido != null)
                            foreach (var prodped in produtosPedido.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0))
                                if (prodped.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                                    if (ProdutoDAO.Instance.ObtemIdCorVidro((int)prodped.IdProd) != prod.IdCorVidro ||
                                        ProdutoDAO.Instance.ObtemEspessura((int)prodped.IdProd) != prod.Espessura)
                                        return "Erro;Todos os produtos devem ter a mesma cor e espessura.";
                    }
                }
                // Se o pedido tiver forma de pagamento Obra, não permite inserir produto com tipo subgrupo VidroLaminado ou VidroDuplo sem produto de composição.
                if (PedidoDAO.Instance.GetIdObra(null, idPedido) > 0 &&
                    (subGrupo.TipoSubgrupo == TipoSubgrupoProd.VidroLaminado || subGrupo.TipoSubgrupo == TipoSubgrupoProd.VidroDuplo))
                {
                    if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa((uint)prod.IdProd))
                        return "Erro;Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.";
                }

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";
                else
                {
                    string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
                    decimal valorProduto = 0;

                    bool pedidoReposicao = false;
                    if (!string.IsNullOrEmpty(idPedidoStr))
                        pedidoReposicao = PedidoDAO.Instance.ObtemTipoVenda(null, idPedido) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;

                    // Recupera o valor de tabela do produto
                    int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                    uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                    float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                    valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true", pedidoReposicao, percDescontoQtde, (int?)idPedido, null, null);

                    retorno += ";" + valorProduto.ToString("F2");

                    retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                        Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd).ToString().ToLower() + ";" +
                        (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                    retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);
                    retorno += ";" + prod.Espessura;
                    retorno += ";" + prod.AliqICMSInterna.ToString().Replace(',', '.');
                    retorno += ";" + prod.Forma;
                    retorno += ";" + (prod.Altura > 0 ? prod.Altura : 0);
                    retorno += ";" + (prod.Largura > 0 ? prod.Largura : 0);

                    bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd) && !isPedidoProducao;

                    retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(UserInfo.GetUserInfo.IdLoja,
                        (uint)prod.IdProd, Glass.Conversoes.StrParaUintNullable(idPedidoStr), isPedidoProducao, false, true).ToString() : "999999999");

                    retorno += ";" + ProdutoDAO.Instance.ExibirMensagemEstoque(prod.IdProd).ToString().ToLower();
                    retorno += ";" + ProdutoLojaDAO.Instance.GetEstoque(UserInfo.GetUserInfo.IdLoja, (uint)prod.IdProd, isPedidoProducao);

                    // Retornar aplicação e processo associados
                    if (prod.IdAplicacao > 0) retorno += ";" + prod.IdAplicacao.Value + ";" + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno((uint)prod.IdAplicacao.Value);
                    else retorno += ";;";

                    // var idRoteiroProduto = RoteiroProducaoDAO.Instance.ObtemRoteiroProduto(prod.IdProd);
                    int? idProcesso = /*idRoteiroProduto > 0 ? RoteiroProducaoDAO.Instance.ObtemIdProcesso(idRoteiroProduto.Value) :*/ null;

                    if ((prod.IdProcesso ?? idProcesso) > 0) retorno += ";" + (prod.IdProcesso ?? idProcesso).Value + ";" + EtiquetaProcessoDAO.Instance.ObtemCodInterno((uint)(prod.IdProcesso ?? idProcesso).Value);
                    else retorno += ";;";

                    return retorno;
                }
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro;", ex);
            }
        }

        public string UsarDiferencaM2Prod(string codInternoProd)
        {
            var prod = ProdutoDAO.Instance.GetByCodInterno(codInternoProd);
            if (prod == null)
                return "false";

            return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd) && prod.IdSubgrupoProd != (int)Utils.SubgrupoProduto.LevesDefeitos &&
                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false) != (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd).ToString().ToLower();
        }

        public string GetProdutoInterno(string codInterno)
        {
            var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";

            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

            //else if (!prod.Compra)
            //    return "Erro;Produto não pode ser utilizado em pedidos internos.";

            string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;

            bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd);
            retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(null, UserInfo.GetUserInfo.IdLoja,
                (uint)prod.IdProd, null, false, false, false).ToString() : "999999999");

            // Verifica como deve ser feito o cálculo do produto
            retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);

            retorno += ";" + (prod.Altura != null ? prod.Altura.Value.ToString() : "") + ";" + (prod.Largura != null ?
                prod.Largura.Value.ToString() : "");

            return retorno;
        }

        public string GetProdutoPreco(string codInterno, string tipoPreco)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                if (prod == null)
                    return "Erro;Não há nenhum produto cadastrado com este código.";

                decimal preco = tipoPreco == "0" ? prod.Custofabbase :
                    tipoPreco == "1" ? prod.CustoCompra :
                    tipoPreco == "2" ? prod.ValorAtacado :
                    tipoPreco == "3" ? prod.ValorBalcao :
                    tipoPreco == "4" ? prod.ValorObra : 0;

                return "Ok;" + prod.IdProd + ";" + prod.Descricao + ";" + (preco > 0 ? preco.ToString() : String.Empty);

            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao carregar produto.", ex);
            }
        }

        public string GetProdutoProduto(string codInterno)
        {
            try
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                if (prod == null)
                    throw new Exception("Produto não encontrado.");

                if (ProdutoDAO.Instance.IsProdutoProducao(null, prod.IdProd))
                    throw new Exception("Produto de produção não pode ser associado a este produto.");

                return "Ok;" + prod.IdProd + ";" + prod.Descricao;
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }
    }
}
