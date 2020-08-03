using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFx.Async;
using UnityFx.Async.Extensions;
using UnityFx.Async.Promises;

namespace Management
{
    public class TransitionManager : MonoBehaviour
    {
        public Image shade;
        public GameObject loadingScreen;
        public static TransitionManager instance;
    
        public TransitionManager()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            //loading.AddRange(DataManager.LoadLevelObjects());
            loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.Menu, LoadSceneMode.Additive).ToAsync());
            StartCoroutine(GetLoadProgress(() => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int) SceneIndexes.Menu))));
        }

        public enum SceneIndexes
        {
            Manager = 0,
            Menu = 1,
            Editor = 2,
            Game = 3
        }
        // SCENE LOADING
        
        List<IAsyncOperation> loading = new List<IAsyncOperation>();

        public IEnumerator ToggleLoadingScreen(bool toggle, Action after)
        {
            shade.gameObject.LeanValue(value => {
                shade.color = value;
            }, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), 0.5f).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                loadingScreen.SetActive(toggle);
                // if (toggle)
                //     // Create tween for loading spinner
                //     loadingSinner.LeanRotateAroundLocal(Vector3.forward, -360, 4f).setLoopClamp();
                // else
                //     LeanTween.cancel(loadingSinner);
                shade.gameObject.LeanValue(value => {
                    shade.color = value;
                }, new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), 0.5f).setIgnoreTimeScale(true);
            });
            yield return new WaitForSeconds(1.5f);
            after();
        }
    
        public void LoadScene(SceneIndexes current, SceneIndexes target)
        {
            StartCoroutine(ToggleLoadingScreen(true, () =>
            {
                loading.Add(SceneManager.UnloadSceneAsync((int) current).ToAsync());
                loading.Add(SceneManager.LoadSceneAsync((int) target, LoadSceneMode.Additive).ToAsync());

                StartCoroutine(GetLoadProgress(() => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int) target))));
            }));
        }

        public void LoadLevel(int num)
        {
            StartCoroutine(ToggleLoadingScreen(true, () =>
            {
                loading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).ToAsync());
                loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.Editor, LoadSceneMode.Additive).ToAsync());
                loading[loading.Count - 1].Then(() =>
                {
                    Task.Delay(1000);
                    GameManager.instance.Initialize(num);
                });

                StartCoroutine(GetLoadProgress(() => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int) SceneIndexes.Editor))));
            }));
        }
        
        public float totalLoadingProgress;
        public IEnumerator GetLoadProgress(Action after) {
            for(var i = 0; i<loading.Count; i++) {
                while (!loading[i].IsCompleted) {
                    totalLoadingProgress = 0;

                    foreach(var operation in loading) {
                        totalLoadingProgress += operation.Progress;
                    }

                    totalLoadingProgress /= loading.Count;

                    // Update progress

                    yield return null;
                }
            }
            loading.Clear();
            StartCoroutine(ToggleLoadingScreen(false, after));
        }
    }
}
