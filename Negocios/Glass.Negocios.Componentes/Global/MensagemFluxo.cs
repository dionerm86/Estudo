using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Colosoft.Business;
using Colosoft.Query;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio das mensagens.
    /// </summary>
    public class MensagemFluxo : IMensagemFluxo
    {
        #region Mensagem

        /// <summary>
        /// Pesquisa as mensagem recebidas pelo funcionário.
        /// </summary>
        /// <param name="idDestinatario">Identificador do destinatário.</param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensRecebidas(int idDestinatario)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Mensagem>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IdRemetente=f.IdFunc", "f")
                .InnerJoin<Data.Model.Destinatario>("m.IdMensagem=d.IdMensagem", "d")
                .Select("m.IdMensagem, m.IdRemetente, f.Nome AS Remetente, m.Assunto, m.DataCad, d.Lida")
                .Where("d.IdFunc=?id AND (d.Cancelada IS NULL OR d.Cancelada=0)").Add("?id", idDestinatario)
                .OrderBy("DataCad DESC")
                .ToVirtualResult<Entidades.MensagemPesquisa>();                
        }

        /// <summary>
        /// Pesquisa as mensagens enviadas.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensEnviadas(int idRemetente)
        {
            var destinatarios = new List<Tuple<int, string>>();

            var resultado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Mensagem>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IdRemetente=f.IdFunc", "f")
                .Select("m.IdMensagem, m.IdRemetente, f.Nome AS Remetente, m.Assunto, m.DataCad")
                .Where("m.IdRemetente=?id").Add("?id", idRemetente)
                .OrderBy("DataCad DESC")
                .BeginSubQuery((sender, e) =>
                    {
                        var idMensagem = 0;
                        var nomes = new List<string>();
                        foreach(var i in e.Result)
                        {
                            idMensagem = i.GetInt32(0);
                            nomes.Add(i.GetString(1));
                        }

                        destinatarios.Add(new Tuple<int, string>(idMensagem, string.Join(", ", nomes.ToArray())));

                    }, 
                    (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.Destinatario>("d")
                    .InnerJoin<Data.Model.Funcionario>("d.IdFunc=f.IdFunc", "f")
                    .Where("d.IdMensagem=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("d.IdMensagem, f.Nome")
                .EndSubQuery()
                .ToVirtualResult<Entidades.MensagemPesquisa>();

            resultado.DataPageLoaded += (sender, e) =>
                {
                    // Carrega os destinatários da mensagem
                    foreach (var i in e.Page)
                        foreach (var j in destinatarios
                            .Where(f => f.Item1 == i.IdMensagem))
                            i.Destinatarios = j.Item2;

                    destinatarios.Clear();
                };

            return resultado;
        }

        /// <summary>
        /// Recupera os detalhes da mensagem.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Entidades.MensagemDetalhes ObtemDetalhesMensagem(int idMensagem)
        {
            Entidades.MensagemDetalhes.Destinatario[] destinatarios = null;

            var mensagem = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Mensagem>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IdRemetente=f.IdFunc", "f")
                .Select("m.IdMensagem, f.Nome AS Remetente, m.Assunto, m.Descricao, m.DataCad")
                .BeginSubQuery(
                    (sender, e) =>
                    {
                        destinatarios = e.Result.Select(f => 
                            new Entidades.MensagemDetalhes.Destinatario
                            {
                                IdDestinatario = f[0],
                                Nome = f[1],
                                Lida = f[2]
                            
                            }).ToArray();

                    }, (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.Destinatario>("d")
                    .InnerJoin<Data.Model.Funcionario>("d.IdFunc=f.IdFunc", "f")
                    .Where("d.IdMensagem=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("f.IdFunc, f.Nome, d.Lida")
                .EndSubQuery()
                .Where("m.IdMensagem=?id")
                .Add("?id", idMensagem)
                .Execute<Entidades.MensagemDetalhes>()
                .FirstOrDefault();

            if (mensagem != null)
                mensagem.Destinatarios.AddRange(destinatarios);

            return mensagem;
        }

        /// <summary>
        /// Recupera os dados da mensagem.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Entidades.Mensagem ObtemMensagem(int idMensagem)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Mensagem>()
                .Where("IdMensagem=?id")
                .Add("?id", idMensagem)
                .ProcessLazyResult<Entidades.Mensagem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da mensagem.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarMensagem(Entidades.Mensagem mensagem)
        {
            mensagem.Require("mensagem").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = mensagem.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da mensagem.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagem(Entidades.Mensagem mensagem)
        {
            mensagem.Require("mensagem").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = mensagem.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Verifica se existem novas mensagems para o destinatário.
        /// </summary>
        /// <param name="idDestinatario">Identificador do destinatário.</param>
        /// <returns></returns>
        public bool ExistemNovasMensagens(int idDestinatario)
        {
            /*return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Destinatario>("d")
                .InnerJoin<Data.Model.Mensagem>("d.IdMensagem=m.IdMensagem", "m")
                .Where("d.IdFunc=?id AND (d.Lida IS NULL OR d.Lida = 0)")
                .Add("?id", idDestinatario)
                .ExistsResult();*/

            /* Chamado 16068.
             * O sql que verificava novas mensagens estava incorreto, alteramos o sql para o mesmo sql que recupera as mensagens,
             * somente adicionando a condição de mensagens não lidas. */
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Mensagem>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IdRemetente=f.IdFunc", "f")
                .InnerJoin<Data.Model.Destinatario>("m.IdMensagem=d.IdMensagem", "d")
                .Select("m.IdMensagem, m.IdRemetente, f.Nome AS Remetente, m.Assunto, m.DataCad, d.Lida")
                .Where("d.IdFunc=?id AND (d.Lida IS NULL OR d.Lida = 0) AND (d.Cancelada IS NULL OR d.Cancelada = 0)").Add("?id", idDestinatario)
                .OrderBy("DataCad DESC")
                .ExistsResult();
        }

        /// <summary>
        /// Envia uma mensagem para o vendedor informando a alteração na data de entrega
        /// </summary>
        public Colosoft.Business.SaveResult EnviarMensagemVendedorAoAlterarDataEntrega(int idRemetente, int idVendedor, int idPedido, DateTime? dataEntrega)
        {
            var msg = new Entidades.Mensagem
            {
                Assunto = "Alteração da data de entrega",
                Descricao = string.Format("O pedido {0} teve sua data de entrega alterada para o dia {1}", idPedido, dataEntrega.Value.Date),
                IdRemetente = idRemetente
            };

            msg.Destinatarios.Add(new Entidades.Destinatario { IdFunc = idVendedor });

            return SalvarMensagem(msg);
        }

        #endregion

        #region Destinatario

        /// <summary>
        /// Pesquisa os possíveis destinatários que são funcionários.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IList<Entidades.DestinatarioPesquisa> PesquisarDestinatariosFuncionario(string nome)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>("f")
                .InnerJoin<Data.Model.TipoFuncionario>("f.IdTipoFunc=tf.IdTipoFuncionario", "tf")
                .Where("f.Situacao=?situacao")
                .Add("?situacao", Glass.Situacao.Ativo)
                .OrderBy("Nome")
                .Select("f.IdFunc AS IdDestinatario, f.Nome, tf.Descricao AS Funcao");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("f.Nome LIKE ?nome")
                    .Add("?nome", string.Format("%{0}%", nome));

            return consulta.ToVirtualResult<Entidades.DestinatarioPesquisa>();
        }

        /// <summary>
        /// Pesquisa os possíveis destinatários que são clientes.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IList<Entidades.DestinatarioPesquisa> PesquisarDestinatariosCliente(string nome)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>()
                .OrderBy("Nome")
                .Select("IdCli AS IdDestinatario, Nome");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("Nome LIKE ?nome")
                    .Add("?nome", string.Format("%{0}%", nome));

            return consulta.ToVirtualResult<Entidades.DestinatarioPesquisa>();
        }

        #endregion

        #region MensagemParceiro

        /// <summary>
        /// Pesquisa as mensagens de parceiros recebidas pelo cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosRecebidasCliente(int idCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IsFunc = 1 AND m.IdRemetente=f.IdFunc", "f")
                .InnerJoin<Data.Model.DestinatarioParceiroCliente>("m.IdMensagemParceiro=d.IdMensagemParceiro", "d")
                .Select("m.IdMensagemParceiro AS IdMensagem, m.IdRemetente, f.Nome AS Remetente, m.Assunto, m.DataCad, d.Lida")
                .Where("d.IdCli=?id").Add("?id", idCliente)
                .OrderBy("DataCad DESC")
                .ToVirtualResult<Entidades.MensagemPesquisa>();      
        }

        /// <summary>
        /// Pesquisa as mensagens de parceiros recebidas pelo funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosRecebidasFuncionario(int idFunc)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>("m")
                .InnerJoin<Data.Model.Cliente>("m.IsFunc = 0 AND m.IdRemetente=c.IdCli", "c")
                .InnerJoin<Data.Model.DestinatarioParceiroFuncionario>("m.IdMensagemParceiro=d.IdMensagemParceiro", "d")
                .Select("m.IdMensagemParceiro AS IdMensagem, m.IdRemetente, c.Nome AS Remetente, m.Assunto, m.DataCad, d.Lida")
                .Where("d.IdFunc=?id").Add("?id", idFunc)
                .OrderBy("DataCad DESC")
                .ToVirtualResult<Entidades.MensagemPesquisa>();    
        }

        /// <summary>
        /// Pesquisa as mensagens de parceiros enviadas pelo cliente.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosEnviadasCliente(int idRemetente)
        {
            var destinatarios = new List<Tuple<int, string>>();

            var resultado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>("m")
                .InnerJoin<Data.Model.Cliente>("m.IsFunc = 0 AND m.IdRemetente=c.IdCli", "c")
                .Select("m.IdMensagemParceiro AS IdMensagem, m.IdRemetente, c.Nome AS Remetente, m.Assunto, m.DataCad")
                .Where("m.IdRemetente=?id").Add("?id", idRemetente)
                .OrderBy("DataCad DESC")
                .BeginSubQuery((sender, e) =>
                    {
                        var idMensagem = 0;
                        var nomes = new List<string>();
                        foreach (var i in e.Result)
                        {
                            idMensagem = i.GetInt32(0);
                            nomes.Add(i.GetString(1));
                        }

                        destinatarios.Add(new Tuple<int, string>(idMensagem, string.Join(", ", nomes.ToArray())));

                    },
                    (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.DestinatarioParceiroFuncionario>("d")
                    .InnerJoin<Data.Model.Funcionario>("d.IdFunc=f.IdFunc", "f")
                    .Where("d.IdMensagemParceiro=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("d.IdMensagemParceiro, f.Nome")
                .EndSubQuery()
                .ToVirtualResult<Entidades.MensagemPesquisa>();

            resultado.DataPageLoaded += (sender, e) =>
            {
                // Carrega os destinatários da mensagem
                foreach (var i in e.Page)
                    foreach (var j in destinatarios
                        .Where(f => f.Item1 == i.IdMensagem))
                        i.Destinatarios = j.Item2;

                destinatarios.Clear();
            };

            return resultado.ToList();
        }

        /// <summary>
        /// Pesquisa as mensagens de parceiros enviadas pelo funcionário.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        public IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosEnviadasFuncionario(int idRemetente)
        {
            var destinatarios = new List<Tuple<int, string>>();

            var resultado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>("m")
                .InnerJoin<Data.Model.Funcionario>("m.IsFunc = 1 AND m.IdRemetente=f.IdFunc", "f")
                .Select("m.IdMensagemParceiro AS IdMensagem, m.IdRemetente, f.Nome AS Remetente, m.Assunto, m.DataCad, d.Lida")
                .Where("m.IdRemetente=?id").Add("?id", idRemetente)
                .OrderBy("DataCad DESC")
                .BeginSubQuery((sender, e) =>
                    {
                        var idMensagem = 0;
                        var nomes = new List<string>();
                        foreach (var i in e.Result)
                        {
                            idMensagem = i.GetInt32(0);
                            nomes.Add(i.GetString(1));
                        }

                        destinatarios.Add(new Tuple<int, string>(idMensagem, string.Join(", ", nomes.ToArray())));

                    },
                    (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.DestinatarioParceiroCliente>("d")
                    .InnerJoin<Data.Model.Cliente>("d.IdCli=f.IdCli", "c")
                    .Where("d.IdMensagemParceiro=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("d.IdMensagemParceiro, c.Nome")
                .EndSubQuery()
                .ToVirtualResult<Entidades.MensagemPesquisa>();

            resultado.DataPageLoaded += (sender, e) =>
            {
                // Carrega os destinatários da mensagem
                foreach (var i in e.Page)
                    foreach (var j in destinatarios
                        .Where(f => f.Item1 == i.IdMensagem))
                        i.Destinatarios = j.Item2;

                destinatarios.Clear();
            };

            return resultado;
        }

        /// <summary>
        /// Recupera os detalhes da mensagem do parceiro.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Entidades.MensagemDetalhes ObtemDetalhesMensagemParceiro(int idMensagem)
        {
            var destinatarios = new List<Entidades.MensagemDetalhes.Destinatario>();

            var mensagem = SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>("m")
                .LeftJoin<Data.Model.Funcionario>("m.IsFunc=1 AND m.IdRemetente=f.IdFunc", "f")
                .LeftJoin<Data.Model.Cliente>("m.IsFunc = 0 AND m.IdRemetente=c.IdCli", "c")
                .Select("m.IdMensagemParceiro AS IdMensagem, m.IdRemetente, ISNULL(f.Nome, c.Nome) AS Remetente, m.Assunto, m.Descricao, m.DataCad")
                .BeginSubQuery(
                    (sender, e) =>
                    {
                        destinatarios.AddRange(e.Result.Select(f =>
                            new Entidades.MensagemDetalhes.Destinatario
                            {
                                IdDestinatario = f[0],
                                Nome = f[1],
                                Lida = f[2]

                            }));

                    }, (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.DestinatarioParceiroFuncionario>("d")
                    .InnerJoin<Data.Model.Funcionario>("d.IdFunc=f.IdFunc", "f")
                    .Where("d.IdMensagemParceiro=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("f.IdFunc, f.Nome, d.Lida")
                .EndSubQuery()
                .BeginSubQuery(
                    (sender, e) =>
                    {
                        destinatarios.AddRange(e.Result.Select(f =>
                            new Entidades.MensagemDetalhes.Destinatario
                            {
                                IdDestinatario = f[0],
                                Nome = f[1],
                                Lida = f[2]

                            }));

                    }, (sender, e) => 
                    {
                        throw e.Result.Error;
                    })
                    .From<Data.Model.DestinatarioParceiroCliente>("d")
                    .InnerJoin<Data.Model.Cliente>("d.IdCli=c.IdCli", "c")
                    .Where("d.IdMensagemParceiro=?id")
                    .Add("?id", new ReferenceParameter("IdMensagem"))
                    .Select("c.IdCli, c.Nome, d.Lida")
                .EndSubQuery()
                .Where("m.IdMensagemParceiro=?id")
                .Add("?id", idMensagem)
                .Execute<Entidades.MensagemDetalhes>()
                .FirstOrDefault();

            if (mensagem != null)
                mensagem.Destinatarios.AddRange(destinatarios);

            return mensagem;
        }

        /// <summary>
        /// Recupera a mensagem.
        /// </summary>
        /// <param name="idMensagemParceiro">Identificador da mensagem.</param>
        /// <returns></returns>
        public Entidades.MensagemParceiro ObtemMensagemParceiro(int idMensagemParceiro)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.MensagemParceiro>()
                .Where("IdMensagemParceiro=?id")
                .Add("?id", idMensagemParceiro)
                .ProcessLazyResult<Entidades.MensagemParceiro>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da mensagem de parceiro.
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarMensagemParceiro(Entidades.MensagemParceiro mensagemParceiro)
        {
            mensagemParceiro.Require("mensagemParceiro").NotNull();

            var enviarPush = !mensagemParceiro.ExistsInStorage;

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = mensagemParceiro.Save(session);
                if (!resultado)
                    return resultado;

                 resultado = session.Execute(false).ToSaveResult();

                if (!resultado)
                    return resultado;

                //Dispara a notificação push para os dispositivos cadastrados
                if (enviarPush)
                    ServiceLocator.Current.GetInstance<IDeviceAppFluxo>().EnviarNotificacao(mensagemParceiro);

                return new SaveResult(true, null);
            }
        }

        /// <summary>
        /// Apaga os dados da mensagem de parceiro.
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagemParceiro(Entidades.MensagemParceiro mensagemParceiro)
        {
            mensagemParceiro.Require("mensagemParceiro").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = mensagemParceiro.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Marca a mensagem informada como lida/não lida para o parceiro logado.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <param name="lida"></param>
        /// <returns></returns>
        public SaveResult AlterarLeituraMensagemParceiro(int idMensagem, bool lida)
        {
            var usuarioAtual = UserInfo.GetUserInfo;

            if (!usuarioAtual.IsCliente)
                return new SaveResult(false, "O usuário logado não é um parceiro.".GetFormatter());

            var mensagem = ObtemMensagemParceiro(idMensagem);

            var destinatarioMensagem = mensagem.DestinatariosCliente.Where(f => f.IdCli == (int)usuarioAtual.IdCliente).FirstOrDefault();

            if (destinatarioMensagem == null)
                return new SaveResult(false, "A mensagem não foi encontrada.".GetFormatter());

            destinatarioMensagem.Lida = lida;

            return SalvarMensagemParceiro(mensagem);
        }

        /// <summary>
        /// Retorna a quantidade de mensagens não lidas do cliente logado
        /// </summary>
        /// <returns></returns>
        public int ObterQtdeMensagemParceiroNaoLidas()
        {
            if (!UserInfo.GetUserInfo.IsCliente)
                return 0;

            var idCli = UserInfo.GetUserInfo.IdCliente;

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.DestinatarioParceiroCliente>()
                .Where("IdCli = ?id AND IsNull(Lida, 0) = 0").Add("?id", idCli)
                .Count()
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault();
        }

        #endregion
    }
}
