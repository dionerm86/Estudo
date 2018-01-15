using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using Glass.Data.Helper;
using Glass.Configuracoes;

/// <summary>
/// Classe com os métodos Ajax para recalcular o orçamento.
/// </summary>
namespace Glass.UI.Web
{
    public static class RecalcularOrcamento
    {
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

                OrcamentoDAO.Instance.RecalcularOrcamentoComTransacao(idOrcamento, tipoEntregaNovo, idClienteNovo, out tipoDesconto,
                    out desconto, out tipoAcrescimo, out acrescimo, out idComissionado, out percComissao, out dadosProd);

                var dadosAmbientes = string.Empty;
                foreach (var idProd in dadosProd.Keys)
                {
                    var dadosDesconto = dadosProd[idProd].Key;
                    var dadosAcrescimo = dadosProd[idProd].Value;

                    dadosAmbientes +=
                        string.Format("{0},{1},{2},{3},{4}|",
                            idProd, dadosDesconto.Key, dadosDesconto.Value.ToString().Replace(",", "."),
                            dadosAcrescimo.Key, dadosAcrescimo.Value.ToString().Replace(",", "."));
                }

                return
                    string.Format("Ok;{0};{1};{2};{3};{4};{5};{6}",
                        tipoDesconto,
                        desconto.ToString().Replace(",", "."),
                        tipoAcrescimo,
                        acrescimo.ToString().Replace(",", "."),
                        idComissionado > 0 ? idComissionado.ToString() : string.Empty,
                        percComissao.ToString().Replace(",", "."),
                        dadosAmbientes.TrimEnd('|'));
            }
            catch (Exception ex)
            {
                return string.Format("Erro;{0}", MensagemAlerta.FormatErrorMsg("Falha ao recalcular orçamento.", ex));
            }
        }

        [Ajax.AjaxMethod]
        public static string GetDadosProdutosRecalcular(string idOrcamentoStr, string idClienteNovoStr)
        {
            uint idOrcamento = Glass.Conversoes.StrParaUint(idOrcamentoStr);
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
    
            uint? idCliente = String.IsNullOrEmpty(idClienteNovoStr) ? OrcamentoDAO.Instance.ObtemIdCliente(idOrcamento) :
                Glass.Conversoes.StrParaUintNullable(idClienteNovoStr);
    
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
        public static string AtualizaBenef(string idProdStr, string tipo, string servicosInfo)
        {
            try
            {
                uint idProd = Glass.Conversoes.StrParaUint(idProdStr);
                GenericBenefCollection beneficiamentos = new GenericBenefCollection();
                beneficiamentos.AddBenefFromServicosInfo(servicosInfo);
    
                switch (tipo.ToLower())
                {
                    case "orçamento":
                        ProdutosOrcamentoDAO.Instance.AtualizaBenef(idProd, beneficiamentos);
                        break;
    
                    case "material":
                        MaterialItemProjetoDAO.Instance.AtualizaBenef(idProd, beneficiamentos);
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
        public static string FinalizarRecalcular(string idOrcamentoStr, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr, string acrescimoStr, 
            string idComissionadoStr, string percComissaoStr, string dadosAmbientes)
        {
            try
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(idOrcamentoStr);
                int tipoDesconto = Glass.Conversoes.StrParaInt(tipoDescontoStr);
                decimal desconto = decimal.Parse(descontoStr.Replace(".", ","));
                int tipoAcrescimo = Glass.Conversoes.StrParaInt(tipoAcrescimoStr);
                decimal acrescimo = decimal.Parse(acrescimoStr.Replace(".", ","));
                uint? idComissionado = Glass.Conversoes.StrParaUintNullable(idComissionadoStr);
                float percComissao = float.Parse(percComissaoStr.Replace(".", ","));

                // Remove o percentual de comissão dos beneficiamentos do orçamento
                // Para que não sejam aplicados 2 vezes (se o cálculo do valor for feito com o percentual aplicado)
                ProdutoOrcamentoBenefDAO.Instance.RemovePercComissaoBenef(idOrcamento, percComissao);

                foreach (ProdutosOrcamento po in ProdutosOrcamentoDAO.Instance.GetByOrcamento(idOrcamento, false))
                    ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(po);

                OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrcamento);

                string[] ambientes = dadosAmbientes.TrimEnd('|').Split('|');
                foreach (string dados in ambientes)
                {
                    if (String.IsNullOrEmpty(dados))
                        continue;

                    string[] dadosProd = dados.Split(',');

                    uint idProd = Glass.Conversoes.StrParaUint(dadosProd[0]);
                    int tipoDescontoProd = Glass.Conversoes.StrParaInt(dadosProd[1]);
                    decimal descontoProd = decimal.Parse(dadosProd[2].Replace(".", ","));
                    int tipoAcrescimoProd = Glass.Conversoes.StrParaInt(dadosProd[3]);
                    decimal acrescimoProd = decimal.Parse(dadosProd[4].Replace(".", ","));

                    ProdutosOrcamentoDAO.Instance.AplicaAcrescimo(idProd, tipoAcrescimoProd, acrescimoProd);
                    ProdutosOrcamentoDAO.Instance.AplicaDesconto(idProd, tipoDescontoProd, descontoProd);
                }

                OrcamentoDAO.Instance.AplicaComissaoDescontoAcrescimo(idOrcamento, idComissionado, percComissao,
                    tipoAcrescimo, acrescimo, tipoDesconto, desconto, Geral.ManterDescontoAdministrador);

                OrcamentoDAO.Instance.AtualizarDataRecalcular(idOrcamento, DateTime.Now, UserInfo.GetUserInfo.CodUser);

                Orcamento orca = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrcamento);
                OrcamentoDAO.Instance.Update(orca);
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recalcular orçamento.", ex);
            }
    
            return "Ok";
        }
    }
}
