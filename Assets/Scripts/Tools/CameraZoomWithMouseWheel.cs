using UnityEngine;
using UniRx;

[RequireComponent(typeof(Camera))]
public class CameraZoomWithMouseWheel : MonoBehaviour
{
    #region [ Variables ]
    [SerializeField] private float scrollSpeed = 10;

    private new Camera camera;

    private CompositeDisposable disposable = new();

    private readonly int MAX_SIZE = 10;
    private readonly int MIN_SIZE = 5;
    #endregion

    #region [ MonoBehaviour Messages ]
    private void OnEnable()
    {
        RegisterUpate();
    }

    private void OnDisable()
    {
        disposable.Clear();
    }

    private void Awake()
    {
        camera = this.GetComponent<Camera>();
    }

    #endregion

    #region [ Register Methods ]
    private void RegisterUpate()
    {
        if (camera.orthographic)
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                var size = camera.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);
                camera.orthographicSize = Mathf.Clamp(size, MIN_SIZE, MAX_SIZE);
            }).AddTo(disposable);
        }
    }
    #endregion
}
