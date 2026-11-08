using System;
using System.Numerics;
using System.Xml;

namespace ScriptCSVToXML
{
    class Program
    {
        static string pathCsv = "../../../NetflixOriginals.csv";
        static string pathXml = "../../../NetflixOriginals.xml";


        static void Main(string[] args)
        {
            bool salir = false; //Controla el bucle del menu: si el usuario le da a la opcion salir cambia a True
            while (!salir) //Mientras no le dé a salir, seguirá en el menú
            {
                //Muestro la opciones del menú
                Console.WriteLine("=====================================");
                Console.WriteLine("     CONVERTIR FORMATO CSV <-> XML");
                Console.WriteLine("====================================");
                Console.WriteLine($"Ruta CSV Actual: {pathCsv}");
                Console.WriteLine($"Ruta XML Actual: {pathXml}");
                Console.WriteLine("------------------------------------");
                Console.WriteLine("1. Configurar ruta del archivo CSV");
                Console.WriteLine("2. Configurar ruta del archivo XML");
                Console.WriteLine("3. Exportar de CSV a XML");
                Console.WriteLine("4. Exportar de XML a CSV");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine()!.Trim();

                switch (opcion)
                {
                    case "1":
                        //Llamo al metodo auxiliar para cambiar la ruta CSV
                        pathCsv = PedirRuta("CSV", pathCsv);
                        break;

                    case "2":
                        //Llamo al metodo auxiliar para cambiar la ruta XML
                        pathXml = PedirRuta("XML", pathXml);
                        break;

                    case "3":
                        //Abrimos el submenú de exportación para convertir de CSV a XML
                        MenuExportacion("CSV");
                        break;

                    case "4":
                        //Abrimos el submenú de exportación para convertir de XML a CSV
                        MenuExportacion("XML");
                        break;

                    case "5":
                        salir = true; //Cuando pulsemos esta opcion se cierra el bucle y acaba el programa
                        Console.WriteLine("Saliendo del programa...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo");
                        break;
                }
            }
        }

        //METODO AUXILIAR PARA PEDIR Y ACTUALIZAR RUTAS (Opciones 1 y 2 del Main)
        static string PedirRuta(string tipoArchivo, string rutaActual)
        {
            //El usuario introduce por consola la ruta del archivo que desea actualizar
            Console.Write($"\nIngrese la ruta del archivo {tipoArchivo} (Actual: {rutaActual}): ");
            string nuevaRuta = Console.ReadLine()!.Trim();

            if (!string.IsNullOrEmpty(nuevaRuta))
            {
                Console.WriteLine($"--> Ruta {tipoArchivo} actualizada correctamente");
                return nuevaRuta;
            }

            //Si el usuario pulsa Enter sin escribir nada, se mantiene la ruta que ya tenia.
            Console.WriteLine($"--> Se mantiene la ruta actual de {tipoArchivo}");
            return rutaActual;
        }

