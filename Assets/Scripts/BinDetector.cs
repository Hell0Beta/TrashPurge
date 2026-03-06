using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;   
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Xml.Serialization;

public class BinDetector : MonoBehaviour
{
    public ModelAsset modelAsset;
    public ARRaycastManager arRaycastManager;
    public GameObject trackerObject;
    public float confidenceThreshold = 0.5f;
    ARCameraManager cameraManager;

    public GameObject binTriggerZonePrefab;
    BinTriggerZone activeTriggerZone;

    Model runtimeModel;
    Worker worker;
    bool isDetecting = false;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        try
        {
            worker = new Worker(runtimeModel, BackendType.GPUCompute);
        }
        catch 
        {
            Debug.LogWarning("GPUCompute not supported, falling back to CPU");
            worker = new Worker(runtimeModel, BackendType.CPU);
        }
        cameraManager = FindFirstObjectByType<ARCameraManager>();
        StartCoroutine(DetectLoop());
    }

    IEnumerator DetectLoop()
    {
        while (true)
        {

            yield return new WaitForSeconds(0.5f); // Adjust detection frequency as needed
            if (!isDetecting)
            {
                StartCoroutine(RunDetection());
            }
        }
    }

    IEnumerator RunDetection()
    {
        isDetecting = true;

        if (cameraManager.TryAcquireLatestCpuImage(out var cpuImage))
        {
            Tensor<float> input = new Tensor<float>(new TensorShape(1, 3, 640, 640));
            Debug.Log("Got camera frame: " + cpuImage.width + "x" + cpuImage.height);
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.RGBA32
            };

            Texture2D camTexture = new Texture2D(cpuImage.width, cpuImage.height);
            cpuImage.Convert(conversionParams, camTexture.GetRawTextureData<byte>());
            camTexture.Apply();
            cpuImage.Dispose();

            Texture2D resized = ResizeTexture(camTexture, 640, 640);
            TextureConverter.ToTensor(resized, input);
            Destroy(camTexture);
            Destroy(resized);

            worker.Schedule(input);
            yield return null;

            Tensor<float> output = worker.PeekOutput() as Tensor<float>;
            var result = output.ReadbackAndClone();
            ProcessDetections(result);

            input.Dispose();
            result.Dispose();
        }
        else
        {
            Debug.Log("No camera frame available");
        }
            isDetecting = false;
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    void ProcessDetections(Tensor<float> output)
    {
        float highestScore = 0f;
        int highestClass = 0;

        for (int i = 0; i < 8400; i++)
        {
            int classId = GetMaxClass(output, i);
            float confidence = output[0, 4, i];

            if (confidence > highestScore)
            {
                highestScore = confidence;
                highestClass = classId;
            }

            if (confidence > confidenceThreshold)
            {
                float cx = output[0, 0, i] / 640f;
                float cy = output[0, 1, i] / 640f;
                PlaceTrackerAtBin(cx, cy);
                Debug.Log($"Bin detected! Confidence: {confidence}");
                Debug.Log($"Bin center at: ({cx}, {cy})");
                break;
            }
        }

        Debug.Log($"Highest detection — Class: {highestClass} Score: {highestScore}");
    }

    int GetMaxClass(Tensor<float> output, int i)
    {
        int maxclass = 0;

        float maxval = 0f;
        
        for(int c = 5; c < 85; c++)
        {
            float val = output[0, c, i];
            if (val < maxval)
            {
                maxval = val; maxclass = c - 5;
            }
        }

        return maxclass;
    }

    void PlaceTrackerAtBin(float cx, float cy) 
    {
        //Vector2 screenPoint = new Vector2(cx * Screen.width, cy * Screen.height);
        Vector2 screenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Debug.Log("Placing tracker at screen point: " + screenPoint);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (arRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Raycast hit!");
            trackerObject.transform.position = hits[0].pose.position;
            Debug.Log("Tracker placed at: " + hits[0].pose.position);

            Vector3 binPosition = hits[0].pose.position;
            Vector3 triggerPosition = binPosition + Vector3.up * 0.3f;

            if (activeTriggerZone == null)
            {
                Debug.Log("Trigger zone spawned above bin");
                var zoneObj = Instantiate(binTriggerZonePrefab, triggerPosition, Quaternion.identity);
                activeTriggerZone = zoneObj.GetComponent<BinTriggerZone>();
                activeTriggerZone.OnObjectEntered += OnDisposalDetected;
            }
            else
            {
                
                activeTriggerZone.transform.position = triggerPosition;
                
            }
        }
        else
        {
            Debug.Log("Raycast missed!");
        }
        
    }

    void OnDisposalDetected()
    {
        Debug.Log("Disposal confirmed!");
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
