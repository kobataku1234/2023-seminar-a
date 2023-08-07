/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieButton : MonoBehaviour
{
    public GameObject zombiePrefab;// 事前に設定するZombie Prefab
    public Camera godCamera; // ゲーム内のカメラ
    private int time=100;//この時間ごとにゲージを更新する
    private float countTime=0f;
    private float nextCountTime=0f;
    public GameObject button1_object = null;

    [SerializeField] int state;

    // ゾンビのボタンリスト
    GameObject Zombie1Button;
    GameObject Zombie2Button;
    GameObject Zombie3Button;
    GameObject Zombie4Button;
    List<GameObject> ZombieButtonList;

    //各ゾンビが1ストックたまるのにかかる時間
    [SerializeField] int MaxCount1;
    [SerializeField] int MaxCount2;
    [SerializeField] int MaxCount3;
    [SerializeField] int MaxCount4;

    //各ゾンビのゲージ
    public static int count1;
    public int count2;
    private int count3;
    private int count4;

    //各ゾンビの最大ストック数
    [SerializeField] int MaxStock1;
    [SerializeField] int MaxStock2;
    [SerializeField] int MaxStock3;
    [SerializeField] int MaxStock4;

    //各ゾンビのストック数
    public static int[] stock;
    
    private Slider CountSlider1;

    // ゾンビをスポーンするかどうかのフラグ
    private bool isZombieSpawnMode = false;
    private int check = 0;
    static int zombieSpawnMode = 0; // 1, 2, 3, 4 のどれかの値を取る

    // 初めのUpdateの前にやる
    void Start()
    {
        // ゾンビのボタンを検索しリスト化
        Zombie1Button = GameObject.Find("Zombie1Button");
        Zombie2Button = GameObject.Find("Zombie2Button");
        Zombie3Button = GameObject.Find("Zombie3Button");
        Zombie4Button = GameObject.Find("Zombie4Button");
        ZombieButtonList = new List<GameObject>()
        {
            Zombie1Button, Zombie2Button, Zombie3Button, Zombie4Button
        };

        // 全てのボタンを白色に
        foreach (GameObject gameObject in ZombieButtonList)
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }

        //ゲージ用
        CountSlider1=GameObject.Find("CountSlider1").GetComponent<Slider>();
        count1=0;
        count2=0;
        count3=0;
        count4=0;
        CountSlider1.value=0;
        stock=new int [4];
    }

    // この関数はUIボタンにリンクさせます。
    // ボタンが押されるとこの関数が呼ばれ、
    // isZombieSpawnModeフラグがtrueになり、次にゲーム内をクリックしたときにゾンビがスポーンします。
    public void ActivateZombieSpawnMode(int mode)
    {
        isZombieSpawnMode = true;
        zombieSpawnMode = mode;
        check = mode;
        
        // 全てのボタンを白色に
        foreach (GameObject gameObject in ZombieButtonList)
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }
        // 選択しているボタンを灰色に
        this.gameObject.GetComponent<Image>().color = Color.gray;
        string ZombieButtonName = this.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        Text button1_text = button1_object.GetComponent<Text>();
        button1_text.text =$"{stock[0]}";
        
        if(state==1){
            if(countTime>=nextCountTime){
                if(count1+time>=MaxCount1&&stock[0]!=MaxStock1){
                    //最大までストック
                    if(stock[0]+1==MaxStock1){
                        count1=MaxCount1;
                        stock[0]=MaxStock1;
                        CountSlider1.value=1;
                    }
                    //ストック数増加
                    else{
                        count1=count1+time-MaxCount1;
                        stock[0]=stock[0]+1;
                        CountSlider1.value=(float)count1/MaxCount1;
                    }
                }
                else if(count1+time<MaxCount1){
                    //ゲージ増加
                    count1=count1+time;
                    CountSlider1.value=(float)count1/MaxCount1;
                }
            }
        }
        if (check==zombieSpawnMode&&stock[0]!=0){
            if (isZombieSpawnMode && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Instantiate(zombiePrefab, hit.point, Quaternion.identity);
                    if(stock[0]==MaxStock1){
                        stock[0]=stock[0]-1;
                        count1=0;
                    }
                    else{
                        stock[0]=stock[0]-1;
                    }
                }
            }
        }
        countTime +=Time.deltaTime;
    }
}

*/