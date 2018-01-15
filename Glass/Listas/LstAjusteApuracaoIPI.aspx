<%@ Page Title="Ajustes da Apuração do IPI" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAjusteApuracaoIPI.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAjusteApuracaoIPI" EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        $(document).ready(function() {

            //codigoBind();

            $(".insert").click(function() {

                var $tipoImposto = 3;  //$("select[id$=drpTipoImposto]").val();

                var enddate = $("input:text[id$=txtData_txtData]").val();
                var split = enddate.split('/');
                var $data = new Date(split[2], split[1] - 1, split[0]);

                var $indicadorTipoAjuste = $("select[id$=drpIndicadorTipoAjuste]").val();

                var $valor = $("input:text[id$=txtValor]").val();

                var $codigo = $("input[id$=selCodAjuste_txtDescr]").val();

                var $indicadorOrigem = $("select[id$=drpIndicadorOrigem]").val();

                var $numeroDocumento = $("input:text[id$=txtNumeroDocumento]").val();

                var $descricao = $("textarea[id$=txtDescricao]").val();

                if ($codigo == "") {
                    alert("Selecione o código.");
                    return false;
                }

                if ($valor == "") {
                    alert("Informe um valor.");
                    return false;
                }

                var postData = { "postData":
                    {
                        "Data": $data, "CodAjuste": $codigo, "IndicadorTipoAjuste": $indicadorTipoAjuste, "Descricao": $descricao,
                        "Valor": $valor.replace(",", "."), "IndicadorOrigem": $indicadorOrigem, "NumeroDocumento": $numeroDocumento,
                        "TipoImposto": $tipoImposto
                    }
                };

                $.ajax(
                {
                    type: "POST",
                    url: "../Service/WebGlassService.asmx/InserirAjusteApuracaoIPI",
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


            /*$('select[id$=drpTipoImposto]').change(function() {
                codigoBind();
            });

            $('select[id$=drpCodigo]').change(function() {
                $("input:hidden[id$=hdfCodigo]").val($(this).val());
            });*/
        });

        function codigoBind() {
            var valor = $('select[id$=drpTipoImposto]').val();

            if (valor > 0) {
                var postData = { "tipoImposto": valor };

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
                                    $('select[id$=drpCodigo]').append('<option value="' + value.CodAjuste + '">' + value.CodigoDescricao + '</option>');
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
                       <%-- <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de Imposto: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoImposto0" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>--%>
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
                    EmptyDataText="Nenhum registro encontrado" ShowFooter="True" 
                    OnRowUpdating="grdAjuste_RowUpdating" 
                    DataKeyNames="Id,TipoImposto,CodAjuste" 
                    ondatabound="grdAjuste_DataBound" onrowcommand="grdAjuste_RowCommand">
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
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
<%--                        <asp:TemplateField HeaderText="Imposto" SortExpression="DescricaoTipoImposto">
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
                                <asp:Label ID="Label52" runat="server" Text='<%# Bind("DescricaoTipoImposto") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoImposto" runat="server" Value='<%# Bind("TipoImposto") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
--%>                        <asp:TemplateField HeaderText="Data" SortExpression="Data">
                            <EditItemTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" Data='<%# Bind("Data") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="txtData" runat="server" ReadOnly="ReadWrite" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ind. Tipo Ajuste" SortExpression="IndicadorTipoAjuste">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpIndicadorTipoAjuste" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoAjuste" DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IndicadorTipoAjuste") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpIndicadorTipoAjuste" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoAjuste" DataTextField="Descr" DataValueField="Id">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescricaoIndicadorTipoAjuste") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("Valor") %>' onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Valor", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Ajuste" SortExpression="CodAjuste">
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="selCodAjuste" runat="server" DataSourceID="odsCodigo" 
                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False" 
                                    TextWidth="150px" TituloTela="Selecione o Cód. Ajuste" 
                                    Valor='<%# Bind("CodAjuste") %>' 
                                    Descricao='<%# Eval("DescricaoCodAjuste") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelPopup ID="selCodAjuste" runat="server" DataSourceID="odsCodigo" 
                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False" 
                                    TextWidth="150px" TituloTela="Selecione o Cód. Ajuste" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescricaoCodAjuste") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ind. Origem" SortExpression="IndicadorOrigem">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpIndicadorOrigem" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsIndicadorOrigem" DataTextField="Descr" 
                                    DataValueField="Id" SelectedValue='<%# Bind("IndicadorOrigem") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpIndicadorOrigem" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsIndicadorOrigem" DataTextField="Descr" DataValueField="Id">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label51" runat="server" Text='<%# Bind("DescricaoIndicadorOrigem") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Documento" SortExpression="NumeroDocumento">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumeroDocumento" runat="server" Text='<%# Bind("NumeroDocumento") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumeroDocumento" runat="server"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NumeroDocumento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' Height="45px"
                                    TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Height="45px" TextMode="MultiLine"
                                    Width="250px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjuste" runat="server" DataObjectTypeName="Glass.Data.Model.AjusteApuracaoIPI"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.AjusteApuracaoIPIDAO" EnablePaging="True" UpdateMethod="Update"
                    >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="3" Name="tipoImposto" Type="Object" />
                        <asp:ControlParameter ControlID="txtDataInicio" Name="dataInicio" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoAjuste" runat="server" SelectMethod="GetIndicadorTipoAjuste"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigo" runat="server" 
                    SelectMethod="GetCodAjusteIpi" TypeName="Glass.Data.EFD.DataSourcesEFD"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndicadorOrigem" runat="server" SelectMethod="GetIndicadorOrigemDocumento"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
