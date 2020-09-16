using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter
{
    public static class StaticBatchingHelper
    {
        [MenuItem("GameObject/Batch Selected...")]
        public static void OrganizeBatching()
        {
            GameObject root = Selection.activeGameObject;
            if (root == null)
            {
                return;
            }

            List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
            root.GetComponentsInChildren(meshRenderers);

            Dictionary<Material, List<MeshRenderer>> map = new Dictionary<Material, List<MeshRenderer>>();

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer.gameObject.activeInHierarchy && meshRenderer.enabled && meshRenderer.sharedMaterials.Length == 1)
                {
                    Material mat = meshRenderer.sharedMaterial;

                    if (mat == null)
                    {
                        Selection.activeGameObject = meshRenderer.gameObject;
                        EditorGUIUtility.PingObject(meshRenderer.gameObject);
                        return;
                    }

                    List<MeshRenderer> matList;
                    if (!map.TryGetValue(mat, out matList))
                    {
                        matList = new List<MeshRenderer>();
                        map.Add(mat, matList);
                    }

                    matList.Add(meshRenderer);
                }
            }

            // int order = 2000;
            foreach (var kvp in map)
            {
                var matRoot = new GameObject(kvp.Key.name);
                matRoot.transform.parent = root.transform;
                var gos = new List<GameObject>();
                foreach (MeshRenderer renderer in kvp.Value)
                {
                    gos.Add(renderer.gameObject);
                    renderer.transform.parent = matRoot.transform;
                }

                // kvp.Key.renderQueue = order++;
//                StaticBatchingUtility.Combine(gos.ToArray(), matRoot);
//
//                var g = new GameObject();
//                g.name = "batch";
//                g.AddComponent<MeshFilter>().sharedMesh = kvp.Value.First().GetComponent<MeshFilter>().sharedMesh;
//                g.AddComponent<MeshRenderer>().sharedMaterial = kvp.Value.First().sharedMaterial;
            }
        }

        [MenuItem("GameObject/Replace With Prefab...")]
        public static void ReplaceWithPrefab()
        {
            foreach (GameObject root in Selection.gameObjects)
            {
                GameObject prefab = FindPrefab(root.name);

                int index = root.transform.GetSiblingIndex();
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform.parent);
                instance.transform.SetSiblingIndex(index);
                instance.transform.SetPositionAndRotation(root.transform.position, root.transform.rotation);
                instance.transform.localScale = root.transform.localScale;
            }

            foreach (GameObject root in Selection.gameObjects)
            {
                Object.DestroyImmediate(root);
            }

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }

        public static GameObject FindPrefab(string name)
        {
            var prefabs = AssetUtility.GetAssetsAtPath<GameObject>(".prefab");
            foreach (GameObject prefab in prefabs)
            {
                if (name.Contains(prefab.name))
                {
                    return prefab;
                }
            }

            return null;
        }

        [MenuItem("GameObject/Group by names...")]
        public static void GroupGameObjects()
        {
            Dictionary<string, GameObject> groups = new Dictionary<string, GameObject>();

            foreach (GameObject obj in Selection.gameObjects)
            {
                string groupName = obj.name.Split(' ').First();

                if(!groups.TryGetValue(groupName, out GameObject group))
                {
                    group = new GameObject();
                    group.name = groupName + "_group";
                    group.transform.parent = obj.transform.parent;
                    group.transform.localPosition = Vector3.zero;

                    groups.Add(groupName, group);
                }

                obj.transform.parent = group.transform;
            }
        }

        [MenuItem("GameObject/Sort by names...")]
        public static void SortGameObjects()
        {
            var list = new List<GameObject>(Selection.gameObjects);
            list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

            int index = 0;
            foreach (GameObject gameObject in list)
            {
                gameObject.transform.SetSiblingIndex(index++);
            }
        }
    }
}