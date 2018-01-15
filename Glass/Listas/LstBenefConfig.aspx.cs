using System;
using System.Linq;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstBenefConfig : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstBenefConfig));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdBenefConfig.PageIndex = 0;
        }
    
        [Ajax.AjaxMethod]
        public string Salvar(string idBenefConfigStr, string dados)
        {
            try
            {
                // Recupera o identificador da configuração
                var idBenefConfig = idBenefConfigStr.StrParaInt();
                string[] linhas = dados.Split('|');

                var precos = linhas.Select(f => 
                    {
                        var valores = f.Split(';');
                        return new
                        {
                            IdBenefConfigPreco = valores[0].StrParaInt(),
                            Custo = valores[1].StrParaDecimal(),
                            ValorAtacado = valores[2].StrParaDecimal(),
                            ValorBalcao = valores[3].StrParaDecimal(),
                            ValorObra = valores[4].StrParaDecimal()
                        };
                    }).ToArray();
    
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IBeneficiamentoFluxo>();

                // Recupera os preços existente
                var precos2 = fluxo.ObtemPrecosBeneficiamento(precos.Select(f => f.IdBenefConfigPreco)).ToArray();

                foreach (var i in precos2)
                {
                    var preco = precos.FirstOrDefault(f => f.IdBenefConfigPreco == i.IdBenefConfigPreco);
                    if (preco != null)
                    {
                        i.Custo = preco.Custo;
                        i.ValorAtacado = preco.ValorAtacado;
                        i.ValorBalcao = preco.ValorBalcao;
                        i.ValorObra = preco.ValorObra;
                    }
                }

                // Salva os preços
                var resultado = fluxo.SalvarPrecosBeneficiamentos(precos2);

                if (!resultado)
                    return string.Format("Erro;{0}", resultado.Message.Format()); 
    
                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar configuração de beneficiamento.", ex);
            }
        }
    }
}
