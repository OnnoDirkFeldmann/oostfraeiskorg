using System.IO;
using System;

namespace oostfraeiskorg;

public static class Server
{
    public static string MapPath(string path)
    {
        return Path.Combine(AppContext.BaseDirectory, path);
    }
}


