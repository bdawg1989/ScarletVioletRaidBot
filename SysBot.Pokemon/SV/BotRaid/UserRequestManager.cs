using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class UserRequestManager
{
    private Dictionary<ulong, UserRequestInfo> userRequests;
    private readonly string filePath;

    public UserRequestManager()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory; // Or another method to determine the base directory
        var directoryPath = Path.Combine(baseDirectory, "raidfilessv");
        Directory.CreateDirectory(directoryPath);
        filePath = Path.Combine(directoryPath, "user_requests.json");
        Load();
    }

    private void Load()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            userRequests = JsonConvert.DeserializeObject<Dictionary<ulong, UserRequestInfo>>(json) ?? new Dictionary<ulong, UserRequestInfo>();
        }
        else
        {
            userRequests = new Dictionary<ulong, UserRequestInfo>();
            Save();
        }
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(userRequests);
        File.WriteAllText(filePath, json);
    }

    public bool CanRequest(ulong userId, int limit, int cooldown, out TimeSpan remainingCooldown)
    {
        remainingCooldown = TimeSpan.Zero;

        if (userRequests.TryGetValue(userId, out var info))
        {
            if (info.RequestCount >= limit)
            {
                var timeSinceLimit = DateTime.UtcNow - info.RequestLimitTime;
                if (timeSinceLimit < TimeSpan.FromMinutes(cooldown))
                {
                    remainingCooldown = TimeSpan.FromMinutes(cooldown) - timeSinceLimit;
                    return false;
                }
                info.RequestCount = 0; // Reset count after cooldown
            }
        }
        else
        {
            info = new UserRequestInfo { UserId = userId };
            userRequests.Add(userId, info);
        }

        info.RequestCount++;
        if (info.RequestCount == limit)
        {
            info.RequestLimitTime = DateTime.UtcNow;
        }

        Save();
        return true;
    }
}

public class UserRequestInfo
{
    public ulong UserId { get; set; }
    public int RequestCount { get; set; }
    public DateTime RequestLimitTime { get; set; }
}