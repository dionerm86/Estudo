using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;
using Glass.Configuracoes;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlCapacidadeProducaoDiaria : System.Web.UI.UserControl
    {
        private WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria _capacidadeProducao;

        #region Classes de suporte
    
        public class DadosCapacidadeProducao
        {
            public class DadosSetor
            {
                public uint Setor { get; set; }
                public int Capacidade { get; set; }
            }

            public class DadosClassificacao
            {
                public int Classificacao { get; set; }
                public int Capacidade { get; set; }
            }
    
            public string Data { get; set; }
            public int? MaximoVendasM2 { get; set; }
            public DadosSetor[] CapacidadeSetores { get; set; }
            public DadosClassificacao[] CapacidadeClassificacoes { get; set; }
        }
    
        #endregion
    
        public WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria CapacidadeProducao
        {
            get
            {
                return _capacidadeProducao ?? 
                    new WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria();
            }
            set { _capacidadeProducao = value; }
        }
    
        protected string GetNomeSetor(object codigoSetor)
        {
            var setor = Data.Helper.Utils.ObtemSetor((uint)codigoSetor);
    
            if(setor == null)
                return null;
            else 
                return setor.Descricao;
        }

        protected string GetNomeClassificacao(object idClassificacao)
        {
            return ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>().ObtemDescricao((int)idClassificacao);
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlCapacidadeProducaoDiaria));
    
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlCapacidadeProducaoDiaria"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlCapacidadeProducaoDiaria", this.ResolveClientUrl("~/Scripts/ctrlCapacidadeProducaoDiaria.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
    
            rptPrincipal.DataSource = new[] { CapacidadeProducao };
            rptPrincipal.DataBind();
        }
    
        protected bool ExibirCapacidadeSetor()
        {
            return Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor;
        }
    
        [Ajax.AjaxMethod]
        public string Salvar(string dadosStr)
        {
            var js = new JavaScriptSerializer();
            var dados = js.Deserialize<DadosCapacidadeProducao>(dadosStr);
    
            var temp = new Dictionary<uint, int>();
            foreach (var s in dados.CapacidadeSetores)
                temp.Add(s.Setor, s.Capacidade);

            var temp2 = new Dictionary<int, int>();
            foreach (var c in dados.CapacidadeClassificacoes)
                temp2.Add(c.Classificacao, c.Capacidade);
    
            var capacidade = new WebGlass.Business.CapacidadeProducaoDiaria.Entidade.CapacidadeProducaoDiaria()
            {
                Data = DateTime.Parse(dados.Data),
                MaximoVendasM2 = dados.MaximoVendasM2,
                CapacidadeSetores = temp,
                CapacidadeClassificacao = temp2
            };
    
            WebGlass.Business.CapacidadeProducaoDiaria.Fluxo.CapacidadeProducaoDiaria.Instance.Salvar(capacidade);
    
            return null;
        }
    }
}
