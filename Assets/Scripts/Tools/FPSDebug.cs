using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSDebug : MonoBehaviour
{

    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    public float UpdateInterval = 1F;

    private float _accum = 0; // FPS accumulated over the interval
    private int _frames = 0; // Frames drawn over the interval
    private Text _label;
    private float _fps;


    void Awake()
    {
        Application.targetFrameRate = 60;


        _label = GetComponent<Text>();
        StartCoroutine(UpdateLabel());
    }

    IEnumerator UpdateLabel()
    {
        while (true)
        {
            _label.color = _fps < 10 ? Color.red : _fps < 29.9 ? Color.yellow : Color.green;
            _label.text = string.Format("FPS: {0:F1}", _fps);
            _accum = 0.0F;
            _frames = 0;
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    void Update()
    {
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;

        // display two fractional digits (f2 format)
        _fps = _accum / _frames;
    }
}