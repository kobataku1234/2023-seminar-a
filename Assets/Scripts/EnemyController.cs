using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

  [SerializeField]
  Animator animator = null;
  // アニメーションをコントロールするためのインターフェース
  [SerializeField]
  UnityEngine.AI.NavMeshAgent navMeshAgent = null;
  // 移動制御の情報
  [SerializeField]
  Transform target = null;
  // ターゲットの位置、回転、スケール
  [SerializeField]
  CapsuleCollider capsuleCollider = null;
  // カプセル状のコライダー
  // コライダーは衝突判定のクラス、リジットボディは物理特性のクラス
  [SerializeField, Min(0)]
  int maxHp = 3;
  // ゾンビの体力変数
  [SerializeField]
  float deadWaitTime = 3;
  [SerializeField]
  float chaseDistance = 5;
  // 索敵距離
  [SerializeField]
  Collider attackCollider = null;
  [SerializeField]
  int attackPower = 10;
  // 攻撃力
  [SerializeField]
  float attackTime = 0.5f;
  // 攻撃に必要な時間
  [SerializeField]
  float attackInterval = 2;
  // 攻撃間隔
  [SerializeField]
  float attackDistance = 2;
  // 攻撃可能距離
  [SerializeField]
  int qtyOfHouses = 6;
  // 家の数
  //[SerializeField]
  //float attackHouseDistance = 1;
  // 家攻撃距離
  [SerializeField]
  float chaseHouseDistance = 15;
  // 家索敵距離
  [SerializeField]
  int attackHousePower = 10;


  // アニメーターのパラメーターのIDを取得（高速化のため）
  readonly int SpeedHash = Animator.StringToHash("Speed");
  readonly int AttackHash = Animator.StringToHash("Attack");
  readonly int DeadHash = Animator.StringToHash("Dead");

  bool isDead = false;
  // 死んでいるかを表す変数
  int hp = 0;
  // ゾンビの体力を表す変数
  Transform thisTransform;
  // 
  bool isAttacking = false;
  // 攻撃中かどうかを表す変数
  Transform player;
  // プレイヤーのtransform
  Transform defaultTarget;
  // 初期ターゲットのtransform
  WaitForSeconds attackWait;
  // コルーチンの実行を待つための時間
  WaitForSeconds attackIntervalWait;
  //
  GameObject[] houses;
  // houseタグをもつオブジェクトのtransformリスト
  
  
  /* プロパティ Hp */
  public int Hp{
    /* フィールドとして代入された場合の処理 */
    set{
      hp = Mathf.Clamp(value, 0, maxHp);
      // value : 代入された値
      // hpにはvalueを0からmaxHpの間に丸められた数が格納される
    }
    /* 参照された場合になにをreturnするかの処理 */
    get{
      return hp;
    }
  }

  /* プロパティ target */
  public Transform Target{
    set{
      target = value;
    }
    get{
      return target;
    }
  }

  /* 最初に行う処理 */
  void Start(){
    thisTransform = transform;  
    // transformをキャッシュ（高速化）
    // transform = this.transform (thisが省略されているということ)
    defaultTarget = target;
    // あらかじめ設定されているtargetがあれば格納
    attackWait = new WaitForSeconds(attackTime);
    // 指定された攻撃に要する時間からコルーチンで使用するためのインスタンスを作成して格納
    // WaitForSecondsはコルーチンに使用する型
    attackIntervalWait = new WaitForSeconds(attackInterval);
    // 指定された攻撃インターバルからコルーチンで使用するためのインスタンスを作成して格納
    InitEnemy();  
    // 初期化処理を行う
    houses = new GameObject[qtyOfHouses];
    // 配列の初期化
  }

  /* ループ処理 */
  void Update(){
    player = GameObject.FindGameObjectWithTag("Player").transform;
    // playerタグがついたオブジェクトのtransformを格納

    /* houseタグがついたオブジェクトを格納 */
    int i = 0;
    foreach (GameObject house in GameObject.FindGameObjectsWithTag("House"))
    {
      houses[i] = house;
      i++;
    }

    /* 死んでいるとき */
    if (isDead){
      return;
    }

    CheckDistance();

    /* CheckDistanceの結果、攻撃中になれば移動を停止 */
    if (isAttacking){
      navMeshAgent.isStopped = true; // Stop moving when attacking
    }
    else{
      Move();
    }

    UpdateAnimator();
  }

  /* 初期化処理 */
  void InitEnemy(){
    Hp = maxHp;
    // 指定したmaxHpをプロパティに反映
  }

  // 被ダメージ処理
  public void Damage(int value)
  {
    if(value <= 0)
    {
      return;
    }

    Hp -= value;

    if(Hp <= 0)
    {
      Dead();
    }
  }

  // 死亡時の処理
  void Dead()
  {
    isDead = true;
    capsuleCollider.enabled = false;
    animator.SetBool(DeadHash, true);

    StartCoroutine(nameof(DeadTimer));
  }

  // 死亡してから数秒間待つ処理
  IEnumerator DeadTimer(){
    yield return new WaitForSeconds(deadWaitTime);

    Destroy(gameObject);
  }

  // 移動処理
  void Move(){
    if(target!=null){
      navMeshAgent.SetDestination(target.position);
    }
    else{
      navMeshAgent.SetDestination(new Vector3(this.transform.position.x,0,30));
    }
  }

  // アニメーターのアップデート処理
  void UpdateAnimator()
  {
    animator.SetFloat(SpeedHash, navMeshAgent.desiredVelocity.magnitude);
  }

  // 距離をチェックして攻撃するかどうかを決める処理
  void CheckDistance(){
    float diff = (player.position - thisTransform.position).sqrMagnitude;
    // playerとzombieの差分ベクトルの長さの2乗を格納

    float diffh = 0;
    // それぞれとの家との差分ベクトルの長さの2乗配列

    /* 最も近い家との差分ベクトルの長さの2乗を計算 */
    float diffTemp;
    // 比較用変数
    GameObject nearhouse = null;
    // ゾンビから最も近い家のGameObject

    foreach(GameObject house in houses){
      if(house != null){
        diffTemp = (house.transform.position - thisTransform.position).sqrMagnitude;
        if(diffh == 0){
          diffh = diffTemp;
          nearhouse = house;
        }
        else{
          if(diffh > diffTemp) {
            diffh = diffTemp;
            nearhouse = house;
          }
        }
      }
    }

    float nearHouseRadius = nearhouse.GetComponent<CapsuleCollider>().radius;
    // 最も近い家のCapsuleCollier

    /* 家が家攻撃距離内にあるとき */
    if(diffh < nearHouseRadius * nearHouseRadius){
      Debug.Log($"Zombie is attacking house {nearHouseRadius}");
      if(!isAttacking){
        StartCoroutine(nameof(Attack));
      }
    }
    /* 攻撃距離より小さいとき */
    else if (diff < attackDistance * attackDistance){
      Debug.Log("Zombie is attacking player");
      /* 攻撃中でないとき */
      if (!isAttacking){
        StartCoroutine(nameof(Attack));
        // Attack()をコルーチンで呼び出す
      }
    }
    /* 家が家索敵距離内にあるとき */
    else if(diffh < chaseHouseDistance * chaseHouseDistance){
      Debug.Log("Zombie is chasing house");
      target = nearhouse.transform;
      isAttacking = false;
      navMeshAgent.isStopped = false;
    }
    /* 攻撃距離より大きいけれど索敵距離より小さいとき */
    else if (diff < chaseDistance * chaseDistance)
    {
      Debug.Log("Zombie is chasing player");
      target = player;
      isAttacking = false; // Stop attacking when the player is out of attack range
      navMeshAgent.isStopped = false;
    }
    /* 索敵距離内にplayerがいないとき */
    else
    {
      target = defaultTarget;
      isAttacking = false; // Stop attacking when the player is out of chase range
      navMeshAgent.isStopped = false;
    }


  }

  // 攻撃処理
  IEnumerator Attack()
  {
    isAttacking = true;
    animator.SetTrigger(AttackHash);
    attackCollider.enabled = true;
    yield return attackWait;
    attackCollider.enabled = false;
    yield return attackIntervalWait;
    isAttacking = false;
  }

  // 攻撃を止める処理
  void StopAttack()
  {
    StopCoroutine(nameof(Attack));
    attackCollider.enabled = false;
    isAttacking = false;
  }

  /* 攻撃撃判定に当たった時の処理 */
  private void OnTriggerEnter(Collider other)
  {
    // otherに格納されているのは接触した相手のコライダー

    /* 接触相手がplayerのとき */
    if (other.gameObject.CompareTag("Player"))
    {
      FirstPersonMovement player = other.gameObject.GetComponent<FirstPersonMovement>();
      if(player != null){
          player.Damage(attackPower);
      }
    } 
    else if(other.gameObject.CompareTag("House")){
      Debug.Log("Zomibe dameges House.");
      HouseController house = other.gameObject.GetComponent<HouseController>();
      if(house != null){
        house.Damage(attackHousePower);
      }
    }
    // else if (other.gameObject.CompareTag("Fence"))
    // {
    //   Debug.Log("Zomibe dameges Fence.");
    //   // ゾンビの現在の向き（角度）を取得
    //   float currentAngle = transform.rotation.eulerAngles.y;

    //   // 逆方向の角度を計算（180度を加算または減算）
    //   float oppositeAngle = (currentAngle > 180) ? currentAngle - 180 : currentAngle + 180;

    //   // 逆方向にランダムな角度を計算（例えば、逆方向の角度±45度の範囲でランダムな角度を生成）
    //   float randomAngle = Random.Range(oppositeAngle - 45, oppositeAngle + 45);

    //   // ゾンビをランダムな角度で回転させる
    //   transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
    // }
    
  }
}