using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Data.SqlClient;



namespace Registro_Libreria

{

    internal class Program

    {

        private static string CadenaConexion = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Registro_Libreria;Integrated Security=True";



        static void Main(string[] args)

        {

            int opcion;

            do

            {

                Console.WriteLine("====================================");

                Console.WriteLine("   SISTEMA DE GESTIÓN DE LIBRERÍA   ");

                Console.WriteLine("====================================");

                Console.WriteLine("1. Registrar libro / Agregar Stock");

                Console.WriteLine("2. Mostrar libros");

                Console.WriteLine("3. Buscar libro");

                Console.WriteLine("4. Registrar venta");

                Console.WriteLine("5. Mostrar ventas");

                Console.WriteLine("6. Salir");

                Console.Write("\nSeleccione una opción: ");



                if (int.TryParse(Console.ReadLine(), out opcion))

                {

                    switch (opcion)

                    {

                        case 1:

                            RegistrarLibro();

                            break;

                        case 2:

                            MostrarLibros();

                            break;

                        case 3:

                            BuscarLibro();

                            break;

                        case 4:

                            RegistrarVenta();

                            break;

                        case 5:

                            MostrarVentas();

                            break;

                        case 6:

                            Console.WriteLine("\nGracias por utilizar el sistema.");

                            break;

                        default:

                            Console.WriteLine("\nOpción no válida. Intente nuevamente.");

                            break;

                    }

                }

                else

                {

                    Console.WriteLine("\nIngrese solo números, por favor.");

                    opcion = 0;

                }



                Console.WriteLine("\nPresione una tecla para continuar...");

                Console.ReadKey(true);

                Console.Clear();



            } while (opcion != 6);

        }





        static void RegistrarLibro()

        {

            Console.WriteLine("\n--- OPCIÓN: REGISTRAR / AGREGAR STOCK ---");

            System.Collections.Generic.List<string> listaCodigos = new System.Collections.Generic.List<string>();



            Console.WriteLine("\n LISTA ACTUAL DE LIBROS:");

            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {

                string consulta = "SELECT Codigo, Nombre, Stock FROM Libros ORDER BY Codigo";

                SqlCommand cmd = new SqlCommand(consulta, con);

                try

                {

                    con.Open();

                    SqlDataReader lector = cmd.ExecuteReader();

                    if (!lector.HasRows)

                    {

                        Console.WriteLine("   (No hay libros registrados todavía)");

                    }

                    else

                    {

                        int posicion = 1;

                        while (lector.Read())

                        {

                            listaCodigos.Add(lector["Codigo"].ToString());

                            Console.WriteLine($"   [{posicion}] | Código: {lector["Codigo"]} | Nombre: {lector["Nombre"]} | Stock actual: {lector["Stock"]}");

                            posicion++;

                        }

                    }

                    lector.Close();

                }

                catch (Exception ex)

                {

                    Console.WriteLine($"\n Error al cargar la lista: {ex.Message}");

                    return;

                }

            }



            Console.WriteLine("\n¿Qué operación desea realizar?");

            Console.WriteLine("1 - Agregar STOCK a un libro existente");

            Console.WriteLine("2 - Registrar un libro NUEVO");

            Console.Write("Seleccione una opción: ");

            int eleccion = Convert.ToInt32(Console.ReadLine());



            if (eleccion == 1)

            {

                Console.WriteLine("\n--- AGREGAR STOCK ---");

                if (listaCodigos.Count == 0) { Console.WriteLine(" No hay libros."); return; }

                Console.Write($"Ingrese posición (1 al {listaCodigos.Count}): ");

                int pos = Convert.ToInt32(Console.ReadLine());

                if (pos < 1 || pos > listaCodigos.Count) { Console.WriteLine(" Posición inválida."); return; }

                string cod = listaCodigos[pos - 1];

                Console.Write("Cantidad a agregar: ");

                int cant = Convert.ToInt32(Console.ReadLine());



                using (SqlConnection con = new SqlConnection(CadenaConexion))

                {

                    string act = "UPDATE Libros SET Stock = Stock + @cant WHERE Codigo = @cod";

                    SqlCommand cmdAct = new SqlCommand(act, con);

                    cmdAct.Parameters.AddWithValue("@cant", cant);

                    cmdAct.Parameters.AddWithValue("@cod", cod);

                    try

                    {

                        con.Open();

                        cmdAct.ExecuteNonQuery();

                        Console.WriteLine("Stock actualizado correctamente.");

                    }

                    catch (Exception ex) { Console.WriteLine($" Error: {ex.Message}"); }

                }

            }

            else if (eleccion == 2)

            {

                Console.WriteLine("\n--- REGISTRAR NUEVO LIBRO ---");

                Console.Write("Código: "); string cod = Console.ReadLine().ToUpper();

                Console.Write("Nombre: "); string nom = Console.ReadLine();

                Console.Write("Precio: "); decimal pre = Convert.ToDecimal(Console.ReadLine());

                Console.Write("Stock inicial: "); int stk = Convert.ToInt32(Console.ReadLine());



                using (SqlConnection con = new SqlConnection(CadenaConexion))

                {

                    string ins = "INSERT INTO Libros (Codigo, Nombre, Precio, Stock) VALUES (@cod, @nom, @pre, @stk)";

                    SqlCommand cmdIns = new SqlCommand(ins, con);

                    cmdIns.Parameters.AddWithValue("@cod", cod);

                    cmdIns.Parameters.AddWithValue("@nom", nom);

                    cmdIns.Parameters.AddWithValue("@pre", pre);

                    cmdIns.Parameters.AddWithValue("@stk", stk);

                    try

                    {

                        con.Open();

                        cmdIns.ExecuteNonQuery();

                        Console.WriteLine(" Libro registrado.");

                    }

                    catch { Console.WriteLine(" ERROR: El código ya existe."); }

                }

            }

        }





