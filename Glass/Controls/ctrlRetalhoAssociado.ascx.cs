using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.DAL;
using System.Text;
using Glass.Data.Model;
using System.Web.Script.Serialization;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlRetalhoAssociado : BaseUserControl
    {
        #region Propriedades
    
        public uint IdProdPed { get; set; }
    
        public string CampoQtdeImprimir { get; set; }
    
        public string IdsRetalhosAssociados
        {
            get { return hdfIdsRetalhosAssociados.Value; }
            set { hdfIdsRetalhosAssociados.Value = value; }
        }
    
        public string Callback { get; set; }
    
        public string CallbackSelecao { get; set; }
    
        #endregion
    
        #region Ajax
    
        private class DadosAtualizar
        {
            public string NomeControle;
            public uint IdProdPed;
            public string IdsRetalhosDisponiveis;
            public string IdsRetalhosAssociados;
            public string CallbackSelecao;
            public string Resposta;
            public string DadosResposta;
            public int NumeroRetalhosFolga;
        }
    
        [Ajax.AjaxMethod]
        public string Atualizar(string dadosString, string duracaoStr)
        {
            var js = new JavaScriptSerializer();
            var dados = js.Deserialize<DadosAtualizar[]>(dadosString);
            int tentativas = 0, duracao = Glass.Conversoes.StrParaInt(duracaoStr);
            bool possuiRetorno = false;
    
            uint idProdPedAnterior = 0;
            List<uint> retalhosDisponiveis = new List<uint>(), retalhosAssociados = new List<uint>();
            List<RetalhoProducao> retalhos = null;
    
            while (tentativas++ < duracao)
            {
                var fila = new Queue<DadosAtualizar>(dados);
                var enfileirar = new Dictionary<DadosAtualizar, byte>();
    
                while (fila.Count > 0)
                {
                    var d = fila.Dequeue();
    
                    if (d.IdProdPed == 0)
                    {
                        d.Resposta = "Sem retalhos";
                        continue;
                    }
                    else if (d.IdProdPed != idProdPedAnterior)
                    {
                        idProdPedAnterior = d.IdProdPed;
    
                        retalhosDisponiveis.Clear();
                        retalhosAssociados.Clear();
    
                        if (!String.IsNullOrEmpty(d.IdsRetalhosDisponiveis))
                            retalhosDisponiveis.AddRange(Array.ConvertAll(d.IdsRetalhosDisponiveis.Split(','), x => Glass.Conversoes.StrParaUint(x)));
    
                        if (!String.IsNullOrEmpty(d.IdsRetalhosDisponiveis))
                            retalhosAssociados.AddRange(Array.ConvertAll(d.IdsRetalhosAssociados.Split(','), x => Glass.Conversoes.StrParaUint(x)));
                    }
    
                    try
                    {
                        bool possuiRetalhos = false;
                        int retalhosFolga = 0;
                        StringBuilder retornoInt = new StringBuilder(), ids = new StringBuilder();
    
                        retalhos = RetalhoProducaoDAO.Instance.ObterRetalhosProducao(d.IdProdPed, false);
    
                        if (!possuiRetorno && ((retalhos.Count == 0 && retalhosDisponiveis.Count == 0) ||
                            (!retalhos.Exists(x => !retalhosDisponiveis.Contains(x.IdRetalhoProducao)) && retalhosDisponiveis.Count <= retalhos.Count)))
                        {
                            if (fila.Count > 0 && !possuiRetorno)
                            {
                                if (!enfileirar.ContainsKey(d))
                                {
                                    fila.Enqueue(d);
                                    enfileirar.Add(d, 0);
                                }
                                else if (++enfileirar[d] < 3)
                                    fila.Enqueue(d);
                            }
    
                            continue;
                        }
    
                        foreach (RetalhoProducao r in retalhos)
                        {
                            retalhosFolga += r.DentroFolga ? 1 : 0;
                            retornoInt.AppendFormat(@"<tr><td><input type='checkbox' value='{0}' id='chk_{3}_{0}' onclick='{4}.SelecionarRetalho(this)' {2}>
                                <label for='chk_{3}_{0}'>{1}</label></td></tr>", r.IdRetalhoProducao, r.DescricaoRetalhoComEtiqueta,
                                retalhosAssociados.Contains(r.IdRetalhoProducao) ? "checked='checked'" : "", d.IdProdPed, d.NomeControle);
    
                            ids.AppendFormat("{0},", r.IdRetalhoProducao);
                        }
    
                        possuiRetalhos = retalhos.Count > 0;
    
                        if (tentativas >= duracao || (possuiRetorno && !possuiRetalhos))
                            d.Resposta = "Inalterado";
    
                        else if (!possuiRetalhos)
                            d.Resposta = "Sem retalhos";
    
                        else
                        {
                            d.Resposta = "Ok";
                            d.NumeroRetalhosFolga = retalhosFolga;
                            d.DadosResposta = retornoInt.ToString();
                            d.IdsRetalhosDisponiveis = ids.ToString().TrimEnd(',');
    
                            possuiRetorno = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        d.Resposta = "Erro";
                        d.DadosResposta = ex.Message;
                    }
                }
    
                if (possuiRetorno)
                    break;
    
                Thread.Sleep(1000);
            }
    
            return js.Serialize(dados);
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlRetalhoAssociado));
    
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlRetalhoAssociado"))
            {
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlRetalhoAssociado",
                    this.ResolveClientUrl("~/Scripts/ctrlRetalhoAssociado.js"));
    
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlRetalhoAssociado_tooltip",
                    this.ResolveClientUrl("~/Scripts/wz_tooltip.js"));
            }
        }
    
        public string ObtemVariavelJavaScript()
        {
            string dadosControle = @"
                IdProdPed: {0},
                CampoQtdeImprimir: '{1}',
                Callback: '{2}',
                CallbackSelecao: '{3}'
            ";
    
            dadosControle = String.Format(dadosControle,
                IdProdPed,
                CampoQtdeImprimir ?? String.Empty,
                Callback ?? String.Empty,
                CallbackSelecao ?? String.Empty
            );
    
            return String.Format("var {0} = new RetalhoAssociadoType('{0}', {1});\n", 
                this.ClientID, "{" + dadosControle + "}");
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, ObtemVariavelJavaScript(), true);
        }
    }
}
