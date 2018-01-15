namespace Glass.UI.Web.Process.Behaviors
{
    /// <summary>
    /// Classe com os métodos de extensão dos controles.
    /// </summary>
    static class ControlExtensions
    {
        /// <summary>
        /// Recupera o viewstate do controle.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static System.Web.UI.StateBag GetViewState(this System.Web.UI.Control control)
        {
            return (System.Web.UI.StateBag)
                typeof(System.Web.UI.Control).GetProperty("ViewState",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(control, null);
        }
    }
}
