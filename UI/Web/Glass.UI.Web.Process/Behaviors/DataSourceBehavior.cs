using System;
using System.Linq;

namespace Glass.UI.Web.Process.Behaviors
{
    /// <summary>
    /// Implementação do comportamento do DataSource.
    /// </summary>
    public class DataSourceBehavior
    {
        #region Variáveis locais

        private Colosoft.WebControls.VirtualObjectDataSource _dataSource;
        private System.Web.UI.WebControls.GridView _gridView;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="dataSource">Instancia do DataSource que será configurado.</param>
        public DataSourceBehavior(Colosoft.WebControls.VirtualObjectDataSource dataSource, System.Web.UI.WebControls.GridView gridViewComFooter)
        {
            _dataSource = dataSource;
            _gridView = gridViewComFooter;

            if (_gridView != null)
                dataSource.Selected += dataSource_Selected;

            dataSource.Inserted += new Colosoft.WebControls.VirtualObjectDataSourceStatusEventHandler(dataSource_Inserted);
            dataSource.Updated += new Colosoft.WebControls.VirtualObjectDataSourceStatusEventHandler(dataSource_Updated);
            dataSource.Deleted += new Colosoft.WebControls.VirtualObjectDataSourceStatusEventHandler(dataSource_Deleted);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Método acionado quando os dados forem selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSource_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is System.Collections.IEnumerable) {
                var dataSource = sender as Colosoft.WebControls.VirtualObjectDataSourceView;
                var temItens = (e.ReturnValue as System.Collections.IEnumerable).Cast<object>().Any();

                if (!temItens && dataSource != null) {
                    var tipo = System.Web.Compilation.BuildManager.GetType(dataSource.DataObjectTypeName, false);
                    if (tipo == null)
                        return;

                    var item = Activator.CreateInstance(tipo);
                    if (item == null)
                        return;

                    (e as Colosoft.WebControls.IVirtualObjectDataSourceStatusEventArgsExtended)
                        .ChangeReturnValue(new[] { item });

                    _gridView.DataBound += (s, ea) => _gridView.Rows[0].Visible = false;
                }
            }
        }

        /// <summary>
        /// Método acionado quando os dados forem apagados pelo DataSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSource_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is Colosoft.Business.DeleteResult) {
                var deleteResult = (Colosoft.Business.DeleteResult)e.ReturnValue;

                if (!deleteResult)
                    throw new Exception(deleteResult.Message.Format());
            }
        }

        /// <summary>
        /// Método acionado quando os dados forem atualizados pelo DataSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSource_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is Colosoft.Business.SaveResult) {
                var saveResult = (Colosoft.Business.SaveResult)e.ReturnValue;

                if (!saveResult)
                    throw new Exception(saveResult.Message.Format());
            }
        }

        /// <summary>
        /// Método acionado quando os dados forem inseridos pelo DataSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSource_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is Colosoft.Business.SaveResult) {
                var saveResult = (Colosoft.Business.SaveResult)e.ReturnValue;

                if (!saveResult)
                    throw new Exception(saveResult.Message.Format());
            }
        }

        #endregion
    }
}
