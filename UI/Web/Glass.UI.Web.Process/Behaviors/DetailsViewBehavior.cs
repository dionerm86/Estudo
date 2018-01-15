using System;
using Colosoft;

namespace Glass.UI.Web.Process.Behaviors
{
    /// <summary>
    /// Representa o comportamento que trata os detailsview.
    /// </summary>
    class DetailsViewBehavior
    {
        #region Local Variables

        private System.Web.UI.WebControls.DetailsView _detailsView;
        private string _successUrl;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="detailsView"></param>
        /// <param name="successUrl"></param>
        public DetailsViewBehavior(
            System.Web.UI.WebControls.DetailsView detailsView,
            string successUrl)
        {
            detailsView.Require("detailsView");
            _detailsView = detailsView;
            _successUrl = successUrl;

            detailsView.ItemInserted += ItemInserted;
            detailsView.ItemUpdated += ItemUpdated;

            var page = detailsView.Page;

            if (System.Web.HttpContext.Current.Request.Form["defaultmode"] != null)
                detailsView.DefaultMode = (System.Web.UI.WebControls.DetailsViewMode)
                    Enum.Parse(typeof(System.Web.UI.WebControls.DetailsViewMode), System.Web.HttpContext.Current.Request.Form["defaultmode"]);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Método acionado quando um item for inserido pelo details view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemInserted(object sender, System.Web.UI.WebControls.DetailsViewInsertedEventArgs e)
        {
            if (e.Exception == null && !string.IsNullOrEmpty(_successUrl))
                System.Web.HttpContext.Current.Response.Redirect(_detailsView.ResolveUrl(_successUrl));

            else if (e.Exception != null)
            {
                var validator = new BehaviorValidator();
                validator.IsValid = false;

                var exception = e.Exception;
                if (exception is System.Reflection.TargetInvocationException)
                    exception = exception.InnerException;

                validator.ErrorMessage = exception.Message;
                _detailsView.Page.Validators.Add(validator);

                e.ExceptionHandled = true;
                e.KeepInInsertMode = true;
            }
        }

        /// <summary>
        /// Método acionado quando um item for alterado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemUpdated(object sender, System.Web.UI.WebControls.DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception == null && !string.IsNullOrEmpty(_successUrl))
                System.Web.HttpContext.Current.Response.Redirect(_detailsView.ResolveUrl(_successUrl));

            else if (e.Exception != null)
            {
                var validator = new BehaviorValidator();
                validator.IsValid = false;

                var exception = e.Exception;
                if (exception is System.Reflection.TargetInvocationException)
                    exception = exception.InnerException;

                validator.ErrorMessage = exception.Message;
                _detailsView.Page.Validators.Add(validator);

                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        #endregion
    }
}
