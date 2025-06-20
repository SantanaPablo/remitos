using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("remitos")]
    public class Remito
    {
        public int Id { get; set; }

        [Required]
        public string Numero { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public string Destino { get; set; }

        public List<ItemRemito> Items { get; set; }
    }
}