using System;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Lectura y manejo general de archivo JSON.
/// </summary>
public static class JsonController
{

  /// <summary> Hace lectura de un archivo JSON y lo deserializa en un objeto.</summary>
  /// <param name="dir">La dirección parcial del archivo .json</param>
  /// <param name="fileName">Nombre del archivo JSON (por ejemplo: file.json)</param>
  /// <param name="Structure">Clase que se usará para deserializar el JSON.</param>
  /// <returns>Retorna el objeto deserializado o null si no se pudo deserializar.</returns>
  public async static Task<Structure> ReadJson<Structure>(string dir, string fileName)
  {
    Debug.Log("Leyendo archivo JSON...");
    string path = Path.Combine(dir, fileName);

    // Verificar si el archivo existe
    if (!File.Exists(path))
    {
      Debug.LogError("El archivo JSON no fue encontrado.");
      return default;
    }

    // Obtiene el contenido del archivo JSON
    string json = await File.ReadAllTextAsync(path);

    try
    {
      // Obtiene la información serializando el JSON con la clase variable "Structure"
      Structure data = JsonUtility.FromJson<Structure>(json);
      Debug.Log("Lectura exitosa!");
      return data;
    }
    catch (Exception e)
    {
      Debug.LogError("Error al deserializar el JSON: " + e.Message);
      return default;
    }
  }

  /// <summary> Genera una archivo JSON a partir de "data".</summary>
  /// <param name="dir">La dirección parcial del archivo .json</param>
  /// <param name="fileName">Nombre del archivo JSON (por ejemplo: file.json)</param>
  /// <param name="data">Datos a almacenar.</param>
  /// <param name="Structure">Clase que se usará para deserializar el JSON.</param>
  /// <returns>Retorna true o false dependiendo de si hubo o no un error al generar el archivo.</returns>
  public static async Task<bool> CreateJson<Structure>(string dir, string fileName, Structure data)
  {
    Debug.Log("Creando archivo JSON...");
    string path = Path.Combine(dir, fileName);
    string directory = Path.GetDirectoryName(path);

    // Verificar si el directorio existe
    if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

    // Guardar el archivo JSON
    string json = JsonUtility.ToJson(data, true);
    await File.WriteAllTextAsync(path, json);
    Debug.Log($"JSON creado en: {path}");
    return true;
  }
}