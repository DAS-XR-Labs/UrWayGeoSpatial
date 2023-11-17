// Copyright 2022 Niantic, Inc. All Rights Reserved.

using UnityEngine;

namespace Niantic.ARDK.Utilities.Logging
{
    /// <summary>
    /// Implementation of IARLogHandler that uses UnityEngine.Debug as its target.
    /// </summary>
    /// <remarks>
    /// This class serves as an implementation of the IARLogHandler interface, utilizing the UnityEngine.Debug class for logging.
    /// </remarks>
    public sealed class UnityARLogHandler: 
    IARLogHandler
  {
    /// <summary>
    /// Gets the singleton instance of this log handler.
    /// </summary>
    public static readonly UnityARLogHandler Instance = new UnityARLogHandler();

    /// <summary>
    /// Initializes a new instance of the UnityARLogHandler class.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
        private UnityARLogHandler()
    {
    }

    /// <inheritdoc />
    /// <summary>
    /// Log a debug message using UnityEngine.Debug.
    /// </summary>
    /// <param name="log">The debug message to be logged.</param>
    /// <returns>No expected outputs.</returns>
    public void Debug(string log)
    {
      UnityEngine.Debug.Log(log);
    }

    /// <inheritdoc />
    /// <summary>
    /// Log a warning message using UnityEngine.Debug.
    /// </summary>
    /// <param name="log">The warning message to be logged.</param>
    /// <returns>No expected outputs.</returns>
    public void Warn(string log)
    {
      UnityEngine.Debug.LogWarning(log);
    }

    /// <inheritdoc />
    /// <summary>
    /// Log a release message using UnityEngine.Debug.
    /// </summary>
    /// <param name="log">The release message to be logged.</param>
    /// <returns>No expected outputs.</returns>
    public void Release(string log)
    {
      UnityEngine.Debug.Log(log);
    }

    /// <inheritdoc />
    /// <summary>
    /// Log an error message using UnityEngine.Debug.
    /// </summary>
    /// <param name="log">The error message to be logged.</param>
    public void Error(string log)
    {
      UnityEngine.Debug.LogError(log);
    }
  }
}
