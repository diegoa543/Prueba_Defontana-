using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba_Defontana.Models
{
    public class ResultadoConsulta
    {
        public Local Local { get; set; }
        public Marca Marca { get; set; }
        public Producto Producto { get; set; }
        public Ventum Venta { get; set; }
        public VentaDetalle Detalle { get; set; }
    }
}
