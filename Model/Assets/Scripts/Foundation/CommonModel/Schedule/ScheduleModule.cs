using UnityEngine;
using UnityEngine.Events;

namespace Foundation.CommonModel.Schedule
{
    public class ScheduleModule: Singleton<ScheduleModule>
    {
        private ScheduleDriver _scheduleDriver;
        
        private static UnityAction updateAction;
        private static UnityAction secondUpdateAction;
        
        
        private float _timer;
        
        public void OnInit()
        {
            if (_scheduleDriver != null)
            {
                return;
            }

            var obj = new GameObject("ScheduleDriver");
            obj.transform.parent = GameUtility.MGR; 
            
            _scheduleDriver = obj.AddComponent<ScheduleDriver>(); 
            obj.AddComponent<ScheduleProcess>();
            _scheduleDriver.Bind(this);
        }

        public void OnDestroy()
        {
            updateAction = null;
            secondUpdateAction = null;
            _timer = 0f;
            if (_scheduleDriver != null)
            {
                GameObject.Destroy(_scheduleDriver.gameObject);
                _scheduleDriver = null;
            }
        }
        

        public void RegisterUpdate(UnityAction handler)
        {
            updateAction += handler; 
        }
        public void UnregisterUpdate(UnityAction handler)
        {
            updateAction -= handler;
            secondUpdateAction -= handler;
        }
        public void RegisterUpdatePerSecond(UnityAction handler)
        {
            secondUpdateAction += handler;
        }

        public void UnregisterUpdatePerSecond(UnityAction handler)
        {
            secondUpdateAction -= handler;
        }
        internal void InternalUpdate(float deltaTime)
        {
          
            if (updateAction != null)
            {
                updateAction.Invoke();
            }
            _timer += deltaTime;
            if (_timer >= 1f)
            {
                _timer = 0;
                if (secondUpdateAction != null)
                {
                    secondUpdateAction.Invoke();
                } 
            }
        }
        internal void InternalDriverDestroyed(ScheduleDriver driver)
        {
            if (driver == _scheduleDriver)
            {
                _scheduleDriver = null; //加个保护
            }
        }
        
    }
}