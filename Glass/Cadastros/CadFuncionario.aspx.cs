using System;
using System.Data;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Sync.Controls;
using System.Linq;
using Glass.Configuracoes;
using System.Collections.Generic;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFuncionario : System.Web.UI.Page
    {
        #region Variáveis Locais

        private IList<Glass.PCP.Negocios.Entidades.SetorDescritor> _setores;

        /// <summary>
        /// Instancia do funcionário que está sendo editada.
        /// </summary>
        private Glass.Global.Negocios.Entidades.Funcionario _funcionario;

        #endregion

        #region Properties

        /// <summary>
        /// Relação dos setores do sistema.
        /// </summary>
        protected IList<Glass.PCP.Negocios.Entidades.SetorDescritor> Setores
        {
            get
            {
                if (_setores == null)
                {
                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.PCP.Negocios.ISetorFluxo>();

                    _setores = fluxo.ObtemSetores().Where(f => f.Id != 1/* Impr. Etiqueta */).ToList();
                }
                return _setores;
            }
        }

        #endregion

        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);

        //    Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

        //    dtvFuncionario.Register("../Listas/LstFuncionario.aspx");
        //    odsFuncionario.Register();

        //}

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (Request["idFunc"] != null)
        //        dtvFuncionario.ChangeMode(DetailsViewMode.Edit);

        //    if (!IsPostBack)
        //    {
        //        // Impede o acesso não autorizado à esta tela
        //        if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario))
        //            Response.Redirect("../WebGlass/Main.aspx");
        //        // Somente o usuário Admin Sync pode editar seu próprio cadastro.
        //        else if (FuncionarioDAO.Instance.IsAdminSync(Request["idFunc"].StrParaUint()) &&
        //            !UserInfo.GetUserInfo.IsAdminSync)
        //            Response.Redirect("../WebGlass/Main.aspx");
        //    }
        //}

        //protected void btnCancelar_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("../Listas/LstFuncionario.aspx");
        //}

        //protected void odsFuncionario_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        //{
        //    _funcionario = e.InputParameters[0] as Glass.Global.Negocios.Entidades.Funcionario;
        //    ProcessarSetores(_funcionario);
        //}

        //protected void odsFuncionario_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        //{
        //    if (e.Exception != null)
        //        throw e.Exception;

        //    else
        //        FuncionarioSalvo(e.ReturnValue as Colosoft.Business.SaveResult);
        //}

        //protected void odsFuncionario_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        //{
        //    _funcionario = e.InputParameters[0] as Glass.Global.Negocios.Entidades.Funcionario;
        //    ProcessarSetores(_funcionario);
        //}

        //protected void odsFuncionario_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        //{
        //    if (e.Exception != null)
        //        throw e.Exception;

        //    else
        //        FuncionarioSalvo(e.ReturnValue as Colosoft.Business.SaveResult);
        //}

        ///// <summary>
        ///// Processa os setores selecionados para o funcionário.
        ///// </summary>
        ///// <param name="funcionario"></param>
        //protected void ProcessarSetores(Glass.Global.Negocios.Entidades.Funcionario funcionario)
        //{
        //    string cblSetorName = dtvFuncionario.CurrentMode == DetailsViewMode.Edit ? "cblSetor" : "cblSetorIns";
        //    CheckBoxList cblSetor = ((CheckBoxList)dtvFuncionario.FindControl(cblSetorName));

        //    string setores = String.Empty;
        //    var atualizados = new List<Glass.Global.Negocios.Entidades.FuncionarioSetor>();
        //    var setoresAdicionados = string.Empty;
        //    var setoresRemovidos = string.Empty;

        //    // Insere os novos setores para o funcionário
        //    foreach (ListItem li in cblSetor.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            var idSetor = int.Parse(li.Value);
        //            var funcionarioSetor = funcionario.FuncionarioSetores.FirstOrDefault(f => f.IdSetor == idSetor);
        //            if (funcionarioSetor == null)
        //            {
        //                funcionarioSetor = new Glass.Global.Negocios.Entidades.FuncionarioSetor
        //                {
        //                    IdSetor = idSetor
        //                };
        //                funcionario.FuncionarioSetores.Add(funcionarioSetor);
        //                setoresAdicionados += SetorDAO.Instance.ObtemDescricaoSetor(funcionarioSetor.IdSetor) + "\n";
        //            }

        //            atualizados.Add(funcionarioSetor);
        //        }
        //    }

        //    // Essa ordenação segue a ordem dos setores na tela, portanto, caso uma seja alterada a outra também deverá ser.
        //    // Recupera os setores que devem ser apagados
        //    foreach (var i in funcionario.FuncionarioSetores.Where(f => !atualizados.Exists(x => f.Equals(x))).OrderBy(f => SetorDAO.Instance.ObtemNumSeq(null, f.IdSetor)).ToArray())
        //    {
        //        setoresRemovidos += SetorDAO.Instance.ObtemDescricaoSetor(i.IdSetor) + "\n";
        //        funcionario.FuncionarioSetores.Remove(i);
        //    }

        //    if (funcionario.IdFunc > 0)
        //        LogAlteracaoDAO.Instance.LogFuncionarioSetor(funcionario.IdFunc, setoresRemovidos, setoresAdicionados);
        //}

        ///// <summary>
        ///// Processa o funcionário salvo.
        ///// </summary>
        ///// <param name="saveResult"></param>
        //protected void FuncionarioSalvo(Colosoft.Business.SaveResult saveResult)
        //{
        //    if (!saveResult)
        //        throw new Colosoft.DetailsException(saveResult.Message);

        //    // Recupera o control de upload
        //    var imagem = (FileUpload)dtvFuncionario.FindControl("filImagem");

        //    if (imagem.HasFile)
        //    {
        //        var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
        //            .Current.GetInstance<Glass.Global.Negocios.Entidades.IFuncionarioRepositorioImagens>();

        //        // Salva a imagem do funcionário
        //        repositorio.SalvarImagem(_funcionario.IdFunc, imagem.PostedFile.InputStream);
        //    }

        //    // Limpa menus deste funcionário
        //    if (!string.IsNullOrEmpty(Request["idFunc"]))
        //        Microsoft.Practices.ServiceLocation.ServiceLocator
        //            .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().RemoveMenuFuncMemoria(int.Parse(Request["idFunc"]));
        //}

        //protected void dtvFuncionario_DataBound(object sender, EventArgs e)
        //{
        //    CheckBoxListDropDown cbdTipoPedido = (CheckBoxListDropDown)dtvFuncionario.FindControl("cbdTipoPedido");
        //    Label lblTipoPedido = (Label)dtvFuncionario.FindControl("lblTipoPedido");

        //    // Verifica se o pedido tem itens que não são permitidos pelo seu tipo
        //    if (!PedidoConfig.DadosPedido.BloquearItensTipoPedido)
        //    {
        //        cbdTipoPedido.Visible = false;
        //        lblTipoPedido.Visible = false;
        //    }

        //    if (!FinanceiroConfig.UtilizarTefCappta)
        //    {
        //        ((TextBox)dtvFuncionario.FindControl("txtNumPdv")).Visible = false;
        //        ((Label)dtvFuncionario.FindControl("lblNumPdv")).Visible = false;
        //    }

        //    if (dtvFuncionario.CurrentMode != DetailsViewMode.Edit)
        //        return;

        //    var funcionario = dtvFuncionario.DataItem as Glass.Global.Negocios.Entidades.Funcionario;

        //    CheckBoxList cblSetor = ((CheckBoxList)dtvFuncionario.FindControl("cblSetor"));

        //    // Seleciona os setores do funcionário
        //    foreach (var fs in funcionario.FuncionarioSetores)
        //        foreach (ListItem li in cblSetor.Items)
        //            if (li.Value == fs.IdSetor.ToString())
        //                li.Selected = true;

        //}

        //protected void cblSetorIns_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var chk = (CheckBoxList)sender;
        //    bool expCarregamento = false, expBalcao = false;

        //    foreach (ListItem item in chk.Items)
        //    {
        //        if (expCarregamento && expBalcao)
        //        {
        //            break;
        //        }

        //        // Recupera o setor associado
        //        var setor = Setores.FirstOrDefault(f => f.Id == int.Parse(item.Value));
        //        if (item.Selected && setor.TipoSetor == TipoSetor.ExpCarregamento)
        //        {
        //            expCarregamento = true;
        //        }
        //        else if (item.Selected && setor.TipoSetor == TipoSetor.Entregue && Glass.Configuracoes.PCPConfig.UsarNovoControleExpBalcao)
        //        {
        //            expBalcao = true;
        //        }
        //    }

        //    if (expCarregamento)
        //    {
        //        foreach (ListItem item in chk.Items)
        //        {
        //            // Recupera o setor associado
        //            var setor = Setores.FirstOrDefault(f => f.Id == int.Parse(item.Value));
        //            if (setor.TipoSetor != TipoSetor.ExpCarregamento && setor.TipoSetor != TipoSetor.Entregue && Glass.Configuracoes.PCPConfig.UsarNovoControleExpBalcao)
        //                item.Selected = false;
        //        }
        //    }

        //    if (expBalcao)
        //    {
        //        foreach (ListItem item in chk.Items)
        //        {
        //            // Recupera o setor associado
        //            var setor = Setores.FirstOrDefault(f => f.Id == int.Parse(item.Value));
        //            if (setor.TipoSetor != TipoSetor.ExpCarregamento && setor.TipoSetor != TipoSetor.Entregue && Glass.Configuracoes.PCPConfig.UsarNovoControleExpBalcao)
        //                item.Selected = false;
        //        }
        //    }
        //}

        //protected void drpLojaEdit_Load(object sender, EventArgs e)
        //{
        //    var idLoja = FuncionarioDAO.Instance.ObtemIdLoja(Request["idFunc"].StrParaUint());
        //    var loja = LojaDAO.Instance.GetElementByPrimaryKey(idLoja);

        //    if (loja.Situacao == Situacao.Inativo)
        //        ((DropDownList)sender).Items.Add(new ListItem(loja.NomeFantasia, loja.IdLoja.ToString()));
        //}
    }
}
