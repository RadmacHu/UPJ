using System;
using System.Collections.Generic;
using UnityEngine;
using MgsCallBack;
using MgsMoudle.MgsDefine;

namespace MgsMoudle
{
    static internal class GMessenger
    {
        #region Internal variables

        //Disable the unused variable warning
#pragma warning disable 0414
        //Ensures that the GMessengerHelper will be created automatically upon start of the game.
        static private GMessengerHelper _gMessengerHelper = (new GameObject("GMessengerHelper")).AddComponent<GMessengerHelper>();
#pragma warning restore 0414

        static public Dictionary<MgsDef, Delegate> eventTable = new Dictionary<MgsDef, Delegate>();

        //Message handlers that should never be removed, regardless of calling Cleanup
        static public List<MgsDef> permanentMessages = new List<MgsDef>();
        #endregion
        #region Helper methods
        //Marks a certain message as permanent.
        static public void MarkAsPermanent(MgsDef eventType)
        {
#if LOG_ALL_MESSAGES
		Debug.Log("GMessenger MarkAsPermanent \t\"" + eventType + "\"");
#endif

            permanentMessages.Add(eventType);
        }


        static public void Cleanup()
        {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure athat none of necessary listeners are removed.");
#endif

            List<MgsDef> messagesToRemove = new List<MgsDef>();

            foreach (KeyValuePair<MgsDef, Delegate> pair in eventTable)
            {
                bool wasFound = false;

                foreach (MgsDef message in permanentMessages)
                {
                    if (pair.Key == message)
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                    messagesToRemove.Add(pair.Key);
            }

            foreach (MgsDef message in messagesToRemove)
            {
                eventTable.Remove(message);
            }
        }

        static public void PrintEventTable()
        {
            Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");

            foreach (KeyValuePair<MgsDef, Delegate> pair in eventTable)
            {
                string name = Enum.GetName(typeof (MgsDef), pair.Key);
                if (name != null)
                {
                    Debug.Log("\t\t\t" + name + "\t\t" + pair.Value);
                }
                else
                {
                    Debug.LogError("Error name is null.");
                }
            }
            Debug.Log("\n");
        }
        #endregion

        #region Message logging and exception throwing
        static public void OnListenerAdding(MgsDef eventType, Delegate listenerBeingAdded)
        {
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif

            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            Delegate d = eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        static public void OnListenerRemoving(MgsDef eventType, Delegate listenerBeingRemoved)
        {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif

            if (eventTable.ContainsKey(eventType))
            {
                Delegate d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but GMessenger doesn't know about this event type.", eventType));
            }
        }

        static public void OnListenerRemoved(MgsDef eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        static public void OnBroadcasting(MgsDef eventType)
        {
#if REQUIRE_LISTENER
        if (!eventTable.ContainsKey(eventType)) {
            throw new BroadcastException(string.Format("Broadcasting message \"{0}\" but no listener found. Try marking the message with GMessenger.MarkAsPermanent.", eventType));
        }
#endif
        }

        static public BroadcastException CreateBroadcastSignatureException(MgsDef eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }
        #endregion

        #region AddListener
        //No parameters
        static public void AddListener(MgsDef eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback)eventTable[eventType] + handler;
        }

        //Single parameter
        static public void AddListener<T>(MgsDef eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
        }

        //Two parameters
        static public void AddListener<T, U>(MgsDef eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
        }

        //Three parameters
        static public void AddListener<T, U, V>(MgsDef eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
        }
        #endregion

        #region RemoveListener
        //No parameters
        static public void RemoveListener(MgsDef eventType, Callback handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Single parameter
        static public void RemoveListener<T>(MgsDef eventType, Callback<T> handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Two parameters
        static public void RemoveListener<T, U>(MgsDef eventType, Callback<T, U> handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Three parameters
        static public void RemoveListener<T, U, V>(MgsDef eventType, Callback<T, U, V> handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
        #endregion

        #region Broadcast
        //No parameters
        static public void Broadcast(MgsDef eventType)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Single parameter
        static public void Broadcast<T>(MgsDef eventType, T arg1)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Two parameters
        static public void Broadcast<T, U>(MgsDef eventType, T arg1, U arg2)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Three parameters
        static public void Broadcast<T, U, V>(MgsDef eventType, T arg1, U arg2, V arg3)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }
        #endregion
    }

    
    //This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
    public sealed class GMessengerHelper : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //Clean up eventTable every time a new level loads.
        public void OnLevelWasLoaded(int unused)
        {
            GMessenger.Cleanup();
        }
    }

}
