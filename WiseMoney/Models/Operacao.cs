using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WiseMoney.Models
{
    public class Operacao
    {
        public int Id { get; set; }
        public Conta Conta { get; set; }
        public Conta ContaDestino { get; set; }
        public bool IsEntrada { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ValorOperacao { get; set; }
    }
}