        static void MostrarLibros()

        {

            Console.WriteLine("\n--- LISTA DE LIBROS REGISTRADOS ---");

            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {

                string consulta = "SELECT * FROM Libros";

                SqlCommand cmd = new SqlCommand(consulta, con);

                try

                {

                    con.Open();

                    SqlDataReader lector = cmd.ExecuteReader();

                    while (lector.Read())

                    {

                        Console.WriteLine($"\nCódigo: {lector["Codigo"]}");

                        Console.WriteLine($"Nombre: {lector["Nombre"]}");

                        Console.WriteLine($"Precio: S/{lector["Precio"]}");

                        Console.WriteLine($"Stock: {lector["Stock"]}");

                    }

                }

                catch (Exception ex) { Console.WriteLine($" Error: {ex.Message}"); }

            }

        }





        static void BuscarLibro()

        {

            Console.WriteLine("\n--- BUSCAR LIBRO ---");

            Console.Write("Ingrese código o nombre del libro: ");

            string busqueda = Console.ReadLine();



            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {

                string consulta = @"SELECT * FROM Libros 

                                    WHERE Codigo = @busc 

                                    OR LOWER(Nombre) LIKE '%' + LOWER(@busc) + '%'";



                SqlCommand cmd = new SqlCommand(consulta, con);

                cmd.Parameters.AddWithValue("@busc", busqueda);



                try

                {

                    con.Open();

                    SqlDataReader lector = cmd.ExecuteReader();



                    if (lector.Read())

                    {

                        Console.WriteLine($"\n LIBRO ENCONTRADO:");

                        do

                        {

                            Console.WriteLine($"Código: {lector["Codigo"]} | Nombre: {lector["Nombre"]} | Precio: S/{lector["Precio"]} | Stock: {lector["Stock"]}");

                        } while (lector.Read());

                    }

                    else

                    {

                        Console.WriteLine("\n No se encontró ningún libro con esos datos.");

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine($"\n Error: {ex.Message}");

                }

            }

        }





        static void RegistrarVenta()

        {

            Console.WriteLine("\n--- REGISTRAR NUEVA VENTA ---");

            System.Collections.Generic.List<string> listaCodigos = new System.Collections.Generic.List<string>();





            Console.WriteLine("\n LISTA DE LIBROS DISPONIBLES:");

            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {

                string consulta = "SELECT Codigo, Nombre, Precio, Stock FROM Libros ORDER BY Codigo";

                SqlCommand cmd = new SqlCommand(consulta, con);

                try

                {

                    con.Open();

                    SqlDataReader lector = cmd.ExecuteReader();

                    if (!lector.HasRows) { Console.WriteLine("   (No hay libros registrados)"); return; }

                    int pos = 1;

                    while (lector.Read())

                    {

                        listaCodigos.Add(lector["Codigo"].ToString());

                        Console.WriteLine($"   [{pos}] | {lector["Nombre"]} | Stock: {lector["Stock"]}");

                        pos++;

                    }

                    lector.Close();

                }

                catch (Exception ex) { Console.WriteLine($" Error: {ex.Message}"); return; }

            }





            Console.WriteLine("\n¿Cómo desea realizar la venta?");

            Console.WriteLine("1 - Ingresando el CÓDIGO (Acepta mayúsculas o minúsculas)");

            Console.WriteLine("2 - Eligiendo por NÚMERO DE POSICIÓN de la lista de arriba");

            Console.Write("Opción: ");

            int opcionVenta = Convert.ToInt32(Console.ReadLine());



            string codigoLibro = "";



            if (opcionVenta == 1)

            {

                Console.Write("Ingrese Código: ");

                codigoLibro = Console.ReadLine();

            }

            else if (opcionVenta == 2)

            {

                Console.Write($"Ingrese número de posición (1 al {listaCodigos.Count}): ");

                int numPos = Convert.ToInt32(Console.ReadLine());

                if (numPos < 1 || numPos > listaCodigos.Count) { Console.WriteLine(" Posición inválida."); return; }

                codigoLibro = listaCodigos[numPos - 1];

            }

            else

            {

                Console.WriteLine(" Opción inválida.");

                return;

            }





            decimal precioLibro = 0; int stockDisponible = 0; string nombreLibro = ""; bool libroExiste = false;



            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {



                string consultaBuscar = "SELECT * FROM Libros WHERE LOWER(Codigo) = LOWER(@cod)";

                SqlCommand comandoBuscar = new SqlCommand(consultaBuscar, con);

                comandoBuscar.Parameters.AddWithValue("@cod", codigoLibro);



                try

                {

                    con.Open();

                    SqlDataReader datos = comandoBuscar.ExecuteReader();

                    if (datos.Read())

                    {

                        libroExiste = true;

                        nombreLibro = datos["Nombre"].ToString();

                        precioLibro = Convert.ToDecimal(datos["Precio"]);

                        stockDisponible = Convert.ToInt32(datos["Stock"]);

                    }

                    datos.Close();



                    if (!libroExiste) { Console.WriteLine(" Código no registrado."); return; }



                    Console.WriteLine($"\n LIBRO SELECCIONADO: {nombreLibro}");

                    Console.WriteLine($"   Precio: S/{precioLibro} | Stock disponible: {stockDisponible}");

                    Console.Write("   Cantidad a vender: ");

                    int cantVender = Convert.ToInt32(Console.ReadLine());



                    if (cantVender <= 0) { Console.WriteLine(" Cantidad inválida."); return; }

                    if (cantVender > stockDisponible) { Console.WriteLine(" No hay suficiente stock."); return; }



                    decimal totalVenta = precioLibro * cantVender;



                    string guardarVenta = "INSERT INTO Ventas (codigo_libro, NombreLibro, Cantidad, Total) VALUES (@cod, @nom, @cant, @tot)";

                    SqlCommand cmdVenta = new SqlCommand(guardarVenta, con);

                    cmdVenta.Parameters.AddWithValue("@cod", codigoLibro.ToUpper());

                    cmdVenta.Parameters.AddWithValue("@nom", nombreLibro);

                    cmdVenta.Parameters.AddWithValue("@cant", cantVender);

                    cmdVenta.Parameters.AddWithValue("@tot", totalVenta);

                    cmdVenta.ExecuteNonQuery();



                    string actStock = "UPDATE Libros SET Stock = Stock - @cant WHERE LOWER(Codigo) = LOWER(@cod)";

                    SqlCommand cmdAct = new SqlCommand(actStock, con);

                    cmdAct.Parameters.AddWithValue("@cant", cantVender);

                    cmdAct.Parameters.AddWithValue("@cod", codigoLibro);

                    cmdAct.ExecuteNonQuery();



                    Console.WriteLine($"\n VENTA REGISTRADA CORRECTAMENTE.");

                    Console.WriteLine($"   Total a pagar: S/{totalVenta}");

                }

                catch (Exception ex) { Console.WriteLine($" Error: {ex.Message}"); }

            }

        }





        static void MostrarVentas()

        {

            Console.WriteLine("\n--- HISTORIAL DE VENTAS ---");

            using (SqlConnection con = new SqlConnection(CadenaConexion))

            {

                string consulta = "SELECT * FROM Ventas";

                SqlCommand cmd = new SqlCommand(consulta, con);

                try

                {

                    con.Open();

                    SqlDataReader lector = cmd.ExecuteReader();

                    int numVenta = 1; decimal sumaTotal = 0;

                    if (!lector.HasRows) { Console.WriteLine("(Sin ventas registradas)"); }

                    else

                    {

                        while (lector.Read())

                        {

                            Console.WriteLine($"\n VENTA N° {numVenta}");

                            Console.WriteLine($"   Código: {lector["codigo_libro"]}");

                            Console.WriteLine($"   Libro: {lector["NombreLibro"]}");

                            Console.WriteLine($"   Cantidad: {lector["Cantidad"]}");

                            Console.WriteLine($"   Subtotal: S/{lector["Total"]}");

                            sumaTotal += Convert.ToDecimal(lector["Total"]);

                            numVenta++;

                        }

                        Console.WriteLine("\n=====================================");

                        Console.WriteLine($" TOTAL GENERAL VENDIDO: S/{sumaTotal}");

                        Console.WriteLine("=====================================");

                    }

                }

                catch (Exception ex) { Console.WriteLine($" Error: {ex.Message}"); }

            }

        }



    }

}