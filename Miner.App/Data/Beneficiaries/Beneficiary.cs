﻿using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Timers;

namespace HD
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Beneficiary
  {
    #region Data
    /// <summary>
    /// To change the wallet, remove and then add a new beneficiary.
    /// </summary>
    [JsonProperty]
    public readonly string wallet;

    [JsonProperty]
    public string name;

    [JsonProperty]
    double _percentTime;

    APINiceHashWorkerList apiWorkerList;

    readonly Timer updateNetworkPerformanceTimer = new Timer(20000);
    #endregion

    #region Properties
    /// <summary>
    /// [0,1]
    /// 
    /// Refresh the total cached if this changes
    /// </summary>
    public double percentTime
    {
      get
      {
        return _percentTime;
      }
      set
      {
        if (value < 0)
        {
          value = 0;
        }
        else if (value > 1)
        {
          value = 1;
        }

        if(percentTime == value)
        {
          return;
        }

        _percentTime = value;
      }
    }

    public bool isValidAndActive
    {
      get
      {
        return IsWalletValid()
          && percentTime > 0;
      }
    }

    public double totalWorkerHashRateMHpS
    {
      get
      {
        return apiWorkerList?.totalWorkerHashRateMHpS ?? 0;
      }
    }
    #endregion

    #region Init
    public Beneficiary(
      string name,
      string wallet,
      double percentTime)
    {
      this.name = name;
      this.wallet = wallet;
      this.percentTime = percentTime;

      updateNetworkPerformanceTimer.AutoReset = false;
      updateNetworkPerformanceTimer.Elapsed += Timer_Elapsed;

      if (IsWalletValid())
      {
        InitAPI();
      }
    }

    [OnDeserialized]
    void OnDeserialized(
      StreamingContext context)
    {
      if (IsWalletValid())
      {
        InitAPI();
      }
    }

    void InitAPI()
    {
      apiWorkerList = new APINiceHashWorkerList(wallet);
      updateNetworkPerformanceTimer.Start();
    }
    #endregion

    #region Events
    void Timer_Elapsed(
      object sender, 
      ElapsedEventArgs e)
    {
      apiWorkerList.ReadWhenReady();
      updateNetworkPerformanceTimer.Start();
    }
    #endregion

    // TODO total mined
    //double secondsSinceLastUpdate = (DateTime.Now - hashRateLastUpdated).TotalSeconds;
    //hashRateLastUpdated = DateTime.Now;

    //  if (Miner.instance.settings.currentBeneficiary != null)
    //  {
    //    double estimatedContributionSinceLastUpdate = this.dailyEstimatedBtc;
    //estimatedContributionSinceLastUpdate /= (24 * 60 * 60); // estimated per second
    //    estimatedContributionSinceLastUpdate *= secondsSinceLastUpdate;
    //    Miner.instance.settings.currentBeneficiary.totalMinedInBitcoin += estimatedContributionSinceLastUpdate;
    //  }

    #region Helpers
    bool IsWalletValid()
    {
      // Is this a valid bitcoin wallet?
      if (string.IsNullOrEmpty(wallet)
      || wallet.Length < 26 // too short to be valid
      || wallet.Length > 35 // too long to be valid
      || wallet[0] != '1' && wallet[0] != '3') // must start with 1 or 3 if valid
      {
        return false;
      }

      return true;
    }
    #endregion
  }
}
