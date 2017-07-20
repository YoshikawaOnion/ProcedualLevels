using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Def.PlayerTag)
        {
            var clearText = GameObject.Find("Canvas/GameUi/ClearText");
            clearText.SetActive(true);
        }
    }
}
