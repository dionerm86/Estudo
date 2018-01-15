<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlConfigAresta.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlConfigAresta" %>

<%@ Register Src="ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<table runat="server" id="tbConfigAresta" clientidmode="Static" cellpadding="1" cellspacing="1">
    <tr>
        <td colspan="2">Subgrupo</td>
        <td colspan="2">Beneficiamento</td>
        <td colspan="3">Espessura</td>
        <td colspan="2">Processo</td>
        <td>Aresta</td>
        <td>
            <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="ConfiguracaoAresta" IdRegistro="1" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList runat="server" ID="drpSubgrupo" AppendDataBoundItems="true" ClientIDMode="Static"
                DataSourceID="odsSubGrupo" DataValueField="IdSubgrupoProd" DataTextField="Descricao"
                OnChange='<%# this.ID +".configArestaCallback();" %>'>
                <asp:ListItem Value="0" Selected="True" Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <img src="../Images/plus.png" />
        </td>
        <td>
            <asp:DropDownList runat="server" ID="drpBenef" AppendDataBoundItems="true" ClientIDMode="Static"
                DataSourceID="odsBenef" DataValueField="Id" DataTextField="Name" OnChange='<%# this.ID +".configArestaCallback();" %>'>
                <asp:ListItem Value="0" Selected="True" Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <img src="../Images/plus.png" />
        </td>
        <td>
            <select id="drpCondEspessura" onchange='<%= this.ID +".configArestaCallback();" %>'>
                <option value="0"></option>
                <option value="1">&#61;</option>
                <option value="2">&lt;&gt;</option>
                <option value="3">&gt;</option>
                <option value="4">&lt;</option>
                <option value="5">&gt;&#61;</option>
                <option value="6">&lt;&#61;</option>
            </select></td>
        <td>
            <input type="text" id="txtEspessura" onkeypress="return soNumeros(event, true, true);" onchange='<%= this.ID +".configArestaCallback();" %>'
                style="width: 50px;" />
        </td>
        <td>
            <img src="../Images/plus.png" />
        </td>
        <td>
            <input type="text" id="txtProc" onchange='<%= this.ID +".loadProc(this);" %>'
                style="width: 50px;" />
            <asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfIdProcesso" />
        </td>
        <td>
            <img src="../Images/equals.png" />
        </td>
        <td>
            <input type="text" id="txtAresta" value='<%# this.ClientID %>' onkeypress="return soNumeros(event, true, true);" onchange='<%= this.ClientID +".configArestaCallback();" %>'
                style="width: 50px;" />
        </td>
        <td>
            <input type="image" id="imgAdicionar" src="../Images/Insert.gif" onclick='<%= this.ID +".adicionarLinha(); return false;" %>' />
            <input type="image" id="imgRemover" src="../Images/ExcluirGrid.gif" onclick='<%= this.ID +".removerLinha(this); return false;" %>' />
        </td>
    </tr>
</table>

<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfSubgrupo" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfBenef" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfCondEspessura" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfEspessura" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfAresta" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfIdProc" />
<asp:HiddenField runat="server" ClientIDMode="Static" ID="hdfCodProc" />

<colo:virtualobjectdatasource culture="pt-BR" id="odsSubgrupo" runat="server" selectmethod="GetList"
    typename="Glass.Data.DAL.SubgrupoProdDAO">
    <SelectParameters>
        <asp:Parameter Name="idGrupoProd" DefaultValue="1" />
    </SelectParameters>
</colo:virtualobjectdatasource>
<colo:virtualobjectdatasource culture="pt-BR" id="odsBenef" runat="server"
    selectmethod="ObtemBenefConfigAtivos"
    typename="Glass.Global.Negocios.IBeneficiamentoFluxo">
</colo:virtualobjectdatasource>


