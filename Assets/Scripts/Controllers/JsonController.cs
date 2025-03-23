using System;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

/// <summary> Lectura y manejo general de archivo JSON.</summary>
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

    string json = await File.ReadAllTextAsync(path);

    try
    {
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

  public static async Task<bool> CreateJson<Structure>(string dir, string fileName, Structure data)
  {
    Debug.Log("Creando archivo JSON...");
    string path = Path.Combine(dir, fileName);
    string json = JsonUtility.ToJson(data, true);

    // Asegurarse de que el directorio existe
    string directory = Path.GetDirectoryName(path);
    if (!Directory.Exists(directory))
    {
      Directory.CreateDirectory(directory);
    }

    // Guardar el archivo JSON
    await File.WriteAllTextAsync(path, json);

    Debug.Log($"JSON creado en: {path}");

    return true;
  }
}