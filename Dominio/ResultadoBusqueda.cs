using System.ComponentModel.DataAnnotations;
public class ResultadoBusqueda
{
   
        public string archivo { get; set; }
        public int fila { get; set; }
        public string numero_envio { get; set; }
        public string serial_encontrado { get; set; }
        public string usuario { get; set; }
        public string reserva_sap { get; set; }
        public object cantidad { get; set; }
    

}
