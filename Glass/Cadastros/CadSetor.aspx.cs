using System;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSetor : System.Web.UI.Page
    {
        private GridViewRow _updatingRow;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdSetor.Register(true, true);
            odsSetor.Register();
            odsSetor.Updating += odsSetor_Updating;
            grdSetor.RowUpdating += grdSetor_RowUpdating;
        }

        private void grdSetor_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            _updatingRow = grdSetor.Rows[e.RowIndex];
        }

        private void odsSetor_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var setor = e.InputParameters[0] as PCP.Negocios.Entidades.Setor;

            var ctrl = (_updatingRow.FindControl("ctrlBenefSetor1") as Glass.UI.Web.Controls.ctrlBenefSetor);

            var beneficiamentos = ctrl.Beneficiamentos.Select(f => (int)f).ToList();

            // Percorre os novos beneficiamentos
            foreach (var i in beneficiamentos.Where(f => !setor.SetorBeneficiamentos.Any(x => x.IdBenefConfig == f)))
                setor.SetorBeneficiamentos.Add(
                    new PCP.Negocios.Entidades.SetorBenef
                    {
                        IdBenefConfig = (int)i
                    });

            // Remove o beneficiamentos que foram removidos
            foreach (var i in setor.SetorBeneficiamentos.Where(f => !beneficiamentos.Any(x => x == f.IdBenefConfig)).ToArray())
                setor.SetorBeneficiamentos.Remove(i);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdSetor.Columns[19].Visible = Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor;
            grdSetor.Columns[8].Visible = Glass.Configuracoes.PCPConfig.ControleCavalete;
        }
    
        protected void grdSetor_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdSetor.ShowFooter = e.CommandName != "Edit";
    
            if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                try
                {
                    var fluxo = ServiceLocator.Current.GetInstance<PCP.Negocios.ISetorFluxo>();

                    fluxo.AlterarPosicao(e.CommandArgument.ToString().StrParaInt(), e.CommandName == "Up");
                    grdSetor.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do setor.", ex, Page);
                }
            }
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var setor = new Glass.PCP.Negocios.Entidades.Setor();
                setor.Sigla = ((TextBox)grdSetor.FooterRow.FindControl("txtSigla")).Text;
                setor.Descricao = ((TextBox)grdSetor.FooterRow.FindControl("txtDescricao")).Text;

                setor.Situacao = (Glass.Situacao)Enum.Parse(typeof(Glass.Situacao), ((DropDownList)grdSetor.FooterRow.FindControl("drpSituacao")).SelectedValue);

                Glass.Data.Model.TipoSetor tipoSetor = TipoSetor.Entregue;

                if (Enum.TryParse<Glass.Data.Model.TipoSetor>(((DropDownList)grdSetor.FooterRow.FindControl("drpTipo")).SelectedValue, out tipoSetor))
                    setor.Tipo = tipoSetor;

                setor.Cor = (Glass.Data.Model.CorSetor)Enum.Parse(typeof(Glass.Data.Model.CorSetor), 
                    ((DropDownList)grdSetor.FooterRow.FindControl("drpCor")).SelectedValue);

                setor.CorTela = (Glass.Data.Model.CorTelaSetor)Enum.Parse(typeof(Glass.Data.Model.CorTelaSetor),
                        ((DropDownList)grdSetor.FooterRow.FindControl("drpCorTela")).SelectedValue);

                setor.EntradaEstoque = (((CheckBox)grdSetor.FooterRow.FindControl("chkEntradaEstoque")).Checked);
                setor.ImpedirAvanco = (((CheckBox)grdSetor.FooterRow.FindControl("chkImpedirAvanco")).Checked);
                setor.InformarRota = (((CheckBox)grdSetor.FooterRow.FindControl("chkInformarRota")).Checked);
                setor.InformarCavalete = (((CheckBox)grdSetor.FooterRow.FindControl("chkInformarCavalete")).Checked);
                setor.Corte = (((CheckBox)grdSetor.FooterRow.FindControl("chkCorte")).Checked);
                setor.Forno = (((CheckBox)grdSetor.FooterRow.FindControl("chkForno")).Checked);
                setor.Laminado = (((CheckBox)grdSetor.FooterRow.FindControl("chkLaminado")).Checked);
                setor.ExibirSetores = (((CheckBox)grdSetor.FooterRow.FindControl("chkSetoresLidos")).Checked);
                setor.ExibirImagemCompleta = (((CheckBox)grdSetor.FooterRow.FindControl("chkImagemCompleta")).Checked);
                setor.TempoLogin = Glass.Conversoes.StrParaInt(((TextBox)grdSetor.FooterRow.FindControl("txtTempoLogin")).Text);
                setor.ConsultarAntes = (((CheckBox)grdSetor.FooterRow.FindControl("cboConsultarAntes")).Checked);
                setor.ExibirRelatorio = (((CheckBox)grdSetor.FooterRow.FindControl("chkExibirRelatorio")).Checked);
                setor.IgnorarCapacidadeDiaria = (((CheckBox)grdSetor.FooterRow.FindControl("chkIgnorarCapacidadeDiaria")).Checked);
                setor.PermitirLeituraForaRoteiro = (((CheckBox)grdSetor.FooterRow.FindControl("chkPermitirLeituraForaRoteiro")).Checked);
                setor.ExibirPainelComercial = (((CheckBox)grdSetor.FooterRow.FindControl("chkExibirPainelComercial")).Checked);
                setor.TempoAlertaInatividade = (((TextBox)grdSetor.FooterRow.FindControl("txtTempoInatividade")).Text).StrParaInt();


                if (!String.IsNullOrEmpty(((TextBox)grdSetor.FooterRow.FindControl("txtDesafioPerda")).Text))
                    setor.DesafioPerda = double.Parse(((TextBox)grdSetor.FooterRow.FindControl("txtDesafioPerda")).Text);
                
                if (!String.IsNullOrEmpty(((TextBox)grdSetor.FooterRow.FindControl("txtMetaPerda")).Text))
                    setor.MetaPerda = double.Parse(((TextBox)grdSetor.FooterRow.FindControl("txtMetaPerda")).Text);

                // Percorre os beneficiamentos selecionados
                foreach (var i in (grdSetor.FooterRow.FindControl("ctrlBenefSetor1") as Glass.UI.Web.Controls.ctrlBenefSetor).Beneficiamentos)
                {
                    setor.SetorBeneficiamentos.Add(
                        new PCP.Negocios.Entidades.SetorBenef
                        {
                            IdBenefConfig = (int)i
                        });
                }

                var fluxo = ServiceLocator.Current.GetInstance<PCP.Negocios.ISetorFluxo>();

                var resultado = fluxo.SalvarSetor(setor);

                if (resultado)
                    grdSetor.DataBind();
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Setor.", resultado);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Setor.", ex, Page);
            }
        }
    }
}
