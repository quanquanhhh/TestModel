namespace Foundation.CommonModel.UI
{

    public class UIWindow : UIBase
    {
        private Canvas _canvas;
        private GraphicRaycaster _canvasGroup;
        
        private UniTaskCompletionSource _tsc;
        private UniTaskCompletionSource _destorytsc;
        
        internal void BindPanelComponent()
        {
            RectTransform rect = gameObject.TryGetOrAddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            _canvasGroup = gameObject.TryGetOrAddComponent<GraphicRaycaster>();
            _canvas = gameObject.TryGetOrAddComponent<Canvas>();
            _canvas.overrideSorting = true;
        }

        internal void SetSorttingDepth(int order)
        {
            _canvas.sortingOrder = order;
        }

        protected int GetSorttingDepth()
        {
            return _canvas.sortingOrder;
        }
 
        public override void OnDestroy()
        {
            DisableUpdate();
            CleanAllSubScribeEvents();
            foreach (var child in Widgets)
            {
                child.OnDestroy();
            }
            GameObject.Destroy(gameObject);
        }

        public virtual void Close()
        {
            if (_tsc != null)
            {
                _tsc.TrySetResult();
            }

            DisableUpdate();
            UIModule.Instance.CloseUI(this.GetType(), CloseWindowCallBack);
        }

        public virtual void CloseWindowCallBack()
        {
            if (_destorytsc != null)
            {
                _destorytsc.TrySetResult();
            }
        }

        #region 动画相关
        public virtual async UniTask ShowCloseAnimation()
        {
            
        }

        #endregion
        
        
        public UniTaskCompletionSource WaitPopClose()
        {
            _tsc = new UniTaskCompletionSource();
            return _tsc;
        }
        public UniTaskCompletionSource WaitPopDestory()
        {
            _destorytsc = new UniTaskCompletionSource();
            return _destorytsc;
        }
        
        
    }
}