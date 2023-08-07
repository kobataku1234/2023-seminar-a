using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseController : MonoBehaviour
{

    [SerializeField]
    int MaxHp = 100;
    [SerializeField]
    int Hp;
    [SerializeField]
    GameObject DestroyEffect;
    [SerializeField]
    GameObject HPUI;
    [SerializeField]
    Slider HPSlider;

    void Start(){
        //Break();
        Hp = MaxHp;
    }

    void FixedUpdate()
    {
        //自身のHPゲージを更新する処理
        HPSlider.value = (float)Hp / (float)MaxHp;
    }

    public void Damage(int value)
    {
        if(value <= 0)
        {
            return;
        }
    
        Hp -= value;
        Debug.Log(Hp);

        if(Hp <= 0)
        {
            Break();
        }
    }

    void Break() {
        //壊れるエフェクトを入れる
        Instantiate(DestroyEffect, this.transform.position, DestroyEffect.transform.rotation);
        
        //オブジェクトを削除する
        Destroy(this.gameObject);

        //HPゲージを非表示にする
        HPUI.SetActive(false);
    }
}
