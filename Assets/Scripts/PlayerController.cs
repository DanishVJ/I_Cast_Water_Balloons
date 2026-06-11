using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private Transform[] playerLocations;
    [SerializeField] private Transform balloonSpawnLocation;
    
    private GameInput _inputActions;
    private int _dropCounter;
    private Transform _currentPlayerLocation;
    private GameObject _currentActiveBalloon;

    void Awake()
    {
        _inputActions = new GameInput();
    }

    void Start()
    {
        SpawnBalloon();
        
        PlayerTeleporter();
    }
    
    void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.DropBalloon.performed += ctx => DropBalloon();
    }

    void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.Player.DropBalloon.performed -= ctx => DropBalloon();
    }

    private void PlayerTeleporter()
    {
        int newRandomIndex = UnityEngine.Random.Range(0, playerLocations.Length);
        _currentPlayerLocation = playerLocations[newRandomIndex];
        
        transform.position = _currentPlayerLocation.position;
    }

    private void SpawnBalloon()
    {
        _currentActiveBalloon = Instantiate(balloonPrefab, balloonSpawnLocation.position, Quaternion.identity);
        _currentActiveBalloon.transform.SetParent(balloonSpawnLocation);
    }
    
    private void DropBalloon()
    {
        if (_currentActiveBalloon == null) return;
        
        _currentActiveBalloon.transform.SetParent(null);
        
        _currentActiveBalloon.GetComponent<Balloon>()?.Release(this);
        
        _currentActiveBalloon = null;
    }

    public void BalloonCollided()
    {
        _dropCounter++;

        if (_dropCounter >= 5)
        {
            PlayerTeleporter();
            _dropCounter = 0;
        }
        
        SpawnBalloon();
    }
    
}
