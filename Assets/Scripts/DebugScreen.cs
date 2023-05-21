using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject hexTile;

    [SerializeField]
    private Text fps;

    private float _dt;

    [SerializeField]
    private GameObject gui;

    private void Start()
    {
#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        Destroy(this);
#endif
        Debug.Log("Debug enabled!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            gui.SetActive(!gui.activeSelf);

        _dt += (Time.deltaTime - _dt) * 0.1f;
        fps.text = $@"{Mathf.Ceil(1.0f / _dt)} fps
{Profiler.usedHeapSizeLong} / {Profiler.maxUsedMemory:e} o allocated by Unity
{Profiler.GetAllocatedMemoryForGraphicsDriver():e} allocated for graphics driver
{Profiler.GetMonoUsedSizeLong():e} / {Profiler.GetMonoHeapSizeLong():e} o allocated managed-memory
{Profiler.GetTempAllocatorSize():e} o temporary allocated
{Profiler.GetTotalReservedMemoryLong():e} o total reserved memory
";
    }
}