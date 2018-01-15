using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Reflection;
using GDA;
using System.Collections;
using System.Data;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelPopup : BaseUserControl
    {
        #region Enumerações
    
        public enum TamanhoTelaEnum
        {
            Tamanho600x400,
            Tamanho700x525,
            Tamanho800x600,
            Tamanho1000x600
        }
    
        #endregion
    
        #region Campos privados
    
        private string _dataSourceID;
        private string _dataTextField;
        private string _dataValueField;
        private bool _exibirIdPopup = false;
        private string _titulosColunas = "Cód.|Descrição";
        private string _tituloTela = "Selecione o item";
        private TamanhoTelaEnum _tamanhoTela = TamanhoTelaEnum.Tamanho600x400;
        private bool _exibirAlertNaoEncontrado = true;
        private string _colunasExibirPopup = "";
        private bool _fazerPostBackBotao = false;
        private string _callback;
        private bool _usarValorRealControle = false;
        private string _url;
        private EventHandler _preRender = null;
    
        #endregion
    
        #region Propriedades
    
        public bool UsarValorRealControle
        {
            get { return _usarValorRealControle; }
            set { _usarValorRealControle = value; }
        }
    
        public string DataSourceID
        {
            get { return _dataSourceID; }
            set { _dataSourceID = value; }
        }
    
        public string DataTextField
        {
            get { return _dataTextField; }
            set { _dataTextField = value; }
        }
    
        public string DataValueField
        {
            get { return _dataValueField; }
            set { _dataValueField = value; }
        }
    
        public bool PermitirVazio
        {
            get { return !ctvSelPopup.Enabled; }
            set { ctvSelPopup.Enabled = !value; }
        }
    
        public string ValidationGroup
        {
            get { return ctvSelPopup.ValidationGroup; }
            set { ctvSelPopup.ValidationGroup = value; }
        }
    
        public string ErrorMessage
        {
            get { return ctvSelPopup.ErrorMessage; }
            set { ctvSelPopup.ErrorMessage = value; }
        }
    
        public bool ExibirIdPopup
        {
            get { return _exibirIdPopup; }
            set { _exibirIdPopup = value; }
        }
    
        public string TitulosColunas
        {
            get { return _titulosColunas; }
            set { _titulosColunas = value; }
        }
    
        public string TituloTela
        {
            get { return _tituloTela; }
            set { _tituloTela = value; }
        }
    
        public Unit TextWidth
        {
            get { return txtDescr.Width; }
            set { txtDescr.Width = value; }
        }
    
        public TamanhoTelaEnum TamanhoTela
        {
            get { return _tamanhoTela; }
            set { _tamanhoTela = value; }
        }
    
        public bool ExibirAlertNaoEncontrado
        {
            get { return _exibirAlertNaoEncontrado; }
            set { _exibirAlertNaoEncontrado = value; }
        }
    
        public string ColunasExibirPopup
        {
            get { return _colunasExibirPopup; }
            set { _colunasExibirPopup = value; }
        }
    
        public bool FazerPostBackBotaoPesquisar
        {
            get { return _fazerPostBackBotao; }
            set { _fazerPostBackBotao = value; }
        }
    
        public string Valor
        {
            get { return hdfValor.Value; }
            set
            {
                hdfValor.Value = value;
    
                if (_preRender != null) this.PreRender -= _preRender;
    
                _preRender = new EventHandler(delegate(object sender, EventArgs e)
                {
                    if (GetDataSource() != null)
                    {
                        var itens = GetDataSource().Select().Cast<object>().ToArray();
    
                        if (itens.Length > 0 && itens[0] is GenericModel)
                            foreach (GenericModel g in itens)
                                if ((g.Id != null ? g.Id.ToString() : null) == this.Valor)
                                {
                                    (sender as Controls.ctrlSelPopup).Descricao = g.Descr;
                                    break;
                                }
                    }
                });
    
                this.PreRender += _preRender;
            }
        }
    
        public string Descricao
        {
            get { return txtDescr.Text; }
            set { txtDescr.Text = value; }
        }
    
        public string CallbackSelecao
        {
            get { return _callback != null ? _callback : String.Empty; }
            set { _callback = value; }
        }
    
        public string UrlPopup
        {
            get { return _url; }
            set { _url = value; }
        }
    
        public CustomValidator Validador
        {
            get { return ctvSelPopup; }
            set
            {
                // Mantém algumas propriedades inalteradas
                value.ID = ctvSelPopup.ID;
                value.ControlToValidate = ctvSelPopup.ControlToValidate;
                value.ValidateEmptyText = ctvSelPopup.ValidateEmptyText;
                value.Display = ctvSelPopup.Display;
    
                foreach (PropertyInfo p in typeof(CustomValidator).GetProperties())
                {
                    try
                    {
                        p.SetValue(ctvSelPopup, p.GetValue(value, null), null);
                    }
                    catch { }
                }
            }
        }
    
        public bool Enabled
        {
            get { return txtDescr.Enabled; }
            set
            {
                txtDescr.Enabled = value;
                imgPesq.Enabled = value;
            }
        }
    
        #endregion
    
        #region Métodos privados
    
        private static string Encode(string s)
        {
            if (s == null)
                return "";
    
            return Convert.ToBase64String(Encoding.Default.GetBytes(s));
        }
    
        private static string Decode(string s)
        {
            if (s == null)
                return "";
    
            return Encoding.Default.GetString(Convert.FromBase64String(s));
        }
    
        private static Control GetControl(Control c, string id)
        {
            Control r = null;
    
            while (r == null && c.Parent != null)
            {
                c = c.Parent;
                r = c.FindControl(id);
            }
    
            return r;
        }
    
        private static string GetParameterValue(Colosoft.WebControls.VirtualObjectDataSource ds, Parameter p, bool liveValue)
        {
            Control c = p is ControlParameter ? GetControl(ds, ((ControlParameter)p).ControlID) : null;
    
            if (c == null || !liveValue)
            {
                MethodInfo eval = typeof(Parameter).GetMethod("Evaluate", BindingFlags.NonPublic | BindingFlags.Instance);
    
                object retorno = eval.Invoke(p, new object[] { HttpContext.Current, c });
                if (retorno == null || retorno == DBNull.Value)
                    retorno = p.DefaultValue;
    
                return retorno != null ? retorno.ToString() : null;
            }
            else
                return "document.getElementById(\"" + c.ClientID + "\").value";
        }
    
        private Colosoft.WebControls.VirtualObjectDataSource GetDataSource()
        {
            return GetControl(this, _dataSourceID) as Colosoft.WebControls.VirtualObjectDataSource;
        }
    
        private static Type ToType(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
    
                case TypeCode.Byte:
                    return typeof(byte);
    
                case TypeCode.Char:
                    return typeof(char);
    
                case TypeCode.DateTime:
                    return typeof(DateTime);
    
                case TypeCode.Decimal:
                    return typeof(decimal);
    
                case TypeCode.Double:
                    return typeof(double);
    
                case TypeCode.Int16:
                    return typeof(short);
    
                case TypeCode.Int32:
                    return typeof(int);
    
                case TypeCode.Int64:
                    return typeof(long);
    
                case TypeCode.Object:
                    return typeof(object);
    
                case TypeCode.SByte:
                    return typeof(sbyte);
    
                case TypeCode.Single:
                    return typeof(float);
    
                case TypeCode.String:
                    return typeof(string);
    
                case TypeCode.UInt16:
                    return typeof(ushort);
    
                case TypeCode.UInt32:
                    return typeof(uint);
    
                case TypeCode.UInt64:
                    return typeof(ulong);
            }
    
            return null;
        }
    
        #endregion
    
        #region Métodos Ajax
    
        private static string GetDbName(Type tipoModel, string campo)
        {
            PersistencePropertyAttribute[] atributos = tipoModel.GetProperty(campo).GetCustomAttributes(
                typeof(PersistencePropertyAttribute), true) as PersistencePropertyAttribute[];
    
            return atributos != null && atributos.Length > 0 ? atributos[0].Name : null;
        }
    
        [Ajax.AjaxMethod]
        public string BuscarByDescricao(string campoId, string campoDescr, string tipoDAO, 
            string nomeMetodo, string parametros, string descricao, string recuperarValores)
        {
            try
            {
                campoId = Decode(campoId);
                campoDescr = Decode(campoDescr);
                tipoDAO = Decode(tipoDAO);
                parametros = recuperarValores != "true" ? Decode(parametros) : parametros;
    
                var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
    
                Type DAO = Type.GetType(tipoDAO + (tipoDAO.Contains("Glass.Data") ? ", Glass.Data" : 
                    tipoDAO.Contains("WebGlass.Business") ? ", WebGlass.Business" : ""));
    
                Type tipoModel = tipoDAO.Contains("DAL") ? DAO.GetMethod("GetElementByPrimaryKey", flags, null, new Type[] { typeof(uint) }, null).ReturnType : null;
    
                bool isCampoDescrInTable = false;
    
                if (tipoDAO.Contains("DAL") && tipoModel != null && String.IsNullOrEmpty(parametros))
                {
                    PropertyInfo propDescr = tipoModel.GetProperty(campoDescr, flags);
    
                    PersistencePropertyAttribute[] descrAtr = propDescr.GetCustomAttributes(typeof(PersistencePropertyAttribute), true) as PersistencePropertyAttribute[];
    
                    if (descrAtr.Length > 0 && descrAtr[0].Direction != DirectionParameter.InputOptional && descrAtr[0].Direction != DirectionParameter.InputOptionalOutput &&
                        descrAtr[0].Direction != DirectionParameter.InputOptionalOutputOnlyInsert)
                        isCampoDescrInTable = true;
                }
    
                if (isCampoDescrInTable)
                {
                    object instance = DAO.GetProperty("Instance", flags).GetValue(null, null);
    
                    string nomeTabela = DAO.GetMethod("GetTableName", flags).Invoke(instance, null) as string;
    
                    if (String.IsNullOrEmpty(nomeTabela))
                        return "Erro|Tabela não encontrada.";
    
                    string sql = "select concat(Cast(" + GetDbName(tipoModel, campoId) + " as char), '|', Cast(" + 
                        GetDbName(tipoModel, campoDescr) + " as char)) from " + nomeTabela + " where " + GetDbName(tipoModel, campoDescr) + "=?descr";
    
                    string retorno = DAO.GetMethod("ExecuteScalar", flags, null, new Type[] { typeof(string), typeof(GDA.GDAParameter[]) }, null).MakeGenericMethod(typeof(string)).
                        Invoke(instance, new object[] { sql, new GDAParameter[] { new GDAParameter("?descr", descricao) } }) as string;
    
                    if (retorno == null)
                        throw new Exception("Item não encontrado.");
    
                    string[] dados = retorno.Split('|');
                    return "Ok|" + dados[0] + "|" + dados[1];
                }
                else
            {
                nomeMetodo = Decode(nomeMetodo).Split(',')[1];

                var metodos = DAO.GetMethods(flags);
                string[] dadosParametros = parametros.Split('|');

                MethodInfo metodo = metodos.Where(x => x.Name == nomeMetodo).
                    Where(x => x.GetParameters().Length == (parametros != String.Empty ? dadosParametros.Length : 0)).
                    FirstOrDefault();

                tipoModel = metodo.ReturnType;

                ParameterInfo[] pi = metodo.GetParameters();
                object[] param = new object[pi.Length];

                for (int j = 0; j < param.Length; j++)
                    for (int k = 0; k < dadosParametros.Length; k++)
                    {
                            string[] dados = dadosParametros[k].Split(':');
                            if (pi[j].Name == dados[0])
                            {
                                param[j] = Conversoes.ConverteValor(pi[j].ParameterType, dados[1]);
                                break;
                            }
                        }
    
                    object instance = metodo.IsStatic ? null :
                        DAO.GetProperty("Instance", flags).GetValue(null, null);
    
                    if (!metodo.IsStatic && instance == null)
                        try { instance = Activator.CreateInstance(DAO); }
                        catch { }
    
                    object retorno = metodo.Invoke(instance, param);
    
                    PropertyInfo id = null, descr = null;
                    object i, d;
    
                    if (typeof(IEnumerable).IsAssignableFrom(tipoModel))
                    {
                        foreach (object o in (IEnumerable)retorno)
                        {
                            if (id == null)
                            {
                                id = o.GetType().GetProperty(campoId);
                                descr = o.GetType().GetProperty(campoDescr);
                            }
    
                            i = id.GetValue(o, null);
                            d = descr.GetValue(o, null);
    
                            string comp = d is string || d is ValueType || d != null ? d.ToString() : null;
                            if (String.Equals(Formatacoes.TrataTextoComparacaoSelPopUp(comp).Replace("  ", "").Replace(" ", "").Replace("-", "").ToLower(),
                                Formatacoes.TrataTextoComparacaoSelPopUp(descricao).Replace("  ", "").Replace(" ", "").Replace("-", "").ToLower(),
                                StringComparison.InvariantCultureIgnoreCase))
                                return "Ok|" + i + "|" + d;
                        }
    
                        return "Erro|Item não encontrado.";
                    }
                    else
                    {
                        id = retorno.GetType().GetProperty(campoId);
                        descr = retorno.GetType().GetProperty(campoDescr);
    
                        i = id.GetValue(retorno, null);
                        d = descr.GetValue(retorno, null);
    
                        string comp = d is string || d is ValueType || d != null ? d.ToString() : null;
                        if (String.Equals(Formatacoes.TrataTextoComparacaoSelPopUp(comp).Replace("  ", "").Replace(" ", "").Replace("-", "").ToLower(),
                            Formatacoes.TrataTextoComparacaoSelPopUp(descricao).Replace("  ", "").Replace(" ", "").Replace("-", "").ToLower(),
                            StringComparison.InvariantCultureIgnoreCase))
                            return "Ok|" + i + "|" + d;
                        else
                            return "Erro|Item não encontrado.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelPopup));
    
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlSelPopup"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlSelPopup", this.ResolveClientUrl("~/Scripts/ctrlSelPopup.js"));
    
            if (String.IsNullOrEmpty(ctvSelPopup.ClientValidationFunction))
                ctvSelPopup.ClientValidationFunction = this.ClientID + ".Validar";
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            var ds = String.IsNullOrEmpty(_dataSourceID) ? null : GetDataSource();
    
            string dadosControle = @"
                Callback: '{0}',
                Url: '{1}',
                PaginaPadrao: {2},
                Altura: {3},
                Largura: {4},
                TituloTela: '{5}',
                NomeCampoId: '{6}',
                NomeCampoDescr: '{7}',
                ExibirCampoId: {8},
                ColunasExibir: '{9}',
                NomeMetodo: '{10}',
                Parametros: '{11}',
                TitulosColunas: '{12}',
                UsarParametrosReais: {13},
                TipoObjetoDados: '{14}',
                ExibirErroItemNaoEncontrado: {15}
            ";
    
            bool paginaPadrao = String.IsNullOrEmpty(_url);
            string url = this.ResolveClientUrl(!paginaPadrao ? _url : "~/Utils/SelPopup.aspx");
            string nomeMetodo = null, parametros = null, nomeDAO = null;
            int altura = 0, largura = 0;
    
            if (ds != null)
            {
                nomeDAO = Encode(ds.TypeName);
    
                nomeMetodo = ds.TypeName + "," + ds.SelectMethod +
                    (!String.IsNullOrEmpty(ds.SelectCountMethod) ? "," + ds.SelectCountMethod : "");
    
                parametros = "";
                foreach (Parameter p in ds.SelectParameters)
                    parametros += p.Name + ":" + GetParameterValue(ds, p, _usarValorRealControle) + ":" + (int)p.Type + "|";
    
                parametros = parametros.TrimEnd('|');
                if (!_usarValorRealControle)
                    parametros = Encode(parametros);
    
                switch (_tamanhoTela)
                {
                    case TamanhoTelaEnum.Tamanho600x400:
                        altura = 400;
                        largura = 600;
                        break;
    
                    case TamanhoTelaEnum.Tamanho700x525:
                        altura = 525;
                        largura = 700;
                        break;
    
                    case TamanhoTelaEnum.Tamanho800x600:
                        altura = 600;
                        largura = 800;
                        break;
    
                    case TamanhoTelaEnum.Tamanho1000x600:
                        altura = 600;
                        largura = 1000;
                        break;
                }
    
                imgPesq.OnClientClick = (_fazerPostBackBotao ? "if (document.getElementById('" + txtDescr.ClientID + "').value == '') { " : "") +
                    this.ClientID + ".AbrirPopup(); return false;" + (_fazerPostBackBotao ? " }" : "");
    
                txtDescr.Attributes.Add("OnBlur", this.ClientID + ".AlterarCampo()");
    
                // O controle de natureza de operação passa para este controle o atributo onchange que deve ser adicionado ao text box txtDescr
                // para que ao alterar a natureza de operação do produto a mesma seja atualizada na Session também.
                if (this.Attributes["onchange"] != null)
                    txtDescr.Attributes.Add("onchange", this.Attributes["onchange"]);

                if (_fazerPostBackBotao)
                    txtDescr.Attributes.Add("OnKeyPress", "if (isEnter(event)) { this.blur(); FindControl('imgPesq', 'input').click(); }");
            }
            else
                imgPesq.Visible = false;
    
            dadosControle = String.Format(dadosControle,
                CallbackSelecao,
                url ?? String.Empty,
                paginaPadrao.ToString().ToLower(),
                altura,
                largura,
                _tituloTela ?? String.Empty,
                Encode(_dataValueField),
                Encode(_dataTextField),
                _exibirIdPopup.ToString().ToLower(),
                Encode(_colunasExibirPopup.TrimEnd('|')),
                Encode(nomeMetodo),
                parametros ?? String.Empty,
                _titulosColunas.TrimEnd('|'),
                _usarValorRealControle.ToString().ToLower(),
                nomeDAO ?? String.Empty,
                _exibirAlertNaoEncontrado.ToString().ToLower()
            );
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID,
                String.Format("var {0} = new SelPopupType('{0}', {1});\n", this.ClientID, "{" + dadosControle + "}"), true);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
