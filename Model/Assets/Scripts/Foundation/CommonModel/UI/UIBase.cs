using System;
using System.Collections.Generic;
using Foundation.CommonModel.Schedule;
using UnityEngine;
using UnityEngine.Events;

namespace Foundation.CommonModel.UI
{
    public class UIBase
    {
                public GameObject gameObject;
        public RectTransform rectTransform;
        public UIBase parent;
        public object[] userDatas;
        public readonly List<UIWidget> Widgets = new List<UIWidget>(4);

        private bool _updateEnabled;
        private UnityAction _updateAction;
        protected List<EventDelegate> events;

        
        internal void OnInitialize(GameObject go, UIBase parentUI, object[] datas)
        {
            gameObject = go;
            rectTransform = go != null ? go.GetComponent<RectTransform>() : null;
            parent = parentUI;
            userDatas = datas;
        }
        public T AddWidget<T>(GameObject obj, bool visible = true, params object[] widgetdata) where T : UIWidget, new()
        {
            T widget = new T();
            widget.OnInitialize(obj, this, widgetdata); 
            widget.BindComponents();
            widget.RegisterEvent();
            widget.OnCreate();
            widget.OnShow();
            widget.gameObject.SetActive(visible);
            Widgets.Add(widget);
            return widget;
        }


        #region 监听
        public virtual void RegisterEvent()
        {
            
        }
        protected void SubScribeEvent<T>(Action<T> action) where T : IEvent
        {
            if (events == null)
            {
                events = new List<EventDelegate>();
            }

            var listener = Event.Instance.Subscribe<T>(action);
            if (listener != null)
            {
                events.Add(listener);
            }
        }

        protected void UnSubScribeEvent<T>(Action<T> action) where T : IEvent
        {
            if (events == null)
            {
                return;
            }

            foreach (var e in events)
            {
                if (e.eventDelegate == (Delegate)action)
                {
                    Event.Instance.UnSubscribe(e);
                }
            }
        }

        protected void CleanAllSubScribeEvents()
        {
            if (events == null)
            {
                return;
            }

            foreach (var e in events)
            {
                Event.Instance.UnSubscribe(e);
            }
            events.Clear();
        }
        #endregion

        
        public virtual void OnCreate()
        {
            
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public void EnableUpdate()
        {
            if (_updateEnabled)
            {
                return;
            }

            if (_updateAction == null)
            {
                _updateAction = OnUpdateInternal;
            }

            _updateEnabled = true;
            ScheduleModule.Instance.RegisterUpdatePerSecond(OnUpdate);
        }

        public void DisableUpdate()
        {
            if (!_updateEnabled)
            {
                return;
            }
            
            _updateEnabled = false;
            ScheduleModule.Instance.UnregisterUpdatePerSecond(OnUpdate);
        }

        public virtual void OnUpdate()
        {
        }

        private void OnUpdateInternal()
        {
            if (_updateEnabled)
            {
                OnUpdate();
            }
        }

 

        public void BindComponents()
        {
            ComponentBinder.BindingComponent(this, rectTransform);
        } 
    }
}