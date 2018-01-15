using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlImagemPopup : BaseUserControl
    {
        #region Propriedades
    
        private Unit _maxSize = new Unit("300px");
    
        public string ImageUrl
        {
            get { return imgImagem.ImageUrl; }
            set { imgImagem.ImageUrl = value; }
        }
    
        public Unit MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }
    
        public string Legenda
        {
            get { return lblLegenda.Text; }
            set { lblLegenda.Text = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("Tooltip", ResolveClientUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
            Page.PreRender += new EventHandler(Page_PreRender);
        }
    
        private void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                int altura = 0;
                int largura = 0;

                if (String.IsNullOrEmpty(imgImagem.ImageUrl))
                {
                    imgIcone.Visible = false;
                    divImagem.Visible = false;
                    imgImagem.Visible = false;
                    return;
                }

                // Se a abertura da imagem for direto do arquivo físico, altera para abrir pelo handler, 
                // impedindo que imagem fique em cache. "../../" é para visualizar nas empresas
                if (!imgImagem.ImageUrl.ToLower().Contains("/handlers/"))
                {
                    string path = HttpContext.Current.Server.MapPath(imgImagem.ImageUrl);
                    if (!File.Exists(path))
                    {
                        imgIcone.Visible = false;
                        divImagem.Visible = false;
                        imgImagem.Visible = false;
                        return;
                    }

                    imgImagem.ImageUrl = "~/Handlers/LoadImage.ashx?path=" + path + "&resize=false&cache=" + DateTime.Now.Ticks;
                }

                if (imgImagem.ImageUrl.Contains("~/"))
                {
                    string[] path = imgImagem.ImageUrl.Split('?');
                    imgImagem.ImageUrl = Page.ResolveClientUrl(path[0]) + (path.Length > 1 ? "?" + path[1] : "");
                }

                imgImagem.ImageUrl = imgImagem.ImageUrl.Replace("../~/", "../");
                imgImagem.ImageUrl = imgImagem.ImageUrl.Replace("../../", "../");

                byte[] dadosImagem = Data.Helper.Utils.GetImageFromRequest(imgImagem.ImageUrl);
                if (dadosImagem.Length > 0)
                {
                    int alturaImagem = 0;
                    int larguraImagem = 0;

                    using (MemoryStream m = new MemoryStream(dadosImagem))
                    {
                        using (Bitmap b = (Bitmap)Bitmap.FromStream(m))
                        {
                            alturaImagem = b.Height;
                            larguraImagem = b.Width;

                            using (var temp = b.Redimensionar((int)_maxSize.Value, (int)_maxSize.Value, 1))
                            {
                                altura = temp.Height;
                                largura = temp.Width;
                            }
                        }
                    }

                    imgIcone.Attributes.Add("onmouseover", "TagToTip('" + divImagem.ClientID + "', FADEIN, 200, COPYCONTENT, false);");
                    imgIcone.Attributes.Add("onmouseout", "UnTip()");

                    imgIcone.OnClientClick = "openWindow(600, 800, '" + ResolveClientUrl("~/Utils/ShowFoto.aspx") + "?path=" +
                     imgImagem.ImageUrl.Replace("../../", "../").Replace("?", "$").Replace("&", "@").Replace("\\", "!") + "'); return false;";

                    if (alturaImagem > _maxSize.Value)
                        imgImagem.Attributes.Add("Height", altura + "px");

                    if (larguraImagem > _maxSize.Value)
                        imgImagem.Attributes.Add("Width", largura + "px");
                }
                else
                {
                    imgIcone.Visible = false;
                    divImagem.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("ctrlImagemPopup", ex);

                imgIcone.Visible = false;
                divImagem.Visible = false;
            }
        }
    }
}
