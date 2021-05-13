using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkTonic.MasterAudio;

public enum WaveState { WaitUntilStartOfTheGame, Standby, IsSpawning }

public class SpawnerSystem : MonoBehaviour
{
    public delegate void WaveAlertHandler(float timer);
    public event WaveAlertHandler OnWaveStartinSoon;
    public event WaveAlertHandler OnWavePossibilityToSpawnState;

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int waveID;
        public GameObject [] waveMinions;
    }

    public WaveState waveState; // public for debug purpose

    [Header("SPAWNER PARAMETERS")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float delayBeforeSpawningFirstWave = 45f;
    [SerializeField] private float spawnRate = 5f;
    [SerializeField] private float delayBetweenWaves = 30f;
    private float firstCountdown = 0f; // put in private after tests
    private float countdown = 0f;
    private int indexOfCurrentWave = 0; // to delete after tests
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private GameObject spawnObjectFeedback;

    [Header("WAVES")]
    [SerializeField] private List<Wave> waves;

    [Header("SOUNDS")]
    [MasterCustomEvent] public string spawnCustomEvent;

    private int waveIndex = 0;

    public List<Transform> Waypoints { get => waypoints; }
    public int IndexOfCurrentWave { get => indexOfCurrentWave; set => indexOfCurrentWave = value; }
    public List<Wave> Waves { get => waves; }
    public float FirstCountdown { get => firstCountdown; set => firstCountdown = value; }
    public float Countdown { get => countdown; set => countdown = value; }
    public float DelayBeforeSpawningFirstWave { get => delayBeforeSpawningFirstWave; set => delayBeforeSpawningFirstWave = value; }
    public float DelayBetweenWaves { get => delayBetweenWaves; set => delayBetweenWaves = value; }

    EventSounds EventSounds => GetComponent<EventSounds>();

    private void OnEnable()
    {
        if (EventSounds != null)
            EventSounds.RegisterReceiver();
    }

    void Start()
    {
        //Waits for a certain time then change wave state to spawn (it's taking in count the delay before the game realy starts)
        StartCoroutine(WaitingState(DelayBeforeSpawningFirstWave, SpawnWave()));

        FirstCountdown = DelayBeforeSpawningFirstWave;
        OnWaveStartinSoon?.Invoke(DelayBeforeSpawningFirstWave);

        if (CanSpawnWave())
            OnWavePossibilityToSpawnState?.Invoke(1);
           
        //Set Fill amount here for the first delay
        Countdown = DelayBetweenWaves;
    }

    void Update()
    {
        if (waveState == WaveState.Standby && Countdown <= 0)
        {
            if (waveState != WaveState.IsSpawning)
            {
                StartCoroutine(SpawnWave());
            }
        }
        else if (waveState == WaveState.Standby)
        {
            Countdown -= Time.deltaTime;
        }
        else if (waveState == WaveState.WaitUntilStartOfTheGame)
        {
            FirstCountdown -= Time.deltaTime;
        }
    }

    private IEnumerator SpawnWave()
    {
        //Deactivate fill displayer content
        if (!CanSpawnWave()) 
        {
            yield break;
        }

        MasterAudio.FireCustomEvent(spawnCustomEvent, transform);

        OnWavePossibilityToSpawnState?.Invoke(0);
        waveState = WaveState.IsSpawning;
        GameManager.Instance.WaveCountHasBeenSet = false;

        if (spawnObjectFeedback != null)
            spawnObjectFeedback.SetActive(true);
        else Debug.LogError("Add the spawnObjectFeedback in the empty field");

        Wave currentWave = Waves[IndexOfCurrentWave];
        Debug.Log(currentWave.waveName);

        //For each minions in it, loop spawning

        for (int i = 0; i < currentWave.waveMinions.Length; i++)
        {
            if (currentWave.waveMinions[i] != null)
            {
                NPCController spawningMinionController = currentWave.waveMinions[i].GetComponent<NPCController>();

                if (spawningMinionController.IsABoss && !GameManager.Instance.ItIsABossWave)
                    GameManager.Instance.ItIsABossWave = true;

                SpawnMinions(currentWave.waveMinions[i]);

                yield return new WaitForSeconds(spawnRate);
            }   
        }

        if (spawnObjectFeedback != null)
            spawnObjectFeedback.SetActive(false);
        else Debug.LogError("Add the spawnObjectFeedback in the empty field");

        //Change wave state
        waveState = WaveState.Standby;

        //Swap Wave ID to spawn an another type of wave
        IndexOfCurrentWave++;

        GameManager.Instance.UpdateWaveCount();
        
        if (CanSpawnWave())
            OnWavePossibilityToSpawnState?.Invoke(1);
        else OnWavePossibilityToSpawnState?.Invoke(0);

        Countdown = DelayBetweenWaves;
        //Set Displayer fill amount here
        OnWaveStartinSoon?.Invoke(DelayBetweenWaves);
        yield break;
    }

    bool CanSpawnWave()
    {
        bool canSpawn = false;

        if (IndexOfCurrentWave >= Waves.Count || GameManager.Instance.ItIsABossWave) return false;

        if (IndexOfCurrentWave <= Waves.Count && Waves[IndexOfCurrentWave].waveMinions.Length > 0)
        {
            float indexNotNull = 0;

            for (int i = Waves[IndexOfCurrentWave].waveMinions.Length - 1; i >= 0; i--)
            {
                if (Waves[IndexOfCurrentWave].waveMinions[i].gameObject != null)
                {
                    indexNotNull++;
                }

                if (indexNotNull == 0)
                {
                    canSpawn = false;
                    OnWavePossibilityToSpawnState?.Invoke(0);
                }
                else if (indexNotNull >= 1)
                {
                    canSpawn = true;
                    OnWavePossibilityToSpawnState?.Invoke(1);
                }
            }
        }

        //if (IndexOfCurrentWave >= Waves.Count && Waves[IndexOfCurrentWave].waveMinions.Length < 0)
        //{
        //    canSpawn = false;
        //    OnWavePossibilityToSpawnState?.Invoke(0);
        //}
        //else if (IndexOfCurrentWave <= Waves.Count && Waves[IndexOfCurrentWave].waveMinions.Length > 0)
        //{
        //    canSpawn = true;
        //    OnWavePossibilityToSpawnState?.Invoke(1);
        //}

        return canSpawn;
    }

    void SpawnMinions(GameObject minion)
    {
        Debug.Log("MinionBehaviour spawned");

        waveIndex++;

        GameObject currentMinion = Instantiate(minion, new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z), spawnPosition.rotation);

        NPCController spawningMinionController = currentMinion.GetComponent<NPCController>();

        if (GameManager.Instance.ItIsABossWave)
        {
            GameManager.Instance.UpdateRemainingMonsterValue(1);
            spawningMinionController.IsABossWaveMember = true;

            Debug.Log("SET SPAWNED MINION AS A BOSS WAVE MEMBER !!!!!!!!!!!!");
        }

        for (int i = 0; i < Waypoints.Count; i++)
        {
            currentMinion.GetComponent<NPCController>().waypoints.Add(Waypoints[i]);
        }
    }

    private IEnumerator WaitingState(float delay, IEnumerator enumerator)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnWave());
    }
}