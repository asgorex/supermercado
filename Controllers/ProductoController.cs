using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using supermercado.Models;

namespace supermercado.Controllers;

public class ProductoController : Controller
{
    private readonly string _connectionString;

    public ProductoController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MariaDBConnection");
    }
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Producto producto)
    {
        string query = "INSERT INTO producto (nombre, precio) VALUES (@nombre, @precio)";

        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using(var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@nombre", MySqlDbType.VarChar, 20).Value = producto.Nombre;
                command.Parameters.Add("@precio", MySqlDbType.Decimal).Value = producto.Precio;
                await command.ExecuteNonQueryAsync();
            }
        }
        return RedirectToAction("Listar", "Producto");
    }

    public async Task<IActionResult> Listar()
    {
        var ListarProductos = new List<Producto>();
        string query = "SELECT id_producto, nombre, precio FROM producto;";
        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using(var command = new MySqlCommand(query, connection))
            {
                using(var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var product = new Producto
                        {
                            IdProducto = reader.GetInt32("id_producto"),
                            Nombre = reader.GetString("nombre"),
                            Precio = reader.GetInt32("precio")
                        };
                        ListarProductos.Add(product);
                    }
                }
            }
        }
        return View(ListarProductos);
    }

    [HttpPost]
    public async Task<IActionResult> Borrar(int id)
    {
        string query = "DELETE FROM producto WHERE  id_producto = @id_producto;";

        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using(var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@id_producto", MySqlDbType.Int32).Value = id;
                

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if(rowsAffected == 0)
                {
                    return NotFound("No habia el registro para borrar");
                }
            }
        }
        return RedirectToAction("Listar", "Producto");
    }

    public async Task<IActionResult> Editar(int id)
    {
        Producto producto = null;

        string query = "SELECT id_producto, nombre, precio FROM producto WHERE id_producto = @id_producto;";
        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using(var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@id_producto", MySqlDbType.Int32).Value = id;
                using(var reader = await command.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        producto = new Producto
                        {
                        IdProducto = reader.GetInt32("id_producto"),
                        Nombre = reader.GetString("nombre"),
                        Precio = reader.GetInt32("precio")  
                        };
                    }
                    
                }
            }
        }
        return View("Actualizar", producto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Producto producto)
    {
        string query = "UPDATE producto SET nombre = @nombre, precio = @precio WHERE id_producto = @Id;";
        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using(var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = producto.Nombre;
                command.Parameters.Add("@precio", MySqlDbType.Int32).Value = producto.Precio;
                command.Parameters.Add("@Id", MySqlDbType.Int32).Value = producto.IdProducto;
                await command.ExecuteNonQueryAsync();
            }
        }
        return RedirectToAction("Listar","Producto");
    }
}
