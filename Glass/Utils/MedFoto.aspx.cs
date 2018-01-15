using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class MedFoto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FotosMedicao foto = FotosMedicaoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idFoto"]));
    
            imgFoto.ImageUrl = "../Handlers/LoadImage.ashx?altura=567&largura=756&path=" +
                Data.Helper.Utils.GetFotosMedicaoPath + "\\Med_" + foto.IdMedicao + "_Foto_" + foto.IdFoto + ".jpg";
            
            //imgFoto.ImageUrl = "../Handlers/LoadImage.ashx?altura=567&largura=756&path=" + 
            //    Utils.GetFotosMedicaoPhysicalPath(HttpContext.Current) + "\\Med_1_Foto_1.jpg";
    
            hdfIdFoto.Value = foto.IdFoto.ToString();
    
            #region Busca os pontos e a escala desta figura se houver
    
            // Zera hidden fields que contém pontos da figura e da escala
            hdfPontosEscala.Value = String.Empty;
            hdfPontosFigura.Value = String.Empty;
    
            // Carrega pontos da figura
            IList<PontoFotoMedicao> lstPontos = PontoFotoMedicaoDAO.Instance.GetByFoto(foto.IdFoto);
            foreach (PontoFotoMedicao ponto in lstPontos)
                hdfPontosFigura.Value += ponto.CoordX + ";" + ponto.CoordY + "|";
            hdfPontosFigura.Value.TrimEnd('|');
    
            // Carrega os dois pontos da Escala
            hdfPontosEscala.Value = foto.EscalaP1X + ";" + foto.EscalaP1Y + ";" + foto.EscalaP2X + ";" + foto.EscalaP2Y;
    
            // Carrega o valor da escala
            if (foto.Escala > 0)
                txtEscala.Text = foto.Escala.ToString();
    
            #endregion
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.MedFoto));
        }
    
        /// <summary>
        /// Salva os pontos utilizados no cálculo da área da foto
        /// </summary>
        /// <param name="idFoto"></param>
        /// <param name="pontos"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string SalvarPontos(string idFoto, string area, string metroLinear, string escala, string escalaP1, string escalaP2, string pontos)
        {
            try
            {
                // Atualiza dados da foto
                FotosMedicao foto = FotosMedicaoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idFoto));
                foto.AreaQuadrada = Single.Parse(area.Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                foto.MetroLinear = Single.Parse(metroLinear.Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                foto.EscalaP1X = Glass.Conversoes.StrParaInt(escalaP1.Split(';')[0]);
                foto.EscalaP1Y = Glass.Conversoes.StrParaInt(escalaP1.Split(';')[1]);
                foto.EscalaP2X = Glass.Conversoes.StrParaInt(escalaP2.Split(';')[0]);
                foto.EscalaP2Y = Glass.Conversoes.StrParaInt(escalaP2.Split(';')[1]);
                foto.Escala = Glass.Conversoes.StrParaInt(escala);
                FotosMedicaoDAO.Instance.Update(foto);
    
                // Insere pontos da foto
                PontoFotoMedicaoDAO.Instance.DeleteByFoto(foto.IdFoto);
                PontoFotoMedicaoDAO.Instance.SalvarPontos(foto.IdFoto, pontos);
    
                return "ok\tÁrea quadrada salva com sucesso.";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar área quadrada.", ex);
            }
        }
    }
}
