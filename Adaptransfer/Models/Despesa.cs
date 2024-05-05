using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adaptransfer.Models
{
    public class Despesa
    {
        //Fazendo a conexão com o banco de dados
        const string stringConexao = "Server=ESN509VMYSQL;Database=db_tccadaptransfer;User=aluno;Password=Senai1234";
        MySqlConnection conecta = new MySqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
        MySqlCommand query; //Variavel que faz os comandos
    
        //Atributos
        private string categoria, nomeInstituicao;
        private double valor;
        private DateTime datavenc;
        private int idConta;

        //Construtor vazio
        public Despesa()
        {
        }

        //Construtor geral
        public Despesa(int idConta, string nomeInstituicao, double valor, string categoria, DateTime datavenc)
        {
            this.idConta = idConta;
            this.datavenc = datavenc;
            this.categoria = categoria;
            this.nomeInstituicao = nomeInstituicao;
            this.valor = valor;
        }

        //Método de GET e SET de cada dado 
        public int IdConta { get => idConta; set => idConta = value; }
        public string NomeInstituicao { get => nomeInstituicao; set => nomeInstituicao = value; }
        public double Valor { get => valor; set => valor = value; }
        public string Categoria { get => categoria; set => categoria = value; }
        public DateTime Datavenc { get => datavenc; set => datavenc = value; }


        //Metodo que cadastra o despesa
        public string CadastroDespesa()
        {
                    try
                    {
                        conecta.Open();
                        query = new MySqlCommand("INSERT into tb_despesa(FK_CONTA_id_conta,nome,valor,categoria,data_vencimento)" +
                        "VALUES (@FK_CONTA_id_conta, @nome, @valor,@categoria,@data_vencimento)", conecta); //Define o comando SQL
                        //Parameters para evitar SQLInjection
                        query.Parameters.AddWithValue("@FK_CONTA_id_conta", idConta);
                        query.Parameters.AddWithValue("@nome", nomeInstituicao);
                        query.Parameters.AddWithValue("@valor", valor);
                        query.Parameters.AddWithValue("@categoria", categoria);
                        query.Parameters.AddWithValue("@data_vencimento", datavenc);

                        query.ExecuteNonQuery(); //Executa o comando
                        return "Despesa cadastrada com sucesso";
                    }
                    catch (MySqlException f) //Em caso de erro, retorna o erro
                    {
                        return "Erro" + f.ToString();
                    }
                    finally
                    {
                        conecta.Close(); //Fecha a conexão independente do caso
                    }

        }
    }
}
