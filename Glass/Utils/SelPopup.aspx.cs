using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SelPopup : System.Web.UI.Page
    {
        #region Métodos privados
    
        private Type _tipo;
    
        protected static string Decode(string s)
        {
            if (s == null)
                return "";
    
            return Encoding.Default.GetString(Convert.FromBase64String(s));
        }
    
        private bool IsColunaBool(string nomeColuna, string tipoDAO, string nomeMetodo, TypeCode[] tiposParametros)
        {
            var flags = System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy;
    
            if (_tipo == null)
            {
                _tipo = System.Web.Compilation.BuildManager.GetType(tipoDAO, false, true);
                if (_tipo != null)
                {
                    var metodo = _tipo.GetMethod(nomeMetodo, flags, null, ToType(tiposParametros), null);
                    if (metodo != null)
                    {
                        _tipo = metodo.ReturnType;
    
                        if (_tipo.IsGenericType)
                            _tipo = _tipo.GetGenericArguments()[0];
                    }
                }
            }
    
            if (_tipo == null) return false;
    
            var prop = _tipo.GetProperty(nomeColuna, flags);
            return prop != null && prop.PropertyType.IsAssignableFrom(typeof(bool));
        }
    
        private static Type[] ToType(TypeCode[] typeCode)
        {
            List<Type> retorno = new List<Type>();
    
            foreach (var code in typeCode)
                switch (code)
                {
                    case TypeCode.Boolean:
                        retorno.Add(typeof(bool));
                        break;
    
                    case TypeCode.Byte:
                        retorno.Add(typeof(byte));
                        break;
    
                    case TypeCode.Char:
                        retorno.Add(typeof(char));
                        break;
    
                    case TypeCode.DateTime:
                        retorno.Add(typeof(DateTime));
                        break;
    
                    case TypeCode.Decimal:
                        retorno.Add(typeof(decimal));
                        break;
    
                    case TypeCode.Double:
                        retorno.Add(typeof(double));
                        break;
    
                    case TypeCode.Int16:
                        retorno.Add(typeof(short));
                        break;
    
                    case TypeCode.Int32:
                        retorno.Add(typeof(int));
                        break;
    
                    case TypeCode.Int64:
                        retorno.Add(typeof(long));
                        break;
    
                    case TypeCode.Object:
                        retorno.Add(typeof(object));
                        break;
    
                    case TypeCode.SByte:
                        retorno.Add(typeof(sbyte));
                        break;
    
                    case TypeCode.Single:
                        retorno.Add(typeof(float));
                        break;
    
                    case TypeCode.String:
                        retorno.Add(typeof(string));
                        break;
    
                    case TypeCode.UInt16:
                        retorno.Add(typeof(ushort));
                        break;
    
                    case TypeCode.UInt32:
                        retorno.Add(typeof(uint));
                        break;
    
                    case TypeCode.UInt64:
                        retorno.Add(typeof(ulong));
                        break;
                }
    
            return retorno.ToArray();
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = Request["tituloTela"];
            
            #region Configuração dos filtros
    
            string[] colunasFiltro = Decode(Request["colunasFiltro"]).Split('|');
            string[] titulosColunasFiltro = Decode(Request["titulosColunasFiltro"]).Split('|');
            string[] numeroFiltrosLinha = Decode(Request["numeroFiltrosLinha"]).Split('|');
            string[] dataSourcesIDsFiltros = Decode(Request["dataSourcesIDsFiltros"]).Split('|');
    
            int numeroFiltros = Math.Min(colunasFiltro.Length, Math.Min(titulosColunasFiltro.Length, dataSourcesIDsFiltros.Length));
            
            filtros.Visible = numeroFiltros > 1 || colunasFiltro[0] != "";
    
            #endregion
    
            #region Configura o ObjectDataSource
    
            string[] nomeMetodo = Decode(Request["nomeMetodo"]).Split(',');
            odsBuscar.TypeName = nomeMetodo[0].Trim();
            odsBuscar.SelectMethod = nomeMetodo[1].Trim();
    
            _tipo = null;
    
            if (nomeMetodo.Length == 3)
                odsBuscar.SelectCountMethod = nomeMetodo[2].Trim();
            else
            {
                odsBuscar.EnablePaging = false;
                odsBuscar.SortParameterName = "";
                grdBuscar.AllowSorting = false;
            }
    
            odsBuscar.SelectParameters.Clear();
    
            string[] parametros = Request["parametrosReais"] == "false" ? Decode(Request["parametros"]).Split('|') : 
                Request["parametros"].Split('|');
    
            foreach (string param in parametros)
            {
                if (String.IsNullOrEmpty(param))
                    continue;
    
                string[] dadosParam = param.Split(':');
    
                int indexParam;
                if (filtros.Visible && (indexParam = Array.IndexOf(colunasFiltro, dadosParam[0])) > -1 && indexParam < numeroFiltros)
                {
                    ControlParameter p = new ControlParameter();
                    p.Name = dadosParam[0];
                    p.ControlID = "ctrlFiltro_" + indexParam;
    
                    if (dadosParam.Length > 2)
                        p.Type = (TypeCode)Glass.Conversoes.StrParaInt(dadosParam[2]);
                    
                    odsBuscar.SelectParameters.Add(p);
                }
                else
                {
                    Parameter p = new Parameter();
                    p.Name = dadosParam[0];
                    p.DefaultValue = dadosParam[1];
                    
                    if (dadosParam.Length > 2)
                        p.Type = (TypeCode)Glass.Conversoes.StrParaInt(dadosParam[2]);
    
                    odsBuscar.SelectParameters.Add(p);
                }
            }
    
            #endregion
    
            if (!IsPostBack)
            {
                #region Cria os filtros
    
                if (filtros.Visible)
                {
                    int numeroControles = 0, numeroLinha = -1;
                    TableRow linha = null;
    
                    for (int i = 0; i < numeroFiltros; i++)
                    {
                        int numeroControlesLinha;
                        
                        if (numeroLinha == -1 || (int.TryParse(numeroFiltrosLinha[numeroLinha > -1 ? numeroLinha : 0],
                            out numeroControlesLinha) && numeroControles == numeroControlesLinha))
                        {
                            linha = new TableRow();
                            tblFiltros.Rows.Add(linha);
    
                            numeroControles = 0;
                            numeroLinha++;
                        }
    
                        #region Coloca o label do filtro
    
                        TableCell label = new TableCell();
    
                        Label l = new Label();
                        l.ID = "lblFiltro_" + i;
                        l.Text = titulosColunasFiltro[i];
                        l.ForeColor = System.Drawing.Color.FromArgb(0, 0x66, 0xFF);
    
                        label.Controls.Add(l);
                        linha.Cells.Add(label);
    
                        #endregion
    
                        #region Coloca o controle do filtro
    
                        TableCell controle = new TableCell();
                        Control c = null;
    
                        if (String.IsNullOrEmpty(dataSourcesIDsFiltros[i]))
                        {
                            TextBox t = new TextBox();
                            t.ID = "ctrlFiltro_" + i;
                        }
                        else
                        {
                            DropDownList d = new DropDownList();
                            d.ID = "ctrlFiltro_" + i;
                            d.Items.Add(new ListItem());
                            d.AppendDataBoundItems = true;
                            //d.DataSource = ;
                        }
    
                        controle.Controls.Add(c);
                        linha.Cells.Add(controle);
    
                        #endregion
    
                        #region Coloca o botão de pesquisa
    
                        TableCell botao = new TableCell();
                        
                        ImageButton b = new ImageButton();
                        b.ID = "imgPesq_" + i;
                        b.ImageUrl = "~/Images/Pesquisar.gif";
                        b.ToolTip = "Pesquisar";
                        b.Click += imgPesq_Click;
    
                        botao.Controls.Add(b);
                        linha.Cells.Add(botao);
    
                        #endregion
                    }
                }
    
                #endregion
    
                #region Configura a GridView
    
                bool exibirId = Request["exibirId"] != null ? Request["exibirId"].ToLower() == "true" : false;
                string[] colunas = Request["colunasExibir"] != null ? Decode(Request["colunasExibir"]).Split('|') : null;
    
                List<BoundField> campos = new List<BoundField>();
    
                if (exibirId)
                {
                    BoundField id = new BoundField();
                    id.DataField = Decode(Request["colunaId"]);
                    campos.Add(id);
                }
    
                foreach (string c in colunas)
                {
                    if (String.IsNullOrEmpty(c))
                        continue;
    
                    // Comentado por conta da necessidade de sempre exibir o campo descrição
                    // porque é o campo que permite a busca direta pelo controle sem abrir o popup
                    if (c.Trim().ToLower() != Decode(Request["colunaId"]).ToLower() /*&& (colunas.Length <= 2 ||
                        c.Trim().ToLower() != Decode(Request["colunaDescr"]).ToLower())*/)
                    {
                        var tiposParametros = odsBuscar.SelectParameters.Cast<Parameter>().Select(x => x.Type).ToArray();
    
                        BoundField campo = !IsColunaBool(c.Trim(), odsBuscar.TypeName, odsBuscar.SelectMethod, tiposParametros) ?
                            new BoundField() : new CheckBoxField();
    
                        campo.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        campo.DataField = c.Trim();
                        campos.Add(campo);
                    }
                }
    
                if (campos.Count == (exibirId ? 1 : 0))
                {
                    BoundField campo = new BoundField();
                    campo.DataField = Decode(Request["colunaDescr"]);
                    campos.Add(campo);
                }
    
                int indiceColuna = exibirId ? 0 : 1;
                string[] titulosColunas = Request["titulosColunas"] != null ? Request["titulosColunas"].Split('|') : new string[0];
    
                foreach (BoundField b in campos)
                {
                    b.HeaderText = titulosColunas.Length > indiceColuna ? titulosColunas[indiceColuna++] : b.DataField;
                    b.SortExpression = b.DataField;
                    grdBuscar.Columns.Add(b);
                }
    
                #endregion
            }
        }
    
        protected void imgPesq_Click(object sender, EventArgs e)
        {
        }
    }
}
