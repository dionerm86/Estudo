using Glass.Data.Helper;
using System.Web;

namespace Glass.Configuracoes
{
    public class Logotipo
    {
        #region Busca logotipo

        private static string CaminhoFisico { get; set; }

        /// <summary>
        /// (Chamado 57264) Retorna o caminho físico da aplicação, método criado para que quando o request estiver indisponível não deixe de fornecer o caminho físico
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ObterCaminhoFisico(HttpRequest request)
        {
            if (string.IsNullOrEmpty(CaminhoFisico))
                CaminhoFisico = request.PhysicalApplicationPath;

            return CaminhoFisico;
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no sistema.
        /// </summary>
        /// <returns></returns>
        public static string GetLogoVirtualPath()
        {
            string arqLogotipo = "logo" + ControleSistema.GetSite() + "Color";

            if (UserInfo.GetUserInfo != null)
                arqLogotipo += UserInfo.GetUserInfo.IdLoja;

            return "~/Images/" + arqLogotipo + ".png";
        }
       
        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo colorido no relatório.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string GetReportLogoColor()
        {
            return GetReportLogoColor(HttpContext.Current.Request);
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no relatório.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string GetReportLogo(uint idLoja)
        {
            return GetReportLogo(HttpContext.Current.Request, idLoja);
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no relatório.
        /// </summary>
        public static string GetReportLogo()
        {
            return GetReportLogo(HttpContext.Current.Request);
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo colorido no relatório.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string GetReportLogoColor(uint idLoja)
        {
            return GetReportLogoColor(HttpContext.Current.Request, idLoja);
        }

        /// <summary>
        /// Retorna o nome do arquivo usado como logotipo colorido no relatório.
        /// </summary>
        public static string GetReportLogoColor(HttpRequest request)
        {
            string arqLogotipo = "logo" + ControleSistema.GetSite() + "Color";

            if (UserInfo.GetUserInfo != null)
                arqLogotipo += UserInfo.GetUserInfo.IdLoja;

            if (EsconderLogotipoRelatorios)
                arqLogotipo = "logoEmBranco";

            return "file:///" + ObterCaminhoFisico(request).Replace('\\', '/') + "Images/" +
                arqLogotipo + ".png";
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no relatório.
        /// </summary>
        public static string GetReportLogo(HttpRequest request)
        {
            return GetReportLogo(request, null);
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no relatório.
        /// </summary>
        /// <param name="request">Requisição do http.</param>
        /// <param name="idLoja">Código da loja, usado para buscar o logotipo.</param>
        public static string GetReportLogo(string diretorio, uint? idLoja)
        {
            var arqLogotipo = "logo" + ControleSistema.GetSite();

            if ((UserInfo.GetUserInfo != null || idLoja.GetValueOrDefault() > 0))
                arqLogotipo += idLoja.GetValueOrDefault() > 0 ? idLoja.Value : UserInfo.GetUserInfo.IdLoja;

            if (EsconderLogotipoRelatorios)
                arqLogotipo = "logoEmBranco";

            return string.Format("file:///{0}", System.IO.Path.Combine(diretorio, arqLogotipo + ".png").Replace("\\", "/"));
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo no relatório.
        /// </summary>
        /// <param name="request">Requisição do http.</param>
        /// <param name="idLoja">Código da loja, usado para buscar o logotipo.</param>
        public static string GetReportLogo(HttpRequest request, uint? idLoja)
        {
            var arqLogotipo = "logo" + ControleSistema.GetSite();

            if ((UserInfo.GetUserInfo != null || idLoja.GetValueOrDefault() > 0))
                arqLogotipo += idLoja.GetValueOrDefault() > 0 ? idLoja.Value : UserInfo.GetUserInfo.IdLoja;

            if (EsconderLogotipoRelatorios)
                arqLogotipo = "logoEmBranco";

            return "file:///" + ObterCaminhoFisico(request).Replace('\\', '/') + "Images/" +
                arqLogotipo + ".png";
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo colorido no relatório.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string GetReportLogoColor(HttpRequest request, uint idLoja)
        {
            string arqLogotipo = "logo" + ControleSistema.GetSite() + "Color";

            if (UserInfo.GetUserInfo != null || idLoja > 0)
                arqLogotipo += idLoja > 0 ? idLoja : UserInfo.GetUserInfo.IdLoja;

            if (EsconderLogotipoRelatorios)
                arqLogotipo = "logoEmBranco";

            return "file:///" + ObterCaminhoFisico(request).Replace('\\', '/') + "Images/" +
                arqLogotipo + ".png";
        }

        /// <summary>
        /// Retorna o nome do arquivo usado como logotipo colorido no orçamento.
        /// </summary>
        public static string GetReportLogoColorOrca(HttpRequest request)
        {
            string path = "file:///" + ObterCaminhoFisico(request).Replace('\\', '/') + "Images/";

            if (EsconderLogotipoRelatorios)
                return path + "logoEmBranco.png";

            path += "logo" + ControleSistema.GetSite() + "Color.png";

            return path;
        }

        /// <summary>
        /// Retorna o caminho do arquivo usado como logotipo do DANFE.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string GetReportLogoNF(HttpRequest request, uint idLoja)
        {
            string arqLogotipo = "logo" + ControleSistema.GetSite() + "Nf";

            if (System.IO.File.Exists(string.Format("{0}Images\\{1}.png", ObterCaminhoFisico(request), arqLogotipo)))
                return "file:///" + ObterCaminhoFisico(request).Replace('\\', '/') + "Images/" + arqLogotipo + ".png";
            else
                return GetReportLogoColor(request, idLoja);
        }

        public static bool EsconderLogotipoRelatorios
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderLogotipoRelatorios); }
        }

        #endregion
    }
}