<script>

    function ObterconfigAresta() {

        var hdfSubgrupo = document.getElementById('hdfSubgrupo').value;
        var hdfBenef = document.getElementById('hdfBenef').value;
        var hdfCondEspessura = document.getElementById('hdfCondEspessura').value;
        var hdfEspessura = document.getElementById('hdfEspessura').value;
        var hdfAresta = document.getElementById('hdfAresta').value;
        var hdfIdProc = document.getElementById('hdfIdProc').value;
        var hdfCodProc = document.getElementById('hdfCodProc').value;

        return hdfSubgrupo + "|" + hdfBenef + "|" + hdfCondEspessura + "|" + hdfEspessura + "|" + hdfAresta + "|" + hdfIdProc+ "|" + hdfCodProc;
    }

    <%= "var "+ this.ID +" = new configAresta('" + this.ClientID + "');" %>

    function configAresta(nomeControle) {

        var vm = this;
        vm.nomeControle = nomeControle;
        vm.tabela = document.getElementById('tbConfigAresta');

        vm.hdfSubgrupo = document.getElementById('hdfSubgrupo');
        vm.hdfBenef = document.getElementById('hdfBenef');
        vm.hdfCondEspessura = document.getElementById('hdfCondEspessura');
        vm.hdfEspessura = document.getElementById('hdfEspessura');
        vm.hdfAresta = document.getElementById('hdfAresta');
        vm.hdfIdProc = document.getElementById('hdfIdProc');
        vm.hdfCodProc = document.getElementById('hdfCodProc');

        vm.adicionarLinha = function () {

            vm.tabela.insertRow(vm.tabela.rows.length);

            var pos = vm.tabela.rows.length - 1;
            vm.tabela.rows[pos].innerHTML = vm.tabela.rows[1].innerHTML;

            var drpSubgrupo = FindControl('drpSubgrupo', 'select', vm.tabela.rows[pos]);
            var drpBenef = FindControl('drpBenef', 'select', vm.tabela.rows[pos]);
            var drpCondEspessura = FindControl('drpCondEspessura', 'select', vm.tabela.rows[pos]);
            var txtEspessura = FindControl('txtEspessura', 'input', vm.tabela.rows[pos]);
            var txtAresta = FindControl('txtAresta', 'input', vm.tabela.rows[pos]);
            var txtProc = FindControl('txtProc', 'input', vm.tabela.rows[pos]);
            var hdfIdProcesso = FindControl('hdfIdProcesso', 'input', vm.tabela.rows[pos]);

            drpSubgrupo.id = vm.alteraTexto(drpSubgrupo.id, 'drpSubgrupo', 'drpSubgrupo' + pos);
            drpBenef.id = vm.alteraTexto(drpBenef.id, 'drpBenef', 'drpBenef' + pos);
            drpCondEspessura.id = vm.alteraTexto(drpCondEspessura.id, 'drpCondEspessura', 'drpCondEspessura' + pos);
            txtEspessura.id = vm.alteraTexto(txtEspessura.id, 'txtEspessura', 'txtEspessura' + pos);
            txtAresta.id = vm.alteraTexto(txtAresta.id, 'txtAresta', 'txtAresta' + pos);
            txtProc.id = vm.alteraTexto(txtProc.id, 'txtProc', 'txtProc' + pos);
            hdfIdProcesso.id = vm.alteraTexto(hdfIdProcesso.id, 'hdfIdProcesso', 'hdfIdProcesso' + pos);

            // Chamado 60602: Limpa os campos de processo, pois ao adicionar estava replicando para a nova linha o valor do campo
            txtProc.value = "";
            hdfIdProcesso.value = "";

            vm.atualizaBotoes();
        }

        vm.alteraTexto = function (textoOriginal, textoBuscar, textoAlterar) {

            var retorno = textoOriginal;
            var pos = 0;

            if (retorno != null)
                while ((pos = retorno.indexOf(textoBuscar, pos)) > -1) {
                    var inicio = retorno.substr(0, pos);
                    var fim = retorno.substr(pos + textoBuscar.length);

                    retorno = inicio + textoAlterar + fim;
                    pos += textoAlterar.length;
                }

            return retorno;
        }

        vm.atualizaBotoes = function () {            
            for (i = 1; i < vm.tabela.rows.length; i++) {

                var isUltimaLinha = (i + 1) == vm.tabela.rows.length;

                var imgAdd = FindControl('imgAdicionar', 'input', vm.tabela.rows[i].cells[10])
                var imgRem = FindControl('imgRemover', 'input', vm.tabela.rows[i].cells[10]);

                if (imgAdd != null)
                    imgAdd.style.display = isUltimaLinha ? '' : 'none';

                if (vm.tabela.rows.length <= 2)
                    imgRem.style.display = 'none';
                if(vm.tabela.rows.length > 2)
                    imgRem.style.display = '';
            }
        }

        vm.removerLinha = function (btnExcluir) {
            var linha = btnExcluir.parentElement.parentElement.rowIndex - 1;               
            debugger;

            var aux = vm.hdfSubgrupo.value.split(';');
            aux.splice(linha, 1);
            vm.hdfSubgrupo.value = aux.join(';');

            aux = vm.hdfBenef.value.split(';');
            aux.splice(linha, 1);
            vm.hdfBenef.value = aux.join(';');

            aux = vm.hdfCondEspessura.value.split(';');
            aux.splice(linha, 1);
            vm.hdfCondEspessura.value = aux.join(';');

            aux = vm.hdfEspessura.value.split(';');
            aux.splice(linha, 1);
            vm.hdfEspessura.value = aux.join(';');

            aux = vm.hdfAresta.value.split(';');
            aux.splice(linha, 1);
            vm.hdfAresta.value = aux.join(';');

            aux = vm.hdfCodProc.value.split(';');
            aux.splice(linha, 1);
            vm.hdfCodProc.value = aux.join(';');

            aux = vm.hdfIdProc.value.split(';');
            aux.splice(linha, 1);
            vm.hdfIdProc.value = aux.join(';');
            
            vm.tabela.deleteRow(linha+1);
            
            vm.atualizaBotoes();
        }

        vm.configArestaCallback = function () {

            var lstSubgrupo = [];
            var lstBenef = [];
            var lstCondEspessura = [];
            var lstEspessura = [];
            var lstAresta = [];
            var lstCodProc = [];
            var lstIdProc = [];

            var row = 0;

            for (i = 1; i < vm.tabela.rows.length; i++) {

                var subgrupo = FindControl('drpSubgrupo', 'select', vm.tabela.rows[i]).value;
                var benef = FindControl('drpBenef', 'select', vm.tabela.rows[i]).value;
                var condEspessura = FindControl('drpCondEspessura', 'select', vm.tabela.rows[i]).value;
                var espessura = FindControl('txtEspessura', 'input', vm.tabela.rows[i]).value;
                var aresta = FindControl('txtAresta', 'input', vm.tabela.rows[i]).value;
                var codProc = FindControl('txtProc', 'input', vm.tabela.rows[i]).value;
                var idProc = FindControl('hdfIdProcesso', 'input', vm.tabela.rows[i]).value;

                lstSubgrupo.push(subgrupo);
                lstBenef.push(benef);
                lstCondEspessura.push(condEspessura);
                lstEspessura.push(espessura);
                lstAresta.push(aresta);
                lstCodProc.push(codProc);
                lstIdProc.push(idProc);
            }

            vm.hdfSubgrupo.value = lstSubgrupo.join(';');
            vm.hdfBenef.value = lstBenef.join(';');
            vm.hdfCondEspessura.value = lstCondEspessura.join(';');
            vm.hdfEspessura.value = lstEspessura.join(';');
            vm.hdfAresta.value = lstAresta.join(';');
            vm.hdfCodProc.value = lstCodProc.join(';');
            vm.hdfIdProc.value = lstIdProc.join(';');
        }

        vm.carregaConfigAresta = function () {

            var numeroItens = vm.hdfSubgrupo.value.split(';').length;
            var row = 1;

            for (var i = 0; i < numeroItens; i++) {

                if (i > 0)
                    vm.adicionarLinha();

                FindControl('drpSubgrupo', 'select', vm.tabela.rows[row]).value = vm.hdfSubgrupo.value.split(';')[i];
                FindControl('drpBenef', 'select', vm.tabela.rows[row]).value = vm.hdfBenef.value.split(';')[i];
                FindControl('drpCondEspessura', 'select', vm.tabela.rows[row]).value = vm.hdfCondEspessura.value.split(';')[i];
                FindControl('txtEspessura', 'input', vm.tabela.rows[row]).value = vm.hdfEspessura.value.split(';')[i];
                FindControl('txtAresta', 'input', vm.tabela.rows[row]).value = vm.hdfAresta.value.split(';')[i];
                FindControl('txtProc', 'input', vm.tabela.rows[row]).value = vm.hdfCodProc.value.split(';')[i];
                FindControl('hdfIdProcesso', 'input', vm.tabela.rows[row]).value = vm.hdfIdProc.value.split(';')[i];

                row++;
            }

            vm.configArestaCallback();
        }

        vm.setProc = function (idProcesso, codInterno, control) {
        
            var parent = control.parentNode;

            FindControl("txtProc", "input", parent).value = codInterno;
            FindControl("hdfIdProcesso", "input", parent).value = idProcesso;

            vm.configArestaCallback();
        }

        vm.loadProc = function (control) {

            if (control.value == "") {
                vm.setProc("", "", control);
                return false;
            }

            var response = MetodosAjax.GetEtiqProcesso(control.value);

            if (response == null || response.error != null || response.value.split("\t")[0] == "Erro") {

                if(response == null)
                     alert("Falha ao buscar Processo. Ajax Error.");
                else if(response.error != null)
                     alert(response.error.description);
                else
                    alert(response.value.split("\t")[1]);
               
                vm.setProc("", "", control);
                return false;
            }

            vm.setProc(response.value.split("\t")[1], response.value.split("\t")[2], control);
        }
    }
    
    var vm = this;
    vm.nomeControle = this.ClientID;
    vm.tabela = document.getElementById('tbConfigAresta');

    var imgRem = FindControl('imgRemover', 'input', vm.tabela.rows[0].cells[10]);

    if (imgRem != null)
        imgRem.style.display = 'none';

</script>
