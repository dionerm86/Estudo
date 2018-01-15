<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlConsultaCadCliSintegra.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlConsultaCadCliSintegra" %>

<script type="text/javascript">
    
        function ConsSitCadContr(idCli){

        if (idCli == "") {
            alert("Selecione um cliente primeiro.");
            return false;
        }

        var retorno = ctrlConsultaCadCliSintegra.ConsultaSitCadContribuinte(idCli).value;
        
        if(retorno.split("&&")[0] == "alert"){
             alert(retorno.split("&&")[1]);
        }
        else if(retorno.split("&&")[0] == "confirm"){
            if(confirm(retorno.split("&&")[1]+"\n\nO cliente está inativo por ultrapassar a data limite da ultima consulta no sintegra, deseja ativa-lo?")){
                ctrlConsultaCadCliSintegra.AtivarCliente(idCli);
                window.location.reload();
                }
        }
        else
            alert(retorno.split("&&")[0]);
    }   
    
</script>

<asp:ImageButton ID="imgConSit" runat="server" OnPreRender="imgConSit_PreRender"
    ImageUrl="~/Images/ConsSitNFe.gif" ToolTip="Consultar a situação do contribuinte no SINTEGRA" />