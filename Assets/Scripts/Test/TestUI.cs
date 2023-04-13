using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TextMeshPro;

    [Inject]
    private void Initialize(Foo foo)
    {
        m_TextMeshPro.text = foo.a;
    }
}
