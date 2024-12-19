// Programa principal de nivel superior
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

QuestPDF.Settings.License = LicenseType.Community;

Console.Write("Ingrese el nombre de la rutina de entrenamiento: ");
string nombreRutina = Console.ReadLine();

Console.Write("Ingrese la cantidad de días en la rutina: ");
int cantidadDias = int.Parse(Console.ReadLine());

var rutina = new RutinaEntrenamiento(nombreRutina, cantidadDias);

while (true)
{
    try
    {
        Console.Clear();
        Console.WriteLine("CREAR RUTINA DE ENTRENAMIENTO");
        Console.WriteLine("1. Agregar Día de Entrenamiento");
        Console.WriteLine("2. Mostrar Rutina");
        Console.WriteLine("3. Generar PDF");
        Console.WriteLine("4. Eliminar último día");
        Console.WriteLine("5. Eliminar último ejercicio");
        Console.WriteLine("6. Salir");
        Console.Write("\nSeleccione una opción: ");

        string opcion = Console.ReadLine()?.Trim();

        switch (opcion)
        {
            case "1":
                AgregarDia(rutina);
                break;
            case "2":
                rutina.MostrarRutina();
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
                break;
            case "3":
                rutina.GenerarPDF();
                Console.WriteLine("PDF generado exitosamente. Presione una tecla para continuar...");
                Console.ReadKey();
                break;
            case "4":
                rutina.EliminarUltimoDia();
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
                break;
            case "5":
                rutina.EliminarUltimoEjercicio();
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
                break;
            case "6":
                Console.WriteLine("Saliendo del programa. Presione una tecla para cerrar...");
                Console.ReadKey();
                return;
            default:
                Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                Console.ReadKey();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ha ocurrido un error: {ex.Message}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}


static void AgregarDia(RutinaEntrenamiento rutina)
{
    Console.Clear();
    var dia = new DiaEntrenamiento();

    Console.Write("Ingrese el título del día (ej: Día de Pierna): ");
    dia.Titulo = Console.ReadLine();

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"Día: {dia.Titulo}");
        Console.WriteLine("1. Agregar Ejercicio");
        Console.WriteLine("2. Finalizar Día");
        Console.Write("Seleccione una opción: ");

        if (Console.ReadLine() == "1")
        {
            AgregarEjercicio(dia);
        }
        else
        {
            rutina.AgregarDia(dia);
            return;
        }
    }
}

static void AgregarEjercicio(DiaEntrenamiento dia)
{
    Console.Write("\nNombre del ejercicio: ");
    var ejercicio = new Ejercicio { Nombre = Console.ReadLine() };

    int numSeries;
    while (true)
    {
        Console.Write("Número de series: ");
        if (int.TryParse(Console.ReadLine(), out numSeries) && numSeries > 0)
        {
            break;
        }
        Console.WriteLine("Por favor, ingrese un número válido de series.");
    }

    int repeticionesMin = 0, repeticionesMax = 0, rir = 0;
    MetodoIntensidad metodoIntensidad = MetodoIntensidad.Ninguno;
    List<int> seriesConMetodo = new List<int>();

    while (true)
    {
        Console.Write("Repeticiones mínimas: ");
        if (int.TryParse(Console.ReadLine(), out repeticionesMin) && repeticionesMin > 0)
        {
            break;
        }
        Console.WriteLine("Por favor, ingrese un número válido de repeticiones mínimas.");
    }

    while (true)
    {
        Console.Write("Repeticiones máximas: ");
        if (int.TryParse(Console.ReadLine(), out repeticionesMax) && repeticionesMax >= repeticionesMin)
        {
            break;
        }
        Console.WriteLine("Por favor, ingrese un número válido de repeticiones máximas.");
    }

    while (true)
    {
        Console.Write("RIR (0 para fallo muscular): ");
        if (int.TryParse(Console.ReadLine(), out rir) && rir >= 0 && rir <= 10)
        {
            break;
        }
        Console.WriteLine("Por favor, ingrese un valor de RIR válido (0-10).");
    }

    while (true)
    {
        Console.WriteLine("\nMétodo de Intensidad: ");
        Console.WriteLine("0. Ninguno\n1. DropSet\n2. RestPauses\n3. SuperSeries\n4. BiSeries\n5. TriSeries");
        Console.Write("Seleccione una opción (o presione Enter para ninguno): ");

        string input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
        {
            break;
        }
        else if (int.TryParse(input, out int intensidadOpcion) &&
            Enum.IsDefined(typeof(MetodoIntensidad), intensidadOpcion))
        {
            metodoIntensidad = (MetodoIntensidad)intensidadOpcion;

            // Permitir selección de múltiples series
            while (true)
            {
                Console.Write($"¿En qué serie(s) desea aplicar {metodoIntensidad}? \n" +
                              "Ingrese los números de serie separados por comas (ejemplo: 1,3,5) " +
                              $"(Rango: 1-{numSeries}, Enter para cancelar): ");

                string seriesInput = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(seriesInput))
                {
                    metodoIntensidad = MetodoIntensidad.Ninguno;
                    break;
                }

                try
                {
                    seriesConMetodo = seriesInput
                        .Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .Where(n => n > 0 && n <= numSeries)
                        .Distinct()
                        .ToList();

                    if (seriesConMetodo.Any())
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("No se seleccionaron series válidas. Intente nuevamente.");
                    }
                }
                catch
                {
                    Console.WriteLine("Entrada inválida. Por favor, use números separados por comas.");
                }
            }

            break;
        }
        else
        {
            Console.WriteLine("Opción inválida. Intente nuevamente.");
        }
    }

    for (int i = 0; i < numSeries; i++)
    {
        var serie = new Serie
        {
            Repeticiones = repeticionesMin,
            RepeticionesMax = repeticionesMax,
            RIR = rir,
            MetodoIntensidad = seriesConMetodo.Contains(i + 1) ? metodoIntensidad : MetodoIntensidad.Ninguno
        };
        ejercicio.Series.Add(serie);
    }
    dia.Ejercicios.Add(ejercicio);
}


