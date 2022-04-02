//--------------------------------------
//           .MTL Processor           
//  Copyright © 2016 Fire Hose Games  
//--------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FHGTools
{
    public static class MtlUtils
    {
        public static void Log(string str, params object[] args)
        {
            if (SavedMtlProcessorSettings.Instance.showLoggingInfo)
                Debug.LogFormat(str, args);
        }
        public static void Warn(string str, params object[] args)
        {
            if (SavedMtlProcessorSettings.Instance.showLoggingInfo)
                Debug.LogWarningFormat(str, args);
        }
        private static void GetModelComponents<T>(Transform obj, ref List<T> ret) where T : Component
        {
            if (obj == null)
                return;
            ret.AddRange(obj.GetComponents<T>());
            foreach(Transform child in obj)
                GetModelComponents<T>(child, ref ret);
        }
        public static T[] GetModelComponents<T>(this GameObject obj) where T : Component
        {
            // In earlier versions of Unity, GetComponentsInChildren doesn't appear to work consistently on models
            if (obj == null)
                return new T[0]{};
            List<T> ret = new List<T>();
            GetModelComponents(obj.transform, ref ret);
            return ret.ToArray();
        }
    }
}