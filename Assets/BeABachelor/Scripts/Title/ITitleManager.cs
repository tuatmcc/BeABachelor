using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeABachelor
{
    public interface ITitleManager
    {
        Action PlayFadeIn { get; set; }
        Action PlayFadeOut { get; set; }
    }
}
