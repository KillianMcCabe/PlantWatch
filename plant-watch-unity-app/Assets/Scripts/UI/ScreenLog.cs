using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLog : MonoBehaviour
{
    private string _logString;
    private Queue _logQueue = new Queue();
    private const int LogSizeLimit = 6;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        _logString = logString;
        string newString = "\n [" + type + "] : " + _logString;
        _logQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            _logQueue.Enqueue(newString);
        }
        _logString = string.Empty;
        foreach(string mylog in _logQueue){
            _logString += mylog;
        }

        while (_logQueue.Count > LogSizeLimit)
        {
            _logQueue.Dequeue();
        }
    }

    void OnGUI () {
        GUILayout.Label(_logString);
    }
}
