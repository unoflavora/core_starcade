using System;
using UnityEngine;
using Agate.Starcade.Runtime.Data;

[Serializable]
[CreateAssetMenu(menuName = "Starcade Scriptable Object/Community Mail SO")]
public class MailSO : ScriptableObject
{
    public MailboxDataItem[] Mails;
}
