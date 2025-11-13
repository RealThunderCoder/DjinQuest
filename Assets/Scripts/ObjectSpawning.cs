using UnityEngine;

public class ObjectSpawning : MonoBehaviour
{
    [Header("Hand Reference")]
    [SerializeField] private OVRHand rightHand;

    [Header("Ice Block Spawning")]
    [SerializeField] private Transform spawnLocation; // FOR TESTING ONLY, change to controller pos later
    private enum IceType { Vertical };
    [SerializeField] private IceType _currentType = IceType.Vertical;
    [SerializeField] private GameObject[] _iceBlockPrefabs;

    [Header("Preview References")]
    [SerializeField] private Material _previewMaterial;
    private bool _isPreviewing = false;
    private IceType _currentPreview;
    private GameObject _previewGO;

    private void SpawnIce(IceType type)
    {
        GameObject prefabToSpawn = null;
        switch (type)
        {
            case IceType.Vertical:
                prefabToSpawn = _iceBlockPrefabs[0];
                break;
        }
        Instantiate(prefabToSpawn, spawnLocation.position, Quaternion.identity);
    }

    private void Preview(IceType type)
    {
        if (type != _currentPreview || _previewGO == null)
        {
            _currentPreview = type;
            switch (type)
            {
                case IceType.Vertical:
                    _previewGO = Instantiate(_iceBlockPrefabs[0], spawnLocation.position, Quaternion.identity);
                    break;
            }
            // Make preview transparent and non-collidable
            foreach (Renderer render in _previewGO.GetComponentsInChildren<Renderer>())
            {
                var mats = render.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = _previewMaterial;
                }
                render.materials = mats;
            }
            foreach (Collider col in _previewGO.GetComponentsInChildren<Collider>())
            {
                if (col) col.enabled = false;
            }
        }
        else
        {
            // TODO: move preview where player's pointing to
        }
    }

    public void TogglePreview()
    {
        if (!_isPreviewing)
        {
            Debug.Log("Preview enabled");
            _isPreviewing = true;
            Preview(_currentType);
        }
        else
        {
            Debug.Log("Preview disabled");
            _isPreviewing = false;
            Destroy(_previewGO);
        }
    }

    public void SpawnCurrentIce()
    {
        SpawnIce(_currentType);
    }
}
