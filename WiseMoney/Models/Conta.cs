using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WiseMoney.Models
{
    public class Conta
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }
    }
}
