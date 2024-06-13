namespace UserService.Utils;

public static class RockyouChecker
{
    public static readonly string[] Rockyou = File.ReadAllLines("Data/Security/rockyou.txt");
    public static bool IsInRockyou(string password)
    {
        foreach (string line in Rockyou)
        {
            if (password == line)
                return true;
        }
        return false;
    }
}