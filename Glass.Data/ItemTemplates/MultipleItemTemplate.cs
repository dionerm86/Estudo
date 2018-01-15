using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.Data.ItemTemplates
{
    public class MultipleItemTemplate : ITemplate
    {
        public class Item
        {
            internal string _eval, _acrescimoLabel, _formato, _textoInicial, _textoFinal, _evalVisible;
            internal Type _tipo;

            public Item(string eval, string acrescimoLabel)
                : this(eval, acrescimoLabel, typeof(string), "", "", "", null)
            {
            }

            public Item(string eval, string acrescimoLabel, Type tipo, string formato)
                : this(eval, acrescimoLabel, tipo, formato, "", "", null)
            {
            }

            public Item(string eval, string acrescimoLabel, Type tipo, string formato, string textoInicial, string textoFinal, string evalVisible)
            {
                _eval = eval;
                _acrescimoLabel = acrescimoLabel;
                _tipo = tipo;
                _formato = formato;
                _textoInicial = textoInicial;
                _textoFinal = textoFinal;
                _evalVisible = evalVisible;
            }
        }

        private Item[] _item;

        public MultipleItemTemplate(params Item[] item)
        {
            _item = item;
        }

        public void InstantiateIn(Control container)
        {
            for (int j = 0; j < _item.Length; j++)
            {
                if (_item[j] == null)
                    continue;

                Random r = new Random();

                Label lblItem = new Label();
                lblItem.ID = "lblItem" + _item[j]._eval + _item[j]._acrescimoLabel + r.Next();
                lblItem.Text = j.ToString();
                
                lblItem.DataBinding += new EventHandler(delegate(object sender, EventArgs e)
                {
                    Item i = _item[Glass.Conversoes.StrParaInt(lblItem.Text)];
                    bool isVisible = String.IsNullOrEmpty(i._evalVisible) ? true :
                        Convert.ToBoolean(DataBinder.Eval(((GridViewRow)lblItem.NamingContainer).DataItem, i._evalVisible));

                    if (!isVisible)
                    {
                        lblItem.Visible = false;
                        return;
                    }

                    object data = DataBinder.Eval(((GridViewRow)lblItem.NamingContainer).DataItem, i._eval);
                    string texto = "";

                    try
                    {
                        if (i._tipo != typeof(string))
                        {
                            object item = Activator.CreateInstance(i._tipo);
                            item = data != null && data.ToString() != String.Empty ? Convert.ChangeType(data, i._tipo) : item;

                            texto = item.ToString();
                            if (i._tipo.GetMethod("ToString", new Type[] { typeof(string) }) != null)
                                texto = i._tipo.GetMethod("ToString", new Type[] { typeof(string) }).Invoke(item, new object[] { i._formato }).ToString();
                        }
                        else
                        {
                            texto = data == null || data.ToString() == String.Empty ? String.Empty :
                                !String.IsNullOrEmpty(i._formato) ? String.Format(i._formato, data) : data.ToString();
                        }
                    }
                    catch
                    {
                        texto = data == null || data.ToString() == String.Empty ? String.Empty :
                            !String.IsNullOrEmpty(i._formato) ? String.Format(i._formato, data) : data.ToString();
                    }

                    lblItem.Text = i._textoInicial + texto + i._textoFinal;

                });

                container.Controls.Add(lblItem);
            }
        }
    }
}
