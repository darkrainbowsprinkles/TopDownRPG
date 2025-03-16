using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Menus
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] Transform contentRoot;
        [SerializeField] GameObject buttonPrefab;

        void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if(savingWrapper == null) return;

            foreach(Transform children in contentRoot)
            {
                Destroy(children.gameObject);
            }

            foreach(string saveFile in savingWrapper.GetSaves())
            {
                GameObject buttonInstance  = Instantiate(buttonPrefab, contentRoot);
                TMP_Text buttonText  = buttonInstance.GetComponentInChildren<TMP_Text>();
                Button button = buttonInstance.GetComponent<Button>();

                buttonText.text = saveFile;
                button.onClick.AddListener(() => savingWrapper.LoadGame(saveFile));
            }
        }
    }
}
