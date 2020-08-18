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

        private void Start()
        {
            // Buttons
            levelButton.onClick.AddListener(ToggleLevelContainer);
            arcadeButton.onClick.AddListener(() =>
            {
            
            });
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

            var counter = 0;
            foreach (var level in dataSource.levels)
            {
                var index = counter;
                var a = Instantiate(buttonPrefab, levelContainer, false);
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
    }
}
