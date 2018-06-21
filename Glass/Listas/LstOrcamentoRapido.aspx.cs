using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using Glass.Configuracoes;
using System.Linq;
using Glass.Global;

namespace Glass.UI.Web.Listas
{
    public partial class LstOrcamentoRapido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstOrcamentoRapido));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            hdfExibePopupEstoque.Value = PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower();
    
            if (!String.IsNullOrEmpty(Request["percComissao"]))
                hdfPercComissao.Value = Request["percComissao"];
    
            if (!String.IsNullOrEmpty(Request["idOrca"]))
                hdfIdOrca.Value = Request["idOrca"];
    
            if (IsPopup())
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "popup", "hideThisPopup();", true);
                if (Request["tipoEntrega"] != null)
                {
                    drpTipoEntrega.SelectedValue = Request["tipoEntrega"];
                    drpTipoEntrega.Enabled = false;
                }
    
                if (Request["revenda"] == "1")
                    chkRevenda.Checked = true;
    
                lblTitleTotalOrca.Text = "Total:";
    
                if (!String.IsNullOrEmpty(Request["idProdOrca"]))
                {
                    StringBuilder produtos = new StringBuilder();
                    StringBuilder beneficiamentos = new StringBuilder();
                    StringBuilder idProdOrca = new StringBuilder();
    
                    foreach (ProdutosOrcamento p in ProdutosOrcamentoDAO.Instance.GetByProdutoOrcamento(Glass.Conversoes.StrParaUint(Request["idProdOrca"])))
                    {
                        string formatProd = "new Array('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')";
                        string formatBenef = "{0};{1};{2};{3}{4}|";
    
                        string benef = "";
                        var produtosOrcamentoBenef = ProdutoOrcamentoBenefDAO.Instance.GetByProdutoOrcamento(p.IdProd);
                        foreach (var b in produtosOrcamentoBenef)
                        {
                            string arg4 = GenericBenefCollection.IsLapidacao(b.IdBenefConfig) ? String.Format("{0};{1}", b.LapAlt, b.LapLarg) :
                                GenericBenefCollection.IsBisote(b.IdBenefConfig) ? String.Format("{0};{1};{2}", b.BisAlt, b.BisLarg, b.EspBisote.ToString().Replace(',', '.')) : 
                                "";
    
                            benef += string.Format(formatBenef, b.IdBenefConfig, b.Qtde, 0, b.Valor.ToString().Replace(',', '.'), arg4);
                        }
    
                        string descricao = p.Descricao;
                        string servicos = "";
                        if (p.Descricao.IndexOf("(") > -1)
                        {
                            descricao = p.Descricao.Substring(0, p.Descricao.IndexOf("("));
                            servicos = p.Descricao.Substring(p.Descricao.IndexOf("(") + 1);
                            servicos = servicos.Substring(0, servicos.Length - 1);
                        }
                        /* Chamado 55626. */
                        else if (!string.IsNullOrWhiteSpace(benef) && produtosOrcamentoBenef != null && produtosOrcamentoBenef.Count() > 0)
                        {
                            var beneficiamentosDescricao = new List<string>();

                            foreach(var produtoOrcamentoBenef in produtosOrcamentoBenef)
                                beneficiamentosDescricao.Add(string.Format("{0}{1}", string.Format("{0} ", produtoOrcamentoBenef.Qtde > 0 ? produtoOrcamentoBenef.Qtde.ToString() : string.Empty),
                                    BenefConfigDAO.Instance.GetDescrBenef(produtoOrcamentoBenef.IdBenefConfig.ToString())));

                            servicos = string.Format("{0}", string.Join(", ", beneficiamentosDescricao));
                        }
    
                        if (!String.IsNullOrEmpty(p.Descricao))
                            p.Descricao = p.Descricao.Replace("'", "").Replace("\"", "");
    
                        if (!String.IsNullOrEmpty(p.CodInterno))
                            p.CodInterno = p.CodInterno.Replace("'", "").Replace("\"", "");
    
                        produtos.Append(", " + String.Format(formatProd, p.IdProd, p.CodInterno, p.ValorProd != null ? p.ValorProd.Value.ToString() : "0",
                            p.Total != null ? (p.Total.Value + p.ValorBenef).ToString() : "0", p.Altura, p.AlturaCalc, p.Largura, p.Qtde > 0 ? p.Qtde.Value : 1,
                            p.Redondo.ToString().ToLower(), p.TotM + (p.TotM != p.TotMCalc ? " (" + p.TotMCalc + ")" : ""), descricao, servicos, 
                            p.Custo.ToString(), p.ValorTabela.ToString(), p.Espessura, p.IdProcesso != null ? p.IdProcesso.Value.ToString() : "",
                            p.CodProcesso, p.IdAplicacao != null ? p.IdAplicacao.Value.ToString() : "", p.CodAplicacao));
    
                        beneficiamentos.Append(", '" + benef + "'");
                        idProdOrca.Append(", '" + p.IdProd + "'");
                    }
    
                    string script = "criarTabela(new Array({0}), new Array({1}), '{2}', new Array({3}));";
                    string prod = produtos.ToString();
                    string ben = beneficiamentos.ToString();
                    string prodOrca = idProdOrca.ToString();
    
                    if (prod.Length > 0) prod = prod.Substring(2);
                    if (ben.Length > 0) ben = ben.Substring(2);
                    if (prodOrca.Length > 0) prodOrca = prodOrca.Substring(2);
    
                    Page.ClientScript.RegisterStartupScript(GetType(), "tabelaProdutos", String.Format(script, prod,
                        ben, "idProdOrcamento", prodOrca), true);
                }
            }
    
            // Troca o posicionamento dos controle de largura e altura
            if (!IsPostBack && PedidoConfig.EmpresaTrabalhaAlturaLargura)
            {
                lblLarguraAltura.Text = "Altura x Largura";
                Control largParent = txtAltura.Parent;
                Control altParent = txtLargura.Parent;
                largParent.Controls.Add(txtLargura);
                altParent.Controls.Add(txtAltura);
    
                lblLargLst.Text = "ALTURA";
                lblAltLst.Text = "LARGURA";
            }
    
            if (!IsPostBack)
            {
                hdfIdCliente.Value = Request["idCliente"];
    
                if (Glass.Configuracoes.Geral.NaoVendeVidro())
                {
                    // Esconde campos da inserção do produto
                    lblLarguraAltura.Style.Add("display", "none");
                    lblTitleEspessura.Style.Add("display", "none");
                    lblMm.Style.Add("display", "none");
                    lblTitleTotM2.Style.Add("display", "none");
                    lblTotM2.Style.Add("display", "none");
                    txtLargura.Style.Add("display", "none");
                    txtAltura.Style.Add("display", "none");
                    txtEspessura.Style.Add("display", "none");
    
                    // Esconde campos da lista de produtos já adicionados
                    lblAltLst.Text = "";
                    lblLargLst.Text = "";
                    lblLstTotM2.Text = "";
                    lblLstServicos.Text = "";
    
                    hdfNaoVendeVidro.Value = "true";
                }
            }
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string idClienteStr, string tipoEntrega, string revenda, string percDescontoQtdeStr)
        {
            var percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
            var idCliente = Conversoes.StrParaUintNullable(idClienteStr);
    
            // Recupera o valor mínimo do produto
            int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
            return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCliente, revenda == "true", false, percDescontoQtde, null, null, null).ToString();
        }
    
        /// <summary>
        /// Retorna o Código/Descrição/Valor aplicável do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string idCli, string idOrca, string tipoEntrega, string revenda, string percDescontoQtdeStr, string orcamentoRapido)
        {
            try
            {
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, UserInfo.GetUserInfo.IdLoja, Glass.Conversoes.StrParaUintNullable(idCli), null, true);

                int? tipoOrcamento = String.IsNullOrEmpty(idOrca) ? null : OrcamentoDAO.Instance.ObtemTipoOrcamento(Glass.Conversoes.StrParaUint(idOrca));

                var idLoja = OrcamentoDAO.Instance.GetIdLoja(null, idOrca.StrParaUint());

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";
                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
                else if (prod.Compra)
                    return "Erro;Produto apenas para compra.";
                else if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && orcamentoRapido == "false" && (tipoOrcamento == (int)Glass.Data.Model.Orcamento.TipoOrcamentoEnum.Venda && (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || (prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))) && prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra))
                    return "Erro;Produtos de revenda não podem ser incluídos em um orçamento de venda.";
                else if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && orcamentoRapido == "false" && (tipoOrcamento == (int)Glass.Data.Model.Orcamento.TipoOrcamentoEnum.Revenda && ((prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd)) || prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)))
                    return "Erro;Produtos de venda não podem ser incluídos em um orçamento de revenda.";

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCliente = !String.IsNullOrEmpty(idCli) ? (uint?)Glass.Conversoes.StrParaUint(idCli) : null;
                float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                retorno += ";" + ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCliente, revenda.ToLower() == "true", false, percDescontoQtde, null, null, idOrca.StrParaIntNullable()).ToString("F2");

                // Verifica qual tipo de entrega será utilizado
                if (revenda.ToLower() == "true" || ClienteDAO.Instance.IsRevenda(null, idCliente)) // Se for cliente revenda, valor de atacado
                    retorno += ";1";
                else
                {
                    switch (tipoEntrega)
                    {
                        case "1":
                        case "4":
                            retorno += ";2";
                            break;

                        case "2":
                        case "3":
                        case "5":
                        case "6":
                            retorno += ";3";
                            break;

                        default:
                            retorno += ";1";
                            break;
                    }
                }

                // Espessura do vidro
                retorno += ";" + prod.Espessura;

                // Verifica se produto é vidro
                retorno += ";" + (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd)).ToString().ToLower();

                // Verifica se produto é alumínio
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + (Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd) || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6).ToString().ToLower();

                // M² mínima para o cálculo do valor do produto
                retorno += ";" + (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                // Verifica se o produto é vidro temperado
                retorno += ";" + (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidroTemperado(null, prod.IdGrupoProd, prod.IdSubgrupoProd)).ToString().ToLower();

                // Verifica o tipo de cálculo que será feito no produto
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);

                // Custo do produto
                retorno += ";" + prod.CustoCompra.ToString().Replace(',', '.');

                // Alíquota do ICMS
                retorno += ";" + prod.AliqICMSInterna.ToString().Replace(',', '.');

                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd);

                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(null, UserInfo.GetUserInfo.IdLoja, (uint)prod.IdProd).ToString() : "100000");
                retorno += ";" + ProdutoDAO.Instance.ExibirMensagemEstoque(prod.IdProd).ToString().ToLower();
                retorno += ";" + ProdutoLojaDAO.Instance.GetEstoque(null, UserInfo.GetUserInfo.IdLoja, (uint)prod.IdProd);

                // Altura/Largura padrão
                if (prod.Altura != null && prod.Largura != null)
                    retorno += ";" + prod.Altura.Value + ";" + prod.Largura.Value;
                else
                    retorno += ";;";

                retorno += ";" + prod.IdProcesso + ";" + (prod.IdProcesso > 0 ?
                    EtiquetaProcessoDAO.Instance.ObtemCodInterno((uint)prod.IdProcesso.Value) : "") + ";" +
                    prod.IdAplicacao + ";" + (prod.IdAplicacao > 0 ?
                    EtiquetaAplicacaoDAO.Instance.ObtemCodInterno((uint)prod.IdAplicacao.Value) : "");

                return retorno;
            }
            catch (Exception ex)
            {
                return "Erro;" + MensagemAlerta.FormatErrorMsg("Falha ao buscar produto.", ex);
            }
        }
    
        /// <summary>
        /// Gera um pedido a partir do orçamento rápido
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GerarPedido(string idCli, string tipoPedido, string tipoEntrega, string dataEntrega, string produtos, string espessura)
        {
            Glass.Data.Model.Pedido ped = new Glass.Data.Model.Pedido();
            ProdutosPedido prodPed;
            
            uint idPedido = 0;
    
            try
            {
                LoginUsuario login = UserInfo.GetUserInfo;
    
                ped.IdCli = Glass.Conversoes.StrParaUint(idCli);
    
                if (ClienteDAO.Instance.GetSituacao(ped.IdCli) != (int)SituacaoCliente.Ativo)
                    throw new Exception("O cliente não está ativo.");
    
                ped.IdFunc = login.CodUser;
                ped.IdLoja = FuncionarioDAO.Instance.ObtemIdLoja(ped.IdFunc);
                ped.TipoVenda = (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista;
                ped.TipoPedido = Glass.Conversoes.StrParaInt(tipoPedido);
                ped.TipoEntrega = Glass.Conversoes.StrParaInt(tipoEntrega);
                ped.FromOrcamentoRapido = true;
                ped.DataPedido = DateTime.Now;
                ped.DataEntregaString = dataEntrega;
                idPedido = PedidoDAO.Instance.Insert(ped);
    
                // Cria o ambiente, se a empresa usar
                uint? idAmbientePedido = null;
                if (PedidoConfig.DadosPedido.AmbientePedido)
                {
                    AmbientePedido ambiente = new AmbientePedido();
                    ambiente.IdPedido = idPedido;
                    ambiente.Ambiente = ((Glass.Data.Model.Pedido.TipoEntregaPedido)ped.TipoEntrega).ToString();
                    ambiente.Descricao = ambiente.Ambiente;
    
                    idAmbientePedido = AmbientePedidoDAO.Instance.Insert(ambiente);
                }
    
                produtos = produtos.Replace("\r\n", "\n");
                string[] vetProds = produtos.TrimEnd('\n').Split('\n');
    
                // Para cada produto do orçamento rápido
                foreach (string prod in vetProds)
                {
                    // [0]Id do produto [1]Valor produto (sem benef.) [2]Valor total [3]Qtd [4]Altura [5]AlturaCalc [6]Largura [7]Redondo [8]Area total
                    // [9]Descrição [10]Custo, [11]Valor tabela [12]Espessura [13]Perc. Desc. Qtde [14]ServicoInfo [15]Perc. Comissão [16]IdProcesso [17]IdAplicacao
                    string[] dadosProd = prod.Split('\t');
    
                    prodPed = new ProdutosPedido();
                    prodPed.IdPedido = idPedido;
                    prodPed.IdAmbientePedido = idAmbientePedido;
                    prodPed.IdProd = Glass.Conversoes.StrParaUint(dadosProd[0]);
                    prodPed.Qtde = float.Parse(dadosProd[3].Replace('.', ','));

                    prodPed.PercDescontoQtde = !String.IsNullOrEmpty(dadosProd[13]) ? float.Parse(dadosProd[13].Replace('.', ',')) : 0;
                    prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela((int)prodPed.IdProd, ped.TipoEntrega, ped.IdCli, false, false, prodPed.PercDescontoQtde, (int?)idPedido, null, null);
                    prodPed.ValorVendido = decimal.Parse(dadosProd[1].Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);

                    if (ProdutoDAO.Instance.IsPrecoTabela(prodPed.IdProd, prodPed.ValorVendido))
                        prodPed.ValorVendido = prodPed.ValorTabelaPedido;

                    prodPed.AlturaReal = !String.IsNullOrEmpty(dadosProd[4]) ? float.Parse(dadosProd[4]) : 0f;
                    prodPed.Altura = !String.IsNullOrEmpty(dadosProd[5]) ? float.Parse(dadosProd[5]) : 0f;
                    prodPed.Largura = !String.IsNullOrEmpty(dadosProd[6]) ? Glass.Conversoes.StrParaInt(dadosProd[6]) : 0;
                    prodPed.Redondo = dadosProd[7] == "true";
                    if (!String.IsNullOrEmpty(dadosProd[8]) && !dadosProd[8].Contains("("))
                        prodPed.TotM = Single.Parse(dadosProd[8].Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                    prodPed.Total = decimal.Parse(dadosProd[2].Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                    if (!String.IsNullOrEmpty(espessura))
                        prodPed.Espessura = Glass.Conversoes.StrParaInt(espessura);
                    prodPed.CustoProd = decimal.Parse(dadosProd[10].Replace('.', ','));
                    prodPed.ValorTabelaOrcamento = decimal.Parse(dadosProd[11].Replace('.', ','));
                    prodPed.TipoCalculoUsadoOrcamento = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)prodPed.IdProd);
                    prodPed.TipoCalculoUsadoPedido = prodPed.TipoCalculoUsadoOrcamento;
                    prodPed.Espessura = !String.IsNullOrEmpty(dadosProd[12]) ? Glass.Conversoes.StrParaFloat(dadosProd[12]) : 0;
                    prodPed.IdProcesso = Glass.Conversoes.StrParaUintNullable(dadosProd[16]);
                    prodPed.IdAplicacao = Glass.Conversoes.StrParaUintNullable(dadosProd[17]);
    
                    if (!String.IsNullOrEmpty(dadosProd[14]))
                    {
                        GenericBenefCollection benef = new GenericBenefCollection();
                        benef.AddBenefFromServicosInfo(dadosProd[14]);
                        prodPed.Beneficiamentos = benef;
                    }
    
                    prodPed.ValorComissao = prodPed.Total * (decimal)(Glass.Conversoes.StrParaFloat(dadosProd[15]) / 100);
                    uint idProdPed = ProdutosPedidoDAO.Instance.Insert(prodPed);
                }
    
                // Atualiza o total do pedido
                PedidoDAO.Instance.UpdateTotalPedido(idPedido);
    
                return "ok\tPedido Gerado com sucesso.\t" + idPedido;
            }
            catch (Exception ex)
            {
                PedidoDAO.Instance.DeleteByPrimaryKey(idPedido);
    
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar pedido.", ex);
            }
        }
    
        /// <summary>
        /// Gera um orçamento a partir do orçamento rápido
        /// </summary>
        /// <param name="nomeCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GerarOrcamento(string nomeCli, string tipoPedido, string tipoEntrega, string dataEntrega, string produtos, string espessura, string numParc)
        {
            var orca = new Data.Model.Orcamento();
            ProdutosOrcamento prodOrca;
    
            uint idOrca = 0;
    
            try
            {
                LoginUsuario login = UserInfo.GetUserInfo;
                //Cliente cliProj = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCli));
    
                //orca.IdCliente = Glass.Conversoes.StrParaUint(idCli);
                orca.NomeCliente = nomeCli;
                orca.IdFuncionario = login.CodUser;
                orca.TipoOrcamento = Glass.Conversoes.StrParaIntNullable(tipoPedido);
                orca.TipoEntrega = Glass.Conversoes.StrParaInt(tipoEntrega);
                orca.Validade = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;
                orca.PrazoEntrega = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;
                orca.FormaPagto = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;
                orca.Situacao = 1;
                orca.DataEntrega = Conversoes.ConverteData(dataEntrega);
                orca.NumeroParcelas = Glass.Conversoes.StrParaInt(numParc);
    
                /*
                orca.Bairro = cliProj.Bairro;
                orca.Cidade = cliProj.Cidade;
                orca.Endereco = cliProj.Endereco + (!String.IsNullOrEmpty(cliProj.Numero) ? ", " + cliProj.Numero : String.Empty);
                orca.TelCliente = cliProj.TelCont;
                orca.CelCliente = cliProj.TelCel;
                orca.Email = cliProj.Email;
                */
    
                idOrca = OrcamentoDAO.Instance.Insert(orca);
    
                orca.IdOrcamento = idOrca;
                orca.ImprimirProdutosOrcamento = true;
                OrcamentoDAO.Instance.Update(orca);
                
                uint? idAmbienteOrca = null;
                if (OrcamentoConfig.AmbienteOrcamento)
                {
                    AmbienteOrcamento rapido = new AmbienteOrcamento();
                    rapido.Ambiente = "Orçamento rápido";
                    rapido.Descricao = "Ambiente gerado pelo orçamento rápido.";
                    rapido.IdOrcamento = idOrca;
    
                    idAmbienteOrca = AmbienteOrcamentoDAO.Instance.Insert(rapido);
                }
    
                uint? idProdParent = null;
                prodOrca = new ProdutosOrcamento();
                prodOrca.Ambiente = "Orçamento rápido";
                prodOrca.Descricao = "Produto gerado pelo orçamento rápido";
                prodOrca.IdOrcamento = idOrca;
                prodOrca.IdAmbienteOrca = idAmbienteOrca;
                prodOrca.Qtde = 1;
    
                idProdParent = ProdutosOrcamentoDAO.Instance.Insert(prodOrca);
    
                produtos = produtos.Replace("\r\n", "\n");
                string[] vetProds = produtos.TrimEnd('\n').Split('\n');
    
                // Para cada produto do orçamento rápido
                foreach (string prod in vetProds)
                {
                    // [0]Id do produto [1]Valor produto (sem benef.) [2]Valor total [3]Qtd [4]Altura [5]AlturaCalc [6]Largura [7]Redondo [8]Area total
                    // [9]Descrição [10]Custo, [11]Valor tabela [12]Espessura [13]Perc. Desc. Qtde [14]ServicoInfo [15]Perc. Comissão [16]IdProcesso [17]IdAplicacao
                    string[] dadosProd = prod.Split('\t');
    
                    prodOrca = new ProdutosOrcamento();
                    prodOrca.IdOrcamento = idOrca;
                    prodOrca.IdAmbienteOrca = idAmbienteOrca;
                    prodOrca.IdProdParent = idProdParent;
                    prodOrca.IdProduto = Glass.Conversoes.StrParaUint(dadosProd[0]);
                    prodOrca.Descricao = dadosProd[9];
                    prodOrca.Qtde = float.Parse(dadosProd[3].Replace('.', ','));

                    prodOrca.PercDescontoQtde = !String.IsNullOrEmpty(dadosProd[13]) ? float.Parse(dadosProd[13].Replace('.', ',')) : 0;
                    prodOrca.ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)prodOrca.IdProduto.Value, orca.TipoEntrega, orca.IdCliente, false, false, prodOrca.PercDescontoQtde, null, null, (int?)idOrca);
                    prodOrca.ValorProd = decimal.Parse(dadosProd[1].Replace('.', ','));

                    if (ProdutoDAO.Instance.IsPrecoTabela(prodOrca.IdProduto.Value, prodOrca.ValorProd.Value))
                        prodOrca.ValorProd = prodOrca.ValorTabela;
                    
                    prodOrca.Altura = !String.IsNullOrEmpty(dadosProd[4]) ? Single.Parse(dadosProd[4].Replace('.', ','), System.Globalization.NumberStyles.Any) : 0;
                    prodOrca.AlturaCalc = !String.IsNullOrEmpty(dadosProd[5]) ? Single.Parse(dadosProd[5].Replace('.', ','), System.Globalization.NumberStyles.Any) : 0;
                    prodOrca.Largura = !String.IsNullOrEmpty(dadosProd[6]) ? Glass.Conversoes.StrParaInt(dadosProd[6]) : 0;
                    prodOrca.Redondo = dadosProd[7] == "true";
                    if (!String.IsNullOrEmpty(espessura))
                        prodOrca.Espessura = Glass.Conversoes.StrParaInt(espessura);
                    prodOrca.Custo = decimal.Parse(dadosProd[10].Replace('.', ','));
                    prodOrca.TipoCalculoUsado = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)prodOrca.IdProduto.Value);
                    prodOrca.Espessura = !String.IsNullOrEmpty(dadosProd[12]) ? Glass.Conversoes.StrParaFloat(dadosProd[12]) : 0;
                    
                    if (!String.IsNullOrEmpty(dadosProd[14]))
                    {
                        GenericBenefCollection benef = new GenericBenefCollection();
                        benef.AddBenefFromServicosInfo(dadosProd[14]);
                        prodOrca.Beneficiamentos = benef;
                    }
    
                    uint idCliente = OrcamentoDAO.Instance.ObtemIdCliente(prodOrca.IdOrcamento).GetValueOrDefault();
                    decimal custo = 0, total = 0;
                    float altura = prodOrca.AlturaCalc, totM2 = 0, totM2Calc = 0;
                    Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente, (int)prodOrca.IdProduto.Value, prodOrca.Largura, prodOrca.Qtde.Value, 1, prodOrca.ValorProd.Value, prodOrca.Espessura, prodOrca.Redondo, 1, false,
                        true, ref custo, ref altura, ref totM2, ref totM2Calc, ref total, false, prodOrca.Beneficiamentos.CountAreaMinima);
                    prodOrca.Total = total;
                    prodOrca.TotM = totM2;
                    prodOrca.TotMCalc = totM2Calc;
                    prodOrca.ValorComissao = total * (decimal)(Glass.Conversoes.StrParaFloat(dadosProd[15]) / 100);
                    prodOrca.IdProcesso = Glass.Conversoes.StrParaUintNullable(dadosProd[16]);
                    prodOrca.IdAplicacao = Glass.Conversoes.StrParaUintNullable(dadosProd[17]);
    
                    ProdutosOrcamentoDAO.Instance.Insert(prodOrca);
                }
    
                // Atualiza o total do orçamento
                OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrca);
    
                return "ok\tOrçamento Gerado com sucesso.\t" + idOrca;
            }
            catch (Exception ex)
            {
                OrcamentoDAO.Instance.DeleteByPrimaryKey(idOrca);
    
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar orçamento.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string IncluirProdutoOrcamento(string idProdString, string produto)
        {
            uint idProd = Glass.Conversoes.StrParaUint(idProdString);
    
            ProdutosOrcamento prodOrca;
            ProdutosOrcamento prodParent = ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(idProd);
            var orca = OrcamentoDAO.Instance.GetElementByPrimaryKey(prodParent.IdOrcamento);

            uint idProdNovo = 0;
    
            try
            {
                // [0]Id do produto [1]Valor produto (sem benef.) [2]Valor total [3]Qtd [4]Altura [5]AlturaCalc [6]Largura [7]Redondo [8]Area total
                // [9]Descrição [10]Custo, [11]Valor tabela [12]Espessura [13]Perc. Desc. Qtde [14]ServicoInfo [15]Perc. Comissão [16]IdProcesso [17]IdAplicacao
                string[] dadosProd = produto.TrimEnd('\n').Split('\t');
    
                prodOrca = new ProdutosOrcamento();
                prodOrca.IdOrcamento = prodParent.IdOrcamento;
                prodOrca.IdProdParent = idProd;
                prodOrca.IdProduto = Glass.Conversoes.StrParaUint(dadosProd[0]);
                prodOrca.NumSeq = prodParent.NumSeq;
                prodOrca.Ambiente = prodParent.Ambiente;
                prodOrca.IdAmbienteOrca = prodParent.IdAmbienteOrca;
                prodOrca.Descricao = !String.IsNullOrEmpty(dadosProd[9]) ? (dadosProd[9].Length > 500 ? dadosProd[9].Substring(0, 500) : dadosProd[9]) : String.Empty;
                prodOrca.Total = decimal.Parse(dadosProd[2].Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                prodOrca.Qtde = float.Parse(dadosProd[3].Replace('.', ','));
                
                prodOrca.PercDescontoQtde = !String.IsNullOrEmpty(dadosProd[13]) ? float.Parse(dadosProd[13].Replace('.', ',')) : 0;
                prodOrca.ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)prodOrca.IdProduto.Value, orca.TipoEntrega, orca.IdCliente, false, false, prodOrca.PercDescontoQtde, null, null, (int?)prodOrca.IdOrcamento);
                prodOrca.ValorProd = decimal.Parse(dadosProd[1].Replace('.', ','));

                if (ProdutoDAO.Instance.IsPrecoTabela(prodOrca.IdProduto.Value, prodOrca.ValorProd.Value))
                    prodOrca.ValorProd = prodOrca.ValorTabela;

                prodOrca.Altura = !String.IsNullOrEmpty(dadosProd[4]) ? Single.Parse(dadosProd[4], System.Globalization.NumberStyles.Any) : 0;
                prodOrca.AlturaCalc = !String.IsNullOrEmpty(dadosProd[5]) ? Single.Parse(dadosProd[5], System.Globalization.NumberStyles.Any) : 0;
                prodOrca.Largura = !String.IsNullOrEmpty(dadosProd[6]) ? Glass.Conversoes.StrParaInt(dadosProd[6]) : 0;
                prodOrca.Redondo = dadosProd[7] == "true";
                prodOrca.TipoCalculoUsado = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)prodOrca.IdProduto.Value);
                prodOrca.Custo = !String.IsNullOrEmpty(dadosProd[10]) ? decimal.Parse(dadosProd[10].Replace('.', ',')) : 0;
                prodOrca.Espessura = !String.IsNullOrEmpty(dadosProd[12]) ? Glass.Conversoes.StrParaFloat(dadosProd[12]) : 0;
                
                if (!String.IsNullOrEmpty(dadosProd[14]))
                {
                    GenericBenefCollection benef = new GenericBenefCollection();
                    benef.AddBenefFromServicosInfo(dadosProd[14]);
                    prodOrca.Beneficiamentos = benef;
                }
    
                uint idCliente = OrcamentoDAO.Instance.ObtemIdCliente(prodOrca.IdOrcamento).GetValueOrDefault();
                decimal custo = 0, total = 0;
                float altura = prodOrca.AlturaCalc, totM2 = 0, totM2Calc = 0;
                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente, (int)prodOrca.IdProduto.Value, prodOrca.Largura, prodOrca.Qtde.Value, 1, prodOrca.ValorProd.Value,
                    prodOrca.Espessura, prodOrca.Redondo, 0, false, true, ref custo, ref altura, ref totM2, ref totM2Calc, ref total,
                    false, prodOrca.Beneficiamentos.CountAreaMinima);
    
                prodOrca.AlturaCalc = altura;
                prodOrca.TotM = totM2;
                prodOrca.TotMCalc = totM2Calc;
                prodOrca.Total = total;
                prodOrca.ValorComissao = total * (decimal)(Glass.Conversoes.StrParaFloat(dadosProd[15]) / 100);
                prodOrca.IdProcesso = Glass.Conversoes.StrParaUintNullable(dadosProd[16]);
                prodOrca.IdAplicacao = Glass.Conversoes.StrParaUintNullable(dadosProd[17]);
    
                idProdNovo = ProdutosOrcamentoDAO.Instance.Insert(prodOrca);
    
                return "ok\tProduto incluído com sucesso no orçamento.\t" + idProdNovo;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao incluir produto no orçamento.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string IncluirProdutosOrcamento(string idProdString, string produtos)
        {
            string idProdutos = "";
    
            try
            {
                string[] vetProds = produtos.TrimEnd('\n').Split('\n');
    
                // Para cada produto do orçamento rápido
                foreach (string prod in vetProds)
                {
                    string[] resultado = IncluirProdutoOrcamento(idProdString, prod).Split('\t');
                    if (resultado[0] == "Erro")
                        throw new Exception(resultado[1].Substring(resultado[1].IndexOf(" Erro: ") + 7));
                    else
                        idProdutos += ", " + resultado[2];
                }
    
                return "ok\tProdutos incluídos com sucesso no orçamento.\t" + idProdutos.Substring(2);
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao incluir produto no orçamento.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string ExcluirProdutoOrcamento(string idProdString)
        {
            uint idProd = Glass.Conversoes.StrParaUint(idProdString);
    
            try
            {
                ProdutoOrcamentoBenefDAO.Instance.DeleteByProdOrca(idProd);
                ProdutosOrcamentoDAO.Instance.DeleteByPrimaryKey(idProd);
                return "ok\tProduto excluído com sucesso do orçamento.\t" + idProd;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao excluir produto do orçamento.", ex);
            }
        }
    
        #endregion
    
        protected bool IsPopup()
        {
            return (Page.Master as Painel).IsPopup();
        }
    
        #region Beneficiamentos
    
        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
    
            benef.CampoAltura = hdfAlturaCalc;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoQuantidade = txtQtde;
            benef.CampoTotalM2 = Beneficiamentos.UsarM2CalcBeneficiamentos ? lblTotM2Calc : lblTotM2;
            benef.CampoValorUnitario = txtValor;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoProdutoID = txtCodProd;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoTipoEntrega = drpTipoEntrega;
            benef.CampoRevenda = chkRevenda;
            benef.CampoPercComissao = hdfPercComissao;
        }
    
        #endregion
    
        protected void btnIncluir_Load(object sender, EventArgs e)
        {
            string percComissao = !String.IsNullOrEmpty(Request["percComissao"]) ? Request["percComissao"] : "0";
            btnIncluir.OnClientClick = "if (!validate()) return false; if (!verificarObrigatoriedadeBeneficiamentos(dadosProduto.ID)) return false; return incluirItem(" + percComissao.Replace(',', '.') + ");";
        }
    
        protected void txtValor_Load(object sender, EventArgs e)
        {
            txtValor.Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }
    
        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)sender;
            desc.CampoQtde = txtQtde;
            desc.CampoProdutoID = hdfIdProd;
            desc.CampoTipoEntrega = drpTipoEntrega;
            desc.CampoClienteID = hdfIdCliente;
            desc.CampoRevenda = chkRevenda;
            desc.CampoValorUnit = txtValor;
            desc.ForcarEsconderControle = false;
        } 
    
        protected void drpTipoPedido_DataBound(object sender, EventArgs e)
        {
            try
            {
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra).ToString()).Enabled = false;
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao).ToString()).Enabled = false;
    
                List<ListItem> colecao = new List<ListItem>();
    
                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                {
                    string tipoPedido = FuncionarioDAO.Instance.ObtemTipoPedido(UserInfo.GetUserInfo.CodUser);
                    string[] values = null;
    
                    if (!String.IsNullOrEmpty(tipoPedido))
                        values = tipoPedido.Split(',');
    
                    if (values != null)
                        foreach (string v in values)
                        {
                            if (((DropDownList)sender).Items.FindByValue(v) != null)
                                colecao.Add(((DropDownList)sender).Items.FindByValue(v));
                        }
    
                    ((DropDownList)sender).Items.Clear();
                    ((DropDownList)sender).Items.AddRange(colecao.ToArray());
                }
                else
                {
                    ((DropDownList)sender).Items.RemoveAt(0);
                }
            }
            catch { }
        }
    
        protected bool UtilizarRoteiroProducao()
        {
            return Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }
    }
}
