using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class ReajusteEstoqueMinimo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblFiltros.Text = "<b>FILTROS</b>";
    
            lblFiltros.Text += String.IsNullOrEmpty(Request["idLoja"]) ? "" : "<br /><b>Loja: </b>" + LojaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idLoja"]));
            lblFiltros.Text += String.IsNullOrEmpty(Request["codInterno"]) ? "" : "<br /><b>Cód. Interno: </b>" + Request["codInterno"];
            lblFiltros.Text += String.IsNullOrEmpty(Request["descricao"]) ? "" : "<br /><b>Descrição: </b>" + Request["descricao"];
            lblFiltros.Text += String.IsNullOrEmpty(Request["idGrupo"]) ? "" : "<br /><b>Grupo: </b>" + (Request["idGrupo"] == "0" ? "Todos" : GrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idGrupo"])));
            lblFiltros.Text += String.IsNullOrEmpty(Request["idSubgrupo"]) ? "" : "<br /><b>Subgrupo: </b>" + (Request["idSubgrupo"] == "0" ? "Todos" : SubgrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(Request["idSubgrupo"])));
            lblFiltros.Text += String.IsNullOrEmpty(Request["idCorVidro"]) ? "" : "<br /><b>Cor do Vidro: </b>" + CorVidroDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCorVidro"]));
            lblFiltros.Text += String.IsNullOrEmpty(Request["idCorFerragem"]) ? "" : "<br /><b>Cor da Ferragem: </b>" + CorFerragemDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCorFerragem"]));
            lblFiltros.Text += String.IsNullOrEmpty(Request["idCorAluminio"]) ? "" : "<br /><b>Cor do Alumínio: </b>" + CorAluminioDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCorAluminio"]));
            lblFiltros.Text += String.IsNullOrEmpty(Request["tipoBox"]) ? "" : "<br /><b>Tipo Box: </b>" + Request["tipoBox"];
            lblFiltros.Text += String.IsNullOrEmpty(Request["abaixoEstMin"]) ? "" : "<br /><b>Abaixo do estoque mínimo: </b>" + (bool.Parse(Request["abaixoEstMin"]) ? "Sim" : "Não");
        }
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            //para evitar valores inválidos como "%-%2-2"
            float reajuste;
            float.TryParse(txtReajuste.Text, out reajuste);
    
            if (reajuste == 0.0)
            {
                return;
            }
            else
            {
                try
                {
                    txtResultados.Text = "Taxa de reajuste: " + txtReajuste.Text + (rdbPorcentagem.Checked ? "%" : " (qtd)") + "\nRESULTADOS:";
    
                    uint idLoja = String.IsNullOrEmpty(Request["idLoja"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idLoja"]);
                    string codInterno = Request["codInterno"];
                    string descricao = Request["descricao"];
                    uint idGrupo = String.IsNullOrEmpty(Request["idGrupo"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idGrupo"]);
                    uint idSubgrupo = String.IsNullOrEmpty(Request["idSubgrupo"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idSubgrupo"]);
                    uint idCorVidro = String.IsNullOrEmpty(Request["idCorVidro"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idCorVidro"]);
                    uint idCorFerragem = String.IsNullOrEmpty(Request["idCorFerragem"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idCorFerragem"]);
                    uint idCorAluminio = String.IsNullOrEmpty(Request["idCorAluminio"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idCorAluminio"]);
                    string tipoBox = Request["tipoBox"];
                    bool abaixoEstMin = bool.Parse(Request["abaixoEstMin"]);
    
                    var produtos = ProdutoLojaDAO.Instance.GetForEstoqueMin(idLoja, codInterno, descricao, idGrupo, idSubgrupo, abaixoEstMin, idCorVidro, idCorFerragem, idCorAluminio, tipoBox, null, 0, 0);
    
                    int count = 0;
                    foreach (ProdutoLoja pl in produtos)
                    {
                        int novoEstMin = rdbPorcentagem.Checked ? (int)(pl.EstMinimo * ((reajuste / 100) + 1)) : (int)(pl.EstMinimo + reajuste);

                        //CHAMADO 37830: essa verificação foi inserida para que não hajam produtos com estoque mínimo negativo.
                        if (novoEstMin < 0)
                            continue;

                        txtResultados.Text += "\n\n" + pl.DescrProduto + "\n\t| Anterior: " + pl.EstMinimo + "\t| Atual: " + novoEstMin;

                        pl.EstMinimo = novoEstMin;
                        ProdutoLojaDAO.Instance.AtualizaEstoqueMinimo(pl);
                        count++;
                    }
    
                    txtReajuste.Text = "";
    
                    lblSucesso.Text = count + " produtos alterados com sucesso.";
                }
                catch (Exception ex)
                {
                    lblSucesso.Text = "Erro ao alterar um ou mais produtos.<br/>" + ex.Message;
                }
    
                txtResultados.Visible = true;

                ClientScript.RegisterClientScriptBlock(typeof(string), "AtualizarTela", "window.opener.redirectUrl(window.opener.location.href);", true);
            }    
        }
    }
}
