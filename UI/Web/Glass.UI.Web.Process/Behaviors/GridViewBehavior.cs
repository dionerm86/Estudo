using System;
using Colosoft;

namespace Glass.UI.Web.Process.Behaviors
{
    /// <summary>
    /// Classe que trata o comportamento de uma GridView.
    /// </summary>
    class GridViewBehavior
    {
        #region Local Variables

        private System.Web.UI.WebControls.GridView _gridView;
        private bool _showFooter;
        private bool _showHeader;
        private System.Web.UI.WebControls.Table _gridTable;

        #endregion

        #region Properties

        /// <summary>
        /// Identifica se é para exibir o rodapé.
        /// </summary>
        public bool ShowFooter
        {
            get { return _showFooter; }
        }

        /// <summary>
        /// Identifica se é para exibir o cabeçalho.
        /// </summary>
        public bool ShowHeader
        {
            get { return _showHeader; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="gridView"></param>
        public GridViewBehavior(System.Web.UI.WebControls.GridView gridView, bool showHeader, bool showFooter)
        {
            gridView.Require("gridView");
            _gridView = gridView;
            _showHeader = showHeader;
            _showFooter = showFooter;

            if (_showFooter)
                _gridView.ShowFooter = true;

            if (_showHeader)
                _gridView.ShowHeader = true;

            Initialize(_gridView);

            gridView.Page.Load += Page_Load;
            gridView.RowDeleted += RowDeleted;
            gridView.RowUpdated += RowUpdated;
            gridView.DataBound += DataBound;
            gridView.Sorted += Sorted;
            gridView.PreRender += PreRender;
            gridView.RowUpdating += RowUpdating;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Método acionado quando a página associada a grid for carregada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            object state = null;
            int count = 0;

            if ((ShowHeader || ShowFooter) &&
                _gridView.Rows.Count == 0 &&
                (state = _gridView.GetViewState()["_!ItemCount"]) != null &&
                 int.TryParse(state.ToString(), out count) &&
                 count == 0)
            {
                _gridView.Controls.Clear();
                _gridView.Controls.Add(_gridTable);
            }
        }

        /// <summary>
        /// Método acionado quando a grid for iniciada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Initialize(System.Web.UI.WebControls.GridView grid)
        {
            if (ShowHeader || ShowFooter)
            {
                var table = grid.CreateChildTable();

                var fields = new System.Web.UI.WebControls.DataControlField[grid.Columns.Count];
                grid.Columns.CopyTo(fields, 0);

                if (ShowHeader)
                {
                    // cria a linha do cabeçalho
                    var headerRow = grid.CreateRow(-1, -1, 
                        System.Web.UI.WebControls.DataControlRowType.Header, 
                        System.Web.UI.WebControls.DataControlRowState.Normal);

                    grid.InitializeRow(headerRow, fields);
                    table.Rows.Add(headerRow);
                }

                //create the empty row
                var emptyRow = new System.Web.UI.WebControls.GridViewRow(-1, -1,
                    System.Web.UI.WebControls.DataControlRowType.EmptyDataRow,
                    System.Web.UI.WebControls.DataControlRowState.Normal);

                var cell = new System.Web.UI.WebControls.TableCell();
                cell.ColumnSpan = grid.Columns.Count;
                cell.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                if (!String.IsNullOrEmpty(grid.EmptyDataText))
                    cell.Controls.Add(new System.Web.UI.LiteralControl(grid.EmptyDataText));


                if (grid.EmptyDataTemplate != null)
                    grid.EmptyDataTemplate.InstantiateIn(cell);

                emptyRow.Cells.Add(cell);
                table.Rows.Add(emptyRow);

                if (ShowFooter)
                {
                    // Cria a linha do rodapé
                    var footerRow = grid.CreateRow(-1, -1,
                        System.Web.UI.WebControls.DataControlRowType.Footer,
                        System.Web.UI.WebControls.DataControlRowState.Normal,
                        false,
                        null,
                        fields,
                        table.Rows,
                        null);

                    footerRow.ID = grid.ID + "_footer";

                    // Define a linha do rodapé
                    grid.SetFooterRow(footerRow);
                }

                _gridTable = table;
            }
        }

        /// <summary>
        /// Método acionado quando ocorre o DataBound da Grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataBound(object sender, EventArgs e)
        {
            var grid = (System.Web.UI.WebControls.GridView)sender;

            // Verifica se está editando alguma linha da grid
            if (grid.EditIndex >= 0)
            {
                // Verifica se possui o footer definido
                if (grid.FooterRow != null)
                {
                    // Cria a linha do footer
                    var row = new System.Web.UI.WebControls.GridViewRow(-1, -1, System.Web.UI.WebControls.DataControlRowType.Footer, System.Web.UI.WebControls.DataControlRowState.Normal);
                    for(var i = 0; i < grid.Columns.Count; i++)
                        row.Cells.Add(new System.Web.UI.WebControls.TableCell());


                    System.Web.UI.WebControls.Table childTable = null;

                    // Localiza o controle que representa a table a Grid
                    for(var i = 0; i < grid.Controls.Count; i++)
                        if (grid.Controls[i] is System.Web.UI.WebControls.Table)
                        {
                            childTable = grid.Controls[i] as System.Web.UI.WebControls.Table;
                            break;
                        }

                    if (childTable != null && childTable.Rows != null)
                        for(var i = 0; i < childTable.Rows.Count; i++)
                        {
                            // Localiza o 
                            if (childTable.Rows[i] == grid.FooterRow)
                            {
                                childTable.Rows.RemoveAt(i);
                                childTable.Rows.AddAt(i, row);
                                break;
                            }
                        }


                    grid.SetFooterRow(row);
                }
            }
        }

        /// <summary>
        /// Método acionado quando a grid for pré-renderizada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreRender(object sender, EventArgs e)
        {
            // Verifica se a GridView está com o template de nenhuma linha encontrada
            if ((ShowHeader || ShowFooter) &&
                ((_gridView.Rows.Count == 1 &&
                 _gridView.Rows[0].Cells.Count == 1) ||
                 _gridView.Rows.Count == 0) &&
                !_gridView.Controls.Contains(_gridTable))
            {
                _gridView.Controls.Clear();
                _gridView.Controls.Add(_gridTable);
            }
        }

        /// <summary>
        /// Método acionado quando a linha estiver sendo atualizada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            // Não faz nada
        }

        /// <summary>
        /// Método acionado quando a linha da grid for atualizada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowUpdated(object sender, System.Web.UI.WebControls.GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                var validator = new BehaviorValidator();
                validator.IsValid = false;

                var exception = e.Exception;

                if (exception is System.Reflection.TargetInvocationException)
                    exception = exception.InnerException;

                validator.ErrorMessage = exception.Message;
                _gridView.Page.Validators.Add(validator);

                e.KeepInEditMode = true;
                e.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Método acionado quando uma linha for apagada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowDeleted(object sender, System.Web.UI.WebControls.GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                var validator = new BehaviorValidator();
                validator.IsValid = false;

                var exception = e.Exception;

                if (exception is System.Reflection.TargetInvocationException)
                    exception = exception.InnerException;

                validator.ErrorMessage = exception.Message;
                _gridView.Page.Validators.Add(validator);

                e.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Método acionado quando a ordenação é alterada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sorted(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_gridView.SortExpression))
                return;

            var sort = new[] { '▲', '▼' };
            
            foreach (System.Web.UI.WebControls.DataControlField c in _gridView.Columns)
            {
                c.HeaderText = c.HeaderText.TrimEnd(' ', sort[0], sort[1]);

                if (c.SortExpression == _gridView.SortExpression)
                    c.HeaderText += String.Format(" {0}", 
                        _gridView.SortDirection == System.Web.UI.WebControls.SortDirection.Ascending ? 
                            sort[0] : 
                            sort[1]);
            }
        }

        #endregion
    }
}
