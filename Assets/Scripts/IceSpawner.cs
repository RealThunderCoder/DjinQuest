using Oculus.Interaction;
using UnityEngine;

public class IceSpawner : MonoBehaviour
{
    public static IceSpawner Instance { get; private set; }
    [SerializeField] private PlayerInputHandler _input;

    [Header("Ice Block Spawning")]
    private Vector3 _spawnLocation; // FOR TESTING ONLY, change to controller pos later
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
        if (_previewGO && _input.rayInteractor.CollisionInfo.HasValue)
        {
            Vector3 cursorPos = _input.rayInteractor.CollisionInfo.Value.Point;
            _spawnLocation = new Vector3(cursorPos.x, cursorPos.y, cursorPos.z);
            _previewGO.transform.SetPositionAndRotation(_spawnLocation, Quaternion.identity);
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
        GameObject iceInstance = Instantiate(prefabToSpawn, _spawnLocation, Quaternion.identity);
        iceInstance.GetComponent<Animator>().SetTrigger("SummonIce");
    }

    private void Preview(IceType type)
    {
        if (type != _currentPreview || _previewGO == null)
        {
            _currentPreview = type;
            switch (type)
            {
                case IceType.Vertical:
                    _previewGO = Instantiate(_iceBlockPrefabs[0], _spawnLocation, Quaternion.identity);
                    break;
            }
            // Make preview transparent and non-collidable
            UpdatePreviewColor();
            foreach (Collider col in _previewGO.GetComponentsInChildren<Collider>())
            {
                if (col) col.enabled = false;
            }
        }
    }

    private void UpdatePreviewColor()
    {
        if (_previewGO == null) return;
        _previewMatInstance.color = CanPlace() ? Color.green : Color.red;
        foreach (Renderer render in _previewGO.GetComponentsInChildren<Renderer>())
        {
            var mats = render.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = _previewMatInstance;
            }
            render.materials = mats;
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
            if (_previewGO != null) Destroy(_previewGO);
        }
    }

    public void SpawnCurrentIce()
    {
        if (!CanPlace() || !_isPreviewing || _input.tipVelocity.y < -_input.spawnVelocityThreshold) return;
        SpawnIce(_currentType);
        AddSpawnCount(1);
    }

    public void AddSpawnCount(int amount)
    {
        _spawnCount += amount;
        if (_spawnCount < 0) _spawnCount = 0;
        UpdatePreviewColor();
    }

    private bool CanPlace()
    {
        return _spawnCount < _spawnLimit;
    }
}