        //SUBMENU PARA ELEGIR EL TIPO DE EXPORTACIÓN (Opciones 3 y 4 del main)
        static void MenuExportacion(string origenFormato)
        {
            //Identifico cúal es el archivo de entrada (Origen) y cual es el archivo de salida (Destino)
            string rutaOrigen = (origenFormato == "CSV") ? pathCsv : pathXml;
            string rutaDestino = (origenFormato == "CSV") ? pathXml : pathCsv;

            //El usuario comprueba si el archivo origen existe antes de seguir  
            if (!File.Exists(rutaOrigen))
            {
                Console.WriteLine($"\n[ERROR] No se encuentra el archivo de origen: {rutaOrigen}");
                return;
            }

            bool salir = false; //Controla el bucle del menu: si el usuario le da a la opcion salir cambia a True
            while (!salir) //Mientras no le dé a salir, seguirá en el menú
            {
                Console.WriteLine("=====================================");
                Console.WriteLine($"     EXPORTAR {origenFormato} a {((origenFormato == "CSV") ? "XML" : "CSV")}");
                Console.WriteLine("====================================");
                //Muestro la 4 opciones de exportación
                Console.WriteLine($"1. Exportar todo el documento:");
                Console.WriteLine($"2. Exportar por género");
                Console.WriteLine($"3. Exportar por año");
                Console.WriteLine($"4. Exportar por lenguaje");
                Console.WriteLine($"5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine()!.Trim();

                switch (opcion)
                {
                    case "1":
                        ExportarTodoElDocumento(origenFormato, rutaOrigen, rutaDestino);
                        break;

                    case "2":
                        ExportarPorGenero(origenFormato, rutaOrigen, rutaDestino);
                        break;

                    case "3":
                        ExportarPorAno(origenFormato, rutaOrigen, rutaDestino);
                        break;

                    case "4":
                        ExportarPorIdioma(origenFormato, rutaOrigen, rutaDestino);
                        break;

                    case "5":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo");
                        break;
                }
            }

        }

        // Método auxiliar para dividir la línea del CSV considerando comillas dobles
        static List<string> ParseCsvLine(string line)
        {
            List<string> resultado = new List<string>();
            // Expresión regular que separa por comas ignorando las comas entre comillas
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@",(?=(?:[^""]*""[^""]*"")*[^""]*$)");

            foreach (string campo in regex.Split(line))
            {
                resultado.Add(campo.Trim('"', ' '));
            }
            return resultado;
        }

