using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace WebGlass.Business.ContasReceber.Ajax
{
    public interface IDescontoAcrescimo
    {
        string AplicarDescontoAcrescimoParcela(string idContaR, string valorString, string descontoString,
            string acrescimoString, string origem, string motivo);
    }

    internal class DescontoAcrescimo : IDescontoAcrescimo
    {
        public string AplicarDescontoAcrescimoParcela(string idContaR, string valorString, string descontoString,
            string acrescimoString, string origemString, string motivo)
        {
            try
            {
                decimal desconto = Glass.Conversoes.StrParaDecimal(descontoString);
                decimal acrescimo = Glass.Conversoes.StrParaDecimal(acrescimoString);
                decimal valor = Glass.Conversoes.StrParaDecimal(valorString);
                uint? idOrigem = Glass.Conversoes.StrParaUintNullable(origemString);
                motivo = motivo.Length > 200 ? motivo.Substring(0, 200) : motivo;

                // O desconto dado não pode ser superior ao valor da parcela
                if (desconto > valor)
                    return "Erro\tO desconto não pode ser superior ao valor da parcela.";

                ContasReceberDAO.Instance.DescontaAcrescentaContaReceber(Glass.Conversoes.StrParaUint(idContaR), desconto, acrescimo, idOrigem, motivo);

                if (desconto > 0)
                    EnviarMsgDesconto(Glass.Conversoes.StrParaUint(idContaR), desconto, motivo);

                return "ok\tDesconto/acréscimo aplicado.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao descontar valor da parcela.", ex);
            }
        }

        public void EnviarMsgDesconto(uint idContaR, decimal desconto, string motivo)
        {
            try
            {
                var conta = ContasReceberDAO.Instance.GetByIdContaR(idContaR);

                var assunto = "Desconto concedido de " + desconto.ToString("C");
                var mensagem = "Cliente: " + conta.IdNomeCli + Environment.NewLine +
                    "Conta: " + conta.Referencia + Environment.NewLine +
                    "Venc. " + conta.DataVec.ToString("dd/MM/yyyy") + Environment.NewLine +
                    "Valor: " + conta.ValorVec.ToString("c") + Environment.NewLine +
                    "Desconto: " + desconto.ToString("C") +"("+ Math.Round((desconto/conta.ValorVec)*100, 2) +"%)"+ Environment.NewLine +
                    "Motivo: " + motivo;

                var msg = new Glass.Global.Negocios.Entidades.Mensagem
                {
                    Assunto = assunto,
                    Descricao = mensagem,
                    IdRemetente = (int)UserInfo.GetUserInfo.CodUser
                };
                
                var usuariosEnvio = EmailConfig.UsuariosQueDevemReceberEmailDescontoMaior;

                var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                if (!string.IsNullOrEmpty(usuariosEnvio))
                {
                    msg.Destinatarios.AddRange(
                        new List<int>(usuariosEnvio.Split(',').Select(f => int.Parse(f)))
                            .Select(f =>
                                new Glass.Global.Negocios.Entidades.Destinatario
                                {
                                    IdFunc = f
                                }));
                }
                else if (EmailConfig.EnviarEmailDescontoMaiorApenasAdminConfig)
                {
                    uint? idAdminEmail = EmailConfig.AdministradorEnviarEmailDescontoMaior;
                    if (idAdminEmail > 0)
                        msg.Destinatarios.Add(new Glass.Global.Negocios.Entidades.Destinatario
                        {
                            IdFunc = (int)idAdminEmail.Value
                        });
                }
                else
                {
                    msg.Destinatarios.AddRange(FuncionarioDAO.Instance.GetAdministradores(false)
                        .Select(f =>
                            new Glass.Global.Negocios.Entidades.Destinatario
                            {
                                IdFunc = f.IdFunc
                            }));
                }

                var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                   .GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                   .SalvarMensagem(msg);
            }
            catch { }
        }
    }
}
