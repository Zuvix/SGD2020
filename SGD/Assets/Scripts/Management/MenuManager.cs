using Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Management
{
    public class MenuManager : MonoBehaviour
    {
        public LevelDataObject dataSource;
        
        [Header("Buttons")] 
        public GameObject buttonPrefab;
        public Button levelButton;
        public Button arcadeButton;
        public Button quitButton;
    
        [Header("Levels")]
        public Transform levelContainer;
        public Transform levelArcadeContainer;
        
        private void Start()
        {
            // Buttons
            levelButton.onClick.AddListener(ToggleLevelContainer);
            arcadeButton.onClick.AddListener(ToggleArcadeLevelContainer);
            quitButton.onClick.AddListener(() =>
            {
                if (Application.isEditor)
                {
                    #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                    #endif
                }
                else
                {
                    Application.Quit();
                }
            });
        
            // Levels
            if (levelContainer.childCount > 0)
            {
                for (var x = 0; x < levelContainer.childCount; x++)
                {
                    Destroy(levelContainer.GetChild(x).gameObject);
                }
            }
            if (levelArcadeContainer.childCount > 0)
            {
                for (var x = 0; x < levelArcadeContainer.childCount; x++)
                {
                    Destroy(levelArcadeContainer.GetChild(x).gameObject);
                }
            }

            var counter = 0;
            foreach (var level in DataManager.instance.StoryLevels)
            {
                var index = counter;
                var a = Instantiate(buttonPrefab, levelContainer, false);
                a.transform.localScale = Vector3.one;
                a.GetComponentInChildren<TMP_Text>().text = counter++.ToString();
                a.GetComponent<Button>().onClick.AddListener(() => TransitionManager.instance.LoadStoryLevel(index));
            }

            counter = 0;
            foreach (var level in dataSource.levels)
            {
                var index = counter;
                var a = Instantiate(buttonPrefab, levelArcadeContainer, false);
                a.transform.localScale = Vector3.one;
                a.GetComponentInChildren<TMP_Text>().text = counter++.ToString();
                a.GetComponent<Button>().onClick.AddListener(() => TransitionManager.instance.LoadLevel(index));
            }
        }

        private void ToggleLevelContainer()
        {
            var a = levelContainer.gameObject.activeInHierarchy;
            // TODO: toggle anim
            levelContainer.gameObject.SetActive(!a);
        }
        
        private void ToggleArcadeLevelContainer()
        {
            var a = levelArcadeContainer.gameObject.activeInHierarchy;
            // TODO: toggle anim
            levelArcadeContainer.gameObject.SetActive(!a);
        }
    }
}
