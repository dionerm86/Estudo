using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using Glass.Data.EFD;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ImpostosCte : CteBaseUserControl
    {
        private List<ImpostoCte> controles;
    
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte> ObjImpostoCte
        {
            get
            {
                var lista = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte>();
    
                foreach (var c in controles)
                    lista.Add(c.ObjImpostoCte);
    
                return lista;
            }
            set
            {
                foreach (var c in controles)
                {
                    var i = value.FirstOrDefault(x => x.TipoImposto == (int)c.TipoImposto);
                    if (i != null)
                        c.ObjImpostoCte = i;
                }
            }
        }
    
        public override Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get { return base.TipoDocumentoCte; }
            set
            {
                base.TipoDocumentoCte = value;
                imp_icms.TipoDocumentoCte = value;
                imp_pis.TipoDocumentoCte = value;
                imp_cofins.TipoDocumentoCte = value;
            }
        }

        public bool ICMSObrigatorio
        {
            get
            {
                if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                    return true;
                else
                    return false;
            }
        }

        public bool PISObrigatorio
        {
            get
            {
                if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                    return FiscalConfig.TelaCadastroCTe.PISObrigatorioCTeSaida;
                else
                    return false;
            }
        }

        public bool COFINSObrigatorio
        {
            get
            {
                if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                    return FiscalConfig.TelaCadastroCTe.COFINSObrigatorioCTeSaida;
                else
                    return false;
            }
        }

        #endregion

        public override IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get
            {
                List<BaseValidator> val = new List<BaseValidator>();
                foreach (var c in controles)
                    val.AddRange(c.ValidadoresObrigatoriosEntrada);
    
                return val;
            }
        }
    
        protected void Page_Init(object sender, EventArgs e)
        {
            controles = new List<ImpostoCte>() { imp_icms, imp_pis, imp_cofins };
    
            imp_icms.ItensCst(DataSourcesEFD.Instance.GetCstIcms().Select(x => new GenericModel(Glass.Conversoes.StrParaUint(x.Key), x.Value)),
                new[] { "00" }, new[] { "20", "70" }, new[] { "10", "30", "60", "70" }, new[] { "90" });
    
            imp_pis.ItensCst(DataSourcesEFD.Instance.GetCstPisCofins().Select(x => new GenericModel(x.Id, x.Id != null ? x.Id.Value.ToString("00") : String.Empty)),
                new[] { "01", "02", "03", "04", "06", "49" }, new[] { "" }, new[] { "05", "75", "98", "99" }, 
                new[] { "50", "51", "52", "53", "54", "55", "56", "60", "61", "62", "63", "64", "65", "66", "67" });
    
            imp_cofins.ItensCst(DataSourcesEFD.Instance.GetCstPisCofins().Select(x => new GenericModel(x.Id, x.Id != null ? x.Id.Value.ToString("00") : String.Empty)),
                new[] { "01", "02", "03", "04", "06", "49" }, new[] { "" }, new[] { "05", "75", "98", "99" },
                new[] { "50", "51", "52", "53", "54", "55", "56", "60", "61", "62", "63", "64", "65", "66", "67" });
        }
    }
}
