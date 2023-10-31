using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete Save File")]
    private void DeleteSavedData() {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataHandler.Delete();
    }

    private void Awake(){
        if(instance != null){
            Destroy(instance.gameObject);
        }
        else{
            instance = this;
        }
    }

    private void Start(){
        dataHandler = new FileDataHandler(Application.persistentDataPath,fileName);
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }

    public void NewGame(){
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // gamedata = data from data handler
        gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No saved data found");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

        Debug.Log("Loaded currency " + gameData.currency);
        // Debug.Log("Path: "+ Application.persistentDataPath);
    }
    public void SaveGame(){
        //data handler save game data
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
        Debug.Log("Saved currency " + gameData.currency);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers(){
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }
}
