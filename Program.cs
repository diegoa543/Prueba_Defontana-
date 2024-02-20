using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Prueba_Defontana.Models;
using System;

class Program
{
    static void Main(string[] args)
    {
        
        List<ResultadoConsulta> resultados = ObtenerDatos();

        //El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).
        int cantTotal = resultados.Sum(r => r.Detalle.Cantidad);
        decimal Totalmonto = resultados.Sum(r => r.Venta.Total);
        Console.WriteLine($"La cantidad total de ventas fue de {cantTotal} y el monto total de ventas fue de {Totalmonto}.");

        //El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).
        var topVenta = resultados.OrderByDescending(r => r.Venta.Total).First();
        var fechaVenta = topVenta.Venta.Fecha;
        var topMontoVenta = topVenta.Venta.Total;
        Console.WriteLine($"La venta con el monto más alto fue realizada el {fechaVenta} con un monto de {topMontoVenta}.");

        //Indicar cuál es el producto con mayor monto total de ventas. 
        var ventasProductos = resultados.GroupBy(r => r.Producto.IdProducto).Select(p => new {Producto= p.First().Producto,MontoTotal = p.Sum(v => v.Venta.Total)});
        var productoTop = ventasProductos.OrderByDescending(p => p.MontoTotal).First();
        var nombreProducto = productoTop.Producto.Nombre;
        var montoTotalProducto = productoTop.MontoTotal;
        Console.WriteLine($"El producto con el mayor monto total de ventas es {nombreProducto} con un monto total de {montoTotalProducto}.");

        //Indicar el local con mayor monto de ventas.
        var ventasLocal = resultados.GroupBy(l => l.Local.IdLocal).Select(r => new {Local = r.First().Local, MontoTotal = r.Sum(r => r.Venta.Total)});
        var localTopMonto = ventasLocal.OrderByDescending(l => l.MontoTotal).First();
        var localNombre = localTopMonto.Local.Nombre;
        var localTop = localTopMonto.MontoTotal;
        Console.WriteLine($"El local con el mayor monto en ventas es {localNombre} con un total de {localTop}.");

        //¿Cuál es la marca con mayor margen de ganancias?
        var ganaciasMarcas = resultados.GroupBy(m => m.Marca.IdMarca).Select(r => new { Marca = r.First().Marca, Ganacias = r.Sum(r => r.Venta.Total - r.Producto.CostoUnitario) });
        var marcaTop = ganaciasMarcas.OrderByDescending(m => m.Ganacias).First();
        var nombreMarca = marcaTop.Marca.Nombre;
        var gananacias = marcaTop.Ganacias;
        Console.WriteLine($"La marca con mayor ganancias es {nombreMarca} con un margen de {gananacias}.");

        //¿Cómo obtendrías cuál es el producto que más se vende en cada local?
        var ventas = resultados.GroupBy(v => new { v.Local.IdLocal, v.Producto.IdProducto }).Select(r => new { Local = r.First().Local, Producto = r.First().Producto, CantidadTotal = r.Sum(s => s.Detalle.Cantidad) });
        var topVendido = ventas.GroupBy(v => new { v.Local.IdLocal}).Select(r => r.OrderByDescending(o => o.CantidadTotal).First());
        foreach(var v in topVendido)
        {
            Console.WriteLine($"El producto más vendido en el local {v.Local.Nombre} es {v.Producto.Nombre} con una cantidad total de {v.CantidadTotal} vendidos.");
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
                            join ventaDetalle in context.VentaDetalles on venta.IdVenta equals ventaDetalle.IdVentaDetalle
                            join producto in context.Productos on ventaDetalle.IdProducto equals producto.IdProducto
                            join marca in context.Marcas on producto.IdMarca equals marca.IdMarca
                            join local in context.Locals on venta.IdLocal equals local.IdLocal
                            where venta.Fecha >= fechaInicio && venta.Fecha <= ultimaFecha
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