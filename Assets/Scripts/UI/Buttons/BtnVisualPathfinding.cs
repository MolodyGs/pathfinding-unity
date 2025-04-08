using Global;
using UI;

class BtnVisualPathfinding : BaseButton
{
  public override void InitialState()
  {
    setting = Settings.VISUAL_PATHFINDING;
    SetPanelAndChildrenState(setting.value);
  }
}