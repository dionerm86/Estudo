﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizacaoItemRentabilidade.aspx.cs" 
    Inherits="Glass.UI.Web.Relatorios.Rentabilidade.VisualizacaoItemRentabilidade" EnableViewState="false" EnableEventValidation="false"
    Title="Rentabilidade" MasterPageFile="~/Layout.master" %>

<asp:Content ID="header" runat="server" ContentPlaceHolderID="Header">
    <style type="text/css">
        .col-descricao {
            width: 250px;
        }

        .col-descricao-nivel1 {
            margin-left: 15px;
            padding-bottom: 3px;
            display: inline-block;
        }

        .col-descricao-nivel2 {
            margin-left: 30px;
            padding-bottom: 3px;
            display: inline-block;
        }

        .col-descricao-nivel3 {
            margin-left: 45px;
            padding-bottom: 3px;
            display: inline-block;
        }

        .opcoes-tabela {
            width: 10px;
        }

        .sub-item1 {
            display: none;
        }

        .sub-item2 {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <div id="tabelaVisualizacao"></div>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Rentabilidade/itemrentabilidadedatasourceview.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()) %>2'></script>
    <script type="text/javascript">

        var dataSource = <%= ObterDadosVisualizacao() %>;
        var visualizacao = new ItemRentabilidadeDataSourceView("rentabilidade1", dataSource);
        visualizacao.render($("#tabelaVisualizacao"));

    </script>

</asp:Content>