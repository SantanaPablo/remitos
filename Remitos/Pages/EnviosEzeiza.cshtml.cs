using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



public class EnviosEzeizaModel : PageModel
{
    [BindProperty]
    public string TextoBusqueda { get; set; }

    public List<ResultadoBusqueda> Resultados { get; set; } = new List<ResultadoBusqueda>();
    public bool BusquedaRealizada { get; set; }
    public string MensajeError { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        BusquedaRealizada = true;
        Resultados.Clear();

        try
        {
            // Construir rutas
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BuscadorExcel");
            var directorioArchivos = Path.Combine(wwwroot, "archivos");

            var pythonPath = Path.Combine(wwwroot, "venv", "Scripts", "python.exe");
            var scriptPath = Path.Combine(wwwroot, "buscador.py");

            // Validaciones previas
            if (!System.IO.File.Exists(pythonPath))
            {
                MensajeError = $"No se encontró Python en: {pythonPath}";
                return Page();
            }
            if (!System.IO.File.Exists(scriptPath))
            {
                MensajeError = $"No se encontró el script en: {scriptPath}";
                return Page();
            }

            // Configurar el proceso
            var psi = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\" \"{TextoBusqueda}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = wwwroot
            };

            using var process = Process.Start(psi);

            string output = await process.StandardOutput.ReadToEndAsync();  // JSON limpio
            string error = await process.StandardError.ReadToEndAsync();    // Logs del script

            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(error))
            {
                // No es un error fatal, puede ser útil
                MensajeError = "Mensajes del script:\n" + error;
            }

            if (string.IsNullOrWhiteSpace(output) || output.TrimStart().StartsWith("No se", StringComparison.OrdinalIgnoreCase))
            {
                Resultados = new List<ResultadoBusqueda>();
            }
            else
            {
                Resultados = JsonSerializer.Deserialize<List<ResultadoBusqueda>>(output, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ResultadoBusqueda>();
            }
        }
        catch (Exception ex)
        {
            MensajeError = $"Error inesperado: {ex.Message}";
        }

        return Page();
    }


}

