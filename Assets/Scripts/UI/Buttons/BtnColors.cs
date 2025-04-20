using Global;
using UI;

class BtnColors : BaseButton
{
  public override void InitialState()
  {
    setting = Settings.COLORS;
    SetChildrenBottonsState(setting.value);
  }
}