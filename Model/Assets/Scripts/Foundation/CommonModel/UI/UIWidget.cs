namespace Foundation.CommonModel.UI
{
    public class UIWidget : UIBase
    {
 
        public override void OnDestroy()
        {
            DisableUpdate();
            CleanAllSubScribeEvents();
            foreach (var child in Widgets)
            {
                child.OnDestroy();
            }
        }
    }
}