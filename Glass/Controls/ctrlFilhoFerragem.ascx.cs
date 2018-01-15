using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlFilhoFerragem : System.Web.UI.UserControl
    {
        #region Variáveis Locais

        private int _idFerragem;
        private IList<Glass.Projeto.Negocios.Entidades.CodigoFerragem> _codigoFerragem;
        private IList<Glass.Projeto.Negocios.Entidades.ConstanteFerragem> _constanteFerragem;
        private bool _podeAlterar;

        #endregion

        #region Propriedades

        public int IdFerragem
        {
            get { return _idFerragem; }
            set { _idFerragem = value; }
        }

        public IList<Glass.Projeto.Negocios.Entidades.CodigoFerragem> CodigoFerragem
        {
            get { return _codigoFerragem; }
            set { _codigoFerragem = value; }
        }

        public IList<Glass.Projeto.Negocios.Entidades.ConstanteFerragem> ConstanteFerragem
        {
            get { return _constanteFerragem; }
            set { _constanteFerragem = value; }
        }

        public bool PodeAlterar
        {
            get { return _podeAlterar; }
            set { _podeAlterar = value; }
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) return;

            // Recupera os dados dos HiddenField da tela para a entidade.
            var idFerragem = Conversoes.StrParaInt(hdfIdFerragem.Value);

            var idsFilhosFerragem = hdfIdsfilhosFerragem.Value.Split(';')
                .Select(f => Conversoes.StrParaInt(f))
                .ToArray();

            var descricoes = hdfDescricoes.Value.Split(';')
                .Where(f => !string.IsNullOrEmpty(f))
                .Select(f => f)
                .ToArray();

            var valores = hdfValores.Value.Split(';')
                .Select(f => Conversoes.StrParaDouble(f))
                .ToArray();

            var index = 0;

            var filhos = idsFilhosFerragem
                .Take(descricoes.Length)
                .Select(f => new
                {
                    IdFilhoFerragem = f,
                    IdFerragem = idFerragem,
                    Valor = valores.Length > index ? valores[index] : 0,
                    Descricao = descricoes[index++]
                }).ToList();

            if (_codigoFerragem != null && _codigoFerragem.Any())
            {
                // Remove os codigos de _codigoFerragem que foram removidas no javaScript
                for (var i = 0; i < _codigoFerragem.Count; i++)
                {
                    var cf = _codigoFerragem[i];

                    var filho = filhos.FirstOrDefault(f => f.IdFilhoFerragem == cf.IdCodigoFerragem);
                    // Se o codigo foi removida
                    if (filho == null)
                    {
                        _codigoFerragem.RemoveAt(i--);
                    }
                    // Se a Codigo foi alterada
                    else
                    {
                        cf.Codigo = filho.Descricao;
                        filhos.Remove(filho);
                    }
                }

                // Adiciona os codigos novas
                foreach (var filho in filhos)
                    _codigoFerragem.Add(new Projeto.Negocios.Entidades.CodigoFerragem
                    {
                        IdFerragem = filho.IdFerragem,
                        Codigo = filho.Descricao
                    });
            }
            else if (_constanteFerragem != null && _constanteFerragem.Any())
            {
                // Remove as constantes de _constanteFerragem que foram removidas no javaScript
                for (var i = 0; i < _constanteFerragem.Count; i++)
                {
                    var cf = _constanteFerragem[i];

                    var filho = filhos.FirstOrDefault(f => f.IdFilhoFerragem == cf.IdConstanteFerragem);
                    // Se a constante foi removida
                    if (filho == null)
                    {
                        _constanteFerragem.RemoveAt(i--);
                    }
                    // Se a Contante foi alterada
                    else
                    {
                        cf.Nome = filho.Descricao;
                        cf.Valor = filho.Valor;
                        filhos.Remove(filho);
                    }
                }

                // Adiciona as constantes novas
                foreach (var filho in filhos)
                    _constanteFerragem.Add(new Projeto.Negocios.Entidades.ConstanteFerragem
                    {
                        IdFerragem = filho.IdFerragem,
                        Nome = filho.Descricao,
                        Valor = filho.Valor
                    });
            }
            else
            {
                _codigoFerragem = new List<Projeto.Negocios.Entidades.CodigoFerragem>
                    (filhos.Select(f => new Projeto.Negocios.Entidades.CodigoFerragem
                    {
                        Codigo = f.Descricao
                    }));

                _constanteFerragem = new List<Projeto.Negocios.Entidades.ConstanteFerragem>
                    (filhos.Select(f => new Projeto.Negocios.Entidades.ConstanteFerragem
                    {
                        Nome = f.Descricao,
                        Valor = f.Valor
                    }));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlFilhoFerragem_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlFilhoFerragem_script", ResolveUrl("~/Scripts/ctrlFilhoFerragem.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinha('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Carrega os valores passados para os HiddenField da tela
            IEnumerable<int> idsfilhosFerragem = null;
            IEnumerable<string> descricoes = null;
            IEnumerable<double> valores = null;

            if (_codigoFerragem != null && _codigoFerragem.Count > 0)
            {
                idsfilhosFerragem = _codigoFerragem.Select(f => f.IdCodigoFerragem);
                descricoes = _codigoFerragem.Select(f => f.Codigo);
            }
            else if (_constanteFerragem != null && _constanteFerragem.Count > 0)
            {
                idsfilhosFerragem = _constanteFerragem.Select(f => f.IdConstanteFerragem);
                descricoes = _constanteFerragem.Select(f => f.Nome);
                valores = _constanteFerragem.Select(f => f.Valor);
            }
            else
                return;

            hdfIdFerragem.Value = _idFerragem.ToString();
            hdfIdsfilhosFerragem.Value = string.Join(";", idsfilhosFerragem.Select(f => f.ToString()).ToArray());
            hdfDescricoes.Value = string.Join(";", descricoes.Select(f => f.ToString()).ToArray());

            if (valores != null)
                hdfValores.Value = string.Join(";", valores.Select(f => f.ToString()).ToArray());

            // O motivo de colocar DateTime.Now.Ticks é para que force a inserção deste script na página quando tiver mais de um
            // controle deste inserido, como ocorre por exemplo no cadastro de produto
            System.Threading.Thread.Sleep(100);
            Page.ClientScript.RegisterStartupScript(GetType(), "iniciar" + new Random().Next(0, 1000), "carregaFilhoInicial('" +
                this.ClientID + "');", true);
        }

        #endregion
    }
}