using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendScoreButton : MonoBehaviour
{
    
    void Start()
    {
        var button = GetComponent<Button>();

        button.OnClickAsObservable()
            .Subscribe(
                async _ =>
                {
                    WWWForm form = new WWWForm();
                    form.AddField("score", 100);

                    using var request = UnityWebRequest.Post("http://213.play.lvh.me:3000/api/v1/rankings/123/scores",form);
                    await request.SendWebRequest();
                    Debug.Log(request.downloadHandler.text);
                }
            )
            .AddTo(this);
    }
}
