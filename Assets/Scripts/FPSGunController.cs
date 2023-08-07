using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSGunController : MonoBehaviour
{

  [SerializeField]
  Transform bulletSpawn = null; // 弾の発射位置
  [SerializeField, Min(1)]
  int damage = 1; // 弾のダメージ
  [SerializeField, Min(1)]
  int maxBullet = 15; // 弾の最大所持数
  [SerializeField, Min(1)]
  float maxRange = 30; // 弾の最大射程距離
  [SerializeField]
  LayerMask hitLayers = 0; // 弾が当たるレイヤー
  [SerializeField, Min(0.01f)]
  float fireInterval = 0.5f; // 弾の発射間隔
  [SerializeField, Min(1)]
  float reloadTime = 2; // リロードにかかる時間
  [SerializeField]
  ParticleSystem muzzleFlashParticle = null; // 銃口のパーティクル
  [SerializeField]
  GameObject bulletHitEffectPrefab = null; // 弾がヒットしたときのエフェクト
  [SerializeField]
  TMPro.TextMeshProUGUI currentAmmoText = null; // 弾の残弾数を表示するテキスト
  [SerializeField]
  float aimSensitivity = 0.5f; // 照準時の感度
  [SerializeField]
  Transform normalPosition; // 通常時の位置
  [SerializeField]
  Transform aimPosition; // 照準時の位置
  [SerializeField]
  float crosshairDistance = 10f; // 照準の距離
  [SerializeField]
  Transform crosshair; // 照準の位置
  [SerializeField]
  TMPro.TextMeshProUGUI countdownText = null; // カウントダウンを表示するテキスト
  [SerializeField]
  float countdownTime = 180f; // カウントダウンの時間
  [SerializeField]
  GameObject hitMarkerPrefab = null; // ヒットマーカーのプレハブ
  [SerializeField]
  TMPro.TextMeshProUGUI GameOver = null; // ゲームオーバーを表示するテキスト
  [SerializeField]
  TMPro.TextMeshProUGUI Victory = null; // ゲームクリアを表示するテキスト
  
  [SerializeField, Min(1)]
  int lv2KillCount = 10; // Lv2になるためのキル数
  [SerializeField, Min(1)]
  int lv3KillCount = 20; // Lv3になるためのキル数
  [SerializeField, Min(1)]
  int lv4KillCount = 30; // Lv4になるためのキル数

  int killCount = 0; // 現在のキル数

  [SerializeField]
  TMPro.TextMeshProUGUI currentLevelText = null; // 現在のレベルを表示するテキスト
  [SerializeField]
  TMPro.TextMeshProUGUI killCountText = null; // 現在のキル数と次のレベルへのキル数を表示するテキスト


  bool fireTimerIsActive = false; // 弾の発射間隔を制御するタイマーが動いているかどうか
  RaycastHit hit; // 弾がヒットしたときの情報を格納する変数
  WaitForSeconds fireIntervalWait; // 弾の発射間隔を制御するタイマー
  int currentAmmo = 0;  // 弾の現在の所持数
  bool isReloading = false; // リロード中かどうか
  bool isAiming = false; // 照準中かどうか

  void Start()
  {
    fireIntervalWait = new WaitForSeconds(fireInterval);  // WaitForSecondsをキャッシュしておく（高速化）
    currentAmmo = maxBullet;
    StartCoroutine(nameof(CountdownTimer));
  }
  void Update()
  {
    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.8f) // Right trigger is pressed
    {
        Fire();
    }
    if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) || currentAmmo <= 0) // Button B is pressed
    {
      Reload();
    }
    // if (Input.GetMouseButtonDown(1))  // 右クリックが押されたら照準
    // {
    //   isAiming = !isAiming; 
    // }

    currentAmmoText.text = isReloading ? "Reloading" : currentAmmo.ToString();   
    transform.localPosition = Vector3.Lerp(transform.localPosition, isAiming ? aimPosition.localPosition : normalPosition.localPosition, Time.deltaTime * 10f);
  
    // クロスヘアの位置を更新
    RaycastHit crosshairHit;
    if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out crosshairHit, crosshairDistance))
    {
        // Raycastが何かに当たった場合、その位置にクロスヘアを移動
        crosshair.position = crosshairHit.point;
    }
    else
    {
        // Raycastが何も当たらなかった場合、最大距離にクロスヘアを配置
        crosshair.position = bulletSpawn.position + bulletSpawn.forward * crosshairDistance;
    }

    UpdateUI();

  }

  // 弾の発射処理
  void Fire()
  {
    if (fireTimerIsActive || currentAmmo <= 0 || isReloading)
    {
      return;
    }

    muzzleFlashParticle.Play(); // 銃口のパーティクルを再生
    currentAmmo--;

    if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit, maxRange, hitLayers, QueryTriggerInteraction.Ignore)) // 弾が当たったかどうか判定
    {
        BulletHit();
    }

    // Add haptic feedback
    OVRHapticsClip hapticsClip = new OVRHapticsClip(100);
    for (int i = 0; i < hapticsClip.Samples.Length; i++)
    {
        hapticsClip.WriteSample((byte)(Mathf.Sin(i * Mathf.PI * 2f / hapticsClip.Samples.Length) * byte.MaxValue));
    }
    OVRHaptics.RightChannel.Preempt(hapticsClip);

    StartCoroutine(nameof(FireTimer));
  }

  // リロード処理
  void Reload()
  {
    if (isReloading || currentAmmo >= maxBullet)
    {
      return;
    }

    isReloading = true;
    isAiming = false;
    StartCoroutine(nameof(ReloadTimer)); 
  }

  // 弾がヒットしたときの処理
  void BulletHit()
  {
    Instantiate(bulletHitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

    if (hit.collider.gameObject.CompareTag("Enemy")) // ヒットしたオブジェクトがEnemyタグを持っていたらダメージを与える
    {
      EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
      enemy.Damage(damage);
      // ヒットマーカーを表示
        hitMarkerPrefab.SetActive(true);
        StartCoroutine(HideHitMarker());
      // 敵が死んだときにキル数を増やす
      if (enemy.Hp <= 0)
      {
        killCount++;
        CheckLevelUp();
      }
    }   
  }

  void CheckLevelUp()
  {
    if (killCount >= lv4KillCount)
    {
      maxBullet = 60; // Lv4の弾数
      fireInterval = 0.5f; // Lv4の発射間隔
      reloadTime = 0.5f; // Lv4のリロード時間
    }
    else if (killCount >= lv3KillCount)
    {
      maxBullet = 45; // Lv3の弾数
      fireInterval = 1f; // Lv3の発射間隔
      reloadTime = 1f; // Lv3のリロード時間
    }
    else if (killCount >= lv2KillCount)
    {
      maxBullet = 30; // Lv2の弾数
      fireInterval = 1.25f; // Lv2の発射間隔
      reloadTime = 1.5f; // Lv2のリロード時間
    }
  }


  // 弾を発射する間隔を制御するタイマー
    IEnumerator FireTimer()
    {
      fireTimerIsActive = true;
      yield return fireIntervalWait;
      fireTimerIsActive = false;
    }

    // リロードする間隔を制御するタイマー
    IEnumerator ReloadTimer()
    {
      yield return new WaitForSeconds(reloadTime);
      currentAmmo = maxBullet;
      isReloading = false;
      Debug.Log("リロード完了");
    }

    IEnumerator HideHitMarker()
    {
        yield return new WaitForSeconds(0.1f); // 1秒後にヒットマーカーを非表示にする
        hitMarkerPrefab.SetActive(false);
    }

    // カウントダウンを制御するタイマー
    IEnumerator CountdownTimer()
    {
      while (countdownTime > 0)
      {
        yield return new WaitForSeconds(1f);
        countdownTime--;
        countdownText.text = countdownTime.ToString();
      }
      Victory.text = "You Win!";
      Time.timeScale = 0f;
    }

    void UpdateUI()
    {
    // レベルを更新
    if (killCount < lv2KillCount)
    {
      currentLevelText.text = "Level: 1";
    }
    else if (killCount < lv3KillCount)
    {
      currentLevelText.text = "Level: 2";
    }
    else if (killCount < lv4KillCount)
    {
      currentLevelText.text = "Level: 3";
    }
    else
    {
      currentLevelText.text = "Level: 4";
    }

    // キル数を更新
    int nextLevelKillCount = lv2KillCount;
    if (killCount >= lv2KillCount)
    {
      nextLevelKillCount = lv3KillCount;
    }
    if (killCount >= lv3KillCount)
    {
      nextLevelKillCount = lv4KillCount;
    }
    if (killCount >= lv4KillCount)
    {
      nextLevelKillCount = killCount;
    }
    killCountText.text = $"Kills: {killCount}/{nextLevelKillCount}";
    }
}



    