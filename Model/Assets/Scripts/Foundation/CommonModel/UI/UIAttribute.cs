using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.CommonModel.UI
{
 public enum WindowLayer
    {
        SystemBottom = 1,
        System = 2,
        Popup = 3,
        Tips = 4
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class WindowAttribute : Attribute
    {
        public readonly WindowLayer Layer;
        public readonly string AssetName;
        public readonly bool CanRepeat = false;

        public WindowAttribute(string assetName,WindowLayer layer,  bool canRepeat = false)
        {
            Layer =  layer;
            AssetName = assetName;
            CanRepeat = canRepeat;
        }
    }
    
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class UIBinderAttribute : Attribute
    {
        public string mPath = string.Empty;
        public string mParent = string.Empty;

        public UIBinderAttribute(string path, string parentName = "")
        {
            mPath = path;
            mParent = parentName;
        }
    }
    
    
    
    public class ComponentBinder
    {
        public static void BindingComponent(object binder, Transform transform)
        {
            var allComponentDict = new Dictionary<string, List<Transform>>();

            CollectAllComponent(transform, allComponentDict);

            var fieldInfoDict = new Dictionary<string, FieldInfo>();

            GetAllFieldInfo(binder.GetType(), fieldInfoDict);

            Type typeAttr = typeof(UIBinderAttribute);

            foreach (var item in fieldInfoDict)
            {
                //ComponentBinderAttribute attr = Attribute.GetCustomAttribute(item.Value,typeAttr) as ComponentBinderAttribute;
                //ILRuntime的bug，只能这么取attribute
                var objAttr = item.Value.GetCustomAttributes(typeAttr, false);
                UIBinderAttribute attr = null;

                if (objAttr.Length > 0)
                {
                    for (var i = 0; i < objAttr.Length; i++)
                    {
                        if (objAttr[i] is UIBinderAttribute)
                        {
                            attr = objAttr[i] as UIBinderAttribute;
                            break;
                        }
                    }
                }

                if (attr != null)
                {
                    var type = item.Value.FieldType;
                    if (type == typeof(GameObject))
                    {
                        type = typeof(Transform);
                    }

                    var v = FindMatchComponent(transform, attr.mPath, attr.mParent, type, allComponentDict);

                    if (item.Value.FieldType == typeof(GameObject))
                    {
                        item.Value.SetValue(binder, v.gameObject);
                    }
                    else
                    {
                        item.Value.SetValue(binder, v);
                    }
                }
            }

            //实现Button函数绑定
            var listMethodInfo = binder.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in listMethodInfo)
            {
                var objAttr = method.GetCustomAttributes(typeAttr, false);
                if (objAttr.Length == 0) continue;
                var attribute = (UIBinderAttribute) objAttr[0];
                if (attribute == null)
                    continue;
                var paramInfos = method.GetParameters();
                if (paramInfos.Length != 0)
                    continue;
                var button = FindMatchComponent(transform, attribute.mPath, attribute.mParent, typeof(Button),
                    allComponentDict) as Button;
                if (button == null)
                    continue;
                button.onClick.AddListener(() => method.Invoke(binder, new object[] { }));
            }
        }

        public static Component FindMatchComponent(Transform transform, string path, string parentName, Type type,
            Dictionary<string, List<Transform>> componentDict)
        {
            Transform tran = transform.Find(path);
            if (tran != null && string.IsNullOrEmpty(parentName))
            {
                return tran.GetComponent(type);
            }

            if (!path.Contains("/"))
            {
                if (componentDict.TryGetValue(path, out var list))
                {
                    if (string.IsNullOrEmpty(parentName))
                    {
                        //找第一个
                        Component component = list[0].GetComponent(type);
                        if (component == null && path == transform.name)
                        {
                            component = transform.GetComponent(type);
                        }

                        return component;
                    }
                    else
                    {
                        //匹配父物体
                        foreach (var item in list)
                        {
                            if (item.parent != null && item.parent.name == parentName)
                            {
                                return item.GetComponent(type);
                            }
                        }
                    }
                }
            }

            return null;
        }


        public static void GetAllFieldInfo(Type type, Dictionary<string, FieldInfo> listInfo)
        {
            if (listInfo == null)
            {
                listInfo = new Dictionary<string, FieldInfo>();
            }

            var listFiledInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            int count = listFiledInfo.Length;

            for (int i = 0; i < count; i++)
            {
                listInfo[listFiledInfo[i].Name] = listFiledInfo[i];
            }

            if (type.BaseType != null)
            {
                GetAllFieldInfo(type.BaseType, listInfo);
            }
        }

        public static void CollectAllComponent(Transform transform,
            Dictionary<string, List<Transform>> allComponentDict)
        {
            var listAllComponent = transform.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (var item in listAllComponent)
            {
                List<Transform> list = null;

                if (!allComponentDict.TryGetValue(item.name, out list))
                {
                    list = new List<Transform>();
                    allComponentDict[item.name] = list;
                }

                list.Add(item);
            }
        }
}