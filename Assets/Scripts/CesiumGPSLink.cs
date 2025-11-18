using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;

public class CesiumGPSLink : MonoBehaviour
{
    public GameObject cubePrefab;
    private GameObject spawnedCube;
    public CesiumGeoreference georeference;
    public float extraHeight = 10.0f; // Para que el cubo no quede bajo el terreno

    void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Modo simulado: instanciando cubo en Madrid");
        SpawnCubeAt(40.4168, -3.7038, 650 + extraHeight);
#else
        Input.location.Start(10f, 5f);
        InvokeRepeating(nameof(CheckGPS), 1f, 1f);
#endif
    }

    void CheckGPS()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            var d = Input.location.lastData;
            Debug.Log($"GPS OK → Lat:{d.latitude:F6}, Lon:{d.longitude:F6}");
            SpawnCubeAt(d.latitude, d.longitude, d.altitude + extraHeight);
            CancelInvoke(nameof(CheckGPS));
        }
    }

    void SpawnCubeAt(double latitude, double longitude, double height)
    {
        if (spawnedCube == null)
        {
            spawnedCube = Instantiate(cubePrefab);

            var anchor = spawnedCube.AddComponent<CesiumGlobeAnchor>();
            anchor.latitude = latitude;
            anchor.longitude = longitude;
            anchor.height = height;
        }
    }
}

