using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using Glass.Data.Helper;
using Glass.Comum.Cache;

/// <summary>
/// Classe com os métodos Ajax para recalcular o orçamento.
/// </summary>
namespace Glass.UI.Web
{
    public static class RecalcularOrcamento
    {
        private static CacheMemoria<Data.Model.Orcamento, string> cacheOrcamentos;

        static RecalcularOrcamento()
        {
            cacheOrcamentos = new CacheMemoria<Data.Model.Orcamento, string>("orcamentos");
        }

        [Ajax.AjaxMethod]
        public static string Recalcular(string idOrcamentoStr, string tipoEntregaNovoStr, string idClienteNovoStr)
        {
            try
            {
                var idOrcamento = idOrcamentoStr.StrParaInt();
                var tipoEntregaNovo = tipoEntregaNovoStr.StrParaIntNullable();
                var idClienteNovo = idClienteNovoStr.StrParaIntNullable();
                int tipoDesconto;
                decimal desconto;
                int tipoAcrescimo;
                decimal acrescimo;
                uint? idComissionado;
                float percComissao;
                Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd;

                var orcamento = ObterOrcamento((uint)idOrcamento);
                OrcamentoDAO.Instance.RecalcularOrcamentoComTransacao(idOrcamento, tipoEntregaNovo, idClienteNovo, out tipoDesconto, out desconto, out tipoAcrescimo, out acrescimo,
                    out idComissionado, out percComissao, out dadosProd, orcamento);

                return OrcamentoDAO.Instance.ObterDadosOrcamentoRecalcular(tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosProd);
            }
            catch (Exception ex)
            {
                return string.Format("Erro;{0}", MensagemAlerta.FormatErrorMsg("Falha ao recalcular orçamento.", ex));
            }
        }

        [Ajax.AjaxMethod]
        public static string GetDadosProdutosRecalcular(string idOrcamentoStr, string idClienteNovoStr)
        {
            uint idOrcamento = Conversoes.StrParaUint(idOrcamentoStr);
            StringBuilder retorno = new StringBuilder();
    
            object[] dadosFormato = new object[9];
            string formato = "" +
                "IdProd: {0}, " +
                "Tipo: '{1}', " +
                "CodInterno: '{2}', " +
                "Espessura: '{3}', " +
                "Altura: '{4}', " +
                "Largura: {5}, " +
                "TotalM2: '{6}', " +
                "ValorUnitario: '{7}', " +
                "Quantidade: {8}";
    
            uint? idCliente = Conversoes.StrParaUintNullable(idClienteNovoStr)
                ?? ObterOrcamento(idOrcamento).IdCliente;
    
            foreach (ProdutosOrcamento p in ProdutosOrcamentoDAO.Instance.GetByOrcamento(idOrcamento, true))
            {
                if (p.TemItensProduto)
                    continue;
    
                p.IdCliente = idCliente;
    
                if (p.IdProduto > 0)
                {
                    dadosFormato[0] = p.IdProd;
                    dadosFormato[1] = "orçamento";
                    dadosFormato[2] = p.CodInterno;
                    dadosFormato[3] = p.Espessura > 0 ? p.Espessura : ProdutoDAO.Instance.ObtemEspessura((int)p.IdProduto.Value);
                    dadosFormato[4] = p.Altura;
                    dadosFormato[5] = p.Largura;
                    dadosFormato[6] = p.TotM;
                    dadosFormato[7] = p.ValorProd.Value;
                    dadosFormato[8] = p.Qtde.ToString().Replace(",", ".");
    
                    retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                }
                else if (p.IdItemProjeto > 0)
                {
                    foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(p.IdItemProjeto.Value))
                        if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)mip.IdGrupoProd) && !SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)mip.IdProd))
                        {
                            mip.IdCliente = idCliente.GetValueOrDefault((uint)mip.IdCliente);
                            dadosFormato[0] = mip.IdMaterItemProj;
                            dadosFormato[1] = "material";
                            dadosFormato[2] = mip.CodInterno;
                            dadosFormato[3] = mip.Espessura;
                            dadosFormato[4] = mip.Altura;
                            dadosFormato[5] = mip.Largura;
                            dadosFormato[6] = mip.TotM;
                            dadosFormato[7] = mip.Valor;
                            dadosFormato[8] = mip.Qtde.ToString().Replace(",", ".");
    
                            retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                        }
                }
            }
    
            return "new Array(" + (retorno.Length > 0 ? retorno.ToString().Substring(2) : "") + ")";
        }
    
        [Ajax.AjaxMethod]
        public static string AtualizaBenef(string idOrcamentoStr, string idProdStr, string tipo, string servicosInfo)
        {
            try
            {
                uint idProd = Conversoes.StrParaUint(idProdStr);
                uint idOrcamento = Conversoes.StrParaUint(idOrcamentoStr);
                var orcamento = ObterOrcamento(idOrcamento);

                GenericBenefCollection beneficiamentos = new GenericBenefCollection();
                beneficiamentos.AddBenefFromServicosInfo(servicosInfo);
    
                switch (tipo.ToLower())
                {
                    case "orçamento":
                        ProdutosOrcamentoDAO.Instance.AtualizaBenef(null, idProd, beneficiamentos, orcamento);
                        break;
    
                    case "material":
                        MaterialItemProjetoDAO.Instance.AtualizaBenef(null, idProd, beneficiamentos, orcamento);
                        break;
                }
    
                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar beneficiamentos do produto.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public static string FinalizarRecalcular(string idOrcamentoStr, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr, string acrescimoStr, string idComissionadoStr,
            string percComissaoStr, string dadosAmbientes)
        {
            try
            {
                var idOrcamento = idOrcamentoStr.StrParaInt();
                var tipoDesconto = tipoDescontoStr.StrParaInt();
                var desconto = descontoStr.Replace(".", ",").StrParaDecimal();
                var tipoAcrescimo = tipoAcrescimoStr.StrParaInt();
                var acrescimo = acrescimoStr.Replace(".", ",").StrParaDecimal();
                var idComissionado = idComissionadoStr.StrParaIntNullable();
                var percComissao = percComissaoStr.Replace(".", ",").StrParaFloat();

                var orcamento = ObterOrcamento((uint)idOrcamento);
                OrcamentoDAO.Instance.FinalizarRecalcularComTransacao(idOrcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes, orcamento);
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recalcular orçamento.", ex);
            }
    
            return "Ok";
        }

        private static Data.Model.Orcamento ObterOrcamento(uint idOrcamento)
        {
            string idCache = string.Format(
                "{0}-{1}",
                UserInfo.GetUserInfo.CodUser,
                idOrcamento
            );

            var orcamento = cacheOrcamentos.RecuperarDoCache(idCache);

            if (orcamento == null)
            {
                orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrcamento);
                cacheOrcamentos.AtualizarItemNoCache(orcamento, idCache);
            }

            return orcamento;
        }
    }
}