public class RutinaEntrenamiento
{
    private readonly string nombre;
    private readonly List<DiaEntrenamiento> dias = new();

    public RutinaEntrenamiento(string nombre, int cantidadDias)
    {
        this.nombre = nombre;
        for (int i = 0; i < cantidadDias; i++)
        {
            dias.Add(new DiaEntrenamiento());
        }
    }

    public IReadOnlyList<DiaEntrenamiento> Dias => dias;

    public void MostrarRutina()
    {
        Console.WriteLine($"Rutina: {nombre}");
        foreach (var dia in dias)
        {
            Console.WriteLine($"\n{dia.Titulo}");
            foreach (var ejercicio in dia.Ejercicios)
            {
                Console.WriteLine($"Ejercicio: {ejercicio.Nombre}");
                foreach (var serie in ejercicio.Series)
                {
                    Console.WriteLine($"  Series: {ejercicio.Series.Count}, " +
                        $"Repeticiones: {serie.Repeticiones}-{serie.RepeticionesMax}, " +
                        $"RIR: {(serie.RIR == 0 ? "Fallo" : serie.RIR.ToString())}, " +
                        $"Método Intensidad: {serie.MetodoIntensidad}");
                }
            }
        }
    }

    public void AgregarDia(DiaEntrenamiento dia)
    {
        dias.Add(dia);
    }

    public void EliminarUltimoDia()
    {
        if (dias.Count > 0)
        {
            dias.RemoveAt(dias.Count - 1);
            Console.WriteLine("Último día eliminado.");
        }
        else
        {
            Console.WriteLine("No hay días en la rutina para eliminar.");
        }
    }

    public void EliminarUltimoEjercicio()
    {
        if (dias.Count > 0 && dias[dias.Count - 1].Ejercicios.Count > 0)
        {
            dias[dias.Count - 1].Ejercicios.RemoveAt(dias[dias.Count - 1].Ejercicios.Count - 1);
            Console.WriteLine("Último ejercicio eliminado.");
        }
        else
        {
            Console.WriteLine("No hay ejercicios en la rutina para eliminar.");
        }
    }

    public void GenerarPDF()
    {
        // Definir la ruta donde se guardarán las rutinas
        string rutinasPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Rutinas");

        // Crear el directorio si no existe
        if (!Directory.Exists(rutinasPath))
        {
            Directory.CreateDirectory(rutinasPath);
        }

        // Ruta completa del archivo PDF
        string pdfPath = Path.Combine(rutinasPath, $"{nombre}.pdf");

        var pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "prueba2.png");

