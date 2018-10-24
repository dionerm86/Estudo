using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class SelModelo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (tbModelos.Controls.Count == 0)
            //    BuscaModelos();
            if (!IsPostBack)
                BuscaModelos(true);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            lblMaisUsados.Visible = false;
            BuscaModelos(false);
        }

        protected void BuscaModelos(bool maisUsados)
        {
            // Limpa tabela
            tbModelos.Controls.Clear();
    
            uint idGrupoModelo = !String.IsNullOrEmpty(drpGrupoModelo.SelectedValue) ? Glass.Conversoes.StrParaUint(drpGrupoModelo.SelectedValue) : (uint)GrupoModelo.GrupoModeloEnum.Correr;

            var parceiro = Request["Parceiro"] == "true" ? true : false;

            List<ProjetoModelo> lstModelos = maisUsados ? ProjetoModeloDAO.Instance.ObtemMaisUsadosCliente((uint)OrcamentoDAO.Instance.ObterIdCliente(null, Request["idProjeto"].StrParaInt()), 21) :
               ProjetoModeloDAO.Instance.GetList(txtCodigo.Text, txtDescricao.Text, idGrupoModelo);
            
            Unit largModelo = new Unit("160px");
            Unit altModelo = new Unit("");
    
            TableRow linha = new TableRow();
    
            if (lstModelos.Count > 0)
            {
                int contador = 0;
    
                CorAluminio[] aluminios = odsCorAluminio.Select() as CorAluminio[];
                CorFerragem[] ferragens = odsCorFerragem.Select() as CorFerragem[];
    
                foreach (ProjetoModelo m in lstModelos)
                {
                    // Permite inserir no máximo 3 modelos por linha
                    if (contador == 3)
                    {
                        tbModelos.Controls.Add(linha);
                        linha = new TableRow();
                        contador = 0;
                    }
    
                    contador++;
    
                    TableCell celula = new TableCell();
    
                    CtrlModeloProjeto ctrModeloProj = (CtrlModeloProjeto)LoadControl(ResolveClientUrl("~/Controls/ctrlModeloProjeto.ascx"));
                    ctrModeloProj.Modelo = m;
                    ctrModeloProj.Vidros = CorVidroDAO.Instance.GetForProjeto(m.IdProjetoModelo);
                    ctrModeloProj.Aluminios = aluminios;
                    ctrModeloProj.Ferragens = ferragens;
                    ctrModeloProj.ExibirCorAluminioFerragem = parceiro && !Configuracoes.ProjetoConfig.TelaCadastroParceiros.ExibirCorAluminioFerragemWebGlassParceiros ? false : true;
    
                    celula.Controls.Add(ctrModeloProj);
                    celula.Style.Value = "padding: 2px";
                    celula.Attributes.Add("align", "center");
    
                    linha.Controls.Add(celula);
                }
            }
            else
            {
                TableCell celula = new TableCell();
                celula.Text = "Nenhum modelo encontrado.";
                linha.Controls.Add(celula);
            }
    
            tbModelos.Controls.Add(linha);
        }
    }
}
