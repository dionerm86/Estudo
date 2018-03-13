using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.DAL;
using System.Text;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlBenefSetor : BaseUserControl
    {
        #region Campos privados
    
        private string _callbackSelecaoItem;
        private string _cssClassLinha;
        private string _cssClassLinhaAlternada;
        private string _funcaoExibir;
        private string _queryStringRecuperarIds;

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Retorna o controle 'Redondo'.
        /// </summary>
        /// <returns></returns>
        private CheckBox GetControlRedondo()
        {
            // Retorna o controle 'Redondo'
            return (CheckBox)tblBenef.FindControl("Redondo_chkSelecao");
        }
    
        /// <summary>
        /// Retorna o prefixo dos nomes dos controles.
        /// </summary>
        /// <param name="benef"></param>
        /// <returns></returns>
        private string PrefixoControles(BenefConfig benef)
        {
            // Retorna uma string com o nome do beneficiamento formatado
            return benef.Nome.Trim().Replace(" ", "_").Replace("²", "2") + "_";
        }
    
        /// <summary>
        /// Cria os beneficiamentos nas células.
        /// </summary>
        /// <param name="celula">A célula dos controles.</param>
        /// <param name="benef">O beneficiamento.</param>
        private void CreateBenef(TableCell celula, BenefConfig benef)
        {
            // Indica na célula dos controles o ID do beneficiamento
            celula.Attributes.Add("IdBeneficiamento", benef.IdBenefConfig.ToString());
    
            // Formata a célula
            celula.Style.Add("White-space", "nowrap");
            celula.Style.Add("Padding-right", "4px");
    
            // Adiciona os controles à célula
            foreach (Control c in GetControls(benef))
                celula.Controls.Add(c);
        }
    
        /// <summary>
        /// Retorna a lista de controles do beneficiamento.
        /// </summary>
        /// <param name="benef">O beneficiamento que será criado.</param>
        /// <returns>Um vetor com os controles do beneficiamento.</returns>
        private Control[] GetControls(BenefConfig benef)
        {
            // Variável de retorno
            List<Control> retorno = new List<Control>();
            string prefixo = PrefixoControles(benef);
    
            // Cria o controle de seleção pai
            CheckBox chkPai = new CheckBox();
            chkPai.ID = prefixo + "chkPai";
            chkPai.Text = benef.Nome;
            chkPai.Attributes.Add("prefixoBenef", prefixo);

            /* Chamado 55594. */
            if (!IsPostBack && !string.IsNullOrWhiteSpace(_queryStringRecuperarIds))
                if (_queryStringRecuperarIds.Split(',').Any(f => f.StrParaUint() == benef.IdBenefConfig || BenefConfigDAO.Instance.GetParentId(f.StrParaUint()) == benef.IdBenefConfig))
                    chkPai.Checked = true;

            retorno.Add(chkPai);
    
            var filhos = GetSubItens(benef);
    
            // Cria os controles de seleção dos filhos
            if (filhos.Count > 0)
            {
                chkPai.Attributes.Add("onclick", "exibirFilhos('" + this.ClientID + "', this); " + this.ClientID + ".Exibir();");
    
                CheckBoxList chkFilhos = new CheckBoxList();
                chkFilhos.ID = prefixo + "tblFilhos";
                chkFilhos.Style.Value = "margin-left: 10px; display: none";
                chkFilhos.RepeatLayout = RepeatLayout.Table;
                chkFilhos.RepeatColumns = 4;
    
                chkFilhos.DataSource = filhos;
                chkFilhos.DataValueField = "IdBenefConfig";
                chkFilhos.DataTextField = "Nome";
                chkFilhos.DataBind();

                foreach (ListItem i in chkFilhos.Items)
                {
                    i.Attributes.Add("idBeneficiamento", i.Value);

                    /* Chamado 55594. */
                    if (!IsPostBack && !string.IsNullOrWhiteSpace(_queryStringRecuperarIds))
                        if (_queryStringRecuperarIds.Split(',').Any(f => f == i.Value.ToString()))
                            i.Selected = true;
                }
    
                retorno.Add(chkFilhos);
            }
    
            // Retorna o vetor
            return retorno.ToArray();
        }
    
        /// <summary>
        /// Retorna os sub-itens de um beneficiamento.
        /// </summary>
        /// <param name="benef">O beneficiamento pai.</param>
        /// <returns>Uma lista com os beneficiamentos vinculados ao pai.</returns>
        private IList<BenefConfig> GetSubItens(BenefConfig benef)
        {
            // Recupera os itens filhos de um beneficiamento
            return BenefConfigDAO.Instance.GetByBenefConfig((uint)benef.IdBenefConfig);
        }
    
        /// <summary>
        /// Retorna o texto da variável de configuração dos beneficiamentos.
        /// </summary>
        /// <param name="calculaveis">Os itens retornados devem ser os calculáveis?</param>
        /// <returns>Uma string com a variável para ser usada no JavaScript.</returns>
        private string GetConfig(bool calculaveis)
        {
            // Recupera a lista de beneficiamentos usados para gerar o retorno
            var benef = calculaveis ? BenefConfigDAO.Instance.GetForConfig() : BenefConfigDAO.Instance.GetForControl(TipoBenef.Todos);
    
            // String com o formato usado para o retorno
            string formato = "" +
                "ID: {0}, " +
                "ParentID: {1}, " +
                "DescricaoParent: '{2}'";
            
            // Variável de retorno
            StringBuilder retorno = new StringBuilder();
    
            // Percorre cada beneficiamento da lista
            foreach (BenefConfig b in benef)
            {
                // Variável com os dados usados para formatar a string
                object[] dadosFormato = new object[3];
    
                if (calculaveis)
                {
                    foreach (BenefConfigPreco bp in BenefConfigPrecoDAO.Instance.GetByIdBenefConfig((uint)b.IdBenefConfig))
                    {
                        dadosFormato[0] = b.IdBenefConfig;
                        dadosFormato[1] = b.IdParent != null ? b.IdParent.Value.ToString() : "null";
                        dadosFormato[2] = b.DescricaoParent != null ? b.DescricaoParent + " " : "";
                        
                        retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                    }
                }
                else
                {
                    dadosFormato[0] = b.IdBenefConfig;
                    dadosFormato[1] = b.IdParent != null ? b.IdParent.Value.ToString() : "null";
                    dadosFormato[2] = b.DescricaoParent != null ? b.DescricaoParent + " " : "";
    
                    retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                }
            }
    
            // Retorna a string com os dados dos beneficiamentos formatados
            return "new Array(" + (retorno.Length > 0 ? retorno.ToString().Substring(2) : "") + ")";
        }
    
        #endregion
    
        #region Geração e recuperação dos beneficiamentos
    
        #region Geração
    
        /// <summary>
        /// Define os beneficiamentos para uma célula da tabela do controle.
        /// </summary>
        /// <param name="beneficiamentos">Os beneficiamentos que serão usados pelo controle.</param>
        /// <param name="celula">A célula que receberá os beneficiamentos.</param>
        private void SetBeneficiamentosToCell(List<uint> beneficiamentos, TableCell celula)
        {
            CheckBox chkPai = celula.Controls[0] as CheckBox;
            CheckBoxList chkFilhos = celula.Controls.Count > 1 ? celula.Controls[1] as CheckBoxList : null;
    
            if (chkFilhos == null)
            {
                if (beneficiamentos.Contains(Glass.Conversoes.StrParaUint(celula.Attributes["idBeneficiamento"])))
                    chkPai.Checked = true;
            }
            else
            {
                for (int i = 0; i < chkFilhos.Items.Count; i++)
                    if (beneficiamentos.Contains(Glass.Conversoes.StrParaUint(chkFilhos.Items[i].Value)))
                    {
                        chkFilhos.Style.Remove("display");
                        chkPai.Checked = true;
                        chkFilhos.Items[i].Selected = true;
                        Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_benef_" + celula.Attributes["idBeneficiamento"] + "_" +
                            chkFilhos.Items[i].Value, "document.getElementById('" + chkFilhos.ClientID + "_" + i + "').checked = true;\n", true);
                    }
            }
        }
    
        #endregion
    
        #region Recuperação
    
        /// <summary>
        /// Retorna os beneficiamentos selecionados em uma célula.
        /// </summary>
        /// <param name="celula">A célula com os controles do beneficiamento.</param>
        /// <returns>Um vetor com os beneficiamentos feitos</returns>
        private uint[] GetBeneficiamentosFromCell(TableCell celula)
        {
            List<uint> retorno = new List<uint>();
            CheckBox chkPai = celula.Controls[0] as CheckBox;
            CheckBoxList chkFilhos = celula.Controls.Count > 1 ? celula.Controls[1] as CheckBoxList : null;
    
            if (chkFilhos == null)
            {
                if (chkPai.Checked)
                    retorno.Add(Glass.Conversoes.StrParaUint(celula.Attributes["idBeneficiamento"]));
            }
            else
            {
                foreach (ListItem i in chkFilhos.Items)
                    if (i.Selected)
                        retorno.Add(Glass.Conversoes.StrParaUint(i.Value));
            }
    
            return retorno.ToArray();
        }
    
        #endregion
    
        #endregion
    
        #region Propriedades
    
        /// <summary>
        /// Callback chamado quando o item for selecionado.
        /// </summary>
        public string CallbackSelecaoItem
        {
            get { return _callbackSelecaoItem; }
            set { _callbackSelecaoItem = value; }
        }
    
        /// <summary>
        /// Classe CSS da linha.
        /// </summary>
        public string CssClassLinha
        {
            get { return _cssClassLinha; }
            set { _cssClassLinha = value; }
        }
    
        /// <summary>
        /// Classe CSS da linha alternada.
        /// </summary>
        public string CssClassLinhaAlternada
        {
            get { return _cssClassLinhaAlternada; }
            set { _cssClassLinhaAlternada = value; }
        }

        public string FuncaoExibir
        {
            get { return _funcaoExibir; }
            set { _funcaoExibir = value; }
        }

        public string QueryStringRecuperarIds
        {
            get { return _queryStringRecuperarIds; }
            set { _queryStringRecuperarIds = value; }
        }

        /// <summary>
        /// Os beneficiamentos selecionados para um setor.
        /// </summary>
        public List<uint> Beneficiamentos
        {
            get
            {
                // Cria a variável de retorno
                List<uint> retorno = new List<uint>();
    
                // Percorre as linhas da tabela
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui no retorno os beneficiamentos da primeira célula
                    retorno.AddRange(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[0]));
                    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[1].Controls.Count == 0)
                        break;
    
                    // Inclui no retorno os beneficiamentos da segunda célula
                    retorno.AddRange(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[1]));
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[2].Controls.Count == 0)
                        break;
    
                    // Inclui no retorno os beneficiamentos da segunda célula
                    retorno.AddRange(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[2]));
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;
    
                    // Inclui no retorno os beneficiamentos da segunda célula
                    retorno.AddRange(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[3]));
                }
    
                return retorno;
            }
            set
            {
                // Percorre as linhas da coluna
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui na primeira célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[0]);
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[1].Controls.Count == 0)
                        break;
    
                    // Inclui na segunda célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[1]);
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[2].Controls.Count == 0)
                        break;
    
                    // Inclui na segunda célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[2]);
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;
    
                    // Inclui na segunda célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[3]);
                }
            }
        }
    
        /// <summary>
        /// O controle de 'Redondo' deve ser marcado?
        /// </summary>
        public bool Redondo
        {
            get
            {
                CheckBox chkRedondo = GetControlRedondo();
                return chkRedondo != null ? chkRedondo.Checked : false;
            }
            set
            {
                CheckBox chkRedondo = GetControlRedondo();
                if (chkRedondo != null)
                    chkRedondo.Checked = value;
            }
        }
    
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set
            {
                // Atualiza o ViewState do controle
                base.EnableViewState = value;
    
                // Atualiza os ViewStates dos controles filhos
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                    for (int j = 0; j < tblBenef.Rows[i].Cells.Count; j++)
                        foreach (Control c in tblBenef.Rows[i].Cells[j].Controls)
                            c.EnableViewState = value;
            }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Registra os scripts
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("ctrlBenefSetor"))
            {
                Page.ClientScript.RegisterClientScriptInclude("ctrlBenefSetor", ResolveClientUrl("~/Scripts/ctrlBenefSetor.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlBenefSetor_Config", "var benefSetor = " + GetConfig(true) + ";\n", true);
            }
    
            // Indica o evento de PreRender da página
            this.PreRender += new EventHandler(ctrlBenef_PreRender);
        }
    
        protected void tblBenef_Load(object sender, EventArgs e)
        {
            // Recupera a lista de beneficiamentos e define o número de linhas
            var lstBenef = BenefConfigDAO.Instance.GetForControl(TipoBenef.Todos);
            int numLinhas = (int)Math.Ceiling((double)lstBenef.Count / 4);
            if ((float)lstBenef.Count % 2 > 0)
                numLinhas++;
    
            // Percorre o número de linhas
            for (int i = 0; i < numLinhas; i++)
            {
                // Cria a linha e as células e adiciona-os à tabela
                TableRow linha = new TableRow();
                TableCell controles1 = new TableCell();
                TableCell controles2 = new TableCell();
                TableCell controles3 = new TableCell();
                TableCell controles4 = new TableCell();
                linha.Cells.AddRange(new TableCell[] { controles1, controles2, controles3, controles4 });
                tblBenef.Rows.Add(linha);
    
                // Define os estilos das linhas (normal ou alternada)
                if (i % 2 == 0)
                {
                    if (!String.IsNullOrEmpty(_cssClassLinha))
                        linha.CssClass = _cssClassLinha;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_cssClassLinhaAlternada))
                        linha.CssClass = _cssClassLinhaAlternada;
                }

                /* Chamado 67377. */
                if (lstBenef.Count <= i)
                    continue;

                // Cria os controles do beneficiamento para a primeira célula
                CreateBenef(controles1, lstBenef[i]);
                
                // Define o estilo dos controles da primeira coluna
                controles1.Style.Add("Padding-right", "8px");
    
                // Verifica se há um beneficiamento para as próximas colunas
                if ((i + numLinhas) >= lstBenef.Count)
                    continue;
    
                // Cria os controles do beneficiamento para a segunda célula
                CreateBenef(controles2, lstBenef[i + numLinhas]);
    
                // Define o estilo dos controles da segunda coluna
                controles2.Style.Add("Padding-right", "8px");
    
                // Verifica se há um beneficiamento para as próximas colunas
                if ((i + numLinhas * 2) >= lstBenef.Count)
                    continue;
    
                // Cria os controles do beneficiamento para a terceira célula
                CreateBenef(controles3, lstBenef[i + numLinhas * 2]);
    
                // Define o estilo dos controles da terceira coluna
                controles3.Style.Add("Padding-right", "8px");
    
                // Verifica se há um beneficiamento para as próximas colunas
                if ((i + numLinhas * 3) >= lstBenef.Count)
                    continue;
    
                // Cria os controles do beneficiamento para a quarta célula
                CreateBenef(controles4, lstBenef[i + numLinhas * 3]);
            }
        }
    
        protected void ctrlBenef_PreRender(object sender, EventArgs e)
        {
            // Registra os controles que serão usados pelo JavaScript
            string formato = @"
                Beneficiamentos: {0}, 
                Exibir: {1}, 
                Selecionados: {2}
            ";
    
            // Cria a linha do script com a variável do controle
            object[] dadosFormato = new object[] {
                GetConfig(false),
                "function() { " + (!String.IsNullOrEmpty(_funcaoExibir) ? _funcaoExibir + (_funcaoExibir.IndexOf("(") == -1 ? "()" : "") : "") + " }",
                "function() { return getBeneficiamentos('" + this.ClientID + "'); }"
            };
    
            // Define o script da variável na tela
            string script = "var " + this.ClientID + " = { " + String.Format(formato, dadosFormato) + " };\n";
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID, script, true);
        }
    }
}
