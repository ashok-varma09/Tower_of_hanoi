using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Common

{

    public class MobileAdsEventExecutor : MonoBehaviour

    {

        private static readonly Queue<Action> adEventsQueue = new Queue<Action>();

        private static bool initialized = false;

        public static void Initialize()

        {

            if (initialized) return;

            var obj = new GameObject("MobileAdsEventExecutor");

            DontDestroyOnLoad(obj);

            obj.AddComponent<MobileAdsEventExecutor>();

            initialized = true;

        }

        public static void ExecuteInUpdate(Action action)

        {

            if (action == null) return;

            lock (adEventsQueue)

            {

                adEventsQueue.Enqueue(action);

            }

        }

        private void Update()

        {

            lock (adEventsQueue)

            {

                while (adEventsQueue.Count > 0)

                {

                    var action = adEventsQueue.Dequeue();

                    action?.Invoke();

                }

            }

        }

    }

}


