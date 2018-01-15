<%@ Page Title="Cadastro de Cartão Não Identificado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadCartaoNaoIdentificado.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCartaoNaoIdentificado" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onInsertUpdate() {
            var numeroAutorizacao = FindControl("txtNumAutCartao", "input");
            var contaBanco = FindControl("drpContaBanco", "select");
            var valor = FindControl("txtValorMov", "input");
            var dataReceb = FindControl("ctrlDataReceb_txtData", "input");
            var tipoCartao = obterTipoCartao(idTipoCartao);

            // Débito.
            if (tipoCartao == "1") {
                drpNumParcelas.value = "1";
                drpNumParcelas.style.display = "none";
            }
                // Crédito.
            else
                drpNumParcelas.style.display = "";

            if (numeroAutorizacao != null && numeroAutorizacao.value == "") {
                alert("Informe o número de autorização.");
                return false;
            }

            if (contaBanco == null || contaBanco.value == 0) {
                alert("Informe a conta bancaria.");
                return false;
            }

            if (valor == null || valor.value == "") {
                alert("Informe o valor.");
                return false;
            }

            if (dataReceb == null || dataReceb.value == "") {
                alert("Informe a data da venda.");
                return false;
            }

            return true;
        }

        function ExibirEsconderParcelas(idTipoCartao) {
            var idTipoCartao = FindControl("drpTipoCartao", "select").value;
            var drpNumParcelas = FindControl("drpNumParcelas", "select");
            var tipoCartao = obterTipoCartao(idTipoCartao);

            // Débito.
            if (tipoCartao == "1") {
                drpNumParcelas.value = "1";
                drpNumParcelas.style.display = "none";
            }
            // Crédito.
            else
                drpNumParcelas.style.display = "";
        }

        function obterTipoCartao(idTipoCartao) {
            var tipoCartao = CadCartaoNaoIdentificado.ObterTipoCartao(idTipoCartao);

            if (tipoCartao.error != null) {
                alertaPadrao("TipoCartao", tipoCartao.error.description, 'erro', 280, 600);
                return false;
            }
            
            if (tipoCartao.value.split('|')[0] == "Erro"){
                alert(tipoCartao.value.split('|')[1]);
            }
            
            // 1 - Débito.
            // 2 - Crédito.
            return tipoCartao.value.split('|')[1];
        }

    </script>

    <table style="width: 100%;">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCartaoNaoIdentificado" runat="server" AutoGenerateRows="False"
                    DataSourceID="odsCartaoNaoIdentificado" DefaultMode="Insert" GridLines="None"
                    DataKeyNames="IdCartaoNaoIdentificado">
                    <Fields>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label4" runat="server" Text="Tipo Cartão" ></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                DataTextField="Descricao" DataValueField="IdTipoCartao" onChange="ExibirEsconderParcelas()"
                                                SelectedValue='<%# Bind("TipoCartao") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="drpNumParcelas" runat="server" DataSourceID="odsTipoCartaoCredito" 
                                                 DataTextField="Value" DataValueField="Key" AppendDataBoundItems="true"
                                                SelectedValue='<%# Bind("NumeroParcelas") %>'>
                                            </asp:DropDownList>
                                             &nbsp;
                                            <asp:Label ID="Label2" runat="server" Text="Nº Autorização"  ></asp:Label> 
                                            <asp:TextBox ID="txtNumAutCartao" runat="server" Width="150px"  Text='<%# Bind("NumAutCartao") %>'></asp:TextBox>
                                        </td>                                      
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblContaBancaria" runat="server" Text="Conta Bancária"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" colspan="3" nowrap="nowrap">
                                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" 
                                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                                SelectedValue='<%# Bind("IdContaBanco") %>'>
                                                <asp:ListItem Value="0" Text="" Selected="True" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorMov" runat="server" Text="Valor"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorMov" runat="server" MaxLength="25" TabIndex="1" Text='<%# Bind("Valor") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>    
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblDataMov" runat="server" Text="Data da Venda"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <uc1:ctrlData ID="ctrlDataReceb" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataVenda") %>' ExibirHoras="false" />
                                        </td>                                
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label3" runat="server" Text="Numero Estabelecimento"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox1" runat="server" MaxLength="25" TabIndex="1" Text='<%# Bind("NumeroEstabelecimento") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>    
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label5" runat="server" Text="Nº Cartão(Últimos 4 dig.)"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox2" runat="server" MaxLength="4" TabIndex="1" Text='<%# Bind("UltimosDigitosCartao") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>    
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Obs"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtObs" runat="server" Height="60px" TextMode="MultiLine" Text='<%# Bind("Observacao") %>'
                                                Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField runat="server" ID="hdfCxDiario" Value='<%# Bind("cxDiario") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblContaBancaria" runat="server" Text="Conta Bancária"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" colspan="3" nowrap="nowrap">
                                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                                SelectedValue='<%# Bind("IdContaBanco") %>' Enabled="false">
                                                <asp:ListItem Value="0" Text="" Selected="True" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorMov" runat="server" Text="Valor"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorMov" runat="server" MaxLength="25" TabIndex="1" Text='<%# Bind("Valor") %>' Enabled='<%# Eval("PodeEditarValor") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblDataMov" runat="server" Text="Data da Venda"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <uc1:ctrlData ID="ctrlDataReceb" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataVenda") %>' Enabled='<%# Eval("PodeEditarValor") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Obs"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtObs" runat="server" Height="60px" TextMode="MultiLine" Text='<%# Bind("Observacao") %>'
                                                Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField runat="server" ID="hdfSituacao" Value='<%# Bind("Situacao") %>' />
                                <asp:HiddenField runat="server" ID="hdfCxDiario" Value='<%# Bind("cxDiario") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAlterar" runat="server" CommandName="Update" Text="Atualizar" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCartaoNaoIdentificado" runat="server" 
        DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.CartaoNaoIdentificado"
        TypeName="Glass.Financeiro.UI.Web.Process.CartaoNaoIdentificado.CadastroCartaoNaoIdentificadoFluxo"
        SelectMethod="get_CNI" 
        InsertMethod="InserirCartaoNaoIdentificado" OnInserted="odsCartaoNaoIdentificado_Inserted"
        UpdateMethod="AlterarCartaoNaoIdentificado" UpdateStrategy="GetAndUpdate" OnUpdated="odsCartaoNaoIdentificado_Updated">    
        <InsertParameters>
            <asp:QueryStringParameter QueryStringField="cxDiario" Name="cxDiario" Type="Boolean"/>
        </InsertParameters>  
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartaoCredito" runat="server" SelectMethod="GetParcelasDrop"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObtemListaPorTipo"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        <SelectParameters>
            <asp:Parameter Name="tipo" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">

        $(function () {
            var hdfCxDiario = FindControl("hdfCxDiario", "input");
            if (hdfCxDiario != null)
                hdfCxDiario.value = GetQueryString("cxDiario") == "1" ? "true" : "false";
        });

    </script>

</asp:Content>