using System;
using System.Threading;
using UnityEngine;

public static class DebugService
{
        public static bool PrintDebug = true;
        public static void Log(string message, object sender, Thread thread = null)
        {
                if(!PrintDebug) return;
                var senderMerged = MergeSenderAndThread(sender, thread);
                Debug.Log(Message(message,senderMerged));
        }                
        public static void Log(string message, string sender, Thread thread = null)
        {
                if(!PrintDebug) return;
                var senderMerged = MergeSenderAndThread(sender, thread);
                Debug.Log(Message(message,senderMerged));
        }

        public static void LogWarning(string message, object sender)
        {
                if(!PrintDebug) return;
                Debug.LogWarning(Message(message,sender));
        }

        public static void LogError(string message, object sender, Exception ex = null)
        {
                if(!PrintDebug) return;
                Debug.LogError(Message(message,sender,ex));
        }

        public static void DebugException(Exception ex)
        {
                if(!PrintDebug) return;
                Debug.LogException(ex);
        }

        private static string Message(string baseMessage, object sender, Exception ex = null)
        {
                return Message(baseMessage, sender.GetType().ToString(), ex);
        }

        private static string Message(string baseMessage, string sender, Exception ex = null)
        {
                var message = sender + ": " + baseMessage;
                if (ex != null)
                {
                        message += " Exception: " + ex;
                }

                return message;
        }

        private static string MergeSenderAndThread(object sender, Thread thread)
        {
                return MergeSenderAndThread(sender.GetType().ToString(), thread);
        }
        
        private static string MergeSenderAndThread(string sender, Thread thread)
        {
                if (thread == null)
                {
                        return sender;
                }
                return sender + " -T(" + thread.ManagedThreadId + ")";
        }


}