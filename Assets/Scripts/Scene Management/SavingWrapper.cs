using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using RPG.Audio;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 0.2f;
        [SerializeField] float fadeOutTime = 0.2f;
        [SerializeField] float waitForFadeTime = 0.3f;
        [SerializeField] float fadeOutMusicTime = 3;
        [SerializeField] float fadeInMusicTime = 3;
        [SerializeField] int firstLevelBuildIndex = 1;
        [SerializeField] int menuLevelBuildIndex = 0;
        SavingSystem savingSystem;
        const string currentSaveKey = "currentSaveFile";

        public IEnumerable<string> GetSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }

        public void ContinueGame()
        {
            if(CanContinue()) 
            {
                StartCoroutine(LoadLastScene());
            }
        }

        public void NewGame(string saveFile)
        {
            if(!string.IsNullOrEmpty(saveFile)) 
            {
                SetCurrentSave(saveFile);
                StartCoroutine(LoadScene(firstLevelBuildIndex));
            }
        }

        public void LoadGame(string saveFile)
        {
            if(CanLoad()) 
            {
                SetCurrentSave(saveFile);
                ContinueGame();
            }
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadScene(menuLevelBuildIndex));
        }

        public bool CanContinue()
        {
            if(!PlayerPrefs.HasKey(currentSaveKey)) 
            {
                return false;
            }

            if(!savingSystem.SaveFileExists(GetCurrentSave())) 
            {
                return false;
            }

            return true;
        }

        public bool CanLoad()
        {
            List<string> savingFiles = new(savingSystem.ListSaves());
            return savingFiles.Count > 0;
        }

        public void Load()
        {
            savingSystem.Load(GetCurrentSave());
        }

        public void Save()
        {
            savingSystem.Save(GetCurrentSave());
        }

        public void Delete()
        {
            savingSystem.Delete(GetCurrentSave());
        }

        void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
        }

        void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(currentSaveKey, saveFile);
        }

        string GetCurrentSave()
        {
            return PlayerPrefs.GetString(currentSaveKey);
        }

        IEnumerator LoadScene(int sceneIndex)
        {
            Fader fader = FindObjectOfType<Fader>();
            AudioManager audioFader = FindObjectOfType<AudioManager>();

            Cursor.visible = false;

            yield return new WaitForSecondsRealtime(waitForFadeTime);

            audioFader.FadeOutMaster(fadeOutMusicTime);

            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            yield return audioFader.FadeInMaster(fadeInMusicTime);
            yield return fader.FadeIn(fadeInTime);

            Cursor.visible = true;
        }

        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            AudioManager audioManager = FindObjectOfType<AudioManager>();

            Cursor.visible = false;

            yield return new WaitForSecondsRealtime(waitForFadeTime);

            audioManager.FadeOutMaster(fadeOutMusicTime);

            yield return fader.FadeOut(fadeOutTime);
            yield return savingSystem.LoadLastScene(GetCurrentSave());
            yield return audioManager.FadeInMaster(fadeInMusicTime);
            yield return fader.FadeIn(fadeInTime);

            Cursor.visible = true;
        }

#if UNITY_EDITOR
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }
#endif
    }
}
