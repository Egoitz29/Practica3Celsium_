using UnityEngine;
using TMPro;
using System.Collections;

public class LocalGPS : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI gpsText;

    [Header("Ajustes de actualización")]
    [Tooltip("Segundos entre refrescos de UI")]
    public float refreshInterval = 0.5f;

    private bool _running;
    private WaitForSeconds _wait;

    void Awake()
    {
        if (gpsText != null) gpsText.text = "Inicializando GPS…";
        _wait = new WaitForSeconds(Mathf.Max(0.1f, refreshInterval));
    }

    IEnumerator Start()
    {
        // 1) ¿El usuario tiene activada la localización?
        if (!Input.location.isEnabledByUser)
        {
            SetText("La localización del dispositivo está desactivada.\nActívala en Ajustes y reinicia la app.");
            yield break;
        }

        // 2) Iniciar servicio (precisión 10 m, refresco cada 5 m)
        Input.location.Start(10f, 5f);

        // 3) Esperar a que inicialice (timeout 20 s)
        float maxWait = 20f;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0f)
        {
            SetText("Inicializando GPS…");
            yield return new WaitForSeconds(1f);
            maxWait -= 1f;
        }

        if (Input.location.status == LocationServiceStatus.Failed || maxWait <= 0f)
        {
            SetText(" No se pudo obtener la localización.\nPrueba en exteriores y revisa permisos.");
            yield break;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            SetText($" Estado del GPS: {Input.location.status}");
            yield break;
        }

        _running = true;
        StartCoroutine(UpdateLoop());
    }

    private IEnumerator UpdateLoop()
    {
        while (_running)
        {
            var d = Input.location.lastData; // últimos datos disponibles

            // Ojo: la altitud puede ser imprecisa en interiores
            string msg =
                $" GPS ACTIVO\n\n" +
                $"Latitud:  {d.latitude:F6}\n" +
                $"Longitud: {d.longitude:F6}\n" +
                $"Altitud:  {d.altitude:F1} m\n" +
                $"Precisión horizontal: ±{d.horizontalAccuracy:F1} m\n" +
                $"Timestamp: {d.timestamp:F0}s\n" +
                $"\nEstado: {Input.location.status}";

            SetText(msg);

            yield return _wait;
        }
    }

    private void SetText(string s)
    {
        if (gpsText != null) gpsText.text = s;
    }

    void OnDisable()
    {
        // Buen hábito: apagar el servicio para ahorrar batería
        if (_running)
        {
            _running = false;
            Input.location.Stop();
        }
    }
}
