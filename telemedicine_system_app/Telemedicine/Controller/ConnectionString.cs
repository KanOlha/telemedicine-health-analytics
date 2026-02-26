using Npgsql;
using System.Configuration;
using System.Data;

namespace Telemedicine.Controller
{
    internal class ConnectionString
    {
        public static string ConnStr => ConfigurationManager.ConnectionStrings["TelemedicineDATA.Postgres"].ConnectionString;
        
    }
}
