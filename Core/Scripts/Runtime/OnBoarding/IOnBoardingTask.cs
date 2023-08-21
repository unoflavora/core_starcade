using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade
{
    public interface IOnBoardingTask
    {
        public bool IsDoingTask { get; set; }
        public void StartTask(object onBoardingEvent, UnityAction onFinishTask, UnityAction onNextTask);
        public void OnSkipTask();
        public void OnTaskFinish();
        public void OnBoardingFinish();
    }
}
