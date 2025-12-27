using Oculus.Interaction;
using UnityEngine;

public class IceSpawner : MonoBehaviour
{
    public static IceSpawner Instance { get; private set; }
    [Header("Input")]
    [SerializeField] private PlayerInputHandler _input;
    [SerializeField] private Grid _grid;
    [Header("Movement Object")]
    [SerializeField] public MoveYController moveController;
    [Header("Ice Block Spawning")]
    private Vector3 _spawnLocation;
    private GridData _mainGridData;
    [SerializeField] private BuildableObjectDataSO _currentType;
    [SerializeField] private int _spawnLimit = 1;
    private int _spawnCount;

    [Header("Preview References")]
    [SerializeField] private Material _previewMaterial;
    private Material _previewMatInstance;
    private bool _isPreviewing = false;
    private bool _isPlacing = false;
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
        _grid = GameObject.FindWithTag("MainMap").GetComponent<Grid>();
        _mainGridData = new GridData();
    }

    private void Update()
    {
        PreviewCheck();
        PlacingCheck();
    }

    private void PreviewCheck()
    {
        if (_previewGO)
        {
            // Display preview in game
            Vector3Int cellLocation = _grid.WorldToCell(_input.GetSelectedPosition());
            Vector3Int offset = new Vector3Int(_currentType.Offset.x, 0, _currentType.Offset.y);
            _spawnLocation = _grid.CellToWorld(cellLocation) + offset;
            _previewGO.transform.SetPositionAndRotation(_spawnLocation, Quaternion.identity);
            UpdatePreviewColor();
        }
    }

    private void PlacingCheck()
    {
        if (!CanPlace() || _input.fingerVelocity.y < _input.spawnVelocityThreshold) return;
        SpawnIce(_currentType);
        AddSpawnCount(1);
    }

    private void SpawnIce(BuildableObjectDataSO type)
    {
        GameObject iceInstance = Instantiate(type.Prefab, _spawnLocation, Quaternion.identity);
        _mainGridData.AddObject(_spawnLocation, _currentType);
        iceInstance.GetComponent<Animator>().SetTrigger("SummonIce");
        
        //  MOVE THE OTHER OBJECT UP
        if (moveController != null)
            moveController.MoveUp();
        }

    private void Preview(BuildableObjectDataSO type)
    {
        if (_previewGO == null)
        {
            _previewGO = Instantiate(type.Prefab, _spawnLocation, Quaternion.identity);
            UpdatePreviewColor();
            // Make preview transparent and non-collidable
            foreach (Collider col in _previewGO.GetComponentsInChildren<Collider>())
            {
                if (col) col.enabled = false;
            }
        }
    }

    private void UpdatePreviewColor()
    {
        if (_previewGO == null) return;
        Debug.Log($"Limit OK: {SpawnLimitCheck()}, Space OK: {_mainGridData.CanPlaceAt(_spawnLocation, _currentType.Size)}");
        _previewMatInstance.color = (SpawnLimitCheck() &&
            _mainGridData.CanPlaceAt(_spawnLocation, _currentType.Size))
            ? Color.green : Color.red;

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

    public void ToggleSpawning()
    {
        if (!_isPlacing)
        {
            Debug.Log("Placing enabled");
            _isPlacing = true;
        }
        else
        {
            Debug.Log("Placing disabled");
            _isPlacing = false;
        }
    }

    public void AddSpawnCount(int amount)
    {
        _spawnCount += amount;
        if (_spawnCount < 0) _spawnCount = 0;
        UpdatePreviewColor();
    }

    public void RemoveSpawnData(Vector3 position, BuildableObjectDataSO data)
    {
        _mainGridData.RemoveObject(position, data);
    }

    private bool CanPlace()
    {
        return (_isPlacing && SpawnLimitCheck() && _isPreviewing &&
            _mainGridData.CanPlaceAt(_spawnLocation, _currentType.Size));
    }

    private bool SpawnLimitCheck()
    {
        return _spawnCount < _spawnLimit;
    }
}
