using System.IO;
using System;

namespace oostfraeiskorg;

public static class Server
{
    public static string MapPath(string path)
    {
        return Path.Combine(AppDomain.CurrentDomain.GetData("ContentRootPath").ToString(), path);
    }
}


