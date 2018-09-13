using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Projeto
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(false, "false");
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            // Verifica qual relat�rio ser� chamado
            switch (Request["rel"])
            {
                case "imagemProjeto":
                    report.ReportPath = NomeArquivoRelatorioImagemProjeto;
    
                    // Identifica se foi selecionado para imprimir alum�nios e ferragens.
                    bool imprAlumFerr = Request["imprAlumFerr"] == "true";
    
                    uint idProjeto = !String.IsNullOrEmpty(Request["idProjeto"]) ? Glass.Conversoes.StrParaUint(Request["idProjeto"]) : 0;
                    var projeto = idProjeto > 0 ? ProjetoDAO.Instance.GetElement(idProjeto) : new Data.Model.Projeto();
                    ItemProjeto[] itemProjeto = ItemProjetoDAO.Instance.GetByString(Request["idItemProjeto"]);
                    List<PecaItemProjeto> lstPeca = new List<PecaItemProjeto>();
                    List<MaterialItemProjeto> lstMaterial = new List<MaterialItemProjeto>();
                    List<Imagem> lstImagens = new List<Imagem>();
                    List<MedidaItemProjeto> lstMedidas = new List<MedidaItemProjeto>();
    
                    // Busca o pedido, se houver, relacionado � este projeto
                    projeto.IdPedido = ProjetoDAO.Instance.GetIdPedidoByProjeto(idProjeto);
                    if (projeto.IdPedido == 0)
                    {
                        // Tenta buscar o pedido pelo item projeto do or�amento
                        uint idPedido = itemProjeto[0].IdPedido > 0 ? itemProjeto[0].IdPedido.Value :
                            itemProjeto[0].IdPedidoEspelho > 0 ? itemProjeto[0].IdPedidoEspelho.Value :
                            itemProjeto[0].IdOrcamento > 0 ? PedidoDAO.Instance.GetIdPedidoByOrcamento(itemProjeto[0].IdOrcamento.Value) : 0;
    
                        if (idPedido > 0)
                        {
                            projeto.IdPedido = idPedido;
                            projeto.NomeCliente = ClienteDAO.Instance.GetNome(PedidoDAO.Instance.ObtemIdCliente(null, idPedido));
                            projeto.NomeFunc = FuncionarioDAO.Instance.GetNome(PedidoDAO.Instance.ObtemIdFunc(null, idPedido));
                            projeto.DataCad = PedidoDAO.Instance.ObtemDataPedido(null, idPedido);
                            projeto.TipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);
                        }
                    }
    
                    for (int i = 0; i < itemProjeto.Length; i++)
                    {
                        var pcp = itemProjeto[i].IdPedidoEspelho.HasValue;
                        ProjetoModelo modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProjeto[i].IdProjetoModelo);
                        lstPeca.AddRange(PecaItemProjetoDAO.Instance.GetByItemProjetoRpt(itemProjeto[i].IdItemProjeto, itemProjeto[i].IdProjetoModelo, pcp));

                        // Caso a imagem da pe�a tenha sido editada ent�o a impress�o n�o deve exibir se a pe�a possui arquivo de otimiza��o.
                        for (var x = 0; x < lstPeca.Count; x++)
                        {
                            var pecaPossuiFiguraAssociada = PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(null, lstPeca[x].IdPecaItemProj);
                            var pecaPossuiEdicaoCadProject = lstPeca[x].IdProdPed != null ? lstPeca[x].ImagemEditada &&  ProdutosPedidoEspelhoDAO.Instance.PossuiEdicaoCadProject((uint)lstPeca[x].IdProdPed, pcp):false;
                            var produtoPossuiImagemEditada = pcp && ProdutosPedidoEspelhoDAO.Instance.PossuiImagemAssociada(null, (uint)lstPeca[x]?.IdProdPed.GetValueOrDefault());

                            if (lstPeca[x].IdArquivoMesaCorte > 0 && lstPeca[x].TipoArquivoMesaCorte > 0)
                                if ((produtoPossuiImagemEditada || lstPeca[x].ImagemEditada || pecaPossuiFiguraAssociada) &&
                                    lstPeca[x].TipoArquivoMesaCorte != (int)TipoArquivoMesaCorte.FMLBasico && !pecaPossuiEdicaoCadProject)
                                {
                                    lstPeca[x].IdArquivoMesaCorte = null;
                                    lstPeca[x].TipoArquivoMesaCorte = null;
                                }
                        }
    
                        int atual = lstMaterial.Count;
                        lstMaterial.AddRange(MaterialItemProjetoDAO.Instance.GetForRptItemProjeto(itemProjeto[i].IdItemProjeto, false));
                        int numeroMateriais = lstMaterial.Count - atual;
    
                        // Verifica se os materiais do itemProjeto dever�o ser impressos tamb�m
                        itemProjeto[i].MostrarMateriais = imprAlumFerr && numeroMateriais > 0;
    
                        // Pega a imagem do projeto com as medidas j� desenhadas e o modelo da imagem
                        if (modelo.IdGrupoModelo != (uint)UtilsProjeto.GrupoModelo.Outros)
                        {
                            itemProjeto[i].ImagemProjeto = Data.Helper.Utils.GetImageFromRequest(UtilsProjeto.GetFiguraAssociadaUrl(itemProjeto[i].IdItemProjeto, modelo));

                            // Salva a imagem na mem�ria, recupera os bytes e os salva na propriedade ImagemProjetoModelo.
                            // Chamado 50315 - o nome da imagem deve ser recuperado dentro do try para n�o retornar erro quando o mesmo for null.
                            try
                            {
                                var arquivo = System.IO.Path.Combine(Glass.Data.Helper.Utils.ModelosProjetoPath(Context), modelo.NomeFigura);
                                if (System.IO.File.Exists(arquivo))
                                {
                                    using (var image = new Bitmap(arquivo))
                                    using (var ms = new System.IO.MemoryStream())
                                    {
                                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        itemProjeto[i].ImagemProjetoModelo = ms.ToArray();
                                    }
                                }
                            }
                            catch { }
                            
                            itemProjeto[i].ImagemProjetoModelo = Data.Helper.Utils.GetImageFromRequest("../../Handlers/LoadImage.ashx?path=" + Data.Helper.Utils.ModelosProjetoPath(Context) + 
                                modelo.NomeFigura + "&altura=" + modelo.AlturaFigura + "&largura=" + modelo.LarguraFigura);
                        }
    
                        // Copia o idPedidoEspelho para o idPedido
                        if (itemProjeto[i].IdPedido == null)
                            itemProjeto[i].IdPedido = itemProjeto[i].IdPedidoEspelho;
    
                        lstImagens.AddRange(ImagemDAO.Instance.GetPecasAlteradas(itemProjeto[i].IdItemProjeto, 
                            Glass.Configuracoes.ProjetoConfig.RelatorioImagemProjeto.PercentualTamanhoImagemRelatorio));
                        lstMedidas.AddRange(MedidaItemProjetoDAO.Instance.GetListByItemProjeto(itemProjeto[i].IdItemProjeto));
                    }

                    lstParam.Add(new ReportParameter("Pedido_PedCli", projeto.IdPedido > 0 ? PedidoDAO.Instance.ObtemPedCli(null, projeto.IdPedido) : string.Empty));
                    lstParam.Add(new ReportParameter("FastDelivery", PedidoDAO.Instance.IsFastDelivery(null, projeto.IdPedido).ToString()));
                    lstParam.Add(new ReportParameter("ExibirImagemModelo", (true).ToString()));    
                    lstParam.Add(new ReportParameter("CorObs", Glass.Configuracoes.ProjetoConfig.RelatorioImagemProjeto.CorObsNoRelatorio));
                    lstParam.Add(new ReportParameter("TemEdicaoCadProject", "true"));    
                    lstParam.Add(new ReportParameter("ImagensPecasIndividuais", "true"));
                    lstParam.Add(new ReportParameter("AgruparBeneficiamentos", PedidoConfig.RelatorioPedido.AgruparBenefRelatorio.ToString()));
                    
                    report.DataSources.Add(new ReportDataSource("Projeto", new Data.Model.Projeto[] { projeto }));
                    report.DataSources.Add(new ReportDataSource("ItemProjeto", itemProjeto));
                    report.DataSources.Add(new ReportDataSource("PecaItemProjeto", lstPeca.ToArray()));
                    report.DataSources.Add(new ReportDataSource("MaterialItemProjeto", lstMaterial.ToArray()));
                    report.DataSources.Add(new ReportDataSource("Imagem", lstImagens.ToArray()));
                    report.DataSources.Add(new ReportDataSource("MedidaItemProjeto", lstMedidas.ToArray()));
                    break;
    
                case "totaisProjeto":
                    report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Projeto/rptTotaisProjeto{0}.rdlc");
                    bool exibirCusto = UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador ||
                        UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Gerente;
    
                    string titulo = String.Empty;
                    string nomeCliente = String.Empty;
                    string tipoEntrega = String.Empty;
                    float taxaPrazo = 0;
    
                    uint idProjeto_Totais = Glass.Conversoes.StrParaUint(Request["idProjeto"]);
                    uint idOrcamento_Totais = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);
                    uint idPedido_Totais = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                    uint idPedidoEspelho_Totais = Glass.Conversoes.StrParaUint(Request["idPedidoEspelho"]);
    
                    if (idProjeto_Totais > 0)
                    {
                        var projeto_Totais = ProjetoDAO.Instance.GetElementByPrimaryKey(idProjeto_Totais);
                        titulo = "Impress�o Projeto N.� " + idProjeto_Totais;
                        nomeCliente = projeto_Totais.IdCliente > 0 ? ClienteDAO.Instance.GetNome(projeto_Totais.IdCliente.Value) : projeto_Totais.NomeCliente;
                        tipoEntrega = projeto_Totais.DescrTipoEntrega;
                        taxaPrazo = projeto_Totais.TaxaPrazo;
                    }
                    else if (idOrcamento_Totais > 0)
                    {
                        var orcamento_Totais = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrcamento_Totais);
                        titulo = "Impress�o dos Projetos do Or�amento N.� " + idOrcamento_Totais;
                        nomeCliente = orcamento_Totais.IdCliente > 0 ? orcamento_Totais.IdCliente + " - " + ClienteDAO.Instance.GetNome(orcamento_Totais.IdCliente.Value) : orcamento_Totais.NomeCliente;
                        tipoEntrega = orcamento_Totais.DescrTipoEntrega;
                        taxaPrazo = orcamento_Totais.TaxaPrazo;
                    }
                    else if (idPedido_Totais > 0 || idPedidoEspelho_Totais > 0)
                    {
                        Glass.Data.Model.Pedido pedido_Totais = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido_Totais > 0 ? idPedido_Totais : idPedidoEspelho_Totais);
                        titulo = idPedido_Totais > 0 ? "Impress�o dos Projetos do Pedido N.� " + idPedido_Totais :
                            "Impress�o dos Projetos da Confer�ncia do Pedido N.� " + idPedidoEspelho_Totais;
                        nomeCliente = pedido_Totais.IdCli + " - " + ClienteDAO.Instance.GetNome(pedido_Totais.IdCli);
                        tipoEntrega = pedido_Totais.DescrTipoEntrega;
                        taxaPrazo = pedido_Totais.TaxaPrazo;
                    }

                    lstParam.Add(new ReportParameter("Titulo", titulo));
                    lstParam.Add(new ReportParameter("NomeCliente", nomeCliente));
                    lstParam.Add(new ReportParameter("TipoEntrega", tipoEntrega));
                    lstParam.Add(new ReportParameter("TaxaPrazo", taxaPrazo.ToString()));
                    lstParam.Add(new ReportParameter("ExibirCusto", exibirCusto.ToString()));
                    lstParam.Add(new ReportParameter("ExibirJuros", "false"));
                    lstParam.Add(new ReportParameter("ExibirLinhaVermelhaSaida", "true"));
    
                    List<MaterialItemProjeto> lstMaterial_Totais = MaterialItemProjetoDAO.Instance.GetForRptTotaisItens(idProjeto_Totais,
                        idOrcamento_Totais, idPedido_Totais, idPedidoEspelho_Totais);
    
                    report.DataSources.Add(new ReportDataSource("MaterialItemProjeto", lstMaterial_Totais.ToArray()));
                    break;
    
                case "ImpressaoModeloProjeto":
                    report.ReportPath = "Relatorios/Projeto/rptImpressaoModeloProjeto.rdlc";
    
                    ProjetoModelo projModelo = ProjetoModeloDAO.Instance.GetByCodigo(Request["projModeloCod"]);
    
                    if (projModelo.IdGrupoModelo != (uint)UtilsProjeto.GrupoModelo.Outros)
                    {
                        // Salva a imagem na mem�ria, recupera os bytes e os salva na propriedade ImagemProjetoModelo.
                        // Chamado 50315 - o nome da imagem deve ser recuperado dentro do try para n�o retornar erro quando o mesmo for null.
                        try
                        {
                            var arquivo = System.IO.Path.Combine(Glass.Data.Helper.Utils.ModelosProjetoPath(Context), projModelo.NomeFigura);
                            if (System.IO.File.Exists(arquivo))
                            {
                                using (var image = new Bitmap(arquivo))
                                using (var ms = new System.IO.MemoryStream())
                                {
                                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    projModelo.ImagemProjetoModelo = ms.ToArray();
                                }
                            }
                        }
                        catch { }
                    }
    
                    report.DataSources.Add(new ReportDataSource("ProjetoModelo", new ProjetoModelo[] { projModelo }));
    
                    PecaProjetoModelo[] pecasModelo = PecaProjetoModeloDAO.Instance.GetByModelo(projModelo.IdProjetoModelo).ToArray();
                    report.DataSources.Add(new ReportDataSource("PecaProjetoModelo", pecasModelo));
    
                    MaterialProjetoModelo[] materialModelo = MaterialProjetoModeloDAO.Instance.GetByProjetoModelo(projModelo.IdProjetoModelo, null).ToArray();
                    report.DataSources.Add(new ReportDataSource("MaterialProjetoModelo", materialModelo));
    
                    MedidaProjetoModelo[] medidaModelo = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(projModelo.IdProjetoModelo, false).ToArray();
                    report.DataSources.Add(new ReportDataSource("MedidaProjetoModelo", medidaModelo));
    
                    Imagem[] imagensPecasModelo = ImagemDAO.Instance.GetPecasModelo(projModelo.IdProjetoModelo);
                    report.DataSources.Add(new ReportDataSource("Imagem", imagensPecasModelo));
                    break;
    
                case "LstModeloProjeto":
                    report.ReportPath = "Relatorios/Projeto/rptLstModeloProjeto.rdlc";
                    Imagem[] imagensProjModelo = ImagemDAO.Instance.GetProjetosModelo(Request["cod"], Request["desc"],
                        Glass.Conversoes.StrParaUint(Request["grupo"]), Request["situacao"].StrParaInt());
                    report.DataSources.Add(new ReportDataSource("Imagem", imagensProjModelo));
                    break;
            }
    
            // Atribui par�metros ao relat�rio
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }

        /// <summary>
        /// Nome do arquivo do relat�rio de imagem de projeto.
        /// </summary>
        public static string NomeArquivoRelatorioImagemProjeto
        {
            get
            {
                var caminhoRelatorio = string.Format("Relatorios/Projeto/rptImagemProjeto{0}.rdlc", ControleSistema.GetSite().ToString());

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                    return caminhoRelatorio;

                return
                    PedidoConfig.LiberarPedido ? "Relatorios/Projeto/rptImagemProjetoAplProc.rdlc" : "Relatorios/Projeto/rptImagemProjeto.rdlc";
            }
        }
    }
}
