using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameLogic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI counterText;

    [SerializeField] GameObject package;

    [SerializeField] GameObject[] customers;

    [SerializeField] GameObject extraTime;

    [SerializeField] AudioClip clipPackage;
    [SerializeField] AudioClip clipCustomer;
    [SerializeField] AudioClip clipExtraTime;

    bool hasPackage;

    float mainTimer;
    float minTime = 8.0f;
    float maxTime = 25.0f;

    float timeExtraTime;
    float spawnExtraTimeTime;
    bool isExtraTimeSpawned = false;

    public Collider2D[] colliders;
    [SerializeField] float radius;

    public int counter = 0;

    void Start()
    {
        mainTimer = 25.0f;
        hasPackage = false;

        SetRandomTime();
        timeExtraTime = minTime;

        SpawnPackage();
        SpawnCustomer();
    }

    void Update()
    {
        int mainTimerInteger = (int)mainTimer;
        timerText.text = mainTimerInteger.ToString();
        counterText.text = counter.ToString();
        
        mainTimer -= Time.deltaTime;
        if (mainTimer <= 0)
        {
            mainTimer = 25f;
            SceneManager.LoadScene("GameOver");
        }

        timeExtraTime += Time.deltaTime;
        if (timeExtraTime >= spawnExtraTimeTime && !isExtraTimeSpawned)
        {
            SpawnExtraTime();
            SetRandomTime();
        }

        int getCurrentLevel = SceneManager.GetActiveScene().buildIndex;
        if(counter == 3 && getCurrentLevel == 2) {
            counter = 0;
            SceneManager.LoadScene("Level2");
        }
        if(counter == 6 && getCurrentLevel == 3) {
            counter = 0;
            SceneManager.LoadScene("Level3");
        }
        if(counter == 9 && getCurrentLevel == 4) {
            counter = 0;
            SceneManager.LoadScene("Victory");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Package" && !hasPackage)
        {
            Debug.Log("Package picked up!");
            hasPackage = true;
            mainTimer += 2f;
            AudioSource.PlayClipAtPoint(clipPackage, transform.position);
            Destroy(other.gameObject);
        }
        if (other.tag == "Customer")
        {
            if (hasPackage)
            {
                Debug.Log("Package delivered!");
                mainTimer += 4f;
                counter++;
                hasPackage = false;
                AudioSource.PlayClipAtPoint(clipCustomer, transform.position);
                Destroy(other.gameObject);
                SpawnPackage();
                SpawnCustomer();
            }
        }
        if (other.tag == "ExtraTime")
        {
            mainTimer += 5f;
            isExtraTimeSpawned = false;
            AudioSource.PlayClipAtPoint(clipExtraTime, transform.position);
            Destroy(other.gameObject);
        }
    }

    void SpawnPackage()
    {
        bool canSpawnHere = false;
        Vector3 packagePosition = new Vector3(0, 0, 0);
        int safetyNet = 0;
        while (!canSpawnHere)
        {
            packagePosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-20f, 20f), 0f);
            canSpawnHere = PreventSpawnOverlap(packagePosition);
            if (canSpawnHere) {
                break;
            }
            safetyNet++;
            if (safetyNet > 50)
            {
                break;
            }
        }
        GameObject newPackage = Instantiate(package, packagePosition, Quaternion.identity) as GameObject;
    }

    void SpawnCustomer()
    {
        bool canSpawnHere = false;
        Vector3 customerPosition = new Vector3(0, 0, 0);
        int safetyNet = 0;
        while (!canSpawnHere)
        {
            customerPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-20f, 20f), 0f);
            canSpawnHere = PreventSpawnOverlap(customerPosition);
            if (canSpawnHere) {
                break;
            }
            safetyNet++;
            if (safetyNet > 50)
            {
                break;
            }
        }
        GameObject newCustomer = Instantiate(customers[Random.Range(0, customers.Length)], customerPosition, Quaternion.identity) as GameObject;
    }

    void SpawnExtraTime()
    {
        timeExtraTime = 0;
        isExtraTimeSpawned = true;
        bool canSpawnHere = false;
        Vector3 extraTimePosition = new Vector3(0, 0, 0);
        int safetyNet = 0;
        while (!canSpawnHere)
        {
            extraTimePosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-20f, 20f), 0f);
            canSpawnHere = PreventSpawnOverlap(extraTimePosition);
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
        GameObject newExtraTime = Instantiate(extraTime, extraTimePosition, Quaternion.identity) as GameObject;
    }

    void SetRandomTime()
    {
        spawnExtraTimeTime = Random.Range(minTime, maxTime);
    }

    bool PreventSpawnOverlap(Vector3 position)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 centerpoint = colliders[i].bounds.center;
            float width = colliders[i].bounds.extents.x;
            float height = colliders[i].bounds.extents.y;

            float leftExtent = centerpoint.x - width;
            float rightExtent = centerpoint.x + width;
            float lowerExtent = centerpoint.y - height;
            float upperExtent = centerpoint.y + height;

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