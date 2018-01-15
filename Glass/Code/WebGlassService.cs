using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using Glass.Data.Model;
using Glass.Data.DAL;
using Sync.Utils.JSON.Models;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;

/// <summary>
/// Summary description for WebGlassService
/// </summary>
namespace Glass.UI.Web
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService] // Define que poderá ser chamado via jquery/json
    public class WebGlassService : System.Web.Services.WebService
    {
        public WebGlassService()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }
    
        /// <summary>
        /// Recupera os dados de ProducaoData da data atual à 7 dias
        /// agrupando por data e situação pendente/pronto, descartando etiquetas não impressas.
        /// </summary>
        /// <returns>List</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Response ObterProducaoDataEntregaSemana()
        {
            Response response = new Response();
    
            try
            {
                List<ProducaoData> lista = ProducaoDataDAO.Instance.ObterProducaoDataEntregaSemana();
                
                List<DateTime> datas = ObterDiasUteis(DateTime.Now, 7);
                //List<DateTime> datas = ObterDiasUteis(new DateTime(2013, 01, 02), 7);
    
                if (lista.Count > 0)
                    foreach (DateTime data in datas)
                    {
                        List<ProducaoData> l = lista.FindAll(delegate(ProducaoData i) { return i.DataHora.Value.ToString("dd/MM/yyyy") == data.ToString("dd/MM/yyyy"); });
    
                        if (l.Count == 0)
                        {
                            ProducaoData pendente = new ProducaoData();
                            pendente.DataHora = data;
                            pendente.Pronto = 0;
                            pendente.TotM2 = 0;
    
                            ProducaoData pronto = new ProducaoData();
                            pronto.DataHora = data;
                            pronto.Pronto = 1;
                            pronto.TotM2 = 0;
    
                            lista.Add(pendente);
                            lista.Add(pronto);
                        }
                    }
                else
                    foreach (DateTime data in datas)
                    {
                        ProducaoData pendente = new ProducaoData();
                        pendente.DataHora = data;
                        pendente.Pronto = 0;
                        pendente.TotM2 = 0;
    
                        ProducaoData pronto = new ProducaoData();
                        pronto.DataHora = data;
                        pronto.Pronto = 1;
                        pronto.TotM2 = 0;
    
                        lista.Add(pendente);
                        lista.Add(pronto);
                    }
    
                lista.Sort(delegate(ProducaoData p1, ProducaoData p2)
                {
                    int ret = p1.DataHora.Value.CompareTo(p2.DataHora.Value);
                    return ret;
                });
    
                response.Status = Response.StatusEnum.SUCCESS;
                response.Message = "Os dados foram recuperados com sucesso.";
                response.Object = lista;
            }
            catch (Exception ex)
            {
                response.Status = Response.StatusEnum.ERROR;
                response.Message = ex.Message;
                response.Object = null;
            }
    
            //return JsonConvert.SerializeObject(response);
    
            return response;
        }
    
        public List<DateTime> ObterDiasUteis(DateTime dataInicial,int dias)
        {
            List<DateTime> lista = new List<DateTime>();
            lista.Add(dataInicial);
    
            //dias = dataInicial.Subtract(dataFinal).Days;
    
            //Módulo 
            if (dias < 0)
                dias = dias * -1;
    
            for (int i = 0; i <= dias; i++)
            {
                int adicionaDias = dataInicial.DayOfWeek == DayOfWeek.Saturday ? 2 : dataInicial.DayOfWeek == DayOfWeek.Sunday ? 1 : 1;
                dataInicial = dataInicial.AddDays(adicionaDias);
                //Conta apenas dias da semana.
                if (dataInicial.DayOfWeek != DayOfWeek.Sunday && dataInicial.DayOfWeek != DayOfWeek.Saturday)
                    lista.Add(dataInicial);
            }
    
            return lista;
        }
        
        #region Sped Fiscal
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public uint InserirAjusteBeneficioIncentivoApuracao(AjusteBeneficioIncentivoApuracao postData)
        {
            return AjusteBeneficioIncentivoApuracaoDAO.Instance.Insert((AjusteBeneficioIncentivoApuracao)postData);
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public uint InserirAjusteApuracaoInfo(AjusteApuracaoInfoAdicional postData)
        {
            return AjusteApuracaoInfoAdicionalDAO.Instance.Insert((AjusteApuracaoInfoAdicional)postData);
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public uint InserirAjusteApuracaoIdentificacaoDocFiscal(AjusteApuracaoIdentificacaoDocFiscal postData)
        {
            return AjusteApuracaoIdentificacaoDocFiscalDAO.Instance.Insert(postData);
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public uint InserirAjusteApuracaoValorDeclaratorio(AjusteApuracaoValorDeclaratorio postData)
        {
            return AjusteApuracaoValorDeclaratorioDAO.Instance.Insert(postData);
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<AjusteBeneficioIncentivo> ObterListaCodigoAjuste(int tipoImposto, string uf, string data)
        {
            return AjusteBeneficioIncentivoDAO.Instance.GetList((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)tipoImposto, uf, DateTime.Parse(data));
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<AjusteBeneficioIncentivoApuracao> ObterListaCodigoAjusteApuracao(int tipoImposto)
        {
            List<AjusteBeneficioIncentivoApuracao> data = AjusteBeneficioIncentivoApuracaoDAO.Instance.GetList((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)tipoImposto, null, null, null);
    
            return data;
        }
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public uint InserirAjusteApuracaoIPI(AjusteApuracaoIPI postData)
        {
            return AjusteApuracaoIPIDAO.Instance.Insert(postData);
    
        }
    
        #endregion
    
        #region Produção
    
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<RetalhoProducao> ObterRetalhos(uint idProdPed)
        {
            return RetalhoProducaoDAO.Instance.ObterRetalhosProducao(idProdPed);
        }
    
        #endregion
    }
}
