<%@ Page Title="Informações Adicionais dos Ajustes da Apuração do ICMS - Identificação dos Documentos Fiscais" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAjusteApuracaoIdentificacaoDocFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteApuracaoIdentificacaoDocFiscal" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../Scripts/jquery/jquery-1.9.0.js" type="text/javascript"></script>

    <script src="../Scripts/jquery/jquery-1.9.0.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
        
            codigoBind();
        
            $(".insert").click(function() {
                var $ajuste = $("select[id$=drpAjuste]").val();
                var $produto = $("select[id$=drpProduto]").val();
                var $codMod = $("select[id$=drpCodMod]").val();
                var $valor = $("input:text[id$=txtValor]").val();
                var $idNf = GetQueryString('idNf');
                var $tipoImposto = $("select[id$=drpTipoImposto]").val();

                var postData = { "postData":
                    { "IdABIA": $ajuste, "IdProd": $produto, "CodMod": $codMod, "ValAjItem": $valor.replace(",", "."), "IdNf": $idNf, "TipoImposto":$tipoImposto }
                };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/InserirAjusteApuracaoIdentificacaoDocFiscal",
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

            $('select[id$=drpAjuste]').change(function() {
                $("input:hidden[id$=hdfAjuste]").val($(this).val());
            });

        });

        function codigoBind() {
            var valor = $('select[id$=drpTipoImposto]').val();

            if (valor > 0) {
                var postData = { "tipoImposto": valor };

                $.ajax(
                    {
                        type: "POST",
                        url: "../Service/WebGlassService.asmx/ObterListaCodigoAjusteApuracao",
                        data: JSON.stringify(postData),
                        processData: false,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success:
                            function(result) {
                                $('select[id$=drpAjuste]').empty();
                                $('select[id$=drpAjuste]').append('<option value=""></option>');

                                $.each(result.d, function(key, value) {
                                    $('select[id$=drpAjuste]').append('<option value="' + value.Id + '">' + value.DescricaoAjuste + '</option>');
                                });

                                var codigo = $("input:hidden[id$=hdfAjuste]").val();

                                if (codigo != "") {
                                    $('select[id$=drpAjuste]').val(codigo);
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
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsInfo" GridLines="None"
                    EmptyDataText="Nenhum registro encontrado" ShowFooter="True" DataKeyNames="Id,IdNf"
                    OnRowUpdating="grdAjuste_RowUpdating" OnDataBound="grdAjuste_DataBound" OnRowCommand="grdAjuste_RowCommand">
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
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse registro?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                          <asp:TemplateField HeaderText="Imposto" SortExpression="DescricaoTipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoImposto") %>'>
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
                                <asp:HiddenField ID="hdfTipoImposto" runat="server" Value='<%# Bind("TipoImposto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ajuste" SortExpression="IdABIA">
                            <EditItemTemplate>
                                <asp:HiddenField ID="hdfAjuste" runat="server" Value='<%# Bind("IdABIA") %>' />
                                <sync:DropDownListTooltip ID="drpAjuste" runat="server"  Width="225px">
                                </sync:DropDownListTooltip>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <sync:DropDownListTooltip ID="drpAjuste" runat="server" Width="225px" ></sync:DropDownListTooltip>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescricaoAjuste") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="IdProd">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpProduto" runat="server" DataSourceID="odsProduto" DataTextField="Descricao"
                                    DataValueField="IdProd" SelectedValue='<%# Bind("IdProd") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpProduto" runat="server" DataSourceID="odsProduto" DataTextField="Descricao"
                                    DataValueField="IdProd" AppendDataBoundItems="True">
                                    <asp:ListItem Value="0">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescricaoProduto") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Mod. Doc. Fiscal" SortExpression="CodMod">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCodMod" runat="server" DataSourceID="odsCodMod" DataTextField="Descricao"
                                    DataValueField="Codigo" SelectedValue='<%# Bind("CodMod") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCodMod" runat="server" AppendDataBoundItems="True" DataSourceID="odsCodMod"
                                    DataTextField="Descricao" DataValueField="Codigo">
                                    <asp:ListItem Value="0">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DescricaoCodMod") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor do Ajuste Oper/Item" SortExpression="VlAjItem">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValAjItem") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValAjItem") %>'></asp:Label>
                            </ItemTemplate>
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfo" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.AjusteApuracaoIdentificacaoDocFiscalDAO"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" EnablePaging="True"
                    UpdateMethod="Update"  DataObjectTypeName="Glass.Data.Model.AjusteApuracaoIdentificacaoDocFiscal">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                          <asp:ControlParameter ControlID="drpTipoImposto0" Name="tipoImposto" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:Parameter Name="idProd" Type="UInt32" />
                        <asp:Parameter Name="idABIA" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" SelectMethod="ObterProdutosNota"
                    TypeName="Glass.Data.DAL.ProdutoDAO" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjuste" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoApuracaoDAO"
                    >
                    <SelectParameters>
                        <asp:Parameter Name="tipoImposto" Type="Int32" />
                        <asp:Parameter Name="dataInicio" Type="String" />
                        <asp:Parameter Name="dataFim" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodMod" runat="server" SelectMethod="GetTabelaDocumentosFiscaisICMS"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
