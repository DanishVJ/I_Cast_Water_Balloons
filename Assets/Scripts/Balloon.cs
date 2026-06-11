using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
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
            if (other.CompareTag("Ground"))
            {
                Debug.Log("The balloon splatted on the ground!");
                // TODO: Change sprite to exploded form here

                _playerController?.BalloonCollided();

                // Destroy this balloon clone immediately
                Destroy(gameObject);
            }
        }
       
}
