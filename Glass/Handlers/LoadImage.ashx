<%@ WebHandler Language="C#" Class="LoadImage" %>

using System;
using System.Web;
using System.Drawing;
using Glass.Data.RelDAL;

public class LoadImage : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        try
        {
            context.Response.ContentType = "image/JPEG";

            string path = context.Request["path"];
            float perc = !String.IsNullOrEmpty(context.Request["perc"]) ? float.Parse(context.Request["perc"].Replace(".", ",")) : 1;
            int largura = (int)((!String.IsNullOrEmpty(context.Request["largura"]) ? Glass.Conversoes.StrParaInt(context.Request["largura"]) : 160) * perc);
            int altura = (int)((!String.IsNullOrEmpty(context.Request["altura"]) ? Glass.Conversoes.StrParaInt(context.Request["altura"]) : 160) * perc);
            bool resize = context.Request["resize"] != "false";
            
            using (Bitmap imageOri = new Bitmap(path))
            {
                if (resize)
                    using (Image imageAux = Glass.ManipulacaoImagem.Redimensionar(imageOri, altura, largura, perc))
                        imageAux.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                else
                    imageOri.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        catch { }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}