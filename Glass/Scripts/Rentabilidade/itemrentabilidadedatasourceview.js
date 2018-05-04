var ItemRentabilidadeDataSourceViewHelper = {
    exibirSubItens: function(element, idPai) {

        element = $(element);
        var trs = element.parent().parent().parent().find('tr');

        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];

            if (tr.getAttribute("pai") == idPai) {
                var item = $(tr);

                if (item.is(':visible')) {
                    item.hide();
                } else {
                    $(tr).show();
                }
            }
        }

        var img = element.find("img");
        if (img.attr("src") == "../../images/mais.gif") {
            img.attr("src", "../../images/menos.gif")
        } else {
            img.attr("src", "../../images/mais.gif")
        }
    }
}

var ItemRentabilidadeDataSourceView = function (nome, itemRentabilidadeDataSource) {
    return {
        nome: nome,
        dataSource: itemRentabilidadeDataSource,
        _descritoresOrdenados: false,

        /**
         * Cria uma linha da tabela para o item informado.
         **/
        criarLinha: function (item, nivel, id) {

            var opcoes = $("<td></td>");
            var tr = $("<tr id=\"" + id + "-tr\"></tr>");

            tr.append(opcoes);
            tr.append($("<td class=\"col-descricao\"><div class=\"col-descricao-nivel" + nivel + "\">" + item.Descricao + "</div></td>"));
            tr.append($("<td>" + item.PercentualRentabilidade + "</td>"));
            tr.append($("<td>" + item.RentabilidadeFinanceira + "</td>"));

            if (!this._descritoresOrdenados) {

                var c = this.dataSource.Descritores.sort(function (a, b) {
                    if (a.Posicao < b.Posicao) {
                        return -1;
                    } else if (a.Posicao > b.Posicao) {
                        return 1;
                    } else {
                        return 0;
                    }
                });

                this._descritoresOrdenados = true;
            }

            for (var j = 0; j < this.dataSource.Descritores.length; j++) {
                var descritor = this.dataSource.Descritores[j];
                var td = $("<td></td>");
                td.text(item[descritor.Nome]);
                tr.append(td);
            }

            return tr;
        },

        /**
         * Cria as linhas para os itens informados.
         **/
        criarLinhas: function (idPai, itens, nivel) {

            var linhas = [];
            for (var i = 0; i < itens.length; i++) {
                var item = itens[i];

                // Monta o identificador do item
                var id = idPai + '-' + i;
                var tr = this.criarLinha(item, nivel, id);

                if (i % 2 == 0) {
                    tr.attr("class", "alt");
                }

                linhas.push(tr);

                if (item.Itens && item.Itens.length > 0) {

                    var linhas2 = this.criarLinhas(id, item.Itens, nivel + 1);

                    if (nivel == 1) {
                        var opcoes = tr.find("td").first();
                        opcoes.append($("<a href=\"#\" onclick=\"ItemRentabilidadeDataSourceViewHelper.exibirSubItens(this, '" + id + "')\"><img src=\"../../images/mais.gif\" /></a>"));
                    }

                    for (var j = 0; j < linhas2.length; j++) {
                        var linha = linhas2[j];

                        linha.attr("class", linha.attr("class") + " sub-item" + nivel);
                        linha.attr("pai", id);
                        linhas.push(linha);
                    }
                }
            }

            return linhas;
        },

        criarTabela: function (item) {

            if (item != null) { //.Itens && item.Itens.length > 0) {
                var tabela = $("<table class=\"gridStyle rentabilidade\"></table>");
                var cabecalho = $("<tr><th class=\"opcoes-tabela\"></th><th>Descrição</th><th>% Rent.</th><th>Rent. Financeira</th></tr>");
                tabela.append(cabecalho);

                // Percorre os descritores para montar o cabeçalho
                for (var i = 0; i < this.dataSource.Descritores.length; i++) {
                    var descritor = this.dataSource.Descritores[i];
                    var th = $("<th></th>");
                    th.text(descritor.Descricao);
                    cabecalho.append(th);
                }

                tabela.append(this.criarLinha(item, 0, nome));

                if (item.Itens && item.Itens.length > 0) {
                    var linhas = this.criarLinhas(nome, item.Itens, 1);
                    for (var i = 0; i < linhas.length; i++) {
                        tabela.append(linhas[i]);
                    }
                }
            }

            return tabela;
        },

        /**
         * Realiza a montagem da tabela no elemento pai informado.
         **/
        render: function (parent) {

            parent.append(this.criarTabela(this.dataSource.Item));
        }
    };
};
