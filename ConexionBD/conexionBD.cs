using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools;
using practiquesIEI.Entities;

namespace apiIEI.ConexionBD
{
    public class ConexionBD
    {
        private static MySqlConnection conn;
        public static async Task Conectar()
        {
            Console.WriteLine("Conectando");
            if (conn != null)
            {
                return;
            }
            try
            {
                string connectionString = "" +
                     "server=172.23.186.115;" +
                    "port=3306;" +
                    "user=Administrador;" +
                    "password=root;" +
                    "database=centrosbd;";
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                Console.WriteLine("Conectado");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
        public static async Task<bool> insertCentro(centro_educativo centro)
        {
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                string consultaExistencia = $"SELECT COUNT(*) " +
                                            $"FROM centro_educativo " +
                                            $"WHERE nombre = @nombre AND " +
                                                  $"direccion = @direccion AND " +
                                                  $"codigo_postal = @cod_postal AND " +
                                                  $"longitud = @longitud AND " +
                                                  $"latitud = @latitud AND " +
                                                  $"telefono = @telefono ";
                using (MySqlCommand commandExistencia = new MySqlCommand(consultaExistencia, conn))
                {
                    commandExistencia.Parameters.AddWithValue("@nombre", centro.nombre);
                    commandExistencia.Parameters.AddWithValue("@direccion", centro.direccion);
                    commandExistencia.Parameters.AddWithValue("@cod_postal", centro.cod_postal);
                    commandExistencia.Parameters.AddWithValue("@longitud", centro.longitud);
                    commandExistencia.Parameters.AddWithValue("@latitud", centro.latitud);
                    commandExistencia.Parameters.AddWithValue("@telefono", centro.telefono);
                    int cantidadExistente = Convert.ToInt32(commandExistencia.ExecuteScalar());
                    if (cantidadExistente > 0)
                    {
                        return false;
                    }
                    else
                    {
                        using (MySqlCommand command = new MySqlCommand(
                         $"INSERT INTO centro_educativo (nombre, tipo, direccion, codigo_postal, longitud, latitud, telefono, descripcion, cod_localidad) VALUES (@nombre, @tipo, @direccion, @cod_postal, @longitud, @latitud, @telefono,@descripcion,@loc_codigo)", conn))
                        {
                            command.Parameters.AddWithValue("@nombre", centro.nombre);
                            command.Parameters.AddWithValue("@tipo", centro.tipo.ToString());
                            command.Parameters.AddWithValue("@direccion", centro.direccion);
                            command.Parameters.AddWithValue("@cod_postal", centro.cod_postal);
                            command.Parameters.AddWithValue("@longitud", centro.longitud);
                            command.Parameters.AddWithValue("@latitud", centro.latitud);
                            command.Parameters.AddWithValue("@telefono", centro.telefono);
                            command.Parameters.AddWithValue("@descripcion", centro.descripcion);
                            command.Parameters.AddWithValue("@loc_codigo", centro.loc_codigo);
                            command.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al insertar centro {centro.nombre}: {e.Message}");
            }
            return false;
        }
        public static async void insertLocalidad(localidad loc)
        {
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                string consultaExistencia = $"SELECT COUNT(*) " +
                                            $"FROM localidad " +
                                            $"WHERE loc_nombre = @nombre AND " +
                                                  $"loc_codigo = @codigo ";
                using (MySqlCommand commandExistencia = new MySqlCommand(consultaExistencia, conn))
                {
                    commandExistencia.Parameters.AddWithValue("@nombre", loc.nombre);
                    commandExistencia.Parameters.AddWithValue("@codigo", loc.codigo);
                    int cantidadExistente = Convert.ToInt32(commandExistencia.ExecuteScalar());
                    if (cantidadExistente == 0)
                    {
                        using (MySqlCommand command = new MySqlCommand(
                        $"INSERT INTO localidad(loc_codigo, loc_nombre, prov_codigo) VALUES (@codigo,@nombre,@prov_codigo)", conn))
                        {
                            command.Parameters.AddWithValue("@nombre", loc.nombre);
                            command.Parameters.AddWithValue("@codigo", loc.codigo);
                            command.Parameters.AddWithValue("@prov_codigo", loc.prov_codigo);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al insertar Localidad: {e.Message}");
            }
        }
        public static async void insertProvincia(provincia prov)
        {
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                string consultaExistencia = $"SELECT COUNT(*) " +
                                            $"FROM provincia " +
                                            $"WHERE prov_nombre = @nombre AND " +
                                                  $"prov_codigo = @codigo";
                using (MySqlCommand commandExistencia = new MySqlCommand(consultaExistencia, conn))
                {
                    commandExistencia.Parameters.AddWithValue("@nombre", prov.nombre);
                    commandExistencia.Parameters.AddWithValue("@codigo", prov.codigo);
                    int cantidadExistente = Convert.ToInt32(commandExistencia.ExecuteScalar());
                    if (cantidadExistente == 0)
                    {
                        using (MySqlCommand command = new MySqlCommand(
                        $"INSERT INTO provincia(prov_codigo, prov_nombre) VALUES (@codigo,@nombre)", conn))
                        {
                            command.Parameters.AddWithValue("@nombre", prov.nombre);
                            command.Parameters.AddWithValue("@codigo", prov.codigo);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al insertar Provincia: {e.Message}");
            }
        }
        public static async Task BorrarCentros()
        {
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                using (MySqlCommand command1 = new MySqlCommand(
                    $"DELETE FROM centro_educativo", conn))
                {
                    await command1.ExecuteNonQueryAsync();
                    Console.WriteLine("Datos de centros borrados correctamente.");
                }
                using (MySqlCommand command2 = new MySqlCommand(
                    $"DELETE FROM localidad", conn))
                {
                    await command2.ExecuteNonQueryAsync();
                    Console.WriteLine("Datos de localidad borrados correctamente.");
                }
                using (MySqlCommand command3 = new MySqlCommand(
                    $"DELETE FROM provincia", conn))
                {
                    await command3.ExecuteNonQueryAsync();
                    Console.WriteLine("Datos  de provincia borrados correctamente. \n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al ejecutar el comando: {e.Message} \n");
            }
        }
        public static async Task<List<centro_educativo>> getAllCentros()
        {

            List<centro_educativo> listCenters = new List<centro_educativo>();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                string consulta = $"SELECT * " +
                                  $"FROM centro_educativo c " +
                                  $"JOIN localidad l ON l.loc_codigo = c.cod_localidad " +
                                  $"JOIN provincia p ON l.prov_codigo = p.prov_codigo ";
                using (MySqlCommand command = new MySqlCommand(consulta, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Verifica si hay filas en el resultado
                        if (reader.HasRows)
                        {
                            // Itera a través de las filas
                            while (reader.Read())
                            {
                                centro_educativo centro = new centro_educativo();
                                centro.nombre = reader["nombre"].ToString();
                                centro.latitud = reader["latitud"].ToString().Replace(',', '.');
                                centro.longitud = reader["longitud"].ToString().Replace(',', '.');
                                switch (reader["tipo"].ToString())
                                {
                                    case "Público":
                                        centro.tipo = tipo_centro.Público;
                                        break;
                                    case "Privado":
                                        centro.tipo = tipo_centro.Privado;
                                        break;
                                    case "Concertado":
                                        centro.tipo = tipo_centro.Concertado;
                                        break;
                                    case "Otros":
                                        centro.tipo = tipo_centro.Otros;
                                        break;
                                    default:
                                        return null;
                                }
                                centro.cod_postal = reader["codigo_postal"].ToString();
                                centro.telefono = int.Parse(reader["telefono"].ToString());
                                centro.descripcion = reader["descripcion"].ToString();
                                centro.direccion = reader["direccion"].ToString();
                                centro.prov_nombre = reader["prov_nombre"].ToString();
                                centro.loc_nombre = reader["loc_nombre"].ToString();
                                listCenters.Add(centro);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al ejecutar el comando: {e.Message} \n");
            }
            return listCenters;
        }
        public static async Task<List<centro_educativo>> FindCentros(string loc, string tipo, string prov, string cp)
        {
            List<centro_educativo> listCenters = new List<centro_educativo>();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            try
            {
                string consulta = $"SELECT * " +
                                  $"FROM centro_educativo c " +
                                  $"JOIN localidad l ON l.loc_codigo = c.cod_localidad " +
                                  $"JOIN provincia p ON l.prov_codigo = p.prov_codigo " +
                                  $"WHERE (c.codigo_postal = COALESCE(@cod_postal, c.codigo_postal) OR COALESCE(@cod_postal, '') = '') " +
                                    $"AND (c.tipo = COALESCE(@tipo, c.tipo) OR COALESCE(@tipo, '') = '') " +
                                    $"AND (l.loc_nombre = COALESCE(@loc_nombre, l.loc_nombre) OR COALESCE(@loc_nombre, '') = '') " +
                                    $"AND (p.prov_nombre = COALESCE(@prov_nombre, p.prov_nombre) OR COALESCE(@prov_nombre, '') = '')";
                using (MySqlCommand command = new MySqlCommand(consulta, conn))
                {
                    command.Parameters.AddWithValue("@cod_postal", cp);
                    command.Parameters.AddWithValue("@tipo", tipo);
                    command.Parameters.AddWithValue("@loc_nombre", loc);
                    command.Parameters.AddWithValue("@prov_nombre", prov);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Verifica si hay filas en el resultado
                        if (reader.HasRows)
                        {
                            // Itera a través de las filas
                            while (reader.Read())
                            {
                                centro_educativo centro = new centro_educativo();
                                centro.nombre = reader["nombre"].ToString();
                                centro.latitud = reader["latitud"].ToString().Replace(',', '.');
                                centro.longitud = reader["longitud"].ToString().Replace(',', '.');
                                switch (reader["tipo"].ToString())
                                {
                                    case "Público":
                                        centro.tipo = tipo_centro.Público;
                                        break;
                                    case "Privado":
                                        centro.tipo = tipo_centro.Privado;
                                        break;
                                    case "Concertado":
                                        centro.tipo = tipo_centro.Concertado;
                                        break;
                                    case "Otros":
                                        centro.tipo = tipo_centro.Otros;
                                        break;
                                    default:
                                        return null;
                                }
                                centro.cod_postal = reader["codigo_postal"].ToString();
                                centro.telefono = int.Parse(reader["telefono"].ToString());
                                centro.descripcion = reader["descripcion"].ToString();
                                centro.direccion = reader["direccion"].ToString();
                                centro.prov_nombre = reader["prov_nombre"].ToString();
                                centro.loc_nombre = reader["loc_nombre"].ToString();

                                listCenters.Add(centro);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al ejecutar el comando: {e.Message} \n");
            }
            return listCenters;
        }

    }
}
