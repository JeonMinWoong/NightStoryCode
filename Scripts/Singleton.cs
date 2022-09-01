using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T _Instance;

    public static T Instance
    {
        get
        {
            //싱글톤 인스턴스가 없으면 찾아라.
            if (_Instance == null)
                _Instance = FindObjectOfType<T>();
            return _Instance;
        }
    }

    //시작 시 싱글톤 인스턴스 초기화
    protected virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = (T)this;
            if(!this.GetComponent<Inventory>())
                DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        //유일한 인스턴스가 자신이었으면, 인스턴스 정적 변수를 비워줌 ( 다른 인스턴스가 들어갈 수 있도록)
        if (_Instance == this)
            _Instance = null;
    }

}
