﻿using System;
using System.Threading;

namespace HD
{
  public class MinerAutoStart
  {
    readonly Thread thread;

    public MinerAutoStart()
    {
      thread = new Thread(Run);
     // thread.Start();
    }

    void Run()
    {
      //while(true)
      //{
      //  Thread.Sleep(1000);
      //  if(Miner.instance.isMinerRunning == false
      //    && Miner.instance.settings.minerConfig.isCurrentlyIdle
      //    && )
      //}
    }
  }
}