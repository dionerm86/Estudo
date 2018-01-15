namespace Glass.UI.Web
{
    /// <summary>
    /// Classe com os métodos de extensão para os controles ASP.NET.
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Mostra a mensagem de erro contida no resultado da operação.
        /// </summary>
        /// <param name="resultado"></param>
        /// <param name="control"></param>
        /// <param name="mensagemAnexo"></param>
        public static void MostrarErro(this System.Web.UI.Control control, Colosoft.Business.SaveResult resultado, string mensagemAnexo = null)
        {
            if (resultado == null) return;

            var validator = new System.Web.UI.WebControls.CustomValidator();
            validator.IsValid = false;

            if (!string.IsNullOrEmpty(mensagemAnexo))
                validator.ErrorMessage = string.Format("{0} - {1}", mensagemAnexo, resultado.Message.Format());
            else
                validator.ErrorMessage = resultado.Message.Format();

            control.Page.Validators.Add(validator);
        }

        /// <summary>
        /// Mostra a mensagem de erro contida no resultado da operação.
        /// </summary>
        /// <param name="resultado"></param>
        /// <param name="control"></param>
        /// <param name="mensagemAnexo"></param>
        public static void MostrarErro(this System.Web.UI.Control control, Colosoft.Business.DeleteResult resultado, string mensagemAnexo = "")
        {
            if (resultado == null) return;

            var validator = new System.Web.UI.WebControls.CustomValidator();
            validator.IsValid = false;

            if (!string.IsNullOrEmpty(mensagemAnexo))
                validator.ErrorMessage = string.Format("{0} - {1}", mensagemAnexo, resultado.Message.Format());
            else
                validator.ErrorMessage = resultado.Message.Format();

            control.Page.Validators.Add(validator);
        }

        /// <summary>
        /// Registra o comportamento para o details View.
        /// </summary>
        /// <param name="detailsView"></param>
        /// <param name="successUrl"></param>
        public static void Register(this System.Web.UI.WebControls.DetailsView detailsView, string successUrl = null)
        {
            new Web.Process.Behaviors.DetailsViewBehavior(detailsView, successUrl);
        }

        /// <summary>
        /// Registra o comportamento para a Grid.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="showHeader">Identifica se é para sempre exibir o cabeçalho.</param>
        /// <param name="showFooter">Identifica se é para sempre exibir o rodapé.</param>
        public static void Register(this System.Web.UI.WebControls.GridView gridView, bool showHeader = false, bool showFooter = false)
        {
            new Web.Process.Behaviors.GridViewBehavior(gridView, showHeader, showFooter);
        }

        /// <summary>
        /// Registra o comportamento para o DataSource.
        /// </summary>
        /// <param name="dataSource"></param>
        public static void Register(this Colosoft.WebControls.VirtualObjectDataSource dataSource, System.Web.UI.WebControls.GridView gridViewComFooter = null)
        {
            new Web.Process.Behaviors.DataSourceBehavior(dataSource, gridViewComFooter);
        }
    }
}
