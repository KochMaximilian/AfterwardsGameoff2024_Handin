using System;
using System.Threading.Tasks;
using UnityEngine;

public class NTPGameTimeHandler : LocalGameTimeHandler
{
    public NTPGameTimeHandler() : base()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            DateTime lastSaveInThisSession = ntpClient.GetNetworkTime();
            TimeData.LastSaveInThisSession = lastSaveInThisSession;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }

    public override TimeSpan GetCurrentTotalPlaytime()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            return TimeData.InitialTotalPlaytime + (ntpClient.GetNetworkTime() - TimeData.LastSaveInThisSession);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }

    public override TimeSpan GetTimeSinceLastSave()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            return ntpClient.GetNetworkTime() - TimeData.LastSaveTime;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }

    public override void OnDataChanged()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            TimeData.InitialTotalPlaytime = GetCurrentTotalPlaytime();
            TimeData.LastSaveTime = ntpClient.GetNetworkTime();
            TimeData.LastSaveInThisSession = TimeData.LastSaveTime;
        
            SavingSystem.SaveData(TimeData.SerializableTimeData, Filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }
    
    public override async Task OnDataChangedAsync()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            TimeData.InitialTotalPlaytime = await GetCurrentTotalPlaytimeAsync();
            TimeData.LastSaveTime = await ntpClient.GetNetworkTimeAsync();
            TimeData.LastSaveInThisSession = TimeData.LastSaveTime;
        
            SavingSystem.SaveData(TimeData.SerializableTimeData, Filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }
    
    public override async Task<TimeSpan> GetCurrentTotalPlaytimeAsync()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            return TimeData.InitialTotalPlaytime + (await ntpClient.GetNetworkTimeAsync() - TimeData.LastSaveInThisSession);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }
    
    public override async Task<TimeSpan> GetTimeSinceLastSaveAsync()
    {
        try
        {
            using NtpClient ntpClient = new NtpClient();
            return await ntpClient.GetNetworkTimeAsync() - TimeData.LastSaveTime;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw e;
        }
    }
}