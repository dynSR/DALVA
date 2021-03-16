using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum WaveState { WaitUntilStartOfTheGame, Standby, IsSpawning }

public class SpawnerSystem : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject [] waveMinions;
        public int waveID;
    }

    public WaveState waveState; // public for debug purpose

    [Header("SPAWNER PARAMETERS")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float delayBeforeSpawningFirstWave = 45f;
    [SerializeField] private float spawnRate = 5f;
    [SerializeField] private float delayBetweenWaves = 30f;
    public float countdown = 0f; // put in private after tests
    public int indexOfCurrentWave = 0; // to delete after tests

    [Header("WAVES")]
    [SerializeField] private List<Wave> waves;

    private int waveIndex = 0;

    void Start()
    {
        //Waits for a certain time then change wave state to spawn (it's taking in count the delay before the game realy starts)
        StartCoroutine(WaitingState(delayBeforeSpawningFirstWave, SpawnWave()));

        countdown = delayBetweenWaves;
    }

    void Update()
    {

        if (waveState == WaveState.Standby && countdown <= 0)
        {
            if (waveState != WaveState.IsSpawning)
            {
                StartCoroutine(SpawnWave());
            }
        }
        else if (waveState == WaveState.Standby)
        {
            countdown -= Time.deltaTime;
        }
    }

    private IEnumerator SpawnWave()
    {
        if (indexOfCurrentWave >= waves.Count) yield break;

        waveState = WaveState.IsSpawning;

        Wave currentWave = waves[indexOfCurrentWave];
        Debug.Log(currentWave.waveName);

        //For each minions in it, loop spawning

        for (int i = 0; i < currentWave.waveMinions.Length; i++)
        {
            SpawnMinions(currentWave.waveMinions[i]);

            yield return new WaitForSeconds(spawnRate);
        }

        //Change wave state
        waveState = WaveState.Standby;

        //Swap Wave ID to spawn an another type of wave
        if (indexOfCurrentWave == 0)
            indexOfCurrentWave = 1;
        else if (indexOfCurrentWave == 1)
            indexOfCurrentWave = 0;

        countdown = delayBetweenWaves;

        yield break;
    }

    void SpawnMinions(GameObject minion)
    {
        Debug.Log("MinionBehaviour spawned");

        waveIndex++;
        GameObject currentMinion = Instantiate(minion, new Vector3(spawnPosition.position.x + waveIndex, spawnPosition.position.y, spawnPosition.position.z), spawnPosition.rotation);
    }


    private IEnumerator WaitingState(float delay, IEnumerator enumerator)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnWave());
    }
}