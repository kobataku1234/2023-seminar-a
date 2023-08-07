using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GodController : MonoBehaviour
{
    [SerializeField]
    GameObject spawnerPrehab; // 設置するSpawner
    [SerializeField]
    Transform target = null; // 敵の目標
    [SerializeField]
    Camera godCamera;
    [SerializeField]
    TextMeshProUGUI countdownText = null; // カウントダウンを表示するテキスト
    [SerializeField]
    TextMeshProUGUI GameOverText = null; // ゲームオーバーを表示するテキスト
    [SerializeField]
    TextMeshProUGUI VictoryText = null; // プレイヤーのHPが0になったときに表示するテキスト
    [SerializeField]
    float countdownTime = 180f; // カウントダウンの時間
    [SerializeField]
    FirstPersonMovement player; // Playerのスクリプト

    Vector3 mousePosition;

    void Start()
    {
        StartCoroutine(nameof(CountdownTimer));
    }

    void Update()
    {
        /* キーボードでZが押されるとsapwnerPrehabを設置 */
        if(Input.GetKeyDown(KeyCode.Z)){
            SetSpawner();
        }      

        /* PlayerのHPを確認 */
        if(player.Hp <= 0){
            VictoryText.text = "You Win!";
            Time.timeScale = 0f;
        }  
        /* すべての家が破壊されたか確認 */
        if(GameObject.FindGameObjectsWithTag("House").Length == 0){
            VictoryText.text = "You Win!";
            // Time.timeScale = 0f;
        }
    }

    void SetSpawner(){
        Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPos = hit.point;
            EnemyController enemy = Instantiate(spawnerPrehab, worldPos, Quaternion.identity).GetComponent<EnemyController>();
            enemy.Target = target;
        }
    }

    IEnumerator CountdownTimer()
    {
      while (countdownTime > 0)
      {
        yield return new WaitForSeconds(1f);
        countdownTime--;
        countdownText.text = countdownTime.ToString();
      }
      // 時間が尽きたときの処理
      GameOverText.text = "GameOver!";
        Time.timeScale = 0f;
    }
}
