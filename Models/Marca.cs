using System;
using System.Collections.Generic;

namespace Prueba_Defontana.Models;

public partial class Marca
{
    public long IdMarca { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; } = new List<Producto>();
}
