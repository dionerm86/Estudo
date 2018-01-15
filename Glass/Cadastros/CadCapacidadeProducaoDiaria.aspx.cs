using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCapacidadeProducaoDiaria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor)
                Response.Redirect("~/WebGlass/Main.aspx", true);
    
            if (!IsPostBack)
            {
                ctrlDataIni.Data = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                ctrlDataFim.Data = ctrlDataIni.Data.AddMonths(1).AddDays(-1);
            }
    
            CriaTabela(ctrlDataIni.Data, ctrlDataFim.Data);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        private void CriaTabela(DateTime dataInicio, DateTime dataFim)
        {
            LimparTabela();
    
            var dados = WebGlass.Business.CapacidadeProducaoDiaria.Fluxo.CapacidadeProducaoDiaria.
                Instance.ObtemPeloPeriodo(dataInicio, dataFim);
    
            bool inicio = true, alt = false;
            TableRow linha = new TableRow();
    
            foreach (var d in dados)
            {
                if (inicio)
                    PreparaTabela(d, ref linha);
    
                inicio = false;
    
                linha.Cells.Add(ObtemCelula(d));
    
                if (d.Data.DayOfWeek == DayOfWeek.Saturday)
                {
                    tblCalendario.Rows.Add(linha);
                    
                    linha = new TableRow();
                    if ((alt = !alt))
                        linha.CssClass = "alt";
                }
            }
    
            if (linha.Cells.Count > 0)
            {
                while (linha.Cells.Count < tblCalendario.Rows[0].Cells.Count)
                    linha.Cells.Add(new TableCell());
    
                tblCalendario.Rows.Add(linha);
            }
        }
    
        private void LimparTabela()
        {
            while (tblCalendario.Rows.Count > 1)
                tblCalendario.Rows.RemoveAt(1);
        }
    
        private void PreparaTabela(WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria dados,
            ref TableRow linha)
        {
            if (dados.Data.DayOfWeek != DayOfWeek.Sunday)
            {
                var diaSemana = DayOfWeek.Sunday;
    
                while (diaSemana != dados.Data.DayOfWeek)
                {
                    linha.Cells.Add(new TableCell());
                    int temp = (int)diaSemana;
                    diaSemana = (DayOfWeek)(++temp);
                }
            }
        }
    
        private TableCell ObtemCelula(WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria dados)
        {
            var celula = new TableCell()
            {
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle
            };
    
            if (dados.Data.Date == DateTime.Now.Date)
                celula.CssClass = "hoje";
    
            if (dados.Data.DiaUtil())
            {
                var controle = LoadControl("~/Controls/ctrlCapacidadeProducaoDiaria.ascx") as Glass.UI.Web.Controls.ctrlCapacidadeProducaoDiaria;
                controle.ID = "CapacidadeProducao_" + dados.Data.ToString("ddMMyyyy");
                controle.CapacidadeProducao = dados;
    
                celula.Controls.Add(controle);
            }
            else
                celula.Text = "Sem produção<br />(não é dia útil)";
    
            return celula;
        }
    }
}
