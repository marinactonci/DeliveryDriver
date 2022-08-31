using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    void Awake() {
        GameObject[] musicObject = GameObject.FindGameObjectsWithTag("Music");
        if (musicObject.Length > 1) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
