using Colosoft.Business;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadIestUfLoja : System.Web.UI.Page
    {
        private uint _idLoja;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdIestUfLoja.Register(true, true);
            odsIestUfLoja.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _idLoja = Conversoes.StrParaUint(Request["IdLoja"]);
        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if(_idLoja == 0)
                    Glass.MensagemAlerta.ErrorMsg("Loja não informada.", new SaveResult());

                // Recupera os dados
                var nomeUF = ((DropDownList)grdIestUfLoja.FooterRow.FindControl("drpNomeUf")).SelectedItem.Text;
                var inscEstSt = ((TextBox)grdIestUfLoja.FooterRow.FindControl("txtInscEstSt")).Text;

                /// Cria um novo IEST
                var iestUfLoja = new Glass.Fiscal.Negocios.Entidades.IestUfLoja();
                iestUfLoja.IdLoja = _idLoja;
                iestUfLoja.NomeUf = nomeUF;
                iestUfLoja.InscEstSt = inscEstSt;

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Fiscal.Negocios.IIestUfLojaFluxo>();
                // Salva novo IEST
                var resultado = fluxo.SalvarIestUfLoja(iestUfLoja);
                if (resultado)
                    grdIestUfLoja.DataBind();
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir IEST.", resultado);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir IEST.", ex, Page);
            }
        }
    }
}