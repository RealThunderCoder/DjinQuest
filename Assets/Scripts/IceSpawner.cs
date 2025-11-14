using UnityEngine;

public class IceSpawner : MonoBehaviour
{
    public static IceSpawner Instance { get; private set; }

    [Header("Hand Reference")]
    [SerializeField] private OVRHand rightHand;

    [Header("Ice Block Spawning")]
    [SerializeField] private Transform spawnLocation; // FOR TESTING ONLY, change to controller pos later
    private enum IceType { Vertical };
    [SerializeField] private IceType _currentType = IceType.Vertical;
    [SerializeField] private GameObject[] _iceBlockPrefabs;
    [SerializeField] private int _spawnLimit = 1;
    private int _spawnCount;

    [Header("Preview References")]
    [SerializeField] private Material _previewMaterial;
    private Material _previewMatInstance;
    private bool _isPreviewing = false;
    private IceType _currentPreview;
    private GameObject _previewGO;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            this.enabled = false;
        }

        _previewMatInstance = new Material(_previewMaterial);
    }

    private void Update()
    {
        if (_previewGO)
        {
            // TODO: move preview where player's pointing to
        }
    }

    private void SpawnIce(IceType type)
    {
        GameObject prefabToSpawn = null;
        switch (type)
        {
            case IceType.Vertical:
                prefabToSpawn = _iceBlockPrefabs[0];
                break;
        }
        GameObject iceInstance = Instantiate(prefabToSpawn, spawnLocation.position, Quaternion.identity);
        iceInstance.GetComponent<Animator>().SetTrigger("SummonIce");
    }

    private void Preview(IceType type)
    {
        if (type != _currentPreview || _previewGO == null)
        {
            _currentPreview = type;
            _previewMatInstance.color = CanPlace()? Color.green : Color.red;
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
                    mats[i] = _previewMatInstance;
                }
                render.materials = mats;
            }
            foreach (Collider col in _previewGO.GetComponentsInChildren<Collider>())
            {
                if (col) col.enabled = false;
            }
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
        if (!CanPlace()) return;
        SpawnIce(_currentType);
        _spawnCount++;
    }

    public void AddSpawnCount(int amount)
    {
        _spawnCount += amount;
        if (_spawnCount < 0) _spawnCount = 0;
    }

    private bool CanPlace()
    {
        return _spawnCount < _spawnLimit;
    }
}
