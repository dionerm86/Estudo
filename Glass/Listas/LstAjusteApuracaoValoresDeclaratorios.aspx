<%@ Page Title="Informações Adicionais dos Ajustes da Apuração - Valores Declaratórios" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAjusteApuracaoValoresDeclaratorios.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteApuracaoValoresDeclaratorios" EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            
            codigoBind();

            $(".insert").click(function() {
                var $codigo = $("select[id$=drpCodigo]").val();
                var $valor = $("input:text[id$=txtValor]").val();
                var $descricao = $("textarea[id$=txtDescricao]").val();
                var enddate = $("input:text[id$=txtData_txtData]").val();
                var split = enddate.split('/');
                var $data = new Date(split[2], split[1] - 1, split[0]);

                var postData = { "postData":
                    { "IdAjBenInc": $codigo, "Valor": $valor.replace(",", "."), "Descricao": $descricao, "Data": $data }
                };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/InserirAjusteApuracaoValorDeclaratorio",
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

            $('select[id$=drpUf]').change(function() {
                codigoBind();
            });

            $('select[id$=drpCodigo]').change(function() {
                $("input:hidden[id$=hdfCodigo]").val($(this).val());
            });
        });

        function codigoBind() {
            var uf = $('select[id$=drpUf]').val();
            var data = $('input[id$=txtData_txtData]').val();

            if (!!uf && !!data) {
                var postData = { "tipoImposto": -1, "data": data, "uf" : uf };

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
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsAjuste" 
                    GridLines="None" ShowFooter="True" OnRowUpdating="grdAjuste_RowUpdating"
                    OnDataBound="grdAjuste_DataBound" OnRowCommand="grdAjuste_RowCommand" 
                    DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False" CommandName="Edit"
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="UF" SortExpression="UF">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key" 
                                    SelectedValue='<%# Eval("Uf") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key" 
                                    SelectedValue='<%# Eval("Uf") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("Uf") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="Data">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" Data='<%# Bind("Data") %>' CallbackSelecionaData="codigoBind();" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" CallbackSelecionaData="codigoBind();" />
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="IdAjBenInc">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodigoDescricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                             <asp:HiddenField ID="hdfCodigo" runat="server" Value='<%# Bind("IdAjBenInc") %>' />
                                <asp:DropDownList ID="drpCodigo" runat="server" Width="300px">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCodigo" runat="server" Width="300px" >
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Valor", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("Valor") %>' onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' TextMode="MultiLine"
                                    Width="229px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Width="229px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <a href="#" class="insert" style="text-decoration: none">
                                    <img style="text-decoration: none" src="../Images/ok.gif" /></a>
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigo" runat="server" TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoDAO"
                     SelectMethod="GetAll">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjuste" runat="server" DataObjectTypeName="Glass.Data.Model.AjusteApuracaoValorDeclaratorio"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.AjusteApuracaoValorDeclaratorioDAO"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" UpdateMethod="Update" 
                    EnablePaging="True">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtDataInicio" Name="dataInicio" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsUf" runat="server" SelectMethod="GetUf" 
                    TypeName="Glass.Data.DAL.CidadeDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
