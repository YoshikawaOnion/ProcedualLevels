using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameUiManager : MonoBehaviour
    {
        public Camera UiCamera;
        public ControllerButton LeftButton;
        public ControllerButton RightButton;
        public ControllerButton JumpButton;
        public TimeLimit TimeLimitLabel;
        public GameObject ClearBackground;
        public GameObject ClearText;
    }
}