/**
[SHYMotionRecorder]

Copyright (c) 2023 co1umbine

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php


[EasyMotionRecorder]

MIT License

Copyright (c) 2018 Duo.inc 2020 @neon-izm
http://opensource.org/licenses/mit-license.php
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Co1umbine.SHYMotionRecorder
{
    public interface IMotionLoader
    {
        public void LoadMotion(string directoryName, string fileName);
    }
}