using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiseMoney.Data;
using WiseMoney.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WiseMoney.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {


        private readonly DataContext _context;

        public ClienteController(DataContext context)
        {
            _context = context;
        }

     

        [HttpGet("Login/{email}, {senha}")]
        public async Task<ActionResult<Conta>> Login(string email, string senha) // aqui ele ta preenchendo o email e a senha com " " até dar 10 caracteres, não tive tempo de resolver
        {
            List<Cliente> clientes = _context.Cliente.ToList();
            Cliente cliente = clientes.Find(t => t.Email == email && t.Senha == senha);
            if (cliente == null)
            {
                return BadRequest("Conta não localizada");
            }
            List<Conta> contas = _context.Conta.ToList();
            Conta conta = contas.Find(t => t.Cliente.Id == cliente.Id);
            return Ok(conta);
        }



        // POST api/<ContaController>
        [HttpPost("CriarConta/{nome}, {email}, {senha}")]
        public async Task<ActionResult<List<Conta>>> CriarConta(string nome, string email, string senha)
        {
            Conta conta = new Conta();
            conta.Cliente = new Cliente();
            conta.Cliente.Nome = nome;
            conta.Cliente.Email = email;
            conta.Cliente.Senha = senha;
            conta.Saldo = 0;

            List<Cliente> clientes = _context.Cliente.ToList();
            Cliente clienteExistente = clientes.Find(t => t.Email == email);
            if (clienteExistente != null)
            {
                return BadRequest("Email já cadastrado");
            }
            else
            {
                int lastInsertedIdCliente;
                try
                {
                    lastInsertedIdCliente = clientes.Last().Id;
                }
                catch (Exception ex)
                {
                    lastInsertedIdCliente = 0;
                }
                if (lastInsertedIdCliente == 0)
                {
                    conta.Cliente.Id = 1;
                }
                else
                {
                    conta.Cliente.Id = lastInsertedIdCliente + 1;
                }
                _context.Cliente.Add(conta.Cliente);


                List<Conta> contas = _context.Conta.ToList();

                int idAleatorioConta;
                Conta contaExistente = null;
                do
                {
                    idAleatorioConta = geraNumeroConta();

                    contaExistente = contas.Find(t => t.Id == idAleatorioConta);
                }
                while (contaExistente != null);
                if (contaExistente == null)
                {
                    conta.Id = idAleatorioConta;
                }

                _context.Conta.Add(conta);
                _context.SaveChanges();
            }

            return _context.Conta.ToList();

        }

        private int geraNumeroConta()
        {
            Random numAleatorio = new Random();
            int valorInteiro = numAleatorio.Next(10, 21);
            return valorInteiro;

        }
    }
}
