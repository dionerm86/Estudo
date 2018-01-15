using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MensagemDAO : BaseDAO<Mensagem, MensagemDAO>
    {
        public void EnviarMsgPrecoProdutoAlterado(Produto prodOld, Produto prodNew)
        {
            try
            {
                if (prodOld.Custofabbase == prodNew.Custofabbase && prodOld.CustoCompra == prodNew.CustoCompra && prodOld.ValorAtacado == prodNew.ValorAtacado &&
                    prodOld.ValorBalcao == prodNew.ValorBalcao && prodOld.ValorObra == prodNew.ValorObra)
                    return;

                var assunto = "Alteração no preço de venda dos produtos.";
                var mensagem = "O produto " + prodOld.Descricao + " teve seu preço alterado conforme abaixo:" + Environment.NewLine +
                    (prodOld.Custofabbase == prodNew.Custofabbase ? "" : "Custo Forn. Antigo: " + prodOld.Custofabbase.ToString("c") + " Custo Forn. Novo: " + prodNew.Custofabbase.ToString("c") + Environment.NewLine) +
                    (prodOld.CustoCompra == prodNew.CustoCompra ? "" : "Custo Imp. Antigo: " + prodOld.CustoCompra.ToString("c") + " Custo Imp. Novo: " + prodNew.CustoCompra.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorAtacado == prodNew.ValorAtacado ? "" : "Valor Atacado Antigo: " + prodOld.ValorAtacado.ToString("c") + " Valor Atacado Novo: " + prodNew.ValorAtacado.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorBalcao == prodNew.ValorBalcao ? "" : "Valor Balcão Antigo: " + prodOld.ValorBalcao.ToString("c") + " Valor Balcão Novo: " + prodNew.ValorBalcao.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorObra == prodNew.ValorObra ? "" : "Valor Obra Antigo: " + prodOld.ValorObra.ToString("c") + " Valor Obra Novo: " + prodNew.ValorObra.ToString("c"));

                var msg = new Mensagem
                {
                    Assunto = assunto,
                    Descricao = mensagem,
                    IdRemetente = (int)UserInfo.GetUserInfo.CodUser
                };

                var idAdminEnvio = EmailConfig.AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado;
                if (idAdminEnvio == null)
                    return;

                if (idAdminEnvio > 0)
                {
                    msg.Destinatarios = idAdminEnvio.ToString();
                }
                else
                    return;

                Insert(msg);
            }
            catch { }
        }

        public void EnviarMsgPrecoListProdutoAlterado(IList<Produto> produtosOld, IList<Produto> produtosNew)
        {
            for(int i=0; i < produtosOld.Count; i++)
            {
                if (produtosOld[i].IdProd == produtosNew[i].IdProd)
                    EnviarMsgPrecoProdutoAlterado(produtosOld[i], produtosNew[i]);
            }
        }

        public override uint Insert(Mensagem objInsert)
        {
            var idMensagem = base.Insert(objInsert);

            List<int> idsDestinatarios = objInsert.Destinatarios.Split(',').Select(f => int.Parse(f)).ToList();
            foreach (var idDest in idsDestinatarios)
                DestinatarioDAO.Instance.Insert(new Destinatario
                {
                    IdMensagem = (int)idMensagem,
                    IdFunc = idDest,
                    Lida = false,
                    Cancelada = false
                });

            return idMensagem;
        }
    }
}
