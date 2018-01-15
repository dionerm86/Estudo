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

            var index = 0;

            var baixas = idProdutos.Take(qtdes.Length).Select(f => new { IdProdBaixa = f, Proc = procs.Length > index ? procs[index] : 0, Apl = apls.Length > index ? apls[index] : 0, Qtde = qtdes[index++] }).ToList();

            if (_baixasEstoque != null)
            {
                // Carrega as baixas do estoque
                for(var i = 0; i < _baixasEstoque.Count; i++)
                {
                    var be = _baixasEstoque[i];

                    var baixa = baixas.FirstOrDefault(f => f.IdProdBaixa == be.IdProdBaixa);
                    if (baixa == null)
                    {
                        _baixasEstoque.RemoveAt(i--);
                    }
                    else
                    {
                        be.Qtde = baixa.Qtde;
                        be.IdProcesso = baixa.Proc;
                        be.IdAplicacao = baixa.Apl;
                        baixas.Remove(baixa);
                    }
                }
                
                foreach (var baixa in baixas)
                    _baixasEstoque.Add(new Estoque.Negocios.Entidades.ProdutoBaixaEstoque
                    {
                        IdProdBaixa = baixa.IdProdBaixa,
                        Qtde = baixa.Qtde,
                        IdProcesso = baixa.Proc,
                        IdAplicacao = baixa.Apl
                    });
                
            }
            else if (_baixasEstoqueFiscal != null)
            {
                // Carrega as baixas do estoque
                for (var i = 0; i < _baixasEstoqueFiscal.Count; i++)
                {
                    var be = _baixasEstoqueFiscal[i];

                    var baixa = baixas.FirstOrDefault(f => f.IdProdBaixa == be.IdProdBaixa);
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
                        IdProdBaixa = baixa.IdProdBaixa,
                        Qtde = baixa.Qtde
                    });
            }
            else
            {
                _baixasEstoque = new List<Estoque.Negocios.Entidades.ProdutoBaixaEstoque>
                    (baixas.Select(f => new Estoque.Negocios.Entidades.ProdutoBaixaEstoque
                    {
                        IdProdBaixa = f.IdProdBaixa,
                        Qtde = f.Qtde,
                        IdProcesso = f.Proc,
                        IdAplicacao = f.Apl
                    }));

                _baixasEstoqueFiscal = new List<Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal>
                    (baixas.Select(f => new Estoque.Negocios.Entidades.ProdutoBaixaEstoqueFiscal
                    {
                        IdProdBaixa = f.IdProdBaixa,
                        Qtde = f.Qtde
                    }));
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Controls.ctrlProdutoBaixaEstoque));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlProdutoBaixaEstoque_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlProdutoBaixaEstoque_script", ResolveUrl("~/scripts/ctrlProdutoBaixaEstoque.js"));
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinha('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            IEnumerable<int> idProds = null;
            IEnumerable<float> qtdes = null;
            IEnumerable<int> idProcs = null;
            IEnumerable<int> idApls = null;

            if (_baixasEstoque != null && _baixasEstoque.Count > 0)
            {
                idProds = _baixasEstoque.Select(f => f.IdProdBaixa);
                qtdes = _baixasEstoque.Select(f => f.Qtde);
                idProcs = _baixasEstoque.Select(f => f.IdProcesso);
                idApls = _baixasEstoque.Select(f => f.IdAplicacao);
            }
            else if (_baixasEstoqueFiscal != null && _baixasEstoqueFiscal.Count > 0)
            {
                idProds = _baixasEstoqueFiscal.Select(f => f.IdProdBaixa);
                qtdes = _baixasEstoqueFiscal.Select(f => f.Qtde);
            }
            else
                return;

            hdfIdProd.Value = string.Join(";", idProds.Select(f => f.ToString()).ToArray());
            hdfQtde.Value = string.Join(";", qtdes.Select(f => f.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray());

            if (idProcs != null)
                hdfProc.Value = string.Join(";", idProcs.Select(f => f.ToString()).ToArray());
            if (idApls != null)
                hdfApl.Value = string.Join(";", idApls.Select(f => f.ToString()).ToArray());

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