        static void ExportarTodoElDocumento(string origenFormato, string rutaOrigen, string rutaDestino)
        {
            // EXPORTACIÓN DE CSV A XML TODO EL DOCUMENTO
            if (origenFormato == "CSV")
            {
                try
                {
                    // Leemos todas las líneas del CSV
                    string[] lineas = File.ReadAllLines(rutaOrigen);

                    // Si el archivo está vacío o solo contiene la línea de cabeceras, cancelamos la exportación
                    if (lineas.Length <= 1)
                    {
                        Console.WriteLine("\n[AVISO] El archivo CSV está vacío o no contiene datos para exportar.");
                        return;
                    }

                    // Creamos el archivo XML utilizando XmlWriter
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true; // como generar el xml formateado (https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlwritersettings.indent?view=net-10.0)

                    int contTotalPeliculas = 0;

                    using (XmlWriter writer = XmlWriter.Create(rutaDestino, settings))
                    {

                        writer.WriteStartDocument();            // Escribe la cabecera: xml version...
                        writer.WriteStartElement("Peliculas"); // Abre el elemento raíz principal: <Peliculas>

                        // Empezamos en i = 1 para omitir la fila de cabeceras (Title, Genre, etc.)
                        for (int i = 1; i < lineas.Length; i++)
                        {
                            // Parsea la línea respetando posibles comas dentro de comillas
                            List<string> campos = ParseCsvLine(lineas[i]);

                            // Verificamos que la línea tenga al menos las 6 columnas del dataset
                            if (campos.Count < 6) continue;

                            string title = campos[0];
                            string genre = campos[1];
                            string premiere = campos[2];
                            string runtime = campos[3];
                            string imdbScore = campos[4];
                            string language = campos[5];

                            // ESTRUCTURA DE CADA ELEMENTO EN EL XML 
                            writer.WriteStartElement("Pelicula"); // Nodo <Pelicula>

                            writer.WriteStartElement("Title");
                            writer.WriteString(title);
                            writer.WriteEndElement();

                            writer.WriteStartElement("Genre");
                            writer.WriteString(genre);
                            writer.WriteEndElement();

                            writer.WriteStartElement("Premiere");
                            writer.WriteString(premiere);
                            writer.WriteEndElement();

                            writer.WriteStartElement("Runtime");
                            writer.WriteString(runtime);
                            writer.WriteEndElement();

                            writer.WriteStartElement("IMDB_Score");
                            writer.WriteString(imdbScore);
                            writer.WriteEndElement();

                            writer.WriteStartElement("Language");
                            writer.WriteString(language);
                            writer.WriteEndElement();

                            writer.WriteEndElement(); // Cierra el Nodo </Pelicula>
                            contTotalPeliculas++;
                        }

                        writer.WriteEndElement(); // Cierra el elemento raíz principal: </Peliculas>
                        writer.WriteEndDocument(); // Cierra el documento XML
                    } // No hace falta Writer.Close(); el bloque using llama a Dispose() automáticamente al terminar y cierra el archivo.

                    Console.WriteLine($"\n--> [ÉXITO] Archivo XML generado en: '{rutaDestino}' (Total peliculas: {contTotalPeliculas})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a XML: {ex.Message}");
                }
            }
            else
            {
                // EXPORTACIÓN DE XML A CSV TODO EL DOCUMENTO
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(rutaOrigen);

                    //Obtengo la lista de todas las etiquetas <Pelicula>
                    XmlNodeList listaPeliculas = doc.GetElementsByTagName("Pelicula");

                    if (listaPeliculas.Count == 0)
                    {
                        Console.WriteLine("\n[AVISO] El archivo XML no contiene elementos '<Pelicula>' para exportar.");
                        return;
                    }

                    int contTotalPeliculas = 0;

                    //Uso el StrimWriter para escribir el fichero CSV
                    using (StreamWriter sw = new StreamWriter(rutaDestino))
                    {
                        //Coloco la columna original del CSV
                        sw.WriteLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");


                        foreach (XmlNode pelicula in listaPeliculas)
                        {
                            string title = pelicula["Title"]?.InnerText ?? "";
                            string genre = pelicula["Genre"]?.InnerText ?? "";
                            string premiere = pelicula["Premiere"]?.InnerText ?? "";
                            string runtime = pelicula["Runtime"]?.InnerText ?? "";
                            string imdbScore = pelicula["IMDB_Score"]?.InnerText ?? "";
                            string language = pelicula["Language"]?.InnerText ?? "";

                            // Formateamos el title y el premiere entre comillas porque hay en algunos casos que continen comas
                            string titleFormateado = title.Contains(",") ? $"\"{title}\"" : title;
                            string premiereFormateado = premiere.Contains(",") ? $"\"{premiere}\"" : premiere;

                            // Escribimos la línea separada por comas en el archivo CSV
                            sw.WriteLine($"{titleFormateado},{genre},{premiereFormateado},{runtime},{imdbScore},{language}");
                            contTotalPeliculas++;
                        }
                    }
                    Console.WriteLine($"\n--> [ÉXITO] Archivo CSV generado en: '{rutaDestino}' (Total peliculas: {contTotalPeliculas})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a CSV: {ex.Message}");
                }
            }
        }

