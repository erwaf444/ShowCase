using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{

    public GameObject prefab;
    public float spawnRate = 1f;
    public float minHeight = -1f;
    public float maxHeight = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    public void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    public void Spawn()
    {
        GameObject pips = Instantiate(prefab, transform.position, Quaternion.identity);
        pips.transform.position += Vector3.up * 4f;
        pips.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }

    public void SpawnWithDelay(float delay)
    {
        StartCoroutine(SpawnAfterDelay(delay));
    }

    private IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnEnable();
    }

}
 