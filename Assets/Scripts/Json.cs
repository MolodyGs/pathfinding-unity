using System;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Rendering;

/// <summary> Lectura y manejo general de archivo JSON.</summary>
public static class Json
{

  /// <summary> Hace lectura de un archivo JSON y lo deserializa en un objeto.</summary>
  /// <param name="dir">La dirección parcial del archivo .json</param>
  /// <param name="fileName">Nombre del archivo JSON (por ejemplo: file.json)</param>
  /// <param name="Type">Clase que se usará para deserializar el JSON.</param>
  /// <returns>Retorna el objeto deserializado o null si no se pudo deserializar.</returns>
  public static System.Object ReadJson(string dir, string fileName, System.Object Type)
  {
    string path = Path.Combine(dir, fileName);
    Type data;

    if (!File.Exists(path))
    {
      Debug.LogError("El archivo JSON no fue encontrado.");
      return null;
    }

    string json = File.ReadAllText(path);

    try
    {
      data = JsonUtility.FromJson<Type>(json);
    }
    catch (System.Exception e)
    {
      Debug.LogError("Error al deserializar el JSON: " + e.Message);
      return null;
    }

    return data;
  }
}