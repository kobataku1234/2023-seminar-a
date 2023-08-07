using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 3.0f;
    [SerializeField]
    Camera godCamera;
    [SerializeField]
    float zoomSpeed = 2.0f;

    [Header("Running")]
    public bool canRun = true; // 
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    // [SerializeField]
    // TMPro.TextMeshProUGUI GameOver = null;

    // [SerializeField]
    // TMPro.TextMeshProUGUI Victory = null;

    // [SerializeField]
    // float gameDuration = 180.0f; // Change this value to the desired game duration in seconds.
    // float timer;

    // [SerializeField]
    // FirstPersonMovement player;


    float ZoomMinBound = 30f;
    float ZoomMaxBound = 70f;

    // Start is called before the first frame update
    void Start()
    {
        // player = GetComponent<FirstPersonMovement>();
        // timer = gameDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // timer -= Time.deltaTime;
        // if (timer <= 0) {
        //     DeclareGameOver();
        // }
        
        // if (player.Hp <= 0) {
        //     DeclareVictory();
        // }
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        // IsRunningがtrueならrunSpeedを、falseならspeedを返す
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        //Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        GetComponent<Rigidbody>().velocity = transform.rotation * new Vector3(targetVelocity.x, GetComponent<Rigidbody>().velocity.y, targetVelocity.y);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        godCamera.fieldOfView -= scroll * speed;
        godCamera.fieldOfView = Mathf.Clamp(godCamera.fieldOfView, ZoomMinBound, ZoomMaxBound);
    }

    // void DeclareGameOver() {
    //     GameOver.text = "Game Over";
    //     Time.timeScale = 0f;
    // }

    // void DeclareVictory() {
    //     Victory.text = "You Win!";
    //     Time.timeScale = 0f;
    // }
}

