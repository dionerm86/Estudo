﻿using Glass.Configuracoes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlImageCadProject : System.Web.UI.UserControl
    {
        #region Propiedades

        public int? IdProdPedEsp { get; set; }

        public bool DiminuirMedidasPopUp { get; set; }

        public string Legenda
        {
            get { return lblLegenda.Text; }
            set { lblLegenda.Text = value; }
        }

        #endregion

        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("Tooltip", ResolveClientUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
            Page.PreRender += Page_PreRender;
        }

        #endregion

        #region Métodos Publicos

        public void Page_PreRender(object sender, EventArgs e)
        {
            imgIcone.OnClientClick = string.Format("openWindow(Math.round(window.screen.height * {0}), Math.round(window.screen.width * {0}), '{1}?idProdPedEsp={2}&cache={3}'); return false;",
                DiminuirMedidasPopUp ? "0.45" : "1", ResolveClientUrl("~/Handlers/LoadSvg.ashx"), IdProdPedEsp.GetValueOrDefault(), DateTime.Now.Ticks);

            if (IdProdPedEsp.GetValueOrDefault(0) > 0)
            {
                var caminho = PCPConfig.CaminhoSalvarCadProject(true) + IdProdPedEsp.Value + ".svg";

                if (!File.Exists(caminho))
                    return;

                var data = File.ReadAllText(caminho);
                divSvg.InnerHtml = MontaDivSvg(data);

                imgIcone.Attributes.Add("onmouseover", "TagToTip('" + divImagem.ClientID + "', FADEIN, 200, COPYCONTENT, false);");
                imgIcone.Attributes.Add("onmouseout", "UnTip()");
            }
        }

        #endregion

        #region Métodos Privados

        private string MontaDivSvg(string svg)
        {
            ////Caminho do template para exibir o SVG
            //var caminhoTemplate = HttpContext.Current.Server.MapPath("~/Misc/") + "SvgTemplate.html";

            ////Caminho do js responsavel por dar o zoom
            //var jsPamZoom = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //jsPamZoom = jsPamZoom + "/Scripts/svg-pan-zoom.js";

            ////Carrega o template
            //var template = File.ReadAllText(caminhoTemplate);

            ////Adiciona o caminho do js
            //template = template.Replace("{0}", jsPamZoom);

            ////Adiciona o SVG
            //template = template.Replace("{1}", svg);

            //return template;

            return @"<div>"+svg+"</div>";
        }

        #endregion

    }
}