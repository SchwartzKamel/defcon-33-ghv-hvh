using BNB;
using HarmonyLib;
using payload;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

[HarmonyPatch]
public static class GameManagerOnUpdatePatch
{
    static MethodBase TargetMethod()
    {
        var asm = AssemblyLoadContext.All
            .SelectMany(a => a.Assemblies)
            .FirstOrDefault(a => a.GetName().Name == "package.fish.blocks_and_bullets"); // adjust if assembly name is different

        if (asm == null)
            return null;

        var t = asm.GetType("BNB.GameManager");
        return t?.GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    static void Postfix(object __instance)
    {
        // This runs after GameManager.OnUpdate finishes
        try
        {
            // Put your continuous logic here
            Loop.Main();
        }
        catch (Exception ex)
        {
            Log.Info($"[Harmony] Error in OnUpdate Postfix: {ex}");
        }
    }
}

[HarmonyPatch]
public static class ChatManagerSendMessagePatch
{
    static MethodBase TargetMethod()
    {
        var asm = AssemblyLoadContext.All
            .SelectMany(a => a.Assemblies)
            .FirstOrDefault(a => a.GetName().Name == "package.fish.blocks_and_bullets");

        if (asm == null)
            return null;

        var t = asm.GetType("BNB.ChatManager");
        return t?.GetMethod("SendMessage", BindingFlags.Public | BindingFlags.Static);
    }

    static void Postfix(object message)
    {
        try
        {
            // Get the type for Chatbox.Message
            var msgType = message.GetType();

            // Example: extract useful info
            var channelProp = msgType.GetProperty("Channel", BindingFlags.Public | BindingFlags.Instance);
            var textProp = msgType.GetProperty("Text", BindingFlags.Public | BindingFlags.Instance);
            var teamRelationProp = msgType.GetProperty("TeamRelation", BindingFlags.Public | BindingFlags.Instance);

            var channelValue = channelProp?.GetValue(message);
            var textValue = textProp?.GetValue(message);
            var teamRelationValue = teamRelationProp?.GetValue(message);

            Log.Info($"[Harmony] SendMessage called: [{channelValue}] {textValue} (Team: {teamRelationValue})");

            if(textValue.ToString().ToLower().StartsWith("alexa,"))
            {
                ChatCommands.LocalChatCommand(textValue.ToString());
            }

            // Your custom logic here
        }
        catch (Exception ex)
        {
            Log.Info($"[Harmony] Error in SendMessage Postfix: {ex}");
        }
    }
}

// bnb_announce takeover
[HarmonyPatch]
public static class GameManagerDebugAnnouncePatch
{
    static MethodBase TargetMethod()
    {
        var asm = AssemblyLoadContext.All
            .SelectMany(a => a.Assemblies)
            .FirstOrDefault(a => a.GetName().Name == "package.fish.blocks_and_bullets");

        if (asm == null)
            return null;

        var t = asm.GetType("BNB.GameManager");
        return t?.GetMethod("DebugAnnounce", BindingFlags.NonPublic | BindingFlags.Static);
    }

    // Prefix: block original by returning false
    static bool Prefix(string message)
    {
        Log.Info($"[Harmony] Blocked DebugAnnounce: \"{message}\"");

        // Your own replacement logic here
        // e.g., trigger your own RPC, log it, ignore, etc.

        return false; // skip the original
    }
}