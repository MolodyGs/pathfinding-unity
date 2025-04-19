using Global;
using UI;

class BtnSteps : BaseButton
{
  public override void InitialState()
  {
    setting = Settings.STEPS;
    SetChildrenBottonsState(setting.value);
  }
}