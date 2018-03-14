using UnityEngine;

namespace AI
{

    public class ConsiderationHandler : ScriptableObject
    {
        public Consideration[] considerations;

        #region Unity Callback
        private void Reset()
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            string[] pathSplit = path.Split('/');

            if(pathSplit.Length > 0)
            {
                name = pathSplit[pathSplit.Length - 1].Replace(".asset", string.Empty);
            }
            else
            {
                name = GetType().Name;
            }

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);

            if(!string.IsNullOrEmpty(assetPath))
            {
                Object[] objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);

                for (int i = 0; i < objs.Length; i++)
                {
                    if(null != objs[i])
                    {
                        DestroyImmediate(objs[i], true);
                    }
                }
            }
#endif
        }
        #endregion
    }
}
