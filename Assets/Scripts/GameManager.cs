using System;
using Sound;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
    }

    public LevelGenerator levelGenerator;
    public GameObject playerPrefab;

    public GameObject loadingCamera;
    public GameObject loadingScreen;

    public EndLevel endLevel = null;

    public PlayerEntity Player;
    public int killCount;
    public int killCap;
    public int difficultyScale;
    public bool isTutorial = false;

    public void Start()
    {
        // Generate Level
        difficultyScale = 1;
        if (isTutorial)
            return;
        loadingScreen.SetActive(true);
        levelGenerator.GenerateLevel(difficultyScale);
        Vector3Int rl = new Vector3Int(levelGenerator.getSpawn().location.x * 12 + levelGenerator.getSpawn().size.x, levelGenerator.getSpawn().location.y * 12 + 1, levelGenerator.getSpawn().location.z * 12 + levelGenerator.getSpawn().size.z);
        GameObject go = Instantiate(playerPrefab, rl, Quaternion.identity);
        Destroy(loadingCamera);
        Player = go.GetComponent<PlayerEntity>();
        loadingScreen.SetActive(false);
        endLevel = FindObjectOfType<EndLevel>();
    }

    public void Update()
    {
        if (endLevel != null)
        {

            if (endLevel.endReached == true)//Input.GetKeyDown("p")
            {
                loadingScreen.SetActive(true);
                endLevel = null;
                Debug.Log("Generate New Level");
                difficultyScale += 1;
                levelGenerator.GenerateLevel(difficultyScale);
                Vector3Int rl = new Vector3Int(levelGenerator.getSpawn().location.x * 12 + levelGenerator.getSpawn().size.x, levelGenerator.getSpawn().location.y * 12 + 1, levelGenerator.getSpawn().location.z * 12 + levelGenerator.getSpawn().size.z);
                Player.gameObject.transform.position = rl;
                loadingScreen.SetActive(false);
                endLevel = FindObjectOfType<EndLevel>();
            }
        }
    }

    public void OnKill()
    {
        killCount++;
        if (killCount >= killCap)
        {
            killCount = 0;
            killCap *= 2;
        }
    }
}