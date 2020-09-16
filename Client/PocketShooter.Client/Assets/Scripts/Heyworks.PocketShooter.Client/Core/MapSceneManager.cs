using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heyworks.PocketShooter.Core
{
    public class MapSceneManager : MonoBehaviour
    {
        [SerializeField]
        private string mainSceneName;

        [SerializeField]
        private List<MapSceneInfo> mapSceneNames;

        private List<string> currentLoadingScenes = new List<string>();
        private string requestedMapName;
        private MapSceneLoadingState mapSceneLoadingState;

        public MapSceneInfo CurrentMapSceneInfo { get; private set; }

        public bool IsMapLoaded
        {
            get
            {
                return CurrentMapSceneInfo != null;
            }
        }

        public event Action MapSceneLoaded;

        public void LoadMapScene(MapNames mapName)
        {
            MapSceneInfo mapSceneInfo = mapSceneNames.Find(_ => _.MapName == mapName);
            requestedMapName = mapSceneInfo.MapSceneName;
            mapSceneLoadingState = MapSceneLoadingState.Loading;

            if (!currentLoadingScenes.Contains(mapSceneInfo.MapSceneName))
            {
                if (CurrentMapSceneInfo != null)
                {
                    StartCoroutine(AsyncUnloadScene(CurrentMapSceneInfo.MapSceneName));
                }

                currentLoadingScenes.Add(mapSceneInfo.MapSceneName);
                StartCoroutine(LoadAsyncMapScene(mapSceneInfo));
            }
        }

        public void UnloadCurrentMapScene()
        {
            mapSceneLoadingState = MapSceneLoadingState.NotRequested;

            if (CurrentMapSceneInfo != null)
            {
                StartCoroutine(AsyncUnloadScene(CurrentMapSceneInfo.MapSceneName));
            }
        }

        public void TransferGameObjectToActiveScene(GameObject gameObject)
        {
            if (gameObject != null)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }

        private IEnumerator LoadAsyncMapScene(MapSceneInfo mapSceneInfo)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapSceneInfo.MapSceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            currentLoadingScenes.Remove(mapSceneInfo.MapSceneName);

            if (mapSceneLoadingState == MapSceneLoadingState.Loading &&
                requestedMapName == mapSceneInfo.MapSceneName)
            {
                mapSceneLoadingState = MapSceneLoadingState.Loaded;
                CurrentMapSceneInfo = mapSceneInfo;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(mapSceneInfo.MapSceneName));
                MapSceneLoaded?.Invoke();
            }
            else
            {
                StartCoroutine(AsyncUnloadScene(mapSceneInfo.MapSceneName));
            }
        }

        private IEnumerator AsyncUnloadScene(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (asyncLoad != null)
            {
                if (mapSceneLoadingState == MapSceneLoadingState.Loading ||
                    mapSceneLoadingState == MapSceneLoadingState.NotRequested)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainSceneName));
                    LightmapSettings.lightProbes = null;
                    CurrentMapSceneInfo = null;
                }
            }
        }

        private enum MapSceneLoadingState
        {
            None,
            NotRequested,
            Loading,
            Loaded,
        }

        [Serializable]
        public class MapSceneInfo
        {
            public MapNames MapName;
            public string MapSceneName;
        }
    }
}