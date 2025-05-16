using UnityEngine;
using UnityEngine.UIElements;

namespace Global
{
  /// <summary>
  /// Clase que contiene configuraciones globales y constantes para el juego.
  /// </summary>
  public static class Colors
  {
    public static readonly Color BLOCKED = new(0.2f, 0.2f, 0.2f);
    public static readonly Color RED = new(1.0f, 0.4f, 0.4f);
    public static readonly Color WHITE = new(1.0f, 1.0f, 1.0f);
    public static readonly Color BLUE = new(0.1f, 0.1f, 1.0f);
    public static readonly Color SOFT_GREEN = new(0.6f, 1.0f, 0.6f);
    public static readonly Color SOFT_WHITE = new(1.0f, 1.0f, 1.0f, 0.5f);
    public static readonly Color GREEN = new(0.1f, 1.0f, 0.1f);
    public static readonly Color YELLOW = new(1.0f, 1.0f, 0.1f);
  }

  /// <summary>
  /// Clase que contiene configuraciones globales relacionadas con configuraciones
  /// </summary>
  public static class Settings
  {
    public static SettingReference VISUAL_PATHFINDING = new(true);
    public static SettingReference COLORS = new();
    public static SettingReference STEPS = new();
  }

  public class SettingReference
  {
    public bool value;
    public SettingReference(bool value = false)
    {
      this.value = value;
    }
  }
}
