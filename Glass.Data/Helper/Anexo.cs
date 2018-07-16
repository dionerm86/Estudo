using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Data.Helper
{
    public static class Anexo
    {
        public static void InserirAnexo(IFoto.TipoFoto tipo, uint idParent, byte[] buffer, string fileName, string descricao)
        {
            // Cadastra a foto
            var foto = IFoto.Nova(tipo);
            foto.IdParent = idParent;

            var extensao = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extensao))
                throw new Exception("O arquivo informado não possui extensão.");

            foto.Extensao = extensao;

            if (foto.ApenasImagens && !Arquivos.IsImagem(foto.Extensao))
                throw new Exception("Apenas imagens podem ser cadastradas.");

            if (PedidoEspelhoDAO.Instance.IsPedidoImpresso(null, foto.IdParent))
                throw new Exception("Não é possível inserir imagem em pedidos que já possuam etiqueta(s) impressa(s).");

            foto.Descricao = descricao;
            foto.IdFoto = foto.Insert();

            if (foto.IdFoto == 0)
                throw new Exception("Falha ao cadastrar foto.");

            try
            {
                // Salva o arquivo da foto
                if (!Directory.Exists(foto.Path))
                    Directory.CreateDirectory(foto.Path);

                ManipulacaoImagem.SalvarImagem(foto.FilePath, buffer);

                if (tipo == IFoto.TipoFoto.Pedido)
                {
                    // Cria o Log de inserção do Anexo imagem Pedido
                    LogAlteracao log = new LogAlteracao();
                    log.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                    log.IdRegistroAlt = (int)idParent;
                    log.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(LogAlteracao.TabelaAlteracao.Pedido, (int)idParent);
                    log.Campo = "Anexo Pedido";
                    log.DataAlt = DateTime.Now;
                    log.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
                    log.ValorAnterior = null;
                    log.ValorAtual = string.Format("{0} - Imagem Anexada", foto.IdFoto);
                    log.Referencia = LogAlteracao.GetReferencia(log.Tabela, idParent);
                    LogAlteracaoDAO.Instance.Insert(log);
                }
            }
            catch (Exception ex)
            {
                foto.Delete();
                throw ex;
            }

        }
    }
}
