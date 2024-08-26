using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForFrames : CustomYieldInstruction
{
    private int targetFrameCount;
    private int initialFrameCount;

    public WaitForFrames(int framesToWait)
    {
        targetFrameCount = framesToWait;
        initialFrameCount = Time.frameCount;
    }

    public override bool keepWaiting
    {
        get
        {
            return Time.frameCount < initialFrameCount + targetFrameCount;
        }
    }
}
