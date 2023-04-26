using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NotifySystem
{
    public static NotifyDisplay Destination;

    /// <summary>
    /// Sends a message to the appropriate display
    /// </summary>
    /// <param name="text"></param>
    public static void Message(string text,float duration = 2.0f)
    {
        // Handle errors of no destination
        if (Destination == null) { return; }

        Destination.QueueMessage(text, duration);

    }
}
