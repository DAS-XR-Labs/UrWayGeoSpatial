using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAS.Urway
{
    public class FCMController : MonoBehaviour
    {
        public Action
            OnMessageReceivedSuccessfully; // Action that gets invoked when a message is received successfully.

        public void Start()
        {
            // Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            // Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        // public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        //     Debug.Log("Received Registration Token: " + token.Token);
        // }

        // public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
        //     Debug.Log("Received a new message from FCM:");
        //     Debug.Log($"From: {e.Message.From}");
        //     Debug.Log($"Title: {e.Message.Notification.Title}");
        //     Debug.Log($"Body: {e.Message.Notification.Body}");
        //     
        //     OnMessageReceivedSuccessfully?.Invoke();
        // }
    }
}