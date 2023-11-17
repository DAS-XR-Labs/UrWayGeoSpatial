using System;
using System.Collections;
using System.Collections.Generic;
using DAS.Urway.Deals;
#if UNITY_IOS || UNITY_EDITOR
using Unity.Notifications.iOS;
#endif
using UnityEngine;
using UnityEngine.Serialization;


namespace DAS.Urway
{
    public class POINotificationController : MonoBehaviour
    {
        [FormerlySerializedAs("dealsManager")] [SerializeField] private DealsManager m_dealsManager; // Reference to the DealsManager component
        [FormerlySerializedAs("dealPanel")] [SerializeField] private ActiveDealsPanel m_dealPanel; // Reference to the ActiveDealsPanel component

        private string m_lasetRecievedID; // ID of the last received notification
        private float m_timer = 0f; // Timer for checking notifications at regular intervals
        private float m_interval = 1f; // Interval for checking notifications


#if UNITY_IOS

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Awake()
        {
            m_lasetRecievedID = "";
            //CheckNotifications();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            iOSNotificationCenter.OnNotificationReceived += OnNotificationReceived;
            m_dealsManager.OnDataUpdated += OnDealsUpdated;
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            iOSNotificationCenter.OnNotificationReceived -= OnNotificationReceived;
            m_dealsManager.OnDataUpdated -= OnDealsUpdated;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_interval)
            {
                CheckNotifications();
                m_timer = 0f;
            }
        }

        /// <summary>
        /// Event handler for deals data being updated.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDealsUpdated()
        {
            //iOSNotificationCenter.RemoveAllScheduledNotifications();
            StartCoroutine(RequestAuthorization(() =>
            {
                Debug.Log("SetPOINotification");
                List<string> scheduledNotifications = new List<string>();

                CheckNotifications();

                foreach (var vir in iOSNotificationCenter.GetScheduledNotifications())
                {
                    Debug.Log("Deal: " + vir.Identifier);
                    Debug.Log("Deal: " + vir.Title);
                    scheduledNotifications.Add(vir.Identifier);
                }

                foreach (var deal in m_dealsManager.Deals)
                {
                    string _id = (deal.Location.Lat + deal.Location.Lng).ToString();
                    if (!scheduledNotifications.Contains(_id))
                    {
                        iOSNotificationCenter.ScheduleNotification(CreateNotification(deal));
                    }
                }
            }));
        }

        /// <summary>
        /// Checks for pending notifications and processes them.
        /// </summary>
        /// <param name="receivedNotification">Received iOS notification (optional).</param>
        /// <returns>No expected outputs.</returns>
        private void CheckNotifications(iOSNotification receivedNotification = null)
        {
            iOSNotification _notification;

            if (receivedNotification == null)
            {
                _notification = iOSNotificationCenter.GetLastRespondedNotification();
            }
            else
            {
                _notification = receivedNotification;
            }

            if (_notification != null)
            {
                Debug.Log("lasetRecievedID: " +
                          m_lasetRecievedID);
                Debug.Log("GetLastRespondedNotification: " + _notification.Identifier);

                if (_notification.Identifier != m_lasetRecievedID)
                {
                    m_lasetRecievedID = _notification.Identifier;
                    var _deal = new DealData();
                    _deal.Company_name = _notification.Title;
                    _deal.Deal_description = _notification.Body;
                    iOSNotificationLocationTrigger _trigger = (iOSNotificationLocationTrigger) _notification.Trigger;
                    _deal.Location = new Urway.Deals.Location();
                    _deal.Location.Lat = _trigger.Center.x;
                    _deal.Location.Lng = _trigger.Center.y;
                    m_dealPanel.Show(_deal);
                }
            }
        }


        /// <summary>
        /// Requests user authorization for notifications.
        /// </summary>
        /// <param name="onComplete">Action to execute when authorization is complete.</param>
        /// <returns>No expected outputs.</returns>
        private IEnumerator RequestAuthorization(Action onComplete)
        {
            yield return new WaitForSeconds(2);
            using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                }

                ;

                string _res = "\n RequestAuthorization: \n";
                _res += "\n finished: " + req.IsFinished;
                _res += "\n granted :  " + req.Granted;
                _res += "\n error:  " + req.Error;
                _res += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log(_res);
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Creates an iOS notification based on the provided deal data.
        /// </summary>
        /// <param name="deal">Deal data to create notification for.</param>
        /// <returns>The created iOS notification.</returns>
        private iOSNotification CreateNotification(DealData deal)
        {
            var _locationTrigger = new iOSNotificationLocationTrigger()
            {
                Center = new Vector2((float) deal.Location.Lat, (float) deal.Location.Lng),
                Radius = 30f,
                NotifyOnEntry = true,
                NotifyOnExit = false,
            };

            return new iOSNotification()
            {
                // You can optionally specify a custom identifier which can later be 
                // used to cancel the notification, if you don't set one, a unique 
                // string will be generated automatically.
                Identifier = (deal.Location.Lat + deal.Location.Lng).ToString(),
                Title = deal.Company_name,
                Body = deal.Deal_description,
                Subtitle = "Message",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "poi",
                ThreadIdentifier = "poi",
                Trigger = _locationTrigger,
            };
        }


        /// <summary>
        /// Event handler for receiving an iOS notification.
        /// </summary>
        /// <param name="notification">Received iOS notification.</param>
        /// <returns>No expected outputs.</returns>
        private void OnNotificationReceived(iOSNotification notification)
        {
            // CheckNotifications(notification);
            Debug.Log("OnRemoteNotificationReceived id: " + notification.Identifier);
            Debug.Log("OnRemoteNotificationReceived Name: " + notification.Title);
        }

        /// <summary>
        /// Called when the application gains or loses focus.
        /// </summary>
        /// <param name="hasFocus">Indicates whether the application has focus.</param>
        /// <returns>No expected outputs.</returns>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                CheckNotifications();
            }
        }

#endif
    }
}