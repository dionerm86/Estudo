using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadPosicaoPecaModelo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfIdProjetoModelo.Value = Request["idProjetoModelo"];
            hdfIdPecaProjMod.Value = Request["idPecaProjMod"];
            hdfItem.Value = Request["item"];
            hdfTipo.Value = Request["tipo"];
            
            if (!IsPostBack)
            {
                if (hdfTipo.Value != "pecaIndividual")
                {
                    // Busca o modelo de projeto passado por querystring
                    ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdProjetoModelo"]));
    
                    Page.Title += " " + projMod.Codigo;
    
                    // Busca as peças com seus respectivos posicionamentos
                    List<PosicaoPecaModelo> lstPosicao = PosicaoPecaModeloDAO.Instance.GetPosicoes(projMod.IdProjetoModelo);
    
                    // Busca a quantidade de peças deste modelo
                    txtQtdInfo.Text = lstPosicao.Count.ToString();
    
                    // Busca valores coletados anteriormente
                    for (int i = 0; i < lstPosicao.Count; i++)
                    {
                        if (lstPosicao[i] == null)
                            continue;
    
                        ((TextBox)info.FindControl("txtCoordX" + (i + 1))).Text = lstPosicao[i].CoordX.ToString();
                        ((TextBox)info.FindControl("txtCoordY" + (i + 1))).Text = lstPosicao[i].CoordY.ToString();
                        ((DropDownList)info.FindControl("drpOrientacao" + (i + 1))).Text = lstPosicao[i].Orientacao.ToString();
                        ((TextBox)info.FindControl("txtCalc" + (i + 1))).Text = lstPosicao[i].Calc;
    
                        ((Glass.UI.Web.Controls.ctrlLogPopup)info.FindControl("ctrlLogPopup" + (i + 1))).Tabela = LogAlteracao.TabelaAlteracao.PosicaoPecaModelo;
                        ((Glass.UI.Web.Controls.ctrlLogPopup)info.FindControl("ctrlLogPopup" + (i + 1))).IdRegistro = (uint?)lstPosicao[i].IdPosicaoPecaModelo;
                    }
    
                    // Carrega imagem na tela
                    imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" +
                        Data.Helper.Utils.GetModelosProjetoPath + projMod.NomeFiguraAssociada;
                }
                else
                {
                    PecaProjetoModelo peca = PecaProjetoModeloDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idPecaProjMod"]));

                    /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                    var codigoProjetoModelo = ProjetoModeloDAO.Instance.ObtemCodigo(Convert.ToUInt32(hdfIdProjetoModelo.Value));
                    string path = Data.Helper.Utils.GetModelosProjetoPath + codigoProjetoModelo + "§" + hdfItem.Value + ".jpg";

                    if (!File.Exists(path))
                        path = Data.Helper.Utils.GetModelosProjetoPath + hdfIdProjetoModelo.Value.PadLeft(3, '0') + "_" + hdfItem.Value + ".jpg";
    
                    if (File.Exists(path))
                    {
                        // Busca as peças com seus respectivos posicionamentos
                        List<PosicaoPecaIndividual> lstPosicao = PosicaoPecaIndividualDAO.Instance.GetPosicoes(Glass.Conversoes.StrParaUint(Request["idPecaProjMod"]),
                            Glass.Conversoes.StrParaInt(Request["item"]));
    
                        // Busca a quantidade de peças deste modelo
                        txtQtdInfo.Text = lstPosicao.Count.ToString();
    
                        // Busca valores coletados anteriormente
                        for (int i = 0; i < lstPosicao.Count; i++)
                        {
                            if (lstPosicao[i] == null)
                                continue;
    
                            ((TextBox)info.FindControl("txtCoordX" + (i + 1))).Text = lstPosicao[i].CoordX.ToString();
                            ((TextBox)info.FindControl("txtCoordY" + (i + 1))).Text = lstPosicao[i].CoordY.ToString();
                            ((DropDownList)info.FindControl("drpOrientacao" + (i + 1))).Text = lstPosicao[i].Orientacao.ToString();
                            ((TextBox)info.FindControl("txtCalc" + (i + 1))).Text = lstPosicao[i].Calc;
    
                            ((Glass.UI.Web.Controls.ctrlLogPopup)info.FindControl("ctrlLogPopup" + (i + 1))).Tabela = LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual;
                            ((Glass.UI.Web.Controls.ctrlLogPopup)info.FindControl("ctrlLogPopup" + (i + 1))).IdRegistro = (uint?)lstPosicao[i].IdPosPecaInd;
                        }
    
                        // Carrega imagem na tela
                        imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoFigura=individual&tipoDesenho=0&path=" + path;
                    }
                }
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadPosicaoPecaModelo));
        }
    
        [Ajax.AjaxMethod()]
        public string SalvarModeloCompleto(string idProjetoModelo, string numPeca, string vetCoord, string vetOrientacao, string vetCalc)
        {
            try
            {
                // Insere/Atualiza valores coletados das posições
                PosicaoPecaModeloDAO.Instance.AtualizaValores(Glass.Conversoes.StrParaUint(idProjetoModelo), Glass.Conversoes.StrParaInt(numPeca), vetCoord.TrimStart('|').Split('|'),
                    vetOrientacao.TrimStart('|').Split('|'), vetCalc.TrimStart('|').Split('|'));
    
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar posicionamentos.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string SalvarModeloIndividual(string idPecaProjMod, string item, string numPeca, string vetCoord, string vetOrientacao, string vetCalc)
        {
            try
            {
                // Insere/Atualiza valores coletados das posições
                PosicaoPecaIndividualDAO.Instance.AtualizaValores(Glass.Conversoes.StrParaUint(idPecaProjMod), Glass.Conversoes.StrParaInt(item), Glass.Conversoes.StrParaInt(numPeca), 
                    vetCoord.TrimStart('|').Split('|'), vetOrientacao.TrimStart('|').Split('|'), vetCalc.TrimStart('|').Split('|'));
    
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar posicionamentos.", ex);
            }
        }
    
        protected void btnInserirImagem_Click(object sender, EventArgs e)
        {
            if (!fluPecaIndividual.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Busque a imagem ante de inserí-la.", Page);
                return;
            }
    
            if (!fluPecaIndividual.FileName.Contains(".jpg"))
            {
                Glass.MensagemAlerta.ShowMsg("Apenas imagens no formato jpg são aceitas.", Page);
                return;
            }
    
            try
            {
                /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                string path = Data.Helper.Utils.GetModelosProjetoPath + hdfIdProjetoModelo.Value.PadLeft(3, '0') +
                    (!String.IsNullOrEmpty(hdfItem.Value) ? ("_" + hdfItem.Value) : String.Empty) + ".jpg";
                if (File.Exists(path))
                    File.Delete(path);
                // Carrega a imagem individual ou a completa, dependendo de qual imagem estiver sendo editada
                var codigoProjetoModelo = ProjetoModeloDAO.Instance.ObtemCodigo(Convert.ToUInt32(hdfIdProjetoModelo.Value));
                path = Data.Helper.Utils.GetModelosProjetoPath + codigoProjetoModelo +
                    (!String.IsNullOrEmpty(hdfItem.Value) ? ("§" + hdfItem.Value) : "§E") + ".jpg";
                if (File.Exists(path))
                    File.Delete(path);

                fluPecaIndividual.SaveAs(path);

                // Se for imagem de peça projeto modelo
                if (!String.IsNullOrEmpty(hdfItem.Value))
                {
                    // Carrega imagem na tela
                    imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoFigura=individual&tipoDesenho=0&path=" + path;
                }
                // Se for imagem de engenharia
                else
                {
                    // Salva o nome da figura associada deste modelo
                    uint idProjetoModelo = Glass.Conversoes.StrParaUint(Request["idProjetoModelo"]);
                    string nomeFiguraAssociada = codigoProjetoModelo + "§E.jpg";
                    ProjetoModeloDAO.Instance.SalvaNomeFiguraAssociada(idProjetoModelo, nomeFiguraAssociada);
    
                    // Carrega imagem na tela
                    imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + path;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir imagem.", ex, Page);
            }
        }
    }
}
