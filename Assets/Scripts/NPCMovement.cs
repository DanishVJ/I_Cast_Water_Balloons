using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    
    private Transform _targetPosition;
    private float _currentSpeed;
    private bool _isDrenched = false;
    private bool _isStalled = false;
    private bool _isWheelchair = false;

    void Start()
    {
        _currentSpeed = walkSpeed;
        _isWheelchair = CompareTag("Wheelchair");;
    }
    
    void Update()
    {
        if (_targetPosition != null && !_isStalled)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition.position, _currentSpeed * Time.deltaTime);
            
            if (transform.position == _targetPosition.position)
            {
                Destroy(gameObject);
            }
        }
    }
    
    public void SetupNPC(Transform target)
    {
        _targetPosition = target;
    }
    
    public void GetDrenched()
    {
        if (_isDrenched) return;
        _isDrenched = true;

        StartCoroutine(DrenchedRoutine());
    }

    private IEnumerator DrenchedRoutine()
    {
        _isStalled = true;
        Debug.Log(gameObject.name + " is shocked and stopped moving!");
        
        yield return new WaitForSeconds(1f);
        
        _isStalled = false;
        
        if (_isWheelchair)
        {
            _currentSpeed = walkSpeed;
            
            // TODO: Tell GameManager to add to nuisance meter
            
            Debug.Log("Hit wheelchair! Adding to nuisance meter.");
        }
        else
        {
            _currentSpeed = runSpeed;
            Debug.Log("Pedestrian is screaming and running away!");
        }
    }
}
