using UnityEngine;
using System;
using System.Collections;
using ETC.Platforms;

/// <summary>
/// An example lighting manager using the DMX class to interface with an Enttec DMX USB Pro.
/// Requires one RGB 3-channel fixture connected as device 1 (i.e. channels 1 through 3).
/// </summary>
/// <remarks>
/// Author: Bryan Maher (bm3n@andrew.cmu.edu) 26-Jan-2015
/// 
/// Feel free to use this example code as starting point for your own project.
/// </remarks>
public class FlowerLighting : MonoBehaviour
{
    /// <summary>
    /// Set this Editor property to the value of the DMX controller's COM port.
    /// <example>COM22</example>
    /// </summary>

    public static FlowerLighting instance;
    public Color currentColor;
	public GameObject[] flowers;
	public int flowerState;

    public Color colorTest;
    public string ComPort = "COM3";

    private bool activeDMX = true;

    /// <summary>
    /// Instance of the DMX class used to control the lights.
    /// </summary>
    private DMX dmx;

    
    public enum Position {
        FrontRight, FrontLeft, BackRight, BackLeft
    }


    private void Awake()
    {
        if (instance == null)
        {
			flowerState = 0;
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update() {
		int a = 0, b = 0, c = 0;
		if (flowers[0].GetComponent<Flower>().pollinationComplete)
			a = 1;
		if (flowers[1].GetComponent<Flower>().pollinationComplete)
			b = 1;
		if (flowers[2].GetComponent<Flower>().pollinationComplete)
			c = 1;
		flowerState = a+b+c;
		switch (flowerState) {
			case 1: ChangeAll(Color.yellow);
					break;
			case 2: ChangeAll(Color.red);
					break;		 
			case 3: ChangeAll(Color.magenta);
					break;
		}
    }


    // Use this for initialization
    private void Start()
    {
        try {
            this.dmx = new DMX(this.ComPort);
        } catch (Exception e) {
            activeDMX = false;
            Destroy(this.gameObject);
        }

        if (activeDMX) {
            this.SetColor(Color.cyan, Position.FrontRight);
            this.SetColor(Color.cyan, Position.FrontLeft);
            this.SetColor(Color.cyan, Position.BackLeft);
            this.SetColor(Color.cyan, Position.BackRight);
        }

    }

    /// <summary>
    /// Sets the lighting fixture to given color.
    /// </summary>
    /// <param name="color">Desired color.</param>
    public void SetColor(Color color, Position position)
    {

        int deviceID = 1;

        switch (position) {

            case Position.BackRight:
            // Back-right
            dmx.Channels[deviceID + 0] = (byte)(Mathf.FloorToInt(255f * color.r));
            dmx.Channels[deviceID + 1] = (byte)(Mathf.FloorToInt(255f * color.g));
            dmx.Channels[deviceID + 2] = (byte)(Mathf.FloorToInt(255f * color.b));
            break;

            case Position.BackLeft:
            // Back-left
            dmx.Channels[deviceID + 3] = (byte)(Mathf.FloorToInt(255f * color.r));
            dmx.Channels[deviceID + 4] = (byte)(Mathf.FloorToInt(255f * color.g));
            dmx.Channels[deviceID + 5] = (byte)(Mathf.FloorToInt(255f * color.b));
            break;

            case Position.FrontRight:
            // Front-right
            dmx.Channels[deviceID + 6] = 255; // on settings
            dmx.Channels[deviceID + 7] = 255; // on settings

            dmx.Channels[deviceID + 8] = (byte)(Mathf.FloorToInt(255f * color.r));
            dmx.Channels[deviceID + 9] = (byte)(Mathf.FloorToInt(255f * color.g));
            dmx.Channels[deviceID + 10] = (byte)(Mathf.FloorToInt(255f * color.b));
            break;

            default:
            case Position.FrontLeft:
            // Front-left
            dmx.Channels[deviceID + 13] = 255; // on settings
            dmx.Channels[deviceID + 14] = 255; // on settings

            dmx.Channels[deviceID + 15] = (byte)(Mathf.FloorToInt(255f * color.r));
            dmx.Channels[deviceID + 16] = (byte)(Mathf.FloorToInt(255f * color.g));
            dmx.Channels[deviceID + 17] = (byte)(Mathf.FloorToInt(255f * color.b));
            break;

        }

        this.dmx.Send();

    }

    /// <summary>
    /// Performs smooth transition between two colors over the given duration.
    /// </summary>
    /// <param name="startColor">Intial color.</param>
    /// <param name="endColor">Final color.</param>
    /// <param name="duration">Duration of transition in seconds.</param>
    private IEnumerator FadeColor(Position pos, Color startColor, Color endColor, float duration)
    {
        float startTime = Time.time;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Color lerpedColor = Color.Lerp(startColor, endColor, t);
            this.SetColor(lerpedColor, pos);
            yield return 0;
            elapsed = Time.time - startTime;
        }
    }

    public void ChangeAll(Color color) {
        StopAllCoroutines();
        this.SetColor(color, Position.FrontRight);
        this.SetColor(color, Position.FrontLeft);
        this.SetColor(color, Position.BackLeft);
        this.SetColor(color, Position.BackRight); 
        currentColor = color;
    }

    public void FadeAll(Color startColor, Color endColor, float duration){
       StartCoroutine(FadeColor(Position.FrontRight, startColor, endColor, duration));
       StartCoroutine(FadeColor(Position.FrontLeft, startColor, endColor, duration));
       StartCoroutine(FadeColor(Position.BackLeft, startColor, endColor, duration));
       StartCoroutine(FadeColor(Position.BackRight, startColor, endColor, duration));
    }

    private void OnApplicationQuit()
    {
        this.SetColor(Color.black, Position.FrontRight);
        this.SetColor(Color.black, Position.FrontLeft);
        this.SetColor(Color.black, Position.BackLeft);
        this.SetColor(Color.black, Position.BackRight);
    }
}
