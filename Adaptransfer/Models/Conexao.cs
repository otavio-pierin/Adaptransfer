using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Adaptransfer.Models
{
    public class Conexao
    {
        public static MySqlConnection getConexao() {
            return new MySqlConnection(
            Configuration().GetConnectionString("Server=ESN509VMYSQL;Database=db_tccadaptransfer;User=aluno;Password=Senai1234"));
        }

        private static IConfigurationRoot Configuration() {
            IConfigurationBuilder builder =
            new ConfigurationBuilder().SetBasePath(
            Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
