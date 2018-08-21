using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadImportarNFEEntrada : System.Web.UI.Page
    {
        private uint _idFornecedor = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                plcResultados.Controls.Clear();
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadImportarNFEEntrada));
    
            #region Omiss�o dos controles quando n�o h� arquivo xml carregado
    
            plcInformacao.Controls.Clear();
            plcProdutos.Controls.Clear();
            plcValores.Controls.Clear();
            plcParcelas.Controls.Clear();
            ctrlParcelas1.Visible = false;
            txtNumParc.Visible = false;
            lblNumParc.Visible = false;

            lblInfoNFE.Text = "";
    
            AlteraVisibilidade(false);

            #endregion
        }
    
        #region Envia XML da nota

        private void ValidaXml(Stream conteudoArquivo)
        {
            string arquivo;

            using (StreamReader f = new StreamReader(conteudoArquivo))
            {
                arquivo = f.ReadToEnd();
            }

            if (arquivo.IndexOf("<xEvento>Carta de Corre", StringComparison.Ordinal) > 0)
                throw new Exception(
                    "Este XML � referente a uma carta de corre��o, � poss�vel importar somente XML de notas fiscais.");
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fupNFEUpload.HasFile)
            {
                plcInformacao.Controls.Clear();
                plcProdutos.Controls.Clear();
                plcValores.Controls.Clear();
                plcParcelas.Controls.Clear();

                XmlDocument nfeFile = new XmlDocument();

                try
                {
                    nfeFile.Load(fupNFEUpload.FileContent);
                }
                catch (Exception ex)
                {
                    /* Chamado 24676. */
                    ValidaXml(fupNFEUpload.FileContent);
                    throw ex;
                }

                if (VerificarNFE(nfeFile, fupNFEUpload.FileName))
                {
                    MostrarNFE(nfeFile);
    
                    int numProdutos = nfeFile.GetElementsByTagName("det").Count;
                    hdfNumProdutos.Value = numProdutos.ToString();
                    btnImportar.OnClientClick = "return validarAssociacoes(" + numProdutos + ");";
    
                    AlteraVisibilidade(true);
    
                    //Converte a XML carregada em um StringWriter e salva no ViewState, 
                    StringWriter sw = new StringWriter();
                    XmlTextWriter xw = new XmlTextWriter(sw);
                    nfeFile.WriteTo(xw);
                    Session["xmlnfe"] = sw.ToString();
                }
            }
        }
    
        #endregion
    
        #region Valida o arquivo XML
    
        /// <summary>
        /// Verifica se o arquivo xml inserido � uma NFe v�lida
        /// </summary>
        protected bool VerificarNFE(XmlDocument nfeFile, string fileName)
        {
            try
            {
                var dadosVerificar = WebGlass.Business.NotaFiscal.Fluxo.Importar.Instance.VerificarNFe(nfeFile, fileName);
    
                lblInfoNFE.Text = dadosVerificar.InfoNFe;
                hdfChaveAcesso.Value = dadosVerificar.ChaveAcesso;
    
                return true;
            }
            catch (Exception ex)
            {
                lblInfoNFE.Text = "Erro de verifica��o:<br/>" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
                return false;
            }
        }
    
        #endregion
    
        #region L� e exibe dados do XML enviado
    
        /// <summary>
        /// Constr�i tabelas asp contendo as informa��es da NFe e as insere nos placeholders da p�gina.
        /// </summary>
        protected void MostrarNFE(XmlDocument nfeFile)
        {
            XmlElement nfeRoot;

            var versao = nfeFile["nfeProc"] != null ?
                nfeFile["nfeProc"]["NFe"]["infNFe"].GetAttribute("versao") :
                nfeFile["NFe"] != null ?
                nfeFile["NFe"]["infNFe"].GetAttribute("versao") :
                /* Chamado 18344. */
                nfeFile["enviNFe"] != null ?
                nfeFile["enviNFe"]["NFe"]["infNFe"].GetAttribute("versao") : String.Empty;

            if (String.IsNullOrEmpty(versao))
                throw new Exception("N�o foi poss�vel recuperar a vers�o do XML.");
    
            try
            {
                if (nfeFile["nfeProc"] != null)
                    nfeRoot = nfeFile["nfeProc"]["NFe"];             
                else if (nfeFile["NFe"] != null)
                    nfeRoot = nfeFile["NFe"];
                else
                    nfeRoot = nfeFile["enviNFe"]["NFe"];
            }
            catch
            {
                try
                {
                    nfeRoot = nfeFile["NFe"];
                }
                catch (Exception ex2)
                {
                    lblInfoNFE.Text = "Erro ao exibir a nota fiscal.<br />" + ex2.Message;
                    return;
                }
            }
    
            XmlElement nfeIde = nfeRoot["infNFe"];
    
            #region Informa��es NFe, Emitente e Destinatario
    
            Table tblHeader = new Table();
            tblHeader.Width = Unit.Parse("560px");
    
            plcInformacao.Controls.Add(tblHeader);
    
            #region TAG 'ide'
    
            nfeIde = nfeRoot["infNFe"]["ide"];
    
            TableRow tblHeaderRow01 = new TableRow();
            tblHeaderRow01.BackColor = Color.LightGray;
            TableCell tblHeaderR01C1 = new TableCell();
            Label lblHeaderIDETitle = new Label();
            lblHeaderIDETitle.Text = "<b>Identifica��o da NF-e</b>";
    
            TableRow tblHeaderRow1 = new TableRow();
            TableCell tblHeaderR1C1 = new TableCell();
    
            Label lblHeaderIDE = new Label();
            lblHeaderIDE.Text = "<b>C�digo UF:</b> " + nfeIde["cUF"].InnerText;
            lblHeaderIDE.Text += "<br /><b>C�digo da Nota Fiscal:</b> " + nfeIde["cNF"].InnerText;
            lblHeaderIDE.Text += "<br /><b>Natureza da Opera��o:</b> " + nfeIde["natOp"].InnerText;
            lblHeaderIDE.Text += "<br /><b>Data da Emiss�o:</b> " + nfeIde["dhEmi"].InnerText;
            lblHeaderIDE.Text += "<br />&nbsp;";
    
            tblHeaderR01C1.Controls.Add(lblHeaderIDETitle);
            tblHeaderRow01.Cells.Add(tblHeaderR01C1);
    
            tblHeaderR1C1.Controls.Add(lblHeaderIDE);
            tblHeaderRow1.Cells.Add(tblHeaderR1C1);
    
            tblHeader.Rows.Add(tblHeaderRow01);
            tblHeader.Rows.Add(tblHeaderRow1);
            #endregion
    
            #region TAG 'emit'
    
            TableRow tblHeaderRow02 = new TableRow();
            tblHeaderRow02.BackColor = Color.LightGray;
            TableCell tblHeaderR02C1 = new TableCell();
            Label lblHeaderEmitTitle = new Label();
            lblHeaderEmitTitle.Text = "<b>Identifica��o do Emitente</b>";
    
            TableRow tblHeaderRow2 = new TableRow();
            TableCell tblHeaderR2C1 = new TableCell();
    
            nfeIde = nfeRoot["infNFe"]["emit"];
            
            Label lblHeaderEmit = new Label();
            lblHeaderEmit.Text = "<b>Nome:</b> " + nfeIde["xNome"].InnerText;
    
            string idFornecedor = null;
            if (nfeIde.GetElementsByTagName("CNPJ").Count == 1)
            {
                idFornecedor = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(nfeIde["CNPJ"].InnerText);
                lblHeaderEmit.Text += "<br /><b>CNPJ:</b> " + nfeIde["CNPJ"].InnerText;
            }
            else if (nfeIde.GetElementsByTagName("CPF").Count == 1)
            {
                idFornecedor = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(nfeIde["CPF"].InnerText);
                lblHeaderEmit.Text += "<br /><b>CPF:</b> " + nfeIde["CPF"].InnerText;
            }
    
            if (idFornecedor != null)
                idFornecedor = idFornecedor.Split(',')[0];
    
            HiddenField hdfIdFornec = new HiddenField();
            hdfIdFornec.ID = "hdfIdFornec";
            hdfIdFornec.Value = idFornecedor.ToString();
            _idFornecedor = idFornecedor.StrParaUint();
    
            lblHeaderEmit.Text += "<br /><b>Endere�o:</b> " + nfeIde["enderEmit"]["xLgr"].InnerText + ", "
                + nfeIde["enderEmit"]["nro"].InnerText + " - " + nfeIde["enderEmit"]["xBairro"].InnerText + ", "
                + nfeIde["enderEmit"]["xMun"].InnerText + " - " + nfeIde["enderEmit"]["UF"].InnerText
                + "<br />&nbsp;&nbsp;&nbsp;&nbsp;CEP: " + nfeIde["enderEmit"]["CEP"].InnerText;
    
            if (nfeIde["enderEmit"]["fone"] != null)
                lblHeaderEmit.Text += "<br /><b>Fone:</b> " + nfeIde["enderEmit"]["fone"].InnerText;
    
            lblHeaderEmit.Text += "<br />&nbsp;";
    
            tblHeaderR02C1.Controls.Add(lblHeaderEmitTitle);
            tblHeaderRow02.Cells.Add(tblHeaderR02C1);
    
            tblHeaderR2C1.Controls.Add(lblHeaderEmit);
            tblHeaderR2C1.Controls.Add(hdfIdFornec);
            tblHeaderRow2.Cells.Add(tblHeaderR2C1);
    
            tblHeader.Rows.Add(tblHeaderRow02);
            tblHeader.Rows.Add(tblHeaderRow2);
    
            #endregion
    
            #region TAG 'dest'
    
            TableRow tblHeaderRow03 = new TableRow();
            tblHeaderRow03.BackColor = Color.LightGray;
            TableCell tblHeaderR03C1 = new TableCell();
            Label lblHeaderDestTitle = new Label();
            lblHeaderDestTitle.Text = "<b>Identifica��o do Destinat�rio</b>";
    
            TableRow tblHeaderRow3 = new TableRow();
            TableCell tblHeaderR3C1 = new TableCell();
    
            nfeIde = nfeRoot["infNFe"]["dest"];
    
            Label lblHeaderDest = new Label();
            lblHeaderDest.Text = "<b>Nome:</b> " + nfeIde["xNome"].InnerText;
            lblHeaderDest.Text += "<br /><b>CNPJ:</b> " + nfeIde["CNPJ"].InnerText;
    
            lblHeaderDest.Text += "<br /><b>Endere�o:</b> " + nfeIde["enderDest"]["xLgr"].InnerText + ", "
                + nfeIde["enderDest"]["nro"].InnerText + " - " + nfeIde["enderDest"]["xBairro"].InnerText + ", "
                + nfeIde["enderDest"]["xMun"].InnerText + " - " + nfeIde["enderDest"]["UF"].InnerText;
    
            try { lblHeaderDest.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;CEP: " + nfeIde["enderDest"]["CEP"].InnerText;}
            catch (NullReferenceException) { }
    
            try{ lblHeaderDest.Text += "<br /><b>Fone:</b> " + nfeIde["enderDest"]["fone"].InnerText; }
            catch (NullReferenceException){ }
    
            lblHeaderDest.Text += "<br />&nbsp;";
    
            tblHeaderR03C1.Controls.Add(lblHeaderDestTitle);
            tblHeaderRow03.Cells.Add(tblHeaderR03C1);
    
            tblHeaderR3C1.Controls.Add(lblHeaderDest);
            tblHeaderRow3.Cells.Add(tblHeaderR3C1);
    
            tblHeader.Rows.Add(tblHeaderRow03);
            tblHeader.Rows.Add(tblHeaderRow3);
            #endregion
    
            #endregion
    
            #region Produtos
    
            Table tblProdutos = new Table();
            tblProdutos.ID = "tblProdutos";
            tblProdutos.Width = Unit.Parse("560px");
    
            plcProdutos.Controls.Add(tblProdutos);
    
            TableRow tblProdutosHeaderRow = new TableRow();
            tblProdutosHeaderRow.BackColor = Color.LightGray;
            TableCell tblProdutosHeaderRowCell = new TableCell();
            Label lblProdutosHeader = new Label();
            lblProdutosHeader.Text = "<b>Produtos</b>";
            tblProdutosHeaderRowCell.Controls.Add(lblProdutosHeader);
            tblProdutosHeaderRow.Cells.Add(tblProdutosHeaderRowCell);
            tblProdutos.Rows.Add(tblProdutosHeaderRow);
    
            #region TAG 'det' - Produto
    
            XmlNodeList xlist = nfeFile.GetElementsByTagName("det");
            for (int count = 0; count < nfeFile.GetElementsByTagName("det").Count; count++)
            {
                XmlElement xel = (XmlElement)xlist[count];
                
                Label lblUpperLine = new Label();
                lblUpperLine.Text = "<b>Produto " + (count + 1).ToString() + " - C�digo: </b>" + xel["prod"]["cProd"].InnerText;
    
                TableCell tblUpperLineCell = new TableCell();
                tblUpperLineCell.Controls.Add(lblUpperLine);
                TableRow tblUpperLineRow = new TableRow();
                tblUpperLineRow.BackColor = Color.DodgerBlue;
                tblUpperLineRow.Cells.Add(tblUpperLineCell);
                tblProdutos.Rows.Add(tblUpperLineRow);
                
                TableRow tblProdutosRow = new TableRow();
                TableCell tblProdutosRowCell = new TableCell();
    
                Label lblProdNome = new Label();
                lblProdNome.Text = "<b>Item:</b> " + xel["prod"]["xProd"].InnerText + "&nbsp;&nbsp;";
    
                TextBox tbxProdCodAssoc = new TextBox();
                tbxProdCodAssoc.ID = ("tbxProdCodAssoc" + count.ToString());
                tbxProdCodAssoc.Width = Unit.Parse("50px");
                tbxProdCodAssoc.Attributes.Add("OnBlur", "setProductFocus(" + count + "); associarProduto('" + xel["prod"]["cProd"].InnerText + "'); return false;");
    
                //Adiciona o bot�o de pesquisar na frente do produto
                ImageButton ibtPesquisar = new ImageButton();
                ibtPesquisar.ImageUrl = "~/Images/Pesquisar.gif";
                ibtPesquisar.OnClientClick = "setProductFocus(" + count + "); openWindow(500, 700, '../Utils/SelProd.aspx'); return false;";
    
                Label lblProduto = new Label();
                lblProduto.Text += "<br /><b>Valor:</b> " + xel["prod"]["vProd"].InnerText;
    
                if (xel["prod"]["vDesc"] != null)
                    lblProduto.Text += "<br /><b>Desconto:</b> " + xel["prod"]["vDesc"].InnerText;
    
                lblProduto.Text += "<br />&nbsp;";
    
                Label lblProdAssociado = new Label();
                lblProdAssociado.ID = ("lblProdAssoc" + count.ToString());
    
                Label lblCodProdAssociado = new Label();
                lblCodProdAssociado.ID = ("lblCodProdAssoc" + count.ToString());
                lblCodProdAssociado.Text = "";
    
                HiddenField hdfCodFornecProd = new HiddenField();
                hdfCodFornecProd.ID = "hdfCodFornecProd" + count;
                hdfCodFornecProd.Value = xel["prod"]["cProd"].InnerText;
    
                HiddenField hdfCodProd = new HiddenField();
                hdfCodProd.ID = "hdfCodProd" + count;
    
                // Verifica se o item da nota fiscal j� possui algum produto associado no banco
                uint idProdAssociado = ProdutoFornecedorDAO.Instance.GetIdProdByIdFornecCodFornec(Glass.Conversoes.StrParaUint(idFornecedor), xel["prod"]["cProd"].InnerText);
    
                if (idProdAssociado == 0 || !ProdutoDAO.Instance.Exists(idProdAssociado))
                {
                    lblProdAssociado.Text = "<br/>Associe este produto a um item do banco de dados.";
                    lblProdAssociado.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    Produto prod = ProdutoDAO.Instance.GetByIdProd(idProdAssociado);
    
                    lblProdAssociado.Text = "<br/>Associado ao item:&nbsp;";
                    lblProdAssociado.ForeColor = System.Drawing.Color.Blue;
    
                    lblCodProdAssociado.Text = prod.CodInterno + " - " + prod.Descricao;
                    hdfCodProd.Value = prod.CodInterno;
                }
    
                HiddenField hdfNatOpProd = new HiddenField();
                hdfNatOpProd.ID = "hdfNatOpProd" + count;
    
                int natOpCount = count;
    
                //Controle com Natureza da Opera��o
                Glass.UI.Web.Controls.ctrlNaturezaOperacao ctrlNatOp = LoadControl("~/Controls/ctrlNaturezaOperacao.ascx") as Glass.UI.Web.Controls.ctrlNaturezaOperacao;
                ctrlNatOp.ID = ("ctrlNatOp" + natOpCount.ToString());
                ctrlNatOp.PermitirVazio = false;
                ctrlNatOp.ErrorMessage = "A Natureza de Opera��o para o produto " + xel["prod"]["xProd"].InnerText + " n�o foi informada.";
    
    
                // Este atributo deve ser repassado at� chegar no campo txtDescr do controle SelPopup que se encontra referenciado no controle ctrlNaturezaOperacao.
                // Andr�: Foi modificado colocando o "this.onblur();" na frente do comando para for�ar a pesquisa da natureza de opera��o antes de set�-la
                // na Session via Ajax, o motivo disso � que em alguns casos o onblur n�o estava sendo chamado, apenas o onchange, mantendo a natureza
                // de opera��o padr�o, ao inv�s de usar a que o usu�rio estava escolhendo.
                ctrlNatOp.Attributes.Add("onchange",
                    "this.onblur(); setProductFocus(" + natOpCount + "); setProdNatOp(this);");
    
                string defaultCFOP = string.Empty;
    
                switch (xel["prod"]["CFOP"].InnerText.Substring(0,1))
                {
                    case "5":
                        defaultCFOP = "1" + xel["prod"]["CFOP"].InnerText.Substring(1, 3);
                        break;
    
                    case "6":
                        defaultCFOP = "2" + xel["prod"]["CFOP"].InnerText.Substring(1, 3);
                        break;
    
                    case "7":
                        defaultCFOP = "3" + xel["prod"]["CFOP"].InnerText.Substring(1, 3);
                        break;
                }
    
                var idCfopProd = Glass.Conversoes.StrParaUint(CfopDAO.Instance.GetCFOPByCodInterno(defaultCFOP));
    
                if (idCfopProd > 0)
				{
					string selectedNatOp = NaturezaOperacaoDAO.Instance.ObtemListaCfop(idCfopProd).Select(x => x.IdNaturezaOperacao.ToString()).FirstOrDefault();
		
					if (!string.IsNullOrEmpty(selectedNatOp))
					{
						ctrlNatOp.CodigoNaturezaOperacao = Glass.Conversoes.StrParaUintNullable(selectedNatOp);
						hdfNatOpProd.Value = selectedNatOp;
						Session["prodNatOp" + natOpCount.ToString()] = selectedNatOp;
					}
				}
    
                Label lblProdCfop = new Label();
                lblProdCfop.Text = "<br /><b>Natureza de Opera��o: </b> ";
				
				Label lblCfopOriginal = new Label();
				lblCfopOriginal.Text = " Original: " + xel["prod"]["CFOP"].InnerText;
    
                Label lblRodape = new Label();
                lblRodape.Text = "<br />&nbsp;";
    
                tblProdutosRowCell.Controls.Add(lblProdNome);
                tblProdutosRowCell.Controls.Add(tbxProdCodAssoc);
                tblProdutosRowCell.Controls.Add(ibtPesquisar);
                tblProdutosRowCell.Controls.Add(lblProdAssociado);
                tblProdutosRowCell.Controls.Add(lblCodProdAssociado);
                tblProdutosRowCell.Controls.Add(hdfCodProd);
                tblProdutosRowCell.Controls.Add(lblProduto);
                tblProdutosRowCell.Controls.Add(lblProdCfop);
                tblProdutosRowCell.Controls.Add(ctrlNatOp);
                tblProdutosRowCell.Controls.Add(hdfNatOpProd);
				tblProdutosRowCell.Controls.Add(lblCfopOriginal);
                tblProdutosRowCell.Controls.Add(hdfCodFornecProd);
                tblProdutosRowCell.Controls.Add(lblRodape);
                tblProdutosRow.Cells.Add(tblProdutosRowCell);
                tblProdutos.Rows.Add(tblProdutosRow);
            }
    
            #endregion
    
            #endregion
    
            #region Total, Transporte, Informa��es Adicionais e Parcelas
    
            Table tblValores = new Table();
            tblValores.Width = Unit.Parse("560px");
            //tblValores.BorderStyle = BorderStyle.Outset;
            //tblValores.BorderWidth = Unit.Parse("1px");
            //tblValores.BorderColor = System.Drawing.Color.Black;
            //tblValores.GridLines = GridLines.Both;
    
            plcValores.Controls.Add(tblValores);
    
            #region TAG 'total'
    
            TableRow tblValoresRow01 = new TableRow();
            tblValoresRow01.BackColor = Color.LightGray;
            TableCell tblValoresR01C1 = new TableCell();
            Label lblValoresTotalTitle = new Label();
            lblValoresTotalTitle.Text = "<b>Total</b>";
    
            TableRow tblValoresRow1 = new TableRow();
            TableCell tblValoresR1C1 = new TableCell();
    
            nfeIde = nfeRoot["infNFe"]["total"];
    
            Label lblValoresTotal = new Label();
            lblValoresTotal.Text = "<b>Valor total dos produtos:</b> " + nfeIde["ICMSTot"]["vProd"].InnerText;
            lblValoresTotal.Text += "<br /><b>Valor total do desconto:</b> " + nfeIde["ICMSTot"]["vDesc"].InnerText;
            lblValoresTotal.Text += "<br /><b>Valor total da nota fiscal:</b> " + nfeIde["ICMSTot"]["vNF"].InnerText;
            lblValoresTotal.Text += "<br />&nbsp;";
    
            tblValoresR01C1.Controls.Add(lblValoresTotalTitle);
            tblValoresRow01.Cells.Add(tblValoresR01C1);
    
            tblValoresR1C1.Controls.Add(lblValoresTotal);
            tblValoresRow1.Cells.Add(tblValoresR1C1);
    
            tblValores.Rows.Add(tblValoresRow01);
            tblValores.Rows.Add(tblValoresRow1);
    
            #endregion
    
            #region TAG 'transp'
    
            TableRow tblValoresRow02 = new TableRow();
            tblValoresRow02.BackColor = Color.LightGray;
            TableCell tblValoresR02C1 = new TableCell();
            Label lblValoresTranspTitle = new Label();
            lblValoresTranspTitle.Text = "<b>Informa��es de Transporte</b>";
    
            TableRow tblValoresRow2 = new TableRow();
            TableCell tblValoresR2C1 = new TableCell();
    
            nfeIde = nfeRoot["infNFe"]["transp"];
    
            Label lblValoresTransp = new Label();
            lblValoresTransp.Text += "<b>Modalidade de frete: </b>";
            var modalidadeFrete = (ModalidadeFrete)nfeIde["modFrete"].InnerText.StrParaInt();
            lblValoresTransp.Text += modalidadeFrete.ToString();
    
            try { lblValoresTransp.Text += "<br /><b>Peso L�quido:</b> " + nfeIde["vol"]["pesoL"].InnerText; }
            catch (NullReferenceException) { }
            try { lblValoresTransp.Text += "<br /><b>Peso Bruto:</b> " + nfeIde["vol"]["pesoB"].InnerText; }
            catch (NullReferenceException) { }
            lblValoresTransp.Text += "<br />&nbsp;";
    
            tblValoresR02C1.Controls.Add(lblValoresTranspTitle);
            tblValoresRow02.Cells.Add(tblValoresR02C1);
    
            tblValoresR2C1.Controls.Add(lblValoresTransp);
            tblValoresRow2.Cells.Add(tblValoresR2C1);
    
            tblValores.Rows.Add(tblValoresRow02);
            tblValores.Rows.Add(tblValoresRow2);
    
            #endregion
    
            #region TAG 'infAdic'
    
            if (nfeFile.GetElementsByTagName("infAdic").Count == 1)
            {
                TableRow tblValoresRow03 = new TableRow();
                tblValoresRow03.BackColor = Color.LightGray;
                TableCell tblValoresR03C1 = new TableCell();
                Label lblValoresInfAdicTitle = new Label();
                lblValoresInfAdicTitle.Text = "<b>Informa��es Adicionais</b>";
    
                TableRow tblValoresRow3 = new TableRow();
                TableCell tblValoresR3C1 = new TableCell();
    
                nfeIde = nfeRoot["infNFe"]["infAdic"];
    
                Label lblValoresInfAdic = new Label();
                try { lblValoresInfAdic.Text = nfeIde["infCpl"].InnerText; }
                catch (NullReferenceException) { }
                lblValoresInfAdic.Text += "<br />&nbsp;";
    
                tblValoresR03C1.Controls.Add(lblValoresInfAdicTitle);
                tblValoresRow03.Cells.Add(tblValoresR03C1);
    
                tblValoresR3C1.Controls.Add(lblValoresInfAdic);
                tblValoresRow3.Cells.Add(tblValoresR3C1);
    
                tblValores.Rows.Add(tblValoresRow03);
                tblValores.Rows.Add(tblValoresRow3);
            }

            #endregion

            #region TAG 'cobr'

            //se tiver a tag cobran�a adiciona na tela as informa��es referentes a mesma
            if (nfeFile.GetElementsByTagName("cobr").Count == 1)
            {
                Table tblParcelas = new Table();
                tblParcelas.ID = "tblParcelas";
                tblParcelas.Width = Unit.Parse("560px");

                TableRow tblParcelasHeaderRow = new TableRow();
                tblParcelasHeaderRow.BackColor = Color.LightGray;
                TableCell tblParcelasHeaderRowCell = new TableCell();
                Label lblParcelassHeader = new Label();
                lblParcelassHeader.Text = "<b>Parcelas</b>";
                tblParcelasHeaderRowCell.Controls.Add(lblParcelassHeader);
                tblParcelasHeaderRow.Cells.Add(tblParcelasHeaderRowCell);
                tblParcelas.Rows.Add(tblParcelasHeaderRow);

                plcParcelas.Controls.Add(tblParcelas);

                TableRow tblParcelasRow = new TableRow();
                TableCell tblParcelasRowCell = new TableCell();

                nfeIde = nfeRoot["infNFe"]["cobr"];

                ctrlParcelas1.Visible = true;
                hdfExibirParcelas.Value = "true";
                hdfTotal.Value = nfeIde["fat"]["vLiq"].InnerText;
                txtNumParc.Text = nfeIde.GetElementsByTagName("dup").Count > 0 ? nfeIde.GetElementsByTagName("dup").Count.ToString() : "1";
                hdfNumParc.Value = txtNumParc.Text;
                txtNumParc.Visible = true;
                lblNumParc.Visible = true;

                var valores = new List<decimal>();
                var datas = new List<DateTime>();
                var numDup = new List<string>();

                //Busca as infoma��es das cobran�as da nota fiscal
                XmlNodeList xlistParc = nfeFile.GetElementsByTagName("dup");
                for (int count = 0; count < nfeFile.GetElementsByTagName("dup").Count; count++)
                {
                    XmlElement xelParc = (XmlElement)xlistParc[count];

                    valores.Add(xelParc["vDup"].InnerText.StrParaDecimal());
                    datas.Add(xelParc["dVenc"].InnerText.StrParaDate().Value);
                    numDup.Add(xelParc["nDup"].InnerText);
                }

                //Preenche os valores, datas e numero do boleto(duplicata)
                //de acordo com o que foi buscado na nota
                ctrlParcelas1.Valores = valores.ToArray();
                ctrlParcelas1.Datas = datas.ToArray();
                ctrlParcelas1.Adicionais = numDup.ToArray();

                tblParcelasRow.Cells.Add(tblParcelasRowCell);
                tblParcelas.Rows.Add(tblParcelasRow);
            }

            #endregion

            #endregion
        }
    
        #endregion
    
        #region Importa os dados da nota para o sistema
    
        protected void btnImportar_Click(object sender, EventArgs e)
        {
            Label lblStatus = new Label();
            plcResultados.Controls.Add(lblStatus);
    
            try
            {
                XmlDocument loadedNFE = new XmlDocument();
                loadedNFE.LoadXml((string)Session["xmlnfe"]);
    
                Dictionary<string, object> naturezaOperacaoProd = new Dictionary<string, object>();
                foreach (string key in Session)
                    if (!naturezaOperacaoProd.ContainsKey(key))
                        naturezaOperacaoProd.Add(key, Session[key]);

                var pagtoImportar = new List<WebGlass.Business.NotaFiscal.Entidade.PagtoNotaFiscal>();
                var parcelasImportar = new List<WebGlass.Business.NotaFiscal.Entidade.ParcelaNf>();
                var pagamentos = ctrlFormaPagtoNotaFiscal.PagtoNotaFiscal;
                var numParc = hdfNumParc.Value.StrParaInt();
                
                foreach (var pagto in pagamentos)
                {
                    pagtoImportar.Add(new WebGlass.Business.NotaFiscal.Entidade.PagtoNotaFiscal
                    {
                        Valor = pagto.Valor,
                        FormaPagto = pagto.FormaPagto
                    });
                }

                for (int i = 0; i < numParc; i++)
                {
                    parcelasImportar.Add(new WebGlass.Business.NotaFiscal.Entidade.ParcelaNf
                    {
                        Valor = ctrlParcelas1.Valores[i],
                        DataVenc = ctrlParcelas1.Datas[i],
                        NumBoleto = ctrlParcelas1.Adicionais[i]
                    });
                }
                

                var dadosImportar = new WebGlass.Business.NotaFiscal.Entidade.DadosImportarNFe()
                {
                    NaturezaOperacaoProd = naturezaOperacaoProd,
                    ChaveAcesso = hdfChaveAcesso.Value,
                    Pagamentos = pagtoImportar,
                    IdPlanoConta = Glass.Conversoes.StrParaUint(drpPlanoConta.SelectedValue),
                    IdNaturezaOperacao = ctrlNaturezaOperacao.CodigoNaturezaOperacao.Value,
                    IdCompra = Glass.Conversoes.StrParaUintNullable(this.txtNumCompra.Text),
                    Parcelas = parcelasImportar
                    
                };
    
                WebGlass.Business.NotaFiscal.Fluxo.Importar.Instance.ImportarNFe(loadedNFE, dadosImportar);
    
                lblStatus.Text = "Nota Fiscal Eletr�nica importada com sucesso.";
    
                for (int i = 0; i < Session.Keys.Count; i++)
                    if (Session.Keys[i].ToLower() != "idusuario")
                    {
                        Session.Remove(Session.Keys[i]);
                        i--;
                    }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro de importa��o:<br />" + ex.Message; //Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        #endregion
    
        /// <summary>
        /// Associa o produto da nota ao produto do sistema com o respectivo fornecedor
        /// </summary>
        /// <param name="codProd"></param>
        /// <param name="numProd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string AssociaProduto(string codProd, string codFornec, string idFornec)
        {
            return WebGlass.Business.NotaFiscal.Fluxo.ProdutoFornecedor.Ajax.AssociaProduto(codProd, codFornec, idFornec);
        }
    
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string SetNaturezaOperacaoProduto(string codNaturezaOperacao, string numProd)
        {
            Session["prodNatOp" + numProd.ToString()] = codNaturezaOperacao;
            return numProd + "#" + codNaturezaOperacao;
        }
    
        /// <summary>
        /// Altera a visibilidade das dropdownlists da p�gina
        /// </summary>
        protected void AlteraVisibilidade(bool visivel)
        {
            lblImportar.Visible = visivel;
            lblImportar.Enabled = visivel;
            btnImportar.Visible = visivel;
            btnImportar.Enabled = visivel;
            lblPlanoConta.Visible = visivel;
            lblPlanoConta.Enabled = visivel;
            lblNaturezaOperacao.Visible = visivel;
            lblNaturezaOperacao.Enabled = visivel;
            ctrlFormaPagtoNotaFiscal.Visible = visivel;
            drpPlanoConta.Visible = visivel;
            drpPlanoConta.Enabled = visivel;
            ctrlNaturezaOperacao.Visible = visivel;
            ctrlNaturezaOperacao.Enabled = visivel;
            lblNumCompra.Visible = visivel;
            txtNumCompra.Visible = visivel;
        }

        protected void drpPlanoConta_DataBound(object sender, EventArgs e)
        {
            // Se o fornecedor j� foi recuperado do XML, e o mesmo tem Plano de Conta associado, seleciona o plano como padr�o
            if (_idFornecedor > 0)
            {
                var idConta = FornecedorDAO.Instance.ObtemIdConta(null, _idFornecedor);
                if (idConta != null)
                    // Try catch colocado para n�o dar erro quando o fornecedor tiver um plano de contas inativo associado, pois o mesmo n�o est� na lista
                    // Ao bloquear a inativa��o do plano de contas em uso ele pode ser removido.
                    try
                    {
                        drpPlanoConta.SelectedValue = idConta.ToString();
                    }
                    catch { }
            }
        }

        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcelas;
            ctrlParcelas.CampoParcelasVisiveis = hdfNumParc;
            ctrlParcelas.CampoValorTotal = hdfTotal;
            ctrlParcelas.NumParcelas = Glass.Configuracoes.FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe;
            ctrlParcelas.DiasSomarDataVazia = 30;
        }
    }
}
