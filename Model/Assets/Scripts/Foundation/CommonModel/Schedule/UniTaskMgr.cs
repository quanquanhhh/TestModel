using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foundation.CommonModel.Schedule
{
    public class UniTaskMgr : Singleton<UniTaskMgr>
    {
        
        private GameObject unitask;
        public void OnInit()
        {
            if (unitask != null)
            {
                return;
            }
            unitask = new GameObject();
            unitask.name = "UniTaskMgr";
            unitask.transform.parent = GameUtility.MGR;
        }

        public async UniTask WaitForSecond(float second)
        {
            await UniTask.Delay((int)(second * 1000), cancellationToken: unitask.GetCancellationTokenOnDestroy());
        }

        public async UniTask WaitForSecond(float second, Action callback)
        {
            await UniTask.Delay((int)(second * 1000), cancellationToken: unitask.GetCancellationTokenOnDestroy());
            callback?.Invoke();
        }

        public async UniTask Delay(int frame)
        {
            await UniTask.Delay(frame, cancellationToken: unitask.GetCancellationTokenOnDestroy());
        }

        public async UniTask Yield()
        {
            await UniTask.Yield(cancellationToken: unitask.GetCancellationTokenOnDestroy());
        }
        public void OnDestroy()
        {
            if (unitask != null)
            {
                GameObject.Destroy(unitask);
            }
        }
    }
}