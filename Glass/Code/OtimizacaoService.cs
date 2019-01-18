using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using Sync.Utils.Otimizacao.Models;
using Glass.Data.Model;
using Glass.Data.DAL;
using Sync.Utils.Otimizacao.BFL;
using Sync.Utils.JSON.Models;
using System.Linq;
using Glass.Data.Helper;
using Glass.Configuracoes;

/// <summary>
/// Summary description for OtimizacaoService
/// </summary>
namespace Glass.UI.Web
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class OtimizacaoService : System.Web.Services.WebService
    {
    
        public OtimizacaoService()
        {
    
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }
    
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string HelloWorld(string val)
        //{
        //    return new JavaScriptSerializer().Serialize(val);
        //}
    
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Response Otimizar(string idchapas, string pedidos, string orcamentos)
        {
            orcamentos = orcamentos.Replace("#", "");

            var model = new List<Sync.Utils.Otimizacao.Models.Otimizacao>();
    
            Response response = new Response();
    
            try
            {
                string[] arrayChapas = idchapas.Split('|');
                string[] idsPedido = pedidos.Split(',');
                string[] idsOrcamento = orcamentos.Split(',');
    
                #region Chapa
    
                string idsChapa = "";
    
                foreach (string c in arrayChapas)
                {
                    string[] ch = c.Split(';');
    
                    idsChapa += ch[0] + ",";
                }
                
                List<ChapaVidro> chapaVidro = ChapaVidroDAO.Instance.ObterListaOtimizacao(idsChapa.TrimEnd(','));
    
                chapaVidro.Sort(
                    delegate(ChapaVidro x1, ChapaVidro x2)
                    {
                        return x1.DescrProduto.CompareTo(x2.DescrProduto);
                    }
                );
    
                List<Chapa> chapas = new List<Chapa>();
    
                foreach (ChapaVidro c in chapaVidro)
                {
                    /*int qtde = 0;
    
                    foreach (string e in arrayChapas)
                    {
                        string[] ch = e.Split(';');
    
                        if (ch[0] == c.IdChapaVidro.ToString())
                        {
                            qtde = Glass.Conversoes.StrParaInt(ch[4]);
                            break;
                        }
                    }*/
    
                    Chapa chapa = new Chapa();
                    chapa.Altura = c.Altura;
                    chapa.Descricao = c.DescrProduto;
                    chapa.Id = c.IdChapaVidro;
                    chapa.IdProd = c.IdProd;
                    chapa.Largura = c.Largura;
                    chapa.Quantidade = c.Quantidade;
    
                    chapas.Add(chapa);
                }
    
                #endregion
    
                int index = 0;
    
                #region Projeto
    
                Sync.Utils.Otimizacao.Models.Projeto projeto = new Sync.Utils.Otimizacao.Models.Projeto();
    
                foreach (string p in idsPedido)
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        var produtos = ProdutosPedidoDAO.Instance.GetByVidroPedido(Glass.Conversoes.StrParaUint(p), true).ToArray();
    
                        foreach (ProdutosPedido pp in produtos)
                        {
                            var pecaItemProj = pp.IdMaterItemProj != null ? PecaItemProjetoDAO.Instance.GetByMaterial(pp.IdMaterItemProj.Value) : null;

                            Peca peca = new Peca();
                            peca.Altura = pecaItemProj != null ? pecaItemProj.Altura : (pp.AlturaReal > 0 ? pp.AlturaReal : pp.Altura);
                            peca.Largura = pecaItemProj != null ? pecaItemProj.Largura : (pp.LarguraReal > 0 ? pp.LarguraReal : pp.Largura);
                            peca.Quantidade = Convert.ToInt32(pp.Qtde);
                            peca.Id = ProdutoBaixaEstoqueDAO.Instance.ObterIdProdBaixa(pp.IdProd);
                            peca.Descricao = pp.DescrProduto;
                            peca.IdPedido = pp.IdPedido;
                            peca.NomeCliente = pp.NomeCliente;
                            peca.Index = index;
    
                            projeto.Pecas.Add(peca);
    
                            index++;
                        }
    
                        projeto.Pedidos.Add(Glass.Conversoes.StrParaUint(p));
                    }
                }
    
                foreach (string o in idsOrcamento)
                {
                    if (!string.IsNullOrEmpty(o))
                    {
                        var produtos = ProdutosOrcamentoDAO.Instance.GetByVidroOrcamento(Glass.Conversoes.StrParaUint(o)).ToArray();
    
                        foreach (ProdutosOrcamento pp in produtos)
                        {
                            Peca peca = new Peca();
                            peca.Altura = pp.Altura;
                            peca.Largura = pp.Largura;
                            peca.Quantidade = Convert.ToInt32(pp.Qtde);
                            peca.Id = ProdutoBaixaEstoqueDAO.Instance.ObterIdProdBaixa(pp.IdProduto.GetValueOrDefault());
                            peca.Descricao = pp.DescrProduto;
                            peca.IdOrcamento = pp.IdOrcamento;
                            peca.NomeCliente = pp.NomeCliente;
                            peca.Index = index;
    
                            projeto.Pecas.Add(peca);
    
                            index++;
                        }

                        var materialItemProjeto = MaterialItemProjetoDAO.Instance.GetByVidroOrcamento(Conversoes.StrParaUint(o));

                        foreach (var mip in materialItemProjeto)
                        {
                            var peca = new Peca();
                            peca.Altura = mip.Altura;
                            peca.Largura = mip.Largura;
                            peca.Quantidade = Convert.ToInt32(mip.Qtde);
                            peca.Id = mip.IdProd;
                            peca.Descricao = mip.DescrProduto;
                            peca.IdOrcamento = mip.IdOrcamento;
                            peca.NomeCliente = ClienteDAO.Instance.GetNome((uint)mip.IdCliente);
                            peca.Index = index;

                            projeto.Pecas.Add(peca);

                            index++;
                        }

                        projeto.Orcamentos.Add(Glass.Conversoes.StrParaUint(o));
                    }
                }
    
                #endregion
    
                foreach (Chapa c in chapas)
                {
                    List<Peca> pecas = projeto.Pecas.FindAll(p => ProdutoBaixaEstoqueDAO.Instance.IsMateriaPrima(p.Id, c.IdProd) || p.Id == c.IdProd);
    
                    IEnumerable<Peca> idPedidoGroup = Glass.MetodosExtensao.Agrupar<Peca>(pecas, new string[] { "IdPedido" }, new string[] { });
                    List<uint> idPedido = new List<uint>(Array.ConvertAll(idPedidoGroup.ToArray(), x => x.IdPedido));
    
                    IEnumerable<Peca> idOrcamentoGroup = Glass.MetodosExtensao.Agrupar<Peca>(pecas, new string[] { "IdOrcamento" }, new string[] { });
                    List<uint> idOrcamento = new List<uint>(Array.ConvertAll(idOrcamentoGroup.ToArray(), x => x.IdOrcamento));    
    
                    Sync.Utils.Otimizacao.Models.Projeto proj = new Sync.Utils.Otimizacao.Models.Projeto();
                    proj.Pecas = pecas;
                    proj.Pedidos = idPedido;
                    proj.Orcamentos = idOrcamento;
    
                    Sync.Utils.Otimizacao.Models.Configuracao config = new Sync.Utils.Otimizacao.Models.Configuracao();
                    
                    config.Chapas = new List<Chapa>();
    
                    //for (int i = 0; i < c.Quantidade; i++)
                    //    config.Chapas.Add(new Chapa() { Altura = c.Altura, Descricao = c.Descricao, Quantidade = 1, IdProd = c.IdProd, Largura = c.Largura, Id = c.Id });
    
                    config.Chapas.Add(new Chapa() { Altura = c.Altura, Descricao = c.Descricao, Quantidade = c.Quantidade, IdProd = c.IdProd, Largura = c.Largura, Id = c.Id });
                    
                    config.Projeto = proj;
    
                    var otm = OtimizacaoFlow.Instance.Otimizar(config);
                    model.AddRange(otm);
                }
    
                response.Status = Response.StatusEnum.SUCCESS;
                response.Message = "Otimização efetuada com sucesso.";
    
                int numRefChapa = 1;
                int numRefPeca = 1;
                foreach (var i in model)
                {
                    i.NumeroChapaStr = numRefChapa.ToString() + "-" + i.DescricaoChapa.Substring(i.DescricaoChapa.LastIndexOf(" ")).Trim();
                    i.NumeroChapa = numRefChapa;
    
                    foreach (PecaMapeada p in i.PecasMapeadas)
                    {
                        p.Chapa = numRefChapa;
                        p.Indice = numRefPeca;
    
                        numRefPeca++;
                    }
    
                    numRefChapa++;
                }
    
                response.Object = model;
    
            }
            catch (Exception ex)
            {
                response.Status = Response.StatusEnum.ERROR;
                response.Message = ex.Message;
                response.Object = null;
            }
    
            return response;
        }
    
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ObterFooterImpressao()
        {
            uint idFunc = UserInfo.GetUserInfo.CodUser;
            return Geral.TextoRodapeRelatorio(FuncionarioDAO.Instance.GetNome(idFunc));
        }
    }
}
