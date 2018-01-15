using System;
using System.Collections.Generic;
using Sync.Utils.Otimizacao.Models;
using Glass.Data.DAL;

namespace Glass.UI.Web.Otimizacao.Views
{
    public partial class DadosOtimizacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            float areaTotalPecas = 0;
    
            float areaTotalPeca = 0;
            long totalPecas = 0;
    
            var infoPeca = new List<InformacaoPeca>();
    
            #region Pedido
    
            if (!string.IsNullOrEmpty(Request["pedidos"]))
            {
                var pecas = new List<Peca>();
                var dicionario = new Dictionary<string, List<Peca>>();
    
                string[] pedidos = Request["pedidos"].Split(',');
    
                foreach (string p in pedidos)
                {
                    var produtos = ProdutosPedidoDAO.Instance.GetByVidroPedido(Glass.Conversoes.StrParaUint(p), true);
    
                    foreach (var pp in produtos)
                    {
                        if (!dicionario.ContainsKey(pp.DescrProduto))
                        {
                            dicionario.Add(pp.DescrProduto, new List<Peca>());
                        }

                        var pecaItemProj = pp.IdMaterItemProj != null ? PecaItemProjetoDAO.Instance.GetByMaterial(pp.IdMaterItemProj.Value) : null;

                        Peca peca = new Peca();
                        peca.Altura = pecaItemProj != null ? pecaItemProj.Altura : pp.AlturaReal;
                        peca.Largura = pecaItemProj != null ? pecaItemProj.Largura : (pp.LarguraReal > 0 ? pp.LarguraReal : pp.Largura);
                        peca.Quantidade = Convert.ToInt32(pp.Qtde);
                        peca.Id = pp.IdProd;
                        peca.Descricao = pp.DescrProduto;
                        peca.IdPedido = pp.IdPedido;
                        peca.NomeCliente = pp.NomeCliente;
    
                        pecas.Add(peca);
    
                        areaTotalPeca += ((peca.Altura * peca.Largura) * peca.Quantidade) / 1000000;
                        totalPecas += peca.Quantidade;
    
                    }
                }
    
                foreach (var d in dicionario)
                {
                    d.Value.AddRange(new List<Peca>(pecas.FindAll(delegate(Peca p) { return p.Descricao == d.Key; })));
                }
    
                foreach (var d in dicionario)
                {
                    float area = 0;
    
                    foreach (Peca p in d.Value)
                    {
                        area += ((p.Altura * p.Largura) * p.Quantidade) / 1000000;
                    }
    
                    var qtd = (Peca[])Glass.MetodosExtensao.Agrupar<Peca>(d.Value, new string[] { "Id" }, new string[] { "Quantidade" });
    
                    infoPeca.Add(new InformacaoPeca() { AreaTotal = area, Descricao = d.Key, Quantidade = qtd[0].Quantidade, Pecas=d.Value });
    
                    //infoUl.InnerHtml += "<li>" + d.Key + " - Qtde:" + d.Value.Count + " | Área Total: " + area + "m²</li>";
                }
    
                areaTotalPecas += areaTotalPeca;
                //areaTotalPecasLabel.InnerText = Convert.ToDecimal(areaTotalPeca) + "m²";
                //totalPecasLabel.InnerText = totalPecas.ToString();
            }
    
            #endregion
    
            #region Orçamento
    
