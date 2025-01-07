using Cysharp.Threading.Tasks;
using TMPro;
using System.Linq;
using UnityEngine;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Utils 
{
    public static async UniTask WritteLine(string line, TMP_Text dialogText, float lettersSpeed = 40)
    {
        dialogText.text = "";

        await line.ToCharArray().Aggregate(UniTask.CompletedTask, async (previousTask, letter) =>
        {
            await previousTask;
            dialogText.text += letter;
            await UniTask.WaitForSeconds(1f / lettersSpeed);
        });
    }

    public static async UniTask<GameObject> InstantiateAddressable(string key)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        else
            return null;
    }

    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
