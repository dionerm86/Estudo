using System;
using Glass.Data.Model;
using System.Xml;
using Glass.Data.DAL;
using Glass.Data.NFeUtils;

namespace Glass.Data.CFeUtils
{
    public static class FormataXML
    {
        #region Atributos

        private static string _versaoDadosEnt = "1.0";
        private static string _cnpjSoftwareHouse = "11111111111111";

        #endregion  

        /// <summary>
        /// Método que gera o XML a ser enviado para o Aparelho SAT para REGISTRAR uma venda
        /// </summary>
        /// <param name="cupomComPrestacaoServico">Bool: Indica se há itens de prestação de serviço no cupom</param>
        /// <param name="ratearDescontoItensServico">Bool: Indica se o ISSQN deve ser rateado entre os itens de prestação de serviço</param>
        /// <param name="objCliente">Cliente: Cliente que realizou a compra</param>
        /// <param name="entregarPedido">Bool: Indica se o pedido será entregue em domicílio</param>
        /// <param name="lstProduto">Array: Array de produtos que compoem o CFe (Max=500)</param>
        /// <param name="idNaturezaOperacao">UInt32: Código da natureza de operação</param>
        /// <param name="loja">Loja: Loja responsável pela emissão do CFe</param>
        /// <param name="lstPagamento">Array: Array de PagamentoCFe com as formas de pagamento do CFe (Max=10)</param>
        /// <param name="Observacoes">String: Informações adicionais de interesse do Emissor do CFe (Max=5000)</param>
        /// <returns></returns>
        public static string MontarXmlVenda(bool cupomComPrestacaoServico, bool ratearDescontoItensServico, Cliente objCliente,
            bool entregarPedido, ProdutosNf[] lstProduto, uint idNaturezaOperacao, Loja loja, PagamentoCFe[] lstPagamento, string Observacoes)
        {
            try
            {
                #region Validações

                string Erro = "";

                // Cada cupom deve possuir entre 1 e 500 ocorrencias de produtos.
                if (lstProduto == null || lstProduto.Length == 0)
                    Erro += "Não há produtos neste cupom.\r\n";
                else if (lstProduto.Length > 500)
                    Erro += "Número máximo de produtos excedido (Max=500).\r\n";

                //CNPJ da Loja é obrigatório
                if (String.IsNullOrEmpty(loja.Cnpj))
                    Erro += "Loja sem CNPJ cadastrado no sistema.\r\n";

                // Inscrição Estadual é obrigatória
                if(String.IsNullOrEmpty(loja.InscEst))
                    Erro += "Loja sem inscrição estadual cadastrada no sistema.\r\n";

                if (cupomComPrestacaoServico && String.IsNullOrEmpty(loja.InscMunic))
                    Erro += "Para emitir Cupom Fiscal com prestação de serviços é necessário cadastrar"
                         + "a Inscrição Estadual da loja no sistema.\r\n";

                // Validação das formas de pagamento do CFe
                if (lstPagamento == null || lstPagamento.Length == 0)
                {
                    Erro += "Pelo menos uma forma de pagamento deve ser informada.\r\n";
                }
                else if(lstPagamento.Length > 10)
                {
                    Erro += "O CFe deve possuir no máximo 10 formas de pagamento.\r\n";
                }
                else
                {

                    foreach (PagamentoCFe pag in lstPagamento)
                    {
                        if ((pag.FormaPagamento == PagamentoCFe.FormaPagamentoCFe.CartaoCredito
                            || pag.FormaPagamento == PagamentoCFe.FormaPagamentoCFe.CartaoDebito)
                            && (pag.OperadoraCartao == PagamentoCFe.OperadorasCartaoCreditoCFe.Nenhum))
                            Erro += "A operadora do cartão referente ao pagamento de valor "
                                 + pag.ValorPagamento.ToString("C") + " não foi informada.\r\n";
                    }
                }

                if (!String.IsNullOrEmpty(Observacoes) && Observacoes.Length > 5000)
                    Erro += "As observações do CFe devem possuir no máximo 5000 caracteres.\r\n";

                if (!String.IsNullOrEmpty(Erro))
                    throw new Exception(Erro);

                #endregion

                #region Declarações de Objetos

                XmlDocument docXml = new XmlDocument();
                XmlElement nodePai;
                XmlNode node;
                XmlNode nodeDet;
                XmlNode nodeImposto;
                XmlNode nodeNomeImposto;
                XmlAttribute attr;

                #endregion

                #region XML Declaration

                node = docXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                docXml.AppendChild(node);

                #endregion

                #region Raiz do Documento XML

                // Nodo CFe (Raiz do documento)
                node = docXml.CreateElement("CFe");
                attr = docXml.CreateAttribute("xmlns");
                attr.Value = "http://www.fazenda.sp.gov.br/sat";
                node.Attributes.Append(attr);
                docXml.AppendChild(node);

                #endregion

                #region Nodo Base do Cupom

                // Nodo infCFe (Base para o cupom)
                node = docXml.CreateElement("infCFe");
                attr = docXml.CreateAttribute("versaoDadosEnt");
                attr.Value = _versaoDadosEnt;
                node.Attributes.Append(attr);
                docXml.SelectSingleNode("/CFe").AppendChild(node);

                #endregion

                #region Informações Basicas Cupom

                // Nodo ide (Informações Básicas do Cupom)
                nodePai = docXml.CreateElement("ide");
                docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                #endregion

                #region Filhos Nodo ide

                // CNPJ
                node = docXml.CreateElement("CNPJ");
                node.InnerText = _cnpjSoftwareHouse.PadLeft(14, '0');
                nodePai.AppendChild(node);

                // signAC
                node = docXml.CreateElement("signAC");
                node.InnerText = _cnpjSoftwareHouse + loja.Cnpj;
                nodePai.AppendChild(node);

                #endregion

                #region Informações Estabelecimento Emissor

                // Nodo emit (Informações do Estabelecimento Emissor)
                nodePai = docXml.CreateElement("emit");
                docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                #region Filhos Nodo emit

                // CNPJ do Estabelecimento Emissor
                node = docXml.CreateElement("CNPJ");
                node.InnerText = loja.Cnpj.PadLeft(14, '0');
                nodePai.AppendChild(node);

                // Inscrição Estadual
                node = docXml.CreateElement("IE");
                node.InnerText = loja.InscEst.PadLeft(12, '0');
                nodePai.AppendChild(node);

                // Inscrição Municipal (Preenchido apenas quando  quando ocorrer 
                // a emissão de CF-e conjugada, com prestação de serviços sujeitos
                // ao ISSQN e fornecimento de peças sujeitos ao ICMS.
                if (cupomComPrestacaoServico)
                {
                    node = docXml.CreateElement("IM");
                    node.InnerText = loja.InscMunic;
                    nodePai.AppendChild(node);
                }

                // cRegTribISSQN - Regime Especial de Tributação do ISSQN
                // Prestação de serviços
                /*if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("regTributISSQN")))
                {
                    loja.RegimeLoja;
                    node = docXml.CreateElement("cRegTribISSQN");
                    node.InnerText = ConfigurationManager.AppSettings.Get("regTributISSQN");
                    nodePai.AppendChild(node);
                }*/

                // indRatISSQN - Indicador de rateio do Desconto sobre subtotal entre itens sujeitos à tributação pelo ISSQN. 
                node = docXml.CreateElement("indRatISSQN");
                node.InnerText = (ratearDescontoItensServico) ? "S" : "N";
                nodePai.AppendChild(node);

                #endregion

                #endregion

                #region Informações Consumidor

                // Nodo dest (Informações do Consumidor)
                nodePai = docXml.CreateElement("dest");
                docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                #region Filhos Nodo dest

                // CPF ou CNPJ do Cliente
                if (objCliente != null && objCliente.TipoPessoa == "J")
                {
                    node = docXml.CreateElement("CNPJ");
                    node.InnerText = objCliente.CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "").PadLeft(14, '0');
                    nodePai.AppendChild(node);
                }
                else if (objCliente != null && objCliente.TipoPessoa == "F")
                {
                    node = docXml.CreateElement("CPF");
                    node.InnerText = objCliente.CpfCnpj.Replace(".", "").Replace("-", "").PadLeft(11, '0');
                    nodePai.AppendChild(node);
                }

                // Nome do Cliente, caso o pedido seja entregue em domicílio
                if (entregarPedido && objCliente != null)
                {
                    node = docXml.CreateElement("xNome");
                    node.InnerText = (objCliente.Nome.Length <= 60) ? objCliente.Nome : objCliente.Nome.Substring(0, 59);
                    nodePai.AppendChild(node);
                }

                #endregion

                #endregion

                #region Entrega

                // Nodo entrega (Informado apenas quando o pedido será entregue em domicílio)
                if (entregarPedido && objCliente != null)
                {
                    nodePai = docXml.CreateElement("entrega");
                    docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                    #region Filhos Nodo entrega

                    // Logradouro
                    node = docXml.CreateElement("xLgr");
                    node.InnerText = (String.IsNullOrEmpty(objCliente.EnderecoEntrega)) ? "--" : objCliente.EnderecoEntrega;
                    nodePai.AppendChild(node);

                    // Numero
                    node = docXml.CreateElement("nro");
                    node.InnerText = (String.IsNullOrEmpty(objCliente.NumeroEntrega)) ? "-" : objCliente.NumeroEntrega;
                    nodePai.AppendChild(node);

                    // Complemento
                    if (!String.IsNullOrEmpty(objCliente.ComplEntrega))
                    {
                        node = docXml.CreateElement("xCpl");
                        node.InnerText = objCliente.ComplEntrega;
                        nodePai.AppendChild(node);
                    }

                    // Bairro
                    node = docXml.CreateElement("xBairro");
                    node.InnerText = (String.IsNullOrEmpty(objCliente.BairroEntrega)) ? "-" : objCliente.BairroEntrega;
                    nodePai.AppendChild(node);

                    // Municipio
                    node = docXml.CreateElement("xMun");
                    node.InnerText = (String.IsNullOrEmpty(objCliente.CidadeEntrega)) ? "--" : objCliente.CidadeEntrega;
                    nodePai.AppendChild(node);

                    // UF
                    node = docXml.CreateElement("UF");
                    node.InnerText = (String.IsNullOrEmpty(objCliente.Uf)) ? "--" : objCliente.Uf;
                    nodePai.AppendChild(node);

                    #endregion
                }

                #endregion

                #region Produtos

                for (int i = 0; i < lstProduto.Length; i++)
                {
                    // Nodo det (Pai do Nodo prod e do Nodo imposto)
                    nodeDet = docXml.CreateElement("det");
                    attr = docXml.CreateAttribute("nItem");
                    attr.Value = (i + 1).ToString();
                    nodeDet.Attributes.Append(attr);

                    // Insere o Nodo det no documento
                    docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodeDet);

                    // Insere o Nodo prod no Nodo det
                    nodePai = docXml.CreateElement("prod");
                    nodeDet.AppendChild(nodePai);

                    #region Informacoes Produto

                    // cProd - Código do Produto
                    node = docXml.CreateElement("cProd");
                    node.InnerText = lstProduto[i].CodInterno;
                    nodePai.AppendChild(node);

                    // cEAN - GTIN (Global Trade Item Number) do produto.
                    if (!String.IsNullOrEmpty(lstProduto[i].GTINProduto))
                    {
                        node = docXml.CreateElement("cEAN");
                        node.InnerText = lstProduto[i].GTINProduto;
                        nodePai.AppendChild(node);
                    }

                    // xProd - Descrição do Produto
                    node = docXml.CreateElement("xProd");
                    node.InnerText = lstProduto[i].DescrProduto;
                    nodePai.AppendChild(node);

                    // NCM - Código NCM com 8 dígitos ou 2 dígitos (gênero)
                    if (!String.IsNullOrEmpty(lstProduto[i].Ncm))
                    {
                        node = docXml.CreateElement("NCM");
                        node.InnerText = lstProduto[i].Ncm;
                        nodePai.AppendChild(node);
                    }

                    // CFOP - Código Fiscal de Operações e Prestações 
                    node = docXml.CreateElement("CFOP");
                    node.InnerText = CfopDAO.Instance.ObtemCodInterno(NaturezaOperacaoDAO.Instance.ObtemIdCfop(idNaturezaOperacao));
                    nodePai.AppendChild(node);

                    // uCom - Unidade Comercial
                    node = docXml.CreateElement("uCom");
                    node.InnerText = (lstProduto[i].Unidade.Length < 7) ? lstProduto[i].Unidade : lstProduto[i].Unidade.Substring(0, 5);
                    nodePai.AppendChild(node);

                    // qCom - Quantidade Comercial
                    node = docXml.CreateElement("qCom");
                    node.InnerText = Math.Round(ProdutosNfDAO.Instance.ObtemQtdDanfe(lstProduto[i]), 4).ToString().Replace(",", ".");
                    nodePai.AppendChild(node);

                    // vUnCom  - Valor Unitário
                    node = docXml.CreateElement("vUnCom");
                    node.InnerText = Math.Round(lstProduto[i].ValorUnitario, 2).ToString().Replace(",", ".");
                    nodePai.AppendChild(node);

                    // indRegra - A = Arredondamento | T = Truncamento
                    // Valor deve ser arredondado, com exceção de operação com combustíveis, quando deve ser truncado
                    // (Convenio ICMS 85/01 e Portaria 30/94 do DNC) 
                    node = docXml.CreateElement("indRegra");
                    node.InnerText = "A";
                    nodePai.AppendChild(node);

                    #endregion

                    // Início Imposto
                    nodeImposto = docXml.CreateElement("imposto");
                    nodeDet.AppendChild(nodeImposto);

                    #region ICMS

                    // Nodo ICMS
                    nodeNomeImposto = docXml.CreateElement("ICMS");
                    nodeImposto.AppendChild(nodeNomeImposto);

                    // Se for regime normal
                    if (loja.Crt == 3 || loja.Crt == 4)
                    {
                        switch (lstProduto[i].Cst)
                        {
                            case "00":
                            case "20":
                            case "90":
                                nodePai = docXml.CreateElement("ICMS00");
                                ManipulacaoXml.SetNode(docXml, nodePai, "orig", lstProduto[i].CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].Cst);
                                ManipulacaoXml.SetNode(docXml, nodePai, "pICMS", Formatacoes.TrataValorDecimal((decimal)lstProduto[i].AliqIcms, 2));
                                nodeNomeImposto.AppendChild(nodePai);
                                break;
                            case "40":
                            case "41":
                            case "50":
                            case "60":
                                nodePai = docXml.CreateElement("ICMS40");
                                ManipulacaoXml.SetNode(docXml, nodePai, "orig", lstProduto[i].CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].Cst);
                                nodeNomeImposto.AppendChild(nodePai);
                                break;
                            case "":
                                throw new Exception("Informe o CST de todos os produtos.");
                            default:
                                throw new Exception("CST informada no produto não existe.");
                        }
                    }
                    else // Simples Nacional
                    {
                        switch (lstProduto[i].Csosn)
                        {
                            case "102":
                            case "300":
                            case "400":
                            case "500":
                                nodePai = docXml.CreateElement("ICMSSN102");
                                ManipulacaoXml.SetNode(docXml, nodePai, "orig", lstProduto[i].CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(docXml, nodePai, "CSOSN", lstProduto[i].Csosn);
                                nodeNomeImposto.AppendChild(nodePai);
                                break;
                            case "900":
                                nodePai = docXml.CreateElement("ICMSSN900");
                                ManipulacaoXml.SetNode(docXml, nodePai, "orig", lstProduto[i].CstOrig.ToString()); // Nacional
                                ManipulacaoXml.SetNode(docXml, nodePai, "CSOSN", lstProduto[i].Csosn);
                                ManipulacaoXml.SetNode(docXml, nodePai, "pICMS", Formatacoes.TrataValorDecimal((decimal)lstProduto[i].AliqIcms, 2));
                                nodeNomeImposto.AppendChild(nodePai);
                                break;
                            case "":
                            case null:
                                throw new Exception("Informe o CSOSN de todos os produtos da nota.");
                            default:
                                throw new Exception("CSOSN não implantada.");
                        }
                    }

                    #endregion

                    #region PIS

                    nodeNomeImposto = docXml.CreateElement("PIS");
                    nodeImposto.AppendChild(nodeNomeImposto);

                    if (loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal)
                    {
                        bool calcPis = NaturezaOperacaoDAO.Instance.CalculaPis(lstProduto[i].IdNaturezaOperacao.Value);

                        if (lstProduto[i].CstPis != null)
                        {
                            switch (lstProduto[i].CstPis.Value)
                            {
                                case 1:
                                case 2:
                                    nodePai = docXml.CreateElement("PISAliq");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].CstPis.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "vBC", Formatacoes.TrataValorDouble(calcPis ? (double)lstProduto[i].BcPis : 0, 2));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "pPIS", Formatacoes.TrataValorDouble(calcPis ? (lstProduto[i].AliqPis > 0 ? lstProduto[i].AliqPis : ConfigNFe.AliqPis((uint)loja.IdLoja)) : 0, 4));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                                case 3:
                                    throw new Exception("PIS CST 03 não implementado.");
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    nodePai = docXml.CreateElement("PISNT");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].CstPis.Value.ToString().PadLeft(2, '0'));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                                case 49:
                                    nodePai = docXml.CreateElement("PISSN");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].CstPis.Value.ToString().PadLeft(2, '0'));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                                default:
                                    nodePai = docXml.CreateElement("PISOutr");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", "99");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "vBC", Formatacoes.TrataValorDouble(calcPis ? (double)lstProduto[i].BcPis : 0, 2));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "pPIS", Formatacoes.TrataValorDouble(calcPis ? (lstProduto[i].AliqPis > 0 ? lstProduto[i].AliqPis : ConfigNFe.AliqPis((uint)loja.IdLoja)) : 0, 4));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                            }
                        }
                    }
                    else // Simples Nacional (Não destaca PIS)
                    {
                        if (lstProduto[i].CstPis == 49)
                        {
                            nodePai = docXml.CreateElement("PISNT");
                            ManipulacaoXml.SetNode(docXml, nodePai, "CST", "49"); // Operação sem incidência da contribuição
                            nodeNomeImposto.AppendChild(nodePai);
                        }
                    }

                    #endregion

                    #region PISST

                    #endregion

                    #region COFINS

                    nodeNomeImposto = docXml.CreateElement("COFINS");
                    nodeImposto.AppendChild(nodeNomeImposto);

                    // Regime normal (Destaca COFINS)
                    if (loja.Crt == (int)CrtLoja.LucroPresumido || loja.Crt == (int)CrtLoja.LucroReal)
                    {
                        bool calcCofins = NaturezaOperacaoDAO.Instance.CalculaCofins(lstProduto[i].IdNaturezaOperacao.Value);

                        if (lstProduto[i].CstCofins == null || lstProduto[i].CstCofins == 0)
                        {
                            nodePai = docXml.CreateElement("COFINSAliq");
                            ManipulacaoXml.SetNode(docXml, nodePai, "CST", ConfigNFe.CstPisCofins(lstProduto[i].IdNf).ToString("0#"));
                            ManipulacaoXml.SetNode(docXml, nodePai, "vBC", Formatacoes.TrataValorDouble(calcCofins ? (double)lstProduto[i].BcCofins : 0, 2));
                            ManipulacaoXml.SetNode(docXml, nodePai, "pCOFINS", Formatacoes.TrataValorDouble(calcCofins ? (lstProduto[i].AliqCofins > 0 ? lstProduto[i].AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));
                            nodeNomeImposto.AppendChild(nodePai);
                        }
                        else
                        {
                            switch (lstProduto[i].CstCofins.Value)
                            {
                                case 1:
                                case 2:
                                    nodePai = docXml.CreateElement("COFINSAliq");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].CstCofins.Value.ToString().PadLeft(2, '0'));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "vBC", Formatacoes.TrataValorDouble(calcCofins ? (double)lstProduto[i].BcCofins : 0, 2));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "pCOFINS", Formatacoes.TrataValorDouble(calcCofins ? (lstProduto[i].AliqCofins > 0 ? lstProduto[i].AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                                case 3:
                                    throw new Exception("COFINS CST 03 não implementado.");
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    nodePai = docXml.CreateElement("COFINSNT");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", lstProduto[i].CstCofins.Value.ToString().PadLeft(2, '0'));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                                default:
                                    nodePai = docXml.CreateElement("COFINSOutr");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "CST", "99");
                                    ManipulacaoXml.SetNode(docXml, nodePai, "vBC", Formatacoes.TrataValorDouble(calcCofins ? (double)lstProduto[i].BcCofins : 0, 2));
                                    ManipulacaoXml.SetNode(docXml, nodePai, "pCOFINS", Formatacoes.TrataValorDouble(calcCofins ? (lstProduto[i].AliqCofins > 0 ? lstProduto[i].AliqCofins : ConfigNFe.AliqCofins((uint)loja.IdLoja)) : 0, 2));
                                    nodeNomeImposto.AppendChild(nodePai);
                                    break;
                            }
                        }
                    }
                    else // Simples Nacional (Não destaca COFINS)
                    {
                        if (lstProduto[i].CstCofins.HasValue && lstProduto[i].CstCofins.Value == 49)
                        {
                            nodePai = docXml.CreateElement("COFINSSN");
                            ManipulacaoXml.SetNode(docXml, nodePai, "CST", "49"); // Operação sem incidência da contribuição
                            nodeNomeImposto.AppendChild(nodePai);
                        }
                    }

                    #endregion

                    #region COFINSST

                    #endregion
                }

                #endregion

                #region Total

                nodePai = docXml.CreateElement("total");
                docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                #endregion

                #region Informações do Pagamento

                nodePai = docXml.CreateElement("MP");
                docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                foreach (PagamentoCFe pag in lstPagamento)
                {
                    XmlElement nodeMP = docXml.CreateElement("MP");
                    nodePai.AppendChild(nodeMP);

                    node = docXml.CreateElement("cMP");
                    node.InnerText = ((int)pag.FormaPagamento).ToString().PadLeft(2, '0');
                    nodeMP.AppendChild(node);

                    node = docXml.CreateElement("vMP");
                    node.InnerText = Formatacoes.TrataValorDecimal((decimal)pag.ValorPagamento, 2);
                    nodeMP.AppendChild(node);

                    if (pag.FormaPagamento == PagamentoCFe.FormaPagamentoCFe.CartaoCredito || pag.FormaPagamento == PagamentoCFe.FormaPagamentoCFe.CartaoDebito)
                    {
                        node = docXml.CreateElement("cAdmC");
                        node.InnerText = ((int)pag.OperadoraCartao).ToString().PadLeft(3, '0');
                        nodeMP.AppendChild(node);
                    }
                }

                #endregion

                #region Informações Adicionais (Observacoes)

                if (!String.IsNullOrEmpty(Observacoes))
                {
                    nodePai = docXml.CreateElement("infAdic");
                    docXml.SelectSingleNode("/CFe/infCFe").AppendChild(nodePai);

                    node = docXml.CreateElement("infCpl");
                    node.InnerText = (Observacoes.Length <= 5000) ? Observacoes : Observacoes.Substring(0, 4999);
                    nodePai.AppendChild(nodePai);
                }

                #endregion

                //docXml.Save(Path.Combine(pathDocXML, "teste1.xml"));

                return docXml.OuterXml;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao montar o Cupom da Venda (xml). " + e.Message);
            }
        }

        /// <summary>
        /// Método que gera o XML a ser enviado para o Aparelho SAT para CANCELAR uma venda
        /// </summary>
        /// <param name="chaveCFeCancelar">String: Chave do último CFe emitido</param>
        /// <param name="numeroCaixa">String: Número do caixa onde o aparelho está conectado</param>
        /// <param name="dataEmissaoCFe">DateTime: Data/Hora de emissão do CFe</param>
        /// <param name="objCliente">Cliente: Objeto do tipo Cliente que realizou a compra</param>
        /// <param name="objLoja">Loja: Objeto do tipo Loja responsável pela emissão do CFe</param>
        /// <returns></returns>
        public static string MontarXmlCancelamento(string chaveCFeCancelar, string numeroCaixa, DateTime dataEmissaoCFe, Cliente objCliente, Loja objLoja)
        {
            try
            {
                #region Validações

                if ((String.IsNullOrEmpty(chaveCFeCancelar) || chaveCFeCancelar.Length != 47) && !chaveCFeCancelar.StartsWith("CFe"))
                    throw new Exception("Chave do CFe para cancelamento em formato inválido.");

                if (String.IsNullOrEmpty(numeroCaixa) || numeroCaixa.Length > 3)
                    throw new Exception("Número do caixa deve ser informado e conter no máximo 3 dígitos.");

                if (dataEmissaoCFe.AddMinutes(30) < DateTime.Now)
                    throw new Exception("Este CFe não pode ser cancelado pois foi emitido a mais de 30 minutos");

                #endregion

                #region Declarações de Objetos

                XmlDocument docXml = new XmlDocument();
                XmlNode nodePai;
                XmlNode node;
                XmlAttribute attr;

                #endregion

                #region XML Declaration

                node = docXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                docXml.AppendChild(node);

                #endregion

                #region Raiz do Documento XML

                // Nodo CFeCanc (Raiz do documento)
                node = docXml.CreateElement("CFeCanc");
                attr = docXml.CreateAttribute("xmlns");
                attr.Value = "http://www.fazenda.sp.gov.br/sat";
                node.Attributes.Append(attr);
                docXml.AppendChild(node);

                #endregion

                #region Nodo Base do Cupom

                // Nodo infCFe (Base para o cupom)
                node = docXml.CreateElement("infCFe");
                attr = docXml.CreateAttribute("chCanc");
                attr.Value = chaveCFeCancelar;
                node.Attributes.Append(attr);
                docXml.SelectSingleNode("/CFeCanc").AppendChild(node);

                #endregion

                #region Informações Basicas Cupom

                // Nodo ide (Informações Básicas do Cupom)
                nodePai = docXml.CreateElement("ide");
                docXml.SelectSingleNode("/CFeCanc/infCFe").AppendChild(nodePai);

                #endregion

                #region Filhos Nodo ide

                // CNPJ
                node = docXml.CreateElement("CNPJ");
                node.InnerText = _cnpjSoftwareHouse.PadLeft(14, '0');
                nodePai.AppendChild(node);

                // signAC
                node = docXml.CreateElement("signAC");
                node.InnerText = _cnpjSoftwareHouse + objLoja.Cnpj;
                nodePai.AppendChild(node);

                // numeroCaixa - número do caixa ao qual o SAT está conectado
                node = docXml.CreateElement("numeroCaixa");
                node.InnerText = numeroCaixa.PadLeft(3, '0');
                nodePai.AppendChild(node);

                #endregion

                #region Informações Estabelecimento Emissor

                // Nodo emit (Informações do Estabelecimento Emissor)
                nodePai = docXml.CreateElement("emit");
                docXml.SelectSingleNode("/CFeCanc/infCFe").AppendChild(nodePai);

                #endregion

                #region Informações Consumidor

                // Nodo dest (Informações do Consumidor)
                nodePai = docXml.CreateElement("dest");
                docXml.SelectSingleNode("/CFeCanc/infCFe").AppendChild(nodePai);

                #region Filhos Nodo dest

                // CPF ou CNPJ do Cliente
                if (objCliente != null && objCliente.TipoPessoa == "J")
                {
                    node = docXml.CreateElement("CNPJ");
                    node.InnerText = objCliente.CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "").PadLeft(14, '0');
                    nodePai.AppendChild(node);
                }
                else if (objCliente != null && objCliente.TipoPessoa == "F")
                {
                    node = docXml.CreateElement("CPF");
                    node.InnerText = objCliente.CpfCnpj.Replace(".", "").Replace("-", "").PadLeft(11, '0');
                    nodePai.AppendChild(node);
                }

                #endregion

                #endregion

                #region Total

                nodePai = docXml.CreateElement("total");
                docXml.SelectSingleNode("/CFeCanc/infCFe").AppendChild(nodePai);

                #endregion

                //docXml.Save(Path.Combine(pathDocXML, "teste1Canc.xml"));

                return docXml.OuterXml;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao montar o Cupom da Venda (xml). " + e.Message);
            }
        }    
    }
}