        //Lista de generos sin duplicados
        static List<string> ObtenerGenerosUnicos(string origenFormato, string rutaOrigen)
        {
            List<string> generos = new List<string>();
            try
            {
                if (origenFormato == "CSV")
                {
                    string[] lineas = File.ReadAllLines(rutaOrigen);
                    for (int i = 1; i < lineas.Length; i++)
                    {
                        // Parsea la línea respetando posibles comas dentro de comillas
                        List<string> campos = ParseCsvLine(lineas[i]);

                        if (campos.Count >= 2)
                        {
                            string genero = campos[1].Trim();
                            if (!string.IsNullOrEmpty(genero) && !generos.Contains(genero))
                            {
                                generos.Add(genero);
                            }
                        }
                    }
                }
                else //XML
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(rutaOrigen);
                    XmlNodeList listaPeliculas = xmlDoc.GetElementsByTagName("Pelicula");

                    foreach (XmlNode pelicula in listaPeliculas)
                    {
                        string genero = pelicula["Genre"]?.InnerText?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(genero) && !generos.Contains(genero))
                        {
                            generos.Add(genero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Ocurrió un error al obtener los géneros: {ex.Message}");
            }

            generos.Sort(); //Ordeno la lista de generos
            return generos;

        }

        static void ExportarPorGenero(string origenFormato, string rutaOrigen, string rutaDestino)
        {
            List<string> generos = ObtenerGenerosUnicos(origenFormato, rutaOrigen);

            if (generos.Count == 0)
            {
                Console.WriteLine("\n[AVISO] No se encontraron generos para exportar. ");
                return;
            }

            //aparece un menu si ha encontrado generos
            Console.WriteLine("\n=== SELECCIONE EL GÉNERO A EXPORTAR ===");
            for (int i = 0; i < generos.Count; i++)
            {
                Console.WriteLine($"{i + 1} {generos[i]}");
            }

            int opcion = 0;
            bool numeroValido = false;

            //Hago un bucle para comprobar que elige el numero correcto
            while (!numeroValido)
            {
                Console.Write("Seleccione un número de la lista: ");
                string entrada = Console.ReadLine()!.Trim();

                if (int.TryParse(entrada, out opcion))
                {
                    if (opcion >= 1 && opcion <= generos.Count)
                    {
                        numeroValido = true;
                    }
                    else
                    {
                        Console.WriteLine($"\n[ERROR] Debe elegir un número entre 1 y {generos.Count}. Intente de nuevo.");
                    }
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Entrada no válida. Debe escribir un numero entero");
                }
            }
            string generoElegido = generos[opcion - 1];

            // EXPORTACIÓN DE CSV A XML POR GENERO
            if (origenFormato == "CSV")
            {
                try
                {
                    string[] lineas = File.ReadAllLines(rutaOrigen);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true; // como generar el xml formateado (https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlwritersettings.indent?view=net-10.0)

                    int contTotalPeliculas = 0;

                    using (XmlWriter xmlWriter = XmlWriter.Create(rutaDestino, settings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Peliculas");

                        for (int i = 1; i < lineas.Length; i++)
                        {
                            // Parsea la línea respetando posibles comas dentro de comillas
                            List<string> campos = ParseCsvLine(lineas[i]);
                            if (campos.Count < 6) continue;

                            if (campos[1].Trim().ToLower() == generoElegido.ToLower())
                            {
                                xmlWriter.WriteStartElement("Pelicula");

                                xmlWriter.WriteStartElement("Title");
                                xmlWriter.WriteString(campos[0]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Genre");
                                xmlWriter.WriteString(campos[1]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Premiere");
                                xmlWriter.WriteString(campos[2]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Runtime");
                                xmlWriter.WriteString(campos[3]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("IMDB_Score");
                                xmlWriter.WriteString(campos[4]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Language");
                                xmlWriter.WriteString(campos[5]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteEndElement(); // Cierra </Pelicula>
                                contTotalPeliculas++;
                            }
                        }

                        xmlWriter.WriteEndElement(); //Cierra </Peliculas> 
                        xmlWriter.WriteEndDocument();
                    }

                    Console.WriteLine($"\n--> [ÉXITO] Se han exportado {contTotalPeliculas} películas del género '{generoElegido}' a '{rutaDestino}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a XML: {ex.Message}");
                }
            }
            else
            {
                // EXPORTACIÓN DE XML A CSV POR GENERO
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(rutaOrigen); // Carga del XML con XmlDocument

                    XmlNodeList listaPeliculas = xmlDoc.GetElementsByTagName("Pelicula");

                    using (StreamWriter sw = new StreamWriter(rutaDestino))
                    {
                        sw.WriteLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");
                        int contTotalPeliculas = 0;

                        foreach (XmlNode pelicula in listaPeliculas)
                        {
                            string genre = pelicula["Genre"]?.InnerText?.Trim() ?? "";
                            if (genre.ToLower() == generoElegido.ToLower())
                            {
                                string title = pelicula["Title"]?.InnerText ?? "";
                                string premiere = pelicula["Premiere"]?.InnerText ?? "";
                                string runtime = pelicula["Runtime"]?.InnerText ?? "";
                                string imdbScore = pelicula["IMDB_Score"]?.InnerText ?? "";
                                string language = pelicula["Language"]?.InnerText ?? "";

                                string titleFormateado = title.Contains(",") ? $"\"{title}\"" : title;
                                string premiereFormateado = premiere.Contains(",") ? $"\"{premiere}\"" : premiere;

                                sw.WriteLine($"{titleFormateado},{genre},{premiereFormateado},{runtime},{imdbScore},{language}");
                                contTotalPeliculas++;
                            }
                        }

                        Console.WriteLine($"\n--> [ÉXITO] Se han exportado {contTotalPeliculas} películas del género '{generoElegido}' a '{rutaDestino}'.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a CSV: {ex.Message}");
                }
            }
        }

        // Método auxiliar para extraer el año de una fecha ("August 5, 2019" --> 2019)
        static int ExtraerAno(string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
            {
                return 0;
            }

            //Parto la fecha por los espacios 
            string[] partes = fecha.Trim().Split(' ');

            // Obtenemos la última palabra (donde está el año)
            string ultimoElemento = partes[partes.Length - 1];

            try
            {
                int ano = int.Parse(ultimoElemento);
                return ano;

            }
            catch
            {
                return 0;
            }
        }

        static void ExportarPorAno(string origenFormato, string rutaOrigen, string rutaDestino)
        {
            int anoLimite = 0;
            bool anoValido = false;
            int anoActual = DateTime.Now.Year;

            //Pido el año y lo recorro dentro de un bucle while
            while (!anoValido)
            {
                Console.Write("\nIngrese el año a partir del cual desea exportar: ");
                string entrada = Console.ReadLine()!.Trim();


                try
                {
                    anoLimite = int.Parse(entrada);

                    if (anoLimite > 1900 && anoLimite <= anoActual)
                    {
                        anoValido = true;
                    }
                    else
                    {
                        Console.WriteLine($"\n[ERROR] Debe escribir un año entre 1900 y {anoActual}. Intente de nuevo.");
                    }
                }
                catch
                {
                    Console.WriteLine("\n[ERROR] Entrada no válida. Debe escribir un número entero.");
                }
            }

            // --- EXPORTACIÓN DE CSV A XML POR AÑO ---
            if (origenFormato == "CSV")
            {
                try
                {
                    string[] lineas = File.ReadAllLines(rutaOrigen);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;

                    int totalPeliculasExportadas = 0;

                    using (XmlWriter xmlWriter = XmlWriter.Create(rutaDestino, settings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Peliculas");


                        for (int i = 1; i < lineas.Length; i++)
                        {
                            // Parsea la línea respetando posibles comas dentro de comillas
                            List<string> campos = ParseCsvLine(lineas[i]);

                            if (campos.Count < 6) continue;

                            string fecha = campos[2];
                            int anoPelicula = ExtraerAno(fecha);

                            // Comprobamos si el año de la película es mayor o igual al año mínimo requerido 
                            if (anoPelicula >= anoLimite)
                            {
                                xmlWriter.WriteStartElement("Pelicula");

                                xmlWriter.WriteStartElement("Title");
                                xmlWriter.WriteString(campos[0]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Genre");
                                xmlWriter.WriteString(campos[1]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Premiere");
                                xmlWriter.WriteString(campos[2]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Runtime");
                                xmlWriter.WriteString(campos[3]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("IMDB_Score");
                                xmlWriter.WriteString(campos[4]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Language");
                                xmlWriter.WriteString(campos[5]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteEndElement();
                                totalPeliculasExportadas++;
                            }
                        }
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                    }

                    Console.WriteLine($"\n--> [ÉXITO] Se han exportado {totalPeliculasExportadas} películas estrenadas en {anoLimite} o posteriormente a '{rutaDestino}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a XML: {ex.Message}");
                }
            }
            else
            {
                // EXPORTACIÓN DE XML A CSV POR AÑO 
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(rutaOrigen); // Carga del XML con XmlDocument

                    XmlNodeList listaNodosPelicula = xmlDoc.GetElementsByTagName("Pelicula");

                    int totalPeliculasExportadas = 0;

                    using (StreamWriter sw = new StreamWriter(rutaDestino))
                    {
                        sw.WriteLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");


                        foreach (XmlNode nodoPelicula in listaNodosPelicula)
                        {
                            string fecha = nodoPelicula["Premiere"]?.InnerText ?? ""; //Extraemos la fecha
                            int anoPelicula = ExtraerAno(fecha);

                            if (anoPelicula >= anoLimite)
                            {
                                string title = nodoPelicula["Title"]?.InnerText ?? "";
                                string genre = nodoPelicula["Genre"]?.InnerText ?? "";
                                string runtime = nodoPelicula["Runtime"]?.InnerText ?? "";
                                string imdbScore = nodoPelicula["IMDB_Score"]?.InnerText ?? "";
                                string language = nodoPelicula["Language"]?.InnerText ?? "";

                                string titleFormateado = title.Contains(",") ? $"\"{title}\"" : title;
                                string fechaFormateada = fecha.Contains(",") ? $"\"{fecha}\"" : fecha;

                                sw.WriteLine($"{titleFormateado},{genre},{fechaFormateada},{runtime},{imdbScore},{language}");
                                totalPeliculasExportadas++;
                            }
                        }
                    }

                    Console.WriteLine($"\n--> [ÉXITO] Se han exportado {totalPeliculasExportadas} películas estrenadas en {anoLimite} o posteriormente a '{rutaDestino}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a CSV: {ex.Message}");
                }
            }
        }

        //Lista de idiomas sin duplicados ordenados alfabeticamente
        static List<string> ObtenerIdiomasUnicos(string origenFormato, string rutaOrigen)
        {
            List<string> idiomas = new List<string>();
            try
            {
                if (origenFormato == "CSV")
                {
                    string[] lineas = File.ReadAllLines(rutaOrigen);
                    for (int i = 1; i < lineas.Length; i++)
                    {
                        // Parsea la línea respetando posibles comas dentro de comillas
                        List<string> campos = ParseCsvLine(lineas[i]);

                        if (campos.Count >= 6)
                        {
                            string idioma = campos[5].Trim();
                            if (!string.IsNullOrEmpty(idioma) && !idiomas.Contains(idioma))
                            {
                                idiomas.Add(idioma);
                            }
                        }
                    }
                }
                else //XML
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(rutaOrigen);
                    XmlNodeList listaPeliculas = xmlDoc.GetElementsByTagName("Pelicula");

                    foreach (XmlNode pelicula in listaPeliculas)
                    {
                        string idioma = pelicula["Language"]?.InnerText?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(idioma) && !idiomas.Contains(idioma))
                        {
                            idiomas.Add(idioma);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Ocurrió un fallo al obtener los idiomas: {ex.Message}");
            }

            idiomas.Sort(); //ordeno la lista de idiomas alfabeticamente
            return idiomas;
        }

        static void ExportarPorIdioma(string origenFormato, string rutaOrigen, string rutaDestino)
        {
            List<string> idiomas = ObtenerIdiomasUnicos(origenFormato, rutaOrigen);

            if (idiomas.Count == 0)
            {
                Console.WriteLine("\n[AVISO] No se encontraron idiomas para exportar");
                return;
            }

            //Muestro el menú con los idiomas encontrados
            Console.WriteLine("\n=== SELECCIONE EL IDIOMA A EXPORTAR ===");
            for (int i = 0; i < idiomas.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {idiomas[i]}");
            }

            int opcion = 0;
            bool numeroValido = false;

            //Hago un bucle while para que elija la opcion correcta y si se equivocara para que no se salga de ese menu (hasta que ponga un numero valido)
            while (!numeroValido)
            {
                Console.Write("Seleccione un numero de la lista: ");
                string entrada = Console.ReadLine()!.Trim();

                if (int.TryParse(entrada, out opcion))
                {
                    if (opcion >= 1 && opcion <= idiomas.Count)
                    {
                        numeroValido = true;
                    }
                    else
                    {
                        Console.WriteLine($"\n[ERROR] Debe elegir un número entre 1 y {idiomas.Count}. Intente de nuevo.");
                    }
                }
                else
                {
                    Console.WriteLine($"\n[ERROR] Entrada no válida. Debe escribir un numero entero.");
                }
            }

            string idiomaElegido = idiomas[opcion - 1];

            // EXPORTACIÓN DE CSV A XML POR IDIOMA
            if (origenFormato == "CSV")
            {
                try
                {
                    string[] lineas = File.ReadAllLines(rutaOrigen);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true; // como generar el xml formateado (https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlwritersettings.indent?view=net-10.0)

                    int totalPeliculasExportadas = 0;

                    using (XmlWriter xmlWriter = XmlWriter.Create(rutaDestino, settings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Peliculas");


                        for (int i = 1; i < lineas.Length; i++)
                        {
                            // Parsea la línea respetando posibles comas dentro de comillas
                            List<string> campos = ParseCsvLine(lineas[i]);

                            if (campos.Count < 6) continue;

                            string idiomaPelicula = campos[5].Trim();

                            if (idiomaPelicula.ToLower() == idiomaElegido.ToLower())
                            {
                                xmlWriter.WriteStartElement("Pelicula");

                                xmlWriter.WriteStartElement("Title");
                                xmlWriter.WriteString(campos[0]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Genre");
                                xmlWriter.WriteString(campos[1]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Premiere");
                                xmlWriter.WriteString(campos[2]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Runtime");
                                xmlWriter.WriteString(campos[3]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("IMDB_Score");
                                xmlWriter.WriteString(campos[4]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Language");
                                xmlWriter.WriteString(campos[5]);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteEndElement(); // Cierra </Pelicula>
                                totalPeliculasExportadas++;
                            }
                        }

                        xmlWriter.WriteEndElement(); //Cierra </Peliculas>
                        xmlWriter.WriteEndDocument();
                    }

                    Console.WriteLine($"\n--> [ÉXITO] Se han exportado {totalPeliculasExportadas} películas en idioma '{idiomaElegido}' a '{rutaDestino}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a XML: {ex.Message}");
                }
            }
            else
            {
                // EXPORTACIÓN DE XML A CSV POR IDIOMA 
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(rutaOrigen); //Carga con XmlDocument

                    XmlNodeList listaNodosPelicula = xmlDoc.GetElementsByTagName("Pelicula");

                    int contPeliculasExportadas = 0;

                    using (StreamWriter sw = new StreamWriter(rutaDestino))
                    {
                        sw.WriteLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");
                        

                        foreach (XmlNode nodoPelicula in listaNodosPelicula)
                        {
                            string language = nodoPelicula["Language"]?.InnerText?.Trim() ?? "";

                            if (language.ToLower() == idiomaElegido.ToLower())
                            {
                                string title = nodoPelicula["Title"]?.InnerText ?? "";
                                string genre = nodoPelicula["Genre"]?.InnerText ?? "";
                                string premiere = nodoPelicula["Premiere"]?.InnerText ?? "";
                                string runtime = nodoPelicula["Runtime"]?.InnerText ?? "";
                                string imdbScore = nodoPelicula["IMDB_Score"]?.InnerText ?? "";

                                string titleFormateado = title.Contains(",") ? $"\"{title}\"" : title;
                                string premiereFormateado = premiere.Contains(",") ? $"\"{premiere}\"" : premiere;

                                sw.WriteLine($"{titleFormateado},{genre},{premiereFormateado},{runtime},{imdbScore},{language}");
                                contPeliculasExportadas++;
                            }
                        }
                    }
                    
                    Console.WriteLine($"\n--> [EXITO] Se han exportado {contPeliculasExportadas} películas en idioma '{idiomaElegido}' a '{rutaDestino}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] Ocurrió un fallo al exportar a CSV: {ex.Message}");
                }
            }
        }
    }
}


