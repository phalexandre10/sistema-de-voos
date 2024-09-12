using SistemaReservasVoos.Data;
using SistemaReservasVoos.Modelos;

namespace SistemaReservasVoos.Servico
{

    public class ServicoPassageiro
    {
        private readonly SistemaReservasContext _db;

        public ServicoPassageiro(SistemaReservasContext db)
        {
            _db = db;
        }

        public async Task<Passageiro> ObterOuCadastrarPassageiro(string cpf, Passageiro novoPassageiro = null)
        {
            var passageiro = await _db.Passageiros.FindAsync(cpf);
            if (passageiro == null && novoPassageiro != null)
            {
                if (string.IsNullOrEmpty(novoPassageiro.CPF) || string.IsNullOrEmpty(novoPassageiro.Nome) ||
                    string.IsNullOrEmpty(novoPassageiro.Endereco) || string.IsNullOrEmpty(novoPassageiro.Celular) ||
                    string.IsNullOrEmpty(novoPassageiro.TelefoneFixo) || string.IsNullOrEmpty(novoPassageiro.Email))
                {
                    throw new ArgumentException("Todos os campos são obrigatórios para o cadastro de passageiro.");
                }

                _db.Passageiros.Add(novoPassageiro);
                await _db.SaveChangesAsync();
                passageiro = novoPassageiro;
            }
            return passageiro;
        }

        public async Task AtualizarDadosPassageiro(Passageiro passageiro)
        {
            var passageiroExistente = await _db.Passageiros.FindAsync(passageiro.CPF);
            if (passageiroExistente == null)
            {
                throw new InvalidOperationException("Passageiro não encontrado.");
            }

            _db.Entry(passageiroExistente).CurrentValues.SetValues(passageiro);
            await _db.SaveChangesAsync();
        }
    }
}