            if (!string.IsNullOrEmpty(Request["orcamentos"]))
            {
                var pecas = new List<Peca>();
                var dicionario = new Dictionary<string, List<Peca>>();
    
                var orcamentos = Request["orcamentos"].Split(',');
    
                foreach (string o in orcamentos)
                {
                    var produtos = ProdutosOrcamentoDAO.Instance.GetByVidroOrcamento(Conversoes.StrParaUint(o));
    
                    foreach (var pp in produtos)
                    {
                        if (!dicionario.ContainsKey(pp.DescrProduto))
                        {
                            dicionario.Add(pp.DescrProduto, new List<Peca>());
                        }

                        var peca = new Peca();
                        peca.Altura = pp.Altura;
                        peca.Largura = pp.Largura;
                        peca.Quantidade = Convert.ToInt32(pp.Qtde);
                        peca.Id = pp.IdProduto.GetValueOrDefault();
                        peca.Descricao = pp.DescrProduto;
                        peca.IdOrcamento = pp.IdOrcamento;
                        peca.NomeCliente = pp.NomeCliente;
    
                        pecas.Add(peca);
    
                        areaTotalPeca += ((peca.Altura * peca.Largura) * peca.Quantidade) / 1000000;
                        totalPecas += peca.Quantidade;
    
                    }

                    var materialItemProjeto = MaterialItemProjetoDAO.Instance.GetByVidroOrcamento(Conversoes.StrParaUint(o));

                    foreach (var mip in materialItemProjeto)
                    {
                        if (!dicionario.ContainsKey(mip.DescrProduto))
                        {
                            dicionario.Add(mip.DescrProduto, new List<Peca>());
                        }

                        var peca = new Peca();
                        peca.Altura = mip.Altura;
                        peca.Largura = mip.Largura;
                        peca.Quantidade = Convert.ToInt32(mip.Qtde);
                        peca.Id = mip.IdProd;
                        peca.Descricao = mip.DescrProduto;
                        peca.IdOrcamento = mip.IdOrcamento;
                        peca.NomeCliente = ClienteDAO.Instance.GetNome((uint)mip.IdCliente);

                        pecas.Add(peca);

                        areaTotalPeca += ((peca.Altura * peca.Largura) * peca.Quantidade) / 1000000;
                        totalPecas += peca.Quantidade;

                    }
                }
    
                foreach (var d in dicionario)
                {
                    d.Value.AddRange(new List<Peca>(pecas.FindAll(delegate(Peca p) { return p.Descricao == d.Key; })));
                }
    
                foreach (var d in dicionario)
                {
                    float area = 0;
    
                    foreach (Peca p in d.Value)
                    {
                        area += ((p.Altura * p.Largura) * p.Quantidade) / 1000000;
                    }
    
                    var qtd = (Peca[])Glass.MetodosExtensao.Agrupar<Peca>(d.Value, new string[] { "Id" }, new string[] { "Quantidade" });
    
                    infoPeca.Add(new InformacaoPeca() { AreaTotal = area, Descricao = d.Key, Quantidade = qtd[0].Quantidade, Pecas = d.Value });
    
                    //infoUl.InnerHtml += "<li>" + d.Key + " - Qtde:" + d.Value.Count + " | Área Total: " + area + "m²</li>";
                }
    
                areaTotalPecas += areaTotalPeca;
                //areaTotalPecasLabel.InnerText = Convert.ToDecimal(areaTotalPeca) + "m²";
                //totalPecasLabel.InnerText = totalPecas.ToString();
            }
    
            #endregion
    
            var descGroup = (InformacaoPeca[])Glass.MetodosExtensao.Agrupar<InformacaoPeca>(infoPeca, new string[] { "Descricao" }, new string[] { });
            var descricoes = new List<string>(Array.ConvertAll(descGroup, x => x.Descricao));
    
            int index = 1;
    
            foreach (string d in descricoes)
            {
                var info = infoPeca.FindAll(x => x.Descricao == d);
    
                var dados = (InformacaoPeca[])Glass.MetodosExtensao.Agrupar<InformacaoPeca>(info, new string[] { "Descricao" }, new string[] { "Quantidade", "AreaTotal" });
    
                string div = "<div id='" + index + "_div' style='display:none;'>";
    
                foreach (InformacaoPeca i in dados)
                {
                    div += "<table class='gridStyle' style='width:270px; margin: 5px 0 20px 0; border:solid 1px #ccc;padding:0'><tr style='background-color:#ccc;'><th>Nome</th><th>Qtde</th><th>Alt.</th><th>Larg.</th></tr>";
                    foreach (Peca p in i.Pecas)
                    {
                        div += "<tr><td>" + p.Descricao + "</td><td>" + p.Quantidade + "</td><td>" + p.Altura + "</td><td>" + p.Largura + "</td></tr>";
                    }
    
                    div += "</table>";
                }
    
                div += "</div>";
    
                infoUl.InnerHtml += "<li title='Clique para exibir as informações das peças.' class='info-li' id='" + index + "' style='cursor:pointer;'><span style='font-weight:bold;'>" + dados[0].Descricao + "</span> - Qtde:" + dados[0].Quantidade + " | Área Total: " + Math.Round((dados[0].AreaTotal), 2) + "m²" + 
                    div + "</li>";
    
                index++;
            }
        }
    }
}
class InformacaoPeca
{
    public string Descricao { get; set; }

    public int Quantidade { get; set; }

    public float AreaTotal { get; set; }

    public List<Peca> Pecas { get; set; }
}

class teste
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
class texto
{
    public string Nome { get; set; }
}