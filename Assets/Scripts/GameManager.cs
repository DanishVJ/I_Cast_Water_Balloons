using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image[] policeBeacons;
    [SerializeField] private Color beaconOffColor = Color.gray;
    [SerializeField] private Color beaconOnColor = Color.red;

    [Header("Police Car Cutscene")]
    [SerializeField] private GameObject policeCarPrefab;
    [SerializeField] private Transform policeSpawnPoint;
    [SerializeField] private Transform policeTargetPoint;
    [SerializeField] private float policeCarSpeed = 4f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sirenAudioSource;
    [SerializeField] private AudioClip beaconAlert;

    private int _score = 0;
    private int _nuisanceMeter = 0;
    private bool _isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (_isGameOver) return;

        _score += amount;
        UpdateUI();
    }

    public void IncreaseNuisance()
    {
        if (_isGameOver) return;

        _nuisanceMeter++;
        UpdateUI();
        
        if (beaconAlert != null)
        {
            AudioSource.PlayClipAtPoint(beaconAlert, transform.position);
        }
            

        if (_nuisanceMeter >= 3)
        {
            TriggerPoliceBust();
        }
    }

    private void UpdateUI()
    {
        // Update score text
        if (scoreText != null)
        {
            scoreText.text = "Score: " + _score;
        }

        // Update the 3 beacon states
        for (int i = 0; i < policeBeacons.Length; i++)
        {
            if (policeBeacons[i] != null)
            {
                policeBeacons[i].color = (i < _nuisanceMeter) ? beaconOnColor : beaconOffColor;
            }
        }
    }

    private void TriggerPoliceBust()
    {
        _isGameOver = true;
        
        // Find and stop the NPC Spawner from making more crowds
        NPCSpawner spawner = FindFirstObjectByType<NPCSpawner>();
        if (spawner != null)
            
            if (bgmAudioSource != null)
            {
                bgmAudioSource.Stop();
            }
            
        {
            spawner.StopSpawning();
        }

        // Play the siren sound if you have one attached
        if (sirenAudioSource != null)
        {
            sirenAudioSource.Play();
        }

        // Spawn the police car and start moving it
        if (policeCarPrefab != null && policeSpawnPoint != null && policeTargetPoint != null)
        {
            GameObject policeCar = Instantiate(policeCarPrefab, policeSpawnPoint.position, Quaternion.identity);
            StartCoroutine(MovePoliceCarRoutine(policeCar));
        }

        // Restart the game after 7 seconds (adjust time here!)
        StartCoroutine(RestartGameRoutine(5f));
    }

    private IEnumerator MovePoliceCarRoutine(GameObject car)
    {
        // Smoothly roll the police car to its stop point
        while (car != null && Vector3.Distance(car.transform.position, policeTargetPoint.position) > 0.05f)
        {
            car.transform.position = Vector3.MoveTowards(
                car.transform.position, 
                policeTargetPoint.position, 
                policeCarSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private IEnumerator RestartGameRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}