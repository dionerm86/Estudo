using System;

namespace Glass.UI.Web.Process.Behaviors
{
    /// <summary>
    /// Classe com métodos de extensão pra trabalhar com a GridView.
    /// </summary>
    static class GridViewExtensions
    {
        /// <summary>
        /// Cria uma linha na grid informada.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="rowIndex"></param>
        /// <param name="dataSourceIndex"></param>
        /// <param name="rowType"></param>
        /// <param name="rowState"></param>
        /// <param name="dataBind"></param>
        /// <param name="dataItem"></param>
        /// <param name="fields"></param>
        /// <param name="rows"></param>
        /// <param name="pagedDataSource"></param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.GridViewRow CreateRow(
            this System.Web.UI.WebControls.GridView gridView,
            int rowIndex, int dataSourceIndex, System.Web.UI.WebControls.DataControlRowType rowType,
            System.Web.UI.WebControls.DataControlRowState rowState, bool dataBind, object dataItem,
            System.Web.UI.WebControls.DataControlField[] fields, System.Web.UI.WebControls.TableRowCollection rows,
            System.Web.UI.WebControls.PagedDataSource pagedDataSource)
        {
            var createRowMethod2 = typeof(System.Web.UI.WebControls.GridView).GetMethod("CreateRow",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null, new Type[] 
                    { 
                        typeof(int),  // rowIndex
                        typeof(int),  // dataSourceIndex
                        typeof(System.Web.UI.WebControls.DataControlRowType), // rowType
                        typeof(System.Web.UI.WebControls.DataControlRowState), // rowState
                        typeof(bool), // dataBind
                        typeof(object), // dataItem,
                        typeof(System.Web.UI.WebControls.DataControlField[]), // fields
                        typeof(System.Web.UI.WebControls.TableRowCollection),  // rows
                        typeof(System.Web.UI.WebControls.PagedDataSource) // pagedDataSource
                    }, null);

            try
            {

                return (System.Web.UI.WebControls.GridViewRow)createRowMethod2.Invoke(gridView, new object[]
                {
                    rowIndex, dataSourceIndex, rowType, rowState, dataBind, dataItem, fields, rows, pagedDataSource
                });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Cria uma linha para a grid.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="rowIndex"></param>
        /// <param name="dataSourceIndex"></param>
        /// <param name="rowType"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.GridViewRow CreateRow(
            this System.Web.UI.WebControls.GridView gridView,
            int rowIndex, int dataSourceIndex,
            System.Web.UI.WebControls.DataControlRowType rowType,
            System.Web.UI.WebControls.DataControlRowState rowState)
        {
            var createRowMethod = typeof(System.Web.UI.WebControls.GridView).GetMethod("CreateRow",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null, new Type[] 
                    { 
                        typeof(int), 
                        typeof(int), 
                        typeof(System.Web.UI.WebControls.DataControlRowType), 
                        typeof(System.Web.UI.WebControls.DataControlRowState) 
                    }, null);

            try
            {

                return (System.Web.UI.WebControls.GridViewRow)createRowMethod.Invoke(gridView, new object[]
                {
                    rowIndex, dataSourceIndex, rowType, rowState
                });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Inicializa a linha da grid.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="row"></param>
        /// <param name="fields"></param>
        public static void InitializeRow(
            this System.Web.UI.WebControls.GridView gridView,
            System.Web.UI.WebControls.GridViewRow row,
            System.Web.UI.WebControls.DataControlField[] fields)
        {
            var initializeRowMethod = typeof(System.Web.UI.WebControls.GridView).GetMethod("InitializeRow",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            try
            {
                initializeRowMethod.Invoke(gridView, new object[]
                {
                    row, fields
                });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Define a linha do rodapé da grid.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="footerRow"></param>
        public static void SetFooterRow(this System.Web.UI.WebControls.GridView gridView, System.Web.UI.WebControls.GridViewRow footerRow)
        {
            try
            {
                // Define a linha do rodapé
                typeof(System.Web.UI.WebControls.GridView)
                    .GetField("_footerRow",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(gridView, footerRow);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Recupera a coleção base das linhas da grid.
        /// </summary>
        /// <param name="gridView"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList GetRows(this System.Web.UI.WebControls.GridView gridView)
        {
            try
            {
                // Define a linha do rodapé
                return (System.Collections.ArrayList)typeof(System.Web.UI.WebControls.GridView)
                    .GetField("_rowsArray",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(gridView);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Cria os controles filhos.
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataBinding"></param>
        public static void CreateChildControls(
            this System.Web.UI.WebControls.GridView gridView,
            System.Collections.IEnumerable dataSource,
            bool dataBinding)
        {
             typeof(System.Web.UI.WebControls.GridView)
                .GetMethod("CreateChildControls",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, new Type[] { typeof(System.Collections.IEnumerable), typeof(bool) }, null);

        }

        /// <summary>
        /// Cria a tabela filha.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.Table CreateChildTable(
            this System.Web.UI.WebControls.GridView grid)
        {
            return (System.Web.UI.WebControls.Table)
               typeof(System.Web.UI.WebControls.GridView)
               .GetMethod("CreateChildTable",
                   System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                   null, new Type[0], null)
               .Invoke(grid, null);
        }
    }
}
