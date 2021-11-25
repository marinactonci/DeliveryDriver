using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
    // Start is called before the first frame update

    [SerializeField] float steerSpeed = 300f;
    [SerializeField] float moveSpeed = 15f;

    void Start() {
        //transform.Rotate( 0, 0, 45 );
    }

    // Update is called once per frame
    void Update() {
        float steerAmount = Input.GetAxis("Horizontal") * steerSpeed * Time.deltaTime;
        float moveAmount = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Rotate(0, 0, -steerAmount);
        transform.Translate(0, moveAmount, 0);
    }
}
