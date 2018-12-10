using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Security;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using System.IO;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class CadProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadProducao));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            // Limpa os campos de perda
            chkPerda.Checked = false;
            ctrlTipoPerda1.IdTipoPerda = null;
            ctrlTipoPerda1.IdSubtipoPerda = null;
            txtObs.Text = "";

            // Altera a cor do label de descrição da etiqueta
            if (IsPostBack)
                lblDescrEtiqueta.ForeColor = lblDescrEtiqueta.ForeColor == System.Drawing.Color.Black ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();

            UserInfo.SetActivity();

            // Obtém os setores que o funcionário possui acesso
            var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

            /* Chamado 37879.
             * Foi solicitado pela Delcilei que todos os tipos de perda fossem exibidos para os marcadores. */
            // Busca somente os tipos de perda do setor selecionado.
            //ctrlTipoPerda1.IdSetor = funcSetor[0].IdSetor;

            // Se não for marcador produção ou se não tiver setores associados, sai desta tela
            if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao)
                Response.Redirect("~/WebGlass/Main.aspx");

            if (PCPConfig.UsarNovoControleExpBalcao && funcSetor.Count > 0)
            {
                foreach (var fs in funcSetor)
                {
                    if (Data.Helper.Utils.ObtemSetor((uint)fs.IdSetor).Tipo == TipoSetor.Entregue)
                    {
                        Response.Redirect("~/Cadastros/Expedicao/CadLeituraExpBalcao.aspx");
                    }
                }
            }

            // Se o usuário não possuir nenhum setor associado, só poderá visualizar a consulta de produção
            if (funcSetor.Count == 0)
            {
                ClientScript.RegisterStartupScript(typeof(string), "consulta", "consultaProducao();", true);
                return;
            }

            var setorPrincipal = SetorDAO.Instance.GetElementByPrimaryKey((uint)funcSetor[0].IdSetor);

            odsTotaisSetor.SelectParameters[0].DefaultValue = setorPrincipal.IdSetor.ToString();

            if (!IsPostBack)
            {
                if (setorPrincipal.Tipo == TipoSetor.Entregue)
                    pedidoNovo.Visible = true;

                if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ReposicaoDePeca))
                    chkPerda.Attributes.Add("style", "display: none");

                codChapa.Visible = setorPrincipal.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte;
                tdCodCavalete.Visible = setorPrincipal.InformarCavalete;
                tdFornada.Visible = setorPrincipal.Forno && setorPrincipal.GerenciarFornada && Configuracoes.PCPConfig.GerenciamentoFornada;
                tbRota.Style.Value = "display: " + (setorPrincipal.InformarRota ? "block" : "none");
                lblTitulo.Text = setorPrincipal.Descricao;
                hdfTitulo.Value = setorPrincipal.Descricao;
                hdfSetor.Value = setorPrincipal.IdSetor.ToString();
                hdfTempoLogin.Value = setorPrincipal.TempoLogin.ToString();
                hdfCorTela.Value = setorPrincipal.DescrCorTela;
                hdfInformarRota.Value = setorPrincipal.InformarRota.ToString().ToLower();
                hdfSituacao.Value = ((int)ProdutoPedidoProducao.SituacaoEnum.Producao).ToString();

                tbImagemCompleta.Visible = setorPrincipal.ExibirImagemCompleta;
                tbSetoresPeca.Visible = setorPrincipal.ExibirSetores;
            }

            #region Mensagens

            var mensagemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMensagemFluxo>();

            // Verifica se há novas mensagens e quantas são
            var msgNova = mensagemFluxo.ExistemNovasMensagens((int)UserInfo.GetUserInfo.CodUser);
            lnkMensagens.Visible = !msgNova;
            lnkMensagensNaoLidas.Visible = msgNova;

            #endregion

            perdaDefinitiva.Visible = false; //Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca;

            // Carrega dados da peça consultada
            // Não pode por !IsPostBack aqui, pois isso faz com que os dados da peça não sejam carregados na tela
            ConsultaDadosPeca();

            //Atualiza a fornada
            if (PCPConfig.GerenciamentoFornada)
            {
                grdFornada.PageSize = 10;
                grdFornada.ShowFooter = false;
                var idFornada = FornadaDAO.Instance.ObterIdUltimaFornadaFunc(null, hdfFunc.Value.StrParaInt());
                txtCodFornada.Text = idFornada.ToString();
                lblDadosFornada.Text = FornadaDAO.Instance.ObterM2QtdeLido(idFornada);

                grdFornada.DataBind();
            }

            // Força a atualização das Grids com os setores
            grdSetoresLidos.DataBind();
            grdSetoresRestantes.DataBind();

            // Verifica se é para exibir os totais por setor
            if (System.Configuration.ConfigurationManager.AppSettings["ExibirTotaisSetor"] == "true")
                dtvTotaisSetor.Visible = true;

            #region Monta menu da produção

            var tr = new TableRow();
            tbMenu.Controls.Add(tr);

            foreach (var fs in funcSetor)
            {
                if (fs.IdSetor == 1)
                    continue;

                var td = new TableCell();
                var lnkButton = new LinkButton();
                lnkButton.Text = fs.DescrSetor;
                lnkButton.Attributes.Add("idSetor", fs.IdSetor.ToString());
                lnkButton.Click += new EventHandler(lnkButton_Click);
                lnkButton.OnClientClick = "permiteMudarMenu();";
                td.Controls.Add(lnkButton);
                tr.Controls.Add(td);
                tr.Controls.Add(new TableCell());
            }

            // Consultar produção.
            var tdCons = new TableCell();
            var lnkButtonCons = new LinkButton();
            lnkButtonCons.Text = "Consultar Produção";
            lnkButtonCons.OnClientClick = "return consultaProducao();";
            tdCons.Controls.Add(lnkButtonCons);
            tr.Controls.Add(tdCons);
            tr.Controls.Add(new TableCell());

            // Painel da produção
            var tdPainel = new TableCell();
            var lnkButtonPainel = new LinkButton();
            lnkButtonPainel.Text = "Painel Produção";

            if (!ProducaoConfig.TelaMarcacaoPeca.ExibirPainelSetores)
                lnkButtonPainel.OnClientClick = "return painelProducao();";
            else
                lnkButtonPainel.OnClientClick = "return painelProducaoSetores();";

            tdPainel.Controls.Add(lnkButtonPainel);
            tr.Controls.Add(tdPainel);
            tr.Controls.Add(new TableCell());

            // Painel geral.
            var tdPainelGeral = new TableCell();
            var lnkButtonPainelGeral = new LinkButton();
            lnkButtonPainelGeral.Text = "Painel Geral";
            lnkButtonPainelGeral.OnClientClick = "return painelGeral();";
            tdPainelGeral.Controls.Add(lnkButtonPainelGeral);
            tr.Controls.Add(tdPainelGeral);
            tr.Controls.Add(new TableCell());

            // Troca/Devolução
            if (Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao))
            {
                var tdTrocaDev = new TableCell();
                var lnkButtonTrocaDev = new LinkButton();
                lnkButtonTrocaDev.Text = "Troca/Devolução";
                lnkButtonTrocaDev.OnClientClick = "openWindow(600, 800, '../../Listas/LstTrocaDev.aspx?popup=1'); return false;";
                tdTrocaDev.Controls.Add(lnkButtonTrocaDev);
                tr.Controls.Add(tdTrocaDev);
                tr.Controls.Add(new TableCell());
            }

            // Peças pendentes para leitura
            var tdPecasPendentes = new TableCell();
            var lnkPecasPendentes = new LinkButton();
            lnkPecasPendentes.Text = "Pedidos pendentes para leitura";
            lnkPecasPendentes.OnClientClick = "return pedidosPendentesLeitura();";
            tdPecasPendentes.Controls.Add(lnkPecasPendentes);
            tr.Controls.Add(tdPecasPendentes);
            tr.Controls.Add(new TableCell());

            #endregion
        }

        #region Dados de peça

        void ConsultaDadosPeca()
        {
            try
            {
                // Carrega a imagem desta peça, se tiver sido informado uma etiqueta válida
                if (!String.IsNullOrEmpty(txtCodPeca.Text) && ProdutoPedidoProducaoDAO.Instance.PecaEstaEmProducao(txtCodPeca.Text))
                {
                    var prodPedProducao = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(txtCodPeca.Text);
                    var prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(null, prodPedProducao.IdProdPed, true);
                    PecaItemProjeto peca = null;

                    if (prodPed.IdMaterItemProj > 0)
                        peca = PecaItemProjetoDAO.Instance.GetByMaterial(prodPed.IdMaterItemProj.Value);

                    uint idItemProjeto = 0;

                    //caminho do arquivo SVG
                    var caminho = PCPConfig.CaminhoSalvarCadProject(true) + prodPed.IdProdPed + ".svg";

                    // Limpa a div da imagem
                    imgPeca.ImageUrl = string.Empty;
                    imgLogoCliente.ImageUrl = string.Empty;

                    // Carrega a logo do cliente.
                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IClienteRepositorioImagens>();

                    var logoCliente = repositorio.ObterUrl(ClienteDAO.Instance.GetByPedido(prodPed.IdPedido).IdCli);
                    if (!logoCliente.ToLower().Contains("/handlers/") && File.Exists(Server.MapPath(logoCliente))) // Se o arquivo físico existir, retorna-o
                        imgLogoCliente.ImageUrl = "../../Handlers/LoadImage.ashx?resize=true&altura=150&largura=150&path=" + Server.MapPath(logoCliente);
                    else
                        imgLogoCliente.ImageUrl = logoCliente;

                    string imagem = prodPedProducao.ImagemPecaUrl;
                    if (!imagem.ToLower().Contains("/handlers/") && File.Exists(Server.MapPath(imagem))) // Se o arquivo físico existir, retorna-o
                        imgPeca.ImageUrl = "../../Handlers/LoadImage.ashx?altura=600&largura=500&path=" + Server.MapPath(imagem);
                    else if (prodPed.IdMaterItemProj > 0 && peca != null && peca.Tipo == 1) // caso não exista arquivo físico, retorna a imagem do projeto
                    {
                        idItemProjeto = MaterialItemProjetoDAO.Instance.GetIdItemProjeto(prodPed.IdMaterItemProj.Value);
                        imgPeca.ImageUrl = UtilsProjeto.GetFiguraAssociadaUrl(idItemProjeto, prodPedProducao.NumEtiqueta, false) + "&perc=1,4";
                        imgPeca.ImageUrl += "&numEtiqueta=" + txtCodPeca.Text;
                    }
                    // Retirado, quando o pedido é de reposição, o endereço da imagem estava sendo apagado.
                    else if (peca != null && peca.Tipo == 2)
                        imgPeca.ImageUrl = String.Empty;
                    else
                        imgPeca.ImageUrl = imagem;

                    // Carrega a imagem completa da peça
                    if (idItemProjeto > 0)
                    {
                        imgProjeto.ImageUrl = UtilsProjeto.GetFiguraAssociadaUrl(idItemProjeto, prodPedProducao.NumEtiqueta, true) + "&perc=0,6";

                        // Carrega a imagem modelo
                        imgModelo.ImageUrl = Data.Helper.Utils.GetModelosProjetoVirtualPath + ProjetoModeloDAO.Instance.ObtemNomeFigura(
                            ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, idItemProjeto));
                    }
                    else
                    {
                        // Carrega a imagem modelo mesmo se a peça for fixa
                        if (prodPed.IdMaterItemProj > 0)
                            imgModelo.ImageUrl = Data.Helper.Utils.GetModelosProjetoVirtualPath + ProjetoModeloDAO.Instance.ObtemNomeFigura(
                                ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, MaterialItemProjetoDAO.Instance.GetIdItemProjeto(prodPed.IdMaterItemProj.Value)));
                        else
                            imgModelo.ImageUrl = String.Empty;

                        imgProjeto.ImageUrl = String.Empty;
                    }

                    // Exibe ou esconde o link para baixa o arquivo ISO, caso seja exibido, configura o link para baixar o arquivo.
                    uint? idCnc = SetorDAO.Instance.ObtemIdCnc(Glass.Conversoes.StrParaUint(hdfSetor.Value));
                    lnkArquivoIso.Visible = idCnc > 0 && PecaProjetoModeloDAO.Instance.ObtemIdArquivoMesaCorte(peca.IdPecaProjMod) > 0;
                    if (lnkArquivoIso.Visible)
                        lnkArquivoIso.NavigateUrl = "../../Handlers/ArquivoOtimizacao.ashx?apenasArqMesa=true&numEtiqueta=" + txtCodPeca.Text +
                            "&idSetor=" + Glass.Conversoes.StrParaUint(hdfSetor.Value);

                    lnkAnexo.OnClientClick = "openWindow(600, 700, '../CadFotos.aspx?id=" + prodPed.IdPedido + "&tipo=pedido'); return false;";
                    lnkAnexo.Visible = true;

                    if (((ProducaoConfig.TelaMarcacaoPeca.ExibirAnexosPedidosMaoDeObraAoConsultarPeca && PedidoDAO.Instance.IsMaoDeObra(null, prodPed.IdPedido)) ||
                        ProducaoConfig.TelaMarcacaoPeca.SempreExibirAnexosPedidosAoConsultarPeca) && FotosPedidoDAO.Instance.PossuiAnexo(prodPed.IdPedido))
                        ClientScript.RegisterStartupScript(typeof(string), "anexo", "document.getElementById(\"lnkAnexo\").onclick();", true);

                    lblTitleCliente.Text = "Cliente: ";
                    lblTitleVendedor.Text = "Vendedor: ";
                    lblTitlePeca.Text = "Peça: ";
                    lblTitleApl.Text = "Aplicação: ";
                    lblTitleProc.Text = "Processo: ";

                    lblCliente.Text = ClienteDAO.Instance.GetByPedido(prodPed.IdPedido).Nome;
                    lblVendedor.Text = PedidoDAO.Instance.ObtemNomeFuncResp(null, prodPed.IdPedido);
                    lblPeca.Text = prodPed.DescrProduto + " " + prodPed.LarguraProducao + "x" + prodPed.AlturaProducao;
                    lblApl.Text = prodPed.CodAplicacao;
                    lblProc.Text = prodPed.CodProcesso;

                    if (idItemProjeto == 0 && prodPed.IdMaterItemProj > 0)
                        idItemProjeto = MaterialItemProjetoDAO.Instance.GetIdItemProjeto(prodPed.IdMaterItemProj.Value);

                    // Exibe a observação do projeto se a peça vier de projeto, for instalação ou se o projeto possuir apenas fixos ou
                    // se a peça for de pedido importado e o processo da mesma bater com a configuração de instalação nos proejtos.
                    // A obervação da peça deve ser exibida sempre que existir
                    lblObsProj.Text = lblObsProjAcima.Text = (idItemProjeto > 0 &&
                        (peca != null && (peca.Tipo == 1 || PecaProjetoModeloDAO.Instance.ProjetoPossuiApenasFixos(ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, idItemProjeto)))) ?
                        ItemProjetoDAO.Instance.ObtemObs(idItemProjeto) : string.Empty) +
                        (!string.IsNullOrEmpty(prodPed.Obs) ? string.Format(" {0}", prodPed.Obs) : string.Empty) + (!string.IsNullOrEmpty(prodPed.DescrBeneficiamentos) ? string.Format(" {0}", prodPed.DescrBeneficiamentos) : string.Empty);

                    /* Chamado 45495, 47269. */
                    // Mostra o numero da etiqueta e obs abaixo/acima da imagem da peça.
                    lblNumEtiquetaAbaixo.Text = txtCodPeca.Text;
                    lblNumEtiquetaAcima.Text = txtCodPeca.Text;
                    lblNumEtiquetaAbaixo.Visible = lblObsProj.Visible = !PCPConfig.ExibirNumeroEtiquetaAcimaImagemPecaTelaMarcacao;
                    lblNumEtiquetaAcima.Visible = lblObsProjAcima.Visible = PCPConfig.ExibirNumeroEtiquetaAcimaImagemPecaTelaMarcacao;
                }
                else
                    LimpaDadosPeca();
            }
            catch (Exception ex)
            {
                hdfTeste.Value = Glass.MensagemAlerta.FormatErrorMsg("", ex);
                LimpaDadosPeca();
            }
        }

        void LimpaDadosPeca()
        {
            imgPeca.ImageUrl = String.Empty;
            imgLogoCliente.ImageUrl = string.Empty;
            imgProjeto.ImageUrl = String.Empty;
            imgModelo.ImageUrl = String.Empty;
            txtCodPeca.Text = String.Empty;

            lblTitleCliente.Text = String.Empty;
            lblTitleVendedor.Text = string.Empty;
            lblTitlePeca.Text = String.Empty;
            lblTitleApl.Text = String.Empty;
            lblTitleProc.Text = String.Empty;

            lblCliente.Text = String.Empty;
            lblVendedor.Text = String.Empty;
            lblPeca.Text = String.Empty;
            lblApl.Text = String.Empty;
            lblProc.Text = String.Empty;
            lblObsProj.Text = String.Empty;

            lnkAnexo.Visible = false;
        }

        private string MontaHtmlSvg(int idProdPedEsp)
        {
            //Caminho do SVG
            var caminhoSvg = PCPConfig.CaminhoSalvarCadProject(true) + idProdPedEsp + ".svg";

            if (!File.Exists(caminhoSvg))
                return null;

            //Carrega o SVG
            var svg = File.ReadAllText(caminhoSvg);

            var html = svg;
            html += @"
                    <script>
                        var panZoom = svgPanZoom('#export-viewer', {
                                        zoomEnabled: true,
                                        controlIconsEnabled: false,
                                        panEnabled: true,
                                        dblClickZoomEnabled: true,
                                        fit: true,
                                        center: true,
                                    });
                    </script>";

            return html;
        }



       #endregion

        void lnkButton_Click(object sender, EventArgs e)
        {
            var idSetor = (((LinkButton)sender).Attributes["idSetor"]).StrParaUintNullable();

            if (idSetor.GetValueOrDefault() == 0)
                throw new Exception("Não foi possível recuperar a identificação do setor.");

            var setor = Data.Helper.Utils.ObtemSetor(idSetor.Value);

            if (setor == null || setor.IdSetor == 0)
                throw new Exception("Este setor foi inativado ou não existe mais.");

            /* Chamado 37879.
             * Foi solicitado pela Delcilei que todos os tipos de perda fossem exibidos para os marcadores. */
            // Busca somente os tipos de perda do setor selecionado.
            //ctrlTipoPerda1.IdSetor = (int)idSetor;

            // Habilita campos se o tipo do setor selecionado for "Entregue"
            var setorEntregue = setor.Tipo == TipoSetor.Entregue;
            pedidoNovo.Visible = setorEntregue;

            // Desmarca checkBoxes
            chkPedidoNovo.Checked = false;
            chkPerda.Checked = false;
            codChapa.Visible = setor.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte;
            tdCodCavalete.Visible = setor.InformarCavalete;
            tdFornada.Visible = setor.Forno && setor.GerenciarFornada && Configuracoes.PCPConfig.GerenciamentoFornada;
            tbRota.Style.Value = "display: " + (setor.InformarRota ? "inline" : "none");

            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ReposicaoDePeca))
                chkPerda.Attributes.Add("style", "display: none");

            // Altera campos para que fique com os dados do setor selecionado
            lblTitulo.Text = setor.Descricao;
            hdfTitulo.Value = setor.Descricao;
            hdfSetor.Value = setor.IdSetor.ToString();
            hdfTempoLogin.Value = setor.TempoLogin.ToString();
            hdfCorTela.Value = setor.DescrCorTela;
            hdfInformarRota.Value = setor.InformarRota.ToString().ToLower();
            hdfSituacao.Value = ((int)ProdutoPedidoProducao.SituacaoEnum.Producao).ToString();

            tbImagemCompleta.Visible = setor.ExibirImagemCompleta;
            tbSetoresPeca.Visible = setor.ExibirSetores;

            ConsultaDadosPeca();

            if (dtvTotaisSetor.Visible)
                dtvTotaisSetor.DataBind();
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod()]
        public string AtualizaSituacao(string idFuncStr, string numChapa, string numEtiqueta, string idSetor, string isPerdaStr, string tipoPerdaStr,
            string subtipoPerdaStr, string obs, string idPedidoNovo, string idRota, string isEntradaEstoqueStr,
            string perdaDefinitivaStr, string altura, string largura, string quantidade, string observacao, string etiquetasMateriaPrima,
            string etiquetaCavalete, string idFornada)
        {
            try
            {
                var isPerda = bool.Parse(isPerdaStr);
                string descrProd = null;
                var pedidoNovo = !string.IsNullOrEmpty(idPedidoNovo) ? (uint?)idPedidoNovo.StrParaUint() : null;
                var perdaDefinitiva = bool.Parse(perdaDefinitivaStr);
                var etiquetasMp = etiquetasMateriaPrima.Split(',').Where(f => !string.IsNullOrEmpty(f)).ToList();

                numEtiqueta = numEtiqueta.Replace("p", "P");

                if (!isPerda)
                {
                    if (Data.Helper.Utils.ObtemSetor(idSetor.StrParaUint()).Tipo == TipoSetor.Entregue
                        && (ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(numEtiqueta) == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal
                            || ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(numEtiqueta) == ProdutoImpressaoDAO.TipoEtiqueta.Retalho))
                    {
                        descrProd = ProdutoPedidoProducaoDAO.Instance.MarcaExpedicaoChapaRetalhoRevendaComTransacao(
                            numEtiqueta,
                            pedidoNovo.GetValueOrDefault(),
                            idSetor.StrParaUint());
                    }
                    else if (Data.Helper.Utils.ObtemSetor(idSetor.StrParaUint()).Tipo == TipoSetor.Entregue
                        && numEtiqueta.ToUpper()[0] == 'V')
                    {
                        descrProd = WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.MarcaExpedicaoVolumeComTransacao(
                            numEtiqueta,
                            0,
                            false);
                    }
                    else if (numEtiqueta.ToUpper()[0] == 'P')
                    {
                        descrProd = ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(
                            idFuncStr.StrParaUint(),
                            numChapa,
                            numEtiqueta,
                            idSetor.StrParaUint(),
                            false,
                            false,
                            null,
                            null,
                            null,
                            pedidoNovo,
                            idRota.StrParaUint(),
                            etiquetasMp,
                            null,
                            false,
                            etiquetaCavalete,
                            idFornada.StrParaInt());
                    }
                    else
                    {
                        descrProd = ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(
                            idFuncStr.StrParaUint(),
                            numChapa,
                            numEtiqueta,
                            idSetor.StrParaUint(),
                            false,
                            false,
                            null,
                            null,
                            null,
                            pedidoNovo,
                            idRota.StrParaUint(),
                            etiquetasMp,
                            null,
                            false,
                            etiquetaCavalete,
                            idFornada.StrParaInt());
                    }
                }
                else if (ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                {
                    descrProd = ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(
                        idFuncStr.StrParaUint(),
                        numChapa,
                        numEtiqueta,
                        idSetor.StrParaUint(),
                        true,
                        false,
                        tipoPerdaStr.StrParaUint(),
                        subtipoPerdaStr.StrParaUintNullable(),
                        obs,
                        null,
                        0,
                        null,
                        null,
                        false,
                        etiquetaCavalete,
                        idFornada.StrParaInt());
                }
                else
                {
                    descrProd = ProdutoPedidoProducaoDAO.Instance.MarcarPecaRepostaComTransacao(
                        numChapa,
                        numEtiqueta,
                        idSetor.StrParaUint(),
                        UserInfo.GetUserInfo.CodUser,
                        DateTime.Now,
                        tipoPerdaStr.StrParaUint(),
                        subtipoPerdaStr.StrParaUintNullable(),
                        obs,
                        perdaDefinitiva);
                }

                //Insere retalho se tiver sido preechido
                bool hasRetalho = !string.IsNullOrEmpty(altura) && !string.IsNullOrEmpty(largura) && !string.IsNullOrEmpty(quantidade);

                if (hasRetalho)
                {
                    if (bool.Parse(IsCorte(idSetor)) && !string.IsNullOrEmpty(numChapa))
                    {
                        ProdutoImpressaoDAO.TipoEtiqueta tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(numChapa);
                        uint idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(numChapa, tipoEtiqueta);
                        uint idProdNf = ProdutoImpressaoDAO.Instance.ObtemIdProdNf(null, numChapa, tipoEtiqueta);
                        uint? idProd = ProdutoImpressaoDAO.Instance.GetIdProd(idProdImpressao);

                        List<RetalhoProducaoAuxiliar> dadosRetalho = new List<RetalhoProducaoAuxiliar>();
                        //Separa os valores dos retalhos
                        string[] alturaArray = altura.Split(';');
                        string[] larguraArray = largura.Split(';');
                        string[] quantidadeArray = quantidade.Split(';');
                        string[] observacaoArray = observacao.Split(';');
                        for (int i = 0; i < alturaArray.Length; i++)
                            dadosRetalho.Add(new RetalhoProducaoAuxiliar(0, decimal.Parse(alturaArray[i]), decimal.Parse(larguraArray[i]),
                                Glass.Conversoes.StrParaInt(quantidadeArray[i]),
                                observacaoArray != null && observacaoArray.Length > 0 && observacaoArray.Length >= i - 1 ? observacaoArray[i] : null));

                        RetalhoProducaoDAO.Instance.CriarRetalho(dadosRetalho, idProd.Value, idProdNf);
                    }
                    else
                        RetalhoProducaoDAO.Instance.CriarRetalho(altura, largura, quantidade, numEtiqueta, observacao);
                }

                return "Ok|" + descrProd.Replace("<br />", " - ");
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string IsProducao(string numEtiqueta)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(numEtiqueta.Split('-')[0]);
                return PedidoDAO.Instance.IsProducao(null, idPedido).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        [Ajax.AjaxMethod]
        public string IsProdLamComposicao(string numEtiqueta)
        {
            try
            {
                return (ProdutoPedidoProducaoDAO.Instance.IsProdLamComposicao(numEtiqueta)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        [Ajax.AjaxMethod]
        public string IsFilhoProdLamComposicao(string numEtiqueta)
        {
            try
            {
                return (ProdutoPedidoProducaoDAO.Instance.ObterIdProdPedProducaoParentByEtiqueta(null, numEtiqueta).GetValueOrDefault(0) > 0).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        [Ajax.AjaxMethod]
        public string VerificaPedidoExiste(string pedido)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(pedido);
                return PedidoDAO.Instance.PedidoExists(idPedido).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        /// <summary>
        /// Verifica se a matéria-prima ja teve um plano de corte lido.
        /// </summary>
        /// <param name="numChapa"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaPlanoDeCorteDaChapa(string numChapa)
        {
            var chapaVinculada = ChapaCortePecaDAO.Instance.ChapaPossuiPlanoCorteVinculado(numChapa);

            return chapaVinculada.ToString().ToLower() + "\t" +
                ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraChapaComPlanoCorteVinculado.ToString().ToLower();
        }

        /// <summary>
        /// Verifica se a matéria-prima ja deu saida atraves de um pedido de revenda
        /// </summary>
        /// <param name="numChapa"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaSeChapaDeuSaidaEmPedidoRevenda(string numChapa)
        {
            return ChapaCortePecaDAO.Instance.ChapaDeuSaidaEmPedidoRevenda(numChapa).ToString().ToLower();
        }

        /// <summary>
        /// Verifica se a matéria-prima ja teve leitura em dias
        /// </summary>
        /// <param name="numChapa"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaLeituraChapaDiasDiferentes(string numChapa)
        {
            if (!ProducaoConfig.BloquearLeituraPecaNaChapaDiasDiferentes)
                return "true";

            var dias = ChapaCortePecaDAO.Instance.ObtemDiasLeitura(numChapa);

            return (dias.Count() == 0 || (dias.Count() == 1 && dias[0].Date == DateTime.Now.Date)).ToString().ToLower();
        }

        /// <summary>
        /// Verifica se a matéria-prima ja teve alguma leitura
        /// </summary>
        /// <param name="numChapa"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaLeituraChapa(string numChapa)
        {
            var temLeitura = ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(numChapa);

            return temLeitura.ToString().ToLower() + "\t" +
                ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraChapaComPlanoCorteVinculado.ToString().ToLower();
        }

        /// <summary>
        /// Busca os produtos de estoque do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetProdutosPedido(string idPedido)
        {
            try
            {
                uint id;
                if (!uint.TryParse(idPedido, out id))
                    throw new Exception("Número do pedido inválido.");

                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && PedidoDAO.Instance.GetTipoPedido(null, id) != Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                    throw new Exception("Apenas pedidos de revenda podem ser utilizados nesta opção.");

                var produtosTemp = id > 0 ? ProdutosPedidoDAO.Instance.GetByPedido(id, true) : new ProdutosPedido[0];
                if (produtosTemp.Count == 0)
                    throw new Exception("Não foram encontrados produtos para esse pedido.");

                var retorno = new StringBuilder(@"<table>
                    <tr><th style='font-size: small'>Produto</th>
                    <th style='padding-left: 8px; font-size: small'>Qtde.</th>
                    <th style='padding-left: 8px; font-size: small'>Qtde. Expedir</th>
                    <th style='padding-left: 8px; font-size: small'>Altura</th>
                    <th style='padding-left: 8px; font-size: small'>Largura</th></tr>");

                var agrupa = new Dictionary<uint,ProdutosPedido>();
                foreach (var p in produtosTemp)
                {
                    if (!agrupa.ContainsKey(p.IdProd))
                        agrupa.Add(p.IdProd, p);
                    else
                        agrupa[p.IdProd].Qtde += p.Qtde;
                }

                foreach (var p in agrupa.Values)
                {
                    int qtde = (int)p.Qtde;
                    qtde -= ProdutoPedidoProducaoDAO.Instance.GetQtdeLiberadaByPedProd(p.IdPedido, null, p.IdProd);

                    if (qtde <= 0)
                        continue;

                    retorno.Append("<tr><td style='font-size: small'>");
                    retorno.Append(p.DescrProduto);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.Qtde);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(qtde);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.AlturaLista);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.Largura);
                    retorno.Append("</td></tr>");
                }

                retorno.Append("</table>");
                return retorno.ToString();
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        [Ajax.AjaxMethod()]
        public string GetRota(string codRota)
        {
            try
            {
                var rota = RotaDAO.Instance.GetByCodInterno(codRota);

                if (rota == null)
                    return "Erro|Não foi encontrada nenhuma rota com o código informado.";

                return "Ok|" + rota.IdRota + "|" + rota.Descricao;
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar rota.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string ConsultaAntes(string setor)
        {
            try
            {
                return SetorDAO.Instance.ConsultaAntes(Glass.Conversoes.StrParaUint(setor)).ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string IsCorte(string setor)
        {
            try
            {
                return (PCPConfig.Etiqueta.UsarControleChapaCorte && SetorDAO.Instance.IsCorte(Glass.Conversoes.StrParaUint(setor))).ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string IsLaminado(string setor)
        {
            try
            {
                return (SetorDAO.Instance.IsLaminado(null, Glass.Conversoes.StrParaUint(setor))).ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string CamposMateriaPrima(string etiqueta)
        {
            ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(null, ref etiqueta);

            var idProd = ProdutoDAO.Instance.ObtemIdProdByEtiqueta(etiqueta);

            var retorno = @"<table id=""tbMateriaPrima"">";

            var subGrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);
            var idProdPedProducaoParent = ProdutoPedidoProducaoDAO.Instance.ObterIdProdPedProducaoParentByEtiqueta(null, etiqueta);

            if (subGrupoProd == TipoSubgrupoProd.VidroDuplo || subGrupoProd == TipoSubgrupoProd.VidroLaminado || idProdPedProducaoParent.GetValueOrDefault(0) > 0)
            {
                uint idProdPed = 0;
                var informouEtiqueta = false;
                var etiquetaInformada = "";

                if (idProdPedProducaoParent.GetValueOrDefault(0) > 0)
                {
                    idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(null, etiqueta);
                    etiquetaInformada = etiqueta;
                    etiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(idProdPedProducaoParent.Value);
                }

                var pecas = ProdutosPedidoEspelhoDAO.Instance.ObterFilhosComposicaoByEtiqueta(null, etiqueta, false)
                    .OrderBy(f => ProdutoDAO.Instance.IsVidro(null, (int)f.IdProd)).ToList();

                var count = 1;

                var retornoNaoLer = string.Empty;

                for (int i = 0; i < pecas.Count; i++)
                {
                    for (int j = 0; j < pecas[i].Qtde; j++)
                    {
                        var etiquetaMostrar = pecas[i].IdProdPed == idProdPed && !informouEtiqueta ?
                            etiquetaInformada : "";

                        if (!string.IsNullOrEmpty(etiquetaMostrar))
                            informouEtiqueta = true;

                        var isVidro = ProdutoDAO.Instance.IsVidro(null, (int)pecas[i].IdProd);
                        var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)pecas[i].IdProd);

                        if (!isVidro && tipoSubgrupo != TipoSubgrupoProd.PVB)
                        {
                            retornoNaoLer += @"
                            <tr>
                                <td>
                                    " + pecas[i].CodInterno + " - " + pecas[i].DescrProduto + @" <br/>
                                </td>
                            </tr>
                            ";
                        }
                        else
                        {

                            retorno += @"
                    <tr>
                        <td>
                            " + pecas[i].CodInterno + " - " + pecas[i].DescrProduto + @" <br/>
                            <input id=""txtMateriaPrima_" + count + @""" type=""text"" style=""font-size:X-Large;width:227px;"" " + (!string.IsNullOrEmpty(etiquetaMostrar) ? "value=\"" + etiquetaMostrar + "\"" : "") + @"
                                onkeypress=""if (isEnter(event)) return validaMateriaPrimaLaminado(this) && "
                                                                     + (i + 1 == pecas.Count && j + 1 == pecas[i].Qtde ?
                                                                     "atualizaSituacao(null, false);" :
                                                                     "setFocus('txtMateriaPrima_" + (count + 1) + "');") + @"""/>
                        </td>
                    </tr>
                    ";
                        }
                        count++;
                    }
                }
                retorno += retornoNaoLer;

            }
            else
            {
                var prodBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(idProd, ProdutoBaixaEstoqueDAO.TipoBuscaProduto.ApenasProducao);

                if (prodBaixaEstoque.Length == 0)
                    return "";

                var count = 1;

                for (var i = 0; i < prodBaixaEstoque.Length; i++)
                {
                    for (var j = 0; j < prodBaixaEstoque[i].Qtde; j++)
                    {
                        var codInterno = ProdutoDAO.Instance.GetCodInterno(prodBaixaEstoque[i].IdProdBaixa);
                        var desc = ProdutoDAO.Instance.GetDescrProduto(prodBaixaEstoque[i].IdProdBaixa);

                        retorno += @"
                    <tr>
                        <td>
                            " + codInterno + " - " + desc + @" <br/>
                            <input id=""txtMateriaPrima_" + count + @""" type=""text"" style=""font-size:X-Large;width:227px;""
                                onkeypress=""if (isEnter(event)) return validaMateriaPrimaLaminado(this) && "
                                                              + (i + 1 == prodBaixaEstoque.Length && j + 1 == prodBaixaEstoque[i].Qtde ?
                                                              "atualizaSituacao(null, false);" :
                                                              "setFocus('txtMateriaPrima_" + (count + 1) + "');") + @"""/>
                        </td>
                    </tr>
                    ";

                        count++;
                    }
                }
            }

            retorno += "</table>";

            if (idProdPedProducaoParent.GetValueOrDefault(0) > 0)
                retorno += "$$" + etiqueta;

            return retorno;
        }

        [Ajax.AjaxMethod]
        public void ValidaMateriaPrimaLaminado(string etiqueta)
        {
            //Busca o tipo de etiqueta
            var tipoEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etiqueta);

            if (tipoEtiquetaChapa == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
            {
                if (ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParentByEtiqueta(null, etiqueta).GetValueOrDefault(0) == 0)
                    throw new Exception("A etiqueta: " + etiqueta + "não esta vinculada a nenhum produto do subgrupo Vidro Duplo ou Laminado.");

                //Verifica se a etiqueta já foi utilizada
                if (ChapaCortePecaDAO.Instance.ValidarChapa(null, ProdutoDAO.Instance.GetByIdProd(ProdutosNfDAO.Instance.GetIdProdByEtiquetaAtiva(null, etiqueta)))
                    && ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(etiqueta, tipoEtiquetaChapa)))
                    throw new Exception("A matéria-prima: " + etiqueta + " já foi utilizada.");

                return;
            }

            //Verifica se a etiqueta é de NF
            else if (tipoEtiquetaChapa != ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal)
                throw new Exception("Apenas etiquetas de nota fiscal podem ser usadas como matéria-prima");

            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(etiqueta, tipoEtiquetaChapa);
            var produto = ProdutoDAO.Instance.GetByIdProd(ProdutosNfDAO.Instance.GetIdProdByEtiquetaAtiva(null, etiqueta));

            //Verifica se a etiqueta já foi utilizada
            if (ChapaCortePecaDAO.Instance.ValidarChapa(null, produto) && ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(idProdImpressaoChapa))
                throw new Exception("A matéria-prima: " + etiqueta + " já foi utilizada.");
        }

        /// <summary>
        /// Verifica se a peça esta parada na producao
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string PecaParadaProducao(string etiqueta)
        {
            var retorno = new List<string>();
            var listaEtiquetas = new List<string>();

            if (etiqueta.Contains('='))
            {
                var posicao = Conversoes.StrParaInt(etiqueta.Split('-')[1].Split('/')[0].Split('.')[0]);

                var intervaloEtiquetas = etiqueta.Split('/')[1].Split('=');
                var inicioIntervalo = Conversoes.StrParaInt(intervaloEtiquetas[0]);
                var fimIntervalo = Conversoes.StrParaInt(intervaloEtiquetas[1]);

                var produtosImpressao = ProdutoImpressaoDAO.Instance.GetByIdPedido(posicao, Conversoes.StrParaInt(etiqueta.Split('-')[0])).OrderBy(f => f.ItemEtiqueta);
                var etiquetas = new List<string>();

                if (inicioIntervalo > fimIntervalo)
                    throw new Exception("item inicial maior que item final.");

                if (fimIntervalo > produtosImpressao.Count())
                    throw new Exception("Número de itens deve maior que a quantidade existente na posição.");

                foreach (var item in produtosImpressao)
                {
                    if (item.ItemEtiqueta >= inicioIntervalo && item.ItemEtiqueta <= fimIntervalo)
                        listaEtiquetas.Add(item.IdPedido.ToString() + "-" + posicao + "." + item.ItemEtiqueta + "/" + item.QtdeProd);
                }

                foreach (var etiq in listaEtiquetas)
                {
                    var pecaParada = ProdutoPedidoProducaoDAO.Instance.VerificaPecaProducaoParada(etiq);

                    if (pecaParada.Split(';')[0] == "true")
                        return pecaParada;
                }
            }
            else
                return ProdutoPedidoProducaoDAO.Instance.VerificaPecaProducaoParada(etiqueta);

            return "false;";
        }

        /// <summary>
        /// Retorna a empresa que está utilizando o sistema.
        /// </summary>
        [Ajax.AjaxMethod]
        public string RedirecionarPainelProducao()
        {
            return ProducaoConfig.TelaMarcacaoPeca.ExibirPainelSetores.ToString().ToLower();
        }

        /// <summary>
        /// Verifica se a etiqueta informado foi impressa
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string PodeImprimir(string etiqueta, string etiquetasMateriaPrima)
        {
            etiquetasMateriaPrima = etiquetasMateriaPrima.TrimEnd(',');

            var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(null, etiqueta);
            var idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(null, etiqueta);
            var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(idProdPed);

            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);

            var isProdutoLaminadoComposicao = tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;

            if (!isProdutoLaminadoComposicao)
                return "Erro|Neste setor, são permitidas leituras somente em etiquetas de peças associadas ao grupo do tipo Vidro Duplo ou do tipo Vidro Laminado.";

            if (ProdutoImpressaoDAO.Instance.EstaImpressa(etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                return "Erro|";

            /* Chamado 55897.
             * Desconsidera a quantidade de peças pai impressas na quantidade disponível de peças pai para a impressão e, além disso,
             * verifica se a etiqueta informada possui filho pendente, para imprimir a etiqueta todos os filhos devem estar prontos. */
            if (ProdutosPedidoEspelhoDAO.Instance.ObterQtdePecasParaImpressaoComposicao((int)idProdPed) - ProdutosPedidoEspelhoDAO.Instance.ObterQtdeImpresso(idProdPed) <= 0)
                return "Erro|Uma ou mais peças da composição não estão prontas, verifique a produção dos produtos filhos da etiqueta informada.";

            if (!string.IsNullOrWhiteSpace(etiquetasMateriaPrima.TrimEnd(',')) && !ProdutoPedidoProducaoDAO.Instance.ValidarComposicao(null, etiqueta, etiquetasMateriaPrima))
                return $"Erro|A combinação das etiquetas, {etiquetasMateriaPrima} não compõe a estrutura do pai {etiqueta}.";

            return "Ok";
        }

        [Ajax.AjaxMethod()]
        public string VoltarPeca(string idProdPedProducao)
        {
            try
            {
                ProdutoPedidoProducaoDAO.Instance.VoltarPeca(idProdPedProducao.StrParaUint(), null, true);
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        [Ajax.AjaxMethod()]
        public string NovaFornada(string idSetor, string idFunc)
        {
            return FornadaDAO.Instance.NovaFornada(idSetor.StrParaUint(), idFunc.StrParaUint()).ToString();
        }

        #endregion

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void imgPesq_Click(object sender, EventArgs e)
        {

        }
    }
}
