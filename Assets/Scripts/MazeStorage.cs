using UnityEngine;

public class MazeStorage : MonoBehaviour
{
    [SerializeField] private float _ySpawnOffset = 100;
    private GameObject _storedMaze;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mazebox") && _storedMaze == null)
        {
            _storedMaze = other.gameObject;

            // disable physics while stored
            if (_storedMaze.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
            }
            _storedMaze.SetActive(false);
        }
    }

    public void RetrieveMaze()
    {
        if (_storedMaze == null) return;

        _storedMaze.SetActive(true);

        _storedMaze.transform.position =
            transform.position - Vector3.up * _ySpawnOffset;

        if (_storedMaze.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }

        _storedMaze = null;
    }
}
