using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class gestureManager : MonoBehaviour
{
    public static gestureManager Instance { get; private set; }
    GestureRecognizer recognizer;
    public Transform splatQuad;
    bool holding = false;
    Color color = Color.blue;

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Awake()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();

        recognizer.TappedEvent += (a, b, c) =>
        {
            holding = !holding;
        };

        recognizer.StartCapturingGestures();

        keywords.Add("Red", () =>
        {
            color = Color.red;
        });

        keywords.Add("Blue", () =>
        {
            color = Color.blue;
        });

        keywords.Add("Green", () =>
        {
            color = Color.green;
        });

        keywords.Add("Yellow", () =>
        {
            color = Color.yellow;
        });

        keywords.Add("Cyan", () =>
        {
            color = Color.cyan;
        });

        keywords.Add("White", () =>
        {
            color = Color.white;
        });

        keywords.Add("Magenta", () =>
        {
            color = Color.magenta;
        });

        keywords.Add("Grey", () =>
        {
            color = Color.grey;
        });

        keywords.Add("Pick a random color", () =>
        {
            color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (holding)
        {
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                var hitRotation = Quaternion.FromToRotation(-Vector3.forward, hitInfo.normal);
                Debug.Log(hitRotation);
                var splat = Instantiate(splatQuad, hitInfo.point + (hitInfo.normal * 0.03f), hitRotation);
                splat.localScale = new Vector3(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f));
                splat.GetComponent<Renderer>().material.color = new Color(Random.Range(0,1), Random.Range(0, 1), Random.Range(0, 1));
                splat.GetComponent<Renderer>().material.SetColor("_Color", color);
            }
        }

    }
}