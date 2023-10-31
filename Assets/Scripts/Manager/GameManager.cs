using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{

    public static GameManager instance;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;


    private void Awake()
    {
        if (instance != null) Destroy(instance.gameObject);
        else instance = this;

    }
    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();
    }
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // public void LoadData(GameData _data)
    // {
    //     foreach(KeyValuePair<string,bool> pair in _data.checkpoints){
    //         foreach (Checkpoint checkpoint in checkpoints)
    //         {
    //             Debug.Log("ID: "+checkpoint.id);
    //             Debug.Log("Checkpoint: "+checkpoint);
    //             Debug.Log("Pair Key: "+pair.Key);
    //             Debug.Log("Pair Value: "+pair.Value);
    //             Debug.Log("Check 1:" + (checkpoint.id == pair.Key).ToString());
    //             Debug.Log("Check 2:" + (pair.Value == true).ToString());
    //             if(checkpoint.id == pair.Key && pair.Value == true){
    //                 checkpoint.ActivateCheckpoint();
    //             }
    //         }
    //     }
    //     Debug.Log(_data.closestCheckpointId);
    //     closestCheckpointId = _data.closestCheckpointId;

    //     Invoke("PlacePlayerAtClosestCheckpoint",.1f);


    // }
    // private void PlacePlayerAtClosestCheckpoint(){
    //     foreach (Checkpoint checkpoint in checkpoints)
    //     {
    //         if(closestCheckpointId == checkpoint.id){
    //             PlayerManager.instance.player.transform.position = checkpoint.transform.position;
    //         }
    //     }
    // }

    public void LoadData(GameData _data)
    {
        closestCheckpointId = _data.closestCheckpointId;

        StartCoroutine(ActivateCheckpoints(_data));

        Invoke("PlacePlayerAtClosestCheckpoint", .1f);
    }

    IEnumerator ActivateCheckpoints(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void PlacePlayerAtClosestCheckpoint()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
        }
    }
    public void SaveData(ref GameData _data)
    {
        _data.closestCheckpointId = FindClosestCheckPoint().id;
        _data.checkpoints.Clear();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    private Checkpoint FindClosestCheckPoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }
}
