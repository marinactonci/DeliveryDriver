using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Driver : MonoBehaviour
{
    // [SerializeField]
    // float steerSpeed = 300f;

    // [SerializeField]
    // float moveSpeed = 20f;

    [Header("Car settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 15.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20.0f;

    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    Rigidbody2D carRigidbody2D;

    [SerializeField]
    GameObject boost;

    [SerializeField]
    AudioClip clipBoost;

    float boostTimer;
    bool isBoosted;

    float time;
    bool isBoostSpawned = false;
    float spawnBoostTime;
    float minTime = 8;
    float maxTime = 20;

    public Collider2D[] colliders;

    [SerializeField]
    float radius;

    void Awake(){
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        boostTimer = 0;
        isBoosted = false;

        SetRandomTime();
        time = minTime;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnBoostTime && !isBoostSpawned)
        {
            SpawnBoost();
            SetRandomTime();
        }

        if (isBoosted)
        {
            boostTimer += Time.deltaTime;
            if (boostTimer >= 3)
            {
                accelerationFactor = 15.0f;
                maxSpeed = 20.0f;
                boostTimer = 0;
                isBoosted = false;
            }
        }

        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        SetInputVector(inputVector);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("End");
        }
    }

    void FixedUpdate() {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce(){
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        if(velocityVsUp > maxSpeed && accelerationInput > 0) {
            return;
        }

        if(velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0) {
            return;
        }

        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0) {
            return;
        }

        if(accelerationInput == 0) {
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else {
            carRigidbody2D.drag = 0;
        }
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering(){
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity() {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity() {
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool isTireScreething(out float lateralVelocity, out bool isBraking) {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if(accelerationInput < 0 && velocityVsUp > 0) {
            isBraking = true;
            return true;
        }

        if(Mathf.Abs(GetLateralVelocity()) > 4.0f) {
            return true;
        }

        return false;
    }

    public void SetInputVector(Vector2 inputVector) {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bush")
        {
            accelerationFactor = 5.0f;
            maxSpeed = 10.0f;
        }
        if (other.tag == "Boost")
        {
            isBoosted = true;
            isBoostSpawned = false;
            accelerationFactor = 25.0f;
            maxSpeed = 30.0f;
            AudioSource.PlayClipAtPoint(clipBoost, transform.position);
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Bush")
        {
            accelerationFactor = 15.0f;
            maxSpeed = 20.0f;
        }
    }

    void SpawnBoost()
    {
        time = 0;
        isBoostSpawned = true;
        bool canSpawnHere = false;
        Vector3 boostPosition = new Vector3(0, 0, 0);
        int safetyNet = 0;
        while (!canSpawnHere)
        {
            boostPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-20f, 20f), 0f);
            canSpawnHere = PreventSpawnOverlap(boostPosition);
            if (canSpawnHere)
            {
                break;
            }
            safetyNet++;
            if (safetyNet > 50)
            {
                break;
            }
        }
        GameObject newBoost = Instantiate(boost, boostPosition, Quaternion.identity) as GameObject;
    }

    void SetRandomTime()
    {
        spawnBoostTime = Random.Range(minTime, maxTime);
    }

    bool PreventSpawnOverlap(Vector3 position)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 centerPoint = colliders[i].bounds.center;
            float width = colliders[i].bounds.extents.x;
            float height = colliders[i].bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float lowerExtent = centerPoint.y - height;
            float upperExtent = centerPoint.y + height;

            if (position.x >= leftExtent && position.x <= rightExtent)
            {
                if (position.y >= lowerExtent && position.y <= upperExtent)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
