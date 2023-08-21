using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentCheck : MonoBehaviour
{
    [SerializeField]
    private Text text = null;
    // Start is called before the first frame update
    void Start()
    {
        #if PRODUCTION
        text.text = "PRODUCTION";
        #elif DEVELOPMENT
        text.text = "DEVELOPMENT";
        #elif TEST
        text.text = "TEST";
        #else
        text.text = "UNKNOWN";
        #endif
    }

}
