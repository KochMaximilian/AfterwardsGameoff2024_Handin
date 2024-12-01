using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class SentenceProcessor
{
    // Predefined valid argument keys
    private static readonly HashSet<string> ValidArguments = new HashSet<string> { "l", "r", "w", "ac", "acl"};
    private static readonly Dictionary<string, Sentence> CachedSentences = new Dictionary<string, Sentence>();
    
    public static IReadOnlyList<Sentence> CachedSentencesList => new List<Sentence>(CachedSentences.Values);
    
    public static void ClearCache()
    {
        CachedSentences.Clear();
    }

    public static Sentence ProcessSentence(string sentence, WordData[] words)
    {
        if (CachedSentences.TryGetValue(sentence, out var cachedSentence))
            return cachedSentence;
        
        // Regex pattern to match both {[args] text} and {text}
        string pattern = @"\{(?:\[(.*?)\])?\s*(.*?)\}";
        string replacement = "$2";

        List<KeyWord> keyWords = new List<KeyWord>();
        int offset = 0; // Tracks the cumulative length of removed/replaced characters

        foreach (Match match in Regex.Matches(sentence, pattern))
        {
            if (match.Groups.Count > 2)
            {
                string args = match.Groups[1].Value; // Arguments inside [ ]
                string text = match.Groups[2].Value; // Text inside the braces
                int originalStartIndex = match.Index; // Start index of the entire match
                int startIndex = originalStartIndex - offset; // Adjusted index as if braces didn't exist

                // Validate arguments if present
                if (!string.IsNullOrWhiteSpace(args) && !AreArgumentsValid(args))
                {
                    Debug.LogError($"Invalid arguments: {args} in sentence: {sentence}");
                    args = string.Empty;
                }

                keyWords.Add(ProcessKeyWord(startIndex, text, args, words));

                // Update offset: length of braces and arguments being removed
                offset += match.Length - text.Length;
            }
        }
        AsignNoTagIndexes(sentence, keyWords, pattern);

        string unformattedText = Regex.Replace(sentence, pattern, replacement);
        Sentence finalSentence = new Sentence(unformattedText, sentence, keyWords);
        CachedSentences.Add(sentence, finalSentence);
        return finalSentence;
    }

    private static void AsignNoTagIndexes(string sentence, List<KeyWord> keyWords, string pattern)
    {
        int offset = 0; // Tracks the cumulative length of removed/replaced characters
        List<int> noTagsIndices = new List<int>(keyWords.Count);
        foreach (Match match in Regex.Matches(RemoveAllTags(sentence), pattern))
        {
            if (match.Groups.Count > 2)
            {
                string text = match.Groups[2].Value; // Text inside the braces
                int originalStartIndex = match.Index; // Start index of the entire match
                int startIndex = originalStartIndex - offset; // Adjusted index as if braces didn't exist
                
                noTagsIndices.Add(startIndex);

                // Update offset: length of braces and arguments being removed
                offset += match.Length - text.Length;
            }
        }

        for (var i = 0; i < keyWords.Count; i++)
        {
            keyWords[i].noTagsIndex = noTagsIndices[i];
        }
    }


    private static KeyWord ProcessKeyWord(int startIndex, string text, string args, WordData[] words)
    {
        var arguments = GetArguments(args);
        int leftMargin = arguments.TryGetValue("l", out var arg) ? int.Parse(arg) : 0;
        int rightMargin = arguments.TryGetValue("r", out arg) ? int.Parse(arg) : 0;
        WordData wordData = arguments.TryGetValue("w", out arg) ? words[int.Parse(arg)] : null;
        bool allowCombining = !arguments.TryGetValue("ac", out arg) || bool.Parse(arg);
        bool allowLiteralCombining = !arguments.TryGetValue("acl", out arg) || bool.Parse(arg);
                
        return new KeyWord(text, startIndex, leftMargin, rightMargin, allowCombining, allowLiteralCombining, wordData);
    }

    private static bool AreArgumentsValid(string args)
    {
        if (string.IsNullOrWhiteSpace(args)) return true;

        string[] arguments = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string arg in arguments)
        {
            string[] keyValue = arg.Split('=');
            if (keyValue.Length != 2 || !ValidArguments.Contains(keyValue[0].Trim()))
            {
                return false;
            }
        }
        return true;
    }
    
    private static Dictionary<string, string> GetArguments(string args)
    {
        Dictionary<string, string> res = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(args)) return res;

        string[] arguments = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string arg in arguments)
        {
            string[] keyValue = arg.Split('=');
            res.Add(keyValue[0].Trim(), keyValue[1].Trim());
        }
        
        return res;
    }

    public static void SetCachedSentences(List<Sentence> collectedSentences)
    {
        ClearCache();
        foreach (var sentence in collectedSentences)
        {
            CachedSentences.Add(sentence.FormattedText, sentence);
        }
    }
    
    public static string RemoveAllTags(string sentence)
    {
        return Regex.Replace(sentence, "<.*?>", string.Empty);
    }
}