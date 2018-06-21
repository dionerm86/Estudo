using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlConfigFaixasRentabilidadeLiberacao : System.Web.UI.UserControl
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador da loja
        /// para o qual serão filtradas as faixas.
        /// </summary>
        public int IdLoja
        {
            get
            {
                int value;
                if (int.TryParse(hdfIdLojaFaixaRentabilidadeLiberacao.Value, out value))
                    return value;

                return 0;
            }
            set
            {
                hdfIdLojaFaixaRentabilidadeLiberacao.Value = value.ToString();
            }
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFaixasRentabilidadeLiberacao.Register(true, true);
            odsFaixaRentabilidadeLiberacao.Register();

            odsFaixaRentabilidadeLiberacao.Updating += OdsFaixaRentabilidadeLiberacao_Updating;
            odsFaixaRentabilidadeLiberacao.Updated += odsFaixaRentabilidadeLiberacao_Updated;
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var rentabilidadeFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Rentabilidade.Negocios.IRentabilidadeFluxo>();

                var faixaRentabilidadeLiberacao = rentabilidadeFluxo.CriarFaixaRentabilidadeLiberacao();

                decimal percentualRentabilidade;

                if (!decimal.TryParse(((TextBox)grdFaixasRentabilidadeLiberacao.FooterRow.FindControl("txtPercentualRentabilidade")).Text,
                        System.Globalization.NumberStyles.Number, Glass.Globalizacao.Cultura.CulturaSistema, out percentualRentabilidade))
                {
                    Glass.MensagemAlerta.ShowMsg("Os valores informados não são numéricos válidos", Page);
                    return;
                }

                var idsFuncionario = ((Sync.Controls.CheckBoxListDropDown)grdFaixasRentabilidadeLiberacao.FooterRow.FindControl("drpFuncionariosFaixasRentabilidadeLiberacao"))?.SelectedValues ?? new int[0];
                var idsTipoFuncionario = ((Sync.Controls.CheckBoxListDropDown)grdFaixasRentabilidadeLiberacao.FooterRow.FindControl("drpTiposFuncionarioFaixasRentabilidadeLiberacao"))?.SelectedValues ?? new int[0];

                var requerLiberacao = ((CheckBox)grdFaixasRentabilidadeLiberacao.FooterRow.FindControl("chkRequerLiberacao")).Checked;

                faixaRentabilidadeLiberacao.IdLoja = IdLoja;
                faixaRentabilidadeLiberacao.PercentualRentabilidade = percentualRentabilidade;
                faixaRentabilidadeLiberacao.RequerLiberacao = requerLiberacao;

                foreach (var idFunc in idsFuncionario.Distinct())
                    faixaRentabilidadeLiberacao.FuncionariosFaixa.Add(new Rentabilidade.Negocios.Entidades.FuncionarioFaixaRentabilidadeLiberacao
                    {
                        IdFunc = idFunc
                    });

                foreach (var idTipoFuncionario in idsTipoFuncionario.Distinct())
                    faixaRentabilidadeLiberacao.TiposFuncionarioFaixa.Add(new Rentabilidade.Negocios.Entidades.TipoFuncionarioFaixaRentabilidadeLiberacao
                    {
                        IdTipoFuncionario = idTipoFuncionario
                    });

                var resultado = rentabilidadeFluxo.SalvarFaixaRentabilidadeLiberacao(faixaRentabilidadeLiberacao);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a faixa de rentabilidade para liberação.", resultado);
                else
                    grdFaixasRentabilidadeLiberacao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a faixa de rentabilidade para liberação.", ex, Page);
            }
        }

        #endregion

        #region Métodos Privados

        private void OdsFaixaRentabilidadeLiberacao_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var faixa = e.InputParameters[0] as Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao;

            ProcessaFuncionariosFaixa(faixa);
            ProcessaTiposFuncionarioFaixa(faixa);
        }

        private void ProcessaFuncionariosFaixa(Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao faixa)
        {
            var idsFuncionario = ((Sync.Controls.CheckBoxListDropDown)grdFaixasRentabilidadeLiberacao.Rows[grdFaixasRentabilidadeLiberacao.EditIndex].FindControl("drpFuncionarios")).SelectedValues ?? new int[0];

            var processados = faixa.FuncionariosFaixa.Where(f => idsFuncionario.Contains(f.IdFunc)).ToList();
            var removidos = faixa.FuncionariosFaixa.Where(f => !processados.Contains(f)).ToList();
            var novos = idsFuncionario.Where(f => !processados.Any(x => x.IdFunc == f))
                .Select(f => new Glass.Rentabilidade.Negocios.Entidades.FuncionarioFaixaRentabilidadeLiberacao
                {
                    IdFunc = f
                }).ToList();

            foreach (var i in removidos)
                faixa.FuncionariosFaixa.Remove(i);

            foreach (var i in novos)
                faixa.FuncionariosFaixa.Add(i);
        }

        private void ProcessaTiposFuncionarioFaixa(Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao faixa)
        {
            var idTiposFuncionario = ((Sync.Controls.CheckBoxListDropDown)grdFaixasRentabilidadeLiberacao.Rows[grdFaixasRentabilidadeLiberacao.EditIndex].FindControl("drpTiposFuncionario")).SelectedValues ?? new int[0];

            var processados = faixa.TiposFuncionarioFaixa.Where(f => idTiposFuncionario.Contains(f.IdTipoFuncionario)).ToList();
            var removidos = faixa.TiposFuncionarioFaixa.Where(f => !processados.Contains(f)).ToList();
            var novos = idTiposFuncionario.Where(f => !processados.Any(x => x.IdTipoFuncionario == f))
                .Select(f => new Glass.Rentabilidade.Negocios.Entidades.TipoFuncionarioFaixaRentabilidadeLiberacao
                {
                    IdTipoFuncionario = f
                }).ToList();

            foreach (var i in removidos)
                faixa.TiposFuncionarioFaixa.Remove(i);

            foreach (var i in novos)
                faixa.TiposFuncionarioFaixa.Add(i);
        }

        private void odsFaixaRentabilidadeLiberacao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao configurar a faixa de rentabilidade para liberação.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #endregion
    }
}