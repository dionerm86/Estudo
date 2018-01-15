using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelExpressao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SelExpressao));

            if (!IsPostBack)
            {
                string variaveis = String.Empty;
                string peca = String.Empty;
                string formulas = string.Empty;
                string folgas = String.Empty;
                string variaveisUltVlrInfos = String.Empty;
                var idProjetoModelo = Glass.Conversoes.StrParaUint(Request["idProjetoModelo"]);

                //Verifica se a express�o � para uma pe�a do modelo de projeto
                //e se o item � 99 para mostrar as op��es de item da etiqueta.
                if (!string.IsNullOrEmpty(Request["idPecaProjMod"]) &&
                    !string.IsNullOrEmpty(Request["item"]) &&
                    (Request["item"] == "90" || Request["item"] == "91" || Request["item"] == "92" || Request["item"] == "93" || Request["item"] == "94" ||
                    Request["item"] == "95" || Request["item"] == "96" || Request["item"] == "97" || Request["item"] == "98" || Request["item"] == "99"))
                {
                    itensFixos.InnerHtml += @", <a href='#' onclick='setValue(this);'>IETQ</a> Item da Etiqueta";
                    itensFixos.InnerHtml += @", <a href='#' onclick='setValue(this);'>REIKI</a> REIKI";
                }

                if (idProjetoModelo > 0)
                {
                    // Mostra na tela quais vari�veis podem ser utilizadas na express�o
                    List<MedidaProjetoModelo> lstMedida = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(idProjetoModelo, false);
                    foreach (MedidaProjetoModelo mpm in lstMedida)
                        variaveis += "<a href='#' onclick='setValue(this);'>" + mpm.CalcTipoMedida.ToUpper() + "</a>, ";

                    // Busca a altura e largura das pe�as que podem ser utilizadas
                    List<PecaProjetoModelo> lstPeca = PecaProjetoModeloDAO.Instance.GetByModelo(idProjetoModelo);
                    foreach (PecaProjetoModelo ppm in lstPeca)
                        peca += String.Format("{0}{1}",
                            "Alt. Pe�a " + ppm.Item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + ppm.Item.Replace(" ", "").ToUpper() + "ALT</a>, " +
                            "Larg. Pe�a " + ppm.Item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + ppm.Item.Replace(" ", "").ToUpper() + "LARG</a>, ",
                            (Request["tipo"] != null && Request["tipo"] == "validacao" ?
                                "Esp. Pe�a " + ppm.Item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + ppm.Item.Replace(" ", "").ToUpper() + "ESP</a>, " :
                                ""));

                    foreach (PecaProjetoModelo ppm in lstPeca)
                        folgas += "Folga Alt. " + ppm.Item.ToUpper() + ": <a href='#' onclick='setValue(this);'>FOLGA" + ppm.Item.Replace(" ", "").ToUpper() + "ALT</a>, " +
                            "Folga Larg. " + ppm.Item.ToUpper() + ": <a href='#' onclick='setValue(this);'>FOLGA" + ppm.Item.Replace(" ", "").ToUpper() + "LARG</a>, ";
                }
                else
                {
                    IList<MedidaProjeto> lstMedida = MedidaProjetoDAO.Instance.GetMedidas();
                    foreach(var mp in lstMedida)
                        variaveis += "<a href='#' onclick='setValue(this);'>" + mp.DescricaoTratada.ToUpper() + "</a>, ";

                    var listItemPeca = PecaProjetoModeloDAO.Instance.GetDistinctItemPecaProjetoModelo();
                    foreach(var item in listItemPeca)
                        peca += String.Format("{0}{1}",
                            "Alt. Pe�a " + item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + item.Replace(" ", "").ToUpper() + "ALT</a>, " +
                            "Larg. Pe�a " + item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + item.Replace(" ", "").ToUpper() + "LARG</a>, ",
                            (Request["tipo"] != null && Request["tipo"] == "validacao" ?
                                "Esp. Pe�a " + item.ToUpper() + ": <a href='#' onclick='setValue(this);'>P" + item.Replace(" ", "").ToUpper() + "ESP</a>, " :
                                ""));

                    foreach (var item in listItemPeca)
                        folgas += "Folga Alt. " + item.ToUpper() + ": <a href='#' onclick='setValue(this);'>FOLGA" + item.Replace(" ", "").ToUpper() + "ALT</a>, " +
                            "Folga Larg. " + item.ToUpper() + ": <a href='#' onclick='setValue(this);'>FOLGA" + item.Replace(" ", "").ToUpper() + "LARG</a>, ";
                }

                var listaFormulas = FormulaExpressaoCalculoDAO.Instance.GetAll();
                foreach (FormulaExpressaoCalculo fec in listaFormulas)
                    if (fec.Expressao != null && fec.Expressao != "")
                        formulas += "Formula: " + fec.Descricao.ToUpper() + ": <a href='#' onclick='setValue(this);'>" + fec.Descricao.Replace(" ", "").ToUpper() + "</a>, ";

                lblVariaveis.Text = variaveis.TrimEnd(' ').TrimEnd(',');
                lblMedidasPecas.Text = peca.TrimEnd(' ').TrimEnd(',');

                if (!string.IsNullOrEmpty(formulas))
                    lblFormulaExpressaoCalculo.Text = formulas.TrimEnd(' ').TrimEnd(',');

                if (!string.IsNullOrEmpty(folgas))
                    lblFolgasPecas.Text = folgas.TrimEnd(' ').TrimEnd(',');
    
                txtExpressao.Text = Request["expr"].Replace("@", "+");
            }
        }

        #region M�todos Ajax
        
        [Ajax.AjaxMethod]
        public string ValidarExpressao(string idProjetoModelo, string idFormulaExpreCalc, string expressao)
        {
            try
            {
                if (idProjetoModelo.StrParaIntNullable().GetValueOrDefault() == 0 && idFormulaExpreCalc.StrParaIntNullable().GetValueOrDefault() == 0)
                    return "Erro|N�o foi poss�vel recuperar o projeto, ou a f�rmula de express�o e validar a express�o de c�lculo.";

                if (string.IsNullOrEmpty(expressao))
                    return "Erro|Express�o de c�lculo n�o informada.";

                var expressaoValida = Data.Helper.UtilsProjeto.ValidarExpressao(idProjetoModelo.StrParaInt(), expressao);

                return expressaoValida ? "Ok|" : "Erro|Express�o de c�lculo inv�lida. Verifique o nome dos par�metros e a express�o de c�lculo.";
            }
            catch (Exception ex)
            {
                return "Erro|Express�o de c�lculo inv�lida. Verifique o nome dos par�metros e a express�o de c�lculo. " + MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        #endregion
    }
}
