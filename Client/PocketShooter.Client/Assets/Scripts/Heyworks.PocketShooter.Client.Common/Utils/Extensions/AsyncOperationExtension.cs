﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils.Extensions
{
    public class AsyncOperationBehaviour : MonoBehaviour
    {
    }

    public static class AsyncOperationExtension
    {
        #region Variables

        static AsyncOperationBehaviour behaviour = null;
        static List<Coroutine> allCoroutines = new List<Coroutine>();

        #endregion

        #region Public methods

        public static Coroutine StartCoroutine(this IEnumerator iterator)
        {
            Initialize();

            Coroutine asyncCoroutine = behaviour.StartCoroutine(iterator);
            if (asyncCoroutine != null)
            {
                allCoroutines.Add(asyncCoroutine);
            }

            return asyncCoroutine;
        }

        public static Coroutine StartCoroutine(this AsyncOperation task, Action finished = null)
        {
            Initialize();

            Coroutine asyncCoroutine = behaviour.StartCoroutine(RunTaskAsyncInner(task, finished));
            if (asyncCoroutine != null)
            {
                allCoroutines.Add(asyncCoroutine);
            }

            return asyncCoroutine;
        }

        public static Coroutine StartCoroutine(this WWW task, Action finished = null)
        {
            Initialize();

            Coroutine asyncCoroutine = behaviour.StartCoroutine(RunTaskWWWInner(task, finished));
            if (asyncCoroutine != null)
            {
                allCoroutines.Add(asyncCoroutine);
            }

            return asyncCoroutine;
        }

        public static void StopCoroutine(this Coroutine coroutine)
        {
            if ((coroutine != null) && (behaviour))
            {
                if (allCoroutines.Contains(coroutine))
                {
                    allCoroutines.Remove(coroutine);
                    behaviour.StopCoroutine(coroutine);
                }
            }
        }

        #endregion

        #region Private methods

        static void Initialize()
        {
            if (behaviour == null)
            {
                GameObject g = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(g);
                g.name = "AsyncOperationExtension_Couroutine";
                g.hideFlags = HideFlags.HideAndDontSave;

                behaviour = g.AddComponent<AsyncOperationBehaviour>();
            }
        }

        static IEnumerator RunTaskInner(IEnumerator task, Action finished = null)
        {
            while (task.MoveNext())
            {
                yield return null;
            }

            finished?.Invoke();
        }


        static IEnumerator RunTaskAsyncInner(AsyncOperation task, Action finished = null)
        {
            while (!task.isDone)
            {
                yield return null;
            }

            finished?.Invoke();
        }


        static IEnumerator RunTaskWWWInner(WWW task, Action finished = null)
        {
            while (!task.isDone)
            {
                yield return null;
            }

            finished?.Invoke();
        }

        #endregion
    }
}