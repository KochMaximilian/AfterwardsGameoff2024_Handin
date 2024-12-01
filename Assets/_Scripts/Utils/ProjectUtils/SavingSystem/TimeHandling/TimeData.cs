using System;

public class TimeData
{
    public TimeSpan InitialTotalPlaytime = TimeSpan.Zero;
    public DateTime LastSaveTime = DateTime.Now;
    public DateTime LastSaveInThisSession = DateTime.Now;
    
    public SerializableTimeData SerializableTimeData => new SerializableTimeData(this);
    
    public TimeData() { }
    public TimeData(SerializableTimeData serializableTimeData)
    {
        InitialTotalPlaytime = TimeSpan.Parse(serializableTimeData.InitialTotalPlaytime);
        LastSaveTime = DateTime.Parse(serializableTimeData.LastSaveTime);
        LastSaveInThisSession = DateTime.Parse(serializableTimeData.LastSaveInThisSession);
    }
}

[Serializable]
public class SerializableTimeData
{
    public string InitialTotalPlaytime;
    public string LastSaveTime;
    public string LastSaveInThisSession;
    
    public SerializableTimeData(TimeData timeData)
    {
        InitialTotalPlaytime = timeData.InitialTotalPlaytime.ToString();
        LastSaveTime = timeData.LastSaveTime.ToString();
        LastSaveInThisSession = timeData.LastSaveInThisSession.ToString();
    }
}