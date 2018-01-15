<%@ Page Title="Ajuste/Benefício/Incentivo da Apuração" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAjusteBeneficioIncentivoApuracao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteBeneficioIncentivoApuracao"
    EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            codigoBind();

            $(".insert").click(function() {
                var $tipoImposto = $("select[id$=drpTipoImposto]").val();
                var enddate = $("input:text[id$=txtData_txtData]").val();
                var split = enddate.split('/');
                var $data = new Date(split[2], split[1] - 1, split[0]);
                var $codigo = $("select[id$=drpCodigo]").val();
                var $valor = $("input:text[id$=txtValor]").val();
                var $obs = $("textarea[id$=txtObs]").val();
                var $uf = $("select[id$=drpUf]").val();

                if ($codigo == "") {
                    alert("Selecione o código.");
                    return false;
                }

                if ($valor == "") {
                    alert("Informe um valor.");
                    return false;
                }

                var postData = { "postData":
                    { "Data": $data, "IdAjBenInc": $codigo, "Valor": $valor.replace(",", "."), "Observacao": $obs, "TipoImposto": $tipoImposto, "Uf": $uf }
                };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/InserirAjusteBeneficioIncentivoApuracao",
                    data: JSON.stringify(postData),
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success:
                        function(result) {
                            document.location.reload(true);
                        },
                    error:
                        function(result) {
                            alert("Ocorreu um erro: " + "\r\n" + jQuery.parseJSON(result.responseText).Message);
                        }
                });
            });

            $('select[id$=drpTipoImposto]').change(function() {
                codigoBind();
            });

            $('select[id$=drpUf]').change(function() {
                codigoBind();
            });

            $('select[id$=drpCodigo]').change(function() {
                $("input:hidden[id$=hdfCodigo]").val($(this).val());
            });

        });

        function codigoBind() {
            var valor = $('select[id$=drpTipoImposto]').val();
            var data = $("input[id$=txtData_txtData]").val();

            if (valor > 0 && !!data) {
                var uf = $("select[id$=drpUf]").val();
                var postData = { "tipoImposto": valor, "uf": uf, "data": data };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/ObterListaCodigoAjuste",
                    data: JSON.stringify(postData),
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success:
                        function(result) {
                            $('select[id$=drpCodigo]').empty();
                            $('select[id$=drpCodigo]').append('<option value=""></option>');

                            $.each(result.d, function(key, value) {
                                $('select[id$=drpCodigo]').append('<option value="' + value.IdAjBenInc + '">' + value.CodigoDescricao + '</option>');
                            });

                            var codigo = $("input:hidden[id$=hdfCodigo]").val();

                            if (codigo != "") {
                                $('select[id$=drpCodigo]').val(codigo);
                            }
                        },
                    error:
                        function(result) {
                            alert("Ocorreu um erro: " + "\r\n" + jQuery.parseJSON(result.responseText).Message);
                        }
                });
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="txtDataInicio" runat="server" ValidateEmptyText="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="txtDataFim" runat="server" ValidateEmptyText="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de Imposto: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoImposto0" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdAjuste" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsAjuste" GridLines="None"
                    DataKeyNames="Id" EmptyDataText="Nenhum registro encontrado" ShowFooter="True"
                    OnRowUpdating="grdAjuste_RowUpdating">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" class="editar" runat="server" CausesValidation="False"
                                    CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false" />
                                <asp:ImageButton ID="imgInfo" runat="server" CausesValidation="False" CommandArgument='<%# Eval("Id") + ";" + Eval("TipoImposto") %>'
                                    CommandName="Info" ImageUrl="~/Images/Nota.gif" ToolTip="INFORMAÇÕES ADICIONAIS DOS AJUSTES DA APURAÇÃO DO ICMS" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Imposto" SortExpression="DescricaoTipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescricaoTipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="Data">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ValidateEmptyText="True" Data='<%# Bind("Data") %>'
                                    ValidationGroup="c" CallbackSelecionaData="codigoBind();" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ValidateEmptyText="True" ValidationGroup="c"
                                     CallbackSelecionaData="codigoBind();" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="UF" SortExpression="Uf">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" DataSourceID="odsUf"
                                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# Bind("Uf") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvUf" runat="server" ControlToValidate="drpUf" ErrorMessage="Informe a UF"
                                    SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" DataSourceID="odsUf"
                                    DataTextField="Value" DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvUf" runat="server" ControlToValidate="drpUf" ErrorMessage="Informe a UF"
                                    SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Uf") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="IdAjBenInc">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodigoDescricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField ID="hdfCodigo" runat="server" Value='<%# Bind("IdAjBenInc") %>' />
                                <asp:DropDownList ID="drpCodigo" runat="server" AppendDataBoundItems="True" Width="500px">
                                    <asp:ListItem Value="0">Selecione</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="drpCodigo"
                                    ErrorMessage="Selecione um código" MaximumValue="999999" MinimumValue="1" Type="Integer"
                                    ValidationGroup="c">*</asp:RangeValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCodigo" runat="server" AppendDataBoundItems="True" Width="500px">
                                </asp:DropDownList>
                                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="drpCodigo"
                                    ErrorMessage="Selecione um código" MaximumValue="999999" MinimumValue="1" Type="Integer"
                                    ValidationGroup="c">*</asp:RangeValidator>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("Valor") %>' onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtValor"
                                    ErrorMessage="Informe o valor" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtValor"
                                    ErrorMessage="Informe o valor" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação" SortExpression="Observacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Observacao") %>' TextMode="MultiLine"
                                    Width="229px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtObs"
                                    ErrorMessage="Informe uma observação" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Width="229px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtObs"
                                    ErrorMessage="Informe uma observação" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <a href="#" class="insert" style="text-decoration: none">
                                    <img style="text-decoration: none" src="../Images/ok.gif" /></a>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjuste" runat="server" DataObjectTypeName="Glass.Data.Model.AjusteBeneficioIncentivoApuracao"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoApuracaoDAO" EnablePaging="True"
                    UpdateMethod="Update" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtDataInicio" Name="dataInicio" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoImposto0" Name="tipoImposto" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigo" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoDAO"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUf" runat="server" SelectMethod="GetUf" TypeName="Glass.Data.DAL.CidadeDAO"
                    >
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfTipoImposto" runat="server" Value='<%# Bind("TipoImposto") %>' />
            </td>
        </tr>
    </table>
</asp:Content>
