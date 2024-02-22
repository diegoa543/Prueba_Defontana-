using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Prueba_Defontana.Models;
using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        
        List<ResultadoConsulta> resultados = ObtenerDatos();

        //El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).
        int cantTotal = resultados.Select(x=>x.Venta).Distinct().Count();
        decimal Totalmonto = resultados.Select(x => x.Venta.Total).Distinct().Sum();
        Console.WriteLine($"La cantidad total de ventas fue de {cantTotal} y el monto total de ventas fue de {Totalmonto}.");

        //El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).
        var topVenta = resultados.OrderByDescending(r => r.Venta.Total).First();
        var fechaVenta = topVenta.Venta.Fecha;
        var topMontoVenta = topVenta.Venta.Total;
        Console.WriteLine($"La venta con el monto más alto fue realizada el {fechaVenta} con un monto de {topMontoVenta}.");

        //Indicar cuál es el producto con mayor monto total de ventas. 
        var ventasProductos = resultados.GroupBy(r => r.Detalle.IdProducto).Select(p => new {Producto= p.First().Producto,MontoTotal = p.Sum(v => v.Detalle.TotalLinea)});
        var productoTop = ventasProductos.OrderByDescending(p => p.MontoTotal).First();
        var nombreProducto = productoTop.Producto.Nombre;
        var montoTotalProducto = productoTop.MontoTotal;
        Console.WriteLine($"El producto con el mayor monto total de ventas es {nombreProducto} con un monto total de {montoTotalProducto}.");

        //Indicar el local con mayor monto de ventas.
        var ventasLocal = resultados.GroupBy(l => l.Local.IdLocal).Select(r => new
        {
            nombre = r.Select(e => e.Local.Nombre).First(),
            monto = r.Select(a => a.Venta.Total).Distinct().Sum()
        }).Distinct().OrderByDescending(l => l.monto).First();
        var localNombre = ventasLocal.nombre;
        var localTop = ventasLocal.monto;
        Console.WriteLine($"El local con el mayor monto en ventas es {localNombre} con un total de {localTop}.");

        //¿Cuál es la marca con mayor margen de ganancias?
        var ganaciasMarcas = resultados.GroupBy(m => m.Marca.IdMarca).Select(r => new { Marca = r.First().Marca, Ganacias = r.Sum(r => r.Detalle.TotalLinea) });
        var marcaTop = ganaciasMarcas.OrderByDescending(m => m.Ganacias).First();
        var nombreMarca = marcaTop.Marca.Nombre;
        var gananacias = marcaTop.Ganacias;
        Console.WriteLine($"La marca con mayor ganancias es {nombreMarca} con un margen de {gananacias}.");

        //¿Cómo obtendrías cuál es el producto que más se vende en cada local?

        var ventas = from r in resultados
                     group r by new { r.Local.IdLocal, r.Producto.IdProducto } into g
                     select new { g.Key.IdLocal, g.Key.IdProducto, CantidadTotal = g.Sum(x => x.Detalle.Cantidad), rn = 1 };

        var topVentas = (from v in ventas
                         group v by v.IdLocal into g
                         select g.OrderByDescending(x => x.CantidadTotal).First()).ToList();

        var resultado = (from tv in topVentas
                         join r in resultados on new { tv.IdLocal, tv.IdProducto } equals new { r.Local.IdLocal, r.Producto.IdProducto }
                         select new { Local = r.Local.Nombre, Producto = r.Producto.Nombre, CantidadTotal = tv.CantidadTotal }).OrderBy(r => r.Local);


        foreach (var r in resultado)
        {
            Console.WriteLine($"Local: {r.Local}, Producto: {r.Producto}, Cantidad Total: {r.CantidadTotal}");
        }

        Console.ReadLine();

    }


    public static List<ResultadoConsulta> ObtenerDatos()
    {
        using (var context = new PruebaContext())
        {
            var ultimaFecha = context.Venta.Max(v => v.Fecha);
            var fechaInicio = ultimaFecha.AddDays(-30);
            var consulta = (from venta in context.Venta
                            join ventaDetalle in context.VentaDetalles on venta.IdVenta equals ventaDetalle.IdVenta
                            join producto in context.Productos on ventaDetalle.IdProducto equals producto.IdProducto
                            join marca in context.Marcas on producto.IdMarca equals marca.IdMarca
                            join local in context.Locals on venta.IdLocal equals local.IdLocal
                            where venta.Fecha >= fechaInicio
                            select new ResultadoConsulta
                            {
                                Local = local,
                                Marca = marca,
                                Producto = producto,
                                Venta = venta,
                                Detalle = ventaDetalle
                            }).ToList();

            return consulta;
        }

    }

}