using System;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SendScoreButton : MonoBehaviour
{
    
    void Start()
    {
        var button = GetComponent<Button>();
        
        button.OnClickAsObservable()
            .Subscribe(
                async _ =>
                {
                    await SendScore(Random.Range(0, 100));
                }
            )
            .AddTo(this);
    }

    async UniTask SendScore(float score)
    {
        var scoreboardId = 1;
        var url = $"http://219.play.lvh.me:3000/api/v1/scoreboards/{scoreboardId}/scores";

        var timestampText = GetUnixTime()
            .ToString();
        var scoreText = score.ToString();

        var sha1Text = sha1($"{timestampText}:{scoreText}", "12345");
        
        WWWForm form = new WWWForm();
        form.AddField("score", scoreText );

        using var request = UnityWebRequest.Post(url,form);
        request.SetRequestHeader("X-Unityroom-Timestamp", timestampText);
        request.SetRequestHeader("X-UNITYROOM-HMAC-SHA1", sha1Text);

        await request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
    }
    
    string sha1(string planeStr, string key) {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] planeBytes = ue.GetBytes(planeStr);
        byte[] keyBytes = ue.GetBytes(key);

        System.Security.Cryptography.HMACSHA1 sha1 = new System.Security.Cryptography.HMACSHA1(keyBytes);
        byte[] hashBytes = sha1.ComputeHash(planeBytes);
        string hashStr = "";
        foreach(byte b in hashBytes) {
            hashStr += $"{b,0:x2}";
        }
        return hashStr;
    }

    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public static long GetUnixTime()
    {
        return (long)(DateTime.Now - UnixEpoch).TotalSeconds;
    }

    // UnixTimeからDateTimeへ変換
    public static DateTime GetDateTime(long unixTime)
    {
        return UnixEpoch.AddSeconds(unixTime);
    }
}
