/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieButton : MonoBehaviour
{
    public GameObject zombiePrefab; // 事前に設定するZombie Prefab
    public Camera godCamera; // ゲーム内のカメラ
    public float max_x;
    public float min_x;
    public float max_z;
    public float min_z;
    private bool isZombieSpawnMode = false; // ゾンビをスポーンするかどうかのフラグ

    // この関数はUIボタンにリンクさせます。ボタンが押されるとこの関数が呼ばれ、
    // isZombieSpawnModeフラグがtrueになり、次にゲーム内をクリックしたときにゾンビがスポーンします。
    public void ActivateZombieSpawnMode()
    {
        isZombieSpawnMode = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isZombieSpawnMode && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.point.z<=max_z && hit.point.z>=min_z){
                    Instantiate(zombiePrefab, hit.point, Quaternion.identity);
                }
            }
        }
    }
}

*/