using System;
using System.Threading.Tasks;

public class LocalGameTimeHandler : IGameTimeHandler
{
    protected const string Filename = "GameTimeData";
    protected readonly TimeData TimeData;
    protected readonly JsonSaving<SerializableTimeData> SavingSystem;
    
    public LocalGameTimeHandler()
    {
        SavingSystem = new JsonSaving<SerializableTimeData>();
        
        object data = SavingSystem.LoadData(Filename);
        if (data == null)
        {
            SavingSystem.SaveData(new TimeData().SerializableTimeData, Filename);
            return;
        }
        
        TimeData = new TimeData((SerializableTimeData) data);
        TimeData.LastSaveInThisSession = DateTime.Now;
    }
    
    public TimeSpan GetInitialTotalPlaytime()
    {
        return TimeData.InitialTotalPlaytime;
    }

    public virtual TimeSpan GetCurrentTotalPlaytime()
    {
        return TimeData.InitialTotalPlaytime + (DateTime.Now - TimeData.LastSaveInThisSession);
    }
        
    public virtual TimeSpan GetTimeSinceLastSave()
    {
        TimeSpan timeSinceLastSave = DateTime.Now - TimeData.LastSaveTime;
        return timeSinceLastSave.TotalSeconds < 0 ? TimeSpan.Zero : timeSinceLastSave;
    }
    
    public DateTime GetLastSaveTime()
    {
        return TimeData.LastSaveTime;
    }

    public virtual void OnDataChanged()
    {
        TimeData.InitialTotalPlaytime = GetCurrentTotalPlaytime();
        TimeData.LastSaveTime = DateTime.Now;
        TimeData.LastSaveInThisSession = TimeData.LastSaveTime;
        
        SavingSystem.SaveData(TimeData.SerializableTimeData, Filename);
    }
    
    public virtual Task OnDataChangedAsync()
    {
        OnDataChanged();
        return Task.CompletedTask;
    }
    
    public virtual Task<TimeSpan> GetCurrentTotalPlaytimeAsync()
    {
        return Task.FromResult(GetCurrentTotalPlaytime());
    }

    public virtual Task<TimeSpan> GetTimeSinceLastSaveAsync()
    {
        return Task.FromResult(GetTimeSinceLastSave());
    }
}