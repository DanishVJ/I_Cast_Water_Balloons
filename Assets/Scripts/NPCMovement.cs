using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private AudioClip personScreamSound;
    [SerializeField] private AudioClip wheelchairScreamSound;
    [SerializeField] private Sprite stunnedSprite;
    
    private Transform _targetPosition;
    private float _currentSpeed;
    private bool _isDrenched = false;
    private bool _isStalled = false;
    private bool _isWheelchair = false;
    
    private SpriteRenderer _spriteRenderer;
    private Sprite _originalSprite;
    private Animator _animator;

    void Start()
    {
        _currentSpeed = walkSpeed;
        _isWheelchair = CompareTag("Wheelchair");;
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        if (_spriteRenderer != null)
        {
            _originalSprite = _spriteRenderer.sprite;
        }
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
        
        if (_targetPosition.position.x < transform.position.x)
        {
            Vector3 flippedScale = transform.localScale;
            flippedScale.x = -Mathf.Abs(flippedScale.x);
            transform.localScale = flippedScale;
        }
        else
        {
            Vector3 normalScale = transform.localScale;
            normalScale.x = Mathf.Abs(normalScale.x);
            transform.localScale = normalScale;
        }
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
  
        if (_animator != null)
        {
            _animator.enabled = false;
        }

        if (_spriteRenderer != null && stunnedSprite != null)
        {
            _spriteRenderer.sprite = stunnedSprite;
        }
        
        if (!_isWheelchair && personScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(personScreamSound, transform.position);
        }
        
        if (_isWheelchair && wheelchairScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(wheelchairScreamSound, transform.position);
        }
        
        yield return new WaitForSeconds(1f);
        
        _isStalled = false;
        
        if (_animator != null)
        {
            _animator.enabled = true;
        }
        
        if (_isWheelchair)
        {
            _currentSpeed = walkSpeed;
            
            GameManager.Instance?.IncreaseNuisance();
            
            Debug.Log("Hit wheelchair! Adding to nuisance meter.");
        }
        else
        {
            _currentSpeed = runSpeed;
            Debug.Log("Pedestrian is screaming and running away!");
        }
    }
}
