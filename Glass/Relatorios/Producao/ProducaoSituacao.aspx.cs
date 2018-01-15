using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.ItemTemplates;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoSituacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Today.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Today.ToString("dd/MM/yyyy");
    
                if (!PedidoConfig.LiberarPedido)
                    grdProducaoSituacao.Columns[6].Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProducaoSituacao.PageIndex = 0;
        }
    
        protected void grdProducaoSituacao_Load(object sender, EventArgs e)
        {
            Setor[] lstSetor = Data.Helper.Utils.GetSetores.Where(f => f.ExibirSetores).ToArray();
            int QTD_COLUNAS_GRID = 7;
    
            if (grdProducaoSituacao.Columns.Count <= QTD_COLUNAS_GRID)
            {
                for (int i = 0; i < lstSetor.Length; i++)
                {
                    TemplateField tf = new TemplateField();
                    tf.HeaderText = lstSetor[i].Descricao;
                    grdProducaoSituacao.Columns.Add(tf);
                }
    
                //TemplateField tfPerda = new TemplateField();
                //tfPerda.HeaderText = "Perda";
                //grdProducaoSituacao.Columns.Add(tfPerda);
            }
    
            for (int i = QTD_COLUNAS_GRID; i < grdProducaoSituacao.Columns.Count; i++)
                ((TemplateField)grdProducaoSituacao.Columns[i]).ItemTemplate = new NumeroItemTemplate("SomaSetor[" + (i - QTD_COLUNAS_GRID) + "]");
        }
    }
}
