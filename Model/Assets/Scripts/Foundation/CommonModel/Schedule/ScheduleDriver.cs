using UnityEngine;

namespace Foundation.CommonModel.Schedule
{
    public class ScheduleDriver : MonoBehaviour
    {        
        private ScheduleModule _schedule;

        internal void Bind(ScheduleModule schedule)
        {
            _schedule = schedule;
        }
        
        private void Update()
        {
            
            if (_schedule != null)
            {
                _schedule.InternalUpdate(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (_schedule != null)
            {
                _schedule.InternalDriverDestroyed(this);
            }
        }
        
    }
}