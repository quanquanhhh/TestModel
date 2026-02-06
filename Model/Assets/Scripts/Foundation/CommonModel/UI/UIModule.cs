using System;

namespace Foundation.CommonModel.UI
{
    public class UIModule: Singleton<UIModule>
    {
        
        public const int LAYOUT_ORDER = 1000;
        public const int WINDOW_ORDER = 10;
        
        
        private readonly Dictionary<Type, UIWindow> _windows = new Dictionary<Type, UIWindow>(16);
        private readonly Dictionary<WindowLayer, int> _layers = new Dictionary<WindowLayer, int>(4);
        
        
        private readonly Dictionary<Type, WindowAttribute> _windowAttributes = new Dictionary<Type, WindowAttribute>(16);
        private Transform _uiRoot;
        
        public void OnInit()
        {
            _uiRoot = GameUtility.UIRoot;
        }

        public void OnDestroy()
        {
        }
        
        
        public async UniTask<T> ShowAsync<T>(params object[] userDatas) where T : UIWindow, new()
        { 

            Type key = typeof(T);
            UIWindow existing;
            WindowAttribute attr = GetWindowAttribute(key);
            if (_windows.TryGetValue(key, out existing))
            {
                if (!existing.gameObject.activeSelf)
                {
                    existing.gameObject.SetActive(true);
                }

                existing.OnShow();
                return existing as T;
            }

            string assetName = attr != null && !string.IsNullOrEmpty(attr.AssetName) ? attr.AssetName : key.Name;
 
            GameObject panel = await AssetLoad.Instance.LoadGameobjectAsync(assetName,_uiRoot);
            panel.name = key.Name;
            
            
            if (panel == null)
            {
                return null;
            }
             
            T window = new T();
            window.OnInitialize(panel, null, userDatas);
            window.BindPanelComponent();
            window.BindComponents();
            window.SetSorttingDepth(GetSorttingByGroup(attr.Layer));
            window.RegisterEvent();
            window.OnCreate();
            window.OnShow();
            _windows[key] = window;
            return window;
        }
        
        public async void Close<T>(Action callback = null) where T : UIWindow
        {
            Type type = typeof(T);
            CloseUI(type, callback);
        }
        public async void CloseUI(Type type, Action callback = null) 
        {
            if (!_windows.ContainsKey(type))
            {
                return;
            }
            UIWindow window = _windows[type];
            await window.ShowCloseAnimation();
            window.OnDestroy();
            SubSorttingByGroup(_windowAttributes[type].Layer);
            _windowAttributes.Remove(type);
            _windows.Remove(type);
            callback?.Invoke();
        }

        public bool HasDialog()
        {
            return _layers[WindowLayer.Popup] != 0;
        }
        private int GetSorttingByGroup(WindowLayer layer)
        {
            if (_layers.ContainsKey(layer))
            {
                _layers[layer]++;
            }
            else
            {
                _layers.Add(layer, 1);
            }

            return (int)layer * LAYOUT_ORDER + _layers[layer] * WINDOW_ORDER;
        }
        
        private void SubSorttingByGroup(WindowLayer layer)
        {
            _layers[layer]--;
        }
        public void Hide<T>() where T : UIWindow
        {
            Type key = typeof(T);
            UIWindow window;
            if (_windows.TryGetValue(key, out window) && window != null && window.gameObject != null)
            {
                window.OnHide();
                if (window.gameObject.activeSelf)
                {
                    window.gameObject.SetActive(false);
                }
            }
        }

        public T Get<T>() where T : UIWindow
        {
            UIWindow window;
            if (_windows.TryGetValue(typeof(T), out window))
            {
                return window as T;
            }

            return null;
        }

        private WindowAttribute GetWindowAttribute(Type type)
        {
            WindowAttribute attr;
            if (_windowAttributes.TryGetValue(type, out attr))
            {
                return attr;
            }

            object[] attributes = type.GetCustomAttributes(typeof(WindowAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                attr = attributes[0] as WindowAttribute;
            }
            else
            {
                attr = null;
            }

            _windowAttributes[type] = attr;
            return attr;
        }
    }
}