using System;
using System.Threading.Tasks;

public interface IGameTimeHandler : IAsyncGameTimeHandler
{
    public TimeSpan GetInitialTotalPlaytime();
    public TimeSpan GetCurrentTotalPlaytime();
    public TimeSpan GetTimeSinceLastSave();
    public DateTime GetLastSaveTime();
}

public interface IAsyncGameTimeHandler
{
    public Task<TimeSpan> GetCurrentTotalPlaytimeAsync();
    public Task<TimeSpan> GetTimeSinceLastSaveAsync();
}

public class GameTimeData
{
    public DateTime LastSaveTime { get; private set; }
    public TimeSpan InitialTotalPlaytime { get; private set; }
    public TimeSpan CurrentTotalPlaytime {get; private set;}
    public TimeSpan TimeSinceLastSave {get; private set;} 
    
    private IGameTimeHandler _gameTimeHandler;
    public GameTimeData(IGameTimeHandler gameTimeHandler)
    {
        _gameTimeHandler = gameTimeHandler;
    }
    
    public async Task<bool> RefreshTimesAsync()
    {
        try
        {
            InitialTotalPlaytime = _gameTimeHandler.GetInitialTotalPlaytime();
            LastSaveTime = _gameTimeHandler.GetLastSaveTime();
            CurrentTotalPlaytime = await _gameTimeHandler.GetCurrentTotalPlaytimeAsync();
            TimeSinceLastSave = await _gameTimeHandler.GetTimeSinceLastSaveAsync();
        
            if(CurrentTotalPlaytime < TimeSpan.Zero)
            {
                InitialTotalPlaytime = TimeSpan.Zero;
                CurrentTotalPlaytime = TimeSpan.Zero;
                TimeSinceLastSave = TimeSpan.Zero;
            }
        
            if(TimeSinceLastSave < TimeSpan.Zero)
            {
                TimeSinceLastSave = TimeSpan.Zero;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
    
    public bool RefreshTimes()
    {
        try
        {
            InitialTotalPlaytime = _gameTimeHandler.GetInitialTotalPlaytime();
            LastSaveTime = _gameTimeHandler.GetLastSaveTime();
            CurrentTotalPlaytime = _gameTimeHandler.GetCurrentTotalPlaytime();
            TimeSinceLastSave = _gameTimeHandler.GetTimeSinceLastSave();
        
            if(CurrentTotalPlaytime < TimeSpan.Zero)
            {
                InitialTotalPlaytime = TimeSpan.Zero;
                CurrentTotalPlaytime = TimeSpan.Zero;
                TimeSinceLastSave = TimeSpan.Zero;
            }
        
            if(TimeSinceLastSave < TimeSpan.Zero)
            {
                TimeSinceLastSave = TimeSpan.Zero;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}
