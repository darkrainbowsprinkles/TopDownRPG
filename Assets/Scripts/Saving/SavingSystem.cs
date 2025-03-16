using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if(state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);

            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Delete(string saveFile)
        {
            print("Deleting: " + GetPathFromSaveFile(saveFile));
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public bool SaveFileExists(string saveFile)
        {
            return File.Exists(GetPathFromSaveFile(saveFile));
        }

        public IEnumerable<string> ListSaves()
        {
            foreach(string path in Directory.EnumerateFiles(Application.persistentDataPath))
            {
                if(Path.GetExtension(path) == ".sav")
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }

        }

        Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if(!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using(FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
        
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);

            print("Saving to " + path);
            
            using(FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        void CaptureState(Dictionary<string, object> state)
        {
            foreach(var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        void RestoreState(Dictionary<string, object> state)
        {
            foreach(var saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();

                if(state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}