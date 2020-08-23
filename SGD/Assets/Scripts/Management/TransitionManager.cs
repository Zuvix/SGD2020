using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using TMPro;
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
        public TMP_Text loadText;
        public TMP_Text skipText;
        public static TransitionManager instance;

        [Header("Settings")] public float flashSpeed = 0.3f;
        
        private AsyncOperation _sceneHolder;
        private Tuple<int, int> _heldLevelIdentification;
        private bool _loadLock;
    
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

        private void Update()
        {
            if (Input.GetButton("Submit"))
            {
                Skip();
            }
        }

        public void Skip()
        {
            if (_sceneHolder != null && loading.Count == 0 && !_loadLock) 
            {
                _loadLock = true;
                var sr = loadingScreen.GetComponent<Image>();
                loadingScreen.LeanValue(value =>
                {
                    sr.color = value;
                }, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), 1f).setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    _sceneHolder.allowSceneActivation = true;
                });
                loading.Add(SceneManager.UnloadSceneAsync(DataManager.instance.screenPairs[_heldLevelIdentification.Item1].targetSceneBuildIndex).ToAsync());
                StartCoroutine(GetLoadProgress(() =>
                {
                    skipText.gameObject.SetActive(false);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_heldLevelIdentification.Item2));
                    _sceneHolder = null;
                    _heldLevelIdentification = null;
                }));
                Debug.Log("ree");
            }
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
                loadingScreen.GetComponent<Image>().color = Color.black;
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

                StartCoroutine(GetLoadProgress(() =>
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int) target))));
            }));
        }
        
        public void LoadScene(SceneIndexes target)
        {
            StartCoroutine(ToggleLoadingScreen(true, () =>
            {
                loading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex).ToAsync());
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
        
        public void LoadStoryLevel(int num, bool cinematic)
        {
            if (cinematic)
            {
                _loadLock = false;
                var loadScreen = DataManager.instance.StoryLevels[num].Item1;
                loadText.text = DataManager.instance.screenPairs[loadScreen].text;
                loading.Clear();
                StartCoroutine(ToggleLoadingScreen(true, () =>
                {
                    loading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).ToAsync());
                    if (DataManager.instance.screenPairs[loadScreen].loadCinematicScene)
                        loading.Add(SceneManager
                            .LoadSceneAsync(DataManager.instance.screenPairs[loadScreen].targetSceneBuildIndex,
                                LoadSceneMode.Additive).ToAsync());
                    _heldLevelIdentification = DataManager.instance.StoryLevels[num];
                    StartCoroutine(GetPausedLoadProgress(() =>
                    {
                        var sr = loadingScreen.GetComponent<Image>();
                        loadingScreen
                            .LeanValue(value => { sr.color = value; }, new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), 1f)
                            .setIgnoreTimeScale(true)
                            .setOnComplete(() =>
                            {
                                skipText.color = new Color(1, 1, 1, 0);
                                skipText.gameObject.SetActive(true);
                                skipText.gameObject.LeanValue(value =>
                                {
                                    skipText.color = value;
                                }, new Color(1, 1, 1, 0), Color.white, flashSpeed).setLoopPingPong();
                            });
                        _sceneHolder = SceneManager.LoadSceneAsync(DataManager.instance.StoryLevels[num].Item2,
                            LoadSceneMode.Additive);
                        _sceneHolder.allowSceneActivation = false;
                    }));
                }));
            }
            else
            {
                loadText.text = "";
                StartCoroutine(ToggleLoadingScreen(true, () =>
                {
                    loading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).ToAsync());
                    loading.Add(SceneManager.LoadSceneAsync(DataManager.instance.StoryLevels[num].Item2, LoadSceneMode.Additive).ToAsync());
                    StartCoroutine(GetLoadProgress(() => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(DataManager.instance.StoryLevels[num].Item2))));
                }));
            }
        }

        public void PassData(int num, List<Tuple<Vector2Int, PoolBlock>> data)
        {
            StartCoroutine(ToggleLoadingScreen(true, () =>
            {
                loading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).ToAsync());
                loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.Game, LoadSceneMode.Additive).ToAsync());
                loading[loading.Count - 1].Then(() =>
                {
                    Task.Delay(1000);
                    LevelManager.Instance.Initialize(num, data);
                });

                StartCoroutine(GetLoadProgress(() => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int) SceneIndexes.Game))));
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
        
        public IEnumerator GetPausedLoadProgress(Action after) {
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
            after();
        }
    }
}
