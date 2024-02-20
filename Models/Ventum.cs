using System;
using System.Collections.Generic;

namespace Prueba_Defontana.Models;

public partial class Ventum
{
    public long IdVenta { get; set; }

    public int Total { get; set; }

    public DateTime Fecha { get; set; }

    public long IdLocal { get; set; }

    public virtual Local IdLocalNavigation { get; set; } = null!;

    public virtual ICollection<VentaDetalle> VentaDetalles { get; } = new List<VentaDetalle>();
}
