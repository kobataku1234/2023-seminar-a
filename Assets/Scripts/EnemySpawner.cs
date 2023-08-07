using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [SerializeField]
  GameObject[] enemyPrefabs = { }; // 敵のプレハブ
  [SerializeField]
  Transform target = null; // 敵の目標
  [SerializeField]
  int maxSpawnCount = 50; // 最大スポーン数
  [SerializeField, Min(0)]
  float spawnDelay = 0; // スポーン開始までの待ち時間
  [SerializeField, Min(0)]
  float spawnInterval = 3; // スポーン間隔
  [SerializeField]
  bool spawnOnStart = false; // スタート時にスポーンするかどうか

  Transform thisTransform; // このスクリプトがアタッチされているオブジェクトのTransform
  WaitForSeconds spawnDelayWait; // スポーン開始までの待ち時間
  WaitForSeconds spawnWait; // スポーン間隔

  void Start()
  {
    thisTransform = transform;
    spawnDelayWait = new WaitForSeconds(spawnDelay); 
    spawnWait = new WaitForSeconds(spawnInterval);
    if (spawnOnStart)
    {
      StartSpawn();
    }
  }

    
  public void StartSpawn()
  {
    StartCoroutine(nameof(SpawnTimer));
  }
  public void StopSpawn()
  {
    StopCoroutine(nameof(SpawnTimer));
  }
  IEnumerator SpawnTimer() 
  {
    yield return spawnDelayWait;
    for(int i = 0; i < maxSpawnCount; i++)
    {
      EnemyController enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], thisTransform.position, Quaternion.identity).GetComponent<EnemyController>();
      enemy.Target = target;
      yield return spawnWait;
    }
  }
}