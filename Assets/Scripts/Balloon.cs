using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private GameObject splashParticlePrefab;
    
    private bool _hasPopped = false;
    private bool _isReleased = false;
    private PlayerController _playerController;

    void Update()
    {
        if (_isReleased)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
    }

    public void Release(PlayerController playerController)
    {
        _isReleased = true;
        _playerController = playerController;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isReleased) return;
        
        if (_hasPopped) return;
        
        _hasPopped = true;
        
        Debug.Log("The balloon splatted!");
       
        if (splashParticlePrefab != null)
        {
            GameObject splashEffect = Instantiate(splashParticlePrefab, transform.position, Quaternion.identity);
            
            Destroy(splashEffect, 1f);
        }
        
        _playerController?.BalloonCollided();
        
        if (other.CompareTag("Person"))
        {
            NPCMovement npc = other.GetComponent<NPCMovement>();
            if (npc != null)
            {
                npc.GetDrenched();
            }

            // TODO: Tell score system to add points
        }
        
        else if (other.CompareTag("Wheelchair"))
        {
            NPCMovement wheelchair = other.GetComponent<NPCMovement>();
            if (wheelchair != null)
            {
                wheelchair.GetDrenched();
            }
        }

        Destroy(gameObject);
    }
       
}
