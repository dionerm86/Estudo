<%@ Page Title="Desconto/Acréscimo em Conta a Pagar" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadDescontoParcelaPag.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDescontoParcelaPag" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
    function alteraTipo()
    {
        var rblTipo = FindControl("rblTipo", "table");
        var opcoes = rblTipo.getElementsByTagName("input");
        
        var tipo = -1;
        for (i = 0; i < opcoes.length; i++)
            if (opcoes[i].checked)
            {
                tipo = opcoes[i].value;
                break;
            }
        
        document.getElementById("compra").style.display = tipo == 0 ? "" : "none";
        document.getElementById("nf").style.display = tipo == 1 ? "" : "none";
        document.getElementById("comissao").style.display = tipo == 2 ? "" : "none";
        document.getElementById("custoFixo").style.display = tipo == 3 ? "" : "none";
        document.getElementById("impostoServico").style.display = tipo == 4 ? "" : "none";
        document.getElementById("cte").style.display = tipo == 5 ? "" : "none";
    }
    
    function setCustoFixo(idCustoFixo, descricao, nomeFornec, nomeLoja, descrPlanoConta, valorVenc, diaVenc, janela)
    {
        FindControl("hdfIdCustoFixo", "input").value = idCustoFixo;
        FindControl("txtCustoFixo", "input").value = descricao;
        janela.close();
    }
    
    function setContaPagar(idContaPg, valor) {
        FindControl("lblNumConta", "span").innerHTML = idContaPg;
        FindControl("lblValor", "span").innerHTML = valor;
    }

    function aplicarDescontoAcrescimo() {
        var numConta = FindControl("lblNumConta", "span").innerHTML;
        var valor = FindControl("lblValor", "span").innerHTML;
        var desconto = FindControl("txtDesconto", "input").value;
        var acrescimo = FindControl("txtAcrescimo", "input").value;
        var motivo = FindControl("txtMotivo", "textarea").value;
        
        if (numConta == "") {
            alert("Selecione uma conta a pagar.");
            return false;
        }

        if (desconto == "" && acrescimo == "")
        {
            alert("Informe o desconto ou acréscimo que será dado.");
            return false;
        }
        else if (desconto != "" && acrescimo != "")
        {
            alert("Informe apenas o desconto ou apenas o acréscimo.");
            return false;
        }

        if (motivo == "") {
            alert("Informe o motivo do desconto/acréscimo.");
            return false;
        }

        var btnDesconto = FindControl("btnDesconto", "input");
        btnDesconto.disabled = true;

        var response = CadDescontoParcelaPag.AplicarDescontoAcrescimo(numConta, valor.replace("R$", "").replace(" ", "").replace(".", ""), desconto, acrescimo, motivo).value;

        if (response == null) {
            alert("Falha ao descontar valor da conta a pagar.");
            btnDesconto.disabled = false;
            return;
        }

        response = response.split('\t');

        alert(response[1]);

        btnDesconto.disabled = false;

        if (response[0] == "Erro")
            return false;

        FindControl("lblNumConta", "span").innerHTML = "";
        FindControl("lblValor", "span").innerHTML = "";
        FindControl("txtDesconto", "input").value = "";
        FindControl("txtAcrescimo", "input").value = "";
        FindControl("txtMotivo", "textarea").value = "";

        cOnClick('btnBuscarCompra', null);
    }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" onclick="alteraTipo()">
                    <asp:ListItem Selected="True" Value="0">Compra</asp:ListItem>
                    <asp:ListItem Value="1">Nota Fiscal</asp:ListItem>
                    <asp:ListItem Value="2">Comissão</asp:ListItem>
                    <asp:ListItem Value="3">Custo Fixo</asp:ListItem>
                    <asp:ListItem Value="4">Imposto/Serviço avulso</asp:ListItem>
                    <asp:ListItem Value="5">CTe</asp:ListItem>
                </asp:RadioButtonList>
                <br />
                <table id="compra">
                    <tr>
                        <td>
                            <asp:Label ID="lblNumCompra" runat="server" Text="Número da Compra:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscarCompra', null);"
                                Width="70px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumCompra" runat="server" ControlToValidate="txtNumCompra"
                                ErrorMessage="*" ValidationGroup="compra"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBuscarCompra" runat="server" Text="Buscar Compra" Height="26px"
                                OnClick="btnBuscarCompra_Click" ValidationGroup="compra" />
                        </td>
                    </tr>
                </table>
                <table id="nf">
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Número da NF:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNf" onkeypress="return soNumeros(event, true, true);" runat="server"
                                onkeydown="if (isEnter(event)) cOnClick('btnBuscarNf', null);" Width="70px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumeroNf" runat="server" ControlToValidate="txtNumeroNf"
                                ErrorMessage="*" ValidationGroup="nf"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBuscarNf" runat="server" Text="Buscar NF" Height="26px" OnClick="btnBuscarNf_Click"
                                ValidationGroup="nf" />
                        </td>
                    </tr>
                </table>
                <div id="comissao">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpTipo_SelectedIndexChanged">
                                    <asp:ListItem Value="0">Funcionário</asp:ListItem>
                                    <asp:ListItem Value="1">Comissionado</asp:ListItem>
                                    <asp:ListItem Value="2">Instalador</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td align="right">
                                <asp:Label ID="Label4" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                            </td>
                            <td>
                                <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario" OnChange="showLoadGif()"
                                    DataTextField="Nome" DataValueField="IdFunc" AutoPostBack="True" OnDataBound="drpNome_DataBound"
                                    OnSelectedIndexChanged="drpNome_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfNome" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Button ID="btnBuscarComissao" runat="server" Text="Buscar Comissão" Height="26px" OnClick="btnBuscarComissao_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <table id="custoFixo">
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Custo Fixo:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCustoFixo" runat="server" Width="300px" onkeypress="return false"></asp:TextBox>
                            <asp:ImageButton ID="imgPesqCustoFixo" runat="server" ImageUrl="../Images/Pesquisar.gif"
                                OnClientClick="openWindow(600, 800, &quot;../Utils/SelCustoFixo.aspx&quot;); return false;" />
                            <asp:RequiredFieldValidator ID="valCustoFixo" runat="server" ControlToValidate="txtCustoFixo"
                                ErrorMessage="*" ValidationGroup="custoFixo"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="hdfIdCustoFixo" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBuscarCustoFixo" runat="server" Text="Buscar Custo Fixo" Height="26px"
                                OnClick="btnBuscarCustoFixo_Click" ValidationGroup="custoFixo" />
                        </td>
                    </tr>
                </table>
                <table id="impostoServico">
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Número do Lançamento de Imposto/Serviço avulso:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumImpostoServ" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscarImpostoServ', null);"
                                Width="70px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valImpostoServ" runat="server" ControlToValidate="txtNumImpostoServ"
                                ErrorMessage="*" ValidationGroup="impostoServ"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBuscarImpostoServ" runat="server" Text="Buscar Imposto/Serviço avulso"
                                Height="26px" OnClick="btnBuscarImpostoServ_Click" ValidationGroup="impostoServ" />
                        </td>
                    </tr>
                </table>
                <table id="cte">
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Número do CTe:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCte" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscarCompra', null);"
                                Width="70px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valCTe" runat="server" ControlToValidate="txtNumCte"
                                ErrorMessage="*" ValidationGroup="cte"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBuscarCte" runat="server" Text="Buscar CTe" Height="26px"
                                OnClick="btnBuscarCte_Click" ValidationGroup="cte" />
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
    </table>

    <script type="text/javascript">
        alteraTipo();
    </script>

    <table id="tbDesconto" runat="server" style="width: 100%" visible="false">
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                    DataKeyNames="IdContaPg" DataSourceID="odsContasPagar" EmptyDataText="Nenhuma conta a pagar encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setContaPagar('<%# Eval("IdContaPg") %>', '<%# ((decimal)Eval("ValorVenc") - (decimal)Eval("AcrescimoParc") + (decimal)Eval("DescontoParc")).ToString("C") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdContaPg" HeaderText="Num. Conta" SortExpression="IdContaPg" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVenc">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValorVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorVenc", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVenc">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescontoParc" DataFormatString="{0:C}" HeaderText="Desconto"
                            SortExpression="DescontoParc" />
                        <asp:BoundField DataField="AcrescimoParc" DataFormatString="{0:C}" HeaderText="Acréscimo"
                            SortExpression="AcrescimoParc" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Num. Conta:" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblNumConta" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Valor:" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblValor" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table width="1px">
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Desconto:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDesconto" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td width="100%">
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" Text="Acréscimo:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtAcrescimo" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label11" runat="server" Text="Motivo:" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4">
                            <asp:TextBox ID="txtMotivo" runat="server" MaxLength="200" Rows="2" TextMode="MultiLine"
                                Width="250px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="5">
                            <asp:Button ID="btnDesconto" runat="server" Text="Aplicar" OnClientClick="return aplicarDescontoAcrescimo();" />
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
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetForDescontoParcela" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ContasPagarDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="rblTipo" Name="tipoBusca" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNf" Name="numeroNf" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfIdCustoFixo" Name="idCustoFixo" PropertyName="Value"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumImpostoServ" Name="idImpostoServ" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipoComissao" PropertyName="SelectedValue"
                            Type="Object" />
                        <asp:ControlParameter ControlID="drpNome" Name="idFuncComissao" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIniComissao" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFimComissao" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedoresForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" SelectMethod="GetComissionadosForComissao"
                    TypeName="Glass.Data.DAL.ComissionadoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalador" runat="server" SelectMethod="GetColocadoresForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
