<%@ WebHandler Language="C#" Class="LoadFiguraAssociada" %>

using System;
using System.Web;
using System.Drawing;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

/// <summary>
/// Classe responsável por desenhar medidas nas figuras associadas à modelos de projeto
/// </summary>
public class LoadFiguraAssociada : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            context.Response.ContentType = "image/JPEG";

            float perc = 0;
            string path = context.Request["path"];

            if (!String.IsNullOrEmpty(context.Request["perc"]))
                perc = float.Parse(context.Request["perc"]);

            if (!System.IO.File.Exists(path))
                return;

            using (Bitmap bmp = new Bitmap(path))
            {
                if (bmp.HorizontalResolution == 96)
                    bmp.SetResolution(72, 72);

                using (Graphics grp = Graphics.FromImage(bmp))
                {
                    // Especifica que as medidas serão desenhadas na vertical
                    StringFormat strFrm = new StringFormat(StringFormatFlags.DirectionVertical);
                    strFrm.Alignment = StringAlignment.Center;

                    // Especifica que as medidas serão desenhadas na horizontal
                    StringFormat strFrmHoriz = new StringFormat();
                    strFrmHoriz.Alignment = StringAlignment.Center;

                    // Desenha altura/largura nas peças
                    if (!String.IsNullOrEmpty(context.Request["IdProjetoModelo"]))
                    {
                        // Especifica que as medidas serão desenhadas na vertical
                        StringFormat drawVert = new StringFormat(StringFormatFlags.DirectionVertical);
                        drawVert.Alignment = StringAlignment.Center;
                        drawVert.LineAlignment = StringAlignment.Center;

                        // Especifica que as medidas serão desenhadas na horizontal
                        StringFormat drawHor = new StringFormat();
                        drawHor.Alignment = StringAlignment.Center;
                        drawHor.LineAlignment = StringAlignment.Center;

                        // Verifica se a imagem é completa ou individual
                        if (String.IsNullOrEmpty(context.Request["item"])) // Completa
                        {
                            uint idProjetoModelo = Glass.Conversoes.StrParaUint(context.Request["IdProjetoModelo"]);
                            uint idItemProjeto = Glass.Conversoes.StrParaUint(context.Request["IdItemProjeto"]);
                            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(idItemProjeto);

                            List<PosicaoPecaModelo> lstPosicao = PosicaoPecaModeloDAO.Instance.GetPosicoes(idProjetoModelo);
                            List<PecaItemProjeto> lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(idItemProjeto, idProjetoModelo);

                            if (itemProj == null)
                                return;

                            // Carrega a lista de medidas do modelo de projeto
                            List<MedidaProjetoModelo> lstMedProjMod = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(itemProj.IdProjetoModelo, true);

                            // Desenha as informações cadastradas para este modelo
                            foreach (PosicaoPecaModelo ppm in lstPosicao)
                            {
                                // Gira a referência da imagem para desenhar a coordenada na vertical corretamente
                                if (ppm.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Vertical)
                                {
                                    grp.RotateTransform(-180);
                                    grp.TranslateTransform(0, 0, System.Drawing.Drawing2D.MatrixOrder.Append);
                                    ppm.CoordX *= -1;
                                    ppm.CoordY *= -1;
                                }

                                object calc = null;

                                try
                                {
                                    calc = UtilsProjeto.CalcExpressao(ppm.Calc, itemProj, lstPeca, lstMedProjMod);
                                }
                                catch (Exception ex)
                                {
                                    ErroDAO.Instance.InserirFromException("LoadFiguraAssociada.ashx linha 79", ex);
                                }

                                if (calc != null)
                                {
                                    // Desenha a informação na figura
                                    grp.DrawString(calc.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Black, new PointF(ppm.CoordX, ppm.CoordY), ppm.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Horizontal ? drawHor : drawVert);

                                    // Volta a referência da imagem para a original, caso tenha sido desenhada na vertical
                                    if (ppm.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Vertical)
                                    {
                                        grp.RotateTransform(180);
                                        grp.TranslateTransform(0, 0, System.Drawing.Drawing2D.MatrixOrder.Append);
                                    }
                                }
                            }
                        }
                        else // Peça individual
                        {
                            uint idProjetoModelo = Glass.Conversoes.StrParaUint(context.Request["IdProjetoModelo"]);
                            uint idItemProjeto = Glass.Conversoes.StrParaUint(context.Request["IdItemProjeto"]);
                            int item = Glass.Conversoes.StrParaInt(context.Request["item"]);
                            string numEtiqueta = context.Request["numEtiqueta"];
                            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(idItemProjeto);

                            PecaProjetoModelo peca = PecaProjetoModeloDAO.Instance.GetByItem(idProjetoModelo, item);
                            List<PecaItemProjeto> lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(idItemProjeto, idProjetoModelo);

                            // Carrega a lista de medidas do modelo de projeto
                            List<MedidaProjetoModelo> lstMedProjMod = MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(itemProj.IdProjetoModelo, true);

                            // Obtém a pecaItemProjeto da pecaProjetoModelo passada
                            uint idPecaItemProj = 0;
                            for (int i = 0; i < lstPeca.Count; i++)
                                if (lstPeca[i].Item.Contains(item.ToString()))
                                    idPecaItemProj = lstPeca[i].IdPecaItemProj;

                            // Desenha as informações cadastradas para este modelo
                            foreach (PosicaoPecaIndividual ppi in PosicaoPecaIndividualDAO.Instance.GetPosicoes(peca.IdPecaProjMod, item))
                            {
                                // Gira a referência da imagem para desenhar a coordenada na vertical corretamente
                                if (ppi.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Vertical)
                                {
                                    grp.RotateTransform(-180);
                                    grp.TranslateTransform(0, 0, System.Drawing.Drawing2D.MatrixOrder.Append);
                                    ppi.CoordX *= -1;
                                    ppi.CoordY *= -1;
                                }

                                object calc = null;

                                try
                                {
                                    calc = UtilsProjeto.CalcExpressao(ppi.Calc, itemProj, lstPeca, lstMedProjMod, numEtiqueta);
                                }
                                catch (Exception ex)
                                {
                                    ErroDAO.Instance.InserirFromException("LoadFiguraAssociada.ashx linha 79", ex);
                                }

                                if (calc != null)
                                {
                                    // Desenha a informação na figura
                                    grp.DrawString(calc.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Black, new PointF(ppi.CoordX, ppi.CoordY), ppi.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Horizontal ? drawHor : drawVert);
                                }

                                // Volta a referência da imagem para a original, caso tenha sido desenhada na vertical
                                if (ppi.Orientacao == (int)PosicaoPecaModelo.OrientacaoEnum.Vertical)
                                {
                                    grp.RotateTransform(180);
                                    grp.TranslateTransform(0, 0, System.Drawing.Drawing2D.MatrixOrder.Append);
                                }
                            }

                            // Desenha as figuras cadastradas para esta imagem.
                            foreach (FiguraPecaItemProjeto fig in FiguraPecaItemProjetoDAO.Instance.GetFigurasByPeca(idPecaItemProj, item))
                            {
                                using (Bitmap bmpFig = new Bitmap(Utils.GetFigurasProjetoPath + fig.IdFiguraProjeto + ".jpg"))
                                    grp.DrawImage(bmpFig, new PointF(fig.CoordX, fig.CoordY));
                            }
                        }
                    }
                }

                if (perc > 0)
                {
                    using (Bitmap imageAux = new Bitmap((int)(bmp.Width * perc), (int)(bmp.Height * perc)))
                    using (Graphics grp2 = Graphics.FromImage(imageAux))
                    {
                        grp2.DrawImage(bmp, 0, 0, imageAux.Width, imageAux.Height);
                        imageAux.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else
                    bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        catch (Exception ex)
        {
            ErroDAO.Instance.InserirFromException("LoadFiguraAssociada.ashx", ex);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}