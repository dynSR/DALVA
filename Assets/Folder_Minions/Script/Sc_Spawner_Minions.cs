using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sc_Spawner_Minions : MonoBehaviour
{
    public float timeBetweenWaves;
    public float waitForSecond;
    public GameObject minions;
    public List<GameObject> groupOfMinions;
    public List<GameObject> groupOfMinions1;
    public Transform startPos;
    //public TMP_Text waveCountDownText;
    public waypoints waypoints;

    private int waveIndex = 0;
    //private float countdown = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("startCoroutine");
        StartCoroutine(StartWave());
    }

    // Update is called once per frame
    void Update()
    {
        /*if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;*/
        
        /*if (Input.GetKeyDown(KeyCode.H))
            StartCoroutine(StartWave());*/
    }
    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        Debug.Log("startCoroutine(SpawnWave)");
        StartCoroutine(SpawnWave());
    }
    IEnumerator SpawnWave()
    {
        Debug.Log("spawnWave " + waveIndex);
        waveIndex++;
        if (waveIndex % 2 == 1)
        {
            Debug.Log("(waveIndex % 2 == 1)");
            for (int j = 0; j < groupOfMinions.Count; j++)
            {
                GameObject currentMinion;
                currentMinion = Instantiate(groupOfMinions[j], startPos.position, startPos.rotation);
                currentMinion.GetComponent<Sc_Minions_Base>().WaypointsToReach = waypoints.points;
                if (j == groupOfMinions.Count-1)
                {
                    StartCoroutine(StartWave());
                }
                yield return new WaitForSeconds(waitForSecond);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("else");
            for (int j = 0; j < groupOfMinions1.Count; j++)
            {
                GameObject currentMinion;
                currentMinion = Instantiate(groupOfMinions1[j], startPos.position, startPos.rotation);
                currentMinion.GetComponent<Sc_Minions_Base>().WaypointsToReach = waypoints.points;
                if (j == groupOfMinions1.Count-1)
                {
                    StartCoroutine(StartWave());
                }
                yield return new WaitForSeconds(waitForSecond);
                yield return new WaitForEndOfFrame();
            }
        }
        /*for (int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(waitForSecond);
        }*/
    }

    /*void SpawnEnemy()
    {
        GameObject currentMinion;
        currentMinion = Instantiate(minions, startPos.position, startPos.rotation);
        currentMinion.GetComponent<Sc_Minions_Base>().WaypointsToReach = waypoints.points;
    }*/
}
