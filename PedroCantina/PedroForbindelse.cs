using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace PedroCantina
{
    public class PedroForbindelse
    {
        SqlConnection _forbind;
        public SqlConnection Forbindelse
        {
            get { return _forbind; }
            set { _forbind = value; }
        }
        public PedroForbindelse()
        {
            _forbind = new SqlConnection();
        }

        public void OpretForbindelse()
        {
            // Build configuration from user secrets
            var configuration = new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
        .Build();

            var connectionString = configuration["Pcstring"];
            _forbind = new SqlConnection(connectionString);
            _forbind.Open();
        }

        public void LukForbindelse()
        {


            _forbind.Close();

        }


    }
}
