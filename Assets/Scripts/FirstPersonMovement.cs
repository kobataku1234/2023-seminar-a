using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true; // 
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    //hpを追加
    [SerializeField]
    public int Hp = 100;
    [SerializeField]
    TMPro.TextMeshProUGUI HP = null; // 弾の残弾数を表示するテキスト
    [SerializeField]
    TMPro.TextMeshProUGUI GameOver = null; // ゲームオーバーを表示するテキスト
    [SerializeField]
    TMPro.TextMeshProUGUI VictoryText = null; // 全ての家が破壊された時に表示するテキスト

    // void Awake()
    // {
    //     // Get the rigidbody on this.
    //     rigidbody = GetComponent<Rigidbody>();
    // }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        // IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        // float targetMovingSpeed = IsRunning ? runSpeed : speed;
        // if (speedOverrides.Count > 0)
        // {
        //     targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        // }

        // Get targetVelocity from input.
        // Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        // rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
        
        HP.text = Hp.ToString();
        if(Hp <= 30)
        {
          HP.color = Color.red;
        }
        else if(Hp <= 60)
        {
          HP.color = Color.yellow;
        }
        else
        {
          HP.color = Color.green;
        }

        /* すべての家が破壊されたか確認 */
        if(GameObject.FindGameObjectsWithTag("House").Length == 0){
            GameOver.text = "GameOver!";
            Time.timeScale = 0f;
        }
        
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

  public void Dead () {
    Hp = 0;
    HP.text = Hp.ToString();
    GameOver.text = "GameOver";
    Time.timeScale = 0f;

  }
}