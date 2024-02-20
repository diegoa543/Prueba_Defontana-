using System;
using System.Collections.Generic;

namespace Prueba_Defontana.Models;

public partial class Local
{
    public long IdLocal { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public virtual ICollection<Ventum> Venta { get; } = new List<Ventum>();
}
