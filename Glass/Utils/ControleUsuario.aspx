<%@ Page Title="Controle de Usuários" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ControleUsuario.aspx.cs" Inherits="Glass.UI.Web.Utils.ControleUsuario" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">

        function OnTreeClick(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
            if (isChkBoxClick) {
                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node
                {
                    if (nxtSibling.tagName.toLowerCase() == "div") //if node has children
                    {
                        //check or uncheck children at all levels
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                //check or uncheck parents at all levels
                CheckUncheckParents(src, src.checked);
            }
        }

        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }

        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;

            if (parentNodeTable) {
                var checkUncheckSwitch;

                if (check) //checkbox checked
                    checkUncheckSwitch = true;
                else //checkbox unchecked
                    return;

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    //do the same recursively
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }

        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //utility function to get the container of an element by tagname
        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }

    </script>

    <style title="text/css">
        .treeNode {
            color: #ee6c6c;
            font: 12px Arial, Sans-Serif;
        }

        .rootNode {
            font-size: 12px;
            width: 100%;
        }

        .leafNode {
        }

        .divMenu {
        }

            .divMenu a {
                color: #008aff;
                font: 12px Arial, Sans-Serif;
            }

            .divMenu img {
                margin-right: 5px;
            }

        .marcProdInfo {
            color: #c32828;
            font-weight: bold;
            font-style: italic;
            text-align: center;
            display: none;
        }
    </style>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblControle" runat="server" Text="Controle" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpControle" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpControle_SelectedIndexChanged">
                                <asp:ListItem Value="1">Funcionário</asp:ListItem>
                                <asp:ListItem Value="2">Tipo Func.</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblFuncionario" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="true"
                                AutoPostBack="True" DataSourceID="odsFuncionarios" DataTextField="Name" DataValueField="Id">
                                <asp:ListItem Value="0">Selecione o Funcionário</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblTipoFunc" runat="server" Text="Tipo Funcionário" ForeColor="#0066FF" Visible="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFunc" runat="server" DataSourceID="odsTipoFunc" DataTextField="Name" 
                                DataValueField="Id" AutoPostBack="True" Visible="False">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div runat="server" id="divMenu" class="divMenu"></div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalvar" runat="server" OnClick="btnSalvar_Click" Text="Salvar alterações" Visible="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionarios" runat="server"
                    SelectMethod="ObtemFuncionariosAtivos" EnablePaging="false"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoFunc" runat="server" 
                    SelectMethod="ObtemTiposFuncionarioParaControleUsuarios" 
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
