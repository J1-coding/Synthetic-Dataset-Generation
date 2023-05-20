using System.IO;
using System.Collections;
using UnityEngine;

public class StreamRotationFromCSV : MonoBehaviour
{
    public GameObject objectToRotate;
    public Camera cameraToCapture;
    private int lineNumber = 0;

    void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "ypr_data.csv");
        StartCoroutine(ReadCSVAndRotateObject(filePath));
    }

    IEnumerator ReadCSVAndRotateObject(string filePath)
    {
        using (var sr = new StreamReader(filePath))
        {
            lineNumber = 0;

            // Skip the header line
            string line = sr.ReadLine();
            lineNumber++;

            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                float yaw = float.Parse(parts[0]);
                float pitch = float.Parse(parts[1]);
                float roll = float.Parse(parts[2]);

                objectToRotate.transform.rotation = Quaternion.Euler(pitch, yaw, roll);

                yield return new WaitForEndOfFrame();

                CaptureAndSaveImage(lineNumber);

                yield return null;

                lineNumber++;
            }
        }
    }

    void CaptureAndSaveImage(int lineNumber)
    {
        RenderTexture renderTexture = new RenderTexture(512, 512, 24);
        cameraToCapture.targetTexture = renderTexture;

        Texture2D renderResult = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, 512, 512);

        cameraToCapture.Render();

        RenderTexture.active = renderTexture;
        renderResult.ReadPixels(rect, 0, 0);

        byte[] byteArray = renderResult.EncodeToPNG();
        File.WriteAllBytes("NewDatasetProduced0/" + lineNumber + ".png", byteArray); // Replace with your output folder path.

        RenderTexture.active = null;
        cameraToCapture.targetTexture = null;
    }
}
