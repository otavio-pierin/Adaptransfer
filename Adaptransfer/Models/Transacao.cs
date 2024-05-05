using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adaptransfer.Models
{
   
    public class Transacao
    {
        //Fazendo a conexão com o banco de dados
        const string stringConexao = "Server=ESN509VMYSQL;Database=db_tccadaptransfer;User=aluno;Password=Senai1234";
        static MySqlConnection conecta = new MySqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
        static MySqlCommand query; //Variavel que faz os comandos

        //Atributos
        private string nome, cpfPCD, tipo;
        private double valor;
        private int idUsuario;

        //Construtor vazio
        public Transacao() {
        }

        //Construtor geral 
        public Transacao(string nome, string cpfPCD, int idUsuario, double valor, string tipo) {
            this.nome = nome;
            this.cpfPCD = cpfPCD;
            this.idUsuario = idUsuario;
            this.valor = valor;
            this.tipo = tipo;
        }

        //Construtor para o List
        public Transacao(string nome, int idUsuario, double valor, string tipo) {
            this.nome = nome;
            this.idUsuario = idUsuario;
            this.valor = valor;
            this.tipo = tipo;
        }

        //Construtor para o BuscaId
        public Transacao(string cpfPCD) {
            this.cpfPCD = cpfPCD;
        }

        //Construtor para o método de Entradas e Saidas
        public Transacao(double valor) {
            this.valor = valor;
        }


        //Get e set dos atributos
        public string Nome { get => nome; set => nome = value; }
        public string CpfPCD { get => cpfPCD; set => cpfPCD = value; }
        public int IDusuario { get => idUsuario; set => idUsuario = value; }
        public double Valor { get => valor; set => valor = value; }
        public string Tipo { get => tipo; set => tipo = value; }


        //Método de busca do id 
        public static string BuscaId(string cpfPCD) {
            string id = "";
            try {
                conecta.Open();
                query = new MySqlCommand("SELECT id_conta from tb_conta where FK_USUARIO_cpf_usuario = @cpfPCD", conecta);
                query.Parameters.AddWithValue("@cpfPCD", cpfPCD);
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) {
                    if (leitor["id_conta"].ToString() != "") {
                        id = leitor["id_conta"].ToString();
                    }
                }
            } catch (Exception) {

                throw;
            } finally {
                conecta.Close();
            }

            return id;
        }

        //método de listar todas as transações do usuário com base no id
        public static List<Transacao> Listar(string id) {
            List<Transacao> transacoes = new List<Transacao>();
            try {
                conecta.Open();
                query = new MySqlCommand("SELECT * from tb_transacao where FK_CONTA_id_conta = @id", conecta);
                query.Parameters.AddWithValue("@id", id);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) {
                 Transacao transacao = new Transacao(leitor["nome"].ToString(),int.Parse(leitor["FK_CONTA_id_conta"].ToString()), double.Parse(leitor["valor"].ToString()), leitor["tipo"].ToString());
                 transacoes.Add(transacao);
                }

            } catch (Exception) {

                throw;
            } finally {
                conecta.Close();
            }

            return transacoes;
        }

        //Método que lista todas os valores de Entradas 
        public static string Entradas(string id) {
            string valorEntradas ="";
            try {
                conecta.Open();
                query = new MySqlCommand("SELECT sum(valor) from tb_transacao where FK_CONTA_id_conta = @id and tipo = 2", conecta);
                //query.Parameters.AddWithValue("@valor", valor);
                query.Parameters.AddWithValue("@id", id);
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) {
                    if (leitor["sum(valor)"].ToString() != "") {
                        valorEntradas = leitor["sum(valor)"].ToString();
                    }
                }
            } catch (Exception) {

                throw;
            } finally {
                conecta.Close();
            }

            return valorEntradas;
        }

        //Método que lista todas os valores de Entradas 
        public static string Saidas(string id) {
            string valorSaidas = "";
            try {
                conecta.Open();
                query = new MySqlCommand("SELECT sum(valor) from tb_transacao where FK_CONTA_id_conta = @id and tipo = '1'", conecta);
                //query.Parameters.AddWithValue("@valor", valor);
                query.Parameters.AddWithValue("@id", id);
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) {
                    if (leitor["sum(valor)"].ToString() != "") {
                        valorSaidas = leitor["sum(valor)"].ToString();
                    }
                }
            } catch (Exception) {

                throw;
            } finally {
                conecta.Close();
            }

            return valorSaidas;
        }

    }
}