        // Obtener los métodos de intensidad utilizados
        var metodosUtilizados = dias
            .SelectMany(d => d.Ejercicios)
            .SelectMany(e => e.Series)
            .Select(s => s.MetodoIntensidad)
            .Distinct()
            .Where(m => m != MetodoIntensidad.Ninguno)
            .ToList();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);

                page.Background().Image(pngPath).FitWidth();

                page.Header().AlignCenter().Text(x =>
                {
                    x.Line("Rutina de Entrenamiento").FontSize(24).SemiBold().FontFamily("Arial");
                    x.Line(nombre).FontSize(16).FontFamily("Arial");
                });

                page.Content().Column(column =>
                {
                    foreach (var dia in dias)
                    {
                        if (dia.Ejercicios.Any())
                        {
                            column.Item().Text(dia.Titulo).FontSize(16).Bold().FontFamily("Arial");

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5)
                                        .Text("Ejercicio").SemiBold().FontFamily("Arial");
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).AlignCenter()
                                        .Text("Series").SemiBold().FontFamily("Arial");
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).AlignCenter()
                                        .Text("Repeticiones").SemiBold().FontFamily("Arial");
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).AlignCenter()
                                        .Text("RIR").SemiBold().FontFamily("Arial");
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).AlignCenter()
                                        .Text("Método Intensidad").SemiBold().FontFamily("Arial");
                                });

                                foreach (var ejercicio in dia.Ejercicios)
                                {
                                    var seriesConMetodo = ejercicio.Series
                                        .Select((serie, index) => new { Serie = serie, Indice = index + 1 })
                                        .Where(x => x.Serie.MetodoIntensidad != MetodoIntensidad.Ninguno)
                                        .ToList();

                                    string metodoIntensidadTexto = seriesConMetodo.Count > 0
                                         ? $"{seriesConMetodo[0].Serie.MetodoIntensidad.GetDisplayName()} (Serie {string.Join(",", seriesConMetodo.Select(s => s.Indice))})"
                                         : "-";

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(3)
                                        .Text(ejercicio.Nombre.ToUpper()).FontFamily("Arial");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(3).AlignCenter()
                                        .Text(ejercicio.Series.Count.ToString()).FontFamily("Arial");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(3).AlignCenter()
                                        .Text($"{ejercicio.Series[0].Repeticiones}-{ejercicio.Series[0].RepeticionesMax}").FontFamily("Arial");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(3).AlignCenter()
                                        .Text(ejercicio.Series[0].RIR == 0 ? "Fallo" : ejercicio.Series[0].RIR.ToString()).FontFamily("Arial");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(3).AlignCenter()
                                        .Text(metodoIntensidadTexto).FontFamily("Arial");
                                }
                            });

                            column.Item().PaddingTop(16);
                        }
                    }

                    // Agregar sección de aclaraciones
                    if (metodosUtilizados.Any() || true) // Siempre mostrar para incluir RIR
                    {
                        column.Item().PaddingTop(16).Text("*Aclaraciones").FontSize(16).Bold().FontFamily("Arial");

                        column.Item().PaddingTop(8).Text(text =>
                        {
                            // Explicación del RIR
                            text.Line("RIR (Repeticiones en Reserva):").Bold().FontFamily("Arial");
                            text.Line("Es el número de repeticiones que podrías hacer pero no haces en cada serie. Por ejemplo, si tu RIR es 2, significa que terminas la serie pudiendo hacer 2 repeticiones más. Si el RIR es 0, significa que debes llegar al fallo muscular (no poder realizar ni una repetición más con técnica correcta).")
                                .FontFamily("Arial");
                        });

                        // Agregar explicaciones de métodos de intensidad utilizados
                        foreach (var metodo in metodosUtilizados)
                        {
                            column.Item().PaddingTop(8).Text(text =>
                            {
                                text.Line($"{metodo}:").Bold().FontFamily("Arial");
                                text.Line(ObtenerDescripcionMetodo(metodo)).FontFamily("Arial");
                            });
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Line("Profesor Nacional de Educación Física").FontFamily("Arial");
                    x.Line("Nicolás Agustín Ibarra").FontFamily("Arial");
                });
            });
        }).GeneratePdf(pdfPath);
    }

    private string ObtenerDescripcionMetodo(MetodoIntensidad metodo)
    {
        return metodo switch
        {
            MetodoIntensidad.DropSet => "Realizar la serie normal hasta el RIR indicado o fallo, inmediatamente reducir el peso (20-30%) y continuar hasta el fallo. Se puede realizar más de una reducción de peso.",
            MetodoIntensidad.RestPause => "Realizar la serie hasta el RIR indicado o fallo, descansar 15-20 segundos, continuar hasta el fallo nuevamente. Repetir 2-3 veces en total.",
            MetodoIntensidad.SuperSeries => "Realizar dos ejercicios diferentes para el mismo grupo muscular de forma consecutiva sin descanso entre ellos.",
            MetodoIntensidad.BiSeries => "Realizar dos ejercicios para grupos musculares diferentes de forma consecutiva sin descanso entre ellos.",
            MetodoIntensidad.TriSeries => "Realizar tres ejercicios de forma consecutiva sin descanso entre ellos.",
            _ => string.Empty
        };
    }
}

public enum MetodoIntensidad
{
    Ninguno,
    [Display(Name = "Drop Set")]
    DropSet,
    [Display(Name = "Rest Pause")]
    RestPause,
    [Display(Name = "Super Series")]
    SuperSeries,
    [Display(Name = "Bi Series")]
    BiSeries,
    [Display(Name = "Tri Series")]
    TriSeries
}

// Agregamos un método de extensión para obtener el nombre de visualización
public static class EnumExtensions
{
    public static string GetDisplayName(this MetodoIntensidad metodo)
    {
        var memberInfo = metodo.GetType().GetMember(metodo.ToString()).FirstOrDefault();
        if (memberInfo != null)
        {
            var attribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
                return attribute.Name;
        }
        return metodo.ToString();
    }
}

public class Serie
{
    public int Repeticiones { get; set; }
    public int RepeticionesMax { get; set; }
    public int RIR { get; set; }
    public MetodoIntensidad MetodoIntensidad { get; set; }
}

public class Ejercicio
{
    public string Nombre { get; set; }
    public List<Serie> Series { get; set; } = new();
}

public class DiaEntrenamiento
{
    public string Titulo { get; set; }
    public List<Ejercicio> Ejercicios { get; set; } = new();
}