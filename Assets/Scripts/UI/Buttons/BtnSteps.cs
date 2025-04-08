using Global;
using UI;

class BtnSteps : BaseButton
{
  public override void InitialState()
  {
    setting = Settings.STEPS;
    SetPanelAndChildrenState(setting.value);
  }
}