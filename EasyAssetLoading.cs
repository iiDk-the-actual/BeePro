using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BeePro
{
    public static class EasyAssetLoading
    {
        public static AssetBundle LoadBundle(Assembly a, string assetDirectory)
        {
            return AssetBundle.LoadFromStream(a.GetManifestResourceStream(assetDirectory));
        }

        public static object LoadAssetToPrefab(Assembly a, string assetDirectory, string gameObjectName)
        {
            AssetBundle assetBundle = EasyAssetLoading.LoadBundle(a, assetDirectory);
            Object @object = assetBundle.LoadAsset(gameObjectName);
            assetBundle.Unload(false);
            return @object;
        }

        public static GameObject InstantiateAsset(Assembly a, string assetDirectory, string gameObjectName)
        {
            return Object.Instantiate<GameObject>((GameObject)EasyAssetLoading.LoadAssetToPrefab(a, assetDirectory, gameObjectName));
        }

        public static GameObject SetupAsset(Assembly a, string assetDirectory, string gameObjectName, Vector3 pos, Quaternion eulers, Transform parent)
        {
            GameObject gameObject = EasyAssetLoading.InstantiateAsset(a, assetDirectory, gameObjectName);
            gameObject.transform.localPosition = pos;
            gameObject.transform.rotation = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        public static GameObject SetupAsset(Assembly a, string assetDirectory, string gameObjectName, Vector3 pos, Quaternion eulers)
        {
            GameObject gameObject = EasyAssetLoading.InstantiateAsset(a, assetDirectory, gameObjectName);
            gameObject.transform.position = pos;
            gameObject.transform.rotation = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
            return gameObject;
        }

        public static GameObject SetupAsset(Assembly a, string assetDirectory, string gameObjectName, Transform parent, bool keepWorldPos)
        {
            GameObject gameObject = EasyAssetLoading.InstantiateAsset(a, assetDirectory, gameObjectName);
            gameObject.transform.SetParent(parent, keepWorldPos);
            return gameObject;
        }
    }
}
