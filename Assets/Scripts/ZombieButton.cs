using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZombieButton : MonoBehaviour
{
    public GameObject zombiePrefab;
    // 生成するゾンビ
    public Camera godCamera; 
    // 神視点のカメラ
    private int time=100;
    // ゲージ更新間隔
    private float countTime=0f;
    private float nextCountTime=0f;
    
    /* それぞれのゾンビが使えるようになるまでの家の数 */
    public static int house2=1;
    public static int house3=2;
    public static int house4=3;

    [SerializeField] int state;
    // どのボタンにスクリプトがアタッチされたかを表す

    //ゾンビのストック数表示用
    [SerializeField]
    private TextMeshProUGUI button1Text;
    [SerializeField]
    private TextMeshProUGUI button2Text;
    [SerializeField]
    private TextMeshProUGUI button3Text;
    [SerializeField]
    private TextMeshProUGUI button4Text;  

    // スポーン可能範囲
    [SerializeField]  
    float max_z;
    [SerializeField] 
    float min_z;
    [SerializeField] 
    float max_x;
    [SerializeField] 
    float min_x;

    // ゾンビのボタンリスト
    GameObject Zombie1Button;
    GameObject Zombie2Button;
    GameObject Zombie3Button;
    GameObject Zombie4Button;
    List<GameObject> ZombieButtonList;

    //各ゾンビが1ストックたまるのにかかる時間
    public static int MaxCount1=30000;
    public static int MaxCount2=50000;
    public static int MaxCount3=100000;
    public static int MaxCount4=100000;

    //各ゾンビのゲージ
    public static int count1;
    public static int count2;
    public static int count3;
    public static int count4;

    //各ゾンビの最大ストック数
    public static int MaxStock1=5;
    public static int MaxStock2=4;
    public static int MaxStock3=3;
    public static int MaxStock4=2;

    //各ゾンビのストック数
    public static int[] stock;

    //ゲージ参照用
    private Slider CountSlider1;
    private Slider CountSlider2;
    private Slider CountSlider3;
    private Slider CountSlider4;

    public static Vector2 position1;
    public static Vector2 size1;

    // ゾンビをスポーンするかどうかのフラグ
    private bool isZombieSpawnMode = false;
    private int check = 0;
    static int zombieSpawnMode = 0; // 1, 2, 3, 4 のどれかの値を取る

    // スクリーン上のボタンの位置
    Vector3 screenButtonPosition1;
    Vector3 screenButtonPosition2;
    Vector3 screenButtonPosition3;
    Vector3 screenButtonPosition4;


    void Start()
    {
        if(state==11){
            // RectTransformコンポーネントを取得
            RectTransform rectTransform1 = GetComponent<RectTransform>();

            // オブジェクトの位置とサイズを取得
            position1 = rectTransform1.anchoredPosition; // ローカル座標系での位置
            size1 = rectTransform1.sizeDelta; // 幅と高さ

            // 座標とサイズの出力（デバッグ目的）
            
        }
        
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
        CountSlider2=GameObject.Find("CountSlider2").GetComponent<Slider>();
        CountSlider3=GameObject.Find("CountSlider3").GetComponent<Slider>();
        CountSlider4=GameObject.Find("CountSlider4").GetComponent<Slider>();
        count1=0;
        count2=0;
        count3=0;
        count4=0;
        CountSlider1.value=0;
        CountSlider2.value=0;
        CountSlider3.value=0;
        CountSlider4.value=0;
        stock=new int [4];
        button1Text.text = "0";
        button2Text.text = "0";
        button3Text.text = "0";
        button4Text.text = "0";
    }


    // この関数はUIボタンにリンクさせます。
    // ボタンが押されるとこの関数が呼ばれ、
    // isZombieSpawnModeフラグがtrueになり、次にゲーム内をクリックしたときにゾンビがスポーンします。
    public void ActivateZombieSpawnMode(int mode)
    {
        isZombieSpawnMode = true;
        zombieSpawnMode = mode;
        // どのボタンにスクリプトがアタッチされたかを表す
        check = mode;
        // どのボタンにスクリプトがアタッチされたかを表す
        
        // 全てのボタンを白色に
        foreach (GameObject gameObject in ZombieButtonList)
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }
        // 選択しているボタンを灰色に
        this.gameObject.GetComponent<Image>().color = Color.gray;
        string ZombieButtonName = this.gameObject.name;
    }

    void Update()
    {
        /* ボタンの位置を取得 */
        RectTransform rtf1 = Zombie1Button.GetComponent<RectTransform>();
        screenButtonPosition1 = rtf1.position;
        RectTransform rtf2 = Zombie2Button.GetComponent<RectTransform>();
        screenButtonPosition2 = rtf2.position;
        RectTransform rtf3 = Zombie3Button.GetComponent<RectTransform>();
        screenButtonPosition3 = rtf3.position;
        RectTransform rtf4 = Zombie4Button.GetComponent<RectTransform>();
        screenButtonPosition4 = rtf4.position;

        /* 壊した家の数を取得 */
        GameObject[] houseRemain = GameObject.FindGameObjectsWithTag("House");
        int housecount = 6 - houseRemain.Length;


        

        //CountSlider1の処理
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
            button1Text.text = stock[0].ToString("00");
        }
        //CountSlider2の処理
        if(state==2&&housecount>=house2){
            if(countTime>=nextCountTime){
                if(count2+time>=MaxCount2&&stock[1]!=MaxStock2){
                    //最大までストック
                    if(stock[1]+1==MaxStock2){
                        count2=MaxCount2;
                        stock[1]=MaxStock2;
                        CountSlider2.value=1;
                    }
                    //ストック数増加
                    else{
                        count2=count2+time-MaxCount2;
                        stock[1]=stock[1]+1;
                        CountSlider2.value=(float)count2/MaxCount2;
                    }
                }
                else if(count2+time<MaxCount2){
                    //ゲージ増加
                    count2=count2+time;
                    CountSlider2.value=(float)count2/MaxCount2;
                }
            }
            button2Text.text = stock[1].ToString("00");
        }
        //CountSlider3の処理
        if(state==3&&housecount>=house3){
            if(countTime>=nextCountTime){
                if(count3+time>=MaxCount3&&stock[2]!=MaxStock3){
                    //最大までストック
                    if(stock[2]+1==MaxStock3){
                        count3=MaxCount3;
                        stock[2]=MaxStock3;
                        CountSlider3.value=1;
                    }
                    //ストック数増加
                    else{
                        count3=count3+time-MaxCount3;
                        stock[2]=stock[2]+1;
                        CountSlider3.value=(float)count3/MaxCount3;
                    }
                }
                else if(count3+time<MaxCount3){
                    //ゲージ増加
                    count3=count3+time;
                    CountSlider3.value=(float)count3/MaxCount3;
                }
            }
            button3Text.text = stock[2].ToString("00");
        }
        //CountSlider4の処理
        if(state==4&&housecount>=house4){
            if(countTime>=nextCountTime){
                if(count4+time>=MaxCount4&&stock[3]!=MaxStock4){
                    //最大までストック
                    if(stock[3]+1==MaxStock4){
                        count4=MaxCount4;
                        stock[3]=MaxStock4;
                        CountSlider4.value=1;
                    }
                    //ストック数増加
                    else{
                        count4=count4+time-MaxCount4;
                        stock[3]=stock[3]+1;
                        CountSlider4.value=(float)count4/MaxCount4;
                    }
                }
                else if(count4+time<MaxCount4){
                    //ゲージ増加
                    count4=count4+time;
                    CountSlider4.value=(float)count4/MaxCount4;
                }
            }
            button4Text.text = stock[3].ToString("00");
        }

        /* ボタン1の処理 */
        if(state==11){
            if (check == zombieSpawnMode && stock[0]!=0){
            // ? && ストックが0ではないとき
                if (isZombieSpawnMode && Input.GetMouseButtonDown(0)){
                // ボタンがクリックされる && 右クリック
                    RaycastHit hit;
                    Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit)){
                        if(IsOnButton(Input.mousePosition)>0 && IsOkSpawn(hit.point.x,hit.point.z)>0){
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
            }
        }
        //ボタン2の処理
        if(state==22){
            if (check==zombieSpawnMode&&stock[1]!=0){
                if (isZombieSpawnMode && Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if(IsOnButton(Input.mousePosition)>0 && IsOkSpawn(hit.point.x,hit.point.z)>0){
                            Instantiate(zombiePrefab, hit.point, Quaternion.identity);
                            if(stock[1]==MaxStock2){
                                stock[1]=stock[1]-1;
                                count2=0;
                            }
                            else{
                                stock[1]=stock[1]-1;
                            }
                        }
                    }
                }
            }
        }
        //ボタン3の処理
        if(state==33){
            if (check==zombieSpawnMode&&stock[2]!=0){
                if (isZombieSpawnMode && Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if(IsOnButton(Input.mousePosition)>0 && IsOkSpawn(hit.point.x,hit.point.z)>0){
                            Instantiate(zombiePrefab, hit.point, Quaternion.identity);
                            if(stock[2]==MaxStock3){
                                stock[2]=stock[2]-1;
                                count3=0;
                            }
                            else{
                                stock[2]=stock[2]-1;
                            }
                        }
                    }
                }
            }
        }
        //ボタン4の処理
        if(state==44){
            if (check==zombieSpawnMode&&stock[3]!=0){
                if (isZombieSpawnMode && Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = godCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if(IsOnButton(Input.mousePosition)>0 && IsOkSpawn(hit.point.x,hit.point.z)>0){
                            Instantiate(zombiePrefab, hit.point, Quaternion.identity);
                            if(stock[3]==MaxStock4){
                                stock[3]=stock[3]-1;
                                count4=0;
                            }
                            else{
                                stock[3]=stock[3]-1;
                            }
                        }
                    }
                }
            }
        }
        countTime +=Time.deltaTime;
        
    }

    
    /* 与えられたスクリーン座標がボタンの上にあるかどうかを返す */
    private int IsOnButton(Vector3 screenMousePosition){
        if(screenMousePosition.x >= screenButtonPosition1.x && screenMousePosition.x <= screenButtonPosition1.x+160){
            if(screenMousePosition.y >= screenButtonPosition1.y && screenMousePosition.y <= screenButtonPosition1.y+30){
                return -1;
            }
        }
        if(screenMousePosition.x >= screenButtonPosition2.x && screenMousePosition.x <= screenButtonPosition2.x+160){
            if(screenMousePosition.y >= screenButtonPosition2.y && screenMousePosition.y <= screenButtonPosition2.y+30){
                return -1;
            }
        }
        if(screenMousePosition.x >= screenButtonPosition3.x && screenMousePosition.x <= screenButtonPosition3.x+160){
            if(screenMousePosition.y >= screenButtonPosition3.y && screenMousePosition.y <= screenButtonPosition3.y+30){
                return -1;
            }
        }
        if(screenMousePosition.x >= screenButtonPosition4.x && screenMousePosition.x <= screenButtonPosition4.x+160){
            if(screenMousePosition.y >= screenButtonPosition4.y && screenMousePosition.y <= screenButtonPosition4.y+30){
                return -1;
            }
        }
        
        return 1;
    }

    int IsOkSpawn(float x,float z){
        if(x> max_x || x<min_x){
            Debug.Log("Cannot spawn : out of field");
            return -1;
        }
        else if(z > max_z || z < min_z){
            Debug.Log("Cannot spawn : out of field");
            return -1;
        }

        return 1;
    }
}
