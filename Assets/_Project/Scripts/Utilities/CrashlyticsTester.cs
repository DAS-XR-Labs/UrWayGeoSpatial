using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CrashlyticsTester : MonoBehaviour {

    int m_updatesBeforeException;

    // Use this for initialization
    void Start () {
      m_updatesBeforeException = 0;
      
    }

 
    private IEnumerator ForceCrash()
    {
        yield return new WaitForSeconds(3);
        Utils.ForceCrash(ForcedCrashCategory.Abort); 
    }
    // Update is called once per frame
    void Update()
    {
        // Call the exception-throwing method here so that it's run
        // every frame update
        throwExceptionEvery60Updates();
    }

    // A method that tests your Crashlytics implementation by throwing an
    // exception every 60 frame updates. You should see reports in the
    // Firebase console a few minutes after running your app with this method.
    void throwExceptionEvery60Updates()
    {
        if (m_updatesBeforeException > 0)
        {
            m_updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            m_updatesBeforeException = 60;

            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }
}