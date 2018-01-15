using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlProdutoBaixaEstoque : BaseUserControl
    {
        #region Variáveis Locais

        private IList<Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoque> _baixasEstoque;
        private IList<Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal> _baixasEstoqueFiscal;

        #endregion

        #region Propriedades

        /// <summary>
        /// Baixas do estoque.
        /// </summary>
        public IList<Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoque> BaixasEstoque
        {
            get { return _baixasEstoque; }
            set { _baixasEstoque = value;  }
        }

        /// <summary>
        /// Baixas do estoque fiscal.
        /// </summary>
        public IList<Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal> BaixasEstoqueFiscal
        {
            get { return _baixasEstoqueFiscal; }
            set { _baixasEstoqueFiscal = value;  }
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) return;

            // Recupera os identificadores dos produtos
            var idsProdBaixaEst = hdfIdsProdBaixaEst.Value.Split(';')
                .Where(f => !string.IsNullOrEmpty(f))
                .Select(f => int.Parse(f))
                .ToArray(); ;

            // Recupera os identificadores dos produtos
            var idProdutos = hdfIdProd.Value.Split(';')
                .Where(f => !string.IsNullOrEmpty(f))
                .Select(f => int.Parse(f));

            // Recupera as quantidades
            var qtdes = hdfQtde.Value.Split(';')
                .Where(f => !string.IsNullOrEmpty(f))
                .Select(f => Conversoes.StrParaFloat(f))
                .ToArray();

            var procs = hdfProc.Value.Split(';')
                .Select(f => f.StrParaInt())
                .ToArray();

            var apls = hdfApl.Value.Split(';')
                .Select(f => f.StrParaInt())
                .ToArray();

            // Recupera as quantidades
            var alts = hdfAlturaProdBaixa.Value.Split(';')
                .Select(f => Conversoes.StrParaInt(f))
                .ToArray();

            // Recupera as quantidades
            var largs = hdfLarguraProdBaixa.Value.Split(';')
                .Select(f => Conversoes.StrParaInt(f))
                .ToArray();

            // Recupera as quantidades
            var formas = hdfFormaProdBaixa.Value.Split(';')
                .Select(f => f)
                .ToArray();

            var index = 0;

            var baixas = idProdutos
                .Take(qtdes.Length)
                .Select(f => new
                {
                    IdProd = f,
                    idProdBaixaEst = idsProdBaixaEst.Length > index ? idsProdBaixaEst[index] : 0,
                    Proc = procs.Length > index ? procs[index] : 0,
                    Apl = apls.Length > index ? apls[index] : 0,
                    Alt = alts.Length > index ? alts[index] : 0,
                    Larg = largs.Length > index ? largs[index] : 0,
                    Forma = formas.Length > index ? formas[index] : "",
                    Qtde = qtdes[index++]

                }).ToList();

            if (_baixasEstoque != null)
            {
                var baixasAdicionar = baixas.Where(f => f.idProdBaixaEst == 0).ToList();
                baixas = baixas.Where(f => f.idProdBaixaEst != 0).ToList();

                // Carrega as baixas do estoque
                for (var i = 0; i < _baixasEstoque.Count; i++)
                {
                    var be = _baixasEstoque[i];

                    var baixa = baixas.FirstOrDefault(f => f.idProdBaixaEst == be.IdProdBaixaEst);
                    if (baixa == null)
                    {
                        _baixasEstoque.RemoveAt(i--);
                    }
                    else
                    {
                        be.IdProdBaixa = baixa.IdProd;
                        be.Qtde = baixa.Qtde;
                        be.IdProcesso = baixa.Proc;
                        be.IdAplicacao = baixa.Apl;
                        be.Altura = baixa.Alt;
                        be.Largura = baixa.Larg;
                        be.Forma = baixa.Forma;

                        baixas.Remove(baixa);
                    }
                }

                baixas.AddRange(baixasAdicionar);
                
                foreach (var baixa in baixas)
                    _baixasEstoque.Add(new Estoque.Negocios.Entidades.ProdutoBaixaEstoque
                    {
                        IdProdBaixa = baixa.IdProd,
                        Qtde = baixa.Qtde,
                        IdProcesso = baixa.Proc,
                        IdAplicacao = baixa.Apl,
                        Altura = baixa.Alt,
                        Largura = baixa.Larg,
                        Forma = baixa.Forma
                    });
                
            }
            else if (_baixasEstoqueFiscal != null)
            {
                // Carrega as baixas do estoque
                for (var i = 0; i < _baixasEstoqueFiscal.Count; i++)
                {
                    var be = _baixasEstoqueFiscal[i];

                    var baixa = baixas.FirstOrDefault(f => f.IdProd == be.IdProdBaixa);
                    if (baixa == null)
                    {
                        _baixasEstoqueFiscal.RemoveAt(i--);
                    }
                    else
                    {
                        be.Qtde = baixa.Qtde;
                        baixas.Remove(baixa);
                    }
                }

                foreach (var baixa in baixas)
                    _baixasEstoqueFiscal.Add(new Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal
                    {
                        IdProdBaixa = baixa.IdProd,
                        Qtde = baixa.Qtde
                    });
            }
            else
            {
                _baixasEstoque = new List<Estoque.Negocios.Entidades.ProdutoBaixaEstoque>
                    (baixas.Select(f => new Estoque.Negocios.Entidades.ProdutoBaixaEstoque
                    {
                        IdProdBaixa = f.IdProd,
                        Qtde = f.Qtde,
                        IdProcesso = f.Proc,
                        IdAplicacao = f.Apl,
                        Altura = f.Alt,
                        Largura = f.Larg,
                        Forma = f.Forma
                    }));

                _baixasEstoqueFiscal = new List<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal>
                    (baixas.Select(f => new Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal
                    {
                        IdProdBaixa = f.IdProd,
                        Qtde = f.Qtde
                    }));
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Controls.ctrlProdutoBaixaEstoque));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlProdutoBaixaEstoque_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlProdutoBaixaEstoque_script", ResolveUrl("~/scripts/ctrlProdutoBaixaEstoque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));

            imgAdicionar.OnClientClick = "adicionarLinha('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            IEnumerable<int> idProdBaixaEst = null;
            IEnumerable<int> idProds = null;
            IEnumerable<float> qtdes = null;
            IEnumerable<int> idProcs = null;
            IEnumerable<int> idApls = null;
            IEnumerable<int> alts = null;
            IEnumerable<int> largs = null;
            IEnumerable<string> formas = null;

            if (_baixasEstoque != null && _baixasEstoque.Count > 0)
            {
                idProdBaixaEst = _baixasEstoque.Select(f => f.IdProdBaixaEst);
                idProds = _baixasEstoque.Select(f => f.IdProdBaixa);
                qtdes = _baixasEstoque.Select(f => f.Qtde);
                idProcs = _baixasEstoque.Select(f => f.IdProcesso);
                idApls = _baixasEstoque.Select(f => f.IdAplicacao);
                alts = _baixasEstoque.Select(f => f.Altura);
                largs = _baixasEstoque.Select(f => f.Largura);
                formas = _baixasEstoque.Select(f => f.Forma);
            }
            else if (_baixasEstoqueFiscal != null && _baixasEstoqueFiscal.Count > 0)
            {
                idProdBaixaEst = _baixasEstoqueFiscal.Select(f => f.IdProdBaixaEstFisc);
                idProds = _baixasEstoqueFiscal.Select(f => f.IdProdBaixa);
                qtdes = _baixasEstoqueFiscal.Select(f => f.Qtde);
            }
            else
                return;

            hdfIdsProdBaixaEst.Value = string.Join(";", idProdBaixaEst.Select(f => f.ToString()).ToArray());
            hdfIdProd.Value = string.Join(";", idProds.Select(f => f.ToString()).ToArray());
            hdfQtde.Value = string.Join(";", qtdes.Select(f => f.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray());

            if (idProcs != null)
                hdfProc.Value = string.Join(";", idProcs.Select(f => f.ToString()).ToArray());
            if (idApls != null)
                hdfApl.Value = string.Join(";", idApls.Select(f => f.ToString()).ToArray());
            if (alts != null)
                hdfAlturaProdBaixa.Value = string.Join(";", alts.Select(f => f.ToString()).ToArray());
            if (largs != null)
                hdfLarguraProdBaixa.Value = string.Join(";", largs.Select(f => f.ToString()).ToArray());
            if (formas != null)
                hdfFormaProdBaixa.Value = string.Join(";", formas.Select(f => f).ToArray());

            var produtoFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IProdutoFluxo>();

            // Recupera os descritores dos produtos
            var produtos = produtoFluxo.ObtemProdutos(idProds);
            produtos = idProds.Select(f => produtos.FirstOrDefault(x => f == x.Id)).Where(f => f != null).ToArray();

            string codigos = "", descricoes = "";
            foreach(var prod in produtos)
            {
                if (string.IsNullOrEmpty(prod.Name) || string.IsNullOrEmpty(prod.Description))
                    continue;

                codigos += "'" + prod.Name.Replace("'", "\'") + "', ";
                descricoes += "'" + prod.Description.Replace("'", "\'") + "', ";
            }

            // O motivo de colocar DateTime.Now.Ticks é para que force a inserção deste script na página quando tiver mais de um
            // controle deste inserido, como ocorre por exemplo no cadastro de produto
            System.Threading.Thread.Sleep(100);
            Page.ClientScript.RegisterStartupScript(GetType(), "iniciar" + new Random().Next(0, 1000), "carregaProdInicial('" +
                this.ClientID + "', new Array(" + codigos.TrimEnd(',', ' ') + "), new Array(" +
                descricoes.TrimEnd(',', ' ') + "));", true);
        }

        #endregion

        #region Métodos Ajax

        /// <summary>
        /// Verifica se o subgrupo informado e do tipo vidro laminado
        /// </summary>
        /// <param name="idSubgrupo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public bool VerificaSubgrupoLaminado(int idSubgrupo)
        {
            var tipo = Data.DAL.SubgrupoProdDAO.Instance.ObtemTipoSubgrupoPorSubgrupo(null, idSubgrupo);

            return tipo == Data.Model.TipoSubgrupoProd.VidroLaminado || tipo == Data.Model.TipoSubgrupoProd.VidroDuplo;
        }

        #endregion
    }
}
