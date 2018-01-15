<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntregaCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.EntregaCte" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<script type="text/javascript">
    function validaDataEntProg(val, args) {
        var data = val.id.indexOf("ctvDataProg") > -1;
        var periodo = FindControl("drpPeriodo" + (data ? "Data" : "Hora"), "select").value;
        args.IsValid = periodo != 1 || args.Value != "";
    }

    function validaDataEntIni(val, args) {
        var data = val.id.indexOf("ctvDataIni") > -1;
        var periodo = FindControl("drpPeriodo" + (data ? "Data" : "Hora"), "select").value;
        args.IsValid = (periodo != 3 && periodo != 4) || args.Value != "";
    }

    function validaDataEntFim(val, args) {
        var data = val.id.indexOf("ctvDataFim") > -1;
        var periodo = FindControl("drpPeriodo" + (data ? "Data" : "Hora"), "select").value;
        args.IsValid = (periodo != 2 && periodo != 4) || args.Value != "";
    }
</script>

<div class="div-entrega">
    <div id="item-data">
        <div id="menu-data">
            <div class="dtvHeader">
                Tipo Período Data
                <%= ObtemTextoCampoObrigatorio(cvdrpPeriodoData) %>
            </div>
            <div class="dtvAlternatingRow">
                <asp:DropDownList ID="drpPeriodoData" runat="server" Height="20px" Width="150px"
                    AppendDataBoundItems="True" Enabled="true" Visible="true" onchange="ExibirControlesData()">
                    <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
                    <asp:ListItem Value="0" Text="Sem data definida"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Na data"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Até a data"></asp:ListItem>
                    <asp:ListItem Value="3" Text="A partir da data"></asp:ListItem>
                    <asp:ListItem Value="4" Text="No período"></asp:ListItem>
                </asp:DropDownList>
                <asp:CompareValidator ID="cvdrpPeriodoData" ControlToValidate="drpPeriodoData" runat="server"
                    ErrorMessage="Selecione o tipo de período: Data de entrega" Display="Dynamic"
                    ValidationGroup="c" ValueToCompare="selecione" Operator="NotEqual">*</asp:CompareValidator>
            </div>
        </div>
        <div id="divDataProg">
            <div class="dtvHeader">
                Data Programada
                <%= ObtemTextoCampoObrigatorio(ctvDataProg) %>
            </div>
            <div class="dtvAlternatingRow">
                <uc1:ctrlData ID="ctrlDataProg" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                    ValidateEmptyText="false" />
                <asp:CustomValidator ID="ctvDataProg" runat="server" ControlToValidate="ctrlDataProg$txtData"
                    Display="Dynamic" ValidationGroup="c" ErrorMessage="Para o tipo de data selecionado, o campo Data Programada deve ser preenchido"
                    ClientValidationFunction="validaDataEntProg" ValidateEmptyText="True">*</asp:CustomValidator>
            </div>
        </div>
        <div id="divDataInicio">
            <div class="dtvHeader">
                Data Inicial
                <%= ObtemTextoCampoObrigatorio(ctvDataIni) %>
            </div>
            <div class="dtvAlternatingRow">
                <div class="dtvAlternatingRow">
                    <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                        ValidateEmptyText="false" />
                    <asp:CustomValidator ID="ctvDataIni" runat="server" ControlToValidate="ctrlDataIni$txtData"
                        Display="Dynamic" ValidationGroup="c" ErrorMessage="Para o tipo de data selecionado, o campo Data Inicial deve ser preenchido"
                        ClientValidationFunction="validaDataEntIni" ValidateEmptyText="True">*</asp:CustomValidator>
                </div>
            </div>
        </div>
        <div id="divDataFim">
            <div class="dtvHeader">
                Data Final
                <%= ObtemTextoCampoObrigatorio(ctvDataFim) %>
            </div>
            <div class="dtvAlternatingRow">
                <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                    ValidateEmptyText="false" />
                <asp:CustomValidator ID="ctvDataFim" runat="server" ControlToValidate="ctrlDataFim$txtData"
                    Display="Dynamic" ValidationGroup="c" ErrorMessage="Para o tipo de data selecionado, o campo Data Final deve ser preenchido"
                    ClientValidationFunction="validaDataEntFim" ValidateEmptyText="True">*</asp:CustomValidator>
            </div>
        </div>
    </div>
    <div id="item-hora">
        <div id="menu-hora">
            <div class="dtvRow">
                <div class="dtvHeader">
                    <div id="drp-hora">
                        Tipo Período Hora
                        <%= ObtemTextoCampoObrigatorio(cvdrpPeriodoHora) %>
                    </div>
                </div>
                <div class="dtvAlternatingRow">
                    <asp:DropDownList ID="drpPeriodoHora" runat="server" Height="20px" Width="150px"
                        AppendDataBoundItems="True" Enabled="true" Visible="true" onchange="ExibirControlesHora()">
                        <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Sem hora definida"></asp:ListItem>
                        <asp:ListItem Value="1" Text="No horário"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Até o horário"></asp:ListItem>
                        <asp:ListItem Value="3" Text="A partir do horário"></asp:ListItem>
                        <asp:ListItem Value="4" Text="No intervalo de tempo"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:CompareValidator ID="cvdrpPeriodoHora" ControlToValidate="drpPeriodoHora" runat="server"
                        ErrorMessage="Selecione o tipo de período: Hora de entrega"
                        Display="Dynamic" ValidationGroup="c" ValueToCompare="selecione" Operator="NotEqual">*</asp:CompareValidator>
                </div>
            </div>
        </div>
        <div id="divHoraProg">
            <div class="dtvHeader">
                Hora Programada
                <%= ObtemTextoCampoObrigatorio(ctvHoraProg) %>
            </div>
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtHoraProg" runat="server" MaxLength="5" Width="44" onkeypress="return soNumeros(event, true, true)"
                    onkeydown="return mascara_hora(event, this)"></asp:TextBox>
                <asp:CustomValidator ID="ctvHoraProg" runat="server" ValidationGroup="c" ControlToValidate="txtHoraProg"
                    Display="Dynamic" ErrorMessage="Para o tipo de hora selecionado, o campo Hora Programada deve ser preenchido"
                    ClientValidationFunction="validaDataEntProg" ValidateEmptyText="True">*</asp:CustomValidator>
            </div>
        </div>
        <div id="divHoraInicio">
            <div class="dtvHeader">
                Hora Inicial
                <%= ObtemTextoCampoObrigatorio(ctvHoraIni) %>
            </div>
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtHoraInicio" runat="server" MaxLength="5" Width="44" onkeypress="return soNumeros(event, true, true)"
                    onkeydown="return mascara_hora(event, this)"></asp:TextBox>
                <asp:CustomValidator ID="ctvHoraIni" runat="server" ControlToValidate="txtHoraInicio"
                    Display="Dynamic" ValidationGroup="c" ErrorMessage="Para o tipo de hora selecionado, o campo Hora Inicial deve ser preenchido"
                    ClientValidationFunction="validaDataEntIni" ValidateEmptyText="True">*</asp:CustomValidator>
            </div>
        </div>
        <div id="divHoraFim">
            <div class="dtvHeader">
                Hora Final
                <%= ObtemTextoCampoObrigatorio(ctvHoraFim) %>
            </div>
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtHoraFim" runat="server" MaxLength="5" Width="44" onkeypress="return soNumeros(event, true, true)"
                    onkeydown="return mascara_hora(event, this)"></asp:TextBox>
                <asp:CustomValidator ID="ctvHoraFim" runat="server" ControlToValidate="txtHoraFim"
                    Display="Dynamic" ValidationGroup="c" ErrorMessage="Para o tipo de hora selecionado, o campo Hora Final deve ser preenchido"
                    ClientValidationFunction="validaDataEntFim" ValidateEmptyText="True">*</asp:CustomValidator>
            </div>
        </div>
    </div>
</div>
