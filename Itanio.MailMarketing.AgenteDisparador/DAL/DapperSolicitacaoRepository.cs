using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Itanio.MailMarketing.AgenteDisparador.Domain;

namespace Itanio.MailMarketing.AgenteDisparador.DAL
{
    public class DapperSolicitacaoRepository : ISolicitacaoRepository
    {
        private SqlConnection _conexao;

        public void Atualizar(Solicitacao item)
        {
            using (_conexao =
                new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                _conexao.Execute(@"Update UPM_SOLICITACAO SET   Total = @Total, 
                                                                Quantidade = @Quantidade , 
                                                                Status = @Status, 
                                                                Percentual = @Percentual, 
                                                                DataProcessamento = @DataProcessamento,
                                                                DataAtualizacao = GETDATE()
                                                                WHERE Id = @Id",
                    new
                    {
                        item.Id,
                        Status = (int) item.Status,
                        item.Percentual,
                        item.DataProcessamento,
                        item.Quantidade,
                        item.Total
                    });
            }
        }

        public IQueryable<Solicitacao> ListarPorMensagem(int idMensagem)
        {
            using (_conexao =
                new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                return _conexao.Query<Solicitacao>("select * from UPM_Solicitacao where idMensagem = @IdMensagem",
                    new {IdMensagem = idMensagem}).AsQueryable();
            }
        }

        public IQueryable<Solicitacao> ListarPorStatus(StatusSolicitacao status)
        {
            using (_conexao =
                new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                return _conexao.Query<Solicitacao>("select * from UPM_Solicitacao where status = @status",
                    new {status = (int) status}).AsQueryable();
            }
        }

        public Solicitacao ObterPorId(long id)
        {
            using (_conexao =
                new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                return _conexao.Query<Solicitacao>("select * from UPM_Solicitacao where id = @id", new {id}).Single();
            }
        }
    }
}