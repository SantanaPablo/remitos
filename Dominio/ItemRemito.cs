using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("items_remito")]
    public class ItemRemito
    {
        [Key]
        public int Id { get; set; }

        public int numero_item { get; set; }

        public string descripcion { get; set; }

        public string serial { get; set; }

        public string usuario { get; set; }

        [Required]
        public int cantidad { get; set; }

        public string detalle { get; set; }

        public string recibido_por { get; set; }

        public int id_remito { get; set; }

        [ForeignKey("id_remito")]
        public Remito Remito { get; set; }
    }
}