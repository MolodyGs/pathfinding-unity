using System;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

/// <summary> Lectura y manejo general de archivo JSON.</summary>
public static class Json
{

  /// <summary> Hace lectura de un archivo JSON y lo deserializa en un objeto.</summary>
  /// <param name="dir">La dirección parcial del archivo .json</param>
  /// <param name="fileName">Nombre del archivo JSON (por ejemplo: file.json)</param>
  /// <param name="Structure">Clase que se usará para deserializar el JSON.</param>
  /// <returns>Retorna el objeto deserializado o null si no se pudo deserializar.</returns>
  public async static Task<Structure> ReadJson<Structure>(string dir, string fileName)
  {
    string path = Path.Combine(dir, fileName);

    // Verificar si el archivo existe
    if (!File.Exists(path))
    {
      Debug.LogError("El archivo JSON no fue encontrado.");
      return default;
    }

    string json = await File.ReadAllTextAsync(path); 

    try
    {
      Structure data = JsonUtility.FromJson<Structure>(json);
      return data;
    }
    catch (Exception e)
    {
      Debug.LogError("Error al deserializar el JSON: " + e.Message);
      return default;
    }
  }
}