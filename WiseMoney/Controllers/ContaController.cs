using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ContaController : ControllerBase
    {
        private readonly DataContext _context;

        public ContaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("ObterSaldo/{email}, {senha}")]
        public async Task<ActionResult<String>> ObterSaldo(string email, string senha)
        {
            List<Cliente> clientes = _context.Cliente.ToList();
            Cliente cliente = clientes.Find(t => t.Email == email && t.Senha == senha);
            if (cliente == null)
            {
                return BadRequest("Conta não localizada");
            }
            List<Conta> contas = _context.Conta.ToList();
            Conta conta = contas.Find(t => t.Cliente.Id == cliente.Id);
            return Ok("Saldo: R$ " + conta.Saldo);

        }

        [HttpGet("ObterExtrato/{email}, {senha}")]
        public async Task<ActionResult<List<string>>> ObterExtrato(string email, string senha)
        {
            List<string> operacoesDoCliente = new List<string>();
            List<Cliente> clientes = _context.Cliente.ToList();
            Cliente cliente = clientes.Find(t => t.Email == email && t.Senha == senha);
            if (cliente != null)
            {
                List<Conta> contas = _context.Conta.ToList();
                Conta conta = contas.Find(t => t.Cliente.Id == cliente.Id);
                List<Operacao> operacoes = _context.Operacao.ToList();
               
                foreach (var operacao in operacoes)
                {
                    if (operacao.ContaDestino.Id == conta.Id)
                        operacoesDoCliente.Add("+" + operacao.ValorOperacao);
                    else if (operacao.Conta.Id == conta.Id || operacao.IsEntrada == false)
                        operacoesDoCliente.Add("-" + operacao.ValorOperacao);

                }
            }
            else
            {
                operacoesDoCliente.Add("Email ou senha inválidos.");
            }
            return operacoesDoCliente;
        }

        [HttpPut("Transferir/{idOrigem},{idDestino},{saldo}")]
        public async Task<ActionResult<List<Operacao>>> Transferir(int idOrigem, int idDestino, decimal saldo)
        {
            List<Conta> contas = await _context.Conta.ToListAsync();
            Conta contaOrigem = contas.Find(t => t.Id == idOrigem);
            Conta contaDestino = contas.Find(t => t.Id == idDestino);

            if (contaOrigem != null && contaDestino != null)
            {
                if (saldo <= contaOrigem.Saldo)
                {
                    contaDestino.Saldo = contaDestino.Saldo + saldo;
                    contaOrigem.Saldo = contaOrigem.Saldo - saldo;
                    Operacao operacao = new Operacao();
                    operacao.Conta = new Conta();
                    operacao.Conta = contaOrigem;
                    operacao.ContaDestino = new Conta();
                    operacao.ContaDestino = contaDestino;

                    operacao.ValorOperacao = saldo;
                    operacao.IsEntrada = true;
                    operacao.Id = 1;
                     _context.Conta.Update(contaOrigem);
                     _context.Conta.Update(contaDestino);
                    _context.Operacao.Add(operacao);
                    await _context.SaveChangesAsync();


                }
                else
                {
                    return BadRequest("Valor para transferência inválido.");
                }
            }
            else
            {
                return BadRequest("Dados informados inválidos.");
            }
            return await _context.Operacao.ToListAsync();
        }

        [HttpPut("Depositar/{id},{valor}")]
        public ActionResult<string> Depositar(int id, decimal valor)
        {
            Conta conta = _context.Conta.Find(id);
            if(conta != null)
            {
                Operacao operacao = new Operacao();
                operacao.IsEntrada = false;
                operacao.ValorOperacao = valor;
                
                List<Operacao> operacoes = _context.Operacao.ToList();
                int lastInsertedIdOperacao;
                try
                {
                    lastInsertedIdOperacao = operacoes.Last().Id;
                }
                catch (Exception ex)
                {
                    lastInsertedIdOperacao = 0;
                }
                if (lastInsertedIdOperacao == 0)
                {
                    operacao.Id = 1;
                }
                else
                {
                    operacao.Id = lastInsertedIdOperacao + 1;
                }
                _context.Operacao.Add(operacao);

                conta.Saldo = conta.Saldo + valor;
                _context.Conta.Update(conta);
                _context.SaveChanges();
               

              
                return Ok("Valor Depositado: R$" + valor);
            }
            return  BadRequest("Conta inválida");
        }
    }

}
