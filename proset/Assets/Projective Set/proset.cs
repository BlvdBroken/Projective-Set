using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using JetBrains.Annotations;

public class proset : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved = false;

    public KMSelectable[] cards;
    public KMSelectable sumbit;
    public GameObject[] dotsOne;
    public GameObject[] dotsTwo;
    public GameObject[] dotsThree;
    public GameObject[] dotsFour;
    public GameObject[] dotsFive;
    public GameObject[] dotsSix;
    public GameObject[] dotsSeven;
    public GameObject[] lights;
    public AudioClip wish;

    private List<int> nums = Enumerable.Range(1, 63).ToList();
    private int[] selected = { 0, 0, 0, 0, 0, 0, 0 };
    private int[] totals = { 0, 0, 0, 0, 0, 0, 0 };
    private Coroutine[] cardAnims = new Coroutine[7];
    private List<int> possibleAns = new List<int>();

    void Awake()
    {
        ModuleId = ModuleIdCounter++;

        for (int i = 0; i < 7; i++)
        {
            int j = i;
            cards[j].OnInteract += delegate ()
            {
                if (cardAnims[j] != null)
                    StopCoroutine(cardAnims[j]);
                cardAnims[j] = StartCoroutine(cardSelect(j));
                return false;
            };
            int ci = Rnd.Range(0, (63 - j));
            totals[j] = nums[ci];
            nums.RemoveAt(ci);
        }
        sumbit.OnInteract += delegate () { submitPress(); return false; };
    }

    void Start()
    {
        GameObject[][] dotsAll = { dotsOne, dotsTwo, dotsThree, dotsFour, dotsFive, dotsSix, dotsSeven };
        for (int i = 0; i < 7; i++)
        {
            int temp = totals[i];
            double color = 0;
            for (int j = 5; j > -1; j--)
            {
                // 32, 16, 8, 4, 2, 1
                // Red, Orange, Yellow, Green, Blue, Violet
                color = Math.Pow(2, (double)j);
                if (temp >= color)
                {
                    dotsAll[i][j].SetActive(true);
                    temp -= (int)color;
                }
                else
                {
                    dotsAll[i][j].SetActive(false);
                }
            }
        }
        possibleAnswer();
    }

    private IEnumerator cardSelect(int which)
    {
        if (ModuleSolved)
            yield break;
        float startgoal = 0.016f;
        float endgoal = 0.023f;
        float elapsed = 0f;
        float duration = 0.1f;
        if (selected[which] == 0)
        {
            selected[which] = 1;
            while (elapsed < duration)
            {
                // cards[which].transform.localPosition += new Vector3(0, 0.001f, 0);
                cards[which].transform.localPosition = new Vector3(cards[which].transform.localPosition.x, Mathf.Lerp(startgoal, endgoal, elapsed / duration), cards[which].transform.localPosition.z);
                yield return null;
                elapsed += Time.deltaTime;
            }
            cards[which].transform.localPosition = new Vector3(cards[which].transform.localPosition.x, endgoal, cards[which].transform.localPosition.z);
            lights[which].SetActive(true);
        }
        else
        {
            selected[which] = 0;
            lights[which].SetActive(false);
            while (elapsed < duration)
            {
                // cards[which].transform.localPosition -= new Vector3(0, 0.001f, 0);
                cards[which].transform.localPosition = new Vector3(cards[which].transform.localPosition.x, Mathf.Lerp(endgoal, startgoal, elapsed / duration), cards[which].transform.localPosition.z);
                yield return null;
                elapsed += Time.deltaTime;
            }
            cards[which].transform.localPosition = new Vector3(cards[which].transform.localPosition.x, startgoal, cards[which].transform.localPosition.z);
        }
    }

    void submitPress()
    {
        if (ModuleSolved)
            return;
        int chosen = 0;
        int xorTotal = 0;
        for (int i = 0; i < 7; i++)
        {
            if (selected[i] == 1)
            {
                // Debug.LogFormat("{0}", totals[i]);
                xorTotal ^= totals[i];
                chosen++;
                StartCoroutine(cardSelect(i));
            }
        }
        if (chosen == 0)
        {
            DebugMsg("You submitted without selecting any cards. Please do better.");
        }
        else if (xorTotal == 0)
        {
            DebugMsg("There is an even number of each colored dot. Nice job.");
            ModuleSolved = true;
            Audio.PlaySoundAtTransform("wish", transform);
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            DebugMsg("There is not an even number of each colored dot. Oops.");
            GetComponent<KMBombModule>().HandleStrike();
        }
    }

    int possibleAnswer()
    {
        int[] variations = Enumerable.Range(1, 127).ToArray();
        int currBin = 0;
        int check = 0;
        int[] corrCheck = { 0, 0, 0, 0, 0, 0, 0 };
        for (int j = 0; j < 128; j++)
        {
            check = 0;
            for (int i = 6; i > -1; i--)
            {
                currBin = (int)Math.Pow(2, i);
                if (variations[j] >= currBin)
                {
                    variations[j] -= currBin;
                    check ^= totals[i];
                    corrCheck[i] = 1;
                }
                else
                {
                    corrCheck[i] = 0;
                }
            }
            if (check == 0)
            {
                string answer = "In reading order, the cards ";
                for (int i = 0; i < 7; i++)
                {
                    if (corrCheck[i] == 1)
                    {
                        answer += $"{i + 1} ";
                        possibleAns.Add(i);
                    }
                }
                DebugMsg(answer + "are a possible solution.");
                return 0;
            }
        }
        DebugMsg("Something has gone terribly wrong.");
        return 1;
    }

    void DebugMsg(string msg)
    {
        Debug.LogFormat("[Proset #{0}] {1}", ModuleId, msg.Replace('\n', ' '));
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit 1 2 3 4 5 6 7 [Submits cards 1, 2, 3, 4, 5, 6, and 7.]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        var m = Regex.Match(Command, @"^\s*submit\s+(?<ans>[1234567;, ]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success)
            yield break;
        var ans = m.Groups["ans"].Value;
        var cardsToSubmit = new List<int>();
        for (int i = 0; i < ans.Length; i++)
        {
            string str = "1234567;, ";
            int ix = str.IndexOf(ans[i]);
            if (ix == -1)
                yield break;
            if (ix > 6)
                continue;
            cardsToSubmit.Add(ix);
        }
        if (cardsToSubmit.Distinct().Count() != cardsToSubmit.Count())
        {
            yield return "sendtochaterror Duplicate cards detected in command. Please do better.";
            yield break;
        }
        if (cardsToSubmit.Count() == 0)
        {
            yield return "sendtochaterror You submitted without selecting any cards. Please do better.";
            yield break;
        }
        yield return null;
        for (int i = 0; i < 7; i++)
        {
            if (selected[i] == 1 != cardsToSubmit.Contains(i))
            {
                cards[i].OnInteract();
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return new WaitForSeconds(0.3f);
        sumbit.OnInteract();
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 7; i++)
        {
            if (selected[i] == 1 != possibleAns.Contains(i))
            {
                cards[i].OnInteract();
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return new WaitForSeconds(0.3f);
        sumbit.OnInteract();
    }
}
